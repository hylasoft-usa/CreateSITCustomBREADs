using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SITCAB.Framework;
using System.Web.UI.WebControls;
using SIT.Libs.Base.CAB.Web.Controls.Images;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Data.SqlClient;
using SITCAB.Common;
using System.Drawing;
using System.Web;
using SITCAB.Framework.Providers;
using SIT.Libs.Base.CAB.Utils;

namespace SIT.Libs.Base.CAB.Web.UI
{
    /// <summary>
    ///     This page extends SITCAB.Framework.CABContentPage providing some common functionalities useful for any CAB page.<br/>
    /// </summary>
    public abstract class CABBaseContentPage : CABContentPage
    {
        #region page variables

        /// <summary>
        ///     Event fired if the refresh icon has been pressed
        /// </summary>
        public event EventHandler Refresh;

        /// <summary>
        /// Used to keep session alive and avoid automatic redirection if selected
        /// </summary>
        private bool AutomaticRedirectSessionTimeoutBackup = false;

        /// <summary>
        /// A semaphore used to synchronize access to Globals.AutomaticRedirectSessionTimeout property between different CABBaseContentPage objects
        /// </summary>
        private static object semaphore = new object();

        /// <summary>
        /// A queue of messages to display
        /// </summary>
        private Queue<MessageStruct> messagesToDisplay = new Queue<MessageStruct>(5);

        // A div used to show messages using jQuery        
        private Label MessageLabel;
        private Panel MessagePanel;
        private CustomImage ImageInfo;
        private CustomImage ImageWarning;
        private CustomImage ImageError;

        /// <summary>
        ///     The header template
        /// </summary>
        private Panel CommonHeaderPanel;

        /// <summary>
        ///     A panel to show information about the system
        /// </summary>
        private Panel InfoMessagePanel;

        /// <summary>
        ///     A panel to show page contex help
        /// </summary>
        private Panel HelpMessagePanel;

        /// <summary>
        ///     The actual container of information about the system
        /// </summary>
        private Label InfoMessageLabel;

        /// <summary>
        ///     An icon to be clicked to refresh page
        /// </summary>
        private CustomImageButton IconImageRefresh;

        /// <summary>
        ///     An icon to be clicked to show information about the system
        /// </summary>
        private CustomImage IconImageInfo;

        /// <summary>
        ///     An icon to be clicked to show page contextual help
        /// </summary>
        private CustomImage IconImageHelp;

        private Collection<Control> controlsNotLockingApplicationOnSubmit = new Collection<Control>();

        #endregion

        #region page's events

        /// <summary>
        ///     <list type="bullet">
        ///         <item><description>Keeps session alive if needed</description></item>
        ///         <item><description>Creates UI-blocking divs</description></item>
        ///         <item><description>Creates common header</description></item>
        ///     </list>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            #region Pings

            if (IsKeepSessionaAlivePing)
            {
                if ((Request.QueryString["keepsessionalive"] != null) && (Request.QueryString["keepsessionalive"].ToLower() == "true"))
                    KeepSessionAliveSessionVariable = DateTime.Now;

                //End current response to avoid to keep on executing page's life cycle because in this case it is useless and time wasting
                Response.End();
            }

            if (IsCheckSqlServerAvalabilityPing)
            {
                #region Verify Sql Server availability

                SqlConnection availabilityConnection = new SqlConnection();
                availabilityConnection.ConnectionString = Globals.SitmesConnectionString;

                try
                {
                    availabilityConnection.Open();
                    Response.Write("OK");
                }
                catch { Response.Write("KO"); }
                finally { availabilityConnection.Close(); }

                #endregion

                //End current response to avoid to keep on executing page's life cycle because in this case it is useless and time wasting
                Response.End();
            }

            if (IsCheckWebServerAvalabilityPing)
            {
                //End current response to avoid to keep on executing page's life cycle because in this case it is useless and time wasting
                Response.End();
            }

            #endregion

            //override CAB Portal behavior regarding automatic redirection
            lock (semaphore)
            {
                if (!IsKeepSessionaAlivePing)
                {
                    AutomaticRedirectSessionTimeoutBackup = Globals.AutomaticRedirectSessionTimeout;

                    //set global automatic redirection to false
                    Globals.AutomaticRedirectSessionTimeout = false;
                }

                base.OnInit(e);

                //restore global automatic redirection
                if (!IsKeepSessionaAlivePing)
                    Globals.AutomaticRedirectSessionTimeout = AutomaticRedirectSessionTimeoutBackup;
            }

            #region append to the page a layer to show if LockApplicationWhenWebServerIsNotAvailable holds true

            //inner table            
            Table obscuringLayerInnerTableWebServer = new Table();
            obscuringLayerInnerTableWebServer.Width = Unit.Percentage(100);
            obscuringLayerInnerTableWebServer.Height = Unit.Percentage(100);

            TableRow obscuringLayerInnerTableRowWebServer = new TableRow();

            TableCell obscuringLayerInnerTableCellWebServer = new TableCell();
            obscuringLayerInnerTableCellWebServer.Style["text-align"] = "center";
            obscuringLayerInnerTableCellWebServer.Style["valign"] = "middle";

            Label noWebServerConnectivityMessageLabelWebServer = new Label();
            noWebServerConnectivityMessageLabelWebServer.Font.Size = FontUnit.Large;
            noWebServerConnectivityMessageLabelWebServer.ForeColor = Color.White;
            noWebServerConnectivityMessageLabelWebServer.BackColor = Color.Black;
            noWebServerConnectivityMessageLabelWebServer.BorderColor = Color.White;
            noWebServerConnectivityMessageLabelWebServer.BorderStyle = BorderStyle.Solid;
            noWebServerConnectivityMessageLabelWebServer.BorderWidth = Unit.Pixel(2);
            noWebServerConnectivityMessageLabelWebServer.Style["padding"] = "5px";
            noWebServerConnectivityMessageLabelWebServer.Text = WebServerNotAvailableMessage;

            obscuringLayerInnerTableCellWebServer.Controls.Add(noWebServerConnectivityMessageLabelWebServer);
            obscuringLayerInnerTableRowWebServer.Cells.Add(obscuringLayerInnerTableCellWebServer);
            obscuringLayerInnerTableWebServer.Rows.Add(obscuringLayerInnerTableRowWebServer);

            //final layer
            Panel obscuringLayerWebServer = new Panel();
            obscuringLayerWebServer.ID = "BasePageObscuringLayerWebServer";
            obscuringLayerWebServer.Width = Unit.Percentage(100);
            obscuringLayerWebServer.Height = Unit.Percentage(100);
            obscuringLayerWebServer.BackColor = Color.LightGray;
            obscuringLayerWebServer.Style["z-index"] = "999999";
            obscuringLayerWebServer.Style["position"] = "absolute";
            obscuringLayerWebServer.Style["top"] = "0px";
            obscuringLayerWebServer.Style["left"] = "0px";
            obscuringLayerWebServer.Style["display"] = "none";
            obscuringLayerWebServer.Style["filter"] = "alpha(opacity = 70)";

            obscuringLayerWebServer.Controls.Add(obscuringLayerInnerTableWebServer);
            this.Controls.Add(obscuringLayerWebServer);

            #endregion

            #region append to the page a layer to show if LockApplicationWhenSqlServerIsNotAvailable holds true

            //inner table            
            Table obscuringLayerInnerTableSqlServer = new Table();
            obscuringLayerInnerTableSqlServer.Width = Unit.Percentage(100);
            obscuringLayerInnerTableSqlServer.Height = Unit.Percentage(100);

