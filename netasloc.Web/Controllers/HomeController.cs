using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.Services;
using netasloc.Web.Models;

namespace netasloc.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly _IDataAccessService _dataAccess;

        public HomeController(ILogger<HomeController> logger, _IDataAccessService dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("HomeController::Index::called.");
            var model = new IndexViewModel();
            try
            {
                model.Releases = _dataAccess.GetAllReleases().Reverse();

                var directories = _dataAccess.GetLastAnalyzedDirectories();
                model.Directories.projectNames = JsonSerializer.Serialize(directories.Select(x => x.ProjectName));
                model.Directories.fileCounts = JsonSerializer.Serialize(directories.Select(x => x.FileCount));
                model.Directories.totalLines = JsonSerializer.Serialize(directories.Select(x => x.TotalLineCount));
                model.Directories.totalCodeLines = JsonSerializer.Serialize(directories.Select(x => x.CodeLineCount));
                model.Directories.totalCommentLines = JsonSerializer.Serialize(directories.Select(x => x.CommentLineCount));
                model.Directories.totalEmptyLines = JsonSerializer.Serialize(directories.Select(x => x.EmptyLineCount));

                DateTime date = directories.Count() > 0 ? directories.ElementAt(0).CreatedAt : DateTime.MinValue;
                model.Directories.date = JsonSerializer.Serialize(date.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController::Index::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("HomeController::Index::finished.");
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
