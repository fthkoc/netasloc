using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.Models;
using netasloc.Core.Services;
using netasloc.Web.Models;

namespace netasloc.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly _ILOCService _locService;

        public TestController(ILogger<TestController> logger, _ILOCService locService)
        {
            _logger = logger;
            _locService = locService;
        }

        [HttpGet("AnalyzeLOCForSingleFile")]
        public IActionResult AnalyzeLOCForSingleFile([FromBody] TestAnalyzeLOCForSingleFileRequest request)
        {
            _logger.LogInformation("TestController::AnalyzeLOCForSingleFile::called.");
            LOCForSingleFileResponse result = new LOCForSingleFileResponse();
            try
            {
                result = _locService.AnalyzeLOCForSingleFile(request.FileDirectory, request.FileName + request.FileExtension, request.FileExtension);
            }
            catch (Exception ex)
            {
                _logger.LogError("TestController::AnalyzeLOCForSingleFile::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("TestController::AnalyzeLOCForSingleFile::finished.");
            return new JsonResult(result);
        }
    }
}
