using System;

namespace netasloc.Data.Entity
{
    // analyze_results table entry
    public class AnalyzeResult : _IAuditable
    {
        public uint id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public uint directory_count { get; set; }
        public string directory_id_list { get; set; }
        public uint total_line_count { get; set; }
        public uint code_line_count { get; set; }
        public uint comment_line_count { get; set; }
        public uint empty_line_count { get; set; }
        public int difference_sloc { get; set; }
        public int difference_loc { get; set; }
    }
}
