using System;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.ExtensionMethods;
using System.Xml.Linq;
using System.Net.Mime;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace System
{
    /// <summary>
    ///     A set of public helper methods
    /// </summary>
    public class HelperMethods
    {
        /// <summary>
        ///     Represents a mail attachment to be used by SendEmail method
        /// </summary>
        public class MailAttachment
        {
            #region fields

            private MemoryStream stream;
            private string filename;
            private string mediaType;

            #endregion

            #region properties

            /// <summary>
            ///     Gets the data stream for this attachment
            /// </summary>
            public Stream Data
            {
                get
                {
                    return stream;
                }
            }

            /// <summary>
            ///     Gets the original filename for this attachment
            /// </summary>
            public string Filename
            {
                get
                {
                    return filename;
                }
            }

            /// <summary>
            ///     Gets the attachment type: bytes or string
            /// </summary>
            public string MediaType
            {
                get
                {
                    return mediaType;
                }
            }

            /// <summary>
            ///     Gets the file for this attachment (as a new attachment)
            /// </summary>
            public Attachment File
            {
                get
                {
                    return new Attachment(Data, Filename, MediaType);
                }
            }

            #endregion

            #region constructors

            /// <summary>
            ///     Construct a mail attachment form a byte array
            /// </summary>
            /// <param name="data">Bytes to attach as a file</param>
            /// <param name="filename">Logical filename for attachment</param>
            public MailAttachment(byte[] data, string filename)
            {
                this.stream = new MemoryStream(data);
                this.filename = filename;
                this.mediaType = MediaTypeNames.Application.Octet;
            }

            /// <summary>
            ///     Construct a mail attachment from a string
            /// </summary>
            /// <param name="data">string to attach as a file</param>
            /// <param name="filename">Logical filename for attachment</param>
            public MailAttachment(string data, string filename)
            {
                this.stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(data));
                this.filename = filename;
                this.mediaType = MediaTypeNames.Text.Html;
            }

            #endregion
        }

        /// <summary>
        ///     Gets the name of the method currently running, i.e. the name of the caller method
        /// </summary>
        /// <param name="getDeclaringType">Whether the declaring type should be returned along with method's name</param>
        /// <returns>The name of the caller method</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName(bool getDeclaringType)
        {
            var st = new StackTrace(new StackFrame(1));
            MethodBase method = st.GetFrame(0).GetMethod();

            if (getDeclaringType)
                return string.Format("{0}.{1}", method.DeclaringType, method.Name);
            else
                return method.Name;
        }

        #region date and time management

        /// <summary>
        ///     Finds the bias from a local date time
        /// </summary>
        /// <param name="dateTime">The local date time to use in order to retrieve the bias</param>
        /// <returns>The UTC offset in minutes</returns>
        public static short GetBias(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc) return 0;
            return (short)TimeZone.CurrentTimeZone.GetUtcOffset(dateTime).TotalMinutes;
        }

        /// <summary>
        ///     Converts a UTC DateTime into a given time zone and formats in accordance with the culture
        /// </summary>
        /// <param name="dateTime">The date time to convert</param>
        /// <param name="timeZone">Time zone used during local time conversion</param>
        /// <param name="culture">Culture used for the date time format</param>
        /// <returns>The string representation of dateTime converted into the given time zone and formatted in accordance with the culture</returns>
        public static string GetLocalTimeAndFormat(DateTime dateTime, string timeZone, string culture)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                dateTime = dateTime.ToUniversalTime();

            TimeZoneInfo userTimeZone = null;
            DateTime localDateTime = new DateTime();
            string localDateAndFormat = string.Empty;

            userTimeZone = string.IsNullOrEmpty(timeZone) ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            localDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, userTimeZone);

            return localDateTime.ToString(new CultureInfo(culture));
        }

        #endregion

        #region SQL helper methods

        const string DATETIME_FORMAT_ROUNDTRIP = "o";

        /// <summary>
        ///     Gets the text of a SQL command replacing parameters
        /// </summary>
        /// <param name="sqc">The command which text must be returned</param>
        /// <returns>A fully working SQL command text</returns>
        /// <remarks>Parameters are replaced "inline", works with commands meant to invoke stored procedures as well</remarks>
        public static string GetCommandText(SqlCommand sqc)
        {
            StringBuilder sbCommandText = new StringBuilder();

            //sbCommandText.AppendLine("-- BEGIN COMMAND");

            // params
            for (int i = 0; i < sqc.Parameters.Count; i++)
                appendParameterAsSqlBatch(sqc.Parameters[i], sbCommandText);

            //sbCommandText.AppendLine("-- END PARAMS");

            // command
            if (sqc.CommandType == CommandType.StoredProcedure)
            {
                sbCommandText.Append("EXEC ");

                bool hasReturnValue = false;

                for (int i = 0; i < sqc.Parameters.Count; i++)
                {
                    if (sqc.Parameters[i].Direction == ParameterDirection.ReturnValue)
                        hasReturnValue = true;
                }

                if (hasReturnValue)
                    sbCommandText.Append("@returnValue = ");

                sbCommandText.Append(sqc.CommandText);

                bool hasPrev = false;

                for (int i = 0; i < sqc.Parameters.Count; i++)
                {
                    var cParam = sqc.Parameters[i];
                    if (cParam.Direction != ParameterDirection.ReturnValue)
                    {
                        if (hasPrev)
                            sbCommandText.Append(", ");

                        sbCommandText.Append(cParam.ParameterName);
                        sbCommandText.Append(" = ");
                        sbCommandText.Append(cParam.ParameterName);

                        if (cParam.Direction.HasFlag(ParameterDirection.Output))
                            sbCommandText.Append(" OUTPUT");

                        hasPrev = true;
                    }
                }
            }
            else
                sbCommandText.AppendLine(sqc.CommandText);

            //sbCommandText.AppendLine("-- RESULTS");
            //sbCommandText.Append("SELECT 1 as Executed");

            for (int i = 0; i < sqc.Parameters.Count; i++)
            {
                var cParam = sqc.Parameters[i];

                if (cParam.Direction == ParameterDirection.ReturnValue)
                    sbCommandText.Append(", @returnValue as ReturnValue");
                else
                {
                    if (cParam.Direction.HasFlag(ParameterDirection.Output))
                    {
                        sbCommandText.Append(", ");
                        sbCommandText.Append(cParam.ParameterName);
                        sbCommandText.Append(" as [");
                        sbCommandText.Append(cParam.ParameterName);
                        sbCommandText.Append(']');
                    }
                }
            }

            sbCommandText.AppendLine(";");
            //sbCommandText.AppendLine("-- END COMMAND");

            return sbCommandText.ToString();
        }

        private static void appendParameterAsSqlBatch(SqlParameter param, StringBuilder sbCommandText)
        {
            sbCommandText.Append("DECLARE ");

            if (param.Direction == ParameterDirection.ReturnValue)
                sbCommandText.AppendLine("@returnValue INT;");
            else
            {
                sbCommandText.Append(param.ParameterName);

                sbCommandText.Append(' ');

                if (param.SqlDbType != SqlDbType.Structured)
                {
                    appendParameterType(param, sbCommandText);
                    sbCommandText.Append(" = ");
                    appendQuotedParameterValue(param.Value, sbCommandText);

                    sbCommandText.AppendLine(";");
                }
                else
                    appendStructuredParameter(param, sbCommandText);
            }
        }

        private static void appendStructuredParameter(SqlParameter param, StringBuilder sbCommandText)
        {
            sbCommandText.AppendLine(" {List Type};");
            var dataTable = (DataTable)param.Value;

            for (int rowNo = 0; rowNo < dataTable.Rows.Count; rowNo++)
            {
                sbCommandText.Append("INSERT INTO ");
                sbCommandText.Append(param.ParameterName);
                sbCommandText.Append(" VALUES (");

                bool hasPrev = true;

                for (int colNo = 0; colNo < dataTable.Columns.Count; colNo++)
                {
                    if (hasPrev)
                        sbCommandText.Append(", ");

                    appendQuotedParameterValue(dataTable.Rows[rowNo].ItemArray[colNo], sbCommandText);
                    hasPrev = true;
                }

                sbCommandText.AppendLine(");");
            }
        }

        private static void appendQuotedParameterValue(object value, StringBuilder sbCommandText)
        {
            try
            {
                if (value == null)
                    sbCommandText.Append("NULL");
                else
                {
                    value = unboxNullable(value);

                    if (value is string || value is char || value is char[] || value is XElement || value is XDocument || value is SqlXml)
                    {
                        if (value is SqlXml)
                            value = ((SqlXml)value).Value;

                        sbCommandText.Append('\'');
                        sbCommandText.Append(value.ToString().Replace("'", "''"));
                        sbCommandText.Append('\'');
                    }
                    else
                    {
                        if (value is bool)
                        {
                            // True -> 1, False -> 0
                            sbCommandText.Append(Convert.ToInt32(value));
                        }
                        else
                        {
                            if (value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal)
                                sbCommandText.Append(((IFormattable)value).ToString(null, CultureInfo.InvariantCulture));
                            else
                            {
                                if (value is DateTime)
                                {
                                    // SQL Server only supports ISO8601 with 3 digit precision on datetime,
                                    // datetime2 (>= SQL Server 2008) parses the .net format, and will 
                                    // implicitly cast down to datetime.
                                    // Alternatively, use the format string "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK"
                                    // to match SQL server parsing
                                    sbCommandText.Append("CAST('");
                                    sbCommandText.Append(((DateTime)value).ToString(DATETIME_FORMAT_ROUNDTRIP));
                                    sbCommandText.Append("' as datetime2)");
                                }
                                else
                                {
                                    if (value is DateTimeOffset)
                                    {
                                        sbCommandText.Append('\'');
                                        sbCommandText.Append(((DateTimeOffset)value).ToString(DATETIME_FORMAT_ROUNDTRIP));
                                        sbCommandText.Append('\'');
                                    }
                                    else
                                    {
                                        if (value is Guid)
                                        {
                                            sbCommandText.Append('\'');
                                            sbCommandText.Append(((Guid)value).ToString());
                                            sbCommandText.Append('\'');
                                        }
                                        else
                                        {
                                            if (value is byte[])
                                            {
                                                var data = (byte[])value;

                                                if (data.Length == 0)
                                                    sbCommandText.Append("NULL");
                                                else
                                                {
                                                    sbCommandText.Append("0x");

                                                    for (int i = 0; i < data.Length; i++)
                                                        sbCommandText.Append(data[i].ToString("h2"));
                                                }
                                            }
                                            else
                                            {
                                                if (value == DBNull.Value)
                                                    sbCommandText.Append("NULL");
                                                else
                                                {
                                                    sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                                                    sbCommandText.Append(value.GetType().ToString());
                                                    sbCommandText.Append(" */ '");
                                                    sbCommandText.Append(value.ToString());
                                                    sbCommandText.Append('\'');
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                sbCommandText.AppendLine("/* Exception occurred while converting parameter: ");
                sbCommandText.AppendLine(ex.ToString());
                sbCommandText.AppendLine("*/");
            }
        }

        private static object unboxNullable(object value)
        {
            var typeOriginal = value.GetType();

            if (typeOriginal.IsGenericType && typeOriginal.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // generic value, unboxing needed
                return typeOriginal.InvokeMember(
                    "GetValueOrDefault",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    null, 
                    value, 
                    null);
            }
            else
                return value;
        }

        private static void appendParameterType(SqlParameter param, StringBuilder sbCommandText)
        {
            switch (param.SqlDbType)
            {
                // variable length
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Binary:
                    {
                        sbCommandText.Append(param.SqlDbType.ToString().ToUpper());
                        sbCommandText.Append('(');
                        sbCommandText.Append(param.Size);
                        sbCommandText.Append(')');
                    }
                    break;
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarBinary:
                    {
                        //sbCommandText.Append(param.SqlDbType.ToString().ToUpper());
                        //sbCommandText.Append("(MAX /* Specified as ");
                        //sbCommandText.Append(param.Size);
                        //sbCommandText.Append(" */)");
                        sbCommandText.Append(param.SqlDbType.ToString().ToUpper());
                        sbCommandText.Append("(");
                        sbCommandText.Append(param.Size == 0 ? "1" : param.Size.ToString());
                        sbCommandText.Append(")");
                    }
                    break;
                // fixed length
                case SqlDbType.Text:
                case SqlDbType.NText:
                case SqlDbType.Bit:
                case SqlDbType.TinyInt:
                case SqlDbType.SmallInt:
                case SqlDbType.Int:
                case SqlDbType.BigInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Money:
                case SqlDbType.Decimal:
                case SqlDbType.Real:
                case SqlDbType.Float:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.Image:
                    {
                        sbCommandText.Append(param.SqlDbType.ToString().ToUpper());
                    }
                    break;
                // Unknown
                case SqlDbType.Timestamp:
                default:
                    {
                        sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                        sbCommandText.Append(param.SqlDbType.ToString().ToUpper());
                        sbCommandText.Append(" */ ");
                        sbCommandText.Append(param.SqlDbType.ToString().ToUpper());
                    }
                    break;
            }
        }

        #endregion

        /// <summary>
        ///     Sends an email
        /// </summary>
        /// <param name="smtpHost">The name/address of the SMTP server to use</param>
        /// <param name="recipientAddresses">A list of recipent addresses</param>
        /// <param name="body">Text of message to send</param>
        /// <param name="subject">Subject line of message</param>
        /// <param name="fromAddress">Message from address</param>
        /// <param name="fromDisplay">Display name for "message from address"</param>
        /// <param name="credentialUser">User whose credentials are used for message send</param>
        /// <param name="credentialPassword">User password used for message send</param>
        /// <param name="attachments">Optional attachments for message</param>
        public static void SendEmail(string smtpHost, List<string> recipientAddresses, string body, string subject, string fromAddress, string fromDisplay, string credentialUser, string credentialPassword, params MailAttachment[] attachments)
        {
            //body = UpgradeEmailFormat(body);

            MailMessage mail = new MailMessage();

            mail.Body = body;
            mail.IsBodyHtml = true;
            foreach (string toAddress in recipientAddresses) mail.To.Add(new MailAddress(toAddress));
            mail.From = new MailAddress(fromAddress, fromDisplay, Encoding.UTF8);
            mail.Subject = subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Priority = MailPriority.Normal;

            if (attachments != null)
                foreach (MailAttachment ma in attachments) mail.Attachments.Add(ma.File);

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(credentialUser, credentialPassword);
            smtp.Host = smtpHost;
            smtp.Send(mail);
        }

        /// <summary>
        ///     Transform an exception stack trace removing newlines, carriage returns and tabulations
        /// </summary>
        /// <param name="ex">Exception to transform</param>
        /// <returns>Exception's stack trace without carriage returns and tabulations</returns>
        public static string ToInlineStackTrace(Exception ex)
        {
            return ToInlineStackTrace(ex.StackTrace);
        }

        /// <summary>
        ///     Transform a stack trace removing newlines, carriage returns and tabulations
        /// </summary>
        /// <param name="stackTrace">Stack trace to transform</param>
        /// <returns>Stack trace without carriage returns and tabulations</returns>
        public static string ToInlineStackTrace(string stackTrace)
        {
            if (stackTrace != null)
                return stackTrace.Replace(System.Environment.NewLine, string.Empty).Replace("\t", string.Empty);

            return string.Empty;
        }
    }    
}
