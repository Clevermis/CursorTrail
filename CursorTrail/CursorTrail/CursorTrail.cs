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

        // 鼠标穿透（WS_EX_TRANSPARENT）
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

            if (trailPoints.Count == 0) return;

            DateTime now = DateTime.Now;

            // 如果只有一个点，画一个小圆点表示当前位置
            if (trailPoints.Count == 1)
            {
                var tp = trailPoints[0];
                double age = (now - tp.BornTime).TotalMilliseconds;
                int alpha = (int)(255 * (1 - age / FADE_DURATION));
                if (alpha < 0) alpha = 0;
                if (alpha > 0)
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 255, 0, 0)))
                    {
                        int size = 4;
                        g.FillEllipse(brush, tp.Location.X - size / 2, tp.Location.Y - size / 2, size, size);
                    }
                }
                return;
            }

            // 绘制拖尾线条（分段绘制，每段透明度根据两端点平均）
            using (Pen pen = new Pen(Color.Red, 2.5f))
            {
                for (int i = 0; i < trailPoints.Count - 1; i++)
                {
                    var p1 = trailPoints[i];
                    var p2 = trailPoints[i + 1];
                    double age1 = (now - p1.BornTime).TotalMilliseconds;
                    double age2 = (now - p2.BornTime).TotalMilliseconds;
                    int alpha1 = (int)(255 * (1 - age1 / FADE_DURATION));
                    int alpha2 = (int)(255 * (1 - age2 / FADE_DURATION));
                    if (alpha1 < 0) alpha1 = 0;
                    if (alpha2 < 0) alpha2 = 0;
                    if (alpha1 == 0 && alpha2 == 0) continue;
                    // 取两端点透明度的平均值
                    int avgAlpha = (alpha1 + alpha2) / 2;
                    if (avgAlpha < 0) avgAlpha = 0;
                    pen.Color = Color.FromArgb(avgAlpha, 255, 0, 0);
                    g.DrawLine(pen, p1.Location, p2.Location);
                }
            }

            // 在最新位置绘制一个小红点（亮红色），表示当前光标
            var last = trailPoints[trailPoints.Count - 1];
            double lastAge = (now - last.BornTime).TotalMilliseconds;
            int lastAlpha = (int)(255 * (1 - lastAge / FADE_DURATION));
            if (lastAlpha > 0)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(lastAlpha, 255, 50, 50)))
                {
                    int size = 6;
                    g.FillEllipse(brush, last.Location.X - size / 2, last.Location.Y - size / 2, size, size);
                }
            }
        }

        // 彻底鼠标穿透
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int WM_MOUSEACTIVATE = 0x0021;
            const int WM_SETCURSOR = 0x0020;
            const int HTTRANSPARENT = -1;
            const int MA_NOACTIVATE = 0x0003;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    m.Result = (IntPtr)HTTRANSPARENT;
                    return;
                case WM_MOUSEACTIVATE:
                    m.Result = (IntPtr)MA_NOACTIVATE;
                    return;
                case WM_SETCURSOR:
                    m.Result = (IntPtr)1;
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