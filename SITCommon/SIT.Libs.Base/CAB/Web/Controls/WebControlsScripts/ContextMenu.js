//Class definition - the overall context menu object
function ContextMenu(contextMenuClientID, parentContextMenuItem, boundControlID, hdnSource, unselectedColor, unselectedBackgroundColor, selectedColor, selectedBackgroundColor)
{
    this.ContextMenuDOMObject = document.getElementById(contextMenuClientID);//A DIV object
    this.ParentContextMenuItem = parentContextMenuItem;//must be null if this is the top level context menu
    this.BoundControlID = boundControlID;
    
    //set source object (the one that was right cliccked) if this is the top menu
    if(hdnSource != null)//this is the top menu
    {
        this.HiddenFieldToStoreSourceObject = document.getElementById(hdnSource);
        this.HiddenFieldToStoreSourceObject.value = boundControlID;
    }
    
    //build entries collection
    this.ContextMenuEntries = new Array();            
    for(var i=0; i<this.ContextMenuDOMObject.childNodes[0].childNodes[0].childNodes.length; i++)//this.ContextMenuDOMObject.childNodes[0].childNodes[0].childNodes stands for DIV.TABLE.TBODY.TRs
        this.ContextMenuEntries.push(new ContextMenuEntry(
                this.ContextMenuDOMObject.childNodes[0].childNodes[0].childNodes[i],//this.ContextMenuDOMObject.childNodes[0].childNodes[0].childNodes[i] stands for DIV.TABLE.TBODY.TR[i]
                this,//menu item being bulit belongs to this context menu object
                parentContextMenuItem,//must be null if this is the top level context menu
                unselectedColor, 
                unselectedBackgroundColor, 
                selectedColor, 
                selectedBackgroundColor));
}

//Gets the top menu this context menu belongs to
ContextMenu.prototype.GetTopMenu = function()
{
    if(this.ParentContextMenuItem == null)
        return this;
    else
        return this.ParentContextMenuItem.BelongingContextMenu.GetTopMenu();
}

//Makes the context menu visible
ContextMenu.prototype.Show = function()
{
    //Showing menu after right click?
    if(window.event.type.toLowerCase() == 'contextmenu')                
        this.UnselectAllMenuEntries();//reset the whole menu in terms of selections 
    
    this.ContextMenuDOMObject.style.display = "";//make this context menu visible by setting the underlying DOM object's style property (underlying DOM object is a DIV)
    
    window.event.cancelBubble = true;            
    return false;
}

//Sets context menu item's position in absolute terms
ContextMenu.prototype.SetPosition = function(x,y)
{
    //next three variables are useful only inside Simatic IT CAB Portal
    var furtherLeftOffset = 0;
    var furtherTopOffset = 0;
    var portalContentArea = document.getElementById('page');
    
    if(portalContentArea != null)
    {
        furtherLeftOffset = portalContentArea.offsetLeft;
        furtherTopOffset = portalContentArea.offsetTop;
    }
    
    this.ContextMenuDOMObject.style.left = x - furtherLeftOffset;
    this.ContextMenuDOMObject.style.top = y - furtherTopOffset;
}

//Makes the context menu invisible
ContextMenu.prototype.Hide = function()
{    
    this.UnselectAllMenuEntries();//reset the whole menu in terms of selections 
    this.ContextMenuDOMObject.style.display = "none";
    
    window.event.cancelBubble = true;            
    return false;
}

//Unselects all the menu entries belonging to this context menu
ContextMenu.prototype.UnselectAllMenuEntries = function()
{
    for(var i=0; i<this.ContextMenuEntries.length; i++)
        this.ContextMenuEntries[i].Unselect();
}

//Gets the width in pixel of this context menu
ContextMenu.prototype.GetWidth = function()
{    
    return this.ContextMenuDOMObject.clientWidth;
}

//Gets the height in pixel of this context menu
ContextMenu.prototype.GetHeight = function()
{ 
    return this.ContextMenuDOMObject.clientHeight;
}
/*------------------------------------------------------------------------------------------*/

