using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FO.CLS.LOG
{
    public class LogOnce
    {
        protected Write logw;

        string fileName = "";
        string prefix = "";

        string lastmsg = string.Empty;

        public LogOnce(string _fileName, string _prefix, Write _log)
        {
            fileName = _fileName;
            prefix = "[" + _prefix + "] ";
            logw = _log;
        }

        public void log(string msg)
        {
            if (lastmsg != msg)
            {
                logw.WriteLog(fileName, prefix + msg);

                lastmsg = msg;
            }
        }
    }
}
