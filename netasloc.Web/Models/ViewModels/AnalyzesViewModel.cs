using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Web.Models.ViewModels
{
    public class AnalyzesViewModel
    {
        public AnalyzesViewModel()
        {
            AnalyzeResults = new List<AnalyzeResultDTO>();
        }

        public IEnumerable<AnalyzeResultDTO> AnalyzeResults { get; set; }
    }
}
