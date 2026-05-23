using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public class SnakeGameEngine
    {
        private const int MaxQueuedDirections = 2;
        private const int PointsPerFood = 10;
        private const int MinimumCellsForObstacles = 20;
        private const int ObstacleCellRatio = 80;
        private const int MinimumObstacleCount = 3;
        private const int MaximumObstacleCount = 18;

        private readonly List<GridCell> snake = new List<GridCell>();
        private readonly List<GridCell> obstacles = new List<GridCell>();
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

        public IReadOnlyList<GridCell> Obstacles
        {
            get { return obstacles; }
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
        public bool UsesObstacles { get; private set; }
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
            StartNew(gridWidth, gridHeight, boundaryMode, false);
        }

        public void StartNew(int gridWidth, int gridHeight, BoundaryMode boundaryMode, bool useObstacles)
        {
            GridWidth = NormalizeDimension(gridWidth);
            GridHeight = NormalizeDimension(gridHeight);
            Score = 0;
            Status = GameStatus.Playing;
            CurrentBoundaryMode = boundaryMode;
            UsesObstacles = useObstacles;
            CurrentDirection = Direction.Down;
            directionQueue.Clear();

            snake.Clear();
            snake.Add(new GridCell(GridWidth / 2, GridHeight / 2));
            GenerateObstacles();

            GenerateFood();
        }

        internal void LoadStateForTesting(
            int gridWidth,
            int gridHeight,
            IEnumerable<GridCell> initialSnake,
            GridCell initialFood,
            Direction initialDirection,
            int score,
            BoundaryMode boundaryMode = BoundaryMode.Wrap,
            IEnumerable<GridCell> initialObstacles = null)
        {
            GridWidth = NormalizeDimension(gridWidth);
            GridHeight = NormalizeDimension(gridHeight);
            Score = Math.Max(0, score);
            Status = GameStatus.Playing;
            CurrentBoundaryMode = boundaryMode;
            CurrentDirection = initialDirection;
            directionQueue.Clear();
            obstacles.Clear();

            snake.Clear();
            foreach (GridCell part in initialSnake)
            {
                snake.Add(WrapCell(part));
            }

            if (snake.Count == 0)
            {
                snake.Add(new GridCell(GridWidth / 2, GridHeight / 2));
            }

            if (initialObstacles != null)
            {
                HashSet<GridCell> occupied = UniqueSnakeCells();
                foreach (GridCell obstacle in initialObstacles)
                {
                    GridCell wrappedObstacle = WrapCell(obstacle);
                    if (!occupied.Contains(wrappedObstacle) && !obstacles.Contains(wrappedObstacle))
                    {
                        obstacles.Add(wrappedObstacle);
                    }
                }
            }

            UsesObstacles = obstacles.Count > 0;
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
            KeepObstaclesInBounds();

            if (Status != GameStatus.Playing && Status != GameStatus.Paused)
            {
                return;
            }

            if (UniqueSnakeCells().Count + obstacles.Count >= GridWidth * GridHeight)
            {
                Status = GameStatus.Won;
                return;
            }

            if (!IsInBounds(food) || IsCellOnSnake(food) || IsCellOnObstacle(food))
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

            if (IsCellOnObstacle(nextHead))
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
                if (snake.Count + obstacles.Count >= GridWidth * GridHeight)
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
            int availableCellCount = CountAvailableFoodCells(occupied);

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
                    if (occupied.Contains(candidate) || IsCellOnObstacle(candidate))
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

        private void GenerateObstacles()
        {
            obstacles.Clear();
            if (!UsesObstacles)
            {
                return;
            }

            int targetCount = Math.Min(GetTargetObstacleCount(), Math.Max(0, CountAvailableObstacleCells() - 1));
            for (int i = 0; i < targetCount; i++)
            {
                int selectedIndex = random.Next(CountAvailableObstacleCells());
                for (int y = 0; y < GridHeight; y++)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        GridCell candidate = new GridCell(x, y);
                        if (!IsAvailableForObstacle(candidate))
                        {
                            continue;
                        }

                        if (selectedIndex == 0)
                        {
                            obstacles.Add(candidate);
                            break;
                        }

                        selectedIndex--;
                    }

                    if (obstacles.Count > i)
                    {
                        break;
                    }
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

        private void KeepObstaclesInBounds()
        {
            if (!UsesObstacles)
            {
                obstacles.Clear();
                return;
            }

            HashSet<GridCell> blocked = UniqueSnakeCells();
            List<GridCell> normalizedObstacles = new List<GridCell>();

            foreach (GridCell obstacle in obstacles)
            {
                GridCell wrappedObstacle = WrapCell(obstacle);
                if (blocked.Add(wrappedObstacle))
                {
                    normalizedObstacles.Add(wrappedObstacle);
                }
            }

            obstacles.Clear();
            obstacles.AddRange(normalizedObstacles);
        }

        private HashSet<GridCell> UniqueSnakeCells()
        {
            return new HashSet<GridCell>(snake);
        }

        private bool IsCellOnSnake(GridCell cell)
        {
            return UniqueSnakeCells().Contains(cell);
        }

        private bool IsCellOnObstacle(GridCell cell)
        {
            return obstacles.Contains(cell);
        }

        private int CountAvailableFoodCells(HashSet<GridCell> occupiedSnakeCells)
        {
            int availableCells = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    GridCell candidate = new GridCell(x, y);
                    if (!occupiedSnakeCells.Contains(candidate) && !IsCellOnObstacle(candidate))
                    {
                        availableCells++;
                    }
                }
            }

            return availableCells;
        }

        private int CountAvailableObstacleCells()
        {
            int availableCells = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    if (IsAvailableForObstacle(new GridCell(x, y)))
                    {
                        availableCells++;
                    }
                }
            }

            return availableCells;
        }

        private int GetTargetObstacleCount()
        {
            int boardCells = GridWidth * GridHeight;
            if (boardCells < MinimumCellsForObstacles)
            {
                return 0;
            }

            return Math.Min(MaximumObstacleCount, Math.Max(MinimumObstacleCount, boardCells / ObstacleCellRatio));
        }

        private bool IsAvailableForObstacle(GridCell cell)
        {
            return !IsCellOnSnake(cell)
                && !IsCellOnObstacle(cell)
                && !IsProtectedSpawnCell(cell);
        }

        private bool IsProtectedSpawnCell(GridCell cell)
        {
            if (snake.Count == 0)
            {
                return false;
            }

            GridCell spawn = snake[0];
            return Math.Abs(cell.X - spawn.X) + Math.Abs(cell.Y - spawn.Y) <= 2;
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