            TableRow obscuringLayerInnerTableRowSqlServer = new TableRow();

            TableCell obscuringLayerInnerTableCellSqlServer = new TableCell();
            obscuringLayerInnerTableCellSqlServer.Style["text-align"] = "center";
            obscuringLayerInnerTableCellSqlServer.Style["valign"] = "middle";

            Label noSqlServerConnectivityMessageLabelSqlServer = new Label();
            noSqlServerConnectivityMessageLabelSqlServer.Font.Size = FontUnit.Large;
            noSqlServerConnectivityMessageLabelSqlServer.ForeColor = Color.White;
            noSqlServerConnectivityMessageLabelSqlServer.BackColor = Color.Black;
            noSqlServerConnectivityMessageLabelSqlServer.BorderColor = Color.White;
            noSqlServerConnectivityMessageLabelSqlServer.BorderStyle = BorderStyle.Solid;
            noSqlServerConnectivityMessageLabelSqlServer.BorderWidth = Unit.Pixel(2);
            noSqlServerConnectivityMessageLabelSqlServer.Style["padding"] = "5px";
            noSqlServerConnectivityMessageLabelSqlServer.Text = SqlServerNotAvailableMessage;

            obscuringLayerInnerTableCellSqlServer.Controls.Add(noSqlServerConnectivityMessageLabelSqlServer);
            obscuringLayerInnerTableRowSqlServer.Cells.Add(obscuringLayerInnerTableCellSqlServer);
            obscuringLayerInnerTableSqlServer.Rows.Add(obscuringLayerInnerTableRowSqlServer);

            //final layer
            Panel obscuringLayerSqlServer = new Panel();
            obscuringLayerSqlServer.ID = "BasePageObscuringLayerSqlServer";
            obscuringLayerSqlServer.Width = Unit.Percentage(100);
            obscuringLayerSqlServer.Height = Unit.Percentage(100);
            obscuringLayerSqlServer.BackColor = Color.LightGray;
            obscuringLayerSqlServer.Style["z-index"] = "999999";
            obscuringLayerSqlServer.Style["position"] = "absolute";
            obscuringLayerSqlServer.Style["top"] = "0px";
            obscuringLayerSqlServer.Style["left"] = "0px";
            obscuringLayerSqlServer.Style["display"] = "none";
            obscuringLayerSqlServer.Style["filter"] = "alpha(opacity = 70)";

            obscuringLayerSqlServer.Controls.Add(obscuringLayerInnerTableSqlServer);
            this.Controls.Add(obscuringLayerSqlServer);

            #endregion

            #region append to the page a layer to be shown after every submit

            Panel PanelWait = new Panel();
            PanelWait.ID = "PanelWait";
            PanelWait.Height = new Unit(50);
            PanelWait.Width = new Unit(250);
            PanelWait.BackColor = Color.Gray;
            PanelWait.Style["color"] = "#fff";
            PanelWait.Style["opacity"] = "0.7";
            PanelWait.Style["filter"] = "alpha(opacity=70)";
            PanelWait.Style["z-index"] = "5";
            PanelWait.Style["left"] = "45%";
            PanelWait.Style["position"] = "absolute";
            PanelWait.Style["top"] = "30%";

            CustomImage ImageOnSubmit = new CustomImage();

            Label LoadingLabel = new Label();
            LoadingLabel.Text = "Processing... Please wait.";
            LoadingLabel.Width = new Unit(188);
            LoadingLabel.Font.Size = FontUnit.Large;
            LoadingLabel.Height = new Unit(39);

            Panel LabelPanel = new Panel();
            LabelPanel.Style["text-align"] = "center";

            LabelPanel.Controls.Add(ImageOnSubmit);
            LabelPanel.Controls.Add(LoadingLabel);

            PanelWait.Controls.Add(LabelPanel);

            //final layer

            Panel obscuringLayerOnSubmit = new Panel();
            obscuringLayerOnSubmit.ID = "BasePageObscuringLayerOnSubmit";
            obscuringLayerOnSubmit.Width = Unit.Percentage(100);
            obscuringLayerOnSubmit.Height = Unit.Percentage(100);
            obscuringLayerOnSubmit.BackColor = Color.LightGray;
            obscuringLayerOnSubmit.Style["z-index"] = "99999";
            obscuringLayerOnSubmit.Style["position"] = "absolute";
            obscuringLayerOnSubmit.Style["top"] = "0px";
            obscuringLayerOnSubmit.Style["left"] = "0px";
            obscuringLayerOnSubmit.Style["display"] = "none";
            obscuringLayerOnSubmit.Style["filter"] = "alpha(opacity = 70)";

            obscuringLayerOnSubmit.Controls.Add(PanelWait);
            this.Controls.Add(obscuringLayerOnSubmit);

            #endregion

            #region append to form a div to show messages using jQuery.UI

            Table MessageTable = new Table();
            MessageTable.Width = Unit.Percentage(100);
            MessageTable.Height = Unit.Percentage(100);

            TableCell MessageTableImageCell = new TableCell();
            MessageTableImageCell.Style["text-align"] = "center";
            MessageTableImageCell.Style["valign"] = "middle";

            ImageInfo = new CustomImage(AvailableImages.Information);
            ImageInfo.ID = "ImageInfo";
            ImageWarning = new CustomImage(AvailableImages.Warning);
            ImageWarning.ID = "ImageWarning";
            ImageError = new CustomImage(AvailableImages.Error);
            ImageError.ID = "ImageError";

            MessageTableImageCell.Controls.Add(ImageInfo);
            MessageTableImageCell.Controls.Add(ImageWarning);
            MessageTableImageCell.Controls.Add(ImageError);

            TableCell MessageTableMessageCell = new TableCell();
            MessageTableMessageCell.Width = Unit.Percentage(100);
            MessageTableMessageCell.Wrap = true;

            MessageLabel = new Label();
            MessageLabel.ID = "MessageLabel";

            MessageTableMessageCell.Controls.Add(MessageLabel);

            TableRow MessageTableRow = new TableRow();
            MessageTableRow.Cells.Add(MessageTableImageCell);
            MessageTableRow.Cells.Add(MessageTableMessageCell);

            MessageTable.Rows.Add(MessageTableRow);

            MessagePanel = new Panel();
            MessagePanel.ID = "MessagePanel";

            MessagePanel.Controls.Add(MessageTable);

            UpdatePanel updatePanel = new UpdatePanel();
            updatePanel.ID = "CABBaseContentPageUpdatePanel";
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Always;
            updatePanel.ContentTemplateContainer.Controls.Add((Control)this.MessagePanel);

            this.Master.FindControl("PortalContent").Controls.AddAt(0, (Control)updatePanel);

            #endregion

            #region append to form a div to show info message using jQuery.UI

            Table InfoMessageTable = new Table();
            InfoMessageTable.Width = Unit.Percentage(100);
            InfoMessageTable.Height = Unit.Percentage(100);

            //TableCell InfoMessageTableImageCell = new TableCell();
            //InfoMessageTableImageCell.Style["text-align"] = "center";
            //InfoMessageTableImageCell.Style["valign"] = "middle";

            //CustomImage InfoMessageImageInfo = new CustomImage(AvailableImages.Information);
            //InfoMessageImageInfo.ID = "InfoMessageImageInfo";

            //InfoMessageTableImageCell.Controls.Add(InfoMessageImageInfo);

