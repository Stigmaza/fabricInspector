using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace main
{
    public class clsPictureBoxRectangle
    {
        bool onMouseOver = false;

        bool dragOn = false;
        Point dragPositionStart;
        Point dragPositionEnd;
        Point positionMouse;

        public List<Rectangle> listRect = new List<Rectangle>();
        public Rectangle workRect = new Rectangle();

        public float scale(float a1, float b1, float a2)
        {
            if (a1 == 0)
                return 0;

            return b1 * a2 / a1;
        }


        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragOn = true;

                dragPositionStart = e.Location;
                dragPositionEnd = e.Location;
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            onMouseOver = true;

            positionMouse = e.Location;

            if (dragOn)
            {
                dragPositionEnd = e.Location;

                int x = Math.Min(dragPositionEnd.X, dragPositionStart.X);
                int y = Math.Min(dragPositionEnd.Y, dragPositionStart.Y);
                int width = Math.Abs(dragPositionEnd.X - dragPositionStart.X);
                int height = Math.Abs(dragPositionEnd.Y - dragPositionStart.Y);
                workRect = new Rectangle(x, y, width, height);
            }
        }

        public void MouseLeave(object sender, EventArgs e)
        {
            onMouseOver = false;
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (sender == null) return;

            PictureBox pbPreview = (PictureBox)sender;

            if (dragOn)
            {
                if (workRect.Width != 0 && workRect.Height != 0)
                {
                    if (pbPreview.Image != null)
                    {
                        int sx = (int)scale(pbPreview.Width, pbPreview.Image.Width, workRect.X);
                        int sy = (int)scale(pbPreview.Height, pbPreview.Image.Height, workRect.Y);
                        int sw = (int)scale(pbPreview.Width, pbPreview.Image.Width, workRect.Width);
                        int sh = (int)scale(pbPreview.Height, pbPreview.Image.Height, workRect.Height);

                        workRect.X = sx;
                        workRect.Y = sy;
                        workRect.Width = sw;
                        workRect.Height = sh;
                    }

                    listRect.Add(workRect);

                    workRect = new Rectangle();
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < listRect.Count; i++)
                {
                    Point t = e.Location;

                    if (pbPreview.Image != null)
                    {
                        int sx = (int)scale(pbPreview.Width, pbPreview.Image.Width, t.X);
                        int sy = (int)scale(pbPreview.Height, pbPreview.Image.Height, t.Y);

                        t = new Point(sx, sy);
                    }

                    if (listRect[i].Contains(t))
                    {
                        listRect.RemoveAt(i);
                        break;
                    }
                }
            }

            dragOn = false;
        }


        public void Paint(object sender, PaintEventArgs e)
        {
            if (sender == null) return;

            PictureBox pbPreview = (PictureBox)sender;

            Graphics g = e.Graphics;

            if (dragOn == false)
            {
                if (onMouseOver)
                {
                    g.DrawLine(Pens.Red, 0, positionMouse.Y, pbPreview.Width, positionMouse.Y);

                    g.DrawLine(Pens.Red, positionMouse.X, 0, positionMouse.X, pbPreview.Height);
                }
            }

            if (workRect.Width != 0 && workRect.Height != 0)
                g.DrawRectangle(Pens.Red, workRect);

            if (listRect.Count > 0)
            {
                for (int i = 0; i < listRect.Count; i++)
                {
                    Rectangle temp = listRect[i];

                    if (pbPreview.Image != null)
                    {
                        int sx = (int)scale(pbPreview.Image.Width, pbPreview.Width, temp.X);
                        int sy = (int)scale(pbPreview.Image.Height, pbPreview.Height, temp.Y);

                        int sw = (int)scale(pbPreview.Image.Width, pbPreview.Width, temp.Width);
                        int sh = (int)scale(pbPreview.Image.Height, pbPreview.Height, temp.Height);

                        temp = new Rectangle(sx, sy, sw, sh);
                    }

                    g.DrawRectangle(Pens.Red, temp);
                }
            }
        }
    }
}
