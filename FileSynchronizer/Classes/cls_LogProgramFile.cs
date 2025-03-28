using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public static class cls_LogProgramFile
    {
        static string str_LogFile = Path.Combine(Application.StartupPath, @"FileSynchronizer.log");
        private static bool bl_LogToCache;
        public static bool LogToCache { get => bl_LogToCache; set => bl_LogToCache = value; }
        static string str_CacheMsg;

        /// <summary>
        /// 初始化日志文件
        /// </summary>
        /// <param name="bl_ClearLog">此动作是否为重置日志文件</param>
        /// <returns></returns>
        public static bool InitLog(bool bl_ClearLog = false)
        {
            bl_LogToCache = false;
            if (!File.Exists(str_LogFile) || bl_ClearLog)
            {
                FileStream fs_NewLogFile = new FileStream(str_LogFile, FileMode.Create, FileAccess.Write);
                fs_NewLogFile.Close();
                fs_NewLogFile.Dispose();
                return true;
            }
            return false;
        }

        public static void LogMessage(string str_Message)
        {
            //if (!cls_FileHelper.IsFileOpen(str_LogFile))
            //{
            //    StreamWriter wr = new StreamWriter(str_LogFile, true, Encoding.UTF8);
            //    Task task = wr.WriteLineAsync(str_Message);
            //    //task.Start();
            //    wr.Close();
            //}
            //else
            //{
            //while (!cls_FileHelper.IsFileOpen(str_LogFile))

            //    {
            //        StreamWriter wr = new StreamWriter(str_LogFile, true, Encoding.UTF8);
            //        Task task = wr.WriteLineAsync(str_Message);
            //        //task.Start();
            //        wr.Close();
            //        break;
            //    }
            ////}

            //开始写入
            //fs_GlobalStream.WriteAsync(data, 0, data.Length, new System.Threading.CancellationToken());
            //清空缓冲区、关闭流
            //fs_GlobalStream.FlushAsync();
            //fs_GlobalStream.Close();

            if (bl_LogToCache)
            {
                str_CacheMsg += str_Message;
                if (str_CacheMsg.Length > 600)
                {
                    LogMsgFromCacheToFile();
                }
                else
                {
                    str_CacheMsg += "\n";
                }
            }
            else
            {
                StreamWriter wr = new StreamWriter(str_LogFile, true, Encoding.UTF8);
                wr.WriteLine(str_Message);
                wr.Close();
            }
        }

        public static bool IsLogFileInUse()
        {
            bool inUse = true;

            FileStream fs = null;
            try
            {

                fs = new FileStream(str_LogFile, FileMode.Open, FileAccess.Read,

                FileShare.None);

                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)

                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用
        }

        public static void LogMsgFromCacheToFile()
        {
            try
            {
                if (!String.IsNullOrEmpty(str_CacheMsg))
                {
                    StreamWriter wr = new StreamWriter(str_LogFile, true, Encoding.UTF8);
                    wr.WriteLine(str_CacheMsg);
                    wr.Close();
                    str_CacheMsg = String.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error when logging to file");
                return;
            }
        }

        public static string LogFileFullName()
        {
            return str_LogFile;
        }

        /// <summary>
        /// 打开日志
        /// </summary>
        public static void OpenProgramLog()
        {
            System.Diagnostics.Process.Start("notepad.exe", str_LogFile);
        }
    }
}
