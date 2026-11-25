using GameOfAbstractions;
using static GameOfAbstractions.Direction;

(string Name, Action Check)[] checks =
[
    ("Acceptance 1: register, move and draw a fourth kind without engine changes", FourthKind),
    ("Acceptance 2: swap one radar argument and reproduce both initial palettes", InitialPalettes),
    ("Acceptance 3: attach a third reaction; relocate the same seed and log once", SeedReactions),
    ("Acceptance 4: a bluff cannot suppress capture or reorder the turn", FixedOrder),
    ("Setup and enemy movements match the brief", Movement),
    ("Radar handles boundaries, diagonal ties and all-red directions", RadarEdges),
    ("Collision uses Manhattan, not the presentation metric", CollisionMetric),
    ("The viewport is centered, clips entities and resolves overlap", Camera),
    ("Bad input does not advance time; a terminal turn does not draw a HUD", ConsoleContract),
    ("The seed log survives the redraw and EOF exits normally", SeedOutput),
];

int failed = 0;
foreach (var (name, check) in checks)
{
    try { check(); Console.WriteLine($"PASS  {name}"); }
    catch (Exception ex) { failed++; Console.Error.WriteLine($"FAIL  {name}\n{ex}"); }
}
Console.WriteLine($"{checks.Length - failed}/{checks.Length} checks passed.");
return failed == 0 ? 0 : 1;

static Game DefaultGame() => GameSetup.Create(new Random(7), _ => { });
static Game Scenario(params Enemy[] enemies) => new(new WorldMap(), new Player(new(0, 0)),
    new Seed(new(15, 15)), enemies);
static string Palette(Game g, Func<Position, Position, long> metric) => string.Join(' ',
    new ArrowRadar(metric).Read(g.Player.Position, g.Enemies).Select(a => a.Color));

static void FourthKind()
{
    var original = DefaultGame();
    var game = new Game(original.Map, original.Player, original.Seed,
        [.. original.Enemies, new(new(5, 3), new Bluffer())]);
    game.TakeTurn(Right);
    Equal(5, game.Enemies.Count);
    Equal(new Position(4, 2), game.Enemies[^1].Position);
    Equal('B', Viewport.Build(game)[8][13]);
}

static void InitialPalettes()
{
    var g = DefaultGame();
    Equal("Red Green Green Yellow", Palette(g, Distance.Manhattan));
    Equal("Red Yellow Yellow Red", Palette(g, Distance.Chebyshev));
}

static void SeedReactions()
{
    var draws = new ScriptedRandom(1, 1, -15, 15);
    var logs = new List<string>();
    var g = GameSetup.Create(draws, logs.Add);
    var seed = g.Seed;
    var seen = new List<(Position Eaten, Position Respawn, Position EnemyBeforeMove)>();
    seed.Eaten += p => seen.Add((p, seed.Position, g.Enemies[0].Position));
    g.TakeTurn(Right);
    var before = g.Enemies[0].Position;
    g.TakeTurn(Up);
    Equal(true, ReferenceEquals(seed, g.Seed));
    Equal("Seed eaten at (1, 1).", logs.Single());
    Equal((new Position(1, 1), new Position(-15, 15), before), seen.Single());
    Equal(new Position(1, 5), g.Enemies[0].Position);
    Equal(4, draws.Calls);
}

static void FixedOrder()
{
    var liar = new Bluffer();
    var g = Scenario(new(new(3, 1), liar), new(new(10, 10), new Ghost()));
    g.TakeTurn(Right);
    Equal(false, liar.HasCaughtPlayer(g.Player.Position));
    Equal(1L, Distance.Manhattan(g.Enemies[0].Position, g.Player.Position));
    Equal(new Position(9, 9), g.Enemies[1].Position);
    Equal(true, g.IsOver);
    Throws<InvalidOperationException>(() => g.TakeTurn(Left));

    var onContact = Scenario(new Enemy(new(1, 0), liar));
    onContact.TakeTurn(Right);
    Equal(true, onContact.IsOver);
    Throws<ArgumentOutOfRangeException>(() => new Step(-2, 0));
}

