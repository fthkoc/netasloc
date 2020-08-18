using System;

namespace netasloc.Core.DTO
{
    public class ReleaseDTO : _IDTO
    {
        public uint ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ReleaseCode { get; set; }
        public uint TotalLineCount { get; set; }
        public uint CodeLineCount { get; set; }
        public uint CommentLineCount { get; set; }
        public uint EmptyLineCount { get; set; }
        public int DifferenceSLOC { get; set; }
        public int DifferenceLOC { get; set; }
    }
}
