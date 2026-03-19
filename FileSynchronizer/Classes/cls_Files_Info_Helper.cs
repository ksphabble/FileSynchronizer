using Common.Components;
using D2Phap.FileWatcherEx;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    internal class cls_Files_Info_Helper
    {
        #region 变量和构造函数
        private string g_sPairID;
        private string g_sPairName;
        private string g_sDir1Path;
        private string g_sDir2Path;
        private DirectoryInfo g_dir1;
        private DirectoryInfo g_dir2;
        private DirectoryInfo[] g_subDir1;
        private DirectoryInfo[] g_subDir2;
        private FileInfo[] g_fileInfos1;
        private FileInfo[] g_fileInfos2;
        private FileSystemWatcherEx fw_Dir1;
        private FileSystemWatcherEx fw_Dir2;

        public cls_Files_Info_Helper(string sPairID, string sPairName, string sDirPath1, string sDirPath2)
        {
            g_sPairID = sPairID;
            g_sPairName = sPairName;
            g_sDir1Path = sDirPath1;
            g_sDir2Path = sDirPath2;
            g_dir1 = new DirectoryInfo(sDirPath1);
            g_dir2 = new DirectoryInfo(sDirPath2);
            InitFileWatchers();
            var task = Task.Factory.StartNew(() => LoadObjects());
        }
        #endregion

        #region 类的事件处理
        public delegate void FileListChangedHandler(object sender);
        public event FileListChangedHandler FileListChanged;
        public delegate void FileWatcherInitDoneHandler(object sender);
        public event FileWatcherInitDoneHandler FileWatcherInitDone;
        public delegate void ObjectsInforReadyHandler(object sender);
        public event ObjectsInforReadyHandler ObjectsInforReady;

        protected virtual void OnFileListChanged()
        {
            if (FileListChanged != null)
            {
                FileListChanged(this);
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
        private async Task LoadObjects()
        {
            Thread thread1 = new Thread(new ParameterizedThreadStart(LoadObjects1));
            thread1.Start(2);
            Thread thread2 = new Thread(new ParameterizedThreadStart(LoadObjects2));
            thread2.Start(2);

            bool bObjectInfoReady = false;
            while (!bObjectInfoReady)
            {
                bObjectInfoReady = ObjectInfoReady();
                if (bObjectInfoReady)
                {
                    break;
                }
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
                    DirectoryInfo[] diTemp = g_dir1.GetDirectories("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    g_subDir1 = diTemp;
                }
                if (i_Type == 1 || i_Type == 2)
                {
                    FileInfo[] fiTemp = g_dir1.GetFiles("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
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
                    DirectoryInfo[] diTemp = g_dir2.GetDirectories("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    g_subDir2 = diTemp;
                }
                if (i_Type == 1 || i_Type == 2)
                {
                    FileInfo[] fiTemp = g_dir2.GetFiles("*", SearchOption.AllDirectories).Where(d => !d.FullName.Contains(Local_Utilities.c_FSBackup_Str)).ToArray();
                    g_fileInfos2 = fiTemp;
                }
            }
            catch
            {

            }
        }

        private void Fw_Dir1_ObjectChanged(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_FullPath = e.FullPath;
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (str_FullPath.Contains(Local_Utilities.c_FSBackup_Str))
            {
                return;
            }

            if (i_Type == -1)
            {
                i_Type = 2;
            }

            Thread thread = new Thread(new ParameterizedThreadStart(LoadObjects1));
            thread.Start(i_Type);

            OnFileListChanged();
        }

        private void Fw_Dir2_ObjectChanged(object sender, FileChangedEvent e)
        {
            int i_Type;
            string str_FullPath = e.FullPath;
            var obj_ChangedItem = FileHelper.ObjFromFullPath(str_FullPath, out i_Type);
            if (str_FullPath.Contains(Local_Utilities.c_FSBackup_Str))
            {
                return;
            }

            if (i_Type == -1)
            {
                i_Type = 2;
            }

            Thread thread = new Thread(new ParameterizedThreadStart(LoadObjects2));
            thread.Start(i_Type);

            OnFileListChanged();
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
        #endregion
    }
}
