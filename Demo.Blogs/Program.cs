using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Demo.Blogs
{
    class Program
    {
        static bool codeStarting = false;
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var builder = new StringBuilder();

            using (var reader = System.IO.File.OpenText("markdown.md"))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var parseLine = ParserLine(line);
                    if (string.IsNullOrEmpty(parseLine)) continue;
                    builder.AppendLine(parseLine);
                }
            }
            builder.Remove(0, 6).AppendLine("</div>");
            builder.Append(@"<div class=""ws-content""><div class=""ws-copyright""> <p>转发请标明出处：https://www.cnblogs.com/WilsonPan/p/12704474.html</p></div></div>");


            await WriteToFileAsync(builder.ToString());

            Console.WriteLine("Finish");
        }
        private static readonly string titlePattern = @"^#{1}\s+(?<title>[\w\W]+)$";
        private static readonly string secondTitlePattern = @"^#{2,6}\s+(?<title>[\w\W]+)$";
        private static readonly string strongTitlePattern = @"\*{2}(?<title>\w+)\*{2}";
        private static readonly string linkPattern = @"\[(?<title>.*?)\]\((?<link>[^)]*)\)";

        static string ParserLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return string.Empty;
            if (line.StartsWith("---")) return string.Empty;

            if (line.StartsWith("```")) codeStarting = !codeStarting;

            if (codeStarting) return string.Empty;

            if (line.StartsWith("```"))
            {
                return @"<p style='color:red'>todo : 代码 </p>";
            }

            var titleMatch = Regex.Match(line, titlePattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (titleMatch.Success)
            {
                return $@"</div><p class=""ws-title"">{titleMatch.Groups["title"]}</p><div class='ws-content'><p>正文</p>";
            }
            var secondTitle = Regex.Match(line, secondTitlePattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (secondTitle.Success)
            {
                return $@"<p><strong>{secondTitle.Groups["title"]}</strong></p>";
            }
            var strongTitle = Regex.Match(line, strongTitlePattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (strongTitle.Success)
            {
                return $@"<p><strong>{strongTitle.Groups["title"]}</strong></p>";
            }
            var link = Regex.Match(line, linkPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (link.Success)
            {
                return $"<p><a href='{link.Groups["link"]}' title='{link.Groups["title"]}' target='_bank'>{link.Groups["title"]}</a></p>";
            }
            return $"<p>{WebUtility.HtmlEncode(line)}</p>"; ;
        }

        static async System.Threading.Tasks.Task WriteToFileAsync(string text)
        {
            if (System.IO.File.Exists("blog.html")) File.Delete("blog.html");
            using (var writer = System.IO.File.OpenWrite("blog.html"))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                await writer.WriteAsync(bytes, 0, bytes.Length);
                await writer.FlushAsync();
            }
        }
    }
}