static void Movement()
{
    var g = DefaultGame();
    Equal(new Position(0, 0), g.Player.Position);
    Equal(new Position(1, 1), g.Seed.Position);
    Equal(true, g.Enemies.Select(e => e.Position).SequenceEqual(
        new Position[] { new(0, 6), new(8, 6), new(0, 14), new(16, 16) }));
    g.TakeTurn(Up);
    Equal(true, g.Enemies.Select(e => e.Position).SequenceEqual(
        new Position[] { new(0, 5), new(7, 6), new(1, 14), new(15, 15) }));
    var wanderer = new Enemy(new(-41, 23), new Wanderer());
    foreach (var target in new Position[] { new(0, 9), new(44, 0), new(1, 1), new(4, 3) })
        wanderer.Advance(target, g.Map);
    Equal(new Position(-41, 23), wanderer.Position);
}

static void RadarEdges()
{
    var diagonal = Scenario(new Enemy(new(-5, -5), new Ghost()));
    Equal("Green Yellow Yellow Green", Palette(diagonal, Distance.Manhattan));
    var red = Scenario(new(new(-4, -5), new Ghost()), new(new(4, 5), new Ghost()));
    Equal("Red Red Red Red", Palette(red, Distance.Manhattan));
    Equal(8589934590L, Distance.Manhattan(new(int.MinValue, int.MinValue), new(int.MaxValue, int.MaxValue)));
}

static void CollisionMetric()
{
    var g = Scenario(new Enemy(new(3, 2), new Bluffer()));
    g.TakeTurn(Right);
    Equal(1L, Distance.Chebyshev(g.Player.Position, g.Enemies[0].Position));
    Equal(2L, Distance.Manhattan(g.Player.Position, g.Enemies[0].Position));
    Equal(false, g.IsOver);
}

static void Camera()
{
    var g = new Game(new WorldMap(), new Player(new(-30, -50)), new Seed(new(-30, -49)),
        [new(new(-30, -49), new Chaser()), new(new(-20, -60), new Ghost()), new(new(-41, -50), new Wanderer())]);
    var cells = Viewport.Build(g);
    Equal(21, cells.Length);
    Equal(true, cells.All(row => row.Length == 21));
    Equal('@', cells[10][10]);
    Equal('C', cells[9][10]);
    Equal('G', cells[20][20]);
    Equal(false, cells.Any(row => row.Contains('W')));
    Equal(Signal.Yellow, new ArrowRadar(Distance.Manhattan).Read(g.Player.Position, g.Enemies)[2].Color);
    g.Seed.Relocate(g.Player.Position);
    Equal('@', Viewport.Build(g)[10][10]);
    var empty = Scenario();
    for (int i = 0; i < 25; i++) empty.TakeTurn(Left);
    Equal(new Position(-25, 0), empty.Player.Position);
    Equal('@', Viewport.Build(empty)[10][10]);
}

static string Session(string input)
{
    using var output = new StringWriter();
    ConsoleGameLoop.Run(new StringReader(input), output, false, new Random(7), new ArrowRadar(Distance.Manhattan));
    return output.ToString();
}

static void ConsoleContract()
{
    string text = Session("bad\n\n88\n 8\n8\n8\n8\n2\n");
    Equal(4, text.Split("Invalid input.").Length - 1);
    Equal(3, text.Split("position (").Length - 1);
    Equal(true, text.Contains("position (0, 2)"));
    Equal(true, text.TrimEnd().EndsWith("Game over: an enemy reached you."));
    Equal(true, Session("2\n4\n").Contains("position (-1, -1)"));
}

static void SeedOutput()
{
    string text = Session("6\n8\n");
    int lastFrame = text.LastIndexOf("position (1, 1)", StringComparison.Ordinal);
    Equal(true, lastFrame >= 0 && text.IndexOf("Seed eaten at (1, 1).", StringComparison.Ordinal) > lastFrame);
    Equal(false, text.Contains("Game over"));
    Equal(true, text.Contains("seed     dx 0   dy 1"));
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"Expected {expected}; got {actual}.");
}

static void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}.");
}

internal sealed class ScriptedRandom(params int[] values) : Random
{
    public int Calls { get; private set; }
    public override int Next(int minValue, int maxValue)
    {
        if ((minValue, maxValue) != (-15, 16)) throw new InvalidOperationException("Unexpected spawn bounds.");
        return values[Calls++];
    }
}
