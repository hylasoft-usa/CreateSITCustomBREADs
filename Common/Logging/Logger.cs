#region Using directives

using NLog;
using System;
using System.Threading;

#endregion

using System.Diagnostics;
using System.Linq;

namespace System.Logging
{
    /// <summary>
    ///     Defines available log severity
    /// </summary>
    public sealed class LogSeverity : IComparable, IEquatable<LogLevel>
    {
        /// <summary>
        ///     Debug log severity
        /// </summary>
        public static readonly LogSeverity Debug = new LogSeverity(0);

        /// <summary>
        ///     Error log severity
        /// </summary>
        public static readonly LogSeverity Error = new LogSeverity(1);

        /// <summary>
        ///     Fatal log severity
        /// </summary>
        public static readonly LogSeverity Fatal = new LogSeverity(2);

        /// <summary>
        ///     Info log severity
        /// </summary>
        public static readonly LogSeverity Info = new LogSeverity(3);

        /// <summary>
        ///     Trace log severity
        /// </summary>
        public static readonly LogSeverity Trace = new LogSeverity(4);

        /// <summary>
        ///     Warn log severity
        /// </summary>        
        public static readonly LogSeverity Warn = new LogSeverity(5);

        private LogLevel[] mappings = { LogLevel.Debug, LogLevel.Error, LogLevel.Fatal, LogLevel.Info, LogLevel.Trace, LogLevel.Warn };
        private LogLevel mapping;

        private LogSeverity(short id)
        {
            mapping = mappings[id];
        }

        internal LogLevel ToNLogLogLevel()
        {
            return mapping;
        }

        /// <summary>
        ///     Parses a string and returns the corresponding LogSeverity
        /// </summary>
        /// <param name="severity">Severity as string</param>
        /// <returns>The LogSeverity corresponding to the provided string or null if the string could not be parsed</returns>
        public static LogSeverity FromString(string severity)
        {
            if (string.Compare(severity, "Debug", StringComparison.OrdinalIgnoreCase) == 0) return Debug;
            if (string.Compare(severity, "Error", StringComparison.OrdinalIgnoreCase) == 0) return Error;
            if (string.Compare(severity, "Fatal", StringComparison.OrdinalIgnoreCase) == 0) return Fatal;
            if (string.Compare(severity, "Info", StringComparison.OrdinalIgnoreCase) == 0) return Info;
            if (string.Compare(severity, "Trace", StringComparison.OrdinalIgnoreCase) == 0) return Trace;
            if (string.Compare(severity, "Warn", StringComparison.OrdinalIgnoreCase) == 0) return Warn;
            
            return null;
        }

        /// <summary>
        ///     Compares the severity to the other LogSeverity object
        /// </summary>
        /// <param name="obj">The LogSeverity to compare</param>
        /// <returns>A value less than zero when this LogSeverity is less than the other LogSeverity, 0 when they are equal and greater than zero when this LogSeverity is greater than the other LogSeverity</returns>
        public int CompareTo(object obj) 
        {
            return ToNLogLogLevel().CompareTo(obj);
        }
        
        /// <summary>
        ///     Determines whether the specified LogSeverity instance is equal to this instance
        /// </summary>
        /// <param name="other">The LogSeverity to compare</param>
        /// <returns>True if the specified LogSeverity is equal to this instance; otherwise, false</returns>
        public bool Equals(LogLevel other)
        {
            return ToNLogLogLevel().Equals(other);
        }
        
        /// <summary>
        ///     Determines whether the specified System.Object is equal to this instance
        /// </summary>
        /// <param name="obj">The System.Object to compare</param>
        /// <returns>True if the specified System.Object is equal to this instance; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            return ToNLogLogLevel().Equals(obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table</returns>
        public override int GetHashCode()
        {
            return ToNLogLogLevel().GetHashCode();
        }

        /// <summary>
        ///     Returns a string representation of the log severity
        /// </summary>
        /// <returns>Log severity's name</returns>
        public override string ToString()
        {
            return ToNLogLogLevel().ToString();
        }

        /// <Summary>
        ///     Compares two LogSeverity objects and returns a value indicating whether the first one is not equal to the second one
        /// </Summary>
        /// <param name="level1">The first level</param>
        /// <param name="level2">The second level</param>
        /// <returns>The value of level1.Ordinal != level2.Ordinal</returns>        
        public static bool operator !=(LogSeverity level1, LogSeverity level2)
        {
            return level1.ToNLogLogLevel() != level2.ToNLogLogLevel();
        }

