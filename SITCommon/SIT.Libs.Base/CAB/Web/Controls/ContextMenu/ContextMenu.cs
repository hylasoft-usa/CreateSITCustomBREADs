using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Collections;
using SIT.Libs.Base.CAB.Web.Controls.ContextMenu.Events;
using System.Collections.ObjectModel;
using SIT.Libs.Base.CAB.Web.UI;
using SIT.Libs.Base.CAB.Web.Controls.ContextMenu.Design;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls.ContextMenu", "wc")]
[assembly: WebResource("SIT.Libs.Base.CAB.Web.Controls.WebControlsScripts.ContextMenu.js", "application/x-javascript")]
namespace SIT.Libs.Base.CAB.Web.Controls.ContextMenu
{
    /// <summary>
    /// A web control representing a context menu
    /// </summary>
	[ParseChildren(true, "ContextMenuItems")]
    [Designer(typeof(ContextMenuDesign))]
    [DefaultEvent("ContextMenuEntryClicked")]
	[DefaultProperty("ContextMenuItems")]
    [ToolboxBitmap(typeof(System.Windows.Forms.MenuStrip))]
    [Description("An extended ASP.NET GridView with several additional features")]
	public class ContextMenu : HierarchicalDataBoundControl, INamingContainer
	{
		#region Private Members
        internal HiddenField boundControlSource;//An hidden field used to keep track of which bound control triggered a control
        internal PanelNamingContainer mainPanel;//The panel containing this context menu
		
        private ContextMenuItemCollection contextMenuItems;
        //private ArrayList boundControls;
        private ContextMenuBoundControlsCollection boundControlsIDs;
        private Collection<Control> boundControls;

        //long progressiveForId = DateTime.Now.Ticks;
		#endregion

		#region Constants
        private const string HrSeparator = "<hr style='height:1px;border:solid 1px #CCCCCC; border-style:inset;' />";
        private const string ShowContextMenu = "return showContextMenu('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');";
        private const string HideContextMenu = "HideAllVisibleContextMenus(); ";
		#endregion
        
		#region Events
        /// <summary>
        /// An event risen whenever a context menu entry has been clicked
        /// </summary>
		public event ContextMenuCommandEventHandler ContextMenuEntryClicked;
		#endregion       

        #region Methods
        /// <summary>
        /// Finds a control in page recursively
        /// </summary>
        /// <param name="id">Control id to find</param>
        /// <param name="parent">Root node to start searching from </param>
        /// <returns>The specified control, or null if the specified control does not exist</returns>
        private Control FindControl(string id, Control parent)
        {
            if (id == null)
                return null;

            if ((parent == Page) && (Page is CABBaseContentPage))
                return ((CABBaseContentPage)Page).FindControl(id, true);
            else
            {
                Control c = parent.FindControl(id);

                if (c == null)
                {
                    foreach (Control cc in parent.Controls)
                    {
                        c = FindControl(id, cc);
                        if (c != null) return c; else continue;
                    }
                }
                else
                    return c;

                return null;
            }
        }

        /// <summary>
        /// Returns the Javascript code to attach the context menu to a HTML element
		/// </summary>
        /// <returns>The Javascript code to attach the context menu to a HTML element</returns> 
		public string GetMenuReference(string BoundControlID)
		{
            return string.Format(ShowContextMenu,
                mainPanel.ClientID, 
                BoundControlID,
                boundControlSource.ClientID,
                ColorTranslator.ToHtml(ForeColor), 
                ColorTranslator.ToHtml(BackColor), 
                ColorTranslator.ToHtml(RolloverColor), 
                ColorTranslator.ToHtml(RolloverBackColor));
		}		
		
