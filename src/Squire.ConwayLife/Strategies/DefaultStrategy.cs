using Squire.ConwayLife.Infrastructure;

namespace Squire.ConwayLife.Strategies;

/// <summary>
///   The default strategy for simulating Conway's Game of Life.
/// </summary>
///
/// <seealso cref="Squire.ConwayLife.StrategyBase" />
///
internal class DefaultStrategy : StrategyBase
{
    /// <summary>
    ///   The <see cref="Squire.ConwayLife.Strategy" /> represented by this implementation.
    /// </summary>
    ///
    public override Strategy Strategy => Strategy.Default;

    /// <summary>
    ///   Simulates the specified number of generations.
    /// </summary>
    ///
    /// <param name="cells">The initial set of live cells.</param>
    /// <param name="generations">The number of generations to simulate.</param>
    ///
    /// <returns>The set of live cells after the specified number of generations.</returns>
    ///
    public override IReadOnlyCollection<Coordinate> Simulate(IReadOnlyCollection<Coordinate> cells,
                                                             int generations)
    {
        // TODO: Implement the simulation.
        throw new NotImplementedException();
    }
}
