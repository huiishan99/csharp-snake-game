using System;
using System.Collections.Generic;

namespace SnakeGame.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                RunAll();
                Console.WriteLine("All SnakeGame checks passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private static void RunAll()
        {
            StartsCenteredAndPlacesFoodOffSnake();
            MovesAndWrapsAcrossEdges();
            SolidWallsEndAtBoundary();
            IgnoresImmediateReverseDirection();
            BuffersCornerInputs();
            KeepsDirectionQueueBounded();
            DoesNotMoveWhilePaused();
            EatsFoodAndGrows();
            DetectsSelfCollision();
            WinsWhenFinalCellIsEaten();
            GeneratesObstaclesAwayFromSnakeAndFood();
            ObstaclesCauseGameOver();
            RegeneratesFoodWhenResizeFindsObstacleConflict();
            ClampsSpeedIntoSupportedRange();
            CalculatesFixedSpeedInterval();
            ProgressiveSpeedGetsFasterAndCaps();
            PresetsClampInvalidValues();
            ModePresetAppliesExpectedRules();
            BoardPresetHasPlayableDimensions();
            ChallengeSeedNormalizesAndHashes();
            ChallengeSeedMakesFoodDeterministic();
            LeaderboardKeepsTopFivePerMode();
        }

        private static void StartsCenteredAndPlacesFoodOffSnake()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.StartNew(5, 5);

            AssertEqual(GameStatus.Playing, game.Status, "new game status");
            AssertEqual(0, game.Score, "new game score");
            AssertEqual(new GridCell(2, 2), game.Snake[0], "new game snake head");
            AssertFalse(game.Food.Equals(game.Snake[0]), "food should not be placed on the snake");
        }

        private static void MovesAndWrapsAcrossEdges()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.StartNew(2, 2);
            game.QueueDirection(Direction.Right);
            game.Step();

            AssertEqual(new GridCell(0, 1), game.Snake[0], "snake should wrap at the right edge");
        }

        private static void SolidWallsEndAtBoundary()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.LoadStateForTesting(
                3,
                3,
                new[] { new GridCell(1, 0) },
                new GridCell(2, 2),
                Direction.Up,
                0,
                BoundaryMode.SolidWalls);

            game.Step();

            AssertEqual(GameStatus.GameOver, game.Status, "solid wall boundary status");
            AssertEqual(new GridCell(1, 0), game.Snake[0], "solid wall boundary head");
        }

        private static void IgnoresImmediateReverseDirection()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.StartNew(5, 5);
            game.QueueDirection(Direction.Up);
            game.Step();

            AssertEqual(Direction.Down, game.CurrentDirection, "reverse input should be ignored");
            AssertEqual(new GridCell(2, 3), game.Snake[0], "snake should continue downward");
        }

        private static void BuffersCornerInputs()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.LoadStateForTesting(
                5,
                5,
                new[] { new GridCell(2, 2) },
                new GridCell(0, 0),
                Direction.Down,
                0);

            game.QueueDirection(Direction.Right);
            game.QueueDirection(Direction.Up);

            game.Step();
            AssertEqual(Direction.Right, game.CurrentDirection, "first queued direction");
            AssertEqual(new GridCell(3, 2), game.Snake[0], "first queued head");

            game.Step();
            AssertEqual(Direction.Up, game.CurrentDirection, "second queued direction");
            AssertEqual(new GridCell(3, 1), game.Snake[0], "second queued head");
        }

        private static void KeepsDirectionQueueBounded()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.LoadStateForTesting(
                5,
                5,
                new[] { new GridCell(2, 2) },
                new GridCell(0, 0),
                Direction.Down,
                0);

            game.QueueDirection(Direction.Right);
            game.QueueDirection(Direction.Up);
            game.QueueDirection(Direction.Left);

            game.Step();
            AssertEqual(Direction.Right, game.CurrentDirection, "bounded queue first direction");

            game.Step();
            AssertEqual(Direction.Up, game.CurrentDirection, "bounded queue second direction");

            game.Step();
            AssertEqual(Direction.Up, game.CurrentDirection, "bounded queue should ignore third direction");
        }

        private static void DoesNotMoveWhilePaused()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.StartNew(5, 5);
            GridCell startingHead = game.Snake[0];
            game.TogglePause();
            game.Step();

            AssertEqual(GameStatus.Paused, game.Status, "paused status");
            AssertEqual(startingHead, game.Snake[0], "paused snake head");
        }

        private static void EatsFoodAndGrows()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.LoadStateForTesting(
                3,
                3,
                new[] { new GridCell(1, 1) },
                new GridCell(1, 2),
                Direction.Down,
                0);

            game.Step();

            AssertEqual(GameStatus.Playing, game.Status, "eat food status");
            AssertEqual(10, game.Score, "eat food score");
            AssertEqual(2, game.Snake.Count, "eat food snake length");
            AssertEqual(new GridCell(1, 2), game.Snake[0], "eat food snake head");
        }

        private static void DetectsSelfCollision()
        {
            SnakeGameEngine game = new SnakeGameEngine();
            List<GridCell> initialSnake = new List<GridCell>
            {
                new GridCell(1, 1),
                new GridCell(1, 0),
                new GridCell(0, 0),
                new GridCell(0, 1)
            };

            game.LoadStateForTesting(3, 3, initialSnake, new GridCell(2, 2), Direction.Up, 0);

            game.Step();

            AssertEqual(GameStatus.GameOver, game.Status, "self collision status");
        }

        private static void WinsWhenFinalCellIsEaten()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.StartNew(1, 2);
            AssertEqual(new GridCell(0, 0), game.Food, "only available food cell");

            game.Step();

            AssertEqual(GameStatus.Won, game.Status, "filled grid status");
            AssertEqual(10, game.Score, "filled grid score");
            AssertEqual(2, game.Snake.Count, "filled grid snake length");
        }

        private static void GeneratesObstaclesAwayFromSnakeAndFood()
        {
            SnakeGameEngine game = new SnakeGameEngine(new Random(12));

            game.StartNew(12, 12, BoundaryMode.Wrap, true);

            AssertTrue(game.Obstacles.Count > 0, "obstacle generation count");
            AssertFalse(ContainsCell(game.Obstacles, game.Snake[0]), "obstacle should not be placed on the snake");
            AssertFalse(ContainsCell(game.Obstacles, game.Food), "food should not be placed on an obstacle");
        }

        private static void ObstaclesCauseGameOver()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.LoadStateForTesting(
                5,
                5,
                new[] { new GridCell(1, 1) },
                new GridCell(4, 4),
                Direction.Right,
                0,
                BoundaryMode.Wrap,
                new[] { new GridCell(2, 1) });

            game.Step();

            AssertEqual(GameStatus.GameOver, game.Status, "obstacle collision status");
            AssertEqual(new GridCell(1, 1), game.Snake[0], "obstacle collision head");
        }

        private static void RegeneratesFoodWhenResizeFindsObstacleConflict()
        {
            SnakeGameEngine game = new SnakeGameEngine(new Random(3));
            GridCell blockedFood = new GridCell(2, 2);

            game.LoadStateForTesting(
                6,
                6,
                new[] { new GridCell(1, 1) },
                blockedFood,
                Direction.Right,
                0,
                BoundaryMode.Wrap,
                new[] { blockedFood });

            game.ResizeGrid(6, 6);

            AssertFalse(game.Food.Equals(blockedFood), "resize should move food off obstacles");
            AssertFalse(ContainsCell(game.Obstacles, game.Food), "resized food should avoid every obstacle");
        }

        private static void ClampsSpeedIntoSupportedRange()
        {
            AssertEqual(1, GameSpeed.ClampSpeed(-2), "low speed clamp");
            AssertEqual(10, GameSpeed.ClampSpeed(99), "high speed clamp");
            AssertEqual(5, GameSpeed.ClampSpeed(5), "in-range speed clamp");
        }

        private static void CalculatesFixedSpeedInterval()
        {
            AssertEqual(210, GameSpeed.GetTimerInterval(5, 0, false), "fixed speed interval");
            AssertEqual(210, GameSpeed.GetTimerInterval(5, 400, false), "fixed speed ignores score");
            AssertEqual("Speed: 5", GameSpeed.GetDisplayLabel(5, false), "fixed speed label");
        }

        private static void ProgressiveSpeedGetsFasterAndCaps()
        {
            AssertEqual(210, GameSpeed.GetTimerInterval(5, 0, true), "progressive speed starting interval");
            AssertEqual(194, GameSpeed.GetTimerInterval(5, 80, true), "progressive speed interval");
            AssertEqual(114, GameSpeed.GetTimerInterval(5, 1000, true), "progressive speed bonus cap");
            AssertEqual(80, GameSpeed.GetTimerInterval(10, 1000, true), "progressive speed minimum interval");
            AssertEqual("Speed: 5+", GameSpeed.GetDisplayLabel(5, true), "progressive speed label");
            AssertEqual("5+", GameSpeed.GetDisplayValue(5, true), "progressive speed value");
        }

        private static void PresetsClampInvalidValues()
        {
            AssertEqual(GameModePreset.Arcade, GamePresets.ParseMode(99), "invalid mode preset");
            AssertEqual(BoardSizePreset.Standard, GamePresets.ParseBoardSize(-1), "invalid board preset");
            AssertEqual(VisualThemePreset.Classic, GamePresets.ParseTheme(20), "invalid theme preset");
        }

        private static void ModePresetAppliesExpectedRules()
        {
            GameModeSettings maze = GamePresets.GetMode(GameModePreset.Maze);

            AssertEqual("Maze", maze.Name, "maze name");
            AssertFalse(maze.WrapWalls, "maze should use solid walls");
            AssertTrue(maze.ProgressiveSpeed, "maze should use progressive speed");
            AssertTrue(maze.Obstacles, "maze should enable obstacles");
        }

        private static void BoardPresetHasPlayableDimensions()
        {
            BoardSizeSettings compact = GamePresets.GetBoardSize(BoardSizePreset.Compact);
            BoardSizeSettings wide = GamePresets.GetBoardSize(BoardSizePreset.Wide);

            AssertTrue(compact.GridWidth > 20, "compact grid width");
            AssertTrue(compact.GridHeight > 20, "compact grid height");
            AssertTrue(wide.GridWidth > compact.GridWidth, "wide grid width");
            AssertEqual("Wide 56x30", wide.DisplayName, "wide display name");
        }

        private static void ChallengeSeedNormalizesAndHashes()
        {
            AssertEqual("ABC-123", ChallengeSeed.Normalize(" abc-123! "), "challenge seed normalize");
            AssertEqual(ChallengeSeed.ToRandomSeed("abc-123"), ChallengeSeed.ToRandomSeed("ABC-123"), "challenge seed hash casing");
            AssertFalse(ChallengeSeed.ToRandomSeed("").HasValue, "empty challenge seed");
        }

        private static void ChallengeSeedMakesFoodDeterministic()
        {
            int seed = ChallengeSeed.ToRandomSeed("MAZE-0421").Value;
            SnakeGameEngine first = new SnakeGameEngine();
            SnakeGameEngine second = new SnakeGameEngine();

            first.StartNew(20, 20, BoundaryMode.Wrap, true, seed);
            second.StartNew(20, 20, BoundaryMode.Wrap, true, seed);

            AssertEqual(first.Food, second.Food, "seeded food");
            AssertEqual(first.Obstacles.Count, second.Obstacles.Count, "seeded obstacle count");
            for (int i = 0; i < first.Obstacles.Count; i++)
            {
                AssertEqual(first.Obstacles[i], second.Obstacles[i], "seeded obstacle " + i);
            }
        }

        private static void LeaderboardKeepsTopFivePerMode()
        {
            LeaderboardStore store = new LeaderboardStore();
            for (int score = 10; score <= 70; score += 10)
            {
                store.Add(new LeaderboardEntry("Arcade", score, "5+", "Standard", string.Empty, new DateTime(2026, 1, score / 10, 0, 0, 0, DateTimeKind.Utc)));
            }

            store.Add(new LeaderboardEntry("Classic", 5, "5", "Compact", string.Empty, DateTime.UtcNow));

            System.Collections.Generic.IList<LeaderboardEntry> arcade = store.GetTopEntries("Arcade");
            AssertEqual(5, arcade.Count, "leaderboard trim count");
            AssertEqual(70, arcade[0].Score, "leaderboard top score");
            AssertEqual(30, arcade[4].Score, "leaderboard fifth score");

            LeaderboardStore restored = LeaderboardStore.Deserialize(store.Serialize());
            AssertEqual(5, restored.GetTopEntries("Arcade").Count, "leaderboard deserialize count");
            AssertEqual(1, restored.GetTopEntries("Classic").Count, "leaderboard separate modes");
        }

        private static void AssertEqual<T>(T expected, T actual, string label)
        {
            if (!object.Equals(expected, actual))
            {
                throw new InvalidOperationException(label + ": expected " + expected + ", got " + actual);
            }
        }

        private static void AssertFalse(bool condition, string label)
        {
            if (condition)
            {
                throw new InvalidOperationException(label + ": expected false");
            }
        }

        private static void AssertTrue(bool condition, string label)
        {
            if (!condition)
            {
                throw new InvalidOperationException(label + ": expected true");
            }
        }

        private static bool ContainsCell(IEnumerable<GridCell> cells, GridCell expected)
        {
            foreach (GridCell cell in cells)
            {
                if (cell.Equals(expected))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
