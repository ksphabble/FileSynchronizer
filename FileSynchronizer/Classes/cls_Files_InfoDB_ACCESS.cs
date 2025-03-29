using ADOX;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace FileSynchronizer
{
    /// <summary>
    /// ACCESS的数据库静态类
    /// </summary>
    public static class cls_Files_InfoDB_ACCESS
    {
        #region Variables
        private static string m_DBFileName = Path.Combine(Application.StartupPath, @"FileSynchronizer_DB.mdb");
        private static string DBlocation_x86 = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + m_DBFileName;
        private static string DBlocation_x64 = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + m_DBFileName;
        private static OleDbConnection dbconn_ACCESS; //数据库连接
        public const string DBDateTimeFormat = @"yyyy/MM/dd HH:mm:ss";
        public static string DBFileName { get => m_DBFileName; }
        #endregion

        #region Basic DB Methods
        public static void OpenConnection()
        {
            if (dbconn_ACCESS == null)
            {
                dbconn_ACCESS = new OleDbConnection(GetConnStrFromCPU());
            }
            if (dbconn_ACCESS.State != ConnectionState.Open)
            {
                dbconn_ACCESS.Open();
            }
        }

        public static void CloseConnection()
        {
            dbconn_ACCESS.Close();
        }

        public static string CheckDBFile(bool bl_IsForceRecreateDB)
        {
            if (bl_IsForceRecreateDB && File.Exists(m_DBFileName))
            {
                File.Delete(m_DBFileName);
            }

            //string str_DB_File_Path = Path.Combine(Application.StartupPath, DBFileName);
            if (!File.Exists(m_DBFileName))
            {
                new Catalog().Create(GetConnStrFromCPU());
                OpenConnection();
                string str_buildResult = BuildGlobalSettingTable();

                if (!String.IsNullOrEmpty(str_buildResult))
                {
                    return str_buildResult;
                }
                else
                {
                    str_buildResult = BuildDirPairTable();
                    if (!String.IsNullOrEmpty(str_buildResult))
                    {
                        return str_buildResult;
                    }
                    else
                    {
                        return "已成功创建ACCESS数据库！";
                    }
                }
            }
            else
            {
                return String.Empty;
            }
        }

        private static string BuildFileTable(string str_TableName)
        {
            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_create = _SQLBuilder.SQL_BuildFileTableCre(str_TableName);
            string sql_SetIDX = _SQLBuilder.SQL_BuildFileTableIdx(str_TableName);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_create, dbconn_ACCESS);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            try
            {
                OleDbCommand cmd2 = new OleDbCommand(sql_SetIDX, dbconn_ACCESS);
                cmd2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return String.Empty;
        }

        private static string DeleteFileTable(string str_TableName)
        {
            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_create = _SQLBuilder.SQL_DeleteFileTable(str_TableName);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_create, dbconn_ACCESS);
                cmd.ExecuteNonQuery();
                return String.Empty;
            }
            catch (Exception ex)
            {
                //dbconn.Close();
                return ex.Message;
            }
        }

        private static string BuildDirPairTable()
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_create = _SQLBuilder.SQL_BuildDirPairTableCre();
            string sql_SetIDX = _SQLBuilder.SQL_BuildDirPairTableIdx();

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_create, dbconn_ACCESS);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            try
            {
                OleDbCommand cmd2 = new OleDbCommand(sql_SetIDX, dbconn_ACCESS);
                cmd2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            return String.Empty;
        }

        public static string BuildGlobalSettingTable()
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_createtable = _SQLBuilder.SQL_BuildGlobalSettingTable();

            try
            {
                OleDbCommand cmd_updDB = new OleDbCommand(sql_createtable, dbconn_ACCESS);
                cmd_updDB.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return String.Empty;
        }

        /// <summary>
        /// 检查数据库版本升级
        /// </summary>
        /// <returns>升级后的最新版本</returns>
        public static string CheckDBUpgrade(string str_TargetVersion)
        {
            string str_LatestDBVer = str_TargetVersion;

            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_CheckDBUpgradeSel(str_TargetVersion);
            string sql_upd = _SQLBuilder.SQL_CheckDBUpgradeUpd(str_TargetVersion);
            //string sql_dbupgradeV10220111_b1 = @"";
            //string sql_dbupgradeV11220702_b1 = @"Alter TABLE DIRPAIR ADD COLUMN IsPaused BIT";
            string sql_dbupgradeV2101_b1 = _SQLBuilder.SQL_BuildSyncDetailTableCre();
            CloseConnection();
            OpenConnection();
            
            try
            {
                OleDbCommand cmd_enq = new OleDbCommand(sql_enq, dbconn_ACCESS);
                OleDbDataReader dr = cmd_enq.ExecuteReader(CommandBehavior.SingleRow);
                bool bl_hasRecord = dr.Read();

                if (bl_hasRecord)
                {
                    string str_CurrentDBVer = dr[0].ToString();
                    if (!str_CurrentDBVer.Equals(str_TargetVersion))
                    {
                        int i_idxUnderScroll = str_CurrentDBVer.IndexOf("_");
                        string str_CurrVer = str_CurrentDBVer.Substring(0, i_idxUnderScroll < 0 ? str_CurrentDBVer.Length : i_idxUnderScroll).Replace(".", "");
                        int int_CurrentVer = Int32.Parse(str_CurrVer);
                        if (int_CurrentVer < 1211)
                        {
                            //更新数据库至版本1.2.1.1
                            //OleDbCommand cmd_updDB_302202111 = new OleDbCommand(sql_dbupgradeV30220211_b1, dbconn);
                            //cmd_updDB_302202111.ExecuteNonQuery();
                        }

                        if (int_CurrentVer < 2101)
                        {
                            //更新数据库至版本2.0.4.1
                            OleDbCommand cmd_dbupgradeV2101_b1 = new OleDbCommand(sql_dbupgradeV2101_b1, dbconn_ACCESS);
                            cmd_dbupgradeV2101_b1.ExecuteNonQuery();
                        }

                        //更新数据库版本至最新
                        OleDbCommand cmd_upd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                        cmd_upd.ExecuteNonQuery();
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                else
                {
                    str_LatestDBVer = String.Empty;
                }

                return str_LatestDBVer;
            }
            catch (Exception ex)
            {
                return "[" + ex.Message + "]";
            }
        }

        private static string GetConnStrFromCPU()
        {
            ConnectionOptions oConn = new ConnectionOptions();
            ManagementScope oMs = new ManagementScope(@"\\localhost", oConn);
            ObjectQuery oQuery = new ObjectQuery(@"select AddressWidth from Win32_Processor");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oReturnCollection = oSearcher.Get();
            string addressWidth = null;

            foreach (ManagementObject oReturn in oReturnCollection)
            {
                addressWidth = oReturn["AddressWidth"].ToString();
            }

            if (addressWidth.Equals("64"))
            {
                return DBlocation_x64;
            }
            else
            {
                return DBlocation_x86;
            }
        }

        /// <summary>
        /// 查询所有数据表名
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllTableName()
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.SQLITE);
            string sql_enq = _SQLBuilder.SQL_GetAllTableName();

            List<string> list_Output = new List<string> { };

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataReader dr = cmd.ExecuteReader();
                bool bl_HasNextRecord = true;

                while (bl_HasNextRecord)
                {
                    bl_HasNextRecord = dr.Read();
                    if (bl_HasNextRecord)
                    {
                        list_Output.Add(dr[0].ToString());
                    }
                }
            }
            catch
            {
                return null;
            }

            return list_Output.ToArray();
        }

        /// <summary>
        /// 执行一条查询SQL语句
        /// </summary>
        /// <param name="str_SQL"></param>
        /// <param name="SQLError"></param>
        /// <returns></returns>
        public static DataTable SQLEnquiry(string str_SQL, out string SQLError)
        {
            SQLError = String.Empty;
            try
            {
                OleDbCommand cmd = new OleDbCommand(str_SQL, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable("NewEnq");
                dataAdapter.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                SQLError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 执行一条非查询SQL语句
        /// </summary>
        /// <param name="str_SQL"></param>
        /// <param name="SQLError"></param>
        /// <returns></returns>
        public static int SQLExecutor(string str_SQL, out string SQLError)
        {
            SQLError = String.Empty;
            try
            {
                OleDbCommand cmd = new OleDbCommand(str_SQL, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row;
            }
            catch (Exception ex)
            {
                SQLError = ex.Message;
                return -1;
            }
        }
        #endregion

        #region Global_Settings Methods
        /// <summary>
        /// 删除所有现有的设置
        /// </summary>
        public static void DelAllGlobalSettings()
        {
            //dbconn.Open();
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_DelAllGlobalSettings();

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                //dbconn.Close();
            }
            catch
            {
                //dbconn.Close();
                return;
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
            if (String.IsNullOrEmpty(str_Name) || String.IsNullOrEmpty(str_Value)) return false;

            //dbconn.Open();
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_AddorUpdGlobalSettingSel(str_Name, str_Value);
            string sql_ins = _SQLBuilder.SQL_AddorUpdGlobalSettingIns(str_Name, str_Value);
            string sql_upd = _SQLBuilder.SQL_AddorUpdGlobalSettingUpd(str_Name, str_Value);

            try
            {
                OleDbCommand cmd_enq = new OleDbCommand(sql_enq, dbconn_ACCESS);
                OleDbDataReader dr = cmd_enq.ExecuteReader(CommandBehavior.SingleRow);
                bool bl_hasRecord = dr.Read();

                if (bl_IsUnique && bl_hasRecord)
                {
                    OleDbCommand cmd_upd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                    cmd_upd.ExecuteNonQuery();
                }
                else
                {
                    OleDbCommand cmd_ins = new OleDbCommand(sql_ins, dbconn_ACCESS);
                    cmd_ins.ExecuteReader();
                }

                //dbconn.Close();
                return true;
            }
            catch (Exception)
            {
                //dbconn.Close();
                return false;
            }
        }

        /// <summary>
        /// 获取所有全局设置
        /// </summary>
        /// <returns></returns>
        public static string[] SelectAllGlobalSettings()
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_SelectAllGlobalSettings();

            List<string> list_Output = new List<string> { };

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataReader dr = cmd.ExecuteReader();
                bool bl_HasNextRecord = true;

                while (bl_HasNextRecord)
                {
                    bl_HasNextRecord = dr.Read();
                    if (bl_HasNextRecord)
                    {
                        list_Output.Add(dr[0].ToString() + "|" + (String.IsNullOrEmpty(dr[1].ToString()) ? String.Empty : dr[1].ToString()));
                    }
                }
            }
            catch
            {
                return null;
            }

            return list_Output.ToArray();
        }
        #endregion

        #region DIRPAIR Infor Methods
        public static DataTable GetDirPairInfor(string str_PairName)
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_GetDirPairInfor(str_PairName);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable("DIRPAIR");
                dataAdapter.Fill(dt);

                return dt;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool AddDirPair(string str_PairName, string str_Dir1_Path, string str_Dir2_Path, string str_FilterRule, string str_SyncInterval, string str_SyncDirection)
        {
            if (String.IsNullOrEmpty(str_PairName) || String.IsNullOrEmpty(str_Dir1_Path) || String.IsNullOrEmpty(str_Dir2_Path))
            {
                return false;
            }

            string str_Folder1_Name = new DirectoryInfo(str_Dir1_Path).Name;
            string str_Folder2_Name = new DirectoryInfo(str_Dir2_Path).Name;
            string str_Pair_Table1 = str_PairName + "_DIR1_" + str_Folder1_Name;
            string str_Pair_Table2 = str_PairName + "_DIR2_" + str_Folder2_Name;

            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_AddDirPairEnq(str_PairName, str_Dir1_Path, str_Dir2_Path, str_FilterRule, str_SyncInterval, str_SyncDirection);
            string sql_ins = _SQLBuilder.SQL_AddDirPairIns(str_PairName, str_Dir1_Path, str_Dir2_Path, str_FilterRule, str_SyncInterval, str_SyncDirection);

            try
            {
                OleDbCommand cmd_enq = new OleDbCommand(sql_enq, dbconn_ACCESS);
                OleDbDataReader dr = cmd_enq.ExecuteReader(CommandBehavior.SingleRow);
                bool bl_hasRecord = dr.Read();

                if (bl_hasRecord)
                {
                    return false;
                }
                else
                {
                    OleDbCommand cmd_ins = new OleDbCommand(sql_ins, dbconn_ACCESS);
                    cmd_ins.ExecuteNonQuery();
                    string str_table1build = BuildFileTable(str_Pair_Table1);
                    string str_table2build = BuildFileTable(str_Pair_Table2);
                    if (String.IsNullOrEmpty(str_table1build) && String.IsNullOrEmpty(str_table2build))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool DelDirPair(string str_PairName, string str_Dir1_Path, string str_Dir2_Path)
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_DelDirPair(str_PairName, str_Dir1_Path, str_Dir2_Path);

            string str_Folder1_Name = (new DirectoryInfo(str_Dir1_Path)).Name;
            string str_Folder2_Name = (new DirectoryInfo(str_Dir2_Path)).Name;
            string str_Pair_Name1 = str_PairName + "_DIR1_" + str_Folder1_Name;
            string str_Pair_Name2 = str_PairName + "_DIR2_" + str_Folder2_Name;

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                string str_table1build = DeleteFileTable(str_Pair_Name1);
                string str_table2build = DeleteFileTable(str_Pair_Name2);
                if (String.IsNullOrEmpty(str_table1build) && String.IsNullOrEmpty(str_table2build) && row > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdatePairSyncStatus(string str_PairID, string str_LastSyncDT, bool bl_SyncSuccessfulIndc, out string OutputMsg)
        {
            OutputMsg = String.Empty;
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_UpdatePairSyncStatus(str_PairID, str_LastSyncDT, bl_SyncSuccessfulIndc);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                OutputMsg = ex.Message;
                return false;
            }
        }

        public static bool UpdatePairInfor(string str_PairID, string str_FilterRule, string str_SyncInterval, string str_SyncDirection, out string OutputMsg)
        {
            OutputMsg = String.Empty;
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_UpdatePairInfor(str_PairID, str_FilterRule, str_SyncInterval, str_SyncDirection);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                OutputMsg = ex.Message;
                return false;
            }
        }

        public static string[] CheckAutoSyncPair()
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_CheckAutoSyncPair();

            List<string> list_Output = new List<string> { };

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataReader dr = cmd.ExecuteReader();
                bool bl_HasNextRecord = true;

                while (bl_HasNextRecord)
                {
                    bl_HasNextRecord = dr.Read();
                    if (bl_HasNextRecord)
                    {
                        list_Output.Add(dr[0].ToString() + "|" + dr[1].ToString());
                    }
                }
            }
            catch
            {
                return null;
            }

            return list_Output.ToArray();
        }

        public static void FixDirPairStatus(bool bl_IsUpdateDT = true)
        {
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_FixDirPairStatus(bl_IsUpdateDT);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return;
            }
        }

        public static bool PausePairAutoSync(string str_PairID, out string OutputMsg)
        {
            OutputMsg = String.Empty;
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_PausePairAutoSync(str_PairID);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                OutputMsg = ex.Message;
                return false;
            }
        }
        #endregion

        #region File Infor Methods
        public static DataTable GetFileInfor(string str_TableName, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_GetFileInfor(str_TableName);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable(str_TableName);
                dataAdapter.Fill(dt);
                cmd.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                str_OutMsg = ex.Message;
                return null;
            }
        }

        public static DataTable GetFileDiff(string str_PairName, string str_Dir1, string str_Dir2, int int_SyncDirection, out string str_OutMsg)
        {
            DirectoryInfo _dir1 = new DirectoryInfo(str_Dir1);
            string str_Dir1TableName = "[" + str_PairName + "_DIR1_" + _dir1.Name + "]";
            DirectoryInfo _dir2 = new DirectoryInfo(str_Dir2);
            string str_Dir2TableName = "[" + str_PairName + "_DIR2_" + _dir2.Name + "]";

            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_GetFileDiff(str_PairName, str_Dir1TableName, str_Dir2TableName, int_SyncDirection);
            str_OutMsg = String.Empty;

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable("FILEDIFF");
                dataAdapter.Fill(dt);
                cmd.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                str_OutMsg = ex.Message;
                return null;
            }
        }

        public static bool AddFileInfor(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (String.IsNullOrEmpty(str_TableName) || String.IsNullOrEmpty(str_FileName) || String.IsNullOrEmpty(str_FilePath) || String.IsNullOrEmpty(str_PairID))
            {
                return false;
            }

            str_FileName = str_FileName.Replace("'", "''");
            str_FilePath = str_FilePath.Replace("'", "''");

            string str_SourceTableName = str_TableName;
            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_AddFileInforEnq(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID);
            string sql_ins = _SQLBuilder.SQL_AddFileInforIns(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID);
            bool bl_Result = false;
            int int_RetryCount = 0;
            bool bl_HasRetry = false;

            do
            {
                try
                {
                    OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                    cmd.CommandTimeout = 600;
                    OleDbDataReader dr = cmd.ExecuteReader();
                    bool bl_HasNextRecord = dr.Read();

                    if (bl_HasNextRecord)
                    {
                        bl_Result = UpdFileInfor(str_SourceTableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutMsg);
                    }
                    else
                    {
                        OleDbCommand cmd_ins = new OleDbCommand(sql_ins, dbconn_ACCESS);
                        bl_Result = cmd_ins.ExecuteNonQuery() > 0;
                        cmd_ins.Dispose();
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (!(ex.Message.Contains("不能再打开其它表") || ex.Message.Contains("Can't open any more tables")))
                    {
                        str_OutMsg = ex.Message;
                        bl_Result = false;
                        break;
                    }
                    else
                    {
                        CloseConnection();
                        Thread.Sleep(100);
                        OpenConnection();
                        int_RetryCount++;
                        bl_HasRetry = true;
                    }
                }
            } while (true);

            if (bl_HasRetry)
            {
                str_OutMsg = str_OutMsg + "SQL(" + sql_ins + ") got table lock exception, had retried after 200ms x " + int_RetryCount.ToString() + " times, result is " + bl_Result.ToString() + "\n";
            }

            return bl_Result;
        }

        public static bool UpdFileInfor(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (String.IsNullOrEmpty(str_TableName) || String.IsNullOrEmpty(str_FileName) || String.IsNullOrEmpty(str_FilePath) || String.IsNullOrEmpty(str_PairID))
            {
                return false;
            }

            str_FileName = str_FileName.Replace("'", "''");
            str_FilePath = str_FilePath.Replace("'", "''");

            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_UpdFileInfor(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID);
            bool bl_Result = false;
            int int_RetryCount = 0;
            bool bl_HasRetry = false;

            do
            {
                try
                {
                    OleDbCommand cmd_upd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                    bl_Result = cmd_upd.ExecuteNonQuery() > 0;
                    cmd_upd.Dispose();
                    break;
                }
                catch (Exception ex)
                {
                    if (!(ex.Message.Contains("不能再打开其它表") || ex.Message.Contains("Can't open any more tables")))
                    {
                        str_OutMsg = ex.Message;
                        bl_Result = false;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(200);
                        int_RetryCount++;
                        bl_HasRetry = true;
                    }
                }
            } while (true);

            if (bl_HasRetry)
            {
                str_OutMsg = str_OutMsg + "SQL(" + sql_upd + ") got table lock exception, had retried after 200ms x " + int_RetryCount.ToString() + "times, result is " + bl_Result.ToString() + "\n";
            }

            return bl_Result;
        }

        public static bool DelFileInforSoft(string str_TableName, string str_FileID, string str_PairID, out string OutMsg)
        {
            OutMsg = String.Empty;
            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_DelFileInforSoft(str_TableName, str_FileID, str_PairID);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                OutMsg = ex.Message;
                return false;
            }
        }

        public static bool DelFileInforAllHard(string str_TableName, string str_PairID)
        {
            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_DelFileInforAllHard(str_TableName, str_PairID);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckFileInDB(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileLastModDate, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;

            str_FileName = str_FileName.Replace("'", "''");
            str_FilePath = str_FilePath.Replace("'", "''");

            str_TableName = "[" + str_TableName + "]";
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_CheckFileInDB(str_TableName, str_FileName, str_FilePath, str_FileSize, str_FileLastModDate);
            bool bl_Result = false;
            int int_RetryCount = 0;
            bool bl_HasRetry = false;

            do
            {
                try
                {
                    OleDbCommand cmd_enq = new OleDbCommand(sql_enq, dbconn_ACCESS);
                    OleDbDataReader dr = cmd_enq.ExecuteReader(CommandBehavior.SingleRow);
                    bl_Result = dr.HasRows;
                    cmd_enq.Dispose();
                    break;
                }
                catch (Exception ex)
                {
                    if (!(ex.Message.Contains("不能再打开其它表") || ex.Message.Contains("Can't open any more tables")))
                    {
                        str_OutMsg = ex.Message;
                        bl_Result = false;
                        break;
                    }
                    else
                    {
                        //ReOpenConnection();
                        Thread.Sleep(200);
                        int_RetryCount++;
                        bl_HasRetry = true;
                    }
                }
            } while (true);

            if (bl_HasRetry)
            {
                str_OutMsg = str_OutMsg + "SQL(" + sql_enq + ") got table lock exception, had retried after 200ms x " + int_RetryCount.ToString() + "times, result is " + bl_Result.ToString() + "\n";
            }

            return bl_Result;
        }

        public static bool AddSyncDetail(string str_PairName, string str_FromFile, string str_ToFile, int int_FileDiffType, bool bl_SyncStatus, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (String.IsNullOrEmpty(str_PairName) || String.IsNullOrEmpty(str_FromFile) || String.IsNullOrEmpty(str_ToFile))
            {
                return false;
            }

            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_ins = _SQLBuilder.SQL_AddSyncDetailIns(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, bl_SyncStatus);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_ins, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                str_OutMsg = ex.Message;
                return false;
            }
        }

        public static bool UpdSyncDetail(string str_PairName, string str_FromFile, string str_ToFile, int int_FileDiffType, bool bl_SyncStatus, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            if (String.IsNullOrEmpty(str_PairName) || String.IsNullOrEmpty(str_FromFile) || String.IsNullOrEmpty(str_ToFile))
            {
                return false;
            }

            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_UpdSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, bl_SyncStatus);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                str_OutMsg = ex.Message;
                return false;
            }
        }

        public static bool CleanSyncDetailRecord(string str_PairName, bool bl_SyncStatus, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_upd = _SQLBuilder.SQL_CleanSyncDetailRecord(str_PairName, bl_SyncStatus);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_upd, dbconn_ACCESS);
                int row = cmd.ExecuteNonQuery();
                return row > 0;
            }
            catch (Exception ex)
            {
                str_OutMsg = ex.Message;
                return false;
            }
        }

        public static DataTable GetUnfinishedSyncDetail(string str_PairName, out string str_OutMsg)
        {
            str_OutMsg = String.Empty;
            cls_SQLBuilder _SQLBuilder = new cls_SQLBuilder(cls_SQLBuilder.DATABASE_TYPE.ACCESS);
            string sql_enq = _SQLBuilder.SQL_GetUnfinishedSyncDetail(str_PairName);

            try
            {
                OleDbCommand cmd = new OleDbCommand(sql_enq, dbconn_ACCESS);
                cmd.CommandTimeout = 600;
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable("SyncDetail");
                dataAdapter.Fill(dt);
                cmd.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                str_OutMsg = ex.Message;
                return null;
            }
        }
        #endregion
    }
}
