using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        [HttpGet("CompareResults")]
        public IActionResult CompareResults([FromBody] CompareResultRequest request)
        {
            _logger.LogInformation("TestController::CompareResults::called.");
            List<CompareResultResponse> result = new List<CompareResultResponse>();
            try
            {
                string netaslocRawData = System.IO.File.ReadAllText(request.netaslocResultFilePath);
                LOCForAllResponse netaslocResult = JsonSerializer.Deserialize<LOCForAllResponse>(netaslocRawData);
                Dictionary<string ,LOCForSingleFileResponse> netaslocFiles = new Dictionary<string, LOCForSingleFileResponse>();
                foreach (var directory in netaslocResult.AllDirectoriesData)
                {
                    foreach (var language in directory.Value.AllLanguagesData)
                    {
                        foreach (var extension in language.Value.AllExtensionsData)
                        {
                            foreach (var file in extension.Value.AllFilesData)
                            {
                                netaslocFiles.Add(Path.Combine(file.FileDirectory, file.FileName).ToLower(), file);
                            }
                        }
                    }
                }

                string counterRawData = System.IO.File.ReadAllText(request.netaslocResultFilePath);
                CounterResult[] counterResult = JsonSerializer.Deserialize<CounterResult[]>(netaslocRawData);

                foreach (var counterItem in counterResult)
                {
                    LOCForSingleFileResponse item = netaslocFiles[counterItem.filename.ToLower()];
                    if (counterItem != null)
                    {
                        if (!((counterItem.total == item.TotalLineCount) && (counterItem.comment == item.CommentLineCount)
                            && (counterItem.blank == item.EmptyLineCount)))
                        {
                            result.Add(new CompareResultResponse()
                            {
                                FileName = Path.Combine(item.FileDirectory, item.FileName),
                                netaslocTotal = item.TotalLineCount,
                                counterTotal = counterItem.total,
                                differenceTotal = ((int) item.TotalLineCount - (int) counterItem.total),
                                netaslocCode = item.CodeLineCount,
                                counterCode = counterItem.total - (counterItem.comment + counterItem.blank),
                                differenceCode = ((int) item.CodeLineCount - (int) (counterItem.total - (counterItem.comment + counterItem.blank))),
                                netaslocComment = item.CommentLineCount,
                                counterComment = counterItem.comment,
                                differenceComment = ((int) item.CommentLineCount - (int) counterItem.comment),
                                netaslocEmpty = item.EmptyLineCount,
                                counterEmpty = counterItem.blank,
                                differenceEmpty = ((int) item.EmptyLineCount - (int) counterItem.blank),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("TestController::CompareResults::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("TestController::CompareResults::finished.");
            return new JsonResult(result);
        }
    }
}
