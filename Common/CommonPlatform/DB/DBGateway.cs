using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using System.Web;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.CommonPlatform;
using System.Xml;
using System.Data.SqlTypes;

namespace System.CommonPlatform.DB
{
    /// <summary>
    ///     Options to execute a batch of SQL commands
    /// </summary>
    public enum BatchExecutionOptions
    {
        /// <summary>
        ///     Execute SQL commands into a single transaction (exploits System.Transactions.TransactionScope) that will be committed if all SQL commands succeed, rolled back otherwise
        /// </summary>
        Transactional,

        /// <summary>
        ///     Executes SQL commands one by one and stop as soon as one fails
        /// </summary>
        NonTransactionalStopWhenFailure,

        /// <summary>
        ///     Executes SQL commands one by one and ignore failures
        /// </summary>
        NonTrasactionalIgnoreFailure
    }

    /// <summary>
    ///     A base class for bridges from CAB Web Applications databases
    /// </summary>
    public abstract class DBGateway
    {
        #region constructors

        /// <summary>
        ///     Creates a new DBGateway object pointing to a custom database
        /// </summary>
        /// <param name="connectionString">The connection string for the target database</param>
        public DBGateway(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        ///     Creates a new DBGateway object pointing to a custom database
        /// </summary>
        /// <param name="connectionString">The connection string for the target database</param>
        /// <param name="queryTimeout">The default query timeout</param>
        public DBGateway(string connectionString, int queryTimeout)
        {
            ConnectionString = connectionString;
            QueryTimeout = queryTimeout;
        }

        #endregion

        #region getting SQL commands

        /// <summary>
        ///     Creates a select command using a supplied query text
        /// </summary>
        /// <param name="queryText">SQL query text</param>
        /// <returns>The corresponding DbCommand ready to be invoked against the target database</returns>
        public abstract DbCommand GetSqlCommand(string queryText);

        /// <summary>
        ///     Returns a DbCommand without CommandText ready to be executed against the target database
        /// </summary>
        /// <returns>An empty DbCommand</returns>
        public abstract DbCommand GetSqlCommand();

        /// <summary>
        ///     Gets a valid DbCommand to execute a stored procedure
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to be executed</param>
        /// <returns>A DbCommand to execute a stored procedure ready to be invoked against the target database</returns>
        public abstract DbCommand GetSqlCommandForStoredProcedure(string storedProcedureName);
        
        /// <summary>
        ///     Adds a parameter to the end of the parameters collection
        /// </summary>
        /// <param name="command">The command whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterType">The type of the parameter</param>
        /// <param name="parameterDirection">Parameter's direction</param>
        /// <returns>The parameter added</returns>
        public abstract DbParameter AddParameter(DbCommand command, string parameterName, object parameterType, ParameterDirection parameterDirection);

        /// <summary>
        ///     Adds a value to the end of the parameters collection using direction INPUT
        /// </summary>
        /// <param name="command">The command whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterValue">The value to be added</param>
        /// <returns>The parameter added</returns>
        public abstract DbParameter AddParameterWithValue(DbCommand command, string parameterName, object parameterValue);

        #endregion

        #region executing SQL commands

        #region select commands

        /// <summary>
        ///     Opens a connection toward the target database, executes a select statement then closes the connection
        /// </summary>
        /// <param name="command">The DbCommand to be executed against the target database</param>
        /// <returns>The DataSet representing the result of the sql command</returns>
        public abstract DataSet ExecuteSelectCommand(DbCommand command);

        /// <summary>
        ///     Opens a connection toward the target database, executes a select statement then closes the connection
        /// </summary>
        /// <param name="commandText">The text of the command to be executed against the target database</param>
        /// <returns>The DataSet representing the result of the sql command</returns>
        public abstract DataSet ExecuteSelectCommand(string commandText);

        /// <summary>
        ///     Executes a SQL statement and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="command">The DbCommand to be executed against the target database</param>
        /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty</returns>
        public abstract object ExecuteScalarQuery(DbCommand command);

        /// <summary>
        ///     Executes a SQL statement and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="commandText">The text of the command to be executed against the target database</param>
        /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty</returns>
        public abstract object ExecuteScalarQuery(string commandText);

        #endregion

        #region DML commands

        /// <summary>
        ///     Opens a connection toward the target database, executes a non-query SQL statement then closes the connection
        /// </summary>
        /// <param name="command">The DbCommand to be executed against the target database</param>
        /// <returns>The number of row returned by the SQL statement</returns>
        public abstract int ExecuteNonQueryCommand(DbCommand command);

        /// <summary>
        ///     Opens a connection toward the target database, executes a non-query SQL statement then closes the connection
        /// </summary>
        /// <param name="commandText">The text of the command to be executed against the target database</param>
        /// <returns>The number of row returned by the SQL statement</returns>
        public abstract int ExecuteNonQueryCommand(string commandText);

        #endregion

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets timeout for queries
        /// </summary>
        public int QueryTimeout { get; set; }

        /// <summary>
        ///     Gets or sets the connection string to the target database
        /// </summary>
        public string ConnectionString {get; set;}

        #endregion
    }

