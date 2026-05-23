using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int CellSize = 16;
        private const int MinimumTimerInterval = 80;
        private const int BaseTimerInterval = 320;
        private const int TimerIntervalStep = 22;
        private const int ProgressiveScoreStep = 40;
        private const int ProgressiveTimerStep = 8;
        private const int ProgressiveTimerBonusMax = 96;
        private const int HudHeight = 40;
        private const int HudPadding = 12;
        private const int HudGap = 10;
        private const int HudControlTop = 8;
        private const int HudControlHeight = 24;
        private const int DefaultSpeed = 5;
        private const bool DefaultWrapWalls = true;
        private const bool DefaultProgressiveSpeed = true;
        private static readonly Color WindowBackColor = Color.FromArgb(18, 24, 27);
        private static readonly Color BoardBackColor = Color.FromArgb(25, 35, 39);
        private static readonly Color GridColor = Color.FromArgb(34, 48, 52);
        private static readonly Color HudBackColor = Color.FromArgb(14, 19, 22);
        private static readonly Color HudTextColor = Color.FromArgb(225, 239, 235);
        private static readonly Color SnakeHeadColor = Color.FromArgb(180, 255, 190);
        private static readonly Color SnakeBodyColor = Color.FromArgb(80, 205, 132);
        private static readonly Color SnakeShadowColor = Color.FromArgb(42, 116, 82);
        private static readonly Color FoodColor = Color.FromArgb(255, 94, 94);
        private static readonly Color FoodHighlightColor = Color.FromArgb(255, 176, 128);
        private static readonly Color OverlayColor = Color.FromArgb(190, 9, 15, 18);
        private static readonly Color OverlayTitleColor = Color.FromArgb(234, 255, 238);
        private static readonly Color OverlayTextColor = Color.FromArgb(184, 207, 200);

        private readonly SnakeGameEngine game = new SnakeGameEngine();
        private int highScore = 0;
        private int selectedSpeed = DefaultSpeed;
        private bool useProgressiveSpeed = DefaultProgressiveSpeed;
        private bool suppressPlayerSettingSave;
        private Font primaryButtonFont;
        private Font secondaryButtonFont;
        private Font overlayTitleFont;
        private Font overlayTextFont;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            KeyPreview = true;
            BackColor = WindowBackColor;

            ConfigureHudLabels();
            ConfigureButtons();
            ConfigureOverlayFonts();
            LoadHighScore();
            LoadPlayerSettings();
            UpdateSettingsFromUI();
            UpdateHud();
            LayoutControls();
        }

        private void StartGame()
        {
            btnStartGame.Visible = false;
            trackBarSpeed.Visible = false;
            chkWrapWalls.Visible = false;
            chkProgressiveSpeed.Visible = false;
            btnPause.Visible = true;
            btnPause.Text = "Pause";

            UpdateSettingsFromUI();
            SavePlayerSettings();
            game.StartNew(GetMaxGridX(), GetMaxGridY(), GetSelectedBoundaryMode());
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
            useProgressiveSpeed = chkProgressiveSpeed.Checked;
            ApplySpeedSetting();
        }

        private void ApplySpeedSetting()
        {
            int interval = BaseTimerInterval - selectedSpeed * TimerIntervalStep - GetProgressiveTimerBonus();
            timer1.Interval = Math.Max(MinimumTimerInterval, interval);
        }

        private int GetProgressiveTimerBonus()
        {
            if (!useProgressiveSpeed)
            {
                return 0;
            }

            int scoreSteps = Math.Max(0, game.Score / ProgressiveScoreStep);
            return Math.Min(ProgressiveTimerBonusMax, scoreSteps * ProgressiveTimerStep);
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
            if (btnStartGame == null || trackBarSpeed == null || btnPause == null || chkWrapWalls == null || chkProgressiveSpeed == null)
            {
                return;
            }

            int centerX = Math.Max(0, (ClientSize.Width - btnStartGame.Width) / 2);
            int menuHeight = btnStartGame.Height + trackBarSpeed.Height + chkWrapWalls.Height + chkProgressiveSpeed.Height + 46;
            int menuTop = Math.Max(HudHeight + 20, (ClientSize.Height - menuHeight) / 2);

            btnStartGame.Location = new Point(centerX, menuTop);
            trackBarSpeed.Location = new Point(centerX, btnStartGame.Bottom + 24);
            chkWrapWalls.Location = new Point(centerX, trackBarSpeed.Bottom + 8);
            chkProgressiveSpeed.Location = new Point(centerX, chkWrapWalls.Bottom + 6);
            LayoutHudControls();
        }

        private void ConfigureHudLabels()
        {
            if (lblScore == null || lblHighScore == null || lblSpeed == null || lblStatus == null)
            {
                return;
            }

            Label[] labels = { lblScore, lblHighScore, lblSpeed, lblStatus };
            foreach (Label label in labels)
            {
                label.AutoSize = false;
                label.AutoEllipsis = true;
                label.ForeColor = HudTextColor;
                label.BackColor = HudBackColor;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Height = HudControlHeight;
            }
        }

        private void ConfigureButtons()
        {
            primaryButtonFont = new Font(Font.FontFamily, 11f, FontStyle.Bold);
            secondaryButtonFont = new Font(Font.FontFamily, 9f, FontStyle.Bold);
            ConfigureButton(btnStartGame, true);
            ConfigureButton(btnPause, false);
            ConfigureCheckBox(chkWrapWalls);
            ConfigureCheckBox(chkProgressiveSpeed);
        }

        private void ConfigureButton(Button button, bool isPrimary)
        {
            if (button == null)
            {
                return;
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = isPrimary ? SnakeBodyColor : Color.FromArgb(35, 48, 53);
            button.ForeColor = isPrimary ? Color.FromArgb(8, 22, 14) : HudTextColor;
            button.Font = isPrimary ? primaryButtonFont : secondaryButtonFont;
        }

        private void ConfigureCheckBox(CheckBox checkBox)
        {
            if (checkBox == null)
            {
                return;
            }

            checkBox.ForeColor = HudTextColor;
            checkBox.BackColor = WindowBackColor;
            checkBox.FlatStyle = FlatStyle.Flat;
        }

        private void LayoutHudControls()
        {
            if (lblScore == null || lblHighScore == null || lblSpeed == null || lblStatus == null || btnPause == null)
            {
                return;
            }

            int pauseX = Math.Max(HudPadding, ClientSize.Width - btnPause.Width - HudPadding);
            btnPause.Location = new Point(pauseX, HudControlTop);

            int availableWidth = Math.Max(160, pauseX - HudPadding - HudGap);
            int columnWidth = Math.Max(72, (availableWidth - HudGap * 3) / 4);

            int x = HudPadding;
            SetHudLabelBounds(lblScore, x, columnWidth);
            x += columnWidth + HudGap;
            SetHudLabelBounds(lblHighScore, x, columnWidth);
            x += columnWidth + HudGap;
            SetHudLabelBounds(lblSpeed, x, columnWidth);
            x += columnWidth + HudGap;

            int statusWidth = Math.Max(72, pauseX - x - HudGap);
            SetHudLabelBounds(lblStatus, x, statusWidth);
        }

        private void SetHudLabelBounds(Label label, int x, int width)
        {
            label.SetBounds(x, HudControlTop, width, HudControlHeight);
        }

        private void UpdateHud()
        {
            lblScore.Text = "Score: " + game.Score;
            lblHighScore.Text = "Best: " + highScore;
            lblSpeed.Text = "Speed: " + selectedSpeed + (useProgressiveSpeed ? "+" : string.Empty);
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

        private void LoadPlayerSettings()
        {
            suppressPlayerSettingSave = true;
            try
            {
                trackBarSpeed.Value = ClampSpeed(SnakeGame.Properties.Settings.Default.Speed);
                chkWrapWalls.Checked = SnakeGame.Properties.Settings.Default.WrapWalls;
                chkProgressiveSpeed.Checked = SnakeGame.Properties.Settings.Default.ProgressiveSpeed;
            }
            catch
            {
                trackBarSpeed.Value = ClampSpeed(DefaultSpeed);
                chkWrapWalls.Checked = DefaultWrapWalls;
                chkProgressiveSpeed.Checked = DefaultProgressiveSpeed;
            }
            finally
            {
                suppressPlayerSettingSave = false;
            }
        }

        private int ClampSpeed(int speed)
        {
            return Math.Max(trackBarSpeed.Minimum, Math.Min(trackBarSpeed.Maximum, speed));
        }

        private void SavePlayerSettings()
        {
            if (suppressPlayerSettingSave)
            {
                return;
            }

            try
            {
                SnakeGame.Properties.Settings.Default.Speed = trackBarSpeed.Value;
                SnakeGame.Properties.Settings.Default.WrapWalls = chkWrapWalls.Checked;
                SnakeGame.Properties.Settings.Default.ProgressiveSpeed = chkProgressiveSpeed.Checked;
                SnakeGame.Properties.Settings.Default.Save();
            }
            catch
            {
                // Player preference persistence should never interrupt gameplay.
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
            Invalidate();
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
            chkWrapWalls.Visible = true;
            chkProgressiveSpeed.Visible = true;
            UpdateHud();
            Invalidate();
        }

        private BoundaryMode GetSelectedBoundaryMode()
        {
            return chkWrapWalls.Checked ? BoundaryMode.Wrap : BoundaryMode.SolidWalls;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawChrome(e.Graphics);

            if (game.Status == GameStatus.Ready && game.Snake.Count == 0)
            {
                DrawOverlay(
                    e.Graphics,
                    "Snake Game",
                    "Press Enter or click Start Game",
                    "Arrow keys or WASD to move. Space pauses.");
                return;
            }

            if (game.IsFinished)
            {
                DrawFinishedState(e.Graphics);
                return;
            }

            DrawFood(e.Graphics);
            DrawSnake(e.Graphics);

            if (game.Status == GameStatus.Paused)
            {
                DrawOverlay(e.Graphics, "Paused", "Press Space or Resume", "Your run is waiting.");
            }
        }

        private void DrawChrome(Graphics canvas)
        {
            using (Brush hudBrush = new SolidBrush(HudBackColor))
            using (Brush boardBrush = new SolidBrush(BoardBackColor))
            {
                canvas.FillRectangle(hudBrush, new Rectangle(0, 0, ClientSize.Width, HudHeight));
                canvas.FillRectangle(boardBrush, GetBoardBounds());
            }

            DrawGrid(canvas);
        }

        private Rectangle GetBoardBounds()
        {
            return new Rectangle(0, HudHeight, ClientSize.Width, Math.Max(0, ClientSize.Height - HudHeight));
        }

        private void DrawGrid(Graphics canvas)
        {
            Rectangle board = GetBoardBounds();
            using (Pen gridPen = new Pen(GridColor, 1))
            {
                for (int x = 0; x <= board.Width; x += CellSize)
                {
                    canvas.DrawLine(gridPen, x, board.Top, x, board.Bottom);
                }

                for (int y = board.Top; y <= board.Bottom; y += CellSize)
                {
                    canvas.DrawLine(gridPen, board.Left, y, board.Right, y);
                }
            }
        }

        private void DrawFood(Graphics canvas)
        {
            GridCell food = game.Food;
            Rectangle foodRect = GetCellBounds(food);
            foodRect.Inflate(-2, -2);

            using (Brush foodBrush = new SolidBrush(FoodColor))
            using (Brush highlightBrush = new SolidBrush(FoodHighlightColor))
            {
                canvas.FillEllipse(foodBrush, foodRect);

                Rectangle highlight = new Rectangle(foodRect.Left + 4, foodRect.Top + 3, 5, 5);
                canvas.FillEllipse(highlightBrush, highlight);
            }
        }

        private void DrawSnake(Graphics canvas)
        {
            for (int i = 0; i < game.Snake.Count; i++)
            {
                GridCell part = game.Snake[i];
                DrawSnakePart(canvas, part, i == 0);
            }
        }

        private void DrawSnakePart(Graphics canvas, GridCell part, bool isHead)
        {
            Rectangle partRect = GetCellBounds(part);
            partRect.Inflate(-1, -1);

            Rectangle shadowRect = partRect;
            shadowRect.Offset(1, 1);

            using (GraphicsPath shadowPath = CreateRoundedRectangle(shadowRect, 4))
            using (GraphicsPath partPath = CreateRoundedRectangle(partRect, 4))
            using (Brush shadowBrush = new SolidBrush(SnakeShadowColor))
            using (Brush partBrush = new SolidBrush(isHead ? SnakeHeadColor : SnakeBodyColor))
            {
                canvas.FillPath(shadowBrush, shadowPath);
                canvas.FillPath(partBrush, partPath);
            }
        }

        private Rectangle GetCellBounds(GridCell cell)
        {
            return new Rectangle(cell.X * CellSize, GetCanvasY(cell.Y), CellSize, CellSize);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void DrawFinishedState(Graphics canvas)
        {
            string title = game.Status == GameStatus.Won ? "You Win" : "Game Over";
            string subtitle = "Score " + game.Score + "  |  Best " + highScore;

            DrawOverlay(canvas, title, subtitle, "Press Enter or click Restart Game");
        }

        private void DrawOverlay(Graphics canvas, string title, string subtitle, string hint)
        {
            Rectangle board = GetBoardBounds();
            if (board.Width <= 0 || board.Height <= 0)
            {
                return;
            }

            using (Brush overlayBrush = new SolidBrush(OverlayColor))
            using (Brush titleBrush = new SolidBrush(OverlayTitleColor))
            using (Brush textBrush = new SolidBrush(OverlayTextColor))
            using (StringFormat centeredFormat = new StringFormat())
            {
                centeredFormat.Alignment = StringAlignment.Center;
                centeredFormat.LineAlignment = StringAlignment.Center;

                canvas.FillRectangle(overlayBrush, board);

                int centerY = board.Top + board.Height / 2;
                Rectangle titleBounds = new Rectangle(board.Left + 20, centerY - 64, board.Width - 40, 44);
                Rectangle subtitleBounds = new Rectangle(board.Left + 20, centerY - 16, board.Width - 40, 28);
                Rectangle hintBounds = new Rectangle(board.Left + 20, centerY + 20, board.Width - 40, 26);

                canvas.DrawString(title, overlayTitleFont, titleBrush, titleBounds, centeredFormat);
                canvas.DrawString(subtitle, overlayTextFont, textBrush, subtitleBounds, centeredFormat);
                canvas.DrawString(hint, overlayTextFont, textBrush, hintBounds, centeredFormat);
            }
        }

        private void ConfigureOverlayFonts()
        {
            overlayTitleFont = new Font(Font.FontFamily, 24f, FontStyle.Bold);
            overlayTextFont = new Font(Font.FontFamily, 10f, FontStyle.Regular);
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
            ApplySpeedSetting();
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
            SavePlayerSettings();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            TogglePause();
            Focus();
        }

        private void chkWrapWalls_CheckedChanged(object sender, EventArgs e)
        {
            SavePlayerSettings();
            Focus();
        }

        private void chkProgressiveSpeed_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSettingsFromUI();
            UpdateHud();
            SavePlayerSettings();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (overlayTitleFont != null)
            {
                overlayTitleFont.Dispose();
            }

            if (overlayTextFont != null)
            {
                overlayTextFont.Dispose();
            }

            if (primaryButtonFont != null)
            {
                primaryButtonFont.Dispose();
            }

            if (secondaryButtonFont != null)
            {
                secondaryButtonFont.Dispose();
            }

            base.OnFormClosed(e);
        }
    }
}
