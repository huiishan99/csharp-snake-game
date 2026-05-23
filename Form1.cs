using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int CellSize = 16;
        private const int HudHeight = 48;
        private const int HudPadding = 12;
        private const int HudGap = 10;
        private const int HudControlTop = 10;
        private const int HudControlHeight = 28;
        private const int StartPanelWidth = 360;
        private const int StartPanelHeight = 286;
        private const int StartPanelRadius = 8;
        private const int StartPanelPadding = 24;
        private const int StartPanelGap = 8;
        private const int SpeedButtonWidth = 44;
        private const string UiFontFamily = "Segoe UI";
        private const bool DefaultWrapWalls = true;
        private const bool DefaultObstacles = false;
        private static readonly Color WindowBackColor = Color.FromArgb(18, 24, 27);
        private static readonly Color BoardBackColor = Color.FromArgb(25, 35, 39);
        private static readonly Color GridColor = Color.FromArgb(34, 48, 52);
        private static readonly Color HudBackColor = Color.FromArgb(14, 19, 22);
        private static readonly Color HudPillBackColor = Color.FromArgb(31, 43, 47);
        private static readonly Color HudDividerColor = Color.FromArgb(50, 70, 75);
        private static readonly Color HudTextColor = Color.FromArgb(225, 239, 235);
        private static readonly Color SnakeHeadColor = Color.FromArgb(180, 255, 190);
        private static readonly Color SnakeBodyColor = Color.FromArgb(80, 205, 132);
        private static readonly Color SnakeShadowColor = Color.FromArgb(42, 116, 82);
        private static readonly Color FoodColor = Color.FromArgb(255, 94, 94);
        private static readonly Color FoodHighlightColor = Color.FromArgb(255, 176, 128);
        private static readonly Color ObstacleColor = Color.FromArgb(103, 121, 126);
        private static readonly Color ObstacleHighlightColor = Color.FromArgb(140, 161, 166);
        private static readonly Color SolidWallColor = Color.FromArgb(235, 93, 93);
        private static readonly Color StartPanelBackColor = Color.FromArgb(28, 39, 43);
        private static readonly Color StartPanelBorderColor = Color.FromArgb(74, 98, 102);
        private static readonly Color StartPanelMutedTextColor = Color.FromArgb(156, 184, 178);
        private static readonly Color ToggleBackColor = Color.FromArgb(37, 52, 57);
        private static readonly Color ToggleBorderColor = Color.FromArgb(76, 99, 104);
        private static readonly Color ToggleActiveBackColor = Color.FromArgb(90, 220, 145);
        private static readonly Color ToggleActiveTextColor = Color.FromArgb(8, 24, 15);
        private static readonly Color SpeedStepInactiveBackColor = Color.FromArgb(25, 34, 38);
        private static readonly Color SpeedStepInactiveTextColor = Color.FromArgb(104, 126, 128);
        private static readonly Color StatusReadyColor = Color.FromArgb(68, 92, 100);
        private static readonly Color StatusPlayingColor = Color.FromArgb(66, 166, 108);
        private static readonly Color StatusPausedColor = Color.FromArgb(205, 166, 74);
        private static readonly Color StatusFinishedColor = Color.FromArgb(206, 83, 83);
        private static readonly Color StatusTextDarkColor = Color.FromArgb(8, 24, 15);
        private static readonly Color OverlayColor = Color.FromArgb(190, 9, 15, 18);
        private static readonly Color OverlayTitleColor = Color.FromArgb(234, 255, 238);
        private static readonly Color OverlayTextColor = Color.FromArgb(184, 207, 200);

        private readonly SnakeGameEngine game = new SnakeGameEngine();
        private int highScore = 0;
        private int selectedSpeed = GameSpeed.DefaultSpeed;
        private bool useProgressiveSpeed = GameSpeed.DefaultProgressiveSpeed;
        private bool useObstacles = DefaultObstacles;
        private bool suppressPlayerSettingSave;
        private Font hudFont;
        private Font primaryButtonFont;
        private Font secondaryButtonFont;
        private Font startTitleFont;
        private Font startHintFont;
        private Font toggleFont;
        private Font overlayTitleFont;
        private Font overlayTextFont;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            KeyPreview = true;
            BackColor = WindowBackColor;

            ConfigureFonts();
            ConfigureHudLabels();
            ConfigureStartPanel();
            ConfigureButtons();
            ConfigureSetupLabels();
            ConfigureOverlayFonts();
            LoadHighScore();
            LoadPlayerSettings();
            UpdateSettingsFromUI();
            UpdateHud();
            LayoutControls();
        }

        private void StartGame()
        {
            SetStartMenuVisible(false);
            btnPause.Visible = true;
            btnPause.Text = "Pause";

            UpdateSettingsFromUI();
            SavePlayerSettings();
            game.StartNew(GetMaxGridX(), GetMaxGridY(), GetSelectedBoundaryMode(), useObstacles);
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
            useProgressiveSpeed = chkProgressiveSpeed.Checked;
            useObstacles = chkObstacles.Checked;
            UpdateToggleStyles();
            ApplySpeedSetting();
        }

        private void ApplySpeedSetting()
        {
            timer1.Interval = GameSpeed.GetTimerInterval(selectedSpeed, game.Score, useProgressiveSpeed);
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
            if (pnlStartMenu == null || btnStartGame == null || btnSpeedDown == null || btnSpeedUp == null || lblStartTitle == null || lblStartHint == null || lblStartSpeed == null || btnPause == null || chkWrapWalls == null || chkProgressiveSpeed == null || chkObstacles == null)
            {
                return;
            }

            int panelWidth = Math.Max(280, Math.Min(StartPanelWidth, ClientSize.Width - 40));
            int panelHeight = StartPanelHeight;
            int panelX = Math.Max(0, (ClientSize.Width - panelWidth) / 2);
            int boardHeight = Math.Max(0, ClientSize.Height - HudHeight);
            int panelY = HudHeight + Math.Max(12, (boardHeight - panelHeight) / 2);
            pnlStartMenu.SetBounds(panelX, panelY, panelWidth, panelHeight);
            UpdateStartPanelRegion();

            int contentWidth = Math.Max(160, panelWidth - StartPanelPadding * 2);
            int x = StartPanelPadding;
            int toggleWidth = Math.Max(80, (contentWidth - StartPanelGap) / 2);

            lblStartTitle.SetBounds(x, 18, contentWidth, 34);
            lblStartHint.SetBounds(x, 52, contentWidth, 22);
            btnStartGame.SetBounds(x, 80, contentWidth, 44);
            btnSpeedDown.SetBounds(x, 138, SpeedButtonWidth, 38);
            lblStartSpeed.SetBounds(x + SpeedButtonWidth + StartPanelGap, 138, contentWidth - SpeedButtonWidth * 2 - StartPanelGap * 2, 38);
            btnSpeedUp.SetBounds(x + contentWidth - SpeedButtonWidth, 138, SpeedButtonWidth, 38);
            chkWrapWalls.SetBounds(x, 190, toggleWidth, 34);
            chkProgressiveSpeed.SetBounds(x + toggleWidth + StartPanelGap, 190, toggleWidth, 34);
            chkObstacles.SetBounds(x, 232, contentWidth, 34);
            LayoutHudControls();
        }

        private void ConfigureFonts()
        {
            hudFont = CreateUiFont(9f, FontStyle.Regular);
            primaryButtonFont = CreateUiFont(12f, FontStyle.Bold);
            secondaryButtonFont = CreateUiFont(9.5f, FontStyle.Bold);
            startTitleFont = CreateUiFont(22f, FontStyle.Bold);
            startHintFont = CreateUiFont(9.5f, FontStyle.Regular);
            toggleFont = CreateUiFont(9.25f, FontStyle.Bold);
        }

        private Font CreateUiFont(float size, FontStyle style)
        {
            return new Font(UiFontFamily, size, style, GraphicsUnit.Point);
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
                label.BackColor = HudPillBackColor;
                label.Font = hudFont;
                label.Padding = new Padding(8, 0, 8, 0);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Height = HudControlHeight;
            }
        }

        private void ConfigureStartPanel()
        {
            if (pnlStartMenu == null)
            {
                return;
            }

            pnlStartMenu.BackColor = StartPanelBackColor;
        }

        private void ConfigureSetupLabels()
        {
            if (lblStartTitle == null || lblStartHint == null || lblStartSpeed == null)
            {
                return;
            }

            lblStartTitle.AutoSize = false;
            lblStartTitle.AutoEllipsis = true;
            lblStartTitle.ForeColor = OverlayTitleColor;
            lblStartTitle.BackColor = StartPanelBackColor;
            lblStartTitle.Font = startTitleFont;
            lblStartTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblStartHint.AutoSize = false;
            lblStartHint.AutoEllipsis = true;
            lblStartHint.ForeColor = StartPanelMutedTextColor;
            lblStartHint.BackColor = StartPanelBackColor;
            lblStartHint.Font = startHintFont;
            lblStartHint.TextAlign = ContentAlignment.MiddleCenter;

            lblStartSpeed.AutoSize = false;
            lblStartSpeed.AutoEllipsis = true;
            lblStartSpeed.ForeColor = OverlayTextColor;
            lblStartSpeed.BackColor = StartPanelBackColor;
            lblStartSpeed.Font = secondaryButtonFont;
            lblStartSpeed.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void ConfigureButtons()
        {
            ConfigureButton(btnStartGame, true);
            ConfigureButton(btnPause, false);
            ConfigureButton(btnSpeedDown, false);
            ConfigureButton(btnSpeedUp, false);
            ConfigureCheckBox(chkWrapWalls);
            ConfigureCheckBox(chkProgressiveSpeed);
            ConfigureCheckBox(chkObstacles);
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
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.MouseOverBackColor = isPrimary ? SnakeHeadColor : Color.FromArgb(47, 65, 70);
            button.FlatAppearance.MouseDownBackColor = isPrimary ? SnakeBodyColor : Color.FromArgb(31, 43, 48);
        }

        private void ConfigureCheckBox(CheckBox checkBox)
        {
            if (checkBox == null)
            {
                return;
            }

            checkBox.Appearance = Appearance.Button;
            checkBox.AutoSize = false;
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.FlatAppearance.BorderSize = 1;
            checkBox.Font = toggleFont;
            checkBox.TextAlign = ContentAlignment.MiddleCenter;
            checkBox.UseVisualStyleBackColor = false;
            StyleToggle(checkBox);
        }

        private void UpdateToggleStyles()
        {
            StyleToggle(chkWrapWalls);
            StyleToggle(chkProgressiveSpeed);
            StyleToggle(chkObstacles);
        }

        private void StyleToggle(CheckBox checkBox)
        {
            if (checkBox == null)
            {
                return;
            }

            bool isChecked = checkBox.Checked;
            checkBox.BackColor = isChecked ? ToggleActiveBackColor : ToggleBackColor;
            checkBox.ForeColor = isChecked ? ToggleActiveTextColor : HudTextColor;
            checkBox.FlatAppearance.BorderColor = isChecked ? ToggleActiveBackColor : ToggleBorderColor;
            checkBox.FlatAppearance.CheckedBackColor = ToggleActiveBackColor;
            checkBox.FlatAppearance.MouseOverBackColor = isChecked ? SnakeHeadColor : Color.FromArgb(47, 65, 70);
            checkBox.FlatAppearance.MouseDownBackColor = isChecked ? SnakeBodyColor : Color.FromArgb(31, 43, 48);
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
            lblScore.Text = "Score " + game.Score;
            lblHighScore.Text = "Best " + highScore;
            lblSpeed.Text = "Speed " + GameSpeed.GetDisplayValue(selectedSpeed, useProgressiveSpeed);
            lblStatus.Text = GetStatusText();
            StyleStatusLabel();
            UpdateStartMenuText();
        }

        private void StyleStatusLabel()
        {
            if (lblStatus == null)
            {
                return;
            }

            Color statusColor;
            Color textColor = StatusTextDarkColor;
            switch (game.Status)
            {
                case GameStatus.Playing:
                    statusColor = StatusPlayingColor;
                    break;
                case GameStatus.Paused:
                    statusColor = StatusPausedColor;
                    break;
                case GameStatus.GameOver:
                case GameStatus.Won:
                    statusColor = StatusFinishedColor;
                    break;
                default:
                    statusColor = StatusReadyColor;
                    textColor = HudTextColor;
                    break;
            }

            lblStatus.BackColor = statusColor;
            lblStatus.ForeColor = textColor;
        }

        private void UpdateStartMenuText()
        {
            if (lblStartTitle == null || lblStartHint == null || lblStartSpeed == null || btnStartGame == null || btnSpeedDown == null || btnSpeedUp == null)
            {
                return;
            }

            if (game.Status == GameStatus.GameOver || game.Status == GameStatus.Won)
            {
                lblStartTitle.Text = game.Status == GameStatus.Won ? "You Win" : "Game Over";
                lblStartHint.Text = "Score " + game.Score + "  |  Best " + highScore;
                btnStartGame.Text = "Restart";
            }
            else
            {
                lblStartTitle.Text = "Snake Game";
                lblStartHint.Text = "Choose your run";
                btnStartGame.Text = "Start";
            }

            lblStartSpeed.Text = "Speed " + GameSpeed.GetDisplayValue(selectedSpeed, useProgressiveSpeed);
            StyleSpeedButton(btnSpeedDown, selectedSpeed > GameSpeed.MinimumSpeed);
            StyleSpeedButton(btnSpeedUp, selectedSpeed < GameSpeed.MaximumSpeed);
        }

        private void StyleSpeedButton(Button button, bool canChange)
        {
            if (button == null)
            {
                return;
            }

            button.BackColor = canChange ? Color.FromArgb(35, 48, 53) : SpeedStepInactiveBackColor;
            button.ForeColor = canChange ? HudTextColor : SpeedStepInactiveTextColor;
        }

        private void SetStartMenuVisible(bool visible)
        {
            if (pnlStartMenu != null)
            {
                pnlStartMenu.Visible = visible;
            }
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
                selectedSpeed = ClampSpeed(SnakeGame.Properties.Settings.Default.Speed);
                chkWrapWalls.Checked = SnakeGame.Properties.Settings.Default.WrapWalls;
                chkProgressiveSpeed.Checked = SnakeGame.Properties.Settings.Default.ProgressiveSpeed;
                chkObstacles.Checked = SnakeGame.Properties.Settings.Default.Obstacles;
            }
            catch
            {
                selectedSpeed = ClampSpeed(GameSpeed.DefaultSpeed);
                chkWrapWalls.Checked = DefaultWrapWalls;
                chkProgressiveSpeed.Checked = GameSpeed.DefaultProgressiveSpeed;
                chkObstacles.Checked = DefaultObstacles;
            }
            finally
            {
                suppressPlayerSettingSave = false;
            }
        }

        private int ClampSpeed(int speed)
        {
            return GameSpeed.ClampSpeed(speed);
        }

        private void SavePlayerSettings()
        {
            if (suppressPlayerSettingSave)
            {
                return;
            }

            try
            {
                SnakeGame.Properties.Settings.Default.Speed = selectedSpeed;
                SnakeGame.Properties.Settings.Default.WrapWalls = chkWrapWalls.Checked;
                SnakeGame.Properties.Settings.Default.ProgressiveSpeed = chkProgressiveSpeed.Checked;
                SnakeGame.Properties.Settings.Default.Obstacles = chkObstacles.Checked;
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
            SetStartMenuVisible(true);
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

            if ((game.Status == GameStatus.Ready && game.Snake.Count == 0) || game.IsFinished)
            {
                DrawStartBackdrop(e.Graphics);
                return;
            }

            DrawObstacles(e.Graphics);
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

            using (Pen dividerPen = new Pen(HudDividerColor, 1))
            {
                canvas.DrawLine(dividerPen, 0, HudHeight - 1, ClientSize.Width, HudHeight - 1);
            }

            DrawGrid(canvas);
            DrawBoundaryIndicator(canvas);
        }

        private void DrawStartBackdrop(Graphics canvas)
        {
            Rectangle board = GetBoardBounds();
            if (board.Width <= 0 || board.Height <= 0)
            {
                return;
            }

            using (Brush overlayBrush = new SolidBrush(OverlayColor))
            {
                canvas.FillRectangle(overlayBrush, board);
            }

            DrawBoundaryIndicator(canvas);
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

        private void DrawBoundaryIndicator(Graphics canvas)
        {
            if (!ShouldDrawSolidWallBorder())
            {
                return;
            }

            Rectangle board = GetBoardBounds();
            if (board.Width <= 3 || board.Height <= 3)
            {
                return;
            }

            Rectangle border = new Rectangle(board.Left + 1, board.Top + 1, board.Width - 3, board.Height - 3);
            using (Pen wallPen = new Pen(SolidWallColor, 3))
            {
                wallPen.Alignment = PenAlignment.Inset;
                canvas.DrawRectangle(wallPen, border);
            }
        }

        private bool ShouldDrawSolidWallBorder()
        {
            if (game.Status == GameStatus.Playing || game.Status == GameStatus.Paused)
            {
                return game.CurrentBoundaryMode == BoundaryMode.SolidWalls;
            }

            if (chkWrapWalls != null)
            {
                return !chkWrapWalls.Checked;
            }

            return false;
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

        private void DrawObstacles(Graphics canvas)
        {
            foreach (GridCell obstacle in game.Obstacles)
            {
                Rectangle obstacleRect = GetCellBounds(obstacle);
                obstacleRect.Inflate(-2, -2);

                Rectangle highlightRect = new Rectangle(obstacleRect.Left + 3, obstacleRect.Top + 3, Math.Max(2, obstacleRect.Width / 3), 3);
                using (GraphicsPath obstaclePath = CreateRoundedRectangle(obstacleRect, 3))
                using (Brush obstacleBrush = new SolidBrush(ObstacleColor))
                using (Brush highlightBrush = new SolidBrush(ObstacleHighlightColor))
                {
                    canvas.FillPath(obstacleBrush, obstaclePath);
                    canvas.FillRectangle(highlightBrush, highlightRect);
                }
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
                DrawBoundaryIndicator(canvas);
            }
        }

        private void ConfigureOverlayFonts()
        {
            overlayTitleFont = CreateUiFont(24f, FontStyle.Bold);
            overlayTextFont = CreateUiFont(10f, FontStyle.Regular);
        }

        private void pnlStartMenu_Paint(object sender, PaintEventArgs e)
        {
            if (pnlStartMenu.Width <= 1 || pnlStartMenu.Height <= 1)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle border = new Rectangle(0, 0, pnlStartMenu.Width - 1, pnlStartMenu.Height - 1);
            using (GraphicsPath borderPath = CreateRoundedRectangle(border, StartPanelRadius))
            using (Pen borderPen = new Pen(StartPanelBorderColor, 1))
            {
                e.Graphics.DrawPath(borderPen, borderPath);
            }
        }

        private void UpdateStartPanelRegion()
        {
            if (pnlStartMenu.Width <= 0 || pnlStartMenu.Height <= 0)
            {
                return;
            }

            Region oldRegion = pnlStartMenu.Region;
            Rectangle bounds = new Rectangle(0, 0, pnlStartMenu.Width, pnlStartMenu.Height);
            using (GraphicsPath panelPath = CreateRoundedRectangle(bounds, StartPanelRadius))
            {
                pnlStartMenu.Region = new Region(panelPath);
            }

            if (oldRegion != null)
            {
                oldRegion.Dispose();
            }
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

        private void btnSpeedDown_Click(object sender, EventArgs e)
        {
            ChangeSpeed(-1);
        }

        private void btnSpeedUp_Click(object sender, EventArgs e)
        {
            ChangeSpeed(1);
        }

        private void ChangeSpeed(int delta)
        {
            int nextSpeed = ClampSpeed(selectedSpeed + delta);
            if (nextSpeed == selectedSpeed)
            {
                return;
            }

            selectedSpeed = nextSpeed;
            UpdateSettingsFromUI();
            UpdateHud();
            SavePlayerSettings();
            Focus();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            TogglePause();
            Focus();
        }

        private void chkWrapWalls_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSettingsFromUI();
            UpdateHud();
            SavePlayerSettings();
            Invalidate();
            Focus();
        }

        private void chkProgressiveSpeed_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSettingsFromUI();
            UpdateHud();
            SavePlayerSettings();
            Focus();
        }

        private void chkObstacles_CheckedChanged(object sender, EventArgs e)
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
            if (hudFont != null)
            {
                hudFont.Dispose();
            }

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

            if (startTitleFont != null)
            {
                startTitleFont.Dispose();
            }

            if (startHintFont != null)
            {
                startHintFont.Dispose();
            }

            if (toggleFont != null)
            {
                toggleFont.Dispose();
            }

            if (pnlStartMenu != null && pnlStartMenu.Region != null)
            {
                pnlStartMenu.Region.Dispose();
            }

            base.OnFormClosed(e);
        }
    }
}
