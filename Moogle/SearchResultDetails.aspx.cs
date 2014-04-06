using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Moogle
{
    public partial class SearchResultDetails : System.Web.UI.Page
    {

        protected DataTable Results = new DataTable();
        private int startAt;
        private int fromItem;
        private int toItem;
        private int total;
        private TimeSpan duration;

        private readonly int maxResults = 10;
        string PageType = string.Empty;
        string Projets = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.Query != null)
                {
                    search();
                    DataBind();
                }
            }
        }

        private void search()
        {
            DateTime start = DateTime.Now;
            // create the result DataTable
            this.Results.Columns.Add("title", typeof(string));
            this.Results.Columns.Add("sample", typeof(string));
            this.Results.Columns.Add("path", typeof(string));
            this.Results.Columns.Add("url", typeof(string));
            this.Results.Columns.Add("Type", typeof(string));

            // create the searcher
            // index is placed in "index" subdirectory
            string indexDirectory = Server.MapPath("~/App_Data/index");


            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            //   List<string> STOP_WORDS =  StopAnalyzer.ENGLISH_STOP_WORDS_SET.ToList<string>(); 
            IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(indexDirectory));
            BooleanQuery bquery = new BooleanQuery();
            //var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", analyzer);
            List<string> SearchTerm = new List<string> { "text", "path", "title", "Extension", "EXTPRP" };
            List<string> Projects = new List<string>();
            if (Session["ProjectList"] != null)
            {
                Projects = (List<string>)Session["ProjectList"];
            }


            List<string> allType = new List<string> { "A", "B", "C" };
            if (this.Request.QueryString["Page"] != null)
            {
                if (allType.Contains(this.Request.QueryString["Page"].ToString()))
                {
                    allType.Remove(this.Request.QueryString["Page"]);
                    foreach (string type in allType)
                    {
                        TermQuery termq1 = new TermQuery(new Term("EXTPRP", type));
                        bquery.Add(termq1, Occur.MUST_NOT);
                        FuzzyQuery termq = new FuzzyQuery(new Term("EXTPRP", type), 0.5f, 0);
                        bquery.Add(termq, Occur.MUST_NOT);
                    }
                }
            }

            //Query query = parser.Parse(this.Query);
            //foreach (string term in SearchTerm)
            //{
            //    if (term == "title")
            //    {
            //        TermQuery termq = new TermQuery(new Term(term, this.Query));
            //        termq.Boost = 50f;
            //        bquery.Add(termq, Occur.SHOULD);
            //    }
            //    else
            //    {
            //        TermQuery termq = new TermQuery(new Term(term, this.Query));
            //        termq.Boost = 5f;
            //        bquery.Add(termq, Occur.SHOULD);
            //    }

            //}

            foreach (string term in SearchTerm)
            {
                if (term == "title")
                {
                    TermQuery termq = new TermQuery(new Term(term, this.Query));
                    termq.Boost = 5f;
                    bquery.Add(termq, Occur.SHOULD);
                }
                else
                {
                    FuzzyQuery termq = new FuzzyQuery(new Term(term, this.Query), 0.5f, 0);
                    termq.Boost = 0.1f;
                    bquery.Add(termq, Occur.SHOULD);
                }
            }


            //foreach (string project in Projects)
            //{
            //    TermQuery termq1 = new TermQuery(new Term("Project", project));
            //    bquery.Add(termq1, Occur.MUST_NOT);

            //}

            //foreach (string project in Projects.Distinct())
            //{
            //    TermQuery termq1 = new TermQuery(new Term("path", project));
            //    bquery.Add(termq1, Occur.MUST);
            //    FuzzyQuery termq = new FuzzyQuery(new Term("path", project), 0.5f, 0);
            //    bquery.Add(termq, Occur.MUST);
            //}

            //bquery.Add(new TermQuery(new Term("Project", "DEV")), Occur.SHOULD);

            //List<ScoreDoc> TempArrList = new List<ScoreDoc>();

            TopDocs hits = searcher.Search(bquery, null, 10000);

            //TopDocs hits = new TopDocs(TempArrList.Count(), TempArrList.ToArray(), hitsWithText.MaxScore);
            //hits.ScoreDocs.CopyTo(hits.ScoreDocs, 0);
            //hits.ScoreDocs = hits.ScoreDocs.OrderBy(obj => searcher.Doc(obj.Doc).Get("path")).ToArray();

            if (Projects.Count() != 0)
            {
                hits.ScoreDocs = hits.ScoreDocs.Where(obj => Projects.Contains(Path.GetDirectoryName(searcher.Doc(obj.Doc).Get("path")))).Distinct().ToArray();
            }

            //foreach (string project in Projects.Distinct())
            //{
            //    //hits.ScoreDocs = hits.ScoreDocs.Where(obj => Regex.IsMatch(searcher.Doc(obj.Doc).Get("path").Replace(@"\", @"\\"), @".*" + project.Replace(@"\", @"\\") + ".*")).ToArray();
            //    string s = Path.GetDirectoryName("\\SAGITEC-1629\\Soogle\\CARS\\bhagyashree.txt");
            //    hits.ScoreDocs = hits.ScoreDocs.Where(obj => Path.GetDirectoryName(searcher.Doc(obj.Doc).Get("path")).Contains(project)).ToArray();
            //}

            this.total = hits.ScoreDocs.Count();

            this.startAt = InitStartAt();

            int resultsCount = Math.Min(total, this.maxResults + this.startAt);

            // create highlighter
            IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;background-color:yellow;\">", "</span>");
            SimpleFragmenter fragmenter = new SimpleFragmenter(200);
            QueryScorer scorer = new QueryScorer(bquery);
            Highlighter highlighter = new Highlighter(formatter, scorer);
            highlighter.TextFragmenter = fragmenter;

            int j = 0;

            for (int i = startAt; i < resultsCount; i++)
            {
                Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                String path = doc.Get("path");
                string getExtension = doc.Get("Extension");

                TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("text")));
                String sample = "";
                try
                {
                    string document = doc.Get("text");
                    if (getExtension.ToLower() == ".png" || getExtension.ToLower() == ".jpg" || getExtension.ToLower() == ".gif" || getExtension.ToLower() == ".bmp")
                    {
                        sample = "";
                    }
                    else
                    {
                        sample = highlighter.GetBestFragment(stream, document);//, 2, "...");
                    }

                }
                catch (Exception ex)
                {
                }

                // create a new row with the result data
                DataRow row = this.Results.NewRow();
                row["title"] = doc.Get("title");
                row["path"] = "http://sagitec-1629/KNBASE/" + path.Replace(@"\", "/").Replace("//SAGITEC-1629/Soogle/", "");
                row["url"] = "http://sagitec-1629/KNBASE/" + path.Replace(@"\", "/").Replace("//SAGITEC-1629/Soogle/", "");
                row["sample"] = sample;
                if (path.Contains('.'))
                {
                    row["Type"] = GetMIMEType(path);
                }
                //if (!Projects.Contains(doc.Get("Project")) || !allType.Contains(doc.Get("EXTPRP")))
                //{
                this.Results.Rows.Add(row);
                //}
                j++;

            }

            Repeater1.DataSource = Results;
            Repeater1.DataBind();

            searcher.Dispose();

            // result information
            this.duration = DateTime.Now - start;
            this.fromItem = startAt + 1;
            this.toItem = Math.Min(startAt + maxResults, total);

        }


        protected DataTable Paging
        {
            get
            {
                if (Repeater1.Items.Count != 0)
                {
                    // pageNumber starts at 1
                    int pageNumber = (startAt + maxResults - 1) / maxResults;

                    DataTable dt = new DataTable();
                    dt.Columns.Add("html", typeof(string));

                    DataRow ar = dt.NewRow();
                    ar["html"] = PagingItemHtml(startAt, pageNumber + 1, false);
                    dt.Rows.Add(ar);

                    int previousPagesCount = 4;
                    for (int i = pageNumber - 1; i >= 0 && i >= pageNumber - previousPagesCount; i--)
                    {
                        int step = i - pageNumber;
                        DataRow r = dt.NewRow();
                        r["html"] = PagingItemHtml(startAt + (maxResults * step), i + 1, true);

                        dt.Rows.InsertAt(r, 0);
                    }

                    int nextPagesCount = 4;
                    for (int i = pageNumber + 1; i <= PageCount && i <= pageNumber + nextPagesCount; i++)
                    {
                        int step = i - pageNumber;
                        DataRow r = dt.NewRow();
                        r["html"] = PagingItemHtml(startAt + (maxResults * step), i + 1, true);

                        dt.Rows.Add(r);
                    }
                    return dt;
                }
                else
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// Prepares HTML of a paging item (bold number for current page, links for others).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="number"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        private string PagingItemHtml(int start, int number, bool active)
        {
            string PageType = "";
            string prj = "";
            if (this.Request.QueryString["Page"] != null)
            {
                PageType = "&Page=" + this.Request.QueryString["Page"];
            }
            if (!string.IsNullOrEmpty(this.Request.QueryString["Projets"]))
            {
                prj = this.Request.QueryString["Projets"];
            }
            if (active)
                return "<a href=\"SearchResultDetails.aspx?q=" + this.Query + "&start=" + start + PageType + "&Projets=" + prj + "\">" + number + "</a>";
            else
                return "<b>" + number + "</b>";
        }

        /// <summary>
        /// Prepares the string with seach summary information.
        /// </summary>
        protected string Summary
        {
            get
            {
                if (total > 0)
                    return "Results <b>" + this.fromItem + " - " + this.toItem + "</b> of <b>" + this.total + "</b> for <b>" + this.Query + "</b>. (" + this.duration.TotalSeconds + " seconds)";
                return "No results found";
            }
        }

        /// <summary>
        /// Return search query or null if not provided.
        /// </summary>
        protected string Query
        {
            get
            {
                string query = this.Request.Params["q"];
                if (query == String.Empty)
                {
                    return null;
                }

                return query;
            }
        }

        /// <summary>
        /// Initializes startAt value. Checks for bad values.
        /// </summary>
        /// <returns></returns>
        private int InitStartAt()
        {
            try
            {
                int sa = Convert.ToInt32(this.Request.Params["start"]);

                // too small starting item, return first page
                if (sa < 0)
                    return 0;

                // too big starting item, return last page
                if (sa >= total - 1)
                {
                    return LastPageStartsAt;
                }

                return sa;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// How many pages are there in the results.
        /// </summary>
        private int PageCount
        {
            get
            {
                return (total - 1) / maxResults; // floor
            }
        }

        /// <summary>
        /// First item of the last page
        /// </summary>
        private int LastPageStartsAt
        {
            get
            {
                return PageCount * maxResults;
            }
        }

        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string> 
        { 
            { "ai", "application/postscript" }, {"xlsx","application/vnd.ms-excel" },
            { "aif", "audio/x-aiff" }, { "aifc", "audio/x-aiff" }, { "aiff", "audio/x-aiff" }, 
            { "asc", "text/plain" }, { "atom", "application/atom+xml" }, { "au", "audio/basic" }, 
            { "avi", "video/x-msvideo" }, { "bcpio", "application/x-bcpio" }, 
            { "bin", "application/octet-stream" }, { "bmp", "image/bmp" }, 
            { "cdf", "application/x-netcdf" }, { "cgm", "image/cgm" }, 
            { "class", "application/octet-stream" }, { "cpio", "application/x-cpio" }, 
            { "cpt", "application/mac-compactpro" }, { "csh", "application/x-csh" }, 
            { "css", "text/css" }, { "dcr", "application/x-director" }, { "dif", "video/x-dv" }, 
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
            { "htm", "text/html" }, { "html", "text/html" }, { "ice", "x-conference/x-cooltalk" }, 
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
            { "pic", "image/pict" }, { "pict", "image/pict" }, { "png", "image/png" }, { "pnm", "image/x-portable-anymap" }, 
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
            { "xml", "application/xml" }, { "xpm", "image/x-xpixmap" }, 
            { "xsl", "application/xml" }, { "xslt", "application/xslt+xml" }, 
            { "xul", "application/vnd.mozilla.xul+xml" }, { "xwd", "image/x-xwindowdump" }, 
            { "xyz", "chemical/x-xyz" }, { "zip", "application/zip" } 
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