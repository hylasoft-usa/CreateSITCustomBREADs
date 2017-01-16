using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    /// <summary>
    /// An extended grid view with the following features:
    /// <ul>
    /// <li>Multi selection by means of a checkbox column</li>
    /// <li>Header freezing</li>
    /// </ul>
    /// </summary>
    public partial class ExtendedGridView : GridView
    {
        #region Constants and variables
        private const string CheckBoxColumHeaderTemplate = "<input type='checkbox' hidefocus='true' id='{0}' name='{0}' {1} onclick='CheckAll(this)'>";
        private const string CheckBoxColumHeaderID = "{0}_HeaderButton";
        #endregion

        /// <summary>
        /// Adds a brand new checkbox column
        /// </summary>
        /// <param name="columnList">The columns list</param>
        /// <returns>The new columns list</returns>
        protected virtual ArrayList AddCheckBoxColumn(ICollection columnList)
        {
            // Get a new container of type ArrayList that contains the given collection. 
            // This is required because ICollection doesn't include Add methods
            // For guidelines on when to use ICollection vs IList see Cwalina's blog
            ArrayList list = new ArrayList(columnList);

            /* check postponed on pre render
            // Determine the check state for the header checkbox
            string shouldCheck = "";
            string checkBoxID = string.Format(CheckBoxColumHeaderID, ClientID);
            if (!DesignMode)
            {
                Object o = Page.Request[checkBoxID];
                if (o != null)
                {
                    shouldCheck = "checked=\"checked\"";
                }
            }
            */

            // Create a new custom CheckBoxField Object 
            InputCheckBoxField field = new InputCheckBoxField();
            //field.HeaderText = string.Format(CheckBoxColumHeaderTemplate, checkBoxID, shouldCheck);
            field.ReadOnly = true;

            // Insert the checkbox field into the list at the specified position
            if (CheckBoxColumnIndex > list.Count)
            {
                // If the desired position exceeds the number of columns 
                // add the checkbox field to the right. Note that this check
                // can only be made here because only now we know exactly HOW 
                // MANY columns we're going to have. Checking Columns.Count in the 
                // property setter doesn't work if columns are auto-generated
                list.Add(field);
                CheckBoxColumnIndex = list.Count - 1;
            }
            else
                list.Insert(CheckBoxColumnIndex, field);

            // Return the new list
            return list;
        }
    
        /// <summary>
        /// Retrieve the style Object based on the row state
        /// </summary>
        /// <param name="state">Row state</param>
        /// <returns>Style Object</returns>
        protected virtual TableItemStyle GetRowStyleFromState(DataControlRowState state)
        {
            switch (state)
            {
                case DataControlRowState.Alternate:
                    return AlternatingRowStyle;
                case DataControlRowState.Edit:
                    return EditRowStyle;
                case DataControlRowState.Selected:
                    return SelectedRowStyle;
                default:
                    return RowStyle;

                // DataControlRowState.Insert is not relevant here
            }
        }
    }
}