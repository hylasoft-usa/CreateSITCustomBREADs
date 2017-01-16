//function that prevents keyboard combination
function KillKeyboardShortcuts_BasePage()
{
	//ALT, CTRL combinations
	if((window.event.ctrlKey) || (window.event.altKey))
	{
		//Allow CTRL+C, CTRL+V, CTRL+X, CTRL+A
		if((window.event.keyCode != 67) && (window.event.keyCode != 86) && (window.event.keyCode != 88) && (window.event.keyCode != 65))
		{
			try{window.event.keyCode = 0;} catch(ex){}
			window.event.cancelBubble = true;
			window.event.returnValue = false;
		}
	}
	//Block TAB or F1 - F12
	else if((window.event.keyCode > 111) && (window.event.keyCode < 124))
	{
		try{window.event.keyCode = 0;}catch(ex){}
		window.event.cancelBubble = true;
		window.event.returnValue = false;
	}
}

document.attachEvent('onkeydown', KillKeyboardShortcuts_BasePage);