    /// <summary>
    ///     A bridge from aplications to SQL Server database
    /// </summary>
    public class SqlServerGateway : DBGateway
    {
        #region constructors

        /// <summary>
        ///     Creates a new SqlServerGateway object pointing to a custom database
        /// </summary>
        /// <param name="connectionString">The connection string for the target database</param>
        public SqlServerGateway(string connectionString) : base(connectionString) { }
        
        /// <summary>
        ///     Creates a new SqlServerGateway object pointing to a custom database
        /// </summary>
        /// <param name="connectionString">The connection string for the target database</param>
        /// <param name="queryTimeout">The default timeout for queries</param>
        public SqlServerGateway(string connectionString, int queryTimeout) : base(connectionString, queryTimeout) { }

        #endregion

        #region getting SQL commands

        /// <summary>
        ///     Creates a select command using a supplied query text
        /// </summary>
        /// <param name="queryText">SQL query text</param>
        /// <returns>The corresponding System.Data.SqlClent.SqlCommand ready to be invoked against the target database</returns>
        public override DbCommand GetSqlCommand(string queryText)
        {
            return new SqlCommand(queryText, new SqlConnection(ConnectionString));
        }

        /// <summary>
        ///     Returns a System.Data.SqlClent.SqlCommand without CommandText ready to be executed against the target database; the timeout specified by QueryTimeout property will be used.
        /// </summary>
        /// <returns>An empty System.Data.SqlClent.SqlCommand</returns>
        public override DbCommand GetSqlCommand()
        {
            return GetSqlCommand(string.Empty);
        }

        /// <summary>
        ///     Gets a valid System.Data.SqlClient.SqlCommand to execute a stored procedure
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to be executed</param>
        /// <returns>A System.Data.SqlClent.SqlCommand to execute a stored procedure ready to be invoked against the target database</returns>
        public override DbCommand GetSqlCommandForStoredProcedure(string storedProcedureName)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = new SqlConnection(ConnectionString);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            command.CommandTimeout = QueryTimeout;

            return command;
        }

        /// <summary>
        ///     Adds a parameter to the end of the parameters collection
        /// </summary>
        /// <param name="command">The command whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterType">The type of the parameter</param>
        /// <param name="parameterDirection">Parameter's direction</param>
        /// <returns>The parameter added</returns>
        public override DbParameter AddParameter(DbCommand command, string parameterName, object parameterType, ParameterDirection parameterDirection)
        {
            if (command is SqlCommand)
            {
                if (parameterType is SqlDbType)
                    return AddParameter((SqlCommand)command, parameterName, (SqlDbType)parameterType, parameterDirection);
                else
                    throw new ArgumentException("SqlServerGateway.AddParameterWithValue parameterType's type must be SqlDbType");
            }
            else
                throw new ArgumentException("SqlServerGateway.AddParameterWithValue command's type must be SqlCommand");
        }