        /*
		/// <summary>
        ///  Returns the Javascript code to dismiss the context menu when the user hits ESC
		/// </summary>
        /// <returns>The Javascript code to dismiss the context menu when the user hits ESC</returns>
		public string GetEscReference()
		{
			return string.Format(TrapEscKey, Controls[0].ClientID);
		}

		/// <summary>
        /// Returns the Javascript code to dismiss the context menu when the user clicks outside the menu
		/// </summary>
        /// <returns>The Javascript code to dismiss the context menu when the user clicks outside the menu</returns> 
		public string GetOnClickReference()
		{
			return string.Format(HideOnClick, Controls[0].ClientID);
		}
        */
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the background color of this context menu when mouse is not over it
        /// </summary>
        [Description("Gets or sets the background color of this context menu when mouse is not over it")]
        [DefaultValue(typeof(Color), "#BFBBB4")]
        [TypeConverter(typeof(WebColorConverter))]
        public override Color BackColor
        {
            get
            {
                if (base.BackColor.IsEmpty)
                    return ColorTranslator.FromHtml("#BFBBB4");

                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the text color of this context menu when mouse is not over it
        /// </summary>
        [Description("Gets or sets the text color of this context menu when mouse is not over it")]
        [DefaultValue(typeof(Color), "Black")]
        [TypeConverter(typeof(WebColorConverter))]
        public override Color ForeColor
        {
            get
            {
                if (base.ForeColor.IsEmpty)
                    return Color.Black;

                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

		/// <summary>
        ///  Gets the collection of the menu items; a textless menu item will be interpreted
        ///  as separator
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Description("Gets the collection of the menu items")]
        [NotifyParentProperty(true)]
		public ContextMenuItemCollection ContextMenuItems
		{
			get 
			{
                if (contextMenuItems == null)
                    contextMenuItems = new ContextMenuItemCollection();

				return contextMenuItems;
			}
		}  

        /// <summary>
        ///  Gets the collection of controls IDs for which the context menu should be displayed; a textless menu item will be interpreted
        ///  as separator
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Gets the collection of controls IDs for which the context menu should be displayed")]
        [NotifyParentProperty(true)]
        public ContextMenuBoundControlsCollection BoundControlsIDs
        {
            get
            {
                if (boundControlsIDs == null)
                    boundControlsIDs = new ContextMenuBoundControlsCollection();

                return boundControlsIDs;
            }
        }

        /// <summary>
        /// Returns the collection of Control objects bound to this context menu
        /// </summary>
        private Collection<Control> BoundControls
        {
            get
            {
                boundControls = new Collection<Control>();

                foreach (ContextMenuBoundControlID c in BoundControlsIDs)
                {
                    if (c.BoundControlID.ToLower() == "form")
                        boundControls.Add(Page.Form);
                    else
                    {
                        Control ctrl = FindControl(c.BoundControlID, Parent);

                        if (ctrl != null)
                            boundControls.Add(ctrl);
                    }
                }

                return boundControls;
            }
        }

        #region colors
        /// <summary>
        /// Gets and sets the background color when the mouse hovers over the menu item
		/// </summary>
		[Description("Gets and sets the background color when the mouse hovers over the menu item")]
        [DefaultValue(typeof(Color), "#0A246A")]
        [TypeConverter(typeof(WebColorConverter))]
        [Category("Appearance")]
        public Color RolloverBackColor
		{
			get 
			{
                Object o = ViewState["RolloverBackColor"];

				if (o == null)
                    return ColorTranslator.FromHtml("#0A246A");

				return (Color)o;
			}

			set 
            {
                ViewState["RolloverBackColor"] = value;
            }
		}

        /// <summary>
        /// Gets and sets the text color when the mouse hovers over the menu item
        /// </summary>
        [Description("Gets and sets the text color when the mouse hovers over the menu item")]
        [DefaultValue(typeof(Color), "White")]
        [TypeConverter(typeof(WebColorConverter))]
        [Category("Appearance")]
        public Color RolloverColor
        {
            get
            {
                Object o = ViewState["RolloverColor"];

                if (o == null)
                    return Color.White;

                return (Color)o;
            }

            set
            {
                ViewState["RolloverColor"] = value;
            }
        }        
        #endregion

        /// <summary>
        /// Determines the pixels around each menu item 
		/// </summary>
		[Description("The space in pixels around each menu item")]
        [DefaultValue(2)]
        [Category("Appearance")]
		public int CellPadding
		{
			get 
			{
				Object o = ViewState["CellPadding"];

				if (o == null)
					return 2;

				return (int) o;
			}

			set 
			{
				ViewState["CellPadding"] = value;
			}
		}
		#endregion

		#region Rendering
		/// <summary>
        /// Determines the standard style of the control
		/// </summary>
        /// <returns>The standard style of the control</returns>
		protected override Style CreateControlStyle()
		{
			Style style = base.CreateControlStyle();

            style.BorderStyle = (BorderStyle == BorderStyle.NotSet) ? BorderStyle.Outset : BorderStyle;
            style.BorderColor = (BorderColor == Color.Empty) ? Color.Snow : BorderColor;
            style.BorderWidth = (BorderWidth == Unit.Empty) ? Unit.Pixel(1) : BorderWidth;
            style.BackColor = BackColor;
            style.ForeColor = ForeColor;
            
			return style;
		}

		/// <summary>
        /// Builds the UI of the control
		/// </summary>
		protected override void CreateChildControls()
		{
			// A context menu is an invisible layer that is moved around via scripting when the user
			// right-clicks on a bound HTML tag
            mainPanel = new PanelNamingContainer();
			mainPanel.ID = "Root";
            mainPanel.Attributes["onclick"] = "event.cancelBubble = true; return;";//to avoid to dismiss the menu if clicking over it            
			mainPanel.Style[HtmlTextWriterStyle.Display] = "none";
            mainPanel.Style[HtmlTextWriterStyle.Position] =  "absolute";

            // Add the button to the control's hierarchy for display
            if ((Controls.Count == 2) && (Controls[1] is Panel))
                /*
                 * CreateChildControl called by PerformDataBinding; it is the second time
                 * it is called (after the automatic call, so the mainPanel previously
                 * added must be removed
                 */
                Controls.RemoveAt(1);

            Controls.Add(mainPanel);
			
            //clicking outside the menu will make it disappear
            if (!DesignMode)
                Page.Form.Attributes["onclick"] += ContextMenu.HideContextMenu;

            Table menu = new Table();
            //menu.ID = "Table" + (progressiveForId++);
			menu.ApplyStyle(CreateControlStyle());
			menu.CellSpacing = 0;
            menu.CellPadding = CellPadding;
            //menu.Attributes["belongingMenu"] = mainPanel.ClientID;
			mainPanel.Controls.Add(menu);

			// Loop on ContextMenuItems and add rows to the table
			foreach(ContextMenuItem item in ContextMenuItems)
			{
                if (item.Visible)
                {
                    // Create and add the menu item
                    TableRow menuItem = new TableRow();
                    menuItem.ToolTip = item.Tooltip;
                    menu.Rows.Add(menuItem);

                    // Configure the menu item
                    TableCell containerImage = new TableCell();
                    menuItem.Cells.Add(containerImage);

                    TableCell containerText = new TableCell();
                    containerText.Style[HtmlTextWriterStyle.WhiteSpace] = "nowrap";
                    menuItem.Cells.Add(containerText);

                    TableCell containerSubMenuArrow = new TableCell();
                    containerSubMenuArrow.Font.Name = "Webdings";
                    menuItem.Cells.Add(containerSubMenuArrow);

                    // Define the menu item's contents
                    System.Web.UI.WebControls.Image icon = null; //an item menu's icon if specified; it will remain null otherwise

                    //image
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        if (!string.IsNullOrEmpty(item.ImageUrl))//no image
                        {
                            //if item's text is null or empty image is ignored
                            icon = new System.Web.UI.WebControls.Image();
                            icon.ImageUrl = item.ImageUrl;
                            containerImage.Controls.Add(icon);
                        }
                    }

                    //text
                    if (string.IsNullOrEmpty(item.Text))
                    {
                        LiteralControl l = new LiteralControl(ContextMenu.HrSeparator);
                        containerText.Controls.Add(l); //Empty item is a separator
                    }
                    else
                    {
                        if (item.IsSubMenuEntry)
                        {
                            LiteralControl l = new LiteralControl(item.Text);
                            containerText.Controls.Add(l); //Only text, without link

                            LiteralControl l2 = new LiteralControl("4");
                            containerSubMenuArrow.Controls.Add(l2); //a right arrow in webdings font

                            ProcessSubMenuEntry(mainPanel, menuItem, item);
                        }
                        else
                        {
                            // Add the link for post back
                            LinkButton button = new LinkButton();
                            containerText.Controls.Add(button);

                            button.Click += new EventHandler(ButtonClicked);
                            button.Width = Unit.Percentage(100);
                            button.Text = item.Text;
                            button.CommandName = item.CommandName;
                            button.Enabled = item.Enabled;
                        }
                    }
                }
			}            

			// Inject any needed script code into the page
			EmbedScriptCode();

			// Inject the script code for all bound controls
            foreach (Control ctrl in BoundControls)
			{
                WebControl ctl1 = (ctrl as WebControl);
                HtmlControl ctl2 = (ctrl as HtmlControl);

				if (ctl1 != null)
                    ctl1.Attributes["oncontextmenu"] = GetMenuReference(ctl1.ClientID);

				if (ctl2 != null)
					ctl2.Attributes["oncontextmenu"] = GetMenuReference(ctl2.ClientID);
			}
		}

		/// <summary>
        /// Renders the UI of the control 
		/// </summary>
        /// <param name="writer">The System.Web.UI.HtmlTextWriter Object that receives the control content</param>
		protected override void Render(HtmlTextWriter writer)
		{
			// Ensures the control behaves well at design-time 
			// (You don't need this, if the control supports data-binding because this gets 
			// implicitly called in DataBind()) 
			EnsureChildControls();

            CreateChildControls();

			// Style controls before rendering
			PrepareControlForRendering();

			// Avoid a surrounding <span> tag
			RenderContents(writer);
		}

		/// <summary>
        /// Apply styles to the control components immediately before rendering
		/// </summary>
		protected virtual void PrepareControlForRendering()
		{
			// Make sure there are controls to work with
			if (Controls.Count != 2)
				return;

			// Apply the table style
            PrepareMenuForRendering(mainPanel);			
		}

        /// <summary>
        /// Apply styles to a menu (as Panel)
        /// </summary>
        /// <param name="menuContainer">The Panel Object representing a menu</param>
        private void PrepareMenuForRendering(Panel menuContainer)
        {
            //Apply style to sub menus (if any)
            foreach (Control c in menuContainer.Controls)
            {
                if (c is Panel)
                    PrepareMenuForRendering((Panel)c);
            }

            menuContainer.Style[HtmlTextWriterStyle.ZIndex] = "999999";

            Table menu = (Table)menuContainer.Controls[0];
            menu.CopyBaseAttributes(this);

            if (ControlStyleCreated)
                menu.ApplyStyle(ControlStyle);

            // Style each menu item individually
            for (int i = 0; i < menu.Rows.Count; i++)
            {
                TableRow menuItem = menu.Rows[i];
                TableCell cell = menuItem.Cells[1];

                // Style the link button
                LinkButton button = (cell.Controls[0] as LinkButton);
                if (button != null)
                {
                    button.ForeColor = ForeColor;
                    button.Style["text-decoration"] = "none";
                }
            }
        }

		/// <summary>
        /// Insert the script code needed to refresh the UI
		/// </summary>
		private void EmbedScriptCode()
		{
            string jsUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "SIT.Libs.Base.CAB.Web.Controls.WebControlsScripts.ContextMenu.js");
            Page.ClientScript.RegisterClientScriptInclude(GetType(), "ContextMenuHelper" + ClientID, jsUrl);
		}

        private void ProcessSubMenuEntry(Panel mainContainer, TableRow parentTableRow, ContextMenuItem itemToProcess)
        {
            // A context sub menu is an invisible SPAN that is moved around via scripting when the user
            // right-clicks on a bound HTML tag
            PanelNamingContainer subMenuPanel = new PanelNamingContainer();
            mainContainer.Controls.Add(subMenuPanel);

            subMenuPanel.Style[HtmlTextWriterStyle.Display] = "none";
            subMenuPanel.Style[HtmlTextWriterStyle.Position] = "fixed";

            Table subMenu = new Table();
            subMenu.ApplyStyle(CreateControlStyle());
            subMenu.CellSpacing = 0;
            subMenu.CellPadding = CellPadding;
            subMenuPanel.Controls.Add(subMenu);

            // Loop on ContextMenuItems and add rows to the table
            foreach (ContextMenuItem item in itemToProcess.SubMenu)
            {
                if (item.Visible)
                {
                    // Create and add the menu item
                    TableRow subMenuItem = new TableRow();
                    subMenuItem.ToolTip = item.Tooltip;
                    subMenu.Rows.Add(subMenuItem);

                    //Add reference to child menu used on client side
                    parentTableRow.Attributes["childMenuID"] = subMenuPanel.ClientID;

                    // Configure the menu item
                    TableCell containerImage = new TableCell();
                    subMenuItem.Cells.Add(containerImage);

                    TableCell containerText = new TableCell();
                    containerText.Style[HtmlTextWriterStyle.WhiteSpace] = "nowrap";
                    subMenuItem.Cells.Add(containerText);

                    TableCell containerSubMenuArrow = new TableCell();
                    containerSubMenuArrow.Font.Name = "Webdings";
                    subMenuItem.Cells.Add(containerSubMenuArrow);

                    // Define the menu item's contents
                    System.Web.UI.WebControls.Image icon = null; //an item menu's icon if specified; it will remain null otherwise

                    //image
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        if (!string.IsNullOrEmpty(item.ImageUrl))//no image
                        {
                            //if item's text is null or empty image is ignored
                            icon = new System.Web.UI.WebControls.Image();
                            icon.ImageUrl = item.ImageUrl;
                            containerImage.Controls.Add(icon);
                        }
                    }

                    //text
                    if (string.IsNullOrEmpty(item.Text))
                    {
                        LiteralControl l = new LiteralControl(ContextMenu.HrSeparator);
                        containerText.Controls.Add(l); //Empty item is a separator
                    }
                    else
                    {
                        if (item.IsSubMenuEntry)
                        {
                            LiteralControl l = new LiteralControl(item.Text);
                            containerText.Controls.Add(l); //Only text, without link

                            LiteralControl l2 = new LiteralControl("4");
                            containerSubMenuArrow.Controls.Add(l2); //a right arrow in webdings font

                            ProcessSubMenuEntry(subMenuPanel, subMenuItem, item);
                        }
                        else
                        {
                            // Add the link for post back
                            LinkButton button = new LinkButton();
                            containerText.Controls.Add(button);

                            button.Click += new EventHandler(ButtonClicked);
                            button.Width = Unit.Percentage(100);
                            button.Text = item.Text;
                            button.CommandName = item.CommandName;
                            button.Enabled = item.Enabled;
                        }
                    }
                }
            }
        }
		#endregion

