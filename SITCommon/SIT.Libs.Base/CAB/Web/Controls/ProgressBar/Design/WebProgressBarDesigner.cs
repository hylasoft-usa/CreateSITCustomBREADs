using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System;

namespace SIT.Libs.Base.CAB.Web.Controls.ProgressBar.Design
{
    /// <summary>
    /// Designer class for the SIT.Libs.Base.CAB.Web.Controls.ProgressBar.WebProgressBar
    /// </summary>
    internal class WebProgressBarDesigner : ControlDesigner
    {
        /// <summary>
        /// Returns the HTML to display in the VS IDE
        /// </summary>
        /// <returns>The HTML to display in the VS IDE</returns>
        public override string GetDesignTimeHtml()
        {
            int w = 37;

            WebProgressBar designedControl = (WebProgressBar)Component;

            StringWriter sw = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(sw);
            writer.InnerWriter = sw;

            Table tbl = new Table();
            tbl.CellPadding = 0;
            tbl.CellSpacing = 0;
            tbl.BorderStyle = BorderStyle.Solid;
            tbl.BorderWidth = Unit.Pixel(2);
            tbl.BorderColor = Color.DarkGray;
            tbl.Width = designedControl.Width;
            tbl.Height = designedControl.Height;

            TableRow row = new TableRow();
            TableCell cell1 = new TableCell();
            TableCell cell2 = new TableCell();

            cell1.Width = Unit.Percentage(w);
            cell1.BackColor = Color.Gray;

            cell2.Width = Unit.Percentage(100-w);
            //Panel p = new Panel();
            //p.Width = Unit.Percentage(37);
            //p.BackColor = Color.Gray;

            //cell.Controls.Add(p);
            row.Cells.Add(cell1);
            row.Cells.Add(cell2);
            tbl.Rows.Add(row);

            tbl.RenderControl(writer);
            return sw.ToString();
        }
    }
}
