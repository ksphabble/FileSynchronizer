using Common.Components;
using D2Phap.FileWatcherEx;
using System;
using System.IO;
using System.Threading.Tasks;
using static FileSynchronizer.Local_Utilities;

namespace FileSynchronizer
{
    internal class cls_Files_Info_Helper
    {
        #region 变量和构造函数
        private string g_sPairID;
        private string g_sPairName;
        private string g_sDir1Path;
        private string g_sDir2Path;
        private string g_sFilterRule;
        private DirectoryInfo g_dir1;
        private DirectoryInfo g_dir2;
        private DirectoryInfo[] g_subDir1;
        private DirectoryInfo[] g_subDir2;
        private FileInfo[] g_fileInfos1;
        private FileInfo[] g_fileInfos2;
        private FileSystemWatcherEx fw_Dir1;
        private FileSystemWatcherEx fw_Dir2;

        public cls_Files_Info_Helper(string sPairID, string sPairName, string sDirPath1, string sDirPath2, string sFilterRule)
        {
            g_sPairID = sPairID;
            g_sPairName = sPairName;
            g_sDir1Path = sDirPath1;
            g_sDir2Path = sDirPath2;
            g_sFilterRule = sFilterRule;
            g_dir1 = new DirectoryInfo(sDirPath1);
            g_dir2 = new DirectoryInfo(sDirPath2);
            InitFileWatchers();
            Task.Run(() => LoadObjects());
        }
        #endregion

        #region 类的事件处理
        public delegate void Dir1ObjectChangedHandler(object sender, FileChangedEvent e);
        public event Dir1ObjectChangedHandler Dir1ObjectChanged;
        public delegate void Dir2ObjectChangedHandler(object sender, FileChangedEvent e);
        public event Dir2ObjectChangedHandler Dir2ObjectChanged;
        public delegate void FileWatcherInitDoneHandler(object sender);
        public event FileWatcherInitDoneHandler FileWatcherInitDone;
        public delegate void ObjectsInforReadyHandler(object sender);
        public event ObjectsInforReadyHandler ObjectsInforReady;

        protected virtual void OnDir1ObjectChanged(FileChangedEvent e)
        {
            if (Dir1ObjectChanged != null)
            {
                Dir1ObjectChanged(this, e);
            }
        }

        protected virtual void OnDir2ObjectChanged(FileChangedEvent e)
        {
            if (Dir2ObjectChanged != null)
            {
                Dir2ObjectChanged(this, e);
            }
        }

        protected virtual void OnFileWatcherInitDone()
        {
            if (FileWatcherInitDone != null)
            {
                FileWatcherInitDone(this);
            }
        }

        protected virtual void OnObjectsInforReady()
        {
            if (ObjectsInforReady != null)
            {
                ObjectsInforReady(this);
            }
        }
        #endregion

        #region 私有方法
        private async void LoadObjects()
        {
            //lock (this)
            //{
            //Thread thread1 = new Thread(new ParameterizedThreadStart(LoadObjects1));
            //thread1.IsBackground = true;
            //thread1.Start(2);
            //Thread thread2 = new Thread(new ParameterizedThreadStart(LoadObjects2));
            //thread2.IsBackground = true;
            //thread2.Start(2);

            await Task.Run(() => LoadObjects1(2));
            await Task.Run(() => LoadObjects2(2));

            bool bObjectInfoReady = false;
            while (!bObjectInfoReady)
            {
                bObjectInfoReady = ObjectInfoReady();
                if (bObjectInfoReady)
                {
                    break;
                }
                //}
            }
            OnObjectsInforReady();
        }