//Class definition - a menu entry
function ContextMenuEntry(contextMenuEntryDOMObject, belongingContextMenu, parentContextMenuItem, unselectedColor, unselectedBackgroundColor, selectedColor, selectedBackgroundColor)
{            
    this.ContextMenuEntryDOMObject = contextMenuEntryDOMObject;//ContextMenuEntryDOMObject is a TR object
    
    this.BelongingContextMenu = belongingContextMenu;
    this.ParentContextMenuItem = parentContextMenuItem;
    
    this.UnselectedColor = unselectedColor;
    this.UnselectedBackgroundColor = unselectedBackgroundColor;
    this.SelectedColor = selectedColor;
    this.SelectedBackgroundColor = selectedBackgroundColor;

    //The textual part of this menu entry (a simple text or a link)
    this.MenuEntryText = new ContextMenuEntryText(
        this.ContextMenuEntryDOMObject.childNodes[1], 
        this,
        unselectedColor, 
        unselectedBackgroundColor, 
        selectedColor, 
        selectedBackgroundColor);
        
    if(this.ContextMenuEntryDOMObject.childMenuID != null)
        this.SubMenu = new ContextMenu(
            this.ContextMenuEntryDOMObject.childMenuID,//child context menu's ID if this is a node entry (read from an HTML attribute), null if it is a comman entry
            this,//child context menu (if any) has this menu entry as parent
            null,//only for top menu 
            null,//only for top menu 
            unselectedColor, 
            unselectedBackgroundColor, 
            selectedColor, 
            selectedBackgroundColor);
    else
        this.SubMenu = null;
        
    //Events handling (using 'closure')
    var self = this;
    
    this.ContextMenuEntryDOMObject.onmouseover = function()
    {                 
        self.Select();
    }
    
    this.ContextMenuEntryDOMObject.onclick = function()
    {           
        self.Click();
        window.event.cancelBubble = true; 
        return false;
    }
    
    //set cursor
    if(this.IsSubMenuEntry())
        this.ContextMenuEntryDOMObject.style.cursor = 'default';
    else
        this.ContextMenuEntryDOMObject.style.cursor = 'hand';
}

//Gets the top menu this entry belongs to
ContextMenuEntry.prototype.GetTopMenu = function()
{
    return this.BelongingContextMenu.GetTopMenu();
}

//Return true if this menu entry is a sub menu node, false if it is a command node
ContextMenuEntry.prototype.IsSubMenuEntry = function()
{
    return this.SubMenu != null;
}

//Programmatically triggers a click on this menu entry
ContextMenuEntry.prototype.Click = function()
{
    if(!this.ContextMenuEntryDOMObject.parentNode.parentNode.disabled)//this.ContextMenuEntryDOMObject.parentNode.parentNode stands for TR.TBODY.TABLE, i.e. the table object containing the TR object associated to this ContextMenuEntry
    {
        if((this.MenuEntryText.LinkDOMObject != null) && (!this.MenuEntryText.LinkDOMObject.disabled))
            this.MenuEntryText.LinkDOMObject.fireEvent("onclick");
    }
}

//Manage the mouseover event over a menu entry
ContextMenuEntry.prototype.Select = function()
{
    //Select parent menu entry
    if(this.ParentContextMenuItem != null)
        this.ParentContextMenuItem.Select();
    
    //Unselect all siblings menu entries            
    this.BelongingContextMenu.UnselectAllMenuEntries(); 
      
    //Self-highlight and make belonging menu visible
    this.MenuEntryText.Select();            
    this.ContextMenuEntryDOMObject.style.backgroundColor = this.SelectedBackgroundColor;
    this.ContextMenuEntryDOMObject.style.color = this.SelectedColor;            
    this.BelongingContextMenu.Show();
    
    //Show child menu (if any) and set its position according to parent menu entry's position
    if(this.SubMenu != null)
    {                 
        this.SubMenu.SetPosition(
            parseInt(this.BelongingContextMenu.ContextMenuDOMObject.style.left, 10) + this.BelongingContextMenu.ContextMenuDOMObject.clientWidth - parseInt(this.BelongingContextMenu.ContextMenuDOMObject.childNodes[0].style.borderWidth ,10) - this.BelongingContextMenu.ContextMenuDOMObject.childNodes[0].cellSpacing - this.BelongingContextMenu.ContextMenuDOMObject.childNodes[0].cellPadding,
            parseInt(this.BelongingContextMenu.ContextMenuDOMObject.style.top, 10) + this.ContextMenuEntryDOMObject.offsetTop - parseInt(this.BelongingContextMenu.ContextMenuDOMObject.childNodes[0].style.borderWidth ,10) - this.BelongingContextMenu.ContextMenuDOMObject.childNodes[0].cellSpacing);
        this.SubMenu.Show();
    }   
        
    //Cancel event bubbling
    window.event.cancelBubble = true; 
    return false;
}

//Called when managing the mouseover event over a menu entry in order to unselect all the others (+ sub menus if any)
ContextMenuEntry.prototype.Unselect = function()
{
    //Unselect all child menu's entries (if any sub menu is present) and then hide it
    if(this.SubMenu != null)
    {
        this.SubMenu.UnselectAllMenuEntries();
        this.SubMenu.Hide();
    }
        
    //Self-unhighlight
    this.MenuEntryText.Unselect();            
    this.ContextMenuEntryDOMObject.style.backgroundColor = this.UnselectedBackgroundColor;
    this.ContextMenuEntryDOMObject.style.color = this.UnselectedColor;
    
    //Cancel event bubbling
    window.event.cancelBubble = true; 
    return false;
}

