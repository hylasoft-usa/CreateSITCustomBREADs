using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using SITCAB.StorageConnectionProvider;
using System.Web;
using SITFndLEAN.Registration;
using System.Collections.ObjectModel;
using Siemens.SimaticIT.MES.Breads;
using SITCAB.DataSource.Libraries;
using System.Data.Common;
using System.CommonPlatform;
using System.CommonPlatform.DB;

namespace SIT.Libs.Base.DB
{
    /// <summary>
    ///     A bridge from SIT aplications to SQL Server database
    /// </summary>
    public class SITSqlServerGateway : SqlServerGateway
    {
        #region constructors

        /// <summary>
        ///     Creates a SITSqlServerGateway DBGateway object pointing to SitMesDB (exploits SIT connection services APIs)
        /// </summary>
        public SITSqlServerGateway() : this(SIT.CABDataSources.SitMesDB) { }        

        /// <summary>
        ///     Creates a new SITSqlServerGateway object pointing to a custom database
        /// </summary>
        /// <param name="connectionString">The connection string for the target database</param>
        public SITSqlServerGateway(string connectionString) : base(connectionString) { }
        
        /// <summary>
        ///     Creates a new SITSqlServerGateway object pointing to a custom database
        /// </summary>
        /// <param name="connectionString">The connection string for the target database</param>
        /// <param name="queryTimeout">The default timeout for queries</param>
        public SITSqlServerGateway(string connectionString, int queryTimeout) : base(connectionString, queryTimeout) { }
        
        /// <summary>
        ///     Creates a new SITSqlServerGateway object pointing to a SIT database
        /// </summary>
        /// <param name="dataSource">The target database</param>
        public SITSqlServerGateway(SIT.CABDataSources dataSource) : base(string.Empty)
        {
            ConnectionString = SIT.HelperMethods.GetConnectionString(dataSource);
        }        

        #endregion

        #region executing SQL commands

        #region DML commands

        /// <summary>
        ///     Executes a non-query SQL statement as part of an external transaction involving SIT BREADs
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <param name="breadTransactionHandler">The Siemens.SimaticIT.MES.Breads.BreadTransaction object carrying the external transaction over</param>
        /// <param name="returnValue">A SITCAB.DataSource.Libraries.ReturnValue object providing information about the outcome of enlisting the command into an existing BREAD transaction</param>
        /// <returns>One in case of successfull execution, zero otherwise</returns>
        public virtual int ExecuteNonQueryCommand(SqlCommand command, BreadTransaction breadTransactionHandler, out ReturnValue returnValue)
        {
            return ExecuteNonQueryCommand(System.HelperMethods.GetCommandText(command), breadTransactionHandler, out returnValue);
        }

        /// <summary>
        ///     Executes a non-query SQL statement as part of an external transaction involving SIT BREADs
        /// </summary>
        /// <param name="commandText">The SQL statement to be executed against the target database</param>
        /// <param name="breadTransactionHandler">The Siemens.SimaticIT.MES.Breads.BreadTransaction object carrying the external transaction over</param>
        /// <param name="returnValue">A SITCAB.DataSource.Libraries.ReturnValue object providing information about the outcome of enlisting the command into an existing BREAD transaction</param>
        /// <returns>One in case of successfull execution, zero otherwise</returns>
        public virtual int ExecuteNonQueryCommand(string commandText, BreadTransaction breadTransactionHandler, out ReturnValue returnValue)
        {
            returnValue = breadTransactionHandler.EnlistCommand(commandText);
            return returnValue.succeeded ? 1 : 0;
        }

        #endregion

        #region command batches and transactions

        /// <summary>
        ///     Executes a set of SQL commands into a single BreadTransaction
        /// </summary>
        /// <param name="commandCollection">The collection of System.Data.SqlClent.SqlCommand to be executed</param>
        /// <param name="breadTransactionHandler">The Siemens.SimaticIT.MES.Breads.BreadTransaction object carrying the external transaction over</param>
        /// <returns>A ReturnValue describing the outcome of the operation; the execution will fail if at least one SQL commands fails and the transaction is therefore rolled back</returns>
        public virtual ReturnValue ExecuteTransaction(IEnumerable<SqlCommand> commandCollection, BreadTransaction breadTransactionHandler)
        {
            ReturnValue toRet = new ReturnValue() { message = string.Empty, numcode = 0, succeeded = true };

            foreach (SqlCommand command in commandCollection)
            {
                ExecuteNonQueryCommand(command, breadTransactionHandler, out toRet);
                if (!toRet.succeeded)
                    break;
            }

            return toRet;
        }

        /// <summary>
        ///     Executes a set of SQL statements into a single BreadTransaction
        /// </summary>
        /// <param name="commandTextCollection">The collection of statements to be executed</param>
        /// <param name="breadTransactionHandler">The Siemens.SimaticIT.MES.Breads.BreadTransaction object carrying the external transaction over</param>
        /// <returns>A ReturnValue describing the outcome of the operation; the execution will fail if at least one SQL commands fails and the transaction is therefore rolled back</returns>
        public virtual ReturnValue ExecuteTransaction(IEnumerable<string> commandTextCollection, BreadTransaction breadTransactionHandler)
        {
            ReturnValue toRet = new ReturnValue() { message = string.Empty, numcode = 0, succeeded = true };

            foreach (string commandText in commandTextCollection)
            {
                ExecuteNonQueryCommand(commandText, breadTransactionHandler, out toRet);
                if (toRet.succeeded)
                    break;
            }

            return toRet;
        }

        #endregion

        #endregion
    }
}