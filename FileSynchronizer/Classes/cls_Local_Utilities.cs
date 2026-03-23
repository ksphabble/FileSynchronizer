using System;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class Local_Utilities
    {
        #region Commomn Const Strings
        public const string c_ProgramTitle = @"FileSynchronizer";
        public const string c_FSBackup_Str = @"_FSBackup";
        public const string c_DirNameChar_Str = "~";
        //public const string c_ThreadPrefix_Str = @"->";
        public const Int32 c_MaxDirLengthWIN32_Int = 260;
        //public const string c_ExtMaxDirLengthPrefix_Str = @"\\?\";
        public const string c_TempUpdFileExt_Str = @".tmp";
        public const string c_UpdateURL_Str = @"https://api.github.com/repos/ksphabble/FileSynchronizer/releases/latest";
        public const string c_GithubURL = @"https://github.com/ksphabble/FileSynchronizer";
        public const string c_Dir1_Str = @"DIR1";
        public const string c_Dir2_Str = @"DIR2";
        public const Int32 c_WaitFileClose_Int = 5000;
        public const Int32 c_Timer_Interval = 30000;
        public const Int32 c_MAX_RetryWaitInfor = 10;

        public enum PairStatus
        {
            FREE = 0, ANALYSIS = 1, SYNC = 2
        }
        #endregion

        #region Common Functions
        /// <summary>
        /// 检查文件是否应该跳过处理（处于_FSBackup目录或者是处于排除列表或者路径长度超过系统限制的260个字符）
        /// </summary>
        /// <param name="str_FilterRule">排除列表，不同的排除关键字之间用英文逗号","分隔</param>
        /// <param name="str_FileFullName">文件或目录的完整路径</param>
        /// <param name="str_OutMessage">如果文件或目录应该跳过，则输出一个字符串用于提示</param>
        /// <returns>返回该文件或目录是否应该跳过，True为跳过，False为处理</returns>
        public static bool CheckFilterRule(string str_FilterRule, string str_FileFullName, out string str_OutMessage)
        {
            bool bl_isSkip = false;
            string[] arr_FilterRule = String.IsNullOrEmpty(str_FilterRule) ? new string[] { } : str_FilterRule.Split(',');
            str_OutMessage = String.Empty;

            //检查文件是否处于_FSBackup目录，如果存在则跳过
            if (str_FileFullName.Contains(c_FSBackup_Str) || str_FileFullName.EndsWith(c_TempUpdFileExt_Str))
            {
                bl_isSkip = true;
            }

            //检查文件是否处于排除列表，如果存在则跳过
            Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
            {
                if (str_FileFullName.Contains(i))
                {
                    bl_isSkip = true;
                    LoopState.Break();
                }
            });

            if (bl_isSkip)
            {
                str_OutMessage = "FilterRuleHit-Skip:" + str_FileFullName;
            }
            //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
            else if (str_FileFullName.Length > c_MaxDirLengthWIN32_Int)
            {
                str_OutMessage = "PathLengthExceedsLimit-Skip:" + str_FileFullName;
                bl_isSkip = true;
            }

            return bl_isSkip;
        }

        public static int GetTraceLevel(int OriginalTraceLevel)
        {
            return Global_Settings.DebugMode ? -1 : OriginalTraceLevel;
        }
        #endregion
    }
}