		#region Event-related Members
        /// <summary>
        /// Adds hidden field boundControlSource to the list of controls
        /// </summary>
        /// <param name="e">The EventArgs containing information about the event</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //An hidden field to keep track of what bound control triggered a ContextMenuEntryClicked event
            boundControlSource = new HiddenField();
            boundControlSource.ID = "Source";
            Controls.Add(boundControlSource);
        }

        /// <summary>
        /// If data bound, rebinds the controls in case of post back
        /// </summary>
        /// <param name="e">The EventArgs containing information about the event</param>
        protected override void OnLoad(EventArgs e)
        {
            if ((Page.IsPostBack) && (!string.IsNullOrEmpty(DataSourceID)))
                PerformDataBinding();

            base.OnLoad(e);
        }

		/// <summary>
        /// Fires the ItemCommand event to the host page
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">The EventArgs containing information about the event</param>
		private void ButtonClicked(Object sender, EventArgs e)
		{
			LinkButton button = sender as LinkButton;
			
            if (button != null)
			{
				CommandEventArgs args = new CommandEventArgs(button.CommandName, button.CommandArgument);
				OnItemCommand(args);
			}
		}		

		/// <summary>
        /// Fires the ItemCommand event to the host page
		/// </summary>
        /// <param name="e">The CommandEventArgs containing information about the event</param>
		protected virtual void OnItemCommand(CommandEventArgs e)
		{
            Control sourceControl = null;

            foreach (Control c in BoundControls)
            {
                if (c.ClientID == boundControlSource.Value)
                {
                    sourceControl = c;
                    break;
                }
            }

			if (ContextMenuEntryClicked != null)
				ContextMenuEntryClicked(this, new ContextMenuCommandEventArgs(e, sourceControl));
		}
		#endregion

