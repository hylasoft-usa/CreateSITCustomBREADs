using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using SIT.Libs.Base.CAB.Web.Controls.TerminalsManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.GreenDot.gif", "image/gif")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.RedDot.gif", "image/gif")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    /// <summary>
    /// A web control capable of showing and constantly checking:
    /// <ul>
    /// <li>The Status of a terminal (reachable/not reachable)</li>
    /// <li>The timestamp of the last message received from that terminal</li>
    /// </ul>
    /// </summary>
    [ToolboxBitmap(typeof(System.Web.UI.WebControls.Button))]
    [Description("A web control capable of showing and constantly checking: 1) The Status of a terminal (reachable/not reachable) 2) The timestamp of the last message received from that terminal")]
    public class TermIDStatusChecker : WebControl, ICallbackEventHandler
    {
        #region variables
        //main table
        Table mainTable;
        TableRow trStatus;
        TableRow trLastMessage;
        TableCell tdStatusDescription;
        TableCell tdStatusImage;
        TableCell tdStatusString;
        TableCell tdLastMessageDescription;
        TableCell tdLastMessageString;

        //control's content
        Label labelStatusDescription;
        Label labelStatus;
        Label labelLastMessageDescription;
        Label labelLastMessage;
        System.Web.UI.WebControls.Image imageStatus;

        //main panel
        Panel mainPanel;

        ClientScriptManager cm;
        string JSONCallbackToken = "{{'statusImageUrl':'{0}','statusDescription':'{1}','lastMessageTimestamp':'{2}','displayMode':{3}}}";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets target terminal ID
        /// </summary>
        [Description("Gets or sets target terminal ID")]
        public string TerminalID
        {
            get
            {
                if ( ViewState["TerminalID"] == null )
                    return string.Empty;
                else
                    return ViewState["TerminalID"].ToString();
            }
            set
            {
                ViewState["TerminalID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the object which will actually check terminal's status
        /// </summary>
        [Description("Gets or sets the object which will actually check terminal's status")]
        [Browsable(false)]
        public TerminalStatusChecker TerminalStatusCheckerObject
        {
            get
            {
                if ( ViewState["TerminalStatusCheckerObject"] == null )
                    return null;
                else
                    return (TerminalStatusChecker)ViewState["TerminalStatusCheckerObject"];
            }
            set
            {
                ViewState["TerminalStatusCheckerObject"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the object which will actually check terminal's last message's timestamp
        /// </summary>
        [Description("Gets or sets the object which will actually check terminal's last message's timestamp")]
        [Browsable(false)]
        public TerminalLastMessageTimestampChecker TerminalLastMessageTimestampCheckerObject
        {
            get
            {
                if ( ViewState["TerminalLastMessageTimestampCheckerObject"] == null )
                    return null;
                else
                    return (TerminalLastMessageTimestampChecker)ViewState["TerminalLastMessageTimestampCheckerObject"];
            }
            set
            {
                ViewState["TerminalLastMessageTimestampCheckerObject"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the label indicating the status of the terminal
        /// </summary>
        [Localizable(true)]
        [Description("Gets or sets the text oft he label indicating the status of the terminal")]
        public string TerminalStatusString
        {
            get
            {
                if ( ViewState["TerminalStatusString"] == null )
                    return "Status";
                else
                    return ViewState["TerminalStatusString"].ToString();
            }

            set
            {
                ViewState["TerminalStatusString"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the label indicating the timestamp of the last message received
        /// from the terminal
        /// </summary>
        [Localizable(true)]
        [Description("Gets or sets the text of the label indicating the timestamp of the last message received from the terminal")]
        public string TerminalLastMessageTimestampString
        {
            get
            {
                if ( ViewState["TerminalLastMessageTimestampString"] == null )
                    return "Last message received on";
                else
                    return ViewState["TerminalLastMessageTimestampString"].ToString();
            }

            set
            {
                ViewState["TerminalLastMessageTimestampString"] = value;
            }
        }

        ///// <summary>
        ///// Gets or sets the tecnique to be used to give visual feedback of the terminal's status
        ///// </summary>
        //[Description("Gets or sets the tecnique to be used to give visual feedback of the terminal's status")]
        //public StatusFeedbackBehaviors StatusFeedbackBehavior
        //{
        //    get
        //    {
        //        if (ViewState["StatusFeedbackBehavior"] == null)
        //            return StatusFeedbackBehaviors.UseColoredDots;
        //        else
        //            return (StatusFeedbackBehaviors)ViewState["StatusFeedbackBehavior"];
        //    }

        //    set
        //    {
        //        ViewState["StatusFeedbackBehavior"] = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the way this control will be rendered on screen
        /// </summary>
        [Description("Gets or sets the way this control will be rendered on screen")]
        public DisplayModes DisplayMode
        {
            get
            {
                if ( ViewState["DisplayMode"] == null )
                    return DisplayModes.Compact;
                else
                    return (DisplayModes)ViewState["DisplayMode"];
            }

            set
            {
                ViewState["DisplayMode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the refreshing interval (in milliseconds)
        /// </summary>
        [Description("Gets or sets the length of the refreshing interval (in milliseconds)")]
        [DefaultValue(5000)]
        public int RefreshIntervalMilliseconds
        {
            get
            {
                if ( ViewState["RefreshIntervalMilliseconds"] == null )
                    return 5000;
                else
                    return (int)ViewState["RefreshIntervalMilliseconds"];
            }

            set
            {
                ViewState["RefreshIntervalMilliseconds"] = value;
            }
        }
        #endregion

        #region overrides
        /// <summary>
        /// Adds children controls needed to render
        /// </summary>
        protected override void CreateChildControls()
        {
            mainPanel = createMainPanel();
            this.Controls.Add(mainPanel);
        }

        /// <summary>
        /// Injects the scripts needed to manage the callback on client side
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender(e);

            //register client script blocks for callbacks
            cm = Page.ClientScript;

            string cbReference = cm.GetCallbackEventReference(this, "arg", string.Format("ReceiveServerData_{0}", ClientID), string.Empty);
            string callbackScript = string.Format("function CallServer_{0}(arg, context) {{ {1}; }}", ClientID, cbReference);

            string receiveServerDataScript = string.Format(@"
                //function processing callback result
                function ReceiveServerData_{0}(arg, context) 
                {{ 
                    var callbackResult = eval('(' + arg + ')');

                    switch(callbackResult.displayMode)
                    {{
                        case 0://Compact
                        {{
                            var img = document.getElementById('{1}');
                            
                            try
                            {{
                                img.src = callbackResult.statusImageUrl;
                                img.title = '{2}'.replace('{{0}}', callbackResult.lastMessageTimestamp);
                            }}
                            catch(ex){{}}
                        }}

                        case 1://Extended
                        {{
                            var img = document.getElementById('{1}');
                            var statusDescriptionLabel = document.getElementById('{3}');
                            var lastMessageLabel = document.getElementById('{4}');
                            
                            try
                            {{
                                img.src = callbackResult.statusImageUrl;
                                statusDescriptionLabel.innerText = callbackResult.statusDescription;
                                lastMessageLabel.innerText = callbackResult.lastMessageTimestamp;
                            }}
                            catch(ex){{}}
                            
                        }}
                    }}
                }}",
                ClientID,
                imageStatus.ClientID,
                GetTooltipMessage(TerminalID, "{0}"),
                labelStatus.ClientID,
                labelLastMessage.ClientID);

            cm.RegisterClientScriptBlock(this.GetType(), "RegisterCallServer_" + ClientID, callbackScript, true);
            cm.RegisterClientScriptBlock(this.GetType(), "RegisterReceiveServerData_" + ClientID, receiveServerDataScript, true);
        }

        /// <summary>
        /// Renders the control to the specified HTML writer
        /// </summary>
        /// <param name="writer">The System.Web.UI.HtmlTextWriter object that receives the control content</param>
        protected override void Render( HtmlTextWriter writer )
        {
            if ( DesignMode )
                createMainPanel();

            if ( string.IsNullOrEmpty(TerminalID) )
            {
                new LiteralControl("ERROR: TerminalID property not set").RenderControl(writer);
            }
            else
            {
                if ( TerminalStatusCheckerObject == null )
                {
                    new LiteralControl("ERROR: TerminalStatusCheckerObject property not set").RenderControl(writer);
                }
                else
                {
                    if ( TerminalLastMessageTimestampCheckerObject == null )//can check status
                    {
                        new LiteralControl("ERROR: TerminalLastMessageTimestampCheckerObject property not set").RenderControl(writer);
                    }
                    else
                    {
                        #region inject scripts

                        string refreshingScript = string.Format(@"
                            window.setInterval(Refresh_{0}, {1});

                            function Refresh_{0}() 
                            {{
                                try
                                {{
                                    CallServer_{0}('{2}');
                                }}
                                catch(ex)
                                {{
                                }}
                            }}",
                            ClientID,
                            RefreshIntervalMilliseconds.ToString(),
                            string.Format("{0}#{1}#{2}", //#is a character not used in base64 serialization, used as separator
                                Serialize(TerminalStatusCheckerObject),
                                Serialize(TerminalLastMessageTimestampCheckerObject),
                                ( (int)DisplayMode ).ToString()));

                        cm.RegisterStartupScript(this.GetType(), "Init_" + ClientID, refreshingScript, true);
                        #endregion

                        #region status
                        labelStatusDescription.Text = TerminalStatusString;

                        TerminalStatusResponseSharedToken terminalStatusToken = parseStatusResponse(TerminalStatusCheckerObject.CheckTerminalStatus());
                        labelStatus.Text = terminalStatusToken.StatusDescription;
                        imageStatus.ImageUrl = terminalStatusToken.ImageUrl;
                        #endregion

                        #region last message received
                        labelLastMessageDescription.Text = TerminalLastMessageTimestampString;
                        if ( TerminalLastMessageTimestampCheckerObject != null )//can check last message
                        {
                            TerminalLastMessageResponseSharedToken terminalLastMessageToken = parseLastMessageTimestampResponse(TerminalLastMessageTimestampCheckerObject.CheckTerminalLastMessageTimestamp());
                            labelLastMessage.Text = terminalLastMessageToken.LastMessageTimestamp;
                        }
                        #endregion

                        switch ( DisplayMode )
                        {
                            case DisplayModes.Compact:
                                {
                                    tdStatusDescription.Visible = false;
                                    tdStatusString.Visible = false;
                                    trLastMessage.Visible = false;

                                    mainPanel.GroupingText = string.Empty;
                                    imageStatus.ToolTip = GetTooltipMessage(TerminalID, labelLastMessage.Text);//show last message's timestamp as tooltip

                                    break;
                                }

                            case DisplayModes.Extended:
                                {
                                    tdStatusDescription.Visible = true;
                                    tdStatusString.Visible = true;
                                    trLastMessage.Visible = true;

                                    mainPanel.GroupingText = TerminalID;
                                    imageStatus.ToolTip = string.Empty;//do not show last message's timestamp as tooltip

                                    break;
                                }
                        }

                        //finally...
                        mainPanel.ApplyStyle(this.ControlStyle);
                        mainPanel.RenderControl(writer);
                    }
                }
            }
        }


        #endregion

        #region privates
        //Used internally to get a valid main panel
        private Panel createMainPanel()
        {
            Panel toRet = new Panel();
            toRet.ID = string.Format("{0}_{1}", this.ID, "MainPanel");

            //inner table definitions
            mainTable = new Table();
            mainTable.Style[HtmlTextWriterStyle.Margin] = Unit.Pixel(3).ToString();
            mainTable.CellPadding = 3;
            mainTable.Width = Unit.Percentage(100.0);

            trStatus = new TableRow();
            trLastMessage = new TableRow();

            tdStatusDescription = new TableCell();
            tdStatusDescription.Wrap = false;
            tdStatusImage = new TableCell();
            tdStatusImage.Wrap = false;
            tdStatusString = new TableCell();
            tdStatusString.Width = Unit.Percentage(100.0);
            tdStatusString.Wrap = false;

            tdLastMessageDescription = new TableCell();
            tdLastMessageDescription.Wrap = false;
            tdLastMessageString = new TableCell();
            tdLastMessageString.Wrap = false;
            tdLastMessageString.Width = Unit.Percentage(100.0);
            tdLastMessageString.ColumnSpan = 2;

            //cell (1,1)
            labelStatusDescription = new Label();
            tdStatusDescription.Controls.Add(labelStatusDescription);

            //cell (1,2)
            imageStatus = new System.Web.UI.WebControls.Image();
            tdStatusImage.Controls.Add(imageStatus);

            //cell (1,3)      
            labelStatus = new Label();
            tdStatusString.Controls.Add(labelStatus);

            //add cells to first row
            trStatus.Cells.Add(tdStatusDescription);
            trStatus.Cells.Add(tdStatusImage);
            trStatus.Cells.Add(tdStatusString);

            //cell (2,1)
            labelLastMessageDescription = new Label();
            tdLastMessageDescription.Controls.Add(labelLastMessageDescription);

            //cell (2,2)
            labelLastMessage = new Label();
            tdLastMessageString.Controls.Add(labelLastMessage);

            //add cells to second row
            trLastMessage.Cells.Add(tdLastMessageDescription);
            trLastMessage.Cells.Add(tdLastMessageString);

            mainTable.Rows.Add(trStatus);
            mainTable.Rows.Add(trLastMessage);

            toRet.Controls.Add(mainTable);
            return toRet;
        }

        //parses a TerminalStatusResponse object
        private TerminalStatusResponseSharedToken parseStatusResponse( TerminalStatusResponse toParse )
        {
            TerminalStatusResponseSharedToken toRet = new TerminalStatusResponseSharedToken();

            toRet.StatusDescription = string.IsNullOrEmpty(toParse.StatusDescription) ? string.Empty : string.Format("({0})", toParse.StatusDescription);
            toRet.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), string.Format("SIT.Libs.Base.CAB.Web.Controls.Images.{0}Dot.gif", (toParse.TerminalReachable ? "Green" : "Red")));

            return toRet;
        }

        //parses a TerminalLastMessageTimestampResponse object
        private TerminalLastMessageResponseSharedToken parseLastMessageTimestampResponse( TerminalLastMessageTimestampResponse toParse )
        {
            TerminalLastMessageResponseSharedToken toRet = new TerminalLastMessageResponseSharedToken();

            toRet.LastMessageTimestamp = toParse.LastMessageTimestamp.ToString();

            return toRet;
        }

        //used to get tooltip's message in case of compact visualization
        private string GetTooltipMessage( string terminalID, string lastMessageTimestamp )
        {
            return string.Format(Localizations.TermIDCheckerCompactModeTooltip, terminalID, lastMessageTimestamp);
        }
        #endregion

        #region ICallbackEventHandler Members
        TerminalStatusChecker deserializedTerminalStatusCheckerObject;
        TerminalLastMessageTimestampChecker deserializedTerminalLastMessageTimestampCheckerObject;
        string deserializedDisplayMode;

        /// <summary>
        /// Returns the results of the refreshing callback
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult()
        {
            string statusImageUrl = string.Empty;
            string statusDescription = string.Empty;
            string lastMessageTimestamp = string.Empty;

            if ( deserializedTerminalStatusCheckerObject != null )
            {
                TerminalStatusResponseSharedToken terminalStatusResponse = parseStatusResponse(deserializedTerminalStatusCheckerObject.CheckTerminalStatus());

                statusImageUrl = terminalStatusResponse.ImageUrl;
                statusDescription = terminalStatusResponse.StatusDescription;
            }

            if ( deserializedTerminalLastMessageTimestampCheckerObject != null )
            {
                TerminalLastMessageResponseSharedToken terminalLastMessageResponse = parseLastMessageTimestampResponse(deserializedTerminalLastMessageTimestampCheckerObject.CheckTerminalLastMessageTimestamp());

                lastMessageTimestamp = terminalLastMessageResponse.LastMessageTimestamp;
            }

            return string.Format(JSONCallbackToken,
                statusImageUrl,
                statusDescription,
                lastMessageTimestamp,
                deserializedDisplayMode);
        }

        /// <summary>
        /// Processes the callback event
        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaiseCallbackEvent( string eventArgument )
        {
            //eventArgument will be composed by the base64 serialization of both
            //TerminalStatusCheckerObject and TerminalLastMessageTimestampCheckerObject
            //separated by a '#'
            if ( !string.IsNullOrEmpty(eventArgument) )
            {
                string[] eventArgumentSplit = eventArgument.Split("#".ToCharArray());

                deserializedTerminalStatusCheckerObject = (TerminalStatusChecker)Deserialize(eventArgumentSplit[0]);
                deserializedTerminalLastMessageTimestampCheckerObject = (TerminalLastMessageTimestampChecker)Deserialize(eventArgumentSplit[1]);
                deserializedDisplayMode = eventArgumentSplit[2];
            }
        }
        #endregion

        #region serialization/deserialization
        /// <summary>
        /// Serializes an object into base-64 encoding
        /// </summary>
        /// <param name="toSerialize">Object to serialize</param>
        /// <returns>Object serialized in base-64 encoding</returns>
        private string Serialize( Object toSerialize )
        {
            string toRet = string.Empty;

            if ( toSerialize != null )
            {
                MemoryStream mStream = new MemoryStream(1024);

                try
                {
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    binFormatter.Serialize(mStream, toSerialize);
                }
                finally
                {
                    mStream.Position = 0;
                    toRet = Convert.ToBase64String(mStream.ToArray());
                    mStream.Close();
                }
            }

            return toRet;
        }

        /// <summary>
        /// Deserializes a base-64 serialized object
        /// </summary>
        /// <param name="serializedObject">The base-64 serialized object</param>
        /// <returns>The object deserialized</returns>
        private Object Deserialize( string serializedObject )
        {
            Object deserialized = null;

            if ( !string.IsNullOrEmpty(serializedObject) )
            {
                MemoryStream mStream = new MemoryStream();
                byte[] toWrite = Convert.FromBase64String(serializedObject);

                mStream.Write(toWrite, 0, toWrite.Length);
                mStream.Position = 0;

                try
                {
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    deserialized = binFormatter.Deserialize(mStream);
                }
                finally
                {
                    mStream.Close();
                }
            }

            return deserialized;
        }
        #endregion
    }

    /// <summary>
    /// Enumeration providing all the possible visualization modes of this control
    /// </summary>
    public enum DisplayModes
    {
        /// <summary>
        /// Very compact visualization: a single coloured dot for the status and a tooltip
        /// for the last message's timestamp
        /// </summary>
        Compact,

        /// <summary>
        /// Extended visualization: all the information are displayed inline
        /// </summary>
        Extended
    }

    /// <summary>
    /// Used internally to store the status description and the image url to be shown
    /// </summary>
    internal struct TerminalStatusResponseSharedToken
    {
        public string StatusDescription;
        public string ImageUrl;
    }

    /// <summary>
    /// Used internally to store the status description and the image url to be shown
    /// </summary>
    internal struct TerminalLastMessageResponseSharedToken
    {
        public string LastMessageTimestamp;
    }
}