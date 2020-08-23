﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using netasloc.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace netasloc.Core.Services
{
    public class LOCService : _ILOCService
    {
        private static readonly int FIRST_RELEASE_YEAR = 2016;
        private static readonly int CYCLE_WEEK_COUNT = 5;
        private static readonly string LANGUAGES_FILE = "languages.json";
        private static List<Language> Languages { get; set; } = new List<Language>();
        private static List<string> SupportedExtensions { get; set; } = new List<string>();

        private readonly ILogger<DataAccessService> _logger;
        private readonly _IDataAccessService _dataAccess;

        public LOCService(ILogger<DataAccessService> logger, _IDataAccessService dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;

            string rawData = File.ReadAllText(LANGUAGES_FILE);
            dynamic json = JObject.Parse(rawData);
            var jsonArray = json.languages as JArray;

            foreach (var language in jsonArray)
                Languages.Add(JsonConvert.DeserializeObject<Language>(language.ToString()));
            foreach (var item in Languages)
                SupportedExtensions.AddRange(item.FileExtensions);
        }

        /// <summary>
        /// Analyzes given directories, calculates LOC, SLOC data
        /// </summary>
        /// <param name="directoryFullPaths">List of directories</param>
        /// <returns>Analyze results for given directory</returns>
        public LOCForAllResponse AnalyzeLOCForAll(IEnumerable<string> directoryFullPaths)
        {
            _logger.LogInformation("LOCService::AnalyzeLOCForAll::called.");
            LOCForAllResponse response = new LOCForAllResponse();
            try
            {
                foreach (string directory in directoryFullPaths)
                {
                    if (!(response.AllDirectoriesData.ContainsKey(directory)))
                    {
                        LOCForDirectoryResponse dirResult = AnalyzeLOCForDirectory(directory);
                        response.FileCount += dirResult.FileCount;
                        response.TotalLineCount += dirResult.TotalLineCount;
                        response.CodeLineCount += dirResult.CodeLineCount;
                        response.CommentLineCount += dirResult.CommentLineCount;
                        response.EmptyLineCount += dirResult.EmptyLineCount;
                        response.AllDirectoriesData.Add(directory, dirResult);

                        string[] tempPath = dirResult.DirectoryFullPath.Split("\\");
                        string projectName = tempPath[tempPath.Length - 1];

                        _dataAccess.CreateDirectory(new DTO.DirectoryDTO()
                        {
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            ProjectName = projectName,
                            FullPath = dirResult.DirectoryFullPath,
                            FileCount = dirResult.FileCount,
                            TotalLineCount = dirResult.TotalLineCount,
                            CodeLineCount = dirResult.CodeLineCount,
                            CommentLineCount = dirResult.CommentLineCount,
                            EmptyLineCount = dirResult.EmptyLineCount
                        });
                    }
                }
                int previousSLOC = 0;
                int previousLOC = 0;
                var previousReleases = _dataAccess.GetAllReleases();
                if (previousReleases.Count() > 0)
                {
                    previousSLOC = (int) previousReleases.ElementAt(0).CodeLineCount;
                    previousLOC = (int) previousReleases.ElementAt(0).TotalLineCount;
                }
                response.DifferenceSLOC = ((int) response.CodeLineCount) - previousSLOC;
                response.DifferenceLOC = ((int) response.TotalLineCount) - previousLOC;

                _dataAccess.CreateRelease(new DTO.ReleaseDTO()
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ReleaseCode = GetReleaseCode(),
                    TotalLineCount = response.TotalLineCount,
                    CodeLineCount = response.CodeLineCount,
                    CommentLineCount = response.CommentLineCount,
                    EmptyLineCount = response.EmptyLineCount,
                    DifferenceSLOC = response.DifferenceSLOC,
                    DifferenceLOC = response.DifferenceLOC
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::AnalyzeLOCForAll::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("LOCService::AnalyzeLOCForAll::finished.");
            return response;
        }

        /// <summary>
        /// Analyzes given directory, calculates LOC, SLOC data
        /// </summary>
        /// <param name="directoryFullPath">Directory full path</param>
        /// <returns>Analyze results for given file</returns>
        public LOCForDirectoryResponse AnalyzeLOCForDirectory(string directoryFullPath)
        {
            _logger.LogInformation("LOCService::AnalyzeLOCForDirectory::called. directoryFullPath:{0}", directoryFullPath);
            LOCForDirectoryResponse response = new LOCForDirectoryResponse();
            try
            {
                string[] fileNames = GetAllFiles(directoryFullPath);
                foreach (var file in fileNames)
                {
                    var fileDirectory = Path.GetDirectoryName(file);
                    var fileName = Path.GetFileName(file);
                    var fileExtension = Path.GetExtension(file);
                    var fileLanguage = GetLanguageForExtension(fileExtension);
                    var currentFileResponse = AnalyzeLOCForSingleFile(fileDirectory, fileName, fileExtension);
                    // If current language has no entry in the dictionary, add new entry
                    if (!response.AllLanguagesData.ContainsKey(fileLanguage))
                        response.AllLanguagesData.Add(fileLanguage, new LOCForLanguage());
                    // If current file extension has no entry in the dictionary, add new entry
                    if (!response.AllLanguagesData[fileLanguage].AllExtensionsData.ContainsKey(fileExtension))
                        response.AllLanguagesData[fileLanguage].AllExtensionsData.Add(fileExtension, new LOCForFileExtension());
                    // Add single file data to the current file extension
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].AllFilesData.Add(currentFileResponse);
                    // Update data for the current file extension
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].FileExtension = fileExtension;
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].FileCount++;
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].TotalLineCount += currentFileResponse.TotalLineCount;
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].CodeLineCount += currentFileResponse.CodeLineCount;
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].CommentLineCount += currentFileResponse.CommentLineCount;
                    response.AllLanguagesData[fileLanguage].AllExtensionsData[fileExtension].EmptyLineCount += currentFileResponse.EmptyLineCount;
                    // Update data for the current language
                    response.AllLanguagesData[fileLanguage].FileLanguage = fileLanguage;
                    response.AllLanguagesData[fileLanguage].FileCount++;
                    response.AllLanguagesData[fileLanguage].TotalLineCount += currentFileResponse.TotalLineCount;
                    response.AllLanguagesData[fileLanguage].CodeLineCount += currentFileResponse.CodeLineCount;
                    response.AllLanguagesData[fileLanguage].CommentLineCount += currentFileResponse.CommentLineCount;
                    response.AllLanguagesData[fileLanguage].EmptyLineCount += currentFileResponse.EmptyLineCount;
                    // Update data for all
                    response.DirectoryFullPath = directoryFullPath;
                    response.FileCount++;
                    response.TotalLineCount += currentFileResponse.TotalLineCount;
                    response.CodeLineCount += currentFileResponse.CodeLineCount;
                    response.CommentLineCount += currentFileResponse.CommentLineCount;
                    response.EmptyLineCount += currentFileResponse.EmptyLineCount;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::AnalyzeLOCForDirectory::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("LOCService::AnalyzeLOCForDirectory::finished. directoryFullPath:{0}, FileCount:{1}, Total:{2}, Comment:{2}, Empty:{3}, Code:{4}", 
                directoryFullPath, response.FileCount, response.TotalLineCount, response.CommentLineCount, response.EmptyLineCount, response.CodeLineCount);
            return response;
        }

        /// <summary>
        /// Analyzes given file, calculates LOC, SLOC data
        /// </summary>
        /// <param name="fileDirectory">Directory full path</param>
        /// <param name="fileName">File name</param>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Analyze results for given file</returns>
        public LOCForSingleFileResponse AnalyzeLOCForSingleFile(string fileDirectory, string fileName, string fileExtension)
        {
            _logger.LogInformation("LOCService::AnalyzeLOCForSingleFile::called. fileName:{0}", fileName);
            LOCForSingleFileResponse response = new LOCForSingleFileResponse();
            try
            {
                string fileFullPath = Path.Combine(fileDirectory, fileName);
                if (File.Exists(fileFullPath))
                {
                    // Read all text from file
                    string rawData = File.ReadAllText(fileFullPath);
                    string[] rawLines = rawData.Split("\n");
                    // Get proper RegEx pattern for the current file extension and delete all block comments from file.
                    string blockCommentPattern = GetBlockCommentRegExPattern(fileExtension);
                    if (string.IsNullOrEmpty(blockCommentPattern))
                        _logger.LogWarning("LOCService::AnalyzeLOCForSingleFile:: {0} is not supported.", fileExtension);
                    else
                        rawData = Regex.Replace(rawData, blockCommentPattern, GetLineCommentCharacters(fileExtension));
                    // Split raw data to lines and clear it from whitespaces, increment the right group count.
                    string[] lines = rawData.Split("\n");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].Trim();
                        if (IsEmptyLine(lines[i]))
                            response.EmptyLineCount++;
                        else if (IsCommentLine(lines[i], fileExtension))
                            response.CommentLineCount++;
                        else
                            response.CodeLineCount++;
                    }
                    // Calculate all comment blocks total line count and add it to the result
                    response.CommentLineCount += (uint)(rawLines.Length - lines.Length);
                    // Update response data
                    response.TotalLineCount = response.EmptyLineCount + response.CommentLineCount + response.CodeLineCount;
                    response.FileName = fileName;
                    response.FileDirectory = fileDirectory;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::AnalyzeLOCForSingleFile::Exception::{0}", ex.Message);
                throw;
            }
            _logger.LogInformation("LOCService::AnalyzeLOCForSingleFile::finished. FileName:{0}, Total:{1}, Comment:{2}, Empty:{3}, Code:{4}", 
                response.FileName, response.TotalLineCount, response.CommentLineCount, response.EmptyLineCount, response.CodeLineCount);
            return response;
        }

        /// <summary>
        /// Returns an array of file paths with supported file extensions which contains below the input directory
        /// </summary>
        /// <param name="directoryFullPath">Top level directory path to perform search</param>
        /// <returns>Array of file paths</returns>
        private string[] GetAllFiles(string directoryFullPath)
        {
            List<string> result = new List<string>();
            string[] allFiles = Directory.GetFiles(directoryFullPath, "*", SearchOption.AllDirectories);
            foreach (var item in allFiles)
                if (SupportedExtensions.Contains(Path.GetExtension(item)))
                    result.Add(item);
            return result.ToArray();
        }

        /// <summary>
        /// Checks given line is represents single line comment or not
        /// </summary>
        /// <param name="line">Single line from file</param>
        /// <param name="extension">File Extension</param>
        /// <returns>True if the line is a single line comment</returns>
        private bool IsCommentLine(string line, string extension)
        {
            string indicator = GetLineCommentCharacters(extension);
            if (!string.IsNullOrEmpty(indicator))
                if (line.Length >= indicator.Length && line.Substring(0, indicator.Length) == indicator)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks given line is empty or not
        /// </summary>
        /// <param name="line">Single line from file</param>
        /// <returns>True if the line is empty</returns>
        private bool IsEmptyLine(string line)
        {
            return string.IsNullOrEmpty(line);
        }

        /// <summary>
        /// Returns Regular Expression pattern to detect block comments for the language that is related to given file extension
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns>Regular Expression pattern for block comments</returns>
        private string GetBlockCommentRegExPattern(string extension)
        {
            string[] blockCommentCharacters = null;
            string blockCommentStart = null;

            foreach (var language in Languages)
                foreach (var item in language.FileExtensions)
                    if (item == extension)
                        if (language.BlockCommentCharacters.Length > 0)
                            blockCommentCharacters = language.BlockCommentCharacters;
                        else
                            blockCommentCharacters = null;

            if (blockCommentCharacters != null)
                blockCommentStart = blockCommentCharacters[0];

            switch (blockCommentStart)
            {
                case "/*":
                    return "\\/\\*(\\*(?!\\/)|[^*])*\\*\\/";
                case "<!--":
                    return "\\<\\!\\-\\-(\\*(?!\\/)|[^*]).*\\-\\-\\>";
                case "\"\"\"":
                    return "\"\"\"[\\s\\S]*?\"\"\"";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Returns single line comment characters for the language that is related to given file extension
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns>Line comment characters for given file extension</returns>
        private string GetLineCommentCharacters(string extension)
        {
            foreach (var language in Languages)
                foreach (var item in language.FileExtensions)
                    if (item == extension)
                        if (language.LineCommentCharacters.Length > 0)
                            return language.LineCommentCharacters;
                        else
                            return "";
            return "";
        }

        /// <summary>
        /// Returns language alias for given file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Language alias</returns>
        private string GetLanguageForExtension(string fileExtension)
        {
            foreach (var item in Languages)
                if (item.FileExtensions.Contains(fileExtension))
                    return item.Alias;
            return "";
        }

        /// <summary>
        /// Generates release code for current sprint cycle
        /// </summary>
        /// <returns>Release code for current sprint cycle</returns>
        private string GetReleaseCode()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            // How many years passed since the first release
            int yearDifference = DateTime.Now.Year - FIRST_RELEASE_YEAR;
            // How many sprint cycle passed this year
            int weekCount = cultureInfo.Calendar.GetWeekOfYear(DateTime.Now, cultureInfo.DateTimeFormat.CalendarWeekRule, cultureInfo.DateTimeFormat.FirstDayOfWeek);
            // Current sprint cycle number
            int cycleNumber = ((yearDifference * 52) + weekCount) / CYCLE_WEEK_COUNT;
            // Release count for current sprint cycle
            int releaseCount = _dataAccess.GetAllReleases().Where(x => x.ReleaseCode.Contains(cycleNumber.ToString())).Count() + 1;
            return cycleNumber.ToString() + "." + releaseCount.ToString();
        }
    }
}
