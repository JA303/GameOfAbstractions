namespace GameOfAbstractions;

internal sealed class Ghost : IEnemyKind
{
    public char Glyph => 'G';
    public Step ChooseStep(Position here, Position player) =>
        new(player.X.CompareTo(here.X), player.Y.CompareTo(here.Y));
}
