using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIT.Libs.Base.CAB.Web.Controls.Popup.Design;
using SIT.Libs.Base.CAB.Web.Controls.Popup.Enumerations;
using SIT.Libs.Base.CAB.Web.Controls.Popup.Events;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls.Popup", "wc")]
namespace SIT.Libs.Base.CAB.Web.Controls.Popup
{
    /// <summary>
    /// A web popup dialog window
    /// </summary>
    /// <remarks>
    /// jQuery 1.3.2 is required (<a>http://jquery.com</a>)
    /// jQuery UI 1.7.2 is required (<a>http://jqueryui.com</a>)
    /// </remarks>
    [ParseChildren(true)]
    [ToolboxBitmap(typeof(System.Windows.Forms.Form))]
    [DefaultProperty("TitleText")]
    [DefaultEvent("ButtonClicked")]
    [ToolboxData("<{0}:Popup runat=server></{0}:Popup>")]
    [Description("A web popup dialog window")]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [Designer(typeof(PopupDesigner))]
    public class Popup : WebControl, INamingContainer
    {
        #region variables
        /// <summary>
        /// Occurs when a footer button (if any) has been clicked
        /// </summary>
        public event PopupButtonClickedEventHandler ButtonClicked;

        /// <summary>
        /// Occurs when this popup has been closed
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Occurs when the content of this popup is instantiated, thus accessible programmatically
        /// </summary>
        public event EventHandler ContentInstantiated;

        /// <summary>
        /// The list of buttons read from design time HTML
        /// </summary>
        private PopupButtonsCollection popupButtons = new PopupButtonsCollection();

        /// <summary>
        /// The content of this popup read from design time HTML
        /// </summary>
        private ITemplate popupContent;

        /// <summary>
        /// A panel that will contain the whole popup, used by jQuery to make a dialog of it
        /// </summary>
        private Panel popupContainer = new Panel();

        /// <summary>
        /// A button used to post back in case popup is closed using top right X icon
        /// </summary>
        private Button fakeCloseButton = new Button();
        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets popup position whenever it is made visible
        /// </summary>
        [Description("Gets or sets popup position whenever it is made visible")]
        [Category("Layout")]
        [DefaultValue(PopupPosition.Center)]
        public PopupPosition Position
        {
            get
            {
                if (ViewState["Position"] == null)
                    return PopupPosition.Center;
                else
                    return (PopupPosition)ViewState["Position"];
            }
            set
            {
                ViewState["Position"] = value;
            }
        }

        /// <summary>
        /// Popup's content
        /// </summary>
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [MergableProperty(false)]
        [TemplateContainer(typeof(Popup))]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate PopupContent
        {
            get
            {
                return popupContent;
            }
            set
            {
                popupContent = value;
            }
        }

        /// <summary>
        /// Popup's buttons collection
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Popup's buttons collection")]
        [Category("Layout")]
        public PopupButtonsCollection PopupButtons
        {
            get
            {
                return popupButtons;
            }
        }

        /// <summary>
        /// Popup's title
        /// </summary>
        [Description("Popup's title")]
        [Category("Layout")]
        [Localizable(true)]
        [DefaultValue("Title")]
        public string TitleText
        {
            get
            {
                if (ViewState["TitleText"] == null)
                    return "Title";
                return (string)ViewState["TitleText"];
            }
            set
            {
                ViewState["TitleText"] = value;
            }
        }

