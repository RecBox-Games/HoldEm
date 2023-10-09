// -------- Mechanics --------

//Mechanics should  contain
//Poker Game Related Functions that aren't just drawing things

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
    let response = prompt("Enter a positive integer that is divisible by 5");
    if(response)
    {
        while (!/^[0-9]+$/.test(response) & (parseInt(response) % 5 == 0)) {
            alert("You did not enter a number divisible by 5");
            response = prompt("Enter a positive integer that is divisible by 5");
            if(!response)
            {
                return;
            }
        }
        console.log(response)
        messages.push("requestMoney:"+ response);
        alert("Sit tight while everyone votes");

    }

    
}
