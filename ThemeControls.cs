using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SnakeGame
{
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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics canvas = pevent.Graphics;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.Clear(Parent == null ? BackColor : Parent.BackColor);

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = CreateRoundedRectangle(bounds, CornerRadius))
            using (Brush backBrush = new SolidBrush(GetCurrentBackColor()))
            {
                canvas.FillPath(backBrush, path);

                if (BorderColor.A > 0)
                {
                    using (Pen borderPen = new Pen(BorderColor, 1))
                    {
                        canvas.DrawPath(borderPen, path);
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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics canvas = pevent.Graphics;
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.Clear(Parent == null ? BackColor : Parent.BackColor);

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = ThemeButton.CreateRoundedRectangle(bounds, CornerRadius))
            using (Brush backBrush = new SolidBrush(GetCurrentBackColor()))
            using (Pen borderPen = new Pen(Checked ? CheckedBorderColor : BorderColor, 1))
            {
                canvas.FillPath(backBrush, path);
                canvas.DrawPath(borderPen, path);
            }

            TextRenderer.DrawText(
                canvas,
                Text,
                Font,
                bounds,
                Checked ? CheckedTextColor : NormalTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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
    }
}
