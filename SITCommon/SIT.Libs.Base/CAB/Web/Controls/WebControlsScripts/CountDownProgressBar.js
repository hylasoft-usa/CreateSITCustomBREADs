//----------------------------------------------------------
//Javascript class representing the actual CountDownPressBar
//----------------------------------------------------------
//Constructor
function CountDownProgressBar(CountDownPressBarID, TimeToFill, BackgroundColor, HighlightColor, OnUpdateEventHandler, OnFinishEventHandler)
{
    //Initializations
    this.ID = CountDownPressBarID;
    this.NumberOfSquares = TimeToFill;
    this.BackgroundColor = BackgroundColor;
    this.HighlightColor = HighlightColor;    
    this.OnUpdateEventHandler = OnUpdateEventHandler;
    this.OnFinishEventHandler = OnFinishEventHandler;
    
    this.ResetAllSquares();
}


//Highlight the first non highlighted square
CountDownProgressBar.prototype.HighlightNextSquare = function()
{
    if (this.FirstNonHighLightedSquareIndex == this.NumberOfSquares)
    {
        this.ResetAllSquares();
        if((this.OnFinishEventHandler) && (this.OnFinishEventHandler != ''))
            eval(this.OnFinishEventHandler + "(" + this.NumberOfSquares + ");");
    }
    
    for(var j = 0; j <= this.FirstNonHighLightedSquareIndex; j++)
    {
        var squareToHighlight = document.getElementById(this.ID + '_cell' + j);
        squareToHighlight.style.backgroundColor = this.HighlightColor;
    }
    
    this.FirstNonHighLightedSquareIndex++;
    
    if((this.OnUpdateEventHandler) && (this.OnUpdateEventHandler != ''))
        eval(this.OnUpdateEventHandler + "(" + this.FirstNonHighLightedSquareIndex + ", " + this.NumberOfSquares + ");");
}

//Resets all squares
CountDownProgressBar.prototype.ResetAllSquares = function()
{
    for(var i=0; i < this.NumberOfSquares; i++)
    {
        var currentSquare = document.getElementById(this.ID + '_cell' + i);
        currentSquare.style.backgroundColor = this.BackgroundColor;
    }
    
    this.FirstNonHighLightedSquareIndex = 0;	    
}