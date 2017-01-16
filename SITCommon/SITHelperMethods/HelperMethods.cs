using System;
using System.Collections.Generic;
using SITFndLEAN.Registration;
using SITCAB.StorageConnectionProvider;

namespace SIT
{
    /// <summary>
    ///     An enumeration of natively supported SIT data sources 
    /// </summary>
    public enum CABDataSources
    {
        /// <summary>
        ///     Simatic IT production database
        /// </summary>
        SitMesDB,

        /// <summary>
        ///     Simatic IT CAB Portal database
        /// </summary>
        PortalDB,

        /// <summary>
        ///     Simatic IT logbook database
        /// </summary>
        LogBook
    }

    /// <summary>
    ///     A set of public SIT related helper methods
    /// </summary>
    public class HelperMethods
    {
        private static DbCnf dbConf;
        private static Dictionary<CABDataSources, string> connectionStrings = new Dictionary<CABDataSources, string>();

        private static DbCnf SetDbConf()
        {
            if (dbConf == null)
                dbConf = new DbCnf();

            return dbConf;
        }

        private static string GetConnectionStringUsingDbCnf(CABDataSources dataSourceType)
        {
            string toRet;
            string dataSourceTypeKeyName = string.Empty;

            switch (dataSourceType)
            {
                case CABDataSources.SitMesDB:
                    {
                        dataSourceTypeKeyName = "SITMESDB";
                        break;
                    }

                case CABDataSources.PortalDB:
                    {
                        dataSourceTypeKeyName = "CABPORTALDB";
                        break;
                    }

                case CABDataSources.LogBook:
                    {
                        dataSourceTypeKeyName = "LOGBOOKDB";
                        break;
                    }
            }

            SetDbConf();

            DbCnfRecord dbConfRecord = null;
            string password = string.Empty;

            DbCnfRetVal retVal = dbConf.ReadConfigurationRecord(dataSourceTypeKeyName, ref dbConfRecord);
            DbCnfRetVal passwordRetVal = dbConfRecord.GetPassword(false, ref password);

            if ((retVal == DbCnfRetVal.DbCnf_Success) && (passwordRetVal == DbCnfRetVal.DbCnf_Success))
            {
                //instanciate DBGateway object
                toRet =
                    string.Format(
                        "data source={0};Initial Catalog={1};User ID={2};Password={3}",
                        dbConfRecord.m_SQLName,
                        dbConfRecord.m_CatalogName,
                        dbConfRecord.m_User,
                        password);

                dbConfRecord.ForceDisposeHandle();
                dbConfRecord = null;
            }
            else
            {
                dbConfRecord.ForceDisposeHandle();
                dbConfRecord = null;

                throw new Exception("Cannot connect to MOSC functionalities");
            }

            return toRet;
        }

        /// <summary>
        ///     Gets the connection string to a SIT DB type
        /// </summary>
        /// <param name="dataSourceType">The target SIT DB type</param>
        /// <returns>The connection string to the DB type indicated by dataSourceType</returns>
        public static string GetConnectionString(CABDataSources dataSourceType)
        {            
            if (connectionStrings.ContainsKey(dataSourceType) && !string.IsNullOrEmpty(connectionStrings[dataSourceType]))
                return connectionStrings[dataSourceType];

            //not yet in application? or not in HTTP context?
            string connectionStringFound = string.Empty;

            //first try using CABStorageConnectionService.GetSITMESConnectionString()
            try
            {
                switch (dataSourceType)
                {
                    case CABDataSources.SitMesDB:
                        {
                            connectionStringFound = CABStorageConnectionService.GetSITMESConnectionString();
                            break;
                        }

                    case CABDataSources.PortalDB:
                        {
                            connectionStringFound = CABStorageConnectionService.GetCABPortalConnectionString();
                            break;
                        }

                    case CABDataSources.LogBook:
                        {
                            connectionStringFound = CABStorageConnectionService.GetLogbookConnectionString();
                            break;
                        }
                }
            }
            catch (CABStorageConnectionProviderException)
            {
                //if failing try using DbCnf
                connectionStringFound = GetConnectionStringUsingDbCnf(dataSourceType);
            }

            //save connection string, so to avoid to instanciate all SITFndLEAN.Registration objects
            connectionStrings[dataSourceType] = connectionStringFound;

            return connectionStringFound;
        }
    }

    /// <summary>
    ///     A set of public ILD-MAL related helper methods
    /// </summary>
    public class MALHelperMethods
    {
        #region date and time management

        /// <summary>
        ///     Formats a DateTime as string, the way Mechatronics messages need
        /// </summary>
        /// <param name="dateTimeToFormat">The date to format</param>
        /// <returns>A string representation of dateTimeToFormat which is suitable for mechatronics messages</returns>
        public static string FormatDateTimeForMechatronicsMessages(DateTime dateTimeToFormat)
        {
            return dateTimeToFormat.ToString("yyyy\\-MM\\-dd HH\\:mm\\:ss");
        }

        #endregion
    }
}
