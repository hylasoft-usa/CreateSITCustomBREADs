using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace SIT.Libs.Base.CAB.Web.Controls.Popup.Events
{ 
    /// <summary>
    /// Represents the method that will handle the button click event for a Popup object
    /// </summary>
    /// <param name="sender">The source button for the event</param>
    /// <param name="e">A EventArgs that contains the event data</param>
    public delegate void PopupButtonClickedEventHandler(PopupButton sender, EventArgs e);
}
