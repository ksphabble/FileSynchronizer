using Common.Components;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Common.Components.Common_Constants;
using static FileSynchronizer.cls_Common_Constants;

namespace FileSynchronizer
{
    public partial class frm_FileSynchronizer : Form
    {
        #region 全局变量
        DataTable dt_DirPair;
        string str_MainProgramVersion = string.Empty;
        string str_SelectedPairName = string.Empty;
        IFormUpdater pu_ProgramUpdater;
        #endregion

        #region 窗体事件
        public frm_FileSynchronizer()
        {
            InitializeComponent();
        }

        private void frmFileSynchronizer_Load(object sender, EventArgs e)
        {
            //检查日志文件
            if (cls_LogProgramFile.InitLog()) LogProgramMessage("生成日志文件", true, true, 0);

            InitLocalDatabase();

            this.Text = String.Join("_Ver.", c_ProgramTitle, str_MainProgramVersion);
            cls_LogProgramFile.LogToCache = false;
            Control.CheckForIllegalCrossThreadCalls = false;
            string str_ErrorMsg = String.Empty;
            InitProgramUpdater();
            SetTimerAutoUpdate();

            //调整调试模式功能
            DebugModeFunction(true);

            //检查程序启动时是否自动最小化窗口
            if (cls_Global_Settings.MinWhenStart)
            {
                HideMainForm();
            }

            timer1_Tick(sender, e);
            timerAutoUpdate_Tick(sender, e);

            if (!cls_Files_InfoDB.RevertUnfinishedSyncDetail(String.Empty, cls_Global_Settings.DebugMode, out str_ErrorMsg))
            {
                LogProgramMessage(str_ErrorMsg, true, true, 1);
            }
            else
            {
                dataGridView1_CellClick(sender, new DataGridViewCellEventArgs(0, 0));
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex <= 0)
            {
                str_SelectedPairName = string.Empty;
                return;
            }

            string str_PairName = tabControl1.SelectedTab.Name;
            str_SelectedPairName = str_PairName;
            DataRow _dr = GetPairInfor(str_PairName);
            if (_dr == null)
            {
                LogProgramMessage("没有找到配对（" + str_PairName + "）的信息", true, true, 1);
                return;
            }

            string str_AutoSyncInterval = _dr.ItemArray[7].ToString();
            int int_AutoSyncInterval = 0;
            if (!Int32.TryParse(str_AutoSyncInterval, out int_AutoSyncInterval)) int_AutoSyncInterval = 0;
            string str_IsPausedSync = _dr.ItemArray[9].ToString();

            btnPauseSync.Visible = int_AutoSyncInterval != 0;
            if (!Boolean.Parse(str_IsPausedSync))
            {
                btnPauseSync.Text = "暂停自动同步";
            }
            else
            {
                btnPauseSync.Text = "恢复自动同步";
            }

            if (dataGridView1.RowCount > 0)
            {
                dataGridView1.Rows[tabControl1.SelectedIndex - 1].Selected = true;
            }
        }

        private void FileSynchronizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            string str_ErrorMsg = String.Empty;
            cls_Files_InfoDB.CleanSyncDetailRecord(String.Empty, true, out str_ErrorMsg);
            cls_Files_InfoDB.FixDirPairStatus(false);
            cls_Files_InfoDB.CloseConnection();
            if (!String.IsNullOrEmpty(str_ErrorMsg))
            {
                LogProgramMessage(str_ErrorMsg, true, true, 0);
            }
            cls_LogProgramFile.LogMsgFromCacheToFile();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                RebindPairTable();
                LogProgramMessage("检查自动同步配对", true, true, 5);
                CheckAutoSyncPair();
                tabControl1_SelectedIndexChanged(this, e);
                if (cls_LogProgramFile.LogToCache) cls_LogProgramFile.LogMsgFromCacheToFile();

                if (DateTime.Now >= DateTime.Today.AddMilliseconds(0) && DateTime.Now <= DateTime.Today.AddMilliseconds(timer1.Interval) && cls_Global_Settings.AutoClearLog)
                {
                    LogProgramMessage("执行每日自动清除界面日志", true, true, 3);
                    ClearAllLogs();
                }
            }
            catch (Exception ex)
            {
                LogProgramMessage(ex.Message, true, true, 5, true);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.RowCount < 1) return;
            int i_SelectedIndex = e.RowIndex;
            tabControl1.SelectedIndex = i_SelectedIndex + 1;

