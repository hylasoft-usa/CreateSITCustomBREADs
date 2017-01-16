using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;
using System.Globalization;
using System.Collections.ObjectModel;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.WebControlsScripts.ExtendedGridView.js", "application/x-javascript")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    /// <summary>
    /// An extended grid view with the following features:
    /// <ul>
    /// <li>Multi selection by means of a checkbox column</li>
    /// <li>Header freezing</li>
    /// </ul>
    /// </summary>
    [ToolboxBitmap(typeof(System.Web.UI.WebControls.GridView))]
    [Description("An extended ASP.NET GridView with several additional features")]
    public partial class ExtendedGridView : System.Web.UI.WebControls.GridView
    {
        #region constants and variables
        private const string MULTIPLESELECTIONGRIDVIEW_JS = "SIT.Libs.Base.CAB.Web.Controls.WebControlsScripts.ExtendedGridView.js";
        private ArrayList cachedSelectedIndexes;
        private const string FrozenTopCssClass = "frozenTop";

        HiddenField verticalScrollPositionHidden;
        private bool dataBound = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether a checkbox column is generated automatically at runtime
        /// to allow multiple selection
        /// </summary>
        [Category("Multiple selection")]
        [DefaultValue(false)]
        [Description("Gets or sets whether a checkbox column is generated automatically at runtime to allow multiple selection")]
        public bool AutoGenerateCheckBoxColumn
        {
            get
            {
                Object o = ViewState["AutoGenerateCheckBoxColumn"];

                if (o == null)
                    return false;

                return (bool)o;
            }

            set 
            { 
                ViewState["AutoGenerateCheckBoxColumn"] = value; 
            }
        }

        /// <summary>
        /// Gets or sets the 0-based position of the checkbox column (if AutoGenerateCheckBoxColumn holds true)
        /// </summary>
        [Category("Multiple selection")]
        [DefaultValue(0)]
        [Description("Gets or sets the 0-based position of the checkbox column (if AutoGenerateCheckBoxColumn holds true)")]
        public int CheckBoxColumnIndex
        {
            get
            {
                Object o = ViewState["CheckBoxColumnIndex"];

                if (o == null)
                    return 0;

                return (int)o;
            }

            set 
            {
                ViewState["CheckBoxColumnIndex"] = (value < 0 ? 0 : value); 
            }
        }

        /// <summary>
        /// Gets or sets whether the header should be frozen when scrolling content
        /// </summary>
        [DefaultValue(true)]
        [Category("Freeze header")]
        [Description("Gets or sets whether the header should be frozen when scrolling content")]
        public bool FreezeHeader
        {
            get
            {
                Object val = this.ViewState["FreezeHeader"];

                if (val == null)
                    return true;

                return (bool)val;
            }

            set
            {
                this.ViewState["FreezeHeader"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of scrolling feature this grid should implement
        /// </summary>
        [DefaultValue(ScrollBars.Auto)]
        [Category("Freeze header")]
        [Description("Gets or sets the type of scrolling feature this grid should implement")]
        public ScrollBars Scrolling
        {
            get
            {
                Object val = this.ViewState["Scrolling"];

                if (val == null)
                    return ScrollBars.Auto;

                return (ScrollBars)val;
            }

            set
            {
                this.ViewState["Scrolling"] = value;
            }
        }
                
        /// <summary>
        /// Gets or sets the width of the grid
        /// </summary>
        [Description("Gets or sets the width of the grid")]
        public override Unit Width
        {
            get
            {
                Object val = this.ViewState["DivWidth"];

                if (val == null)
                    return Unit.Empty;

                return (Unit)val;
            }

            set
            {
                this.ViewState["DivWidth"] = value;
            }
        }        

        /// <summary>
        /// Gets or sets the height of the grid
        /// </summary>
        [Description("Gets or sets the height of the grid")]
        public override Unit Height
        {
            get
            {
                Object val = this.ViewState["DivHeight"];

                if (val == null)
                    return Unit.Empty;

                return (Unit)val;
            }

            set
            {
                this.ViewState["DivHeight"] = value;
            }
        }

        /// <summary>
        /// Gets the horizontal overflow 
        /// </summary>
        private string OverflowX
        {
            get
            {
                if (this.Scrolling == ScrollBars.Horizontal || this.Scrolling == ScrollBars.Both)
                    return "scroll";
                else
                {
                    if (this.Scrolling == ScrollBars.Auto)
                        return "auto";
                    else
                        return "visible";
                }
            }
        }

        /// <summary>
        /// Gets the vertical overflow 
        /// </summary>
        private string OverflowY
        {
            get
            {
                if (this.Scrolling == ScrollBars.Vertical || this.Scrolling == ScrollBars.Both)
                    return "scroll";
                else
                {
                    if (this.Scrolling == ScrollBars.Auto)
                        return "auto";
                    else
                        return "visible";
                }
            }
        }

        /// <summary>
        /// Gets or sets whether vertical scroll position should be remembered after
        /// postbacks
        /// </summary>
        [DefaultValue(ExtendedGridViewScrollPositionPersistance.IfNotDataBound)]
        [Category("Freeze header")]
        [Description("Gets or sets whether vertical scroll position should be remembered after postbacks")]
        public ExtendedGridViewScrollPositionPersistance RememberVerticalScrollPosition
        {
            get
            {
                Object val = this.ViewState["RemeberVerticalScrollPosition"];

                if (val == null)
                    return ExtendedGridViewScrollPositionPersistance.IfNotDataBound;

                return (ExtendedGridViewScrollPositionPersistance)val;
            }

            set
            {
                this.ViewState["RemeberVerticalScrollPosition"] = value;
            }
        }

        /// <summary>
        /// Gets the selected rows if multi selection is enabled
        /// </summary>
        [Browsable(false)]
        public Collection<GridViewRow> SelectedRows
        {
            get
            {
                Collection<GridViewRow> toRet = new Collection<GridViewRow>();

                foreach (int index in SelectedIndexes)
                {
                    toRet.Add(this.Rows[index]);
                }

                return toRet;
            }
        }

        /// <summary>
        /// Gets or sets the array of selected indexes if multi selection is enabled
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int[] SelectedIndexes
        {
            get
            {
                cachedSelectedIndexes = new ArrayList();

                for (int i = 0; i < Rows.Count; i++)
                {
                    // Retrieve the reference to the checkbox
                    CheckBox cb = (CheckBox)Rows[i].FindControl(InputCheckBoxField.CheckBoxID);

                    if (cb == null)
                        return (int[])cachedSelectedIndexes.ToArray(typeof(int));
                    if (cb.Checked)
                        cachedSelectedIndexes.Add(i);
                }

                return (int[])cachedSelectedIndexes.ToArray(typeof(int));
            }

            set
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    // Retrieve the reference to the checkbox
                    CheckBox cb = (CheckBox)Rows[i].FindControl(InputCheckBoxField.CheckBoxID);

                    if (cb == null)
                        return;
                    else
                    {
                        foreach (int index in value)
                        {
                            if (index == i)
                            {
                                cb.Checked = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        
        #endregion

        #region Members overrides
        /// <summary>
        /// Overridden; cancel vertical scrollbar position "memory"
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            dataBound = true;
        }

        /// <summary>
        /// Overridden; adds the hidden field to restore vertical scrollbar position
        /// to the control's hierarchy
        /// </summary>
        /// <param name="e">An System.EventArgs Object that contains the event data</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            verticalScrollPositionHidden = new HiddenField();
            verticalScrollPositionHidden.ID = string.Format("__gv{0}__hidden", ClientID);
            verticalScrollPositionHidden.Value = "0";
        }

        /// <summary>
        /// Overridden; restores hidden fields' state
        /// </summary>
        /// <param name="savedState">An System.Object that contains the saved control state values for the control</param>
        protected override void LoadControlState(Object savedState)
        {
            base.LoadControlState(savedState);

            try
            {
                if (Page.Request[verticalScrollPositionHidden.ClientID] != null)
                    verticalScrollPositionHidden.Value = Page.Request[verticalScrollPositionHidden.ClientID];
            }
            catch { }
        }

        /// <summary>
        /// Overriden; adds a checkbox column if necessary
        /// </summary>
        /// <param name="dataSource">A System.Web.UI.WebControls.PagedDataSource that 
        /// represents the data source</param>
        /// <param name="useDataSource">true to use the data source specified by the 
        /// dataSource parameter; otherwise, false</param>
        /// <returns>A System.Collections.ICollection that contains the fields used to 
        /// build the control hierarchy</returns>
        protected override ICollection CreateColumns(PagedDataSource dataSource, bool useDataSource)
        {
            // Let the GridView create the default set of columns
            ICollection columnList = base.CreateColumns(dataSource, useDataSource);
            
            if (!AutoGenerateCheckBoxColumn)
                return columnList;

            IEnumerator columnsEnumerator = columnList.GetEnumerator();
            while (columnsEnumerator.MoveNext())
            {
                if (columnsEnumerator.Current.GetType() == typeof(CommandField))
                {
                    CommandField curCol = (CommandField)columnsEnumerator.Current;
                    
                    bool keepVisible = 
                        curCol.ShowInsertButton || curCol.ShowEditButton || curCol.ShowDeleteButton;

                    if (keepVisible)//simply disable selection
                        curCol.ShowSelectButton = false;
                    else
                        curCol.Visible = false; //hide column
                }
            }

            // Add a checkbox column if required
            ArrayList extendedColumnList = AddCheckBoxColumn(columnList);
            return extendedColumnList;
        }

        /// <summary>
        /// Overriden; injects the script necessary to manage the checkbox column
        /// on client side
        /// </summary>
        /// <param name="e">An System.EventArgs Object that contains the event data</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Type t = this.GetType();
            
            string url = Page.ClientScript.GetWebResourceUrl(t, MULTIPLESELECTIONGRIDVIEW_JS);
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(t, MULTIPLESELECTIONGRIDVIEW_JS))
                Page.ClientScript.RegisterClientScriptInclude(t, MULTIPLESELECTIONGRIDVIEW_JS, url);

            //script to adjust width on client side
            string adjustWidthScript = @"                
                function AdjustGridActualWidth_" + ClientID + @"() 
                {
                    try
                    {
                        document.getElementById('" + ClientID + @"').style.pixelWidth = document.getElementById('" + string.Format(CultureInfo.InvariantCulture, "__gv{0}__div", ClientID) + @"').clientWidth;
                    }
                    catch(ex) { }
                }";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "AdjustActualGridWidthDeclaration" + ClientID, adjustWidthScript, true);
            Page.ClientScript.RegisterStartupScript(GetType(), "AdjustActualGridWidthOnLoad" + ClientID, "window.attachEvent('onload', AdjustGridActualWidth_" + ClientID + ");", true);
            Page.ClientScript.RegisterStartupScript(GetType(), "AdjustActualGridWidthOnResize" + ClientID, "window.attachEvent('onresize', AdjustGridActualWidth_" + ClientID + ");", true);
        }

        /// <summary>
        /// Overriden; restores checkboxes' state, attaches them the client side click event handler,
        /// updates row state according to the corresponding checkbox' state, sets the
        /// style for the header to freeze and injects scripts to restore vertical scroll position
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            // Do as usual
            base.OnPreRender(e);

            //script to remember scroll position
            Page.ClientScript.RegisterStartupScript(GetType(), "SaveScrollPositionOnClientSide" + ClientID, "document.getElementById('" + string.Format(CultureInfo.InvariantCulture, "__gv{0}__div", ClientID) + "').attachEvent('onscroll', function() { document.getElementById('" + verticalScrollPositionHidden.ClientID + @"').value = document.getElementById('" + string.Format(CultureInfo.InvariantCulture, "__gv{0}__div", ClientID) + "').scrollTop; });", true);
            if (RestoreVerticalScroll)
                Page.ClientScript.RegisterStartupScript(GetType(), "RestoreScrollPosition" + ClientID, "document.getElementById('" + string.Format(CultureInfo.InvariantCulture, "__gv{0}__div", ClientID) + @"').scrollTop = " + (string.IsNullOrEmpty(verticalScrollPositionHidden.Value) ? "0" : verticalScrollPositionHidden.Value) + ";", true);

            if (AutoGenerateCheckBoxColumn)
            {
                string shouldCheck = (SelectedIndexes.Length == Rows.Count) ? "checked=\"checked\"" : string.Empty;
                string checkBoxID = string.Format(CheckBoxColumHeaderID, ClientID);

                if (HeaderRow != null)
                    HeaderRow.Cells[CheckBoxColumnIndex].Text =
                        string.Format(CheckBoxColumHeaderTemplate, checkBoxID, shouldCheck);
            }

            // Adjust each data row
            foreach (GridViewRow r in Rows)
            {
                // Get the appropriate style Object for the row
                TableItemStyle style = GetRowStyleFromState(r.RowState);

                // Retrieve the reference to the checkbox
                CheckBox cb = (CheckBox)r.FindControl(InputCheckBoxField.CheckBoxID);

                // Build the ID of the checkbox in the header
                string headerCheckBoxID = string.Format(CheckBoxColumHeaderID, ClientID);

                if (cb != null)
                {
                    // Add script code to enable selection
                    cb.Attributes["onclick"] = string.Format("ApplyStyle(this, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                            ColorTranslator.ToHtml(SelectedRowStyle.ForeColor),
                            ColorTranslator.ToHtml(SelectedRowStyle.BackColor),
                            ColorTranslator.ToHtml(style.ForeColor),
                            ColorTranslator.ToHtml(style.BackColor),
                            (style.Font.Bold ? 700 : 400),
                            headerCheckBoxID);

                    // Update the style of the checkbox if checked
                    if (cb.Checked)
                    {
                        r.BackColor = SelectedRowStyle.BackColor;
                        r.ForeColor = SelectedRowStyle.ForeColor;
                        r.Font.Bold = SelectedRowStyle.Font.Bold;
                    }
                    else
                    {
                        r.BackColor = style.BackColor;
                        r.ForeColor = style.ForeColor;
                        r.Font.Bold = style.Font.Bold;
                    }
                }
            }

            this.FreezeCells();

            if (this.FreezeHeader && !this.Page.Items.Contains(ExtendedGridView.FrozenTopCssClass))
            {
                this.Page.Items[ExtendedGridView.FrozenTopCssClass] = "1";
                FrozenTopStyle frozenTopStyle = new FrozenTopStyle();
                this.Page.Header.StyleSheet.CreateStyleRule(frozenTopStyle, null, "." + ExtendedGridView.FrozenTopCssClass);
            }
        }

        /// <summary>
        /// Overidden; creates a container div to scroll if necessary
        /// </summary>
        /// <param name="writer">The System.Web.UI.HtmlTextWriter used to render this
        /// control content on the client's browser.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                this.Page.VerifyRenderingInServerForm(this);
            }

            this.PrepareControlHierarchy();

            if (!this.DesignMode)
            {
                string clientID = this.ClientID;

                if (clientID == null)
                    throw new HttpException("ExtendedGridView must be parented");

                writer.AddAttribute(HtmlTextWriterAttribute.Id, string.Format(CultureInfo.InvariantCulture, "__gv{0}__div", clientID), true);
                writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowX, this.OverflowX);
                writer.AddStyleAttribute(HtmlTextWriterStyle.OverflowY, this.OverflowY);
                
                if (!this.Width.IsEmpty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString(CultureInfo.InvariantCulture));

                if (!this.Height.IsEmpty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height.ToString(CultureInfo.InvariantCulture));

                writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
            }

            this.RenderContents(writer);

            if (!this.DesignMode)
            {
                writer.RenderEndTag();
                verticalScrollPositionHidden.RenderControl(writer);
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Programmatically resets vertical scroll position
        /// </summary>
        public void ResetVerticalScrollPosition()
        {
            verticalScrollPositionHidden.Value = "0";
        }
        #endregion

        #region privates
        /// <summary>
        /// Used internally to restore vertical scroll bar position if not data bound
        /// </summary>
        private bool RestoreVerticalScroll
        {
            get
            {
                if (RememberVerticalScrollPosition == ExtendedGridViewScrollPositionPersistance.Never)
                    return false;

                if (RememberVerticalScrollPosition == ExtendedGridViewScrollPositionPersistance.Always)
                    return true;

                return !dataBound;
            }
        }

        /// <summary>
        /// Freezes header row
        /// </summary>
        private void FreezeCells()
        {
            if (this.FreezeHeader)
            {
                if (this.HeaderRow != null)
                {
                    foreach (DataControlFieldHeaderCell th in this.HeaderRow.Cells)
                        th.CssClass = ExtendedGridView.FrozenTopCssClass + " " + th.CssClass;
                }
            }
        }

        private class FrozenTopStyle : Style
        {
            internal FrozenTopStyle() { }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);

                attributes[HtmlTextWriterStyle.Top] = "expression(this.offsetParent.scrollTop)";
                attributes[HtmlTextWriterStyle.Position] = "relative";
                //attributes[HtmlTextWriterStyle.ZIndex] = "999999";
            }
        }

        #endregion
    }

    /// <summary>
    /// An enumeration providing the possible options to remember scroll bar position
    /// after postbacks for an ExtendedGridView
    /// </summary>
    public enum ExtendedGridViewScrollPositionPersistance
    {
        /// <summary>
        /// Always remember scroll bar position
        /// </summary>
        Always,

        /// <summary>
        /// Remember scroll bar position if grid has not been data bound
        /// </summary>
        IfNotDataBound,

        /// <summary>
        /// Don't remember scroll bar position
        /// </summary>
        Never
    }
}