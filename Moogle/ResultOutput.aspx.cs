using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Moogle
{
    public partial class ResultOutput : System.Web.UI.Page
    {
        //Converting pdf, doc and docx files into html and then it will show in web browser
        //http://www.aspose.com/community/forums/thread/438542/i-want-all-browser-support-to-show-different-file-format-in-a-common-control-like-iframe-or-any-othe.aspx
        string VirtualPath = Convert.ToString(ConfigurationSettings.AppSettings["VirtualPath"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            string strPath = Convert.ToString(Request.QueryString["rs"]);

            string filename = Path.GetFileName(strPath);

            string Extension = Path.GetExtension(strPath);

            //byte[] buffer = null;

            string MIMEType = string.Empty;
            MIMEType = GetMIMEType(filename);
            //text

            string[] splitpath = strPath.Split('/');

            string FilePath = VirtualPath;
            string FolderName = "";
            for (int i = 4; i < splitpath.Length - 1; i++)
            {
                FolderName = splitpath[i];
                if (FolderName != "" && i != splitpath.Length - 1)
                {
                    FilePath = FilePath + FolderName + "\\";
                }
                else
                {
                    FilePath = FilePath + FolderName;
                }
                FolderName = "";
            }

            //buffer = File.ReadAllBytes(FilePath.Replace("%20", " ") + "\\" + filename);

            //string s = File.ReadAllText(FilePath.Replace("%20", " ") + "\\" + filename);

            //string ss = "";
            //if (s.Contains("PCS"))
            //{
            //    ss = HighlightKeyWords(s, "PCS", "yellow", false);
            //}


            //Response.Write(ss);
            //Response.Write ("<p style=\"color:#FF0000; font-weight:bold;\">Some Text</p>");

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Buffer = true;
            Response.ContentType = MIMEType;

            string[] FilesTypes = { "image" };
            int co = MIMETypesDictionary.Where(obj => "." + obj.Key == Extension.ToLower() && MatchLikeWord(FilesTypes, obj.Value)).Count();

            //open in borwser
            if (MIMEType == "text/plain")
            {
                Response.AddHeader("Content-Disposition", "inline");
            }
            else if (co != 0)
            {
                Response.AddHeader("Content-Disposition", "inline");
            }
            else
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            }
            //Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);

            //Response.OutputStream.Write(buffer, 0, buffer.Length);
            //Response.OutputStream.Flush();
            //Response.OutputStream.Close();
            //Response.End();

            Response.WriteFile(FilePath.Replace("%20", " ") + "\\" + filename);
            //if (ss != "")
            //{
            //    Response.Write("<pre>" + ss + "</pre>");
            //}
            //else
            //{ Response.Write(s); 
            //}

            Response.Flush();
            Response.Close();
            Response.End();

        }

        public static bool MatchLikeWord(string[] arr, string word)
        {
            foreach (string element in arr)
            {
                if (word.Contains(element))
                {
                    return true;
                }
            }
            return false;
        }

        public static string HighlightKeyWords(string text, string keywords, string cssClass, bool fullMatch)
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

        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string> 
        { 
            { "ai", "application/postscript" }, {"xlsx","application/vnd.ms-excel" },
            { "aif", "audio/x-aiff" }, { "aifc", "audio/x-aiff" }, { "aiff", "audio/x-aiff" }, 
            { "asc", "text/plain" }, 
            { "atom", "application/atom+xml" }, { "au", "audio/basic" }, 
            { "avi", "video/x-msvideo" }, { "bcpio", "application/x-bcpio" }, 
            { "bin", "application/octet-stream" }, { "bmp", "image/bmp" }, 
            { "cdf", "application/x-netcdf" }, { "cgm", "image/cgm" }, 
            { "class", "application/octet-stream" }, { "cpio", "application/x-cpio" }, 
            { "cpt", "application/mac-compactpro" }, { "csh", "application/x-csh" }, 
            //{ "css", "text/css" }, 
            { "dcr", "application/x-director" }, { "dif", "video/x-dv" }, 
            { "dir", "application/x-director" }, { "djv", "image/vnd.djvu" }, 
            { "djvu", "image/vnd.djvu" }, { "dll", "application/octet-stream" }, 
            { "dmg", "application/octet-stream" }, { "dms", "application/octet-stream" }, 
            { "doc", "application/msword" }, { "docx", "application/msword" }, 
            { "dtd", "application/xml-dtd" }, { "dv", "video/x-dv" }, { "dvi", "application/x-dvi" }, 
            { "dxr", "application/x-director" }, { "eps", "application/postscript" }, 
            { "etx", "text/x-setext" }, { "exe", "application/octet-stream" }, 
            { "ez", "application/andrew-inset" }, { "gif", "image/gif" }, { "gram", "application/srgs" }, 
            { "grxml", "application/srgs+xml" }, { "gtar", "application/x-gtar" }, 
            { "hdf", "application/x-hdf" }, { "hqx", "application/mac-binhex40" }, 
            { "htm", "text/html" }, { "html", "text/html" }, 
            { "ice", "x-conference/x-cooltalk" }, 
            { "ico", "image/x-icon" }, { "ics", "text/calendar" }, { "ief", "image/ief" }, 
            { "ifb", "text/calendar" }, { "iges", "model/iges" }, { "igs", "model/iges" }, 
            { "jnlp", "application/x-java-jnlp-file" }, { "jp2", "image/jp2" }, { "jpe", "image/jpeg" }, 
            { "jpeg", "image/jpeg" }, { "jpg", "image/jpeg" }, { "js", "application/x-javascript" }, 
            { "kar", "audio/midi" }, { "latex", "application/x-latex" }, 
            { "lha", "application/octet-stream" }, { "lzh", "application/octet-stream" }, 
            { "m3u", "audio/x-mpegurl" }, { "m4a", "audio/mp4a-latm" }, { "m4b", "audio/mp4a-latm" }, 
            { "m4p", "audio/mp4a-latm" }, { "m4u", "video/vnd.mpegurl" }, 
            { "m4v", "video/x-m4v" }, { "mac", "image/x-macpaint" }, 
            { "man", "application/x-troff-man" }, { "mathml", "application/mathml+xml" }, 
            { "me", "application/x-troff-me" }, { "mesh", "model/mesh" }, { "mid", "audio/midi" }, 
            { "midi", "audio/midi" }, { "mif", "application/vnd.mif" }, { "mov", "video/quicktime" }, 
            { "movie", "video/x-sgi-movie" }, { "mp2", "audio/mpeg" }, { "mp3", "audio/mpeg" }, 
            { "mp4", "video/mp4" }, { "mpe", "video/mpeg" }, { "mpeg", "video/mpeg" }, 
            { "mpg", "video/mpeg" }, { "mpga", "audio/mpeg" }, { "ms", "application/x-troff-ms" }, 
            { "msh", "model/mesh" }, { "mxu", "video/vnd.mpegurl" }, 
            { "nc", "application/x-netcdf" }, { "oda", "application/oda" }, 
            { "ogg", "application/ogg" }, { "pbm", "image/x-portable-bitmap" }, 
            { "pct", "image/pict" }, { "pdb", "chemical/x-pdb" }, { "pdf", "application/pdf" }, 
            { "pgm", "image/x-portable-graymap" }, { "pgn", "application/x-chess-pgn" }, 
            { "pic", "image/pict" }, { "pict", "image/pict" }, 
            { "png", "image/png" }, { "pnm", "image/x-portable-anymap" }, 
            { "pnt", "image/x-macpaint" }, { "pntg", "image/x-macpaint" }, { "ppm", "image/x-portable-pixmap" }, 
            { "ppt", "application/vnd.ms-powerpoint" },{ "pptx", "application/vnd.ms-powerpoint" }, 
            { "ps", "application/postscript" }, { "qt", "video/quicktime" }, { "qti", "image/x-quicktime" }, 
            { "qtif", "image/x-quicktime" }, { "ra", "audio/x-pn-realaudio" }, { "ram", "audio/x-pn-realaudio" }, 
            { "ras", "image/x-cmu-raster" }, { "rdf", "application/rdf+xml" }, { "rgb", "image/x-rgb" }, 
            { "rm", "application/vnd.rn-realmedia" }, { "roff", "application/x-troff" }, { "rtf", "text/rtf" }, 
            { "rtx", "text/richtext" }, { "sgm", "text/sgml" }, { "sgml", "text/sgml" }, 
            { "sh", "application/x-sh" }, { "shar", "application/x-shar" }, { "silo", "model/mesh" }, 
            { "sit", "application/x-stuffit" }, { "skd", "application/x-koan" }, 
            { "skm", "application/x-koan" }, { "skp", "application/x-koan" }, 
            { "skt", "application/x-koan" }, { "smi", "application/smil" }, 
            { "smil", "application/smil" }, { "snd", "audio/basic" }, 
            { "so", "application/octet-stream" }, { "spl", "application/x-futuresplash" }, 
            { "src", "application/x-wais-source" }, { "sv4cpio", "application/x-sv4cpio" }, 
            { "sv4crc", "application/x-sv4crc" }, { "svg", "image/svg+xml" }, 
            { "swf", "application/x-shockwave-flash" }, { "t", "application/x-troff" }, 
            { "tar", "application/x-tar" }, { "tcl", "application/x-tcl" }, 
            { "tex", "application/x-tex" }, { "texi", "application/x-texinfo" }, 
            { "texinfo", "application/x-texinfo" }, { "tif", "image/tiff" }, 
            { "tiff", "image/tiff" }, { "tr", "application/x-troff" }, 
            { "tsv", "text/tab-separated-values" }, { "txt", "text/plain" }, 
            { "ustar", "application/x-ustar" }, { "vcd", "application/x-cdlink" }, 
            { "vrml", "model/vrml" }, { "vxml", "application/voicexml+xml" }, 
            { "wav", "audio/x-wav" }, { "wbmp", "image/vnd.wap.wbmp" }, 
            { "wbmxl", "application/vnd.wap.wbxml" }, { "wml", "text/vnd.wap.wml" }, 
            { "wmlc", "application/vnd.wap.wmlc" }, { "wmls", "text/vnd.wap.wmlscript" }, 
            { "wmlsc", "application/vnd.wap.wmlscriptc" }, { "wrl", "model/vrml" }, 
            { "xbm", "image/x-xbitmap" }, { "xht", "application/xhtml+xml" }, 
            { "xhtml", "application/xhtml+xml" }, { "xls", "application/vnd.ms-excel" }, 
            //{ "xml", "application/xml" },
            { "xml", "text/plain" },
            { "config", "application/xml" },
            { "xpm", "image/x-xpixmap" }, 
            { "xsl", "application/xml" }, { "xslt", "application/xslt+xml" }, 
            { "xul", "application/vnd.mozilla.xul+xml" }, { "xwd", "image/x-xwindowdump" }, 
            { "xyz", "chemical/x-xyz" }, { "zip", "application/zip" },
            { "cab", "application/x-cabinet" },
            { "rar", "application/x-rar-compressed" },
            { "cs", "text/plain" },
            { "aspx", "text/plain" },
            { "css", "text/plain" },
            { "sql", "application/x-sql" }
        };

        public static string GetMIMEType(string fileName)
        {
            if (MIMETypesDictionary.ContainsKey(Path.GetExtension(fileName).Remove(0, 1)))
            {
                return MIMETypesDictionary[Path.GetExtension(fileName).Remove(0, 1)];
            }
            return "application/octet-stream";
        }
    }
}