        #region HierarchicalDataBoundControl overrides and management
        /// <summary>
        /// The data source field name containing text for menu items
		/// </summary>
		[Description("The data source field name containing text for menu items")]
        [DefaultValue("")]
        [Category("Data")]
		public string DataTextField
		{
			get 
			{
				Object o = ViewState["DataTextField"];

				if (o == null)
					return string.Empty;

				return o.ToString();
			}

			set 
			{
				ViewState["DataTextField"] = value;
			}
		}

        /// <summary>
        /// The data source field name containing command name for menu items
		/// </summary>
		[Description("The data source field name containing command name for menu items")]
        [DefaultValue("")]
        [Category("Data")]
		public string DataCommandNameField
		{
			get 
			{
				Object o = ViewState["DataCommandNameField"];

				if (o == null)
					return string.Empty;

				return o.ToString();
			}

			set 
			{
				ViewState["DataCommandNameField"] = value;
			}
		}

        /// <summary>
        /// The data source field name containing image url for menu items
		/// </summary>
		[Description("The data source field name containing image url for menu items")]
        [DefaultValue("")]
        [Category("Data")]
		public string DataImageUrlField
		{
			get 
			{
				Object o = ViewState["DataImageUrlField"];

				if (o == null)
					return string.Empty;

				return o.ToString();
			}

			set 
			{
				ViewState["DataImageUrlField"] = value;
			}
		}

