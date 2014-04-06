using System;
using System.Data;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using System.Linq;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Lucene.Net.Index;
using System.Net;
using System.Web;
using System.Xml;

namespace Moogle
{
    public partial class Search : System.Web.UI.Page
    {
        string PageType = string.Empty;
        string Projets = string.Empty;

        List<string> liProjectsPath = new List<string>();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Press ButtonSearch on enter
            Managemenu();

            tvFolderStructure.Attributes.Add("onclick", "OnCheckBoxOfTreeview(event)");
            if (!IsPostBack)
            {
                Session["ProjectList"] = null;

                if (Query != null)
                {
                    //List<string> liProjects = new List<string>();

                    TreeViewBind();
                    CallNodesSelector(1);
                    //if (liProjects.Count == 0)
                    //{
                    Session["ProjectList"] = null;
                    //}
                    //else
                    //{
                    //    Session["ProjectList"] = liProjects;
                    //}


                    TextBoxQuery.Text = Convert.ToString(Request.QueryString["q"]);
                    Show.Attributes.Add("src", "SearchResultDetails.aspx?q=" + this.TextBoxQuery.Text + "&Page=All");
                    //PopulateTreeView();
                    //GenerateXML();
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //foreach (ListItem li in ChkDocumentType.Items)
            //{
            //    li.Selected = true;

            //}
            this.ButtonSearch.Click += new System.EventHandler(this.ButtonSearch_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        protected void Managemenu()
        {
            if (this.Request.QueryString["Page"] != null)
            {
                PageType = this.Request.QueryString["Page"];
            }
            else
            {
                PageType = "All";
            }
            if (PageType == "All")
            {
                All.Attributes.Add("class", "hdtb_mitem hdtb_msel");

            }
            else if (PageType == "A")
            {
                Doc.Attributes.Add("class", "hdtb_mitem hdtb_msel");
            }
            else if (PageType == "B")
            {
                Code.Attributes.Add("class", "hdtb_mitem hdtb_msel");
            }
            else if (PageType == "C")
            {
                Other.Attributes.Add("class", "hdtb_mitem hdtb_msel");
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
                string query = this.Request.Params["q"];
                if (query == String.Empty)
                {
                    return null;
                }

                return query;
            }
        }

        protected void ButtonSearch_Click(object sender, System.EventArgs e)
        {
            Projets = "";
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

            if (this.Request.QueryString["Page"] == null)
            {
                Show.Attributes.Add("src", "SearchResultDetails.aspx?q=" + this.TextBoxQuery.Text + "&Page=All" + "&Projets=" + Projets);
                //this.Response.Redirect("SearchResultDetails.aspx?q=" + this.TextBoxQuery.Text + "&Page=All" + "&Projets=" + Projets);
            }
            else
            {
                Show.Attributes.Add("src", "SearchResultDetails.aspx?q=" + this.TextBoxQuery.Text + "&Page=" + this.Request.QueryString["Page"] + "&Projets=" + Projets);
                //this.Response.Redirect("SearchResultDetails.aspx?q=" + this.TextBoxQuery.Text + "&Page=" + this.Request.QueryString["Page"] + "&Projets=" + Projets);
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


        //****************************** Populate treeview from Virtual Directory *********************************************
        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(@"\\SAGITEC-1629\Soogle\");
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
        //*************************************************************************************************************************


        //*************************** Generate XML File *************************************************************************
        private void GenerateXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<directory>" +
                       "</directory>");

            //recurse through directories and return XmlNodes
            XmlElement elem = GetData(@"\\SAGITEC-1629\Soogle\", doc);
            doc.DocumentElement.AppendChild(elem);

            doc.Save(Server.MapPath(@"~\XML\dirinfo.xml"));
        }

        private static XmlElement GetData(string dirName, XmlDocument doc)
        {
            //create a new node for this directory
            XmlElement elem = doc.CreateElement("dir");
            DirectoryInfo di = new DirectoryInfo(dirName);
            elem.SetAttribute("name", di.Name);
            elem.SetAttribute("path", di.FullName);

            foreach (DirectoryInfo dinf in di.GetDirectories())
            {
                //Recursively call the directory with all underlying dirs and files
                XmlElement elemDir = GetData(dirName + "\\" + dinf.Name, doc);
                elem.AppendChild(elemDir);
            }

            //Append the files in this directory to the current xml node
            //foreach (FileInfo finf in di.GetFiles("*.htm*"))
            //{
            //    XmlElement elemDir = doc.CreateElement("file");
            //    elemDir.SetAttribute("name", finf.Name);
            //    elem.AppendChild(elemDir);
            //}

            return elem;
        }
        //*************************************************************************************************************************


        //******************************** Populate treeview from XML File ***********************************************************
        private void TreeViewBind()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(Server.MapPath(@"~\XML\dirinfo.xml"));
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

        protected void btnSetSessionValue_Click(object sender, EventArgs e)
        {
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

            Show.Attributes.Add("src", hnkClickLink.Value);

        }
    }

}