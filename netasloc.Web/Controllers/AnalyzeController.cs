using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netasloc.Core.DTO;
using netasloc.Core.Services;
using netasloc.Web.Models.ViewModels;
using System;
using System.IO;
using System.Linq;

namespace netasloc.Web.Controllers
{
    public class AnalyzeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly _IDataAccessService _dataAccess;
        private readonly _ILOCService _locService;

        public AnalyzeController(ILogger<HomeController> logger, IConfiguration configuration, 
            _IDataAccessService dataAccess, _ILOCService locService)
        {
            _logger = logger;
            _configuration = configuration;
            _dataAccess = dataAccess;
            _locService = locService;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("AnalyzeController::Index::called.");
            AnalyzesViewModel model = new AnalyzesViewModel();
            try
            {
                model.AnalyzeResults = _dataAccess.GetAllAnalyzeResults().Take(30).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::Index::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("AnalyzeController::Index::finished.");
            return View(model);
        }

        public IActionResult Details(uint id)
        {
            _logger.LogInformation("AnalyzeController::Details::called.");
            AnalyzeDetailsViewModel model = new AnalyzeDetailsViewModel();
            try
            {
                model.AnalyzeResult = _dataAccess.GetAnalyzeResultByID(id);
                string[] idList = model.AnalyzeResult.DirectoryIDList.Split(',');
                foreach (string directoryID in idList)
                    if (!string.IsNullOrEmpty(directoryID))
                        model.Directories.Add(_dataAccess.GetDirectoryByID(uint.Parse(directoryID)));
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::Details::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("AnalyzeController::Details::finished.");
            return View(model);
        }

        public IActionResult AnalyzeRemote()
        {
            _logger.LogInformation("AnalyzeController::AnalyzeRemote::called.");
            try
            {
                string workingDirectoryPath = _configuration.GetSection("AnalyzeConfiguration:WorkingDirectoryPath").Get<string>();
                string resultsFolderName = _configuration.GetSection("AnalyzeConfiguration:ResultsFolderName").Get<string>();
                string[] remoteRepositories = _configuration.GetSection("AnalyzeConfiguration:RemoteRepositories").Get<string[]>();

                var directoryFullPaths = _locService.GetRepositoriesFromGit(workingDirectoryPath, remoteRepositories);
                var result = _locService.AnalyzeLOCForAll(directoryFullPaths);

                string resultsFolderFullPath = Path.Combine(workingDirectoryPath, resultsFolderName);

                bool isWritten = _locService.WriteResultToFile(resultsFolderFullPath, result);
                if (!isWritten)
                    throw new Exception("Error at writing result in '" + resultsFolderFullPath + "'!");

                AnalyzeResultDTO analyzeResult = _dataAccess.GetAllAnalyzeResults().ElementAt(0);
                _logger.LogInformation("AnalyzeController::AnalyzeRemote::finished.");
                return RedirectToAction("Details", new { id = analyzeResult.ID });
            }
            catch (Exception ex)
            {
                _logger.LogError("AnalyzeController::AnalyzeRemote::Exception::{0}", ex.Message);
                return null;
            }
        }
    }
}
