using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netasloc.Core.Models;
using netasloc.Core.Services;
using netasloc.Web.Models.API;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace netasloc.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly ILogger<APIController> _logger;
        private readonly IConfiguration _configuration;
        private readonly _ILOCService _locService;

        public APIController(ILogger<APIController> logger, IConfiguration configuration, _ILOCService locService)
        {
            _logger = logger;
            _configuration = configuration;
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

        [HttpGet("AnalyzeTrackedRepositories")]
        public IActionResult AnalyzeTrackedRepositories()
        {
            _logger.LogInformation("AnalyzeController::AnalyzeAllRepositories::called.");
            LOCForAllResponse result = new LOCForAllResponse();
            try
            {
                string resultFilePath = _configuration.GetSection("AnalyzeConfiguration:ResultFilePath").Get<string>();
                string[] trackedRepositories = _configuration.GetSection("AnalyzeConfiguration:TrackedRepositories").Get<string[]>();

                result = _locService.AnalyzeLOCForAll(trackedRepositories);

                if (string.IsNullOrEmpty(resultFilePath))
                    resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Analyze Results");

                if (!Directory.Exists(resultFilePath))
                    Directory.CreateDirectory(resultFilePath);

                string fileFullPath = Path.Combine(resultFilePath, "result_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json");

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
