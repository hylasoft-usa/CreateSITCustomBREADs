using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls.Popup", "wc")]
namespace SIT.Libs.Base.CAB.Web.Controls.Popup
{
	/// <summary>
    /// Represents a SIT.Libs.Base.CAB.Web.Controls.Popup.Popup's button, which is rendered but
    /// not visible
	/// </summary>
	[PersistChildren(false)]
	[DefaultProperty(null)]
	[ToolboxData("<{0}:PopupButton runat=server></{0}:PopupButton>")]
    [ToolboxItem(false)]
    public class PopupButton : Button
    {
        /// <summary>
        /// Gets or sets whether this button should close the popup it belongs to
        /// when clicked
        /// </summary>
        public bool WillClosePopup
        {
            get
            {
                if (ViewState["WillClosePopup"] == null)
                    return true;
                else
                    return (bool)ViewState["WillClosePopup"];
            }
            set
            {
                ViewState["WillClosePopup"] = value;
            }
        }

        #region overridden properties hidden in designer and which behavior is fixed
        /// <summary>
        /// Returns always false
        /// </summary>
        [Browsable(false)]
        public override bool CausesValidation 
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }
              
        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string OnClientClick
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }        

        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string PostBackUrl
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }
                
        /// <summary>
        /// Return always false
        /// </summary>
        [Browsable(false)]
        public override bool UseSubmitBehavior
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }
        
        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string ValidationGroup
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }       
                
        /// <summary>
        /// Returns always true
        /// </summary>
        [Browsable(false)]
        public override bool Visible 
        {
            get
            {
                return true;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string AccessKey
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always Color.Empty
        /// </summary>
        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return Color.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always Color.Empty
        /// </summary>
        [Browsable(false)]
        public override Color BorderColor
        {
            get
            {
                return Color.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always BorderStyle.None
        /// </summary>
        [Browsable(false)]
        public override BorderStyle BorderStyle
        {
            get
            {
                return BorderStyle.None;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always Unit.Empty
        /// </summary>
        [Browsable(false)]
        public override Unit BorderWidth
        {
            get
            {
                return Unit.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string CssClass
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always true
        /// </summary>
        [Browsable(false)]
        public override bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always false
        /// </summary>
        [Browsable(false)]
        public override bool EnableTheming
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always Color.Empty
        /// </summary>
        [Browsable(false)]
        public override Color ForeColor
        {
            get
            {
                return Color.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always Unit.Empty
        /// </summary>
        [Browsable(false)]
        public override Unit Height
        {
            get
            {
                return Unit.Empty;
            }
            set
            {
                ;
            }
        }       

        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string SkinID
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always 0
        /// </summary>
        [Browsable(false)]
        public override short TabIndex
        {
            get
            {
                return 0;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public override string ToolTip
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always Unit.Empty
        /// </summary>
        [Browsable(false)]
        public override Unit Width
        {
            get
            {
                return Unit.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Returns always string.Empty
        /// </summary>
        [Browsable(false)]
        public new string CommandArgument
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// Not used by this SIT.Libs.Base.CAB.Web.Controls.Popup.Popup
        /// </summary>
        [Browsable(false)]
        public override FontInfo Font
        {
            get
            {
                return base.Font;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the SIT.Libs.Base.CAB.Web.Controls.Popup.Popup class
        /// </summary>
        public PopupButton() 
        {
            this.Style[HtmlTextWriterStyle.Display] = "none";
        }
    }
}
