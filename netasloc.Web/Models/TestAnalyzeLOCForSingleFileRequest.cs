using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netasloc.Web.Models
{
    public class TestAnalyzeLOCForSingleFileRequest
    {
        public string FileDirectory { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
