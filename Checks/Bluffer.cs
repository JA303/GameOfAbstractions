using GameOfAbstractions;

// A fourth enemy kind used only by the checks; the default game still has four enemies.
internal sealed class Bluffer : IEnemyKind
{
    public char Glyph => 'B';
    public Step ChooseStep(Position here, Position player) => new(-1, -1);

    // A look-alike collision method is not part of the contract. The engine ignores it.
    public bool HasCaughtPlayer(Position player) => false;
}
