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
    public class Junk
    {
        
        /// <summary>
        /// Creates a new index in <c>directory</c>. Overwrites the existing index in that directory.
        /// </summary>
        /// <param name="directory">Path to index (will be created if not existing).</param>
        public Junk(DirectoryInfo directory)
        {
            foreach (FileInfo fi in directory.GetFiles())
            {
              string assd=   fi.Name.Substring(fi.Name.Length - 5);
                File.Copy(fi.FullName, @"C:\SAMUEL\" + fi.Name);
            }
        }

       
       
       
    }
}
