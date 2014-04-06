using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    class Program
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string VirtualPath = Convert.ToString(ConfigurationSettings.AppSettings["VirtualPath"]);
        static string IndexPath = Convert.ToString(ConfigurationSettings.AppSettings["IndexPath"]);
        static string XMLFilePath = Convert.ToString(ConfigurationSettings.AppSettings["XMLFilePath"]);

        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                Console.WriteLine("Indexing...");
                DateTime start = DateTime.Now;

                IntranetIndexer indexer;
                // indexer.AddDirectory(new DirectoryInfo(@"\\PUNE-DATA-01\Support Projects"), "*.*");
                //indexer.AddDirectory(new DirectoryInfo(@"\\SAGITEC-1629\Soogle\SDCERS\"), "*.*");
                //  indexer.AddDirectory(new DirectoryInfo(@"C:\\NILASsource\NilasSupport\DEV\slnNeoSpin\NeoSpinBusinessObjects\Leaves"), "*.*");
                // indexer.AddDirectory(new DirectoryInfo(@"\\SAGITEC-1629\Soogle\SDCERS\"), "*.*");

                //indexer.AddDirectory(new DirectoryInfo(@" \\SAGITEC-1629\Soogle"), "*.*");
                Boolean create = true;
                foreach (DirectoryInfo di in new DirectoryInfo(VirtualPath).GetDirectories())
                {

                    indexer = new IntranetIndexer(IndexPath, create);
                    indexer.AddDirectory(di, "*.*");
                    indexer.Close();
                    create = false;
                }
                // indexer.AddDirectory(new DirectoryInfo(VirtualPath), "*.*");

                // indexer.Close();

                GenerateXML.GenerateXMLFile(XMLFilePath);

            }
            catch (Exception m)
            {
                MyException mobj = new MyException(m.Message);
            }
        }
    }
}
