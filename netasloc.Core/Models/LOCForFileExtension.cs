using System;
using System.Collections.Generic;
using System.Text;

namespace netasloc.Core.Models
{
    public class LOCForFileExtension
    {
        public LOCForFileExtension()
        {
            FileExtension = "";
            FileCount = 0;
            TotalLineCount = 0;
            CodeLineCount = 0;
            CommentLineCount = 0;
            EmptyLineCount = 0;
            AllFilesData = new List<LOCForSingleFileResponse>();
        }

        public string FileExtension { get; set; }
        public uint FileCount { get; set; }
        public uint TotalLineCount { get; set; }
        public uint CodeLineCount { get; set; }
        public uint CommentLineCount { get; set; }
        public uint EmptyLineCount { get; set; }
        public IList<LOCForSingleFileResponse> AllFilesData { get; set; }
    }
}