        /// <summary>
        /// If set to true, a gray layer will be rendered on page under the popup but above page's content
        /// </summary>
        [Category("Behavior")]
        [Description("If set to true, a gray layer will be rendered on page under the popup but above page's content")]
        [DefaultValue(true)]
        public bool Modal
        {
            get
            {
                if (ViewState["Modal"] == null)
                    return true;
                else
                    return (bool)ViewState["Modal"];
            }
            set
            {
                ViewState["Modal"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if this popup should be resizable
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets if this popup should be resizable")]
        [DefaultValue(true)]
        public bool Resizable
        {
            get
            {
                if (ViewState["Resizable"] == null)
                    return true;
                else
                    return (bool)ViewState["Resizable"];
            }
            set
            {
                ViewState["Resizable"] = value;
            }
        }

        /// <summary>
        /// Gets or sets if this popup should be draggable
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets if this popup should be draggable")]
        [DefaultValue(true)]
        public bool Draggable
        {
            get
            {
                if (ViewState["Draggable"] == null)
                    return true;
                else
                    return (bool)ViewState["Draggable"];
            }
            set
            {
                ViewState["Draggable"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of this popup control
        /// </summary>
        /// <remarks>If not set this property will hold 600 pixels</remarks>
        [DefaultValue(typeof(Unit), "600")]
        public override Unit Width
        {
            get
            {
                return (base.Width.IsEmpty ? Unit.Pixel(600) : base.Width);
            }
            set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of this popup control
        /// </summary>
        /// <remarks>If not set this property will hold 400 pixels</remarks>
        [DefaultValue(typeof(Unit), "400")]
        public override Unit Height
        {
            get
            {
                return (base.Height.IsEmpty ? Unit.Pixel(400) : base.Height);
            }
            set
            {
                base.Height = value;
            }
        }
          
        /// <summary>
        /// Gets or sets whether this control, although rendered because Visible, should
        /// be hidden (i.e. not displayed) in page
        /// </summary>
        /// <remarks>This property is useful when it is necessary to access this popup's content
        /// without showing it in page. In fact, if Visible holds false, the content of this
        /// popup, being a child in the controls hierarchy, would not be rendered as well as
        /// the popup itself</remarks>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or sets whether this control, although rendered because Visible, should be hidden (i.e. not displayed) in page")]
        public bool Hidden
        {
            get
            {
                if (ViewState["Hidden"] == null)
                    return false;
                else
                    return (bool)ViewState["Hidden"];
            }

            set
            {
                ViewState["Hidden"] = value;

                if (value && UseHiddenBehaviorWhenClosing)//raise Closed event if necessary
                    if (Closed != null) Closed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this popup control is rendered
        /// as UI on the page.
        /// </summary>
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }

            set
            {
                base.Visible = value;

                if(value) //raise Closed event
                    if (Closed != null) Closed(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Gets or set whether, when closing, this popup should set Visible = false or
        /// Hidden = true;
        /// </summary>
        /// <remarks>As per Hidden property, this one is useful when it is necessary to access 
        /// this popup's content without showing it in page. In fact, if Visible holds false,
        /// the content of this popup, being a child in the controls hierarchy, would not be 
        /// rendered as well as the popup itself</remarks>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Gets or set whether, when closing, this popup should set Visible = false or Hidden = true;")]
        public bool UseHiddenBehaviorWhenClosing
        {
            get
            {
                if (ViewState["UseHiddenBehaviorWhenClosing"] == null)
                    return false;
                else
                    return (bool)ViewState["UseHiddenBehaviorWhenClosing"];
            }

            set
            {
                ViewState["UseHiddenBehaviorWhenClosing"] = value;
            }
        }
        #endregion

        #region events
        /// <summary>
        /// Internal handler for popup's buttons click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Popup_ButtonClick(Object sender, EventArgs e)
        {
            PopupButton source = (PopupButton)sender;

            if (ButtonClicked != null)
                ButtonClicked(source, EventArgs.Empty);

            if (source.WillClosePopup)
                OnClosePopup();
        }

        /// <summary>
        /// Internal handler for popup's top left X icon clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fakeCloseButton_Click(Object sender, EventArgs e)
        {
            OnClosePopup();
        }

        /// <summary>
        /// Sets popup visibility to false and raises Closed event
        /// </summary>
        private void OnClosePopup()
        {
            if (UseHiddenBehaviorWhenClosing)
                Hidden = true;
            else
                Visible = false;

            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }
        #endregion

        #region overridden methods

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based
        /// implementation to create any child controls they contain in preparation for
        /// posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            popupContainer.ID = this.ID + "_container";

            if (popupContent != null)
            {
                PopupContent.InstantiateIn(popupContainer);

                if (ContentInstantiated != null)
                    ContentInstantiated(this, EventArgs.Empty);
            }

            for (int i = 0; i < PopupButtons.Count; i++)
            {
                PopupButton pbc = PopupButtons[i];

                if (string.IsNullOrEmpty(pbc.ID)) pbc.ID = "pbc" + i;
                pbc.Click += new EventHandler(Popup_ButtonClick);

                this.Controls.Add(pbc);
            }

            fakeCloseButton.ID = "fakeCloseButton";
            fakeCloseButton.Click += new EventHandler(fakeCloseButton_Click);

            this.Controls.Add(fakeCloseButton);
            this.Controls.Add(popupContainer);
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The System.Web.UI.HtmlTextWriter Object that receives the control content</param>
        protected override void Render(HtmlTextWriter writer)
        {
            string position = string.Empty;

            switch (Position)
            {
                case PopupPosition.BottomCenter:
                    {
                        position = "['center','bottom']";
                        break;
                    }
                case PopupPosition.BottomLeft:
                    {
                        position = "['left','bottom']";
                        break;
                    }
                case PopupPosition.BottomRight:
                    {
                        position = "['right','bottom']";
                        break;
                    }
                case PopupPosition.Center:
                    {
                        position = "'center'";
                        break;
                    }
                case PopupPosition.Left:
                    {
                        position = "'left'";
                        break;
                    }
                case PopupPosition.Right:
                    {
                        position = "'right'";
                        break;
                    }
                case PopupPosition.TopCenter:
                    {
                        position = "['center','top']";
                        break;
                    }
                case PopupPosition.TopLeft:
                    {
                        position = "['left','top']";
                        break;
                    }
                case PopupPosition.TopRight:
                    {
                        position = "['right','top']";
                        break;
                    }
                default:
                    {
                        position = "'center'";
                        break;
                    }
            }

            string script = @"try{
                $(document).ready(function(e)
                {
                    $('#" + this.popupContainer.ClientID + @"').dialog(
                    {
                        autoOpen: " + (!Hidden).ToString().ToLower() + @",
                        title: '" + TitleText + @"', 
                        width: " + Math.Round(Width.Value) + @",  
                        height: " + Math.Round(Height.Value) + @", 
                        show: 'fold',  
                        position: " + position + @", 
                        modal: " + Modal.ToString().ToLower() + @", 
                        resizable: " + Resizable.ToString().ToLower() + @", 
                        draggable: " + Draggable.ToString().ToLower() + @", 
                        closeOnEscape: false, 
                        close : function(type, data) { " + Page.ClientScript.GetPostBackEventReference(fakeCloseButton, string.Empty) + @"; },
                        open : function(type, data) { $(this).parent().appendTo('form');}";

            if (PopupButtons.Count > 0)
            {
                script += @",
                        buttons: 
                        {";

                for (int j = 0; j < PopupButtons.Count; j++)
                {
                    PopupButton button = (PopupButton)PopupButtons[j];
                    button.Style[HtmlTextWriterStyle.Display] = "none"; //to be sure

                    script += "'" + button.Text + "' : function(e) {" + Page.ClientScript.GetPostBackEventReference(button, string.Empty) + ";}";
                    script += (j == (PopupButtons.Count - 1) ? string.Empty : ",");
                }

                script += "}";
            }

            script += "});});}catch(ex){ alert('Unable to instanciate popup " + this.ID + ", have you tried setting InjectJQueryScript and InjectJQueryUIScript properties to true?'); }";

            fakeCloseButton.Style[HtmlTextWriterStyle.Display] = "none"; //to be sure

            popupContainer.RenderControl(writer);
            writer.WriteLine("<script type=\"text/javascript\">" + script + "</script>");

            Page.ClientScript.RegisterForEventValidation(fakeCloseButton.UniqueID);
        }
        #endregion
    }
}