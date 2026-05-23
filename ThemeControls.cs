using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnakeGame
{
    internal static class ThemePaint
    {
        internal static void PaintParentBackground(
            Control control,
            Graphics canvas,
            Color fallbackBackColor,
            System.Action<Control, PaintEventArgs> paintBackground,
            System.Action<Control, PaintEventArgs> paint)
        {
            if (control.Parent == null)
            {
                canvas.Clear(fallbackBackColor);
                return;
            }

            GraphicsState state = canvas.Save();
            try
            {
                canvas.TranslateTransform(-control.Left, -control.Top);
                using (PaintEventArgs parentPaint = new PaintEventArgs(canvas, new Rectangle(control.Left, control.Top, control.Width, control.Height)))
                {
                    paintBackground(control.Parent, parentPaint);
                    paint(control.Parent, parentPaint);
                }
            }
            finally
            {
                canvas.Restore(state);
            }
        }
    }

    public class ThemeButton : Button
    {
        private bool isHovered;
        private bool isPressed;

        public ThemeButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint,
                true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            CornerRadius = 6;
            BorderColor = Color.Transparent;
            NormalBackColor = Color.FromArgb(37, 52, 57);
            HoverBackColor = Color.FromArgb(47, 65, 70);
            PressedBackColor = Color.FromArgb(31, 43, 48);
            DisabledBackColor = Color.FromArgb(25, 34, 38);
            NormalTextColor = Color.FromArgb(225, 239, 235);
            DisabledTextColor = Color.FromArgb(104, 126, 128);
        }

        public int CornerRadius { get; set; }
        public Color NormalBackColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color PressedBackColor { get; set; }
        public Color DisabledBackColor { get; set; }
        public Color NormalTextColor { get; set; }
        public Color DisabledTextColor { get; set; }
        public Color BorderColor { get; set; }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnEnabledChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics canvas = pevent.Graphics;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            ThemePaint.PaintParentBackground(this, canvas, BackColor, InvokePaintBackground, InvokePaint);

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            Color currentBackColor = GetCurrentBackColor();
            Color topColor = Mix(currentBackColor, Color.White, Enabled ? 14 : 4);
            Color bottomColor = Mix(currentBackColor, Color.Black, Enabled ? 8 : 0);
            using (GraphicsPath path = CreateRoundedRectangle(bounds, CornerRadius))
            using (LinearGradientBrush backBrush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                canvas.FillPath(backBrush, path);

                if (BorderColor.A > 0)
                {
                    using (Pen borderPen = new Pen(BorderColor, 1))
                    {
                        canvas.DrawPath(borderPen, path);
                    }
                }

                if (Focused && Enabled)
                {
                    Rectangle focusBounds = Rectangle.Inflate(bounds, -2, -2);
                    using (GraphicsPath focusPath = CreateRoundedRectangle(focusBounds, System.Math.Max(1, CornerRadius - 2)))
                    using (Pen focusPen = new Pen(Color.FromArgb(100, NormalTextColor), 1))
                    {
                        canvas.DrawPath(focusPen, focusPath);
                    }
                }
            }

            TextRenderer.DrawText(
                canvas,
                Text,
                Font,
                bounds,
                Enabled ? NormalTextColor : DisabledTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private Color GetCurrentBackColor()
        {
            if (!Enabled)
            {
                return DisabledBackColor;
            }

            if (isPressed)
            {
                return PressedBackColor;
            }

            return isHovered ? HoverBackColor : NormalBackColor;
        }

        internal static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            int clampedRadius = System.Math.Max(1, System.Math.Min(radius, System.Math.Min(bounds.Width, bounds.Height) / 2));
            int diameter = clampedRadius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        internal static Color Mix(Color first, Color second, int secondPercent)
        {
            int clampedPercent = System.Math.Max(0, System.Math.Min(100, secondPercent));
            int firstPercent = 100 - clampedPercent;
            return Color.FromArgb(
                first.A,
                (first.R * firstPercent + second.R * clampedPercent) / 100,
                (first.G * firstPercent + second.G * clampedPercent) / 100,
                (first.B * firstPercent + second.B * clampedPercent) / 100);
        }
    }

    public class ThemeToggle : CheckBox
    {
        private bool isHovered;
        private bool isPressed;

        public ThemeToggle()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint,
                true);
            Appearance = Appearance.Button;
            AutoSize = false;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            CornerRadius = 6;
            NormalBackColor = Color.FromArgb(37, 52, 57);
            CheckedBackColor = Color.FromArgb(90, 220, 145);
            HoverBackColor = Color.FromArgb(47, 65, 70);
            PressedBackColor = Color.FromArgb(31, 43, 48);
            NormalTextColor = Color.FromArgb(225, 239, 235);
            CheckedTextColor = Color.FromArgb(8, 24, 15);
            BorderColor = Color.FromArgb(76, 99, 104);
            CheckedBorderColor = Color.FromArgb(90, 220, 145);
        }

        public int CornerRadius { get; set; }
        public Color NormalBackColor { get; set; }
        public Color CheckedBackColor { get; set; }
        public Color HoverBackColor { get; set; }
        public Color PressedBackColor { get; set; }
        public Color NormalTextColor { get; set; }
        public Color CheckedTextColor { get; set; }
        public Color BorderColor { get; set; }
        public Color CheckedBorderColor { get; set; }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnCheckedChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnCheckedChanged(e);
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics canvas = pevent.Graphics;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            ThemePaint.PaintParentBackground(this, canvas, BackColor, InvokePaintBackground, InvokePaint);

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            Color currentBackColor = GetCurrentBackColor();
            Color topColor = ThemeButton.Mix(currentBackColor, Color.White, 10);
            Color bottomColor = ThemeButton.Mix(currentBackColor, Color.Black, 8);
            using (GraphicsPath path = ThemeButton.CreateRoundedRectangle(bounds, CornerRadius))
            using (LinearGradientBrush backBrush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
            using (Pen borderPen = new Pen(Checked ? CheckedBorderColor : BorderColor, 1))
            {
                canvas.FillPath(backBrush, path);
                canvas.DrawPath(borderPen, path);
            }

            DrawStateDot(canvas);

            Rectangle textBounds = new Rectangle(22, 0, System.Math.Max(0, Width - 26), Height);
            TextRenderer.DrawText(
                canvas,
                Text,
                Font,
                textBounds,
                Checked ? CheckedTextColor : NormalTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (Focused)
            {
                Rectangle focusBounds = Rectangle.Inflate(bounds, -2, -2);
                using (GraphicsPath focusPath = ThemeButton.CreateRoundedRectangle(focusBounds, System.Math.Max(1, CornerRadius - 2)))
                using (Pen focusPen = new Pen(Color.FromArgb(90, Checked ? CheckedTextColor : NormalTextColor), 1))
                {
                    canvas.DrawPath(focusPen, focusPath);
                }
            }
        }

        private Color GetCurrentBackColor()
        {
            if (isPressed)
            {
                return PressedBackColor;
            }

            if (isHovered && !Checked)
            {
                return HoverBackColor;
            }

            return Checked ? CheckedBackColor : NormalBackColor;
        }

        private void DrawStateDot(Graphics canvas)
        {
            Rectangle dotBounds = new Rectangle(9, Height / 2 - 4, 8, 8);
            if (Checked)
            {
                using (Brush dotBrush = new SolidBrush(CheckedTextColor))
                {
                    canvas.FillEllipse(dotBrush, dotBounds);
                }
            }
            else
            {
                using (Pen dotPen = new Pen(BorderColor, 1))
                {
                    canvas.DrawEllipse(dotPen, dotBounds);
                }
            }
        }
    }

    public class ThemePillLabel : Label
    {
        public ThemePillLabel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint,
                true);
            AutoSize = false;
            CornerRadius = 8;
            PillBackColor = Color.FromArgb(31, 43, 47);
            PillBorderColor = Color.FromArgb(44, 62, 66);
            PillTextColor = Color.FromArgb(225, 239, 235);
        }

        public int CornerRadius { get; set; }
        public Color PillBackColor { get; set; }
        public Color PillBorderColor { get; set; }
        public Color PillTextColor { get; set; }

        protected override void OnTextChanged(System.EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            ThemePaint.PaintParentBackground(this, canvas, BackColor, InvokePaintBackground, InvokePaint);

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            Color topColor = ThemeButton.Mix(PillBackColor, Color.White, 9);
            Color bottomColor = ThemeButton.Mix(PillBackColor, Color.Black, 8);
            using (GraphicsPath path = ThemeButton.CreateRoundedRectangle(bounds, CornerRadius))
            using (LinearGradientBrush backBrush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                canvas.FillPath(backBrush, path);

                if (PillBorderColor.A > 0)
                {
                    using (Pen borderPen = new Pen(PillBorderColor, 1))
                    {
                        canvas.DrawPath(borderPen, path);
                    }
                }
            }

            Rectangle textBounds = new Rectangle(Padding.Left, 0, System.Math.Max(0, Width - Padding.Left - Padding.Right), Height);
            TextRenderer.DrawText(
                canvas,
                Text,
                Font,
                textBounds,
                PillTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
