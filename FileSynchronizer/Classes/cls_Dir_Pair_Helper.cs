using Common.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FileSynchronizer.cls_Common_Constants;

namespace FileSynchronizer
{
    /// <summary>
    /// cls_Dir_Pair_Helper类
    /// </summary>
    internal class cls_Dir_Pair_Helper
    {
        #region 变量和构造函数
        private string str_PairID;
        private string str_PairName;
        private string str_Dir1Path;
        private string str_Dir2Path;
        private string str_DirLastSyncTime;
        private PairStatus ps_PairStatus;
        private string str_FilterRule;
        private int int_AutoSyncInterval;
        private int int_SyncDirection;
        private bool bl_IsPaused;
        private cls_LogPairFile cls_PairLogFile;

        public cls_Dir_Pair_Helper(string PairID, string PairName, string DirPath1, string DirPath2, string LastSyncDT, PairStatus PairStatus, string FilterRule, int AutoSyncInterval, int SyncDirection, bool IsPaused)
        {
            str_PairID = PairID;
            str_PairName = PairName;
            str_Dir1Path = DirPath1;
            str_Dir2Path = DirPath2;
            str_DirLastSyncTime = LastSyncDT;
            ps_PairStatus = PairStatus;
            str_FilterRule = FilterRule;
            int_AutoSyncInterval = AutoSyncInterval;
            int_SyncDirection = SyncDirection;
            bl_IsPaused = IsPaused;
            cls_PairLogFile = new cls_LogPairFile(PairName, false);
        }
        #endregion

