var CheckSqlServerConnection_ResourceToRequest = 'ResourceToRequestPlaceHolder';//to be set by CABBaseContentPage on server side, supposed to be a page URL
var CheckSqlServerConnectionReq_BasePage = new ActiveXObject('Microsoft.XMLHTTP');

function CheckSqlServerConnection_BasePage(HandlerToCall)
{
    CheckSqlServerConnectionReq_BasePage.abort();
    CheckSqlServerConnectionReq_BasePage.open('POST', CheckSqlServerConnection_ResourceToRequest + "?sid=" + Math.random() + "&checkingsqlserveravailability=true", true);
	CheckSqlServerConnectionReq_BasePage.onreadystatechange = HandlerToCall;
	CheckSqlServerConnectionReq_BasePage.send();
}

function IsSqlServerRequestReady()
{ 
    try 
    {        
        return (CheckSqlServerConnectionReq_BasePage.readyState == 4);
    }
    catch(ex) { return false; }     
}

function IsSqlServerConnected()
{
    try 
    {        
		//status 200 = OK; status 0 = form submission which should be ignored 
		//responseText is used to see if a sql server connection has been successfully opened
		window.status = 'Sql poll response code = ' + CheckSqlServerConnectionReq_BasePage.status + ', Sql poll response text = ' + CheckSqlServerConnectionReq_BasePage.responseText;
        return ((CheckSqlServerConnectionReq_BasePage.status == 200) && (CheckSqlServerConnectionReq_BasePage.responseText == "OK"))  || (CheckSqlServerConnectionReq_BasePage.status == 0);
    }
    catch(ex) { return false; }     
}

var CheckSqlServerAvailabilityAndManageObscuringLayer_Interval;

function startCheckingSqlServerAvailability()
{
	CheckSqlServerAvailabilityAndManageObscuringLayer_Interval = setInterval('CheckSqlServerAvailabilityAndManageObscuringLayer()', 5000);
}

function stopCheckingSqlServerAvailability()
{
	clearInterval(CheckSqlServerAvailabilityAndManageObscuringLayer_Interval);
}

function CheckSqlServerAvailabilityAndManageObscuringLayer()
{	
	CheckSqlServerConnection_BasePage(CheckSqlServerAvailabilityAndManageObscuringLayer_Internal);
}

function CheckSqlServerAvailabilityAndManageObscuringLayer_Internal()
{
	window.status = 'Checking sql server availability at: ' + new Date();
	
	stopCheckingSqlServerAvailability();

	if(IsSqlServerRequestReady())
	{
		pnlBasePageObscuringLayerSqlServer = document.getElementById('BasePageObscuringLayerSqlServer');
		pnlBasePageObscuringLayerWebServer = document.getElementById('BasePageObscuringLayerWebServer');
		
		if(IsSqlServerConnected())
			pnlBasePageObscuringLayerSqlServer.style.display =  'none';
		else
		{	
			if(pnlBasePageObscuringLayerWebServer != null)
			{
				if(pnlBasePageObscuringLayerWebServer.style.display != 'block')
					pnlBasePageObscuringLayerSqlServer.style.display =  'block';
			}
			else
				pnlBasePageObscuringLayerSqlServer.style.display =  'block';
		}	
	}
	
	startCheckingSqlServerAvailability();
}

startCheckingSqlServerAvailability();