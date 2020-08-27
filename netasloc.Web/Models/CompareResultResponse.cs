
namespace netasloc.Web.Models
{
    public class CompareResultResponse
    {
        public string FileName { get; set; }
        public uint netaslocTotal { get; set; }
        public uint counterTotal { get; set; }
        public int differenceTotal { get; set; }
        public uint netaslocCode { get; set; }
        public uint counterCode { get; set; }
        public int differenceCode { get; set; }
        public uint netaslocComment { get; set; }
        public uint counterComment { get; set; }
        public int differenceComment { get; set; }
        public uint netaslocEmpty { get; set; }
        public uint counterEmpty { get; set; }
        public int differenceEmpty { get; set; }
    }
}
