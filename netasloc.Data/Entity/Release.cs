using System;

namespace netasloc.Data.Entity
{
    // releases table entry
    public class Release : _IAuditable
    {
        public uint id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string release_code { get; set; }
        public DateTime release_start { get; set; }
        public DateTime release_end { get; set; }
        public uint total_line_count { get; set; }
        public uint code_line_count { get; set; }
        public uint comment_line_count { get; set; }
        public uint empty_line_count { get; set; }
        public int difference_sloc { get; set; }
        public int difference_loc { get; set; }
    }
}
