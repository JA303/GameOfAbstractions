using System.Text;

namespace GameOfAbstractions;

internal sealed class ConsoleRenderer(TextWriter output, bool interactive, ArrowRadar radar)
{
    private bool useTerminal = interactive;

    public void Draw(Game game, IReadOnlyList<string> messages)
    {
        var frame = new StringBuilder();
        foreach (var row in Viewport.Build(game))
            frame.AppendJoin(' ', row).AppendLine();

        var p = game.Player.Position;
        var s = game.Seed.Position;
        frame.AppendLine($"position ({p.X}, {p.Y})");
        frame.Append("arrows   [ ");
        var arrows = radar.Read(p, game.Enemies);

        if (useTerminal)
        {
            try { Console.Clear(); }
            catch (IOException) { useTerminal = false; } // Some IDE consoles have no usable screen buffer.
        }
        output.Write(frame.ToString());
        foreach (var arrow in arrows)
        {
            if (useTerminal) Console.ForegroundColor = arrow.Color switch
            {
                Signal.Red => ConsoleColor.Red,
                Signal.Yellow => ConsoleColor.Yellow,
                Signal.Green => ConsoleColor.Green,
                _ => throw new ArgumentOutOfRangeException(nameof(arrow.Color))
            };
            output.Write(arrow.Glyph);
            if (useTerminal) Console.ResetColor();
            output.Write(' ');
        }
        output.Write(']');
        // Preserve the meaning of color when piping output or running without a terminal.
        if (!useTerminal) output.Write("  " + string.Join(' ', arrows.Select(a => a.Color)));
        output.WriteLine();
        output.WriteLine($"seed     dx {(long)s.X - p.X}   dy {(long)s.Y - p.Y}");
        foreach (var message in messages) output.WriteLine(message);
    }
}
