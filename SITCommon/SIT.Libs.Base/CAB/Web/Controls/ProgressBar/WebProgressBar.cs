using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SIT.Libs.Base.CAB.Web.Controls.ProgressBar.Design;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls.ProgressBar", "wc")]
namespace SIT.Libs.Base.CAB.Web.Controls.ProgressBar
{
    /// <summary>
    /// A web control showing a progress bar that fills up in a given range of time
    /// and then, if set, restarts its progression
    /// </summary>
    /// <remarks>
    /// jQuery 1.3.2 is required (<a>http://jquery.com</a>)
    /// jQuery UI 1.7.2 is required (<a>http://jqueryui.com</a>)
    /// </remarks>
    [ToolboxBitmap(typeof(System.Windows.Forms.ProgressBar))]
    [DefaultProperty("IntervalDuration")]
    [ToolboxData("<{0}:WebProgressBar runat=server></{0}:WebProgressBar>")]
    [Description("A progress bar that fills up in a given range of time and then restarts its progression")]
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [Designer(typeof(WebProgressBarDesigner))]
    public class WebProgressBar : WebControl
    {
        #region variables
        /// <summary>
        /// A panel that will contain the whole progress bar, used by jQuery to make a progress bar of it
        /// </summary>
        private Panel progressBarContainer = new Panel();

        /// <summary>
        /// A table that will contain progressBarContainer, used to be able to make use of Width and Height if set in %
        /// </summary>
        private Table progressBarContainerTable = new Table();
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the how often (in milliseconds) this progressbar's progress must be updated
        /// </summary>
        /// <remarks>The lower this value is the higher CPU load on client machines will be</remarks>
        [Category("Behavior")]
        [Description("Gets or sets the how often this progressbar's progress must be updated")]
        [DefaultValue(1000)]
        public int UpdateInterval
        {
            get
            {
                if (ViewState["UpdateInterval"] == null)
                    return 1000;
                else
                    return (int)ViewState["UpdateInterval"];
            }
            set
            {
                if (value > 0)
                    ViewState["UpdateInterval"] = value;
                else
                    throw new ArgumentException("The time interval to update a SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar's progress must be positive (passed in value = " + value + ")");
            }
        }

        /// <summary>
        /// Gets or sets whether this progress bar should restart when completely filled
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets whether this progress bar should restart when completely filled")]
        [DefaultValue(false)]
        public bool RestartWhenComplete
        {
            get
            {
                if (ViewState["RestartWhenComplete"] == null)
                    return false;
                else
                    return (bool)ViewState["RestartWhenComplete"];
            }
            set
            {
                ViewState["RestartWhenComplete"] = value;
            }
        }

        /// <summary>
        /// The amount of time in milliseconds this SIT.Libs.Base.CAB.Web.Controls.CountDownProgressBar
        /// takes in order to completely fill up
        /// </summary>
        /// <remarks>Default value is 60 seconds</remarks>
        [Description("The amount of time, in seconds, this SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar takes in order to be completely filled up")]
        [Category("Data")]
        [Browsable(true)]
        [DefaultValue(60000)]
        public int IntervalDuration
        {
            get
            {
                if (ViewState["IntervalDuration"] == null)
                    return 60000;
                else
                    return (int)ViewState["IntervalDuration"];
            }
            set
            {
                if (value > 0)
                    ViewState["IntervalDuration"] = value;
                else
                    throw new ArgumentException("The amount of time for filling up a SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar must be positive (passed in value = " + value + ")");
            }
        }

