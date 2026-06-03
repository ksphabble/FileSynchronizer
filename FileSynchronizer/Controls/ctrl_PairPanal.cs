using Common.Components;
using System;
using System.Data;
using System.IO;
using System.Text;
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
        string g_sFilterRule;
        int g_iAutoSyncInterval;
        int g_SyncDirection;
        bool g_bIspaused;
        int g_iTotalFileAnalysisFound;
        int g_iTotalFileAnalysisDone;
        int g_iTotalFileSyncFound;
        int g_iTotalFileSyncDone;
        string g_sDateTimeFormat;
        cls_Dir_Pair_Helper Dir_Pair_Helper;
        private PairStatus ps_PairStatus;
        public PairStatus PairStatus { get => ps_PairStatus; }
        public string PairName { get => g_sPairName; }
        //v3.0.0.1
        CancellationTokenSource gTokenSource;
        TaskFactory gTaskFactory;
        Task gCurrentTask;
        string c_StopMonitoring = @"停止监控";
        string c_ProgressLabel = @"已分析{0:p0}，找到{1}个对象，其中{2}个需同步，已同步{3:p0}";
        string c_BeingAction = @"正在进行：{0}";
        string c_PairInforPairInfor = @"配对“{0}”{1}：{2}{3}{4}";
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建一个配对控件实例
        /// </summary>
        /// <param name="dr_DirPair">配对表的记录</param>
        public ctrl_PairPanal(DataRow dr_DirPair, string dateTimeFormat)
        {
            InitializeComponent();

            g_sPairID = dr_DirPair.ItemArray[0].ToString();
            g_sPairName = dr_DirPair.ItemArray[1].ToString();
            g_sDir1Path = dr_DirPair.ItemArray[2].ToString();
            g_sDir2Path = dr_DirPair.ItemArray[3].ToString();
            g_sFilterRule = dr_DirPair.ItemArray[6].ToString();
            string str_AutoSyncInterval = dr_DirPair.ItemArray[7].ToString();
            if (!Int32.TryParse(str_AutoSyncInterval, out g_iAutoSyncInterval))
            { 
                g_iAutoSyncInterval = Int32.MinValue;
            }

            string str_SyncDirection = dr_DirPair.ItemArray[8].ToString();
            string sDirection = String.Empty;
            if (!String.IsNullOrEmpty(str_SyncDirection))
            {
                g_SyncDirection = Convert.ToInt32(str_SyncDirection);
                switch (g_SyncDirection)
                {
                    case 0:
                    case 1: sDirection = " <---> "; break;
                    case 2:
                    case 3: sDirection = " ----> "; break;
                    case 4:
                    case 5: sDirection = " <---- "; break;
                    default:
                        break;
                }
            }

            string str_IsPaused = dr_DirPair.ItemArray[9].ToString();
            if (!Boolean.TryParse(str_IsPaused, out g_bIspaused))
            {
                g_bIspaused = false;
            }

            lblPairInfor.Text = String.Format(c_PairInforPairInfor, g_sPairName, g_iAutoSyncInterval == 0 ? "（实时同步）" : "", g_sDir1Path, sDirection, g_sDir2Path);
            g_sDateTimeFormat = dateTimeFormat;

            //Task.Run(InitElements);
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
        /// 设置待分析数
        /// </summary>
        /// <param name="i_TotalFileAnalysisFound">待分析数</param>
        private void SetAnalysisCount(int i_TotalFileAnalysisFound)
        {
            g_iTotalFileAnalysisFound = i_TotalFileAnalysisFound;
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
            lblBeingSync.Text = String.Format(c_BeingAction, str_OngoingItem);
            lblBeingSync.Left = labelProgress.Right;
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

                    //WriteTextBox(str_LogMsg);
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

            labelProgress.Text = String.Format(c_ProgressLabel, db_progrsss_Analysis, g_iTotalFileAnalysisFound, g_iTotalFileSyncFound, db_progrsss_Sync);
            lblBeingSync.Left = labelProgress.Right;

            //lblFileCountFound.Text = g_iTotalFileAnalysisFound.ToString();
            //lblFileCountSync.Text = g_iTotalFileSyncFound.ToString();
            //lblAnalysisProgress.Text = string.Format("{0:p}", db_progrsss_Analysis);
            //lblSyncProgress.Text = string.Format("{0:p}", db_progrsss_Sync);
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
                UpdateLastSyncTime(DateTime.Now, false);
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
                DataTable dataTableFileDiff = await Dir_Pair_Helper.AnalysisDirPair(IsAnalysisOnly);

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
                        await Task.Run(() => Dir_Pair_Helper.SyncDirPair(dataTableFileDiff, false));
                    }
                }

                //更新最后同步时间
                UpdateLastSyncTime(DateTime.Now, true);
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

        private void UpdateLastSyncTime(DateTime dateTime, bool bl_SyncSuccessfulIndc)
        {
            string str_SyncTime = dateTime.ToString(Files_InfoDB.DBDateTimeFormat);
            if (Dir_Pair_Helper.UpdateLastSyncTime(str_SyncTime, bl_SyncSuccessfulIndc))
            {
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
            }
            return bRet;
        }

        /// <summary>
        /// 刷新配对的文件夹/文件信息
        /// </summary>
        public void RefreshFileAndDirInfo()
        {
            Dir_Pair_Helper.RefreshFileAndDirInfo();
        }

        public void InitElements()
        {
            ResetSyncLabels();
            SetOngoingItem(string.Empty);
            Dir_Pair_Helper = new cls_Dir_Pair_Helper(g_sPairID, g_sPairName, g_sDir1Path, g_sDir2Path, PairStatus.FREE, g_sFilterRule, g_iAutoSyncInterval, g_SyncDirection, g_bIspaused);
            Dir_Pair_Helper.LogPairMsg += Dir_Pair_Helper_LogPairMsg;
            Dir_Pair_Helper.SetOngoingItem += Dir_Pair_Helper_SetOngoingItem;
            Dir_Pair_Helper.Add1Analysis += Dir_Pair_Helper_Add1Analysis;
            Dir_Pair_Helper.Add1Sync += Dir_Pair_Helper_Add1Sync;
            Dir_Pair_Helper.PairStatusChange += Dir_Pair_Helper_PairStatusChange;
            Dir_Pair_Helper.ObjectsInforReady += Dir_Pair_Helper_ObjectsInforReady;

            bool bIsRemovableDriveRelated = false;
            InitilizeRemovableDrive(out bIsRemovableDriveRelated);
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

            bool bIsRemovableDriveRelated = false;
            InitilizeRemovableDrive(out bIsRemovableDriveRelated);

            if (ChangeType ==0)
            {
                Dir_Pair_Helper.RemovableDriveHandler(DriveLetter, ChangeType);
            }
            else if (ChangeType == 1)
            {
                if (bIsRemovableDriveRelated)
                {
                    string str_LogMsgU = "可移动设备（" + DriveLetter + "）从系统移除，将停止监控";
                    Dir_Pair_Helper.LogPairMessage(g_sPairName, str_LogMsgU, true, true, GetTraceLevel(1));
                    Dir_Pair_Helper.RemoveRemovableDrive(DriveLetter);
                    btnStopMonitoringRMD_Click(this, new EventArgs());
                }
            }
        }

        public void InitilizeRemovableDrive(out bool IsRemovableDriveRelated)
        {
            IsRemovableDriveRelated = false;
            string sDir1Root = Path.GetPathRoot(g_sDir1Path);
            string sDir2Root = Path.GetPathRoot(g_sDir2Path);
            bool bEnabled = WinformHelper.CheckRemovableDrive(sDir1Root) || WinformHelper.CheckRemovableDrive(sDir2Root);

            string sDriveLetter = WinformHelper.CheckRemovableDrive(sDir1Root) ? sDir1Root : WinformHelper.CheckRemovableDrive(sDir2Root) ? sDir2Root : String.Empty;

            btnStopMonitoringRMD.Visible = bEnabled;
            btnStopMonitoringRMD.Enabled = bEnabled;
            btnStopMonitoringRMD.Text = c_StopMonitoring + sDriveLetter;

            if (bEnabled)
            {
                IsRemovableDriveRelated = sDir1Root.Equals(sDriveLetter) || sDir2Root.Equals(sDriveLetter);
            }
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
