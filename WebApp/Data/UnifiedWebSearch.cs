using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApp.Data
{
    public static class UnifiedWebSearch
    {
        // Default folder    
        static readonly string rootFolder = "~/Views/";

        public static List<ViewModels.SearchResultModel> LocalFileSearch(string keywords)
        {
            if (string.IsNullOrEmpty(keywords) == true) return new List<ViewModels.SearchResultModel>();

            var fullPath = HttpContext.Current.Server.MapPath(rootFolder);
            List<String> fileList = DirectorySearch(fullPath);

            var result = new List<ViewModels.SearchResultModel>();
            foreach (string file in fileList)
            {
                var profile = ReadFileContent(file, keywords);
                if (profile.Item1 == false) continue;

                result.Add(new ViewModels.SearchResultModel()
                {
                    Description = Substring(profile.Item2),
                    PagePath = GetVirtualPath(file),
                    Title = Substring(profile.Item2, 0, 10),
                });
            }

            return result;
        }

        private static List<String> DirectorySearch(string sDir)
        {
            if (string.IsNullOrEmpty(sDir) == true) return null;

            List<String> files = new List<String>();

            foreach (string f in Directory.GetFiles(sDir, "*.cshtml", SearchOption.AllDirectories))
            {
                files.Add(f);
            }

            return files;
        }

        private static Tuple<bool, string> ReadFileContent(string filePath, string keywords)
        {
            //Create an object of FileInfo for specified path            
            FileInfo fi = new FileInfo(filePath);

            //Open a file for Read\Write
            using (FileStream fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                //Create an object of StreamReader by passing FileStream object on which it needs to operates on
                using (StreamReader sr = new StreamReader(fs))
                {

                    //Use the ReadToEnd method to read all the content from file
                    string fileContent = sr.ReadToEnd();
                    if (fileContent == null) return new Tuple<bool, string>(false, null);
                    fileContent = fileContent.ToLower().Trim();

                    if (fileContent.Contains(keywords.ToLower().Trim()) == true)
                    {
                        fileContent = HighlightKeyWords(fileContent, keywords, "", false);
                        return new Tuple<bool, string>(true, fileContent);
                    }

                    //Close the StreamReader object after operation
                    sr.Close();
                    fs.Close();
                }
            }

            return new Tuple<bool, string>(false, null);

        }

        private static string HighlightKeyWords(this string text, string keywords, string cssClass, bool fullMatch)
        {
            if (text == String.Empty || keywords == String.Empty || cssClass == String.Empty)
                return text;
            var words = keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!fullMatch)
                return words.Select(word => word.Trim()).Aggregate(text,
                             (current, pattern) =>
                             Regex.Replace(current,
                                             pattern,
                                               string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                               cssClass,
                                               "$0"),
                                               RegexOptions.IgnoreCase));
            return words.Select(word => "\\b" + word.Trim() + "\\b")
                        .Aggregate(text, (current, pattern) =>
                                  Regex.Replace(current,
                                  pattern,
                                    string.Format("<span style=\"background-color:{0}\">{1}</span>",
                                    cssClass,
                                    "$0"),
                                    RegexOptions.IgnoreCase));

        }

        private static string GetVirtualPath(string physicalPath)
        {
            string rootpath = HttpContext.Current.Server.MapPath("~/");
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");
            var newURL =  String.Format("{0}://{1}/{2}", HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority, physicalPath);

            newURL = newURL.Replace("Views/","").Replace(".cshtml","").Trim();
            return newURL;
        }

        private static string Substring(string value, int startIndex = 0, int length = 20)
        {
            if (value.Length > length)
            {
                string substring = value.Substring(startIndex, length);
                return (substring);
            }
            return value;
        }

    }
}