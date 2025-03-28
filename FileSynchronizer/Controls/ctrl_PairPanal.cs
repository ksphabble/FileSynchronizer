using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using static FileSynchronizer.cls_Common_Constants;

namespace FileSynchronizer
{
    public partial class ctrl_PairPanal : UserControl
    {
        #region 控件属性
        string m_PairID;
        string m_PairName;
        string m_Dir1Path;
        string m_Dir2Path;
        string m_DateTimeFormat;
        DateTime m_LastSyncTime;
        DateTime m_NextSyncTime;
        string m_FilterRule;
        int i_SyncDirection;
        int i_AutoSyncInterval;
        int int_TotalFileAnalysisFound;
        int int_TotalFileAnalysisDone;
        int int_TotalFileSyncFound;
        int int_TotalFileSyncDone;
        bool bl_Ispaused;
        cls_Dir_Pair_Helper Dir_Pair_Helper;
        Thread threadOperation;
        private PairStatus ps_PairStatus;
        public PairStatus PairStatus { get => ps_PairStatus; }
        public string PairName { get => m_PairName; }
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
            m_PairID = str_PairID;
            m_PairName = str_PairName;
            m_Dir1Path = str_Dir1Path;
            m_Dir2Path = str_Dir2Path;
            m_FilterRule = str_FilterRule;
            i_SyncDirection = int_SyncDirection;
            i_AutoSyncInterval = int_AutoSyncInterval;
            string str_Direction = String.Empty;
            bl_Ispaused = bl_IsPaused;

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

            lblPairInfor.Text = "配对（" + PairName + "）: " + m_Dir1Path + str_Direction + m_Dir2Path;
            m_DateTimeFormat = dateTimeFormat;
            SetSyncTime(LastSyncDT, i_AutoSyncInterval);
            Dir_Pair_Helper = new cls_Dir_Pair_Helper(m_PairID, PairName, m_Dir1Path, m_Dir2Path, LastSyncDT, PairStatus.FREE, str_FilterRule, i_AutoSyncInterval, i_SyncDirection, bl_IsPaused);
            Dir_Pair_Helper.LogPairMsg += Dir_Pair_Helper_LogPairMsg;
            Dir_Pair_Helper.SetOngoingItem += Dir_Pair_Helper_SetOngoingItem;
            Dir_Pair_Helper.Add1Analysis += Dir_Pair_Helper_Add1Analysis;
            Dir_Pair_Helper.Add1Sync += Dir_Pair_Helper_Add1Sync;
            Dir_Pair_Helper.PairStatusChange += Dir_Pair_Helper_PairStatusChange;
        }

        private void Dir_Pair_Helper_PairStatusChange(object sender, PairStatus pairStatus)
        {
            ps_PairStatus = pairStatus;
        }

        private void Dir_Pair_Helper_Add1Sync(object sender, int TotalPending)
        {
            int_TotalFileSyncFound = TotalPending;
            Add1Sync();
        }