        #region 分析和同步方法
        /// <summary>
        /// 分析文件夹配对
        /// </summary>
        /// <param name="IsAnalysisOnly"></param>
        public DataTable AnalysisDirPair(bool IsAnalysisOnly)
        {
            #region Define Varibles
            LogPairMessage(str_PairName, "开始分析配对（" + str_PairName + "）", true, true, 2);
            OnPairStatusChange(PairStatus.ANALYSIS);
            string str_OutLogMsg = String.Empty;
            string[] arr_FilterRule = String.IsNullOrEmpty(str_FilterRule) ? new string[] { } : str_FilterRule.Split(',');
            DateTime dt_DirLastSyncTime;
            bool bl_LastSyncStatus = DateTime.TryParse(str_DirLastSyncTime, out dt_DirLastSyncTime);
            if (!bl_LastSyncStatus)
            {
                LogPairMessage(str_PairName, "没有找到上次同步时间，首次分析同步需时较长，请耐心等待", true, true, 1);
                dt_DirLastSyncTime = DateTime.MinValue;
            }

            //DIR1的子目录和文件信息
            LogPairMessage(str_PairName, "开始获取DIR1（" + str_Dir1Path + "）的目录和文件信息", true, true, 2);
            DirectoryInfo _dir1 = new DirectoryInfo(str_Dir1Path);
            //检查DIR1根目录是否存在，若不存在，则提示出错并停止分析
            if (bl_LastSyncStatus && !_dir1.Exists)
            {
                LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录1可能出现问题，请检查，若目录内容为空，请忽略此提示", true, true, 1);
                return null;
            }
            string str_Dir1TableName = str_PairName + "_DIR1_" + _dir1.Name;
            DirectoryInfo[] subDir1 = _dir1.GetDirectories("*", SearchOption.AllDirectories);
            FileInfo[] fileInfos1 = _dir1.GetFiles("*", SearchOption.AllDirectories);
            DataTable dt_File1InforDB = cls_Files_InfoDB.GetFileInfor(str_Dir1TableName, out str_OutLogMsg);

            //DIR2的子目录和文件信息
            LogPairMessage(str_PairName, "开始获取DIR2（" + str_Dir2Path + "）的目录和文件信息", true, true, 2);
            DirectoryInfo _dir2 = new DirectoryInfo(str_Dir2Path);
            //检查DIR2根目录是否存在，若不存在，则提示出错并停止分析
            if (bl_LastSyncStatus && !_dir2.Exists)
            {
                LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录2可能出现问题，请检查，若目录内容为空，请忽略此提示", true, true, 1);
                return null;
            }
            string str_Dir2TableName = str_PairName + "_DIR2_" + _dir2.Name;
            DirectoryInfo[] subDir2 = _dir2.GetDirectories("*", SearchOption.AllDirectories);
            FileInfo[] fileInfos2 = _dir2.GetFiles("*", SearchOption.AllDirectories);
            DataTable dt_File2InforDB = cls_Files_InfoDB.GetFileInfor(str_Dir2TableName, out str_OutLogMsg);

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
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = directoryInfo.FullName;
                    str_FileName = C_StrDirNameChar;
                    str_FilePath = directoryInfo.FullName;
                    str_FileSize = "0";
                    str_FileMD5 = String.Empty;
                    str_DirCreDate = directoryInfo.CreationTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(C_StrFSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Exclude DIR: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_DirCreDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File1InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR1-ExistsInDB-Exclude DIR: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > C_IMaxDirLengthWIN32)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Path length exceeds limit-Skip DIR: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        OnAdd1Analysis(int_TotalFileFound);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Add DIR: " + str_FilePath;
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    OnSetOngoingItem(str_LogMsgAddItem);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-1-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-1-Exception:" + ex.Message;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从目录1的文件分析至数据库
            foreach (FileInfo fileInfo in fileInfos1)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = fileInfo.FullName;
                    str_FileName = fileInfo.Name;
                    str_FilePath = fileInfo.DirectoryName;
                    str_FileSize = fileInfo.Length.ToString();
                    str_FileLastModDate = fileInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(C_StrFSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Exclude File: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_FileLastModDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File1InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR1-ExistsInDB-Exclude File: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > C_IMaxDirLengthWIN32)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Path length exceeds limit-Skip File: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入文件信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        OnAdd1Analysis(int_TotalFileFound);
                        continue;
                    }

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Calculating File MD5: " + str_FileFullName;
                    LogPairMessage(str_PairName, str_LogMsgCalcMD5, false, true, 4);
                    OnSetOngoingItem(str_LogMsgCalcMD5);
                    str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, IsAnalysisOnly, out str_OutLogMsg);
                    LogPairMessage(str_PairName, " - " + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "Step-2-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Add File: " + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-2-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-2-ExceptionB:" + ex.Message;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从数据库1分析至文件和目录
            foreach (DataRow dataRow in dt_File1InforDB.Rows)
            {
                str_FileID = dataRow.ItemArray[0].ToString();
                str_FileName = dataRow.ItemArray[1].ToString();
                str_FilePath = dataRow.ItemArray[2].ToString();
                str_FileMD5 = dataRow.ItemArray[4].ToString();
                str_FileFullName = Path.Combine(str_FilePath, str_FileName);
                bool bl_hitFilterRule = false;

                //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                if (!Directory.Exists(str_Dir1Path))
                {
                    bl_ExceptionFound = true;
                    break;
                }

                //检查文件是否处于_FSBackup目录，如果存在则跳过
                if (str_FileFullName.Contains(C_StrFSBackup))
                {
                    bl_hitFilterRule = true;
                }

                //检查文件是否处于排除列表，如果存在则跳过
                Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                {
                    if (str_FileFullName.Contains(i))
                    {
                        bl_hitFilterRule = true;
                        LoopState.Break();
                    }
                });
                //for (int i = 0; i < arr_FilterRule.Length; i++)
                //{
                //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                //    {
                //        bl_hitFilterRule = true;
                //        break;
                //    }
                //}

                if (bl_hitFilterRule)
                {
                    string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Soft Delete item: " + str_FileFullName + " due to filter rule";
                    LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                    OnSetOngoingItem(str_LogMsgA);
                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                    {
                        LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR1-FilterRule-Soft Delete item: " + str_FileFullName + " due to filter rule", true, true, 4);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        Thread.Sleep(i_SleepInterval);
                    }
                    OnAdd1Analysis(int_TotalFileFound);
                    continue;
                }

                if (str_FileName.Equals(C_StrDirNameChar))
                {
                    //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir1Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!Directory.Exists(str_FilePath))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Soft Delete DIR: " + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Check DIR: " + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }
                else
                {
                    //检查DIR1根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir1Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!File.Exists(str_FileFullName))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Soft Delete File: " + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] " + str_LogMsgAddItem, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR1-Check File: " + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }

                LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                OnAdd1Analysis(int_TotalFileFound);
            }
            LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录1（" + str_Dir1Path + "）分析完成", true, true, 2);

            //从目录2的子目录分析至数据库
            foreach (DirectoryInfo directoryInfo in subDir2)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = directoryInfo.FullName;
                    str_FileName = C_StrDirNameChar;
                    str_FilePath = directoryInfo.FullName;
                    str_FileSize = "0";
                    str_FileMD5 = String.Empty;
                    str_DirCreDate = directoryInfo.CreationTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(C_StrFSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR2-FilterRule-Exclude DIR: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                        OnSetOngoingItem(str_LogMsgA);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_DirCreDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File2InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR2-ExistsInDB-Exclude DIR: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > C_IMaxDirLengthWIN32)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Path length exceeds limit-Skip DIR: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入目录信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        OnAdd1Analysis(int_TotalFileFound);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Add DIR: " + str_FilePath;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_DirCreDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-4-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "目录名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-4-Exception:" + ex.Message;
                        LogPairMessage(str_PairName, "写入目录信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从目录2的文件分析至数据库
            foreach (FileInfo fileInfo in fileInfos2)
            {
                try
                {
                    str_OutLogMsg = String.Empty;
                    str_FileFullName = fileInfo.FullName;
                    str_FileName = fileInfo.Name;
                    str_FilePath = fileInfo.DirectoryName;
                    str_FileSize = fileInfo.Length.ToString();
                    str_FileLastModDate = fileInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat);
                    bool bl_hitFilterRule = false;

                    //检查文件是否处于_FSBackup目录，如果存在则跳过
                    if (str_FileFullName.Contains(C_StrFSBackup))
                    {
                        bl_hitFilterRule = true;
                    }

                    //检查文件是否处于排除列表，如果存在则跳过
                    Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                    {
                        if (str_FileFullName.Contains(i))
                        {
                            bl_hitFilterRule = true;
                            LoopState.Break();
                        }
                    });
                    //for (int i = 0; i < arr_FilterRule.Length; i++)
                    //{
                    //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                    //    {
                    //        bl_hitFilterRule = true;
                    //        break;
                    //    }
                    //}

                    if (bl_hitFilterRule)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR2-FilterRule-Exclude File: " + str_FileFullName + " due to filter rule";
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //检查文件是否已经在数据库存在，如果存在则跳过
                    str_Where = @"FileName='" + str_FileName.Replace("'", "''") + @"' and FilePath='" + str_FilePath.Replace("'", "''") + @"' and FileLastModDate='" + str_FileLastModDate + @"' and FileSize = " + str_FileSize;
                    if (dt_File2InforDB.Select(str_Where).Length > 0)
                    {
                        string str_LogMsgB = "PAIR-ANALYSIS: " + str_PairName + " DIR2-ExistsInDB-Exclude File: " + str_FileFullName;
                        LogPairMessage(str_PairName, str_LogMsgB, true, true, 4);
                        OnSetOngoingItem(str_LogMsgB);
                        OnAdd1Analysis(int_TotalFileFound);
                        //Thread.Sleep(50);
                        continue;
                    }

                    //绝对路径总长度超过260，系统无法处理，抛出信息后跳过
                    if (str_FileFullName.Length > C_IMaxDirLengthWIN32)
                    {
                        string str_LogMsgC = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Path length exceeds limit-Skip File: " + str_FileFullName;
                        string str_LogMsgC_CN = "写入文件信息至数据库失败：" + str_FileFullName + "，路径长度超过系统限制（260个字符）";
                        LogPairMessage(str_PairName, str_LogMsgC, true, true, 4);
                        LogPairMessage(str_PairName, str_LogMsgC_CN, true, true, 1);
                        OnAdd1Analysis(int_TotalFileFound);
                        continue;
                    }

                    //计算文件的MD5
                    string str_LogMsgCalcMD5 = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Calculating File MD5: " + str_FileFullName;
                    LogPairMessage(str_PairName, str_LogMsgCalcMD5, false, true, 4);
                    OnSetOngoingItem(str_LogMsgCalcMD5);
                    str_FileMD5 = CalcFileMD5withLocal(str_FileFullName, IsAnalysisOnly, out str_OutLogMsg);
                    LogPairMessage(str_PairName, " - " + str_FileMD5, true, false, 4);

                    if (String.IsNullOrEmpty(str_FileMD5))
                    {
                        if (str_OutLogMsg.Contains("PathTooLongException"))
                        {
                            LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        else
                        {
                            str_OutLogMsg = "Step-5-ExceptionA:" + str_OutLogMsg;
                            LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                            LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        }
                        Thread.Sleep(i_SleepInterval);
                        continue;
                    }

                    string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Add File: " + str_FileFullName;
                    OnSetOngoingItem(str_LogMsgAddItem);
                    LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                    if (!cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FilePath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_OutLogMsg))
                    {
                        str_OutLogMsg = "Step-5-AddFileInfor:" + str_OutLogMsg;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    else
                    {
                        LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                        OnAdd1Analysis(int_TotalFileFound);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("PathTooLongException"))
                    {
                        LogPairMessage(str_PairName, "文件名过长，请检查后再试或者缩短路径长度：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    }
                    else
                    {
                        str_OutLogMsg = "Step-5-ExceptionB:" + ex.Message;
                        LogPairMessage(str_PairName, "写入文件信息至数据库失败：" + str_FileFullName, true, true, 1);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                    }
                    Thread.Sleep(i_SleepInterval);
                }
            }

            //从数据库2分析至文件和目录
            foreach (DataRow dataRow in dt_File2InforDB.Rows)
            {
                str_FileID = dataRow.ItemArray[0].ToString();
                str_FileName = dataRow.ItemArray[1].ToString();
                str_FilePath = dataRow.ItemArray[2].ToString();
                str_FileMD5 = dataRow.ItemArray[4].ToString();
                str_FileFullName = Path.Combine(str_FilePath, str_FileName);
                bool bl_hitFilterRule = false;

                //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                if (!Directory.Exists(str_Dir2Path))
                {
                    bl_ExceptionFound = true;
                    break;
                }

                //检查文件是否处于_FSBackup目录，如果存在则跳过
                if (str_FileFullName.Contains(C_StrFSBackup))
                {
                    bl_hitFilterRule = true;
                }

                //检查文件是否处于排除列表，如果存在则跳过
                Parallel.ForEach(arr_FilterRule, (i, LoopState) =>
                {
                    if (str_FileFullName.Contains(i))
                    {
                        bl_hitFilterRule = true;
                        LoopState.Break();
                    }
                });
                //for (int i = 0; i < arr_FilterRule.Length; i++)
                //{
                //    if (str_FileFullName.Contains(arr_FilterRule[i]))
                //    {
                //        bl_hitFilterRule = true;
                //        break;
                //    }
                //}

                if (bl_hitFilterRule)
                {
                    string str_LogMsgA = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete item: " + str_FileFullName + " due to filter rule";
                    LogPairMessage(str_PairName, str_LogMsgA, true, true, 4);
                    OnSetOngoingItem(str_LogMsgA);
                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                    {
                        LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete item: " + str_FileFullName + " due to filter rule", true, true, 4);
                        LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                        Thread.Sleep(i_SleepInterval);
                    }
                    OnAdd1Analysis(int_TotalFileFound);
                    continue;
                }

                if (str_FileName.Equals(C_StrDirNameChar))
                {
                    //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir2Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!Directory.Exists(str_FilePath))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete DIR: " + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete DIR: " + str_FilePath, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Check DIR: " + str_FilePath;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }
                else
                {
                    //检查DIR2根目录是否存在，若不存在，则提示出错并停止同步
                    if (!Directory.Exists(str_Dir2Path))
                    {
                        bl_ExceptionFound = true;
                        break;
                    }
                    else
                    {
                        if (!File.Exists(str_FileFullName))
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete File: " + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                            LogPairMessage(str_PairName, str_LogMsgAddItem, true, true, 4);
                            if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_OutLogMsg))
                            {
                                LogPairMessage(str_PairName, "[FAILED!!!] PAIR-ANALYSIS: " + str_PairName + " DIR2-Soft Delete File: " + str_FileFullName, true, true, 4);
                                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
                                Thread.Sleep(i_SleepInterval);
                            }
                        }
                        else
                        {
                            string str_LogMsgAddItem = "PAIR-ANALYSIS: " + str_PairName + " DIR2-Check File: " + str_FileFullName;
                            OnSetOngoingItem(str_LogMsgAddItem);
                        }
                    }
                }

                LogPairMessage(str_PairName, "PAIR-ANALYSIS: " + str_PairName + " Add 1 analysis done count", true, true, 4);
                OnAdd1Analysis(int_TotalFileFound);
            }
            LogPairMessage(str_PairName, "配对（" + str_PairName + "）的目录2（" + str_Dir2Path + "）分析完成", true, true, 2);
            #endregion

            //当没有出现错误的时候才继续
            if (!bl_ExceptionFound)
            {
                //重置当前操作
                //PairPanal.SetOngoingItem();

                #region 获取配对差异
                //分析结果超过10000，意味着目录里面已经超过2500个项目，分析用时较长，故提醒
                //v2.0.2.1 - 提示“配对相关的目录和文件数量较多，分析差异需时较长，请耐心等待”的阈值调整至SQLITE是100000，ACCESS是40000
                int i_AlertThreshold = cls_Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.SQLITE ? 100000 : 40000;
                if (int_TotalFileFound > i_AlertThreshold)
                {
                    LogPairMessage(str_PairName, "配对相关的目录和文件数量较多，分析差异需时较长，请耐心等待", true, true, 1);
                }
                LogPairMessage(str_PairName, "Started getting DIR/FILE difference", true, true, 4);
                //DataTable dt_fileDiff = cls_Files_InfoDB.GetFileDiff(str_PairName, str_Dir1Path, str_Dir2Path, int_SyncDirection, out str_OutLogMsg);
                DataTable dt_File1InforDB_AfterAnalysis = cls_Files_InfoDB.GetFileInfor(str_Dir1TableName, out str_OutLogMsg);
                if (!String.IsNullOrEmpty(str_OutLogMsg))
                {
                    LogPairMessage(str_PairName, str_OutLogMsg, true, true, 1);
                }
                DataTable dt_File2InforDB_AfterAnalysis = cls_Files_InfoDB.GetFileInfor(str_Dir2TableName, out str_OutLogMsg);
                if (!String.IsNullOrEmpty(str_OutLogMsg))
                {
                    LogPairMessage(str_PairName, str_OutLogMsg, true, true, 1);
                }
                DataTable dt_fileDiff = Get_File_Diff(str_PairID, str_PairName, dt_File1InforDB_AfterAnalysis, dt_File2InforDB_AfterAnalysis, str_Dir1Path, str_Dir2Path, int_SyncDirection, out str_OutLogMsg);
                if (!String.IsNullOrEmpty(str_OutLogMsg))
                {
                    LogPairMessage(str_PairName, str_OutLogMsg, true, true, 1);
                }
                LogPairMessage(str_PairName, "Started getting DIR/FILE difference --- done", true, true, 4);

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
                        bool bl_hitFilterRule = false;

                        #region 排除文件、目录
                        //检查文件是否处于_FSBackup目录，如果存在则跳过
                        if (str_FromFile.Contains(C_StrFSBackup))
                        {
                            LogPairMessage(str_PairName, "Exclude DIR/File: " + str_FromFile + " due to backup folder", true, true, 3);
                            bl_hitFilterRule = true;
                        }

                        //检查文件是否处于排除列表，如果存在则跳过
                        for (int i = 0; i < arr_FilterRule.Length; i++)
                        {
                            if (str_FromFile.Contains(arr_FilterRule[i]))
                            {
                                LogPairMessage(str_PairName, "Exclude DIR/File: " + str_FromFile + " due to filter rule", true, true, 3);
                                bl_hitFilterRule = true;
                                break;
                            }
                        }

                        if (bl_hitFilterRule) continue;
                        #endregion

                        //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 1)
                        {
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileFromPath + " -A-> " + str_FileToPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_FromFile + " -A-> " + str_ToFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 2)
                        {
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileToPath + " <-A- " + str_FileFromPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_ToFile + " <-A- " + str_FromFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 3)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(C_StrDirNameChar)) continue;

                            str_LogMsg = "文件: " + str_FromFile + " -U-> " + str_ToFile;
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 4)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(C_StrDirNameChar)) continue;

