using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace Kayone.TestFoundation
{
    public abstract class LoggingTest
    {
        [SetUp]
        public void LoggingTestSetup()
        {
            if (LogManager.Configuration == null || LogManager.Configuration is XmlLoggingConfiguration)
            {
                LogManager.Configuration = new LoggingConfiguration();
                ExceptionVerification.Register();

                SetupLogger();
            }
        }


        protected virtual void SetupLogger()
        {
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }

            RegisterConsoleLogger(LogLevel.Trace);
            RegisterUdpLogger();
        }

        [TearDown]
        public void LoggingDownBase()
        {
            ExceptionVerification.AssertNoUnexcpectedLogs();
        }


        private static void RegisterConsoleLogger(LogLevel minLevel, string loggerNamePattern = "*")
        {
            var consoleTarget = new ConsoleTarget();
            consoleTarget.Layout = "${message} ${exception}";
            LogManager.Configuration.AddTarget(consoleTarget.GetType().Name, consoleTarget);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule(loggerNamePattern, minLevel, consoleTarget));

            LogManager.ConfigurationReloaded += (sender, args) => RegisterConsoleLogger(minLevel, loggerNamePattern);
        }

        private static void RegisterUdpLogger()
        {
            var udpTarget = new NLogViewerTarget();
            udpTarget.Address = "udp://127.0.0.1:20480";
            udpTarget.IncludeCallSite = true;
            udpTarget.IncludeSourceInfo = true;
            udpTarget.IncludeNLogData = true;
            udpTarget.IncludeNdc = true;
            udpTarget.Parameters.Add(new NLogViewerParameterInfo
            {
                Name = "Exception",
                Layout = "${exception:format=ToString}"
            });

            LogManager.Configuration.AddTarget(udpTarget.GetType().Name, udpTarget);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, udpTarget));

            LogManager.ConfigurationReloaded += (sender, args) => RegisterUdpLogger();
        }
    }
}
