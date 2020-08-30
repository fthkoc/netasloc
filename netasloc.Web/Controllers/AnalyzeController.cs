using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netasloc.Core.DTO;
using netasloc.Core.Models;
using netasloc.Core.Services;
using netasloc.Web.Models.ViewModels;

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

        public IActionResult Create()
        {
            string resultFilePath = _configuration.GetSection("AnalyzeConfiguration:ResultFilePath").Get<string>();
            string[] trackedRepositories = _configuration.GetSection("AnalyzeConfiguration:TrackedRepositories").Get<string[]>();

            LOCForAllResponse result = _locService.AnalyzeLOCForAll(trackedRepositories);

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

            AnalyzeResultDTO analyzeResult = _dataAccess.GetAllAnalyzeResults().ElementAt(0);
            return RedirectToAction("Details", new { id = analyzeResult.ID });
        }

        public IActionResult AnalyzeRemote()
        {
            string workingDirectoryPath = _configuration.GetSection("AnalyzeConfiguration:WorkingDirectoryPath").Get<string>();
            string resultsFolderName = _configuration.GetSection("AnalyzeConfiguration:ResultsFolderName").Get<string>();
            string[] remoteRepositories = _configuration.GetSection("AnalyzeConfiguration:RemoteRepositories").Get<string[]>();
            List<string> directoryFullPaths = new List<string>();

            if (!Directory.Exists(workingDirectoryPath))
                Directory.CreateDirectory(workingDirectoryPath);

            foreach (string repository in remoteRepositories)
            {
                string projectName = System.IO.Path.GetFileNameWithoutExtension(repository);
                string projectFullPath = Path.Combine(workingDirectoryPath, projectName);
                using (PowerShell powershell = PowerShell.Create())
                {
                    if (!Directory.Exists(projectFullPath))
                        powershell.AddScript(@"git clone" + " " + repository + " " + projectFullPath);
                    else
                    {
                        powershell.AddScript($"cd" + " " + projectFullPath);
                        powershell.AddScript($"git pull");
                    }
                    Collection<PSObject> results = powershell.Invoke();
                }
                directoryFullPaths.Add(projectFullPath);
            }

            var result = _locService.AnalyzeLOCForAll(directoryFullPaths.ToArray());

            string resultsFolderFullPath = Path.Combine(workingDirectoryPath, resultsFolderName);
            if (!Directory.Exists(resultsFolderFullPath))
                Directory.CreateDirectory(resultsFolderFullPath);

            string fileFullPath = Path.Combine(resultsFolderFullPath, "result_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json");

            if (System.IO.File.Exists(fileFullPath))
                throw new FileNotFoundException("{0} exists.", fileFullPath);

            var file = System.IO.File.Create(fileFullPath);
            var jsonResult = JsonSerializer.Serialize(result);
            file.Write(Encoding.Default.GetBytes(jsonResult), 0, jsonResult.Length);
            file.Close();

            return RedirectToAction("Index");
        }
    }
}
