using Microsoft.AspNetCore.Mvc;
using netasloc.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netasloc.Web.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Releases = new List<ReleaseDTO>();
            Directories = new DirectoryData();
        }

        public IEnumerable<ReleaseDTO> Releases { get; set; }
        public DirectoryData Directories { get; set; }
    }

    public class DirectoryData
    {
        public string projectNames { get; set; }
        public string fileCounts { get; set; }
        public string totalLines { get; set; }
        public string totalCodeLines { get; set; }
        public string totalCommentLines { get; set; }
        public string totalEmptyLines { get; set; }
        public string date { get; set; }
    }
}
