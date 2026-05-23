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
            this.lblStartSpeed = new System.Windows.Forms.Label();
            this.pnlStartMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlStartMenu
            //
            this.pnlStartMenu.Controls.Add(this.chkObstacles);
            this.pnlStartMenu.Controls.Add(this.chkProgressiveSpeed);
            this.pnlStartMenu.Controls.Add(this.chkWrapWalls);
            this.pnlStartMenu.Controls.Add(this.lblStartSpeed);
            this.pnlStartMenu.Controls.Add(this.btnSpeedUp);
            this.pnlStartMenu.Controls.Add(this.btnSpeedDown);
            this.pnlStartMenu.Controls.Add(this.btnStartGame);
            this.pnlStartMenu.Controls.Add(this.lblStartHint);
            this.pnlStartMenu.Controls.Add(this.lblStartTitle);
            this.pnlStartMenu.Location = new System.Drawing.Point(228, 92);
            this.pnlStartMenu.Name = "pnlStartMenu";
            this.pnlStartMenu.Size = new System.Drawing.Size(326, 246);
            this.pnlStartMenu.TabIndex = 0;
            this.pnlStartMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlStartMenu_Paint);
            //
            // lblStartTitle
            //
            this.lblStartTitle.Location = new System.Drawing.Point(22, 14);
            this.lblStartTitle.Name = "lblStartTitle";
            this.lblStartTitle.Size = new System.Drawing.Size(282, 30);
            this.lblStartTitle.TabIndex = 0;
            this.lblStartTitle.Text = "SNAKE";
            this.lblStartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblStartHint
            //
            this.lblStartHint.Location = new System.Drawing.Point(22, 43);
            this.lblStartHint.Name = "lblStartHint";
            this.lblStartHint.Size = new System.Drawing.Size(282, 20);
            this.lblStartHint.TabIndex = 1;
            this.lblStartHint.Text = "Ready when you are";
            this.lblStartHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.btnStartGame.Location = new System.Drawing.Point(22, 68);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(282, 36);
            this.btnStartGame.TabIndex = 2;
            this.btnStartGame.Text = "Start";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            //
            // btnSpeedDown
            //
            this.btnSpeedDown.Location = new System.Drawing.Point(22, 116);
            this.btnSpeedDown.Name = "btnSpeedDown";
            this.btnSpeedDown.Size = new System.Drawing.Size(36, 32);
            this.btnSpeedDown.TabIndex = 3;
            this.btnSpeedDown.Text = "-";
            this.btnSpeedDown.UseVisualStyleBackColor = true;
            this.btnSpeedDown.Click += new System.EventHandler(this.btnSpeedDown_Click);
            //
            // btnSpeedUp
            //
            this.btnSpeedUp.Location = new System.Drawing.Point(268, 116);
            this.btnSpeedUp.Name = "btnSpeedUp";
            this.btnSpeedUp.Size = new System.Drawing.Size(36, 32);
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
            this.chkWrapWalls.Location = new System.Drawing.Point(22, 166);
            this.chkWrapWalls.Name = "chkWrapWalls";
            this.chkWrapWalls.Size = new System.Drawing.Size(138, 28);
            this.chkWrapWalls.TabIndex = 6;
            this.chkWrapWalls.Text = "Wrap";
            this.chkWrapWalls.UseVisualStyleBackColor = true;
            this.chkWrapWalls.CheckedChanged += new System.EventHandler(this.chkWrapWalls_CheckedChanged);
            //
            // chkProgressiveSpeed
            //
            this.chkProgressiveSpeed.Checked = true;
            this.chkProgressiveSpeed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProgressiveSpeed.Location = new System.Drawing.Point(166, 166);
            this.chkProgressiveSpeed.Name = "chkProgressiveSpeed";
            this.chkProgressiveSpeed.Size = new System.Drawing.Size(138, 28);
            this.chkProgressiveSpeed.TabIndex = 7;
            this.chkProgressiveSpeed.Text = "Progressive";
            this.chkProgressiveSpeed.UseVisualStyleBackColor = true;
            this.chkProgressiveSpeed.CheckedChanged += new System.EventHandler(this.chkProgressiveSpeed_CheckedChanged);
            //
            // chkObstacles
            //
            this.chkObstacles.Location = new System.Drawing.Point(22, 202);
            this.chkObstacles.Name = "chkObstacles";
            this.chkObstacles.Size = new System.Drawing.Size(282, 28);
            this.chkObstacles.TabIndex = 8;
            this.chkObstacles.Text = "Obstacles";
            this.chkObstacles.UseVisualStyleBackColor = true;
            this.chkObstacles.CheckedChanged += new System.EventHandler(this.chkObstacles_CheckedChanged);
            //
            // lblStartSpeed
            //
            this.lblStartSpeed.Location = new System.Drawing.Point(64, 116);
            this.lblStartSpeed.Name = "lblStartSpeed";
            this.lblStartSpeed.Size = new System.Drawing.Size(198, 32);
            this.lblStartSpeed.TabIndex = 4;
            this.lblStartSpeed.Text = "Speed 5+";
            this.lblStartSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 453);
            this.MinimumSize = new System.Drawing.Size(520, 360);
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
        private System.Windows.Forms.Label lblStartSpeed;
    }
}
