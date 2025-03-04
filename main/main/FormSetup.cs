using FO.CLS.DB;
using FO.CLS.UTIL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace main
{
    public partial class FormSetup : FormBase
    {
        public FormSetup()
        {
            InitializeComponent();
        }

        private void FormSetup_Load(object sender, EventArgs e)
        {
            CenterToParent();

            loadSet();

            loadCameraList();
        }

        private void loadSet()
        {
            SQLITEINI ini = new SQLITEINI(Program.DB_STIGMA);

            textBox1.Text = ini.ReadValue("IP");
            textBox2.Text = ini.ReadValue("PORT");
            textBox3.Text = ini.ReadValue("DB");
            textBox4.Text = ini.ReadValue("ID");
            textBox5.Text = ini.ReadValue("PW");

        }

        private void saveSet()
        {
            SQLITEINI ini = new SQLITEINI(Program.DB_STIGMA);

            ini.WriteValue("IP", textBox1.Text);
            ini.WriteValue("PORT", textBox2.Text);
            ini.WriteValue("DB", textBox3.Text);
            ini.WriteValue("ID", textBox4.Text);
            ini.WriteValue("PW", textBox5.Text);

            ini.WriteValue("IMAGE_ROOT", textBox6.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveSet();

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MsSql db = new MsSql(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);

            if (db.connectTest())
            {
                MessageBox.Show("연결성공");
            }
            else
            {
                MessageBox.Show("연결실패");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            loadCameraList();
        }

        clsHKLineScanCamera hlsc = new clsHKLineScanCamera();

        private void loadCameraList()
        {
            comboBox1.Items.Clear();

            List<string> cameraInfoList = hlsc.findCamera();

            for (int i = 0; i < cameraInfoList.Count; i++)
            {
                comboBox1.Items.Add(cameraInfoList[i]);
            }

            selectCamera();
        }

        private void selectCamera()
        {
            SQLITEINI ini = new SQLITEINI("CAMERA_01");

            string cameraip = ini.ReadValue("IP");

            for (int i = 0; i < hlsc.cameraIpList.Count; i++)
            {
                if (hlsc.cameraIpList[i].IndexOf(cameraip) != -1)
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
            }

            comboBox2.SelectedIndex = etc.toIntDef(ini.ReadValue("TRIGGER_MODE"));
            comboBox3.SelectedIndex = etc.toIntDef(ini.ReadValue("TRIGGER_SOURCE"));

            textBox6.Text = ini.ReadValue("IMAGE_ROOT");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;

            if (index == -1)
            {
                MessageBox.Show("카메라를 선택하십시요");

                return;
            }

            SQLITEINI ini = new SQLITEINI("CAMERA_01");

            ini.WriteValue("IP", hlsc.cameraIpList[index]);

            ini.WriteValue("TRIGGER_MODE", comboBox2.SelectedIndex.ToString());
            ini.WriteValue("TRIGGER_SOURCE", comboBox3.SelectedIndex.ToString());

            ini.WriteValue("IMAGE_ROOT", textBox6.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog(); 

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;

                textBox6.Text = selectedPath;

            }

        }
    }
}
