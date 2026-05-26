using System;
using System.Drawing;
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
        private const int StartPanelWidth = 460;
        private const int StartPanelHeight = 430;
        private const int StartPanelRadius = 6;
        private const int StartPanelPadding = 24;
        private const int StartPanelGap = 8;
        private const int SpeedButtonWidth = 36;
        private const int SpeedRowHeight = 30;
        private const int StartButtonHeight = 34;
        private const int ToggleHeight = 26;
        private const int SetupLabelHeight = 16;
        private const int SetupInputHeight = 26;
        private const int ChallengeButtonWidth = 52;
        private const string UiFontFamily = "Segoe UI";
        private const string UiDisplayFontFamily = "Consolas";
        private const string UiHeadingFontFamily = "Segoe UI Semibold";
        private const bool DefaultWrapWalls = true;
        private const bool DefaultObstacles = false;
        private const bool DefaultSoundEnabled = true;
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
        private LeaderboardStore leaderboard = new LeaderboardStore();
        private int highScore = 0;
        private int selectedSpeed = GameSpeed.DefaultSpeed;
        private int attractFrame = 0;
        private int scoreFlashFrames = 0;
        private GridCell eatFeedbackCell;
        private bool hasEatFeedbackCell = false;
        private bool useProgressiveSpeed = GameSpeed.DefaultProgressiveSpeed;
        private bool useObstacles = DefaultObstacles;
        private bool useSound = DefaultSoundEnabled;
        private GameModePreset selectedModePreset = GamePresets.DefaultMode;
        private BoardSizePreset selectedBoardSizePreset = GamePresets.DefaultBoardSize;
        private VisualThemePreset selectedThemePreset = GamePresets.DefaultTheme;
        private string activeChallengeSeed = string.Empty;
        private bool hasRecordedFinishedRun;
        private bool suppressPlayerSettingSave;
        private Size unlockedMinimumSize;
        private Size unlockedMaximumSize;
        private bool unlockedMaximizeBox;
        private bool isWindowSizeLocked;
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
            unlockedMinimumSize = MinimumSize;
            unlockedMaximumSize = MaximumSize;
            unlockedMaximizeBox = MaximizeBox;

            ConfigureAttractTimer();
            ConfigureFeedbackTimer();
            ConfigureFonts();
            ConfigureHudLabels();
            ConfigureStartPanel();
            ConfigureButtons();
            ConfigureSetupLabels();
            ConfigurePresetControls();
            ConfigureOverlayFonts();
            LoadHighScore();
            LoadLeaderboard();
            LoadPlayerSettings();
            UpdateSettingsFromUI();
            UpdateHud();
            LayoutControls();
            UpdateAttractTimer();
        }

        private void StartGame()
        {
            ApplySelectedBoardSize();
            SetStartMenuVisible(false);
            btnPause.Visible = true;
            btnPause.Text = "Pause";
            ClearEatFeedback();
            hasRecordedFinishedRun = false;
            activeChallengeSeed = ChallengeSeed.Normalize(txtChallengeSeed.Text);
            LockWindowSizeForRun();

            UpdateSettingsFromUI();
            SavePlayerSettings();
            game.StartNew(GetMaxGridX(), GetMaxGridY(), GetSelectedBoundaryMode(), useObstacles, ChallengeSeed.ToRandomSeed(activeChallengeSeed));
            PlayRunStartSound();
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
            useSound = chkSound.Checked;
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
            if (pnlStartMenu == null || btnStartGame == null || btnSpeedDown == null || btnSpeedUp == null || lblStartTitle == null || lblStartHint == null || lblStartSpeed == null || btnPause == null || chkWrapWalls == null || chkProgressiveSpeed == null || chkObstacles == null || chkSound == null)
            {
                return;
            }

            int panelWidth = Math.Max(420, Math.Min(StartPanelWidth, ClientSize.Width - 40));
            int panelHeight = StartPanelHeight;
            int panelX = Math.Max(0, (ClientSize.Width - panelWidth) / 2);
            int boardHeight = Math.Max(0, ClientSize.Height - HudHeight);
            int panelY = HudHeight + Math.Max(12, (boardHeight - panelHeight) / 2);
            pnlStartMenu.SetBounds(panelX, panelY, panelWidth, panelHeight);
            UpdateStartPanelRegion();

            int contentWidth = Math.Max(160, panelWidth - StartPanelPadding * 2);
            int x = StartPanelPadding;
            int secondColumnX = x + (contentWidth + StartPanelGap) / 2;
            int columnWidth = Math.Max(120, (contentWidth - StartPanelGap) / 2);
            int toggleWidth = Math.Max(80, (contentWidth - StartPanelGap) / 2);

            lblStartTitle.SetBounds(x, 16, contentWidth, 32);
            lblStartHint.SetBounds(x, 48, contentWidth, 20);
            btnStartGame.SetBounds(x, 126, contentWidth, StartButtonHeight);
            LayoutSetupPair(lblMode, cmbMode, x, 172, columnWidth);
            LayoutSetupPair(lblBoardSize, cmbBoardSize, secondColumnX, 172, columnWidth);
            LayoutSetupPair(lblTheme, cmbTheme, x, 226, columnWidth);
            LayoutChallengeControls(secondColumnX, 226, columnWidth);
            btnSpeedDown.SetBounds(x, 282, SpeedButtonWidth, SpeedRowHeight);
            lblStartSpeed.SetBounds(x + SpeedButtonWidth + StartPanelGap, 282, contentWidth - SpeedButtonWidth * 2 - StartPanelGap * 2, SpeedRowHeight);
            btnSpeedUp.SetBounds(x + contentWidth - SpeedButtonWidth, 282, SpeedButtonWidth, SpeedRowHeight);
            chkWrapWalls.SetBounds(x, 320, toggleWidth, ToggleHeight);
            chkProgressiveSpeed.SetBounds(x + toggleWidth + StartPanelGap, 320, toggleWidth, ToggleHeight);
            chkObstacles.SetBounds(x, 352, toggleWidth, ToggleHeight);
            chkSound.SetBounds(x + toggleWidth + StartPanelGap, 352, toggleWidth, ToggleHeight);
            if (lblLeaderboard != null)
            {
                lblLeaderboard.SetBounds(x, 388, contentWidth, 28);
            }
            LayoutHudControls();
        }

        private void LayoutSetupPair(Label label, Control input, int x, int y, int width)
        {
            if (label == null || input == null)
            {
                return;
            }

            label.SetBounds(x, y, width, SetupLabelHeight);
            input.SetBounds(x, y + SetupLabelHeight + 2, width, SetupInputHeight);
        }

        private void LayoutChallengeControls(int x, int y, int width)
        {
            if (lblChallenge == null || txtChallengeSeed == null || btnChallengeSeed == null)
            {
                return;
            }

            lblChallenge.SetBounds(x, y, width, SetupLabelHeight);
            txtChallengeSeed.SetBounds(x, y + SetupLabelHeight + 2, width - ChallengeButtonWidth - StartPanelGap, SetupInputHeight);
            btnChallengeSeed.SetBounds(x + width - ChallengeButtonWidth, y + SetupLabelHeight + 2, ChallengeButtonWidth, SetupInputHeight);
        }

        private void LockWindowSizeForRun()
        {
            if (isWindowSizeLocked)
            {
                return;
            }

            unlockedMinimumSize = MinimumSize;
            unlockedMaximumSize = MaximumSize;

            Size lockedSize = Size;
            MinimumSize = lockedSize;
            MaximumSize = lockedSize;
            unlockedMaximizeBox = MaximizeBox;
            MaximizeBox = false;
            isWindowSizeLocked = true;
        }

        private void UnlockWindowSizeAfterRun()
        {
            if (!isWindowSizeLocked)
            {
                return;
            }

            MinimumSize = unlockedMinimumSize;
            MaximumSize = unlockedMaximumSize;
            MaximizeBox = unlockedMaximizeBox;
            isWindowSizeLocked = false;
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

            ConfigureSetupLabel(lblMode);
            ConfigureSetupLabel(lblBoardSize);
            ConfigureSetupLabel(lblTheme);
            ConfigureSetupLabel(lblChallenge);
            ConfigureLeaderboardLabel();
        }

        private void ConfigureSetupLabel(Label label)
        {
            if (label == null)
            {
                return;
            }

            label.AutoSize = false;
            label.AutoEllipsis = true;
            label.ForeColor = StartPanelMutedTextColor;
            label.BackColor = Color.Transparent;
            label.Font = startHintFont;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void ConfigureLeaderboardLabel()
        {
            if (lblLeaderboard == null)
            {
                return;
            }

            lblLeaderboard.AutoSize = false;
            lblLeaderboard.AutoEllipsis = true;
            lblLeaderboard.ForeColor = OverlayTextColor;
            lblLeaderboard.BackColor = Color.Transparent;
            lblLeaderboard.Font = hudFont;
            lblLeaderboard.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void ConfigurePresetControls()
        {
            ConfigureComboBox(cmbMode, GamePresets.GetModeNames());
            ConfigureComboBox(cmbBoardSize, GamePresets.GetBoardSizeNames());
            ConfigureComboBox(cmbTheme, GamePresets.GetThemeNames());
            ConfigureTextBox(txtChallengeSeed);
        }

        private void ConfigureComboBox(ComboBox comboBox, string[] items)
        {
            if (comboBox == null)
            {
                return;
            }

            comboBox.Items.Clear();
            comboBox.Items.AddRange(items);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.BackColor = ToggleBackColor;
            comboBox.ForeColor = HudTextColor;
            comboBox.Font = startHintFont;
            comboBox.IntegralHeight = false;
            comboBox.MaxDropDownItems = 6;
        }

        private void ConfigureTextBox(TextBox textBox)
        {
            if (textBox == null)
            {
                return;
            }

            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = ToggleBackColor;
            textBox.ForeColor = HudTextColor;
            textBox.Font = startHintFont;
            textBox.CharacterCasing = CharacterCasing.Upper;
        }

        private void ConfigureButtons()
        {
            ConfigureButton(btnStartGame, true);
            ConfigureButton(btnPause, false);
            ConfigureButton(btnSpeedDown, false);
            ConfigureButton(btnSpeedUp, false);
            ConfigureButton(btnChallengeSeed, false);
            ConfigureCheckBox(chkWrapWalls);
            ConfigureCheckBox(chkProgressiveSpeed);
            ConfigureCheckBox(chkObstacles);
            ConfigureCheckBox(chkSound);
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
            StyleToggle(chkSound);
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
            UpdateLeaderboardText();
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
                lblStartHint.Text = GamePresets.GetMode(selectedModePreset).Description;
                btnStartGame.Text = "START RUN";
            }

            lblStartSpeed.Text = "Speed " + GameSpeed.GetDisplayValue(selectedSpeed, useProgressiveSpeed);
            StyleSpeedButton(btnSpeedDown, selectedSpeed > GameSpeed.MinimumSpeed);
            StyleSpeedButton(btnSpeedUp, selectedSpeed < GameSpeed.MaximumSpeed);
        }

        private void UpdateLeaderboardText()
        {
            if (lblLeaderboard == null || leaderboard == null)
            {
                return;
            }

            string modeName = GamePresets.GetMode(selectedModePreset).Name;
            System.Collections.Generic.IList<LeaderboardEntry> topEntries = leaderboard.GetTopEntries(modeName);
            if (topEntries.Count == 0)
            {
                lblLeaderboard.Text = "Top " + modeName + ": -";
                return;
            }

            string first = topEntries[0].Score.ToString();
            string second = topEntries.Count > 1 ? topEntries[1].Score.ToString() : "-";
            string third = topEntries.Count > 2 ? topEntries[2].Score.ToString() : "-";
            lblLeaderboard.Text = "Top " + modeName + ": 1 " + first + "   2 " + second + "   3 " + third;
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

        private void LoadLeaderboard()
        {
            try
            {
                leaderboard = LeaderboardStore.Deserialize(SnakeGame.Properties.Settings.Default.LeaderboardData);
            }
            catch
            {
                leaderboard = new LeaderboardStore();
            }
        }

        private void LoadPlayerSettings()
        {
            suppressPlayerSettingSave = true;
            try
            {
                selectedModePreset = GamePresets.ParseMode(SnakeGame.Properties.Settings.Default.ModePreset);
                selectedBoardSizePreset = GamePresets.ParseBoardSize(SnakeGame.Properties.Settings.Default.BoardSizePreset);
                selectedThemePreset = GamePresets.ParseTheme(SnakeGame.Properties.Settings.Default.ThemePreset);
                activeChallengeSeed = ChallengeSeed.Normalize(SnakeGame.Properties.Settings.Default.ChallengeSeed);
                selectedSpeed = ClampSpeed(SnakeGame.Properties.Settings.Default.Speed);
                chkWrapWalls.Checked = SnakeGame.Properties.Settings.Default.WrapWalls;
                chkProgressiveSpeed.Checked = SnakeGame.Properties.Settings.Default.ProgressiveSpeed;
                chkObstacles.Checked = SnakeGame.Properties.Settings.Default.Obstacles;
                chkSound.Checked = SnakeGame.Properties.Settings.Default.SoundEnabled;
            }
            catch
            {
                selectedModePreset = GamePresets.DefaultMode;
                selectedBoardSizePreset = GamePresets.DefaultBoardSize;
                selectedThemePreset = GamePresets.DefaultTheme;
                activeChallengeSeed = string.Empty;
                selectedSpeed = ClampSpeed(GameSpeed.DefaultSpeed);
                chkWrapWalls.Checked = DefaultWrapWalls;
                chkProgressiveSpeed.Checked = GameSpeed.DefaultProgressiveSpeed;
                chkObstacles.Checked = DefaultObstacles;
                chkSound.Checked = DefaultSoundEnabled;
            }
            finally
            {
                SelectComboIndex(cmbMode, (int)selectedModePreset);
                SelectComboIndex(cmbBoardSize, (int)selectedBoardSizePreset);
                SelectComboIndex(cmbTheme, (int)selectedThemePreset);
                SetChallengeSeedText(activeChallengeSeed);
                suppressPlayerSettingSave = false;
            }
        }

        private void SetChallengeSeedText(string challengeSeed)
        {
            if (txtChallengeSeed == null)
            {
                return;
            }

            txtChallengeSeed.Text = ChallengeSeed.Normalize(challengeSeed);
        }

        private void SelectComboIndex(ComboBox comboBox, int index)
        {
            if (comboBox == null || comboBox.Items.Count == 0)
            {
                return;
            }

            comboBox.SelectedIndex = Math.Max(0, Math.Min(comboBox.Items.Count - 1, index));
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
                SnakeGame.Properties.Settings.Default.SoundEnabled = chkSound.Checked;
                SnakeGame.Properties.Settings.Default.ModePreset = (int)selectedModePreset;
                SnakeGame.Properties.Settings.Default.BoardSizePreset = (int)selectedBoardSizePreset;
                SnakeGame.Properties.Settings.Default.ThemePreset = (int)selectedThemePreset;
                SnakeGame.Properties.Settings.Default.ChallengeSeed = ChallengeSeed.Normalize(txtChallengeSeed == null ? activeChallengeSeed : txtChallengeSeed.Text);
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

        private void SaveLeaderboard()
        {
            try
            {
                SnakeGame.Properties.Settings.Default.LeaderboardData = leaderboard.Serialize();
                SnakeGame.Properties.Settings.Default.Save();
            }
            catch
            {
                // Leaderboard persistence should never interrupt gameplay.
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
                PlayPauseSound();
            }
            else
            {
                timer1.Start();
                btnPause.Text = "Pause";
                PlayResumeSound();
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
            RecordFinishedRun();
            btnPause.Visible = false;
            btnPause.Text = "Pause";
            UnlockWindowSizeAfterRun();
            SetStartMenuVisible(true);
            UpdateHud();
            Invalidate();
        }

        private void RecordFinishedRun()
        {
            if (hasRecordedFinishedRun || game.Score <= 0)
            {
                return;
            }

            hasRecordedFinishedRun = true;
            GameModeSettings mode = GamePresets.GetMode(selectedModePreset);
            BoardSizeSettings board = GamePresets.GetBoardSize(selectedBoardSizePreset);
            leaderboard.Add(new LeaderboardEntry(
                mode.Name,
                game.Score,
                GameSpeed.GetDisplayValue(selectedSpeed, useProgressiveSpeed),
                board.DisplayName,
                activeChallengeSeed,
                DateTime.UtcNow));
            SaveLeaderboard();
        }

        private BoundaryMode GetSelectedBoundaryMode()
        {
            return chkWrapWalls.Checked ? BoundaryMode.Wrap : BoundaryMode.SolidWalls;
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

            GameStatus statusBeforeStep = game.Status;
            int scoreBeforeStep = game.Score;
            GridCell foodBeforeStep = game.Food;
            game.Step();
            if (game.Score > scoreBeforeStep)
            {
                TriggerEatFeedback(foodBeforeStep);
                if (game.Status == GameStatus.Won)
                {
                    PlayFinishSound();
                }
            }
            else if (game.Status != statusBeforeStep && game.IsFinished)
            {
                PlayFinishSound();
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
            PlayEatSound();
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

        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMode == null || cmbMode.SelectedIndex < 0)
            {
                return;
            }

            selectedModePreset = GamePresets.ParseMode(cmbMode.SelectedIndex);
            if (!suppressPlayerSettingSave)
            {
                ApplyModePreset(selectedModePreset);
                SavePlayerSettings();
            }

            UpdateHud();
            Invalidate();
            Focus();
        }

        private void cmbBoardSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBoardSize == null || cmbBoardSize.SelectedIndex < 0)
            {
                return;
            }

            selectedBoardSizePreset = GamePresets.ParseBoardSize(cmbBoardSize.SelectedIndex);
            if (!suppressPlayerSettingSave)
            {
                ApplySelectedBoardSize();
                SavePlayerSettings();
            }

            UpdateHud();
            Invalidate();
            Focus();
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTheme == null || cmbTheme.SelectedIndex < 0)
            {
                return;
            }

            selectedThemePreset = GamePresets.ParseTheme(cmbTheme.SelectedIndex);
            if (!suppressPlayerSettingSave)
            {
                SavePlayerSettings();
            }

            Invalidate();
            Focus();
        }

        private void txtChallengeSeed_TextChanged(object sender, EventArgs e)
        {
            if (txtChallengeSeed == null)
            {
                return;
            }

            activeChallengeSeed = ChallengeSeed.Normalize(txtChallengeSeed.Text);
            if (!suppressPlayerSettingSave)
            {
                SavePlayerSettings();
            }
        }

        private void txtChallengeSeed_Leave(object sender, EventArgs e)
        {
            if (txtChallengeSeed == null)
            {
                return;
            }

            string normalized = ChallengeSeed.Normalize(txtChallengeSeed.Text);
            if (!string.Equals(txtChallengeSeed.Text, normalized, StringComparison.Ordinal))
            {
                txtChallengeSeed.Text = normalized;
            }

            activeChallengeSeed = normalized;
            SavePlayerSettings();
        }

        private void btnChallengeSeed_Click(object sender, EventArgs e)
        {
            SetChallengeSeedText(ChallengeSeed.Generate(DateTime.UtcNow));
            activeChallengeSeed = ChallengeSeed.Normalize(txtChallengeSeed.Text);
            SavePlayerSettings();
            Focus();
        }

        private void ApplyModePreset(GameModePreset preset)
        {
            GameModeSettings mode = GamePresets.GetMode(preset);
            bool wasSuppressing = suppressPlayerSettingSave;
            suppressPlayerSettingSave = true;
            selectedSpeed = mode.Speed;
            chkWrapWalls.Checked = mode.WrapWalls;
            chkProgressiveSpeed.Checked = mode.ProgressiveSpeed;
            chkObstacles.Checked = mode.Obstacles;
            suppressPlayerSettingSave = wasSuppressing;

            UpdateSettingsFromUI();
            UpdateHud();
        }

        private void ApplySelectedBoardSize()
        {
            if (isWindowSizeLocked)
            {
                return;
            }

            BoardSizeSettings boardSize = GamePresets.GetBoardSize(selectedBoardSizePreset);
            ClientSize = new Size(boardSize.GridWidth * CellSize, HudHeight + boardSize.GridHeight * CellSize);
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

        private void chkSound_CheckedChanged(object sender, EventArgs e)
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
