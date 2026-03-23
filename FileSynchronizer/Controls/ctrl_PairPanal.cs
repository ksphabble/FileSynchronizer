using Common.Components;
using D2Phap.FileWatcherEx;
using System;
using System.Data;
using System.Diagnostics;
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
        string g_sDir1Path;
        string g_sDir2Path;
        string g_sPairName;
        string g_sDateTimeFormat;
        DateTime g_dLastSyncTime;
        DateTime g_dNextSyncTime;
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
        string c_StopMonitoring = @"停止监控";
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
            g_sDir1Path = str_Dir1Path;
            g_sDir2Path = str_Dir2Path;
            g_sPairName = str_PairName;
            g_iAutoSyncInterval = int_AutoSyncInterval;
            string sDirection = String.Empty;
            g_bIspaused = bl_IsPaused;

            switch (int_SyncDirection)
            {
                case 0:
                case 1: sDirection = "  <--->  "; break;
                case 2:
                case 3: sDirection = "  ---->  "; break;
                case 4:
                case 5: sDirection = "  <----  "; break;
                default:
                    break;
            }

            lblPairInfor.Text = "配对（" + g_sPairName + "）: " + g_sDir1Path + sDirection + g_sDir2Path;
            g_sDateTimeFormat = dateTimeFormat;
            SetSyncTime(LastSyncDT, g_iAutoSyncInterval);
            Dir_Pair_Helper = new cls_Dir_Pair_Helper(str_PairID, g_sPairName, g_sDir1Path, g_sDir2Path, LastSyncDT, PairStatus.FREE, str_FilterRule, g_iAutoSyncInterval, int_SyncDirection, g_bIspaused);
            Dir_Pair_Helper.LogPairMsg += Dir_Pair_Helper_LogPairMsg;
            Dir_Pair_Helper.SetOngoingItem += Dir_Pair_Helper_SetOngoingItem;
            Dir_Pair_Helper.Add1Analysis += Dir_Pair_Helper_Add1Analysis;
            Dir_Pair_Helper.Add1Sync += Dir_Pair_Helper_Add1Sync;
            Dir_Pair_Helper.PairStatusChange += Dir_Pair_Helper_PairStatusChange;
            Dir_Pair_Helper.ObjectsInforReady += Dir_Pair_Helper_ObjectsInforReady;

            InitilizeRemovableDrive();
        }

        private void Dir_Pair_Helper_ObjectsInforReady(object sender, string InitDoneMsg)
        {
            if (g_iAutoSyncInterval.Equals(0))
            {
                DoAnalysisSyncDirPair(false);
            }
            OnObjectsInforReady(InitDoneMsg);
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
            if (DateTime.TryParse(str_LastSyncTime, out g_dLastSyncTime))
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
        private async void AnalysisSyncDirPair(object bIsAnalysisOnly)
        {
            try
            {
                //在特定时候检查任务是否被取消
                if (gTokenSource.IsCancellationRequested) { return; }

                //更新最后同步时间
                UpdateLastSyncTime(DateTime.Now, false, g_iAutoSyncInterval);
                OnOperationStarted();
                ResetSyncLabels();

                //在特定时候检查任务是否被取消
                if (gTokenSource.IsCancellationRequested) { return; }
                string str_ErrorMsg = String.Empty;
                if (!Files_InfoDB.RevertUnfinishedSyncDetail(g_sPairName, Global_Settings.DevelopMode, out str_ErrorMsg))
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
                        await Task.Run(() => Dir_Pair_Helper.SyncDirPair(dataTableFileDiff));
                    }
                }

                //更新最后同步时间
                UpdateLastSyncTime(DateTime.Now, true, g_iAutoSyncInterval);
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

        #region 控件的操作事件
        public delegate void OperationDoneHandler(object sender);
        public event OperationDoneHandler OperationDone;
        public delegate void OperationStartedHandler(object sender);
        public event OperationStartedHandler OperationStarted;
        public delegate void ObjectsInforReadyHandler(object sender, string InitDoneMsg);
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

        protected virtual void OnObjectsInforReady(string InitDoneMsg)
        {
            if (ObjectsInforReady != null)
            {
                ObjectsInforReady(this, InitDoneMsg);
            }
        }
        #endregion

        #region v3.0.0.4 可移动设备的处理
        public void RemovableDriveHandler(string DriveLetter, int ChangeType)
        {
            if (ChangeType < 0 || String.IsNullOrEmpty(DriveLetter)) return;

            InitilizeRemovableDrive();

            if (ChangeType ==0)
            {
                Dir_Pair_Helper.RemovableDriveHandler(DriveLetter, ChangeType);
            }
            else if (ChangeType == 1)
            {
                string str_LogMsgU = "可移动设备（" + DriveLetter + "）从系统移除，将停止监控";
                Dir_Pair_Helper.LogPairMessage(g_sPairName, str_LogMsgU, true, true, GetTraceLevel(1));
                Dir_Pair_Helper.RemoveRemovableDrive(DriveLetter);
                btnStopMonitoringRMD_Click(this, new EventArgs());
            }
        }

        public void InitilizeRemovableDrive()
        {
            string sDir1Root = Path.GetPathRoot(g_sDir1Path);
            string sDir2Root = Path.GetPathRoot(g_sDir2Path);
            bool bEnabled = WinformHelper.CheckRemovableDrive(sDir1Root) || WinformHelper.CheckRemovableDrive(sDir2Root);

            string sDriveLetter = WinformHelper.CheckRemovableDrive(sDir1Root) ? sDir1Root : WinformHelper.CheckRemovableDrive(sDir2Root) ? sDir2Root : String.Empty;

            btnStopMonitoringRMD.Visible = bEnabled;
            btnStopMonitoringRMD.Enabled = bEnabled;
            btnStopMonitoringRMD.Text = c_StopMonitoring + sDriveLetter;
        }

        private void btnStopMonitoringRMD_Click(object sender, EventArgs e)
        {
            string sDir1Root = Path.GetPathRoot(g_sDir1Path);
            string sDir2Root = Path.GetPathRoot(g_sDir2Path);
            string sDriveLetter = WinformHelper.CheckRemovableDrive(sDir1Root) ? sDir1Root : WinformHelper.CheckRemovableDrive(sDir2Root) ? sDir2Root : String.Empty;
            Dir_Pair_Helper.RemoveRemovableDrive(sDriveLetter);
            btnStopMonitoringRMD.Visible = false;
            btnStopMonitoringRMD.Enabled = false;
        }
        #endregion
    }
}
