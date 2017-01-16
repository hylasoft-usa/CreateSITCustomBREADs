using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.question.png", "image/gif")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    /// <summary>
    ///     An extended ASP.NET image button requiring a confirmation in order to post back page    /// 
    /// </summary>
    /// <remarks>
    /// jQuery is required (<a>http://jquery.com</a>)
    /// jQuery UI is required (<a>http://jqueryui.com</a>)
    /// </remarks>
    [ToolboxBitmap(typeof(Button))]
    [Description("An extended ASP.NET button requiring a confirmation in order to post back page")]
    public class ConfirmImageButton : ImageButton
    {
        private const string JQUERY_JS = "SIT.Libs.Base.CAB.Resources.jquery.min.js";
        private const string JQUERY_UI_JS = "SIT.Libs.Base.CAB.Resources.jquery-ui.min.js";

        /// <summary>
        ///     A div used to show messages using jQuery
        /// </summary>
        private Label MessageLabel;
        private Panel MessagePanel;
        private System.Web.UI.WebControls.Image ImageQuestion;
        private string HandlerToAttach;

        /// <summary>
        ///     Gets or sets whether the full JQuery library should be injected in page
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets whether the full JQuery library should be injected in page")]
        [Category("Behavior")]
        public bool InjectJQueryScript
        {
            get
            {
                if (this.ViewState["InjectJQueryScript"] == null)
                    return false;
                else
                    return (bool)this.ViewState["InjectJQueryScript"];
            }
            set
            {
                this.ViewState["InjectJQueryScript"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the full JQuery UI library should be injected in page
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or sets whether the full JQuery UI library should be injected in page")]
        public bool InjectJQueryUIScript
        {
            get
            {
                if (this.ViewState["InjectJQueryUIScript"] == null)
                    return false;
                else
                    return (bool)this.ViewState["InjectJQueryUIScript"];
            }
            set
            {
                this.ViewState["InjectJQueryUIScript"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether the confirmation message should be displayed or not; in the latter case this button will behave exactly as a normal button.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets whether the confirmation message should be displayed or not; in the latter case this button will behave exactly as a normal button")]
        [Category("Behavior")]
        public bool AskForConfirmation
        {
            get
            {
                if (this.ViewState["ConfirmButton_AskForConfirmation"] == null)
                    return true;
                else
                    return (bool)this.ViewState["ConfirmButton_AskForConfirmation"];
            }
            set
            {
                this.ViewState["ConfirmButton_AskForConfirmation"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the name of a parameterless client side function that must return a truth value to be used to decide whether the confirmation message should be displayed or not; in the latter case this button will behave exactly as a normal button. The specified client side function will be evaluated only if AskForConfirmation holds true
        /// </summary>
        [Description("Gets or sets the name of a parameterless client side function that must return a truth value to be used to decide whether the confirmation message should be displayed or not; in the latter case this button will behave exactly as a normal button. The specified client side function will be evaluated only if AskForConfirmation holds true")]
        [Category("Behavior")]
        [Browsable(true)]
        public string AskForConfirmationClientSideFunction
        {
            get
            {
                if (this.ViewState["ConfirmButton_AskForConfirmationClientSideFunction"] == null)
                    return string.Empty;
                else
                    return this.ViewState["ConfirmButton_AskForConfirmationClientSideFunction"].ToString();
            }
            set
            {
                this.ViewState["ConfirmButton_AskForConfirmationClientSideFunction"] = value;
            }
        }

        /// <summary>
        ///     The title of the popup
        /// </summary>
        [Localizable(true)]
        [Category("Appearance")]
        [Browsable(true)]
        [Description("The title of the popup")]
        public string PopupTitle
        {
            get
            {
                if (this.ViewState["ConfirmButton_PopupTitle"] == null)
                    return "Message";
                else
                    return this.ViewState["ConfirmButton_PopupTitle"].ToString();
            }
            set
            {
                this.ViewState["ConfirmButton_PopupTitle"] = value;
            }
        }

        /// <summary>
        ///     The message appearing to confirm page submit
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Localizable(true)]
        [Description("The message appearing to confirm page submit")]
        public string ConfirmationMessage
        {
            get
            {
                if (this.ViewState["ConfirmButton_ConfirmationMessage"] == null)
                    return "Confirm?";
                else
                    return this.ViewState["ConfirmButton_ConfirmationMessage"].ToString();
            }
            set
            {
                this.ViewState["ConfirmButton_ConfirmationMessage"] = value;
            }
        }

        /// <summary>
        ///     The text for the confirmation button
        /// </summary>
        [Description("The text for the confirmation button")]
        [Browsable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        public string ConfirmButtonText
        {
            get
            {
                if (this.ViewState["ConfirmationButtonText"] == null)
                    return "Ok";
                else
                    return this.ViewState["ConfirmationButtonText"].ToString();
            }
            set
            {
                this.ViewState["ConfirmationButtonText"] = value;
            }
        }

        /// <summary>
        ///     The text for the cancelling button
        /// </summary>
        [Description("The text for the cancelling button")]
        [Category("Appearance")]
        [Browsable(true)]
        [Localizable(true)]
        public string CancelButtonText
        {
            get
            {
                if (this.ViewState["CancelButtonText"] == null)
                    return "Cancel";
                else
                    return this.ViewState["CancelButtonText"].ToString();
            }
            set
            {
                this.ViewState["CancelButtonText"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the location of an image to be used as question mark icon within this ConfirmButton control in lieu of the default embedded image
        /// </summary>
        [Bindable(true)]
        [Description("The location of an image to be used as question mark icon within this ConfirmButton control")]
        [Browsable(true)]
        [Category("Appearance")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [UrlProperty]
        public string QuestionMarkIconURL
        {
            get
            {
                if (this.ViewState["ConfirmButton_QuestionMarkIconURL"] == null)
                    return string.Empty;
                else
                    return this.ViewState["ConfirmButton_QuestionMarkIconURL"].ToString();
            }
            set
            {
                this.ViewState["ConfirmButton_QuestionMarkIconURL"] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Cascading Style Sheet (CSS) class rendered by the confirmation message label on the client
        /// </summary>
        [Browsable(true)]
        [CssClassProperty]
        [Category("Appearance")]
        [DefaultValue("")]
        public virtual string ConfirmationMessageCssClass
        {
            get
            {
                if (this.ViewState["ConfirmButton_ConfirmationMessageCssClass"] == null)
                    return string.Empty;
                else
                    return this.ViewState["ConfirmButton_ConfirmationMessageCssClass"].ToString();
            }
            set
            {
                this.ViewState["ConfirmButton_ConfirmationMessageCssClass"] = value;
            }
        }

        /// <summary>
        ///     Override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            #region append to form a div to show messages using jQuery.UI

            Table MessageTable = new Table();
            MessageTable.Width = Unit.Percentage(100);
            MessageTable.Height = Unit.Percentage(100);

            TableCell MessageTableImageCell = new TableCell();
            MessageTableImageCell.Style["text-align"] = "center";
            MessageTableImageCell.Style["valign"] = "middle";

            ImageQuestion = new System.Web.UI.WebControls.Image();
            ImageQuestion.ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.question.png");
            ImageQuestion.ID = this.UniqueID + "_ImageInfo";

            MessageTableImageCell.Controls.Add(ImageQuestion);

            TableCell MessageTableMessageCell = new TableCell();
            MessageTableMessageCell.Width = Unit.Percentage(100);
            MessageTableMessageCell.Wrap = true;

            MessageLabel = new Label();
            MessageLabel.ID = this.UniqueID + "_MessageLabel";

            MessageTableMessageCell.Controls.Add(MessageLabel);

            TableRow MessageTableRow = new TableRow();
            MessageTableRow.Cells.Add(MessageTableImageCell);
            MessageTableRow.Cells.Add(MessageTableMessageCell);

            MessageTable.Rows.Add(MessageTableRow);

            MessagePanel = new Panel();
            MessagePanel.ID = this.UniqueID + "_MessagePanel";
            MessagePanel.Style[HtmlTextWriterStyle.Display] = "none";

            MessagePanel.Controls.Add(MessageTable);
            this.HandlerToAttach = "$('#" + this.MessagePanel.ClientID + "').dialog('open'); return false;";

            #endregion

            HandlerToAttach = "$('#" + MessagePanel.ClientID + @"').dialog('open'); return false;";
        }

        /// <summary>
        ///     Override
        /// </summary>
        /// <param name="writer"/>
        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                Type type = typeof(ConfirmButton);

                if (this.InjectJQueryScript)
                {
                    string webResourceUrl = this.Page.ClientScript.GetWebResourceUrl(type, "SIT.Libs.Base.CAB.Resources.jquery.min.js");

                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(type, "SIT.Libs.Base.CAB.Resources.jquery.min.js"))
                        this.Page.ClientScript.RegisterClientScriptInclude(type, "SIT.Libs.Base.CAB.Resources.jquery.min.js", webResourceUrl);
                }

                if (this.InjectJQueryUIScript)
                {
                    string webResourceUrl = this.Page.ClientScript.GetWebResourceUrl(type, "SIT.Libs.Base.CAB.Resources.jquery-ui.min.js");

                    if (!this.Page.ClientScript.IsClientScriptIncludeRegistered(type, "SIT.Libs.Base.CAB.Resources.jquery-ui.min.js"))
                        this.Page.ClientScript.RegisterClientScriptInclude(type, "SIT.Libs.Base.CAB.Resources.jquery-ui.min.js", webResourceUrl);
                }

                if (this.AskForConfirmation)
                    OnClientClick += (!string.IsNullOrEmpty(this.AskForConfirmationClientSideFunction) ? string.Format("if({0}()) {{ {1} }}", this.AskForConfirmationClientSideFunction, this.HandlerToAttach) : this.HandlerToAttach);

                MessageLabel.CssClass = this.ConfirmationMessageCssClass;
                MessageLabel.Text = this.ConfirmationMessage;

                string toInject = @"
                    try
                    {
                        $(document).ready(function(e)
                        {
                            $('#" + this.MessagePanel.ClientID + @"').dialog(
                            {
                                autoOpen: false,
                                title: '" + this.PopupTitle + @"',
                                width: 700,
                                show: 'fold',
                                modal: true,
                                resizable: true,
                                buttons:
                                {
                                    '" + this.ConfirmButtonText + @"' : function(e)
                                    {
                                        $(this).dialog('close');
                                        __doPostBack('" + this.UniqueID + @"', '');
                                    },
                                    '" + this.CancelButtonText + @"' : function(e)
                                    {
                                        $(this).dialog('close');
                                    }
                                }
                            });
                        });
                    }
                    catch(ex){ alert('Unable to instanciate confirm button " + this.ID + ", have you tried setting InjectJQueryScript and InjectJQueryUIScript properties to true?'); }";

                ScriptManager.RegisterStartupScript((Control)this, this.GetType(), this.UniqueID + "_InitConfirmPopup", toInject, true);
            }

            base.Render(writer);

            if (this.DesignMode)
                return;

            this.MessagePanel.RenderControl(writer);
        }
    }
}