        /// <Summary>
        ///     Compares two LogSeverity objects and returns a value indicating whether the first one is less than the second one.
        /// </Summary>:
        /// <param name="level1">The first level</param>
        /// <param name="level2">The second level</param>
        /// <returns>The value of level1.Ordinal &lt; level2.Ordinal</returns>
        public static bool operator <(LogSeverity level1, LogSeverity level2)
        {
            return level1.ToNLogLogLevel() < level2.ToNLogLogLevel();
        }
        ///
        /// <Summary>
        ///     Compares two LogSeverity objects and returns a value indicating whether the first one is less than or equal to the second one.
        /// </Summary>:
        /// <param name="level1">The first level</param>
        /// <param name="level2">The second level</param>
        /// <returns>The value of level1.Ordinal &lt;= level2.Ordinal</returns>
        public static bool operator <=(LogSeverity level1, LogSeverity level2)
        {
            return level1.ToNLogLogLevel() <= level2.ToNLogLogLevel();
        }
        ///
        /// <Summary>
        ///     Compares two LogSeverity objects and returns a value indicating whether the first one is equal to the second one.
        /// </Summary>:
        /// <param name="level1">The first level</param>
        /// <param name="level2">The second level</param>
        /// <returns>The value of level1.Ordinal == level2.Ordinal</returns>
        public static bool operator ==(LogSeverity level1, LogSeverity level2)
        {
            return level1.ToNLogLogLevel() == level2.ToNLogLogLevel();
        }
        ///
        /// <Summary>
        ///     Compares two LogSeverity objects and returns a value indicating whether the first one is greater than the second one.
        /// </Summary>:
        /// <param name="level1">The first level</param>
        /// <param name="level2">The second level</param>
        /// <returns>The value of level1.Ordinal &gt; level2.Ordinal</returns>
        public static bool operator >(LogSeverity level1, LogSeverity level2)
        {
            return level1.ToNLogLogLevel() > level2.ToNLogLogLevel();
        }
        ///
        /// <Summary>
        ///     Compares two LogSeverity objects and returns a value indicating whether the first one is greater than or equal to the second one.
        /// </Summary>:
        /// <param name="level1">The first level</param>
        /// <param name="level2">The second level</param>
        /// <returns>The value of level1.Ordinal &gt;= level2.Ordinal</returns>
        public static bool operator >=(LogSeverity level1, LogSeverity level2)
        {
            return level1.ToNLogLogLevel() >= level2.ToNLogLogLevel();
        }
    }

    /// <summary>
    ///     Web logger for CAB applications based on Microsoft Enterprise Library framework
    /// </summary>
    public class Logger
    {
        #region variables

        private static NLog.Logger logger;

        #endregion

        #region public properties

        /// <summary>
        ///     Enables/disables tracing for this logger
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        ///     Logging application name, used when writing a log entry
        /// </summary>
        public string LoggingApplication
        {
            get;
            set;
        }

        /// <summary>
        ///     A long integer identifier that will be used as additional ID for log entries produced by this logger; it can be useful to help grouping sets of logically related log entries
        /// </summary>
        /// <remarks>0 is interpreted as "no grouping"</remarks>
        public long LogGroup
        {
            get;
            set;
        }

        #endregion

        #region constructors

        /// <summary>
        ///     Creates a new Logger object
        /// </summary>
        /// <param name="loggingApplication">The name of the application using this log - can be used as filter inside log's runtime configuration</param>
        public Logger(string loggingApplication) : this(loggingApplication, 0) { }

        /// <summary>
        ///     Creates a new Logger object
        /// </summary>
        /// <param name="loggingApplication">The name of the application using this log - can be used as filter inside log's runtime configuration</param>
        /// <param name="logGroup">An integer identifier that will be used as additional ID for log entries produced by this logger; it can be useful to help grouping sets of logically related log entries</param>
        public Logger(string loggingApplication, int logGroup)
        {
            LoggingApplication = loggingApplication;
            LogGroup = logGroup;
            Enabled = true;

            logger = LogManager.GetLogger(LoggingApplication);
        }

        #endregion

        #region methods exposed in order to log various categories of events

        /// <summary>
        ///     Writes a log entry
        /// </summary>
        /// <param name="severity">Log entry's severity</param>
        /// <param name="message">The message to log</param>
        public void WriteLogEntry(LogSeverity severity, string message)
        {
            DoWriteLogEntry(severity, message, DateTime.Now, string.Empty);
        }

        /// <summary>
        ///     Writes a log entry
        /// </summary>
        /// <param name="severity">Log entry's severity</param>
        /// <param name="message">The message to log</param>
        /// <param name="timeStamp">Log entry's timestamp</param>
        public void WriteLogEntry(LogSeverity severity, string message, DateTime timeStamp)
        {
            DoWriteLogEntry(severity, message, timeStamp, string.Empty);
        }

        /// <summary>
        ///     Writes a log entry
        /// </summary>
        /// <param name="severity">Log entry's severity</param>
        /// <param name="timeStamp">Log entry's timestamp</param>
        /// <param name="message">The message to log</param>
        /// <param name="stackTrace">The stack trace associated to the log entry being written</param>
        public void WriteLogEntry(LogSeverity severity, string message, DateTime timeStamp, string stackTrace)
        {
            DoWriteLogEntry(severity, message, timeStamp, stackTrace);
        }

        /// <summary>
        ///     Logs an exception object
        /// </summary>
        /// <param name="raisedException">Exception to log</param>
        public void LogException(Exception raisedException)
        {
            DoWriteLogEntry(LogSeverity.Error, raisedException.Message, DateTime.Now, raisedException.StackTrace);
        }

        /// <summary>
        ///     Logs an exception object
        /// </summary>
        /// <param name="raisedException">Exception to log</param>
        /// <param name="timeStamp">Log entry's timestamp</param>
        public void LogException(Exception raisedException, DateTime timeStamp)
        {
            DoWriteLogEntry(LogSeverity.Error, raisedException.Message, timeStamp, raisedException.StackTrace);
        }

        private void DoWriteLogEntry(LogSeverity severity, string message, DateTime timeStamp, string stackTrace)
        {
            LogEventInfo logEntry = LogEventInfo.Create(severity.ToNLogLogLevel(), logger.Name, message);

            logEntry.TimeStamp = timeStamp;
            logEntry.Properties["GroupID"] = LogGroup;
            logEntry.Properties["StackTrace"] = stackTrace;

            if (Enabled)
                logger.Log(logEntry);
        }

        #endregion
    }
}