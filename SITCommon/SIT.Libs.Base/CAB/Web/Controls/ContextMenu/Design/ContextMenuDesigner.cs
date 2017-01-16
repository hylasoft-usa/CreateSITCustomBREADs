using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI.Design.WebControls;

namespace SIT.Libs.Base.CAB.Web.Controls.ContextMenu.Design
{
    #region ContextMenuDesign Class
    /// <summary>
    /// Supports design time operations for a ContextMenu control
    /// </summary>
    public class ContextMenuDesign : HierarchicalDataBoundControlDesigner
    {
        #region Private members
        private ContextMenu contextMenuInstance;        
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new ContextMenuDesign Object
        /// </summary>
        public ContextMenuDesign() : base() { }
        #endregion

        #region Overridden methods
        /// <summary>
        /// Initialize the control to render at design-time
        /// </summary>
        /// <param name="component">The control being designed</param>
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            contextMenuInstance = (ContextMenu)component;

            base.Initialize(component);
        }

        /// <summary>
        /// Returns the HTML to display in the VS IDE
        /// </summary>
        /// <returns>The HTML to display in the VS IDE</returns>
        public override string GetDesignTimeHtml()
        {
            bool willBeBound = !string.IsNullOrEmpty(contextMenuInstance.DataSourceID);
            ContextMenuItemCollection contextMenuItemsBackup = contextMenuInstance.ContextMenuItems; 
            int numOfItems = contextMenuInstance.ContextMenuItems.Count;

            if (willBeBound)//ContextMenu will be bound
            {
                contextMenuItemsBackup = contextMenuInstance.ContextMenuItems;//backup ContextMenuItems to restore them later

                contextMenuInstance.ContextMenuItems.Clear();

                ContextMenuItem selectedItem = new ContextMenuItem("Bound Item", "");
                contextMenuInstance.ContextMenuItems.Add(selectedItem);

                for (int i = 0; i < 5; i++)
                {
                    ContextMenuItem item = new ContextMenuItem("Bound Item", "");
                    contextMenuInstance.ContextMenuItems.Add(item);
                }
            }
            else
            {
                if (numOfItems == 0)//not bound and empty
                {
                    contextMenuInstance.ContextMenuItems.Clear();

                    ContextMenuItem selectedItem = new ContextMenuItem("Selected Item", "");
                    contextMenuInstance.ContextMenuItems.Add(selectedItem);

                    for (int i = 0; i < 5; i++)
                    {
                        ContextMenuItem item = new ContextMenuItem("Item", "");
                        contextMenuInstance.ContextMenuItems.Add(item);
                    }
                }
            }

            int selectedItemPos = 0;

            // Pseudo-rendering
            StringWriter swTemp = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(swTemp);
            contextMenuInstance.RenderControl(writer);
            writer.Close();
            swTemp.Close();

            // Modify the background color of the selected item
            Table menu = (Table)(contextMenuInstance.mainPanel).Controls[0];
            TableRow row = menu.Rows[selectedItemPos];
            row.BackColor = contextMenuInstance.RolloverBackColor;
            row.ForeColor = contextMenuInstance.RolloverColor;

            if(row.Cells[1].Controls[0] is LinkButton)
                ((LinkButton)row.Cells[1].Controls[0]).ForeColor = contextMenuInstance.RolloverColor;

            StringWriter sw = new StringWriter();
            writer.InnerWriter = sw;
            menu.RenderControl(writer);

            if (willBeBound)
            {
                contextMenuInstance.ContextMenuItems.Clear();

                foreach (ContextMenuItem menuItem in contextMenuItemsBackup)
                    contextMenuInstance.ContextMenuItems.Add(menuItem);
            }
            else
            {
                if (numOfItems == 0)
                    contextMenuInstance.ContextMenuItems.Clear();
            }

            return sw.ToString();
        }

        /// <summary>
        /// Adds Data binding properties to smart tag actions if this context menu is
        /// selected to be data bound
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection toRet = base.ActionLists;

                if (!string.IsNullOrEmpty(contextMenuInstance.DataSourceID))
                    toRet.Add(new ContextMenuDesignerActionList(contextMenuInstance));

