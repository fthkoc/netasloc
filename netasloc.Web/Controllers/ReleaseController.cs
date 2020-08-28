using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.DTO;
using netasloc.Core.Services;
using netasloc.Web.Models.ViewModels;
using System;
using System.Linq;

namespace netasloc.Web.Controllers
{
    public class ReleaseController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly _IDataAccessService _dataAccess;

        public ReleaseController(ILogger<HomeController> logger, _IDataAccessService dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("ReleaseController::Index::called.");
            ReleasesViewModel model = new ReleasesViewModel();
            try
            {
                model.Releases = _dataAccess.GetAllReleases().Take(5).Reverse().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseController::Index::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("ReleaseController::Index::finished.");
            return View(model);
        }

        public IActionResult Details(uint id)
        {
            _logger.LogInformation("ReleaseController::Details::called.");
            ReleaseDetailsViewModel model = new ReleaseDetailsViewModel();
            try
            {
                model.Release = _dataAccess.GetReleaseByID(id);
                model.AnalyzeResults = _dataAccess.GetAnalyzeResultsForRelease(model.Release.ReleaseStart, model.Release.ReleaseEnd).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseController::Details::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("ReleaseController::Details::finished.");
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ReleaseCode,ReleaseStart,ReleaseEnd")] ReleaseDTO release)
        {
            _logger.LogInformation("ReleaseController::Create::called.");
            try
            {
                if (ModelState.IsValid)
                {
                    var analyzeResults = _dataAccess.GetAnalyzeResultsForRelease(release.ReleaseStart, release.ReleaseEnd).ToList();
                    if (analyzeResults.Count > 0)
                    {
                        var lastAnalyze = analyzeResults.ElementAt(0);
                        var firstAnalyze = analyzeResults.ElementAt(analyzeResults.Count - 1);
                        release.CreatedAt = DateTime.Now;
                        release.UpdatedAt = DateTime.Now;
                        release.TotalLineCount = lastAnalyze.TotalLineCount;
                        release.CommentLineCount = lastAnalyze.CommentLineCount;
                        release.EmptyLineCount = lastAnalyze.EmptyLineCount;
                        release.CodeLineCount = lastAnalyze.CodeLineCount;
                        release.DifferenceLOC = (int)lastAnalyze.TotalLineCount - ((int)firstAnalyze.TotalLineCount + firstAnalyze.DifferenceLOC);
                        release.DifferenceSLOC = (int)lastAnalyze.CodeLineCount - ((int)firstAnalyze.CodeLineCount + firstAnalyze.DifferenceSLOC);
                        bool isCreated = _dataAccess.CreateRelease(release);
                        if (isCreated)
                        {
                            ReleaseDTO createdRelease = _dataAccess.GetAllReleases().ElementAt(0);
                            return RedirectToAction("Details", new { id = createdRelease.ID });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseController::Create::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("ReleaseController::Create::finished.");
            return View(release);
        }
    }
}
