using NUnit.Framework;
using Squire.ConwayLife.Infrastructure;

namespace Squire.ConwayLife.Tests;

/// <summary>
///   The suite of tests for the <see cref="EntryPoint" /> class.
/// </summary>
///
/// <remarks>
///   Because the <see cref="EntryPoint" /> interacts with the <see cref="Console" />
///   and must mock the static instance, these tests cannot be run in parallel.
/// </remarks>
///
[TestFixture]
[NonParallelizable]
public class EntryPointTests
{
    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.TryParseCoordinate" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("1 2", 1, 2)]
    [TestCase("-100 20", -100, 20)]
    [TestCase("0 0", 0, 0)]
    [TestCase("1  2", 1, 2)]
    [TestCase("9223372036854775807 -9223372036854775808", long.MaxValue, long.MinValue)]
    public void TryParseCoordinateAcceptsValidCoordinates(string value,
                                                          long expectedX,
                                                          long expectedY)
    {
        var result = EntryPoint.TryParseCoordinate(value.AsSpan(), out var coordinate);

        Assert.That(result, Is.True, $"The coordinate pair `{ value }` should be valid.");
        Assert.That(coordinate, Is.EqualTo(new Coordinate(expectedX, expectedY)), "The parsed coordinate was incorrect.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.TryParseCoordinate" /> method.
    /// </summary>
    ///
    [Test]
    [TestCase("")]
    [TestCase("1")]
    [TestCase(" 1 2")]
    [TestCase("1 ")]
    [TestCase("1 2 3")]
    [TestCase("a b")]
    [TestCase("1.5 2")]
    public void TryParseCoordinateRejectsInvalidCoordinates(string value)
    {
        var result = EntryPoint.TryParseCoordinate(value.AsSpan(), out _);
        Assert.That(result, Is.False, $"The coordinate pair `{ value }` should be invalid.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsReturnsCoordinates()
    {
        var expected = new Coordinate[]
        {
            new(1, 2),
            new(3, 4)
        };

        var input = expected.Select(item => $"{item.X} {item.Y}");

        var result = EntryPoint.ParseFileContents(input);
        Assert.That(result, Is.EquivalentTo(expected), "All coordinates in the file should be returned.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsIgnoresComments()
    {
        var result = EntryPoint.ParseFileContents(["#Life 1.06", "# A comment", "1 2"]);
        Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Comments should not produce coordinates.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsIgnoresBlankLines()
    {
        var expected = new Coordinate[]
        {
            new(1, 2),
            new(3, 4)
        };

        var result = EntryPoint.ParseFileContents(["", "1 2", "   ", "3 4"]);
        Assert.That(result, Is.EquivalentTo(expected), "Blank lines should not produce coordinates.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsCollapsesDuplicateCoordinates()
    {
        var result = EntryPoint.ParseFileContents(["1 2", "1 2"]);
        Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Duplicate coordinates should be collapsed.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsReturnsEmptyPopulationForEmptyInput()
    {
        var result = EntryPoint.ParseFileContents([]);
        Assert.That(result, Is.Empty, "An empty file should produce an empty population.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ParseFileContents" /> method.
    /// </summary>
    ///
    [Test]
    public void ParseFileContentsIgnoresNonParsableLines()
    {
        var result = EntryPoint.ParseFileContents(["invalid", "1 2"]);
        Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Non-parsable lines should not produce coordinates.");
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleReturnsCoordinatesEnteredBeforeBlankLine()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader($"1 2{Environment.NewLine}3 4{Environment.NewLine}{Environment.NewLine}5 6");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2), new Coordinate(3, 4) ]), "Input after the blank line should not be read.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleIgnoresComments()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader($"#Life 1.06{Environment.NewLine}1 2{Environment.NewLine}# A comment{Environment.NewLine}3 4{Environment.NewLine}{Environment.NewLine}");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2), new Coordinate(3, 4) ]), "Comments should not produce coordinates.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleReportsInvalidCoordinates()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            var input = new StringReader($"1 2{Environment.NewLine}invalid{Environment.NewLine}3 4{Environment.NewLine}{Environment.NewLine}");
            var output = new StringWriter();
            var error = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);
            Console.SetError(error);

            var result = EntryPoint.ReadFromConsole();

            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2), new Coordinate(3, 4) ]), "An invalid coordinate should not be added.");
            Assert.That(error.ToString(), Does.Contain("Invalid coordinate: invalid"), "The invalid coordinate should be reported.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleReturnsEmptyPopulationForBlankInput()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader(Environment.NewLine);
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.Empty, "A blank first line should produce an empty population.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.ReadFromConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void ReadFromConsoleCollapsesDuplicateCoordinates()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;

        try
        {
            var input = new StringReader($"1 2{Environment.NewLine}1 2{Environment.NewLine}{Environment.NewLine}");
            var output = new StringWriter();

            Console.SetIn(input);
            Console.SetOut(output);

            var result = EntryPoint.ReadFromConsole();
            Assert.That(result, Is.EquivalentTo([ new Coordinate(1, 2) ]), "Duplicate coordinates should be collapsed.");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.WriteToConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void WriteToConsoleWritesLifeFileContents()
    {
        var originalOut = Console.Out;

        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            EntryPoint.WriteToConsole([new Coordinate(1, 2), new Coordinate(3, 4)]);

            var expected = $"#Life 1.06{Environment.NewLine}1 2{Environment.NewLine}3 4{Environment.NewLine}";
            Assert.That(output.ToString(), Is.EqualTo(expected), "The population should be written in Life 1.06 format.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.WriteToConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void WriteToConsoleWritesHeaderForEmptyPopulation()
    {
        var originalOut = Console.Out;

        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            EntryPoint.WriteToConsole([]);
            Assert.That(output.ToString(), Is.EqualTo($"#Life 1.06{Environment.NewLine}"), "An empty population should write only the header.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///   Verifies functionality of the <see cref="EntryPoint.WriteToConsole" /> method.
    /// </summary>
    ///
    [Test]
    public void WriteToConsoleWritesNegativeCoordinates()
    {
        var originalOut = Console.Out;

        try
        {
            var output = new StringWriter();
            Console.SetOut(output);

            EntryPoint.WriteToConsole([new Coordinate(-5, 10)]);

            var expected = $"#Life 1.06{Environment.NewLine}-5 10{Environment.NewLine}";
            Assert.That(output.ToString(), Is.EqualTo(expected), "Negative coordinates should be written without modification.");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
