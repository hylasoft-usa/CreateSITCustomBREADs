using System;
using Siemens.SimaticIT.MES.Breads;
using System.CommonPlatform.Events;

namespace SIT.Libs.Base.Data.BREADs
{
    /// <summary>
    ///     A factory class to create BaseSITBREAD objects
    /// </summary>
    public static class BaseSITBREADsFactory
    {
        /// <summary>
        ///     Gets a working instance of any BaseSITBREAD derived class
        /// </summary>
        /// <typeparam name="T">The type of the required BREAD</typeparam>
        public static T GetBreadInstance<T>() where T : BaseSITBREAD
        {
            return GetBreadInstance<T>(null, null);
        }

        /// <summary>
        ///     Gets a working instance of any BaseSITBREAD derived class
        /// </summary>
        /// <typeparam name="T">The type of the required BREAD</typeparam>
        /// <param name="breadEventHandler">A delegate function to manage event raised by the newly created BREAD</param>
        /// <returns>A working instance of a BREAD having type breadInstanceType</returns>
        public static T GetBreadInstance<T>(ActionPerformedEventHandler breadEventHandler) where T : BaseSITBREAD
        {
            return GetBreadInstance<T>(null, breadEventHandler);
        }

        /// <summary>
        ///     Gets a working instance of any BaseSITBREAD derived class
        /// </summary>
        /// <typeparam name="T">The type of the required BREAD</typeparam>
        /// <param name="breadTransactionHandler">A SIT BREAD transaction the returned bread will be enrolled in</param>
        /// <param name="breadEventHandler">A delegate function to manage event raised by the newly created BREAD</param>
        /// <returns>A working instance of a BREAD having type breadInstanceType enrolled in the given transaction</returns>
        public static T GetBreadInstance<T>(BreadTransaction breadTransactionHandler, ActionPerformedEventHandler breadEventHandler) where T : BaseSITBREAD
        {
            T toRet = (T)Activator.CreateInstance(typeof(T));

            if (breadTransactionHandler != null) toRet.BreadTransactionHandler = breadTransactionHandler;
            if (breadEventHandler != null) toRet.ActionPerformed += breadEventHandler;

            return toRet;
        }
    }
}
