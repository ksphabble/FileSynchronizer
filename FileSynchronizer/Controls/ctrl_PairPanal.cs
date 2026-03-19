using Common.Components;
using D2Phap.FileWatcherEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FileSynchronizer.Local_Utilities;

namespace FileSynchronizer
{
    public partial class ctrl_PairPanal : UserControl
    {
        #region 控件属性
        string g_sPairID;
        string g_sPairName;
        string g_sDir1Path;
        string g_sDir2Path;
        string g_sDateTimeFormat;
        DateTime g_dLastSyncTime;
        DateTime g_dNextSyncTime;
        string g_sFilterRule;
        int g_iSyncDirection;
        int g_iAutoSyncInterval;
        int g_iTotalFileAnalysisFound;
        int g_iTotalFileAnalysisDone;
        int g_iTotalFileSyncFound;
        int g_iTotalFileSyncDone;
        bool g_bIspaused;
        cls_Dir_Pair_Helper Dir_Pair_Helper;
        private PairStatus ps_PairStatus;
        public PairStatus PairStatus { get => ps_PairStatus; }
        public string PairName { get => g_sPairName; }
        //v3.0.0.1
        CancellationTokenSource gTokenSource;
        TaskFactory gTaskFactory;
        Task gCurrentTask;
        FileSystemWatcherEx fw_Dir1 = null;
        FileSystemWatcherEx fw_Dir2 = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建一个配对控件实例
        /// </summary>
        /// <param name="str_PairName">配对名</param>
        /// <param name="str_Dir1Path">目录1路径</param>
        /// <param name="str_Dir2Path">目录2路径</param>
        public ctrl_PairPanal(string str_PairID, string str_PairName, string str_Dir1Path, string str_Dir2Path, string LastSyncDT, string str_FilterRule, int int_AutoSyncInterval, int int_SyncDirection, bool bl_IsPaused, string dateTimeFormat)
        {
            InitializeComponent();
            ResetSyncLabels();
            SetOngoingItem(string.Empty);
            g_sPairID = str_PairID;
            g_sPairName = str_PairName;
            g_sDir1Path = str_Dir1Path;
            g_sDir2Path = str_Dir2Path;
            g_sFilterRule = str_FilterRule;
            g_iSyncDirection = int_SyncDirection;
            g_iAutoSyncInterval = int_AutoSyncInterval;
            string str_Direction = String.Empty;
            g_bIspaused = bl_IsPaused;

            switch (int_SyncDirection)
            {
                case 0:
                case 1: str_Direction = "  <--->  "; break;
                case 2:
                case 3: str_Direction = "  ---->  "; break;
                case 4:
                case 5: str_Direction = "  <----  "; break;
                default:
                    break;
            }

            lblPairInfor.Text = "配对（" + PairName + "）: " + g_sDir1Path + str_Direction + g_sDir2Path;
            g_sDateTimeFormat = dateTimeFormat;
            SetSyncTime(LastSyncDT, g_iAutoSyncInterval);
            Dir_Pair_Helper = new cls_Dir_Pair_Helper(g_sPairID, PairName, g_sDir1Path, g_sDir2Path, LastSyncDT, PairStatus.FREE, str_FilterRule, g_iAutoSyncInterval, g_iSyncDirection, bl_IsPaused);
            Dir_Pair_Helper.LogPairMsg += Dir_Pair_Helper_LogPairMsg;
            Dir_Pair_Helper.SetOngoingItem += Dir_Pair_Helper_SetOngoingItem;
            Dir_Pair_Helper.Add1Analysis += Dir_Pair_Helper_Add1Analysis;
            Dir_Pair_Helper.Add1Sync += Dir_Pair_Helper_Add1Sync;
            Dir_Pair_Helper.PairStatusChange += Dir_Pair_Helper_PairStatusChange;
            Dir_Pair_Helper.ObjectsInforReady += Dir_Pair_Helper_ObjectsInforReady;

            if (g_iAutoSyncInterval.Equals(0))
            {
                InitFileWatchers();
            }
        }

