using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netasloc.Core.Models;
using netasloc.Core.Services;
using netasloc.Web.Models.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Text.Json;

namespace netasloc.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APIController : ControllerBase
    {
        private readonly ILogger<APIController> _logger;
        private readonly _ILOCService _locService;

        public APIController(ILogger<APIController> logger, _ILOCService locService)
        {
            _logger = logger;
            _locService = locService;
        }

        [HttpGet("AnalyzeLocalRepositories")]
        public IActionResult AnalyzeLocalRepositories([FromBody] AnalyzeLocalRepositoriesRequest request)
        {
            _logger.LogInformation("AnalyzeController::AnalyzeLocalRepositories::called.");
            LOCForAllResponse result;
            try
            {
                result = _locService.AnalyzeLOCForAll(request.Repositories);
                if (string.IsNullOrEmpty(request.ResultsDirectory))
                    request.ResultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "Analyze Results");
                bool isWritten = _locService.WriteResultToFile(request.ResultsDirectory, result);
                if (!isWritten)
                    throw new Exception("Error at writing result in '" + request.ResultsDirectory + "'!");
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::AnalyzeLocalRepositories::Exception::{0}", ex.Message);
                return null;
            }
            _logger.LogInformation("AnalyzeController::AnalyzeLocalRepositories::finished.");
            return new JsonResult(result);
        }

        [HttpGet("AnalyzeRemoteRepositories")]
        public IActionResult AnalyzeRemoteRepositories([FromBody] AnalyzeRemoteRepositoriesRequest request)
        {
            _logger.LogInformation("AnalyzeController::AnalyzeRemoteRepositories::called.");
            LOCForAllResponse result;
            try
            {
                var directoryFullPaths = _locService.GetRepositoriesFromGit(request.WorkingDirectoryPath, request.RemoteRepositories);
                result = _locService.AnalyzeLOCForAll(directoryFullPaths);
                string resultsFolderFullPath = Path.Combine(request.WorkingDirectoryPath, request.ResultsFolderName);
                bool isWritten = _locService.WriteResultToFile(resultsFolderFullPath, result);
                if (!isWritten)
                    throw new Exception("Error at writing result in '" + resultsFolderFullPath + "'!");
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::AnalyzeRemoteRepositories::Exception::{0}", ex.Message);
                return null;
            }
            _logger.LogInformation("AnalyzeController::AnalyzeRemoteRepositories::finished.");
            return new JsonResult(result);
        }
    }
}
