var pingTime_BasePage = PingTimePlaceHolder;//to be set by CABBaseContentPage on server side
var pingTimerId_BasePage;
var resourceToRequest_BasePage = 'ResourceToRequestPlaceHolder';//to be set by CabcommonComp.Web.CABBaseContentPage on server side, supposed to be a page URL
var pingReq_BasePage = new ActiveXObject('Microsoft.XMLHTTP');

function ping_BasePage()
{
    pingReq_BasePage.abort();
    pingReq_BasePage.open('POST', resourceToRequest_BasePage + '?keepsessionalive=true&sid=' + Math.random(), true);
    pingReq_BasePage.onreadystatechange = receive_BasePage;
    pingReq_BasePage.send();
}

function receive_BasePage()
{
    window.status = 'Response to Pong request received';
    if (pingReq_BasePage.readyState == 4)
    {
        if (pingReq_BasePage.status == 200)
            window.status = 'Pong on: ' + new Date();
		else
			window.status = 'Pong failed, XMLHTTP request status = ' + pingReq_BasePage.status;
    }
}

//Activate the timer
pingTimerId_BasePage = window.setInterval('ping_BasePage();', pingTime_BasePage);

//Let's get rid of CABPortal's popup which notifies thatn within three minutes session will expire
try { doReminder = function(){}; } catch(ex){}