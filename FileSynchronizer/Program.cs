using System;
using System.Threading;
using System.Windows.Forms;

namespace FileSynchronizer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bl_AppRunning = false;
            Mutex _mutex = new Mutex(true, System.Diagnostics.Process.GetCurrentProcess().ProcessName, out bl_AppRunning);
            if (!bl_AppRunning)
            {
                Environment.Exit(1);
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frm_FileSynchronizer());
            }
            catch (Exception ex)
            {
                cls_LogProgramFile.LogMessage(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
