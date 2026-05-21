using System;

namespace SnakeGame.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                RunAll();
                Console.WriteLine("All SnakeGameEngine checks passed.");
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
            IgnoresImmediateReverseDirection();
            DoesNotMoveWhilePaused();
            WinsWhenFinalCellIsEaten();
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

        private static void IgnoresImmediateReverseDirection()
        {
            SnakeGameEngine game = new SnakeGameEngine();

            game.StartNew(5, 5);
            game.QueueDirection(Direction.Up);
            game.Step();

            AssertEqual(Direction.Down, game.CurrentDirection, "reverse input should be ignored");
            AssertEqual(new GridCell(2, 3), game.Snake[0], "snake should continue downward");
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
