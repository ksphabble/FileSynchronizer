using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class Sync_Queue_Helper
    {
        #region Commomn Const Strings
        public const string Seperator = @"|";

        public enum SyncAction
        {
            ADD, UPDATE, DELETE, RENAME, ADDUPDATE
        }

        private static List<string> g_lFwSyncQueue = new List<string>();
        private static List<string> g_lNmSyncQueue = new List<string>();
        private static List<string> g_lNewFileSyncQueue = new List<string>();
        #endregion

        #region Common Functions
        /// <summary>
        /// 往File Watcher同步序列里新增一个任务
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="Action">同步操作</param>
        /// <param name="FullPath">被同步的完整路径</param>
        public static void Fw_Sync_AddQueue(string PairName, string FromDirIdx, string ToDirIdx, SyncAction Action, string FullPath)
        {
            string str_CurrentSyncQueue = String.Join(Seperator, PairName, FromDirIdx, Action, ToDirIdx, FullPath);
            g_lFwSyncQueue.Add(str_CurrentSyncQueue);
        }

        /// <summary>
        /// 检查File Watcher同步序列中是否存在相同任务
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="Action">同步操作</param>
        /// <param name="FullPath">被同步的完整路径</param>
        /// <returns>如果同步序列中存在相同任务则返回False并且清除该任务，否则返回True</returns>
        public static bool Fw_Sync_CheckQueue(string PairName, string FromDirIdx, string ToDirIdx, SyncAction Action, string FullPath)
        {
            string str_CheckSyncQueue = String.Join(Seperator, PairName, FromDirIdx, Action, ToDirIdx, FullPath);
            bool bl_FwSyncQueueContains = g_lFwSyncQueue.Contains(str_CheckSyncQueue);

            string str_CheckSyncQueueAdd = String.Join(Seperator, PairName, FromDirIdx, SyncAction.ADD, ToDirIdx, FullPath);
            bool bl_FwSyncQueueContainsAdd = g_lFwSyncQueue.Contains(str_CheckSyncQueueAdd);

            //首先检查由File Watcher发起的同步序列
            if (bl_FwSyncQueueContains)
            {
                return false;
            }
            else if (bl_FwSyncQueueContainsAdd && Action == SyncAction.UPDATE)
            {
                return false;
            }
            else
            {
                //File Watcher发起的同步序列找不到，然后检查由正常同步发起的同步序列
                return Normal_Sync_CheckQueue(PairName, FromDirIdx, ToDirIdx, Action, FullPath);
            }
        }

        /// <summary>
        /// 从File Watcher同步序列里移除一个任务
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="Action">同步操作</param>
        /// <param name="FullPath">被同步的完整路径</param>
        public static void Fw_Sync_DelQueue(string PairName, string FromDirIdx, string ToDirIdx, SyncAction Action, string FullPath)
        {
            string str_CurrentSyncQueue = String.Join(Seperator, PairName, FromDirIdx, Action, ToDirIdx, FullPath);
            g_lFwSyncQueue.Remove(str_CurrentSyncQueue);
        }

        /// <summary>
        /// 往正常同步序列里新增一个任务
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="Action">同步操作</param>
        /// <param name="FullPath">被同步的完整路径</param>
        public static void Normal_Sync_AddQueue(string PairName, string FromDirIdx, string ToDirIdx, SyncAction Action, string FullPath)
        {
            string str_CurrentSyncQueue = String.Join(Seperator, PairName, FromDirIdx, Action, ToDirIdx, FullPath);
            g_lNmSyncQueue.Add(str_CurrentSyncQueue);
        }

        /// <summary>
        /// 检查正常同步序列中是否存在相同任务
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="Action">同步操作</param>
        /// <param name="FullPath">被同步的完整路径</param>
        /// <returns>如果同步序列中存在相同任务则返回False并且清除该任务，否则返回True</returns>
        public static bool Normal_Sync_CheckQueue(string PairName, string FromDirIdx, string ToDirIdx, SyncAction Action, string FullPath)
        {
            string str_CheckSyncQueue = String.Join(Seperator, PairName, FromDirIdx, Action, ToDirIdx, FullPath);
            string str_CheckSyncQueueAdd = String.Join(Seperator, PairName, FromDirIdx, SyncAction.ADD, ToDirIdx, FullPath);

            if (g_lNmSyncQueue.Contains(str_CheckSyncQueue))
            {
                g_lNmSyncQueue.Remove(str_CheckSyncQueue);
                return false;
            }
            else if (g_lNmSyncQueue.Contains(str_CheckSyncQueueAdd) && Action == SyncAction.UPDATE)
            {
                g_lNmSyncQueue.Remove(str_CheckSyncQueueAdd);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 从正常同步序列里移除一个任务
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="Action">同步操作</param>
        /// <param name="FullPath">被同步的完整路径</param>
        public static void Normal_Sync_DelQueue(string PairName, string FromDirIdx, string ToDirIdx, SyncAction Action, string FullPath)
        {
            string str_CurrentSyncQueue = String.Join(Seperator, PairName, FromDirIdx, Action, ToDirIdx, FullPath);
            g_lNmSyncQueue.Remove(str_CurrentSyncQueue);
        }

        /// <summary>
        /// 往新增文件序列里新增一个记录
        /// </summary>
        /// <param name="PairName">配对名称</param>
        /// <param name="FromDirIdx">同步源的DIR索引</param>
        /// <param name="ToDirIdx">同步目标的DIR索引</param>
        /// <param name="FullPath">被同步的完整路径</param>
        public static void New_File_AddQueue(string PairName, string FromDirIdx, string ToDirIdx, string FullPath)
        {
            string str_CurrentSyncQueue = String.Join(Seperator, PairName, FromDirIdx, SyncAction.ADD, ToDirIdx, FullPath);
            g_lNewFileSyncQueue.Add(str_CurrentSyncQueue);
        }

        /// <summary>
        /// 清除正常同步序列中的所有任务
        /// </summary>
        public static void Normal_Sync_ResetQueue(string PairName)
        {
            if (String.IsNullOrEmpty(PairName))
            {
                g_lNmSyncQueue.Clear();
            }
            else
            {
                g_lNmSyncQueue.RemoveAll(item => item.StartsWith(PairName));
            }
        }

        /// <summary>
        /// 返回File Watcher同步序列的字符串形式
        /// </summary>
        /// <returns></returns>
        public static string Get_FwSyncQueue_Str()
        {
            if (g_lFwSyncQueue.Count == 0)
            {
                return String.Empty;
            }
            else
            {
                return String.Join(Environment.NewLine, g_lFwSyncQueue);
            }
        }

        /// <summary>
        /// 返回正常同步序列的字符串形式
        /// </summary>
        /// <returns></returns>
        public static string Get_NmSyncQueue_Str()
        {
            if (g_lNmSyncQueue.Count == 0)
            {
                return String.Empty;
            }
            else
            {
                return String.Join(Environment.NewLine, g_lNmSyncQueue);
            }
        }

        /// <summary>
        /// 返回新增文件序列的字符串形式
        /// </summary>
        /// <returns></returns>
        public static string Get_NewFileQueue_()
        {
            if (g_lNewFileSyncQueue.Count == 0)
            {
                return String.Empty;
            }
            else
            {
                return String.Join(Environment.NewLine, g_lNewFileSyncQueue);
            }
        }
        #endregion
    }
}
