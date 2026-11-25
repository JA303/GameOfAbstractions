namespace GameOfAbstractions;

internal static class Viewport
{
    public const int Radius = 10;
    public const int Size = 2 * Radius + 1;

    public static char[][] Build(Game game)
    {
        var center = game.Player.Position;
        var cells = new char[Size][];
        for (int row = 0; row < Size; row++)
        {
            cells[row] = new char[Size];
            for (int col = 0; col < Size; col++)
            {
                var world = new Position(center.X + (col - Radius), center.Y + (Radius - row));
                cells[row][col] = game.Map.IsOpen(world) ? '.' : '#';
            }
        }

        // Paint from back to front. Sharing a cell never deletes the hidden entity.
        Paint(game.Seed);
        foreach (var enemy in game.Enemies) Paint(enemy);
        Paint(game.Player);
        return cells;

        void Paint(IMapEntity entity)
        {
            long col = Radius + ((long)entity.Position.X - center.X);
            long row = Radius - ((long)entity.Position.Y - center.Y);
            if (col is >= 0 and < Size && row is >= 0 and < Size)
                cells[(int)row][(int)col] = entity.Glyph;
        }
    }
}
