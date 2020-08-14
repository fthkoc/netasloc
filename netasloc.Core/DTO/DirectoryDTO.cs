using System;

namespace netasloc.Core.DTO
{
    public class DirectoryDTO : _IDTO
    {
        public uint ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ProjectName { get; set; }
        public string FullPath { get; set; }
        public uint FileCount { get; set; }
        public uint TotalLineCount { get; set; }
        public uint CodeLineCount { get; set; }
        public uint CommentLineCount { get; set; }
        public uint EmptyLineCount { get; set; }
    }
}
