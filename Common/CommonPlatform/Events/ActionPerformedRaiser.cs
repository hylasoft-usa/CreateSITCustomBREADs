using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.CommonPlatform.Events;

namespace System.CommonPlatform.Events
{
    /// <summary>
    ///     A class with built in methods to raise ActionPerformed events
    /// </summary>
    public class ActionPerformedRaiser
    {
        #region variables

        /// <summary>
        ///     Occurs when a meaningful action is to be notified
        /// </summary>
        public event ActionPerformedEventHandler ActionPerformed;

        #endregion

        #region public/inheritable methods

        /// <summary>
        ///     Raises an event carrying an ActionPerformedEvent object to describe it
        /// </summary>
        /// <param name="e">The ActionPerformedEvent object describing the event</param>
        /// <remarks>Use this methid to notify about a meaningful action or benchmark</remarks>
        protected void RaiseActionPerformedEvent(ActionPerformedEvent e)
        {
            RaiseActionPerformedEvent(this, e);
        }

        /// <summary>
        ///     Raises an event carrying an ActionPerformedEvent object to describe it
        /// </summary>
        /// <param name="sender">The sender of this event</param>
        /// <param name="e">The ActionPerformedEvent object describing the event</param>
        /// <remarks>Use this methid to notify about a meaningful action or benchmark</remarks>
        protected void RaiseActionPerformedEvent(object sender, ActionPerformedEvent e)
        {
#if DEBUG
            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine(e.ActionText, GetType().Name);
            Debug.WriteLine("----------------------------------------------------------------");
#endif

            if (ActionPerformed != null) ActionPerformed(sender, e);
        }

        #endregion
    }
}
