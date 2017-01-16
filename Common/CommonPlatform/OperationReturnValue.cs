using System;
using System.Diagnostics;
using System.Reflection;

namespace System.CommonPlatform
{
    /// <summary>
    ///     Represents the outcome of a generic operation
    /// </summary>
    public class OperationReturnValue
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the error code
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     Gets or sets the error description
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        ///     True if ErrorCode equals zero
        /// </summary>
        public bool Succeeded
        {
            get
            {
                return ErrorCode == 0;
            }
        }

        #endregion

        #region constructor

        /// <summary>
        ///     Creates an OperationReturnValue object representing a successfull operation (error code = 0, empty error description) 
        /// </summary>
        public OperationReturnValue() : this(0, string.Empty) { }

        /// <summary>
        ///     Creates an OperationReturnValue object
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="errorDescription">Error description</param>
        public OperationReturnValue(int errorCode, string errorDescription)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
        }

        /// <summary>
        ///     Creates an OperationReturnValue object using a formatted error description
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="includeCallerMethodNameInDescription">Whether the caller method full name should be suffixed to the formatted error description</param>
        /// <param name="errorDescription">A composite format error description</param>
        /// <param name="args">An array containing zero or more objects to format</param>
        public OperationReturnValue(int errorCode, bool includeCallerMethodNameInDescription, string errorDescription, params object[] args)
        {
            ErrorCode = errorCode;

            if (includeCallerMethodNameInDescription)
            {
                //look for the real caller, discarding internal calls from other constructors inside this class
                StackFrame actualCallerStackFrame = null;
                StackFrame[] stackFrames = new StackTrace().GetFrames();

                foreach (StackFrame sf in stackFrames)
                {
                    //is currently inspected StackFrame referring to a method that does not belong to this class?
                    if (string.Compare(sf.GetMethod().DeclaringType.FullName, GetType().FullName, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        actualCallerStackFrame = sf;
                        break;
                    }
                }

                if (actualCallerStackFrame == null)
                    actualCallerStackFrame = stackFrames[0];

                MethodBase method = actualCallerStackFrame.GetMethod(); //get calling method
                string callingMethodFullName = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);

                ErrorDescription = string.Format("[{0}][{1}]", callingMethodFullName, string.Format(errorDescription, args));
            }
            else
                ErrorDescription = string.Format(errorDescription, args);
        }

        /// <summary>
        ///     Creates an OperationReturnValue object using a formatted error prefixed by the caller method full name
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <param name="errorDescription">A composite format error description</param>
        /// <param name="args">An array containing zero or more objects to format</param>
        public OperationReturnValue(int errorCode, string errorDescription, params object[] args) : this(errorCode, true, errorDescription, args) { }

        #endregion

        #region overrides

        /// <summary>
        ///     Returns a string that represents the current OperationReturnValue object
        /// </summary>
        /// <returns>A string that represents the current OperationReturnValue object</returns>
        public override string ToString()
        {
            return string.Format(
                "Error code: '{0}', error description: '{1}'",
                ErrorCode,
                ErrorDescription);
        }

        #endregion
    }
}
