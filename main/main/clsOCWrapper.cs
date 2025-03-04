using CCMCOMPARE;
using FO.CLS.UTIL;
using MvCamCtrl.NET;
using OCR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main
{
    public class clsOCWrapper
    {
        OceanCore oc = null;

        public string lastMsg = string.Empty;

        public RGB rgb = new RGB();
        public LAB lab = new LAB();

        public double[] spectrumNormal = new double[0];
        public double[] spectrumCali = new double[0];

        bool calibration = false;
        double maxRef = 0;


        CancellationTokenSource cancellationTokenSource;
        Thread threadHandleMain = null;

        public class RGB
        {
            public int r;
            public int g;
            public int b;

            public override string ToString()
            {
                return r.ToString() + ", " + g.ToString() + ", " + b.ToString();
            }
        }

        public class LAB
        {
            public int l;
            public int a;
            public int b;

            public override string ToString()
            {
                return l.ToString() + ", " + a.ToString() + ", " + b.ToString();
            }
        }

        public bool open()
        {
            string path = Environment.CurrentDirectory;

            oc = new OceanCore(path);

            int numberDevice = oc.OpenUSB();

            if (numberDevice == 0)
            {
                oc = null;

                lastMsg = "분광기를 찾을수 없습니다.";

                return false;
            }

            loadCalib();

            return true;
        }

        public void close()
        {
            if (isRunning())
            {
                stop();
            }

            oc.CloseAll();

            oc = null;
        }

        public void loadCalib()
        {
            SQLITEINI ini = new SQLITEINI("OC");

            string t = ini.ReadValue("maxRef", "0");
                       
            if (double.TryParse(t, out double val) == false)
            {
                val = 0;
            }

            maxRef = val;
        }

        public bool isOpened()
        {
            return oc != null;
        }


        public void saveCalib()
        {
            SQLITEINI ini = new SQLITEINI("OC");

            ini.WriteValue("maxRef", maxRef.ToString());
        }

        public bool start()
        {
            if (threadHandleMain != null)
            {
                lastMsg = "aleady start";

                return false;
            }

            cancellationTokenSource = new CancellationTokenSource();
            threadHandleMain = new Thread(() => threadMain(cancellationTokenSource.Token));
            threadHandleMain.IsBackground = true;
            threadHandleMain.Start();

            return true;
        }

        public bool stop()
        {
            bool stopSuccess = true;

            if (threadHandleMain != null)
            {
                cancellationTokenSource.Cancel();

                DateTime max = DateTime.Now.AddSeconds(5);

                while (threadHandleMain.IsAlive)
                {
                    Thread.Sleep(1);
                    Application.DoEvents();

                    if (max < DateTime.Now)
                    {
                        stopSuccess = false;
                        break;
                    }
                }
            }

            threadHandleMain = null;

            return stopSuccess;
        }

        public bool isRunning()
        {
            return threadHandleMain != null;
        }

        public void doCalibration()
        {
            calibration = true;
        }

        private void threadMain(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    int deviceIndex = 0;

                    double[] waves = oc.GetWavelengths(deviceIndex);

                    double[] data = oc.GetSpectrum(deviceIndex);

                    // ----------------------------------------------------

                    if (calibration)
                    {
                        calibration = false;

                        maxRef = data.OrderByDescending(x => Math.Abs(x)).First();

                        saveCalib();
                    }

                    spectrumNormal = data.ToArray();

                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = data[i] - maxRef;

                        if (data[i] < 0) data[i] = 0;
                    }

                    spectrumCali = data.ToArray();

                    // ----------------------------------------------------

                    // clsCCM 함수에 적용하기 위해 스펙트럼 추출, 400, 700, 10 고정

                    double[] filterdata = filterSpectrum(waves, data, 400, 700, 10);

                    filterdata = filterdata.Select(x => x * 3).ToArray();

                    double[] colorRGB = clsCCM.REFLtoRGB("D65-10", filterdata);
                    double[] colorLAB = clsCCM.REFLtoLAB("D65-10", filterdata);

                    rgb.r = (int)colorRGB[0];
                    rgb.g = (int)colorRGB[1];
                    rgb.b = (int)colorRGB[2];

                    lab.l = (int)colorLAB[0];
                    lab.a = (int)colorLAB[1];
                    lab.b = (int)colorLAB[2];

                    // ----------------------------------------------------


                }
                catch (Exception ex)
                {
                    lastMsg = ex.Message + Environment.NewLine + ex.StackTrace;                 
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }

        // --------------------------------------------------------------------------------

        Font font = new Font("Arial", 12);
        Brush brush = Brushes.Black;

        private void drawString(Graphics g, int x, int y, string text)
        {
            g.DrawString(text, font, brush, new PointF(x, y));
        }

        private double[] filterSpectrum(double[] waves, double[] data, double start, double max, double step)
        {
            int wavesCount = (int)((max - start) / step) + 1;

            int rIndex = 0;
            double[] r = new double[wavesCount];

            for (double i = start; i <= max; i += step, rIndex++)
            {
                int index = Array.IndexOf(waves, waves.OrderBy(x => Math.Abs(x - i)).First());

                double t = waves[index];

                r[rIndex] = data[index];
            }

            return r;
        }

        private void drawSpectrun(PictureBox pb, double[] waves, double[] data)
        {
            Graphics g = pb.CreateGraphics();

            g.Clear(Color.White);

            int wid = pb.Width;
            int hei = pb.Height;

            PointF[] points = new PointF[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                points[i] = new PointF((float)(i * wid / (data.Length - 1)), (float)(hei - data[i]));
            }

            g.DrawLines(Pens.Black, points);

            {
                string colorText = string.Empty;

                colorText = rgb.ToString();

                drawString(g, 10, 10, "RGB : " + colorText);

                colorText = lab.ToString();

                drawString(g, 10, 30, "LAB : " + colorText);
            }
        }


    }
}
