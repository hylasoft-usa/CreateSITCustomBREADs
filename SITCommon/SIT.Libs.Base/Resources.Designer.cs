﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIT.Libs.Base {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SIT.Libs.Base.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to document.attachEvent(&apos;oncontextmenu&apos;, function BlockContextMenu(){ return false; });.
        /// </summary>
        internal static string BlockContextMenuScript {
            get {
                return ResourceManager.GetString("BlockContextMenuScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var pingTime_BasePage = PingTimePlaceHolder;//to be set by CABBaseContentPage on server side
        ///var pingTimerId_BasePage;
        ///var resourceToRequest_BasePage = &apos;ResourceToRequestPlaceHolder&apos;;//to be set by CabcommonComp.Web.CABBaseContentPage on server side, supposed to be a page URL
        ///var pingReq_BasePage = new ActiveXObject(&apos;Microsoft.XMLHTTP&apos;);
        ///
        ///function ping_BasePage()
        ///{
        ///    pingReq_BasePage.abort();
        ///    pingReq_BasePage.open(&apos;POST&apos;, resourceToRequest_BasePage + &apos;?keepsessionalive=true&amp;sid=&apos; + Math.random [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string KeepSessionAliveScript {
            get {
                return ResourceManager.GetString("KeepSessionAliveScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to //function that prevents keyboard combination
        ///function KillKeyboardShortcuts_BasePage()
        ///{
        ///	//ALT, CTRL combinations
        ///	if((window.event.ctrlKey) || (window.event.altKey))
        ///	{
        ///		//Allow CTRL+C, CTRL+V, CTRL+X, CTRL+A
        ///		if((window.event.keyCode != 67) &amp;&amp; (window.event.keyCode != 86) &amp;&amp; (window.event.keyCode != 88) &amp;&amp; (window.event.keyCode != 65))
        ///		{
        ///			try{window.event.keyCode = 0;} catch(ex){}
        ///			window.event.cancelBubble = true;
        ///			window.event.returnValue = false;
        ///		}
        ///	}
        ///	//Block TAB or F1 - F1 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string KillKeyboardShortcutsScript {
            get {
                return ResourceManager.GetString("KillKeyboardShortcutsScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var CheckSqlServerConnection_ResourceToRequest = &apos;ResourceToRequestPlaceHolder&apos;;//to be set by CABBaseContentPage on server side, supposed to be a page URL
        ///var CheckSqlServerConnectionReq_BasePage = new ActiveXObject(&apos;Microsoft.XMLHTTP&apos;);
        ///
        ///function CheckSqlServerConnection_BasePage(HandlerToCall)
        ///{
        ///    CheckSqlServerConnectionReq_BasePage.abort();
        ///    CheckSqlServerConnectionReq_BasePage.open(&apos;POST&apos;, CheckSqlServerConnection_ResourceToRequest + &quot;?sid=&quot; + Math.random() + &quot;&amp;checkingsqlserveravailability [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ManageObscuringLayerScriptSqlServer {
            get {
                return ResourceManager.GetString("ManageObscuringLayerScriptSqlServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var CheckWebServerConnection_ResourceToRequest = &apos;ResourceToRequestPlaceHolder&apos;;//to be set by CABBaseContentPage on server side, supposed to be a page URL
        ///var CheckWebServerConnectionReq_BasePage = new ActiveXObject(&apos;Microsoft.XMLHTTP&apos;);
        ///
        ///function CheckWebServerConnection_BasePage(HandlerToCall)
        ///{
        ///    CheckWebServerConnectionReq_BasePage.abort();
        ///    CheckWebServerConnectionReq_BasePage.open(&apos;POST&apos;, CheckWebServerConnection_ResourceToRequest + &quot;?sid=&quot; + Math.random() + &quot;&amp;checkingwebserveravailability [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ManageObscuringLayerScriptWebServer {
            get {
                return ResourceManager.GetString("ManageObscuringLayerScriptWebServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var CheckWebServerConnectionAutoRefresh_ResourceToRequest = &apos;ResourceToRequestPlaceHolder&apos;;//to be set by CABBaseContentPage on server side, supposed to be a page URL
        ///var CheckWebServerConnectionReqAutoRefresh_BasePage = new ActiveXObject(&apos;Microsoft.XMLHTTP&apos;);
        ///
        ///function CheckWebServerConnectionAutoRefresh_BasePage(HandlerToCall)
        ///{
        ///    CheckWebServerConnectionReqAutoRefresh_BasePage.abort();
        ///    CheckWebServerConnectionReqAutoRefresh_BasePage.open(&apos;POST&apos;, CheckWebServerConnectionAutoRefresh_ResourceToR [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SubmitPageAfterScript {
            get {
                return ResourceManager.GetString("SubmitPageAfterScript", resourceCulture);
            }
        }
    }
}
