using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System;

namespace SIT.Libs.Base.CAB.Web.Controls.Popup.Design
{
    /// <summary>
    /// Designer class for the SIT.Libs.Base.CAB.Web.Controls.Popup
    /// </summary>
    internal class PopupDesigner : ControlDesigner
    {		
        TemplateGroupCollection templateGroup = null;
        Popup designedControl = null;

        public override void Initialize(IComponent component)
        {
            // Initialize the base
            base.Initialize(component);

            // Turn on template editing
            SetViewFlags(ViewFlags.TemplateEditing, true);

            designedControl = (Popup)Component;
        }

        /// <summary>
        /// Returns the HTML to display in the VS IDE
        /// </summary>
        /// <returns>The HTML to display in the VS IDE</returns>
        public override string GetDesignTimeHtml()
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(sw);
            writer.InnerWriter = sw;

            Table tbl = new Table();
            tbl.CellPadding = 0;
            tbl.CellSpacing = 0;
            tbl.Width = designedControl.Width;
            tbl.BorderColor = Color.DarkBlue;
            tbl.BorderWidth = Unit.Pixel(1);
            tbl.BorderStyle = BorderStyle.Solid;

            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Height = Unit.Pixel(5);
            cell.BackColor = Color.Blue;
            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            row = new TableRow();
            cell = new TableCell();
            cell.BackColor = Color.White;

            Label l = new Label();
            l.Text = "Web Popup - Click here and use the task menu to edit content";
            cell.Controls.Add(l);

            row.Cells.Add(cell);
            tbl.Rows.Add(row);

            tbl.RenderControl(writer);
            return sw.ToString();
        }

        public override TemplateGroupCollection TemplateGroups
        {
            get
            {
                if (templateGroup == null)
                {
                    // Get the base collection
                    templateGroup = base.TemplateGroups;

                    // Create variables
                    TemplateGroup tempGroup;
                    TemplateDefinition tempDef;                    

                    // Create a TemplateGroup
                    tempGroup = new TemplateGroup("Popup Content");

                    // Create a TemplateDefinition
                    tempDef = new TemplateDefinition(this, "Popup Content",
                        designedControl, "PopupContent", true);

                    // Add the TemplateDefinition to the TemplateGroup
                    tempGroup.AddTemplateDefinition(tempDef);

                    // Add the TemplateGroup to the TemplateGroupCollection
                    templateGroup.Add(tempGroup);                    
                }

                return templateGroup;
            }
        }

        // Do not allow direct resizing unless in TemplateMode
        public override bool AllowResize
        {
            get
            {
                if (this.InTemplateMode)
                    return true;
                else
                    return false;
            }
        }

    }
}
