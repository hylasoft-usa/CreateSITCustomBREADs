#region Using directives

using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Web;

#endregion

namespace System.CommonPlatform.Utils
{
    /// <summary>
    ///     A set of static method/properties providing informations about current running environment
    /// </summary>
    public class HttpEnvironment
    {
        #region Application name

        /// <summary>
        ///     Getter method to retrieve current application's name.
        ///     In case of web application, it is based on the application's virtual directory name
        ///     (i.e. it will return the name of the virtual directory where the application lies)
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string appPath = HttpContext.Current.Request.ApplicationPath;
                    return appPath.Substring(1); //removes first occurence of '/' character
                }

                return Process.GetCurrentProcess().ProcessName;
            }
        }

        #endregion

        #region Remote machine name

        /// <summary>
        ///     Getter method to retrieve the name of the machine the current web request is coming from
        ///     or the current process is running on
        /// </summary>
        /// <remarks>Returned machine name is always lowercase</remarks>
        public static string MachineName
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    try
                    {
                        if ((HttpContext.Current.Application["DNSIsWorking"] == null) || ((bool)HttpContext.Current.Application["DNSIsWorking"]))
                        {
                            string toRet = Dns.GetHostEntry(MachineIP).HostName.ToLower();
                            HttpContext.Current.Application["DNSIsWorking"] = true;

                            return toRet;
                        }

                        return "Unable to resolve remote machine name using DNS services";
                    }
                    catch
                    {
                        const string toRet = "Unable to resolve remote machine name using DNS services";
                        HttpContext.Current.Application["DNSIsWorking"] = false;

                        return toRet;
                    }
                }

                return Process.GetCurrentProcess().MachineName.ToLower();
            }
        }

        #endregion

        #region Remote machine IP

        /// <summary>
        ///     Getter method to retrieve the IP address of the machine the current web request is coming from 
        ///     or the current process is running on
        /// </summary>
        public static IPAddress MachineIP
        {
            get
            {
                if (HttpContext.Current != null)
                    return IPAddress.Parse(HttpContext.Current.Request.UserHostAddress);

                try
                {
                    if (string.Compare(MachineName, ".", StringComparison.OrdinalIgnoreCase) == 0)
                        return IPAddress.Parse("127.0.0.1");

                    return Dns.GetHostEntry(MachineName).AddressList[0];
                }
                catch
                {
                    return IPAddress.None;
                }
            }
        }

        #endregion

        #region Page or Webservice URL, name, base path

        /// <summary>
        ///     In case of web request, this getter method retrieves the URL of the requesting page or Webservice,
        ///     empty string otherwise
        /// </summary>
        public static string PageOrWebserviceURL
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.FilePath;

                return string.Empty;
            }
        }

        /// <summary>
        ///     In case of web request, this getter method retrieves the name of the requesting page or webservice 
        ///     without path, empty string otherwise
        /// </summary>
        public static string PageOrWebserviceName
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string[] URLSegments = HttpContext.Current.Request.Url.Segments;
                    return URLSegments[URLSegments.Length - 1];
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     In case of web request, this getter method retrieves the base URL (i.e. without page or WS name)
        ///     of the requesting page or webservice, empty string otherwise
        /// </summary>
        public static string PageOrWebserviceURLBasePath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string[] URLSegments = HttpContext.Current.Request.Url.Segments;
                    string toRet = string.Empty;

                    for (int i = 0; i < URLSegments.Length - 1; i++)
                        toRet += URLSegments[i];

                    return toRet;
                }

                return string.Empty;
            }
        }

        #endregion

        #region Currently logged user name

        /// <summary>
        ///     In case of a CABPortal application, this getter method retrieved the name of the user currently logged in, 
        ///     empty string otherwise
        /// </summary>
        public static string CurrentUser
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Profile.IsAnonymous ? "Anonymous user" : HttpContext.Current.Profile.UserName;

                return string.Empty;
            }
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