                            str_LogMsg = "文件: " + str_ToFile + " <-U- " + str_FromFile;
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除
                        if (int_FileDiffType == 5)
                        {
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileFromPath + " -X-> " + str_FileToPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_FromFile + " -X-> " + str_ToFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除
                        if (int_FileDiffType == 6)
                        {
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_LogMsg = "目录: " + str_FileToPath + " <-X- " + str_FileFromPath;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                            else
                            {
                                str_LogMsg = "文件: " + str_ToFile + " <-X- " + str_FromFile;
                                LogPairMessage(str_PairName, str_LogMsg, true, true, 1);
                            }
                        }
                        //可能出现文件冲突的项目，不做同步，仅发出提醒，DIFFTYPE=7
                        if (int_FileDiffType == 7)
                        {
                            str_LogMsg = "文件<" + str_FileName + ">，目录1<" + str_FileFromPath + ">，目录2<" + str_FileToPath + ">可能存在冲突，请检查";
                            LogPairMessage(str_PairName, str_LogMsg, true, true, 2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogPairMessage(str_PairName, "获取配对差异出错，操作终止，请查阅配对日志文件获取具体信息", true, true, 1);
                    LogPairMessage(str_PairName, ex.Message, true, true, 5, true);
                    return null;
                }
                #endregion

                DataTable dt_fileDiffExType7 = Create_FileDiff_Empty();
                if (dt_fileDiff.Rows.Count > 0)
                {
                    LogPairMessage(str_PairName, "去除差异等级为7的记录", true, true, 3);
                    DataRow[] dr_Temp = dt_fileDiff.Select("DIFFTYPE<>'7'");
                    if (dr_Temp.Length > 0)
                    {
                        dt_fileDiffExType7 = dr_Temp.CopyToDataTable();
                    }
                }

                string str_AnalysisResult = "配对（" + str_PairName + "）分析完成，共发现" + int_TotalFileFound.ToString() + "条记录，";
                if (dt_fileDiffExType7.Rows.Count.Equals(0))
                {
                    str_AnalysisResult += "没有差异";
                }
                else
                {
                    str_AnalysisResult += "其中" + dt_fileDiffExType7.Rows.Count.ToString() + "条记录需同步";
                }
                LogPairMessage(str_PairName, str_AnalysisResult, true, true, 1);

                OnPairStatusChange(PairStatus.FREE);
                OnSetOngoingItem(string.Empty);
                return dt_fileDiffExType7;
            }
            else
            {
                LogPairMessage(str_PairName, "分析过程发生异常被中止，可能是因为配对的目录断开连接，请确认后再试", true, true, 1);
                OnPairStatusChange(PairStatus.FREE);
                OnSetOngoingItem(string.Empty);
                return Create_FileDiff_Empty();
            }
        }