        /// <summary>
        /// Width of this SIT.Libs.Base.CAB.Web.Controls.CountDownProgressBar
        /// </summary>
        /// <remarks>Default value is 100%</remarks>
        [Description("Width of this SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar")]
        [Category("Layout")]
        [Browsable(true)]
        public new Unit Width
        {
            get
            {
                if ((base.Width == Unit.Empty) || (base.Width == null) || (base.Width.Value == 0))
                    return Unit.Percentage(100.0);
                else
                    return base.Width;
            }
            set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Height of this SIT.Libs.Base.CAB.Web.Controls.CountDownProgressBar 
        /// </summary>
        /// <remarks>Default value is 20px</remarks>
        [Description("Height of this SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar")]
        [Category("Layout")]
        [Browsable(true)]
        public new Unit Height
        {
            get
            {
                if ((base.Height == Unit.Empty) || (base.Height == null) || (base.Height.Value == 0))
                    return Unit.Pixel(20);
                else
                    return base.Height;
            }
            set
            {
                base.Height = value;
            }
        }

        /// <summary>
        /// An optional client side (javascript, vbscript) function name that will be called
        /// whenever a new tick inside this control's time range has elapsed.
        /// Two integer parameters will be automatically passed in to the specified function,
        /// i.e. how much time has elapsed from the beginning of this control's cycle and 
        /// how musch time it is remaining before it is fully painted
        /// </summary>
        [Description("An optional client side (javascript, vbscript) function name that will be called whenever a new tick inside this control's time range has elapsed. Two integer parameters will be automatically passed in to the specified function, i.e. how much time has elapsed from the beginning of this control's cycle and how much time it is remaining before it is fully painted")]
        [Category("Misc")]
        [Browsable(true)]
        public string OnUpdateEventHandlerFunction
        {
            get
            {
                if (ViewState["OnUpdateEventHandlerFunction"] == null)
                    return string.Empty;
                else
                    return ViewState["OnUpdateEventHandlerFunction"].ToString();
            }
            set
            {
                ViewState["OnUpdateEventHandlerFunction"] = value;
            }
        }

        /// <summary>
        /// An optional client side (javascript, vbscript) function name that will be called
        /// whenever this control's time range has fully elapsed.
        /// An integer parameters will be automatically passed in to the specified function,
        /// i.e. the total amount of time elapsed from the beginning of this control's cycle
        /// </summary>
        [Description("An optional client side (javascript, vbscript) function name that will be called whenever this control's time range has fully elapsed. An integer parameters will be automatically passed in to the specified function, i.e. the total amount of time elapsed from the beginning of this control's cycle")]
        [Category("Misc")]
        [Browsable(true)]
        public string OnFinishEventHandlerFunction
        {
            get
            {
                if (ViewState["OnFinishEventHandlerFunction"] == null)
                    return string.Empty;
                else
                    return ViewState["OnFinishEventHandlerFunction"].ToString();
            }
            set
            {
                ViewState["OnFinishEventHandlerFunction"] = value;
            }
        }
        #endregion

        #region overridden methods
        /// <summary>
        /// Raises the System.Web.UI.Control.Init event.
        /// </summary>
        /// <param name="e">An System.EventArgs Object that contains the event data</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            progressBarContainer.ID = this.ID + "_container";
            progressBarContainerTable.ID = this.ID + "_containerTable";
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The System.Web.UI.HtmlTextWriter Object that receives the control content</param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            progressBarContainerTable.Width = Width;
            progressBarContainerTable.Height = Height;

            TableRow tr = new TableRow();
            tr.Width = Unit.Percentage(100);

            TableCell tc = new TableCell();
            tc.Width = Unit.Percentage(100);

            progressBarContainer.Width = Unit.Percentage(100);
            progressBarContainer.Height = Unit.Percentage(100);

            tc.Controls.Add(progressBarContainer);
            tr.Cells.Add(tc);
            progressBarContainerTable.Rows.Add(tr);

            progressBarContainerTable.RenderControl(writer);

            string script = @"
                try
                {
                    $(document).ready(function(e)
                    {
                        $('#" + this.progressBarContainer.ClientID + @"').progressbar(
                        {
                            value: 0
                        });
                    });
                }
                catch(ex){ alert('Unable to instanciate progress bar " + this.ID + @", have you tried setting InjectJQueryScript and InjectJQueryUIScript properties to true?'); }
            
                function update_" + UniqueIdentifierForJavascriptReferences + @"()
                {
                    try
                    {
                        val_" + UniqueIdentifierForJavascriptReferences + @" += " + UpdateInterval + @";

                        var percentageVal = (val_" + UniqueIdentifierForJavascriptReferences + @" * 100) / " + IntervalDuration + @";
                        $('#" + this.progressBarContainer.ClientID + @"').progressbar('option', 'value', percentageVal);
                        
                        if(val_" + UniqueIdentifierForJavascriptReferences + @" >= " + IntervalDuration + @")
                        {
                            val_" + UniqueIdentifierForJavascriptReferences + @" = 0;";

            if(!RestartWhenComplete)
                script += @"
                            clearInterval(timer_" + UniqueIdentifierForJavascriptReferences + @");";

            script += @"
                        }";                        
                        
            if (!string.IsNullOrEmpty(OnFinishEventHandlerFunction))
                script += @"
                        if(val_" + UniqueIdentifierForJavascriptReferences + @" == 0)
                            try { eval(" + OnFinishEventHandlerFunction + "()); } catch(exeval1) { }";
            
            if (!string.IsNullOrEmpty(OnUpdateEventHandlerFunction))
                script += @"
                        if(val_" + UniqueIdentifierForJavascriptReferences + @" != 0)
                            try { eval(" + OnUpdateEventHandlerFunction + "(percentageVal)); } catch(exeval2) { }";

            script += @"
                    }
                    catch(ex){ alert('Unable to instanciate progress bar " + this.ID + @", have you tried setting InjectJQueryScript and InjectJQueryUIScript properties to true?'); }
                }

                var timer_" + UniqueIdentifierForJavascriptReferences + " = window.setInterval('update_" + UniqueIdentifierForJavascriptReferences + @"()', " + UpdateInterval + @");
                var val_" + UniqueIdentifierForJavascriptReferences + @" = 0;";

            writer.WriteLine("<script type=\"text/javascript\">" + script + "</script>");
        }
        #endregion

        #region privates
        /// <summary>
        /// Gets a unique identifier for javascript references that need to be unique
        /// </summary>
        private string UniqueIdentifierForJavascriptReferences
        {
            get
            {
                return this.UniqueID.Replace("$", string.Empty);
            }
        }
        #endregion
    }
}
