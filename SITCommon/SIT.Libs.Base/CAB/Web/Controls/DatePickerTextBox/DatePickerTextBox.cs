using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.Images.calendar.gif", "image/gif")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    /// <summary>
    /// A readonly textbox that, when clicked, will show a date picker allowing user
    /// to choose a date
    /// </summary>   
    /// <remarks>
    /// jQuery 1.3.2 is required (<a>http://jquery.com</a>)
    /// jQuery UI 1.7.2 is required (<a>http://jqueryui.com</a>)
    /// </remarks>
    [ToolboxBitmap(typeof(System.Web.UI.WebControls.TextBox))]
    [Description("A readonly textbox that, when clicked, will show a date picker allowing user to choose a date")]
    public class DatePickerTextBox : WebControl
    {
        #region variables
        private TextBox inputTextBox = new TextBox();
        #endregion

        #region events
        /// <summary>
        /// Inits control ID
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit(e);

            inputTextBox.ID = "inputTxt_" + this.ID;
            this.Controls.Add(inputTextBox);
        }

        /// <summary>
        /// Renders this control to the specified <see cref="T:System.Web.UI.HtmlTextWriter"/> Object.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> that receives the rendered output.</param>
        protected override void Render( HtmlTextWriter writer )
        {
            inputTextBox.RenderControl(writer);

            //building days names
            string[] shortestDaysNames = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortestDayNames;

            StringBuilder sbDays = new StringBuilder("[");
            for(int i = 0; i < shortestDaysNames.Length - 1; i++)
                sbDays.Append("'" + shortestDaysNames[i] + "',");
            sbDays.Append("'" + shortestDaysNames[shortestDaysNames.Length - 1] + "']");

            //building month names
            string[] abbreviatedMonthNames = Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;
            
            StringBuilder sbMonths = new StringBuilder("[");
            for(int i = 0; i < abbreviatedMonthNames.Length - 2; i++)
                sbMonths.Append("'" + abbreviatedMonthNames[i] + "',");
            sbMonths.Append("'" + abbreviatedMonthNames[abbreviatedMonthNames.Length - 2] + "']");

            //building yearsRange
            string yearsRange;

            if(ApplyYearsRangeAlsoToTheFuture)
                yearsRange = "'" + ( DateTime.Now.Year - YearsRange ) + ":" + ( DateTime.Now.Year + YearsRange ) + "'";
            else
                yearsRange = "'" + ( DateTime.Now.Year - YearsRange ) + ":" + DateTime.Now.Year + "'";

            //building buttonImage and showOn
            string buttonImage;
            string showOn;

            if(ShowCalendarIcon)
            {
                buttonImage = "'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "SIT.Libs.Base.CAB.Web.Controls.Images.calendar.gif") + "'";
                showOn = "'both'";
            }
            else
            {
                buttonImage = "''";
                showOn = "'focus'";
            }

            //jQuery management of datepicker
            string script = @"
                try
                {
                    $(document).ready(function(e)
                    {
                        $('#" + inputTextBox.ClientID + @"').datepicker(
                        {
                            dayNamesMin: " + sbDays.ToString() + @",
                            monthNamesShort: " + sbMonths.ToString() + @",
                            showOn: " + showOn + @",
                            buttonImage: " + buttonImage + @",
                            gotoCurrent: true,
                            changeMonth: true,
                            changeYear: true,
                            buttonImageOnly: true,
                            dateFormat: '" + Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.Replace("yy","y").Replace("M", "m") + @"',
                            nextText: '',
                            prevText: '',
                            buttonText: '',
                            yearRange: " + yearsRange + @"                            
                        });
                    });
                }
                catch(ex){ alert('Unable to instanciate date picker text box " + this.ID + ", have you tried setting InjectJQueryScript and InjectJQueryUIScript properties to true?'); }";

            ScriptManager.RegisterStartupScript(this, GetType(), this.UniqueID + "_InitDatePickerTextBox",
                script, true);

            //
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets selected date - or null for no selected date
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gets or sets selected date - or null for no selected date")]
        public DateTime? SelectedDate
        {
            get
            {
                if(string.IsNullOrEmpty(inputTextBox.Text))
                    return null;

                DateTime parsed;
                return DateTime.TryParse(inputTextBox.Text, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out parsed) ? 
                    (DateTime?)parsed : null;
            }

            set 
            {
                inputTextBox.Text = (value == null) ? 
                    string.Empty : ((DateTime)value).ToShortDateString();
            }
        }

        /// <summary>
        /// Gets or sets the range of years displayed in the year drop-down list
        /// </summary>
        /// <value>The years range.</value>
        /// <remarks>Default value = 5; can be used in conjunction with
        /// ApplyYearsRangeAlsoToTheFuture property</remarks>
        [Browsable(true)]
        [Description("Gets or sets the range of years displayed in the year drop-down list")]
        [DefaultValue(5)]
        [Category("Behavior")]
        public int YearsRange
        {
            get
            {
                if(ViewState["YearsRange"] == null)
                    return 5;
                else
                    return (int) ViewState["YearsRange"];
            }

            set
            {
                if(value < 0)
                    throw new ArgumentException("YearsRange cannot be negative");

                ViewState["YearsRange"] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether YearsRange should be applied only backward in time or also
        /// in the future
        /// </summary>
        /// <remarks>Default value = false (only past years)</remarks>
        [Browsable(true)]
        [Description("Gets or sets whether YearsRange should be applied only backward in time or also in the future")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ApplyYearsRangeAlsoToTheFuture
        {
            get
            {
                if(ViewState["ApplyYearsRangeAlsoToTheFuture"] == null)
                    return false;
                else
                    return (bool)ViewState["ApplyYearsRangeAlsoToTheFuture"];
            }

            set
            {
                ViewState["ApplyYearsRangeAlsoToTheFuture"] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether a calendar button should be displayed next to this control
        /// in order to trigger calendar appereance
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets whether a calendar button should be displayed next to this control in order to trigger calendar appereance")]
        [DefaultValue(true)]
        [Category("Layout")]
        public bool ShowCalendarIcon
        {
            get
            {
                if(ViewState["ShowCalendarIcon"] == null)
                    return true;
                else
                    return (bool)ViewState["ShowCalendarIcon"];
            }

            set
            {
                ViewState["ShowCalendarIcon"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the this control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Web.UI.WebControls.Unit"/> that represents the width of the control. The default is <see cref="F:System.Web.UI.WebControls.Unit.Empty"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The width of the Web server control was set to a negative value.
        /// </exception>
        public override Unit Width
        {
            get
            {
                return inputTextBox.Width;
            }
            set
            {
                inputTextBox.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of this control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Web.UI.WebControls.Unit"/> that represents the height of the control. The default is <see cref="F:System.Web.UI.WebControls.Unit.Empty"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The height was set to a negative value.
        /// </exception>
        public override Unit Height
        {
            get
            {
                return inputTextBox.Height;
            }
            set
            {
                inputTextBox.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab index of this control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The tab index of the Web server control. The default is 0, which indicates that this property is not set.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The specified tab index is not between -32768 and 32767.
        /// </exception>
        public override short TabIndex
        {
            get
            {
                return inputTextBox.TabIndex;
            }
            set
            {
                inputTextBox.TabIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is enabled.
        /// </summary>
        /// <value></value>
        /// <returns>true if control is enabled; otherwise, false. The default is true.
        /// </returns>
        public override bool Enabled
        {
            get
            {
                return inputTextBox.Enabled;
            }
            set
            {
                inputTextBox.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed when the mouse pointer hovers over this control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The text displayed when the mouse pointer hovers over the Web server control. The default is <see cref="F:System.string.Empty"/>.
        /// </returns>
        public override string ToolTip
        {
            get
            {
                return inputTextBox.ToolTip;
            }
            set
            {
                inputTextBox.ToolTip = value;
            }
        }
        #endregion
    }
}
