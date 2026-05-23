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
            DoesNotMoveWhilePaused();
            EatsFoodAndGrows();
            DetectsSelfCollision();
            WinsWhenFinalCellIsEaten();
            ClampsSpeedIntoSupportedRange();
            CalculatesFixedSpeedInterval();
            ProgressiveSpeedGetsFasterAndCaps();
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
    }
}
