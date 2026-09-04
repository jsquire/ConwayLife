using Squire.ConwayLife.Infrastructure;

namespace Squire.ConwayLife;

/// <summary>
///   The contract to be satisfied by a strategy implementation for
///   simulating Conway's Game of Life.
/// </summary>
///
internal abstract class StrategyBase
{
    /// <summary>
    ///   The <see cref="Strategy" /> represented by this implementation.
    /// </summary>
    ///
    public abstract Strategy Strategy { get; }

    /// <summary>
    ///   Simulates the specified number of generations.
    /// </summary>
    ///
    /// <param name="cells">The initial set of live cells.</param>
    /// <param name="generations">The number of generations to simulate.</param>
    ///
    /// <returns>The set of live cells after the specified number of generations.</returns>
    ///
    public abstract IReadOnlyCollection<Coordinate> Simulate(IReadOnlyCollection<Coordinate> cells,
                                                             int generations);
}
