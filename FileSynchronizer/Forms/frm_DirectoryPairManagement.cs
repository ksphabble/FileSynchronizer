using Common.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public partial class frm_DirectoryPairManagement : Form
    {
        public string[] arr_Return_Message;
        private List<string> list_Return_Message;
        const string str_FSBackup = @"_FSBackup";
        DataTable _DirPairDataTable;
        public bool bl_RefreshListRequired;

        public frm_DirectoryPairManagement()
        {
            InitializeComponent();
            list_Return_Message = new List<string> { };
            bl_RefreshListRequired = false;
        }

        private void DirectoryPairManager_Load(object sender, EventArgs e)
        {
            RefreshDirPair();
        }

        private void btnNewPair_Click(object sender, EventArgs e)
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

            int int_SyncDirection = comboBox1.SelectedIndex < 0 ? 0 : comboBox1.SelectedIndex;
            if (cls_Files_InfoDB.AddDirPair(txtBoxPairName.Text, txtBoxDir1.Text, txtBoxDir2.Text, txtBoxFilterRule.Text, txtBoxSyncInterval.Text, int_SyncDirection.ToString()))
            {
                list_Return_Message.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " --- " + "添加配对(" + txtBoxPairName.Text + ")，目录1(" + txtBoxDir1.Text + ")，目录2(" + txtBoxDir2.Text + ")");
                RefreshDirPair();
                bl_RefreshListRequired = true;
            }
        }

        private void RefreshDirPair()
        {
            if (_DirPairDataTable != null)
            {
                _DirPairDataTable.Reset();
            }
            _DirPairDataTable = cls_Files_InfoDB.GetDirPairInfor(String.Empty);
            if (_DirPairDataTable == null)
            {
                MessageBox.Show("获取配对发生错误，可能是数据库被占用，请稍后再试或者点击刷新");
                return;
            }

            if (_DirPairDataTable.Rows.Count > 0)
            {
                dataGridView1.DataSource = _DirPairDataTable;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "配对名称";
                dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[2].HeaderText = "目录1";
                dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[3].HeaderText = "目录2";
                dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[4].HeaderText = "最后一次同步时间";
                dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[5].HeaderText = "最后一次同步状态";
                dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[8].Visible = false;
                dataGridView1.Columns[9].HeaderText = "暂停自动同步";
                dataGridView1.Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDirPair();
        }

        private void btnDelPair_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount < 1)
            {
                return;
            }

            string str_PairName = dataGridView1.SelectedRows[0].Cells["PAIRNAME"].Value.ToString();
            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();

            cls_Files_InfoDB.DelDirPair(str_PairName, str_PairDir1, str_PairDir2);
            list_Return_Message.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " --- " + "删除配对(" + txtBoxPairName.Text + ")");
            RefreshDirPair();
            bl_RefreshListRequired = true;
        }

        private void btnUpdPair_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount < 1)
            {
                return;
            }

            string str_PairID = dataGridView1.SelectedRows[0].Cells["PK_PairID"].Value.ToString();
            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            if (!txtBoxDir1.Text.Equals(str_PairDir1) || !txtBoxDir2.Text.Equals(str_PairDir2))
            {
                MessageBox.Show("无法修改已添加配对的目录，请删除配对后重新添加");
                return;
            }

            string str_outputMsg = String.Empty;
            cls_Files_InfoDB.UpdatePairInfor(str_PairID, txtBoxFilterRule.Text, txtBoxSyncInterval.Text, comboBox1.SelectedIndex.ToString(), out str_outputMsg);
            list_Return_Message.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " --- " + "更改配对(" + txtBoxPairName.Text + ")");
            if (!String.IsNullOrEmpty(str_outputMsg))
            {
                list_Return_Message.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " --- " + str_outputMsg);
            }
            RefreshDirPair();
            bl_RefreshListRequired = true;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count.Equals(0)) return;

            string str_PairName = dataGridView1.SelectedRows[0].Cells["PAIRNAME"].Value.ToString();
            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            string str_FilterRule = dataGridView1.SelectedRows[0].Cells["FilterRule"].Value.ToString();
            string str_AutoSyncInterval = dataGridView1.SelectedRows[0].Cells["AutoSyncInterval"].Value.ToString();
            string str_SyncDirection = dataGridView1.SelectedRows[0].Cells["SyncDirection"].Value.ToString();
            
            txtBoxPairName.Text = str_PairName;
            txtBoxDir1.Text = str_PairDir1;
            txtBoxDir2.Text = str_PairDir2;
            txtBoxFilterRule.Text = str_FilterRule;
            txtBoxSyncInterval.Text = str_AutoSyncInterval;
            if (!String.IsNullOrEmpty(str_SyncDirection))
            {
                comboBox1.SelectedIndex = Convert.ToInt32(str_SyncDirection);
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }

            try
            {
                CalcBKSpace(str_PairDir1, str_PairDir2);
                cls_LogPairFile cls_LogPairFile = new cls_LogPairFile(str_PairName, false);
                lblPairLogSize.Text = FileHelper.CalcFileSizeStr(cls_LogPairFile.LogFileFullName());
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void DirectoryPairManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            arr_Return_Message = list_Return_Message.ToArray();
        }

        private void CalcBKSpace(string str_PairDir1, string str_PairDir2)
        {
            string str_PairDir1BK = Path.Combine(str_PairDir1, str_FSBackup);
            string str_PairDir2BK = Path.Combine(str_PairDir2, str_FSBackup);
            lblDir1BackupSpace.Text = FileHelper.CalcDirSizeStr(str_PairDir1BK);
            lblDir2BackupSpace.Text = FileHelper.CalcDirSizeStr(str_PairDir2BK);
        }

        private void btnClearDir1BK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count.Equals(0)) return;

            if (MessageBox.Show("此操作将会永久清除备份目录里的所有内容，确定？", "提示", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            string str_PairDir1BK = Path.Combine(str_PairDir1, str_FSBackup);
            FileHelper.DeleteDirectoryOrFile(str_PairDir1BK, true);

            //DirectoryInfo directoryInfo1 = new DirectoryInfo(str_PairDir1BK);
            //if (directoryInfo1.Exists)
            //{
            //    directoryInfo1.Delete(true);
            //}

            CalcBKSpace(str_PairDir1, str_PairDir2);
        }

        private void btnClearDir2BK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count.Equals(0)) return;

            if (MessageBox.Show("此操作将会永久清除备份目录里的所有内容，确定？", "提示", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            string str_PairDir2BK = Path.Combine(str_PairDir2, str_FSBackup);
            FileHelper.DeleteDirectoryOrFile(str_PairDir2BK, true);

            //DirectoryInfo directoryInfo2 = new DirectoryInfo(str_PairDir2BK);
            //if (directoryInfo2.Exists)
            //{
            //    directoryInfo2.Delete(true);
            //}

            CalcBKSpace(str_PairDir1, str_PairDir2);
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

        private void btnClearPairLog_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count.Equals(0) || String.IsNullOrEmpty(txtBoxPairName.Text)) return;

            string str_PairName = txtBoxPairName.Text;
            cls_LogPairFile cls_LogPairFile = new cls_LogPairFile(str_PairName, true);
            lblPairLogSize.Text = FileHelper.CalcFileSizeStr(cls_LogPairFile.LogFileFullName());
        }
    }
}
