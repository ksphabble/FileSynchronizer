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
            frm_NewPair _NewPair = new frm_NewPair(_DirPairDataTable);
            _NewPair.StartPosition = FormStartPosition.CenterParent;
            _NewPair.ShowDialog();
            if (_NewPair.DialogResult == DialogResult.OK)
            {
                this.bl_RefreshListRequired = _NewPair.bl_RefreshListRequired;
                this.list_Return_Message.AddRange(_NewPair.arr_Return_Message);
                RefreshDirPair();
            }

            #region v3.0.0.6 old code
            //if (!Directory.Exists(txtBoxDir1.Text))
            //{
            //    MessageBox.Show("无法添加：目录1不存在，请确认后重试");
            //    return;
            //}

            //if (!Directory.Exists(txtBoxDir2.Text))
            //{
            //    MessageBox.Show("无法添加：目录2不存在，请确认后重试");
            //    return;
            //}

            //int int_PairNameChk = _DirPairDataTable.Select("PairName='" + txtBoxPairName.Text + "'").Length;
            //if (int_PairNameChk > 0)
            //{
            //    MessageBox.Show("无法添加：配对名称已经存在");
            //    return;
            //}

            //int int_PairDirChk = _DirPairDataTable.Select("DIR1='" + txtBoxDir1.Text + "' and DIR2='" + txtBoxDir2.Text + "'").Length;
            //if (int_PairDirChk > 0)
            //{
            //    MessageBox.Show("无法添加：请勿重复添加配对目录");
            //    return;
            //}

            //int int_SyncDirection = comboBox1.SelectedIndex < 0 ? 0 : comboBox1.SelectedIndex;
            //if (Files_InfoDB.AddDirPair(txtBoxPairName.Text, txtBoxDir1.Text, txtBoxDir2.Text, txtBoxFilterRule.Text, txtBoxSyncInterval.Text, int_SyncDirection.ToString()))
            //{
            //    list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + "添加配对(" + txtBoxPairName.Text + ")，目录1(" + txtBoxDir1.Text + ")，目录2(" + txtBoxDir2.Text + ")");
            //    RefreshDirPair();
            //    bl_RefreshListRequired = true;
            //}
            #endregion
        }

        private void RefreshDirPair()
        {
            if (_DirPairDataTable != null)
            {
                _DirPairDataTable.Reset();
            }
            _DirPairDataTable = Files_InfoDB.GetDirPairInfor(String.Empty);
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

                if (Global_Settings.HasDBVersionReached(3011))
                {
                    dataGridView1.Columns[10].Visible = false;
                    dataGridView1.Columns[11].Visible = false;
                }
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

            Files_InfoDB.DelDirPair(str_PairName, str_PairDir1, str_PairDir2);
            list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + "删除配对(" + txtBoxPairName.Text + ")");
            RefreshDirPair();
            bl_RefreshListRequired = true;
        }

        private void btnUpdPair_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount < 1)
            {
                return;
            }

            string str_PairName = txtBoxPairName.Text;
            string str_PairID = dataGridView1.SelectedRows[0].Cells["PK_PairID"].Value.ToString();
            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            if (!txtBoxDir1.Text.Equals(str_PairDir1) || !txtBoxDir2.Text.Equals(str_PairDir2))
            {
                MessageBox.Show("无法修改已添加配对的目录，请删除配对后重新添加");
                return;
            }

            if (((radioButtonSyncInterval.Checked && String.IsNullOrEmpty(txtBoxSyncInterval.Text)) || (radioButtonFixedTime.Checked && String.IsNullOrEmpty(comBoxSyncDay.Text))) && !checkBoxRealTimeSync.Checked)
            {
                MessageBox.Show("请填入正确的自动同步设置");
                return;
            }

            string str_outputMsg = String.Empty;
            string str_SyncDirection = comboBox1.SelectedIndex.ToString();
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
                if (!bConvertRet || (bConvertRet && i_SyncInterval <= 0))
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
            bool bResult = Files_InfoDB.UpdatePairInfor(str_PairID, str_PairName, txtBoxFilterRule.Text, str_SyncInterval, str_SyncDirection, str_AutoSyncFixedInterval, out str_outputMsg);
            if (!bResult)
            {
                MessageBox.Show("更改配对失败，请检查输入并重试");
                return;
            }

            string str_IsPausedOld = dataGridView1.SelectedRows[0].Cells["IsPaused"].Value.ToString();
            bool bl_IsPausedOld = Boolean.Parse(str_IsPausedOld);
            if (!bl_IsPausedOld.Equals(checkBoxPauseSync.Checked))
            {
                Files_InfoDB.PausePairAutoSync(str_PairID, out str_outputMsg);
            }

            list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + "更改配对(" + str_PairName + ")");
            if (!String.IsNullOrEmpty(str_outputMsg))
            {
                list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + str_outputMsg);
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
            string str_AutoSyncFixedTime = dataGridView1.SelectedRows[0].Cells["AutoSyncFixedTime"].Value.ToString();
            string str_IsPaused = dataGridView1.SelectedRows[0].Cells["IsPaused"].Value.ToString();

            txtBoxPairName.Text = str_PairName;
            txtBoxDir1.Text = str_PairDir1;
            txtBoxDir2.Text = str_PairDir2;
            txtBoxFilterRule.Text = str_FilterRule;
            checkBoxPauseSync.Checked = Boolean.Parse(str_IsPaused);
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

                if (!String.IsNullOrEmpty(str_AutoSyncInterval))
                {
                    int i_AutoSyncInterval = Int32.Parse(str_AutoSyncInterval);
                    if (i_AutoSyncInterval.Equals(0))
                    {
                        checkBoxRealTimeSync.Checked = true;
                        checkBoxRealTimeSync_CheckedChanged(this, e);
                        txtBoxSyncInterval.Clear();
                    }
                    else
                    {
                        checkBoxRealTimeSync.Checked = false;
                        checkBoxRealTimeSync_CheckedChanged(this, e);

                        //radioButtonFixedTime.Checked = false;
                        txtBoxSyncInterval.Text = str_AutoSyncInterval;
                    }
                    radioButtonSyncInterval.Checked = true;
                    txtBoxSyncInterval.Enabled = true;
                    comBoxSyncDay.Enabled = false;
                    timePickerSyncTime.Enabled = false;
                    comBoxSyncDay.SelectedIndex = -1;
                }
                else if (!String.IsNullOrEmpty(str_AutoSyncFixedTime))
                {
                    checkBoxRealTimeSync.Checked = false;
                    checkBoxRealTimeSync_CheckedChanged(this, e);
                    string str_SyncDay = str_AutoSyncFixedTime.Split('|')[0];
                    string str_SyncTime = str_AutoSyncFixedTime.Split('|')[1];
                    //radioButtonSyncInterval.Checked = false;
                    radioButtonFixedTime.Checked = true;
                    comBoxSyncDay.Enabled = true;
                    comBoxSyncDay.SelectedIndex = Int32.Parse(str_SyncDay);
                    timePickerSyncTime.Enabled = true;
                    timePickerSyncTime.Text = str_SyncTime;
                    txtBoxSyncInterval.Clear();
                    txtBoxSyncInterval.Enabled = false;
                }
            }
            catch (Exception)
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
            string str_PairDir1BK = Path.Combine(str_PairDir1, Local_Utilities.c_FSBackup_Str);
            string str_PairDir2BK = Path.Combine(str_PairDir2, Local_Utilities.c_FSBackup_Str);
            lblDir1BackupSpace.Text = FileHelper.CalcDirSizeStr(str_PairDir1BK);
            lblDir2BackupSpace.Text = FileHelper.CalcDirSizeStr(str_PairDir2BK);
        }

        private void btnClearDir1BK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count.Equals(0)) return;

            if (MessageBox.Show("此操作将会永久清除备份目录里的所有内容，确定？", "提示", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            string str_PairDir1BK = Path.Combine(str_PairDir1, Local_Utilities.c_FSBackup_Str);

            try
            {
                FileHelper.xDelete(str_PairDir1BK, true);

                //DirectoryInfo directoryInfo1 = new DirectoryInfo(str_PairDir1BK);
                //if (directoryInfo1.Exists)
                //{
                //    directoryInfo1.Delete(true);
                //}
            }
            catch (Exception ex)
            {
                list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + "清除备份目录" + str_PairDir1BK + "出现错误：" + ex.Message);
            }

            CalcBKSpace(str_PairDir1, str_PairDir2);
        }

        private void btnClearDir2BK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count.Equals(0)) return;

            if (MessageBox.Show("此操作将会永久清除备份目录里的所有内容，确定？", "提示", MessageBoxButtons.YesNo) == DialogResult.No) return;

            string str_PairDir1 = dataGridView1.SelectedRows[0].Cells["DIR1"].Value.ToString();
            string str_PairDir2 = dataGridView1.SelectedRows[0].Cells["DIR2"].Value.ToString();
            string str_PairDir2BK = Path.Combine(str_PairDir2, Local_Utilities.c_FSBackup_Str);

            try
            {
                FileHelper.xDelete(str_PairDir2BK, true);

                //DirectoryInfo directoryInfo2 = new DirectoryInfo(str_PairDir2BK);
                //if (directoryInfo2.Exists)
                //{
                //    directoryInfo2.Delete(true);
                //}
            }
            catch (Exception ex)
            {
                list_Return_Message.Add(DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " + "清除备份目录" + str_PairDir2BK + "出现错误：" + ex.Message);
            }

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
