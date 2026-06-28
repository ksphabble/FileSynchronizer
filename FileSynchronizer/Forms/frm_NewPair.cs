using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public partial class frm_NewPair : Form
    {
        public string[] arr_Return_Message;
        private List<string> list_Return_Message;
        DataTable _DirPairDataTable;
        public bool bl_RefreshListRequired;

        public frm_NewPair()
        {
            InitializeComponent();
            list_Return_Message = new List<string> { };
            bl_RefreshListRequired = false;
        }

        public frm_NewPair(DataTable DirPairDataTable)
        {
            InitializeComponent();
            list_Return_Message = new List<string> { };
            bl_RefreshListRequired = false;
            _DirPairDataTable = DirPairDataTable;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSelectDIR1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择目录1路径";
            if (!String.IsNullOrEmpty(txtBoxDir1.Text))
            {
                dialog.SelectedPath = txtBoxDir1.Text;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtBoxDir1.Text = dialog.SelectedPath;
            }
        }

        private void btnSelectDIR2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择目录2路径";
            if (!String.IsNullOrEmpty(txtBoxDir2.Text))
            {
                dialog.SelectedPath = txtBoxDir2.Text;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtBoxDir2.Text = dialog.SelectedPath;
            }
        }

        private void btnConfirmed_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtBoxDir1.Text))
            {
                MessageBox.Show("无法添加：目录1不存在，请确认后重试");
                return;
            }

            if (!Directory.Exists(txtBoxDir2.Text))
            {
                MessageBox.Show("无法添加：目录2不存在，请确认后重试");
                return;
            }

            int int_PairNameChk = _DirPairDataTable.Select("PairName='" + txtBoxPairName.Text + "'").Length;
            if (int_PairNameChk > 0)
            {
                MessageBox.Show("无法添加：配对名称已经存在");
                return;
            }

            int int_PairDirChk = _DirPairDataTable.Select("DIR1='" + txtBoxDir1.Text + "' and DIR2='" + txtBoxDir2.Text + "'").Length;
            if (int_PairDirChk > 0)
            {
                MessageBox.Show("无法添加：请勿重复添加配对目录");
                return;
            }

            if ((radioButtonSyncInterval.Checked && String.IsNullOrEmpty(txtBoxSyncInterval.Text)) || (radioButtonFixedTime.Checked && String.IsNullOrEmpty(comBoxSyncDay.Text)) ||
                (!radioButtonSyncInterval.Checked && !radioButtonFixedTime.Checked && !checkBoxRealTimeSync.Checked))
            {
                MessageBox.Show("请填入正确的自动同步设置");
                return;
            }

            int int_SyncDirection = comboBox1.SelectedIndex < 0 ? 0 : comboBox1.SelectedIndex;
            bool bl_IsPaused = checkBoxPauseSync.Checked;
            string str_SyncInterval = txtBoxSyncInterval.Text;
            string str_AutoSyncFixedInterval = String.Join(@"|", comBoxSyncDay.SelectedIndex.ToString(), timePickerSyncTime.Text.ToString());

            if (checkBoxRealTimeSync.Checked)
            {
                str_SyncInterval = "0";
                str_AutoSyncFixedInterval = String.Empty;
            }
            else
            {
                int i_SyncInterval = Int32.MinValue;
                bool bConvertRet = Int32.TryParse(str_SyncInterval, out i_SyncInterval);
                if (radioButtonSyncInterval.Checked && (!bConvertRet || (bConvertRet && i_SyncInterval <= 0)))
                {
                    MessageBox.Show("请填入正确的自动同步设置");
                    return;
                }
                if (radioButtonSyncInterval.Checked)
                {
                    str_AutoSyncFixedInterval = String.Empty;
                }
                else if (radioButtonFixedTime.Checked)
                {
                    str_SyncInterval = String.Empty;
                }
            }

            if (Files_InfoDB.AddDirPair(txtBoxPairName.Text, txtBoxDir1.Text, txtBoxDir2.Text, txtBoxFilterRule.Text, str_SyncInterval, int_SyncDirection.ToString(), str_AutoSyncFixedInterval, bl_IsPaused ? 1 : 0))
            {
                list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + "添加配对(" + txtBoxPairName.Text + ")，目录1(" + txtBoxDir1.Text + ")，目录2(" + txtBoxDir2.Text + ")");
                //RefreshDirPair();
                bl_RefreshListRequired = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("无法添加：添加配对失败，请重试");
            }
        }

        private void frm_NewPair_FormClosing(object sender, FormClosingEventArgs e)
        {
            arr_Return_Message = list_Return_Message.ToArray();
            if (this.DialogResult != DialogResult.OK)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void radioButtonSyncInterval_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxSyncInterval.Enabled = true;
            comBoxSyncDay.Enabled = false;
            timePickerSyncTime.Enabled = false;
        }

        private void radioButtonFixedTime_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxSyncInterval.Enabled = false;
            comBoxSyncDay.Enabled = true;
            timePickerSyncTime.Enabled = true;
        }

        private void checkBoxRealTimeSync_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonSyncInterval.Visible = !checkBoxRealTimeSync.Checked;
            txtBoxSyncInterval.Visible = !checkBoxRealTimeSync.Checked;
            radioButtonFixedTime.Visible = !checkBoxRealTimeSync.Checked;
            comBoxSyncDay.Visible = !checkBoxRealTimeSync.Checked;
            timePickerSyncTime.Visible = !checkBoxRealTimeSync.Checked;
            checkBoxPauseSync.Visible = !checkBoxRealTimeSync.Checked;
            if (checkBoxRealTimeSync.Checked)
            {
                checkBoxPauseSync.Checked = false;
                txtBoxSyncInterval.Clear();
                comBoxSyncDay.SelectedIndex = -1;
            }
        }
    }
}
