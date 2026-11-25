namespace GameOfAbstractions;

internal interface IMapEntity
{
    Position Position { get; }
    char Glyph { get; }
}

internal sealed class Player(Position start) : IMapEntity
{
    public Position Position { get; private set; } = start;
    public char Glyph => '@';

    public void Move(Direction direction, WorldMap map)
    {
        Step step = direction switch
        {
            Direction.Up    => new(0, 1),
            Direction.Down  => new(0, -1),
            Direction.Left  => new(-1, 0),
            Direction.Right => new(1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
        Position = map.Move(Position, step);
    }
}

internal sealed class Seed(Position start) : IMapEntity
{
    public Position Position { get; private set; } = start;
    public char Glyph => '*';
    public event Action<Position>? Eaten;

    public void TryEatAt(Position player)
    {
        if (player == Position) Eaten?.Invoke(player);
    }

    public void Relocate(Position destination) => Position = destination;
}
