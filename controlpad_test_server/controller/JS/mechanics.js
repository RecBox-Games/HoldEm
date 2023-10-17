// -------- Mechanics --------

//Mechanics should  contain
//Poker Game Related Functions that aren't just drawing things

// ---- Assets ----


const chipValues = [100,25,10,5];
const chipFiles = ["/resources/chip3.png", "./resources/chip3_green.png","./resources/chip3_blue.png", "./resources/chip3_red.png"];

//Provides Chips for UI Element as they increase betting 
function chipStack(){
    chips.replaceChildren();
    chips.style.display="block";
    let offsetChip = 20;
    chipArray = [0,0,0,0];
    let divideAmount = playerCall;
    let value = 0;
    for(i=0; i < chipValues.length; i++)
    {

        if(divideAmount < 5)
        {
            break;
        }
        else
        {
            value = divideAmount/chipValues[i];
            if(value >= 1)
            {
                value = Math.floor(value);
                chipArray[i] = value;
                for(j=0; j<value; j++)
                {
                    var chip_img = document.createElement("IMG");
                    chip_img.setAttribute("src", chipFiles[i]);
                    chip_img.setAttribute("class", "chip");
                    chip_img.setAttribute("style", ("bottom: " + offsetChip.toString() + "px"));
                    document.getElementById("chipStack").appendChild(chip_img);
                    offsetChip = offsetChip + 20;
                    divideAmount = divideAmount - (chipValues[i])
                }


            }
        }
    }
}

//Provides a response when a user hits the action button
function captureAction(){

    if(playerCall == 0)
    {
        action = "Check";
    }
    else if (playerCall == currentCall)
    {
        action = "Call";
    }
    else 
    {
        action = "Raise";
    }
    sendResponse();
}

//Instantiates Card Elements based on given array
function addCard(cardarray)
{
    let card = {
        suit: cardarray[0],
        rank: cardarray[1]
    }
    cards.push(card);
    if (cards.length > 1){
        card1.suit = cards[0].suit;
        card1.rank = cards[0].rank;
        card2.suit = cards[1].suit;
        card2.rank = cards[1].rank;
        
        $("#card").flip({
            trigger: 'manual'
          });
    }
}

//Will run everytime a user increments their bet to make sure the actions and amounts line up
function UpdateMoney(amount) 
{
    if(playerMoney <= currentCall)
    {
        playerCall = playerMoney;
        action = "All In";
    }
    else if (currentCall > 0){
        action = "Call";
    }
    else {
        action = "Check";
    }

    let attemptedValue = playerCall + amount*betIncrement;

    if(amount > 0)
    {
        playChipSound();
    }
    if(attemptedValue >= playerMoney)
    {
        action = "All In"
        playerCall = playerMoney;
        upArrow.style.display = "none";
        if(attemptedValue <= currentCall)
        {
            downArrow.style.display="none";
        }
    }
    else if (attemptedValue === 0) {
        action = "Check";
        playerCall = currentCall;
        upArrow.style.display = "flex";
        downArrow.style.display = "none";
    }
    
    else if (attemptedValue === currentCall)
    {
        action = "Call";
        playerCall = currentCall;
        upArrow.style.display = "flex";
        downArrow.style.display = "none";

        
    }
    else if (attemptedValue > currentCall) {
        action = "Raise";
        playerCall = playerCall + amount*betIncrement;
        downArrow.style.display = "flex";
        upArrow.style.display = "flex";

    }
    updateActionButton();
    chipStack();

}

//Start Game Function
function startGame(event)
{
    event.preventDefault();
    event.stopImmediatePropagation();        
    const startingMoney = document.getElementById('starting-money').value;
    const gameType = "ante";
    let minimumBet = null;
    msg=null;
    
    minimumBet = document.getElementById('minimumBetAmount').value;

    msg = ["StartGame",startingMoney,gameType,minimumBet];

    messages.push(msg.join(":"));
    

}

//Used when a user makes a decision on anteing
function playingRound(value){
    if (value == 1){
        messages.push("playingRound:Playing");

    }
    if (value == 0){
        messages.push("playingRound:Sitting");
    }
    anteMenu.style.display = 'none';

    setState(["state","JoinedWaiting"]);




}

//Used when a user has requested funds
function addFunds()
{
    let response = prompt("Enter a positive integer less than 1000");
    if(response)
    {
        if(!/^[0-9]+$/.test(response))
        {
        alert("You did not enter a valid number");
        return;
        }
        if(parseInt(response) > 1000)
        {
        alert("You did not enter a number less than 1000");
        return;
        }
        messages.push("requestMoney:"+ response);
        alert("Sit tight while everyone votes");

    }

    
}
