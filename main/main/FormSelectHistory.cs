using FO.CLS.DB;
using FO.CLS.UTIL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace main
{
    public partial class FormSelectHistory : FormBase
    {
        public delegate void QUERYORDERDETAIL(string orderNO, string orderSeq);

        public string dateFrom;
        public string dateTo;

        public FormBase formParent;

        QUERYORDERDETAIL queryOrderDetail;

        public FormSelectHistory(FormBase p, QUERYORDERDETAIL _queryOrderDetail)
        {
            InitializeComponent();

            formParent = p;
            queryOrderDetail = _queryOrderDetail;
        }

        private void FormSelectHistory_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }

        private void FormSelectHistory_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }

        public void queryOrderHistory()
        {
            string q = @"
				WITH SUB AS(
					SELECT ORDERNO
					     , ORDERSEQ
					     , (ITEMSEQ*90) ENCPOSITION
					  FROM TB_INSPECT_DETAIL WITH(NOLOCK)
					 GROUP BY ORDERNO, ORDERSEQ
				)				
                SELECT MAIN.*
                     , CONVERT(VARCHAR(10), CDATE, 120) AS CDATE24
                     , ISNULL(SUB.ENCPOSITION, 0) ENCPOSITION
                  FROM TB_INSPECT_ORDER 	MAIN WITH(NOLOCK)
                  LEFT JOIN SUB  ON SUB.ORDERNO = MAIN.ORDERNO
                                AND SUB.ORDERSEQ = MAIN.ORDERSEQ
                 WHERE CDATE >= :DATEFROM
                   AND CDATE <= :DATETO
                 ORDER BY CDATE DESC
            ";

            q = q.Replace(":DATEFROM", etc.qs(dateFrom));
            q = q.Replace(":DATETO", etc.qs(dateTo));

            DataTable dt = queryFromStigmaWithWaitPnl(q);

            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            etc.dataGridFillFromDataTable(dataGridView1, dt, "CDATE24, ORDERNO, ORDERSEQ, ENCPOSITION");
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int index = etc.getGridSelectedDataGridIndex(sender);

            if (index == -1) return;

            string orderNo = etc.getGridSelectedColumnData(sender, "ORDERNO");
            string orderSeq = etc.getGridSelectedColumnData(sender, "ORDERSEQ");

            queryOrderDetail(orderNo, orderSeq);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            formParent.BringToFront();
        }
    }
}
