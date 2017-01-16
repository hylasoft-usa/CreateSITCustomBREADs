using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: TagPrefix("SIT.Libs.Base.CAB.Web.Controls", "wc")]
namespace SIT.Libs.Base.CAB.Web.Controls
{
    /// <summary>
    /// This extended web drop down list may contain a user definied custom element or a blank element 
    /// or both on top of its items collection. In case both elements (custom and blank) are enabled,
    /// blank element is displayed before the custom one
    /// </summary>
    [ToolboxBitmap(typeof(System.Web.UI.WebControls.DropDownList))]
    [Description("A drop down list which may contain an empty element or a custom element or both on top of its items collection")]
    public class ExtendedDropDownList : DropDownList
    {
        #region properties
        /// <summary>
        /// A string defining the value for the empty element in order to make it recognizable.
        /// Ignored if TopBlankElementEnabled holds false
        /// </summary>
        [Description("A string defining the value for the empty element in order to make it recognizable. Ignored if TopBlankElementEnabled holds false")]
        [Category("Data")]
        [Browsable(true)]
        public string BlankElementValue
        {
            get
            {
                if (ViewState["TopBlankElementValue"] == null)
                    return this.UniqueID + "_TopBlankElementValueForThisSpecificDropDown";
                else
                    return ViewState["TopBlankElementValue"].ToString();
            }

            set
            {
                ViewState["TopBlankElementValue"] = value;
            }
        }

        /// <summary>
        /// This property holds true if the blank element is selected, false otherwise
        /// </summary>
        [Browsable(false)]
        public bool BlankElementItemSelected
        {
            get
            {
                return this.SelectedValue == BlankElementValue;
            }
        }

        /// <summary>
        /// Set this property to true to display a blank element on top of this drop down
        /// list's items collection. No blank element will be displayed if drop down list's
        /// items collection is empty
        /// </summary>
        [Description("Insert a blank element on top of this dropdown's items collection? No blank element will be displayed if drop down list's items collection is empty")]
        [Category("Behavior")]
        [Browsable(true)]
        public bool BlankElementEnabled
        {
            get
            {
                if (ViewState["InsertBlankElement"] == null)
                    return true;
                else
                    return Boolean.Parse(ViewState["InsertBlankElement"].ToString());
            }

            set
            {
                ViewState["InsertBlankElement"] = value;
            }
        }

        /// <summary>
        /// This property holds true if the custom element is selected, false otherwise
        /// </summary>
        [Browsable(false)]
        public bool CustomValueSelected
        {
            get
            {
                return this.SelectedValue == CustomItemValue;
            }
        }

        /// <summary>
        /// Set this property to something different from empty string to display a 
        /// custom element on top of this drop down list's items collection.
        /// Custom element will be displayed as second one if blank element is enabled.
        /// If an empty string is provided it assumed that no custom item will be displayed.
        /// In order to set a value for the custom item, use CustomItemValue property
        /// </summary>
        [Description("Set this property to something different from empty string to display a custom element on top of this drop down list's items collection. Custom element will be displayed as second one if blank element is enabled. If an empty string is provided it assumed that no custom item will be displayed. In order to set a value for the custom item, use CustomItemValue property")]
        [Category("Data")]
        [Browsable(true)]
        public string CustomItemText
        {
            get
            {
                if (ViewState["CustomItemText"] == null)
                    return string.Empty;
                else
                    return ViewState["CustomItemText"].ToString();
            }

            set
            {
                ViewState["CustomItemText"] = value;
            }
        }

        /// <summary>
        /// A string defining the value for the custom element in order to make it recognizable.
        /// Ignored if CustomItemText is the empty string
        /// </summary>
        [Description("A string defining the value for the custom element in order to make it recognizable. Ignored if CustomItemText is the empty string")]
        [Category("Data")]
        [Browsable(true)]
        public string CustomItemValue
        {
            get
            {
                if (ViewState["CustomItemValue"] == null)
                    return this.UniqueID + "_CustomItemValueForThisSpecificDropDown";
                else
                    return ViewState["CustomItemValue"].ToString();
            }

            set
            {
                ViewState["CustomItemValue"] = value;
            }
        }
        /*
        /// <summary>
        /// Should this drop down list display distinct text values? 
        /// If set ot true only the first instance of each text will be kept, 
        /// discarding the others. Use carefully since associated values might be different
        /// </summary>
        [Description("Should this drop down list display distinct text values? If set ot true only the first instance of each text will be kept, discarding the others. Use carefully since associated values might be different")]
        [Category("Behavior")]
        [Browsable(true)]
        public bool DisplayDistinctTexts
        {
            get
            {
                if (ViewState["DisplayDistinctTexts"] == null)
                    return false;
                else
                    return Boolean.Parse(ViewState["DisplayDistinctTexts"].ToString());
            }

            set
            {
                ViewState["DisplayDistinctTexts"] = value;
            }
        }*/

        /// <summary>
        /// This property holds true if a custom element different from empty string was previously set
        /// for this drop down list
        /// </summary>
        [Browsable(false)]
        private bool CustomValueEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(CustomItemText);
            }
        }
        #endregion

        #region privates
        /// <summary>
        /// Inserts on top of the item collection blank and custom element if specified
        /// </summary>
        private void InsertBlankAndCustomElements()
        {            
            if (CustomValueEnabled)
            {
                if (this.Items.FindByValue(CustomItemValue) == null)
                {
                    ListItem customValue = new ListItem(CustomItemText, CustomItemValue);
                    this.Items.Insert(0, customValue);
                }
            }

            if (BlankElementEnabled)
            {
                if (this.Items.FindByValue(BlankElementValue) == null)
                {
                    ListItem blankValue = new ListItem(string.Empty, BlankElementValue);
                    this.Items.Insert(0, blankValue);
                }
            }
        }
        #endregion

        #region overrides
        /// <summary>
        /// Inserts on top of the item collection blank and custom element if specified
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if(!DesignMode)
                InsertBlankAndCustomElements();

            base.OnInit(e);
        }

        /// <summary>
        /// Inserts on top of the item collection blank and custom element if specified
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBound(EventArgs e)
        {
            InsertBlankAndCustomElements();
            this.SelectedIndex = -1;

            base.OnDataBound(e);
        }

        /// <summary>
        /// Catches ArgumentOutOfRangeException caused by BlankElement or CustomElement 
        /// addition
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBinding(EventArgs e)
        {
            try
            {
                base.OnDataBinding(e);
            }
            catch(ArgumentOutOfRangeException aoore)
            {
                if (aoore.ParamName.ToLower() != "value")//don't catch, bubble it
                    throw aoore;
            }
        }
        #endregion
    }
}
