using Common.Components;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FileSynchronizer
{
    public partial class frm_GlobalSettings : Form
    {
        public frm_GlobalSettings(string str_MainPgmVer)
        {
            InitializeComponent();
            lblPGMVer.Text = str_MainPgmVer;
        }

        private void frm_GlobalSettings_Load(object sender, EventArgs e)
        {
            cls_Global_Settings.Init_Settings();
            Fill_Setting();
            chkboxLogMsgToFile.Text = "保存日志到文件：" + FileHelper.CalcFileSizeStr(cls_LogProgramFile.LogFileFullName());
            if (cls_Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.SQLITE)
            {
                lblCurrentDB.Text = lblCurrentDB.Text + @"SQLITE";
            }
            else if (cls_Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.ACCESS)
            {
                lblCurrentDB.Text = lblCurrentDB.Text + @"ACCESS";
            }
            else
            {
                lblCurrentDB.Text = lblCurrentDB.Text + @"!NONE!";
            }
        }

        private void Fill_Setting()
        {
            chkboxDebugMode.Checked = cls_Global_Settings.DebugMode;
            chkboxLogMsgToFile.Checked = cls_Global_Settings.LogMessageToFile;
            lblDBVer.Text = cls_Global_Settings.DBVersion;
            txtboxTraceLevel.Text = cls_Global_Settings.TraceLevel.ToString();
            chkboxDeleteToBackup.Checked = cls_Global_Settings.DelToBackup;
            chkboxAutoRun.Checked = cls_Global_Settings.AutoRun;
            txtboxRetrySyncInterval.Text = cls_Global_Settings.RetryIntervalWhenSyncFailed.ToString();
            txtboxRetrySyncCount.Text = cls_Global_Settings.RetryCountWhenSyncFailed.ToString();
            chkboxUseLocalTemp.Checked = cls_Global_Settings.UseLocalTemp;
            txtboxLocalTempFolder.Text = cls_Global_Settings.LocalTempFolder;
            chkBoxAutoClearLog.Checked = cls_Global_Settings.AutoClearLog;
            chkBoxMinStart.Checked = cls_Global_Settings.MinWhenStart;
            txtboxMaxKeepBackup.Text = cls_Global_Settings.MaxKeepBackup.ToString();

            //调试模式下的功能
            pnlDebugTools.Visible = cls_Global_Settings.DebugMode;
            if (cls_Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.ACCESS)
            {
                comboxDBMigration.SelectedIndex = 0;
            }
            else if (cls_Files_InfoDB.DBType == cls_SQLBuilder.DATABASE_TYPE.SQLITE)
            {
                comboxDBMigration.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int int_tempTraceLevel = 1;
            int int_tempRetrySyncInterval = 0;
            int int_tempRetrySyncCount = 0;
            int int_tempMaxKeepBackup = 0;
            if (!Int32.TryParse(txtboxTraceLevel.Text, out int_tempTraceLevel))
            {
                MessageBox.Show("日志等级设置错误，请输入数字！", "信息");
                return;
            }
            if (int_tempTraceLevel < 1 || int_tempTraceLevel > 5)
            {
                MessageBox.Show("日志等级设置错误，请输入1~5！", "信息");
                return;
            }
            if (!Int32.TryParse(txtboxRetrySyncInterval.Text, out int_tempRetrySyncInterval))
            {
                MessageBox.Show("重试等待时间设置错误，请输入数字！", "信息");
                return;
            }
            if (!Int32.TryParse(txtboxRetrySyncCount.Text, out int_tempRetrySyncCount))
            {
                MessageBox.Show("重试次数设置错误，请输入数字！", "信息");
                return;
            }
            if (!Int32.TryParse(txtboxMaxKeepBackup.Text, out int_tempMaxKeepBackup))
            {
                MessageBox.Show("最大保留备份数量设置错误，请输入数字！", "信息");
                return;
            }

            cls_Global_Settings.DebugMode = chkboxDebugMode.Checked;
            cls_Global_Settings.LogMessageToFile = chkboxLogMsgToFile.Checked;
            cls_Global_Settings.TraceLevel = int_tempTraceLevel;
            cls_Global_Settings.DelToBackup = chkboxDeleteToBackup.Checked;
            cls_Global_Settings.AutoRun = chkboxAutoRun.Checked;
            cls_Global_Settings.RetryIntervalWhenSyncFailed = int_tempRetrySyncInterval;
            cls_Global_Settings.RetryCountWhenSyncFailed = int_tempRetrySyncCount;
            cls_Global_Settings.UseLocalTemp = chkboxUseLocalTemp.Checked;
            cls_Global_Settings.LocalTempFolder = txtboxLocalTempFolder.Text;
            cls_Global_Settings.AutoClearLog = chkBoxAutoClearLog.Checked;
            cls_Global_Settings.MinWhenStart = chkBoxMinStart.Checked;
            cls_Global_Settings.MaxKeepBackup = int_tempMaxKeepBackup;

            cls_Global_Settings.SaveInfoToDB();
            MessageBox.Show("信息已保存！", "信息");
            Fill_Setting();

            //设置程序自启动
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (chkboxAutoRun.Checked)
            {
                registryKey.SetValue("FileSynchronizer", Application.ExecutablePath);
            }
            else
            {
                registryKey.DeleteValue("FileSynchronizer", false);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (cls_Files_InfoDB.BackupDBFile(lblPGMVer.Text))
            {
                MessageBox.Show("备份数据库成功！", "提示");
            }
            else
            {
                MessageBox.Show("备份数据库失败！", "错误");
            }
        }

        /// <summary>
        /// 修复文件夹配对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFixPair_Click(object sender, EventArgs e)
        {
            cls_Files_InfoDB.FixDirPairStatus();
            MessageBox.Show("所有文件夹配对已修复！", "信息");
        }

        /// <summary>
        /// 清理日志文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearLogFile_Click(object sender, EventArgs e)
        {
            cls_LogProgramFile.InitLog(true);
            chkboxLogMsgToFile.Text = "保存日志到文件 (日志文件大小：" + FileHelper.CalcFileSizeStr(cls_LogProgramFile.LogFileFullName()) + ")";
        }

        /// <summary>
        /// 开始数据库迁移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartDBMigration_Click(object sender, EventArgs e)
        {
            if (!cls_Files_InfoDB.BackupDBFile(lblPGMVer.Text))
            {
                MessageBox.Show("备份数据库失败，不能继续数据库迁移！", "错误");
                return;
            }

            bool bl_MigrateResult = false;
            string str_OutMsg = String.Empty;

            //To SQLITE
            if (comboxDBMigration.SelectedIndex == 0)
            {
                bl_MigrateResult = cls_Files_InfoDB.DBMigration(cls_SQLBuilder.DATABASE_TYPE.SQLITE, out str_OutMsg);
            }
            //To ACCESS
            else if (comboxDBMigration.SelectedIndex == 1)
            {
                bl_MigrateResult = cls_Files_InfoDB.DBMigration(cls_SQLBuilder.DATABASE_TYPE.ACCESS, out str_OutMsg);
            }

            if (bl_MigrateResult)
            {
                MessageBox.Show("数据库迁移成功，程序即将关闭，请重新启动程序以启用新数据库！", "提示");
                bool bl_DelCurrentDB = cls_Files_InfoDB.DeleteDBFile();
                if (!bl_DelCurrentDB)
                {
                    MessageBox.Show("删除当前正在使用的数据库失败，请手动删除以启用新数据库！", "提示");
                }
                Application.Exit();
            }
            else
            {
                MessageBox.Show("数据库迁移失败！原因：" + str_OutMsg, "错误");
            }
        }

        private void btnSelectLocalTempFolder_Click(object sender, EventArgs e)
        {
            string str_LocalTempFolder = String.Empty;
            if (cls_Global_Settings.LocalTempFolder.Equals(@".\_FSTemp"))
            {
                str_LocalTempFolder = Path.Combine(Application.StartupPath, @"_FSTemp");
                if (!Directory.Exists(str_LocalTempFolder))
                {
                    DirectoryInfo _directoryInfo = new DirectoryInfo(str_LocalTempFolder);
                    _directoryInfo.Create();
                    _directoryInfo.Attributes = FileAttributes.Hidden;
                }
            }
            else
            {
                str_LocalTempFolder = cls_Global_Settings.LocalTempFolder;
            }

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = str_LocalTempFolder;
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.ShowDialog();
            txtboxLocalTempFolder.Text = folderBrowserDialog.SelectedPath;
        }

        private void chkboxUseLocalTemp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkboxUseLocalTemp.Checked)
            {
                string str_LocalTempFolder = cls_Global_Settings.LocalTempFolder;
                if (!Directory.Exists(str_LocalTempFolder))
                {
                    DirectoryInfo _directoryInfo = new DirectoryInfo(str_LocalTempFolder);
                    _directoryInfo.Create();
                    _directoryInfo.Attributes = FileAttributes.Hidden;
                }
            }
        }
    }
}