        /// <summary>
        /// 同步文件夹配对
        /// </summary>
        /// <param name="TableFileDiff"></param>
        public void SyncDirPair(DataTable TableFileDiff)
        {
            #region Define Varibles
            LogPairMessage(str_PairName, "开始同步配对（" + str_PairName + "）", true, true, 2);
            OnPairStatusChange(PairStatus.SYNC);
            string[] arr_FilterRule = String.IsNullOrEmpty(str_FilterRule) ? new string[] { } : str_FilterRule.Split(',');
            DirectoryInfo directoryInfo1 = new DirectoryInfo(str_Dir1Path);
            string str_Dir1TableName = str_PairName + "_DIR1_" + directoryInfo1.Name;
            DirectoryInfo directoryInfo2 = new DirectoryInfo(str_Dir2Path);
            string str_Dir2TableName = str_PairName + "_DIR2_" + directoryInfo2.Name;
            int int_TotalChngCount = TableFileDiff.Rows.Count;
            int int_SyncedCount = 0;
            string str_ExceptionFile = String.Empty;
            string str_SyncTimestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMdd_HHmmss");
            bool bExceptionFound = false;
            #endregion

            foreach (DataRow dataRow in TableFileDiff.Rows)
            {
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
                        bool bl_hitFilterRule = false;
                        str_ExceptionFile = "同步" + (str_FileName.Equals(C_StrDirNameChar) ? "目录" : "文件") + "：(" + str_FileName + ")从<" + str_FileFromPath + ">至<" + str_FileToPath + ">发生异常：";
                        #endregion

                        #region 排除文件、目录
                        //检查文件是否处于_FSBackup目录，如果存在则跳过
                        if (str_FromFile.Contains(C_StrFSBackup))
                        {
                            //更新同步进度
                            OnAdd1Sync(int_TotalChngCount);
                            bl_hitFilterRule = true;
                        }

                        //检查文件是否处于排除列表，如果存在则跳过
                        for (int i = 0; i < arr_FilterRule.Length; i++)
                        {
                            if (str_FromFile.Contains(arr_FilterRule[i]))
                            {
                                //更新同步进度
                                OnAdd1Sync(int_TotalChngCount);
                                bl_hitFilterRule = true;
                                break;
                            }
                        }

                        if (bl_hitFilterRule)
                        {
                            int_SyncedCount++;
                            bl_SyncRecordDone = true;
                            FileHelper.DeleteDirectoryOrFile(str_FromFileTemp);
                            LogPairMessage(str_PairName, "PAIR-SYNC: " + str_PairName + " FilterRule-Exclude Item: " + str_FileFromPath + " due to filter rule", true, true, 4);
                            continue;
                        }
                        #endregion

                        #region Process Diff Records
                        //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 1)
                        {
                            //先判断目标目录是否存在，如果不存在则先创建（仅在非调试模式下生效）
                            if (!cls_Global_Settings.DebugMode && !Directory.Exists(str_FileToPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(str_FileToPath);
                                directoryInfo.Create();
                                cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, C_StrDirNameChar, str_FileToPath, "0", String.Empty, directoryInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat), str_PairID, out str_DatabaseErrorMsg);
                            }

                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileFromPath + " -A-> " + str_FileToPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_FromFile + " -A-> " + str_ToFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                if (!cls_Files_InfoDB.AddSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                                }
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                    {
                                        if (File.Exists(str_FromFileTemp))
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                        }
                                        else
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                        }
                                        if (bl_SyncRecordDone)
                                        {
                                            if (!cls_Files_InfoDB.AddFileInfor(str_Dir2TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_DatabaseErrorMsg))
                                            {
                                                LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 1);
                                            }
                                            else
                                            {
                                                if (!cls_Files_InfoDB.UpdSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                                {
                                                    LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        str_OngoingRecMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                        LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 1);
                                        bl_SyncRecordDone = true;
                                    }
                                }
                            }
                        }
                        //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 2)
                        {
                            //先判断目标目录是否存在，如果不存在则先创建（仅在非调试模式下生效）
                            if (!cls_Global_Settings.DebugMode && !Directory.Exists(str_FileToPath))
                            {
                                DirectoryInfo directoryInfo = new DirectoryInfo(str_FileToPath);
                                directoryInfo.Create();
                                cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, C_StrDirNameChar, str_FileToPath, "0", String.Empty, directoryInfo.LastWriteTime.ToString(cls_Files_InfoDB.DBDateTimeFormat), str_PairID, out str_DatabaseErrorMsg);
                            }

                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileToPath + " <-A- " + str_FileFromPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                bl_SyncRecordDone = true;
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_ToFile + " <-A- " + str_FromFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                if (!cls_Files_InfoDB.AddSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                                {
                                    LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                                }
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                    {
                                        if (File.Exists(str_FromFileTemp))
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                        }
                                        else
                                        {
                                            bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                        }
                                        if (bl_SyncRecordDone)
                                        {
                                            if (!cls_Files_InfoDB.AddFileInfor(str_Dir1TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_DatabaseErrorMsg))
                                            {
                                                LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 1);
                                            }
                                            else
                                            {
                                                if (!cls_Files_InfoDB.UpdSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                                {
                                                    LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        str_OngoingRecMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                        LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 1);
                                        bl_SyncRecordDone = true;
                                    }
                                }
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2
                        if (int_FileDiffType == 3)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                bl_SyncRecordDone = true;
                                continue;
                            }

                            str_OngoingRecMsg = "同步文件: " + str_FromFile + " -U-> " + str_ToFile;
                            OnSetOngoingItem(str_OngoingRecMsg);
                            LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                            if (!cls_Files_InfoDB.AddSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                            {
                                LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                            }
                            if (!cls_Global_Settings.DebugMode)
                            {
                                if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                {
                                    if (cls_Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(str_Dir2Path, str_ToFile, false, str_SyncTimestamp);
                                    }
                                    if (File.Exists(str_FromFileTemp))
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                    }
                                    else
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                    }
                                    if (bl_SyncRecordDone)
                                    {
                                        if (!cls_Files_InfoDB.UpdFileInfor(str_Dir2TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_DatabaseErrorMsg))
                                        {
                                            LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 1);
                                        }
                                        else
                                        {
                                            if (!cls_Files_InfoDB.UpdSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                            {
                                                LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    str_OngoingRecMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                    LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 1);
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1
                        if (int_FileDiffType == 4)
                        {
                            //文件夹目录，跳过
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                bl_SyncRecordDone = true;
                                continue;
                            }

                            str_OngoingRecMsg = "同步文件: " + str_ToFile + " <-U- " + str_FromFile;
                            OnSetOngoingItem(str_OngoingRecMsg);
                            LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                            if (!cls_Files_InfoDB.AddSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, false, out str_DatabaseErrorMsg))
                            {
                                LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                            }
                            if (!cls_Global_Settings.DebugMode)
                            {
                                if (File.Exists(str_FromFileTemp) || File.Exists(str_FromFile))
                                {
                                    if (cls_Global_Settings.DelToBackup)
                                    {
                                        MoveFileToBackup(str_Dir1Path, str_ToFile, false, str_SyncTimestamp);
                                    }
                                    if (File.Exists(str_FromFileTemp))
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFileTemp, str_ToFile, str_FromFile != str_FromFileTemp, true);
                                    }
                                    else
                                    {
                                        bl_SyncRecordDone = FileHelper.CopyOrMoveFile(str_FromFile, str_ToFile, false, true);
                                    }
                                    if (bl_SyncRecordDone)
                                    {
                                        if (!cls_Files_InfoDB.UpdFileInfor(str_Dir1TableName, str_FileName, str_FileToPath, str_FileSize, str_FileMD5, str_FileLastModDate, str_PairID, out str_DatabaseErrorMsg))
                                        {
                                            {
                                                LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 1);
                                            }
                                        }
                                        else
                                        {
                                            if (!cls_Files_InfoDB.UpdSyncDetail(str_PairName, str_FromFile, str_ToFile, int_FileDiffType, true, out str_DatabaseErrorMsg))
                                            {
                                                LogPairMessage(str_PairName, str_DatabaseErrorMsg, true, true, 3);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    str_OngoingRecMsg = "文件" + (cls_Global_Settings.UseLocalTemp ? str_FromFileTemp : str_FromFile) + "不存在，同步失败，请检查文件";
                                    LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 1);
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除
                        if (int_FileDiffType == 5)
                        {
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileFromPath + " -X-> " + str_FileToPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (Directory.Exists(str_FileToPath))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir2Path, str_FileToPath, true, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            FileHelper.DeleteDirectoryOrFile(str_FileToPath, true);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_FromFile + " -X-> " + str_ToFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_ToFile))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir2Path, str_ToFile, false, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            FileHelper.DeleteDirectoryOrFile(str_ToFile);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir2TableName, str_FileID, str_PairID, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                        }
                        //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除
                        if (int_FileDiffType == 6)
                        {
                            if (str_FileName.Equals(C_StrDirNameChar))
                            {
                                str_OngoingRecMsg = "同步目录: " + str_FileToPath + " <-X- " + str_FileFromPath;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (Directory.Exists(str_FileToPath))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir1Path, str_FileToPath, true, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            FileHelper.DeleteDirectoryOrFile(str_FileToPath, true);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
                            }
                            else
                            {
                                str_OngoingRecMsg = "同步文件: " + str_ToFile + " <-X- " + str_FromFile;
                                OnSetOngoingItem(str_OngoingRecMsg);
                                LogPairMessage(str_PairName, str_OngoingRecMsg, true, true, 3);
                                if (!cls_Global_Settings.DebugMode)
                                {
                                    if (File.Exists(str_ToFile))
                                    {
                                        if (cls_Global_Settings.DelToBackup)
                                        {
                                            MoveFileToBackup(str_Dir1Path, str_ToFile, false, str_SyncTimestamp);
                                        }
                                        else
                                        {
                                            FileHelper.DeleteDirectoryOrFile(str_ToFile);
                                        }
                                    }
                                    if (!cls_Files_InfoDB.DelFileInforSoft(str_Dir1TableName, str_FileID, str_PairID, out str_DatabaseErrorMsg))
                                    {
                                        LogPairMessage(str_PairName, str_OngoingRecMsg + "失败", true, true, 3);
                                    }
                                    bl_SyncRecordDone = true;
                                }
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
                            LogPairMessage(str_PairName, str_ExceptionFile + str_DatabaseErrorMsg, true, true, 3, true);
                        }
                        //调试模式下强制同步成功
                        if (cls_Global_Settings.DebugMode)
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
                            if (int_TrySyncCount >= cls_Global_Settings.RetryCountWhenSyncFailed)
                            {
                                LogPairMessage(str_PairName, str_ExceptionFile + "超过最大重试次数", true, true, 1);
                                //更新同步进度
                                OnAdd1Sync(int_TotalChngCount);
                                int_SyncedCount++;
                                bl_SyncRecordDone = true;
                                bExceptionFound = true;
                            }
                            else
                            {
                                LogPairMessage(str_PairName, str_ExceptionFile + "等待" + cls_Global_Settings.RetryIntervalWhenSyncFailed.ToString() + "分钟后重试", true, true, 1);
                                Thread.Sleep(cls_Global_Settings.RetryIntervalWhenSyncFailed * 60000);
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    bExceptionFound = true;
                    string str_PrintMsg = str_ExceptionFile + ex.Message;
                    LogPairMessage(str_PairName, str_PrintMsg, true, true, 3, true);
                }
            }

            if (bExceptionFound)
            {
                LogPairMessage(str_PairName, "同步过程发生了一些错误！！！检查日志文件", true, true, 1);
            }

            if (!cls_Global_Settings.DebugMode)
            {
                //彻底删除状态标记为'DL'的文件记录
                LogPairMessage(str_PairName, "Hard Delete DIR/FILE records whose status is 'DL'", true, true, 4);
                cls_Files_InfoDB.DelFileInforAllHard(str_Dir1TableName, str_PairID);
                cls_Files_InfoDB.DelFileInforAllHard(str_Dir2TableName, str_PairID);

                //根据最大保留的backup数调整
                LogPairMessage(str_PairName, "Delete backup DIR according to the Max. count of backup keep", true, true, 4);
                ClearBackupMaxKeep(str_Dir1Path, out str_ExceptionFile);
                ClearBackupMaxKeep(str_Dir2Path, out str_ExceptionFile);
                if (!String.IsNullOrEmpty(str_ExceptionFile))
                {
                    LogPairMessage(str_PairName, str_ExceptionFile, true, true, 5, true);
                }
            }

            if (int_SyncedCount.Equals(int_TotalChngCount))
            {
                LogPairMessage(str_PairName, "配对（" + str_PairName + "）同步完成，共同步了" + int_SyncedCount + "条记录", true, true, 1);
            }
            OnPairStatusChange(PairStatus.FREE);
            OnSetOngoingItem(string.Empty);
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
                        if (!FileHelper.CopyOrMoveFile(str_FileFullName, str_NewFileFullPath, false, true))
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

            if (cls_Global_Settings.UseLocalTemp)
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
                    str_NewFileFullPath = Path.Combine(cls_Global_Settings.LocalTempFolder, str_NewFileName);
                }
            }

            return str_NewFileFullPath;
        }

        private void MoveFileToBackup(string str_RootDirPath, string str_FileFullPath, bool bl_IsDirectory, string str_Timestamp)
        {
            string str_BackupFolder = Path.Combine(str_RootDirPath, C_StrFSBackup);
            string str_BackupFolder_WithTime = Path.Combine(str_BackupFolder, str_Timestamp);
            string str_TargetFilePath = str_FileFullPath.Replace(str_RootDirPath, str_BackupFolder_WithTime);

            if (!Directory.Exists(str_BackupFolder))
            {
                DirectoryInfo _directoryInfo = new DirectoryInfo(str_BackupFolder);
                _directoryInfo.Create();
                _directoryInfo.Attributes = FileAttributes.Hidden;
            }

            if (!bl_IsDirectory)
            {
                FileHelper.CopyOrMoveFile(str_FileFullPath, str_TargetFilePath, true, true);
            }
            else
            {
                FileHelper.CopyOrMoveDirectory(str_FileFullPath, str_TargetFilePath, true, true);
            }
        }

        private void ClearBackupMaxKeep(string str_RootDirPath, out string str_ErrorMsg)
        {
            str_ErrorMsg = String.Empty;
            string str_BackupFolder = Path.Combine(str_RootDirPath, C_StrFSBackup);

            try
            {
                DirectoryInfo[] directories = (new DirectoryInfo(str_BackupFolder).GetDirectories("*", SearchOption.TopDirectoryOnly)).OrderByDescending(f => f.Name).ToArray();
                for (int i = 0; i < directories.Length; i++)
                {
                    int i_CurrentIdx = i + 1;
                    if (i_CurrentIdx > cls_Global_Settings.MaxKeepBackup)
                    {
                        FileHelper.DeleteDirectoryOrFile(directories[i].FullName, true);
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
            bool bRet = cls_Files_InfoDB.UpdatePairSyncStatus(str_PairID, strDateTime, SyncSuccessfulIndc, out str_OutLogMsg);
            if (!String.IsNullOrEmpty(str_OutLogMsg))
            {
                LogPairMessage(str_PairName, str_OutLogMsg, true, true, 5, true);
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
            string str_LogMsgToFile = (IsAddTS ? DateTime.Now.ToString(cls_Files_InfoDB.DBDateTimeFormat) + " --- " : String.Empty) + LogMessage;
            //string str_LogMsgChngLine = str_LogMsgToFile + (IsChangeLine ? "\n" : "");
            bool bl_HasWrittenFile = IsALwaysLogFile;

            if (IsALwaysLogFile)
            {
                cls_PairLogFile.LogMessage(str_LogMsgToFile);
                bl_HasWrittenFile = true;
            }

            try
            {
                //输入的MsgTraceLevel是0，则属于顶级日志，直接处理
                if (MsgTraceLevel == 0)
                {
                    //((ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0]).LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
                    OnLogPairMsg(LogMessage, IsChangeLine, IsAddTS);
                    if (!bl_HasWrittenFile)
                    {
                        cls_PairLogFile.LogMessage(str_LogMsgToFile);
                    }
                }
                //输入的MsgTraceLevel > 0，则对比全局变量设置的日志等级做判断处理
                else if (MsgTraceLevel > 0 && MsgTraceLevel <= cls_Global_Settings.TraceLevel)
                {
                    //((ctrl_PairPanal)tabControl1.TabPages[PairName].Controls[0]).LogPairMessage(LogMessage, IsChangeLine, IsAddTS);
                    OnLogPairMsg(LogMessage, IsChangeLine, IsAddTS);
                    if (cls_Global_Settings.LogMessageToFile && !bl_HasWrittenFile)
                    {
                        cls_PairLogFile.LogMessage(str_LogMsgToFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                LogPairMessage(PairName, ex.Message, true, true, 5);
            }
        }

        /// <summary>
        /// 打开此配对的日志文件
        /// </summary>
        public void OpenPairLog()
        {
            cls_PairLogFile.OpenPairLog();
        }

        /// <summary>
        /// 暂停此配对的自动同步
        /// </summary>
        /// <param name="WarningMsg"></param>
        /// <returns></returns>
        public bool PausePairAutoSync()
        {
            string WarningMsg = string.Empty;
            bool bRet = cls_Files_InfoDB.PausePairAutoSync(str_PairID, out WarningMsg);
            if (!bRet)
            {
                LogPairMessage(str_PairName, WarningMsg, true, true, 5, true);
            }
            return bRet;
        }

        private int CalculateAnalysisInterval()
        {
            return 0;
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
            ps_PairStatus = pairStatus;
            if (PairStatusChange != null)
            {
                PairStatusChange(this, ps_PairStatus);
            }
        }
        #endregion
    }
}
