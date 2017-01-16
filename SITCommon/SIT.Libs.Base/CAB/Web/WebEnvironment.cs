using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace SIT.Libs.Base.CAB.Utils
{
    /// <summary>
    ///     A set of static method/properties providing informations about current  running web environment
    /// </summary>
    public class WebEnvironment
    {
        #region Application name

        /// <summary>
        ///     Getter method to retrieve current application's name. It is based on the application's virtual directory name (i.e. it will return the name of the virtual directory where the application lies)
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
                    return appPath.Substring(1);//removes first occurence of '/' character
                }
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }

        #endregion

        #region Remote machine name

        /// <summary>
        ///     Getter method to retrieve the name of the machine current web request is coming from
        /// </summary>
        /// <remarks>Returned machine name is always lowercase</remarks>
        public static string RemoteMachineName
        {
            get
            {
                try
                {
                    if (HttpContext.Current != null)
                        return Dns.GetHostEntry(RemoteMachineIP).HostName.ToLower();
                    else
                        throw new Exceptions.Web.NotAWebApplicationException();
                }
                catch
                {
                    return "Unable to resolve remote machine name";
                }
            }
        }

        #endregion

        #region Remote machine IP

        /// <summary>
        ///     Getter method to retrieve the IP of the machine current web request is coming from
        /// </summary>
        public static IPAddress RemoteMachineIP
        {
            get
            {
                if (HttpContext.Current != null)
                    return IPAddress.Parse(HttpContext.Current.Request.UserHostAddress);
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }

        #endregion

        #region Page or Webservice URL, name, base path

        /// <summary>
        ///     Getter method to retrieve the URL of the page or Webservice which performed the current web request
        /// </summary>
        public static string PageOrWebserviceURL
        {
            get
            {
                if (HttpContext.Current != null)
                    return System.Web.HttpContext.Current.Request.FilePath;
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }

        /// <summary>
        ///     Getter method to retrieve page or webservice name without path
        /// </summary>
        public static string PageOrWebserviceName
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string[] URLSegments = System.Web.HttpContext.Current.Request.Url.Segments;
                    return URLSegments[URLSegments.Length - 1];
                }
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }
        
        /// <summary>
        /// Getter method to retrieve the base URL (i.e. without page or WS name) of the page or
        ///     Webservice which performed the current web request
        /// </summary>
        public static string PageOrWebserviceURLBasePath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string[] URLSegments = System.Web.HttpContext.Current.Request.Url.Segments;
                    string toRet = string.Empty;

                    for (int i = 0; i < URLSegments.Length - 1; i++)
                        toRet += URLSegments[i];

                    return toRet;
                }
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }

        #endregion        

        #region A unique ID for a single web page based on remote machine's IP and page URL

        /// <summary>
        ///     Getter method to retrieve an unique ID for the page which performed the current web request; it actually returns a string made up like RemoteMachineIP_PageURL
        /// </summary>
        public static string RemotePageUniqueID
        {
            get
            {
                if (HttpContext.Current != null)
                    return RemoteMachineIP + "_" + PageOrWebserviceURL;
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }

        #endregion

        #region Web request coming from localhost?

        /// <summary>
        ///     Returns true if the requesting machine's ip address corresponds to at least one belonging to web server
        /// </summary>
        public static bool RemoteAddressIsLocalMachineOne
        {
            get
            {
                string remoteMachineIP = RemoteMachineIP.ToString();

                if (remoteMachineIP == "127.0.0.1") 
                    return true;
                else
                {
                    IPAddress[] LocalMachineAddresses = Dns.GetHostAddresses(Dns.GetHostName());

                    for (int i = 0; i < LocalMachineAddresses.Length; i++)
                        if (remoteMachineIP == LocalMachineAddresses[i].ToString())
                            return true;

                    return false;
                }
            }
        }

        #endregion

        #region Currently logged user name

        /// <summary>
        ///     Getter method to retrieve user currently logged in CABPortal
        /// </summary>
        public static string CurrentUser
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Profile.IsAnonymous ? 
                        "Anonymous user" : HttpContext.Current.Profile.UserName;
                else
                    throw new Exceptions.Web.NotAWebApplicationException();
            }
        }

        #endregion

        #region Change an application setting in configuration file

        /// <summary>
        ///     Updates an application setting in a specific web application configuration file, saving the file to disk. It may require an application restart
        /// </summary>
        /// <param name="ConfigurationFileBasePath">A full path to the configuration file, without configuration file name</param>
        /// <param name="ApplicationSettingKey">The key to be updated</param>
        /// <param name="ApplicationSettingNewValue">The new value for the specified key</param>
        public static void ChangeApplicationSetting(string ConfigurationFileBasePath, string ApplicationSettingKey, string ApplicationSettingNewValue)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(ConfigurationFileBasePath);            
            config.AppSettings.Settings[ApplicationSettingKey].Value =  ApplicationSettingNewValue;
            
            FileInfo f = new FileInfo(config.FilePath);
            bool isReadOnly = f.IsReadOnly;
            
            f.IsReadOnly = false;
            config.Save();
            f.IsReadOnly = isReadOnly;
            
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        ///     Updates an application setting in the root web application configuration file (normally web.config), saving the file to disk. It may require an application restart
        /// </summary>
        /// <param name="ApplicationSettingKey">The key to be updated</param>
        /// <param name="ApplicationSettingNewValue">The new value for the specified key</param>
        public static void ChangeApplicationSetting(string ApplicationSettingKey, string ApplicationSettingNewValue)
        {
            ChangeApplicationSetting(HttpContext.Current.Request.ApplicationPath, ApplicationSettingKey, ApplicationSettingNewValue);
        }

        #endregion

        #region Current culture

        /// <summary>
        ///     Get the culture used by the the currently logged in user
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }

        /// <summary>
        ///     Get the UI culture used by the the currently logged in user
        /// </summary>
        public static CultureInfo CurrentUICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }

        #endregion
    }
}
