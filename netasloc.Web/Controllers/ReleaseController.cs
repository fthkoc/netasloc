using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using netasloc.Core.DTO;
using netasloc.Core.Services;
using netasloc.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                model.Releases = _dataAccess.GetAllReleases().OrderByDescending(x => x.ReleaseCode).Take(10).ToList();
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
                        var previousReleases = _dataAccess.GetAllReleases();
                        var lastAnalyze = analyzeResults.ElementAt(0);
                        release.CreatedAt = DateTime.Now;
                        release.UpdatedAt = DateTime.Now;
                        release.TotalLineCount = lastAnalyze.TotalLineCount;
                        release.CommentLineCount = lastAnalyze.CommentLineCount;
                        release.EmptyLineCount = lastAnalyze.EmptyLineCount;
                        release.CodeLineCount = lastAnalyze.CodeLineCount;
                        if (previousReleases.Count() == 0)
                        {
                            release.DifferenceLOC = (int)lastAnalyze.TotalLineCount;
                            release.DifferenceSLOC = (int)lastAnalyze.CodeLineCount;
                        }
                        else
                        {
                            release.DifferenceLOC = ((int)lastAnalyze.TotalLineCount - (int)previousReleases.ElementAt(0).TotalLineCount);
                            release.DifferenceSLOC = ((int)lastAnalyze.CodeLineCount - (int)previousReleases.ElementAt(0).CodeLineCount);
                        }
                        bool isCreated = _dataAccess.CreateRelease(release);
                        if (isCreated)
                        {
                            ReleaseDTO createdRelease = _dataAccess.GetAllReleases().ElementAt(0);
                            return RedirectToAction("Details", new { id = createdRelease.ID });
                        }
                        else
                        {
                            throw new Exception("CreateRelease error!");
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

        public IActionResult Delete(uint id)
        {
            _logger.LogInformation("ReleaseController::Delete::called.");
            ReleaseDetailsViewModel model = new ReleaseDetailsViewModel();
            try
            {
                model.Release = _dataAccess.GetReleaseByID(id);
                model.AnalyzeResults = _dataAccess.GetAnalyzeResultsForRelease(model.Release.ReleaseStart, model.Release.ReleaseEnd).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseController::Delete::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("ReleaseController::Delete::finished.");
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(uint id)
        {
            _logger.LogInformation("ReleaseController::DeleteConfirmed::called.");
            try
            {
                bool isDeleted = _dataAccess.DeleteRelease(id);
                if (!isDeleted)
                    throw new Exception("DeleteRelease error!");
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseController::DeleteConfirmed::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("ReleaseController::DeleteConfirmed::finished.");
            return RedirectToAction("Index");
        }

        public IActionResult ExportCSV()
        {
            _logger.LogInformation("ReleaseController::ExportCSV::called.");
            string resultFileName = "releases_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            List<ReleaseDTO> releases;
            string fileContent = "ID,CreatedAt,UpdatedAt,ReleaseCode,ReleaseStart,ReleaseEnd,TotalLineCount,CodeLineCount,CommentLineCount,EmptyLineCount,DifferenceSLOC,DifferenceLOC";
            try
            {
                releases = _dataAccess.GetAllReleases().ToList();
                foreach (var release in releases)
                {
                    fileContent += "\n";
                    fileContent += release.ID.ToString() + ",";
                    fileContent += release.CreatedAt.ToString() + ",";
                    fileContent += release.UpdatedAt.ToString() + ",";
                    fileContent += release.ReleaseCode.ToString() + ",";
                    fileContent += release.ReleaseStart.ToString() + ",";
                    fileContent += release.ReleaseEnd.ToString() + ",";
                    fileContent += release.TotalLineCount.ToString() + ",";
                    fileContent += release.CodeLineCount.ToString() + ",";
                    fileContent += release.CommentLineCount.ToString() + ",";
                    fileContent += release.EmptyLineCount.ToString() + ",";
                    fileContent += release.DifferenceSLOC.ToString() + ",";
                    fileContent += release.DifferenceLOC.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReleaseController::ExportCSV::Exception::{0}", ex.Message);
            }
            _logger.LogInformation("ReleaseController::ExportCSV::finished.");
            return File(Encoding.Default.GetBytes(fileContent), "application/octet-stream", resultFileName);
        }
    }
}
