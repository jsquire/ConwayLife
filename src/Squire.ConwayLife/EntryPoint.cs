using System.CommandLine;
using Squire.ConwayLife.Infrastructure;
using Squire.ConwayLife.Strategies;

namespace Squire.ConwayLife;

/// <summary>
///   Hosts the entry point for the application along with any supporting infrastructure.
/// </summary>
///
public static class EntryPoint
{
    /// <summary>
    ///   Simulates Conway's Game of Life in 64-bit signed integer space.
    /// </summary>
    ///
    /// <param name="args">The command-line arguments.</param>
    ///
    /// <example>
    ///   <c>Squire.ConwayLife</c>
    /// </example>
    ///
    /// <example>
    ///   <c>Squire.ConwayLife --file glider.lif --generations 10 --strategy Default</c>
    /// </example>
    ///
    /// <example>
    ///   <c>Squire.ConwayLife --generations 5</c>
    /// </example>
    ///
    /// <seealso href="https://github.com/jsquire/Interview-Challenges/blob/main/challenges/conway-life/README.md" />
    ///
    public static void Main(string[] args)
    {
        var rootCommand = new RootCommand("Conway's Game of Life in 64-bit signed integer space.");

        var fileOption = new Option<string?>("--file")
        {
            Description = "Path to a Life 1.06 file containing the initial population."
        };

        var generationsOption = new Option<int>("--generations")
        {
            Description = "Number of generations to simulate.",
            DefaultValueFactory = _ => 10
        };

        var strategyOption = new Option<Strategy>("--strategy")
        {
            Description = "The strategy to use for simulating the generations.",
            Required = true,
            DefaultValueFactory = _ => Strategy.Default
        };

        rootCommand.Options.Add(fileOption);
        rootCommand.Options.Add(generationsOption);
        rootCommand.Options.Add(strategyOption);

        rootCommand.SetAction(parseResult =>
        {
            var file = parseResult.GetValue(fileOption);
            var generations = parseResult.GetValue(generationsOption);
            var strategy = parseResult.GetValue(strategyOption);

            Execute(file, generations, strategy);
        });

        rootCommand
            .Parse(args)
            .Invoke();
    }

    /// <summary>
    ///   Reads the initial population, runs the simulation, and writes the result.
    /// </summary>
    ///
    /// <param name="file">Optional path to a Life 1.06 file.</param>
    /// <param name="generations">Number of generations to simulate.</param>
    /// <param name="strategy">The <see cref="Strategy" /> to use for simulating the generations.</param>
    ///
    internal static void Execute(string? file,
                                 int generations,
                                 Strategy strategy)
    {
        if (file is not null)
        {
            if (!Path.Exists(file))
            {
                throw new FileNotFoundException($"The file '{file}' does not exist.");
            }
        }

        var initialPopulation = file is not null
            ? ParseFileContents(File.ReadAllLines(file))
            : ReadFromConsole();

        var result = CreateStrategy(strategy).Simulate(initialPopulation, generations);

        WriteToConsole(result);
    }

    /// <summary>
    ///   Creates a <see cref="StrategyBase" /> which applies the given <paramref name="strategy" />
    ///   to simulate Conway's Game of Life.
    /// </summary>
    ///
    /// <param name="strategy">The strategy to create an implementation for.</param>
    ///
    /// <returns>The <see cref="StrategyBase" /> for the requested <paramref name="strategy" />.</returns>
    ///
    internal static StrategyBase CreateStrategy(Strategy strategy) => strategy switch
    {
        Strategy.Default => new DefaultStrategy(),
        _ => throw new ArgumentException($"Unknown strategy: `{ strategy }`.", nameof(strategy))
    };

    /// <summary>
    ///   Reads coordinates from standard input until a blank line is encountered.
    /// </summary>
    ///
    /// <returns>The set of live cell coordinates.</returns>
    ///
    internal static HashSet<Coordinate> ReadFromConsole()
    {
        Console.WriteLine("Enter live cell coordinates (x y), one per line.");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  1 1");
        Console.WriteLine("  2 1");
        Console.WriteLine("  -3 1");
        Console.WriteLine("  -21003 -1");
        Console.WriteLine();
        Console.WriteLine("Enter a blank line when finished.");
        Console.WriteLine();

        var cells = new HashSet<Coordinate>();
        while (true)
        {
            var line = Console.ReadLine();
            var lineSpan = line.AsSpan();

            // A line containing only whitespace is considered a blank line and terminates input.

            if ((lineSpan.IsEmpty) || (lineSpan.IsWhiteSpace()))
            {
                break;
            }

            // Comments are allowed in Life 1.06 format and should be ignored.

            if (lineSpan.StartsWith('#'))
            {
                continue;
            }

            if (TryParseCoordinate(lineSpan, out var coordinate))
            {
                cells.Add(coordinate);
            }
            else
            {
                Console.Error.WriteLine($"Invalid coordinate: {line}");
            }
        }

        return cells;
    }

    /// <summary>
    ///   Reads coordinates from a Life 1.06 file.
    /// </summary>
    ///
    /// <param name="fileContents">The contents of the file.</param>
    ///
    /// <returns>The set of live cell coordinates.</returns>
    ///
    internal static HashSet<Coordinate> ParseFileContents(IEnumerable<string> fileContents)
    {
        var cells = new HashSet<Coordinate>();

        foreach (var line in fileContents)
        {
            var lineSpan = line.AsSpan();

            // Blank lines and comments are allowed in Life 1.06 format and should be ignored.

            if ((lineSpan.IsEmpty)
                || (lineSpan.IsWhiteSpace())
                || (lineSpan.StartsWith('#')))
            {
                continue;
            }

            if (TryParseCoordinate(lineSpan, out var coordinate))
            {
                cells.Add(coordinate);
            }
        }

        return cells;
    }

    /// <summary>
    ///   Writes the population to standard output in Life 1.06 format.
    /// </summary>
    ///
    /// <param name="cells">The collection of live cell coordinates.</param>
    ///
    internal static void WriteToConsole(IEnumerable<Coordinate> cells)
    {
        Console.WriteLine("#Life 1.06");

        foreach (var cell in cells)
        {
            Console.WriteLine($"{cell.X} {cell.Y}");
        }
    }

    /// <summary>
    ///   Attempts to parse a coordinate pair from a line of text.
    /// </summary>
    ///
    /// <param name="line">The line to parse.</param>
    /// <param name="coordinate">The parsed coordinate, if successful.</param>
    ///
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    ///
    internal static bool TryParseCoordinate(ReadOnlySpan<char> line,
                                            out Coordinate coordinate)
    {
        coordinate = default;

        var separatorIndex = line.IndexOf(' ');

        // If there is no space after the first character or the only space comes
        // as the last character, then the line is not a valid coordinate pair.

        if ((separatorIndex <= 0) || ((separatorIndex + 1) >= line.Length))
        {
            return false;
        }

        // Slice the span on the embedded string to separate the x and y
        // values.  Since TryParse ignores leading and trailing whitespace, there
        // is no need to trim the slices.

        if ((long.TryParse(line[..separatorIndex], out var x))
            && (long.TryParse(line[(separatorIndex + 1)..], out var y)))
        {
            coordinate = new Coordinate(x, y);
            return true;
        }

        return false;
    }
}
