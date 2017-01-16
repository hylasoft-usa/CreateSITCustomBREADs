using DIS_SDK_Connector;
using System.Xml;
using System;
using System.IO;
using System.Globalization;
using SIT.Libs.Base.SDK;
using System.Logging;
using System.Web;
using System.Diagnostics;

namespace LoggingConnector
{
    /// <summary>
    ///     A logging SDK connector using System.Logging APIs
    /// </summary>
    public class LoggingConnector : BaseSDKConnector
    {
        #region variables

        Logger logger;

        #endregion

        /// <summary>
        ///     Logs a set of entries
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <param name="messageType">Incomg message's type</param>
        /// <param name="messageSchema">Incomg message's schema</param>
        /// <param name="messagePK">Incomg message's DIS PK</param>
        /// <param name="runID">Run unique ID</param>
        /// <returns>A dis_conn_error structure providing an outcome of the processing result</returns>
        /// <remarks>Format of incoming message:
        /// <code>      
        ///     <?xml version=\"1.0\" encoding=\"UTF-16\"?>
        ///     <LogMessages>
        ///         <LoggingConfigurationFileFullPath>...</LoggingConfigurationFileFullPath>
        ///         <LogMessage>
        ///             <Text>...</Text>
        ///             <Severity>...</Severity>
        ///             <ApplicationName>...</ApplicationName>
        ///             <RunLogID>...</RunLogID>
        ///             <Timestamp>...</Timestamp><!-- formatted as Ticks -->
        ///         </LogMessage>
        ///         <LogMessage>...</LogMessage>
        ///     </LogMessages>
        /// </code> 
        /// </remarks>
        protected override dis_conn_error ProcessMessage()
        {
            try
            {
                /*Format of incoming message:
                
                <?xml version=\"1.0\" encoding=\"UTF-16\"?>
                <LogMessages>
                    <LogMessage>
                        <Text>...</Text>
                        <Severity>...</Severity>
                        <ApplicationName>...</ApplicationName>
                        <RunLogID>...</RunLogID>
                        <Timestamp>...</Timestamp><!-- optional, formatted as Ticks -->
                    </LogMessage>
                    <LogMessage>...</LogMessage>
                </LogMessages>
                */

                XmlDocument xmlIncomingMessage = new XmlDocument();
                xmlIncomingMessage.LoadXml(IncomingMessage);
                
                XmlNodeList logMessages = xmlIncomingMessage.SelectNodes("/LogMessages/LogMessage");

                foreach (XmlNode logMessage in logMessages)
                {
                    //Reading values
                    string logText, severity, applicationName, runLogID;
                    DateTime timeStamp = DateTime.Now;

                    logText = HttpUtility.HtmlDecode(logMessage.SelectSingleNode("Text").InnerXml);
                    severity = logMessage.SelectSingleNode("Severity").InnerXml;
                    applicationName = logMessage.SelectSingleNode("ApplicationName").InnerXml;
                    runLogID = logMessage.SelectSingleNode("RunLogID").InnerXml;

                    XmlNode timeStampNode = logMessage.SelectSingleNode("Timestamp");
                    if (timeStampNode != null)
                    {
                        long ticks;

                        if (long.TryParse(timeStampNode.InnerXml, out ticks))
                            timeStamp = new DateTime(ticks);
                    }
                    
                    //create a Logger instance                
                    logger = new Logger(applicationName);

                    LogSeverity logEntrySeverity = LogSeverity.FromString(severity);
                    logger.LogGroup = Int64.Parse(runLogID);

                    logger.WriteLogEntry(logEntrySeverity, logText, timeStamp);
                }
            }
            catch (Exception ex)
            {
                do
                {
                    m_TraceIt(
                        GetType().FullName,
                        string.Format(
                            "[{0}] {1} {2}{3}",
                            DateTime.Now,
                            ex.Message,
                            ex.StackTrace,
                            Environment.NewLine),
                        TraceCategory.Error);

                    ex = ex.InnerException;
                }
                while (ex != null);

                logger.LogException(ex);

                return dis_conn_error.UNRECOVERABLE;
            }

            return dis_conn_error.SUCCESS;
        }
    }
}
