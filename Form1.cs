using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private const int CellSize = 16;
        private const int HudHeight = 44;
        private const int HudPadding = 10;
        private const int HudGap = 8;
        private const int HudControlTop = 8;
        private const int HudControlHeight = 26;
        private const int HudPauseButtonWidth = 76;
        private const int StartPanelWidth = 340;
        private const int StartPanelHeight = 286;
        private const int StartPanelRadius = 6;
        private const int StartPanelPadding = 22;
        private const int StartPanelGap = 6;
        private const int SpeedButtonWidth = 36;
        private const int SpeedRowHeight = 30;
        private const int StartButtonHeight = 34;
        private const int ToggleHeight = 26;
        private const string UiFontFamily = "Segoe UI";
        private const string UiDisplayFontFamily = "Consolas";
        private const string UiHeadingFontFamily = "Segoe UI Semibold";
        private const bool DefaultWrapWalls = true;
        private const bool DefaultObstacles = false;
        private static readonly Color WindowBackColor = Color.FromArgb(18, 24, 27);
        private static readonly Color BoardTopColor = Color.FromArgb(28, 40, 44);
        private static readonly Color BoardBottomColor = Color.FromArgb(19, 28, 31);
        private static readonly Color BoardTextureColor = Color.FromArgb(22, 65, 82, 76);
        private static readonly Color GridColor = Color.FromArgb(34, 48, 52);
        private static readonly Color GridMajorColor = Color.FromArgb(43, 62, 66);
        private static readonly Color HudBackColor = Color.FromArgb(14, 19, 22);
        private static readonly Color HudPillBackColor = Color.FromArgb(31, 43, 47);
        private static readonly Color HudPillBorderColor = Color.FromArgb(44, 62, 66);
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
        private static readonly Color StartPanelBackColor = Color.FromArgb(22, 31, 34);
        private static readonly Color StartPanelTopColor = Color.FromArgb(25, 36, 39);
        private static readonly Color StartPanelBottomColor = Color.FromArgb(18, 27, 30);
        private static readonly Color StartPanelBorderColor = Color.FromArgb(64, 93, 91);
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
        private static readonly Color PauseOverlayColor = Color.FromArgb(172, 22, 22, 18);
        private static readonly Color WinBackdropColor = Color.FromArgb(154, 22, 58, 37);
        private static readonly Color GameOverBackdropColor = Color.FromArgb(166, 58, 22, 24);
        private static readonly Color OverlayTextColor = Color.FromArgb(184, 207, 200);
        private static readonly Color ScoreFlashBackColor = Color.FromArgb(217, 183, 83);
        private static readonly Color ScoreFlashTextColor = Color.FromArgb(30, 22, 9);
        private static readonly Color ScoreFlashBorderColor = Color.FromArgb(255, 219, 120);

        private readonly SnakeGameEngine game = new SnakeGameEngine();
        private readonly Timer attractTimer = new Timer();
        private readonly Timer feedbackTimer = new Timer();
        private int highScore = 0;
        private int selectedSpeed = GameSpeed.DefaultSpeed;
        private int attractFrame = 0;
        private int scoreFlashFrames = 0;
        private GridCell eatFeedbackCell;
        private bool hasEatFeedbackCell = false;
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

            ConfigureAttractTimer();
            ConfigureFeedbackTimer();
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
            UpdateAttractTimer();
        }

        private void StartGame()
        {
            SetStartMenuVisible(false);
            btnPause.Visible = true;
            btnPause.Text = "Pause";
            ClearEatFeedback();

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

            lblStartTitle.SetBounds(x, 14, contentWidth, 32);
            lblStartHint.SetBounds(x, 44, contentWidth, 20);
            btnStartGame.SetBounds(x, 132, contentWidth, StartButtonHeight);
            btnSpeedDown.SetBounds(x, 178, SpeedButtonWidth, SpeedRowHeight);
            lblStartSpeed.SetBounds(x + SpeedButtonWidth + StartPanelGap, 178, contentWidth - SpeedButtonWidth * 2 - StartPanelGap * 2, SpeedRowHeight);
            btnSpeedUp.SetBounds(x + contentWidth - SpeedButtonWidth, 178, SpeedButtonWidth, SpeedRowHeight);
            chkWrapWalls.SetBounds(x, 220, toggleWidth, ToggleHeight);
            chkProgressiveSpeed.SetBounds(x + toggleWidth + StartPanelGap, 220, toggleWidth, ToggleHeight);
            chkObstacles.SetBounds(x, 252, contentWidth, ToggleHeight);
            LayoutHudControls();
        }

        private void ConfigureAttractTimer()
        {
            attractTimer.Interval = 140;
            attractTimer.Tick += attractTimer_Tick;
        }

        private void ConfigureFeedbackTimer()
        {
            feedbackTimer.Interval = 34;
            feedbackTimer.Tick += feedbackTimer_Tick;
        }

        private void ConfigureFonts()
        {
            hudFont = CreateUiFont(UiFontFamily, 8.75f, FontStyle.Regular);
            primaryButtonFont = CreateUiFont(UiHeadingFontFamily, 10.5f, FontStyle.Regular);
            secondaryButtonFont = CreateUiFont(UiHeadingFontFamily, 8.75f, FontStyle.Regular);
            startTitleFont = CreateUiFont(UiDisplayFontFamily, 20f, FontStyle.Bold);
            startHintFont = CreateUiFont(UiFontFamily, 8.75f, FontStyle.Regular);
            toggleFont = CreateUiFont(UiHeadingFontFamily, 8.5f, FontStyle.Regular);
        }

        private Font CreateUiFont(string familyName, float size, FontStyle style)
        {
            try
            {
                return new Font(familyName, size, style, GraphicsUnit.Point);
            }
            catch
            {
                return new Font(FontFamily.GenericSansSerif, size, style, GraphicsUnit.Point);
            }
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
                label.Font = hudFont;
                label.Padding = new Padding(8, 0, 8, 0);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Height = HudControlHeight;

                ThemePillLabel pillLabel = label as ThemePillLabel;
                if (pillLabel != null)
                {
                    pillLabel.CornerRadius = 8;
                    pillLabel.PillBackColor = HudPillBackColor;
                    pillLabel.PillBorderColor = HudPillBorderColor;
                    pillLabel.PillTextColor = HudTextColor;
                }
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
            lblStartTitle.ForeColor = SnakeHeadColor;
            lblStartTitle.BackColor = Color.Transparent;
            lblStartTitle.Font = startTitleFont;
            lblStartTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblStartHint.AutoSize = false;
            lblStartHint.AutoEllipsis = true;
            lblStartHint.ForeColor = StartPanelMutedTextColor;
            lblStartHint.BackColor = Color.Transparent;
            lblStartHint.Font = startHintFont;
            lblStartHint.TextAlign = ContentAlignment.MiddleLeft;

            lblStartSpeed.AutoSize = false;
            lblStartSpeed.AutoEllipsis = true;
            lblStartSpeed.ForeColor = OverlayTextColor;
            lblStartSpeed.BackColor = Color.Transparent;
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
            button.BackColor = isPrimary ? Color.FromArgb(24, 70, 50) : Color.FromArgb(35, 48, 53);
            button.ForeColor = isPrimary ? SnakeHeadColor : HudTextColor;
            button.Font = isPrimary ? primaryButtonFont : secondaryButtonFont;
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.MouseOverBackColor = isPrimary ? Color.FromArgb(31, 88, 61) : Color.FromArgb(47, 65, 70);
            button.FlatAppearance.MouseDownBackColor = isPrimary ? Color.FromArgb(20, 59, 43) : Color.FromArgb(31, 43, 48);

            ThemeButton themeButton = button as ThemeButton;
            if (themeButton != null)
            {
                themeButton.CornerRadius = isPrimary ? 7 : 6;
                themeButton.NormalBackColor = isPrimary ? Color.FromArgb(24, 70, 50) : ToggleBackColor;
                themeButton.HoverBackColor = isPrimary ? Color.FromArgb(31, 88, 61) : Color.FromArgb(47, 65, 70);
                themeButton.PressedBackColor = isPrimary ? Color.FromArgb(20, 59, 43) : Color.FromArgb(31, 43, 48);
                themeButton.DisabledBackColor = SpeedStepInactiveBackColor;
                themeButton.NormalTextColor = isPrimary ? SnakeHeadColor : HudTextColor;
                themeButton.DisabledTextColor = SpeedStepInactiveTextColor;
                themeButton.BorderColor = isPrimary ? Color.FromArgb(87, 171, 112) : ToggleBorderColor;
            }
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

            ThemeToggle themeToggle = checkBox as ThemeToggle;
            if (themeToggle != null)
            {
                themeToggle.CornerRadius = 6;
                themeToggle.NormalBackColor = ToggleBackColor;
                themeToggle.CheckedBackColor = ToggleActiveBackColor;
                themeToggle.HoverBackColor = Color.FromArgb(47, 65, 70);
                themeToggle.PressedBackColor = Color.FromArgb(31, 43, 48);
                themeToggle.NormalTextColor = HudTextColor;
                themeToggle.CheckedTextColor = ToggleActiveTextColor;
                themeToggle.BorderColor = ToggleBorderColor;
                themeToggle.CheckedBorderColor = ToggleActiveBackColor;
            }

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

            ThemeToggle themeToggle = checkBox as ThemeToggle;
            if (themeToggle != null)
            {
                themeToggle.Invalidate();
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

            int pauseX = Math.Max(HudPadding, ClientSize.Width - HudPauseButtonWidth - HudPadding);
            btnPause.SetBounds(pauseX, HudControlTop, HudPauseButtonWidth, HudControlHeight);

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
            StyleScoreLabel();
            StyleStatusLabel();
            UpdateStartMenuText();
        }

        private void StyleScoreLabel()
        {
            if (lblScore == null)
            {
                return;
            }

            Color backColor = scoreFlashFrames > 0 ? ScoreFlashBackColor : HudPillBackColor;
            Color textColor = scoreFlashFrames > 0 ? ScoreFlashTextColor : HudTextColor;
            Color borderColor = scoreFlashFrames > 0 ? ScoreFlashBorderColor : HudPillBorderColor;

            ThemePillLabel scorePill = lblScore as ThemePillLabel;
            if (scorePill != null)
            {
                lblScore.BackColor = HudBackColor;
                lblScore.ForeColor = textColor;
                scorePill.PillBackColor = backColor;
                scorePill.PillBorderColor = borderColor;
                scorePill.PillTextColor = textColor;
                scorePill.Invalidate();
                return;
            }

            lblScore.BackColor = backColor;
            lblScore.ForeColor = textColor;
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

            ThemePillLabel statusPill = lblStatus as ThemePillLabel;
            if (statusPill != null)
            {
                lblStatus.BackColor = HudBackColor;
                lblStatus.ForeColor = textColor;
                statusPill.PillBackColor = statusColor;
                statusPill.PillBorderColor = ThemeButton.Mix(statusColor, HudBackColor, 36);
                statusPill.PillTextColor = textColor;
                statusPill.Invalidate();
                return;
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
                lblStartTitle.Text = game.Status == GameStatus.Won ? "GRID CLEAR" : "GAME OVER";
                lblStartHint.Text = "SCORE " + game.Score + " / BEST " + highScore;
                btnStartGame.Text = "TRY AGAIN";
            }
            else
            {
                lblStartTitle.Text = "SNAKE";
                lblStartHint.Text = "SURVIVE THE GRID";
                btnStartGame.Text = "START RUN";
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

            button.Enabled = canChange;
            ThemeButton themeButton = button as ThemeButton;
            if (themeButton != null)
            {
                themeButton.Invalidate();
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

            UpdateAttractTimer();
        }

        private void UpdateAttractTimer()
        {
            if (pnlStartMenu != null && pnlStartMenu.Visible && (game.Status == GameStatus.Ready || game.IsFinished))
            {
                attractTimer.Start();
                return;
            }

            attractTimer.Stop();
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

            if (game.Status == GameStatus.Ready && game.Snake.Count == 0)
            {
                DrawStartBackdrop(e.Graphics);
                return;
            }

            DrawObstacles(e.Graphics);
            DrawFood(e.Graphics);
            DrawSnake(e.Graphics);
            DrawEatFeedback(e.Graphics);

            if (game.IsFinished)
            {
                DrawStartBackdrop(e.Graphics);
                return;
            }

            if (game.Status == GameStatus.Paused)
            {
                DrawOverlay(e.Graphics, "Paused", "Press Space or Resume", "Your run is waiting.", PauseOverlayColor, StatusPausedColor);
            }
        }

        private void DrawChrome(Graphics canvas)
        {
            Rectangle hudBounds = new Rectangle(0, 0, ClientSize.Width, HudHeight);
            Rectangle board = GetBoardBounds();

            using (LinearGradientBrush hudBrush = new LinearGradientBrush(hudBounds, Color.FromArgb(17, 23, 26), HudBackColor, LinearGradientMode.Vertical))
            {
                canvas.FillRectangle(hudBrush, hudBounds);
            }

            if (board.Width > 0 && board.Height > 0)
            {
                using (LinearGradientBrush boardBrush = new LinearGradientBrush(board, BoardTopColor, BoardBottomColor, LinearGradientMode.Vertical))
                {
                    canvas.FillRectangle(boardBrush, board);
                }
            }

            DrawBoardTexture(canvas);
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

            Color overlayColor = OverlayColor;
            Color signalColor = StartPanelBorderColor;
            if (game.Status == GameStatus.Won)
            {
                overlayColor = WinBackdropColor;
                signalColor = SnakeHeadColor;
            }
            else if (game.Status == GameStatus.GameOver)
            {
                overlayColor = GameOverBackdropColor;
                signalColor = FoodColor;
            }

            using (Brush overlayBrush = new SolidBrush(overlayColor))
            {
                canvas.FillRectangle(overlayBrush, board);
            }

            if (game.IsFinished)
            {
                DrawBackdropSignal(canvas, board, signalColor);
            }

            DrawBoundaryIndicator(canvas);
        }

        private void DrawBackdropSignal(Graphics canvas, Rectangle board, Color signalColor)
        {
            using (Pen signalPen = new Pen(Color.FromArgb(80, signalColor), 1))
            using (Pen dimSignalPen = new Pen(Color.FromArgb(34, signalColor), 1))
            {
                for (int y = board.Top + 18; y < board.Bottom; y += 34)
                {
                    canvas.DrawLine(signalPen, board.Left, y, board.Right, y);
                    canvas.DrawLine(dimSignalPen, board.Left, y + 3, board.Right, y + 3);
                }
            }
        }

        private Rectangle GetBoardBounds()
        {
            return new Rectangle(0, HudHeight, ClientSize.Width, Math.Max(0, ClientSize.Height - HudHeight));
        }

        private void DrawGrid(Graphics canvas)
        {
            Rectangle board = GetBoardBounds();
            using (Pen gridPen = new Pen(GridColor, 1))
            using (Pen majorGridPen = new Pen(GridMajorColor, 1))
            {
                for (int x = 0; x <= board.Width; x += CellSize)
                {
                    Pen pen = (x / CellSize) % 4 == 0 ? majorGridPen : gridPen;
                    canvas.DrawLine(pen, x, board.Top, x, board.Bottom);
                }

                for (int y = board.Top; y <= board.Bottom; y += CellSize)
                {
                    Pen pen = ((y - board.Top) / CellSize) % 4 == 0 ? majorGridPen : gridPen;
                    canvas.DrawLine(pen, board.Left, y, board.Right, y);
                }
            }
        }

        private void DrawBoardTexture(Graphics canvas)
        {
            Rectangle board = GetBoardBounds();
            if (board.Width <= 0 || board.Height <= 0)
            {
                return;
            }

            using (Brush textureBrush = new SolidBrush(BoardTextureColor))
            {
                for (int y = board.Top; y < board.Bottom; y += CellSize)
                {
                    int row = (y - board.Top) / CellSize;
                    for (int x = board.Left; x < board.Right; x += CellSize)
                    {
                        int column = (x - board.Left) / CellSize;
                        if ((row + column) % 2 != 0)
                        {
                            continue;
                        }

                        canvas.FillRectangle(textureBrush, x + 1, y + 1, CellSize - 2, CellSize - 2);
                    }
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
            using (Pen stemPen = new Pen(Color.FromArgb(150, SnakeBodyColor), 2))
            {
                canvas.FillEllipse(foodBrush, foodRect);
                int stemTop = Math.Max(GetBoardBounds().Top + 1, foodRect.Top - 3);
                canvas.DrawLine(stemPen, foodRect.Left + foodRect.Width / 2, foodRect.Top + 1, foodRect.Left + foodRect.Width / 2 + 3, stemTop);

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
                using (Pen groovePen = new Pen(Color.FromArgb(95, 45, 56, 60), 1))
                {
                    canvas.FillPath(obstacleBrush, obstaclePath);
                    canvas.FillRectangle(highlightBrush, highlightRect);
                    canvas.DrawLine(groovePen, obstacleRect.Left + 3, obstacleRect.Bottom - 3, obstacleRect.Right - 3, obstacleRect.Top + 3);
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

            if (isHead)
            {
                DrawSnakeHeadDetail(canvas, partRect);
            }
        }

        private void DrawSnakeHeadDetail(Graphics canvas, Rectangle headRect)
        {
            Point firstEye;
            Point secondEye;
            int eyeSize = 2;

            switch (game.CurrentDirection)
            {
                case Direction.Up:
                    firstEye = new Point(headRect.Left + 4, headRect.Top + 4);
                    secondEye = new Point(headRect.Right - 6, headRect.Top + 4);
                    break;
                case Direction.Left:
                    firstEye = new Point(headRect.Left + 4, headRect.Top + 4);
                    secondEye = new Point(headRect.Left + 4, headRect.Bottom - 6);
                    break;
                case Direction.Right:
                    firstEye = new Point(headRect.Right - 6, headRect.Top + 4);
                    secondEye = new Point(headRect.Right - 6, headRect.Bottom - 6);
                    break;
                default:
                    firstEye = new Point(headRect.Left + 4, headRect.Bottom - 6);
                    secondEye = new Point(headRect.Right - 6, headRect.Bottom - 6);
                    break;
            }

            using (Brush eyeBrush = new SolidBrush(Color.FromArgb(38, 71, 45)))
            {
                canvas.FillRectangle(eyeBrush, firstEye.X, firstEye.Y, eyeSize, eyeSize);
                canvas.FillRectangle(eyeBrush, secondEye.X, secondEye.Y, eyeSize, eyeSize);
            }
        }

        private void DrawEatFeedback(Graphics canvas)
        {
            if (!hasEatFeedbackCell || scoreFlashFrames <= 0)
            {
                return;
            }

            Rectangle cellBounds = GetCellBounds(eatFeedbackCell);
            int age = 10 - scoreFlashFrames;
            int alpha = Math.Max(0, Math.Min(230, scoreFlashFrames * 23));
            Rectangle burstBounds = cellBounds;
            burstBounds.Inflate(2 + age * 2, 2 + age * 2);

            using (Pen burstPen = new Pen(Color.FromArgb(alpha, FoodHighlightColor), 2))
            {
                canvas.DrawEllipse(burstPen, burstBounds);

                Rectangle textBounds = new Rectangle(cellBounds.Left - 12, Math.Max(HudHeight, cellBounds.Top - 22 - age), 40, 18);
                TextRenderer.DrawText(
                    canvas,
                    "+10",
                    hudFont ?? Font,
                    textBounds,
                    Color.FromArgb(alpha, FoodHighlightColor),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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

        private void DrawOverlay(Graphics canvas, string title, string subtitle, string hint, Color overlayBackColor, Color titleColor)
        {
            Rectangle board = GetBoardBounds();
            if (board.Width <= 0 || board.Height <= 0)
            {
                return;
            }

            using (Brush overlayBrush = new SolidBrush(overlayBackColor))
            using (Brush titleBrush = new SolidBrush(titleColor))
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
            overlayTitleFont = CreateUiFont(UiHeadingFontFamily, 22f, FontStyle.Regular);
            overlayTextFont = CreateUiFont(UiFontFamily, 9.5f, FontStyle.Regular);
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
            using (LinearGradientBrush panelBrush = new LinearGradientBrush(border, StartPanelTopColor, StartPanelBottomColor, LinearGradientMode.Vertical))
            using (Pen borderPen = new Pen(StartPanelBorderColor, 1))
            {
                e.Graphics.FillPath(panelBrush, borderPath);
                DrawStartPanelFrame(e.Graphics, border);
                DrawStartPanelPreview(e.Graphics);
                e.Graphics.DrawPath(borderPen, borderPath);
            }
        }

        private void DrawStartPanelFrame(Graphics canvas, Rectangle border)
        {
            int markLength = 12;
            int inset = 8;

            using (Pen cornerPen = new Pen(Color.FromArgb(135, SnakeBodyColor), 1))
            using (Pen mutedPen = new Pen(Color.FromArgb(100, StartPanelBorderColor), 1))
            {
                canvas.DrawLine(cornerPen, border.Left + inset, border.Top + inset, border.Left + inset + markLength, border.Top + inset);
                canvas.DrawLine(cornerPen, border.Left + inset, border.Top + inset, border.Left + inset, border.Top + inset + markLength);
                canvas.DrawLine(cornerPen, border.Right - inset - markLength, border.Top + inset, border.Right - inset, border.Top + inset);
                canvas.DrawLine(cornerPen, border.Right - inset, border.Top + inset, border.Right - inset, border.Top + inset + markLength);
                canvas.DrawLine(mutedPen, StartPanelPadding, 66, pnlStartMenu.Width - StartPanelPadding, 66);
                TextRenderer.DrawText(
                    canvas,
                    "BEST " + highScore,
                    hudFont ?? Font,
                    new Rectangle(pnlStartMenu.Width - 98, 15, 76, 18),
                    Color.FromArgb(150, StartPanelMutedTextColor),
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private void DrawStartPanelPreview(Graphics canvas)
        {
            Rectangle preview = GetStartPreviewBounds();
            if (preview.Width <= 0 || preview.Height <= 0)
            {
                return;
            }

            using (Brush backBrush = new SolidBrush(Color.FromArgb(20, 27, 30)))
            using (Pen borderPen = new Pen(Color.FromArgb(72, 102, 98), 1))
            {
                canvas.FillRectangle(backBrush, preview);
                canvas.DrawRectangle(borderPen, preview);
            }

            DrawStartPreviewGrid(canvas, preview);
            DrawStartPreviewSnake(canvas, preview);
        }

        private Rectangle GetStartPreviewBounds()
        {
            return new Rectangle(StartPanelPadding, 74, Math.Max(0, pnlStartMenu.Width - StartPanelPadding * 2), 44);
        }

        private void DrawStartPreviewGrid(Graphics canvas, Rectangle preview)
        {
            using (Pen gridPen = new Pen(Color.FromArgb(45, GridColor), 1))
            {
                for (int x = preview.Left + 8; x < preview.Right; x += 8)
                {
                    canvas.DrawLine(gridPen, x, preview.Top + 1, x, preview.Bottom - 1);
                }

                for (int y = preview.Top + 8; y < preview.Bottom; y += 8)
                {
                    canvas.DrawLine(gridPen, preview.Left + 1, y, preview.Right - 1, y);
                }
            }
        }

        private void DrawStartPreviewSnake(Graphics canvas, Rectangle preview)
        {
            int cell = preview.Width < 270 ? 7 : 8;
            Point[] path =
            {
                new Point(2, 3), new Point(3, 3), new Point(4, 3), new Point(5, 3),
                new Point(6, 3), new Point(7, 3), new Point(8, 2), new Point(9, 2),
                new Point(10, 2), new Point(11, 2), new Point(12, 2), new Point(13, 2),
                new Point(14, 3), new Point(15, 3), new Point(16, 3), new Point(17, 3),
                new Point(18, 3), new Point(19, 2), new Point(20, 2), new Point(21, 2),
                new Point(22, 2), new Point(23, 2), new Point(24, 3), new Point(25, 3),
                new Point(26, 3), new Point(27, 3), new Point(28, 3), new Point(29, 2)
            };

            int usedWidth = 32 * cell;
            int usedHeight = 5 * cell;
            int originX = preview.Left + Math.Max(4, (preview.Width - usedWidth) / 2);
            int originY = preview.Top + Math.Max(2, (preview.Height - usedHeight) / 2);
            int headIndex = (attractFrame / 2) % path.Length;
            int foodIndex = (headIndex + 9) % path.Length;

            using (Brush shadowBrush = new SolidBrush(Color.FromArgb(90, SnakeShadowColor)))
            using (Brush bodyBrush = new SolidBrush(Color.FromArgb(190, SnakeBodyColor)))
            using (Brush headBrush = new SolidBrush(SnakeHeadColor))
            using (Brush foodBrush = new SolidBrush(FoodColor))
            using (Brush highlightBrush = new SolidBrush(FoodHighlightColor))
            {
                Point food = path[foodIndex];
                Rectangle foodRect = new Rectangle(originX + food.X * cell + 1, originY + food.Y * cell + 1, cell - 2, cell - 2);
                canvas.FillEllipse(foodBrush, foodRect);
                canvas.FillEllipse(highlightBrush, foodRect.Left + 2, foodRect.Top + 1, 2, 2);

                for (int i = 7; i >= 0; i--)
                {
                    int pathIndex = (headIndex - i + path.Length) % path.Length;
                    Point part = path[pathIndex];
                    Brush partBrush = i == 0 ? headBrush : bodyBrush;
                    Rectangle partRect = new Rectangle(originX + part.X * cell + 1, originY + part.Y * cell + 1, cell - 2, cell - 2);
                    Rectangle shadowRect = partRect;
                    shadowRect.Offset(1, 1);
                    canvas.FillRectangle(shadowBrush, shadowRect);
                    canvas.FillRectangle(partBrush, partRect);
                }
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

            if ((game.Status == GameStatus.Ready || game.IsFinished) && HandleStartMenuKey(e.KeyCode))
            {
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

        private bool HandleStartMenuKey(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left:
                case Keys.OemMinus:
                case Keys.Subtract:
                    ChangeSpeed(-1);
                    return true;
                case Keys.Right:
                case Keys.Oemplus:
                case Keys.Add:
                    ChangeSpeed(1);
                    return true;
                default:
                    return false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (game.Status != GameStatus.Playing)
            {
                return;
            }

            int scoreBeforeStep = game.Score;
            GridCell foodBeforeStep = game.Food;
            game.Step();
            if (game.Score > scoreBeforeStep)
            {
                TriggerEatFeedback(foodBeforeStep);
            }

            UpdateHighScore();
            ApplySpeedSetting();
            UpdateHud();
            Invalidate();
            FinishRoundIfNeeded();
        }

        private void TriggerEatFeedback(GridCell eatenCell)
        {
            eatFeedbackCell = eatenCell;
            hasEatFeedbackCell = true;
            scoreFlashFrames = 10;
            feedbackTimer.Start();
        }

        private void ClearEatFeedback()
        {
            scoreFlashFrames = 0;
            hasEatFeedbackCell = false;
            feedbackTimer.Stop();
            StyleScoreLabel();
        }

        private void attractTimer_Tick(object sender, EventArgs e)
        {
            if (pnlStartMenu == null || !pnlStartMenu.Visible)
            {
                attractTimer.Stop();
                return;
            }

            attractFrame = (attractFrame + 1) % 96;
            pnlStartMenu.Invalidate();
        }

        private void feedbackTimer_Tick(object sender, EventArgs e)
        {
            if (scoreFlashFrames > 0)
            {
                scoreFlashFrames--;
            }

            if (scoreFlashFrames == 0)
            {
                hasEatFeedbackCell = false;
                feedbackTimer.Stop();
            }

            StyleScoreLabel();
            Invalidate();
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
            attractTimer.Stop();
            attractTimer.Tick -= attractTimer_Tick;
            attractTimer.Dispose();

            feedbackTimer.Stop();
            feedbackTimer.Tick -= feedbackTimer_Tick;
            feedbackTimer.Dispose();

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
