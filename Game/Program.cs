using GameOfAbstractions;

bool interactive = !Console.IsOutputRedirected && Environment.GetEnvironmentVariable("TERM") != "dumb";
ConsoleGameLoop.Run(Console.In, Console.Out, interactive, Random.Shared, new ArrowRadar(Distance.Manhattan));