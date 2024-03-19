using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public class cls_LogPairFile
    {
        private string str_LogFile = String.Empty;
        private string str_FileName = String.Empty;

        /// <summary>
        /// 创建一个新的配对日志记录类
        /// </summary>
        /// <param name="PairName">配对名</param>
        /// <param name="IsClearLog">是否清除所有配对日志</param>
        public cls_LogPairFile(string PairName, bool IsClearLog)
        {
            str_FileName = PairName;
            str_LogFile = Path.Combine(Application.StartupPath, @"FileSynchronizer_" + str_FileName + ".log");
            InitLog(IsClearLog);
        }

        /// <summary>
        /// 初始化日志文件
        /// </summary>
        /// <param name="bl_ClearLog">此动作是否为重置日志文件</param>
        /// <returns></returns>
        public bool InitLog(bool IsClearLog)
        {
            if (!File.Exists(str_LogFile) || IsClearLog)
            {
                FileStream fs_NewLogFile = new FileStream(str_LogFile, FileMode.Create, FileAccess.Write);
                fs_NewLogFile.Close();
                fs_NewLogFile.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 记录配对日志到文件
        /// </summary>
        /// <param name="Message">日志消息</param>
        public void LogMessage(string Message)
        {
            StreamWriter wr = new StreamWriter(str_LogFile, true, Encoding.UTF8);
            wr.WriteLine(Message);
            wr.Close();
        }

        public bool IsLogFileInUse()
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

        public string LogFileFullName()
        {
            return str_LogFile;
        }

        /// <summary>
        /// 打开日志
        /// </summary>
        public void OpenPairLog()
        {
            str_LogFile = Path.Combine(Application.StartupPath, @"FileSynchronizer_" + str_FileName + ".log");
            System.Diagnostics.Process.Start("notepad.exe", str_LogFile);
        }
    }
}
