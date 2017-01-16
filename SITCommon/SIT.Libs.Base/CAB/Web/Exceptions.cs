using System;

namespace SIT.Libs.Base.CAB.Exceptions.Web
{
    /// <summary>
    /// Exception thron whenever an operation supposed to be done under a web context is
    /// done from a non-web context
    /// </summary>
    public class NotAWebApplicationException : Exception
    {
        /// <summary>
        /// Initializes a new emptySIT.Libs.Base.CAB.Exceptions.Web.NotAWebApplicationException Object
        /// </summary>
        public NotAWebApplicationException() : base() { }

        /// <summary>
        /// Initializes a newSIT.Libs.Base.CAB.Exceptions.Web.NotAWebApplicationException with a specified
        /// error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public NotAWebApplicationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a newSIT.Libs.Base.CAB.Exceptions.Web.NotAWebApplicationException Object with a 
        /// specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="InnerException">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public NotAWebApplicationException(string message, Exception InnerException) : base(message, InnerException) { }
    }

    /// <summary>
    /// Exception thrown whenever a translation operation fails for any reason
    /// </summary>
    public class TranslationException : Exception
    {
        /// <summary>
        /// Initializes a new emptySIT.Libs.Base.CAB.Exceptions.Web.TranslationException Object
        /// </summary>
        public TranslationException() : base() { }

        /// <summary>
        /// Initializes a newSIT.Libs.Base.CAB.Exceptions.Web.TranslationException with a specified
        /// error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public TranslationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a newSIT.Libs.Base.CAB.Exceptions.Web.TranslationException Object with a 
        /// specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="InnerException">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public TranslationException(string message, Exception InnerException) : base(message, InnerException) { }
    }

    /// <summary>
    /// Exception thrown whenever aSIT.Libs.Base.CAB.Web.UI.CabUserControlBase is not hosted inside a
    ///SIT.Libs.Base.CAB.Web.UI.CABBaseContentPage
    /// </summary>
    public class CabUserControlBaseNotHostedInsideCabPageBaseException : Exception
    {
        /// <summary>
        /// Initializes a new empty 
        ///SIT.Libs.Base.CAB.Exceptions.Web.CabUserControlBaseNotHostedInsideCabPageBaseException Object
        /// </summary>
        public CabUserControlBaseNotHostedInsideCabPageBaseException() : base() { }

        /// <summary>
        /// Initializes a new 
        ///SIT.Libs.Base.CAB.Exceptions.Web.CabUserControlBaseNotHostedInsideCabPageBaseException 
        /// with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public CabUserControlBaseNotHostedInsideCabPageBaseException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new 
        ///SIT.Libs.Base.CAB.Exceptions.Web.CabUserControlBaseNotHostedInsideCabPageBaseException Object 
        /// with a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="InnerException">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public CabUserControlBaseNotHostedInsideCabPageBaseException(string message, Exception InnerException) : base(message, InnerException) { }
    }
}
