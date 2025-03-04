using FO.CLS.DB;
using FO.CLS.LOG;
using FO.CLS.UTIL;
using MvCamCtrl.NET;
using OCR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace main
{
    public partial class FormMain : FormBase
    {
        LogOnce logCamera;

        clsHKLineScanCamera hlsc = new clsHKLineScanCamera();

        bool ocTestMode = false;
        bool ocRunMode = false;
        clsOCWrapper ocw = new clsOCWrapper();

        List<string> cameraList = new List<string>();

        FormSelectHistory selectHistory;

        DataTable defectList;

        clsDrawLab drawLab = new clsDrawLab();

        clsDrawFabric fpv = new clsDrawFabric();

        clsPictureBoxRectangle pbr = new clsPictureBoxRectangle();

        clsProcessImage processImage = null;

        public FormMain()
        {
            InitializeComponent();

            selectHistory = new FormSelectHistory(this, queryOrderDetail);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CenterToScreen();

            makeFabricImage();

            hlsc.findCamera();

            log = new Write();

            logCamera = new LogOnce("CAMERASTAT", "", log);
        }

        private void makeFabricImage()
        {
            float h = 0;
            
            //h = hlsc.scanLength / 1000;

            DataTable dtImageList = (DataTable)dataGridView1.DataSource;

            if (dtImageList == null || dtImageList.Rows.Count == 0)
            {
                h = 10;
            }
            else
            {
                h = (float)etc.toDoubleDef(dtImageList.Rows[0]["ENCPOSITIONMM"]);
            }            

            h = (float)Math.Ceiling(h);

            pbAllFabric.Image = fpv.makeFabricImage(pbAllFabric, h, defectList);
        }

        private void pbAllFabric_SizeChanged(object sender, EventArgs e)
        {
            makeFabricImage();
        }

        private bool checkValidOrderNo()
        {
            textBox1.Text = textBox1.Text.Trim();
            textBox2.Text = textBox2.Text.Trim();

            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty)
                return true;

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string orderNo = textBox1.Text;

            if (orderNo == string.Empty)
                orderNo = DateTime.Now.ToString("yyMMdd");

            string sql = @"
                SELECT ISNULL(MAX(ORDERSEQ),0) AS SEQ
                  FROM TB_INSPECT_ORDER
                 WHERE ORDERNO = :ORDERNO 
            ";

            sql = sql.Replace(":ORDERNO", etc.qs(orderNo));

            DataTable dt = queryFromStigmaWithWaitPnl(sql);

            if (dt.Rows.Count == 1)
            {
                int t = etc.toIntDef(dt.Rows[0]["SEQ"]);

                textBox1.Text = orderNo;
                textBox2.Text = (t + 1).ToString();
            }
            else
            {
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
            }

            if (checkValidOrderNo())
            {
                sql = @"
                    insert 
                      into TB_INSPECT_ORDER 
                         ( ORDERNO, ORDERSEQ, CDATE)
                    values
                         ( :ORDERNO
                         , :ORDERSEQ
                         , CONVERT(VARCHAR(19), GETDATE(), 120)
                         )
                ";

                sql = sql.Replace(":ORDERNO", etc.qs(textBox1.Text));
                sql = sql.Replace(":ORDERSEQ", etc.qs(textBox2.Text));

                nonQueryStigmaWithPnl(sql);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            selectHistory.dateFrom = dtpStart.Text + " 00:00:00";
            selectHistory.dateTo = dtpEnd.Text + " 23:59:59";

            selectHistory.Show();

            selectHistory.queryOrderHistory();
        }

        private void btnShowEnv_Click(object sender, EventArgs e)
        {
            FormSetup f = new FormSetup();

            f.ShowDialog();
        }



        private int countColumnByData(DataTable dt, string column, string data)
        {
            string q = ":column = :data";

            q = q.Replace(":column", column);
            q = q.Replace(":data", etc.qs(data));

            return dt.Select(q).ToList().Count;
        }

        private string makeCountText(DataTable dt, string data)
        {
            return string.Format("{0,6} : {1,5:N0}", data, countColumnByData(dt, "DEFECTTYPE", data));
        }

        private void updateDefectTypeCount(DataTable dt)
        {
            label4.Text = makeCountText(dt, "구멍");
            label5.Text = makeCountText(dt, "오염");
            label6.Text = makeCountText(dt, "불균염");
            label7.Text = makeCountText(dt, "경사");
            label8.Text = makeCountText(dt, "위사");
        }

        private DataTable queryOrderResultInline(string orderNo, string orderSeq)
        {
            string sql = @"
                SELECT A.*
                     , FORMAT(CAST((B.ITEMSEQ - 1)*90 AS FLOAT) / 1000, 'N2') as ENCPOSITIONMM
                     , B.CAMERANO
                  FROM TB_INSPECT_RESULT      A WITH(NOLOCK)
                  LEFT JOIN TB_INSPECT_DETAIL B WITH(NOLOCK) ON B.ORDERNO = A.ORDERNO
                                                            AND B.ORDERSEQ = A.ORDERSEQ
                                                            AND B.ITEMSEQ = A.ITEMSEQ
                 WHERE A.ORDERNO = :ORDERNO
                   AND A.ORDERSEQ = :ORDERSEQ
            ";

            sql = sql.Replace(":ORDERNO", etc.qs(orderNo));
            sql = sql.Replace(":ORDERSEQ", etc.qs(orderSeq));

            //return queryFromStigmaWithWaitPnl(sql);
            
            return queryFromStigma(sql);
        }

        private DataTable queryOrderImageInline(string orderNo, string orderSeq)
        {
            string sql = @"
                SELECT *
                     , FORMAT(CAST((ITEMSEQ - 1)*90 AS FLOAT) / 1000, 'N2') as ENCPOSITIONMM
                  FROM TB_INSPECT_DETAIL
                 WHERE ORDERNO = :ORDERNO
                   AND ORDERSEQ = :ORDERSEQ
                 ORDER BY ITEMSEQ desc
            ";

            sql = sql.Replace(":SCALEMM", clsDrawFabric.scaleMM.ToString());
            sql = sql.Replace(":ORDERNO", etc.qs(orderNo));
            sql = sql.Replace(":ORDERSEQ", etc.qs(orderSeq));
            
            //return  queryFromStigmaWithWaitPnl(sql);

            return queryFromStigma(sql);
        }

        private void queryOrderDetail(string orderNo, string orderSeq)
        {
            DataTable imageList = queryOrderImageInline(orderNo, orderSeq);

            defectList = queryOrderResultInline(orderNo, orderSeq);

            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            pbr.listRect.Clear();
            pbPreview.Image = null;
            etc.dataGridFillFromDataTable(dataGridView1, imageList, "ENCPOSITIONMM");
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            makeFabricImage();

            updateDefectTypeCount(defectList);

            textBox1.Text = orderNo;
            textBox2.Text = orderSeq;
        }



        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            FormPopup f = new FormPopup();

            try
            {
                etc.showPanelModaless(this, this, f.pnlWait);

                int index = etc.getGridSelectedDataGridIndex(sender);

                if (index == -1) return;

                string imgpath = etc.getGridSelectedColumnData(sender, "IMGPATH");

                loadDefectResult(textBox1.Text, textBox2.Text, imgpath);
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

        private void loadDefectResult(string orderno, string orderseq, string imgpath)
        {
            pbr.listRect.Clear();

            pbPreview.Image = Bitmap.FromFile(imgpath);

            string sql = @"
                SELECT A.*
                     , FORMAT(CAST((B.ITEMSEQ - 1)*90 AS FLOAT) / 1000, 'N2') as ENCPOSITIONMM
                     , B.CAMERANO
                  FROM TB_INSPECT_RESULT      A WITH(NOLOCK)
                  LEFT JOIN TB_INSPECT_DETAIL B WITH(NOLOCK) ON B.ORDERNO = A.ORDERNO
                                                            AND B.ORDERSEQ = A.ORDERSEQ
                                                            AND B.ITEMSEQ = A.ITEMSEQ
                 WHERE A.ORDERNO = :ORDERNO
                   AND A.ORDERSEQ = :ORDERSEQ
                   AND A.ITEMSEQ = :ITEMSEQ
            ";

            sql = sql.Replace(":ORDERNO", etc.qs(orderno));
            sql = sql.Replace(":ORDERSEQ", etc.qs(orderseq));
            sql = sql.Replace(":ITEMSEQ", etc.qs(etc.getGridSelectedColumnData(dataGridView1, "ITEMSEQ")));

            DataTable dt = queryFromStigma(sql);

            etc.dataGridFillFromDataTable(dgvDetail, dt, "ENCPOSITIONMM, X, Y, WIDTH, HEIGHT, AREA, DEFECTTYPE");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int x = etc.toIntDef(dt.Rows[i]["X"]);
                int y = etc.toIntDef(dt.Rows[i]["Y"]);
                int wid = etc.toIntDef(dt.Rows[i]["WIDTH"]);
                int hei = etc.toIntDef(dt.Rows[i]["HEIGHT"]);

                pbr.listRect.Add(new Rectangle(x, y, wid, hei));
            }
        }

        private void pbPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (hlsc.isRun()) return;

            pbr.MouseDown(sender, e);
        }

        private void pbPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (hlsc.isRun()) return;

            pbr.MouseMove(sender, e);

            pbPreview.Invalidate();
        }

        private void pbPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (hlsc.isRun()) return;

            pbr.MouseUp(sender, e);

            pbPreview.Invalidate();
        }

        private void pbPreview_Paint(object sender, PaintEventArgs e)
        {
            if (hlsc.isRun()) return;

            pbr.Paint(sender, e);
        }

        private void pbPreview_MouseLeave(object sender, EventArgs e)
        {
            if (hlsc.isRun()) return;

            pbr.MouseLeave(sender, e);

            pbPreview.Invalidate();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (sender == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgv.RowCount) return;

            DataTable dt = (DataTable)dgv.DataSource;

            int defectCount = etc.toIntDef(dt.Rows[e.RowIndex]["DEFECTCOUNT"]);

            if (defectCount > 0)
            {
                e.CellStyle.BackColor = Color.Red;
            }
        }

        private void pbAllFabric_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender == null) return;
            if (e.Button != MouseButtons.Left) return;
            if (dataGridView1.DataSource == null) return;

            DataTable dt = (DataTable)dataGridView1.DataSource;

            if (dt.Rows.Count == 0) return;

            PictureBox pbox = (PictureBox)sender;

            float fabricWidthPixel = pbox.Width - clsDrawFabric.marginLeft - clsDrawFabric.marginRight;
            float fabricHeightPixel = pbox.Height - clsDrawFabric.marginBottom - clsDrawFabric.marginTop;

            RectangleF r = new RectangleF(clsDrawFabric.marginLeft, clsDrawFabric.marginTop, fabricWidthPixel, fabricHeightPixel);

            if (r.Contains(e.Location))
            {
                int y = (int)Math.Ceiling(scale(r.Height, fpv.fabricHeightM, r.Height - (e.Y - r.Top)));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double t = etc.toDoubleDef(dt.Rows[i]["ENCPOSITIONMM"]);

                    if (t < y)
                    {
                        etc.ScrollDataGridView(dataGridView1, i);
                        break;
                    }
                }

            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ocw.stop();

            hlsc.grabStop();

            hlsc.disconnectCamera();
        }

        int dl = 0;
        int da = 0;
        int db = 0;

        private void pbab_Paint(object sender, PaintEventArgs e)
        {
            drawLab.drawAB((PictureBox)sender, e.Graphics, "labSpace.bmp", ocw.lab.a, ocw.lab.b);
        }

        private void pbl_Paint(object sender, PaintEventArgs e)
        {
            drawLab.drawL((PictureBox)sender, e.Graphics, ocw.lab.l);
        }

        private string getCameraIp()
        {
            SQLITEINI ini = new SQLITEINI("CAMERA_01");

            return ini.ReadValue("IP");
        }

        private string getImageRoot()
        {
            SQLITEINI ini = new SQLITEINI("CAMERA_01");

            return ini.ReadValue("IMAGE_ROOT");
        }

        private bool getTriggerMode()
        {
            SQLITEINI ini = new SQLITEINI("CAMERA_01");

            if (ini.ReadValue("TRIGGER_MODE") == "0")
                return false;

            return true;
        }

        private int getTriggerSource()
        {
            SQLITEINI ini = new SQLITEINI("CAMERA_01");

            return etc.toIntDef(ini.ReadValue("TRIGGER_SOURCE"));
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (ocTestMode)
            {
                ocTestMode = false;
                button4.Text = "테스트 시작";
            }

            if (ocw.isOpened())
            {
                ocw.stop();
                ocw.close();
            }


            if (checkValidOrderNo() == false)
            {
                MessageBox.Show("작업번호가 올바르지 않습니다.");
                return;
            }

            if (button9.Text == "시작")
            {
                forderno = textBox1.Text;
                forderseq = textBox2.Text;

                button9.Text = "중지";

                //timer2.Enabled = true;
            }
            else
            {
                button9.Text = "시작";

                //timer2.Enabled = false;
            }




            if (hlsc.isRun() == false)
            {
                if (ocw.open() == false)
                {
                    MessageBox.Show(ocw.lastMsg);

                    return;
                }

                queryOrderDetail(forderno, forderseq);

                string cameraIp = getCameraIp();
                bool triggerMode = getTriggerMode();
                int triggerSource = getTriggerSource();

                if (hlsc.connectCamera(cameraIp, triggerMode, triggerSource) == false)
                {
                    MessageBox.Show(hlsc.lastErrorMsg);

                    return;
                }

                //queryOrderDetail(textBox1.Text, textBox2.Text);

                hlsc.setPictureBox(pbPreview);

                string path = getImageRoot() + "\\" + textBox1.Text + "-" + textBox2.Text;

                hlsc.setSavePath(path);

                hlsc.saveImageStart();

                hlsc.setAppendImage(appendImage);

                hlsc.makeScanImageBuffer();

                hlsc.grabStart();

                button9.Text = "중지";

                // ---------------------------

                ocRunMode = true;

                ocw.start();

                processImage = new clsProcessImage(textBox1.Text, textBox2.Text);

                processImage.start();
            }
            else
            {

                ocRunMode = false;

                processImage.stop();

                hlsc.grabStop();

                hlsc.disconnectCamera();

                button9.Text = "시작";
            }
        }

        private void updateLab(string orderNo, string orderSeq, clsOCWrapper.LAB lab)
        {
            try
            {
                string sql = @"
                    UPDATE TB_INSPECT_DETAIL
                       SET L = :L
                         , A = :A
                         , B = :B
                     WHERE ORDERNO = :ORDERNO
                       AND ORDERSEQ = :ORDERSEQ
                       AND ITEMSEQ = (SELECT MAX(ITEMSEQ) FROM TB_INSPECT_DETAIL WHERE ORDERNO = :ORDERNO AND ORDERSEQ = :ORDERSEQ )
                       AND L IS NULL 
                       AND A IS NULL 
                       AND B IS NULL 
                ";

                sql = sql.Replace(":ORDERNO", etc.qs(orderNo));
                sql = sql.Replace(":ORDERSEQ", etc.qs(orderSeq));

                sql = sql.Replace(":L", etc.qs(lab.l.ToString()));
                sql = sql.Replace(":A", etc.qs(lab.a.ToString()));
                sql = sql.Replace(":B", etc.qs(lab.b.ToString()));

                nonQueryStigma(sql);
            }
            catch (Exception e)
            {
                log.WriteLog("updateLab : " + e.Message);
            }
            finally
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            try
            {
                string frameNo = hlsc.frameNo.ToString("N0");

                logCamera.log(hlsc.lastErrorMsg + " : " + frameNo);

                //toolStripStatusLabel1.Text = "카메라 상태 : " + hlsc.lastErrorMsg;
                //toolStripStatusLabel2.Text = "frameNo : " + hlsc.frameNo.ToString("N0");
                //toolStripStatusLabel3.Text = (hlsc.scanLength / 1000).ToString("#,##0.0") + " m";

                toolStripStatusLabel1.Text = "카메라 상태 : OK";
                //toolStripStatusLabel2.Text = "frameNo : " + limit.ToString("N0");
                //toolStripStatusLabel3.Text = ((float)limit * 90/ 1000).ToString("#,##0.0") + " m";

                if (ocRunMode)
                {
                    updateLab(textBox1.Text, textBox2.Text, ocw.lab);
                }

                if (ocTestMode)
                {
                    label9.Text = ocw.rgb.ToString() + Environment.NewLine + ocw.lab.ToString();

                    pbab.Invalidate(true);
                    pbl.Invalidate(true);
                }
            }
            catch (Exception ex) 
            {
                log.WriteLog("timer1_Tick : " + ex.Message);
            }
            finally
            {
                timer1.Enabled = true;
            }
        }

        private void appendImage(string path)
        {
            try
            {
                string sql = @"
                    insert 
                      into TB_INSPECT_DETAIL 
                         ( ORDERNO, ORDERSEQ, ITEMSEQ, PROCESSED, CAMERANO
                         , CDATE, DEFECTCOUNT, IMGPATH)
                    values
                         ( :ORDERNO
                         , :ORDERSEQ
                         , (SELECT ISNULL(MAX(ITEMSEQ),0) + 1 FROM TB_INSPECT_DETAIL WHERE ORDERNO = :ORDERNO AND ORDERSEQ = :ORDERSEQ)
                         , '0'
                         , '1'
                         , CONVERT(VARCHAR(19), GETDATE(), 120)
                         , '0'
                         , :IMGPATH
                         )
                ";

                sql = sql.Replace(":ORDERNO", etc.qs(textBox1.Text));
                sql = sql.Replace(":ORDERSEQ", etc.qs(textBox2.Text));
                sql = sql.Replace(":IMGPATH", etc.qs(path));

                nonQueryStigma(sql);

                // ----------------------------------------------------------

                dataGridView1.Invoke(new Action(() =>
                {
                    DataTable dt = (DataTable)dataGridView1.DataSource;

                    DataRow dr = dt.NewRow();

                    dr["ENCPOSITIONMM"] = ((float)(dt.Rows.Count * 90) / 1000).ToString("N2");
                    dr["IMGPATH"] = path;

                    dt.Rows.InsertAt(dr, 0);

                    pbab.Invalidate();
                    pbl.Invalidate();
                }));
            }
            catch
            {

            }
            finally
            {
                if (hlsc.isRun())
                    makeFabricImage();
            }
        }

        string forderno;
        string forderseq;

        Random random = new Random();

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                timer2.Enabled = false;

                queryOrderDetail(forderno, forderseq);

                textBox1.Text = "250114";
                textBox2.Text = "5";

                DataTable dt = ( DataTable )dataGridView1.DataSource;

                string imgpath = dt.Rows[0]["IMGPATH"].ToString();

                dataGridView1.ClearSelection();
                dataGridView1.Rows[0].Selected = true;
                loadDefectResult(forderno, forderseq,imgpath);

                dl = random.Next(-10, 10);
                da = random.Next(-10, 10);
                db = random.Next(-10, 10);

                pbab.Invalidate();
                pbl.Invalidate();

            }
            catch
            {

            }
            finally
            {
                timer2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ocw.doCalibration();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ocw.isRunning() == false)
            {
                if (ocw.open() == false)
                {
                    MessageBox.Show(ocw.lastMsg);

                    return;
                }

                ocw.start();

                ocTestMode = true;

                button4.Text = "테스트 중지";
            }
            else
            {
                ocTestMode = false;

                button4.Text = "테스트 시작";

                ocw.stop();
                ocw.close();
            }

        }
    }
}
