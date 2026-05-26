using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1
    {
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
                canvas.DrawLine(mutedPen, StartPanelPadding, 70, pnlStartMenu.Width - StartPanelPadding, 70);
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
            return new Rectangle(StartPanelPadding, 76, Math.Max(0, pnlStartMenu.Width - StartPanelPadding * 2), 42);
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
    }
}
