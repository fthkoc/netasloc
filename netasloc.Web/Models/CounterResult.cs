using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netasloc.Web.Models
{
    public class CounterResult
    {
        public string filename { get; set; }
        public string language { get; set; }
        public uint comment { get; set; }
        public uint blank { get; set; }
        public uint total { get; set; }
    }
}
