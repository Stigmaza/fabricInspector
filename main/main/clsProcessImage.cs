using CCMCOMPARE;
using FO.CLS.DB;
using FO.CLS.UTIL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static main.clsOCWrapper;

namespace main
{
    public class clsProcessImage
    {
        FOETC etc = new FOETC();

        string orderNo;
        string orderSeq;

        CancellationTokenSource cancellationTokenSource;
        Thread threadHandleMain = null;

        public string lastMsg = string.Empty;

        public clsProcessImage(string _orderNo, string _orderSeq)
        {
            orderNo = _orderNo;
            orderSeq = _orderSeq;
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

                while (threadHandleMain != null && threadHandleMain.IsAlive)
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

        private DataTable queryUnprocessedImage(MsSql db)
        {
            string sql = @"
                SELECT TOP 1 *
                  FROM TB_INSPECT_DETAIL
                 WHERE ORDERNO = :ORDERNO
                   AND ORDERSEQ = :ORDERSEQ
                   AND PROCESSED = '0'
                 ORDER BY CDATE
            ";

            sql = sql.Replace(":ORDERNO", etc.qs(orderNo));
            sql = sql.Replace(":ORDERSEQ", etc.qs(orderSeq));

            return db.Select(sql);
        }

        private void updateImageStatProcessed(MsSql db, string orderNo, string orderSeq, string itemSeq)
        {
            try
            {
                string r = string.Empty;

                string sql = @"
                    UPDATE TB_INSPECT_DETAIL
                       SET PROCESSED = '1'
                     WHERE ORDERNO = :ORDERNO
                       AND ORDERSEQ = :ORDERSEQ
                       AND ITEMSEQ = :ITEMSEQ
                ";

                db.beginTransaction();

                db.TransactionNonQuery(sql);

                db.commit();
            }
            catch (Exception e)
            {
                db.rollback();
            }
            finally
            {

            }
        }

        private Image loadImage(string path)
        {
            if (File.Exists(path))
                return Image.FromFile(path);

            return null;
        }


        private bool processImage(Image img)
        {
            return true;
        }


        private void threadMain(CancellationToken token)
        {
            MsSql db = new MsSql();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (db.Connect())
                    {
                        DataTable dt = queryUnprocessedImage(db);
                         
                        if (dt.Rows.Count > 0)
                        {
                            //string imgPath = dt.Rows[0]["IMGPATH"].ToString();
                            //string orderNo = dt.Rows[0]["ORDERNO"].ToString();
                            //string orderSeq = dt.Rows[0]["ORDERSEQ"].ToString();
                            //string itemSeq = dt.Rows[0]["ITEMSEQ"].ToString();

                            //Image img = loadImage(imgPath);

                            //if (processImage(img))
                            //{
                            //    updateImageStatProcessed(db, orderNo, orderSeq, itemSeq);
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastMsg = ex.Message + Environment.NewLine + ex.StackTrace;
                }
                finally
                {
                    db.Disconnect();

                    Thread.Sleep(100);
                }
            }

            threadHandleMain = null;
        }
    }
}
