using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.DTO;
using netasloc.Core.Services;

namespace netasloc.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly ILogger<DataController> _logger;
        private readonly _IDataAccessService _dataAccess;

        public DataController(ILogger<DataController> logger, _IDataAccessService dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpGet("GetLastAnalyzedDirectories")]
        public JsonResult GetLastAnalyzedDirectories()
        {
            _logger.LogInformation("DataController::GetLastAnalyzedDirectories::called.");

            IEnumerable<DirectoryDTO> response = _dataAccess.GetLastAnalyzedDirectories();

            List<string> projectNames = new List<string>();
            List<uint> fileCounts = new List<uint>();
            List<uint> totalLines= new List<uint>();
            List<uint> totalCodeLines = new List<uint>();
            List<uint> totalCommentLines = new List<uint>();
            List<uint> totalEmptyLines = new List<uint>();
            System.DateTime? date = null;

            foreach (var item in response)
            {
                projectNames.Add(item.ProjectName);
                fileCounts.Add(item.FileCount);
                totalLines.Add(item.TotalLineCount);
                totalCodeLines.Add(item.CodeLineCount);
                totalCommentLines.Add(item.CommentLineCount);
                totalEmptyLines.Add(item.EmptyLineCount);
                date = item.CreatedAt;
            }

            _logger.LogInformation("DataController::GetLastAnalyzedDirectories::finished.");

            return Json(new {
                projectNames = projectNames.ToArray(),
                fileCounts = fileCounts.ToArray(),
                totalLines = totalLines.ToArray(),
                totalCodeLines = totalCodeLines.ToArray(),
                totalCommentLines = totalCommentLines.ToArray(),
                totalEmptyLines = totalEmptyLines.ToArray(),
                date = date.ToString()
            });
        }

    }
}
