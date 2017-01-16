#region Using directives

using System;
using System.Reflection;
using System.Xml;
using DIS_SDK_Connector;
using System.Diagnostics;
using System.Text;
using SIT.Libs.Base.DB;
using System.CommonPlatform.Events;
using System.Configuration;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Logging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Runtime.CompilerServices;

#endregion

namespace SIT.Libs.Base.SDK
{
    #region BaseSDKConnector

    /// <summary>
    ///     Base class for all SDK connectors implementing some basic functionalities
    /// </summary>
    public abstract class BaseSDKConnector : IExtProcessor
    {
        #region variables

        /// <summary>
        ///     Incoming message's schema
        /// </summary>
        protected string MessageSchema {get; private set;}

        /// <summary>
        ///     Incoming message's type
        /// </summary>
        protected string MessageType { get; private set; }

        /// <summary>
        ///     Incoming message's PK in DIS
        /// </summary>
        protected int MessagePK { get; private set; }

        /// <summary>
        ///     The unique ID assigned to this connector's run
        /// </summary>
        protected int RunID { get; private set; }

        /// <summary>
        ///     The incoming message
        /// </summary>
        protected string IncomingMessage { get; private set; }

        /// <summary>
        ///     Gets or sets a message in case any error during cunfiguration phase occures; if a message is set, no meesage will be processed by this connector and the error message will be logged using the method LogConfigurationError
        /// </summary>
        protected string ConfigurationErrorMessage { get; set; }

        #endregion

        #region configuration

        /// <summary>
        ///     Reads mandatory configuration infos from connector's xml configuration
        /// </summary>
        /// <param name="p_xmlconfig">SDK connector's xml configuration</param>
        /// <returns></returns>
        public override dis_conn_error configure_start(string p_xmlconfig)
        {
            try
            {
                //setting up DB gateway
                DBGateway = new SITSqlServerGateway();
            }
            catch (Exception ex)
            {
                ConfigurationErrorMessage = string.Format(
                    "Unhandled exception during configuration reading phase: '{0}' at '{1}'",
                    ex.Message,
                    HelperMethods.ToInlineStackTrace(ex));

                return dis_conn_error.SUCCESS;
            }

            return dis_conn_error.SUCCESS;
        }

        #endregion

        #region messages handling

        /// <summary>
        ///     Grabs the first message of a MessageSet and then clears the MessageSet
        /// </summary>
        /// <param name="InputSet">The MessageSet which first message is grabbed</param>
        private void GetFirstMessageAndClearInputSet(ref MessageSet InputSet)
        {
            IncomingMessage = MessageType = MessageSchema = string.Empty;

            if (InputSet.m_SubMessageArray.Count > 0)
            {
                IncomingMessage = ((SubMessage)InputSet.m_SubMessageArray[0]).m_subMessage;
                MessageType = ((SubMessage)InputSet.m_SubMessageArray[0]).m_type;
                MessageSchema = ((SubMessage)InputSet.m_SubMessageArray[0]).m_schema;
            }

            InputSet.m_SubMessageArray.Clear();
        }
        
        /// <summary>
        ///     Sends a message to DIS without waiting for its processing
        /// </summary>
        /// <param name="messageToSend">The message to send</param>
        /// <param name="messageType">messageToSend's type</param>
        /// <param name="messageSchema">messageToSend's schema</param>
        /// <returns>A dis_conn_error object describing the operation's outcome</returns>
        protected dis_conn_error SendAsynchronousMessageToDIS(string messageToSend, string messageType, string messageSchema)
        {
            if (!string.IsNullOrEmpty(messageToSend))
            {
                MessageSet messageSetToSend = new MessageSet(messageType, messageSchema, ref messageToSend);
                return m_asyncSender(ref messageSetToSend, string.Empty);
            }
            else
                return dis_conn_error.SUCCESS;
        }

        #endregion

        #region date and time management

        /// <summary>
        ///     Gets a date/time object from its unix time representation
        /// </summary>
        /// <param name="unixTime">The unix time to convert</param>
        /// <returns>The date time object represented by unixTime</returns>
        protected DateTime GetDateFromUnixTime(double unixTime)
        {
            DateTime toRet = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            toRet = toRet.AddSeconds(unixTime);

            return toRet;
        }

