using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoiPoiGet
{
    public class Log
    {
        public class LogServer
        {
            public static ILog Loger(LogerEnum le)
            {
                var name = Enum.GetName(typeof(LogerEnum), le);
                return log4net.LogManager.GetLogger(name);
            }
        }
        public enum LogerEnum
        {
            MySql,
            PageError,
            Info,
            Test
        }

        public ILog PageLog
        {
            get
            {
                return Log.LogServer.Loger(Log.LogerEnum.PageError);
            }
        }
        public ILog InfoLog
        {
            get
            {
                return Log.LogServer.Loger(Log.LogerEnum.Info);
            }
        }
        public ILog TestLog
        {
            get
            {
                return Log.LogServer.Loger(Log.LogerEnum.Test);
            }
        }
    }
}
