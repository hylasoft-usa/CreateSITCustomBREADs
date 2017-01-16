using System.Collections;

namespace SIT.Libs.Base.CAB.Web.Controls.Popup
{
	/// <summary>
    /// Represent a collection of SIT.Libs.Base.CAB.Web.Controls.Popup.PopupFooterButtons
	/// </summary>
	public class PopupButtonsCollection : CollectionBase
	{
		/// <summary>
        /// Gets the SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton at the specific position
		/// </summary>
		public PopupButton this[int index]
		{
			get  
			{
                return ((PopupButton)List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Convert the list into ArrayList
		/// </summary>
		/// <returns>The ArrayList</returns>
		public ArrayList ToArrayList()
		{
			ArrayList retArray = new ArrayList();

			for (int i = 0; i < this.Count; i++)
				retArray.Add(this[i]);

			return retArray;
		}

		/// <summary>
        /// Add a SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton to the collection
		/// </summary>
		/// <param name="value">The Object to add</param>
		/// <returns>CollectionBase returned value</returns>
        public int Add(PopupButton value)  
		{
			return(List.Add(value));
		}

		/// <summary>
        /// Get the index of a SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton into the collection
		/// </summary>
        /// <param name="value">The SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton to find</param>
        /// <returns>The index of a SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton into the collection</returns>
        public int IndexOf(PopupButton value)  
		{
			return(List.IndexOf(value));
		}

		/// <summary>
		/// Insert an Object into a specific position into the collection
		/// </summary>
		/// <param name="index">Index of position</param>
		/// <param name="value">The Object to add</param>
        public void Insert(int index, PopupButton value)  
		{
			List.Insert(index, value);
		}

		/// <summary>
		/// Remove an Object from the list
		/// </summary>
		/// <param name="value">The Object to remove</param>
        public void Remove(PopupButton value)  
		{
			List.Remove(value);
		}

		/// <summary>
        /// Check if the SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton is already contained into the list
		/// </summary>
        /// <param name="value">The SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton to check</param>
        /// <returns>True if the SIT.Libs.Base.CAB.Web.Controls.Popup.PopupButton is already 
        /// contained into the list, otherwise false</returns>
        public bool Contains(PopupButton value)  
		{
			return(List.Contains(value));
		}

		/// <summary>
		/// Constructor with parameters
		/// </summary>
		/// <param name="items">List of items</param>
		public PopupButtonsCollection(ArrayList items) : base()
		{
			for (int i = 0; i<items.Count; i++)
			{
                if (items[i] is PopupButton)
                    this.Add((PopupButton)items[i]);
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
        public PopupButtonsCollection() : base() { }
	}
}
