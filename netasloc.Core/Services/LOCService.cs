using Microsoft.Extensions.Configuration;
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
                    var fileLanguage = GetAlias(fileExtension);
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
                    char[] delimiters = new char[] { '\n' };
                    string[] rawLines = rawData.Split(delimiters);
                    for (int i = 0; i < rawLines.Length; i++)
                    {
                        rawLines[i] = rawLines[i].Trim();
                        if (IsEmptyLine(rawLines[i]))
                            response.EmptyLineCount++;
                    }
                    // Get proper RegEx pattern for the current file extension and delete all block comments from file.
                    string blockCommentPattern = GetBlockCommentRegExPattern(fileExtension);
                    string stringPattern = GetStringRegExPattern(fileExtension);

                    if (string.IsNullOrEmpty(blockCommentPattern))
                    {
                        _logger.LogWarning("LOCService::AnalyzeLOCForSingleFile:: {0} doesn't have a defined block comment pattern.", fileExtension);
                    }
                    else
                    {
                        // Find comment blocks and remove empty lines in comment block from total
                        MatchCollection commentBlocks = Regex.Matches(rawData, blockCommentPattern);
                        foreach (Match block in commentBlocks)
                        {
                            string[] blockLines = block.Value.Split(delimiters);
                            for (int i = 0; i < blockLines.Length; i++)
                            {
                                blockLines[i] = blockLines[i].Trim();
                                if (IsEmptyLine(blockLines[i]))
                                    response.EmptyLineCount--;
                            }
                        }
                        // Replace comment blocks with a single empty line
                        rawData = Regex.Replace(rawData, blockCommentPattern, "");
                    }
                    // Split raw data to lines and clear it from whitespaces, increment the right group count.
                    string[] lines = rawData.Split(delimiters);
                    uint updatedEmptyLines = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].Trim();
                        if (IsEmptyLine(lines[i]))
                            updatedEmptyLines++;
                        else if (IsCommentLine(lines[i], fileExtension))
                            response.CommentLineCount++;
                        else
                            response.CodeLineCount++;
                    }
                    // Calculate comment lines by using replaced single empty line counts
                    uint blockCommentsTotalLineCount = (uint) (rawLines.Length - lines.Length);
                    uint blockCommentCount = updatedEmptyLines - response.EmptyLineCount;
                    response.CommentLineCount += (blockCommentsTotalLineCount + blockCommentCount);
                    // Update response data
                    response.TotalLineCount = (uint) rawLines.Length;
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
            //_logger.LogInformation("LOCService::GetAllFiles::called. directoryFullPath:{0}", directoryFullPath);
            List<string> result = new List<string>();
            try
            {
                string[] allFiles = Directory.GetFiles(directoryFullPath, "*", SearchOption.AllDirectories);
                foreach (var item in allFiles)
                    if (SupportedExtensions.Contains(Path.GetExtension(item)))
                        result.Add(item);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetAllFiles::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetAllFiles::finished.");
            return result.ToArray();
        }

        /// <summary>
        /// Checks given line is represents single line comment or not
        /// </summary>
        /// <param name="line">Single line from file</param>
        /// <param name="fileExtension">File Extension</param>
        /// <returns>True if the line is a single line comment</returns>
        private bool IsCommentLine(string line, string fileExtension)
        {
            //_logger.LogInformation("LOCService::IsCommentLine::called. fileExtension:{0}", fileExtension);
            try
            {
                string indicator = GetLineCommentCharacters(fileExtension);
                if (!string.IsNullOrEmpty(indicator))
                {
                    if (line.Length >= indicator.Length && line.Substring(0, indicator.Length) == indicator)
                    {
                        //_logger.LogInformation("LOCService::IsCommentLine::finished. true");
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::IsCommentLine::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::IsCommentLine::finished. false");
            return false;
        }

        /// <summary>
        /// Checks given line is empty or not
        /// </summary>
        /// <param name="line">Single line from file</param>
        /// <returns>True if the line is empty</returns>
        private bool IsEmptyLine(string line)
        {
            //_logger.LogInformation("LOCService::IsEmptyLine::called.");
            bool result = string.IsNullOrEmpty(line);
            //_logger.LogInformation("LOCService::IsEmptyLine::finished. result:{0}", result);
            return result;
        }

        /// <summary>
        /// Returns Regular Expression pattern to detect block comments for the language that is related to given file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Regular Expression pattern for block comments</returns>
        private string GetBlockCommentRegExPattern(string fileExtension)
        {
            //_logger.LogInformation("LOCService::GetBlockCommentRegExPattern::called. fileExtension:{0}", fileExtension);
            try
            {
                string[] blockCommentCharacters = GetBlockCommentCharacters(fileExtension);
                string blockCommentStart = null;
                string blockCommentEnd = null;
                string regexPattern = "";

                if (blockCommentCharacters != null && blockCommentCharacters.Length > 1)
                {
                    blockCommentStart = blockCommentCharacters[0];
                    blockCommentEnd = blockCommentCharacters[1];

                    foreach (char character in blockCommentStart)
                        regexPattern += "\\" + character.ToString();

                    regexPattern += "[\\s\\S]*?";

                    foreach (char character in blockCommentEnd)
                        regexPattern += "\\" + character.ToString();

                    //_logger.LogInformation("LOCService::GetBlockCommentRegExPattern::finished. regexPattern:{0}", regexPattern);
                    return regexPattern;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetBlockCommentRegExPattern::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetBlockCommentRegExPattern::finished. null");
            return null;
        }

        private string GetStringRegExPattern(string fileExtension)
        {
            //_logger.LogInformation("LOCService::GetStringRegExPattern::called. fileExtension:{0}", fileExtension);
            try
            {
                char stringChar = GetStringCharacter(fileExtension);
                string[] blockCommentCharacters = GetBlockCommentCharacters(fileExtension);
                string blockCommentStart = null;
                string blockCommentEnd = null;
                string regexPattern = "";

                // \"[^\\n\\r]*?\\/\\*[\\s\\S]*?\\*\\/[^\n\r]*?\"

                if (blockCommentCharacters != null && blockCommentCharacters.Length > 1)
                {
                    blockCommentStart = blockCommentCharacters[0];
                    blockCommentEnd = blockCommentCharacters[1];

                    regexPattern += "\\" + stringChar.ToString();
                    regexPattern += "[^\\n\\r]*?";
                    foreach (char character in blockCommentStart)
                        regexPattern += "\\" + character.ToString();
                    regexPattern += "[\\s\\S]*?";
                    foreach (char character in blockCommentEnd)
                        regexPattern += "\\" + character.ToString();
                    regexPattern += "[^\\n\\r]*?";
                    regexPattern += "\\" + stringChar.ToString();
                    //_logger.LogInformation("LOCService::GetBlockCommentRegExPattern::finished. regexPattern:{0}", regexPattern);
                    return regexPattern;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetStringRegExPattern::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetStringRegExPattern::finished. null");
            return null;
        }

        /// <summary>
        /// Returns language alias for given file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Language alias</returns>
        private string GetAlias(string fileExtension)
        {
            //_logger.LogInformation("LOCService::GetAlias::called. fileExtension:{0}", fileExtension);
            try
            {
                foreach (var language in Languages)
                {
                    if (language.FileExtensions.Contains(fileExtension))
                    {
                        //_logger.LogInformation("LOCService::GetAlias::finished. Alias:{0}", language.Alias);
                        return language.Alias;
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetAlias::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetAlias::finished. null");
            return null;
        }

        /// <summary>
        /// Returns single line comment characters for the language that is related to given file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Line comment characters for given file extension</returns>
        private string GetLineCommentCharacters(string fileExtension)
        {
            //_logger.LogInformation("LOCService::GetLineCommentCharacters::called. fileExtension:{0}", fileExtension);
            try
            {
                foreach (var language in Languages)
                {
                    if (language.FileExtensions.Contains(fileExtension))
                    {
                        //_logger.LogInformation("LOCService::GetLineCommentCharacters::finished. LineCommentCharacters:{0}", language.LineCommentCharacters);
                        return language.LineCommentCharacters;
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetLineCommentCharacters::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetLineCommentCharacters::finished. null");
            return null;
        }

        /// <summary>
        /// Returns block comment characters for the language that is related to given file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <returns>Block comment characters for given file extension</returns>
        private string[] GetBlockCommentCharacters(string fileExtension)
        {
            //_logger.LogInformation("LOCService::GetBlockCommentCharacters::called. fileExtension:{0}", fileExtension);
            try
            {
                foreach (var language in Languages)
                {
                    if (language.FileExtensions.Contains(fileExtension))
                    {
                        //_logger.LogInformation("LOCService::GetBlockCommentCharacters::finished. BlockCommentCharacters:{0}", language.BlockCommentCharacters);
                        return language.BlockCommentCharacters;
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetBlockCommentCharacters::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetBlockCommentCharacters::finished. null");
            return null;
        }

        private char GetStringCharacter(string fileExtension)
        {
            return '\"';
        }

        /// <summary>
        /// Generates release code for current sprint cycle
        /// </summary>
        /// <returns>Release code for current sprint cycle</returns>
        private string GetReleaseCode()
        {
            //_logger.LogInformation("LOCService::GetReleaseCode::called.");
            string releaseCode = "";
            try
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
                releaseCode = cycleNumber.ToString() + "." + releaseCount.ToString();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("LOCService::GetReleaseCode::Exception::{0}", ex.Message);
                throw;
            }
            //_logger.LogInformation("LOCService::GetReleaseCode::finished. releaseCode:{0}", releaseCode);
            return releaseCode;
        }
    }
}