        /// <summary>
        ///     Adds a parameter to the end of the parameters collection
        /// </summary>
        /// <param name="command">The command whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterType">The type of the parameter</param>
        /// <param name="parameterDirection">Parameter's direction</param>
        /// <returns>The parameter added</returns>
        public virtual SqlParameter AddParameter(SqlCommand command, string parameterName, SqlDbType parameterType, ParameterDirection parameterDirection)
        {
            return (parameterType == SqlDbType.NVarChar) ?
                command.Parameters.Add(new SqlParameter(parameterName, parameterType) { Direction = parameterDirection, Size = 4000 }) :
                command.Parameters.Add(new SqlParameter(parameterName, parameterType) { Direction = parameterDirection });
        }
        
        /// <summary>
        ///     Adds an input parameter to the end of the parameters collection
        /// </summary>
        /// <param name="command">The command whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterType">The type of the parameter</param>
        /// <returns>The parameter added</returns>
        public virtual SqlParameter AddParameter(SqlCommand command, string parameterName, SqlDbType parameterType)
        {
            return AddParameter(command, parameterName, parameterType, ParameterDirection.Input);
        }

        /// <summary>
        ///     Adds a value to the end of the parameters collection
        /// </summary>
        /// <param name="command">The System.Data.SqlClient.SqlCommand whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterValue">The value to be added</param>
        /// <returns>The System.Data.SqlClient.SqlParameter added</returns>
        public override DbParameter AddParameterWithValue(DbCommand command, string parameterName, object parameterValue)
        {
            if (command is SqlCommand)
                return AddParameterWithValue((SqlCommand)command, parameterName, parameterValue);
            else
                throw new ArgumentException("SqlServerGateway.AddParameterWithValue command's type must be SqlCommand");
        }

        /// <summary>
        ///     Adds a value to the end of the parameters collection
        /// </summary>
        /// <param name="command">The System.Data.SqlClient.SqlCommand whose parameter collection has to be updated</param>
        /// <param name="parameterName">The name of the parameter <b>without any prefix</b></param>
        /// <param name="parameterValue">The value to be added</param>
        /// <returns>The System.Data.SqlClient.SqlParameter added</returns>
        public virtual SqlParameter AddParameterWithValue(SqlCommand command, string parameterName, object parameterValue)
        {
            if (parameterValue is XmlDocument)
            {
                SqlParameter param = new SqlParameter(string.Format("@{0}", parameterName), SqlDbType.Xml);
                param.Value = new SqlXml(new XmlTextReader(((XmlDocument)parameterValue).InnerXml, XmlNodeType.Document, null));
                
                return command.Parameters.Add(param);
            }

            return command.Parameters.AddWithValue(string.Format("@{0}", parameterName), parameterValue);
        }

        #endregion

        #region executing SQL commands

        #region select commands

        /// <summary>
        ///     Opens a connection toward the target database, executes a select statement then closes the connection
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>The System.Data.DataSet representing the result of the sql command</returns>
        /// <exception cref="System.ArgumentException">The supplied command is not a System.Data.SqlClient.SqlCommand</exception>
        public override DataSet ExecuteSelectCommand(DbCommand command)
        {
            if (command is SqlCommand)
                return ExecuteSelectCommand((SqlCommand)command);
            else
                throw new ArgumentException("SqlServerGateway.ExecuteSelectCommand command's type must be SqlCommand");
        }

        /// <summary>
        ///     Opens a connection toward the target database, executes a select statement then closes the connection
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>The System.Data.DataSet representing the result of the sql command</returns>
        public virtual DataSet ExecuteSelectCommand(SqlCommand command)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataSet ds = new DataSet();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (command)
                {
                    sqlConnection.Open();

                    command.Connection = sqlConnection;
                    command.CommandTimeout = QueryTimeout;

                    dataAdapter.Fill(ds);
                }
            }

