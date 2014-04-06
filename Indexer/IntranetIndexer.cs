using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;



namespace Indexer
{
    public class IntranetIndexer
    {
        private IndexWriter writer;
        private string docRootDirectory;
        private string pattern;

        /// <summary>
        /// Creates a new index in <c>directory</c>. Overwrites the existing index in that directory.
        /// </summary>
        /// <param name="directory">Path to index (will be created if not existing).</param>
        public IntranetIndexer(string directory)
        {
            writer = new IndexWriter(FSDirectory.Open(directory), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);
            //writer = new IndexWriter(FSDirectory.Open(directory), new Lucene.Net.Analysis.WhitespaceAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED);
            writer.UseCompoundFile = true;
        }
        public IntranetIndexer(string directory, bool create)
        {
            writer = new IndexWriter(FSDirectory.Open(directory), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), create, IndexWriter.MaxFieldLength.LIMITED);
            //writer = new IndexWriter(FSDirectory.Open(directory), new Lucene.Net.Analysis.WhitespaceAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED);
            writer.UseCompoundFile = true;
        }

        /// <summary>
        /// Add HTML files from <c>directory</c> and its subdirectories that match <c>pattern</c>.
        /// </summary>
        /// <param name="directory">Directory with the HTML files.</param>
        /// <param name="pattern">Search pattern, e.g. <c>"*.html"</c></param>
        public void AddDirectory(DirectoryInfo directory, string pattern)
        {
            try
            {
                this.docRootDirectory = directory.FullName;
                this.pattern = pattern;

                addSubDirectory(directory);
            }
            catch (Exception m)
            {
                MyException mobj = new MyException(m.Message);
            }
        }

        private void addSubDirectory(DirectoryInfo directory)
        {
            try
            {
                int i = 0;
                foreach (FileInfo fi in directory.GetFiles(pattern))
                {
                    // string[] ext = { ".dll", ".img", ".exe", ".zip", ".png", ".exe", ".jpeg", ".jpg", ".gif", ".config" };
                    //string[] Folders = { "Correspondence","KPERS" };
                    //if (!ext.Contains(fi.Extension))
                    //{

                    FileAttributes attributes = File.GetAttributes(fi.FullName);
                    if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden || fi.Length < 209715200)
                    {
                        AddHtmlDocument(fi.FullName, fi.Name, fi.Extension);
                    }
                    else
                    {
                        AddHtmlDocument(fi.FullName, fi.Name, fi.Extension, true);
                    }
                    i++;
                    if (i == 100)
                    {
                        i = 0;
                        Console.WriteLine(fi.FullName);
                    }
                }
            }
            catch (Exception m)
            {
                writer.Dispose();
                writer = new IndexWriter(FSDirectory.Open(directory), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), false, IndexWriter.MaxFieldLength.LIMITED);

