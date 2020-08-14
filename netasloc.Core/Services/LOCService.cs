using netasloc.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace netasloc.Core.Services
{
    /// <summary>
    /// Services to extract LOC data from a file or a repository
    /// </summary>
    public class LOCService : _ILOCService
    {
        private static readonly string LANGUAGES_FILE = "languages.json";
        private static List<Language> Languages { get; set; } = new List<Language>();

        private readonly _IDataAccessService _dataAccess;

        /// <summary>
        /// Constructor, reads languages from 'languages.json'
        /// </summary>
        public LOCService(_IDataAccessService dataAccess)
        {
            string rawData = File.ReadAllText(LANGUAGES_FILE);
            dynamic json = JObject.Parse(rawData);
            var jsonArray = json.languages as JArray;

            foreach (var language in jsonArray)
                Languages.Add(JsonConvert.DeserializeObject<Language>(language.ToString()));

            _dataAccess = dataAccess;
        }

        /// <summary>
        /// Calculates LOC data for all given directories
        /// </summary>
        /// <param name="request">List of directories</param>
        /// <returns>LOC data for all given directories</returns>
        public LOCForAllResponse AnalyzeLOCForAll(IEnumerable<string> directoryFullPaths)
        {
            LOCForAllResponse response = new LOCForAllResponse();

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

                    _dataAccess.Create(new DTO.DirectoryDTO()
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

            return response;
        }

        /// <summary>
        /// Gets a directory and calculates LOC data for all supported files in input directory
        /// </summary>
        /// <param name="request">Top level directory full path</param>
        /// <returns>LOC data of all supported files</returns>
        public LOCForDirectoryResponse AnalyzeLOCForDirectory(string directoryFullPath)
        {
            LOCForDirectoryResponse response = new LOCForDirectoryResponse();

            // Get all files with the proper file extension
            string[] fileNames = GetAllFiles(directoryFullPath);

            foreach (var file in fileNames)
            {
                var fileDirectory = Path.GetDirectoryName(file);
                var fileName = Path.GetFileName(file);
                var fileExtension = Path.GetExtension(file);
                // Calculate each file statistics
                LOCForSingleFileResponse currentFileResponse = AnalyzeLOCForSingleFile(fileDirectory, fileName, fileExtension);
                // If current file extension has an entry in the dictionary already
                if (response.AllTypesData.ContainsKey(fileExtension))
                {
                    // Update data for the current file extension
                    response.AllTypesData[fileExtension].TotalLineCount += currentFileResponse.TotalLineCount;
                    response.AllTypesData[fileExtension].CodeLineCount += currentFileResponse.CodeLineCount;
                    response.AllTypesData[fileExtension].CommentLineCount += currentFileResponse.CommentLineCount;
                    response.AllTypesData[fileExtension].EmptyLineCount += currentFileResponse.EmptyLineCount;
                    // Add single file data to the current file extension
                    response.AllTypesData[fileExtension].AllFilesData.Add(currentFileResponse);
                }
                else
                {
                    // Add new entry for the current file extension
                    response.AllTypesData.Add(fileExtension, new LOCDataForFileExtension()
                    {
                        FileExtension = fileExtension,
                        CodeLineCount = currentFileResponse.CodeLineCount,
                        CommentLineCount = currentFileResponse.CommentLineCount,
                        EmptyLineCount = currentFileResponse.EmptyLineCount,
                        TotalLineCount = currentFileResponse.TotalLineCount,
                        AllFilesData = new List<LOCForSingleFileResponse>()
                        {
                            currentFileResponse
                        }
                    });
                }
                // Update data for all types of files
                response.DirectoryFullPath = directoryFullPath;
                response.FileCount++;
                response.TotalLineCount += currentFileResponse.TotalLineCount;
                response.CodeLineCount += currentFileResponse.CodeLineCount;
                response.CommentLineCount += currentFileResponse.CommentLineCount;
                response.EmptyLineCount += currentFileResponse.EmptyLineCount;
                // Update data for the current file extension
                response.AllTypesData[fileExtension].FileCount++;
            }

            response.AllTypesData.OrderBy(x => x.Key);
            foreach (var item in response.AllTypesData.Values)
                item.AllFilesData.OrderBy(x => x.FileDirectory).ThenBy(x => x.FileName);

            return response;
        }

        /// <summary>
        /// Gets a single file data as an input and creates LOC data for it.
        /// </summary>
        /// <param name="request">File directory, file name, file extension</param>
        /// <returns>LOC data of given file</returns>
        public LOCForSingleFileResponse AnalyzeLOCForSingleFile(string fileDirectory, string fileName, string fileExtension)
        {
            LOCForSingleFileResponse response = new LOCForSingleFileResponse();
            string fileFullPath = Path.Combine(fileDirectory, fileName);

            if (File.Exists(fileFullPath))
            {
                // Read all text from file
                string rawData = File.ReadAllText(fileFullPath);
                string[] rawLines = rawData.Split("\n");
                // Get proper RegEx pattern for the current file extension and delete all block comments from file.
                string blockCommentPattern = GetBlockCommentRegExPattern(fileExtension);

                if (blockCommentPattern == null)
                    throw new ArgumentNullException("{0} doesn't have a RegEx definition in the program.", fileExtension);

                rawData = Regex.Replace(rawData, blockCommentPattern, "");
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

            return response;
        }

        /// <summary>
        /// Returns an array of file paths with supported file extensions which contains below the input directory
        /// </summary>
        /// <param name="directoryFullPath">Top level directory path to perform search</param>
        /// <returns>Array of file paths</returns>
        private string[] GetAllFiles(string directoryFullPath)
        {
            List<string> supportedExtensions = new List<string>();
            List<string> result = new List<string>();

            foreach (var item in Languages)
            {
                supportedExtensions.AddRange(item.FileExtensions);
            }

            foreach (var item in supportedExtensions)
            {
                result.AddRange(System.IO.Directory.GetFiles(directoryFullPath, "*" + item, SearchOption.AllDirectories));
            }

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

            if (string.IsNullOrEmpty(indicator))
                return false;

            int length = indicator.Length;
            if (line.Length >= length && line.Substring(0, length) == indicator)
                return true;
            else
                return false;
        }

        private bool IsEmptyLine(string line)
        {
            return String.IsNullOrEmpty(line);
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
            string blockCommentEnd = null;

            foreach (var language in Languages)
                foreach (var item in language.FileExtensions)
                    if (item == extension)
                        if (language.LineCommentCharacters.Length > 0)
                            blockCommentCharacters = language.BlockCommentCharacters[0];
                        else
                            blockCommentCharacters = null;

            if (blockCommentCharacters != null)
            {
                blockCommentStart = blockCommentCharacters[0];
                blockCommentEnd = blockCommentCharacters[1];
            }

            switch (blockCommentStart)
            {
                case "/*":
                    return "\\/\\*(\\*(?!\\/)|[^*])*\\*\\/";
                case "<!--":
                    return "\\<\\!\\-\\-(\\*(?!\\/)|[^*]).*\\-\\-\\>";
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
                            return language.LineCommentCharacters[0];
                        else
                            return null;
            return null;
        }
    }
}
