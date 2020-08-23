using System;
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
        public IActionResult AnalyzeLOCForSingleFile([FromBody] Test_AnalyzeLOCForSingleFileRequest request)
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

        [HttpGet("AnalyzeLOCForDirectory")]
        public IActionResult AnalyzeLOCForDirectory([FromBody] Test_AnalyzeLOCForDirectoryRequest request)
        {
            _logger.LogInformation("TestController::AnalyzeLOCForDirectory::called.");
            LOCForDirectoryResponse result = new LOCForDirectoryResponse();
            try
            {
                result = _locService.AnalyzeLOCForDirectory(request.DirectoryFullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError("TestController::AnalyzeLOCForDirectory::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("TestController::AnalyzeLOCForDirectory::finished.");
            return new JsonResult(result);
        }

        [HttpGet("AnalyzeLOCForAll")]
        public IActionResult AnalyzeLOCForAll([FromBody] Test_AnalyzeLOCForAllRequest request)
        {
            _logger.LogInformation("TestController::AnalyzeLOCForAll::called.");
            LOCForAllResponse result = new LOCForAllResponse();
            try
            {
                result = _locService.AnalyzeLOCForAll(request.DirectoryFullPaths);
            }
            catch (Exception ex)
            {
                _logger.LogError("TestController::AnalyzeLOCForAll::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("TestController::AnalyzeLOCForAll::finished.");
            return new JsonResult(result);
        }
    }
}