                MyException mobj = new MyException(m.Message);
            }
            foreach (DirectoryInfo di in directory.GetDirectories())
            {
                addSubDirectory(di);
            }
        }

        /// <summary>
        /// Loads, parses and indexes an HTML file.
        /// </summary>
        /// <param name="path"></param>
        public void AddHtmlDocument(string path, string FileName, string Extension, bool skipFile = false)
        {
            try
            {
                Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
                doc.GetField("Name");
                string html = "";
                string[] Image_ext = { ".img", "png", ".jpeg", ".jpg", ".gif" };
                string[] Other_ext = { ".vsd", ".rpt" };

                if (Extension == ".docx" || Extension == ".doc")
                {
                    WordParcer wordParcer = new WordParcer();
                    html = wordParcer.ExtractTextWordOpenXML(path);
                }
                else if (Extension == ".xlsx" || Extension == ".xls")
                {
                    XSLSParser xslxParser = new XSLSParser();
                    html = xslxParser.ExtractText(path);
                }
                else if (Extension == ".pdf")
                {
                    PDFParser pdfParser = new PDFParser();
                    html = pdfParser.ExtractText(path);
                }
                else if (Extension == ".ppt" || Extension == ".pptx")
                {
                    WordParcer wordParcer = new WordParcer();
                    html = wordParcer.ExtractTextPptOpenXML(path);
                }
                else if (Image_ext.Contains(Extension))
                {
                    html = "";
                }
                else if (Other_ext.Contains(Extension))
                {
                    html = "NULL";
                }
                else if (skipFile)
                {
                    html = "NULL";
                }
                else
                {
                    string[] FilesTypes = { "audio", "video", "image", "octet-stream", "zip", "x-rar-compressed", "xml", "x-shockwave-flash", "x-cabinet", "x-pdb" };
                    int co = Mime.MIMETypesDictionary.Where(obj => "." + obj.Key == Extension.ToLower() && Mime.MatchLikeWord(FilesTypes, obj.Value)).Count();
                    if (co == 0)
                    {
                        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                        {
                            html = sr.ReadToEnd();
                        }
                    }
                    else
                    {
                        html = "NULL";
                    }

                }
                if (html != null)
                {
                    int relativePathStartsAt = this.docRootDirectory.EndsWith("\\") ? this.docRootDirectory.Length : this.docRootDirectory.Length + 1;
                    string relativePath = path.Substring(relativePathStartsAt);
                    if (Extension == ".html")
                    {
                        doc.Add(new Field("text", ParseHtml(html), Field.Store.YES, Field.Index.ANALYZED));
                    }
                    else if (Extension == ".xml")
                    {

                        doc.Add(new Field("text", ParseXML(html), Field.Store.YES, Field.Index.ANALYZED));
                    }
                    else
                    {
                        doc.Add(new Field("text", (html), Field.Store.YES, Field.Index.ANALYZED));
                    }

                    doc.Add(new Field("path", path, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("title", FileName.Replace(Extension, ""), Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("Extension", Extension, Field.Store.YES, Field.Index.ANALYZED));

                    IEnumerable<string> subwords = path.Split('\\').Where(x => x.Length > 0).Select(x => x.Trim());
                    int index = 2;//index is the index location of the project name in the path-2 in order to get name of each individual support project
                    string word = subwords.ElementAt<string>(index);
                    if (!word.Equals(null))
                        doc.Add(new Field("Project", word, Field.Store.YES, Field.Index.NOT_ANALYZED));

                    if (Extension == ".docx" || Extension == ".doc" || Extension == ".pdf")
                    {
                        doc.Add(new Field("EXTPRP", "Doc", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    else if (Extension == ".txt" || Extension == ".cs" || Extension == ".xml" || Extension == ".css" || Extension == ".aspx" || Extension == ".htm" || Extension == ".js")
                    {
                        doc.Add(new Field("EXTPRP", "Code", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    else if (Extension == ".png" || Extension == ".jpg" || Extension == ".jpeg" || Extension == ".bmp" || Extension == ".gif")
                    {
                        doc.Add(new Field("EXTPRP", "Images", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    else
                    {
                        doc.Add(new Field("EXTPRP", "Other", Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                   // Console.WriteLine(FileName);
                    writer.AddDocument(doc);

                }
            }
            catch (Exception m)
            {
                MyException mobj = new MyException(m.Message);
            }
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. 
        /// </summary>
        /// <param name="html">HTML document</param>
        /// <returns>Plain text.</returns>
        private static string ParseHtml(string html)
        {
            string temp = Regex.Replace(html, "<[^>]*>", "");
            return temp.Replace("&nbsp;", " ");
        }
        private static string ParseXML(string html)
        {
            string temp = Regex.Replace(html, "\"", "");
            return temp.Replace("&nbsp;", " ");
        }
        /// <summary>
        /// Finds a title of HTML file. Doesn't work if the title spans two or more lines.
        /// </summary>
        /// <param name="html">HTML document.</param>
        /// <returns>Title string.</returns>
        private static string GetTitle(string html)
        {
            Match m = Regex.Match(html, "<title>(.*)</title>");
            if (m.Groups.Count == 2)
                return m.Groups[1].Value;
            return "(unknown)";
        }

        /// <summary>
        /// Optimizes and save the index.
        /// </summary>
        public void Close()
        {
            try
            {
                writer.Optimize();
                writer.Dispose();
            }
            catch(Exception m)
            {
                MyException mobj = new MyException(m.Message);
            }
        }
    }
}
