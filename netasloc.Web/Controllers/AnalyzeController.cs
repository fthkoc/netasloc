using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.Models;
using netasloc.Core.Services;
using netasloc.Web.Models;
using System;

namespace netasloc.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyzeController : ControllerBase
    {
        private readonly ILogger<AnalyzeController> _logger;
        private readonly _ILOCService _locService;

        public AnalyzeController(ILogger<AnalyzeController> logger, _ILOCService locService)
        {
            _logger = logger;
            _locService = locService;
        }

        [HttpGet("AnalyzeAllRepositories")]
        public AnalyzeAllRepositoriesResponse AnalyzeAllRepositories([FromBody] AnalyzeAllRepositoriesRequest request)
        {
            _logger.LogInformation("AnalyzeController::AnalyzeAllRepositories::called.");
            LOCForAllResponse result = new LOCForAllResponse();
            try
            {
                result = _locService.AnalyzeLOCForAll(request.DirectoryList);
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::AnalyzeAllRepositories::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("AnalyzeController::AnalyzeAllRepositories::finished.");
            return new AnalyzeAllRepositoriesResponse() 
            {
                FileCount = result.FileCount,
                TotalLineCount = result.TotalLineCount,
                CodeLineCount = result.CodeLineCount,
                CommentLineCount = result.CommentLineCount,
                EmptyLineCount = result.EmptyLineCount,
                AllDirectoriesData = result.AllDirectoriesData
            };
        }
    }
}
