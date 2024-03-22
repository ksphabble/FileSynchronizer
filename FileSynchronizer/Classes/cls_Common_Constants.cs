using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class cls_Common_Constants
    {
        public const string str_FSBackup = @"_FSBackup";
        public const string str_DirNameChar = "~";
        public const string str_ThreadPrefix = @"->";

        public enum PairStatus
        {
            FREE = 0, ANALYSIS = 1, SYNC = 2
        }
    }
}
