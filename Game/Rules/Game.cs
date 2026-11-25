namespace GameOfAbstractions;

internal sealed class Game(WorldMap map, Player player, Seed seed, IEnumerable<Enemy> enemies)
{
    public WorldMap Map { get; } = map;
    public Player Player { get; } = player;
    public Seed Seed { get; } = seed;
    public IReadOnlyList<Enemy> Enemies { get; } = Array.AsReadOnly(enemies.ToArray());
    public bool IsOver { get; private set; }

    // Only this method owns turn order. Movement strategies cannot replace any phase.
    public void TakeTurn(Direction direction)
    {
        if (IsOver) throw new InvalidOperationException("The game has already ended.");
        Player.Move(direction, Map);
        Seed.TryEatAt(Player.Position);
        foreach (var enemy in Enemies) enemy.Advance(Player.Position, Map);
        IsOver = Enemies.Any(e => Distance.Manhattan(e.Position, Player.Position) <= 1);
    }
}
