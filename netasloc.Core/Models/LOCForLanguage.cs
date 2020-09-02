using System.Collections.Generic;

namespace netasloc.Core.Models
{
    public class LOCForLanguage
    {
        public LOCForLanguage()
        {
            FileLanguage = "";
            FileCount = 0;
            TotalLineCount = 0;
            CodeLineCount = 0;
            CommentLineCount = 0;
            EmptyLineCount = 0;
            AllExtensionsData = new Dictionary<string, LOCForFileExtension>();
        }

        public string FileLanguage { get; set; }
        public uint FileCount { get; set; }
        public uint TotalLineCount { get; set; }
        public uint CodeLineCount { get; set; }
        public uint CommentLineCount { get; set; }
        public uint EmptyLineCount { get; set; }
        public IDictionary<string, LOCForFileExtension> AllExtensionsData { get; set; }
    }
}
