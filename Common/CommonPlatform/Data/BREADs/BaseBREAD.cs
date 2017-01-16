using System.CommonPlatform.DB;
using System.CommonPlatform.Events;

namespace System.CommonPlatform.Data.BREADs
{
    /// <summary>
    ///     Base class for all BREADs
    /// </summary>
    public class BaseBREAD : ActionPerformedRaiser
    {
        #region properties

        /// <summary>
        ///     Gets the data layer object to be used to interface the underlying database
        /// </summary>
        protected DBGateway DBGateway { get; set; }

        #endregion

        #region constructors

        /// <summary>
        ///     Creates a new empty BaseBREAD object
        /// </summary>
        protected BaseBREAD() { }

        /// <summary>
        ///     Creates a new BaseBREAD object
        /// </summary>
        /// <param name="dbGateway">The data layer object to be used to interface the underlying database</param>
        protected BaseBREAD(DBGateway dbGateway)
        {
            //instanciate DBGateway object
            DBGateway = dbGateway;
        }

        #endregion

        #region other methods

        /// <summary>
        ///     Creates a unique long PK
        /// </summary>
        /// <returns>Guid based unique long PK</returns>
        protected long CreateUniqueLongPK()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        ///     Creates a unique int PK
        /// </summary>
        /// <returns>Guid based unique int PK</returns>
        protected int CreateUniqueIntPK()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        ///     Creates a unique unsigned int PK
        /// </summary>
        /// <returns>Guid based unique int PK</returns>
        protected uint CreateUniqueUnsignedIntPK()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        ///     Creates a unique short PK
        /// </summary>
        /// <returns>Guid based unique short PK</returns>
        protected short CreateUniqueShortPK()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt16(buffer, 0);
        }

        #endregion
    }
}
