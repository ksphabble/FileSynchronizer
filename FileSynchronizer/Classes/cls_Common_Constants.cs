using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSynchronizer
{
    public static class cls_Common_Constants
    {
        public const string c_ProgramTitle = @"FileSynchronizer";
        public const string c_FSBackup_Str = @"_FSBackup";
        public const string c_DirNameChar_Str = "~";
        public const string c_ThreadPrefix_Str = @"->";
        public const Int32 c_MaxDirLengthWIN32_Int = 260;
        public const string c_ExtMaxDirLengthPrefix_Str = @"\\?\";
        public const string c_TempUpdFileExt_Str = @".tmp";
        public const string c_UpdateURL_Str = @"https://api.github.com/repos/ksphabble/FileSynchronizer/releases/latest";
        public const string c_GithubToken_Str = @"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";
        public const string c_GithubEnqLimitToken_Str = @"curl -H ""Authorization: token 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"" https://api.github.com/rate_limit";
        public const string c_GithubEnqLimit_Str = @"curl -i https://api.github.com/rate_limit";

        public enum PairStatus
        {
            FREE = 0, ANALYSIS = 1, SYNC = 2
        }
    }
}
