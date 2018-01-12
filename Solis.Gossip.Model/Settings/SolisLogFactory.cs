using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Solis.Gossip.Model.Settings
{
    public class SolisLogFactory
    {
        static SolisLogFactory()
        {
            ColoredConsoleTarget target = new ColoredConsoleTarget();
            target.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            
            SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);
        }

        public static Logger GetLogger(Type type)
        {
            var logger = LogManager.GetLogger(type.Name, type);

            return logger;
        }
    }
}
