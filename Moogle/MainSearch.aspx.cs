using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace Moogle
{
    public partial class MainSearch : System.Web.UI.Page
    {
        #region "Variables"

        protected DataTable Results = new DataTable();
        private int startAt;
        private int fromItem;
        private int toItem;
        private int total;
        private TimeSpan duration;
        private readonly int maxResults = 10;
        string PageType = string.Empty;
        string Projets = string.Empty;
        List<string> liProjectsPath = new List<string>();

        #endregion

        #region "ReadWebConfigSettings"

        string VirtualPath = Convert.ToString(ConfigurationSettings.AppSettings["VirtualPath"]);
        string ApplicationPath = Convert.ToString(ConfigurationSettings.AppSettings["ApplicationPath"]);
        string XMLFilePath = Convert.ToString(ConfigurationSettings.AppSettings["XMLFilePath"]);
        string IndexDirPath = Convert.ToString(ConfigurationSettings.AppSettings["IndexDirPath"]);

        #endregion

        #region "Events"

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["SearchKeyword"] == null)
            {
                Response.Redirect("~/Home.aspx");
            }

            this.form1.DefaultButton = ButtonSearch.UniqueID;

            // Press ButtonSearch on enter
            //Managemenu();

            tvFolderStructure.Attributes.Add("onclick", "OnCheckBoxOfTreeview(event)");
            //TextBoxQuery.Attributes.Add("onchange", "OnTextChange()");
            if (!IsPostBack)
            {
                ViewState["PageNumber"] = null;
                Session["AnchorClickText"] = null;
                Managemenu(2);
                Session["start"] = null;
                hnkClickLink.Value = "";
                Session["ProjectList"] = null;
                hnkPageCount.Value = "0";
                TextBoxQuery.Text = Convert.ToString(Session["SearchKeyword"]);

                if (Query != null)
                {
                    TreeViewBind();
                    CallNodesSelector(1);
                    search();
                    DataBind();
                    //PopulateTreeView();
                }
            }

        }

        protected void ButtonSearch_Click(object sender, System.EventArgs e)
        {
            if (TextBoxQuery.Text != "")
            {
                ViewState["PageNumber"] = null;
                Thread.Sleep(1000);
                liProjectsPath.Clear();
                Session["start"] = null;
                hnkClickLink.Value = "";
                Session["AnchorClickText"] = null;
                hnkPageCount.Value = "0";
                List<string> liProjects = new List<string>();

                liProjects = CallNodesSelector(0);
                if (liProjects.Count == 0)
                {
                    Session["ProjectList"] = null;
                }
                else
                {
                    Session["ProjectList"] = liProjects;
                }

                search();
                DataBind();
                Managemenu(2);
            }
            ButtonSearch.Focus();
        }

        protected void btnSetSessionValue_Click(object sender, EventArgs e)
        {
            ViewState["PageNumber"] = null;
            Thread.Sleep(1000);
            liProjectsPath.Clear();
            Session["start"] = null;
            Session["AnchorClickText"] = hnkClickLink.Value;

            List<string> liProjects = new List<string>();

            liProjects = CallNodesSelector(0);
            if (liProjects.Count == 0)
            {
                Session["ProjectList"] = null;
            }
            else
            {
                Session["ProjectList"] = liProjects;
            }

            search();
            DataBind();
            Managemenu(1);
        }

        protected void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            PageNumber = Convert.ToInt32(e.CommandArgument) - 1;
            search();
            DataBind();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            PageNumber++;
            search();
            DataBind();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            PageNumber--;
            search();
            DataBind();
        }

        #endregion

        #region "Methods"

        protected void Managemenu(int flag)
        {
            All.Attributes.Add("class", "hdtb_mitem hdtb_msel");
            Doc.Attributes.Add("class", "hdtb_mitem hdtb_msel");
            Code.Attributes.Add("class", "hdtb_mitem hdtb_msel");
            Images.Attributes.Add("class", "hdtb_mitem hdtb_msel");
            Other.Attributes.Add("class", "hdtb_mitem hdtb_msel");

            if (hnkClickLink.Value == "")
            {
                if (this.Request.QueryString["Page"] != null)
                {
                    PageType = this.Request.QueryString["Page"];
                }
                else
                {
                    PageType = "All";
                }
            }
            else
            {
                if (flag == 1)
                {
                    if (hnkClickLink.Value == "All")
                    {
                        All.Attributes.Add("class", "ClickAnchorColor");
                    }
                    else if (hnkClickLink.Value == "Doc")
                    {
                        Doc.Attributes.Add("class", "ClickAnchorColor");
                    }
                    else if (hnkClickLink.Value == "Code")
                    {
                        Code.Attributes.Add("class", "ClickAnchorColor");
                    }
                    else if (hnkClickLink.Value == "Images")
                    {
                        Images.Attributes.Add("class", "ClickAnchorColor");
                    }
                    else if (hnkClickLink.Value == "Other")
                    {
                        Other.Attributes.Add("class", "ClickAnchorColor");
                    }
                }
            }
            //else if (PageType == "Ext")
            //{
            //    Ext.Attributes.Add("Ext", "hdtb_mitem hdtb_msel");
            //}


            //if (this.Request.QueryString["Projets"] != null)
            //{
            //    if (!string.IsNullOrEmpty(this.Request.QueryString["Projets"]))
            //    {
            //        foreach (ListItem list in chkProject.Items)
            //        {
            //            if (this.Request.QueryString["Projets"].ToString().Split(',').Contains(list.Value))
            //            {
            //                list.Selected = false;
            //            }
            //        }
            //    }
            //}

        }

        protected string Query
        {
            get
            {
                string query = TextBoxQuery.Text;
                Session["SearchKeyword"] = query;
                if (query == String.Empty)
                {
                    Session["SearchKeyword"] = null;
                    return null;
                }

                return query;
            }
        }

        private List<string> CallNodesSelector(int flag)
        {
            TreeNodeCollection nodes = this.tvFolderStructure.Nodes;
            foreach (TreeNode n in nodes)
            {
                GetNodeRecursive(n, flag);
            }
            return liProjectsPath;
        }

        private List<string> GetNodeRecursive(TreeNode treeNode, int flag)
        {
            if (flag == 1)
            {
                treeNode.Checked = true;
            }
            else
            {
                if (treeNode.Checked == true)
                {
                    liProjectsPath.Add(treeNode.Value);
                }
            }
            foreach (TreeNode tn in treeNode.ChildNodes)
            {
                GetNodeRecursive(tn, flag);
            }
            return liProjectsPath;
        }

        //******************************** Populate treeview from XML File ***********************************************************
        private void TreeViewBind()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(Server.MapPath(XMLFilePath));
                string strXmlString = document.InnerXml;
                document.LoadXml(strXmlString);
                XmlNode nodeXml = document.DocumentElement;
                TreeNode nodeTree = null;
                foreach (XmlNode node in nodeXml.ChildNodes)
                {
                    nodeTree = new TreeNode();
                    XmlElement elementXml = (XmlElement)node;
                    nodeTree.Text = elementXml.GetAttribute("name").ToString();
                    nodeTree.Value = elementXml.GetAttribute("path").ToString();
                    this.AddChildNode(nodeTree, node);
                    this.tvFolderStructure.Nodes.Add(nodeTree);
                }
            }
            catch (XmlException xmlEx)
            {
                tvFolderStructure.Nodes.Clear();
            }
            catch (Exception ex)
            {
                tvFolderStructure.Nodes.Clear();
            }
        }

        private void AddChildNode(TreeNode nodeParent, XmlNode node)
        {
            TreeNode nodeTreeChild = null;
            foreach (XmlNode nodeChild in node.ChildNodes)
            {
                if (node.ChildNodes.Count == 0)
                {
                    nodeParent.ChildNodes.Add(nodeTreeChild);
                }
                else
                {
                    nodeTreeChild = new TreeNode();
                    XmlElement elementChild = (XmlElement)nodeChild;
                    nodeTreeChild.Text = elementChild.GetAttribute("name").ToString();
                    nodeTreeChild.Value = elementChild.GetAttribute("path").ToString();
                    this.AddChildNode(nodeTreeChild, nodeChild);
                    nodeParent.ChildNodes.Add(nodeTreeChild);
                }
            }
        }

        //********************************************************************************************************************************
        private void search()
        {
            if (TextBoxQuery.Text != "")
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
                string indexDirectory = Server.MapPath(IndexDirPath);
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

                List<string> allType = null;
                if (hnkClickLink.Value == "")
                {
                    allType = new List<string>();
                }
                else
                {
                    allType = new List<string> { "Doc", "Code", "Images", "Other" };
                }

                if (this.Request.QueryString["Page"] != null)
                {
                    if (allType.Contains(Convert.ToString(hnkClickLink.Value)))
                    {
                        allType.Remove(Convert.ToString(hnkClickLink.Value));
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
                foreach (string term in SearchTerm)
                {
                    if (term == "title")
                    {
                        TermQuery termq = new TermQuery(new Term(term, this.Query.ToLower()));
                        termq.Boost = 5f;
                        bquery.Add(termq, Occur.SHOULD);
                    }
                    else
                    {
                        TermQuery termq = new TermQuery(new Term(term, this.Query.ToLower()));
                        termq.Boost = 0.1f;
                        bquery.Add(termq, Occur.SHOULD);
                    }
                }

                foreach (string term in SearchTerm)
                {
                    if (this.Query.Contains("."))
                    {
                        string SearchKeyword = this.Query.Replace(".", "");
                        if (term == "Extension")
                        {
                            TermQuery termq = new TermQuery(new Term(term, SearchKeyword.ToLower()));
                            termq.Boost = 5f;
                            bquery.Add(termq, Occur.SHOULD);
                        }
                    }
                    else
                    {
                        if (term == "title")
                        {
                            FuzzyQuery termq = new FuzzyQuery(new Term(term, this.Query.ToLower()));
                            termq.Boost = 5f;
                            bquery.Add(termq, Occur.SHOULD);
                        }
                        else
                        {
                            //FuzzyQuery termq = new FuzzyQuery(new Term(term, this.Query), 0.5f, 0);
                            //termq.Boost = 0.1f;
                            //bquery.Add(termq, Occur.SHOULD);
                        }
                    }
                }

                TopDocs hits = searcher.Search(bquery, null, 10000);

                //TopDocs hits = new TopDocs(TempArrList.Count(), TempArrList.ToArray(), hitsWithText.MaxScore);
                //hits.ScoreDocs.CopyTo(hits.ScoreDocs, 0);
                //hits.ScoreDocs = hits.ScoreDocs.OrderBy(obj => searcher.Doc(obj.Doc).Get("path")).ToArray();

                if (Projects.Count() != 0)
                {
                    hits.ScoreDocs = hits.ScoreDocs.Where(obj => Projects.Contains(SplitPath(Path.GetDirectoryName(searcher.Doc(obj.Doc).Get("path"))))).Distinct().ToArray();
                }

                this.total = hits.ScoreDocs.Count();

                this.startAt = InitStartAt();

                int resultsCount = Math.Min(total, this.maxResults + this.startAt);

                // create highlighter
                IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;background-color:yellow;\">", "</span>");
                SimpleFragmenter fragmenter = new SimpleFragmenter(200);
                QueryScorer scorer = new QueryScorer(bquery);
                Highlighter highlighter = new Highlighter(formatter, scorer);
                highlighter.TextFragmenter = fragmenter;
                //highlighter.MaxDocCharsToAnalyze=200;



                //for (int i = startAt; i < resultsCount; i++)
                //{
                //    Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                //    String path = doc.Get("path");
                //    string getExtension = doc.Get("Extension");

                //    TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("text")));
                //    String sample = "";
                //    try
                //    {
                //        string document = doc.Get("text");
                //        if (getExtension.ToLower() == ".png" || getExtension.ToLower() == ".jpg" || getExtension.ToLower() == ".gif" || getExtension.ToLower() == ".bmp" || getExtension.ToLower() == ".jpeg")
                //        {
                //            sample = "";
                //        }
                //        else
                //        {
                //            string outp = highlighter.GetBestFragment(stream, document);
                //            if (outp != null)
                //                sample = ReplaceSpecialChar(outp.Trim()); //, 2, "...");
                //            else
                //                sample = Limit(doc.Get("text").Trim(), 200);
                //        }

                //    }
                //    catch (Exception ex)
                //    {
                //    }

                //    // create a new row with the result data
                //    DataRow row = this.Results.NewRow();
                //    row["title"] = doc.Get("title");
                //    row["path"] = ApplicationPath + path.Replace(@"\", "/").Replace(VirtualPath, "");
                //    row["url"] = ApplicationPath + path.Replace(@"\", "/").Replace(VirtualPath, "");
                //    row["sample"] = sample;
                //    if (path.Contains('.'))
                //    {
                //        row["Type"] = GetMIMEType(path);
                //    }

                //    this.Results.Rows.Add(row);
                //}



                for (int i = 0; i < this.total; i++)
                {
                    Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                    String path = doc.Get("path");
                    string getExtension = doc.Get("Extension");

                    TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("text")));
                    String sample = "";
                    try
                    {
                        string document = doc.Get("text");
                        if (getExtension.ToLower() == ".png" || getExtension.ToLower() == ".jpg" || getExtension.ToLower() == ".gif" || getExtension.ToLower() == ".bmp" || getExtension.ToLower() == ".jpeg")
                        {
                            sample = "";
                        }
                        else
                        {
                            string outp = highlighter.GetBestFragment(stream, document);
                            if (outp != null)
                                sample = Limit(outp.Trim(), 200); //, 2, "...");
                            else
                                sample = Limit(doc.Get("text").Trim(), 200);
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                    // create a new row with the result data
                    DataRow row = Results.NewRow();
                    row["title"] = doc.Get("title");
                    row["path"] = ApplicationPath + path.Replace(@"\", "/").Replace(VirtualPath, "");
                    row["url"] = ApplicationPath + path.Replace(@"\", "/").Replace(VirtualPath, "");
                    row["sample"] = sample;
                    if (path.Contains('.'))
                    {
                        row["Type"] = GetMIMEType(path);
                    }

                    Results.Rows.Add(row);
                }

                //****************************** Logic for Paging for Repeater Control****************************************
                PagedDataSource pgitems = new PagedDataSource();
                DataView dv = new DataView(Results);
                pgitems.DataSource = dv;

                pgitems.AllowPaging = true;

                pgitems.PageSize = 10;//You can set the number of items here using some logic.

                pgitems.CurrentPageIndex = PageNumber;

                btnPrev.Visible = !pgitems.IsFirstPage;
                btnNext.Visible = !pgitems.IsLastPage;

                if (pgitems.PageCount > 1)
                {
                    rptPages.Visible = true;
                    ArrayList pages = new ArrayList();
                    for (int i = PageNumber; i < 5 + PageNumber; i++)
                        pages.Add((i + 1).ToString());
                    rptPages.DataSource = pages;
                    rptPages.DataBind();
                }
                else
                    rptPages.Visible = false;

                Repeater1.DataSource = pgitems;
                Repeater1.DataBind();
                //*************************************************************************************************************

                //Repeater1.DataSource = Results;
                //Repeater1.DataBind();

                searcher.Dispose();

                // result information
                this.duration = DateTime.Now - start;
                this.fromItem = startAt + 1;
                this.toItem = Math.Min(startAt + maxResults, total);
            }
        }

        public int PageNumber
        {
            get
            {
                if (ViewState["PageNumber"] != null)
                    return Convert.ToInt32(ViewState["PageNumber"]);
                else
                    return 0;
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }

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
            {
                //return "<a href=\"SearchResultDetails.aspx?q=" + this.Query + "&start=" + start + PageType + "&Projets=" + prj + "\">" + number + "</a>";
                Session["start"] = start;
                prj = hnkClickLink.Value;
                return "<a href\"#\" class=\"q\" onclick=\"GetSession('" + prj + "',this);\"\">" + number + "</a>";
            }
            else
            {
                Session["start"] = null;
                return "<b>" + number + "</b>";
            }
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
        /// Initializes startAt value. Checks for bad values.
        /// </summary>
        /// <returns></returns>
        private int InitStartAt()
        {
            try
            {
                //int sa = Convert.ToInt32(this.Request.Params["start"]);
                int sa = Convert.ToInt32(hnkPageCount.Value);

                if (Convert.ToInt32(hnkPageCount.Value) > 1)
                {
                    sa = (Convert.ToInt32(hnkPageCount.Value) - 1) * 10;
                }
                else
                {
                    sa = 0;
                }

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
            { "pic", "image/pict" }, { "pict", "image/pict" }, 
            { "png", "image/png" }, 
            { "pnm", "image/x-portable-anymap" }, 
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
            { "xml", "application/xml" }, 
            { "xpm", "image/x-xpixmap" }, 
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

        protected string Limit(object Desc, int length)
        {
            StringBuilder strDesc = new StringBuilder();
            strDesc.Insert(0, Desc.ToString());

            if (strDesc.Length > length)
                return ReplaceSpecialChar(strDesc.ToString().Substring(0, length)) + "...";
            else
                return ReplaceSpecialChar(strDesc.ToString());
        }

        private string ReplaceSpecialChar(string desc)
        {
            return desc.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace("<summary>", "summary").Replace("</summary>", "summary");
        }

        #endregion

        #region "Unused Code"

        //****************************** Populate treeview from Virtual Directory(Unused Code) *********************************************
        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(VirtualPath);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name, info.FullName);
                GetDirectories(info.GetDirectories(), rootNode);
                tvFolderStructure.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            //DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, subDir.FullName);
                DirectoryInfo[] subSubDirs = subDir.GetDirectories();
                try
                {
                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }
                }
                catch (System.UnauthorizedAccessException)
                {
                    subSubDirs = new DirectoryInfo[0];
                }
                nodeToAddTo.ChildNodes.Add(aNode);
            }
        }

        //**********************************************************************************************************************************

        #endregion
    }
}