            return ds;
        }

        /// <summary>
        ///     Opens a connection toward the target database, executes a select statement then closes the connection
        /// </summary>
        /// <param name="commandText">The text of the command to be executed against the target database</param>
        /// <returns>The System.Data.DataSet representing the result of the sql command</returns>
        public override DataSet ExecuteSelectCommand(string commandText)
        {
            DbCommand command = GetSqlCommand(commandText);
            return ExecuteSelectCommand(command);
        }

        /// <summary>
        ///     Executes a SQL statement and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty</returns>
        public virtual object ExecuteScalarQuery(SqlCommand command)
        {
            object toRet;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (command)
                {
                    sqlConnection.Open();
                    command.Connection = sqlConnection;

                    toRet = command.ExecuteScalar();
                }
            }

            return toRet;
        }

        /// <summary>
        ///     Executes a SQL statement and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty</returns>
        public override object ExecuteScalarQuery(DbCommand command)
        {
            if (command is SqlCommand)
                return ExecuteScalarQuery((SqlCommand)command);
            else
                throw new ArgumentException("SqlServerGateway.ExecuteScalarQuery command's type must be SqlCommand");
        }

        /// <summary>
        ///     Executes a SQL statement and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="commandText">The text of the command to be executed against the target database</param>
        /// <returns>The first column of the first row in the result set, or a null reference if the result set is empty</returns>
        public override object ExecuteScalarQuery(string commandText)
        {
            DbCommand command = GetSqlCommand(commandText);
            return ExecuteScalarQuery(command);
        }

        #endregion

        #region DML commands

        /// <summary>
        ///     Opens a connection toward the target database, executes a non-query SQL statement then closes the connection
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>The number of row returned by the SQL statement</returns>
        /// <exception cref="System.ArgumentException">The supplied command is not a System.Data.SqlClient.SqlCommand</exception>
        public override int ExecuteNonQueryCommand(DbCommand command)
        {
            if (command is SqlCommand)
                return ExecuteNonQueryCommand((SqlCommand)command);
            else
                throw new ArgumentException("SqlServerGateway.ExecuteNonQueryCommand command's type must be SqlCommand");
        }

        /// <summary>
        ///     Opens a connection toward the target database, executes a non-query SQL statement then closes the connection
        /// </summary>
        /// <param name="command">The System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>The number of row returned by the SQL statement</returns>
        public virtual int ExecuteNonQueryCommand(SqlCommand command)
        {
            int toRet;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (command)
                {
                    sqlConnection.Open();
                    command.Connection = sqlConnection;

                    toRet = command.ExecuteNonQuery();
                }
            }

            return toRet;
        }
        
        /// <summary>
        ///     Opens a connection toward the target database, executes a non-query SQL statement then closes the connection
        /// </summary>
        /// <param name="commandText">The text of the command to be executed against the target database</param>
        /// <returns>The number of row returned by the SQL statement</returns>
        public override int ExecuteNonQueryCommand(string commandText)
        {
            DbCommand command = GetSqlCommand(commandText);
            return ExecuteNonQueryCommand(command);
        }

        #endregion

        #region command batches and transactions

        /// <summary>
        ///     Executes a batch of SQL commands using a single connection
        /// </summary>
        /// <param name="commandCollection">The collection of System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <param name="ignoreFailures">Whether any single failure in executing an SQL command must be ignored</param>
        /// <returns>An OperationReturnValue describing the outcome of the operation; the execution will fail if ignoreFailures is false and at least one command fails (or if any generic exception is caught)</returns>
        private OperationReturnValue ExecuteBatch(IEnumerable<SqlCommand> commandCollection, bool ignoreFailures)
        {
            OperationReturnValue toRet = new OperationReturnValue();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    foreach (SqlCommand sqlc in commandCollection)
                    {
                        sqlc.Connection = sqlConnection;

                        try
                        {
                            using(sqlc) sqlc.ExecuteNonQuery();
                        }
                        catch (SqlException sqlex)
                        {
                            if (!ignoreFailures)
                            {
                                toRet = new OperationReturnValue(sqlex.ErrorCode, ReturnCodes.ERROR998, sqlex.Message, sqlex.StackTrace);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                toRet = new OperationReturnValue(-999, ReturnCodes.ERROR999, ex.Message, ex.StackTrace);
            }

            return toRet;
        }

        private OperationReturnValue ExecuteBatch(IEnumerable<string> commandTextCollection, bool ignoreFailures)
        {
            Collection<SqlCommand> commands = new Collection<SqlCommand>();

            foreach (string commandText in commandTextCollection)
                commands.Add((SqlCommand)GetSqlCommand(commandText));

            return ExecuteBatch(commands, ignoreFailures);
        }

        /// <summary>
        ///     Executes a batch of SQL commands using a single connection
        /// </summary>
        /// <param name="commandCollection">The collection of System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <param name="batchExecutionOption">How the batch must be executed</param>
        /// <returns>An OperationReturnValue describing the outcome of the operation; the execution will fail if batchExecutionOption is either Transactional or NonTransactionalStopWhenFailure and at least one SQL commands fails</returns>
        public virtual OperationReturnValue ExecuteBatch(IEnumerable<SqlCommand> commandCollection, BatchExecutionOptions batchExecutionOption)
        {
            OperationReturnValue toRet = new  OperationReturnValue();

            if (batchExecutionOption == BatchExecutionOptions.Transactional)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    toRet = ExecuteBatch(commandCollection, false);

                    if (toRet.Succeeded)
                        scope.Complete();
                }
            }
            else
                toRet = ExecuteBatch(commandCollection, batchExecutionOption == BatchExecutionOptions.NonTrasactionalIgnoreFailure);

            return toRet;
        }

        /// <summary>
        ///     Executes a batch of SQL commands using a single connection
        /// </summary>
        /// <param name="commandTextCollection">The collection of command texts to be executed against the target database</param>
        /// <param name="batchExecutionOption">How the batch must be executed</param>
        /// <returns>An OperationReturnValue describing the outcome of the operation; the execution will fail if batchExecutionOption is either Transactional or NonTransactionalStopWhenFailure and at least one SQL commands fails</returns>
        public virtual OperationReturnValue ExecuteBatch(IEnumerable<string> commandTextCollection, BatchExecutionOptions batchExecutionOption)
        {
            Collection<SqlCommand> commands = new Collection<SqlCommand>();

            foreach (string commandText in commandTextCollection)
                commands.Add((SqlCommand)GetSqlCommand(commandText));

            return ExecuteBatch(commands, batchExecutionOption);
        }

        /// <summary>
        ///     Executes a set of SQL commands into a single SQL transaction (exploits System.Transactions.TransactionScope)
        /// </summary>
        /// <param name="commandCollection">The collection of System.Data.SqlClent.SqlCommand to be executed against the target database</param>
        /// <returns>An OperationReturnValue describing the outcome of the operation; the execution will fail if at least one SQL commands fails and the transaction is therefore rolled back</returns>
        public virtual OperationReturnValue ExecuteTransaction(IEnumerable<SqlCommand> commandCollection)
        {
            return ExecuteBatch(commandCollection, BatchExecutionOptions.Transactional);
        }

        /// <summary>
        ///     Executes a set of SQL statements into a single SQL transaction (exploits System.Transactions.TransactionScope)
        /// </summary>
        /// <param name="commandTextCollection">The collection of command texts to be executed against the target database</param>
        /// <returns>An OperationReturnValue describing the outcome of the operation; the execution will fail if at least one SQL commands fails and the transaction is therefore rolled back</returns>
        public virtual OperationReturnValue ExecuteTransaction(IEnumerable<string> commandTextCollection)
        {
            Collection<SqlCommand> commands = new Collection<SqlCommand>();

            foreach (string commandText in commandTextCollection)
                commands.Add((SqlCommand)GetSqlCommand(commandText));

            return ExecuteTransaction(commands);
        }

        #endregion

        #endregion

        /// <summary>
        ///     Converts a SQL Server type to a CLR type
        /// </summary>
        /// <param name="sqlType">The SQL Server type to convert</param>
        /// <returns>The CLR equivalent type</returns>
        public static Type GetClrType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal);

                case SqlDbType.Float:
                    return typeof(double);

                case SqlDbType.Int:
                    return typeof(int);

                case SqlDbType.Real:
                    return typeof(float);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);

                case SqlDbType.SmallInt:
                    return typeof(short);

                case SqlDbType.TinyInt:
                    return typeof(byte);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }
    }
}