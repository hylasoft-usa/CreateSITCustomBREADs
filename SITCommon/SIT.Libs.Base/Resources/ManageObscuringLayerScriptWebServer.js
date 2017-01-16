var CheckWebServerConnection_ResourceToRequest = 'ResourceToRequestPlaceHolder';//to be set by CABBaseContentPage on server side, supposed to be a page URL
var CheckWebServerConnectionReq_BasePage = new ActiveXObject('Microsoft.XMLHTTP');

function CheckWebServerConnection_BasePage(HandlerToCall)
{
    CheckWebServerConnectionReq_BasePage.abort();
    CheckWebServerConnectionReq_BasePage.open('POST', CheckWebServerConnection_ResourceToRequest + "?sid=" + Math.random() + "&checkingwebserveravailability=true", true);
	CheckWebServerConnectionReq_BasePage.onreadystatechange = HandlerToCall;
	CheckWebServerConnectionReq_BasePage.send();
}
 
function IsWebServerRequestReady()
{ 
    try 
    {        
        return (CheckWebServerConnectionReq_BasePage.readyState == 4);
    }
    catch(ex) { return false; }     
}

function IsWebServerConnected()
{ 
    try 
    {        
		//status 200 = OK; status 0 = form submission which should be ignored
		window.status = 'Web server poll response code = ' + CheckWebServerConnectionReq_BasePage.status;
        return (CheckWebServerConnectionReq_BasePage.status == 200 || CheckWebServerConnectionReq_BasePage.status == 0);
    }
    catch(ex) { return false; }     
}

var CheckWebServerAvailabilityAndManageObscuringLayer_Interval;

function startCheckingWebServerAvailability()
{
	CheckWebServerAvailabilityAndManageObscuringLayer_Interval = setInterval('CheckWebServerAvailabilityAndManageObscuringLayer()', 5000);
}

function stopCheckingWebServerAvailability()
{
	clearInterval(CheckWebServerAvailabilityAndManageObscuringLayer_Interval);
}

function CheckWebServerAvailabilityAndManageObscuringLayer()
{	
	CheckWebServerConnection_BasePage(CheckWebServerAvailabilityAndManageObscuringLayer_Internal);
}

function CheckWebServerAvailabilityAndManageObscuringLayer_Internal()
{
	window.status = 'Checking web server availability at: ' + new Date();
	
	stopCheckingWebServerAvailability();
	
	if(IsWebServerRequestReady())
	{
		pnlBasePageObscuringLayerSqlServer = document.getElementById('BasePageObscuringLayerSqlServer');
		pnlBasePageObscuringLayerWebServer = document.getElementById('BasePageObscuringLayerWebServer');
		
		if(IsWebServerConnected())
			pnlBasePageObscuringLayerWebServer.style.display =  'none';
		else
		{	
			if(pnlBasePageObscuringLayerSqlServer != null)
			{
				if(pnlBasePageObscuringLayerSqlServer.style.display != 'block')
					pnlBasePageObscuringLayerWebServer.style.display =  'block';
			}
			else
				pnlBasePageObscuringLayerWebServer.style.display =  'block';
		}	
	}
	
	startCheckingWebServerAvailability();
}

startCheckingWebServerAvailability();