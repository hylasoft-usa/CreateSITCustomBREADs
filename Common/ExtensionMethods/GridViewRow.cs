using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace System.ExtensionMethods
{
    /// <summary>
    ///     Class containing extension methods for System.Web.UI.WebControls.GridViewRow
    /// </summary>
    public static class GridViewRowExtensions
    {
        /// <summary>
        ///     Finds a bound cell by name in a databound System.Web.UI.WebControls.GridViewRow
        /// </summary>
        /// <param name="row">The target System.Web.UI.WebControls.GridViewRow</param>
        /// <param name="boundFieldName">The name of the bound field which column must be sought</param>
        /// <returns>The index of the column found inside the grid, -1 if no column is found</returns>
        public static int GetBoundColumnIndexByName(this GridViewRow row, string boundFieldName)
        {
            int toRet = 0;
            bool found = false;

            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.ContainingField is BoundField)
                {
                    if (string.Compare(((BoundField)cell.ContainingField).DataField, boundFieldName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        found = true;
                        break;
                    }
                }

                toRet++; //keep adding 1 while we don't have the correct name
            }

            if (!found)
                toRet = -1;

            return toRet;
        }

        /// <summary>
        ///     Finds a System.Web.UI.Control embedded as first level child into any cell of a System.Web.UI.WebControls.GridViewRow
        /// </summary>
        /// <param name="row">The row embedded control has to be searched in</param>
        /// <param name="controlID">The ID of the System.Web.UI.Control to find</param>
        /// <returns>The System.Web.UI.Control found or null if none was found</returns>
        public static Control FindFirstLevelControl(this GridViewRow row, string controlID)
        {
            foreach (TableCell cell in row.Cells)
            {
                Control ctrl = cell.FindControl(controlID);
                if (ctrl != null) return ctrl;
            }

            return null;
        }
    }
}
