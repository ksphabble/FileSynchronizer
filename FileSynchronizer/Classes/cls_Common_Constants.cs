using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class cls_Common_Constants
    {
        public const string C_StrFSBackup = @"_FSBackup";
        public const string C_StrDirNameChar = "~";
        public const string C_StrThreadPrefix = @"->";
        public const Int32 C_IMaxDirLengthWIN32 = 260;
        public const string C_StrExtMaxDirLengthPrefix = @"\\?\";

        public enum PairStatus
        {
            FREE = 0, ANALYSIS = 1, SYNC = 2
        }
    }
}
