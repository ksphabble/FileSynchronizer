using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class cls_Common_Constants
    {
        public const string c_FSBackup_Str = @"_FSBackup";
        public const string c_DirNameChar_Str = "~";
        public const string c_ThreadPrefix_Str = @"->";
        public const Int32 c_MaxDirLengthWIN32_Int = 260;
        public const string c_ExtMaxDirLengthPrefix_Str = @"\\?\";

        public enum PairStatus
        {
            FREE = 0, ANALYSIS = 1, SYNC = 2
        }
    }
}