                return toRet;
            }
        }
        #endregion

        /// <summary>
        /// Represents the Data binding properties section to be added to smart tag menu for
        /// </summary>
        internal class ContextMenuDesignerActionList : DesignerActionList
        {
            #region constructors
            /// <summary>
            /// Creates a new ContextMenuDesignerActionList for the supplied ContextMenu component
            /// </summary>
            /// <param name="component">The target ContextMenu component</param>
            public ContextMenuDesignerActionList(ContextMenu component) : base(component) { }
            #endregion

            #region overrides
            /// <summary>
            /// Returns the list of actions exposed by this ContextMenuDesignerActionList
            /// </summary>
            /// <returns>The list of actions exposed by this ContextMenuDesignerActionList</returns>
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection actionItems = new DesignerActionItemCollection();

                actionItems.Add(new DesignerActionHeaderItem("Data binding properties"));

                actionItems.Add(new DesignerActionPropertyItem(
                    "DataTextField",
                    "Set text field (mandatory)",
                    "Data binding properties",
                    string.Format(
                        "Set the field of {0} used to bind text for {1}",
                        ContextMenu.DataSourceID,
                        ContextMenu.ID)));

                actionItems.Add(new DesignerActionPropertyItem(
                    "DataCommandNameField",
                    "Set command name field",
                    "Data binding properties",
                    string.Format(
                        "Set the field of {0} used to bind command name for {1}",
                        ContextMenu.DataSourceID,
                        ContextMenu.ID)));

                actionItems.Add(new DesignerActionPropertyItem(
                    "DataVisibleField",
                    "Set visibility field",
                    "Data binding properties",
                    string.Format(
                        "Set the field of {0} used to bind visibility for {1}",
                        ContextMenu.DataSourceID,
                        ContextMenu.ID)));

                actionItems.Add(new DesignerActionPropertyItem(
                    "DataImageUrlField",
                    "Set image URL field",
                    "Data binding properties",
                    string.Format(
                        "Set the field of {0} used to bind image URL for {1}",
                        ContextMenu.DataSourceID,
                        ContextMenu.ID)));

                actionItems.Add(new DesignerActionPropertyItem(
                    "DataToolTipField",
                    "Set tooltip field",
                    "Data binding properties",
                    string.Format(
                        "Set the field of {0} used to bind tooltip for {1}",
                        ContextMenu.DataSourceID,
                        ContextMenu.ID)));
                
                return actionItems;
            }
            #endregion

            #region Context menu properties exposed by smart tag
            /// <summary>
            /// Context menu's DataTextField field
            /// </summary>
            public string DataTextField
            {
                get
                {
                    return this.ContextMenu.DataTextField;
                }

                set
                {
                    SetProperty("DataTextField", value);
                }
            }

            /// <summary>
            /// Context menu's DataCommandNameField field
            /// </summary>
            public string DataCommandNameField
            {
                get
                {
                    return this.ContextMenu.DataCommandNameField;
                }

                set
                {
                    SetProperty("DataCommandNameField", value);
                }
            }

            /// <summary>
            /// Context menu's DataToolTipField field
            /// </summary>
            public string DataToolTipField
            {
                get
                {
                    return this.ContextMenu.DataToolTipField;
                }

                set
                {
                    SetProperty("DataToolTipField", value);
                }
            }

            /// <summary>
            /// Context menu's DataVisibleField field
            /// </summary>
            public string DataVisibleField
            {
                get
                {
                    return this.ContextMenu.DataVisibleField;
                }

                set
                {
                    SetProperty("DataVisibleField", value);
                }
            }

            /// <summary>
            /// Context menu's DataImageUrlField field
            /// </summary>
            public string DataImageUrlField
            {
                get
                {
                    return this.ContextMenu.DataImageUrlField;
                }

                set
                {
                    SetProperty("DataImageUrlField", value);
                }
            }
            #endregion

            #region privates
            /// <summary>
            /// Sets a Context menu's porperty safely
            /// </summary>
            /// <param name="propertyName">Property to set</param>
            /// <param name="value">Property value</param>
            private void SetProperty(string propertyName, object value)
            {                
                //Get property
                PropertyDescriptor property =
                    TypeDescriptor.GetProperties(this.ContextMenu)[propertyName];

                //Set property
                property.SetValue(this.ContextMenu, value);
            }

            private ContextMenu ContextMenu
            {
                get
                {
                    return (ContextMenu)this.Component;
                }
            }
            #endregion
        }
    }
    #endregion
}
