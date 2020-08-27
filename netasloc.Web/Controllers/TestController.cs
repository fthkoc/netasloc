using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

                Dictionary<string, LOCForSingleFileResponse> counterFiles = new Dictionary<string, LOCForSingleFileResponse>();
                using (var reader = new StreamReader(request.counterResultFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        string fileName = values[0];
                        int comment = int.Parse(values[15]);
                        int blank = int.Parse(values[16]);
                        int total = int.Parse(values[17]);
                        int code = total - (comment + blank);

                        LOCForSingleFileResponse item = netaslocFiles[fileName.ToLower()];
                        if (item != null)
                        {
                            if (!((total == item.TotalLineCount) && (comment == item.CommentLineCount) && (blank == item.EmptyLineCount)))
                            {
                                result.Add(new CompareResultResponse()
                                {
                                    FileName = Path.Combine(item.FileDirectory, item.FileName),
                                    netaslocTotal = item.TotalLineCount,
                                    counterTotal = (uint) total,
                                    differenceTotal = (int) item.TotalLineCount - (int) total,
                                    netaslocCode = item.CodeLineCount,
                                    counterCode = (uint) code,
                                    differenceCode = (int) item.CodeLineCount - code,
                                    netaslocComment = item.CommentLineCount,
                                    counterComment = (uint) comment,
                                    differenceComment = (int) item.CommentLineCount - comment,
                                    netaslocEmpty = item.EmptyLineCount,
                                    counterEmpty = (uint) blank,
                                    differenceEmpty = (int) item.EmptyLineCount - blank,
                                });
                            }
                        }
                    }
                }

                string fileFullPath = Path.Combine(request.resultDirectory, "compare_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json");

                if (System.IO.File.Exists(fileFullPath))
                    throw new FileNotFoundException("{0} exists.", fileFullPath);

                var resultFile = System.IO.File.Create(fileFullPath);
                var jsonResult = JsonSerializer.Serialize(result);
                resultFile.Write(Encoding.Default.GetBytes(jsonResult), 0, jsonResult.Length);
                resultFile.Close();
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
