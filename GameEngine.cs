using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public class SnakeGameEngine
    {
        private const int MaxQueuedDirections = 2;
        private const int PointsPerFood = 10;

        private readonly List<GridCell> snake = new List<GridCell>();
        private readonly Queue<Direction> directionQueue = new Queue<Direction>();
        private readonly Random random;
        private GridCell food;

        public SnakeGameEngine()
            : this(new Random())
        {
        }

        internal SnakeGameEngine(Random random)
        {
            this.random = random;
            Status = GameStatus.Ready;
            CurrentDirection = Direction.Down;
            CurrentBoundaryMode = BoundaryMode.Wrap;
        }

        public IReadOnlyList<GridCell> Snake
        {
            get { return snake; }
        }

        public GridCell Food
        {
            get { return food; }
        }

        public int Score { get; private set; }
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public BoundaryMode CurrentBoundaryMode { get; private set; }
        public GameStatus Status { get; private set; }

        public bool IsFinished
        {
            get { return Status == GameStatus.GameOver || Status == GameStatus.Won; }
        }

        public void StartNew(int gridWidth, int gridHeight)
        {
            StartNew(gridWidth, gridHeight, BoundaryMode.Wrap);
        }

        public void StartNew(int gridWidth, int gridHeight, BoundaryMode boundaryMode)
        {
            GridWidth = NormalizeDimension(gridWidth);
            GridHeight = NormalizeDimension(gridHeight);
            Score = 0;
            Status = GameStatus.Playing;
            CurrentBoundaryMode = boundaryMode;
            CurrentDirection = Direction.Down;
            directionQueue.Clear();

            snake.Clear();
            snake.Add(new GridCell(GridWidth / 2, GridHeight / 2));

            GenerateFood();
        }

        internal void LoadStateForTesting(
            int gridWidth,
            int gridHeight,
            IEnumerable<GridCell> initialSnake,
            GridCell initialFood,
            Direction initialDirection,
            int score,
            BoundaryMode boundaryMode = BoundaryMode.Wrap)
        {
            GridWidth = NormalizeDimension(gridWidth);
            GridHeight = NormalizeDimension(gridHeight);
            Score = Math.Max(0, score);
            Status = GameStatus.Playing;
            CurrentBoundaryMode = boundaryMode;
            CurrentDirection = initialDirection;
            directionQueue.Clear();

            snake.Clear();
            foreach (GridCell part in initialSnake)
            {
                snake.Add(WrapCell(part));
            }

            if (snake.Count == 0)
            {
                snake.Add(new GridCell(GridWidth / 2, GridHeight / 2));
            }

            food = WrapCell(initialFood);
        }

        public void ResizeGrid(int gridWidth, int gridHeight)
        {
            GridWidth = NormalizeDimension(gridWidth);
            GridHeight = NormalizeDimension(gridHeight);

            if (snake.Count == 0)
            {
                return;
            }

            KeepSnakeInBounds();

            if (Status != GameStatus.Playing && Status != GameStatus.Paused)
            {
                return;
            }

            if (UniqueSnakeCells().Count >= GridWidth * GridHeight)
            {
                Status = GameStatus.Won;
                return;
            }

            if (!IsInBounds(food) || IsCellOnSnake(food))
            {
                GenerateFood();
            }
        }

        public void TogglePause()
        {
            if (Status == GameStatus.Playing)
            {
                Status = GameStatus.Paused;
            }
            else if (Status == GameStatus.Paused)
            {
                Status = GameStatus.Playing;
            }
        }

        public void QueueDirection(Direction newDirection)
        {
            if (Status != GameStatus.Playing)
            {
                return;
            }

            Direction comparisonDirection = GetDirectionForQueueValidation();
            if (comparisonDirection == newDirection || IsOppositeDirection(comparisonDirection, newDirection))
            {
                return;
            }

            if (directionQueue.Count >= MaxQueuedDirections)
            {
                return;
            }

            directionQueue.Enqueue(newDirection);
        }

        public void Step()
        {
            if (Status != GameStatus.Playing || snake.Count == 0)
            {
                return;
            }

            if (directionQueue.Count > 0)
            {
                CurrentDirection = directionQueue.Dequeue();
            }

            GridCell nextHead = GetNextHead();
            if (!IsInBounds(nextHead))
            {
                Status = GameStatus.GameOver;
                return;
            }

            bool willEat = nextHead.Equals(food);

            HashSet<GridCell> occupied = UniqueSnakeCells();
            if (!willEat)
            {
                occupied.Remove(snake[snake.Count - 1]);
            }

            if (occupied.Contains(nextHead))
            {
                Status = GameStatus.GameOver;
                return;
            }

            snake.Insert(0, nextHead);

            if (willEat)
            {
                Score += PointsPerFood;
                if (snake.Count >= GridWidth * GridHeight)
                {
                    Status = GameStatus.Won;
                    return;
                }

                GenerateFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private void GenerateFood()
        {
            HashSet<GridCell> occupied = UniqueSnakeCells();
            int availableCellCount = GridWidth * GridHeight - occupied.Count;

            if (availableCellCount <= 0)
            {
                Status = GameStatus.Won;
                return;
            }

            int selectedIndex = random.Next(availableCellCount);
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    GridCell candidate = new GridCell(x, y);
                    if (occupied.Contains(candidate))
                    {
                        continue;
                    }

                    if (selectedIndex == 0)
                    {
                        food = candidate;
                        return;
                    }

                    selectedIndex--;
                }
            }
        }

        private GridCell GetNextHead()
        {
            GridCell head = snake[0];
            GridCell nextHead;
            switch (CurrentDirection)
            {
                case Direction.Right:
                    nextHead = new GridCell(head.X + 1, head.Y);
                    break;
                case Direction.Left:
                    nextHead = new GridCell(head.X - 1, head.Y);
                    break;
                case Direction.Up:
                    nextHead = new GridCell(head.X, head.Y - 1);
                    break;
                default:
                    nextHead = new GridCell(head.X, head.Y + 1);
                    break;
            }

            return CurrentBoundaryMode == BoundaryMode.Wrap ? WrapCell(nextHead) : nextHead;
        }

        private Direction GetDirectionForQueueValidation()
        {
            Direction direction = CurrentDirection;
            foreach (Direction queuedDirection in directionQueue)
            {
                direction = queuedDirection;
            }

            return direction;
        }

        private void KeepSnakeInBounds()
        {
            HashSet<GridCell> seen = new HashSet<GridCell>();
            List<GridCell> normalizedSnake = new List<GridCell>();

            foreach (GridCell part in snake)
            {
                GridCell wrappedPart = WrapCell(part);
                if (seen.Add(wrappedPart))
                {
                    normalizedSnake.Add(wrappedPart);
                }
            }

            snake.Clear();
            snake.AddRange(normalizedSnake);

            if (snake.Count == 0)
            {
                snake.Add(new GridCell(GridWidth / 2, GridHeight / 2));
            }
        }

        private HashSet<GridCell> UniqueSnakeCells()
        {
            return new HashSet<GridCell>(snake);
        }

        private bool IsCellOnSnake(GridCell cell)
        {
            return UniqueSnakeCells().Contains(cell);
        }

        private bool IsInBounds(GridCell cell)
        {
            return cell.X >= 0 && cell.X < GridWidth && cell.Y >= 0 && cell.Y < GridHeight;
        }

        private GridCell WrapCell(GridCell cell)
        {
            return new GridCell(WrapCoordinate(cell.X, GridWidth), WrapCoordinate(cell.Y, GridHeight));
        }

        private static int WrapCoordinate(int value, int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                return 0;
            }

            int wrapped = value % maxExclusive;
            return wrapped < 0 ? wrapped + maxExclusive : wrapped;
        }

        private static int NormalizeDimension(int value)
        {
            return Math.Max(1, value);
        }

        private static bool IsOppositeDirection(Direction currentDirection, Direction newDirection)
        {
            return (currentDirection == Direction.Up && newDirection == Direction.Down)
                || (currentDirection == Direction.Down && newDirection == Direction.Up)
                || (currentDirection == Direction.Left && newDirection == Direction.Right)
                || (currentDirection == Direction.Right && newDirection == Direction.Left);
        }
    }
}
