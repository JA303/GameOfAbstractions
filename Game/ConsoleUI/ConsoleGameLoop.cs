namespace GameOfAbstractions;

internal static class ConsoleGameLoop
{
    public static void Run(TextReader input, TextWriter output, bool interactive,
        Random random, ArrowRadar radar)
    {
        var messages = new List<string>();
        var game = GameSetup.Create(random, messages.Add);
        var renderer = new ConsoleRenderer(output, interactive, radar);
        renderer.Draw(game, messages);

        while (true)
        {
            output.WriteLine("Move: 8 up | 2 down | 4 left | 6 right, then Enter");
            string? line = input.ReadLine();
            if (line is null) return;
            Direction? move = line switch
            {
                "8" => Direction.Up, "2" => Direction.Down,
                "4" => Direction.Left, "6" => Direction.Right,
                _ => null
            };
            if (move is null)
            {
                output.WriteLine("Invalid input. Enter exactly 8, 2, 4 or 6.");
                continue;
            }

            messages.Clear();
            game.TakeTurn(move.Value);
            if (game.IsOver)
            {
                foreach (var message in messages) output.WriteLine(message);
                output.WriteLine("Game over: an enemy reached you.");
                return;
            }
            renderer.Draw(game, messages);
        }
    }
}
