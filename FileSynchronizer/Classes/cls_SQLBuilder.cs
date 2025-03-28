using System;

namespace FileSynchronizer
{
    /// <summary>
    /// SQL语句的创建类
    /// </summary>
    public class cls_SQLBuilder
    {
        public enum DATABASE_TYPE : Int32
        {
            NONE = 0,
            ACCESS = 1,
            SQLITE = 2
        }
        private DATABASE_TYPE m_DBType;

        #region Basic DB Methods
        public cls_SQLBuilder(DATABASE_TYPE DBType)
        {
            m_DBType= DBType;
        }

        public string SQL_BuildFileTableCre(string str_TableName)
        {
            string sql_out = @"Create table " + str_TableName + @" (PK_FileID ";
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out += @"AUTOINCREMENT PRIMARY KEY";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out += @"INTEGER PRIMARY KEY";
            }
            sql_out += @",FileName TEXT(255) NOT NULL,FilePath TEXT(255) NOT NULL,FileSize DOUBLE,FileMD5 TEXT(32) NOT NULL,FileLastModDate ";
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out += @"DATETIME";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out += @"VARCHAR";
            }
            sql_out += @",PAIRID INTEGER NOT NULL,FileStatus TEXT(2))";
            return sql_out;
        }

        public string SQL_BuildFileTableIdx(string str_TableName)
        {
            string sql_out = @"CREATE INDEX idxFileID ON " + str_TableName + @" (PK_FileID)";
            return sql_out;
        }

        public string SQL_DeleteFileTable(string str_TableName)
        {
            string sql_out = @"DROP table " + str_TableName;
            return sql_out;
        }

        public string SQL_BuildDirPairTableCre()
        {
            string sql_out = @"Create table DIRPAIR (PK_PairID ";
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out += @"AUTOINCREMENT PRIMARY KEY";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out += @"INTEGER PRIMARY KEY";
            }
            sql_out += @",PAIRNAME TEXT(50) NOT NULL UNIQUE,DIR1 TEXT(255) NOT NULL,DIR2 TEXT(255) NOT NULL,LastSyncDT ";
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out += @"DATETIME";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out += @"VARCHAR";
            }
            sql_out += @",LastSyncStatus BIT,FilterRule TEXT(255),AutoSyncInterval INTEGER,SyncDirection INTEGER,IsPaused BIT)";
            return sql_out;
        }

        public string SQL_BuildDirPairTableIdx()
        {
            string sql_out = @"CREATE UNIQUE INDEX idxPairID ON DIRPAIR (PK_PairID) WITH DISALLOW NULL";
            return sql_out;
        }

        public string SQL_BuildGlobalSettingTable()
        {
            string sql_out = @"Create table Global_Settings (Setting_Name TEXT(255) NOT NULL PRIMARY KEY,Setting_Value TEXT(255))";
            return sql_out;
        }

        public string SQL_CheckDBUpgradeSel(string str_TargetVersion)
        {
            string sql_out = @"select Setting_Value from Global_Settings where Setting_Name='DBVersion'";
            return sql_out;
        }

        public string SQL_CheckDBUpgradeUpd(string str_TargetVersion)
        {
            string sql_out = @"update Global_Settings set Setting_Value = '" + str_TargetVersion + @"' where Setting_Name='DBVersion'";
            return sql_out;
        }

        public string SQL_BuildSyncDetailTableCre()
        {
            string sql_out = @"Create table SyncDetail (PK_SyncDetail ";
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out += @"AUTOINCREMENT PRIMARY KEY";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out += @"INTEGER PRIMARY KEY";
            }
            sql_out += @",PAIRNAME TEXT(50) NOT NULL,FromFile TEXT(255),ToFile TEXT(255),FileDiffType INTEGER,SyncStatus BIT)";
            return sql_out;
        }

        public string SQL_GetAllTableName()
        {
            string sql_out = String.Empty;
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out = @"Select name FROM MSysObjects WHERE type=1 and flags=0 ORDER BY name;";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out = @"Select name FROM sqlite_master WHERE type='table' ORDER BY name";
            }
            return sql_out;
        }
        #endregion

        #region Global_Settings Methods
        /// <summary>
        /// 删除所有现有的设置
        /// </summary>
        public string SQL_DelAllGlobalSettings()
        {
            string sql_out = @"Delete from Global_Settings";
            return sql_out;
        }

        /// <summary>
        /// 添加或者修改设置
        /// </summary>
        /// <param name="str_Name">设置名称</param>
        /// <param name="str_Value">设置值</param>
        /// <param name="bl_IsUnique">该设置是否为唯一，如果是，则修改，否则添加</param>
        /// <returns></returns>
        public string SQL_AddorUpdGlobalSettingSel(string str_Name, string str_Value)
        {
            string sql_out = @"select * from Global_Settings where Setting_Name='" + str_Name + "'";
            return sql_out;
        }

        public string SQL_AddorUpdGlobalSettingIns(string str_Name, string str_Value)
        {
            string sql_out = @"Insert Into Global_Settings values('" + str_Name + "','" + str_Value + "')";
            return sql_out;
        }

        public string SQL_AddorUpdGlobalSettingUpd(string str_Name, string str_Value)
        {
            string sql_out = @"update Global_Settings set Setting_Value = '" + str_Value + "' where Setting_Name='" + str_Name + "'";
            return sql_out;
        }

        public string SQL_SelectAllGlobalSettings()
        {
            string sql_out = @"select * from Global_Settings";
            return sql_out;
        }
        #endregion

        #region DIRPAIR Infor Methods
        public string SQL_GetDirPairInfor(string str_PairName)
        {
            string sql_out = @"select * from DIRPAIR ";
            if (!String.IsNullOrEmpty(str_PairName))
            {
                sql_out += @"where PAIRNAME='" + str_PairName + "'";
            }
            sql_out += @" order by PK_PairID ASC";
            return sql_out;
        }

        public string SQL_AddDirPairEnq(string str_PairName, string str_Dir1_Path, string str_Dir2_Path, string str_FilterRule, string str_SyncInterval, string str_SyncDirection)
        {
            string sql_out = @"select * from DIRPAIR where PAIRNAME='" + str_PairName + "' AND DIR1='" + str_Dir1_Path + "' AND DIR2='" + str_Dir2_Path + "'";
            return sql_out;
        }

        public string SQL_AddDirPairIns(string str_PairName, string str_Dir1_Path, string str_Dir2_Path, string str_FilterRule, string str_SyncInterval, string str_SyncDirection)
        {
            string sql_out = @"Insert Into DIRPAIR (PAIRNAME,DIR1,DIR2,LastSyncStatus,FilterRule,AutoSyncInterval,SyncDirection,IsPaused) values('" + str_PairName + "','" + str_Dir1_Path + "','" + str_Dir2_Path + "',true";
            sql_out += String.IsNullOrEmpty(str_FilterRule) ? @",''" : (",'" + str_FilterRule + "'");
            sql_out += String.IsNullOrEmpty(str_SyncInterval) ? @",0" : ("," + str_SyncInterval);
            sql_out += String.IsNullOrEmpty(str_SyncDirection) ? @",0" : ("," + str_SyncDirection);
            sql_out += @",0)";
            return sql_out;
        }

        public string SQL_DelDirPair(string str_PairName, string str_Dir1_Path, string str_Dir2_Path)
        {
            string sql_out = @"Delete from DIRPAIR where PAIRNAME='" + str_PairName + "' AND DIR1='" + str_Dir1_Path + "' AND DIR2='" + str_Dir2_Path + "'";
            return sql_out;
        }

        public string SQL_UpdatePairSyncStatus(string str_PairID, string str_LastSyncDT, bool bl_SyncSuccessfulIndc)
        {
            string sql_out = @"Update DIRPAIR set LastSyncDT='" + str_LastSyncDT + "', LastSyncStatus=" + bl_SyncSuccessfulIndc.ToString() + " where PK_PAIRID=" + str_PairID;
            return sql_out;
        }

        public string SQL_UpdatePairInfor(string str_PairID, string str_FilterRule, string str_SyncInterval, string str_SyncDirection)
        {
            string sql_out = @"Update DIRPAIR set FilterRule='" + str_FilterRule + @"', AutoSyncInterval=" + (String.IsNullOrEmpty(str_SyncInterval) ? "0" : str_SyncInterval);
            sql_out += @",SyncDirection=" + str_SyncDirection + @" where PK_PAIRID=" + str_PairID;
            return sql_out;
        }

        public string SQL_CheckAutoSyncPair()
        {
            string sql_out = String.Empty;
            if (m_DBType.Equals(DATABASE_TYPE.ACCESS))
            {
                sql_out = @"SELECT PK_PairID,PairName from DIRPAIR where LastSyncStatus=true and AutoSyncInterval<>0 and (DateDiff(""n"",LastSyncDT,now)>AutoSyncInterval or LastSyncDT is null) and IsPaused=false";
            }
            else if (m_DBType.Equals(DATABASE_TYPE.SQLITE))
            {
                sql_out = @"SELECT PK_PairID,PairName from DIRPAIR where LastSyncStatus=true and AutoSyncInterval<>0 and ((strftime('%s',datetime('now','localtime'))-strftime('%s',LastSyncDT))/60>=AutoSyncInterval or LastSyncDT is null) and IsPaused=false;";
            }
            return sql_out;
        }

        public string SQL_FixDirPairStatus(bool bl_IsUpdateDT = true)
        {
            string sql_out = @"Update DIRPAIR set LastSyncStatus=true";
            if (bl_IsUpdateDT)
            {
                sql_out += @",LastSyncDT=now";
            }
            return sql_out;
        }

        public string SQL_PausePairAutoSync(string str_PairID)
        {
            string sql_out = @"Update DIRPAIR set IsPaused=iif(IsPaused,0,1) where PK_PAIRID=" + str_PairID;
            return sql_out;
        }
        #endregion

        #region File Infor Methods
        public string SQL_GetFileInfor(string str_TableName)
        {
            string sql_out = @"select * from " + str_TableName + " order by PK_FileID ASC";
            return sql_out;
        }

        public string SQL_GetFileDiff(string str_PairName, string str_Dir1TableName, string str_Dir2TableName, int int_SyncDirection)
        {
            string sql_out = String.Empty;
            if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(1) || int_SyncDirection.Equals(2) || int_SyncDirection.Equals(3))
            {
                //DIR1中有DIR2中没有的，DIFFTYPE=1，需要从DIR1同步至DIR2，同步方向0/1/2/3
                sql_out += @"SELECT T1.FileName,T1.FilePath AS FROMPATH,P1.DIR2&RIGHT(T1.FilePath,LEN(T1.FilePath)-LEN(P1.DIR1)) AS TOPATH,T1.FileMD5,T1.FileLastModDate,T1.FileSize,1 AS DIFFTYPE,T1.PK_FileID ";
                sql_out += @"FROM " + str_Dir1TableName + @" T1, DIRPAIR P1 WHERE T1.PAIRID=P1.PK_PAIRID and T1.FileStatus<>'DL' ";
                sql_out += @"and (select count(1) from " + str_Dir2TableName + @" T2 where T2.FileName=T1.FileName and T2.FilePath=P1.DIR2&RIGHT(T1.FilePath,LEN(T1.FilePath)-LEN(P1.DIR1)))=0 UNION ";
                //DIR1和DIR2都有但是MD5值不同，而且DIR1比DIR2修改时间晚的，DIFFTYPE=3，需要从DIR1同步至DIR2，同步方向0/1/2/3
                sql_out += @"SELECT T1.FileName,T1.FilePath AS FROMPATH,P1.DIR2&RIGHT(T1.FilePath,LEN(T1.FilePath)-LEN(P1.DIR1)) AS TOPATH,T1.FileMD5,T1.FileLastModDate,T1.FileSize,3 AS DIFFTYPE,T1.PK_FileID ";
                sql_out += @"FROM " + str_Dir1TableName + @" T1," + str_Dir2TableName + @" T2, DIRPAIR P1 WHERE T1.PAIRID=P1.PK_PAIRID ";
                sql_out += @"and T2.FileName=T1.FileName and T2.FilePath=P1.DIR2&RIGHT(T1.FilePath,LEN(T1.FilePath)-LEN(P1.DIR1)) AND T1.FileMD5<>T2.FileMD5 ";
                sql_out += @"and T1.FileStatus<>'DL' AND T1.FileLastModDate>T2.FileLastModDate ";
            }
            if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(1) || int_SyncDirection.Equals(4) || int_SyncDirection.Equals(5))
            {
                if (!String.IsNullOrEmpty(sql_out))
                {
                    sql_out += @" UNION ";
                }
                //DIR2中有DIR1中没有的，DIFFTYPE=2，需要从DIR2同步至DIR1，同步方向0/1/4/5
                sql_out += @"SELECT T2.FileName,T2.FilePath AS FROMPATH,P1.DIR1&RIGHT(T2.FilePath,LEN(T2.FilePath)-LEN(P1.DIR2)) AS TOPATH,T2.FileMD5,T2.FileLastModDate,T2.FileSize,2 AS DIFFTYPE,T2.PK_FileID ";
                sql_out += @"FROM " + str_Dir2TableName + @" T2, DIRPAIR P1 WHERE T2.PAIRID=P1.PK_PAIRID and T2.FileStatus<>'DL' ";
                sql_out += @"and (select count(1) from " + str_Dir1TableName + @" T1 where T1.FileName=T2.FileName and T1.FilePath=P1.DIR1&RIGHT(T2.FilePath,LEN(T2.FilePath)-LEN(P1.DIR2)))=0 UNION ";
                //DIR1和DIR2都有但是MD5值不同，而且DIR2比DIR1修改时间晚的，DIFFTYPE=4，需要从DIR2同步至DIR1，同步方向0/1/4/5
                sql_out += @"SELECT T2.FileName,T2.FilePath AS FROMPATH,P1.DIR1&RIGHT(T2.FilePath,LEN(T2.FilePath)-LEN(P1.DIR2)) AS TOPATH,T2.FileMD5,T2.FileLastModDate,T2.FileSize,4 AS DIFFTYPE,T2.PK_FileID ";
                sql_out += @"FROM " + str_Dir1TableName + @" T1," + str_Dir2TableName + @" T2, DIRPAIR P1 WHERE T2.PAIRID=P1.PK_PAIRID ";
                sql_out += @"and T2.FileName=T1.FileName and T1.FilePath=P1.DIR1&RIGHT(T2.FilePath,LEN(T2.FilePath)-LEN(P1.DIR2)) AND T1.FileMD5<>T2.FileMD5 ";
                sql_out += @"and T2.FileStatus<>'DL' AND T1.FileLastModDate<T2.FileLastModDate ";
            }
            if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(2))
            {
                //DIR1和DIR2都有而且MD5值相同，但是DIR1中文件状态是'DL'，DIFFTYPE=5，需要从DIR2中删除，同步方向0/2
                sql_out += @" UNION SELECT T1.FileName,T1.FilePath AS FROMPATH,P1.DIR2&RIGHT(T1.FilePath,LEN(T1.FilePath)-LEN(P1.DIR1)) AS TOPATH,T1.FileMD5,T1.FileLastModDate,T1.FileSize,5 AS DIFFTYPE,T2.PK_FileID ";
                sql_out += @"FROM " + str_Dir1TableName + @" T1," + str_Dir2TableName + @" T2, DIRPAIR P1 WHERE T1.PAIRID=P1.PK_PAIRID ";
                sql_out += @"and T2.FileName=T1.FileName and T2.FilePath=P1.DIR2&RIGHT(T1.FilePath,LEN(T1.FilePath)-LEN(P1.DIR1)) AND T1.FileMD5=T2.FileMD5 and T1.FileStatus='DL' and T2.FileStatus<>'DL' ";
            }
            if (int_SyncDirection.Equals(0) || int_SyncDirection.Equals(4))
            {
                //DIR1和DIR2都有而且MD5值相同，但是DIR2中文件状态是'DL'，DIFFTYPE=6，需要从DIR1中删除，同步方向0/4
                sql_out += @" UNION SELECT T2.FileName,T2.FilePath AS FROMPATH,P1.DIR1&RIGHT(T2.FilePath,LEN(T2.FilePath)-LEN(P1.DIR2)) AS TOPATH,T2.FileMD5,T2.FileLastModDate,T2.FileSize,6 AS DIFFTYPE,T1.PK_FileID ";
                sql_out += @"FROM " + str_Dir1TableName + @" T1," + str_Dir2TableName + @" T2, DIRPAIR P1 WHERE T1.PAIRID=P1.PK_PAIRID ";
                sql_out += @"and T2.FileName=T1.FileName and T1.FilePath=P1.DIR1&RIGHT(T2.FilePath,LEN(T2.FilePath)-LEN(P1.DIR2)) AND T1.FileMD5=T2.FileMD5 and T2.FileStatus='DL' and T1.FileStatus<>'DL' ";
            }
            sql_out += @"ORDER BY DIFFTYPE,FROMPATH,PK_FileID ASC";
            return sql_out;
        }

        public string SQL_AddFileInforEnq(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID)
        {
            string sql_out = @"Select PK_FileID from " + str_TableName + @" where FileName='" + str_FileName + @"' and FilePath='" + str_FilePath + @"' and FileStatus='AC'";
            return sql_out;
        }

        public string SQL_AddFileInforIns(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID)
        {
            string sql_out = @"Insert Into " + str_TableName + @" (FileName,FilePath,FileSize,FileMD5,FileLastModDate,PAIRID,FileStatus) values('" + str_FileName + "','" + str_FilePath + "'," + str_FileSize + ",'" + str_FileMD5 + "','" + str_FileLastModDate + "'," + str_PairID + ",'AC')";
            return sql_out;
        }

        public string SQL_UpdFileInfor(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileMD5, string str_FileLastModDate, string str_PairID)
        {
            string sql_out = @"update " + str_TableName + " set FilePath = '" + str_FilePath + "' ";
            if (!String.IsNullOrEmpty(str_FileSize))
            {
                sql_out += @",FileSize = " + str_FileSize;
            }
            if (!String.IsNullOrEmpty(str_FileMD5))
            {
                sql_out += @",FileMD5='" + str_FileMD5 + "'";
            }
            if (!String.IsNullOrEmpty(str_FileLastModDate))
            {
                sql_out += @",FileLastModDate='" + str_FileLastModDate + "'";
            }
            sql_out += @"where FileName='" + str_FileName + "' and FilePath='" + str_FilePath + "' and PAIRID=" + str_PairID;
            return sql_out;
        }

        public string SQL_DelFileInforSoft(string str_TableName, string str_FileID, string str_PairID)
        {
            string sql_out = @"Update " + str_TableName + @" set FileStatus='DL' where PAIRID=" + str_PairID + @" and PK_FileID=" + str_FileID;
            return sql_out;
        }

        public string SQL_DelFileInforAllHard(string str_TableName, string str_PairID)
        {
            string sql_out = @"Delete from " + str_TableName + @" where FileStatus = 'DL' and PAIRID=" + str_PairID;
            return sql_out;
        }

        public string SQL_CheckFileInDB(string str_TableName, string str_FileName, string str_FilePath, string str_FileSize, string str_FileLastModDate)
        {
            string sql_out = @"select * from " + str_TableName + @" where FileName='" + str_FileName + @"' and FilePath='" + str_FilePath + @"' and FileLastModDate=cdate('" + str_FileLastModDate + @"') and FileSize = " + str_FileSize;
            return sql_out;
        }

        public string SQL_AddSyncDetailIns(string str_PairName, string str_FromFile, string str_ToFile, int int_FileDiffType, bool bl_SyncStatus)
        {
            string sql_out = @"Insert Into SyncDetail (PAIRNAME,FromFile,ToFile,FileDiffType,SyncStatus) values('" + str_PairName + "','" + str_FromFile + "','" + str_ToFile + "'," + int_FileDiffType.ToString() + "," + bl_SyncStatus.ToString() + ")";
            return sql_out;
        }

        public string SQL_UpdSyncDetail(string str_PairName, string str_FromFile, string str_ToFile, int int_FileDiffType, bool bl_SyncStatus)
        {
            string sql_out = @"Update SyncDetail set SyncStatus=" + bl_SyncStatus.ToString() + @" where PAIRNAME='" + str_PairName + "' and FromFile='" + str_FromFile + "' and ToFile='" + str_ToFile + "' and FileDiffType=" + int_FileDiffType.ToString();
            return sql_out;
        }

        public string SQL_CleanSyncDetailRecord(string str_PairName, bool bl_SyncStatus)
        {
            string sql_out = @"Delete from SyncDetail where SyncStatus=" + bl_SyncStatus.ToString();
            if (!String.IsNullOrEmpty(str_PairName))
            {
                sql_out += @" and PAIRNAME='" + str_PairName + "'";
            }
            return sql_out;
        }

        public string SQL_GetUnfinishedSyncDetail(string str_PairName)
        {
            string sql_out = @"Select * from SyncDetail where SyncStatus=false";
            if (!String.IsNullOrEmpty(str_PairName))
            {
                sql_out += @" and PAIRNAME='" + str_PairName + "'";
            }
            return sql_out;
        }
        #endregion
    }
}
