namespace GameOfAbstractions;

internal readonly record struct Position(int X, int Y);

// A strategy can request one adjacent cell (including diagonals), or stay still.
internal readonly record struct Step
{
    public int X { get; }
    public int Y { get; }

    public Step(int x, int y)
    {
        if (x is < -1 or > 1) throw new ArgumentOutOfRangeException(nameof(x));
        if (y is < -1 or > 1) throw new ArgumentOutOfRangeException(nameof(y));
        (X, Y) = (x, y);
    }
}

internal enum Direction { Up, Down, Left, Right }

internal sealed class WorldMap
{
    // Terrain is implicit: no grid is allocated and every cell is open for now.
    public bool IsOpen(Position cell) => true;

    public Position Move(Position from, Step step)
    {
        var target = new Position(checked(from.X + step.X), checked(from.Y + step.Y));
        return IsOpen(target) ? target : from;
    }
}