        private void Dir_Pair_Helper_Add1Analysis(object sender, int TotalPending)
        {
            int_TotalFileAnalysisFound = TotalPending;
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
            if(DateTime.TryParse(str_LastSyncTime, out m_LastSyncTime))
            {
                lblLastSyncTime.Text = m_LastSyncTime.ToString(m_DateTimeFormat);

                i_AutoSyncInterval = int_AutoSyncInterval;
                if (i_AutoSyncInterval > 0 && !bl_Ispaused)
                {
                    m_NextSyncTime = m_LastSyncTime.AddMinutes(int_AutoSyncInterval);
                    lblNextSyncTime.Text = m_NextSyncTime.ToString(m_DateTimeFormat);
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
            int_TotalFileAnalysisFound = i_TotalFileAnalysisFound;
            lblFileCountFound.Text = int_TotalFileAnalysisFound.ToString();
            //设置总分析数的同时需要重置已分析数
            int_TotalFileAnalysisDone = 0;
            CalPercentage();
        }

        /// <summary>
        /// 增加一个分析完成数
        /// </summary>
        private void Add1Analysis()
        {
            int_TotalFileAnalysisDone++;
            CalPercentage();
        }

        /// <summary>
        /// 设置待同步数
        /// </summary>
        /// <param name="i_TotalFileChangeFound">待同步数</param>
        private void SetSyncCount(int i_TotalFileChangeFound)
        {
            int_TotalFileSyncFound = i_TotalFileChangeFound;
            lblFileCountSync.Text = int_TotalFileSyncFound.ToString();
            //设置总同步数的同时需要重置已同步数
            int_TotalFileSyncDone = 0;
            CalPercentage();
        }

        /// <summary>
        /// 增加一个同步完成数
        /// </summary>
        private void Add1Sync()
        {
            int_TotalFileSyncDone++;
            CalPercentage();
        }

        /// <summary>
        /// 重置控件的所有计数器（分析数+同步数）
        /// </summary>
        private void ResetSyncLabels()
        {
            int_TotalFileAnalysisFound = 0;
            int_TotalFileAnalysisDone = 0;
            int_TotalFileSyncFound = 0;
            int_TotalFileSyncDone = 0;
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
        private void LogPairMessage(string LogMessage, bool IsChangeLine, bool IsAddTS)
        {
            string str_LogMsg = (IsAddTS ? DateTime.Now.ToString(m_DateTimeFormat) + " --- " : String.Empty) + LogMessage + (IsChangeLine ? "\n" : "");

            if (TxtPairLog.TextLength + str_LogMsg.Length > TxtPairLog.MaxLength)
            {
                TxtPairLog.Clear();
            }
            TxtPairLog.AppendText(str_LogMsg);
            TxtPairLog.ScrollToCaret();
        }

        /// <summary>
        /// 计算百分比并显示（分析数+同步数）
        /// </summary>
        private void CalPercentage()
        {
            decimal db_progrsss_Analysis = 0;
            decimal db_progrsss_Sync = 0;

            if (!int_TotalFileAnalysisFound.Equals(0))
            {
                db_progrsss_Analysis = (decimal)int_TotalFileAnalysisDone / int_TotalFileAnalysisFound;
            }
            if (!int_TotalFileSyncFound.Equals(0))
            {
                db_progrsss_Sync = (decimal)int_TotalFileSyncDone / int_TotalFileSyncFound;
            }

            lblFileCountFound.Text = int_TotalFileAnalysisFound.ToString();
            lblFileCountSync.Text = int_TotalFileSyncFound.ToString();
            lblAnalysisProgress.Text = string.Format("{0:p}", db_progrsss_Analysis);
            lblSyncProgress.Text = string.Format("{0:p}", db_progrsss_Sync);
        }

        /// <summary>
        /// 分析文件夹配对
        /// </summary>
        /// <param name="obj_PairName"></param>
        private DataTable AnalysisDirPair(bool IsAnalysisOnly)
        {
            return Dir_Pair_Helper.AnalysisDirPair(IsAnalysisOnly);
        }

        /// <summary>
        /// 同步文件夹配对
        /// </summary>
        /// <param name="str_PairName"></param>
        private void SyncDirPair(DataTable dt_fileDiff)
        {
            Dir_Pair_Helper.SyncDirPair(dt_fileDiff);
        }

        /// <summary>
        /// 分析+同步文件夹配对
        /// </summary>
        private void AnalysisSyncDirPair(object IsAnalysisOnly)
        {
            //更新最后同步时间
            UpdateLastSyncTime(DateTime.Now, false, i_AutoSyncInterval);
            OnOperationStarted();
            bool bl_IsAnalysisOnly = (bool)IsAnalysisOnly;
            ResetSyncLabels();

            bool bl_IsSync = Thread.CurrentThread.Name.Contains("Sync");
            DataTable dataTableFileDiff = AnalysisDirPair(!bl_IsSync);

            if (!bl_IsAnalysisOnly)
            {
                if (bl_IsSync && dataTableFileDiff != null)
                {
                    int int_TotalChngCount = dataTableFileDiff.Rows.Count;
                    SetSyncCount(int_TotalChngCount);
                    if (int_TotalChngCount > 0)
                    {
                        //配对有差异，线程暂停500毫秒之后开始同步
                        Thread.Sleep(500);
                        SyncDirPair(dataTableFileDiff);
                    }
                }
            }

            //更新最后同步时间
            UpdateLastSyncTime(DateTime.Now, true, i_AutoSyncInterval);
            OnOperationDone();
            Thread.Sleep(300);
            Thread.CurrentThread.Abort();
        }

        private void UpdateLastSyncTime(DateTime dateTime, bool bl_SyncSuccessfulIndc, int int_AutoSyncInterval)
        {
            string str_SyncTime = dateTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
            if (Dir_Pair_Helper.UpdateLastSyncTime(str_SyncTime, bl_SyncSuccessfulIndc))
            {
                SetSyncTime(str_SyncTime, int_AutoSyncInterval);
                if (bl_SyncSuccessfulIndc)
                {
                    ResetSyncLabels();
                }
            }
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
            threadOperation = new Thread(new ParameterizedThreadStart(AnalysisSyncDirPair));
            threadOperation.Name = (IsAnalysisOnly ? "Click_Analysis" : "Click_Sync") + cls_Common_Constants.C_StrThreadPrefix + PairName;
            threadOperation.IsBackground = true;
            threadOperation.Start(IsAnalysisOnly);
        }

        public bool IsPairBusy(out PairStatus PairStatus)
        {
            PairStatus = ps_PairStatus;
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
            if (ps_PairStatus != PairStatus.FREE && threadOperation != null && threadOperation.IsAlive)
            {
                threadOperation.Abort();
                Thread.Sleep(500);
                ps_PairStatus = PairStatus.FREE;
                ResetSyncLabels();
                SetOngoingItem(string.Empty);
                return true;
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
            if (IsPause)
            {
                if (ps_PairStatus != PairStatus.FREE && threadOperation != null && threadOperation.IsAlive)
                {
                    threadOperation.Suspend();
                }
            }
            else
            {
                if (threadOperation != null && threadOperation.IsAlive)
                {
                    threadOperation.Resume();
                }
            }
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
                bl_Ispaused = !bl_Ispaused;
                SetSyncTime(lblLastSyncTime.Text, i_AutoSyncInterval);
            }
            return bRet;
        }
        #endregion

        #region 控件的操作事件
        public delegate void OperationDoneHandler(object sender);
        public event OperationDoneHandler OperationDone;
        public delegate void OperationStartedHandler(object sender);
        public event OperationStartedHandler OperationStarted;

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
        #endregion
    }
}
