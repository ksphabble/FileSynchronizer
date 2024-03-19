using System;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public partial class ctrl_PairPanal : UserControl
    {
        #region 控件属性
        string m_PairName;
        string m_Dir1Path;
        string m_Dir2Path;
        string m_DateTimeFormat;
        DateTime m_LastSyncTime;
        DateTime m_NextSyncTime;
        int i_SyncDirection;
        int i_AutoSyncInterval;
        int int_TotalFileAnalysisFound;
        int int_TotalFileAnalysisDone;
        int int_TotalFileSyncFound;
        int int_TotalFileSyncDone;
        #endregion

        #region 构造函数
        /// <summary>
        /// 创建一个配对控件实例
        /// </summary>
        public ctrl_PairPanal()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 创建一个配对控件实例
        /// </summary>
        /// <param name="str_PairName">配对名</param>
        /// <param name="str_Dir1Path">目录1路径</param>
        /// <param name="str_Dir2Path">目录2路径</param>
        public ctrl_PairPanal(string str_PairName, string str_Dir1Path, string str_Dir2Path, int int_AutoSyncInterval, int int_SyncDirection, string dateTimeFormat)
        {
            InitializeComponent();
            ResetSyncLabels();
            SetOngoingItem();
            m_PairName = str_PairName;
            m_Dir1Path = str_Dir1Path;
            m_Dir2Path = str_Dir2Path;
            i_SyncDirection = int_SyncDirection;
            i_AutoSyncInterval = int_AutoSyncInterval;
            string str_Direction = String.Empty;

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

            lblPairInfor.Text = "配对（" + m_PairName + "）: " + m_Dir1Path + str_Direction + m_Dir2Path;
            m_DateTimeFormat = dateTimeFormat;
            SetSyncTime(String.Empty, i_AutoSyncInterval);
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 设置控件的最后同步时间和同步间隔
        /// </summary>
        /// <param name="dt_LastSyncTime">最后同步时间</param>
        /// <param name="i_AutoSyncInterval">同步间隔</param>
        public void SetSyncTime(string str_LastSyncTime, int int_AutoSyncInterval)
        {
            if(DateTime.TryParse(str_LastSyncTime, out m_LastSyncTime))
            {
                lblLastSyncTime.Text = m_LastSyncTime.ToString(m_DateTimeFormat);

                i_AutoSyncInterval = int_AutoSyncInterval;
                if (i_AutoSyncInterval > 0)
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
        public void SetAnalysisCount(int i_TotalFileAnalysisFound)
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
        public void Add1Analysis()
        {
            int_TotalFileAnalysisDone++;
            CalPercentage();
        }

        /// <summary>
        /// 设置待同步数
        /// </summary>
        /// <param name="i_TotalFileChangeFound">待同步数</param>
        public void SetSyncCount(int i_TotalFileChangeFound)
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
        public void Add1Sync()
        {
            int_TotalFileSyncDone++;
            CalPercentage();
        }

        /// <summary>
        /// 重置控件的所有计数器（分析数+同步数）
        /// </summary>
        public void ResetSyncLabels()
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
        public void SetOngoingItem(string str_OngoingItem = "")
        {
            lblBeingSync.Text = str_OngoingItem;
        }

        /// <summary>
        /// 记录配对日志消息
        /// </summary>
        /// <param name="LogMessage">日志消息</param>
        /// <param name="IsChangeLine">是否换行</param>
        /// <param name="IsAddTS">是否添加时间戳</param>
        public void LogPairMessage(string LogMessage, bool IsChangeLine, bool IsAddTS)
        {
            string str_LogMsg = (IsAddTS ? DateTime.Now.ToString(m_DateTimeFormat) + " --- " : String.Empty) + LogMessage + (IsChangeLine ? "\n" : "");

            if (TxtPairLog.TextLength > 150000)
            {
                TxtPairLog.Clear();
            }
            TxtPairLog.AppendText(str_LogMsg);
            TxtPairLog.ScrollToCaret();
        }

        /// <summary>
        /// 清空控件中的配对日志消息
        /// </summary>
        public void ClearPairLogs()
        {
            TxtPairLog.Clear();
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
        #endregion
    }
}