        /// <summary>
        ///     Gets the unix time representation of a date/time object
        /// </summary>
        /// <param name="dateTime">The date time to convert</param>
        /// <returns>The unix time representing the datetime</returns>
        protected double GetUnixTimeFromDate(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        #endregion

        #region interface to SitMesDB

        /// <summary>
        ///     Gets an object bridging to the underlying SitMesDB
        /// </summary>
        protected SITSqlServerGateway DBGateway { get; private set; }

        #endregion

        /// <summary>
        ///     Retrieves message's PK inside DIS and sets the log ID used along the whole run
        /// </summary>
        /// <param name="ProcessSet">MessageSet containing the incoming message; only the first message will be considered</param>
        /// <param name="ExternalID"></param>
        /// <param name="TCookie"></param>
        /// <param name="TimeOut"></param>
        /// <returns></returns>
        public override sealed dis_conn_error process_messageToExternal(ref MessageSet ProcessSet, string ExternalID, object TCookie, int TimeOut)
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationErrorMessage))
                {
                    //set current culture
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Culture"]))
                        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigurationManager.AppSettings["Culture"]);

                    MessagePK = ProcessSet.m_SubMessageArray.Count > 0 ? ((SubMessage)ProcessSet.m_SubMessageArray[0]).m_OperationInfo : -1;
                    RunID = (MessagePK <= 0) ? DateTime.Now.GetHashCode() : MessagePK;
                    GetFirstMessageAndClearInputSet(ref ProcessSet);

