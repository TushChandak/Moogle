using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Indexer
{
    public class GenerateXML
    {
        static string VirtualPath = Convert.ToString(ConfigurationSettings.AppSettings["VirtualPath"]);

        //*************************** Generate XML File *************************************************************************
        public static void GenerateXMLFile(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<directory>" +
                       "</directory>");

            //recurse through directories and return XmlNodes
            XmlElement elem = GetData(VirtualPath, doc);
            doc.DocumentElement.AppendChild(elem);

            doc.Save(path);
        }

        public static XmlElement GetData(string dirName, XmlDocument doc)
        {
            //create a new node for this directory
            XmlElement elem = doc.CreateElement("dir");
            DirectoryInfo di = new DirectoryInfo(dirName);
            elem.SetAttribute("name", di.Name);
            elem.SetAttribute("path", di.FullName);

            string R = dirName.Replace("\\\\", "\\");
            string[] s = R.Split('\\');

            if (s.Count() <= 7)
            {
                foreach (DirectoryInfo dinf in di.GetDirectories())
                {
                    //Recursively call the directory with all underlying dirs and files
                    XmlElement elemDir = GetData(dirName + "\\" + dinf.Name, doc);
                    elem.AppendChild(elemDir);
                }
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
    }
}