        private void InitFileWatchers()
        {
            Task task = Task.Run(() =>
            {
                fw_Dir1 = new FileSystemWatcherEx(g_sDir1Path);
                fw_Dir1.IncludeSubdirectories = true;
                fw_Dir1.OnCreated += Fw_Dir1_ObjectChanged;
                fw_Dir1.OnChanged += Fw_Dir1_ObjectChanged;
                fw_Dir1.OnDeleted += Fw_Dir1_ObjectChanged;
                fw_Dir1.OnRenamed += Fw_Dir1_ObjectChanged;
                //fw_Dir1.SynchronizingObject = this;
                fw_Dir1.Start();

                fw_Dir2 = new FileSystemWatcherEx(g_sDir2Path);
                fw_Dir2.IncludeSubdirectories = true;
                fw_Dir2.OnCreated += Fw_Dir2_ObjectChanged;
                fw_Dir2.OnChanged += Fw_Dir2_ObjectChanged;
                fw_Dir2.OnDeleted += Fw_Dir2_ObjectChanged;
                fw_Dir2.OnRenamed += Fw_Dir2_ObjectChanged;
                //fw_Dir2.SynchronizingObject = this;
                fw_Dir2.Start();

                OnFileWatcherInitDone();
            });
        }

        private void LoadObjects1(object iType)
        {
            try
            {
                int i_Type = (int)iType;
                if (i_Type == 0 || i_Type == 2)
                {
                    //DirectoryInfo[] diTemp = g_dir1.GetDirectories("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    DirectoryInfo[] diTemp = g_dir1.GetDirectories("*", SearchOption.AllDirectories);
                    g_subDir1 = diTemp;
                }
                if (i_Type == 1 || i_Type == 2)
                {
                    //FileInfo[] fiTemp = g_dir1.GetFiles("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    FileInfo[] fiTemp = g_dir1.GetFiles("*", SearchOption.AllDirectories);
                    g_fileInfos1 = fiTemp;
                }
            }
            catch
            {

            }
        }

        private void LoadObjects2(object iType)
        {
            try
            {
                int i_Type = (int)iType;
                if (i_Type == 0 || i_Type == 2)
                {
                    //DirectoryInfo[] diTemp = g_dir2.GetDirectories("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    DirectoryInfo[] diTemp = g_dir2.GetDirectories("*", SearchOption.AllDirectories);
                    g_subDir2 = diTemp;
                }
                if (i_Type == 1 || i_Type == 2)
                {
                    //FileInfo[] fiTemp = g_dir2.GetFiles("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    FileInfo[] fiTemp = g_dir2.GetFiles("*", SearchOption.AllDirectories);
                    g_fileInfos2 = fiTemp;
                }
            }
            catch
            {

            }
        }

        private async void Fw_Dir1_ObjectChanged(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;
            //Thread thread = new Thread(new ParameterizedThreadStart(LoadObjects1));
            //thread.IsBackground = true;
            //thread.Start(i_Type >= 0 ? i_Type : 2);
            await Task.Run(() => LoadObjects1(i_Type >= 0 ? i_Type : 2));

            OnDir1ObjectChanged(e);
        }

        private async void Fw_Dir2_ObjectChanged(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_OutLogMsg = String.Empty;
            string str_FullPath = e.FullPath;
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (CheckFilterRule(g_sFilterRule, str_FullPath, out str_OutLogMsg)) return;
            //Thread thread = new Thread(new ParameterizedThreadStart(LoadObjects2));
            //thread.IsBackground = true;
            //thread.Start(i_Type >= 0 ? i_Type : 2);
            await Task.Run(() => LoadObjects2(i_Type >= 0 ? i_Type : 2));

            OnDir2ObjectChanged(e);
        }
        #endregion

        #region 公有方法
        public DirectoryInfo[] GetDirectoryInfos1()
        {
            return g_subDir1;
        }

        public DirectoryInfo[] GetDirectoryInfos2()
        {
            return g_subDir2;
        }

        public FileInfo[] GetFileInfos1()
        {
            return g_fileInfos1;
        }

        public FileInfo[] GetFileInfos2()
        {
            return g_fileInfos2;
        }

        public bool ObjectInfoReady()
        {
            bool bDir1Null = g_subDir1 == null;
            bool bDir2Null = g_subDir2 == null;
            bool bFile1Null = g_fileInfos1 == null;
            bool bFile2Null = g_fileInfos2 == null;
            return !(bDir1Null || bDir2Null || bFile1Null || bFile2Null);
        }

        public int TotalObjectCount()
        {
            if (!ObjectInfoReady())
            {
                return 0;
            }
            else
            {
                return g_subDir1.Length + g_subDir2.Length + g_fileInfos1.Length + g_fileInfos2.Length;
            }
        }
        #endregion
    }
}
