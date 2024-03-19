using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class cls_Global_Settings_Old
    {
        #region 变量
        private const string StrDebugMode = "DebugMode";
        public static string DebugMode;
        public static bool IsDebugMode { get => String.Equals(DebugMode, true.ToString(), StringComparison.OrdinalIgnoreCase); }
        private const string StrLogMessageToFile = "LogMessageToFile";
        public static string LogMessageToFile;
        public static bool IsLogMessageToFile { get => String.Equals(LogMessageToFile, true.ToString(), StringComparison.OrdinalIgnoreCase); }
        private const string StrDBVersion = "DBVersion";
        public static string DBVersion;
        private const string StrTraceLevel = "TraceLevel";
        public static int TraceLevel;
        private const string StrDelToBackup = "DelToBackup";
        public static string DelToBackup;
        public static bool IsDelToBackup { get => String.Equals(DelToBackup, true.ToString(), StringComparison.OrdinalIgnoreCase); }
        private const string StrAutoRun = "AutoRun";
        public static string AutoRun;
        public static bool IsAutoRun { get => String.Equals(AutoRun, true.ToString(), StringComparison.OrdinalIgnoreCase); }
        private const string StrRetryCountWhenSyncFailed = "RetryCountWhenSyncFailed";
        public static int RetryCountWhenSyncFailed;
        private const string StrRetryIntervalWhenSyncFailed = "RetryIntervalWhenSyncFailed";
        public static int RetryIntervalWhenSyncFailed;
        private const string StrUseLocalTemp = "UseLocalTemp";
        public static string UseLocalTemp;
        public static bool IsUseLocalTemp { get => String.Equals(UseLocalTemp, true.ToString(), StringComparison.OrdinalIgnoreCase); }
        private const string StrLocalTempFolder = "LocalTempFolder";
        public static string LocalTempFolder;
        private const string StrAutoClearLog = "AutoClearLog";
        public static string AutoClearLog;
        public static bool IsAutoClearLog { get => String.Equals(AutoClearLog, true.ToString(), StringComparison.OrdinalIgnoreCase); }

        private const string StrMinWhenStart = "MinWhenStart";
        public static string MinWhenStart;
        public static bool IsMinWhenStart { get => String.Equals(MinWhenStart, true.ToString(), StringComparison.OrdinalIgnoreCase); }
        #endregion

        public static void Init_Settings()
        {
            GetInfoFromDB();
        }

        private static void GetInfoFromDB()
        {
            string[] arr_GblSettings = cls_Files_InfoDB.SelectAllGlobalSettings();

            string str_RetryCountWhenSyncFailed = arr_GblSettings.FirstOrDefault(x => x.Contains(StrRetryCountWhenSyncFailed));
            if (String.IsNullOrEmpty(str_RetryCountWhenSyncFailed))
            {
                SetDefaultInfo(StrRetryCountWhenSyncFailed);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrRetryCountWhenSyncFailed, RetryCountWhenSyncFailed.ToString(), true);
            }
            else
            {
                RetryCountWhenSyncFailed = Convert.ToInt32(str_RetryCountWhenSyncFailed.Split('|')[1]);
            }

            string str_RetryIntervalWhenSyncFailed = arr_GblSettings.FirstOrDefault(x => x.Contains(StrRetryIntervalWhenSyncFailed));
            if (String.IsNullOrEmpty(str_RetryIntervalWhenSyncFailed))
            {
                SetDefaultInfo(StrRetryIntervalWhenSyncFailed);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrRetryIntervalWhenSyncFailed, RetryIntervalWhenSyncFailed.ToString(), true);
            }
            else
            {
                RetryIntervalWhenSyncFailed = Convert.ToInt32(str_RetryIntervalWhenSyncFailed.Split('|')[1]);
            }

            DebugMode = arr_GblSettings.FirstOrDefault(x => x.Contains(StrDebugMode));
            if (String.IsNullOrEmpty(DebugMode))
            {
                SetDefaultInfo(StrDebugMode);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrDebugMode, DebugMode, true);
            }
            else
            {
                DebugMode = DebugMode.Split('|')[1];
            }

            LogMessageToFile = arr_GblSettings.FirstOrDefault(x => x.Contains(StrLogMessageToFile));
            if (String.IsNullOrEmpty(LogMessageToFile))
            {
                SetDefaultInfo(StrLogMessageToFile);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrLogMessageToFile, LogMessageToFile, true);
            }
            else
            {
                LogMessageToFile = LogMessageToFile.Split('|')[1];
            }

            DBVersion = arr_GblSettings.FirstOrDefault(x => x.Contains(StrDBVersion));
            if (String.IsNullOrEmpty(DBVersion))
            {
                SetDefaultInfo(StrDBVersion);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrDBVersion, DBVersion, true);
            }
            else
            {
                DBVersion = DBVersion.Split('|')[1];
            }

            string str_TraceLevelSetup = arr_GblSettings.FirstOrDefault(x => x.Contains(StrTraceLevel));
            if (String.IsNullOrEmpty(str_TraceLevelSetup))
            {
                SetDefaultInfo(StrTraceLevel);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrTraceLevel, TraceLevel.ToString(), true);
            }
            else
            {
                TraceLevel = Convert.ToInt32(str_TraceLevelSetup.Split('|')[1]);
            }

            DelToBackup = arr_GblSettings.FirstOrDefault(x => x.Contains(StrDelToBackup));
            if (String.IsNullOrEmpty(DelToBackup))
            {
                SetDefaultInfo(StrDelToBackup);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrDelToBackup, DelToBackup, true);
            }
            else
            {
                DelToBackup = DelToBackup.Split('|')[1];
            }

            AutoRun = arr_GblSettings.FirstOrDefault(x => x.Contains(StrAutoRun));
            if (String.IsNullOrEmpty(AutoRun))
            {
                SetDefaultInfo(StrAutoRun);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrAutoRun, AutoRun, true);
            }
            else
            {
                AutoRun = AutoRun.Split('|')[1];
            }

            UseLocalTemp = arr_GblSettings.FirstOrDefault(x => x.Contains(StrUseLocalTemp));
            if (String.IsNullOrEmpty(UseLocalTemp))
            {
                SetDefaultInfo(StrUseLocalTemp);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrUseLocalTemp, UseLocalTemp, true);
            }
            else
            {
                UseLocalTemp = UseLocalTemp.Split('|')[1];
            }

            LocalTempFolder = arr_GblSettings.FirstOrDefault(x => x.Contains(StrLocalTempFolder));
            if (String.IsNullOrEmpty(LocalTempFolder))
            {
                SetDefaultInfo(StrLocalTempFolder);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrLocalTempFolder, LocalTempFolder, true);
            }
            else
            {
                LocalTempFolder = LocalTempFolder.Split('|')[1];
            }

            AutoClearLog = arr_GblSettings.FirstOrDefault(x => x.Contains(StrAutoClearLog));
            if (String.IsNullOrEmpty(AutoClearLog))
            {
                SetDefaultInfo(StrAutoClearLog);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrAutoClearLog, AutoClearLog, true);
            }
            else
            {
                AutoClearLog = AutoClearLog.Split('|')[1];
            }

            MinWhenStart = arr_GblSettings.FirstOrDefault(x => x.Contains(StrMinWhenStart));
            if (String.IsNullOrEmpty(MinWhenStart))
            {
                SetDefaultInfo(StrMinWhenStart);
                cls_Files_InfoDB.AddorUpdGlobalSetting(StrMinWhenStart, MinWhenStart, true);
            }
            else
            {
                MinWhenStart = MinWhenStart.Split('|')[1];
            }
        }

        /// <summary>
        /// 这里只需要保存GlobalSettings窗体中有的属性
        /// </summary>
        public static void SaveInfoToDB()
        {
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrDebugMode, DebugMode, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrLogMessageToFile, LogMessageToFile, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrTraceLevel, TraceLevel.ToString(), true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrDelToBackup, DelToBackup, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrAutoRun, AutoRun, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrRetryCountWhenSyncFailed, RetryCountWhenSyncFailed.ToString(), true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrRetryIntervalWhenSyncFailed, RetryIntervalWhenSyncFailed.ToString(), true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrUseLocalTemp, UseLocalTemp, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrLocalTempFolder, LocalTempFolder, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrAutoClearLog, AutoClearLog, true);
            cls_Files_InfoDB.AddorUpdGlobalSetting(StrMinWhenStart, MinWhenStart, true);
        }

        private static void SetDefaultInfo(string str_Source)
        {
            switch (str_Source)
            {
                case StrDebugMode: DebugMode = @"false"; break;
                case StrLogMessageToFile: LogMessageToFile = @"true"; break;
                case StrDBVersion: DBVersion = "1.2.1.1"; break;
                case StrTraceLevel: TraceLevel = 1; break;
                case StrDelToBackup: DelToBackup = @"true"; break;
                case StrAutoRun: AutoRun = @"false"; break;
                case StrRetryCountWhenSyncFailed: RetryCountWhenSyncFailed = 3; break;
                case StrRetryIntervalWhenSyncFailed: RetryIntervalWhenSyncFailed = 5; break;
                case StrUseLocalTemp: UseLocalTemp = @"false"; break;
                case StrLocalTempFolder: LocalTempFolder = @".\_FSTemp"; break;
                case StrAutoClearLog: AutoClearLog = @"false"; break;
                case StrMinWhenStart: MinWhenStart = @"true"; break;
                default: break;
            }
        }
    }
}
