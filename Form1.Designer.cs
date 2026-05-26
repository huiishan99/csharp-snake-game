namespace SnakeGame
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlStartMenu = new System.Windows.Forms.Panel();
            this.lblStartTitle = new System.Windows.Forms.Label();
            this.lblStartHint = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnStartGame = new SnakeGame.ThemeButton();
            this.btnSpeedDown = new SnakeGame.ThemeButton();
            this.btnSpeedUp = new SnakeGame.ThemeButton();
            this.lblScore = new SnakeGame.ThemePillLabel();
            this.lblHighScore = new SnakeGame.ThemePillLabel();
            this.lblSpeed = new SnakeGame.ThemePillLabel();
            this.lblStatus = new SnakeGame.ThemePillLabel();
            this.btnPause = new SnakeGame.ThemeButton();
            this.chkWrapWalls = new SnakeGame.ThemeToggle();
            this.chkProgressiveSpeed = new SnakeGame.ThemeToggle();
            this.chkObstacles = new SnakeGame.ThemeToggle();
            this.chkSound = new SnakeGame.ThemeToggle();
            this.lblStartSpeed = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.lblBoardSize = new System.Windows.Forms.Label();
            this.cmbBoardSize = new System.Windows.Forms.ComboBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.lblChallenge = new System.Windows.Forms.Label();
            this.txtChallengeSeed = new System.Windows.Forms.TextBox();
            this.btnChallengeSeed = new SnakeGame.ThemeButton();
            this.lblLeaderboard = new System.Windows.Forms.Label();
            this.pnlStartMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlStartMenu
            //
            this.pnlStartMenu.Controls.Add(this.lblLeaderboard);
            this.pnlStartMenu.Controls.Add(this.btnChallengeSeed);
            this.pnlStartMenu.Controls.Add(this.txtChallengeSeed);
            this.pnlStartMenu.Controls.Add(this.lblChallenge);
            this.pnlStartMenu.Controls.Add(this.cmbTheme);
            this.pnlStartMenu.Controls.Add(this.lblTheme);
            this.pnlStartMenu.Controls.Add(this.cmbBoardSize);
            this.pnlStartMenu.Controls.Add(this.lblBoardSize);
            this.pnlStartMenu.Controls.Add(this.cmbMode);
            this.pnlStartMenu.Controls.Add(this.lblMode);
            this.pnlStartMenu.Controls.Add(this.chkSound);
            this.pnlStartMenu.Controls.Add(this.chkObstacles);
            this.pnlStartMenu.Controls.Add(this.chkProgressiveSpeed);
            this.pnlStartMenu.Controls.Add(this.chkWrapWalls);
            this.pnlStartMenu.Controls.Add(this.lblStartSpeed);
            this.pnlStartMenu.Controls.Add(this.btnSpeedUp);
            this.pnlStartMenu.Controls.Add(this.btnSpeedDown);
            this.pnlStartMenu.Controls.Add(this.btnStartGame);
            this.pnlStartMenu.Controls.Add(this.lblStartHint);
            this.pnlStartMenu.Controls.Add(this.lblStartTitle);
            this.pnlStartMenu.Location = new System.Drawing.Point(161, 55);
            this.pnlStartMenu.Name = "pnlStartMenu";
            this.pnlStartMenu.Size = new System.Drawing.Size(460, 430);
            this.pnlStartMenu.TabIndex = 0;
            this.pnlStartMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlStartMenu_Paint);
            //
            // lblStartTitle
            //
            this.lblStartTitle.Location = new System.Drawing.Point(24, 16);
            this.lblStartTitle.Name = "lblStartTitle";
            this.lblStartTitle.Size = new System.Drawing.Size(412, 32);
            this.lblStartTitle.TabIndex = 0;
            this.lblStartTitle.Text = "SNAKE";
            this.lblStartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStartHint
            //
            this.lblStartHint.Location = new System.Drawing.Point(24, 48);
            this.lblStartHint.Name = "lblStartHint";
            this.lblStartHint.Size = new System.Drawing.Size(412, 20);
            this.lblStartHint.TabIndex = 1;
            this.lblStartHint.Text = "SURVIVE THE GRID";
            this.lblStartHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // timer1
            //
            this.timer1.Enabled = false;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // btnStartGame
            //
            this.btnStartGame.AccessibleName = "";
            this.btnStartGame.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.btnStartGame.Location = new System.Drawing.Point(24, 126);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(412, 34);
            this.btnStartGame.TabIndex = 2;
            this.btnStartGame.Text = "START RUN";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            //
            // btnSpeedDown
            //
            this.btnSpeedDown.Location = new System.Drawing.Point(24, 282);
            this.btnSpeedDown.Name = "btnSpeedDown";
            this.btnSpeedDown.Size = new System.Drawing.Size(36, 30);
            this.btnSpeedDown.TabIndex = 3;
            this.btnSpeedDown.Text = "-";
            this.btnSpeedDown.UseVisualStyleBackColor = true;
            this.btnSpeedDown.Click += new System.EventHandler(this.btnSpeedDown_Click);
            //
            // btnSpeedUp
            //
            this.btnSpeedUp.Location = new System.Drawing.Point(400, 282);
            this.btnSpeedUp.Name = "btnSpeedUp";
            this.btnSpeedUp.Size = new System.Drawing.Size(36, 30);
            this.btnSpeedUp.TabIndex = 5;
            this.btnSpeedUp.Text = "+";
            this.btnSpeedUp.UseVisualStyleBackColor = true;
            this.btnSpeedUp.Click += new System.EventHandler(this.btnSpeedUp_Click);
            //
            // lblScore
            //
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(12, 12);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(62, 16);
            this.lblScore.TabIndex = 2;
            this.lblScore.Text = "Score: 0";
            //
            // lblHighScore
            //
            this.lblHighScore.AutoSize = true;
            this.lblHighScore.Location = new System.Drawing.Point(120, 12);
            this.lblHighScore.Name = "lblHighScore";
            this.lblHighScore.Size = new System.Drawing.Size(49, 16);
            this.lblHighScore.TabIndex = 3;
            this.lblHighScore.Text = "Best: 0";
            //
            // lblSpeed
            //
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(228, 12);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(65, 16);
            this.lblSpeed.TabIndex = 4;
            this.lblSpeed.Text = "Speed: 5";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(336, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(51, 16);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Ready";
            //
            // btnPause
            //
            this.btnPause.Location = new System.Drawing.Point(680, 8);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(90, 28);
            this.btnPause.TabIndex = 6;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Visible = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            //
            // chkWrapWalls
            //
            this.chkWrapWalls.Checked = true;
            this.chkWrapWalls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWrapWalls.Location = new System.Drawing.Point(24, 320);
            this.chkWrapWalls.Name = "chkWrapWalls";
            this.chkWrapWalls.Size = new System.Drawing.Size(203, 26);
            this.chkWrapWalls.TabIndex = 6;
            this.chkWrapWalls.Text = "Wrap";
            this.chkWrapWalls.UseVisualStyleBackColor = true;
            this.chkWrapWalls.CheckedChanged += new System.EventHandler(this.chkWrapWalls_CheckedChanged);
            //
            // chkProgressiveSpeed
            //
            this.chkProgressiveSpeed.Checked = true;
            this.chkProgressiveSpeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProgressiveSpeed.Location = new System.Drawing.Point(233, 320);
            this.chkProgressiveSpeed.Name = "chkProgressiveSpeed";
            this.chkProgressiveSpeed.Size = new System.Drawing.Size(203, 26);
            this.chkProgressiveSpeed.TabIndex = 7;
            this.chkProgressiveSpeed.Text = "Progressive";
            this.chkProgressiveSpeed.UseVisualStyleBackColor = true;
            this.chkProgressiveSpeed.CheckedChanged += new System.EventHandler(this.chkProgressiveSpeed_CheckedChanged);
            //
            // chkObstacles
            //
            this.chkObstacles.Location = new System.Drawing.Point(24, 352);
            this.chkObstacles.Name = "chkObstacles";
            this.chkObstacles.Size = new System.Drawing.Size(203, 26);
            this.chkObstacles.TabIndex = 8;
            this.chkObstacles.Text = "Obstacles";
            this.chkObstacles.UseVisualStyleBackColor = true;
            this.chkObstacles.CheckedChanged += new System.EventHandler(this.chkObstacles_CheckedChanged);
            //
            // chkSound
            //
            this.chkSound.Checked = true;
            this.chkSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSound.Location = new System.Drawing.Point(233, 352);
            this.chkSound.Name = "chkSound";
            this.chkSound.Size = new System.Drawing.Size(203, 26);
            this.chkSound.TabIndex = 9;
            this.chkSound.Text = "Sound";
            this.chkSound.UseVisualStyleBackColor = true;
            this.chkSound.CheckedChanged += new System.EventHandler(this.chkSound_CheckedChanged);
            //
            // lblStartSpeed
            //
            this.lblStartSpeed.Location = new System.Drawing.Point(66, 282);
            this.lblStartSpeed.Name = "lblStartSpeed";
            this.lblStartSpeed.Size = new System.Drawing.Size(328, 30);
            this.lblStartSpeed.TabIndex = 4;
            this.lblStartSpeed.Text = "Speed 5+";
            this.lblStartSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblMode
            //
            this.lblMode.Location = new System.Drawing.Point(24, 172);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(202, 16);
            this.lblMode.TabIndex = 10;
            this.lblMode.Text = "MODE";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbMode
            //
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Location = new System.Drawing.Point(24, 190);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(202, 24);
            this.cmbMode.TabIndex = 11;
            this.cmbMode.SelectedIndexChanged += new System.EventHandler(this.cmbMode_SelectedIndexChanged);
            //
            // lblBoardSize
            //
            this.lblBoardSize.Location = new System.Drawing.Point(234, 172);
            this.lblBoardSize.Name = "lblBoardSize";
            this.lblBoardSize.Size = new System.Drawing.Size(202, 16);
            this.lblBoardSize.TabIndex = 12;
            this.lblBoardSize.Text = "BOARD";
            this.lblBoardSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbBoardSize
            //
            this.cmbBoardSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoardSize.FormattingEnabled = true;
            this.cmbBoardSize.Location = new System.Drawing.Point(234, 190);
            this.cmbBoardSize.Name = "cmbBoardSize";
            this.cmbBoardSize.Size = new System.Drawing.Size(202, 24);
            this.cmbBoardSize.TabIndex = 13;
            this.cmbBoardSize.SelectedIndexChanged += new System.EventHandler(this.cmbBoardSize_SelectedIndexChanged);
            //
            // lblTheme
            //
            this.lblTheme.Location = new System.Drawing.Point(24, 226);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(202, 16);
            this.lblTheme.TabIndex = 14;
            this.lblTheme.Text = "THEME";
            this.lblTheme.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbTheme
            //
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(24, 244);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(202, 24);
            this.cmbTheme.TabIndex = 15;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            //
            // lblChallenge
            //
            this.lblChallenge.Location = new System.Drawing.Point(234, 226);
            this.lblChallenge.Name = "lblChallenge";
            this.lblChallenge.Size = new System.Drawing.Size(202, 16);
            this.lblChallenge.TabIndex = 16;
            this.lblChallenge.Text = "CHALLENGE";
            this.lblChallenge.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtChallengeSeed
            //
            this.txtChallengeSeed.Location = new System.Drawing.Point(234, 244);
            this.txtChallengeSeed.Name = "txtChallengeSeed";
            this.txtChallengeSeed.Size = new System.Drawing.Size(144, 22);
            this.txtChallengeSeed.TabIndex = 17;
            this.txtChallengeSeed.TextChanged += new System.EventHandler(this.txtChallengeSeed_TextChanged);
            this.txtChallengeSeed.Leave += new System.EventHandler(this.txtChallengeSeed_Leave);
            //
            // btnChallengeSeed
            //
            this.btnChallengeSeed.Location = new System.Drawing.Point(384, 244);
            this.btnChallengeSeed.Name = "btnChallengeSeed";
            this.btnChallengeSeed.Size = new System.Drawing.Size(52, 26);
            this.btnChallengeSeed.TabIndex = 18;
            this.btnChallengeSeed.Text = "NEW";
            this.btnChallengeSeed.UseVisualStyleBackColor = true;
            this.btnChallengeSeed.Click += new System.EventHandler(this.btnChallengeSeed_Click);
            //
            // lblLeaderboard
            //
            this.lblLeaderboard.Location = new System.Drawing.Point(24, 388);
            this.lblLeaderboard.Name = "lblLeaderboard";
            this.lblLeaderboard.Size = new System.Drawing.Size(412, 28);
            this.lblLeaderboard.TabIndex = 19;
            this.lblLeaderboard.Text = "Top: -";
            this.lblLeaderboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 524);
            this.MinimumSize = new System.Drawing.Size(600, 540);
            this.Controls.Add(this.pnlStartMenu);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.lblHighScore);
            this.Controls.Add(this.lblScore);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Snake Game";
            this.pnlStartMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlStartMenu;
        private System.Windows.Forms.Label lblStartTitle;
        private System.Windows.Forms.Label lblStartHint;
        private System.Windows.Forms.Timer timer1;
        private SnakeGame.ThemeButton btnStartGame;
        private SnakeGame.ThemeButton btnSpeedDown;
        private SnakeGame.ThemeButton btnSpeedUp;
        private SnakeGame.ThemePillLabel lblScore;
        private SnakeGame.ThemePillLabel lblHighScore;
        private SnakeGame.ThemePillLabel lblSpeed;
        private SnakeGame.ThemePillLabel lblStatus;
        private SnakeGame.ThemeButton btnPause;
        private SnakeGame.ThemeToggle chkWrapWalls;
        private SnakeGame.ThemeToggle chkProgressiveSpeed;
        private SnakeGame.ThemeToggle chkObstacles;
        private SnakeGame.ThemeToggle chkSound;
        private System.Windows.Forms.Label lblStartSpeed;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label lblBoardSize;
        private System.Windows.Forms.ComboBox cmbBoardSize;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Label lblChallenge;
        private System.Windows.Forms.TextBox txtChallengeSeed;
        private SnakeGame.ThemeButton btnChallengeSeed;
        private System.Windows.Forms.Label lblLeaderboard;
    }
}
