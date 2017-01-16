using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System;

namespace SIT.Libs.Base.CAB.Web.Controls.Design
{
    /// <summary>
    /// Designer class for the SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar
    /// </summary>
    internal class DatePickerTextBoxDesigner : ControlDesigner
    {
        /// <summary>
        /// Returns the HTML to display in the VS IDE
        /// </summary>
        /// <returns>The HTML to display in the VS IDE</returns>
        public override string GetDesignTimeHtml()
        {
            DatePickerTextBox designedControl = (DatePickerTextBox)Component;

            StringWriter sw = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(sw);
            writer.InnerWriter = sw;

            TextBox txt = new TextBox();
            txt.Width = designedControl.Width;
            txt.Height = designedControl.Height;

            txt.RenderControl(writer);
            return sw.ToString();
        }
    }
}
