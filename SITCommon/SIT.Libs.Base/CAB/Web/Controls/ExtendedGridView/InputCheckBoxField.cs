using System.Web.UI;
using System.Web.UI.WebControls;
using System;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    internal sealed class InputCheckBoxField : CheckBoxField
    {
        public const string CheckBoxID = "CheckBoxButton";
        
        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            base.InitializeDataCell(cell, rowState);

            // Add a checkbox anyway, if not done already
            if (cell.Controls.Count == 0)
            {
                CheckBox chk = new CheckBox();
                chk.ID = InputCheckBoxField.CheckBoxID;
                cell.Controls.Add(chk);
            }
        }
    }
}
