using FO.CLS.DB;
using FO.CLS.LOG;
using FO.CLS.UTIL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main
{
    public partial class FormBase : Form
    {
        public Write log;

        public FOETC etc = new FOETC();

        public FormBase()
        {
            InitializeComponent();
        }


        public DataTable queryFromStigma(string sql)
        {
            DataTable r = new DataTable();

            MsSql db = new MsSql();

            try
            {
                if (db.Connect())
                {
                    r = db.Select(sql);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                db.Disconnect();
            }

            return r;
        }

        public DataTable queryFromStigmaWithWaitPnl(string sql)
        {
            DataTable r = new DataTable();

            FormPopup f = new FormPopup();

            try
            {
                etc.showPanelModaless(this, this, f.pnlWait);

                r = queryFromStigma(sql);
            }
            catch
            {
                throw;
            }
            finally
            {
                etc.closePanel(f.pnlWait);
            }

            return r;
        }

        public void nonQueryStigma(string sql)
        {
            MsSql db = new MsSql();

            try
            {
                if (db.Connect())
                {
                    db.beginTransaction();

                    db.TransactionNonQuery(sql);

                    db.commit();
                }
            }
            catch
            {
                db.rollback();
                throw;
            }
            finally
            {
                db.Disconnect();
            }
        }

        public void nonQueryStigmaWithPnl(string sql)
        {
            FormPopup f = new FormPopup();

            try
            {
                etc.showPanelModaless(this, this, f.pnlWait);

                nonQueryStigma(sql);
            }
            catch
            {
                throw;
            }
            finally
            {
                etc.closePanel(f.pnlWait);
            }
        }

        public float scale(float a1, float a2, float b1)
        {
            if (a1 == 0)
                return 0;

            return b1 * a2 / a1;
        }

        public byte[] fileToByteArray(string path)
        {
            byte[] rawData = new byte[0];

            try
            {
                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    UInt32 FileSize = (UInt32)fs.Length;

                    rawData = new byte[FileSize];
                    fs.Read(rawData, 0, (int)FileSize);
                    fs.Close();
                }
                else
                {
                    MessageBox.Show("no file : " + path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("imageToByteArray1 except : " + ex.ToString());
            }
            return rawData;
        }
        public List<string> getImageFileList(string path)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            FileInfo[] files = di.GetFiles("*.bmp");

            return files.Select(f => f.Directory + "\\" + f.Name).ToList();
        }



    }
}
