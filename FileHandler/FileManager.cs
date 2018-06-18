using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yt2mp3.FileHandler.Models;

namespace yt2mp3.FileHandler
{
    public static class FileManager
    {
        /// <summary>
        /// Get the raw string representation of file content.
        /// 
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"/>
        /// </summary>
        /// <param name="path">Path to the file to read</param>
        /// <returns>A new string containing all file content. Default string.Empty.</returns>
        public static string GetFileRawContent(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException();
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string result = string.Empty;

            try
            {
                result = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                // FIXME Logs
                throw e;
            }
            return result ?? string.Empty;
        }

        /// <summary>
        /// Delete a file from hard drive.
        /// 
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"/>
        /// /// </summary>
        /// <param name="path">Path to file to delete.</param>
        public static void DeleteFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException();
            if (!File.Exists(path))
                throw new FileNotFoundException();

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                // FIXME Logs
                throw e;
            }
        }

        /// <summary>
        /// Return the list of filenames in a directory matching extension parameter.
        /// Handy when both audio and video files are in same folder.
        /// Does not go in sub directory depth, only local.
        /// 
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"/>
        /// </summary>
        /// <param name="path">Full valid path to directory</param>
        /// <param name="extension">File extension without decoration (dot or whatever)</param>
        /// <returns></returns>
        public static IEnumerable<string> GetFileListFromDirectory(string path, string extension)
        {
            if (String.IsNullOrWhiteSpace(path) || String.IsNullOrWhiteSpace(extension))
                throw new ArgumentNullException();
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            IEnumerable<string> result = null;

            try
            {
                result = Directory.EnumerateFiles(path, $"*.{extension}", System.IO.SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                // FIXME Logs
                throw e;
            }
            return result;
        }

        /// <summary>
        /// Opens and read links csv file.
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>List of LinksFileModel</returns>
        public static IList<LinksModel> GetLinksListFromCsv(string path)
        {
            List<LinksModel> res = new List<LinksModel>();

            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { ";" });
                csvParser.HasFieldsEnclosedInQuotes = true;
                // csvParser.ReadLine() // if headers

                while (csvParser.EndOfData)
                {
                    var temp = new LinksModel();
                    string[] fields = csvParser.ReadFields();

                    if (!String.IsNullOrWhiteSpace(fields[0]))
                    {
                        temp.rawUrl = fields[0];
                        res.Add(temp);
                    }
                }
            }
            return res;
        }
    }
}
