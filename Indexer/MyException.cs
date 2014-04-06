using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public class MyException : Exception
    {
        ILog logger = log4net.LogManager.GetLogger(typeof(MyException));

        public MyException()
        {
        }

        public MyException(string message)
            : base(message)
        {
            logger.Error(message);
        }

        public MyException(string message, Exception ex)
            : base(message, ex)
        {
          
            
        }
    }
}
