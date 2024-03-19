using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FileSynchronizer
{
    /// <summary>
    /// 数据库总入口，能够给根据当前数据库类型调用对应的数据库方法
    /// </summary>
    public static class cls_Files_InfoDB
    {
        #region Variables
        private static cls_SQLBuilder.DATABASE_TYPE m_DBType;
        private static string m_DBDateTimeFormat;
        public static cls_SQLBuilder.DATABASE_TYPE DBType { get => m_DBType; }
        public static string DBDateTimeFormat { get => m_DBDateTimeFormat; }
        #endregion

        #region Basic DB Methods
        public static void OpenConnection()
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                cls_Files_InfoDB_ACCESS.OpenConnection();
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                cls_Files_InfoDB_SQLITE.OpenConnection();
            }
        }

        public static void CloseConnection()
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                cls_Files_InfoDB_ACCESS.CloseConnection();
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                cls_Files_InfoDB_SQLITE.CloseConnection();
            }
        }

        /// <summary>
        /// 检查数据库文件并返回检查结果（！！！此方法必须在所有数据库操作之前完成！！！）
        /// </summary>
        /// <returns></returns>
        public static string CheckDBFile()
        {
            //首先检查数据库文件是否存在，并确定数据库类型（顺序：SQLITE->ACCESS）
            m_DBType = cls_SQLBuilder.DATABASE_TYPE.NONE;
            if (File.Exists(cls_Files_InfoDB_SQLITE.DBFileName))
            {
                m_DBType = cls_SQLBuilder.DATABASE_TYPE.SQLITE;
                m_DBDateTimeFormat = cls_Files_InfoDB_SQLITE.DBDateTimeFormat;
            }
            else if (File.Exists(cls_Files_InfoDB_ACCESS.DBFileName))
            {
                m_DBType = cls_SQLBuilder.DATABASE_TYPE.ACCESS;
                m_DBDateTimeFormat = cls_Files_InfoDB_ACCESS.DBDateTimeFormat;
            }

            //如果数据库文件不存在，则默认生成SQLITE数据库
            if (m_DBType == cls_SQLBuilder.DATABASE_TYPE.NONE)
            {
                m_DBType = cls_SQLBuilder.DATABASE_TYPE.SQLITE;
                m_DBDateTimeFormat = cls_Files_InfoDB_SQLITE.DBDateTimeFormat;
                return cls_Files_InfoDB_SQLITE.CheckDBFile(false);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 检查数据库版本升级
        /// </summary>
        /// <returns>升级后的最新版本</returns>
        public static string CheckDBUpgrade(string str_TargetVersion)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.CheckDBUpgrade(str_TargetVersion);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.CheckDBUpgrade(str_TargetVersion);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="str_BKKeyString">输入备份词</param>
        /// <returns></returns>
        public static bool BackupDBFile(string str_BKKeyString)
        {
            string str_CurrentDBFile = String.Empty;

            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                str_CurrentDBFile = cls_Files_InfoDB_ACCESS.DBFileName;
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                str_CurrentDBFile = cls_Files_InfoDB_SQLITE.DBFileName;
            }

            string str_NewDBFileDir = Path.Combine(Application.StartupPath, "FileSynchronizer_DB_Backup");
            string str_BKKeyStringWithTS = str_BKKeyString + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string str_DatabaseExt = new FileInfo(str_CurrentDBFile).Extension;
            string str_NewDBFile = Path.Combine(str_NewDBFileDir, "FileSynchronizer_DB_bk" + str_BKKeyStringWithTS + str_DatabaseExt);
            if (!Directory.Exists(str_NewDBFileDir))
            {
                Directory.CreateDirectory(str_NewDBFileDir);
            }

            try
            {
                File.Copy(str_CurrentDBFile, str_NewDBFile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 数据库迁移
        /// </summary>
        /// <param name="ToDatabaseType">迁移至新的数据库类型</param>
        /// <param name="str_OutMsg">如果迁移结果为失败，则输出相关的错误字符串</param>
        /// <returns>迁移结果</returns>
        public static bool DBMigration(cls_SQLBuilder.DATABASE_TYPE ToDatabaseType, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            string str_NewDBFileName = String.Empty;
            //检查输入的数据库类型是否跟目前正在使用的数据库类型相同，如果是，则抛出错误
            if (DBType.Equals(ToDatabaseType))
            {
                str_OutMsg = @"不能迁移到相同类型的数据库中！";
                return false;
            }

            //Step 1: 创建新的目标数据库类型的空数据库
            if (ToDatabaseType == cls_SQLBuilder.DATABASE_TYPE.SQLITE)
            {
                str_OutMsg = cls_Files_InfoDB_SQLITE.CheckDBFile(true);
                str_NewDBFileName = cls_Files_InfoDB_SQLITE.DBFileName;
            }
            else if (ToDatabaseType == cls_SQLBuilder.DATABASE_TYPE.ACCESS)
            {
                str_OutMsg = cls_Files_InfoDB_ACCESS.CheckDBFile(true);
                str_NewDBFileName = cls_Files_InfoDB_ACCESS.DBFileName;
            }
            else
            {
                str_OutMsg = "输入的数据库类型错误！";
                return false;
            }

            //处理创建数据库过程发生的错误
            if (!str_OutMsg.Contains(@"成功创建")) return false;

            str_OutMsg = String.Empty;
            //Step 2: 从当前数据库获取所有全局设置
            string[] arr_AllGlobalSettings = SelectAllGlobalSettings();
            if (arr_AllGlobalSettings != null)
            {
                foreach (string item in arr_AllGlobalSettings)
                {
                    if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
                    {
                        cls_Files_InfoDB_ACCESS.AddorUpdGlobalSetting(item.Split('|')[0], item.Split('|')[1], true);
                    }
                    else if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
                    {
                        cls_Files_InfoDB_SQLITE.AddorUpdGlobalSetting(item.Split('|')[0], item.Split('|')[1], true);
                    }
                }
            }
            else
            {
                str_OutMsg = "全局设置获取失败！";
                File.Delete(str_NewDBFileName);
                return false;
            }

            //Step 3: 从当前数据库获取所有配对信息和其所有目录和文件记录
            DataTable dt_Pairs = GetDirPairInfor(String.Empty);
            if (dt_Pairs != null)
            {
                foreach (DataRow dataRow in dt_Pairs.Rows)
                {
                    string str_PairID = String.Empty;
                    string str_PairName = dataRow.ItemArray[1].ToString();
                    string str_Dir1Path = dataRow.ItemArray[2].ToString();
                    string str_Dir2Path = dataRow.ItemArray[3].ToString();
                    string str_FilterRule = dataRow.ItemArray[6].ToString();
                    string str_AutoSyncInterval = dataRow.ItemArray[7].ToString();
                    string str_SyncDirection = dataRow.ItemArray[8].ToString();
                    int int_SyncDirection = 0;
                    if (!String.IsNullOrEmpty(str_SyncDirection)) int_SyncDirection = Convert.ToInt32(str_SyncDirection);
                    int int_AutoSyncInterval = 0;
                    if (!Int32.TryParse(str_AutoSyncInterval, out int_AutoSyncInterval)) int_AutoSyncInterval = 0;
                    string str_IsPaused = dataRow.ItemArray[9].ToString();
                    bool bl_IsPaused = Boolean.Parse(str_IsPaused);

                    //添加配对信息至新数据库
                    if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
                    {
                        cls_Files_InfoDB_ACCESS.AddDirPair(str_PairName, str_Dir1Path, str_Dir2Path, str_FilterRule, str_AutoSyncInterval, str_SyncDirection);
                        DataTable dataTable = cls_Files_InfoDB_ACCESS.GetDirPairInfor(str_PairName);
                        str_PairID = dataTable.Rows[0][0].ToString();
                        if (bl_IsPaused)
                            cls_Files_InfoDB_ACCESS.PausePairAutoSync(str_PairID, out str_OutMsg);
                    }
                    else if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
                    {
                        cls_Files_InfoDB_SQLITE.AddDirPair(str_PairName, str_Dir1Path, str_Dir2Path, str_FilterRule, str_AutoSyncInterval, str_SyncDirection);
                        DataTable dataTable = cls_Files_InfoDB_SQLITE.GetDirPairInfor(str_PairName);
                        str_PairID = dataTable.Rows[0][0].ToString();
                        if (bl_IsPaused)
                            cls_Files_InfoDB_SQLITE.PausePairAutoSync(str_PairID, out str_OutMsg);
                    }

                    //从当前数据库获取配对的目录和文件记录
                    string str_Folder1_Name = new DirectoryInfo(str_Dir1Path).Name;
                    string str_Folder2_Name = new DirectoryInfo(str_Dir2Path).Name;
                    string str_Pair_Table1 = str_PairName + "_DIR1_" + str_Folder1_Name;
                    string str_Pair_Table2 = str_PairName + "_DIR2_" + str_Folder2_Name;
                    DataTable dt_PairRecord1 = GetFileInfor(str_Pair_Table1, out str_OutMsg);
                    DataTable dt_PairRecord2 = GetFileInfor(str_Pair_Table2, out str_OutMsg);

                    //添加配对的所有目录/和文件记录至新数据库
                    foreach (DataRow dr_item in dt_PairRecord1.Rows)
                    {
                        string str_FileName = dr_item["FileName"].ToString();
                        string str_FilePath = dr_item["FilePath"].ToString();
                        string str_FileSize = dr_item["FileSize"].ToString();
                        string str_FileMD5 = dr_item["FileMD5"].ToString();
                        string str_FileLastModDate = dr_item["FileLastModDate"].ToString();
                        DateTime dateTime;
                        if (!DateTime.TryParse(str_FileLastModDate, out dateTime)) dateTime = DateTime.MinValue;
                        if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS)) str_FileLastModDate = dateTime.ToString(cls_Files_InfoDB_ACCESS.DBDateTimeFormat);
                        else if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE)) str_FileLastModDate = dateTime.ToString(cls_Files_InfoDB_SQLITE.DBDateTimeFormat);

                        if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
                        {
                            cls_Files_InfoDB_ACCESS.AddFileInfor(str_Pair_Table1, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
                        }
                        else if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
                        {
                            cls_Files_InfoDB_SQLITE.AddFileInfor(str_Pair_Table1, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
                        }
                    }
                    foreach (DataRow dr_item in dt_PairRecord2.Rows)
                    {
                        string str_FileName = dr_item["FileName"].ToString();
                        string str_FilePath = dr_item["FilePath"].ToString();
                        string str_FileSize = dr_item["FileSize"].ToString();
                        string str_FileMD5 = dr_item["FileMD5"].ToString();
                        string str_FileLastModDate = dr_item["FileLastModDate"].ToString();
                        DateTime dateTime;
                        if (!DateTime.TryParse(str_FileLastModDate, out dateTime)) dateTime = DateTime.MinValue;
                        if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS)) str_FileLastModDate = dateTime.ToString(cls_Files_InfoDB_ACCESS.DBDateTimeFormat);
                        else if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE)) str_FileLastModDate = dateTime.ToString(cls_Files_InfoDB_SQLITE.DBDateTimeFormat);

                        if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
                        {
                            cls_Files_InfoDB_ACCESS.AddFileInfor(str_Pair_Table2, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
                        }
                        else if (ToDatabaseType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
                        {
                            cls_Files_InfoDB_SQLITE.AddFileInfor(str_Pair_Table2, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
                        }
                    }

                    if (!String.IsNullOrEmpty(str_OutMsg))
                    {
                        //File.Delete(str_NewDBFileName);
                        return false;
                    }
                }
            }
            else
            {
                str_OutMsg = "配对获取失败！";
                File.Delete(str_NewDBFileName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除当前数据库文件
        /// </summary>
        public static bool DeleteDBFile()
        {
            try
            {
                CloseConnection();
                Thread.Sleep(1000);
                if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
                {
                    if (File.Exists(cls_Files_InfoDB_ACCESS.DBFileName))
                        File.Delete(cls_Files_InfoDB_ACCESS.DBFileName);
                }
                else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
                {
                    if (File.Exists(cls_Files_InfoDB_SQLITE.DBFileName))
                        File.Delete(cls_Files_InfoDB_SQLITE.DBFileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Global_Settings Methods
        /// <summary>
        /// 删除所有现有的设置
        /// </summary>
        public static void DelAllGlobalSettings()
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                cls_Files_InfoDB_ACCESS.DelAllGlobalSettings();
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                cls_Files_InfoDB_SQLITE.DelAllGlobalSettings();
            }
        }

        /// <summary>
        /// 添加或者修改设置
        /// </summary>
        /// <param name="str_Name">设置名称</param>
        /// <param name="str_Value">设置值</param>
        /// <param name="bl_IsUnique">该设置是否为唯一，如果是，则修改，否则添加</param>
        /// <returns></returns>
        public static bool AddorUpdGlobalSetting(string str_Name, string str_Value, bool bl_IsUnique)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.AddorUpdGlobalSetting(str_Name, str_Value, bl_IsUnique);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.AddorUpdGlobalSetting(str_Name, str_Value, bl_IsUnique);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有全局设置
        /// </summary>
        /// <returns></returns>
        public static string[] SelectAllGlobalSettings()
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.SelectAllGlobalSettings();
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.SelectAllGlobalSettings();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有全局设置到字典类型（Dictionary）
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> SelectAllGlobalSettingsDic()
        {
            Dictionary<string, string> dic_Settings = new Dictionary<string, string>();
            string[] arr_Settings = SelectAllGlobalSettings();
            for (int i = 0; i < arr_Settings.Length; i++)
            {
                string str_Setting_Name = arr_Settings[i].Split('|')[0];
                string str_Setting_Value = arr_Settings[i].Split('|')[1];
                dic_Settings.Add(str_Setting_Name, str_Setting_Value);
            }

            return dic_Settings;
        }
        #endregion

        #region DIRPAIR Infor Methods
        public static DataTable GetDirPairInfor(string str_PairName)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.GetDirPairInfor(str_PairName);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.GetDirPairInfor(str_PairName);
            }
            else
            {
                return null;
            }
        }

        public static bool AddDirPair(string str_PairName, string str_Dir1_Path, string str_Dir2_Path, string str_FilterRule, string str_SyncInterval, string str_SyncDirection)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.AddDirPair(str_PairName, str_Dir1_Path, str_Dir2_Path, str_FilterRule, str_SyncInterval, str_SyncDirection);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.AddDirPair(str_PairName, str_Dir1_Path, str_Dir2_Path, str_FilterRule, str_SyncInterval, str_SyncDirection);
            }
            else
            {
                return false;
            }
        }

        public static bool DelDirPair(string str_PairName, string str_Dir1_Path, string str_Dir2_Path)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.DelDirPair(str_PairName, str_Dir1_Path, str_Dir2_Path);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.DelDirPair(str_PairName, str_Dir1_Path, str_Dir2_Path);
            }
            else
            {
                return false;
            }
        }

        public static bool UpdatePairSyncStatus(string str_PairID, string str_LastSyncDT, bool bl_SyncSuccessfulIndc, out string OutputMsg)
        {
            OutputMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.UpdatePairSyncStatus(str_PairID, str_LastSyncDT, bl_SyncSuccessfulIndc, out OutputMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.UpdatePairSyncStatus(str_PairID, str_LastSyncDT, bl_SyncSuccessfulIndc, out OutputMsg);
            }
            else
            {
                return false;
            }
        }

        public static bool UpdatePairInfor(string str_PairID, string str_FilterRule, string str_SyncInterval, string str_SyncDirection, out string OutputMsg)
        {
            OutputMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.UpdatePairInfor(str_PairID, str_FilterRule, str_SyncInterval, str_SyncDirection, out OutputMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.UpdatePairInfor(str_PairID, str_FilterRule, str_SyncInterval, str_SyncDirection, out OutputMsg);
            }
            else
            {
                return false;
            }
        }

        public static string[] CheckAutoSyncPair()
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.CheckAutoSyncPair();
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.CheckAutoSyncPair();
            }
            else
            {
                return null;
            }
        }

        public static void FixDirPairStatus(bool bl_IsUpdateDT = true)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                cls_Files_InfoDB_ACCESS.FixDirPairStatus(bl_IsUpdateDT);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                cls_Files_InfoDB_SQLITE.FixDirPairStatus(bl_IsUpdateDT);
            }
        }

        public static bool PausePairAutoSync(string str_PairName, out string OutputMsg)
        {
            OutputMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.PausePairAutoSync(str_PairName, out OutputMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.PausePairAutoSync(str_PairName, out OutputMsg);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region File Infor Methods
        public static DataTable GetFileInfor(string str_TableName, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.GetFileInfor(str_TableName, out str_OutMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.GetFileInfor(str_TableName, out str_OutMsg);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetFileDiff(string str_PairName, string str_Dir1, string str_Dir2, int int_SyncDirection, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.GetFileDiff(str_PairName, str_Dir1, str_Dir2, int_SyncDirection, out str_OutMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.GetFileDiff(str_PairName, str_Dir1, str_Dir2, int_SyncDirection, out str_OutMsg);
            }
            else
            {
                return null;
            }
        }

        public static bool AddFileInfor(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.AddFileInfor(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.AddFileInfor(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
            }
            else
            {
                return false;
            }
        }

        public static bool UpdFileInfor(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.UpdFileInfor(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.UpdFileInfor(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
            }
            else
            {
                return false;
            }
        }

        public static bool DelFileInforSoft(string str_TableName, string str_FileID, string str_PairID, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.DelFileInforSoft(str_TableName, str_FileID, str_PairID, out str_OutMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.DelFileInforSoft(str_TableName, str_FileID, str_PairID, out str_OutMsg);
            }
            else
            {
                return false;
            }
        }

        public static bool DelFileInforAllHard(string str_TableName, string str_PairID)
        {
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.DelFileInforAllHard(str_TableName, str_PairID);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.DelFileInforAllHard(str_TableName, str_PairID);
            }
            else
            {
                return false;
            }
        }

        public static bool CheckFileInDB(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileLastModDate, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.ACCESS))
            {
                return cls_Files_InfoDB_ACCESS.CheckFileInDB(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileLastModDate, out str_OutMsg);
            }
            else if (DBType.Equals(cls_SQLBuilder.DATABASE_TYPE.SQLITE))
            {
                return cls_Files_InfoDB_SQLITE.CheckFileInDB(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileLastModDate, out str_OutMsg);
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
