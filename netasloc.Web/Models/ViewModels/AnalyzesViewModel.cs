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

        public IList<AnalyzeResultDTO> AnalyzeResults { get; set; }
    }
}