            str_SelectedPairName = dataGridView1.Rows[i_SelectedIndex].Cells["PairName"].Value.ToString();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.RowCount < 1) return;

            int i_SelectedIndex = e.RowIndex;
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                string str_Dir = dataGridView1.Rows[i_SelectedIndex].Cells[e.ColumnIndex].Value.ToString();
                if (Directory.Exists(str_Dir))
                {
                    System.Diagnostics.Process.Start("Explorer.exe", str_Dir);
                }
            }
        }

        private void timerAutoUpdate_Tick(object sender, EventArgs e)
        {
            CheckForNewVersion();
        }
        #endregion

        #region 按钮事件
        /// <summary>
        /// 分析当前配对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) return;
            LogProgramMessage("点击分析当前配对按钮", true, true, 3);

            string str_PairName = tabControl1.SelectedTab.Name;
            DoDirPairOperation(str_PairName, true);
        }

        /// <summary>
        /// 同步当前配对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) return;
            LogProgramMessage("点击同步当前配对按钮", true, true, 3);

            string str_PairName = tabControl1.SelectedTab.Name;
            DoDirPairOperation(str_PairName, false);
        }

        /// <summary>
        /// 同步所有配对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyncAll_Click(object sender, EventArgs e)
        {
            if (dt_DirPair == null || dt_DirPair.Rows.Count.Equals(0)) return;

            LogProgramMessage("点击同步所有配对按钮", true, true, 3);
            for (int i = 1; i < tabControl1.TabCount; i++)
            {
                string str_PairName = tabControl1.TabPages[i].Name;
                DoDirPairOperation(str_PairName, false);
            }
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearLog_Click(object sender, EventArgs e)
        {
            LogProgramMessage("点击清空日志按钮", true, true, 3);
            if (tabControl1.SelectedIndex == 0)
            {
                TxtProgramLog.Clear();
            }
            else
            {
                ((ctrl_PairPanal)tabControl1.SelectedTab.Controls[0]).ClearPairLogs();
            }
        }

        /// <summary>
        /// 打开日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenPairLog_Click(object sender, EventArgs e)
        {
            LogProgramMessage("点击打开日志按钮", true, true, 3);
            if (tabControl1.SelectedIndex == 0)
            {
                cls_LogProgramFile.OpenProgramLog();
            }
            else
            {
                ((ctrl_PairPanal)tabControl1.SelectedTab.Controls[0]).OpenPairLog();
            }
        }

        /// <summary>
        /// 停止所有操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopOpr_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("此操作将会停止所有正在进行的分析或同步动作，确定？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            LogProgramMessage("点击停止所有操作按钮", true, true, 3);
            StopAllOperations();
        }

        /// <summary>
        /// 暂停自动同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPauseSync_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) return;
            LogProgramMessage("点击暂停自动同步按钮", true, true, 3);

            ctrl_PairPanal CurrentPair = (ctrl_PairPanal)tabControl1.SelectedTab.Controls[0];
            if (CurrentPair.PausePairAutoSync())
                timer1_Tick(this, e);
        }

        /// <summary>
        /// 测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            LogProgramMessage("Exexute Ends", true, true, 3);
            ClearAllLogs();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 检查自动同步的配对
        /// </summary>
        private void CheckAutoSyncPair()
        {
            string[] arr_PairInfor = cls_Files_InfoDB.CheckAutoSyncPair();
            if (arr_PairInfor != null)
            {
                Parallel.ForEach(arr_PairInfor, (item) =>
                {
                    string str_PairName = item.Split('|')[1];
                    LogProgramMessage("开始自动同步配对（" + str_PairName + "）", true, true, 1);
                    DoDirPairOperation(str_PairName, false);
                });
            }
        }

        /// <summary>
        /// 通过配对名在全局配对表中查找配对信息
        /// </summary>
        /// <param name="str_PairName">配对名</param>
        /// <returns></returns>
        private DataRow GetPairInfor(string str_PairName)
        {
            if (dt_DirPair == null || dt_DirPair.Rows.Count.Equals(0))
            {
                return null;
            }
            DataTable dt_Temp = dt_DirPair.Copy();

            DataRow[] _dr = dt_Temp.Select("PAIRNAME='" + str_PairName + "'");
            if (_dr == null || _dr.Length == 0)
            {
                return null;
            }
            else
            {
                return _dr[0];
            }
        }

        /// <summary>
        /// 记录程序日志消息
        /// </summary>
        /// <param name="LogMessage">日志消息</param>
        /// <param name="IsChangeLine">是否换行</param>
        /// <param name="IsAddTS">是否添加时间戳</param>
        /// <param name="MsgTraceLevel">记录的日志消息等级</param>
        /// <param name="IsALwaysLogFile">是否强制写入日志文件（默认否）</param>
        private void LogProgramMessage(string LogMessage, bool IsChangeLine, bool IsAddTS, int MsgTraceLevel, bool IsALwaysLogFile = false)
        {
            string str_LogMsgToFile = (IsAddTS ? DateTime.Now.ToString(cls_Files_InfoDB.DBDateTimeFormat) + " --- " : String.Empty) + LogMessage;
            string str_LogMsgChngLine = str_LogMsgToFile + (IsChangeLine ? Environment.NewLine : "");
            bool bl_HasWrittenFile = IsALwaysLogFile;

            if (IsALwaysLogFile)
            {
                cls_LogProgramFile.LogMessage(str_LogMsgToFile);
                bl_HasWrittenFile = true;
            }

            try
            {
                //输入的MsgTraceLevel是0，则属于程序顶级日志，直接处理
                if (MsgTraceLevel == 0)
                {
                    if (TxtProgramLog.TextLength > 150000)
                    {
                        TxtProgramLog.Clear();
                    }
                    TxtProgramLog.AppendText(str_LogMsgChngLine);
                    TxtProgramLog.ScrollToCaret();
                    if (!bl_HasWrittenFile)
                    {
                        cls_LogProgramFile.LogMessage(str_LogMsgToFile);
                    }
                }
                //输入的MsgTraceLevel > 0，则对比全局变量设置的日志等级做判断处理
                else if (MsgTraceLevel > 0 && MsgTraceLevel <= cls_Global_Settings.TraceLevel)
                {
                    if (TxtProgramLog.TextLength > 150000)
                    {
                        TxtProgramLog.Clear();
                    }
                    TxtProgramLog.AppendText(str_LogMsgChngLine);
                    TxtProgramLog.ScrollToCaret();

                    if (cls_Global_Settings.LogMessageToFile && !bl_HasWrittenFile)
                    {
                        cls_LogProgramFile.LogMessage(str_LogMsgToFile);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProgramMessage(ex.Message, true, true, 5, true);
            }
        }

        private void RebindPairTable(bool bl_IsForceRefresh = false)
        {
            bool bl_IsFirstGetDirPair = false;

            if (dt_DirPair == null)
            {
                dt_DirPair = new DataTable();
                bl_IsFirstGetDirPair = true;
            }
            DataTable dt_Temp = dt_DirPair.Copy();
            try
            {
                //dt_DirPair.Reset();
                dt_DirPair = cls_Files_InfoDB.GetDirPairInfor(String.Empty);
                if (dt_DirPair == null)
                {
                    LogProgramMessage("程序发生严重错误，获取配对列表失败！！！", true, true, 1, true);
                    LogProgramMessage("Failed to Get PAIR table!!!", true, true, 5, true);
                    dt_DirPair = dt_Temp.Copy();
                }

                bool bl_IsOnlyRefreshGrid = true;
                bool bl_IsDirPairSame = IsDirPairSame(dt_DirPair, dt_Temp, out bl_IsOnlyRefreshGrid);
                if (bl_IsForceRefresh || !bl_IsDirPairSame)
                {
                    if (bl_IsForceRefresh || !bl_IsOnlyRefreshGrid)
                    {
                        BindDirPairToTabCntl();
                    }

                    //生成、绑定至列表
                    if (bl_IsFirstGetDirPair)
                    {
                        CreatePairToGridView();
                    }
                    else
                    {
                        BindPairToGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                LogProgramMessage(ex.Message, true, true, 5, true);
                dt_DirPair = dt_Temp.Copy();
                return;
            }
        }

        private string FormatPairStatusString(string PairName, PairStatus pairStatus)
        {
            if (pairStatus == PairStatus.FREE)
            {
                return string.Empty;
            }
            else
            {
                string str_OutMsg = "配对（" + PairName + "）正在";

                switch (pairStatus)
                {
                    case PairStatus.FREE:
                        break;
                    case PairStatus.ANALYSIS:
                        str_OutMsg += "分析，请稍后再试"; break;
                    case PairStatus.SYNC:
                        str_OutMsg += "同步，请稍后再试"; break;
                    default:
                        break;
                }
                return str_OutMsg;
            }
        }

        private void BindDirPairToTabCntl()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { BindDirPairToTabCntl(); }));
                return;
            }

            int str_SlectedTabIdx = tabControl1.SelectedIndex;

            for (int i = tabControl1.Controls.Count - 1; i > 0; i--)
            {
                tabControl1.Controls[i].Dispose();
            }

            if (dt_DirPair == null) return;
            if (dt_DirPair.Rows.Count.Equals(0))
            {
                LogProgramMessage("没有找到任何配对信息，请先到“程序-管理目录配对”窗口添加配对", true, true, 1);
                return;
            }

            DataTable dt_Temp = dt_DirPair.Copy();
            foreach (DataRow dataRow in dt_Temp.Rows)
            {
                string str_PairID = dataRow.ItemArray[0].ToString();
                string str_PairName = dataRow.ItemArray[1].ToString();
                string str_Dir1Path = dataRow.ItemArray[2].ToString();
                string str_Dir2Path = dataRow.ItemArray[3].ToString();
                string str_LastSyncTime = dataRow.ItemArray[4].ToString();
                string str_FilterRule = dataRow.ItemArray[6].ToString();
                string str_AutoSyncInterval = dataRow.ItemArray[7].ToString();
                string str_SyncDirection = dataRow.ItemArray[8].ToString();
                string str_IsPaused = dataRow.ItemArray[9].ToString();
                int int_SyncDirection = 0;
                if (!String.IsNullOrEmpty(str_SyncDirection)) int_SyncDirection = Convert.ToInt32(str_SyncDirection);
                int int_AutoSyncInterval = 0;
                if (!Int32.TryParse(str_AutoSyncInterval, out int_AutoSyncInterval)) int_AutoSyncInterval = 0;
                bool bl_IsPaused = false;
                if (!Boolean.TryParse(str_IsPaused, out bl_IsPaused)) bl_IsPaused = false;

                TabPage tabPageDirPair = new TabPage(str_PairName);
                tabPageDirPair.Tag = str_PairID;
                tabPageDirPair.Name = str_PairName;
                ctrl_PairPanal PairPanal = new ctrl_PairPanal(str_PairID, str_PairName, str_Dir1Path, str_Dir2Path, str_LastSyncTime, str_FilterRule, int_AutoSyncInterval, int_SyncDirection, bl_IsPaused, cls_Files_InfoDB.DBDateTimeFormat) { Dock = DockStyle.Fill };
                PairPanal.OperationStarted += PairPanal_OperationStarted;
                PairPanal.OperationDone += PairPanal_OperationDone;
                tabPageDirPair.Controls.Add(PairPanal);
                tabControl1.Controls.Add(tabPageDirPair);
            }

            if (tabControl1.TabCount > str_SlectedTabIdx)
            {
                tabControl1.SelectedIndex = str_SlectedTabIdx;
            }
        }

        private void PairPanal_OperationStarted(object sender)
        {
            timer1_Tick(this, new EventArgs());
        }

        private void PairPanal_OperationDone(object sender)
        {
            timer1_Tick(this, new EventArgs());
        }

        private void CreatePairToGridView()
        {
            if (dt_DirPair == null) return;

            dataGridView1.DataSource = dt_DirPair;
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
            dataGridView1.Columns[7].HeaderText = "自动同步间隔(分钟)";
            dataGridView1.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[8].Visible = false;
            dataGridView1.Columns[9].HeaderText = "暂停自动同步";
            dataGridView1.Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
            groupBox1.Height = 60 + dataGridView1.ColumnHeadersHeight * dt_DirPair.Rows.Count;
        }

        private void BindPairToGridView()
        {
            if (dt_DirPair == null) return;

            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke((MethodInvoker)delegate ()
                {
                    BindPairToGridView();
                });
            }
            else
            {
                dataGridView1.DataSource = dt_DirPair;
                dataGridView1.Refresh();
                groupBox1.Height = 60 + dataGridView1.ColumnHeadersHeight * dt_DirPair.Rows.Count;

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (item.Cells["PairName"].Value.ToString().Equals(str_SelectedPairName))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        private void InitLocalDatabase()
        {
            //检查数据库文件（！！！必须在所有数据库操作之前完成！！！）
            string str_chkDBResult = cls_Files_InfoDB.CheckDBFile();
            if (!String.IsNullOrEmpty(str_chkDBResult)) LogProgramMessage(str_chkDBResult, true, true, 0);

            //打开数据库
            cls_Files_InfoDB.OpenConnection();
            //初始化全局设置
            cls_Global_Settings.Init_Settings();

            //获取主程序和数据库版本
            string str_Current = cls_Global_Settings.DBVersion;
            str_MainProgramVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (!str_Current.Equals(str_MainProgramVersion))
            {
                cls_Files_InfoDB.BackupDBFile(str_Current);
                string str_DBVersion = cls_Files_InfoDB.CheckDBUpgrade(str_MainProgramVersion);
                if (!String.IsNullOrEmpty(str_DBVersion))
                {
                    LogProgramMessage("数据库已经升级到版本" + str_DBVersion, true, true, 1);
                }
            }
        }

        private void ClearAllLogs()
        {
            LogProgramMessage("Clear All Logs on main screen", true, true, 5);

            TxtProgramLog.Clear();

            for (int i = 1; i < tabControl1.TabCount; i++)
            {
                ((ctrl_PairPanal)tabControl1.Controls[i].Controls[0]).ClearPairLogs();
            }
        }

        private bool IsDirPairSame(DataTable DirTable1, DataTable DirTable2, out bool bl_IsOnlyRefreshGrid)
        {
            bool bl_Is2TablesSame = false;
            bl_IsOnlyRefreshGrid = false;
            if (DirTable1 == null || DirTable2 == null)
            {
                return false;
            }

            if (!DirTable1.Rows.Count.Equals(DirTable2.Rows.Count))
            {
                return false;
            }

            if (!DirTable1.Columns.Count.Equals(DirTable2.Columns.Count))
            {
                return false;
            }

            for (int i = 0; i < DirTable1.Rows.Count; i++)
            {
                string str_Table1Row = String.Join("|", DirTable1.Rows[i].ItemArray[0], DirTable1.Rows[i].ItemArray[1], DirTable1.Rows[i].ItemArray[2], DirTable1.Rows[i].ItemArray[3]);
                string str_Table2Row = String.Join("|", DirTable2.Rows[i].ItemArray[0], DirTable2.Rows[i].ItemArray[1], DirTable2.Rows[i].ItemArray[2], DirTable2.Rows[i].ItemArray[3]);

                string str_Table1Row2 = String.Join("|", DirTable1.Rows[i].ItemArray[4], DirTable1.Rows[i].ItemArray[5], DirTable1.Rows[i].ItemArray[7], DirTable1.Rows[i].ItemArray[8], DirTable1.Rows[i].ItemArray[9]);
                string str_Table2Row2 = String.Join("|", DirTable2.Rows[i].ItemArray[4], DirTable2.Rows[i].ItemArray[5], DirTable2.Rows[i].ItemArray[7], DirTable2.Rows[i].ItemArray[8], DirTable2.Rows[i].ItemArray[9]);

                if (!str_Table1Row.Equals(str_Table2Row))
                {
                    bl_Is2TablesSame = false;
                    bl_IsOnlyRefreshGrid = false;
                    break;
                }
                else
                {
                    bl_Is2TablesSame = true;
                    if (!bl_IsOnlyRefreshGrid)
                    {
                        bl_IsOnlyRefreshGrid = str_Table1Row2 != str_Table2Row2;
                    }
                }
            }

            return bl_Is2TablesSame && !bl_IsOnlyRefreshGrid;
        }

        private void DoDirPairOperation(string PairName, bool IsAnalysis)
        {
            ctrl_PairPanal CurrentPair = (ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0];
            PairStatus pairStatus;
            if (CurrentPair.IsPairBusy(out pairStatus))
            {
                string str_PairStatus = FormatPairStatusString(PairName, pairStatus);
                LogProgramMessage(str_PairStatus, true, true, 1);
            }
            else
            {
                CurrentPair.DoAnalysisSyncDirPair(IsAnalysis);
            }
        }

        private void StopAllOperations()
        {
            try
            {
                LogProgramMessage("Stoping all oprating threads", true, true, 4);

                //查找正在进行的分析/同步线程并停止
                for (int i = 1; i < tabControl1.TabCount; i++)
                {
                    ctrl_PairPanal CurrentPair = (ctrl_PairPanal)tabControl1.TabPages[i].Controls[0];
                    string str_PairName = CurrentPair.PairName;
                    PairStatus pairStatus;
                    if (CurrentPair.IsPairBusy(out pairStatus))
                    {
                        CurrentPair.StopOngoingOperation();
                        LogProgramMessage("成功停止了配对（" + str_PairName + "）正在进行的操作", true, true, 1);
                    }
                }

                Thread.Sleep(1000);
                if (cls_LogProgramFile.LogToCache) cls_LogProgramFile.LogMsgFromCacheToFile();
            }
            catch (Exception ex)
            {
                LogProgramMessage(ex.Message, true, true, 5);
            }
        }

        private void DebugModeFunction(bool bIsCallFromMain)
        {
            if (cls_Global_Settings.DebugMode)
            {
                LogProgramMessage(@"程序处于调试模式，请注意对功能的影响！", true, true, 1);
            }

            btnAnalysis.Visible = cls_Global_Settings.DebugMode;
            btnTest.Visible = cls_Global_Settings.DebugMode;
            //检查更新ToolStripMenuItem.Visible = cls_Global_Settings.DebugMode;

            //调试模式功能：把程序版本降一级
            if (cls_Global_Settings.DebugMode)
            {
                int i_PrevRevision = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision - 1;
                str_MainProgramVersion = str_MainProgramVersion.Substring(0, str_MainProgramVersion.Length - 1) + i_PrevRevision.ToString();
            }
            else
            {
                str_MainProgramVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            this.Text = String.Join("_Ver.", c_ProgramTitle, str_MainProgramVersion);

            if (pu_ProgramUpdater != null)
            {
                pu_ProgramUpdater.SetDebugMode(cls_Global_Settings.DebugMode);
                pu_ProgramUpdater.SetMainPgmVer(str_MainProgramVersion);
                if (!bIsCallFromMain)
                {
                    CheckForNewVersion();
                }
            }
        }

        private void InitProgramUpdater()
        {
            if (pu_ProgramUpdater == null)
            {
                pu_ProgramUpdater = new IFormUpdater(c_ProgramTitle, str_MainProgramVersion, c_UpdateURL_Str, ProgramUpdateSource.GITHUB, cls_Global_Settings.DebugMode);
            }
        }

        private bool CheckForNewVersion()
        {
            bool bHasNewVesion = false;
            string str_ErrorMsg = String.Empty;

            if (pu_ProgramUpdater != null)
            {
                bHasNewVesion = pu_ProgramUpdater.CheckNewVersion(out str_ErrorMsg);
                if (bHasNewVesion)
                {
                    检查更新ToolStripMenuItem.Text = @"检查更新（有新版本可用）";
                    LogProgramMessage(str_ErrorMsg, true, true, 3);
                }
                else
                {
                    检查更新ToolStripMenuItem.Text = @"检查更新";
                    LogProgramMessage(str_ErrorMsg, true, true, 3);
                }
            }

            return bHasNewVesion;
        }

        private void SetTimerAutoUpdate()
        {
            timerAutoUpdate.Enabled = cls_Global_Settings.AutoCheckUpdateInterval == 0 ? false : true;
            timerAutoUpdate.Interval = cls_Global_Settings.AutoCheckUpdateInterval == 0 ? 100 : cls_Global_Settings.AutoCheckUpdateInterval * 24 * 60 * 1000;
        }
        #endregion

        #region 窗体方法 处理窗体的 显示 隐藏
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //鼠标左键才有效   
            if (((MouseEventArgs)e).Button != MouseButtons.Left)
                return;
            //this.timer1.Stop();
            if (this.WindowState == FormWindowState.Normal)
            {
                HideMainForm();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                ShowMainForm();
            }
        }

        private void FileSynchronizer_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                HideMainForm();
            }
        }

        private void HideMainForm()
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            base.SetVisibleCore(false);
        }

        private void ShowMainForm()
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            base.SetVisibleCore(true);
            tabControl1_SelectedIndexChanged(this, new EventArgs());
        }
        #endregion

        #region 右键菜单的方法
        private void MenuItemSyncAll_Click(object sender, EventArgs e)
        {
            LogProgramMessage("点击程序菜单的同步所有配对选项", true, true, 3);
            btnSyncAll.PerformClick();
        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MenuItemShowMain_Click(object sender, EventArgs e)
        {
            ShowMainForm();
        }

        private void MenuItemSync_MouseEnter(object sender, EventArgs e)
        {
            if (dt_DirPair == null || dt_DirPair.Rows.Count.Equals(0)) return;

            if (!MenuItemSync.HasDropDownItems)
            {
                for (int i = 0; i < dt_DirPair.Rows.Count; i++)
                {
                    string str_PairName = dt_DirPair.Rows[i][1].ToString();
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(str_PairName);
                    toolStripMenuItem.Click += ToolStripMenuItem_Click;
                    MenuItemSync.DropDownItems.Add(toolStripMenuItem);
                }
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str_PairName = ((ToolStripMenuItem)sender).Text;
            LogProgramMessage("点击程序菜单的同步配对选项，目标配对：" + str_PairName, true, true, 3);
            DoDirPairOperation(str_PairName, false);
        }

        private void MenuItemOther_MouseEnter(object sender, EventArgs e)
        {
            MenuItemSync.DropDownItems.Clear();
        }
        #endregion

        #region 程序菜单
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 管理同步文件夹对ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            LogProgramMessage("Pause all oprating threads", true, true, 4);
            //查找正在进行的分析/同步线程并停止
            for (int i = 1; i < tabControl1.TabCount; i++)
            {
                ctrl_PairPanal CurrentPair = (ctrl_PairPanal)tabControl1.TabPages[i].Controls[0];
                CurrentPair.PauseResumeOngoingOperation(true);
            }

            frm_DirectoryPairManagement directoryPairManager = new frm_DirectoryPairManagement();
            directoryPairManager.ShowDialog();
            string[] str_ManageResult = directoryPairManager.arr_Return_Message;
            bool bl_IsStopAllOpr = false;
            bool bl_IsForceRefresh = directoryPairManager.bl_RefreshListRequired;
            for (int i = 0; i < str_ManageResult.Length; i++)
            {
                LogProgramMessage(str_ManageResult[i], true, false, 1);
                if (str_ManageResult[i].Contains("更改配对") || str_ManageResult[i].Contains("删除配对"))
                {
                    bl_IsStopAllOpr = true;
                }
            }
            RebindPairTable(bl_IsForceRefresh);

            if (bl_IsStopAllOpr)
            {
                LogProgramMessage("Stop all oprating threads", true, true, 4);
                StopAllOperations();
            }
            else
            {
                LogProgramMessage("Resume all oprating threads", true, true, 4);
                //查找正在进行的分析/同步线程并停止
                for (int i = 1; i < tabControl1.TabCount; i++)
                {
                    ctrl_PairPanal CurrentPair = (ctrl_PairPanal)tabControl1.TabPages[i].Controls[0];
                    CurrentPair.PauseResumeOngoingOperation(false);
                }
            }
            timer1.Start();
        }

        private void 全局设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            new frm_GlobalSettings(str_MainProgramVersion).ShowDialog();

            SetTimerAutoUpdate();
            //调整调试模式功能
            DebugModeFunction(false);
            timer1.Start();
        }

        private void 关于ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        private void 更新日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frm_ChangeLog().ShowDialog();
        }

        private void 项目Github主页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://github.com/ksphabble/FileSynchronizer");
        }

        private void 检查更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pu_ProgramUpdater != null)
            {
                pu_ProgramUpdater.Show();
            }
        }
        #endregion
    }
}
