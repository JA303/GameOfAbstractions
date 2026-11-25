namespace GameOfAbstractions;

internal static class GameSetup
{
    public static Game Create(Random random, Action<string> log)
    {
        var map = new WorldMap();
        var seed = new Seed(new(1, 1));
        seed.Eaten += player => seed.Relocate(NextSeedCell(random, player, map));
        seed.Eaten += cell => log($"Seed eaten at ({cell.X}, {cell.Y}).");

        return new Game(map, new Player(new(0, 0)), seed,
        [
            new(new(0, 6), new Chaser()),
            new(new(8, 6), new Chaser()),
            new(new(0, 14), new Wanderer()),
            new(new(16, 16), new Ghost()),
        ]);
    }

    private static Position NextSeedCell(Random random, Position player, WorldMap map)
    {
        Position cell;
        do { cell = new(random.Next(-15, 16), random.Next(-15, 16)); }
        while (cell == player || !map.IsOpen(cell));
        return cell;
    }
}
