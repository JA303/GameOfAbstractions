namespace GameOfAbstractions;

internal static class Distance
{
    public static long Manhattan(Position a, Position b) =>
        Math.Abs((long)a.X - b.X) + Math.Abs((long)a.Y - b.Y);

    public static long Chebyshev(Position a, Position b) =>
        Math.Max(Math.Abs((long)a.X - b.X), Math.Abs((long)a.Y - b.Y));
}

internal enum Signal { Green, Yellow, Red }
internal readonly record struct Arrow(char Glyph, Signal Color);

internal sealed class ArrowRadar(Func<Position, Position, long> distance)
{
    private const long Infinity = long.MaxValue;
    private const string Symbols = "^v<>";

    public Arrow[] Read(Position player, IEnumerable<Enemy> enemies)
    {
        long[] nearest = [Infinity, Infinity, Infinity, Infinity];
        foreach (var enemy in enemies)
        {
            var p = enemy.Position;
            long d = distance(p, player);
            if (p.Y > player.Y) nearest[0] = Math.Min(nearest[0], d);
            if (p.Y < player.Y) nearest[1] = Math.Min(nearest[1], d);
            if (p.X < player.X) nearest[2] = Math.Min(nearest[2], d);
            if (p.X > player.X) nearest[3] = Math.Min(nearest[3], d);
        }

        // Empty directions must participate in ties for the lowest non-red distance.
        long yellow = nearest.Where(d => d >= 10).DefaultIfEmpty(Infinity).Min();
        return nearest.Select((d, i) => new Arrow(Symbols[i],
            d < 10 ? Signal.Red : d == yellow ? Signal.Yellow : Signal.Green)).ToArray();
    }
}
