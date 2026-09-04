namespace Squire.ConwayLife.Infrastructure;

/// <summary>
///   Represents a coordinate in the 64-bit signed integer space.
/// </summary>
///
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
///
public readonly record struct Coordinate(long X, long Y);
