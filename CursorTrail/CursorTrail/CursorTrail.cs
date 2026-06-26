using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CursorTrail
{
    public partial class CursorTrail : Form
    {
        private List<TrailPoint> trailPoints = new List<TrailPoint>();
        private Timer timer = new Timer();
        private const int MAX_TRAIL_LENGTH = 40;
        private const int DOT_SIZE = 10;
        private const int FADE_DURATION = 800; // 毫秒

        public CursorTrail()
        {
            InitializeComponent();

            // 窗体设置
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;

            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // 关键：添加 WS_EX_TRANSPARENT 样式，使鼠标消息穿透
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point clientPos = this.PointToClient(Cursor.Position);

            if (clientPos.X >= 0 && clientPos.Y >= 0 &&
                clientPos.X < this.ClientSize.Width &&
                clientPos.Y < this.ClientSize.Height)
            {
                if (trailPoints.Count == 0 || trailPoints[trailPoints.Count - 1].Location != clientPos)
                {
                    trailPoints.Add(new TrailPoint(clientPos, DateTime.Now));
                    if (trailPoints.Count > MAX_TRAIL_LENGTH)
                        trailPoints.RemoveAt(0);
                }
            }

            // 移除超时点
            DateTime now = DateTime.Now;
            trailPoints.RemoveAll(p => (now - p.BornTime).TotalMilliseconds > FADE_DURATION);

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 调试红点（确认绘制）
            using (SolidBrush debugBrush = new SolidBrush(Color.Red))
            {
                g.FillEllipse(debugBrush, 10, 10, 50, 50);
            }

            if (trailPoints.Count == 0) return;

            DateTime now = DateTime.Now;
            foreach (var tp in trailPoints)
            {
                double age = (now - tp.BornTime).TotalMilliseconds;
                int alpha = (int)(255 * (1 - age / FADE_DURATION));
                if (alpha < 0) alpha = 0;
                if (alpha == 0) continue;

                int size = (int)(DOT_SIZE * (0.3 + 0.7 * (alpha / 255.0)));
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 255, 0, 0)))
                {
                    Point p = tp.Location;
                    g.FillEllipse(brush, p.X - size / 2, p.Y - size / 2, size, size);
                }
            }
        }

        // 全面拦截鼠标消息，强制穿透
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int WM_MOUSEACTIVATE = 0x0021;
            const int WM_SETCURSOR = 0x0020;
            const int HTTRANSPARENT = -1;
            const int MA_NOACTIVATE = 0x0003; // 不激活窗口

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    m.Result = (IntPtr)HTTRANSPARENT;
                    return;

                case WM_MOUSEACTIVATE:
                    // 不激活窗口，并且穿透
                    m.Result = (IntPtr)MA_NOACTIVATE;
                    return;

                case WM_SETCURSOR:
                    // 不改变光标，让下层窗口处理
                    m.Result = (IntPtr)1; // 表示已处理
                    return;
            }

            base.WndProc(ref m);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.TopMost = true;
            this.BringToFront();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.TopMost = true;
        }

        private class TrailPoint
        {
            public Point Location { get; set; }
            public DateTime BornTime { get; set; }
            public TrailPoint(Point loc, DateTime born)
            {
                Location = loc;
                BornTime = born;
            }
        }
    }
}