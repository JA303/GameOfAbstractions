namespace GameOfAbstractions;

internal sealed class Wanderer : IEnemyKind
{
    private static readonly Step[] Route = [new(1, 0), new(0, -1), new(-1, 0), new(0, 1)];
    private int next;
    public char Glyph => 'W';

    public Step ChooseStep(Position here, Position player)
    {
        var step = Route[next];
        next = (next + 1) % Route.Length;
        return step;
    }
}
