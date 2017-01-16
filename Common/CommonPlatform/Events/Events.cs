using System;

namespace System.CommonPlatform.Events
{
    /// <summary>
    ///     An enumeration of possible event's severity levels
    /// </summary>
    public enum EventSeverity
    {
        /// <summary>
        ///     An event that might be useful for internal debug logics
        /// </summary>
        Debug,

        /// <summary>
        ///     An informational event
        /// </summary>
        Information,

        /// <summary>
        ///     An event represening a failed operation
        /// </summary>
        Error,

        /// <summary>
        ///     An event raised once a performance measurement is available
        /// </summary>
        PerformanceMeasurement,

        /// <summary>
        ///     An event raised once an operation is about to begin
        /// </summary>
        BeginOperation,

        /// <summary>
        ///     An event raised once an operation is over
        /// </summary>
        EndOperation,

        /// <summary>
        ///     An event representing a warning
        /// </summary>
        Warning,
    }

    /// <summary>
    ///     Describes the event occuring in case a meaningful action occurs that can be managed from outside
    /// </summary>
    public class ActionPerformedEvent : EventArgs
    {
        #region constructors

        /// <summary>
        ///     Creates a new ActionPerformedEvent object containing information about the action being taken
        /// </summary>
        /// <param name="actionText">The description of the action being taken</param>
        /// <param name="actionCode">The numeric code of the action being taken</param>
        /// <param name="actionSeverity">The severity level of the action being taken</param>
        public ActionPerformedEvent(string actionText, int actionCode, EventSeverity actionSeverity)
        {
            ActionText = actionText;
            ActionCode = actionCode;
            ActionSeverity = actionSeverity;
        }

        #endregion

        #region getters/setters

        /// <summary>
        ///     Gets the action description
        /// </summary>
        public string ActionText { get; set; }

        /// <summary>
        ///     Gets the action numeric code
        /// </summary>
        public int ActionCode { get; set; }

        /// <summary>
        ///     Gets the severity level
        /// </summary>
        public EventSeverity ActionSeverity { get; set; }

        #endregion

        #region overrides

        /// <summary>
        ///     Return a string representation of this event
        /// </summary>
        /// <returns>A string representation of this event</returns>
        public override string ToString()
        {
            return string.Format("{0}, [{1}, return code = {2}]", ActionText, ActionSeverity, ActionCode);
        }

        #endregion
    }

    /// <summary>
    ///     Defines the method that must be implemented in order to manage events described by an ActionPerformedEvent object
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">An ActionPerformedEvent object that contains the event data</param>
    public delegate void ActionPerformedEventHandler(object sender, ActionPerformedEvent e);
}