        /// <summary>
        /// The data source field name containing tooltip for menu items
		/// </summary>
		[Description("The data source field name containing tooltip for menu items")]
        [DefaultValue("")]
        [Category("Data")]
		public string DataToolTipField
		{
			get 
			{
				Object o = ViewState["DataToolTipField"];

				if (o == null)
					return string.Empty;

				return o.ToString();
			}

			set 
			{
				ViewState["DataToolTipField"] = value;
			}
		}

        /// <summary>
        /// The data source field name containing visibility for menu items
        /// </summary>
        [Description("The data source field name containing visibility for menu items")]
        [DefaultValue("")]
        [Category("Data")]
        public string DataVisibleField
        {
            get
            {
                Object o = ViewState["DataVisibleField"];

                if (o == null)
                    return string.Empty;

                return o.ToString();
            }

            set
            {
                ViewState["DataVisibleField"] = value;
            }
        }

        /// <summary>
        /// Binds data from the data source to the context menu
        /// </summary>
        protected override void PerformDataBinding()
        {
            base.PerformDataBinding();

            //do not bind data if there's no data source set
            if (!IsBoundUsingDataSourceID && (DataSource == null))
                return;

            HierarchicalDataSourceView view = GetData(string.Empty);

            if (view == null)
                throw new InvalidOperationException("No view returned by data source control");

            IHierarchicalEnumerable enumerable = view.Select();
            if (enumerable != null)
            {
                ContextMenuItems.Clear();

                try
                {
                    RecurseDataBindInternal(ContextMenuItems, enumerable);
                    CreateChildControls();//Recreate controls hierarchy
                }
                finally { }
            }
        }
        
