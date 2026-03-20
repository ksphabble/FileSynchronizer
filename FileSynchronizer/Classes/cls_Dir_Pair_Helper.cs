using Common.Components;
using D2Phap.FileWatcherEx;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static FileSynchronizer.Local_Utilities;

namespace FileSynchronizer
{
    /// <summary>
    /// cls_Dir_Pair_Helper类
    /// </summary>
    internal class cls_Dir_Pair_Helper
    {
        #region 变量和构造函数
        private string g_sPairID;
        private string g_sPairName;
        private string g_sDir1Path;
        private string g_sDir2Path;
        private string g_sDirLastSyncTime;
        private PairStatus g_psPairStatus;
        private string g_sFilterRule;
        private int g_iAutoSyncInterval;
        private int g_iSyncDirection;
        private bool g_bIsPaused;
        private cls_LogPairFile g_PairLogFile;
        //v3.0.0.1
        private bool g_bCancelRequested = false;
        private cls_Files_Info_Helper g_Files_Info;
        private bool g_ObjectsInforReady = false;
        private DateTime g_InitTime;

        public bool ObjectsInforReadyIndc { get => g_ObjectsInforReady; }

        public cls_Dir_Pair_Helper(string sPairID, string sPairName, string sDirPath1, string sDirPath2, string sLastSyncDT, PairStatus pPairStatus, string sFilterRule, int iAutoSyncInterval, int iSyncDirection, bool bIsPaused)
        {
            g_InitTime = DateTime.Now;
            g_sPairID = sPairID;
            g_sPairName = sPairName;
            g_sDir1Path = sDirPath1;
            g_sDir2Path = sDirPath2;
            g_sDirLastSyncTime = sLastSyncDT;
            g_psPairStatus = pPairStatus;
            g_sFilterRule = sFilterRule;
            g_iAutoSyncInterval = iAutoSyncInterval;
            g_iSyncDirection = iSyncDirection;
            g_bIsPaused = bIsPaused;
            g_PairLogFile = new cls_LogPairFile(sPairName, false);
            g_Files_Info = new cls_Files_Info_Helper(sPairID, sPairName, sDirPath1, sDirPath2, sFilterRule);
            g_Files_Info.ObjectsInforReady += G_Files_Info_ObjectsInforReady;
            g_Files_Info.Dir1ObjectChanged += G_Files_Info_Dir1ObjectChanged;
            g_Files_Info.Dir2ObjectChanged += G_Files_Info_Dir2ObjectChanged;
        }
        #endregion

