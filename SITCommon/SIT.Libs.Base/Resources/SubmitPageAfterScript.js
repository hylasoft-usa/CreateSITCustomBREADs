var CheckWebServerConnectionAutoRefresh_ResourceToRequest = 'ResourceToRequestPlaceHolder';//to be set by CABBaseContentPage on server side, supposed to be a page URL
var CheckWebServerConnectionReqAutoRefresh_BasePage = new ActiveXObject('Microsoft.XMLHTTP');

function CheckWebServerConnectionAutoRefresh_BasePage(HandlerToCall)
{
    CheckWebServerConnectionReqAutoRefresh_BasePage.abort();
    CheckWebServerConnectionReqAutoRefresh_BasePage.open('POST', CheckWebServerConnectionAutoRefresh_ResourceToRequest + "?sid=" + Math.random() + "&checkingwebserveravailability=true", true);
	CheckWebServerConnectionReqAutoRefresh_BasePage.onreadystatechange = HandlerToCall;
	CheckWebServerConnectionReqAutoRefresh_BasePage.send();
}
 
function IsWebServerRequestReadyAutoRefresh()
{ 
    try 
    {        
        return (CheckWebServerConnectionReqAutoRefresh_BasePage.readyState == 4);
    }
    catch(ex) { return false; }     
}

function IsWebServerConnectedAutoRefresh()
{ 
    try 
    {        
        return (CheckWebServerConnectionReqAutoRefresh_BasePage.status == 200);
    }
    catch(ex) { return false; }     
}

var RefreshBySubmit = RefreshBySubmitPlaceHolder;
setTimeout('RefreshPage_BasePage()', RefreshTimeMillisecondsPlaceHolder);

function RefreshPage_BasePage()
{
	CheckWebServerConnectionAutoRefresh_BasePage(RefreshPage_BasePage_Internal);
}

function RefreshPage_BasePage_Internal()
{
	if(IsWebServerRequestReadyAutoRefresh())
	{
		if(IsWebServerConnectedAutoRefresh())
		{
			window.status = 'Refreshing...';
			
			if (RefreshBySubmit)
				document.forms[0].submit();
			else
				document.location.href = document.location.href;
		}
		else RefreshPage_BasePage();
	}
}