/*------------------------------------------------------------------------------------------*/

//Class definition - the textual element of a context menu entry
function ContextMenuEntryText(contextMenuEntryTextDOMObject, belongingContextMenuEntry, unselectedColor, unselectedBackgroundColor, selectedColor, selectedBackgroundColor)
{    
    this.ContextMenuEntryTextDOMObject = contextMenuEntryTextDOMObject;
    this.BelongingContextMenuEntry = belongingContextMenuEntry;
    
    this.UnselectedColor = unselectedColor;
    this.UnselectedBackgroundColor = unselectedBackgroundColor;
    this.SelectedColor = selectedColor;
    this.SelectedBackgroundColor = selectedBackgroundColor;
    
    //ContextMenuEntryTextDOMObject is a TD object, having an A child its menu entry is a command or simply an inner text if it is a link to a sub menu    
    if(this.ContextMenuEntryTextDOMObject.childNodes[0].tagName == null)
    {
        this.LinkDOMObject = null;
        this.Text = this.ContextMenuEntryTextDOMObject.childNodes[0].nodeValue;
    }
    else//ContextMenuEntryTextDOMObject.childNodes[0].tagName == 'A'
    {
        this.LinkDOMObject = this.ContextMenuEntryTextDOMObject.childNodes[0];
        this.Text = this.LinkDOMObject.innerText;
        
        //var self = this;//closure
        this.LinkDOMObject.onclick = function()
        {
            //var topMenu = self.GetTopMenu();
            
            //topMenu.HiddenFieldToStoreSourceObject.value = topMenu.BoundControlID;
            this.click();
            
            //click is handled by onclick handler associated to the belonging menu entry (a TD)
            //the above mentioned handler fires the onclick event on this.LinkDOMObject
            //if we do not cencel bubbling at this level we will have a stack overflow error
            window.event.cancelBubble = true;
        }
    }  
}

//Gets the top menu this entry text belongs to
ContextMenuEntryText.prototype.GetTopMenu = function()
{
    return this.BelongingContextMenuEntry.GetTopMenu();
}

//Sets background and foreground color according to the presence of a link (A tag)
ContextMenuEntryText.prototype.Select = function()
{
    if(this.LinkDOMObject == null)
    {
        this.ContextMenuEntryTextDOMObject.style.backgroundColor = this.SelectedBackgroundColor;
        this.ContextMenuEntryTextDOMObject.style.color = this.SelectedColor;
    }
    else
    {
        this.LinkDOMObject.style.backgroundColor = this.SelectedBackgroundColor;
        this.LinkDOMObject.style.color = this.SelectedColor;
    }
    
    window.event.cancelBubble = true; 
    return false;
}

//Sets background and foreground color according to the presence of a link (A tag)
ContextMenuEntryText.prototype.Unselect = function()
{
    if(this.LinkDOMObject == null)
    {
        this.ContextMenuEntryTextDOMObject.style.backgroundColor = this.UnselectedBackgroundColor;
        this.ContextMenuEntryTextDOMObject.style.color = this.UnselectedColor;
    }
    else
    {
        this.LinkDOMObject.style.backgroundColor = this.UnselectedBackgroundColor;
        this.LinkDOMObject.style.color = this.UnselectedColor;
    }
    
    window.event.cancelBubble = true; 
    return false;
}

//-------------------------------------------------------------------
//function to be attached to the contextmenu event of all controls bound to a context menu
function showContextMenu(menuClientID, boundControlID, hdnSource, unselectedColor, unselectedBackgroundColor, selectedColor, selectedBackgroundColor)
{
    HideAllVisibleContextMenus();
    
    var menu = new ContextMenu(menuClientID, null, boundControlID, hdnSource, unselectedColor, unselectedBackgroundColor, selectedColor, selectedBackgroundColor);
            
    var menuOffset = 2;    
    var menuX = window.event.clientX - menuOffset;
    var menuY = window.event.clientY - menuOffset
               
    menu.SetPosition(menuX, menuY);     
    menu.Show();
    
    VisibleContextMenus.push(menuClientID);
    
    //window.event.cancelBubble = true;
    return false;
}

//function to be attached to the click event of the form to hide a context menu
function hideContextMenu(menuClientID)
{
    new ContextMenu(menuClientID).Hide();
}

var VisibleContextMenus = new Array();

//Hides all visible context menus
function HideAllVisibleContextMenus()
{
    for(var i=0; i<VisibleContextMenus.length; i++)
        new ContextMenu(VisibleContextMenus[i]).Hide();
        
    VisibleContextMenus = new Array();
}