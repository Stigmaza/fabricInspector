using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FO.CLS.UTIL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace main
{
    public class clsDrawFabric
    {
        FOETC etc = new FOETC();

        public const int scaleMM = 1000;
        public const int CAMERACOUNT = 1;

        public const int marginLeft = 80;       // 왼쪽 길이 표시할 영역
        public const int marginBottom = 100;    // 아래쪽 여백
        public const int marginTop = 20;       // 위쪽 여백
        public const int marginRight = 20;       // 오른쪽 여백

        public float fabricHeightM = 20;

        public Image makeFabricImage(PictureBox pbox, double fabricLengthM, DataTable dtFail)
        {
            if (pbox.Width == 0 || pbox.Height == 0) return null;

            Image img = new Bitmap(pbox.Width, pbox.Height);
            Graphics g = Graphics.FromImage(img);

            // 화면에 보여지는 원단의 실제 길이 - 길이는 고정이라 나중에는 비율로 계산
            double displayFabricLength = fabricLengthM;

            // 표시되는 최소 단위 길이
            int minimumLength = 10;

            displayFabricLength /= minimumLength;

            displayFabricLength = Math.Ceiling(displayFabricLength) * minimumLength;

            int[] failCount = new int[CAMERACOUNT];

            if (dtFail != null)
            {
                for (int i = 0; i < failCount.Length; i++)
                {
                    string q = "CAMERANO = :CAMERANO";
                    q = q.Replace(":CAMERANO", etc.qs((i + 1).ToString()));

                    DataTable t = etc.dataRowsToTable(dtFail.Select(q));

                    failCount[i] = t.Rows.Count;
                }
            }

            makeBackground(pbox, g, displayFabricLength, failCount, dtFail);

            fabricHeightM = (float)displayFabricLength;

            return img;
        }

        public void makeBackground(PictureBox pbox, Graphics g, double displayFabricLength, int[] failCount, DataTable dtFail)
        {
            g.Clear(Color.White);

            // ----------------------------------------------------------------
            // 약간 검은색으로 원단 처럼 사각형 그리기
            float fabricWidthPixel = pbox.Width - marginLeft - marginRight;
            float fabricHeightPixel = pbox.Height - marginBottom - marginTop;

            SolidBrush b = new SolidBrush(SystemColors.ButtonShadow);
            RectangleF r = new RectangleF(marginLeft, marginTop, fabricWidthPixel, fabricHeightPixel);

            g.FillRectangle(b, r);

            // ----------------------------------------------------------------
            // 세로선 그리기
            Pen penWhite = new Pen(Color.White);

            //float w = fabricWidthPixel;
            float h = fabricHeightPixel + marginTop;
            float camWidth = fabricWidthPixel / failCount.Length;

            for (int i = 0; i < failCount.Length - 1; i++)
            {
                g.DrawLine(penWhite, camWidth * (i + 1) + marginLeft, marginTop, camWidth * (i + 1) + marginLeft, h);
            }

            float verticalLineCount = 20f;

            // 가로선 그리기
            float perMeter = fabricHeightPixel / verticalLineCount;    // 가로선의 간격

            for (int i = 0; i < verticalLineCount; i++)
            {
                g.DrawLine(penWhite
                         , marginLeft, perMeter * i + marginTop
                         , marginLeft + fabricWidthPixel, perMeter * i + marginTop);
            }

            if (displayFabricLength == 0)
                return;

            // ----------------------------------------------------------------
            // 눈금, 미터 숫자 그리기

            Pen penBlack = new Pen(Color.Black);
            Font drawFont = new Font("굴림", 10);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // 눈금이랑 원단이 붙지 않도록 떨어트림
            int gaugeMargin = 5;
            int logLineLength = 10;

            float perMil = perMeter / 4;

            // 긴 눈금의 픽셀 간격
            double perLogLine = displayFabricLength / verticalLineCount;

            for (int i = 0; i <= verticalLineCount; i++)
            {
                // 긴 눈금
                g.DrawLine(penBlack
                         , marginLeft - gaugeMargin, perMeter * i + marginTop
                         , marginLeft - logLineLength - gaugeMargin, perMeter * i + marginTop);

                double m = displayFabricLength - i * perLogLine;
                string strToDraw;

                strToDraw = String.Format("{0:#,0}m", m);

                StringFormat format = new StringFormat() { Alignment = StringAlignment.Far };
                RectangleF rect = new RectangleF(0, perMeter * i + marginTop - (drawFont.Height / 2), marginLeft - gaugeMargin - logLineLength, 20);

                g.DrawString(strToDraw, drawFont, drawBrush, rect, format);

                // 짧은 눈금
                if (i < verticalLineCount)
                {
                    float top = perMeter * i;
                    for (int k = 1; k < 4; k++)
                    {
                        g.DrawLine(penBlack
                                 , marginLeft - gaugeMargin, perMil * k + top + marginTop
                                 , marginLeft - logLineLength - gaugeMargin + perMil, perMil * k + top + marginTop);
                    }
                }
            }

            int gapLine = 26;
            int marginBottomRegion = 18;

            // 카메라의 불량 개수 그리기
            {
                for (int i = 0; i < failCount.Length; i++)
                {
                    float xx = marginLeft + camWidth * i;
                    float yy = pbox.Height - marginBottom + marginBottomRegion;

                    // 카메라 번호
                    StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                    RectangleF rect = new RectangleF(xx, yy, camWidth, marginBottom);
                    string str = "CAM " + (i + 1).ToString();
                    g.DrawString(str, drawFont, drawBrush, rect, format);

                    // 결점 갯수
                    format = new StringFormat() { Alignment = StringAlignment.Center };
                    rect = new RectangleF(xx, yy + gapLine, camWidth, marginBottom);
                    str = failCount[i].ToString();
                    g.DrawString(str, drawFont, drawBrush, rect, format);
                }

                {
                    int tableLocationTop = pbox.Height - marginBottom + marginBottomRegion;

                    StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };
                    RectangleF rect = new RectangleF(0, tableLocationTop + gapLine, marginLeft, 20);
                    g.DrawString("결점 수", drawFont, drawBrush, rect, format);

                    rect = new RectangleF(0, tableLocationTop + gapLine * 2, marginLeft, 20);
                    g.DrawString("Total", drawFont, drawBrush, rect, format);

                    rect = new RectangleF(marginLeft, tableLocationTop + gapLine * 2, camWidth * failCount.Length, 20);
                    g.DrawString(failCount.Sum().ToString(), drawFont, drawBrush, rect, format);
                }
            }



            // 표 그리기
            {
                int tableMarginTop = 10;
                int tableMarginRight = 10;

                int tableLocationLeft = 10;
                int tableLocationTop = pbox.Height - marginBottom + tableMarginTop;

                int marginCell = 0;

                // 가로행
                for (int k = 1; k < 4; k++)
                {
                    g.DrawLine(penBlack
                             , tableLocationLeft, tableLocationTop + (k * gapLine) + marginCell
                             , pbox.Width - tableMarginRight, tableLocationTop + (k * gapLine) + marginCell);
                }

                // 표 중간 세로행 : 행높이 2
                for (int k = 1; k < failCount.Length; k++)
                {
                    float xx = marginLeft + camWidth * k;

                    g.DrawLine(penBlack
                             , xx, tableLocationTop + gapLine
                             , xx, tableLocationTop + gapLine * 2);
                }

                // 표 좌우에있는 행높이 3
                g.DrawLine(penBlack
                         , tableLocationLeft, tableLocationTop + gapLine
                         , tableLocationLeft, tableLocationTop + gapLine * 3);

                g.DrawLine(penBlack
                         , marginLeft, tableLocationTop + gapLine
                         , marginLeft, tableLocationTop + gapLine * 3);

                g.DrawLine(penBlack
                         , pbox.Width - tableMarginRight, tableLocationTop + gapLine
                         , pbox.Width - tableMarginRight, tableLocationTop + gapLine * 3);
            }


            if (dtFail != null)
            {
                Pen penBlue = new Pen(Color.Blue, 1);

                for (int i = 0; i < dtFail.Rows.Count; i++)
                {
                    double pos = etc.toDoubleDef(dtFail.Rows[i]["ENCPOSITIONMM"], 0);
                    string type = etc.toStrDef(dtFail.Rows[i]["DEFECTTYPE"]);

                    if (pos > 0)
                    {
                        float pixpos = (float)(fabricHeightPixel / displayFabricLength * pos);

                        int camIndex = 0;

                        float left = camWidth * camIndex + marginLeft + 3;
                        float width = camWidth - 6;

                        float top = fabricHeightPixel + marginTop - pixpos;

                        if (type == "L" || type == "A" || type == "B")
                        {
                            float mx = width - left;

                            mx = left + mx / 2 + 20;

                            g.FillEllipse(Brushes.Red, mx, top, 10, 10);
                        }
                        else
                        {
                            g.DrawLine(penBlue
                                    , left, top
                                    , left + width, top);
                        }


                        //failList.Add(new ErrorItemInfo(dtFail.Rows[i]["POSITION"].ToString()
                        //                              , dtFail.Rows[i]["CAMNO"].ToString()
                        //                              , (int)left
                        //                              , (int)(top - (penBlue.Width / 2) - 1)
                        //                              , (int)width
                        //                              , (int)(penBlue.Width + 1)));
                    }
                }
            }
        }
    }
}
