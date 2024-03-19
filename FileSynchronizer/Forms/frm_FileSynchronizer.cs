using Common.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public partial class frm_FileSynchronizer : Form
    {
        #region 全局变量
        DataTable dt_DirPair;
        const string str_FSBackup = @"_FSBackup";
        const string str_ThreadPrefix = @"->";
        List<Thread> threadPoolOpr;
        string str_MainProgramVersion = String.Empty;
        string str_SelectedPairName = String.Empty;
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

            this.Text = String.Join("_Ver.", "FileSynchronizer", str_MainProgramVersion);
            cls_LogProgramFile.LogToCache = false;
            Control.CheckForIllegalCrossThreadCalls = false;

            //检查程序启动时是否自动最小化窗口
            if (cls_Global_Settings.MinWhenStart)
            {
                HideMainForm();
            }

            threadPoolOpr = new List<Thread> { };
            btnAnalysis.Visible = cls_Global_Settings.DebugMode;
            btnTest.Visible = cls_Global_Settings.DebugMode;

            timer1_Tick(sender, e);
            dataGridView1_CellClick(sender, new DataGridViewCellEventArgs(0, 0));
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex <= 0) return;

            string str_PairName = tabControl1.SelectedTab.Name;
            str_SelectedPairName = str_PairName;
            DataRow _dr = GetPairInfor(str_PairName);
            if (_dr == null)
            {
                LogPairMessage(str_PairName, "没有找到配对（" + str_PairName + "）的信息", true, true, 1);
                return;
            }

            string str_IsPausedSync = _dr.ItemArray[9].ToString();

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
            cls_LogProgramFile.LogMsgFromCacheToFile();
            cls_Files_InfoDB.FixDirPairStatus(false);
            cls_Files_InfoDB.CloseConnection();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                RebindPairTable();
                RemoveEndedThreads();
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

            string str_PairName = tabControl1.SelectedTab.Name;
            LogProgramMessage("点击分析当前配对按钮，目标配对：" + str_PairName, true, true, 3);
            string str_WarningMsg;
            if (!CheckCurrentOpr(str_PairName, out str_WarningMsg))
            {
                LogProgramMessage(str_WarningMsg, true, true, 1);
                return;
            }

            Thread threadSync = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
            threadSync.Name = "Click_Analysis" + str_ThreadPrefix + str_PairName;
            threadSync.IsBackground = true;
            threadPoolOpr.Add(threadSync);
            threadSync.Start(str_PairName);
        }

        /// <summary>
        /// 同步当前配对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, EventArgs e)
        {
            //if (tabControl1.SelectedIndex == 0) return;
            if (String.IsNullOrEmpty(str_SelectedPairName)) return;

            string str_PairName = str_SelectedPairName;
            LogProgramMessage("点击同步当前配对按钮，目标配对：" + str_PairName, true, true, 3);
            string str_WarningMsg;
            if (!CheckCurrentOpr(str_PairName, out str_WarningMsg))
            {
                LogProgramMessage(str_WarningMsg, true, true, 1);
                return;
            }

            Thread threadSync = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
            threadSync.Name = "Click_Sync" + str_ThreadPrefix + str_PairName;
            threadSync.IsBackground = true;
            threadPoolOpr.Add(threadSync);
            threadSync.Start(str_PairName);
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
            for (int i = 0; i < dt_DirPair.Rows.Count; i++)
            {
                string str_PairName = dt_DirPair.Rows[i][1].ToString();
                string str_WarningMsg;
                if (!CheckCurrentOpr(str_PairName, out str_WarningMsg))
                {
                    LogProgramMessage(str_WarningMsg, true, true, 1);
                    continue;
                }

                Thread threadSync = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
                threadSync.Name = "Click_All_Sync" + str_ThreadPrefix + str_PairName;
                threadSync.IsBackground = true;
                threadPoolOpr.Add(threadSync);
                threadSync.Start(str_PairName);
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
                ((ctrl_PairPanal)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0]).ClearPairLogs();
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
                if (tabControl1.SelectedIndex == 0) return;

                string str_PairName = tabControl1.SelectedTab.Name;
                new cls_LogPairFile(str_PairName, false).OpenPairLog();
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
            try
            {
                int int_StoppedThread = 0;
                LogProgramMessage("Stoping all oprating threads", true, true, 4);
                //查找正在进行的分析/同步线程并停止
                foreach (Thread sub_thread in threadPoolOpr)
                {
                    if ((sub_thread.Name.Contains("Sync") || sub_thread.Name.Contains("Analysis")) && sub_thread.IsAlive)
                    {
                        string str_PairName = sub_thread.Name.Substring(sub_thread.Name.LastIndexOf(str_ThreadPrefix) + str_ThreadPrefix.Length);
                        sub_thread.Abort();
                        int_StoppedThread++;
                        ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
                        PairPanal.ResetSyncLabels();
                        LogProgramMessage("Aborted oprating threads:" + sub_thread.Name, true, true, 4);
                    }
                }

                if (int_StoppedThread > 0)
                {
                    LogProgramMessage("成功停止了正在进行的分析/同步操作", true, true, 1);
                }
                Thread.Sleep(1000);

                RemoveEndedThreads();
                if (cls_LogProgramFile.LogToCache) cls_LogProgramFile.LogMsgFromCacheToFile();
            }
            catch (Exception ex)
            {
                LogProgramMessage(ex.Message, true, true, 5);
            }
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
            string str_PairID = tabControl1.SelectedTab.Tag.ToString();
            string str_WarningMsg;

            if (!cls_Files_InfoDB.PausePairAutoSync(str_PairID, out str_WarningMsg))
            {
                LogProgramMessage(str_WarningMsg, true, true, 5);
            }
            else
            {
                timer1_Tick(this, e);
            }
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

        #region 配对方法
        /// <summary>
        /// 分析文件夹配对
        /// </summary>
        /// <param name="obj_PairName"></param>
        private DataTable AnalysisDirPair(string str_PairName, bool bl_IsAnalysisOnly)
        {
            LogPairMessage(str_PairName, "开始分析配对（" + str_PairName + "）", true, true, 2);

            #region Define Varibles
            DataRow _dr = GetPairInfor(str_PairName);
            if (_dr == null)
            {
                LogPairMessage(str_PairName, "没有找到配对（" + str_PairName + "）的信息", true, true, 1);
                return null;
            }

            string str_PairID = _dr.ItemArray[0].ToString();
            string str_Dir1Path = _dr.ItemArray[2].ToString();
            string str_Dir2Path = _dr.ItemArray[3].ToString();
            string str_DirLastSyncTime = _dr.ItemArray[4].ToString();
            string str_FilterRule = _dr.ItemArray[6].ToString();
            string str_SyncDirection = _dr.ItemArray[8].ToString();
            string str_OutLogMsg = String.Empty;
            int int_SyncDirection = 0;
            if (!String.IsNullOrEmpty(str_SyncDirection))
            {
                int_SyncDirection = Convert.ToInt32(str_SyncDirection);
            }
            string[] arr_FilterRule = String.IsNullOrEmpty(str_FilterRule) ? new string[] { } : str_FilterRule.Split(',');
            DateTime dt_DirLastSyncTime;
            bool bl_LastSyncStatus = DateTime.TryParse(str_DirLastSyncTime, out dt_DirLastSyncTime);
            if (!bl_LastSyncStatus)
            {
                LogPairMessage(str_PairName, "没有找到上次同步时间，首次分析同步需时较长，请耐心等待", true, true, 1);
                dt_DirLastSyncTime = DateTime.MinValue;
            }

            //DIR1的子目录和文件信息
            LogPairMessage(str_PairName, "开始获取DIR1（" + str_Dir1Path + "）的目录和文件信息", true, true, 2);
            DirectoryInfo _dir1 = new DirectoryInfo(str_Dir1Path);
            //检查DIR1根目录是否存在，若不存在，则提示出错并停止分析
            if (bl_LastSyncStatus && !_dir1.Exists)
            {
                LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录1可能出现问题，请检查，若目录内容为空，请忽略此提示", true, true, 1);
                return null;
            }
            string str_Dir1TableName = str_PairName + "_DIR1_" + _dir1.Name;
            DirectoryInfo[] subDir1 = _dir1.GetDirectories("*", SearchOption.AllDirectories);
            FileInfo[] fileInfos1 = _dir1.GetFiles("*", SearchOption.AllDirectories);
            DataTable dt_File1InforDB = cls_Files_InfoDB.GetFileInfor(str_Dir1TableName, out str_OutLogMsg);

            //DIR2的子目录和文件信息
            LogPairMessage(str_PairName, "开始获取DIR2（" + str_Dir2Path + "）的目录和文件信息", true, true, 2);
            DirectoryInfo _dir2 = new DirectoryInfo(str_Dir2Path);
            //检查DIR2根目录是否存在，若不存在，则提示出错并停止分析
            if (bl_LastSyncStatus && !_dir2.Exists)
            {
                LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录2可能出现问题，请检查，若目录内容为空，请忽略此提示", true, true, 1);
                return null;
            }
            string str_Dir2TableName = str_PairName + "_DIR2_" + _dir2.Name;
            DirectoryInfo[] subDir2 = _dir2.GetDirectories("*", SearchOption.AllDirectories);
            FileInfo[] fileInfos2 = _dir2.GetFiles("*", SearchOption.AllDirectories);
            DataTable dt_File2InforDB = cls_Files_InfoDB.GetFileInfor(str_Dir2TableName, out str_OutLogMsg);

            int int_TotalFileFound = subDir1.Length + fileInfos1.Length + subDir2.Length + fileInfos2.Length + dt_File1InforDB.Rows.Count + dt_File2InforDB.Rows.Count;
            ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
            PairPanal.SetAnalysisCount(int_TotalFileFound);
            string str_FileFullName = String.Empty;
            string str_FileName = String.Empty;
            string str_FilePath = String.Empty;
            string str_FileSize = String.Empty;
            string str_FileLastModDate = String.Empty;
            string str_FileCreDate = String.Empty;
            string str_DirCreDate = String.Empty;
            string str_FileMD5 = String.Empty;
            string str_FileID = String.Empty;
            const string str_DirNameChar = "~";
            string str_Where = String.Empty;
            bool bl_ExceptionFound = false;
            int i_SleepInterval = 20;
            #endregion

            #region 分析目录和文件至数据库
            //从目录1的子目录分析至数据库
            foreach (DirectoryInfo directoryInfo in subDir1)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = directoryInfo.FullName;
                    str_FileName = str_DirNameChar;
                    str_FilePath = directoryInfo.FullName;
                    str_FileSize = "0";
                    str_FileMD5 = String.Empty;
                    str_DirCreDate = directoryInfo.CreationTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(str_FSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Exclude DIR: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgA);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_DirCreDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File1InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR1-ExistsInDB-Exclude DIR: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgB);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > 260)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Path length exceeds limit-Skip DIR: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        Add1Analysis(str_PairName);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Add DIR: " + str_FilePath;
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    PairPanal.SetOngoingItem(str_LogMsgAddItem);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-1-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        Add1Analysis(str_PairName);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-1-Exception:" + ex.Message;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从目录1的文件分析至数据库
            foreach (FileInfo fileInfo in fileInfos1)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = fileInfo.FullName;
                    str_FileName = fileInfo.Name;
                    str_FilePath = fileInfo.DirectoryName;
                    str_FileSize = fileInfo.Length.ToString();
                    str_FileLastModDate = fileInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(str_FSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Exclude File: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgA);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_FileLastModDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File1InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR1-ExistsInDB-Exclude File: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgB);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > 260)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Path length exceeds limit-Skip File: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入文件信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        Add1Analysis(str_PairName);
                        continue;
                    }

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Calculating File MD5: " + str_FileFullName;
                    LogPairMessage(str_PairName, str_LogMsgCalcMD5, false, true, 4);
                    PairPanal.SetOngoingItem(str_LogMsgCalcMD5);
                    str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, bl_IsAnalysisOnly, out str_OutLogMsg);
                    LogPairMessage(str_PairName, " - " + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "Step-2-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Add File: " + str_FileFullName;
                    PairPanal.SetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-2-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        Add1Analysis(str_PairName);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-2-ExceptionB:" + ex.Message;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从数据库1分析至文件和目录
            foreach (DataRow dataRow in dt_File1InforDB.Rows)
            {
                str_FileID = dataRow.ItemArray[0].ToString();
                str_FileName = dataRow.ItemArray[1].ToString();
                str_FilePath = dataRow.ItemArray[2].ToString();
                str_FileMD5 = dataRow.ItemArray[4].ToString();
                str_FileFullName = Path.Combine(str_FilePath, str_FileName);
                bool bl_hitFilterRule = false;

                //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                if (!Directory.Exists(str_Dir1Path))
                {
                    bl_ExceptionFound = true;
                    break;
                }

                //检查文件是否处于_FSBackup目录，如果存在则跳过
                if (str_FileFullName.Contains(str_FSBackup))
                {
                    bl_hitFilterRule = true;
                }

                //检查文件是否处于排除列表，如果存在则跳过
                Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                {
                    if (str_FileFullName.Contains(i))
                    {
                        bl_hitFilterRule = true;
                        LoopState.Break();
                    }
                });
                //for (int i = 0; i < arr_FilterRule.Length; i++)
                //{
                //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                //    {
                //        bl_hitFilterRule = true;
                //        break;
                //    }
                //}

                if (bl_hitFilterRule)
                {
                    string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Soft Delete item: " + str_FileFullName + " due to filter rule";
                    LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                    PairPanal.SetOngoingItem(str_LogMsgA);
                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                    {
                        LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Soft Delete item: " + str_FileFullName + " due to filter rule", true, true, 4);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        Thread.Sleep(i_SleepInterval);
                    }
                    Add1Analysis(str_PairName);
                    continue;
                }

                if (str_FileName.Equals(str_DirNameChar))
                {
                    //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir1Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!Directory.Exists(str_FilePath))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Soft Delete DIR: " + str_FilePath;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Check DIR: " + str_FilePath;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }
                else
                {
                    //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir1Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!File.Exists(str_FileFullName))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Soft Delete File: " + str_FileFullName;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Check File: " + str_FileFullName;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }

                LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                Add1Analysis(str_PairName);
            }
            LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录1（" + str_Dir1Path + "）分析完成", true, true, 2);

            //从目录2的子目录分析至数据库
            foreach (DirectoryInfo directoryInfo in subDir2)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = directoryInfo.FullName;
                    str_FileName = str_DirNameChar;
                    str_FilePath = directoryInfo.FullName;
                    str_FileSize = "0";
                    str_FileMD5 = String.Empty;
                    str_DirCreDate = directoryInfo.CreationTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(str_FSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR2-FilterRule-Exclude DIR: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgA);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_DirCreDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File2InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR2-ExistsInDB-Exclude DIR: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgB);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > 260)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Path length exceeds limit-Skip DIR: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        Add1Analysis(str_PairName);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Add DIR: " + str_FilePath;
                    PairPanal.SetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-4-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        Add1Analysis(str_PairName);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-4-Exception:" + ex.Message;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从目录2的文件分析至数据库
            foreach (FileInfo fileInfo in fileInfos2)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = fileInfo.FullName;
                    str_FileName = fileInfo.Name;
                    str_FilePath = fileInfo.DirectoryName;
                    str_FileSize = fileInfo.Length.ToString();
                    str_FileLastModDate = fileInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(str_FSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR2-FilterRule-Exclude File: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgB);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_FileLastModDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File2InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR2-ExistsInDB-Exclude File: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        PairPanal.SetOngoingItem(str_LogMsgB);
                        Add1Analysis(str_PairName);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > 260)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Path length exceeds limit-Skip File: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入文件信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        Add1Analysis(str_PairName);
                        continue;
                    }

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Calculating File MD5: " + str_FileFullName;
                    LogPairMessage(str_PairName, str_LogMsgCalcMD5, false, true, 4);
                    PairPanal.SetOngoingItem(str_LogMsgCalcMD5);
                    str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, bl_IsAnalysisOnly, out str_OutLogMsg);
                    LogPairMessage(str_PairName, " - " + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "Step-5-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Add File: " + str_FileFullName;
                    PairPanal.SetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-5-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        Add1Analysis(str_PairName);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-5-ExceptionB:" + ex.Message;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从数据库2分析至文件和目录
            foreach (DataRow dataRow in dt_File2InforDB.Rows)
            {
                str_FileID = dataRow.ItemArray[0].ToString();
                str_FileName = dataRow.ItemArray[1].ToString();
                str_FilePath = dataRow.ItemArray[2].ToString();
                str_FileMD5 = dataRow.ItemArray[4].ToString();
                str_FileFullName = Path.Combine(str_FilePath, str_FileName);
                bool bl_hitFilterRule = false;

                //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                if (!Directory.Exists(str_Dir2Path))
                {
                    bl_ExceptionFound = true;
                    break;
                }

                //检查文件是否处于_FSBackup目录，如果存在则跳过
                if (str_FileFullName.Contains(str_FSBackup))
                {
                    bl_hitFilterRule = true;
                }

                //检查文件是否处于排除列表，如果存在则跳过
                Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                {
                    if (str_FileFullName.Contains(i))
                    {
                        bl_hitFilterRule = true;
                        LoopState.Break();
                    }
                });
                //for (int i = 0; i < arr_FilterRule.Length; i++)
                //{
                //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                //    {
                //        bl_hitFilterRule = true;
                //        break;
                //    }
                //}

                if (bl_hitFilterRule)
                {
                    string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete item: " + str_FileFullName + " due to filter rule";
                    LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                    PairPanal.SetOngoingItem(str_LogMsgA);
                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                    {
                        LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete item: " + str_FileFullName + " due to filter rule", true, true, 4);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        Thread.Sleep(i_SleepInterval);
                    }
                    Add1Analysis(str_PairName);
                    continue;
                }

                if (str_FileName.Equals(str_DirNameChar))
                {
                    //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir2Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!Directory.Exists(str_FilePath))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete DIR: " + str_FilePath;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete DIR: " + str_FilePath, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Check DIR: " + str_FilePath;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }
                else
                {
                    //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir2Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!File.Exists(str_FileFullName))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete File: " + str_FileFullName;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete File: " + str_FileFullName, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Check File: " + str_FileFullName;
                            PairPanal.SetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }

                LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                Add1Analysis(str_PairName);
            }
            LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录2（" + str_Dir2Path + "）分析完成", true, true, 2);
            #endregion

            //当没有出现错误的时候才继续
            if (!bl_ExceptionFound)
            {
                //重置当前操作
                PairPanal.SetOngoingItem();

                #region 获取配对差异
                //分析结果超过10000，意味着目录里面已经超过2500个项目，分析用时较长，故提醒
                //v2.0.2.1 - 提示“配对相关的目录和文件数量较多，分析差异需时较长，请耐心等待”的阈值调整至SQLITE是100000，ACCESS是40000
                int i_AlertThreshold = cls_Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.SQLITE ? 100000 : 40000;
                if (int_TotalFileFound > i_AlertThreshold)
                {
                    LogPairMessage(str_PairName, "配对相关的目录和文件数量较多，分析差异需时较长，请耐心等待", true, true, 1);
                }
                LogPairMessage(str_PairName, "Started getting DIR/FILE difference", true, true, 4);
                //DataTable dt_fileDiff = cls_Files_InfoDB.GetFileDiff(str_PairName, str_Dir1Path, str_Dir2Path, int_SyncDirection, out str_OutLogMsg);
                DataTable dt_File1InforDB_AfterAnalysis = cls_Files_InfoDB.GetFileInfor(str_Dir1TableName, out str_OutLogMsg);
                if (!String.IsNullOrEmpty(str_OutLogMsg))
                {
                    LogPairMessage(str_PairName, str_OutLogMsg, true, true, 1);
                }
                DataTable dt_File2InforDB_AfterAnalysis = cls_Files_InfoDB.GetFileInfor(str_Dir2TableName, out str_OutLogMsg);
                if (!String.IsNullOrEmpty(str_OutLogMsg))
                {
                    LogPairMessage(str_PairName, str_OutLogMsg, true, true, 1);
                }
                DataTable dt_fileDiff = Get_File_Diff(str_PairID, str_PairName, dt_File1InforDB_AfterAnalysis, dt_File2InforDB_AfterAnalysis, str_Dir1Path, str_Dir2Path, int_SyncDirection, out str_OutLogMsg);
                if (!String.IsNullOrEmpty(str_OutLogMsg))
                {
                    LogPairMessage(str_PairName, str_OutLogMsg, true, true, 1);
                }
                LogPairMessage(str_PairName, "Started getting DIR/FILE difference --- done", true, true, 4);

                try
                {
                    foreach (DataRow dataRow in dt_fileDiff.Rows)
                    {
                        str_FileName = dataRow.ItemArray[0].ToString();
                        string str_FileFromPath = dataRow.ItemArray[1].ToString();
                        string str_FileToPath = dataRow.ItemArray[2].ToString();
                        str_FileMD5 = dataRow.ItemArray[3].ToString();
                        str_FileLastModDate = dataRow.ItemArray[4].ToString();
                        str_FileSize = dataRow.ItemArray[5].ToString();
                        int int_FileDiffType = int.Parse(dataRow.ItemArray[6].ToString());
                        string str_FromFile = Path.Combine(str_FileFromPath, str_FileName);
                        string str_ToFile = Path.Combine(str_FileToPath, str_FileName);
                        string str_LogMsg = String.Empty;
                        bool bl_hitFilterRule = false;

                        #region 排除文件、目录
                        //检查文件是否处于_FSBackup目录，如果存在则跳过
                        if (str_FromFile.Contains(str_FSBackup))
                        {
                            LogPairMessage(str_PairName, "Exclude DIR/File: " + str_FromFile + " due to backup folder", true, true, 3);
                            bl_hitFilterRule = true;
                        }

                        //检查文件是否处于排除列表，如果存在则跳过
                        for (int i = 0; i < arr_FilterRule.Length; i++)
                        {
                            if (str_FromFile.Contains(arr_FilterRule[i]))
                            {
                                LogPairMessage(str_PairName, "Exclude DIR/File: " + str_FromFile + " due to filter rule", true, true, 3);
                                bl_hitFilterRule = true;
                                break;
                            }
                        }

                        if (bl_hitFilterRule) continue;
                        #endregion

                        //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 1)
                        {
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileFromPath + " -A-> " + str_FileToPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_FromFile + " -A-> " + str_ToFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 2)
                        {
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileToPath + " <-A- " + str_FileFromPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_ToFile + " <-A- " + str_FromFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 3)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(str_DirNameChar)) continue;

                            str_LogMsg = "文件: " + str_FromFile + " -U-> " + str_ToFile;
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 4)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(str_DirNameChar)) continue;

                            str_LogMsg = "文件: " + str_ToFile + " <-U- " + str_FromFile;
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除
                        if (int_FileDiffType == 5)
                        {
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileFromPath + " -X-> " + str_FileToPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_FromFile + " -X-> " + str_ToFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除
                        if (int_FileDiffType == 6)
                        {
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileToPath + " <-X- " + str_FileFromPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_ToFile + " <-X- " + str_FromFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //可能出现文件冲突的项目，不做同步，仅发出提醒，DIFFTYPE=7
                        if (int_FileDiffType == 7)
                        {
                            str_LogMsg = "文件<" + str_FileName + ">，目录1<" + str_FileFromPath + ">，目录2<" + str_FileToPath + ">可能存在冲突，请检查";
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogPairMessage(str_PairName, "获取配对差异出错，操作终止，请查阅配对日志文件获取具体信息", true, true, 1);
                    LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    return null;
                }
                #endregion

                DataTable dt_fileDiffExType7 = Create_FileDiff_Empty();
                if (dt_fileDiff.Rows.Count > 0)
                {
                    LogPairMessage(str_PairName, "去除差异等级为7的记录", true, true, 3);
                    DataRow[] dr_Temp = dt_fileDiff.Select("DIFFTYPE<>'7'");
                    if (dr_Temp.Length > 0)
                    {
                        dt_fileDiffExType7 = dr_Temp.CopyToDataTable();
                    }
                }

                string str_AnalysisResult = "配对（" + str_PairName + "）分析完成，共发现" + int_TotalFileFound.ToString() + "条记录，";
                if (dt_fileDiffExType7.Rows.Count.Equals(0))
                {
                    str_AnalysisResult += "没有差异";
                }
                else
                {
                    str_AnalysisResult += "其中" + dt_fileDiffExType7.Rows.Count.ToString() + "条记录需同步";
                }
                LogPairMessage(str_PairName, str_AnalysisResult, true, true, 1);

                return dt_fileDiffExType7;
            }
            else
            {
                LogPairMessage(str_PairName, "分析过程发生异常被中止，可能是因为配对的目录断开连接，请确认后再试", true, true, 1);
                return Create_FileDiff_Empty();
            }
        }

        /// <summary>
        /// 同步文件夹配对
        /// </summary>
        /// <param name="str_PairName"></param>
        private void SyncDirPair(string str_PairName, DataTable dt_fileDiff)
        {
            #region Define Varibles
            LogPairMessage(str_PairName, "开始同步配对（" + str_PairName + "）", true, true, 2);
            DataRow _dr = GetPairInfor(str_PairName);
            if (_dr == null)
            {
                LogPairMessage(str_PairName, "没有找到配对（" + str_PairName + "）的信息，同步失败！", true, true, 1);
                return;
            }

            string str_PairID = _dr.ItemArray[0].ToString();
            string str_Dir1Path = _dr.ItemArray[2].ToString();
            string str_Dir2Path = _dr.ItemArray[3].ToString();
            string str_FilterRule = _dr.ItemArray[6].ToString();
            string str_OutLogMsg = String.Empty;

            string[] arr_FilterRule = String.IsNullOrEmpty(str_FilterRule) ? new string[] { } : str_FilterRule.Split(',');
            DirectoryInfo directoryInfo1 = new DirectoryInfo(str_Dir1Path);
            string str_Dir1TableName = str_PairName + "_DIR1_" + directoryInfo1.Name;
            DirectoryInfo directoryInfo2 = new DirectoryInfo(str_Dir2Path);
            string str_Dir2TableName = str_PairName + "_DIR2_" + directoryInfo2.Name;
            int int_TotalChngCount = dt_fileDiff.Rows.Count;
            int int_SyncedCount = 0;
            ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
            PairPanal.SetSyncCount(int_TotalChngCount);
            const string str_DirNameChar = "~";
            string str_ExceptionFile = String.Empty;
            string str_SyncTimestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMdd_HHmmss");
            #endregion

            foreach (DataRow dataRow in dt_fileDiff.Rows)
            {
                try
                {
                    bool bl_SyncRecordDone = false;
                    int int_TrySyncCount = 0;
                    while (!bl_SyncRecordDone)
                    {
                        #region Define Varibles
                        int_TrySyncCount++;
                        string str_LogMsg = String.Empty;
                        string str_FileName = dataRow.ItemArray[0].ToString();
                        string str_FileFromPath = dataRow.ItemArray[1].ToString();
                        string str_FileToPath = dataRow.ItemArray[2].ToString();
                        string str_FileMD5 = dataRow.ItemArray[3].ToString();
                        string str_FileLastModDate = dataRow.ItemArray[4].ToString();
                        string str_FileSize = dataRow.ItemArray[5].ToString();
                        int int_FileDiffType = int.Parse(dataRow.ItemArray[6].ToString());
                        string str_FromFile = Path.Combine(str_FileFromPath, str_FileName);
                        string str_FromFileTemp = GetTempFileNameWithLocal(str_FromFile, out str_LogMsg);
                        string str_ToFile = Path.Combine(str_FileToPath, str_FileName);
                        string str_FileID = dataRow.ItemArray[7].ToString();
                        bool bl_hitFilterRule = false;
                        str_ExceptionFile = "文件：(" + str_FileName + ")从<" + str_FileFromPath + ">至<" + str_FileToPath + ">发生异常";
                        #endregion

                        #region 排除文件、目录
                        //检查文件是否处于_FSBackup目录，如果存在则跳过
                        if (str_FromFile.Contains(str_FSBackup))
                        {
                            //更新同步进度
                            Add1Sync(str_PairName);
                            bl_hitFilterRule = true;
                        }

                        //检查文件是否处于排除列表，如果存在则跳过
                        for (int i = 0; i < arr_FilterRule.Length; i++)
                        {
                            if (str_FromFile.Contains(arr_FilterRule[i]))
                            {
                                //更新同步进度
                                Add1Sync(str_PairName);
                                bl_hitFilterRule = true;
                                break;
                            }
                        }

                        if (bl_hitFilterRule)
                        {
                            int_SyncedCount++;
                            bl_SyncRecordDone = true;
                            if (File.Exists(str_FromFileTemp))
                            {
                                File.Delete(str_FromFileTemp);
                            }
                            LogPairMessage(str_PairName, "PAIR-SYNC: " + str_PairName + " FilterRule-Exclude Item: " + str_FileFromPath + " due to filter rule", true, true, 4);
                            continue;
                        }
                        #endregion

                        #region Process Diff Records
                        //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 1)
                        {
                            //先判断目标目录是否存在，如果不存在则先创建（仅在非调试模式下生效）
                            if (!cls_Global_Settings.DebugMode && !Directory.Exists(str_FileToPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(str_FileToPath);
                                directoryInfo.Create();
                                cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_DirNameChar, str_FileToPath, "0", String.Empty, directoryInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat), str_PairID, out str_OutLogMsg);
                            }

                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "同步目录: " + str_FileFromPath + " -A-> " + str_FileToPath;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_LogMsg = "同步文件: " + str_FromFile + " -A-> " + str_ToFile;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                    {
                                        if (File.Exists(str_FromFileTemp))
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                        }
                                        else
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                        }
                                        if (bl_SyncRecordDone)
                                        {
                                            if (!cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                                            {
                                                LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        str_LogMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                        LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                                        bl_SyncRecordDone = true;
                                    }
                                }
                            }
                        }
                        //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 2)
                        {
                            //先判断目标目录是否存在，如果不存在则先创建（仅在非调试模式下生效）
                            if (!cls_Global_Settings.DebugMode && !Directory.Exists(str_FileToPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(str_FileToPath);
                                directoryInfo.Create();
                                cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_DirNameChar, str_FileToPath, "0", String.Empty, directoryInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat), str_PairID, out str_OutLogMsg);
                            }

                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "同步目录: " + str_FileToPath + " <-A- " + str_FileFromPath;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_LogMsg = "同步文件: " + str_ToFile + " <-A- " + str_FromFile;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                    {
                                        if (File.Exists(str_FromFileTemp))
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                        }
                                        else
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                        }
                                        if (bl_SyncRecordDone)
                                        {
                                            if (!cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                                            {
                                                LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        str_LogMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                        LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                                        bl_SyncRecordDone = true;
                                    }
                                }
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 3)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                bl_SyncRecordDone = true;
                                continue;
                            }

                            str_LogMsg = "同步文件: " + str_FromFile + " -U-> " + str_ToFile;
                            PairPanal.SetOngoingItem(str_LogMsg);
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                            if (!cls_Global_Settings.DebugMode)
                            {
                                if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                {
                                    if (cls_Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(str_Dir2Path, str_ToFile, false, str_SyncTimestamp);
                                    }
                                    if (File.Exists(str_FromFileTemp))
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                    }
                                    else
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                    }
                                    if (bl_SyncRecordDone)
                                    {
                                        if (!cls_Files_InfoDB.UpdFileInfor(str_Dir2TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                                        {
                                            LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 1);
                                        }
                                    }
                                }
                                else
                                {
                                    str_LogMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                    LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 4)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                bl_SyncRecordDone = true;
                                continue;
                            }

                            str_LogMsg = "同步文件: " + str_ToFile + " <-U- " + str_FromFile;
                            PairPanal.SetOngoingItem(str_LogMsg);
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                            if (!cls_Global_Settings.DebugMode)
                            {
                                if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                {
                                    if (cls_Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(str_Dir1Path, str_ToFile, false, str_SyncTimestamp);
                                    }
                                    if (File.Exists(str_FromFileTemp))
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                    }
                                    else
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                    }
                                    if (bl_SyncRecordDone)
                                    {
                                        if (!cls_Files_InfoDB.UpdFileInfor(str_Dir1TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                                        {
                                            {
                                                LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    str_LogMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                    LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除
                        if (int_FileDiffType == 5)
                        {
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "同步目录: " + str_FileFromPath + " -X-> " + str_FileToPath;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (Directory.Exists(str_FileToPath))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir2Path, str_FileToPath, true, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            Directory.Delete(str_FileToPath, true);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                                    {
                                        LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                            else
                            {
                                str_LogMsg = "同步文件: " + str_FromFile + " -X-> " + str_ToFile;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_ToFile))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir2Path, str_ToFile, false, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            File.Delete(str_ToFile);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                                    {
                                        LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除
                        if (int_FileDiffType == 6)
                        {
                            if (str_FileName.Equals(str_DirNameChar))
                            {
                                str_LogMsg = "同步目录: " + str_FileToPath + " <-X- " + str_FileFromPath;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (Directory.Exists(str_FileToPath))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir1Path, str_FileToPath, true, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            Directory.Delete(str_FileToPath, true);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                                    {
                                        LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                            else
                            {
                                str_LogMsg = "同步文件: " + str_ToFile + " <-X- " + str_FromFile;
                                PairPanal.SetOngoingItem(str_LogMsg);
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_ToFile))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir1Path, str_ToFile, false, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            File.Delete(str_ToFile);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                                    {
                                        LogPairMessage(str_PairName, str_LogMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //可能出现文件冲突的项目，不做同步，仅发出提醒，DIFFTYPE=7
                        if (int_FileDiffType == 7)
                        {
                            bl_SyncRecordDone = true;
                        }
                        #endregion

                        #region Post Processing
                        //检查同步过程发生的错误消息
                        if (!String.IsNullOrEmpty(str_OutLogMsg))
                        {
                            LogPairMessage(str_PairName, "同步过程发生了一些错误，请检查日志文件", true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        //调试模式下强制同步成功
                        if (cls_Global_Settings.DebugMode)
                        {
                            bl_SyncRecordDone = true;
                        }
                        if (bl_SyncRecordDone)
                        {
                            //更新同步进度
                            Add1Sync(str_PairName);
                            int_SyncedCount++;
                            Thread.Sleep(100);
                        }
                        else
                        {
                            if (int_TrySyncCount >= cls_Global_Settings.RetryCountWhenSyncFailed)
                            {
                                LogPairMessage(str_PairName, "同步" + str_FromFile + "失败，超过最大重试次数", true, true, 1);
                                //更新同步进度
                                Add1Sync(str_PairName);
                                int_SyncedCount++;
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                LogPairMessage(str_PairName, "同步" + str_FromFile + "的时候发生了错误，等待" + cls_Global_Settings.RetryIntervalWhenSyncFailed.ToString() + "分钟后重试", true, true, 1);
                                Thread.Sleep(cls_Global_Settings.RetryIntervalWhenSyncFailed * 60000);
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    LogPairMessage(str_PairName, "同步的时候发生了错误！！！检查日志文件", true, true, 1);
                    LogPairMessage(str_PairName, str_ExceptionFile, true, true, 1);
                    LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                }
            }

            if (!cls_Global_Settings.DebugMode)
            {
                //彻底删除状态标记为'DL'的文件记录
                LogPairMessage(str_PairName, "Hard Delete DIR/FILE records whose status is 'DL'", true, true, 4);
                cls_Files_InfoDB.DelFileInforAllHard(str_Dir1TableName, str_PairID);
                cls_Files_InfoDB.DelFileInforAllHard(str_Dir2TableName, str_PairID);
            }

            if (int_SyncedCount.Equals(int_TotalChngCount))
            {
                LogPairMessage(str_PairName, "配对（" + str_PairName + "）同步完成，共同步了" + int_SyncedCount + "条记录", true, true, 1);
                PairPanal.SetOngoingItem();
            }
        }

        /// <summary>
        /// 分析+同步文件夹配对
        /// </summary>
        /// <param name="obj_PairName"></param>
        private void AnalysisSyncDirPair(object obj_PairName)
        {
            string str_PairName = obj_PairName.ToString();
            DataRow _dr = GetPairInfor(str_PairName);
            if (_dr == null)
            {
                LogPairMessage(str_PairName, "没有找到配对（" + str_PairName + "）的信息，分析+同步失败！", true, true, 1);
                return;
            }

            string str_PairID = _dr.ItemArray[0].ToString();
            string str_AutoSyncInterval = _dr.ItemArray[7].ToString();
            int int_AutoSyncInterval = 0;
            if (!Int32.TryParse(str_AutoSyncInterval, out int_AutoSyncInterval)) int_AutoSyncInterval = 0;
            ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
            PairPanal.ResetSyncLabels();

            //更新最后同步时间
            UpdateLastSyncTime(str_PairID, str_PairName, DateTime.Now, false, int_AutoSyncInterval);
            RebindPairTable();

            //RebindPairTable(true); 
            bool bl_IsSync = Thread.CurrentThread.Name.Contains("Sync");
            DataTable dataTableFileDiff = AnalysisDirPair(str_PairName, !bl_IsSync);
            Thread.Sleep(200);
            if (bl_IsSync && dataTableFileDiff != null)
            {
                if (dataTableFileDiff.Rows.Count > 0)
                {
                    SyncDirPair(str_PairName, dataTableFileDiff);
                }
                else
                {
                    //配对没有差异，线程停止1秒等待
                    Thread.Sleep(1000);
                }
            }

            //更新最后同步时间
            UpdateLastSyncTime(str_PairID, str_PairName, DateTime.Now, true, int_AutoSyncInterval);
            RebindPairTable();

            Thread.Sleep(500);
            Thread.CurrentThread.Abort();
        }

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
                    LogPairMessage(str_PairName, "开始自动同步配对（" + str_PairName + "）", true, true, 3);

                    Thread threadSync = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
                    threadSync.Name = "Auto_Sync" + str_ThreadPrefix + str_PairName;
                    threadSync.IsBackground = true;
                    threadPoolOpr.Add(threadSync);
                    threadSync.Start(str_PairName);
                });
                //foreach (string item in arr_PairInfor)
                //{
                //    string str_PairName = item.Split('|')[1];
                //    LogPairMessage(str_PairName, "开始自动同步配对（" + str_PairName + "）", true, true, 3);

                //    Thread threadSync = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
                //    threadSync.Name = "Auto_Sync" + str_ThreadPrefix + str_PairName;
                //    threadSync.IsBackground = true;
                //    threadPoolOpr.Add(threadSync);
                //    threadSync.Start(str_PairName);
                //}
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

        private DataTable Create_FileDiff_Empty()
        {
            DataTable dt_FileDiff = new DataTable("FILEDIFF");
            dt_FileDiff.Columns.Add("FileName");
            dt_FileDiff.Columns.Add("FROMPATH");
            dt_FileDiff.Columns.Add("TOPATH");
            dt_FileDiff.Columns.Add("FileMD5");
            dt_FileDiff.Columns.Add("FileLastModDate");
            dt_FileDiff.Columns.Add("FileSize");
            dt_FileDiff.Columns.Add("DIFFTYPE");
            dt_FileDiff.Columns.Add("PK_FileID");
            return dt_FileDiff;
        }

        private DataTable Get_File_Diff(string str_PairID, string str_PairName, DataTable dt_File1InforDB, DataTable dt_File2InforDB, string str_Dir1Path, string str_Dir2Path, int int_SyncDirection, out string str_OutLogMsg)
        {
            DataTable dt_fileDiff = Create_FileDiff_Empty();
            str_OutLogMsg = String.Empty;
            List<string> lst_ComparedDB2Record = new List<string>();
            DirectoryInfo _dir1 = new DirectoryInfo(str_Dir1Path);
            string str_Dir1TableName = str_PairName + "_DIR1_" + _dir1.Name;
            DirectoryInfo _dir2 = new DirectoryInfo(str_Dir2Path);
            string str_Dir2TableName = str_PairName + "_DIR2_" + _dir2.Name;

            if (dt_File1InforDB != null && dt_File2InforDB != null)
            {
                //首先以Dir1的数据库记录为主
                foreach (DataRow dr_DB1 in dt_File1InforDB.Rows)
                {
                    string str_FileID1 = dr_DB1["PK_FileID"].ToString();
                    string str_FileName1 = dr_DB1["FileName"].ToString();
                    string str_FilePath1 = dr_DB1["FilePath"].ToString();
                    string str_FileSize1 = dr_DB1["FileSize"].ToString();
                    string str_FileMD51 = dr_DB1["FileMD5"].ToString();
                    string str_FileLastModDate1 = dr_DB1["FileLastModDate"].ToString();
                    string str_FileStatus1 = dr_DB1["FileStatus"].ToString();
                    string str_FullFilePath1 = Path.Combine(str_FilePath1, str_FileName1);

                    //从DIR2数据库中寻找此DIR1文件记录对应的记录
                    string str_ToPath1 = str_FilePath1.Replace(str_Dir1Path, str_Dir2Path);
                    DataRow[] dr_SrchFromDB2 = dt_File2InforDB.Select("FileName='" + str_FileName1.Replace("'", "''") + "' and FilePath='" + str_ToPath1.Replace("'", "''") + "'");
                    //如果DIR2数据库中找到匹配，则需要对比是否一样以及同步方向
                    if (dr_SrchFromDB2.Length > 0)
                    {
                        string str_FileID2 = dr_SrchFromDB2[0]["PK_FileID"].ToString();
                        string str_FileSize2 = dr_SrchFromDB2[0]["FileSize"].ToString();
                        string str_FileMD52 = dr_SrchFromDB2[0]["FileMD5"].ToString();
                        string str_FileLastModDate2 = dr_SrchFromDB2[0]["FileLastModDate"].ToString();
                        string str_FileStatus2 = dr_SrchFromDB2[0]["FileStatus"].ToString();
                        string str_FullFilePath2 = Path.Combine(str_ToPath1, str_FileName1);

                        //如果DIR1记录的MD5和DIR2记录的MD5相同，则看是否需要同步删除
                        if (str_FileMD51.Equals(str_FileMD52))
                        {
                            //DIR1中标记为删除，DIR2未被标记为删除的，DIFFTYPE=5，需要从DIR2中删除，同步方向0/2
                            if (str_FileStatus1.Equals("DL") && !str_FileStatus2.Equals("DL"))
                            {
                                if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(2))
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "5", str_FileID2);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //DIR2中标记为删除，DIR1未被标记为删除的，DIFFTYPE=6，需要从DIR1中删除，同步方向0/4
                            if (!str_FileStatus1.Equals("DL") && str_FileStatus2.Equals("DL"))
                            {
                                if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(4))
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_ToPath1, str_FilePath1, str_FileMD52, str_FileLastModDate2, str_FileSize2, "6", str_FileID1);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //DIR1和DIR2都状态相同且MD5相同，无需同步，但要记录下来供DIR2的数据库对比使用
                            if (str_FileStatus1.Equals(str_FileStatus2))
                            {
                                lst_ComparedDB2Record.Add(str_FullFilePath2);
                            }
                        }
                        //如果DIR1记录的MD5和DIR2记录的MD5不同，则看是否需要同步更新
                        else
                        {
                            DateTime dt_Date1 = DateTime.Parse(str_FileLastModDate1);
                            DateTime dt_Date2 = DateTime.Parse(str_FileLastModDate2);
                            //DIR1中文件的修改时间比DIR2的晚，DIFFTYPE=3，需要从DIR1同步至DIR2，同步方向0/1/2/3
                            if (str_FileStatus1.Equals("AC") && str_FileStatus2.Equals("AC") && dt_Date1 > dt_Date2)
                            {
                                if (int_SyncDirection <= 3)
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "3", str_FileID1);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //DIR2中文件的修改时间比DIR1的晚，DIFFTYPE=4，需要从DIR2同步至DIR1，同步方向0/1/4/5
                            if (str_FileStatus1.Equals("AC") && str_FileStatus2.Equals("AC") && dt_Date1 < dt_Date2)
                            {
                                if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(1) || int_SyncDirection.Equals(4) || int_SyncDirection.Equals(5))
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_ToPath1, str_FilePath1, str_FileMD52, str_FileLastModDate2, str_FileSize2, "4", str_FileID2);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //文件MD5不同，但是DIR1或者DIR2中被标记删除，则可能出现冲突，需要手动同步，DIFFTYPE=7
                            if (str_FileStatus1.Equals("DL") || str_FileStatus2.Equals("DL"))
                            {
                                dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "7", str_FileID1);
                                lst_ComparedDB2Record.Add(str_FullFilePath2);
                            }
                            //特殊情况：当MD5不同，但是修改时间和大小都一样的情况，说明其中一边的MD5有问题，需要校正
                            if (str_FileStatus1.Equals("AC") && str_FileStatus2.Equals("AC") && dt_Date1.Equals(dt_Date2) && str_FileSize1.Equals(str_FileSize2))
                            {
                                #region 暂时收回这段代码
                                //LogMessage("文件" + str_FileName1 + "的MD5码存在异常，需要校正", false, true, 1);
                                //LogMessage("开始校正MD5：文件1<" + str_FullFilePath1 + ">", false, true, 2);
                                //string str_TempMD51 = cls_FileHelper.MD5Encrypt(str_FullFilePath1, out str_OutLogMsg);
                                //LogMessage(":" + str_TempMD51, true, false, 2);
                                //LogMessage("开始校正MD5：文件2<" + str_FullFilePath1 + ">", false, true, 2);
                                //string str_TempMD52 = cls_FileHelper.MD5Encrypt(str_FullFilePath2, out str_OutLogMsg);
                                //LogMessage(":" + str_TempMD52, true, false, 2);

                                ////计算后DIR1的MD5不正确，则更新到DIR1的数据库
                                //if (!str_TempMD51.Equals(str_FileMD51))
                                //{
                                //    cls_Files_InfoDB.UpdFileInfor(str_Dir1TableName, str_FileName1, str_FilePath1, str_FileSize1, str_TempMD51, str_FileLastModDate1, str_PairID, out str_OutLogMsg);
                                //    string str_OutLogMsg1 = "校正文件<" + str_FileName1 + ">在DIR1数据库中的MD5";
                                //    LogMessage(str_OutLogMsg1, true, true, 1);
                                //}
                                ////计算后DIR2的MD5不正确，则更新到DIR2的数据库
                                //if (!str_TempMD52.Equals(str_FileMD52))
                                //{
                                //    cls_Files_InfoDB.UpdFileInfor(str_Dir2TableName, str_FileName1, str_ToPath1, str_FileSize2, str_TempMD52, str_FileLastModDate2, str_PairID, out str_OutLogMsg);
                                //    string str_OutLogMsg1 = "校正文件<" + str_FileName1 + ">在DIR2数据库中的MD5";
                                //    LogMessage(str_OutLogMsg1, true, true, 1);
                                //}

                                //校正后两个MD5确实不同，而修改时间和大小都一样的情况，则说明文件有冲突，DIFFTYPE=7
                                //if (!String.IsNullOrEmpty(str_TempMD51) && !String.IsNullOrEmpty(str_TempMD52) && !str_TempMD51.Equals(str_TempMD52))
                                //{
                                //    dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "7", str_FileID1);
                                //}
                                #endregion

                                dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "7", str_FileID1);
                                lst_ComparedDB2Record.Add(str_FullFilePath2);
                            }
                        }
                    }
                    //如果DIR2数据库中找不到匹配，DIFFTYPE=1，需要从DIR1同步至DIR2，同步方向0/1/2/3
                    else
                    {
                        //仅当DIR1的记录为'AC'的时候才需要添加至DIR2
                        if (str_FileStatus1.Equals("AC"))
                        {
                            if (int_SyncDirection <= 3)
                            {
                                dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "1", str_FileID1);
                            }
                        }
                    }
                }

                //以DIR2的数据库记录为主
                foreach (DataRow dr_DB2 in dt_File2InforDB.Rows)
                {
                    string str_FileID2 = dr_DB2["PK_FileID"].ToString();
                    string str_FileName2 = dr_DB2["FileName"].ToString();
                    string str_FilePath2 = dr_DB2["FilePath"].ToString();
                    string str_FileSize2 = dr_DB2["FileSize"].ToString();
                    string str_FileMD52 = dr_DB2["FileMD5"].ToString();
                    string str_FileLastModDate2 = dr_DB2["FileLastModDate"].ToString();
                    string str_FileStatus2 = dr_DB2["FileStatus"].ToString();
                    string str_TempPath = Path.Combine(str_FilePath2, str_FileName2);
                    //上面检查DIR1的时候已经检查过该文件，跳过
                    if (lst_ComparedDB2Record.Contains(str_TempPath)) continue;

                    //从DIR1数据库中寻找此DIR2文件记录对应的记录
                    string str_ToPath2 = str_FilePath2.Replace(str_Dir2Path, str_Dir1Path);
                    DataRow[] dr_SrchFromDB1 = dt_File2InforDB.Select("FileName='" + str_FileName2.Replace("'", "''") + "' and FilePath='" + str_ToPath2.Replace("'", "''") + "'");

                    //如果DIR1数据库中找不到匹配，DIFFTYPE=2，需要从DIR2同步至DIR1，同步方向0/1/4/5
                    if (dr_SrchFromDB1.Length.Equals(0))
                    {
                        //仅当DIR1的记录为'AC'的时候才需要添加至DIR2
                        if (str_FileStatus2.Equals("AC"))
                        {
                            if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(1) || int_SyncDirection.Equals(4) || int_SyncDirection.Equals(5))
                            {
                                dt_fileDiff.Rows.Add(str_FileName2, str_FilePath2, str_ToPath2, str_FileMD52, str_FileLastModDate2, str_FileSize2, "2", str_FileID2);
                            }
                        }
                    }
                }
            }

            return dt_fileDiff;
        }

        private void Add1Analysis(string str_PairName)
        {
            try
            {
                ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
                PairPanal.Add1Analysis();
            }
            catch (Exception ex)
            {
                LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
            }
        }

        private void Add1Sync(string str_PairName)
        {
            try
            {
                ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
                PairPanal.Add1Sync();
            }
            catch (Exception ex)
            {
                LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
            }

        }

        private void UpdateLastSyncTime(string str_PairID, string str_PairName, DateTime dateTime, bool bl_SyncSuccessfulIndc, int int_AutoSyncInterval)
        {
            string str_SyncTime = dateTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
            string str_OutLogMsg = String.Empty;
            //更新最后同步时间
            if (cls_Files_InfoDB.UpdatePairSyncStatus(str_PairID, str_SyncTime, bl_SyncSuccessfulIndc, out str_OutLogMsg))
            {
                ctrl_PairPanal PairPanal = (ctrl_PairPanal)tabControl1.TabPages[str_PairName].Controls[0];
                PairPanal.SetSyncTime(str_SyncTime, int_AutoSyncInterval);
                if (bl_SyncSuccessfulIndc)
                {
                    PairPanal.ResetSyncLabels();
                }
            }
            if (!String.IsNullOrEmpty(str_OutLogMsg) && cls_Global_Settings.DebugMode)
            {
                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
            }
        }
        #endregion

        #region 日志记录方法
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
            string str_LogMsgChngLine = str_LogMsgToFile + (IsChangeLine ? "\n" : "");
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
                LogProgramMessage(ex.Message, true, true, 5);
            }
        }

        /// <summary>
        /// 记录配对日志消息
        /// </summary>
        /// <param name="PairName">配对名</param>
        /// <param name="LogMessage">日志消息</param>
        /// <param name="IsChangeLine">是否换行</param>
        /// <param name="IsAddTS">是否添加时间戳</param>
        /// <param name="MsgTraceLevel">记录的日志消息等级</param>
        /// <param name="IsALwaysLogFile">是否强制写入日志文件（默认否）</param>
        private void LogPairMessage(string PairName, string LogMessage, bool IsChangeLine, bool IsAddTS, int MsgTraceLevel, bool IsALwaysLogFile = false)
        {
            cls_LogPairFile cls_PairLogFile = new cls_LogPairFile(PairName, false);
            string str_LogMsgToFile = (IsAddTS ? DateTime.Now.ToString(cls_Files_InfoDB.DBDateTimeFormat) + " --- " : String.Empty) + LogMessage;
            string str_LogMsgChngLine = str_LogMsgToFile + (IsChangeLine ? "\n" : "");
            bool bl_HasWrittenFile = IsALwaysLogFile;

            if (IsALwaysLogFile)
            {
                cls_PairLogFile.LogMessage(str_LogMsgToFile);
                bl_HasWrittenFile = true;
            }

            try
            {
                //输入的MsgTraceLevel是0，则属于顶级日志，直接处理
                if (MsgTraceLevel == 0)
                {
                    ((ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0]).LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
                    if (!bl_HasWrittenFile)
                    {
                        cls_PairLogFile.LogMessage(str_LogMsgToFile);
                    }
                }
                //输入的MsgTraceLevel > 0，则对比全局变量设置的日志等级做判断处理
                else if (MsgTraceLevel > 0 && MsgTraceLevel <= cls_Global_Settings.TraceLevel)
                {
                    ((ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0]).LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
                    if (cls_Global_Settings.LogMessageToFile && !bl_HasWrittenFile)
                    {
                        cls_PairLogFile.LogMessage(str_LogMsgToFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                LogPairMessage(PairName, ex.Message, true, true, 5);
            }
        }
        #endregion

        #region 其他方法
        private void MoveFileToBackup(string str_RootDirPath, string str_FileFullPath, bool bl_IsDirectory, string str_Timestamp)
        {
            string str_BackupFolder = Path.Combine(str_RootDirPath, str_FSBackup);
            string str_BackupFolder_WithTime = Path.Combine(str_BackupFolder, str_Timestamp);
            string str_TargetFilePath = str_FileFullPath.Replace(str_RootDirPath, str_BackupFolder_WithTime);

            if (!Directory.Exists(str_BackupFolder))
            {
                DirectoryInfo _directoryInfo = new DirectoryInfo(str_BackupFolder);
                _directoryInfo.Create();
                _directoryInfo.Attributes = FileAttributes.Hidden;
            }

            if (!bl_IsDirectory)
            {
                FileHelper.CopyOrMoveFile(str_FileFullPath, str_TargetFilePath, true, true);
            }
            else
            {
                FileHelper.CopyOrMoveDirectory(str_FileFullPath, str_TargetFilePath, true, true);
            }

            DirectoryInfo[] directories = (new DirectoryInfo(str_BackupFolder).GetDirectories("*", SearchOption.TopDirectoryOnly)).Reverse().ToArray();
            directories.Reverse();
            for (int i = 0; i < directories.Length; i++) 
            {
                int i_CurrentIdx = i + 1;
                if (i_CurrentIdx > cls_Global_Settings.MaxKeepBackup)
                {
                    directories[i].Delete(true);
                }
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

        private void RemoveEndedThreads()
        {
            try
            {
                //查找正在进行的分析/同步线程并停止
                threadPoolOpr.RemoveAll(thread => (thread.ThreadState == ThreadState.Suspended || thread.ThreadState == ThreadState.Stopped));

                if (threadPoolOpr.Count == 0)
                {
                    string[] arr_TmpFiles = Directory.GetFiles(cls_Global_Settings.LocalTempFolder);
                    Parallel.ForEach(arr_TmpFiles, (i) => {
                        File.Delete(i);
                    });
                }
            }
            catch (Exception ex)
            {
                LogProgramMessage(ex.Message, true, true, 5);
            }
        }

        private bool CheckCurrentOpr(string str_PairName, out string str_OutMsg)
        {
            str_OutMsg = "配对（" + str_PairName + "）正在";

            foreach (var thread in threadPoolOpr)
            {
                if (thread.Name.Contains(str_PairName) && thread.IsAlive)
                {
                    if (thread.Name.Contains("Analysis"))
                    {
                        str_OutMsg += "分析，请稍后再试";
                        return false;
                    }
                    if (thread.Name.Contains("Sync"))
                    {
                        str_OutMsg += "同步，请稍后再试";
                        return false;
                    }
                }
            }
            return true;
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

            if (dt_DirPair == null || dt_DirPair.Rows.Count.Equals(0)) return;
            DataTable dt_Temp = dt_DirPair.Copy();

            foreach (DataRow dataRow in dt_Temp.Rows)
            {
                string str_PairID = dataRow.ItemArray[0].ToString();
                string str_PairName = dataRow.ItemArray[1].ToString();
                string str_Dir1Path = dataRow.ItemArray[2].ToString();
                string str_Dir2Path = dataRow.ItemArray[3].ToString();
                string str_LastSyncTime = dataRow.ItemArray[4].ToString();
                string str_AutoSyncInterval = dataRow.ItemArray[7].ToString();
                string str_SyncDirection = dataRow.ItemArray[8].ToString();
                int int_SyncDirection = 0;
                if (!String.IsNullOrEmpty(str_SyncDirection)) int_SyncDirection = Convert.ToInt32(str_SyncDirection);
                int int_AutoSyncInterval = 0;
                if (!Int32.TryParse(str_AutoSyncInterval, out int_AutoSyncInterval)) int_AutoSyncInterval = 0;

                TabPage tabPageDirPair = new TabPage(str_PairName);
                tabPageDirPair.Tag = str_PairID;
                tabPageDirPair.Name = str_PairName;
                ctrl_PairPanal PairPanal = new ctrl_PairPanal(str_PairName, str_Dir1Path, str_Dir2Path, int_AutoSyncInterval, int_SyncDirection, cls_Files_InfoDB.DBDateTimeFormat) { Dock = DockStyle.Fill };
                PairPanal.SetSyncTime(str_LastSyncTime, int_AutoSyncInterval);
                tabPageDirPair.Controls.Add(PairPanal);
                tabControl1.Controls.Add(tabPageDirPair);
            }

            if (tabControl1.TabCount > str_SlectedTabIdx)
            {
                tabControl1.SelectedIndex = str_SlectedTabIdx;
            }
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

        private string CalcFileMD5withLocal(string str_FileFullName, bool bl_IsForceSameFile, out string str_OutLogMsg)
        {
            str_OutLogMsg = String.Empty;
            string str_NewFileFullPath = bl_IsForceSameFile ? str_FileFullName : GetTempFileNameWithLocal(str_FileFullName, out str_OutLogMsg);

            if (!String.IsNullOrEmpty(str_NewFileFullPath))
            {
                if (!String.Equals(str_FileFullName, str_NewFileFullPath))
                {
                    if (!FileHelper.CopyOrMoveFile(str_FileFullName, str_NewFileFullPath, false, true))
                    {
                        return String.Empty;
                    }
                }

                return CommonFunctions.MD5Encrypt(str_NewFileFullPath, out str_OutLogMsg);
            }
            else
            {
                return String.Empty;
            }
        }

        private string GetTempFileNameWithLocal(string str_FileFullName, out string str_OutLogMsg)
        {
            str_OutLogMsg = String.Empty;
            string str_NewFileFullPath = str_FileFullName;

            if (cls_Global_Settings.UseLocalTemp)
            {
                bool IsFileInLocal = NetHelper.IsLocalPath(str_FileFullName);
                //string str_PathRoot = Path.GetPathRoot(str_FileFullName);
                //DriveInfo driveInfo = new DriveInfo(str_PathRoot);
                //if (driveInfo.DriveType != DriveType.Fixed)
                if (!IsFileInLocal)
                {
                    string str_NewFileName = CommonFunctions.MD5EncryptString(str_NewFileFullPath, out str_OutLogMsg);
                    if (String.IsNullOrEmpty(str_NewFileName))
                    {
                        return String.Empty;
                    }
                    str_NewFileFullPath = Path.Combine(cls_Global_Settings.LocalTempFolder, str_NewFileName);
                }
            }

            return str_NewFileFullPath;
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
            string str_WarningMsg;
            if (!CheckCurrentOpr(str_PairName, out str_WarningMsg))
            {
                LogProgramMessage(str_WarningMsg, true, true, 1);
                return;
            }

            Thread threadSync = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
            threadSync.Name = "Click_Sync" + str_ThreadPrefix + str_PairName;
            threadSync.IsBackground = true;
            threadPoolOpr.Add(threadSync);
            threadSync.Start(str_PairName);
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
            List<Thread> threadPoolOprSus = new List<Thread> { };
            //查找正在进行的分析/同步线程并挂起
            Parallel.ForEach(threadPoolOpr, (sub_thread) =>
            {
                if (sub_thread.IsAlive)
                {
                    sub_thread.Suspend();
                    threadPoolOprSus.Add(sub_thread);
                    LogProgramMessage("Paused oprating threads:" + sub_thread.Name, true, true, 4);
                }
            });

            if (threadPoolOprSus.Count > 0)
            {
                LogProgramMessage("程序暂停了正在进行的分析/同步操作", true, true, 3);
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
                LogProgramMessage("Abort all oprating threads", true, true, 4);
                //查找正在进行的分析/同步线程并终止
                Parallel.ForEach(threadPoolOprSus, (sub_thread) =>
                {
                    sub_thread.Resume();
                    sub_thread.Abort();
                    LogProgramMessage("Aborted oprating threads:" + sub_thread.Name, true, true, 4);

                });
                if (threadPoolOprSus.Count > 0)
                {
                    LogProgramMessage("因为更改或者删除了配对设置，程序停止了正在进行的分析/同步操作，请重新尝试分析/同步", true, true, 1);
                }
            }
            else
            {
                LogProgramMessage("Resume all oprating threads", true, true, 4);
                //查找正在进行的分析/同步线程并恢复
                Parallel.ForEach(threadPoolOprSus, (sub_thread) =>
                {
                    sub_thread.Resume();
                    LogProgramMessage("Resumed oprating threads:" + sub_thread.Name, true, true, 4);

                });
                if (threadPoolOprSus.Count > 0)
                {
                    LogProgramMessage("程序恢复了暂停了的分析/同步操作", true, true, 3);
                }
            }
            timer1.Start();
        }

        private void 全局设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            new frm_GlobalSettings(str_MainProgramVersion).ShowDialog();
            btnAnalysis.Visible = cls_Global_Settings.DebugMode;
            btnTest.Visible = cls_Global_Settings.DebugMode;
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
        #endregion
    }
}
