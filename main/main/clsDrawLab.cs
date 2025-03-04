using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main
{
    public class clsDrawLab
    {
        public float maxDL = 10;
        public float maxDAB = 1;

        Font fontArial = new Font("Arial", 8f);
        private void DrawString(Graphics g, float x, float y, string Str, Color myColor)
        {
            SolidBrush solidBrush = new SolidBrush(myColor);
            PointF point = new PointF(x, y);
            g.DrawString(Str, fontArial, (Brush)solidBrush, point);
        }

        public void drawAB(PictureBox pb, Graphics g, string pathBg, int da, int db)
        {
            if (pb == null) return;

            Bitmap bmp = new Bitmap(pathBg);

            g.DrawImage(bmp, 0, 0, pb.Width, pb.Height);

            Point[] points = new Point[2];
            points[0].X = 0;
            points[0].Y = pb.Height / 2;
            points[1].X = pb.Width;
            points[1].Y = pb.Height / 2;
            g.DrawLines(Pens.Black, points);
            points[0].X = pb.Width / 2;
            points[0].Y = 0;
            points[1].X = pb.Width / 2;
            points[1].Y = pb.Height;
            g.DrawLines(Pens.Black, points);


            float num1 = (float)pb.Width / (2f * maxDAB);
            float num2 = (float)-((double)pb.Height / (2.0 * (double)maxDAB));
            float num3 = (float)pb.Width / 2f;
            float num4 = (float)pb.Height / 2f;

            Font font = new Font("Arial", 8f);
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            SizeF sizeF2 = g.MeasureString("+3.0", font);
            float height1 = sizeF2.Height;
            float width1 = sizeF2.Width;
            if ((double)maxDAB == 1.0)
            {
                float x1 = (float)(pb.Width / 4);
                float y1 = (float)(pb.Height / 4);
                g.DrawRectangle(Pens.White, x1, y1, x1 * 2f, y1 * 2f);
                g.DrawEllipse(Pens.White, x1, y1, x1 * 2f, y1 * 2f);
                g.DrawEllipse(Pens.White, 0, 0, pb.Width, pb.Height);
                for (float num5 = -1f; (double)num5 <= 1.0; num5 += 0.5f)
                {
                    if ((double)num5 != 0.0)
                    {
                        float x2 = num3 + num5 * num1;
                        float y2 = num4;
                        if ((double)num5 == 1.0)
                            x2 -= width1;
                        DrawString(g, x2, y2, num5.ToString("0.0"), Color.Black);
                        float x3 = num3;
                        float y3 = num4 + num5 * num2;
                        if ((double)num5 == -1.0)
                            y3 -= height1;
                        DrawString(g, x3, y3, num5.ToString("0.0"), Color.Black);
                    }
                }
            }
            else
            {
                for (float num5 = 1f; (double)num5 <= (double)maxDAB; ++num5)
                {
                    float x1 = num3 - num5 * num1;
                    float y1 = num4 + num5 * num2;
                    float width2 = (float)((double)num5 * (double)num1 * 2.0);
                    float height2 = (float)-((double)num5 * (double)num2 * 2.0);
                    g.DrawEllipse(Pens.White, x1, y1, width2, height2);
                    if ((double)num5 == 1.0)
                        g.DrawRectangle(Pens.White, x1, y1, width2, height2);
                    float x2 = num3;
                    DrawString(g, x2, y1, num5.ToString("+0.0"), Color.Black);
                    float y2 = num4 - num5 * num2;
                    if ((double)num5 == (double)maxDAB)
                        y2 -= height1;
                    DrawString(g, x2, y2, num5.ToString("-0.0"), Color.Black);
                    float x3 = num3 - num5 * num1;
                    float y3 = num4;
                    DrawString(g, x3, y3, num5.ToString("-0.0"), Color.Black);
                    float x4 = num3 + num5 * num1;
                    if ((double)num5 == (double)maxDAB)
                        x4 -= width1;
                    DrawString(g, x4, y3, num5.ToString("+0.0"), Color.Black);
                }
            }

            {
                Brush br = Brushes.Blue;

                int px = (int)scale(pb.Width, 255, da);
                int py = (int)scale(pb.Height, 255, db);

                Point abPosition = new Point(pb.Width / 2 + px, pb.Height / 2 - py);
                int abRadius = 10;

                g.FillEllipse(br, abPosition.X - (int)(abRadius / 2), abPosition.Y - (int)(abRadius / 2), (int)abRadius, (int)abRadius);
            }
        }

        public void drawL(PictureBox pb, Graphics g, int dl)
        {
            if (pb == null) return;

            float heightSizeF2 = 0;
            float widthSizeF2 = 0;

            g.Clear(Color.White);
            float height = (float)pb.Height / (float)byte.MaxValue;
            for (float num11 = 0.0f; (double)num11 < (double)byte.MaxValue; ++num11)
            {
                int num22 = (int)((double)byte.MaxValue - (double)num11);
                float y = num11 * height;
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(num22, num22, num22));
                g.FillRectangle((Brush)solidBrush, 0.0f, y, (float)pb.Width, height);
            }


            if (heightSizeF2 == 0 && heightSizeF2 == 0)
            {
                Font fontGraphL = new Font("Arial", 8f);

                SizeF sizeF2 = g.MeasureString("+3.0", fontGraphL);
                heightSizeF2 = sizeF2.Height;
                widthSizeF2 = sizeF2.Width;

                fontGraphL.Dispose();
            }

            Pen pen1 = new Pen(Color.Black, 1f);
            Pen pen2 = new Pen(Color.White, 1f);
            Point[] pointArray = new Point[2];
            float num1 = (float)pb.Height / 2f;
            float num2 = (float)-pb.Height / (maxDL * 2f);
            if ((double)maxDL == 1.0)
            {
                this.DrawString(g, 0.0f, 0.0f, "+1.0", Color.Black);
                float y1 = (float)pb.Height / 4f;
                pointArray[0].X = 0;
                pointArray[0].Y = (int)y1;
                pointArray[1].X = pb.Width;
                pointArray[1].Y = pointArray[0].Y;
                g.DrawLine(pen1, pointArray[0], pointArray[1]);
                this.DrawString(g, 0.0f, y1, "+0.5", Color.Black);
                float y2 = (float)pb.Height / 2f;
                pointArray[0].X = 0;
                pointArray[0].Y = (int)y2;
                pointArray[1].X = pb.Width;
                pointArray[1].Y = pointArray[0].Y;
                g.DrawLine(pen2, pointArray[0], pointArray[1]);
                this.DrawString(g, 0.0f, y2, " 0.0", Color.Black);
                float y3 = (float)((double)pb.Height / 4.0 * 3.0);
                pointArray[0].X = 0;
                pointArray[0].Y = (int)y3;
                pointArray[1].X = pb.Width;
                pointArray[1].Y = pointArray[0].Y;
                g.DrawLine(pen2, pointArray[0], pointArray[1]);
                this.DrawString(g, 0.0f, y3, "-0.5", Color.White);
                this.DrawString(g, 0.0f, (float)pb.Height - heightSizeF2, "-1.0", Color.White);
            }
            else
            {
                for (float num3 = 0.0f; (double)num3 <= (double)maxDL; ++num3)
                {
                    if ((double)num3 == 0.0)
                    {
                        float y = num1;
                        pointArray[0].X = 0;
                        pointArray[0].Y = (int)num1;
                        pointArray[1].X = pb.Width;
                        pointArray[1].Y = pointArray[0].Y;
                        g.DrawLine(pen2, pointArray[0], pointArray[1]);
                        this.DrawString(g, 0.0f, y, " 0.0", Color.White);
                    }
                    else if ((double)num3 == (double)maxDL)
                    {
                        this.DrawString(g, 0.0f, num1 + num3 * num2, num3.ToString("+0.0"), Color.Black);
                        this.DrawString(g, 0.0f, num1 - num3 * num2 - heightSizeF2, num3.ToString("-0.0"), Color.White);
                    }
                    else
                    {
                        float y1 = num1 + num3 * num2;
                        pointArray[0].X = 0;
                        pointArray[0].Y = (int)y1;
                        pointArray[1].X = pb.Width;
                        pointArray[1].Y = pointArray[0].Y;
                        g.DrawLine(pen1, pointArray[0], pointArray[1]);
                        this.DrawString(g, 0.0f, y1, num3.ToString("+0.0"), Color.Black);
                        float y2 = num1 - num3 * num2;
                        pointArray[0].X = 0;
                        pointArray[0].Y = (int)y2;
                        pointArray[1].X = pb.Width;
                        pointArray[1].Y = pointArray[0].Y;
                        g.DrawLine(pen2, pointArray[0], pointArray[1]);
                        this.DrawString(g, 0.0f, y2, num3.ToString("-0.0"), Color.White);
                    }
                }
            }


            {
                Brush br = Brushes.Blue;

                int py = (int)scale(100, pb.Height, dl);

                Point lPosition = new Point(pb.Width / 2 + 10, pb.Height - py);
                int lRadius = 10;

                g.FillEllipse(br, lPosition.X - (int)(lRadius / 2), lPosition.Y - (int)(lRadius / 2), (int)lRadius, (int)lRadius);
            }
        }

        public float scale(float a1, float a2, float b1 )
        {
            if (a1 == 0)
                return 0;

            if (b1 >= a1)
                b1 = a1;

            return b1 * a2 / a1;
        }
    }
}
