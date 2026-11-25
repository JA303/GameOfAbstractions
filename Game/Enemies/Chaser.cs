namespace GameOfAbstractions;

internal sealed class Chaser : IEnemyKind
{
    public char Glyph => 'C';

    // Resolve horizontal separation first, then vertical separation.
    public Step ChooseStep(Position here, Position player) => here.X != player.X
        ? new(player.X.CompareTo(here.X), 0)
        : new(0, player.Y.CompareTo(here.Y));
}
