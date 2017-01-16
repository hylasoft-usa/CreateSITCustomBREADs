using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using Siemens.SimaticIT.MES.Breads;
using SIT.Libs.Base.DB;
using System.CommonPlatform.Events;
using SITCAB.DataSource.Libraries;
using System.CommonPlatform.Data.BREADs;
using System.ExtensionMethods;

namespace SIT.Libs.Base.Data.BREADs
{
    /// <summary>
    ///     Base class for all SIT BREADs
    /// </summary>
    public class BaseSITBREAD : BaseBREAD
    {
        #region variables

        private BreadTransaction breadTransactionHandler;

        #endregion

        #region properties

        /// <summary>
        ///     Gets the data layer object to be used to interface the underlying database
        /// </summary>
        protected new SITSqlServerGateway DBGateway { get; private set; }
        
        /// <summary>
        ///     Gets or sets the BreadTransaction object that will be used by methods in order to participate to an external transaction involving SIT BREADs; can be set to null
        /// </summary>
        public BreadTransaction BreadTransactionHandler 
        {
            get
            { 
                return breadTransactionHandler; 
            }
            set
            {
                breadTransactionHandler = value;

                if (value == null)
                {
                    //unset current transaction handler to all SIT BREADs
                    FieldInfo[] allFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (FieldInfo field in allFields)
                    {
                        if (field.FieldType.IsSameOrSubclass(typeof(EntityWithPlugins_BREAD)))
                        {
                            object fieldValue = field.GetValue(this);
                            if (fieldValue != null)
                                ((EntityWithPlugins_BREAD)fieldValue).UnsetCurrentTransactionHandle();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Return the user name to be passed on to SIT BREADS to perform write calls; it is read from an application setting named BREADUserName if any, otherwise "manager" is returned
        /// </summary>
        public string UserName
        {
            get
            {
                string toRet = ConfigurationManager.AppSettings["BREADUserName"];
                return string.IsNullOrEmpty(toRet) ? "manager" : toRet;
            }
        }

        /// <summary>
        ///     Return the password to be passed on to SIT BREADS to perform write calls; it is read from an application setting named BREADPassword if any, otherwise an empty string is returned
        /// </summary>
        public string Password
        {
            get
            {
                string toRet = ConfigurationManager.AppSettings["BREADPassword"];
                return string.IsNullOrEmpty(toRet) ? string.Empty : toRet;
            }
        }

        #endregion

        #region constructors

        /// <summary>
        ///     Creates a new BaseBREAD object by reading connection string from SIT services
        /// </summary>
        protected BaseSITBREAD()
        {
            //instanciate DBGateway object
            DBGateway = new SITSqlServerGateway();
            breadTransactionHandler = null;
        }

        /// <summary>
        ///     Creates a new BaseBREAD object
        /// </summary>
        protected BaseSITBREAD(SITSqlServerGateway dbGateway) : base(dbGateway)
        {
            breadTransactionHandler = null;
        }

        /// <summary>
        ///     Creates a new BaseBREAD object by reading connection string from SIT services
        /// </summary>
        /// <param name="breadTransactionHandler">A BreadTransaction object that will be used by methods in order to participate to an external transaction involving SIT BREADs</param>
        protected BaseSITBREAD(BreadTransaction breadTransactionHandler) : this()
        {
            BreadTransactionHandler = breadTransactionHandler;
        }

        #endregion

        #region public/inheritable methods

        /// <summary>
        ///     Executes a non query command taking care of transactional behavior if needed (i.e. breadTransactionHandler is not null)
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="returnValue">A SITCAB.DataSource.Libraries.ReturnValue object providing information about the number of rows returned by the Sql command in case of non transactional behavior or the outcome of enlisting the command into an existing BREAD transaction</param>
        /// <returns>The number of row returned by the SQL statement in case of non transactional behavior, one in case of transactional behavior and successfull execution, zero otherwise</returns>
        public int ExecuteNonQueryCommand(SqlCommand command, out ReturnValue returnValue)
        {
            if (breadTransactionHandler == null)
            {
                returnValue = new ReturnValue() { message = string.Empty, numcode = DBGateway.ExecuteNonQueryCommand(command), succeeded = true };
                return (int)returnValue.numcode;
            }
            else
                return DBGateway.ExecuteNonQueryCommand(command, breadTransactionHandler, out returnValue);
        }

        #endregion
    }
}