            TableCell InfoMessageTableMessageCell = new TableCell();
            InfoMessageTableMessageCell.Width = Unit.Percentage(100);
            InfoMessageTableMessageCell.Wrap = true;

            InfoMessageLabel = new Label();
            InfoMessageLabel.ID = "InfoMessageLabel";

            InfoMessageTableMessageCell.Controls.Add(InfoMessageLabel);

            TableRow InfoMessageTableRow = new TableRow();
            //InfoMessageTableRow.Cells.Add(InfoMessageTableImageCell);
            InfoMessageTableRow.Cells.Add(InfoMessageTableMessageCell);

            InfoMessageTable.Rows.Add(InfoMessageTableRow);

            InfoMessagePanel = new Panel();
            InfoMessagePanel.Style["display"] = "none";
            InfoMessagePanel.ID = "InfoMessagePanel";

            InfoMessagePanel.Controls.Add(InfoMessageTable);

            #endregion

            #region append to form a div to show help message using jQuery.UI

            Table HelpMessageTable = new Table();
            HelpMessageTable.Width = Unit.Percentage(100);
            HelpMessageTable.Height = Unit.Percentage(100);

            //TableCell HelpMessageTableImageCell = new TableCell();
            //HelpMessageTableImageCell.Style["text-align"] = "center";
            //HelpMessageTableImageCell.Style["valign"] = "middle";

            //CustomImage HelpMessageImageHelp = new CustomImage(AvailableImages.Information);
            //HelpMessageImageHelp.ID = "ImageHelp";

            //HelpMessageTableImageCell.Controls.Add(HelpMessageImageHelp);

            TableCell HelpMessageTableMessageCell = new TableCell();
            HelpMessageTableMessageCell.Width = Unit.Percentage(100);
            HelpMessageTableMessageCell.Wrap = true;

            Label HelpMessageLabel = new Label();
            HelpMessageLabel.ID = "HelpMessageLabel";
            HelpMessageLabel.Text = PageHelpContent;

            HelpMessageTableMessageCell.Controls.Add(HelpMessageLabel);

            TableRow HelpMessageTableRow = new TableRow();
            //HelpMessageTableRow.Cells.Add(HelpMessageTableImageCell);
            HelpMessageTableRow.Cells.Add(HelpMessageTableMessageCell);

            HelpMessageTable.Rows.Add(HelpMessageTableRow);

            HelpMessagePanel = new Panel();
            HelpMessagePanel.Style["display"] = "none";
            HelpMessagePanel.ID = "HelpMessagePanel";

            HelpMessagePanel.Controls.Add(HelpMessageTable);

            #endregion

            #region Create common header

            Unit padding = Unit.Pixel(6);

            Table CommonHeaderTable = new Table();
            CommonHeaderTable.Width = Unit.Percentage(100.0);
            CommonHeaderTable.Attributes["id"] = "breadcrumb";
            CommonHeaderTable.Style[HtmlTextWriterStyle.PaddingLeft] = CommonHeaderTable.Style[HtmlTextWriterStyle.PaddingRight] = "0px";

            TableRow CommonHeaderTableRow = new TableRow();

            TableCell CommonHeaderPageTitleCell = new TableCell();
            CommonHeaderPageTitleCell.Style[HtmlTextWriterStyle.PaddingRight] = padding.ToString();
            CommonHeaderPageTitleCell.Width = Unit.Percentage(100.0);
            CommonHeaderPageTitleCell.Controls.Add(new Label() { Text = PageTitle });

            TableCell CommonHeaderRefreshImageCell = new TableCell();
            CommonHeaderRefreshImageCell.Style[HtmlTextWriterStyle.PaddingRight] = padding.ToString();
            IconImageRefresh = new CustomImageButton(AvailableImages.IconRefresh);
            IconImageRefresh.Click += new ImageClickEventHandler(IconImageRefresh_Click);
            CommonHeaderRefreshImageCell.Controls.Add(IconImageRefresh);

            TableCell CommonHeaderInfoImageCell = new TableCell();
            CommonHeaderInfoImageCell.Style[HtmlTextWriterStyle.PaddingRight] = padding.ToString();
            IconImageInfo = new CustomImage(AvailableImages.IconInformation);
            CommonHeaderInfoImageCell.Controls.Add(IconImageInfo);

            TableCell CommonHeaderHelpImageCell = new TableCell();
            CommonHeaderHelpImageCell.Style[HtmlTextWriterStyle.PaddingRight] = padding.ToString();
            IconImageHelp = new CustomImage(AvailableImages.IconHelp);
            CommonHeaderHelpImageCell.Controls.Add(IconImageHelp);

            TableCell CommonHeaderSiemensLogoImageCell = new TableCell();

            CustomImage SiemensLogoImage = new CustomImage(AvailableImages.SiemensLogo);
            CommonHeaderSiemensLogoImageCell.Controls.Add(SiemensLogoImage);

            CommonHeaderTableRow.Cells.Add(CommonHeaderPageTitleCell);
            CommonHeaderTableRow.Cells.Add(CommonHeaderRefreshImageCell);
            CommonHeaderTableRow.Cells.Add(CommonHeaderInfoImageCell);
            CommonHeaderTableRow.Cells.Add(CommonHeaderHelpImageCell);
            CommonHeaderTableRow.Cells.Add(CommonHeaderSiemensLogoImageCell);

            CommonHeaderTable.Rows.Add(CommonHeaderTableRow);

            Label PageDescriptionLabel = new Label();
            PageDescriptionLabel.Text = PageDescription;

            CommonHeaderPanel = new Panel();
            CommonHeaderPanel.Style[HtmlTextWriterStyle.PaddingLeft] = this.CommonHeaderPanel.Style[HtmlTextWriterStyle.PaddingRight] = padding.ToString();
            CommonHeaderPanel.ID = "PageHeader";
            CommonHeaderPanel.Controls.Add(CommonHeaderTable);

            if (!string.IsNullOrEmpty(PageDescriptionLabel.Text))
            {
                CommonHeaderPanel.Controls.Add(new LiteralControl("<hr />"));
                CommonHeaderPanel.Controls.Add(PageDescriptionLabel);
                CommonHeaderPanel.Controls.Add(new LiteralControl("<br /><br />"));
            }
            else
                this.CommonHeaderPanel.Controls.Add((Control)new LiteralControl("<hr style='margin-bottom: 3px' />"));

            #endregion

            Master.FindControl("PortalContent").Controls.AddAt(0, InfoMessagePanel);
            Master.FindControl("PortalContent").Controls.AddAt(0, HelpMessagePanel);
            Master.FindControl("PortalContent").Controls.AddAt(0, CommonHeaderPanel);
        }