        #region 分析和同步方法
        /// <summary>
        /// 分析文件夹配对
        /// </summary>
        /// <param name="IsAnalysisOnly"></param>
        public DataTable AnalysisDirPair(bool IsAnalysisOnly)
        {
            #region Pre-Validation
            string str_StartOprMessage = "开始分析配对（" + g_sPairName + "）" + (Global_Settings.DevelopMode ? " --- 程序处于开发者模式，会导致此操作不能全部完成，请注意！" : "");
            LogPairMessage(g_sPairName, str_StartOprMessage, true, true, 1);
            #endregion

            #region Define Varibles
            g_bCancelRequested = false; 
            OnPairStatusChange(PairStatus.ANALYSIS);
            string str_OutLogMsg = String.Empty;
            DateTime dt_DirLastSyncTime;
            bool bl_LastSyncStatus = DateTime.TryParse(g_sDirLastSyncTime, out dt_DirLastSyncTime);
            if (!bl_LastSyncStatus)
            {
                LogPairMessage(g_sPairName, "没有找到上次同步时间，首次分析同步需时较长，请耐心等待", true, true, 1);
                dt_DirLastSyncTime = DateTime.MinValue;
            }

            //DIR1的子目录和文件信息
            LogPairMessage(g_sPairName, "开始获取DIR1（" + g_sDir1Path + "）的目录和文件信息", true, true, 2);
            DirectoryInfo _dir1 = new DirectoryInfo(g_sDir1Path);
            //检查DIR1根目录是否存在，若不存在，则提示出错并停止分析
            if (bl_LastSyncStatus && !_dir1.Exists)
            {
                LogPairMessage(g_sPairName, "配对（" + g_sPairName + "）的目录1可能出现问题，请检查，若目录内容为空，请忽略此提示", true, true, 1);
                return null;
            }
            string str_Dir1TableName = g_sPairName + "_DIR1_" + _dir1.Name;
            DirectoryInfo[] subDir1 = FetchDirInformation(c_Dir1_Str);
            FileInfo[] fileInfos1 = FetchFileInformation(c_Dir1_Str);
            DataTable dt_File1InforDB = Files_InfoDB.GetFileInfor(str_Dir1TableName, out str_OutLogMsg);

            //DIR2的子目录和文件信息
            LogPairMessage(g_sPairName, "开始获取DIR2（" + g_sDir2Path + "）的目录和文件信息", true, true, 2);
            DirectoryInfo _dir2 = new DirectoryInfo(g_sDir2Path);
            //检查DIR2根目录是否存在，若不存在，则提示出错并停止分析
            if (bl_LastSyncStatus && !_dir2.Exists)
            {
                LogPairMessage(g_sPairName, "配对（" + g_sPairName + "）的目录2可能出现问题，请检查，若目录内容为空，请忽略此提示", true, true, 1);
                return null;
            }
            string str_Dir2TableName = g_sPairName + "_DIR2_" + _dir2.Name;
            DirectoryInfo[] subDir2 = FetchDirInformation(c_Dir2_Str);
            FileInfo[] fileInfos2 = FetchFileInformation(c_Dir2_Str);
            DataTable dt_File2InforDB = Files_InfoDB.GetFileInfor(str_Dir2TableName, out str_OutLogMsg);

            int int_TotalFileFound = subDir1.Length + fileInfos1.Length + subDir2.Length + fileInfos2.Length + dt_File1InforDB.Rows.Count + dt_File2InforDB.Rows.Count;
            string str_FileFullName = String.Empty;
            string str_FileName = String.Empty;
            string str_FilePath = String.Empty;
            string str_FileSize = String.Empty;
            string str_FileLastModDate = String.Empty;
            string str_FileCreDate = String.Empty;
            string str_DirCreDate = String.Empty;
            string str_FileMD5 = String.Empty;
            string str_FileID = String.Empty;
            string str_Where = String.Empty;
            bool bl_ExceptionFound = false;
            int i_SleepInterval = 20;
            #endregion

            #region 分析目录和文件至数据库
            //从目录1的子目录分析至数据库
            foreach (DirectoryInfo directoryInfo in subDir1)
            {
                if (g_bCancelRequested) break;
                if (!Directory.Exists(directoryInfo.FullName)) continue;

                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = directoryInfo.FullName;
                    str_FileName = c_DirNameChar_Str;
                    str_FilePath = directoryInfo.FullName;
                    str_FileSize = "0";
                    str_FileMD5 = String.Empty;
                    str_DirCreDate = directoryInfo.CreationTime.ToString(Files_InfoDB.DBDateTimeFormat);

                    if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);

                        if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                        {
                            string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                            LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                        }

                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_DirCreDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File1InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-ExistsInDB-ExcludeDIR:" + str_FileFullName;
                        LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-AddDIR:" + str_FilePath;
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    OnSetOngoingItem(str_LogMsgAddItem);
                    if (!Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-1-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(g_sPairName, "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(g_sPairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-1-Exception:" + ex.Message;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从目录1的文件分析至数据库
            foreach (FileInfo fileInfo in fileInfos1)
            {
                if (g_bCancelRequested) break;
                if (!File.Exists(fileInfo.FullName)) continue;

                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = fileInfo.FullName;
                    str_FileName = fileInfo.Name;
                    str_FilePath = fileInfo.DirectoryName;
                    str_FileSize = fileInfo.Length.ToString();
                    str_FileLastModDate = fileInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat);

                    if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);

                        if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                        {
                            string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                            LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                        }

                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_FileLastModDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File1InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-ExistsInDB-ExcludeFile:" + str_FileFullName;
                        LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-CalculatingFileMD5:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgCalcMD5, false, true, 4);
                    OnSetOngoingItem(str_LogMsgCalcMD5);
                    str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, IsAnalysisOnly, out str_OutLogMsg);
                    LogPairMessage(g_sPairName, "-" + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(g_sPairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "Step-2-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-AddFile:" + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    if (!Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-2-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(g_sPairName, "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(g_sPairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-2-ExceptionB:" + ex.Message;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从数据库1分析至文件和目录
            foreach (DataRow dataRow in dt_File1InforDB.Rows)
            {
                if (g_bCancelRequested) break;

                str_FileID = dataRow.ItemArray[0].ToString();
                str_FileName = dataRow.ItemArray[1].ToString();
                str_FilePath = dataRow.ItemArray[2].ToString();
                str_FileMD5 = dataRow.ItemArray[4].ToString();
                str_FileFullName = Path.Combine(str_FilePath, str_FileName);

                //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                if (!Directory.Exists(g_sDir1Path))
                {
                    bl_ExceptionFound = true;
                    break;
                }

                if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
                {
                    string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-" + str_OutLogMsg + "-SoftDeleteItem:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                    OnSetOngoingItem(str_LogMsgA);
                    if (!Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, g_sPairID, out str_OutLogMsg))
                    {
                        LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgA, true, true, 4);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        Thread.Sleep(i_SleepInterval);
                    }
                    OnAdd1Analysis(int_TotalFileFound);
                    continue;
                }

                if (str_FileName.Equals(c_DirNameChar_Str))
                {
                    //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(g_sDir1Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!Directory.Exists(str_FilePath))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-SoftDeleteDIR:" + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                            if (!Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, g_sPairID, out str_OutLogMsg))
                            {
                                LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-CheckDIR:" + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }
                else
                {
                    //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(g_sDir1Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!System.IO.File.Exists(str_FileFullName))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-SoftDeleteFile:" + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                            if (!Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, g_sPairID, out str_OutLogMsg))
                            {
                                LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-CheckFile:" + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }

                LogPairMessage(g_sPairName, "PAIR-ANALYSIS:" + g_sPairName + "-DIR1-Add 1 analysis done count", true, true, 4);
                OnAdd1Analysis(int_TotalFileFound);
            }
            LogPairMessage(g_sPairName, "配对（" + g_sPairName + "）的目录1（" + g_sDir1Path + "）分析完成", true, true, 2);

            //从目录2的子目录分析至数据库
            foreach (DirectoryInfo directoryInfo in subDir2)
            {
                if (g_bCancelRequested) break;
                if (!Directory.Exists(directoryInfo.FullName)) continue;

                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = directoryInfo.FullName;
                    str_FileName = c_DirNameChar_Str;
                    str_FilePath = directoryInfo.FullName;
                    str_FileSize = "0";
                    str_FileMD5 = String.Empty;
                    str_DirCreDate = directoryInfo.CreationTime.ToString(Files_InfoDB.DBDateTimeFormat);

                    if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);

                        if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                        {
                            string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                            LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                        }

                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_DirCreDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File2InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-ExistsInDB-ExcludeDIR:" + str_FileFullName;
                        LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-AddDIR:" + str_FilePath;
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    OnSetOngoingItem(str_LogMsgAddItem);
                    if (!Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-4-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(g_sPairName, "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(g_sPairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-4-Exception:" + ex.Message;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从目录2的文件分析至数据库
            foreach (FileInfo fileInfo in fileInfos2)
            {
                if (g_bCancelRequested) break;
                if (!File.Exists(fileInfo.FullName)) continue;

                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = fileInfo.FullName;
                    str_FileName = fileInfo.Name;
                    str_FilePath = fileInfo.DirectoryName;
                    str_FileSize = fileInfo.Length.ToString();
                    str_FileLastModDate = fileInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat);

                    if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);

                        if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                        {
                            string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                            LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                        }

                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_FileLastModDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File2InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-ExistsInDB-ExcludeFile:" + str_FileFullName;
                        LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-CalculatingFileMD5:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgCalcMD5, false, true, 4);
                    OnSetOngoingItem(str_LogMsgCalcMD5);
                    str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, IsAnalysisOnly, out str_OutLogMsg);
                    LogPairMessage(g_sPairName, "-" + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(g_sPairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "Step-5-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-AddFile:" + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    if (!Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-5-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(g_sPairName, "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(g_sPairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-5-ExceptionB:" + ex.Message;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从数据库2分析至文件和目录
            foreach (DataRow dataRow in dt_File2InforDB.Rows)
            {
                if (g_bCancelRequested) break;

                str_FileID = dataRow.ItemArray[0].ToString();
                str_FileName = dataRow.ItemArray[1].ToString();
                str_FilePath = dataRow.ItemArray[2].ToString();
                str_FileMD5 = dataRow.ItemArray[4].ToString();
                str_FileFullName = Path.Combine(str_FilePath, str_FileName);

                //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                if (!Directory.Exists(g_sDir2Path))
                {
                    bl_ExceptionFound = true;
                    break;
                }

                if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
                {
                    string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-" + str_OutLogMsg + "-SoftDeleteItem:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                    OnSetOngoingItem(str_LogMsgA);
                    if (!Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, g_sPairID, out str_OutLogMsg))
                    {
                        LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgA, true, true, 4);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        Thread.Sleep(i_SleepInterval);
                    }
                    OnAdd1Analysis(int_TotalFileFound);
                    continue;
                }

                if (str_FileName.Equals(c_DirNameChar_Str))
                {
                    //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(g_sDir2Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!Directory.Exists(str_FilePath))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-SoftDeleteDIR:" + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                            if (!Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, g_sPairID, out str_OutLogMsg))
                            {
                                LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-CheckDIR:" + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }
                else
                {
                    //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(g_sDir2Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!System.IO.File.Exists(str_FileFullName))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-SoftDeleteFile:" + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                            if (!Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, g_sPairID, out str_OutLogMsg))
                            {
                                LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-CheckFile:" + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }

                LogPairMessage(g_sPairName, "PAIR-ANALYSIS:" + g_sPairName + "-DIR2-Add 1 analysis done count", true, true, 4);
                OnAdd1Analysis(int_TotalFileFound);
            }
            LogPairMessage(g_sPairName, "配对（" + g_sPairName + "）的目录2（" + g_sDir2Path + "）分析完成", true, true, 2);
            #endregion

            #region 当没有出现错误的时候才继续，获取配对差异
            if (!bl_ExceptionFound)
            {
                if (!g_bCancelRequested)
                {
                    //重置当前操作
                    //PairPanal.SetOngoingItem();

                    #region 获取配对差异
                    //分析结果超过10000，意味着目录里面已经超过2500个项目，分析用时较长，故提醒
                    //v2.0.2.1 - 提示“配对相关的目录和文件数量较多，分析差异需时较长，请耐心等待”的阈值调整至SQLITE是100000，ACCESS是40000
                    int i_AlertThreshold = Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.SQLITE ? 100000 : 40000;
                    if (int_TotalFileFound > i_AlertThreshold)
                    {
                        LogPairMessage(g_sPairName, "配对相关的目录和文件数量较多，分析差异需时较长，请耐心等待", true, true, 1);
                    }
                    LogPairMessage(g_sPairName, "Started getting DIR/FILE difference", true, true, 4);
                    //DataTable dt_fileDiff = cls_Files_InfoDB.GetFileDiff(str_PairName, str_Dir1Path, str_Dir2Path, int_SyncDirection, out str_OutLogMsg);
                    DataTable dt_File1InforDB_AfterAnalysis = Files_InfoDB.GetFileInfor(str_Dir1TableName, out str_OutLogMsg);
                    if (!String.IsNullOrEmpty(str_OutLogMsg))
                    {
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 1);
                    }
                    DataTable dt_File2InforDB_AfterAnalysis = Files_InfoDB.GetFileInfor(str_Dir2TableName, out str_OutLogMsg);
                    if (!String.IsNullOrEmpty(str_OutLogMsg))
                    {
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 1);
                    }
                    DataTable dt_fileDiff = Get_File_Diff(g_sPairID, g_sPairName, dt_File1InforDB_AfterAnalysis, dt_File2InforDB_AfterAnalysis, g_sDir1Path, g_sDir2Path, g_iSyncDirection, out str_OutLogMsg);
                    if (!String.IsNullOrEmpty(str_OutLogMsg))
                    {
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 1);
                    }
                    LogPairMessage(g_sPairName, "Started getting DIR/FILE difference --- done", true, true, 4);

                    try
                    {
                        foreach (DataRow dataRow in dt_fileDiff.Rows)
                        {
                            str_FileName = dataRow.ItemArray[0].ToString();
                            string str_FileFromPath = dataRow.ItemArray[1].ToString();
                            string str_FileToPath = dataRow.ItemArray[2].ToString();
                            str_FileMD5 = dataRow.ItemArray[3].ToString();
                            str_FileLastModDate = dataRow.ItemArray[4].ToString();
                            str_FileSize = dataRow.ItemArray[5].ToString();
                            int int_FileDiffType = int.Parse(dataRow.ItemArray[6].ToString());
                            string str_FromFile = Path.Combine(str_FileFromPath, str_FileName);
                            string str_ToFile = Path.Combine(str_FileToPath, str_FileName);
                            string str_LogMsg = String.Empty;

                            if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FromFile, out str_OutLogMsg))
                            {
                                string str_LogMsgA = "PAIR-ANALYSIS:" + g_sPairName + "-GetDiff-" + str_OutLogMsg;
                                LogPairMessage(g_sPairName, str_LogMsgA, true, true, 3);
                                continue;
                            }

                            //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2
                            if (int_FileDiffType == 1)
                            {
                                if (str_FileName.Equals(c_DirNameChar_Str))
                                {
                                    str_LogMsg = "目录: " + str_FileFromPath + " -A-> " + str_FileToPath;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                                else
                                {
                                    str_LogMsg = "文件: " + str_FromFile + " -A-> " + str_ToFile;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                            }
                            //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1
                            if (int_FileDiffType == 2)
                            {
                                if (str_FileName.Equals(c_DirNameChar_Str))
                                {
                                    str_LogMsg = "目录: " + str_FileToPath + " <-A- " + str_FileFromPath;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                                else
                                {
                                    str_LogMsg = "文件: " + str_ToFile + " <-A- " + str_FromFile;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                            }
                            //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2
                            if (int_FileDiffType == 3)
                            {
                                //文件夹目录，跳过
                                if (str_FileName.Equals(c_DirNameChar_Str)) continue;

                                str_LogMsg = "文件: " + str_FromFile + " -U-> " + str_ToFile;
                                LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                            }
                            //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1
                            if (int_FileDiffType == 4)
                            {
                                //文件夹目录，跳过
                                if (str_FileName.Equals(c_DirNameChar_Str)) continue;

                                str_LogMsg = "文件: " + str_ToFile + " <-U- " + str_FromFile;
                                LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                            }
                            //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除
                            if (int_FileDiffType == 5)
                            {
                                if (str_FileName.Equals(c_DirNameChar_Str))
                                {
                                    str_LogMsg = "目录: " + str_FileFromPath + " -X-> " + str_FileToPath;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                                else
                                {
                                    str_LogMsg = "文件: " + str_FromFile + " -X-> " + str_ToFile;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                            }
                            //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除
                            if (int_FileDiffType == 6)
                            {
                                if (str_FileName.Equals(c_DirNameChar_Str))
                                {
                                    str_LogMsg = "目录: " + str_FileToPath + " <-X- " + str_FileFromPath;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                                else
                                {
                                    str_LogMsg = "文件: " + str_ToFile + " <-X- " + str_FromFile;
                                    LogPairMessage(g_sPairName, str_LogMsg, true, true, 1);
                                }
                            }
                            //可能出现文件冲突的项目，不做同步，仅发出提醒，DIFFTYPE=7
                            if (int_FileDiffType == 7)
                            {
                                str_LogMsg = "文件<" + str_FileName + ">，目录1<" + str_FileFromPath + ">，目录2<" + str_FileToPath + ">可能存在冲突，请检查";
                                LogPairMessage(g_sPairName, str_LogMsg, true, true, 2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogPairMessage(g_sPairName, "获取配对差异出错，操作终止，请查阅配对日志文件获取具体信息", true, true, 1);
                        LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                        return null;
                    }
                    #endregion

                    DataTable dt_fileDiffExType7 = Create_FileDiff_Empty();
                    if (dt_fileDiff.Rows.Count > 0)
                    {
                        LogPairMessage(g_sPairName, "去除差异等级为7的记录", true, true, 3);
                        DataRow[] dr_Temp = dt_fileDiff.Select("DIFFTYPE<>'7'");
                        if (dr_Temp.Length > 0)
                        {
                            dt_fileDiffExType7 = dr_Temp.CopyToDataTable();
                        }
                    }

                    if (!g_bCancelRequested)
                    {
                        string str_AnalysisResult = "配对（" + g_sPairName + "）分析完成，共发现" + int_TotalFileFound.ToString() + "条记录，";
                        if (dt_fileDiffExType7.Rows.Count.Equals(0))
                        {
                            str_AnalysisResult += "没有差异";
                        }
                        else
                        {
                            str_AnalysisResult += "其中" + dt_fileDiffExType7.Rows.Count.ToString() + "条记录需同步";
                        }
                        LogPairMessage(g_sPairName, str_AnalysisResult, true, true, 1);

                        OnPairStatusChange(PairStatus.FREE);
                        OnSetOngoingItem(string.Empty);
                        return dt_fileDiffExType7;
                    }
                    else
                    {
                        LogPairMessage(g_sPairName, "分析操作被中止", true, true, 1);
                        OnPairStatusChange(PairStatus.FREE);
                        OnSetOngoingItem(string.Empty);
                        return Create_FileDiff_Empty();
                    }
                }
                else
                {
                    LogPairMessage(g_sPairName, "分析操作被中止", true, true, 1);
                    OnPairStatusChange(PairStatus.FREE);
                    OnSetOngoingItem(string.Empty);
                    return Create_FileDiff_Empty();
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "分析过程发生异常被中止，可能是因为配对的目录断开连接，请确认后再试", true, true, 1);
                OnPairStatusChange(PairStatus.FREE);
                OnSetOngoingItem(string.Empty);
                return Create_FileDiff_Empty();
            }
            #endregion
        }

        /// <summary>
        /// 同步文件夹配对
        /// </summary>
        /// <param name="TableFileDiff"></param>
        public bool SyncDirPair(DataTable TableFileDiff, bool bRealTime = false)
        {
            #region Define Varibles
            string str_DebugModeWarning = Global_Settings.DevelopMode ? " --- 程序处于开发者模式，会导致同步操作无法完成，请注意！" : "";
            string str_StartOprMessage = (bRealTime ? "" : ("开始同步配对（" + g_sPairName + "）")) + str_DebugModeWarning;
            if (!String.IsNullOrEmpty(str_StartOprMessage))
            {
                LogPairMessage(g_sPairName, str_StartOprMessage, true, true, 1);
            }
            g_bCancelRequested = false; 
            OnPairStatusChange(PairStatus.SYNC);
            DirectoryInfo directoryInfo1 = new DirectoryInfo(g_sDir1Path);
            string str_Dir1TableName = g_sPairName + "_DIR1_" + directoryInfo1.Name;
            DirectoryInfo directoryInfo2 = new DirectoryInfo(g_sDir2Path);
            string str_Dir2TableName = g_sPairName + "_DIR2_" + directoryInfo2.Name;
            int int_TotalChngCount = TableFileDiff.Rows.Count;
            int int_SyncedCount = 0;
            string str_ExceptionFile = String.Empty;
            string str_SyncTimestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMdd_HHmmss");
            bool bExceptionFound = false;
            bool bHasUpdDelOnDir1 = false;
            bool bHasUpdDelOnDir2 = false;
            #endregion

            #region 开始同步
            foreach (DataRow dataRow in TableFileDiff.Rows)
            {
                if (g_bCancelRequested) break;

                try
                {
                    bool bl_SyncRecordDone = false;
                    int int_TrySyncCount = 0;
                    while (!bl_SyncRecordDone)
                    {
                        #region Define Varibles
                        int_TrySyncCount++;
                        string str_OngoingRecMsg = String.Empty;
                        string str_DatabaseErrorMsg = String.Empty;
                        string str_OutLogMsg = String.Empty;
                        string str_FileName = dataRow.ItemArray[0].ToString();
                        string str_FileFromPath = dataRow.ItemArray[1].ToString();
                        string str_FileToPath = dataRow.ItemArray[2].ToString();
                        string str_FileMD5 = dataRow.ItemArray[3].ToString();
                        string str_FileLastModDate = dataRow.ItemArray[4].ToString();
                        string str_FileSize = dataRow.ItemArray[5].ToString();
                        int int_FileDiffType = int.Parse(dataRow.ItemArray[6].ToString());
                        string str_FromFile = Path.Combine(str_FileFromPath, str_FileName);
                        string str_FromFileTemp = GetTempFileNameWithLocal(str_FromFile, out str_DatabaseErrorMsg);
                        string str_ToFile = Path.Combine(str_FileToPath, str_FileName);
                        string str_FileID = dataRow.ItemArray[7].ToString();
                        bool bl_IsDirectory = str_FileName.Equals(c_DirNameChar_Str);
                        str_ExceptionFile = "同步" + (bl_IsDirectory ? "目录" : "文件") + "：(" + str_FileName + ")从<" + str_FileFromPath + ">至<" + str_FileToPath + ">发生异常：";

                        //排除文件、目录
                        if (Local_Utilities.CheckFilterRule(g_sFilterRule, str_FromFile, out str_OutLogMsg))
                        {
                            int_SyncedCount++;
                            bl_SyncRecordDone = true;
                            FileHelper.xDelete(str_FromFileTemp);
                            string str_LogMsgA = "PAIR-SYNC:" + g_sPairName + "-" + str_OutLogMsg;
                            LogPairMessage(g_sPairName, str_LogMsgA, true, true, 4);
                            continue;
                        }
                        #endregion

                        #region Process Diff Records
                        //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 1)
                        {
                            //如果同步间隔为0分钟，则代表自动同步已经打开，并且不是从File Watcher调用的时候，需要把操作添加至正常同步序列
                            if (g_iAutoSyncInterval == 0 && !bRealTime)
                            {
                                string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir1Path, "");
                                Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                            }

                            //先判断目标目录是否存在，如果不存在则先创建（仅在非调试模式下生效）
                            if (!Directory.Exists(str_FileToPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(str_FileToPath);
                                directoryInfo.Create();
                                Files_InfoDB.AddFileInfor(str_Dir2TableName, c_DirNameChar_Str, str_FileToPath, "0", String.Empty, directoryInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat), g_sPairID, out str_DatabaseErrorMsg);
                            }

                            if (bl_IsDirectory)
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileFromPath + " -A-> " + str_FileToPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_FromFile + " -A-> " + str_ToFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                                if (int_TrySyncCount <= 1)
                                {
                                    if (!Files_InfoDB.AddSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                    }
                                }

                                if (System.IO.File.Exists(str_FromFileTemp) || System.IO.File.Exists(str_FromFile))
                                {

                                    if (System.IO.File.Exists(str_FromFileTemp))
                                    {
                                        bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                    }
                                    else
                                    {
                                        bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFile, str_ToFile, false, true);
                                    }
                                    if (bl_SyncRecordDone)
                                    {
                                        if (!Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_DatabaseErrorMsg))
                                        {
                                            LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 1);
                                        }
                                        else
                                        {
                                            if (!Files_InfoDB.UpdSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                            {
                                                LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    str_OngoingRecMsg = "文件" + (Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 2)
                        {
                            //如果同步间隔为0分钟，则代表自动同步已经打开，并且不是从File Watcher调用的时候，需要把操作添加至正常同步序列
                            if (g_iAutoSyncInterval == 0 && !bRealTime)
                            {
                                string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir2Path, "");
                                Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                            }

                            //先判断目标目录是否存在，如果不存在则先创建（仅在非调试模式下生效）
                            if (!Directory.Exists(str_FileToPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(str_FileToPath);
                                directoryInfo.Create();
                                Files_InfoDB.AddFileInfor(str_Dir1TableName, c_DirNameChar_Str, str_FileToPath, "0", String.Empty, directoryInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat), g_sPairID, out str_DatabaseErrorMsg);
                            }

                            if (bl_IsDirectory)
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileToPath + " <-A- " + str_FileFromPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_ToFile + " <-A- " + str_FromFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                                if (int_TrySyncCount <= 1)
                                {
                                    if (!Files_InfoDB.AddSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                    }
                                }

                                if (System.IO.File.Exists(str_FromFileTemp) || System.IO.File.Exists(str_FromFile))
                                {
                                    if (System.IO.File.Exists(str_FromFileTemp))
                                    {
                                        bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                    }
                                    else
                                    {
                                        bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFile, str_ToFile, false, true);
                                    }
                                    if (bl_SyncRecordDone)
                                    {
                                        if (!Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_DatabaseErrorMsg))
                                        {
                                            LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 1);
                                        }
                                        else
                                        {
                                            if (!Files_InfoDB.UpdSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                            {
                                                LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    str_OngoingRecMsg = "文件" + (Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 3)
                        {
                            //文件夹目录，跳过
                            if (bl_IsDirectory)
                            {
                                bl_SyncRecordDone = true;
                                continue;
                            }

                            //如果同步间隔为0分钟，则代表自动同步已经打开，并且不是从File Watcher调用的时候，需要把操作添加至正常同步序列
                            if (g_iAutoSyncInterval == 0 && !bRealTime)
                            {
                                string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir1Path, "");
                                Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                            }

                            str_OngoingRecMsg = "同步文件: " + str_FromFile + " -U-> " + str_ToFile;
                            OnSetOngoingItem(str_OngoingRecMsg);
                            LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                            if (int_TrySyncCount <= 1)
                            {
                                if (!Files_InfoDB.AddSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                }
                            }

                            if (System.IO.File.Exists(str_FromFileTemp) || System.IO.File.Exists(str_FromFile))
                            {
                                if (Global_Settings.DelToBackup)
                                {
                                    MoveFileToBackup(g_sDir2Path, str_ToFile, false, str_SyncTimestamp, bRealTime);
                                    bHasUpdDelOnDir2 = true;
                                }
                                if (System.IO.File.Exists(str_FromFileTemp))
                                {
                                    bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                }
                                else
                                {
                                    bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFile, str_ToFile, false, true);
                                }
                                if (bl_SyncRecordDone)
                                {
                                    if (!Files_InfoDB.UpdFileInfor(str_Dir2TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 1);
                                    }
                                    else
                                    {
                                        if (!Files_InfoDB.UpdSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                        {
                                            LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                str_OngoingRecMsg = "文件" + (Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                                bl_SyncRecordDone = true;
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 4)
                        {
                            //文件夹目录，跳过
                            if (bl_IsDirectory)
                            {
                                bl_SyncRecordDone = true;
                                continue;
                            }

                            //如果同步间隔为0分钟，则代表自动同步已经打开，并且不是从File Watcher调用的时候，需要把操作添加至正常同步序列
                            if (g_iAutoSyncInterval == 0 && !bRealTime)
                            {
                                string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir2Path, "");
                                Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                            }

                            str_OngoingRecMsg = "同步文件: " + str_ToFile + " <-U- " + str_FromFile;
                            OnSetOngoingItem(str_OngoingRecMsg);
                            LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);
                            if (int_TrySyncCount <= 1)
                            {
                                if (!Files_InfoDB.AddSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                }
                            }

                            if (System.IO.File.Exists(str_FromFileTemp) || System.IO.File.Exists(str_FromFile))
                            {
                                if (Global_Settings.DelToBackup)
                                {
                                    MoveFileToBackup(g_sDir1Path, str_ToFile, false, str_SyncTimestamp, bRealTime);
                                    bHasUpdDelOnDir1 = true;
                                }
                                if (System.IO.File.Exists(str_FromFileTemp))
                                {
                                    bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                }
                                else
                                {
                                    bl_SyncRecordDone = FileHelper.xCopyFile(str_FromFile, str_ToFile, false, true);
                                }
                                if (bl_SyncRecordDone)
                                {
                                    if (!Files_InfoDB.UpdFileInfor(str_Dir1TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 1);
                                    }
                                    else
                                    {
                                        if (!Files_InfoDB.UpdSyncDetail(g_sPairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                        {
                                            LogPairMessage(g_sPairName, str_DatabaseErrorMsg, true, true, 3);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                str_OngoingRecMsg = "文件" + (Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                                bl_SyncRecordDone = true;
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除
                        if (int_FileDiffType == 5)
                        {
                            if (bl_IsDirectory)
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileFromPath + " -X-> " + str_FileToPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                                if (Directory.Exists(str_FileToPath))
                                {
                                    if (Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(g_sDir2Path, str_FileToPath, true, str_SyncTimestamp, bRealTime);
                                        bHasUpdDelOnDir2 = true;
                                    }
                                    else
                                    {
                                        //如果同步间隔为0分钟，则代表自动同步已经打开，并且不是从File Watcher调用的时候，需要把操作添加至正常同步序列
                                        if (g_iAutoSyncInterval == 0 && !bRealTime)
                                        {
                                            string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir1Path, "");
                                            Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                                        }

                                        FileHelper.xDelete(str_FileToPath, true);
                                    }
                                }
                                if (!Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, g_sPairID, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 3);
                                }
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_FromFile + " -X-> " + str_ToFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                                if (System.IO.File.Exists(str_ToFile))
                                {
                                    if (Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(g_sDir2Path, str_ToFile, false, str_SyncTimestamp, bRealTime);
                                        bHasUpdDelOnDir2 = true;
                                    }
                                    else
                                    {
                                        //如果同步间隔为0分钟，则代表自动同步已经打开，需要把操作添加至正常同步序列
                                        if (g_iAutoSyncInterval == 0 && !bRealTime)
                                        {
                                            string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir1Path, "");
                                            Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                                        }

                                        FileHelper.xDelete(str_ToFile);
                                    }
                                }
                                if (!Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, g_sPairID, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 3);
                                }
                                bl_SyncRecordDone = true;
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除
                        if (int_FileDiffType == 6)
                        {
                            if (bl_IsDirectory)
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileToPath + " <-X- " + str_FileFromPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                                if (Directory.Exists(str_FileToPath))
                                {
                                    if (Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(g_sDir1Path, str_FileToPath, true, str_SyncTimestamp, bRealTime);
                                        bHasUpdDelOnDir1 = true;
                                    }
                                    else
                                    {
                                        //如果同步间隔为0分钟，则代表自动同步已经打开，并且不是从File Watcher调用的时候，需要把操作添加至正常同步序列
                                        if (g_iAutoSyncInterval == 0 && !bRealTime)
                                        {
                                            string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir2Path, "");
                                            Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                                        }

                                        FileHelper.xDelete(str_FileToPath, true);
                                    }
                                }
                                if (!Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, g_sPairID, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 3);
                                }
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_ToFile + " <-X- " + str_FromFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 3);

                                if (System.IO.File.Exists(str_ToFile))
                                {
                                    if (Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(g_sDir1Path, str_ToFile, false, str_SyncTimestamp, bRealTime);
                                        bHasUpdDelOnDir1 = true;
                                    }
                                    else
                                    {
                                        //如果同步间隔为0分钟，则代表自动同步已经打开，需要把操作添加至正常同步序列
                                        if (g_iAutoSyncInterval == 0 && !bRealTime)
                                        {
                                            string str_ObjectName = (bl_IsDirectory ? str_FileFromPath : str_FromFile).Replace(g_sDir2Path, "");
                                            Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                                        }

                                        FileHelper.xDelete(str_ToFile);
                                    }
                                }
                                if (!Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, g_sPairID, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg + "失败", true, true, 3);
                                }
                                bl_SyncRecordDone = true;
                            }
                        }
                        //可能出现文件冲突的项目，不做同步，仅发出提醒，DIFFTYPE=7
                        if (int_FileDiffType == 7)
                        {
                            bl_SyncRecordDone = true;
                        }
                        #endregion

                        #region Post Processing
                        //检查同步过程发生的错误消息
                        if (!String.IsNullOrEmpty(str_DatabaseErrorMsg))
                        {
                            bExceptionFound = true;
                            LogPairMessage(g_sPairName, str_ExceptionFile + str_DatabaseErrorMsg, true, true, 3, true);
                        }
                        //调试模式下强制同步成功
                        if (Global_Settings.DevelopMode)
                        {
                            bl_SyncRecordDone = true;
                        }
                        if (bl_SyncRecordDone)
                        {
                            //更新同步进度
                            OnAdd1Sync(int_TotalChngCount);
                            int_SyncedCount++;
                            Thread.Sleep(100);
                        }
                        else
                        {
                            if (int_TrySyncCount >= Global_Settings.RetryCountWhenSyncFailed)
                            {
                                LogPairMessage(g_sPairName, str_ExceptionFile + "超过最大重试次数", true, true, 1);
                                //更新同步进度
                                OnAdd1Sync(int_TotalChngCount);
                                int_SyncedCount++;
                                bl_SyncRecordDone = true;
                                bExceptionFound = true;
                            }
                            else
                            {
                                LogPairMessage(g_sPairName, str_ExceptionFile + "等待" + Global_Settings.RetryIntervalWhenSyncFailed.ToString() + "分钟后重试", true, true, 1);
                                Thread.Sleep(Global_Settings.RetryIntervalWhenSyncFailed * 60000);
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    bExceptionFound = true;
                    string str_PrintMsg = str_ExceptionFile + ex.Message;
                    LogPairMessage(g_sPairName, str_PrintMsg, true, true, 3, true);
                }
            }

            if (!g_bCancelRequested && (bHasUpdDelOnDir1 || bHasUpdDelOnDir2))
            {
                //彻底删除状态标记为'DL'的文件记录
                LogPairMessage(g_sPairName, "Hard Delete DIR/FILE records whose status is 'DL'", true, true, 4);
                Files_InfoDB.DelFileInforAllHard(str_Dir1TableName, g_sPairID);
                Files_InfoDB.DelFileInforAllHard(str_Dir2TableName, g_sPairID);

                //根据最大保留的backup数调整
                LogPairMessage(g_sPairName, "Delete backup DIR according to the Max. count of backup keep", true, true, 4);
                if (bHasUpdDelOnDir1)
                {
                    ClearBackupMaxKeep(g_sDir1Path, out str_ExceptionFile);
                    if (!String.IsNullOrEmpty(str_ExceptionFile))
                    {
                        bExceptionFound = true;
                        LogPairMessage(g_sPairName, str_ExceptionFile, true, true, 5, true);
                    }
                }
                if (bHasUpdDelOnDir2)
                {
                    ClearBackupMaxKeep(g_sDir2Path, out str_ExceptionFile);
                    if (!String.IsNullOrEmpty(str_ExceptionFile))
                    {
                        bExceptionFound = true;
                        LogPairMessage(g_sPairName, str_ExceptionFile, true, true, 5, true);
                    }
                }
            }
            #endregion

            #region 后期处理
            if (bExceptionFound)
            {
                LogPairMessage(g_sPairName, "同步过程发生了一些错误！！！检查日志文件", true, true, 1);
            }

            if (!g_bCancelRequested)
            {
                if (int_SyncedCount.Equals(int_TotalChngCount) && !bRealTime)
                {
                    LogPairMessage(g_sPairName, "配对（" + g_sPairName + "）同步完成，共同步了" + int_SyncedCount + "条记录", true, true, 1);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "同步操作被中止", true, true, 1);
            }

            OnPairStatusChange(PairStatus.FREE);
            OnSetOngoingItem(string.Empty);
            return !bExceptionFound;
            #endregion
        }

        private DataTable Create_FileDiff_Empty()
        {
            DataTable dt_FileDiff = new DataTable("FILEDIFF");
            dt_FileDiff.Columns.Add("FileName");
            dt_FileDiff.Columns.Add("FROMPATH");
            dt_FileDiff.Columns.Add("TOPATH");
            dt_FileDiff.Columns.Add("FileMD5");
            dt_FileDiff.Columns.Add("FileLastModDate");
            dt_FileDiff.Columns.Add("FileSize");
            dt_FileDiff.Columns.Add("DIFFTYPE");
            dt_FileDiff.Columns.Add("PK_FileID");
            return dt_FileDiff;
        }

        private DataTable Get_File_Diff(string str_PairID, string str_PairName, DataTable dt_File1InforDB, DataTable dt_File2InforDB, string str_Dir1Path, string str_Dir2Path, int int_SyncDirection, out string str_OutLogMsg)
        {
            DataTable dt_fileDiff = Create_FileDiff_Empty();
            str_OutLogMsg = String.Empty;
            List<string> lst_ComparedDB2Record = new List<string>();
            DirectoryInfo _dir1 = new DirectoryInfo(str_Dir1Path);
            string str_Dir1TableName = str_PairName + "_DIR1_" + _dir1.Name;
            DirectoryInfo _dir2 = new DirectoryInfo(str_Dir2Path);
            string str_Dir2TableName = str_PairName + "_DIR2_" + _dir2.Name;

            if (dt_File1InforDB != null && dt_File2InforDB != null)
            {
                //首先以Dir1的数据库记录为主
                foreach (DataRow dr_DB1 in dt_File1InforDB.Rows)
                {
                    string str_FileID1 = dr_DB1["PK_FileID"].ToString();
                    string str_FileName1 = dr_DB1["FileName"].ToString();
                    string str_FilePath1 = dr_DB1["FilePath"].ToString();
                    string str_FileSize1 = dr_DB1["FileSize"].ToString();
                    string str_FileMD51 = dr_DB1["FileMD5"].ToString();
                    string str_FileLastModDate1 = dr_DB1["FileLastModDate"].ToString();
                    string str_FileStatus1 = dr_DB1["FileStatus"].ToString();
                    string str_FullFilePath1 = Path.Combine(str_FilePath1, str_FileName1);

                    //从DIR2数据库中寻找此DIR1文件记录对应的记录
                    string str_ToPath1 = str_FilePath1.Replace(str_Dir1Path, str_Dir2Path);
                    DataRow[] dr_SrchFromDB2 = dt_File2InforDB.Select("FileName='" + str_FileName1.Replace("'", "''") + "' and FilePath='" + str_ToPath1.Replace("'", "''") + "'");
                    //如果DIR2数据库中找到匹配，则需要对比是否一样以及同步方向
                    if (dr_SrchFromDB2.Length > 0)
                    {
                        string str_FileID2 = dr_SrchFromDB2[0]["PK_FileID"].ToString();
                        string str_FileSize2 = dr_SrchFromDB2[0]["FileSize"].ToString();
                        string str_FileMD52 = dr_SrchFromDB2[0]["FileMD5"].ToString();
                        string str_FileLastModDate2 = dr_SrchFromDB2[0]["FileLastModDate"].ToString();
                        string str_FileStatus2 = dr_SrchFromDB2[0]["FileStatus"].ToString();
                        string str_FullFilePath2 = Path.Combine(str_ToPath1, str_FileName1);

                        //如果DIR1记录的MD5和DIR2记录的MD5相同，则看是否需要同步删除
                        if (str_FileMD51.Equals(str_FileMD52))
                        {
                            //DIR1中标记为删除，DIR2未被标记为删除的，DIFFTYPE=5，需要从DIR2中删除，同步方向0/2
                            if (str_FileStatus1.Equals("DL") && !str_FileStatus2.Equals("DL"))
                            {
                                if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(2))
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "5", str_FileID2);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //DIR2中标记为删除，DIR1未被标记为删除的，DIFFTYPE=6，需要从DIR1中删除，同步方向0/4
                            if (!str_FileStatus1.Equals("DL") && str_FileStatus2.Equals("DL"))
                            {
                                if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(4))
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_ToPath1, str_FilePath1, str_FileMD52, str_FileLastModDate2, str_FileSize2, "6", str_FileID1);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //DIR1和DIR2都状态相同且MD5相同，无需同步，但要记录下来供DIR2的数据库对比使用
                            if (str_FileStatus1.Equals(str_FileStatus2))
                            {
                                lst_ComparedDB2Record.Add(str_FullFilePath2);
                            }
                        }
                        //如果DIR1记录的MD5和DIR2记录的MD5不同，则看是否需要同步更新
                        else
                        {
                            DateTime dt_Date1 = DateTime.Parse(str_FileLastModDate1);
                            DateTime dt_Date2 = DateTime.Parse(str_FileLastModDate2);
                            //DIR1中文件的修改时间比DIR2的晚，DIFFTYPE=3，需要从DIR1同步至DIR2，同步方向0/1/2/3
                            if (str_FileStatus1.Equals("AC") && str_FileStatus2.Equals("AC") && dt_Date1 > dt_Date2)
                            {
                                if (int_SyncDirection <= 3)
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "3", str_FileID1);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //DIR2中文件的修改时间比DIR1的晚，DIFFTYPE=4，需要从DIR2同步至DIR1，同步方向0/1/4/5
                            if (str_FileStatus1.Equals("AC") && str_FileStatus2.Equals("AC") && dt_Date1 < dt_Date2)
                            {
                                if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(1) || int_SyncDirection.Equals(4) || int_SyncDirection.Equals(5))
                                {
                                    dt_fileDiff.Rows.Add(str_FileName1, str_ToPath1, str_FilePath1, str_FileMD52, str_FileLastModDate2, str_FileSize2, "4", str_FileID2);
                                    lst_ComparedDB2Record.Add(str_FullFilePath2);
                                }
                            }
                            //文件MD5不同，但是DIR1或者DIR2中被标记删除，则可能出现冲突，需要手动同步，DIFFTYPE=7
                            if (str_FileStatus1.Equals("DL") || str_FileStatus2.Equals("DL"))
                            {
                                dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "7", str_FileID1);
                                lst_ComparedDB2Record.Add(str_FullFilePath2);
                            }
                            //特殊情况：当MD5不同，但是修改时间和大小都一样的情况，说明其中一边的MD5有问题，需要校正
                            if (str_FileStatus1.Equals("AC") && str_FileStatus2.Equals("AC") && dt_Date1.Equals(dt_Date2) && str_FileSize1.Equals(str_FileSize2))
                            {
                                #region 暂时收回这段代码
                                //LogMessage("文件" + str_FileName1 + "的MD5码存在异常，需要校正", false, true, 1);
                                //LogMessage("开始校正MD5：文件1<" + str_FullFilePath1 + ">", false, true, 2);
                                //string str_TempMD51 = cls_FileHelper.MD5Encrypt(str_FullFilePath1, out str_OutLogMsg);
                                //LogMessage(":" + str_TempMD51, true, false, 2);
                                //LogMessage("开始校正MD5：文件2<" + str_FullFilePath1 + ">", false, true, 2);
                                //string str_TempMD52 = cls_FileHelper.MD5Encrypt(str_FullFilePath2, out str_OutLogMsg);
                                //LogMessage(":" + str_TempMD52, true, false, 2);

                                ////计算后DIR1的MD5不正确，则更新到DIR1的数据库
                                //if (!str_TempMD51.Equals(str_FileMD51))
                                //{
                                //    cls_Files_InfoDB.UpdFileInfor(str_Dir1TableName, str_FileName1, str_FilePath1, str_FileSize1, str_TempMD51, str_FileLastModDate1, str_PairID, out str_OutLogMsg);
                                //    string str_OutLogMsg1 = "校正文件<" + str_FileName1 + ">在DIR1数据库中的MD5";
                                //    LogMessage(str_OutLogMsg1, true, true, 1);
                                //}
                                ////计算后DIR2的MD5不正确，则更新到DIR2的数据库
                                //if (!str_TempMD52.Equals(str_FileMD52))
                                //{
                                //    cls_Files_InfoDB.UpdFileInfor(str_Dir2TableName, str_FileName1, str_ToPath1, str_FileSize2, str_TempMD52, str_FileLastModDate2, str_PairID, out str_OutLogMsg);
                                //    string str_OutLogMsg1 = "校正文件<" + str_FileName1 + ">在DIR2数据库中的MD5";
                                //    LogMessage(str_OutLogMsg1, true, true, 1);
                                //}

                                //校正后两个MD5确实不同，而修改时间和大小都一样的情况，则说明文件有冲突，DIFFTYPE=7
                                //if (!String.IsNullOrEmpty(str_TempMD51) && !String.IsNullOrEmpty(str_TempMD52) && !str_TempMD51.Equals(str_TempMD52))
                                //{
                                //    dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "7", str_FileID1);
                                //}
                                #endregion

                                dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "7", str_FileID1);
                                lst_ComparedDB2Record.Add(str_FullFilePath2);
                            }
                        }
                    }
                    //如果DIR2数据库中找不到匹配，DIFFTYPE=1，需要从DIR1同步至DIR2，同步方向0/1/2/3
                    else
                    {
                        //仅当DIR1的记录为'AC'的时候才需要添加至DIR2
                        if (str_FileStatus1.Equals("AC"))
                        {
                            if (int_SyncDirection <= 3)
                            {
                                dt_fileDiff.Rows.Add(str_FileName1, str_FilePath1, str_ToPath1, str_FileMD51, str_FileLastModDate1, str_FileSize1, "1", str_FileID1);
                            }
                        }
                    }
                }

                //以DIR2的数据库记录为主
                foreach (DataRow dr_DB2 in dt_File2InforDB.Rows)
                {
                    string str_FileID2 = dr_DB2["PK_FileID"].ToString();
                    string str_FileName2 = dr_DB2["FileName"].ToString();
                    string str_FilePath2 = dr_DB2["FilePath"].ToString();
                    string str_FileSize2 = dr_DB2["FileSize"].ToString();
                    string str_FileMD52 = dr_DB2["FileMD5"].ToString();
                    string str_FileLastModDate2 = dr_DB2["FileLastModDate"].ToString();
                    string str_FileStatus2 = dr_DB2["FileStatus"].ToString();
                    string str_TempPath = Path.Combine(str_FilePath2, str_FileName2);
                    //上面检查DIR1的时候已经检查过该文件，跳过
                    if (lst_ComparedDB2Record.Contains(str_TempPath)) continue;

                    //从DIR1数据库中寻找此DIR2文件记录对应的记录
                    string str_ToPath2 = str_FilePath2.Replace(str_Dir2Path, str_Dir1Path);
                    DataRow[] dr_SrchFromDB1 = dt_File2InforDB.Select("FileName='" + str_FileName2.Replace("'", "''") + "' and FilePath='" + str_ToPath2.Replace("'", "''") + "'");

                    //如果DIR1数据库中找不到匹配，DIFFTYPE=2，需要从DIR2同步至DIR1，同步方向0/1/4/5
                    if (dr_SrchFromDB1.Length.Equals(0))
                    {
                        //仅当DIR1的记录为'AC'的时候才需要添加至DIR2
                        if (str_FileStatus2.Equals("AC"))
                        {
                            if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(1) || int_SyncDirection.Equals(4) || int_SyncDirection.Equals(5))
                            {
                                dt_fileDiff.Rows.Add(str_FileName2, str_FilePath2, str_ToPath2, str_FileMD52, str_FileLastModDate2, str_FileSize2, "2", str_FileID2);
                            }
                        }
                    }
                }
            }

            return dt_fileDiff;
        }

        private string CalcFileMD5withLocal(string str_FileFullName, bool bl_IsForceSameFile, out string str_OutLogMsg)
        {
            str_OutLogMsg = String.Empty;

            try
            {
                string str_NewFileFullPath = bl_IsForceSameFile ? str_FileFullName : GetTempFileNameWithLocal(str_FileFullName, out str_OutLogMsg);

                if (!String.IsNullOrEmpty(str_NewFileFullPath))
                {
                    if (!String.Equals(str_FileFullName, str_NewFileFullPath))
                    {
                        if (!FileHelper.xCopyFile(str_FileFullName, str_NewFileFullPath, false, true))
                        {
                            return String.Empty;
                        }
                    }

                    return CommonFunctions.MD5Encrypt(str_NewFileFullPath, out str_OutLogMsg);
                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                str_OutLogMsg = ex.ToString();
                return String.Empty;
            }
        }

        private string GetTempFileNameWithLocal(string str_FileFullName, out string str_OutLogMsg)
        {
            str_OutLogMsg = String.Empty;
            string str_NewFileFullPath = str_FileFullName;

            if (Global_Settings.UseLocalTemp)
            {
                bool IsFileInLocal = NetHelper.IsLocalPath(str_FileFullName);
                //string str_PathRoot = Path.GetPathRoot(str_FileFullName);
                //DriveInfo driveInfo = new DriveInfo(str_PathRoot);
                //if (driveInfo.DriveType != DriveType.Fixed)
                if (!IsFileInLocal)
                {
                    string str_NewFileName = CommonFunctions.MD5EncryptString(str_NewFileFullPath, out str_OutLogMsg);
                    if (String.IsNullOrEmpty(str_NewFileName))
                    {
                        str_OutLogMsg = "MD5 Encrypt String failed for " + str_NewFileFullPath;
                        return String.Empty;
                    }
                    str_NewFileFullPath = Path.Combine(Global_Settings.LocalTempFolder, str_NewFileName);
                }
            }

            return str_NewFileFullPath;
        }

        private void MoveFileToBackup(string str_RootDirPath, string str_FileFullPath, bool bl_IsDirectory, string str_Timestamp, bool bRealTime)
        {
            string str_BackupFolder = Path.Combine(str_RootDirPath, c_FSBackup_Str);
            string str_BackupFolder_WithTime = Path.Combine(str_BackupFolder, str_Timestamp);
            string str_TargetFilePath = str_FileFullPath.Replace(str_RootDirPath, str_BackupFolder_WithTime);

            if (!Directory.Exists(str_BackupFolder))
            {
                DirectoryInfo _directoryInfo = new DirectoryInfo(str_BackupFolder);
                _directoryInfo.Create();
                _directoryInfo.Attributes = FileAttributes.Hidden;
            }

            //如果同步间隔为0分钟，则代表自动同步已经打开，需要把操作添加至正常同步序列
            if (g_iAutoSyncInterval == 0 && !bRealTime)
            {
                string str_ObjectName = str_FileFullPath.Replace(str_RootDirPath, "");
                if (str_RootDirPath.Equals(g_sDir2Path))
                {
                    Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                }
                else
                {
                    Sync_Queue_Helper.Normal_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                }
            }

            if (!bl_IsDirectory)
            {
                FileHelper.xCopyFile(str_FileFullPath, str_TargetFilePath, true, true);
            }
            else
            {
                FileHelper.xCopyDirectory(str_FileFullPath, str_TargetFilePath, true, true);
            }
        }

        private void ClearBackupMaxKeep(string str_RootDirPath, out string str_ErrorMsg)
        {
            str_ErrorMsg = String.Empty;
            string str_BackupFolder = Path.Combine(str_RootDirPath, c_FSBackup_Str);

            if (!Directory.Exists(str_BackupFolder)) return;

            try
            {
                DirectoryInfo[] directories = (new DirectoryInfo(str_BackupFolder).GetDirectories("*", SearchOption.TopDirectoryOnly)).OrderByDescending(f => f.Name).ToArray();
                for (int i = 0; i < directories.Length; i++)
                {
                    int i_CurrentIdx = i + 1;
                    if (i_CurrentIdx > Global_Settings.MaxKeepBackup)
                    {
                        FileHelper.xDelete(directories[i].FullName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                str_ErrorMsg = ex.Message;
            }
        }

        /// <summary>
        /// 更新最后同步时间
        /// </summary>
        /// <param name="strDateTime">最后同步时间</param>
        /// <param name="SyncSuccessfulIndc">是否成功标记</param>
        /// <returns>更新操作是否成功</returns>
        public bool UpdateLastSyncTime(string strDateTime, bool SyncSuccessfulIndc)
        {
            string str_OutLogMsg = String.Empty;
            //更新最后同步时间
            bool bRet = Files_InfoDB.UpdatePairSyncStatus(g_sPairID, strDateTime, SyncSuccessfulIndc, out str_OutLogMsg);
            if (!String.IsNullOrEmpty(str_OutLogMsg))
            {
                LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
            }
            return bRet;
        }

        /// <summary>
        /// 记录配对日志消息
        /// </summary>
        /// <param name="LogMessage">日志消息</param>
        /// <param name="IsChangeLine">是否换行</param>
        /// <param name="IsAddTS">是否添加时间戳</param>
        /// <param name="MsgTraceLevel">记录的日志消息等级</param>
        /// <param name="IsALwaysLogFile">是否强制写入日志文件（默认否）</param>
        public void LogPairMessage(string PairName, string LogMessage, bool IsChangeLine, bool IsAddTS, int MsgTraceLevel, bool IsALwaysLogFile = false)
        {
            string str_LogMsgToFile = (IsAddTS ? DateTime.Now.ToString(Files_InfoDB.DBDateTimeFormat) + " --- " : String.Empty) + LogMessage;
            //string str_LogMsgChngLine = str_LogMsgToFile + (IsChangeLine ? "\n" : "");
            bool bl_HasWrittenFile = IsALwaysLogFile;

            try
            {
                if (IsALwaysLogFile)
                {
                    g_PairLogFile.LogMessage(str_LogMsgToFile);
                    bl_HasWrittenFile = true;
                }

                //输入的MsgTraceLevel是0，则属于顶级日志，直接处理
                if (MsgTraceLevel == 0)
                {
                    //((ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0]).LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
                    OnLogPairMsg(LogMessage, IsChangeLine, IsAddTS);
                    if (!bl_HasWrittenFile)
                    {
                        g_PairLogFile.LogMessage(str_LogMsgToFile);
                    }
                }
                //输入的MsgTraceLevel > 0，则对比全局变量设置的日志等级做判断处理
                else if (MsgTraceLevel > 0 && MsgTraceLevel <= Global_Settings.TraceLevel)
                {
                    //((ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0]).LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
                    OnLogPairMsg(LogMessage, IsChangeLine, IsAddTS);
                    if (Global_Settings.LogMessageToFile && !bl_HasWrittenFile)
                    {
                        g_PairLogFile.LogMessage(str_LogMsgToFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                LogPairMessage(PairName, ex.Message, true, true, 5, true);
            }
        }

        /// <summary>
        /// 打开此配对的日志文件
        /// </summary>
        public void OpenPairLog()
        {
            g_PairLogFile.OpenPairLog();
        }

        /// <summary>
        /// 暂停此配对的自动同步
        /// </summary>
        /// <param name="WarningMsg"></param>
        /// <returns></returns>
        public bool PausePairAutoSync()
        {
            string WarningMsg = string.Empty;
            bool bRet = Files_InfoDB.PausePairAutoSync(g_sPairID, out WarningMsg);
            if (!bRet)
            {
                LogPairMessage(g_sPairName, WarningMsg, true, true, 5, true);
            }
            return bRet;
        }

        private int CalculateAnalysisInterval()
        {
            return 0;
        }

        /// <summary>
        /// 停止配对当前的操作
        /// </summary>
        public void CancelOperation()
        {
            g_bCancelRequested = true;
        }

        private DirectoryInfo[] FetchDirInformation(string str_DirString)
        {
            DirectoryInfo[] directoryInfos = null;
            if (str_DirString == c_Dir1_Str)
            {
                DirectoryInfo _dir1 = new DirectoryInfo(g_sDir1Path);
                directoryInfos = g_Files_Info.GetDirectoryInfos1();
                if (directoryInfos == null || directoryInfos.Length == 0)
                {
                    directoryInfos = _dir1.GetDirectories("*", SearchOption.AllDirectories);
                }
            }
            else if (str_DirString == c_Dir2_Str)
            {
                DirectoryInfo _dir2 = new DirectoryInfo(g_sDir2Path);
                directoryInfos = g_Files_Info.GetDirectoryInfos2();
                if (directoryInfos == null || directoryInfos.Length == 0)
                {
                    directoryInfos = _dir2.GetDirectories("*", SearchOption.AllDirectories);
                }
            }
            return directoryInfos;
        }

        private FileInfo[] FetchFileInformation(string str_DirString)
        {
            FileInfo[] fileInfos = null;
            if (str_DirString == c_Dir1_Str)
            {
                DirectoryInfo _dir1 = new DirectoryInfo(g_sDir1Path);
                fileInfos = g_Files_Info.GetFileInfos1();
                if (fileInfos == null || fileInfos.Length == 0)
                {
                    fileInfos = _dir1.GetFiles("*", SearchOption.AllDirectories);
                }
            }
            else if (str_DirString == c_Dir2_Str)
            {
                DirectoryInfo _dir2 = new DirectoryInfo(g_sDir2Path);
                fileInfos = g_Files_Info.GetFileInfos2();
                if (fileInfos == null || fileInfos.Length == 0)
                {
                    fileInfos = _dir2.GetFiles("*", SearchOption.AllDirectories);
                }
            }
            return fileInfos;
        }
        #endregion

        #region 类的事件处理
        public delegate void LogPairMessageHandler(object sender, string LogMessage, bool IsChangeLine, bool IsAddTS);
        public event LogPairMessageHandler LogPairMsg;
        public delegate void SetOngoingItemHandler(object sender, string OngoingItem);
        public event SetOngoingItemHandler SetOngoingItem;
        public delegate void Add1AnalysisHandler(object sender, int TotalPending);
        public event Add1AnalysisHandler Add1Analysis;
        public delegate void Add1SyncHandler(object sender, int TotalPending);
        public event Add1SyncHandler Add1Sync;
        public delegate void PairStatusChangeHandler(object sender, PairStatus pairStatus);
        public event PairStatusChangeHandler PairStatusChange;
        public delegate void ObjectsInforReadyHandler(object sender, string InitDoneMsg);
        public event ObjectsInforReadyHandler ObjectsInforReady;

        protected virtual void OnLogPairMsg(string LogMessage, bool IsChangeLine, bool IsAddTS)
        {
            if (LogPairMsg != null)
            {
                LogPairMsg(this, LogMessage, IsChangeLine, IsAddTS);
            }
        }

        protected virtual void OnSetOngoingItem(string OngoingItem)
        {
            if (SetOngoingItem != null)
            {
                SetOngoingItem(this, OngoingItem);
            }
        }

        protected virtual void OnAdd1Analysis(int TotalPending)
        {
            if (Add1Analysis != null)
            {
                Add1Analysis(this, TotalPending);
            }
        }

        protected virtual void OnAdd1Sync(int TotalPending)
        {
            if (Add1Sync != null)
            {
                Add1Sync(this, TotalPending);
            }
        }

        protected virtual void OnPairStatusChange(PairStatus pairStatus)
        {
            g_psPairStatus = pairStatus;
            if (PairStatusChange != null)
            {
                PairStatusChange(this, g_psPairStatus);
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

        #region v3.0.0.1 File Watcher 的处理
        /// <summary>
        /// 处理File Watcher监测到的新增文件/文件夹并返回一个可用于同步方法的DataTable
        /// </summary>
        /// <param name="obj_AddedItem">File Watcher监测到的对象</param>
        /// <param name="int_ObjType">对象类型，0是文件夹，1是文件</param>
        /// <param name="str_DirIdx">用于指示操作对象属于此配对的DIR1还是DIR2</param>
        /// <returns>可用于同步方法的DataTable</returns>
        private DataTable Fw_Object_Created(object obj_AddedItem, int int_ObjType, string str_DirIdx)
        {
            string str_OutLogMsg = String.Empty;
            bool bl_IsSyncFromDir1 = str_DirIdx == c_Dir1_Str;
            DataTable dt_fileDiff = Create_FileDiff_Empty();
            DirectoryInfo _dir = new DirectoryInfo(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path);
            string str_DirTableName = String.Join("_", g_sPairName, str_DirIdx, _dir.Name);
            string str_FileFullName = String.Empty;

            //检查DIR根目录是否存在，若不存在，则停止同步
            if (!_dir.Exists)
            {
                return dt_fileDiff;
            }

            //获取对象的实例，文件夹获取DirectoryInfo，文件获取FileInfo
            if (int_ObjType.Equals(0))
            {
                str_FileFullName = ((DirectoryInfo)obj_AddedItem).FullName;
            }
            else if (int_ObjType.Equals(1))
            {
                str_FileFullName = ((FileInfo)obj_AddedItem).FullName;
                while (FileHelper.IsFileOpenFS(str_FileFullName))
                {
                    string str_LogMsgW = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-FileOccupied:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgW, true, true, 4);
                    Thread.Sleep(c_WaitFileClose_Int);
                }
            }

            string str_LogMsgA = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-ObjectCreationDetected:" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgA, true, true, 3);
            OnSetOngoingItem(str_LogMsgA);
            string str_LogMsgACN = "检测到目录" + (bl_IsSyncFromDir1 ? "1" : "2") + "中新增了" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgACN, true, true, 1);

            //检查文件是否处于_FSBackup目录或者是处于排除列表
            if (CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
            {
                string str_LogMsgB = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-" + str_OutLogMsg;
                LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                OnSetOngoingItem(str_LogMsgB);

                if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                {
                    string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                    LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                }

                return dt_fileDiff;
            }

            try
            {
                if (int_ObjType.Equals(0))
                {
                    DirectoryInfo directoryInfo = (DirectoryInfo)obj_AddedItem;
                    string str_FileName = c_DirNameChar_Str;
                    string str_FilePath = str_FileFullName;
                    string str_FileSize = "0";
                    string str_FileMD5 = String.Empty;
                    string str_DirCreDate = directoryInfo.CreationTime.ToString(Files_InfoDB.DBDateTimeFormat);
                    string str_ToPath = str_FilePath.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);

                    string str_LogMsgAddItem = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-AddDIRInfor:" + str_FilePath;
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    OnSetOngoingItem(str_LogMsgAddItem);
                    if (!Files_InfoDB.AddFileInfor(str_DirTableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-AddDIRInforFailed:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    string str_OngoingRecMsg = "同步目录: " + str_FilePath + " -A-> " + str_ToPath;
                    OnSetOngoingItem(str_OngoingRecMsg);
                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                    dt_fileDiff.Rows.Add(str_FileName, str_FilePath, str_ToPath, str_FileMD5, str_DirCreDate, str_FileSize, bl_IsSyncFromDir1 ? "1" : "2", "");
                }
                else if (int_ObjType.Equals(1))
                {
                    FileInfo fileInfo = (FileInfo)obj_AddedItem;
                    string str_FileName = fileInfo.Name;
                    string str_FilePath = fileInfo.DirectoryName;
                    string str_FileSize = fileInfo.Length.ToString();
                    string str_FileLastModDate = fileInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat);
                    string str_ToPath = str_FilePath.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-CalculatingFileMD5:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgCalcMD5, false, true, 4);
                    OnSetOngoingItem(str_LogMsgCalcMD5);
                    string str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, true, out str_OutLogMsg);
                    LogPairMessage(g_sPairName, " - " + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(g_sPairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "PAIR-FileWatcher-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        return dt_fileDiff;
                    }

                    string str_LogMsgAddItem = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-AddFileInfor:" + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    if (!Files_InfoDB.AddFileInfor(str_DirTableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-AddFileInforFailed:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    string str_FromFile = Path.Combine(str_FilePath, str_FileName);
                    string str_ToFile = Path.Combine(str_ToPath, str_FileName);
                    string str_OngoingRecMsg = "同步文件: " + str_FromFile + " -A-> " + str_ToFile;
                    OnSetOngoingItem(str_OngoingRecMsg);
                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                    dt_fileDiff.Rows.Add(str_FileName, str_FilePath, str_ToPath, str_FileMD5, str_FileLastModDate, str_FileSize, bl_IsSyncFromDir1 ? "1" : "2", "");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PathTooLongException"))
                {
                    LogPairMessage(g_sPairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                }
                else
                {
                    str_OutLogMsg = "PAIR-FileWatcher-" + str_DirIdx + "-AddInfor-Exception:" + ex.Message;
                    LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                }
            }

            return dt_fileDiff;
        }

        /// <summary>
        /// 处理File Watcher监测到的修改文件并返回一个可用于同步方法的DataTable
        /// </summary>
        /// <param name="obj_ModifiedItem">File Watcher监测到的对象</param>
        /// <param name="int_ObjType">对象类型，1是文件</param>
        /// <param name="str_DirIdx">用于指示操作对象属于此配对的DIR1还是DIR2</param>
        /// <returns>可用于同步方法的DataTable</returns>
        private DataTable Fw_Object_Changed(object obj_ModifiedItem, int int_ObjType, string str_DirIdx)
        {
            string str_OutLogMsg = String.Empty;
            bool bl_IsSyncFromDir1 = str_DirIdx == c_Dir1_Str;
            DataTable dt_fileDiff = Create_FileDiff_Empty();
            DirectoryInfo _dir = new DirectoryInfo(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path);
            string str_DirTableName = String.Join("_", g_sPairName, str_DirIdx, _dir.Name);
            string str_FileFullName = String.Empty;

            //检查DIR根目录是否存在，若不存在，则停止同步
            if (!_dir.Exists)
            {
                return dt_fileDiff;
            }

            //获取对象的实例，文件获取FileInfo
            if (int_ObjType.Equals(1))
            {
                str_FileFullName = ((FileInfo)obj_ModifiedItem).FullName;
                while (FileHelper.IsFileOpenFS(str_FileFullName))
                {
                    string str_LogMsgW = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-FileOccupied:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgW, true, true, 4);
                    Thread.Sleep(c_WaitFileClose_Int);
                }
            }

            string str_LogMsgA = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-ObjectChangeDetected:" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgA, true, true, 3);
            OnSetOngoingItem(str_LogMsgA);
            string str_LogMsgACN = "检测到目录" + (bl_IsSyncFromDir1 ? "1" : "2") + "中修改了" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgACN, true, true, 1);

            //检查文件是否处于_FSBackup目录或者是处于排除列表
            if (CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
            {
                string str_LogMsgB = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-" + str_OutLogMsg;
                LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                OnSetOngoingItem(str_LogMsgB);

                if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                {
                    string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                    LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                }

                return dt_fileDiff;
            }

            try
            {
                if (int_ObjType.Equals(1))
                {
                    FileInfo fileInfo = (FileInfo)obj_ModifiedItem;
                    string str_FileName = fileInfo.Name;
                    string str_FilePath = fileInfo.DirectoryName;
                    string str_FileSize = fileInfo.Length.ToString();
                    string str_FileLastModDate = fileInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat);
                    string str_ToPath = str_FilePath.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-CalculatingFileMD5:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgCalcMD5, false, true, 4);
                    OnSetOngoingItem(str_LogMsgCalcMD5);
                    string str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, true, out str_OutLogMsg);
                    LogPairMessage(g_sPairName, "-" + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(g_sPairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "PAIR-FileWatcher-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }
                        return dt_fileDiff;
                    }

                    string str_LogMsgAddItem = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-UpdateFileInfor:" + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    if (!Files_InfoDB.AddFileInfor(str_DirTableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, g_sPairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-UpdateFileInforFailed:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    string str_FromFile = Path.Combine(str_FilePath, str_FileName);
                    string str_ToFile = Path.Combine(str_ToPath, str_FileName);
                    string str_OngoingRecMsg = "同步文件: " + str_FromFile + " -U-> " + str_ToFile;
                    OnSetOngoingItem(str_OngoingRecMsg);
                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                    dt_fileDiff.Rows.Add(str_FileName, str_FilePath, str_ToPath, str_FileMD5, str_FileLastModDate, str_FileSize, bl_IsSyncFromDir1 ? "3" : "4", "");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PathTooLongException"))
                {
                    LogPairMessage(g_sPairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                }
                else
                {
                    str_OutLogMsg = "PAIR-FileWatcher-" + str_DirIdx + "-ChangeInfor-Exception:" + ex.Message;
                    LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                }
            }

            return dt_fileDiff;
        }

        /// <summary>
        /// 处理File Watcher监测到的删除文件/文件夹并返回一个可用于同步方法的DataTable
        /// </summary>
        /// <param name="str_DeletedItem">File Watcher监测到的对象</param>
        /// <param name="int_ObjType">对象类型，0是文件夹，1是文件</param>
        /// <param name="str_DirIdx">用于指示操作对象属于此配对的DIR1还是DIR2</param>
        /// <returns>可用于同步方法的DataTable</returns>
        private DataTable Fw_Object_Deleted(string str_DeletedItem, int int_ObjType, string str_DirIdx)
        {
            string str_OutLogMsg = String.Empty;
            bool bl_IsSyncFromDir1 = str_DirIdx == c_Dir1_Str;
            DataTable dt_fileDiff = Create_FileDiff_Empty();
            DirectoryInfo _FromDir = new DirectoryInfo(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path);
            DirectoryInfo _ToDir = new DirectoryInfo(bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);
            string str_FromDirTableName = String.Join("_", g_sPairName, bl_IsSyncFromDir1 ? c_Dir1_Str : c_Dir2_Str, _FromDir.Name);
            string str_ToDirTableName = String.Join("_", g_sPairName, bl_IsSyncFromDir1 ? c_Dir2_Str : c_Dir1_Str, _ToDir.Name);
            string str_FileFullName = str_DeletedItem;

            //检查DIR根目录是否存在，若不存在，则停止同步
            if (!_FromDir.Exists)
            {
                return dt_fileDiff;
            }

            string str_LogMsgA = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-ObjectDeletionDetected:" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgA, true, true, 3);
            OnSetOngoingItem(str_LogMsgA);
            string str_LogMsgACN = "检测到目录" + (bl_IsSyncFromDir1 ? "1" : "2") + "中删除了" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgACN, true, true, 1);

            //检查文件是否处于_FSBackup目录或者是处于排除列表
            if (CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
            {
                string str_LogMsgB = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-" + str_OutLogMsg;
                LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                OnSetOngoingItem(str_LogMsgB);

                if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                {
                    string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                    LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                }

                return dt_fileDiff;
            }

            try
            {
                string[] arr_FileIDs = Files_InfoDB.GetFileIDs(str_FromDirTableName, str_FileFullName, out str_OutLogMsg);
                if (arr_FileIDs == null)
                {
                    LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 4);
                    return dt_fileDiff;
                }
                else
                {
                    foreach (string item in arr_FileIDs)
                    {
                        string _FileID = item.Split('|')[0];
                        string _FilePath = item.Split('|')[1];
                        string _FileName = item.Split('|')[2];
                        bool _IsFile = _FileName != c_DirNameChar_Str;
                        string str_LogMsgAddItem = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-SoftDeleteFileInfor:" + str_FileFullName;
                        OnSetOngoingItem(str_LogMsgAddItem);
                        LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                        if (!Files_InfoDB.DelFileInforSoft(str_FromDirTableName, _FileID, g_sPairID, out str_OutLogMsg))
                        {
                            LogPairMessage(g_sPairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                            LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                        }

                        string str_ToFile = str_FileFullName.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);
                        string str_ToPath = _FilePath.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);

                        string[] arr_toFileIDs = Files_InfoDB.GetFileIDs(str_ToDirTableName, str_ToFile, out str_OutLogMsg);
                        if (arr_toFileIDs != null)
                        {
                            foreach (string item2 in arr_toFileIDs)
                            {
                                string _ToFileID = item2.Split('|')[0];
                                //从DIR1中删除并同步至DIR2的，DIFFTYPE=5，需要从DIR2中删除，同步方向0/2
                                //从DIR2中删除并同步至DIR1的，DIFFTYPE=6，需要从DIR1中删除，同步方向0/4
                                if (bl_IsSyncFromDir1 && (g_iSyncDirection.Equals(0) || g_iSyncDirection.Equals(2)) ||
                                    (!bl_IsSyncFromDir1 && (g_iSyncDirection.Equals(0) || g_iSyncDirection.Equals(4))))
                                {
                                    string str_OngoingRecMsg = "同步" + (_IsFile ? "文件: " : "目录: ") + str_FileFullName + " -X-> " + str_ToFile;
                                    OnSetOngoingItem(str_OngoingRecMsg);
                                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);
                                    dt_fileDiff.Rows.Add(_FileName, _FilePath, str_ToPath, "", "", "", bl_IsSyncFromDir1 ? "5" : "6", _ToFileID);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PathTooLongException"))
                {
                    LogPairMessage(g_sPairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                }
                else
                {
                    str_OutLogMsg = "PAIR-FileWatcher-" + str_DirIdx + "-DeleteInfor-Exception:" + ex.Message;
                    LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                }
            }

            return dt_fileDiff;
        }

        /// <summary>
        /// 处理File Watcher监测到的重命名操作并同步（重命名操作是唯一不需要返回DataTable再同步的）
        /// </summary>
        /// <param name="obj_RenamedItem">File Watcher监测到的对象</param>
        /// <param name="int_ObjType">对象类型，0是文件夹，1是文件</param>
        /// <param name="str_DirIdx">用于指示操作对象属于此配对的DIR1还是DIR2</param>
        /// <param name="str_OldFullPath">重命名前的对象路径</param>
        /// <returns>同步是否成功</returns>
        private bool Fw_Object_Renamed(object obj_RenamedItem, int int_ObjType, string str_DirIdx, string str_OldFullPath)
        {
            string str_OutLogMsg = String.Empty;
            bool bl_IsSyncFromDir1 = str_DirIdx == c_Dir1_Str;
            DataTable dt_fileDiff = Create_FileDiff_Empty();
            DirectoryInfo _FromDir = new DirectoryInfo(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path);
            DirectoryInfo _ToDir = new DirectoryInfo(bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);
            string str_FromDirTableName = String.Join("_", g_sPairName, str_DirIdx, _FromDir.Name);
            string str_ToDirTableName = String.Join("_", g_sPairName, bl_IsSyncFromDir1 ? c_Dir2_Str : c_Dir1_Str, _ToDir.Name);
            string str_FileFullName = String.Empty;

            //检查DIR根目录是否存在，若不存在，则停止同步
            if (!_FromDir.Exists)
            {
                return false;
            }

            //获取对象的实例，文件夹获取DirectoryInfo，文件获取FileInfo
            if (int_ObjType.Equals(0))
            {
                str_FileFullName = ((DirectoryInfo)obj_RenamedItem).FullName;
            }
            else if (int_ObjType.Equals(1))
            {
                str_FileFullName = ((FileInfo)obj_RenamedItem).FullName;
                while (FileHelper.IsFileOpenFS(str_FileFullName))
                {
                    string str_LogMsgW = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-FileOccupied:" + str_FileFullName;
                    LogPairMessage(g_sPairName, str_LogMsgW, true, true, 4);
                    Thread.Sleep(c_WaitFileClose_Int);
                }
            }

            string str_LogMsgA = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-ObjectRenameDetected:" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgA, true, true, 3);
            OnSetOngoingItem(str_LogMsgA);
            string str_LogMsgACN = "检测到目录" + (bl_IsSyncFromDir1 ? "1" : "2") + "中重命名了" + str_OldFullPath + "至" + str_FileFullName;
            LogPairMessage(g_sPairName, str_LogMsgACN, true, true, 1);

            //检查文件是否处于_FSBackup目录或者是处于排除列表
            if (CheckFilterRule(g_sFilterRule, str_FileFullName, out str_OutLogMsg))
            {
                string str_LogMsgB = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-" + str_OutLogMsg;
                LogPairMessage(g_sPairName, str_LogMsgB, true, true, 4);
                OnSetOngoingItem(str_LogMsgB);

                if (str_OutLogMsg.Contains("PathLengthExceedsLimit"))
                {
                    string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                    LogPairMessage(g_sPairName, str_LogMsgC_CN, true, true, 1);
                }

                return false;
            }

            try
            {
                if (int_ObjType.Equals(0))
                {
                    //发起同步一方的目录
                    string str_NewDirPath1 = str_FileFullName;
                    string str_OldDirPath1 = str_OldFullPath;
                    //接受同步一方的目录
                    string str_NewDirPath2 = str_NewDirPath1.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);
                    string str_OldDirPath2 = str_OldDirPath1.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);

                    string str_LogMsgAddItem = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-RenameDIRInfor:" + str_OldDirPath1 + "-to-" + str_NewDirPath1;
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    OnSetOngoingItem(str_LogMsgAddItem);
                    if (!Files_InfoDB.RenameFileOrDir(str_FromDirTableName, String.Empty, str_OldDirPath1, str_NewDirPath1, false, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-RenameDIRInforFailed-A:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_NewDirPath1, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    string str_OngoingRecMsg = "同步目录: " + str_NewDirPath1 + " -R-> " + str_NewDirPath2;
                    OnSetOngoingItem(str_OngoingRecMsg);
                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);

                    FileHelper.xCopyDirectory(str_OldDirPath2, str_NewDirPath2, true, true);
                    if (!Files_InfoDB.RenameFileOrDir(str_ToDirTableName, String.Empty, str_OldDirPath2, str_NewDirPath2, false, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-RenameDIRInforFailed-B:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_NewDirPath2, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    OnSetOngoingItem(string.Empty);
                    return Directory.Exists(str_NewDirPath2);
                }
                else if (int_ObjType.Equals(1))
                {
                    FileInfo fileInfo = (FileInfo)obj_RenamedItem;
                    string str_ParentDir1 = fileInfo.DirectoryName;
                    string str_ParentDir2 = str_ParentDir1.Replace(bl_IsSyncFromDir1 ? g_sDir1Path : g_sDir2Path, bl_IsSyncFromDir1 ? g_sDir2Path : g_sDir1Path);
                    string str_NewFileName = fileInfo.Name;
                    string str_OldFileName = Path.GetFileName(str_OldFullPath);

                    string str_LogMsgAddItem = "PAIR-FileWatcher:" + g_sPairName + "-" + str_DirIdx + "-RenameFileInfor:" + str_OldFullPath + "-to-" + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(g_sPairName, str_LogMsgAddItem, true, true, 4);
                    if (!Files_InfoDB.RenameFileOrDir(str_FromDirTableName, str_ParentDir1, str_OldFileName, str_NewFileName, true, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-RenameFileInforFailed-A:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    string str_FromFile2 = Path.Combine(str_ParentDir2, str_OldFileName);
                    string str_ToFile2 = Path.Combine(str_ParentDir2, str_NewFileName);
                    string str_OngoingRecMsg = "同步文件: " + str_FileFullName + " -R-> " + str_ToFile2;
                    OnSetOngoingItem(str_OngoingRecMsg);
                    LogPairMessage(g_sPairName, str_OngoingRecMsg, true, true, 1);

                    FileHelper.xCopyFile(str_FromFile2, str_ToFile2, true, true);
                    if (!Files_InfoDB.RenameFileOrDir(str_ToDirTableName, str_ParentDir2, str_OldFileName, str_NewFileName, true, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "PAIR-FileWatcher-RenameDIRInforFailed-B:" + str_OutLogMsg;
                        LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_ToFile2, true, true, 1);
                        LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                    }

                    OnSetOngoingItem(string.Empty);
                    return File.Exists(str_ToFile2);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("PathTooLongException"))
                {
                    LogPairMessage(g_sPairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, ex.Message, true, true, 5, true);
                }
                else
                {
                    str_OutLogMsg = "PAIR-FileWatcher-" + str_DirIdx + "-RenameInfor-Exception:" + ex.Message;
                    LogPairMessage(g_sPairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                    LogPairMessage(g_sPairName, str_OutLogMsg, true, true, 5, true);
                }
                return false;
            }
        }

        /// <summary>
        /// 检查输入文件的所在目录里是否有丢失的文件与输入文件的大小，修改时间都相同，以此判断是否被重命名
        /// </summary>
        /// <param name="str_FileFullName">检查文件的完整路径</param>
        /// <param name="str_Dir_String">配对的目录索引字符串</param>
        /// <param name="str_OutMissingFileFullName">若有文件丢失，则返回丢失文件的完整路径</param>
        /// <returns></returns>
        private bool CheckMissingFile(string str_FileFullName, string str_Dir_String, out string str_OutMissingFileFullName)
        {
            str_OutMissingFileFullName = string.Empty;
            bool bRet = false;
            string str_OutLogMsg = string.Empty;
            FileInfo fileInfo = new FileInfo(str_FileFullName);
            string str_FileName = fileInfo.Name;
            string str_ParentDir = fileInfo.DirectoryName;
            string sFileSize = fileInfo.Length.ToString();
            string sFileLastModDate = fileInfo.LastWriteTime.ToString(Files_InfoDB.DBDateTimeFormat);
            string str_TableName = g_sPairName + "_" + str_Dir_String + "_";
            if (str_Dir_String == "DIR1")
            {
                str_TableName += new DirectoryInfo(g_sDir1Path).Name;
            }
            else if (str_Dir_String == "DIR2")
            {
                str_TableName += new DirectoryInfo(g_sDir2Path).Name;
            }

            string str_Where = @"FilePath='" + str_ParentDir + @"' and FileSize='" + sFileSize + @"' and FileLastModDate='" + sFileLastModDate + @"' and FileName<>'" + str_FileName + @"' and FileStatus='AC'";
            DataTable dt_FileInforDB = Files_InfoDB.GetFileInfor(str_TableName, out str_OutLogMsg);
            DataRow[] dr_FilesInParentDir = dt_FileInforDB.Select(str_Where);

            if (dr_FilesInParentDir.Length > 0)
            {
                for (int i = 0; i < dr_FilesInParentDir.Length; i++)
                {
                    string sMissingFileName = dr_FilesInParentDir[i]["FileName"].ToString();
                    string sMissingFileFullName = Path.Combine(str_ParentDir, sMissingFileName);
                    if (!File.Exists(sMissingFileFullName))
                    {
                        str_OutMissingFileFullName = Path.Combine(str_ParentDir, sMissingFileName);
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private void G_Files_Info_ObjectsInforReady(object sender)
        {
            var dt_InitGap = DateTime.Now.Subtract(g_InitTime);
            int iTotalObjectCount = g_Files_Info.TotalObjectCount();
            string sInitDoneMsg = "配对（" + g_sPairName + "）信息初始化完成，花费" + dt_InitGap.TotalSeconds.ToString() + "秒，找到" + iTotalObjectCount.ToString() + "个文件/文件夹对象";
            LogPairMessage(g_sPairName, sInitDoneMsg, true, true, 2);
            if (g_iAutoSyncInterval.Equals(0))
            {
                LogPairMessage(g_sPairName, "即将进行程序启动的第一次同步", true, true, 2);
            }
            g_ObjectsInforReady = true;
            OnObjectsInforReady(sInitDoneMsg);
        }

        private void G_Files_Info_Dir1ObjectChanged(object sender, FileChangedEvent e)
        {
            #region 同步间隔为0，做实时同步操作
            if (g_iAutoSyncInterval.Equals(0))
            {
                switch (e.ChangeType)
                {
                    case ChangeType.CHANGED:
                        Fw_Dir1_OnChanged(e); break;
                    case ChangeType.CREATED:
                        Fw_Dir1_OnCreated(e); break;
                    case ChangeType.DELETED:
                        Fw_Dir1_OnDeleted(e); break;
                    case ChangeType.RENAMED:
                        Fw_Dir1_OnRenamed(e); break;
                    case ChangeType.LOG:
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region 同步间隔不为0，做定时同步操作
            else
            {
                return;
            }
            #endregion
        }

        private void G_Files_Info_Dir2ObjectChanged(object sender, FileChangedEvent e)
        {
            #region 同步间隔为0，做实时同步操作
            if (g_iAutoSyncInterval.Equals(0))
            {
                switch (e.ChangeType)
                {
                    case ChangeType.CHANGED:
                        Fw_Dir2_OnChanged(e); break;
                    case ChangeType.CREATED:
                        Fw_Dir2_OnCreated(e); break;
                    case ChangeType.DELETED:
                        Fw_Dir2_OnDeleted(e); break;
                    case ChangeType.RENAMED:
                        Fw_Dir2_OnRenamed(e); break;
                    case ChangeType.LOG:
                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region 同步间隔不为0，做定时同步操作
            else
            {
                return;
            }
            #endregion
        }

        private async void Fw_Dir1_OnCreated(FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;
            while (FileHelper.IsFileOpenFS(str_FullPath))
            {
                string str_PrintMsgF = "Fw_Dir1_OnCreated - File in use! " + str_FullPath;
                LogPairMessage(g_sPairName, str_PrintMsgF, true, true, 3);
                Thread.SpinWait(1000);
            }

            string str_ObjectName = str_FullPath.Replace(g_sDir1Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir1Path, g_sDir2Path);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgA = "Fw_Dir1_OnCreated - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
            string str_PrintMsgB = "Fw_Dir1_OnCreated - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
            string str_PrintMsgC = "Fw_Dir1_OnCreated - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                //bool bRealTimeSyncResult = await RealTimeSyncItem(obj_ChangedItem, 0, c_Dir1_Str);
                bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(obj_ChangedItem, 0, c_Dir1_Str));
                if (!bRealTimeSyncResult)
                {
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnCreated - Failed to Sync", true, true, 4);
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnCreated - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "Fw_Dir1_OnCreated - Remove from Queue", true, true, 4);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir1_OnChanged(FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir1Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir1Path, g_sDir2Path);
            bool bl_Sync_CheckQueueOKFromAdd = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgF = "Fw_Dir1_OnChanged - Check Queue From Add result: " + bl_Sync_CheckQueueOKFromAdd.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgF, true, true, 4);

            if (!bl_Sync_CheckQueueOKFromAdd) return;

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (i_Type.Equals(1))
            {
                bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

                string str_PrintMsgA = "Fw_Dir1_OnChanged - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
                LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
                string str_PrintMsgB = "Fw_Dir1_OnChanged - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
                LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
                string str_PrintMsgC = "Fw_Dir1_OnChanged - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
                LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

                string str_MissingFileName = String.Empty;
                if (CheckMissingFile(str_FullPath, c_Dir1_Str, out str_MissingFileName))
                {
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnChanged - Located missing file, route to Rename action", true, true, 4);
                    FileChangedEvent e2 = e;
                    e2.OldFullPath = str_MissingFileName;
                    Fw_Dir1_OnRenamed(e2);
                    return;
                }

                if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
                {
                    Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    //bool bRealTimeSyncResult = await RealTimeSyncItem(obj_ChangedItem, 1, c_Dir1_Str);
                    bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(obj_ChangedItem, 1, c_Dir1_Str));
                    if (!bRealTimeSyncResult)
                    {
                        LogPairMessage(g_sPairName, "Fw_Dir1_OnChanged - Failed to Sync", true, true, 4);
                        LogPairMessage(g_sPairName, "Fw_Dir1_OnChanged - Remove from Queue", true, true, 4);
                        Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    }
                }
                else
                {
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnChanged - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir1_OnDeleted(FileChangedEvent e)
        {
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir1Path, "");
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir1_OnDeleted - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
            string str_PrintMsgB = "Fw_Dir1_OnDeleted - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
            string str_PrintMsgC = "Fw_Dir1_OnDeleted - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(str_FullPath, 2, c_Dir1_Str));
                if (!bRealTimeSyncResult)
                {
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnDeleted - Failed to Sync", true, true, 4);
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnDeleted - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "Fw_Dir1_OnDeleted - Remove from Queue", true, true, 4);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir1_OnRenamed(FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_NewFullPath = e.FullPath;
            string str_OldFullPath = e.OldFullPath;
            if (CheckFilterRule(g_sFilterRule, str_NewFullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_NewFullPath.Replace(g_sDir1Path, "");
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_NewFullPath, out i_Type);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir1_OnRenamed - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
            string str_PrintMsgB = "Fw_Dir1_OnRenamed - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
            string str_PrintMsgC = "Fw_Dir1_OnRenamed - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(obj_ChangedItem, 3, c_Dir1_Str, str_OldFullPath));
                if (!bRealTimeSyncResult)
                {
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnRenamed - Failed to Sync", true, true, 4);
                    LogPairMessage(g_sPairName, "Fw_Dir1_OnRenamed - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "Fw_Dir1_OnRenamed - Remove from Queue", true, true, 4);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir2_OnCreated(FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;
            while (FileHelper.IsFileOpenFS(str_FullPath))
            {
                string str_PrintMsgF = "Fw_Dir2_OnCreated - File in use! " + str_FullPath;
                LogPairMessage(g_sPairName, str_PrintMsgF, true, true, 3);
                Thread.SpinWait(1000);
            }

            string str_ObjectName = str_FullPath.Replace(g_sDir2Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir2Path, g_sDir1Path);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgA = "Fw_Dir2_OnCreated - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
            string str_PrintMsgB = "Fw_Dir2_OnCreated - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
            string str_PrintMsgC = "Fw_Dir2_OnCreated - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(obj_ChangedItem, 0, c_Dir2_Str));
                if (!bRealTimeSyncResult)
                {
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnCreated - Failed to Sync", true, true, 4);
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnCreated - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "Fw_Dir2_OnCreated - Remove from Queue", true, true, 4);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir2_OnChanged(FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir2Path, "");
            string str_SyncFileName = str_FullPath.Replace(g_sDir2Path, g_sDir1Path);
            bool bl_Sync_CheckQueueOKFromAdd = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
            bool bl_Are2FilesSame = FileHelper.CheckTwoFilesSame(str_FullPath, str_SyncFileName);

            string str_PrintMsgF = "Fw_Dir2_OnChanged - Check Queue From Add result: " + bl_Sync_CheckQueueOKFromAdd.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgF, true, true, 4);

            if (!bl_Sync_CheckQueueOKFromAdd) return;

            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (i_Type.Equals(1))
            {
                bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

                string str_PrintMsgA = "Fw_Dir2_OnChanged - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
                LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
                string str_PrintMsgB = "Fw_Dir2_OnChanged - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
                LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
                string str_PrintMsgC = "Fw_Dir2_OnChanged - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
                LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

                string str_MissingFileName = String.Empty;
                if (CheckMissingFile(str_FullPath, c_Dir2_Str, out str_MissingFileName))
                {
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnChanged - Located missing file, route to Rename action", true, true, 4);
                    FileChangedEvent e2 = e;
                    e2.OldFullPath = str_MissingFileName;
                    Fw_Dir2_OnRenamed(e2);
                    return;
                }

                if (bl_Sync_CheckQueueOK && !bl_Are2FilesSame)
                {
                    Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(obj_ChangedItem, 1, c_Dir2_Str));
                    if (!bRealTimeSyncResult)
                    {
                        LogPairMessage(g_sPairName, "Fw_Dir2_OnChanged - Failed to Sync", true, true, 4);
                        LogPairMessage(g_sPairName, "Fw_Dir2_OnChanged - Remove from Queue", true, true, 4);
                        Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                    }
                }
                else
                {
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnChanged - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.ADD, str_ObjectName);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir2_OnDeleted(FileChangedEvent e)
        {
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_FullPath.Replace(g_sDir2Path, "");
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir2_OnDeleted - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
            string str_PrintMsgB = "Fw_Dir2_OnDeleted - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
            string str_PrintMsgC = "Fw_Dir2_OnDeleted - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(str_FullPath, 2, c_Dir2_Str));
                if (!bRealTimeSyncResult)
                {
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnDeleted - Failed to Sync", true, true, 4);
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnDeleted - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "Fw_Dir2_OnDeleted - Remove from Queue", true, true, 4);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.DELETE, str_ObjectName);
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async void Fw_Dir2_OnRenamed(FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_NewFullPath = e.FullPath;
            string str_OldFullPath = e.OldFullPath;
            if (CheckFilterRule(g_sFilterRule, str_NewFullPath, out str_OutLogMsg)) return;

            string str_ObjectName = str_NewFullPath.Replace(g_sDir2Path, "");
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_NewFullPath, out i_Type);
            bool bl_Sync_CheckQueueOK = Sync_Queue_Helper.Fw_Sync_CheckQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);

            string str_PrintMsgA = "Fw_Dir2_OnRenamed - Check Queue result: " + bl_Sync_CheckQueueOK.ToString();
            LogPairMessage(g_sPairName, str_PrintMsgA, true, true, 4);
            string str_PrintMsgB = "Fw_Dir2_OnRenamed - FwSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_FwSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgB, true, true, 4);
            string str_PrintMsgC = "Fw_Dir2_OnRenamed - NmSyncQueue:" + Environment.NewLine + Sync_Queue_Helper.Get_NmSyncQueue_Str();
            LogPairMessage(g_sPairName, str_PrintMsgC, true, true, 4);

            if (bl_Sync_CheckQueueOK)
            {
                Sync_Queue_Helper.Fw_Sync_AddQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                bool bRealTimeSyncResult = await Task.Run(() => RealTimeSyncItem(obj_ChangedItem, 3, c_Dir2_Str, str_OldFullPath));
                if (!bRealTimeSyncResult)
                {
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnRenamed - Failed to Sync", true, true, 4);
                    LogPairMessage(g_sPairName, "Fw_Dir2_OnRenamed - Remove from Queue", true, true, 4);
                    Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir2_Str, c_Dir1_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
                }
            }
            else
            {
                LogPairMessage(g_sPairName, "Fw_Dir2_OnRenamed - Remove from Queue", true, true, 4);
                Sync_Queue_Helper.Fw_Sync_DelQueue(g_sPairName, c_Dir1_Str, c_Dir2_Str, Sync_Queue_Helper.SyncAction.UPDATE, str_ObjectName);
            }

            LogPairMessage(g_sPairName, "-----------------------------------------------------------------", true, true, 4);
        }

        private async Task<bool> RealTimeSyncItem(object obj_SyncItem, int int_Action, string str_DirIdx, string str_OldFullPath = "")
        {
            if (obj_SyncItem == null) return false;
            DataTable dt_fileDiff = new DataTable();

            //文件夹操作，只有新增/修改/重命名
            if (obj_SyncItem.GetType() == typeof(DirectoryInfo))
            {
                //int_Action=0 --- 新增
                if (int_Action.Equals(0))
                {
                    dt_fileDiff = Fw_Object_Created(obj_SyncItem, 0, str_DirIdx);
                }
                //int_Action=1 --- 修改
                else if (int_Action.Equals(1))
                {
                    dt_fileDiff = Fw_Object_Changed(obj_SyncItem, 0, str_DirIdx);
                }
                //int_Action=3 --- 重命名是唯一不需要返回DataTable再同步的
                else if (int_Action.Equals(3))
                {
                    return Fw_Object_Renamed(obj_SyncItem, 0, str_DirIdx, str_OldFullPath);
                }
                else
                {
                    return false;
                }
            }
            //文件操作，只有新增/修改/重命名
            else if (obj_SyncItem.GetType() == typeof(FileInfo))
            {
                //int_Action=0 --- 新增
                if (int_Action.Equals(0))
                {
                    dt_fileDiff = Fw_Object_Created(obj_SyncItem, 1, str_DirIdx);
                }
                //int_Action=1 --- 修改
                else if (int_Action.Equals(1))
                {
                    dt_fileDiff = Fw_Object_Changed(obj_SyncItem, 1, str_DirIdx);
                }
                //int_Action=3 --- 重命名是唯一不需要返回DataTable再同步的
                else if (int_Action.Equals(3))
                {
                    return Fw_Object_Renamed(obj_SyncItem, 1, str_DirIdx, str_OldFullPath);
                }
                else
                {
                    return false;
                }
            }
            //路径操作，只有删除
            else if (obj_SyncItem.GetType() == typeof(String))
            {
                //int_Action=2 --- 删除
                if (int_Action.Equals(2))
                {
                    dt_fileDiff = Fw_Object_Deleted(obj_SyncItem.ToString(), 1, str_DirIdx);
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

            if (dt_fileDiff != null && dt_fileDiff.Rows.Count > 0)
            {
                bool bSyncResult = await Task.Run(() => SyncDirPair(dt_fileDiff, true));
                return bSyncResult;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
