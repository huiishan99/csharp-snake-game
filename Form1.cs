using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int MinimumTimerInterval = 80;
        private const int BaseTimerInterval = 320;
        private const int TimerIntervalStep = 22;
        private const int HudHeight = 40;

        // 贪吃蛇的每个部分和食物都将使用Circle类
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        private int highScore = 0;
        private bool isPaused = false;
        private bool gameWon = false;
        private Direction pendingDirection;
        public Form1()
        {
            InitializeComponent();

            // 启用双缓冲
            this.DoubleBuffered = true;
            this.KeyPreview = true;  // 确保窗体可以接收到键盘事件

            gameStarted = false; // 确保游戏未开始前不生成食物
            LoadHighScore();
            UpdateSettingsFromUI();
            UpdateHud();
            LayoutControls();
        }
        private void StartGame()
        {
            // UI 控件隐藏
            btnStartGame.Visible = false;  // 隐藏开始按钮
            trackBarSpeed.Visible = false; // 隐藏速度滑块
            btnPause.Visible = true;
            btnPause.Text = "Pause";
            isPaused = false;
            gameStarted = true; // 标记游戏开始，允许食物生成

            // 读取设置
            UpdateSettingsFromUI();

            // 初始化游戏状态
            ResetGameState();
            GenerateFood();
            UpdateHud();

            if (GameSettings.GameOver)
            {
                return;
            }

            // 启动游戏逻辑
            timer1.Start();  // 启动定时器
            this.Focus();
        }

        private void UpdateSettingsFromUI()
        {
            // 从 UI 控件读取设置
            GameSettings.Speed = trackBarSpeed.Value;  // 从滑块读取速度
            ApplySpeedSetting();
        }

        private void ApplySpeedSetting()
        {
            timer1.Interval = Math.Max(MinimumTimerInterval, BaseTimerInterval - GameSettings.Speed * TimerIntervalStep);
        }

        private int GetMaxGridX()
        {
            return Math.Max(1, this.ClientSize.Width / GameSettings.Width);
        }

        private int GetMaxGridY()
        {
            return Math.Max(1, (this.ClientSize.Height - HudHeight) / GameSettings.Height);
        }

        private int GetCanvasY(int gridY)
        {
            return HudHeight + gridY * GameSettings.Height;
        }

        private void LayoutControls()
        {
            if (btnStartGame == null || trackBarSpeed == null || btnPause == null)
            {
                return;
            }

            int centerX = Math.Max(0, (this.ClientSize.Width - btnStartGame.Width) / 2);
            int menuTop = Math.Max(HudHeight + 20, (this.ClientSize.Height - btnStartGame.Height - trackBarSpeed.Height - 24) / 2);

            btnStartGame.Location = new Point(centerX, menuTop);
            trackBarSpeed.Location = new Point(centerX, btnStartGame.Bottom + 24);
            btnPause.Location = new Point(Math.Max(12, this.ClientSize.Width - btnPause.Width - 12), 8);
        }

        private void UpdateHud()
        {
            lblScore.Text = "Score: " + GameSettings.Score;
            lblHighScore.Text = "Best: " + highScore;
            lblSpeed.Text = "Speed: " + GameSettings.Speed;
            lblStatus.Text = GetStatusText();
        }

        private string GetStatusText()
        {
            if (gameWon)
            {
                return "You Win";
            }

            if (GameSettings.GameOver)
            {
                return "Game Over";
            }

            if (isPaused)
            {
                return "Paused";
            }

            if (gameStarted)
            {
                return "Playing";
            }

            return "Ready";
        }

        private void LoadHighScore()
        {
            try
            {
                highScore = Math.Max(0, SnakeGame.Properties.Settings.Default.HighScore);
            }
            catch
            {
                highScore = 0;
            }
        }

        private void UpdateHighScore()
        {
            if (GameSettings.Score <= highScore)
            {
                return;
            }

            highScore = GameSettings.Score;
            SaveHighScore();
        }

        private void SaveHighScore()
        {
            try
            {
                SnakeGame.Properties.Settings.Default.HighScore = highScore;
                SnakeGame.Properties.Settings.Default.Save();
            }
            catch
            {
                // High score persistence should never interrupt gameplay.
            }
        }

        private void ResetGameState()
        {
            GameSettings.Score = 0;   // 初始得分
            GameSettings.GameOver = false;  // 游戏开始时未结束
            GameSettings.Direction = Direction.Down;  // 初始方向
            pendingDirection = GameSettings.Direction;
            gameWon = false;

            // 创建一个初始蛇
            Snake.Clear();
            Circle head = new Circle { X = GetMaxGridX() / 2, Y = GetMaxGridY() / 2 };
            Snake.Add(head);

            // 可能需要添加代码来清除屏幕上的旧食物或游戏结束信息
            this.Invalidate(); // 强制重绘窗体以清除任何旧图形
        }
        private bool gameStarted = false; // 新增一个标志来判断游戏是否开始
        private Random random = new Random(); // 定义为类的成员变量
        private void GenerateFood()
        {
            if (!gameStarted)
            {
                return; // 如果游戏未开始，不执行食物生成
            }

            int maxXPos = GetMaxGridX();
            int maxYPos = GetMaxGridY();

            List<Circle> availableCells = new List<Circle>();
            for (int x = 0; x < maxXPos; x++)
            {
                for (int y = 0; y < maxYPos; y++)
                {
                    if (!IsCellOnSnake(x, y))
                    {
                        availableCells.Add(new Circle { X = x, Y = y });
                    }
                }
            }

            if (availableCells.Count == 0)
            {
                WinGame();
                return;
            }

            food = availableCells[random.Next(availableCells.Count)];
        }

        private bool IsCellOnSnake(int x, int y)
        {
            foreach (Circle part in Snake)
            {
                if (part.X == x && part.Y == y)
                {
                    return true;
                }
            }

            return false;
        }

        private int WrapCoordinate(int value, int maxExclusive)
        {
            if (maxExclusive <= 0)
            {
                return 0;
            }

            int wrapped = value % maxExclusive;
            return wrapped < 0 ? wrapped + maxExclusive : wrapped;
        }

        private void KeepGameObjectsInBounds()
        {
            if (Snake.Count == 0)
            {
                return;
            }

            int maxXPos = GetMaxGridX();
            int maxYPos = GetMaxGridY();

            foreach (Circle part in Snake)
            {
                part.X = WrapCoordinate(part.X, maxXPos);
                part.Y = WrapCoordinate(part.Y, maxYPos);
            }

            if (!gameStarted || GameSettings.GameOver)
            {
                return;
            }

            bool foodOutOfBounds = food.X < 0 || food.X >= maxXPos || food.Y < 0 || food.Y >= maxYPos;
            if (foodOutOfBounds || IsCellOnSnake(food.X, food.Y))
            {
                GenerateFood();
            }
        }

        private void MovePlayer()
        {
            int maxXPos = GetMaxGridX();
            int maxYPos = GetMaxGridY();
            GameSettings.Direction = pendingDirection;

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)  // 蛇头
                {
                    // 根据方向移动蛇头
                    switch (GameSettings.Direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    // 处理蛇穿越边界的情况
                    if (Snake[i].X < 0) Snake[i].X = maxXPos - 1; // 从左边穿出去，从右边出现
                    if (Snake[i].X >= maxXPos) Snake[i].X = 0; // 从右边穿出去，从左边出现
                    if (Snake[i].Y < 0) Snake[i].Y = maxYPos - 1; // 从上面穿出去，从下面出现
                    if (Snake[i].Y >= maxYPos) Snake[i].Y = 0; // 从下面穿出去，从上面出现
                    // 检测碰到自己的身体
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            EndGame(); // 结束游戏
                            return;
                        }
                    }

                    // 检测蛇头是否碰到食物
                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }
                }
                else
                {
                    // 其余部分跟随前一个移动
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }
        private void EndGame()
        {
            FinishGame("Game over! Your score: " + GameSettings.Score, false);
        }

        private void WinGame()
        {
            FinishGame("You win! Final score: " + GameSettings.Score, true);
        }

        private void FinishGame(string message, bool won)
        {
            GameSettings.GameOver = true;
            gameWon = won;
            timer1.Stop();
            UpdateHighScore();
            isPaused = false;
            btnPause.Visible = false;
            btnPause.Text = "Pause";
            UpdateHud();
            MessageBox.Show(message);
            btnStartGame.Visible = true;  // 显示开始按钮
            btnStartGame.Text = "Restart Game";
            trackBarSpeed.Visible = true; // 显示速度滑块
            gameStarted = false; // 重置游戏开始标志
            UpdateHud();
            this.Invalidate();
        }
        private void EatFood()
        {
            // 吃食物：在蛇的尾部增加一个新的部分
            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(body);

            // 更新分数等
            GameSettings.Score += 10; // 假设每吃一个食物得10分
            UpdateHighScore();
            UpdateHud();

            // 生成新的食物
            GenerateFood();
        }

        private void TogglePause()
        {
            if (!gameStarted || GameSettings.GameOver)
            {
                return;
            }

            isPaused = !isPaused;
            if (isPaused)
            {
                timer1.Stop();
                btnPause.Text = "Resume";
            }
            else
            {
                timer1.Start();
                btnPause.Text = "Pause";
            }

            UpdateHud();
        }

        private void QueueDirection(Direction newDirection)
        {
            if (IsOppositeDirection(GameSettings.Direction, newDirection))
            {
                return;
            }

            pendingDirection = newDirection;
        }

        private bool IsOppositeDirection(Direction currentDirection, Direction newDirection)
        {
            return (currentDirection == Direction.Up && newDirection == Direction.Down)
                || (currentDirection == Direction.Down && newDirection == Direction.Up)
                || (currentDirection == Direction.Left && newDirection == Direction.Right)
                || (currentDirection == Direction.Right && newDirection == Direction.Left);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics canvas = e.Graphics;

            if (!gameStarted && Snake.Count == 0)
            {
                return;
            }

            if (!GameSettings.GameOver)
            {
                // 绘制食物
                Brush foodColor = Brushes.Red;
                canvas.FillEllipse(foodColor, new Rectangle(food.X * GameSettings.Width,
                                                            GetCanvasY(food.Y),
                                                            GameSettings.Width, GameSettings.Height));

                // 绘制蛇
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColor = i == 0 ? Brushes.Black : Brushes.Green; // 头部用黑色表示，其余用绿色
                    canvas.FillRectangle(snakeColor,
                        new Rectangle(Snake[i].X * GameSettings.Width,
                                      GetCanvasY(Snake[i].Y),
                                      GameSettings.Width, GameSettings.Height));
                }
            }
            else
            {
                string gameOverText = gameWon
                    ? "You win\nYour final score is: " + GameSettings.Score
                    : "Game over\nYour final score is: " + GameSettings.Score;
                canvas.DrawString(gameOverText, new Font("Arial", 12), Brushes.Black, new PointF(10, HudHeight + 10));
            }

        }

        // 捕捉键盘按键用于控制蛇的移动方向
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter && (!gameStarted || GameSettings.GameOver))
            {
                StartGame();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (!gameStarted || GameSettings.GameOver || isPaused)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    QueueDirection(Direction.Up);
                    break;
                case Keys.S:
                case Keys.Down:
                    QueueDirection(Direction.Down);
                    break;
                case Keys.A:
                case Keys.Left:
                    QueueDirection(Direction.Left);
                    break;
                case Keys.D:
                case Keys.Right:
                    QueueDirection(Direction.Right);
                    break;
            }

            base.OnKeyDown(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GameSettings.GameOver || !gameStarted || isPaused)
            {
                return;
            }

            MovePlayer();
            this.Invalidate(); // 强制窗体重绘
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            StartGame();  // 开始游戏
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            GameSettings.Speed = trackBarSpeed.Value; // 更新速度设置
            ApplySpeedSetting(); // 调整 Timer 的 Interval 值
            UpdateHud();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            TogglePause();
            this.Focus();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutControls();
            KeepGameObjectsInBounds();
            this.Invalidate();
        }
    }

    public class Circle
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Circle()
        {
            X = 0;
            Y = 0;
        }
    }

    // 设置类，可以放在同一个文件中或者分离到其他文件
    public class GameSettings
    {
        public static int Width { get; set; } = 16;  // 每个位置的宽度
        public static int Height { get; set; } = 16;  // 每个位置的高度
        public static int Speed { get; set; }
        public static int Score { get; set; }
        public static bool GameOver { get; set; }
        public static Direction Direction { get; set; }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