        /// <summary>
        ///     Injects some scripts in order to:
        ///     <list type="bullet">
        ///         <item><description>Block context menu on client side (i.e. right click) if necessary</description></item>
        ///         <item><description>Block keyboard combinations on client side if necessary</description></item>
        ///         <item><description>Keep the session alive for this page if necessary</description></item>
        ///         <item><description>Sets automatic refresh for this page if necesary</description></item>
        ///         <item><description>Show a panel blocking UI in case of web server unavailability</description></item>
        ///         <item><description>Show a panel blocking UI in case of sql server unavailability</description></item>
        ///         <item><description>Show a blocking panel during submits</description></item>
        ///     </list>
        ///     Moreover the message queue is processed here
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);

            if (!IsPartialPostback)
            {
                //script to allow programmatic refresh of page
                if (ShowHeaderTemplate)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "scripttoallowclientsiderefresh", ClientSideRefreshFunction, true);

                    // If the content to show in current page is empty, the help button is hidden
                    if (string.IsNullOrEmpty(PageHelpContent))
                        IconImageHelp.Style[HtmlTextWriterStyle.Display] = "none";
                }

                //script for blocking mouse right click
                if (BlockContextMenu)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "blockcontextmenu", BlockContextMenuScript, true);

                //script for blocking keyboard combinations
                if (BlockKeyboardCombinations)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "killkeycombinations", BlockKeyboardCombinationsScript, true);

                //script to keep session alive
                if (KeepSessionAlive)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "keepsessionalivescript", KeepSessionAliveScript, true);

                //script to schedule an automatic refresh
                if (AutomaticallyRefreshPage)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "automaticrefresh", AutomaticallyRefreshPageScript, true);

                //script to prevent user clicks when web server is not reachable
                if (LockApplicationWhenWebServerIsNotAvailable)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "lockwebapplicationifwebserverisdown", ManageObscuringLayerScriptWebServer, true);

                //script to prevent user clicks when web server is not reachable
                if (LockApplicationWhenSqlServerIsNotAvailable)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "lockwebapplicationifsqlserverisdown", ManageObscuringLayerScriptSqlServer, true);
                
                foreach (string s in ControlsIDNotLockingApplicationOnSubmit)
                {
                    Control c = FindControl(s, true);

                    if (c is Button)
                        ((Button)c).UseSubmitBehavior = false;

                    if (c != null)
                        controlsNotLockingApplicationOnSubmit.Add(c);
                }
            }
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideobscuringlayeronsubmit", HideOnSubmitCoveringLayerScript, true);

            if (LockApplicationOnSubmit)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideportalpartialpostackpanel", "try { document.getElementById('ctl01_UpdateProgress1').style.visibility = 'hidden'; } catch(ex){}", true);
                ScriptManager.RegisterOnSubmitStatement(this, this.GetType(), "onsubmitfunction", ShowOnSubmitCoveringLayerScript);
            }
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideportalpartialpostackpanel", "try { document.getElementById('ctl01_UpdateProgress1').style.visibility = 'visible'; } catch(ex){}", true);

            //enable mechanism to show popup to warn user that, in case of inactivity, a redirection will be performed
            if (!KeepSessionAlive)
                MetaTagsManager.SetAutomaticRedirectAfterSessionExpired(this);

            //attach auto select script on textboxes if necessary
            if (AutomaticallyAutoSelectTextBoxesContentOnFocus)
                ProcessControl(IsTextField, SetFocusOnTextField);

            CommonHeaderPanel.Visible = ShowHeaderTemplate;
            InitHeaderButtons();

            DisplayMessages();
        }

        #endregion

        #region private methods

        /// <summary>
        ///     Injects scripts in order to initialize the buttons in common header
        /// </summary>
        private void InitHeaderButtons()
        {
            string script = @"
                $(document).ready(function(e)
                {                    
                    $('#" + InfoMessagePanel.ClientID + @"').dialog(
                    {
                        title: 'Info', 
                        width: 800, 
                        show: 'fold', 
                        modal: false, 
                        resizable: true,
                        autoOpen: false,                        
                        buttons: 
                        {
                            'Ok' : function(e)
                            {
                                $(this).dialog('close');
                            }
                        }
                    });

                    $('#" + HelpMessagePanel.ClientID + @"').dialog(
                    {
                        title: 'Help', 
                        width: 800, 
                        show: 'fold', 
                        modal: false, 
                        resizable: true,
                        autoOpen: false,                        
                        buttons: 
                        {
                            'Ok' : function(e)
                            {
                                $(this).dialog('close');
                            }
                        }
                    });

                    $('#" + IconImageInfo.ClientID + @"').click(function(e){" + ShowInfoPopupScript + @"});
                    $('#" + IconImageHelp.ClientID + @"').click(function(e){" + ShowHelpPopupScript + @"});
                    $('#" + IconImageInfo.ClientID + @"').mouseover(function(e){document.body.style.cursor = 'hand';});
                    $('#" + IconImageHelp.ClientID + @"').mouseover(function(e){document.body.style.cursor = 'hand';});
                    $('#" + IconImageInfo.ClientID + @"').mouseleave(function(e){document.body.style.cursor = 'default';});
                    $('#" + IconImageHelp.ClientID + @"').mouseleave(function(e){document.body.style.cursor = 'default';});
                });";

            ScriptManager.RegisterStartupScript(this, GetType(), "InitInfoAndHelpPopup", script, true);

            InfoMessageLabel.Text = SystemInfo;
        }

        #endregion

        #region abstract/virtual methods/properties

        /// <summary>
        /// If ShowHeaderTemplate holds true, this help content will be shown whenever the help icon is pressed
        /// </summary>
        protected abstract string PageHelpContent
        {
            get;
        }

        /// <summary>
        /// If ShowHeaderTemplate holds true, these information will be shown whenever the system info icon is pressed
        /// </summary>
        protected abstract string SystemInfo
        {
            get;
        }

        #endregion

        #region getters/setters

        /// <summary>
        /// Gets whether a postback is a partial or complete one
        /// </summary>
        public bool IsPartialPostback
        {
            get
            {
                if (!IsPostBack) return false;

                string CustomHeader = HttpContext.Current.Request.Headers["x-microsoftajax"];
                return (CustomHeader != null) && (CustomHeader.ToLower() == "delta=true");
            }
        }

        #region Client side keyboard and mouse features

        /// <summary>
        ///     Sets whether page should block user from invoking context menu (by right clicking or using the keyboard)
        /// </summary>
        /// <remarks>Default value = false</remarks>
        public bool BlockContextMenu
        {
            get
            {
                if (ViewState["BlockContextMenu"] == null)
                    return false;
                else
                    return (bool)ViewState["BlockContextMenu"];
            }
            set { ViewState["BlockContextMenu"] = value; }
        }

        /// <summary>
        ///     The client-side script responsible to block context menu
        /// </summary>
        private string BlockContextMenuScript
        {
            get
            {
                return Environment.NewLine + Resources.BlockContextMenuScript + Environment.NewLine;
            }
        }

        /// <summary>
        ///     Sets whether page should ignore keybord combinations or special keys pressure.
        /// </summary>
        /// <remarks>Default value = false. This implementation blocks all ALT+key combinations, CTRL+key combinations and F1 to F12 keys except CTRL+(C,V,X,A)</remarks>
        public bool BlockKeyboardCombinations
        {
            get
            {
                if (ViewState["AvoidKeyboardCombinations"] == null)
                    return false;
                else
                    return (bool)ViewState["AvoidKeyboardCombinations"];
            }
            set { ViewState["AvoidKeyboardCombinations"] = value; }
        }

        /// <summary>
        ///     The client-side script responsible to block keyboard combinations
        /// </summary>
        private string BlockKeyboardCombinationsScript
        {
            get
            {

                return Environment.NewLine + Resources.KillKeyboardShortcutsScript + Environment.NewLine;
            }
        }

        #endregion

        #region Keep session alive

        /// <summary>
        ///     Should this page keep its session from expiring?
        /// </summary>
        /// <remarks>Default value = false; PingingTimeMilliSeconds is the pinging frequency (in milliseconds)</remarks>
        private bool KeepSessionAlive
        {
            get
            {
                if (ViewState["KeepSessionAlive"] == null)
                    return false;
                else
                    return (bool)ViewState["KeepSessionAlive"];
            }
            set { ViewState["KeepSessionAlive"] = value; }
        }

        /// <summary>
        /// How often, in milliseconds, should this page access the session in order to keep it alive?
        /// <remarks>Ignored if KeepSessionAlive holds false, default value = 60000 (60 seconds)</remarks>
        /// </summary>
        private long PingingTimeMilliSeconds
        {
            get
            {
                if (ViewState["PingingTime"] == null)
                    return 60000;
                else
                    return (long)ViewState["PingingTime"];
            }
            set
            {
                if (value > 0)
                    ViewState["PingingTime"] = value;
            }
        }

        /// <summary>
        ///     An ad-hoc session variable used, if necessary, to keep the session alive
        /// </summary>
        private DateTime KeepSessionAliveSessionVariable
        {
            get
            {
                if (HttpContext.Current.Session["KeepSessionAliveSessionVariable"] == null)
                    return DateTime.MinValue;
                else
                    return (DateTime)HttpContext.Current.Session["KeepSessionAliveSessionVariable"];
            }
            set
            {
                HttpContext.Current.Session["KeepSessionAliveSessionVariable"] = value;
            }
        }

        /// <summary>
        ///     The client-side script responsible to keep session alive
        /// </summary>
        private string KeepSessionAliveScript
        {
            get
            {
                return Environment.NewLine + Resources.KeepSessionAliveScript.Replace(
                    "PingTimePlaceHolder", PingingTimeMilliSeconds.ToString()).Replace(
                    "ResourceToRequestPlaceHolder", WebEnvironment.PageOrWebserviceURL) + Environment.NewLine;
            }
        }

        /// <summary>
        ///     Use this method to keep session alive for this page. This method can be called once, as it will persist across postbacks.
        /// </summary>
        /// <param name="PingingFrequencyMilliseconds">How often, in milliseconds, a request must be sent to the servr in order to keep session alive</param>
        public void KeepPageSessionAlive(long PingingFrequencyMilliseconds)
        {
            KeepSessionAlive = true;
            PingingTimeMilliSeconds = PingingFrequencyMilliseconds;
        }

        /// <summary>
        ///     Use this method to keep session alive for this page. This method can be called once, as it will persist across postbacks.
        /// </summary>
        /// <remarks>The pinging frequency is equal to Session timeout less one minute</remarks>
        public void KeepPageSessionAlive()
        {
            KeepPageSessionAlive((long)(Session.Timeout - 1) * 60 * 1000);
        }

        #endregion

        #region Automatic refresh

        /// <summary>
        ///     Should this page refresh automatically?
        /// </summary>
        /// <remarks>Default value = false; AutomaticallyRefreshPageTimeMilliSeconds sets after how much time automatic refresh should be prformed; no refresh will be performed if web server is not responding</remarks>
        private bool AutomaticallyRefreshPage
        {
            get
            {
                if (ViewState["AutomaticallyRefreshPage"] == null)
                    return false;
                else
                    return (bool)ViewState["AutomaticallyRefreshPage"];
            }
            set { ViewState["AutomaticallyRefreshPage"] = value; }
        }

        /// <summary>
        ///     How much time should pass from page load on client before automatically refreshing?
        /// <remarks>Ignored if AutomaticallyRefreshPage holds false, default value = 60000 (60 seconds)</remarks>
        /// </summary>
        private long AutomaticallyRefreshPageTimeMilliSeconds
        {
            get
            {
                if (ViewState["AutomaticallyRefreshPageTimeMilliSeconds"] == null)
                    return 60000;
                else
                    return (long)ViewState["AutomaticallyRefreshPageTimeMilliSeconds"];
            }
            set
            {
                if (value > 0)
                    ViewState["AutomaticallyRefreshPageTimeMilliSeconds"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether automatic refresh should be performed by submit or by URL reload
        /// </summary>
        private bool AutomaticallyRefreshPageBySubmit
        {
            get
            {
                if (ViewState["AutomaticallyRefreshPageBySubmit"] == null)
                    return false;
                else
                    return (bool)ViewState["AutomaticallyRefreshPageBySubmit"];
            }
            set
            {
                ViewState["AutomaticallyRefreshPageBySubmit"] = value;
            }
        }

        /// <summary>
        ///     The client-side script responsible to refresh page automatically after a given number of milliseconds
        /// </summary>
        private string AutomaticallyRefreshPageScript
        {
            get
            {
                return Environment.NewLine + Resources.SubmitPageAfterScript.Replace(
                    "ResourceToRequestPlaceHolder", WebEnvironment.PageOrWebserviceURL).Replace(
                    "RefreshTimeMillisecondsPlaceHolder", AutomaticallyRefreshPageTimeMilliSeconds.ToString()).Replace(
                    "RefreshBySubmitPlaceHolder", AutomaticallyRefreshPageBySubmit.ToString().ToLower()) + Environment.NewLine;
            }
        }

        /// <summary>
        ///     Use this method to set an automatic refresh of the page at a given frquency
        /// </summary>
        /// <param name="RefreshFrequencyMilliseconds">How often, in milliseconds, this page should refresh automatically</param>
        /// <param name="RefreshBySubmit">If true refresh will be performed by submit, otherwise by URL reloading. This will lead to having IsPostBack = true after refresh in the first case, false otherwise</param>
        public void AutomaticallyRefreshPageEvery(long RefreshFrequencyMilliseconds, bool RefreshBySubmit)
        {
            AutomaticallyRefreshPage = true;
            AutomaticallyRefreshPageBySubmit = RefreshBySubmit;
            AutomaticallyRefreshPageTimeMilliSeconds = RefreshFrequencyMilliseconds;
        }

        /// <summary>
        ///     Use this method to set an automatic refresh of the page at a given frquency
        /// </summary>
        /// <param name="RefreshFrequencyMilliseconds">How often, in milliseconds, this page should refresh automatically</param>
        /// <remarks>Refresh will be performed using URL reloading</remarks>
        public void AutomaticallyRefreshPageEvery(long RefreshFrequencyMilliseconds)
        {
            AutomaticallyRefreshPageEvery(RefreshFrequencyMilliseconds, false);
        }

        /// <summary>
        /// Use this method to set an automatic refresh of the page every 60 seconds
        /// </summary>
        /// <remarks>Refresh will be performed using URL reloading</remarks>
        public void AutomaticallyRefreshPageEvery()
        {
            AutomaticallyRefreshPageEvery(60000, false);
        }

        /// <summary>
        /// Gets wheteher this page is processing an ad-hoc request made to keep the session alive
        /// </summary>
        public bool IsKeepSessionaAlivePing
        {
            get
            {
                if (Request != null)
                {
                    return
                        (Request.QueryString["sid"] != null) &&
                        (Request.QueryString["keepsessionalive"] != null) &&
                        (Request.QueryString["keepsessionalive"].ToLower() == "true");
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets whether this page is processing an ad-hoc request made to check web server connection
        /// </summary>
        public bool IsCheckWebServerAvalabilityPing
        {
            get
            {
                return
                    (Request.QueryString["sid"] != null) &&
                    (Request.QueryString["checkingwebserveravailability"] != null) &&
                    (Request.QueryString["checkingwebserveravailability"].ToLower() == "true");
            }
        }

        /// <summary>
        /// Gets whether this page is processing an ad-hoc request made to check sql server connection
        /// </summary>
        public bool IsCheckSqlServerAvalabilityPing
        {
            get
            {
                return
                    (Request.QueryString["sid"] != null) &&
                    (Request.QueryString["checkingsqlserveravailability"] != null) &&
                    (Request.QueryString["checkingsqlserveravailability"].ToLower() == "true");
            }
        }
        #endregion

        #region Lock web application when web server or SQL Server are not reachable

        private string ManageObscuringLayerScriptWebServer
        {
            get
            {
                return Environment.NewLine + Resources.ManageObscuringLayerScriptWebServer.Replace(
                    "ResourceToRequestPlaceHolder", WebEnvironment.PageOrWebserviceURL) + Environment.NewLine;
            }
        }

        /// <summary>
        /// Gets or sets wheter web application should render and obscuring layer to prevent user clicks during a submit
        /// </summary>
        private bool LockApplicationOnSubmit
        {
            get
            {
                if (ViewState["LockApplicationOnSubmit"] == null)
                    return true;
                else
                    return (bool)ViewState["LockApplicationOnSubmit"];
            }

            set { ViewState["LockApplicationOnSubmit"] = value; }
        }

        /// <summary>
        /// Gets or sets whether web application should render and obscuring layer to prevent user clicks when web server is not reachable
        /// </summary>
        public bool LockApplicationWhenWebServerIsNotAvailable
        {
            get
            {
                if (ViewState["LockApplicationWhenWebServerIsNotAvailable"] == null)
                    return false;
                else
                    return (bool)ViewState["LockApplicationWhenWebServerIsNotAvailable"];
            }

            set { ViewState["LockApplicationWhenWebServerIsNotAvailable"] = value; }
        }

        /// <summary>
        ///     If LockApplicationOnSubmit holds true, this collection represents a set of controls' ID that, even if triggering a submit, will not lock the application. By means of this property an higher level of granularity can be set in order to better decide which controls will lock the application and which will not in case of submit.
        /// </summary>
        private Collection<string> ControlsIDNotLockingApplicationOnSubmit
        {
            get
            {
                if (ViewState["ControlsIDNotLockingApplicationOnSubmit"] == null)
                    return new Collection<string>();
                else
                    return (Collection<string>)ViewState["ControlsIDNotLockingApplicationOnSubmit"];
            }

            set { ViewState["ControlsIDNotLockingApplicationOnSubmit"] = value; }
        }

        /// <summary>
        ///     Sets wheter web application should render and obscuring layer to prevent user clicks during a submit. This method can be called once, as it will persist across postbacks.
        /// </summary>
        public void SetLockApplicationOnSubmit()
        {
            SetLockApplicationOnSubmit(new Collection<string>());
        }

        /// <summary>
        ///     Sets whether web application should render and obscuring layer to prevent user clicks during a submit. This method can be called once, as it will persist across postbacks.
        /// </summary>
        /// <param name="controlsIDToExclude">A collection of controls ID that will not lock the application on submit</param>
        public void SetLockApplicationOnSubmit(Collection<string> controlsIDToExclude)
        {
            LockApplicationOnSubmit = true;
            ControlsIDNotLockingApplicationOnSubmit = controlsIDToExclude;
        }

        /// <summary>
        ///     The script managing obscuring layer whenever sql server is not reachable
        /// </summary>
        /// <remarks>Used if LockApplicationWhenSqlServerIsNotAvailable holds true, ignored otherwise</remarks>
        private string ManageObscuringLayerScriptSqlServer
        {
            get
            {
                return Environment.NewLine + Resources.ManageObscuringLayerScriptSqlServer.Replace(
                    "ResourceToRequestPlaceHolder", WebEnvironment.PageOrWebserviceURL) + Environment.NewLine;
            }
        }

        /// <summary>
        ///     Gets or sets whether web application should render and obscuring layer to prevent user clicks when sql server is not reachable
        /// </summary>
        public bool LockApplicationWhenSqlServerIsNotAvailable
        {
            get
            {
                if (ViewState["LockApplicationWhenSqlServerIsNotAvailable"] == null)
                    return false;
                else
                    return (bool)ViewState["LockApplicationWhenSqlServerIsNotAvailable"];
            }

            set { ViewState["LockApplicationWhenSqlServerIsNotAvailable"] = value; }
        }

        /// <summary>
        /// The message shown whenever the sql server is not reachable. Override to have, for instance, a localized message.
        /// </summary>
        /// <remarks>Used if LockApplicationWhenSqlServerIsNotAvailable holds true, ignored otherwise</remarks>
        private string SqlServerNotAvailableMessage
        {
            get
            {
                return Localizations.NoSqlServerConnection;
            }
        }

        /// <summary>
        ///     The message shown whenever the web server is not reachable. Override to have, for instance, a localized message.
        /// </summary>
        /// <remarks>Used if LockApplicationWhenWebServerIsNotAvailable holds true, ignored otherwise</remarks>
        private string WebServerNotAvailableMessage
        {
            get
            {
                return Localizations.NoWebServerConnection;
            }
        }

        #endregion

        #region Other scripts

        /// <summary>
        ///     The client-side function allowing programmatic refresh
        /// </summary>
        private string ClientSideRefreshFunction
        {
            get
            {
                return string.Format(@"                    
                    function RefreshPage()
                    {{
                        document.getElementById('{0}').click();
                    }}",
                    IconImageRefresh.ClientID);
            }
        }

        private string ShowOnSubmitCoveringLayerScript
        {
            get
            {
                string toRet = @"
                    try 
                    { 
                        var sourceObj = event.srcElement;
                        var controlsToExclude = new Array(";

                foreach (Control c in controlsNotLockingApplicationOnSubmit)
                    toRet += "'" + c.ClientID + "',";

                if (controlsNotLockingApplicationOnSubmit.Count > 0)
                    toRet = toRet.Substring(0, toRet.Length - 1);

                toRet += @");
                        
                        var lockApplication = true;

                        for(var i=0; i<controlsToExclude.length; i++)
                        {
                            if(controlsToExclude[i] == sourceObj.id)
                            {
                                lockApplication = false;
                                break;
                            }
                        }

                        if(lockApplication)
                            document.getElementById('BasePageObscuringLayerOnSubmit').style.display = 'block'; 
                    } 
                    catch(ex) { }";

                return toRet;
            }
        }

        private string HideOnSubmitCoveringLayerScript
        {
            get
            {
                return "try { document.getElementById('BasePageObscuringLayerOnSubmit').style.display = 'none'; } catch(ex) { }";
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        ///     Gets or sets whether textboxes control should automatically select their content on focus
        /// </summary>
        public bool AutomaticallyAutoSelectTextBoxesContentOnFocus
        {
            get
            {
                if (ViewState["AutomaticallyAutoSelectTextBoxesContentOnFocus"] == null)
                    return false;
                else
                    return (bool)ViewState["AutomaticallyAutoSelectTextBoxesContentOnFocus"];
            }
            set
            {
                ViewState["AutomaticallyAutoSelectTextBoxesContentOnFocus"] = value;
            }
        }

        #endregion

        #region header properties

        /// <summary>
        ///     Gets or set whether the common header template should be rendered or not
        /// </summary>
        /// <remarks>The header template contains page title, page description, an image button providing some information about the system page is running on and an image button providing help for the page
        /// </remarks>
        public bool ShowHeaderTemplate
        {
            get
            {
                if (ViewState["ShowHeaderTemplate"] == null)
                    return true;
                else
                    return (bool)ViewState["ShowHeaderTemplate"];
            }

            set
            {
                ViewState["ShowHeaderTemplate"] = value;
            }
        }

        /// <summary>
        ///     Reads localized page title from site map
        /// </summary>
        private string PageTitle
        {
            get
            {
                string toRet = CABSiteMap.FindLocalizationTitle(Convert.ToInt32(CABSiteMap.CurrentNode.Key));

                if (string.IsNullOrEmpty(toRet))
                    toRet = CABSiteMap.CurrentNode.Title;

                return toRet;
            }
        }

        /// <summary>
        ///     Reads localized page title from site map
        /// </summary>
        private string PageDescription
        {
            get
            {
                string toRet = CABSiteMap.FindLocalizationDescription(Convert.ToInt32(CABSiteMap.CurrentNode.Key));

                if (string.IsNullOrEmpty(toRet))
                    toRet = CABSiteMap.CurrentNode.Description;

                return toRet;
            }
        }

        /// <summary>
        ///     The script to show info popup on client side
        /// </summary>
        private string ShowInfoPopupScript
        {
            get
            {
                return "document.getElementById('" + InfoMessagePanel.ClientID + "').style.display = ''; $('#" + InfoMessagePanel.ClientID + @"').dialog('open'); return;";
            }
        }

        /// <summary>
        ///     The script to show help popup on client side
        /// </summary>
        private string ShowHelpPopupScript
        {
            get
            {
                return "document.getElementById('" + HelpMessagePanel.ClientID + "').style.display = ''; $('#" + HelpMessagePanel.ClientID + @"').dialog('open'); return;";
            }
        }

        #endregion

        #endregion

        #region Message handling

        /// <summary>
        ///     Gets the title of the message window (shown by calling ShowMessage method)
        /// </summary>
        public virtual string MessageWindowTitle
        {
            get
            {
                return "Message";
            }
        }

        /// <summary>
        ///     Shows a simple popup to notify user
        /// </summary>
        /// <param name="Message">The message to show</param>
        public void ShowMessage(string Message)
        {
            ShowMessage(Message, MessageTypes.None);
        }

        /// <summary>
        ///     Shows a simple popup to notify user
        /// </summary>
        /// <param name="Message">The message to show</param>
        public void ShowMessage(object Message)
        {
            ShowMessage(Message, MessageTypes.None);
        }

        /// <summary>
        ///     Shows a simple popup to notify user
        /// </summary>
        /// <param name="Message">The message to show</param>
        /// <param name="MessageType">The type of the message to show</param>
        public void ShowMessage(object Message, MessageTypes MessageType)
        {
            ShowMessage(Message, MessageType, string.Empty);
        }

        /// <summary>
        ///     Shows a simple popup to notify user
        /// </summary>
        /// <param name="Message">The message to show</param>
        /// <param name="MessageType">The type of the message to show</param>
        public void ShowMessage(string Message, MessageTypes MessageType)
        {
            ShowMessage(Message, MessageType, string.Empty);
        }

        /// <summary>
        ///     Shows a simple popup to notify user and redirects to a given url after message has been read
        /// </summary>
        /// <param name="Message">The message to show</param>
        /// <param name="MessageType">The type of the message to show</param>
        /// <param name="RedirectUrl">The relative url to be redirected to when message has been read. Empty string means no redirection</param>
        /// <remarks>If more than one message has been queued, redirection will happen when the last message has been read. If more than one message is asking a redirection, the first one will be executed</remarks>
        public void ShowMessage(object Message, MessageTypes MessageType, string RedirectUrl)
        {
            if (Message == null)
                ShowMessage(null, MessageType, RedirectUrl);
            else
                ShowMessage(Message.ToString(), MessageType, RedirectUrl);
        }

        /// <summary>
        /// Shows a simple popup to notify user and redirects to a given url after message has been read
        /// </summary>
        /// <param name="Message">The message to show</param>
        /// <param name="MessageType">The type of the message to show</param>
        /// <param name="RedirectUrl">The relative url to be redirected to when message has been read. Empty string means no redirection</param>
        /// <remarks>If more than one message has been queued, redirection will happen when the last message has been read. If more than one message is asking a redirection, the first one will be executed</remarks>
        public void ShowMessage(string Message, MessageTypes MessageType, string RedirectUrl)
        {
            ShowMessage(Message, MessageType, RedirectUrl, true);
        }

        /// <summary>
        /// Shows a simple popup to notify user and redirects to a given url after message has been read
        /// </summary>
        /// <param name="Message">The message to show</param>
        /// <param name="MessageType">The type of the message to show</param>
        /// <param name="RedirectUrl">The relative url to be redirected to when message has been read. Empty string means no redirection</param>
        /// <param name="ReplaceCarriageReturnWithBrTag">If true carriage returns will be replaced with a br html tag, with a space char otherwise</param>
        /// <remarks>If more than one message has been queued, redirection will happen when the last message has been read. If more than one message is asking a redirection, the first one will be executed</remarks>
        public void ShowMessage(object Message, MessageTypes MessageType, string RedirectUrl, bool ReplaceCarriageReturnWithBrTag)
        {
            if (Message == null)
                ShowMessage(null, MessageType, RedirectUrl, ReplaceCarriageReturnWithBrTag);
            else
                ShowMessage(Message.ToString(), MessageType, RedirectUrl, ReplaceCarriageReturnWithBrTag);
        }

        /// <summary>
        /// Shows a simple popup to notify user and redirects to a given url after message has been read
        /// </summary>
        /// <param name="Message">The message to show</param>
        /// <param name="MessageType">The type of the message to show</param>
        /// <param name="RedirectUrl">The relative url to be redirected to when message has been read. Empty string means no redirection</param>
        /// <param name="ReplaceCarriageReturnWithBrTag">If true carriage returns will be replaced with a br html tag, with a space char otherwise</param>
        /// <remarks>If more than one message has been queued, redirection will happen when the last message has been read. If more than one message is asking a redirection, the first one will be executed</remarks>
        public void ShowMessage(string Message, MessageTypes MessageType, string RedirectUrl, bool ReplaceCarriageReturnWithBrTag)
        {
            if (Message == null)
                Message = "null";

            MessageStruct msg = new MessageStruct();
            msg.Text = ReplaceCarriageReturnWithBrTag ? Message.Replace(Environment.NewLine, "<br />") : Message.Replace(Environment.NewLine, " ");
            msg.Type = MessageType;
            msg.RedirectUrl = Server.HtmlEncode(RedirectUrl);

            messagesToDisplay.Enqueue(msg);
        }

        /// <summary>
        ///     Called on pre render complete in order to show messages requested by ShowMessage methods
        /// </summary>
        private void DisplayMessages()
        {
            if (messagesToDisplay.Count > 0)
            {
                MessageStruct firstMessage = messagesToDisplay.Dequeue();

                MessagePanel.Visible = true;
                MessageLabel.Text = (firstMessage.Text == null) ? "null" : firstMessage.Text;

                string script = @"
                $(document).ready(function(e)
                {
                    " + (firstMessage.Type == MessageTypes.Information ? "$('#" + ImageInfo.ClientID + "').show();" : "$('#" + ImageInfo.ClientID + "').hide();") + @" 
                    " + (firstMessage.Type == MessageTypes.Warning ? "$('#" + ImageWarning.ClientID + "').show();" : "$('#" + ImageWarning.ClientID + "').hide();") + @" 
                    " + (firstMessage.Type == MessageTypes.Error ? "$('#" + ImageError.ClientID + "').show();" : "$('#" + ImageError.ClientID + "').hide();") + @" 
                    
                    $('#" + MessagePanel.ClientID + @"').dialog(
                    {
                        title: '" + MessageWindowTitle.Replace("'", " ") + @"', 
                        width: 900, 
                        show: 'fold', 
                        modal: true, 
                        resizable: true, 
                        autoOpen: true, 
                        beforeclose: function(event)
                        {
                            var nextMessageIndex = -1;

                            for(var i=0; i<messagesQueue.messages.length; i++)
                            {
                                if(!messagesQueue.messages[i].shown)
                                {
                                    nextMessageIndex = i;
                                    messagesQueue.messages[i].shown = true;
                                    break;
                                }
                            }

                            if(nextMessageIndex != -1)
                            {
                                $('#" + MessageLabel.ClientID + @"').html(messagesQueue.messages[nextMessageIndex].text);
                                if (messagesQueue.messages[nextMessageIndex].type == 'information') $('#" + ImageInfo.ClientID + @"').show(); else $('#" + ImageInfo.ClientID + @"').hide();
                                if (messagesQueue.messages[nextMessageIndex].type == 'error') $('#" + ImageError.ClientID + @"').show(); else $('#" + ImageError.ClientID + @"').hide();
                                if (messagesQueue.messages[nextMessageIndex].type == 'warning') $('#" + ImageWarning.ClientID + @"').show(); else $('#" + ImageWarning.ClientID + @"').hide();
                                $(this).dialog('option', 'position', 'center');

                                return false;
                            }
                            else
                            {
                                var firstRedirectUrlFound = '" + firstMessage.RedirectUrl + @"';

                                for(var i=0; i<messagesQueue.messages.length; i++)
                                {
                                    if(messagesQueue.messages[i].redirecturl != '')
                                    {
                                        firstRedirectUrlFound = messagesQueue.messages[i].redirecturl;
                                        break;
                                    }
                                }  

                                if(firstRedirectUrlFound != '')
                                {
                                    document.location.href = firstRedirectUrlFound;
                                    " + ShowOnSubmitCoveringLayerScript + @"
                                }

                                return true;
                            }
                        },
                        buttons: 
                        {
                            'Ok' : function(e)
                            {
                                $(this).dialog('close');
                            }
                        }
                    });
                });";

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessage", script, true);

                //other messages in queue
                StringBuilder otherMessagesScript = new StringBuilder(@"
                    var messagesQueue =
                    {
                        messages:
                        [ ");

                while (messagesToDisplay.Count > 0)
                {
                    MessageStruct curMessage = messagesToDisplay.Dequeue();
                    string curMessageText = (curMessage.Text == null) ? "null" : curMessage.Text;

                    otherMessagesScript.Append(@"
                            {
                                text: '" + curMessageText.Replace("'", "\"").Replace("\\", "\\\\") + @"',
                                type: '" + curMessage.Type.ToString().ToLower() + @"',
                                redirecturl: '" + curMessage.RedirectUrl + @"',
                                shown: false
                            },");
                }

                //cut last comma
                otherMessagesScript.Remove(otherMessagesScript.Length - 1, 1);

                otherMessagesScript.Append(@"
                        ]
                    };");

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessageOtherMessagesStruct",
                    otherMessagesScript.ToString(), true);
            }
            else
                MessagePanel.Visible = false;
        }
        #endregion

        #region Utilities methods

        /// <summary>
        ///     Performs some actions over controls in a recursive fashion
        /// </summary>
        /// <param name="checkingMethod">A method that estabilishes if a control is to be modified</param>
        /// <param name="processingMethod">A method actually modifying the control</param>
        protected void ProcessControl(CheckControl checkingMethod, ControlProcessor processingMethod)
        {
            RecursiveProcessControl(this, checkingMethod, processingMethod);
        }

        /// <summary>
        ///     Used by ProcessControl
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="checkingMethod"></param>
        /// <param name="processingMethod"></param>
        private void RecursiveProcessControl(Control ctrl, CheckControl checkingMethod, ControlProcessor processingMethod)
        {
            if (checkingMethod(ctrl))
            {
                processingMethod(ctrl);
                return;
            }

            foreach (Control c in ctrl.Controls)
                RecursiveProcessControl(c, checkingMethod, processingMethod);
        }

        /// <summary>
        ///     Used to set focus on text fields automatically
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private bool IsTextField(Control ctrl)
        {
            return ((ctrl is ITextControl) && (ctrl is WebControl));
        }

        /// <summary>
        ///     Used to set focus on text fields automatically
        /// </summary>
        /// <param name="ctrl"></param>
        private void SetFocusOnTextField(Control ctrl)
        {
            WebControl c = (WebControl)ctrl;
            c.Attributes["onfocus"] += "try { this.select(); } catch(ex) { }";
        }

        /// <summary>
        ///     Used by FindControl(Control id, bool recursiveSearch)
        /// </summary>
        /// <param name="id">Control id to find</param>
        /// <param name="parent">Root node to start searching from </param>
        /// <returns>The specified control, or null if the specified control does not exist</returns>
        private Control RecursiveFindControl(string id, Control parent)
        {
            Control c = parent.FindControl(id);

            if (c == null)
            {
                foreach (Control cc in parent.Controls)
                {
                    c = RecursiveFindControl(id, cc);
                    if (c != null) return c; else continue;
                }
            }
            else
                return c;

            return null;
        }

        /// <summary>
        /// A recursive version of FindControl method, which digs into the whole control hierarchy to find a specified control
        /// </summary>
        /// <param name="id">Control id to find</param>
        /// <param name="recursiveSearch">If false the usual FindControl will be performed, if true the recursive version</param>
        /// <returns>The specified control, or null if the specified control does not exist</returns>
        public Control FindControl(string id, bool recursiveSearch)
        {
            if (recursiveSearch)
                return RecursiveFindControl(id, this);
            else
                return base.FindControl(id);
        }

        #endregion

        #region event handlers

        #region refresh management

        void IconImageRefresh_Click(object sender, ImageClickEventArgs e)
        {
            OnRefresh(EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the Refresh event
        /// </summary>
        /// <param name="e">The System.EventArgs object that contains the event data</param>
        protected virtual void OnRefresh(EventArgs e)
        {
            if (Refresh != null)
                Refresh(this, e);
        }

        #endregion

        #endregion
    }

    /// <summary>
    ///     The list of all possible message types managed by CABBaseContentPage
    /// </summary>
    public enum MessageTypes
    {
        /// <summary>
        /// Informational message
        /// </summary>
        Information,

        /// <summary>
        /// Warning message
        /// </summary>
        Warning,

        /// <summary>
        /// Error message
        /// </summary>
        Error,

        /// <summary>
        /// Unspecified
        /// </summary>
        None
    }

    /// <summary>
    ///     A private struct used by CABBaseContentPage to show a queue of messages
    /// </summary>
    struct MessageStruct
    {
        public string Text;
        public MessageTypes Type;
        public string RedirectUrl;
    }

    /// <summary>
    ///     A delegate to a function performing a check against a Control; used by ProcessControl method to actually check if a control has to be processed
    /// </summary>
    /// <param name="controlToCheck">The control to check</param>
    /// <returns>True if check succeeds, false otherwise</returns>
    public delegate bool CheckControl(Control controlToCheck);

    /// <summary>
    ///     A delegate to a function which modifies a Control; used by ProcessControl method to actually perform control processing
    /// </summary>
    /// <param name="controlToModify">The control to modify</param>
    public delegate void ControlProcessor(Control controlToModify);
}