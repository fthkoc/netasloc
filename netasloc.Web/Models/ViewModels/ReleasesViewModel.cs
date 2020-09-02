using netasloc.Core.DTO;
using System.Collections.Generic;

namespace netasloc.Web.Models.ViewModels
{
    public class ReleasesViewModel
    {
        public ReleasesViewModel()
        {
            Releases = new List<ReleaseDTO>();
        }
        public IList<ReleaseDTO> Releases { get; set; }
    }
}
