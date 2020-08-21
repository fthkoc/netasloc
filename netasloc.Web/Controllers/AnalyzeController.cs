using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.Models;
using netasloc.Core.Services;
using netasloc.Web.Models;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

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
        public IActionResult AnalyzeAllRepositories([FromBody] AnalyzeAllRepositoriesRequest request)
        {
            _logger.LogInformation("AnalyzeController::AnalyzeAllRepositories::called.");
            LOCForAllResponse result = new LOCForAllResponse();
            try
            {
                result = _locService.AnalyzeLOCForAll(request.Repositories);

                if (string.IsNullOrEmpty(request.ResultsDirectory))
                    request.ResultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "AnalyzeResults");

                if (!Directory.Exists(request.ResultsDirectory))
                    Directory.CreateDirectory(request.ResultsDirectory);

                string fileFullPath = Path.Combine(request.ResultsDirectory, "result_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json");

                if (System.IO.File.Exists(fileFullPath))
                    throw new FileNotFoundException("{0} exists.", fileFullPath);

                var file = System.IO.File.Create(fileFullPath);
                var jsonResult = JsonSerializer.Serialize(result);
                file.Write(Encoding.Default.GetBytes(jsonResult), 0, jsonResult.Length);
                file.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::AnalyzeAllRepositories::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("AnalyzeController::AnalyzeAllRepositories::finished.");
            return new JsonResult(result);
        }
    }
}
