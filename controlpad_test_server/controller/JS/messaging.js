// ----- Messaging -----

//Messaging contains
//When a messages is recieved by the game
//When a user wants to send a message to the game

// Function: handleMessage

// Inputs:
    // message (string): The message received by the function.

// Returns:
    // None

// Input-Output Relation:
    // The function takes a single input, which is a message received as 
    // a string. It processes this message by splitting it into sections 
    // based on the delimiter ":". Depending on the content of the sections,
    // the function may call other functions (setState and stateRequest) 
    // to perform certain actions. The function doesn't directly return 
    // any value.

// Side Effects:

    // Logs a message to the console.
    // May call the setState and stateRequest functions.
    
// Assumptions about State:
    // Assumes the existence of the setState and stateRequest functions.
    // Assumes that the input message is properly formatted with 
    // sections separated by ":".

// The handleMessage function processes incoming messages and triggers 
// actions based on their content. It splits the message into sections, 
// and based on the content of the sections, it can change the game 
// state or request a state update from the game.

function handleMessage(message) {
    console.log('got ' + message);
    sections = message.split(":");

    switch (sections[0]) {
        //Used for an updated state request
        case "state":
            setState(sections); 
            break;

        //Used to force the control to make a stateRequest
        case "refresh":
            stateRequest();
            break;

        //Used to update a custom setting
        case "setting":
            updateSettings(sections);
            break;

        //Used to grab available colors
        case "colors":
            updateAvailableColors(sections);
            break;

        //Used to designate user as host
        case "host":
            isHost=true;
            break;
        
        //Used to send a prompt to the user
        case "alert":
            alert(sections[1]);{
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
            break;
        
        //Used to update the status of the user dynamically
        case "UpdateStatus":
            playerStatus = sections[1];
            drawStatus();
            break;

        default:
            break;
    }
}


//Used to send a player response like Call, Fold, Raise...
function sendResponse(){
    let message = "";
    
    switch (action) {
        case "Fold":
            message = "Fold this round?";
            break;
        default:
            message = action + " " + playerCall + "?";
            break;    
    }
    let response = confirm(message);
    if(response)
    {
        let msg;

        switch (action) {
            case "Fold":
                playFoldSound();
                setState(["state","JoinedWaiting",playerName,playerColor,playerMoney]);
                msg="PlayerResponse:Fold";
                break;
            case "Call":
            case "Check":
                playCommitSound();
                setState(["state","PlayingWaiting",playerName,playerColor,(playerMoney-currentCall)]);
                msg = "PlayerResponse:Call";
                break;
            
            default:
                let amountRaised = playerCall - currentCall;
                playCommitSound();
                setState(["state","PlayingWaiting",playerName,playerColor,(playerMoney-amountRaised-currentCall)]);
                msg = ("PlayerResponse:Raise:" + amountRaised.toString())
                break;
        
        }
        playerTurn = false;
        messages.push(msg);
    }    
}

//Used to send a yes or no to a players request for money
function moneyChoice(msg)
{
    messages.push("moneyResponse:" + msg);
    stateRequest();
}