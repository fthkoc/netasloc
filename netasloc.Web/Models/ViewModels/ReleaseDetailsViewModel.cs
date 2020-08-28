using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Web.Models.ViewModels
{
    public class ReleaseDetailsViewModel
    {
        public ReleaseDetailsViewModel()
        {
            AnalyzeResults = new List<AnalyzeResultDTO>();
        }

        public ReleaseDTO Release { get; set; }
        public IList<AnalyzeResultDTO> AnalyzeResults { get; set; }
    }
}
