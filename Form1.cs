using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int CellSize = 16;
        private const int MinimumTimerInterval = 80;
        private const int BaseTimerInterval = 320;
        private const int TimerIntervalStep = 22;
        private const int HudHeight = 40;

        private readonly SnakeGameEngine game = new SnakeGameEngine();
        private int highScore = 0;
        private int selectedSpeed = 5;
        private bool finishMessageShown = false;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            KeyPreview = true;

            LoadHighScore();
            UpdateSettingsFromUI();
            UpdateHud();
            LayoutControls();
        }

        private void StartGame()
        {
            btnStartGame.Visible = false;
            trackBarSpeed.Visible = false;
            btnPause.Visible = true;
            btnPause.Text = "Pause";
            finishMessageShown = false;

            UpdateSettingsFromUI();
            game.StartNew(GetMaxGridX(), GetMaxGridY());
            UpdateHighScore();
            UpdateHud();

            if (game.IsFinished)
            {
                FinishRoundIfNeeded();
                return;
            }

            timer1.Start();
            Focus();
        }

        private void UpdateSettingsFromUI()
        {
            selectedSpeed = trackBarSpeed.Value;
            ApplySpeedSetting();
        }

        private void ApplySpeedSetting()
        {
            timer1.Interval = Math.Max(MinimumTimerInterval, BaseTimerInterval - selectedSpeed * TimerIntervalStep);
        }

        private int GetMaxGridX()
        {
            return Math.Max(1, ClientSize.Width / CellSize);
        }

        private int GetMaxGridY()
        {
            return Math.Max(1, (ClientSize.Height - HudHeight) / CellSize);
        }

        private int GetCanvasY(int gridY)
        {
            return HudHeight + gridY * CellSize;
        }

        private void LayoutControls()
        {
            if (btnStartGame == null || trackBarSpeed == null || btnPause == null)
            {
                return;
            }

            int centerX = Math.Max(0, (ClientSize.Width - btnStartGame.Width) / 2);
            int menuTop = Math.Max(HudHeight + 20, (ClientSize.Height - btnStartGame.Height - trackBarSpeed.Height - 24) / 2);

            btnStartGame.Location = new Point(centerX, menuTop);
            trackBarSpeed.Location = new Point(centerX, btnStartGame.Bottom + 24);
            btnPause.Location = new Point(Math.Max(12, ClientSize.Width - btnPause.Width - 12), 8);
        }

        private void UpdateHud()
        {
            lblScore.Text = "Score: " + game.Score;
            lblHighScore.Text = "Best: " + highScore;
            lblSpeed.Text = "Speed: " + selectedSpeed;
            lblStatus.Text = GetStatusText();
        }

        private string GetStatusText()
        {
            switch (game.Status)
            {
                case GameStatus.Won:
                    return "You Win";
                case GameStatus.GameOver:
                    return "Game Over";
                case GameStatus.Paused:
                    return "Paused";
                case GameStatus.Playing:
                    return "Playing";
                default:
                    return "Ready";
            }
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
            if (game.Score <= highScore)
            {
                return;
            }

            highScore = game.Score;
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

        private void TogglePause()
        {
            if (game.Status != GameStatus.Playing && game.Status != GameStatus.Paused)
            {
                return;
            }

            game.TogglePause();
            if (game.Status == GameStatus.Paused)
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

        private void FinishRoundIfNeeded()
        {
            if (!game.IsFinished)
            {
                return;
            }

            timer1.Stop();
            UpdateHighScore();
            btnPause.Visible = false;
            btnPause.Text = "Pause";
            btnStartGame.Visible = true;
            btnStartGame.Text = "Restart Game";
            trackBarSpeed.Visible = true;
            UpdateHud();
            Invalidate();

            if (!finishMessageShown)
            {
                finishMessageShown = true;
                MessageBox.Show(GetFinishMessage());
            }
        }

        private string GetFinishMessage()
        {
            if (game.Status == GameStatus.Won)
            {
                return "You win! Final score: " + game.Score;
            }

            return "Game over! Your score: " + game.Score;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (game.Status == GameStatus.Ready && game.Snake.Count == 0)
            {
                return;
            }

            if (game.IsFinished)
            {
                DrawFinishedState(e.Graphics);
                return;
            }

            DrawFood(e.Graphics);
            DrawSnake(e.Graphics);
        }

        private void DrawFood(Graphics canvas)
        {
            GridCell food = game.Food;
            canvas.FillEllipse(Brushes.Red, new Rectangle(food.X * CellSize, GetCanvasY(food.Y), CellSize, CellSize));
        }

        private void DrawSnake(Graphics canvas)
        {
            for (int i = 0; i < game.Snake.Count; i++)
            {
                GridCell part = game.Snake[i];
                Brush snakeColor = i == 0 ? Brushes.Black : Brushes.Green;
                canvas.FillRectangle(
                    snakeColor,
                    new Rectangle(part.X * CellSize, GetCanvasY(part.Y), CellSize, CellSize));
            }
        }

        private void DrawFinishedState(Graphics canvas)
        {
            string text = game.Status == GameStatus.Won
                ? "You win\nYour final score is: " + game.Score
                : "Game over\nYour final score is: " + game.Score;

            canvas.DrawString(text, Font, Brushes.Black, new PointF(10, HudHeight + 10));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter && (game.Status == GameStatus.Ready || game.IsFinished))
            {
                StartGame();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (game.Status != GameStatus.Playing)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    game.QueueDirection(Direction.Up);
                    break;
                case Keys.S:
                case Keys.Down:
                    game.QueueDirection(Direction.Down);
                    break;
                case Keys.A:
                case Keys.Left:
                    game.QueueDirection(Direction.Left);
                    break;
                case Keys.D:
                case Keys.Right:
                    game.QueueDirection(Direction.Right);
                    break;
            }

            base.OnKeyDown(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (game.Status != GameStatus.Playing)
            {
                return;
            }

            game.Step();
            UpdateHighScore();
            UpdateHud();
            Invalidate();
            FinishRoundIfNeeded();
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            UpdateSettingsFromUI();
            UpdateHud();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            TogglePause();
            Focus();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (game == null)
            {
                return;
            }

            LayoutControls();
            game.ResizeGrid(GetMaxGridX(), GetMaxGridY());
            if (lblScore != null)
            {
                UpdateHud();
            }

            Invalidate();
            FinishRoundIfNeeded();
        }
    }
}
