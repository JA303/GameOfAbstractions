namespace GameOfAbstractions;

// The only extension point for an enemy: no game, entity references or turn hooks.
internal interface IEnemyKind
{
    char Glyph { get; }
    Step ChooseStep(Position here, Position player);
}

internal sealed class Enemy(Position start, IEnemyKind kind) : IMapEntity
{
    public Position Position { get; private set; } = start;
    public char Glyph => kind.Glyph;

    public void Advance(Position player, WorldMap map)
    {
        // Contact is already made if the player steps onto this enemy. Do not let a
        // diagonal retreat erase that contact before the central collision phase.
        if (Position != player) Position = map.Move(Position, kind.ChooseStep(Position, player));
    }
}