                    return ProcessMessage();
                }
                else
                {
                    LogInternalError(ConfigurationErrorMessage);
                    return dis_conn_error.UNRECOVERABLE;
                }
            }
            catch (Exception ex)
            {
                LogInternalError(string.Format(
                    "Unhandled exception: '{0}' at '{1}'",
                    ex.Message,
                    HelperMethods.ToInlineStackTrace(ex)));

                return dis_conn_error.UNRECOVERABLE;
            }
        }

        /// <summary>
        ///     Logs ConfigurationErrorMessage to the trace file configured within DIS configuration for this connector
        /// </summary>
        /// <param name="errorMessage">The error message to log</param>
        protected virtual void LogInternalError(string errorMessage)
        {
            try
            {
                m_TraceIt(
                    GetType().Name,
                    string.Format(
                        "[{0}]: {1}{2}",
                        DateTime.Now,
                        errorMessage,
                        Environment.NewLine),
                        TraceCategory.Error);
            }
            catch { }
        }

        /// <summary>
        ///     Does the actual processing - child classes should access inherited properties to grab incoming message and other useful information
        /// </summary>
        /// <returns>A dis_conn_error structure defining whether the processing nicely completed or returned an error</returns>
        protected abstract dis_conn_error ProcessMessage();
    }

    #endregion

    #region BaseLoggingSDKConnector

    /// <summary>
    ///     Base class for all SDK connectors with logging capabilities
    /// </summary>
    public abstract class BaseLoggingSDKConnector : BaseSDKConnector
    {
        #region variables

        //Log
        private Logger logger;

        #endregion

        #region configuration

        /// <summary>
        ///     Common configurations
        /// </summary>
        /// <param name="p_xmlconfig">Connector's xml configuration file</param>
        /// <returns></returns>
        public override dis_conn_error configure_start(string p_xmlconfig)
        {
            base.configure_start(p_xmlconfig);

            logger = new Logger(GetType().Name);

            return dis_conn_error.SUCCESS;
        }

        #endregion   

        #region logging
        
        #region diagnostic

        /// <summary>
        ///     Logs action described by a ActionPerformedEvent event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bread_ActionPerformed(object sender, ActionPerformedEvent e)
        {
            LogEvent(e.ActionText, e.ActionSeverity);
        }

        #endregion 

        /// <summary>
        ///     Logs an event by building a unique string containing all events. Such string will be eventually sent as asynchronous message to DIS
        /// </summary>
        /// <param name="eventText">Text to log</param>
        /// <param name="eventSeverity">Severity</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void LogEvent(string eventText, EventSeverity eventSeverity)
        {
            LogSeverity logSeverity = LogSeverity.Info;
            DateTime logTimeStamp = DateTime.Now;

            switch (eventSeverity)
            {
                case EventSeverity.Information:
                    {
                        logSeverity = LogSeverity.Info;
                        break;
                    }

                case EventSeverity.Error:
                    {
                        logSeverity = LogSeverity.Error;
                        break;
                    }

                case EventSeverity.Warning:
                    {
                        logSeverity = LogSeverity.Warn;
                        break;
                    }

                case EventSeverity.BeginOperation:
                case EventSeverity.EndOperation:
                    {
                        logSeverity = LogSeverity.Trace;
                        break;
                    }

                case EventSeverity.Debug:
                case EventSeverity.PerformanceMeasurement:
                    {
                        logSeverity = LogSeverity.Debug;
                        break;
                    }
            }

            //write logs using a thread
            StackTrace stackTrace = new StackTrace(1, true);

            var t = new Thread(() => DoWriteLogEntry(eventText, logSeverity, stackTrace));
            t.Start();
        }
        
        //Method actually writing log entries
        private void DoWriteLogEntry(string message, LogSeverity severity, StackTrace stackTrace)
        {
            LogSeverity logEntrySeverity = severity;
            logger.LogGroup = RunID;

            logger.WriteLogEntry(logEntrySeverity, message, DateTime.Now, stackTrace);
        }

        #endregion

        /// <summary>
        ///     Logs some basic information and calls DoProcessMessage
        /// </summary>
        /// <returns>A dis_conn_error structure defining whether the processing nicely completed or returned an error</returns>
        protected override dis_conn_error ProcessMessage()
        {
            dis_conn_error processResult;
            Stopwatch sw = Stopwatch.StartNew();

            LogEvent(
                string.Format(
                    "Incoming message: '{0}'",
                    IncomingMessage.Replace(Environment.NewLine, " ").Replace("\t", " ")),
                EventSeverity.Information);

            LogEvent(
                string.Format(
                    "New run starting, class name '{0}', assembly version '{1}'. DIS Message PK = '{2}'",
                    GetType().FullName,
                    Assembly.GetExecutingAssembly().GetName().Version,
                    MessagePK <= 0 ? string.Empty : MessagePK.ToString()),
                EventSeverity.BeginOperation);

            try
            {
                //call actual job performer
                processResult = DoProcessMessage();
            }
            catch (Exception ex)
            {
                LogEvent(
                    string.Format(
                        "Unhandled exception while processing message having PK '{0}': '{1}' at '{2}'",
                        MessagePK,
                        ex.Message,
                        HelperMethods.ToInlineStackTrace(ex)),
                    EventSeverity.Error);

                processResult = dis_conn_error.UNRECOVERABLE;
            }

            LogEvent(
                string.Format(
                    "Run completed in '{0}' milliseconds",
                    sw.ElapsedMilliseconds),
                EventSeverity.EndOperation);

            return processResult;
        }

        /// <summary>
        ///     Does the actual processing
        /// </summary>
        /// <returns>A dis_conn_error structure defining whether the processing nicely completed or returned an error</returns>
        protected abstract dis_conn_error DoProcessMessage();

        /// <summary>
        ///     Uses logging configuration to log the error and than calls the base logging method
        /// </summary>
        /// <param name="errorMessage">The error message to log</param>
        protected override void LogInternalError(string errorMessage)
        {
            try
            {
                LogEvent(errorMessage, EventSeverity.Error);
            }
            catch { }

            base.LogInternalError(errorMessage);
        }
    }

    #endregion

    #region BaseXmlSDKConnector

    /// <summary>
    ///     Base class for all SDK connectors accepting XML documents as input messages
    /// </summary>
    public abstract class BaseXmlSDKConnector : BaseLoggingSDKConnector
    {
        #region variables

        /// <summary>
        ///     The incoming message as XmlDocument
        /// </summary>
        protected XmlDocument XmlIncomingMessage { get; private set; }

        #endregion

        #region DoProcess implementation

        /// <summary>
        ///     Delegates the message's processing to ProcessIncomingMessage and finally flushes all the log entries
        /// </summary>
        /// <returns>A dis_conn_error structure defining whether the processing nicely completed or returned an error</returns>
        protected override sealed dis_conn_error DoProcessMessage()
        {
            dis_conn_error processResult;

            try
            {
                //call actual job performer
                XmlIncomingMessage = new XmlDocument();
                XmlIncomingMessage.LoadXml(IncomingMessage);

                processResult = ProcessIncomingMessage();
            }
            catch (Exception ex)
            {
                LogEvent(
                    string.Format(
                        "Unhandled exception while processing message having PK '{0}': '{1}' at '{2}'",
                        MessagePK,
                        ex.Message,
                        HelperMethods.ToInlineStackTrace(ex)),
                    EventSeverity.Error);

                processResult = dis_conn_error.UNRECOVERABLE;
            }

            return processResult;
        }

        #endregion

        #region abstracts

        /// <summary>
        ///     Method to be implemented by children classes in order to process an incoming message - children classes should access inherited properties to grab incoming message and other useful information
        /// </summary>
        /// <returns>A dis_conn_error structure defining whether the processing nicely completed or returned an error</returns>
        protected abstract dis_conn_error ProcessIncomingMessage();

        #endregion
    }

    #endregion
}