        /// <summary>
        /// Internal recursive implementation used by PerformDataBinding
        /// </summary>
        /// <param name="contextMenuItems"></param>
        /// <param name="enumerable"></param>
        private void RecurseDataBindInternal(ContextMenuItemCollection contextMenuItems, IHierarchicalEnumerable enumerable)
        {
            foreach (Object item in enumerable)
            {
                IHierarchyData data = enumerable.GetHierarchyData(item);

                if (data != null)
                {
                    ContextMenuItem cmi = new ContextMenuItem();

                    try
                    {
                        if (!string.IsNullOrEmpty(DataTextField))
                            cmi.Text = DataBinder.GetPropertyValue(data, DataTextField, null);

                        if (!string.IsNullOrEmpty(DataToolTipField))
                            cmi.Tooltip = DataBinder.GetPropertyValue(data, DataToolTipField, null);

                        if (!string.IsNullOrEmpty(DataImageUrlField))
                            cmi.ImageUrl = DataBinder.GetPropertyValue(data, DataImageUrlField, null);

                        if (!string.IsNullOrEmpty(DataCommandNameField))
                            cmi.CommandName = DataBinder.GetPropertyValue(data, DataCommandNameField, null);

                        if (!string.IsNullOrEmpty(DataVisibleField))
                        {
                            bool visible;
                            if (Boolean.TryParse(DataBinder.GetPropertyValue(data, DataVisibleField, null), out visible))
                                cmi.Visible = visible;
                        }
                    }
                    catch (System.Web.HttpException) { }//property not found in data item

                    if (data.HasChildren)
                    {
                        IHierarchicalEnumerable newEnumerable = data.GetChildren();

                        if (newEnumerable != null)
                        {
                            cmi.SubMenu.Clear();

                            RecurseDataBindInternal(cmi.SubMenu, newEnumerable);
                        }
                    }

                    contextMenuItems.Add(cmi);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// A panel implementing the INamingContainer interface
    /// </summary>
    internal class PanelNamingContainer : Panel, INamingContainer { }
}
