using SnakeGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int MinimumTimerInterval = 80;
        private const int BaseTimerInterval = 320;
        private const int TimerIntervalStep = 22;

        // 贪吃蛇的每个部分和食物都将使用Circle类
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        public Form1()
        {
            InitializeComponent();

            // 启用双缓冲
            this.DoubleBuffered = true;
            this.KeyPreview = true;  // 确保窗体可以接收到键盘事件

            gameStarted = false; // 确保游戏未开始前不生成食物
            UpdateSettingsFromUI();
        }
        private void StartGame()
        {
            // UI 控件隐藏
            btnStartGame.Visible = false;  // 隐藏开始按钮
            trackBarSpeed.Visible = false; // 隐藏速度滑块
            gameStarted = true; // 标记游戏开始，允许食物生成

            // 读取设置
            UpdateSettingsFromUI();

            // 初始化游戏状态
            ResetGameState();
            GenerateFood();

            // 启动游戏逻辑
            timer1.Start();  // 启动定时器
        }

        private void UpdateSettingsFromUI()
        {
            // 从 UI 控件读取设置
            Settings.Speed = trackBarSpeed.Value;  // 从滑块读取速度
            ApplySpeedSetting();
        }

        private void ApplySpeedSetting()
        {
            timer1.Interval = Math.Max(MinimumTimerInterval, BaseTimerInterval - Settings.Speed * TimerIntervalStep);
        }

        private void ResetGameState()
        {
            Settings.Score = 0;   // 初始得分
            Settings.GameOver = false;  // 游戏开始时未结束
            Settings.direction = Direction.Down;  // 初始方向

            // 创建一个初始蛇
            Snake.Clear();
            Circle head = new Circle { X = 10, Y = 5 };
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

            int maxXPos = Math.Max(1, this.ClientSize.Width / Settings.Width);
            int maxYPos = Math.Max(1, this.ClientSize.Height / Settings.Height);

            food = new Circle();
            bool isOnSnake;
            do
            {
                isOnSnake = false;
                food.X = random.Next(0, maxXPos);
                food.Y = random.Next(0, maxYPos);

                foreach (Circle part in Snake)
                {
                    if (part.X == food.X && part.Y == food.Y)
                    {
                        isOnSnake = true;
                        break;
                    }
                }
            } while (isOnSnake); // 如果食物在蛇身上，重新生成位置
        }

        private void MovePlayer()
        {
            int maxXPos = Math.Max(1, this.ClientSize.Width / Settings.Width);
            int maxYPos = Math.Max(1, this.ClientSize.Height / Settings.Height);

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)  // 蛇头
                {
                    // 根据方向移动蛇头
                    switch (Settings.direction)
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
            Settings.GameOver = true;
            timer1.Stop();
            MessageBox.Show("GameOver! YourScore: " + Settings.Score);
            btnStartGame.Visible = true;  // 显示开始按钮
            trackBarSpeed.Visible = true; // 显示速度滑块
            gameStarted = false; // 重置游戏开始标志
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
            Settings.Score += 10; // 假设每吃一个食物得10分

            // 生成新的食物
            GenerateFood();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics canvas = e.Graphics;

            if (!gameStarted && Snake.Count == 0)
            {
                return;
            }

            if (!Settings.GameOver)
            {
                // 绘制食物
                Brush foodColor = Brushes.Red;
                canvas.FillEllipse(foodColor, new Rectangle(food.X * Settings.Width,
                                                            food.Y * Settings.Height,
                                                            Settings.Width, Settings.Height));

                // 绘制蛇
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColor = i == 0 ? Brushes.Black : Brushes.Green; // 头部用黑色表示，其余用绿色
                    canvas.FillRectangle(snakeColor,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOverText = "Game over\nYour final score is: " + Settings.Score;
                canvas.DrawString(gameOverText, new Font("Arial", 12), Brushes.Black, new PointF(10, 10));
            }

        }

        // 捕捉键盘按键用于控制蛇的移动方向
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    if (Settings.direction != Direction.Down)
                        Settings.direction = Direction.Up;
                    break;
                case Keys.S:
                case Keys.Down:
                    if (Settings.direction != Direction.Up)
                        Settings.direction = Direction.Down;
                    break;
                case Keys.A:
                case Keys.Left:
                    if (Settings.direction != Direction.Right)
                        Settings.direction = Direction.Left;
                    break;
                case Keys.D:
                case Keys.Right:
                    if (Settings.direction != Direction.Left)
                        Settings.direction = Direction.Right;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Settings.GameOver || !gameStarted)
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
            Settings.Speed = trackBarSpeed.Value; // 更新速度设置
            ApplySpeedSetting(); // 调整 Timer 的 Interval 值
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
    public class Settings
    {
        public static int Width { get; set; } = 16;  // 每个位置的宽度
        public static int Height { get; set; } = 16;  // 每个位置的高度
        public static int Speed { get; set; }
        public static int Score { get; set; }
        public static bool GameOver { get; set; }
        public static Direction direction { get; set; }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
