using System;

namespace netasloc.Data.Entity
{
    // directories table entry
    public class Directory : _IAuditable
    {
        public uint id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string project_name { get; set; }
        public string full_path { get; set; }
        public uint file_count { get; set; }
        public uint total_line_count { get; set; }
        public uint code_line_count { get; set; }
        public uint comment_line_count { get; set; }
        public uint empty_line_count { get; set; }
    }
}
