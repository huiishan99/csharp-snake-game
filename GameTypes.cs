using System;

namespace SnakeGame
{
    public enum GameStatus
    {
        Ready,
        Playing,
        Paused,
        GameOver,
        Won
    }

    public enum BoundaryMode
    {
        Wrap,
        SolidWalls
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public struct GridCell : IEquatable<GridCell>
    {
        public GridCell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Equals(GridCell other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCell && Equals((GridCell)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}