        private void Dir_Pair_Helper_ObjectsInforReady(object sender)
        {
            if (g_iAutoSyncInterval.Equals(0))
            {
                LogPairMessage("Initilized done, do first Sync", true, true, true);
                DoAnalysisSyncDirPair(false);
            }
            OnObjectsInforReady();
        }

        private void Dir_Pair_Helper_PairStatusChange(object sender, PairStatus pairStatus)
        {
            ps_PairStatus = pairStatus;
        }

        private void Dir_Pair_Helper_Add1Sync(object sender, int TotalPending)
        {
            g_iTotalFileSyncFound = TotalPending;
            Add1Sync();
        }

        private void Dir_Pair_Helper_Add1Analysis(object sender, int TotalPending)
        {
            g_iTotalFileAnalysisFound = TotalPending;
            Add1Analysis();
        }

        private void Dir_Pair_Helper_SetOngoingItem(object sender, string OngoingItem)
        {
            SetOngoingItem(OngoingItem);
        }

        private void Dir_Pair_Helper_LogPairMsg(object sender, string LogMessage, bool IsChangeLine, bool IsAddTS)
        {
            LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 设置控件的最后同步时间和同步间隔
        /// </summary>
        /// <param name="dt_LastSyncTime">最后同步时间</param>
        /// <param name="i_AutoSyncInterval">同步间隔</param>
        private void SetSyncTime(string str_LastSyncTime, int int_AutoSyncInterval)
        {
            if(DateTime.TryParse(str_LastSyncTime, out g_dLastSyncTime))
            {
                lblLastSyncTime.Text = g_dLastSyncTime.ToString(g_sDateTimeFormat);

                g_iAutoSyncInterval = int_AutoSyncInterval;
                if (g_iAutoSyncInterval > 0 && !g_bIspaused)
                {
                    g_dNextSyncTime = g_dLastSyncTime.AddMinutes(int_AutoSyncInterval);
                    lblNextSyncTime.Text = g_dNextSyncTime.ToString(g_sDateTimeFormat);
                    lblNextSyncTime.Visible = true;
                    label9.Visible = true;
                    lblNextSyncTime.BringToFront();
                    label9.BringToFront();
                }
                else
                {
                    lblNextSyncTime.Text = String.Empty;
                    lblNextSyncTime.Visible = false;
                    label9.Visible = false;
                }
            }
            else
            {
                lblLastSyncTime.Text = String.Empty;
                lblNextSyncTime.Text = String.Empty;
                lblNextSyncTime.Visible = false;
                label9.Visible = false;
            }
            this.Refresh();
        }

        /// <summary>
        /// 设置待分析数
        /// </summary>
        /// <param name="i_TotalFileAnalysisFound">待分析数</param>
        private void SetAnalysisCount(int i_TotalFileAnalysisFound)
        {
            g_iTotalFileAnalysisFound = i_TotalFileAnalysisFound;
            lblFileCountFound.Text = g_iTotalFileAnalysisFound.ToString();
            //设置总分析数的同时需要重置已分析数
            g_iTotalFileAnalysisDone = 0;
            CalPercentage();
        }

        /// <summary>
        /// 增加一个分析完成数
        /// </summary>
        private void Add1Analysis()
        {
            g_iTotalFileAnalysisDone++;
            CalPercentage();
        }

        /// <summary>
        /// 设置待同步数
        /// </summary>
        /// <param name="i_TotalFileChangeFound">待同步数</param>
        private void SetSyncCount(int i_TotalFileChangeFound)
        {
            g_iTotalFileSyncFound = i_TotalFileChangeFound;
            lblFileCountSync.Text = g_iTotalFileSyncFound.ToString();
            //设置总同步数的同时需要重置已同步数
            g_iTotalFileSyncDone = 0;
            CalPercentage();
        }

        /// <summary>
        /// 增加一个同步完成数
        /// </summary>
        private void Add1Sync()
        {
            g_iTotalFileSyncDone++;
            CalPercentage();
        }

        /// <summary>
        /// 重置控件的所有计数器（分析数+同步数）
        /// </summary>
        private void ResetSyncLabels()
        {
            g_iTotalFileAnalysisFound = 0;
            g_iTotalFileAnalysisDone = 0;
            g_iTotalFileSyncFound = 0;
            g_iTotalFileSyncDone = 0;
            CalPercentage();
        }

        /// <summary>
        /// 设置正在进行的操作
        /// </summary>
        /// <param name="str_OngoingItem">正在进行的操作</param>
        private void SetOngoingItem(string str_OngoingItem)
        {
            lblBeingSync.Text = str_OngoingItem;
        }

        /// <summary>
        /// 记录配对日志消息
        /// </summary>
        /// <param name="LogMessage">日志消息</param>
        /// <param name="IsChangeLine">是否换行</param>
        /// <param name="IsAddTS">是否添加时间戳</param>
        private void LogPairMessage(string LogMessage, bool IsChangeLine, bool IsAddTS, bool OnlyDevModeOrDebugMode = false)
        {
            string str_LogMsg = (IsAddTS ? DateTime.Now.ToString(g_sDateTimeFormat) + " --- " : String.Empty) + LogMessage + (IsChangeLine ? Environment.NewLine : "");

            if (!OnlyDevModeOrDebugMode || (OnlyDevModeOrDebugMode && (Global_Settings.DebugMode || Global_Settings.DevelopMode)))
            {
                lock (TxtPairLog)
                {
                    if (TxtPairLog.TextLength + str_LogMsg.Length > TxtPairLog.MaxLength)
                    {
                        TxtPairLog.Clear();
                    }

                    if (str_LogMsg.Length > TxtPairLog.MaxLength)
                    {
                        str_LogMsg = str_LogMsg.Substring(str_LogMsg.Length - TxtPairLog.MaxLength + 1);
                    }

                    TxtPairLog.AppendText(str_LogMsg);
                    TxtPairLog.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// 计算百分比并显示（分析数+同步数）
        /// </summary>
        private void CalPercentage()
        {
            decimal db_progrsss_Analysis = 0;
            decimal db_progrsss_Sync = 0;

            if (!g_iTotalFileAnalysisFound.Equals(0))
            {
                db_progrsss_Analysis = (decimal)g_iTotalFileAnalysisDone / g_iTotalFileAnalysisFound;
            }
            if (!g_iTotalFileSyncFound.Equals(0))
            {
                db_progrsss_Sync = (decimal)g_iTotalFileSyncDone / g_iTotalFileSyncFound;
            }

            lblFileCountFound.Text = g_iTotalFileAnalysisFound.ToString();
            lblFileCountSync.Text = g_iTotalFileSyncFound.ToString();
            lblAnalysisProgress.Text = string.Format("{0:p}", db_progrsss_Analysis);
            lblSyncProgress.Text = string.Format("{0:p}", db_progrsss_Sync);
        }

        /// <summary>
        /// 分析+同步文件夹配对
        /// </summary>
        private void AnalysisSyncDirPair(object bIsAnalysisOnly)
        {
            try
            {
                lock (this)
                {
                    //在特定时候检查任务是否被取消
                    if (gTokenSource.IsCancellationRequested) { return; }

                    //更新最后同步时间
                    UpdateLastSyncTime(DateTime.Now, false, g_iAutoSyncInterval);
                    OnOperationStarted();
                    ResetSyncLabels();

                    //int iRetryWait = 1;
                    //while (!Dir_Pair_Helper.ObjectsInforReadyIndc)
                    //{
                    //    if (iRetryWait > c_MAX_RetryWaitInfor)
                    //    {
                    //        string str_ObjectInfoNotReadyMessage = "配对信息未完全准备好并且超过最大重试次数，稍后请手动尝试！";
                    //        LogPairMessage(str_ObjectInfoNotReadyMessage, true, true);
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        string str_ObjectInfoNotReadyMessage = "配对信息未完全准备好，稍后将自动重试！";
                    //        LogPairMessage(str_ObjectInfoNotReadyMessage, true, true);
                    //        Thread.Sleep(1000);
                    //        iRetryWait++;
                    //    }
                    //}

                    //在特定时候检查任务是否被取消
                    if (gTokenSource.IsCancellationRequested) { return; }
                    string str_ErrorMsg = String.Empty;
                    if (!Files_InfoDB.RevertUnfinishedSyncDetail(g_sPairName, Global_Settings.DebugMode, out str_ErrorMsg))
                    {
                        LogPairMessage(str_ErrorMsg, true, true);
                    }

                    //在特定时候检查任务是否被取消
                    if (gTokenSource.IsCancellationRequested) { return; }
                    bool IsAnalysisOnly = (bool)bIsAnalysisOnly;
                    DataTable dataTableFileDiff = Dir_Pair_Helper.AnalysisDirPair(IsAnalysisOnly);

                    //在特定时候检查任务是否被取消
                    if (gTokenSource.IsCancellationRequested) { return; }
                    if (!IsAnalysisOnly && dataTableFileDiff != null)
                    {
                        int int_TotalChngCount = dataTableFileDiff.Rows.Count;
                        SetSyncCount(int_TotalChngCount);
                        if (int_TotalChngCount > 0)
                        {
                            //配对有差异，线程暂停500毫秒之后开始同步
                            Thread.Sleep(500);
                            Dir_Pair_Helper.SyncDirPair(dataTableFileDiff);
                        }
                    }

                    //更新最后同步时间
                    UpdateLastSyncTime(DateTime.Now, true, g_iAutoSyncInterval);
                }
            }
            catch (Exception ex)
            {
                LogPairMessage(ex.Message, true, true);
            }
            finally
            {
                StopOngoingOperation();
                OnOperationDone();
            }
        }

        private void UpdateLastSyncTime(DateTime dateTime, bool bl_SyncSuccessfulIndc, int int_AutoSyncInterval)
        {
            string str_SyncTime = dateTime.ToString(Files_InfoDB.DBDateTimeFormat);
            if (Dir_Pair_Helper.UpdateLastSyncTime(str_SyncTime, bl_SyncSuccessfulIndc))
            {
                SetSyncTime(str_SyncTime, int_AutoSyncInterval);
                if (bl_SyncSuccessfulIndc)
                {
                    ResetSyncLabels();
                }
            }
        }

        private void InitTaskFactory()
        {
            gTokenSource = new CancellationTokenSource();
            gTaskFactory = new TaskFactory(gTokenSource.Token);
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 清空控件中的配对日志消息
        /// </summary>
        public void ClearPairLogs()
        {
            TxtPairLog.Clear();
        }

        /// <summary>
        /// 进行分析、同步文件夹配对操作
        /// </summary>
        /// <param name="IsAnalysisOnly">是否仅进行分析操作，true表示仅分析，false表示分析+同步操作</param>
        public void DoAnalysisSyncDirPair(bool IsAnalysisOnly)
        {
            InitTaskFactory();

            gCurrentTask = gTaskFactory.StartNew(() =>
            {
                if (gTaskFactory.CancellationToken.IsCancellationRequested) { return; }
                AnalysisSyncDirPair(IsAnalysisOnly);
            }, gTokenSource.Token);
        }

        public bool IsPairBusy(out PairStatus PairStatus)
        {
            PairStatus = ps_PairStatus;
            return IsPairBusy();
        }

        public bool IsPairBusy()
        {
            return ps_PairStatus != PairStatus.FREE;
        }

        /// <summary>
        /// 打开此配对的日志文件
        /// </summary>
        public void OpenPairLog()
        {
            Dir_Pair_Helper.OpenPairLog();
        }

        /// <summary>
        /// 停止此配对正在进行的操作
        /// </summary>
        /// <returns></returns>
        public bool StopOngoingOperation()
        {
            if (IsPairBusy() && gCurrentTask != null)
            {
                if (!(gCurrentTask.IsCompleted || gCurrentTask.IsCanceled))
                {
                    Dir_Pair_Helper.CancelOperation();
                    gTokenSource.Cancel();
                    gCurrentTask = null;
                    gTokenSource.Dispose();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 暂停或者恢复此配对正在进行的操作
        /// </summary>
        /// <param name="IsPause">是否暂停此配对正在进行的操作，true表示暂停，false表示恢复</param>
        public void PauseResumeOngoingOperation(bool IsPause)
        {
            
        }

        /// <summary>
        /// 暂停此配对的自动同步
        /// </summary>
        /// <param name="WarningMsg"></param>
        /// <returns></returns>
        public bool PausePairAutoSync()
        {
            bool bRet = Dir_Pair_Helper.PausePairAutoSync();
            if (bRet)
            {
                g_bIspaused = !g_bIspaused;
                SetSyncTime(lblLastSyncTime.Text, g_iAutoSyncInterval);
            }
            return bRet;
        }
        #endregion

        #region v3.0.0.1 File Watcher 事件
        private void InitFileWatchers()
        {
            Task task = Task.Run(() =>
            {
                fw_Dir1 = new FileSystemWatcherEx(g_sDir1Path);
                fw_Dir1.IncludeSubdirectories = true;
                fw_Dir1.OnCreated += Fw_Dir1_OnCreated;
                fw_Dir1.OnChanged += Fw_Dir1_OnChanged;
                fw_Dir1.OnDeleted += Fw_Dir1_OnDeleted;
                fw_Dir1.OnRenamed += Fw_Dir1_OnRenamed;
                fw_Dir1.SynchronizingObject = this;
                fw_Dir1.Start();

                fw_Dir2 = new FileSystemWatcherEx(g_sDir2Path);
                fw_Dir2.IncludeSubdirectories = true;
                fw_Dir2.OnCreated += Fw_Dir2_OnCreated;
                fw_Dir2.OnChanged += Fw_Dir2_OnChanged;
                fw_Dir2.OnDeleted += Fw_Dir2_OnDeleted;
                fw_Dir2.OnRenamed += Fw_Dir2_OnRenamed;
                fw_Dir2.SynchronizingObject = this;
                fw_Dir2.Start();

                OnFileWatcherInitDone();
            });
        }

        private void Fw_Dir1_OnCreated(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;
            while (FileHelper.IsFileOpenFS(str_FullPath))
            {
                string str_PrintMsgF = "Fw_Dir1_ObjectChanged - File in use! " + str_FullPath;
                LogPairMessage(str_PrintMsgF, true, true, true);
                Thread.SpinWait(500);
            }

            string str_ObjectName = str_FullPath.Replace(g_sDir1Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir1Path, g_sDir2Path);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgA = "Fw_Dir1_ObjectChanged - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(str_PrintMsgA, true, true, true);
            string str_PrintMsgB = "Fw_Dir1_ObjectChanged - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(str_PrintMsgB, true, true, true);
            string str_PrintMsgC = "Fw_Dir1_ObjectChanged - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(str_PrintMsgC, true, true, true);

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                if (!RealTimeSyncItem(obj_ChangedItem, 0, c_Dir1_Str))
                {
                    LogPairMessage("Fw_Dir1_ObjectChanged - Failed to Sync", true, true, true);
                    LogPairMessage("Fw_Dir1_ObjectChanged - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage("Fw_Dir1_ObjectChanged - Remove from Queue", true, true, true);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir1_OnChanged(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir1Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir1Path, g_sDir2Path);
            bool bl_Sync_CheckQueueOKFromAdd = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgF = "Fw_Dir1_OnChanged - Check Queue From Add result: " + bl_Sync_CheckQueueOKFromAdd.ToString();
            LogPairMessage(str_PrintMsgF, true, true, true);

            if (!bl_Sync_CheckQueueOKFromAdd) return;

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (i_Type.Equals(1))
            {
                bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

                string str_PrintMsgA = "Fw_Dir1_OnChanged - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
                LogPairMessage(str_PrintMsgA, true, true, true);
                string str_PrintMsgB = "Fw_Dir1_OnChanged - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
                LogPairMessage(str_PrintMsgB, true, true, true);
                string str_PrintMsgC = "Fw_Dir1_OnChanged - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
                LogPairMessage(str_PrintMsgC, true, true, true);

                string str_MissingFileName = String.Empty;
                if (Dir_Pair_Helper.CheckMissingFile(str_FullPath, c_Dir1_Str, out str_MissingFileName))
                {
                    LogPairMessage("Fw_Dir1_OnChanged - Located missing file, route to Rename action", true, true, true);
                    FileChangedEvent e2 = e;
                    e2.OldFullPath = str_MissingFileName;
                    Fw_Dir1_OnRenamed(sender, e2);
                    return;
                }

                if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
                {
                    Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    if (!RealTimeSyncItem(obj_ChangedItem, 1, c_Dir1_Str))
                    {
                        LogPairMessage("Fw_Dir1_OnChanged - Failed to Sync", true, true, true);
                        LogPairMessage("Fw_Dir1_OnChanged - Remove from Queue", true, true, true);
                        Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    }
                }
                else
                {
                    LogPairMessage("Fw_Dir1_OnChanged - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir1_OnDeleted(object sender, FileChangedEvent e)
        {
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir1Path, "");
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir1_OnDeleted - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(str_PrintMsgA, true, true, true);
            string str_PrintMsgB = "Fw_Dir1_OnDeleted - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(str_PrintMsgB, true, true, true);
            string str_PrintMsgC = "Fw_Dir1_OnDeleted - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(str_PrintMsgC, true, true, true);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                if (!RealTimeSyncItem(str_FullPath, 2, c_Dir1_Str))
                {
                    LogPairMessage("Fw_Dir1_OnDeleted - Failed to Sync", true, true, true);
                    LogPairMessage("Fw_Dir1_OnDeleted - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage("Fw_Dir1_OnDeleted - Remove from Queue", true, true, true);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir1_OnRenamed(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_NewFullPath = e.FullPath;
            string str_OldFullPath = e.OldFullPath;
            if (CheckFilterRule(g_sFilterRule, str_NewFullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_NewFullPath.Replace(g_sDir1Path, "");
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_NewFullPath, out i_Type);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir1_OnRenamed - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(str_PrintMsgA, true, true, true);
            string str_PrintMsgB = "Fw_Dir1_OnRenamed - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(str_PrintMsgB, true, true, true);
            string str_PrintMsgC = "Fw_Dir1_OnRenamed - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(str_PrintMsgC, true, true, true);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                if (!RealTimeSyncItem(obj_ChangedItem, 3, c_Dir1_Str, str_OldFullPath))
                {
                    LogPairMessage("Fw_Dir1_OnRenamed - Failed to Sync", true, true, true);
                    LogPairMessage("Fw_Dir1_OnRenamed - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage("Fw_Dir1_OnRenamed - Remove from Queue", true, true, true);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir2_OnCreated(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;
            while (FileHelper.IsFileOpenFS(str_FullPath))
            {
                string str_PrintMsgF = "Fw_Dir2_OnCreated - File in use! " + str_FullPath;
                LogPairMessage(str_PrintMsgF, true, true, true);
                Thread.SpinWait(500);
            }

            string str_ObjectName = str_FullPath.Replace(g_sDir2Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir2Path, g_sDir1Path);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgA = "Fw_Dir2_OnCreated - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(str_PrintMsgA, true, true, true);
            string str_PrintMsgB = "Fw_Dir2_OnCreated - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(str_PrintMsgB, true, true, true);
            string str_PrintMsgC = "Fw_Dir2_OnCreated - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(str_PrintMsgC, true, true, true);

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                if (!RealTimeSyncItem(obj_ChangedItem, 0, c_Dir2_Str))
                {
                    LogPairMessage("Fw_Dir2_OnCreated - Failed to Sync", true, true, true);
                    LogPairMessage("Fw_Dir2_OnCreated - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage("Fw_Dir2_OnCreated - Remove from Queue", true, true, true);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir2_OnChanged(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir2Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir2Path, g_sDir1Path);
            bool bl_Sync_CheckQueueOKFromAdd = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgF = "Fw_Dir2_OnChanged - Check Queue From Add result: " + bl_Sync_CheckQueueOKFromAdd.ToString();
            LogPairMessage(str_PrintMsgF, true, true, true);

            if (!bl_Sync_CheckQueueOKFromAdd) return;

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (i_Type.Equals(1))
            {
                bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

                string str_PrintMsgA = "Fw_Dir2_OnChanged - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
                LogPairMessage(str_PrintMsgA, true, true, true);
                string str_PrintMsgB = "Fw_Dir2_OnChanged - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
                LogPairMessage(str_PrintMsgB, true, true, true);
                string str_PrintMsgC = "Fw_Dir2_OnChanged - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
                LogPairMessage(str_PrintMsgC, true, true, true);

                string str_MissingFileName = String.Empty;
                if (Dir_Pair_Helper.CheckMissingFile(str_FullPath, c_Dir2_Str, out str_MissingFileName))
                {
                    LogPairMessage("Fw_Dir2_OnChanged - Located missing file, route to Rename action", true, true, true);
                    FileChangedEvent e2 = e;
                    e2.OldFullPath = str_MissingFileName;
                    Fw_Dir2_OnRenamed(sender, e2);
                    return;
                }

                if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
                {
                    Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    if (!RealTimeSyncItem(obj_ChangedItem, 1, c_Dir2_Str))
                    {
                        LogPairMessage("Fw_Dir2_OnChanged - Failed to Sync", true, true, true);
                        LogPairMessage("Fw_Dir2_OnChanged - Remove from Queue", true, true, true);
                        Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    }
                }
                else
                {
                    LogPairMessage("Fw_Dir2_OnChanged - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir2_OnDeleted(object sender, FileChangedEvent e)
        {
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir2Path, "");
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir2_OnDeleted - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(str_PrintMsgA, true, true, true);
            string str_PrintMsgB = "Fw_Dir2_OnDeleted - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(str_PrintMsgB, true, true, true);
            string str_PrintMsgC = "Fw_Dir2_OnDeleted - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(str_PrintMsgC, true, true, true);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                if (!RealTimeSyncItem(str_FullPath, 2, c_Dir2_Str))
                {
                    LogPairMessage("Fw_Dir2_OnDeleted - Failed to Sync", true, true, true);
                    LogPairMessage("Fw_Dir2_OnDeleted - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage("Fw_Dir2_OnDeleted - Remove from Queue", true, true, true);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private void Fw_Dir2_OnRenamed(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_NewFullPath = e.FullPath;
            string str_OldFullPath = e.OldFullPath;
            if (CheckFilterRule(g_sFilterRule, str_NewFullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_NewFullPath.Replace(g_sDir2Path, "");
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_NewFullPath, out i_Type);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir2_OnRenamed - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(str_PrintMsgA, true, true, true);
            string str_PrintMsgB = "Fw_Dir2_OnRenamed - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(str_PrintMsgB, true, true, true);
            string str_PrintMsgC = "Fw_Dir2_OnRenamed - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(str_PrintMsgC, true, true, true);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                if (!RealTimeSyncItem(obj_ChangedItem, 3, c_Dir2_Str, str_OldFullPath))
                {
                    LogPairMessage("Fw_Dir2_OnRenamed - Failed to Sync", true, true, true);
                    LogPairMessage("Fw_Dir2_OnRenamed - Remove from Queue", true, true, true);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage("Fw_Dir2_OnRenamed - Remove from Queue", true, true, true);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
            }

            LogPairMessage("-----------------------------------------------------------------", true, true, true);
        }

        private bool RealTimeSyncItem(object obj_SyncItem, int int_Action, string str_DirIdx, string str_OldFullPath = "")
        {
            if (obj_SyncItem == null) return false;
            DataTable dt_fileDiff = new DataTable();

            //文件夹操作，只有新增/修改/重命名
            if (obj_SyncItem.GetType() == typeof(DirectoryInfo))
            {
                //int_Action=0 --- 新增
                if (int_Action.Equals(0))
                {
                    dt_fileDiff = Dir_Pair_Helper.Fw_Object_Created(obj_SyncItem, 0, str_DirIdx);
                }
                //int_Action=1 --- 修改
                else if (int_Action.Equals(1))
                {
                    dt_fileDiff = Dir_Pair_Helper.Fw_Object_Changed(obj_SyncItem, 0, str_DirIdx);
                }
                //int_Action=3 --- 重命名是唯一不需要返回DataTable再同步的
                else if (int_Action.Equals(3))
                {
                    return Dir_Pair_Helper.Fw_Object_Renamed(obj_SyncItem, 0, str_DirIdx, str_OldFullPath);
                }
                else
                {
                    return false;
                }
            }
            //文件操作，只有新增/修改/重命名
            else if (obj_SyncItem.GetType() == typeof(FileInfo))
            {
                //int_Action=0 --- 新增
                if (int_Action.Equals(0))
                {
                    dt_fileDiff = Dir_Pair_Helper.Fw_Object_Created(obj_SyncItem, 1, str_DirIdx);
                }
                //int_Action=1 --- 修改
                else if (int_Action.Equals(1))
                {
                    dt_fileDiff = Dir_Pair_Helper.Fw_Object_Changed(obj_SyncItem, 1, str_DirIdx);
                }
                //int_Action=3 --- 重命名是唯一不需要返回DataTable再同步的
                else if (int_Action.Equals(3))
                {
                    return Dir_Pair_Helper.Fw_Object_Renamed(obj_SyncItem, 1, str_DirIdx, str_OldFullPath);
                }
                else
                {
                    return false;
                }
            }
            //路径操作，只有删除
            else if (obj_SyncItem.GetType() == typeof(String))
            {
                //int_Action=2 --- 删除
                if (int_Action.Equals(2))
                {
                    dt_fileDiff = Dir_Pair_Helper.Fw_Object_Deleted(obj_SyncItem.ToString(), 1, str_DirIdx);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (dt_fileDiff != null && dt_fileDiff.Rows.Count > 0)
            {
                return Dir_Pair_Helper.SyncDirPair(dt_fileDiff, true).Result;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 控件的操作事件
        public delegate void OperationDoneHandler(object sender);
        public event OperationDoneHandler OperationDone;
        public delegate void OperationStartedHandler(object sender);
        public event OperationStartedHandler OperationStarted;
        public delegate void FileWatcherInitDoneHandler(object sender);
        public event FileWatcherInitDoneHandler FileWatcherInitDone;
        public delegate void ObjectsInforReadyHandler(object sender);
        public event ObjectsInforReadyHandler ObjectsInforReady;

        protected virtual void OnOperationDone()
        {
            if (OperationDone != null)
            {
                OperationDone(this);
            }
        }

        protected virtual void OnOperationStarted()
        {
            if (OperationStarted != null)
            {
                OperationStarted(this);
            }
        }

        protected virtual void OnFileWatcherInitDone()
        {
            if (FileWatcherInitDone != null)
            {
                FileWatcherInitDone(this);
            }
        }

        protected virtual void OnObjectsInforReady()
        {
            if (ObjectsInforReady != null)
            {
                ObjectsInforReady(this);
            }
        }
        #endregion
    }
}
