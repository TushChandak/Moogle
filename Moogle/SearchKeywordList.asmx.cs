using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using Lucene.Net.QueryParsers;

namespace Moogle
{
    /// <summary>
    /// Summary description for SearchKeywordList
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SearchKeywordList : System.Web.Services.WebService
    {
        string IndexDirPath = Convert.ToString(ConfigurationSettings.AppSettings["IndexDirPath"]);

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //[WebMethod(EnableSession = true)]
        //public List<string> FetchSearchKeywordList(string searchkeyword)
        //{

        //    List<string> liSearchResult = new List<string>();

        //    // create the searcher
        //    // index is placed in "index" subdirectory
        //    string indexDirectory = Server.MapPath(IndexDirPath);
        //    var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        //    //   List<string> STOP_WORDS =  StopAnalyzer.ENGLISH_STOP_WORDS_SET.ToList<string>(); 
        //    IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(indexDirectory));
        //    BooleanQuery bquery = new BooleanQuery();
        //    //var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", analyzer);
        //    List<string> SearchTerm = new List<string> { "text", "path", "title", "Extension", "EXTPRP" };
        //    List<string> Projects = new List<string>();
        //    if (Session["ProjectList"] != null)
        //    {
        //        Projects = (List<string>)Session["ProjectList"];
        //    }

        //    List<string> allType = null;
        //    if (Session["AnchorClickText"] == null)
        //    {
        //        allType = new List<string>();
        //    }
        //    else
        //    {
        //        allType = new List<string> { "Doc", "Code", "Images", "Other" };
        //    }


        //    if (allType.Contains(Convert.ToString(Session["AnchorClickText"])))
        //    {
        //        allType.Remove(Convert.ToString(Session["AnchorClickText"]));
        //        foreach (string type in allType)
        //        {
        //            TermQuery termq1 = new TermQuery(new Term("EXTPRP", type));
        //            bquery.Add(termq1, Occur.MUST_NOT);
        //            FuzzyQuery termq = new FuzzyQuery(new Term("EXTPRP", type), 0.5f, 0);
        //            bquery.Add(termq, Occur.MUST_NOT);
        //        }
        //    }

        //    //Query query = parser.Parse(this.Query);
        //    foreach (string term in SearchTerm)
        //    {
        //        if (term == "title")
        //        {
        //            TermQuery termq = new TermQuery(new Term(term, searchkeyword.ToLower()));
        //            termq.Boost = 5f;
        //            bquery.Add(termq, Occur.SHOULD);
        //        }
        //        else
        //        {
        //            TermQuery termq = new TermQuery(new Term(term, searchkeyword.ToLower()));
        //            termq.Boost = 0.1f;
        //            bquery.Add(termq, Occur.SHOULD);
        //        }
        //    }

        //    foreach (string term in SearchTerm)
        //    {
        //        if (searchkeyword.Contains("."))
        //        {
        //            string SearchKeyword = searchkeyword.Replace(".", "");
        //            if (term == "Extension")
        //            {
        //                TermQuery termq = new TermQuery(new Term(term, SearchKeyword.ToLower()));
        //                termq.Boost = 5f;
        //                bquery.Add(termq, Occur.SHOULD);
        //            }
        //        }
        //        else
        //        {
        //            if (term == "title")
        //            {
        //                TermQuery termq = new TermQuery(new Term(term, searchkeyword.ToLower()));
        //                termq.Boost = 5f;
        //                bquery.Add(termq, Occur.SHOULD);
        //            }
        //        }
        //    }

        //    TopDocs hits = searcher.Search(bquery, null, 10000);
        //    if (Projects.Count() != 0)
        //    {
        //        hits.ScoreDocs = hits.ScoreDocs.Where(obj => Projects.Contains(SplitPath(Path.GetDirectoryName(searcher.Doc(obj.Doc).Get("path"))))).Distinct().ToArray();
        //    }

        //    int total = hits.ScoreDocs.Count();

        //    // create highlighter
        //    IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;background-color:yellow;\">", "</span>");
        //    SimpleFragmenter fragmenter = new SimpleFragmenter(200);
        //    QueryScorer scorer = new QueryScorer(bquery);
        //    Highlighter highlighter = new Highlighter(formatter, scorer);
        //    highlighter.TextFragmenter = fragmenter;

        //    for (int i = 0; i < 15; i++)
        //    {
        //        if (i < total)
        //        {
        //            Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
        //            string title = doc.Get("title");
        //            liSearchResult.Add(title);
        //        }
        //    }
        //    searcher.Dispose();

        //    List<string> l = liSearchResult.Where(s => s.ToLower().StartsWith(searchkeyword.ToLower())).ToList();
        //    return l;
        //}

        private string SplitPath(string path)
        {
            string OriginalPath = path.Replace("\\\\", "\\").Trim();
            string[] strSplitPath = OriginalPath.Split('\\');
            string strNewPath = "";
            if (strSplitPath.Count() >= 7)
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (strNewPath == "")
                    {
                        strNewPath = "\\\\" + strSplitPath[i];
                    }
                    else
                    {
                        strNewPath = strNewPath + "\\" + strSplitPath[i];
                    }
                }
            }
            else
            {
                strNewPath = path;
            }
            return strNewPath;
        }


        [WebMethod(EnableSession = true)]
        public List<string> FetchSearchKeywordList(string searchkeyword)
        {
        // create the searcher
            // index is placed in "index" subdirectory
            string indexDirectory = Server.MapPath("~/App_Data/index");

            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(indexDirectory));

            // parse the query, "text" is the default field to search
            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", analyzer);

            Query query = parser.Parse(searchkeyword); List<ScoreDoc> TempArrList = new List<ScoreDoc>();
                    int count;
                    TopDocs hitsWithText = searcher.Search(query, null, 200);
                    List<string> l = hitsWithText.ScoreDocs.Select(s => searcher.Doc(s.Doc).Get("title")).ToList();
                    return l;       
                    

        }
    }
}
