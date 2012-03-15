﻿// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace Kayone.TestFoundation
{
    public class ExceptionVerification : Target
    {
        private static List<LogEventInfo> _logs = new List<LogEventInfo>();

        public static void Register()
        {
            var exceptionVerification = new ExceptionVerification();
            LogManager.Configuration.AddTarget("ExceptionVerification", exceptionVerification);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, exceptionVerification));

            Clear();
        }


        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Level >= LogLevel.Warn)
            {
                _logs.Add(logEvent);
            }
        }

        /// <summary>
        /// Clear all logged event
        /// </summary>
        public static void Clear()
        {
            _logs = new List<LogEventInfo>();
        }

        /// <summary>
        /// Verifies that no unexpected events were logged.
        /// </summary>
        public static void AssertNoUnexcpectedLogs()
        {
            ExpectedFatals(0);
            ExpectedErrors(0);
            ExpectedWarns(0);
        }

        private static string GetLogsString(IEnumerable<LogEventInfo> logs)
        {
            string errors = "";
            foreach (var log in logs)
            {
                string exception = "";
                if (log.Exception != null)
                {
                    exception = String.Format("[{0}: {1}]", log.Exception.GetType(), log.Exception.Message);
                }

                errors += Environment.NewLine + String.Format("[{0}] {1}: {2} {3}", log.Level, log.LoggerName, log.FormattedMessage, exception);
            }
            return errors;
        }


        public static void ExpectedErrors(int count)
        {
            Expected(LogLevel.Error, count);
        }

        public static void ExpectedFatals(int count)
        {
            Expected(LogLevel.Fatal, count);
        }

        public static void ExpectedWarns(int count)
        {
            Expected(LogLevel.Warn, count);
        }

        public static void IgnoreWarns()
        {
            Ignore(LogLevel.Warn);
        }

        public static void IgnoreErrors()
        {
            Ignore(LogLevel.Error);
        }

        public static void MarkInconclusive(Type exception)
        {
            var inconclusiveLogs = _logs.Where(l => l.Exception != null && l.Exception.GetType() == exception).ToList();

            if (inconclusiveLogs.Any())
            {
                inconclusiveLogs.ForEach(c => _logs.Remove(c));
                Assert.Inconclusive(GetLogsString(inconclusiveLogs));
            }
        }

        public static void MarkInconclusive(string text)
        {
            var inconclusiveLogs = _logs.Where(l => l.FormattedMessage.ToLower().Contains(text.ToLower())).ToList();

            if (inconclusiveLogs.Any())
            {
                inconclusiveLogs.ForEach(c => _logs.Remove(c));
                Assert.Inconclusive(GetLogsString(inconclusiveLogs));
            }
        }

        private static void Expected(LogLevel level, int count)
        {
            var levelLogs = _logs.Where(l => l.Level == level).ToList();

            if (levelLogs.Count != count)
            {

                var message = String.Format("{0} {1}(s) were expected but {2} were logged.\n\r{3}",
                    count, level, levelLogs.Count, GetLogsString(levelLogs));

                message = "\n\r****************************************************************************************\n\r"
                    + message +
                    "\n\r****************************************************************************************";

                Assert.Fail(message);
            }

            levelLogs.ForEach(c => _logs.Remove(c));
        }

        private static void Ignore(LogLevel level)
        {
            var levelLogs = _logs.Where(l => l.Level == level).ToList();
            levelLogs.ForEach(c => _logs.Remove(c));
        }
    }
}