
namespace netasloc.Core.Models
{
    public class LOCForSingleFileResponse
    {
        public LOCForSingleFileResponse()
        {
            FileDirectory = "";
            FileName = "";
            TotalLineCount = 0;
            CodeLineCount = 0;
            CommentLineCount = 0;
            EmptyLineCount = 0;
        }

        public string FileDirectory { get; set; }
        public string FileName { get; set; }
        public uint TotalLineCount { get; set; }
        public uint CodeLineCount { get; set; }
        public uint CommentLineCount { get; set; }
        public uint EmptyLineCount { get; set; }
    }
}
