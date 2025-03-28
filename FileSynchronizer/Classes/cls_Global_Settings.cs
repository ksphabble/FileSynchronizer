using Common.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class cls_Global_Settings
    {
        #region Variables
        private static GlobalSettingHelper globalSettingHelper;
        private static Dictionary<string, string> dicGlobalSettings;
        private static readonly string[] listGlobalSettings = new string[] {
        //Program Common Variables
        @"DebugMode|bool|false",
        @"LogMessageToFile|bool|true",
        @"DBVersion|string|1.2.1.1",
        @"TraceLevel|int|1",
        @"DelToBackup|bool|true",
        @"AutoRun|bool|false",
        @"RetryCountWhenSyncFailed|int|3",
        @"RetryIntervalWhenSyncFailed|int|5",
        @"UseLocalTemp|bool|false",
        @"LocalTempFolder|string|.\_FSTemp",
        @"AutoClearLog|bool|false",
        @"MinWhenStart|bool|false",
        @"MaxKeepBackup|int|5",
        };

        #region Program Common Variables
        public static bool DebugMode;
        public static bool LogMessageToFile;
        public static string DBVersion;
        public static int TraceLevel;
        public static bool DelToBackup;
        public static bool AutoRun;
        public static int RetryCountWhenSyncFailed;
        public static int RetryIntervalWhenSyncFailed;
        public static bool UseLocalTemp;
        public static string LocalTempFolder;
        public static bool AutoClearLog;
        public static bool MinWhenStart;
        public static int MaxKeepBackup;
        #endregion
        #endregion

        #region Settings Methods
        public static void Init_Settings()
        {
            dicGlobalSettings = cls_Files_InfoDB.SelectAllGlobalSettingsDic();

            if (globalSettingHelper == null)
            {
                globalSettingHelper = new GlobalSettingHelper(dicGlobalSettings);
            }
            else
            {
                globalSettingHelper.LoadGlobalSettingsList(dicGlobalSettings);
            }

            for (int i = 0; i < listGlobalSettings.Length; i++)
            {
                string strVarName = listGlobalSettings[i].Split('|')[0];
                object obj_SettingValue;
                bool bl_GetValueFromDefault = globalSettingHelper.GetGlobalSetting(listGlobalSettings[i], out obj_SettingValue);
                if (bl_GetValueFromDefault)
                {
                    SaveInfoToDB(strVarName, obj_SettingValue.ToString());
                }
                typeof(cls_Global_Settings).GetField(strVarName).SetValue(null, obj_SettingValue);
            }
        }

        /// <summary>
        /// 这里只需要保存GlobalSettings/AutoBrowserSettings窗体中有的属性
        /// </summary>
        public static void SaveInfoToDB()
        {
            for (int i = 0; i < listGlobalSettings.Length; i++)
            {
                string strVarName = listGlobalSettings[i].Split('|')[0];
                string str_Value = typeof(cls_Global_Settings).GetField(strVarName).GetValue(null).ToString();

                SaveInfoToDB(strVarName, str_Value);
            }
        }

        private static bool SaveInfoToDB(string SettingName, string SettingValue)
        {
            string str_Value = string.Empty;
            dicGlobalSettings.TryGetValue(SettingName, out str_Value);
            if (String.IsNullOrEmpty(str_Value) || !str_Value.Equals(SettingValue, StringComparison.OrdinalIgnoreCase))
            {
                return cls_Files_InfoDB.AddorUpdGlobalSetting(SettingName, SettingValue, true);
            }
            return true;
        }
        #endregion
    }
}
