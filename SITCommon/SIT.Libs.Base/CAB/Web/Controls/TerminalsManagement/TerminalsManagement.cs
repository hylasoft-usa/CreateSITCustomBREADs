using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace SIT.Libs.Base.CAB.Web.Controls.TerminalsManagement
{
    #region definitions
    /// <summary>
    /// Class wrapping the response obtained by querying the status of a terminal
    /// </summary>
    public class TerminalStatusResponse
    {
        #region variables
        bool terminalReachable;
        string statusDescription;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets whether the terminal is reachable (on or off)
        /// </summary>
        public bool TerminalReachable
        {
            get
            {
                return terminalReachable;
            }

            set
            {
                terminalReachable = value;
            }
        }

        /// <summary>
        /// Gets or sets the string describing the current status of the terminal
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return statusDescription;
            }

            set
            {
                statusDescription = value;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Builds a new TerminalStatusResponse object
        /// </summary>
        /// <param name="terminalReachable">Whether the terminal is reachable (on or off)</param>
        /// <param name="statusDescription">The string describing the current status of the terminal</param>
        public TerminalStatusResponse( bool terminalReachable, string statusDescription )
        {
            this.terminalReachable = terminalReachable;
            this.statusDescription = statusDescription;
        }

        /// <summary>
        /// 
        /// Builds a new TerminalStatusResponse object with empty description
        /// </summary>
        /// <param name="terminalReachable">Whether the terminal is reachable (on or off)</param>
        public TerminalStatusResponse( bool terminalReachable ) : this(terminalReachable, string.Empty) { }

        /// <summary>
        /// Builds a new TerminalStatusResponse object setting reachability to true and an empty description
        /// </summary>
        public TerminalStatusResponse() : this(true, string.Empty) { }
        #endregion
    }

    /// <summary>
    /// Class wrapping the response obtained by querying the timestamp
    /// last message from a terminal was received
    /// </summary>
    public class TerminalLastMessageTimestampResponse
    {
        #region variables
        DateTime lastMessageTimestamp;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets last message's timestamp
        /// </summary>
        public DateTime LastMessageTimestamp
        {
            get
            {
                return lastMessageTimestamp;
            }

            set
            {
                lastMessageTimestamp = value;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Builds a new TerminalLastMessageTimestampResponse object
        /// </summary>
        /// <param name="lastMessageTimestamp">The timestamp of the last message received from the terminal</param>
        public TerminalLastMessageTimestampResponse( DateTime lastMessageTimestamp )
        {
            this.lastMessageTimestamp = lastMessageTimestamp;
        }

        /// <summary>
        /// Builds a new TerminalLastMessageTimestampResponse using the current date/time
        /// </summary>
        public TerminalLastMessageTimestampResponse() : this(DateTime.Now) { }
        #endregion
    }

    /// <summary>
    /// The interface every class supposed to check the status of a terminal must implement
    /// </summary>
    public interface TerminalStatusChecker
    {
        /// <summary>
        /// Gets the status of a terminal
        /// </summary>
        /// <returns></returns>
        TerminalStatusResponse CheckTerminalStatus();
    }

    /// <summary>
    /// The interface every class supposed to check 
    /// the timestamp of the last message received from a terminal must implement
    /// </summary>
    public interface TerminalLastMessageTimestampChecker
    {
        /// <summary>
        /// Gets the timestamp last message from a terminal was received
        /// </summary>
        /// <returns></returns>
        TerminalLastMessageTimestampResponse CheckTerminalLastMessageTimestamp();
    }
    #endregion

    #region basic implementations
    /// <summary>
    /// Class which will check activity from a terminal by looking for the last EV message received
    /// </summary>
    [Serializable]
    public class LastEVTimestampChecker : TerminalLastMessageTimestampChecker
    {
        #region variables
        string terminalID;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets target terminal ID
        /// </summary>
        public string TerminalID
        {
            get
            {
                return terminalID;
            }

            set
            {
                terminalID = value;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new LastEVTimestampChecker
        /// </summary>
        /// <param name="terminalID">Target terminal ID</param>
        public LastEVTimestampChecker( string terminalID )
        {
            this.terminalID = terminalID;
        }
        #endregion

        #region TerminalLastMessageTimestampChecker Members
        /// <summary>
        /// Gets the timestamp of the last EV message received from a terminal ID
        /// </summary>
        /// <returns></returns>
        public TerminalLastMessageTimestampResponse CheckTerminalLastMessageTimestamp()
        {
            return new TerminalLastMessageTimestampResponse();
        }
        #endregion
    }

    /// <summary>
    /// Class which will check terminal's status by sending an ICMP (ping) request
    /// </summary>
    [Serializable]
    public class ICMPStatusChecker : TerminalStatusChecker
    {
        #region variables
        IPAddress terminalIP;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets target terminal IP address
        /// </summary>
        public IPAddress TerminalIP
        {
            get
            {
                return terminalIP;
            }

            set
            {
                terminalIP = value;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new ICMPStatusChecker
        /// </summary>
        /// <param name="terminalIP">Target terminal IP</param>
        public ICMPStatusChecker( IPAddress terminalIP )
        {
            this.terminalIP = terminalIP;
        }
        #endregion

        #region TerminalStatusChecker Members
        /// <summary>
        /// Checks the status of a terminal by sending an ICMP (ping) request
        /// </summary>
        /// <returns></returns>
        public TerminalStatusResponse CheckTerminalStatus()
        {
            TerminalStatusResponse toRet = new TerminalStatusResponse();
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            PingReply reply = null;

            try
            {
                reply = pingSender.Send(TerminalIP, timeout, buffer, options);

                if ( reply.Status == IPStatus.Success )
                {
                    toRet.TerminalReachable = true;
                    toRet.StatusDescription = string.Format(
                        @"Terminal reachable, Terminal address: {0}, ICMP round trip time: {1}, ICMP time to live: {2}",
                        reply.Address.ToString(),
                        reply.RoundtripTime,
                        reply.Options.Ttl);
                }
                else
                {
                    toRet.TerminalReachable = false;
                    toRet.StatusDescription = string.Format("Terminal unreachable: {0}", reply.Status);
                }
            }
            catch ( Exception ex )
            {
                toRet.TerminalReachable = false;
                toRet.StatusDescription = string.Format("Terminal unreachable: {0} - {1}", ex.GetType().Name, ex.Message);
            }

            return toRet;
        }
        #endregion
    }

    #endregion
}