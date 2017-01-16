using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing.Design;


namespace SIT.Libs.Base.CAB.Web.Controls.ContextMenu
{
    #region ContextMenuItem Class
    /// <summary>
    /// Represents a context menu item
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ParseChildren(true, "SubMenu")]
    [DefaultProperty("ContextMenuItems")]
    public class ContextMenuItem
    {
        #region Private members
        private string text = string.Empty;
        private string commandName = string.Empty;
        private string tooltip = string.Empty;
        private string imageUrl = string.Empty;
        private ContextMenuItemCollection subMenu;
        private bool enabled = true;
        private bool visible = true;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Creates a new empty ContextMenuItem
        /// </summary>
        public ContextMenuItem() { }

        /// <summary>
        /// Creates a new ContextMenuItem
        /// </summary>
        /// <param name="text">Item's text</param>
        /// <param name="commandName">Item's command name</param>
        public ContextMenuItem(string text, string commandName)
        {
            this.text = text;
            this.commandName = commandName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text of this context menu item
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Gets or sets the text of this context menu item")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        /// <summary>
        /// Gets or sets the Command name associated with this context menu item
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Gets or sets the Command name associated with this context menu item")]
        [NotifyParentProperty(true)]
        public string CommandName
        {
            get
            {
                return commandName;
            }

            set
            {
                commandName = value;
            }
        }


        /// <summary>
        /// Gets or sets the tooltip for this context menu item
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("Gets or sets the tooltip for this context menu item")]
        [NotifyParentProperty(true)]
        [Localizable(true)]
        public string Tooltip
        {
            get
            {
                return tooltip;
            }

            set
            {
                tooltip = value;
            }
        }

        /// <summary>
        /// Gets or sets an url of an image to associate to this context menu item
        /// </summary>
        [DefaultValue("")]
        [UrlProperty]
        [Category("Appearance")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Url of an image to associate to this context menu item")]
        [NotifyParentProperty(true)]
        public string ImageUrl
        {
            get
            {
                return imageUrl;
            }

            set
            {
                imageUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the visibility of this context menu item
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Indicates whether this context menu item is rendered and visible")]
        [NotifyParentProperty(true)]
        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this context menu item is enabled
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Indicates whether this context menu item can accept user clicks")]
        [NotifyParentProperty(true)]
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
            }
        }

        /// <summary>
        ///  Gets the collection of the this menu item's sub menu items; if empty this menu item
        ///  will be considered as a top level one. A textless menu item will be interpreted
        ///  as separator
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("Gets the collection of the sub menu items")]
        public ContextMenuItemCollection SubMenu
        {
            get
            {
                if (subMenu == null)
                    subMenu =  new ContextMenuItemCollection();

                return subMenu;
            }
        }

        /// <summary>
        /// Gets if this menu entry is a sub menu or a simple command item
        /// </summary>
        [Browsable(false)]
        public bool IsSubMenuEntry
        {
            get
            {
                return SubMenu.Count > 0;
            }
        }
        #endregion
    }
    #endregion 

	#region ContextMenuItemCollection Class
    /// <summary>
    /// A collection of ContextMenuItem
    /// </summary>
    [Editor(typeof(ContextMenuItemCollectionEditor), typeof(UITypeEditor))]
    [Serializable]
	public sealed class ContextMenuItemCollection : CollectionBase
	{
		/// <summary>
        /// Creates a new empty ContextMenuItemCollection
		/// </summary>
        public ContextMenuItemCollection() { }
		
		/// <summary>
        /// Gets or sets the ContextMenuItem at the specified position
		/// </summary>
        /// <param name="index">ContextMenuItem's position</param>
        /// <returns>The ContextMenuItem at the specified position</returns> 
		public ContextMenuItem this[int index]
		{
			get 
            { 
                return (ContextMenuItem) InnerList[index]; 
            }

			set 
            { 
                InnerList[index] = value; 
            }
		}
        
        /// <summary>
        /// Adds a ContextMenuItem to the collection
        /// </summary>
        /// <param name="item">The ContextMenuItem to add</param>
		public void Add(ContextMenuItem item)
		{
			InnerList.Add(item);
		}
        /*
		/// <summary>
        /// Adds a ContextMenuItem to the collection at the specified position
		/// </summary>
        /// <param name="index">The position the ContextMenuItem will be added at</param>
        /// <param name="item">The ContextMenuItem to add</param>
		public void AddAt(int index, ContextMenuItem item)
		{
			InnerList.Insert(index, item);
		}*/
	}
	#endregion

    #region ContextMenuItemCollectionEditor Class
    /// <summary>
    /// A design time editor for a ContextMenuItemCollection Object
    /// </summary>
    public class ContextMenuItemCollectionEditor : CollectionEditor
    {
        private CollectionForm collectionForm;

        /// <summary>
        /// Initializes a new instance of the ContextMenuItemCollectionEditor
        /// class using the specified collection type.        
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit</param>
        public ContextMenuItemCollectionEditor(Type type) : base(type) { } 

        /// <summary>
        /// Gets the data type that this collection contains: this implentation returns
        /// the type of ContextMenuItem
        /// </summary>
        /// <returns>Always typeof(ContextMenuItem)</returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof(ContextMenuItem);
        }

        /// <summary>
        /// Edits the value of the specified Object using the specified service provider
        /// and context
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that can be used to gain
        /// additional context information</param>
        /// <param name="provider">A service provider Object through which editing services can be obtained</param>
        /// <param name="value">The Object to edit the value of</param>
        /// <returns>The new value of the Object. If the value of the Object has not changed, 
        /// this should return the same Object it was passed</returns>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
        {
            if (this.collectionForm != null && this.collectionForm.Visible)
            {
                ContextMenuItemCollectionEditor editor = 
                    new ContextMenuItemCollectionEditor(this.CollectionType);

                return editor.EditValue(context, provider, value);
            }
            else
                return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Creates a new form to display and edit the current collection
        /// </summary>
        /// <returns>A System.ComponentModel.Design.CollectionEditor.CollectionForm to provide
        /// as the user interface for editing the collection</returns>
        protected override CollectionEditor.CollectionForm CreateCollectionForm()
        {
            this.collectionForm = base.CreateCollectionForm();
            return this.collectionForm;
        }
    }
    #endregion

    //----------------------------------------------------------------//

    #region ContextMenuBoundControlID Class
    /// <summary>
    /// Represents the ID of a control bound to a context menu item
    /// </summary>
    public class ContextMenuBoundControlID
    {
        #region Private members
        private string boundControlID;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Creates a new empty ContextMenuBoundControlID
        /// </summary>
        public ContextMenuBoundControlID() { }

        /// <summary>
        /// Creates a new ContextMenuBoundControlID
        /// </summary>
        /// <param name="boundControlID">The ID of the bound control</param>
        public ContextMenuBoundControlID(string boundControlID)
        {
            this.boundControlID = boundControlID;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ID of the control bound to a context menu
        /// </summary>
        [Description("Gets or sets the ID of the control bound to a context menu")]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(ControlIDTypeConverter))]
        public string BoundControlID
        {
            get
            {
                return boundControlID;
            }

            set
            {
                boundControlID = value;
            }
        }
        #endregion
    }
    #endregion     

    #region ContextMenuBoundControlsCollection Class
    /// <summary>
    /// A collection of ContextMenuBoundControlID
    /// </summary>
    //[Editor(typeof(ContextMenuBoundControlsCollectionEditor), typeof(UITypeEditor))]
    [Serializable]
    public sealed class ContextMenuBoundControlsCollection : CollectionBase
    {
        /// <summary>
        /// Creates a new empty ContextMenuBoundControlsCollection
        /// </summary>
        public ContextMenuBoundControlsCollection() { }

        /// <summary>
        /// Gets or sets the ContextMenuBoundControlID at the specified position
        /// </summary>
        /// <param name="index">ContextMenuBoundControlID's position</param>
        /// <returns>The ContextMenuBoundControlID at the specified position</returns> 
        public ContextMenuBoundControlID this[int index]
        {
            get
            {
                return (ContextMenuBoundControlID)InnerList[index];
            }

            set
            {
                InnerList[index] = value;
            }
        }

        /// <summary>
        /// Adds a ContextMenuBoundControlID to the collection
        /// </summary>
        /// <param name="item">The ContextMenuBoundControlID to add</param>
        public void Add(ContextMenuBoundControlID item)
        {
            InnerList.Add(item);
        }
    }
    #endregion    

    #region ControlIDTypeConverter Class
    /// <summary>
    /// Used to be able to browse controls' ID using designer
    /// </summary>
    internal class ControlIDTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be
        /// picked from a list.
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context</param>
        /// <returns>Always true, i.e. the object supports a common set of values</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of controls' ID browsed in the page
        /// </summary>
        /// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context
        /// that can be used to extract additional information about the environment
        /// from which this converter is invoked</param>
        /// <returns>A collection of controls' ID browsed in the page</returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                IReferenceService rs = context.GetService(typeof(IReferenceService)) as IReferenceService;

                if (rs == null)
                    return null;

                Object[] controls = rs.GetReferences(typeof(Control));

                ArrayList ids = new ArrayList();

                foreach (Control ctrl in controls)
                {
                    if (ctrl != null)
                    {
                        string controlID = rs.GetName(ctrl);
                        ids.Add(controlID);
                    }
                }

                ids.Add("Form");

                return new StandardValuesCollection(ids);
            }

            return base.GetStandardValues(context);
        }
    }
    #endregion
}
