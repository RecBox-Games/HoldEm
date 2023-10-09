// ---- StateRequest ----


//StateRequest includes

//Setting a user's state with the given message
//Updating variables upon a state request

// Function: setState

// Inputs:
    // sections (array): An array of sections extracted from a message.

// Returns:
    // None

// Input-Output Relation:
    // The function takes a single input, which is an array of sections 
    // extracted from a message. It checks if the second section (index 1) 
    // exists. If it does, it updates the global controlpadState variable 
    // with the value of the second section and calls the drawScreen 
    // function, passing in the sections array. If the second section 
    // doesn't exist, it logs an error message to the console indicating 
    // that the state is not recognized. The function doesn't directly 
    // return any value.

// Side Effects:
    // Modifies the global variable controlpadState.
    // Calls the drawScreen function.
    // Logs a message to the console.

// Assumptions about State:
    // Assumes the existence of the global variable controlpadState.
    // Assumes the existence of the drawScreen function.

// The setState function updates the game's controlpad state based
// on the sections of a message. It can update the state and trigger
// a redraw of the screen based on the new state information provided
// in the sections. If the state is not recognized, it logs an error
// message.

function setState(sections) {
    if (sections[1])
    {
        controlpadState = sections[1];
        drawScreen(sections);
    }
    else {
        console.log("State not recognized");
    }
}


// Function: drawScreen

// Inputs:
    // sections (array): An array of sections extracted from a message.

// Returns:
    // None

// Input-Output Relation:
    // The function takes a single input, which is an array of sections
    // extracted from a message. It first clears the screen using the
    // wipeScreen function, then updates variables using the 
    // updateVariables function based on the provided sections. The
    // function then uses a switch-case structure to determine which
    // screen to draw based on the current controlpadState. It calls
    // specific functions for drawing different screens based on the
    // state. The function doesn't directly return any value.

// Side Effects:
    // Calls various screen drawing functions based on the current controlpadState.
    // Calls the wipeScreen and updateVariables functions.

// Assumptions about State:

    // Assumes the existence of the global variable controlpadState.
    // Assumes the existence of the wipeScreen, updateVariables, and various screen drawing functions.

// The drawScreen function manages the process of updating the screen
// based on the current state of the game. It clears the screen, updates
// variables, and then draws the appropriate screen based on the current
// controlpad state using a switch-case structure. Each case corresponds
// to a specific game state, and the function calls the corresponding
// drawing function for that state.

function drawScreen(sections) {
    wipeScreen(); 
    updateVariables(sections); //Updates name, money, current call
    switch(controlpadState) {
        case "Loading":
            drawLoadingScreen();
            break;
        case "ReadyToJoin":
            drawReadyToJoin();
            break;
        case "JoinedWaitingToStart":
            drawJoinedWaitingToStart();
            break;
        case "JoinedHost":
            drawJoinedHost();
            break;
        case "PlayingPregame":
            if (autoSitOut) {
                messages.push("playingRound:Sitting");
                stateRequest(); 
            }
            else
            {
            console.log(autoSitOut);
            drawPregame();
            }
            break;
        case "ReadyUp":
            drawReadyUp();
        //Waiting till next hand
        case "JoinedWaiting":
            drawJoinedWaiting();
            break;
        case "PlayingFolded":
            drawPlayingFolded();
            break;
        case "PlayingWaiting":
            drawPlayingWaiting();
            break;
        case "PlayingPlayerTurn":
            drawPlayingPlayerTurn();
            break;Messaging
        case "GameFinished":
            drawGameFinished();
            break;
        case "MoneyRequest":
            drawMoneyRequest();
            break;


        default:
            break;
    }
    needs_draw=true;
}


function stateRequest() {
    let msg = "RequestState";
    messages.push(msg);
}

function updateVariables(sections){
    cards = [];
    for (i = 1; i < sections.length; i++){
        switch(i) {
            case 2:
                playerName = sections[i];
                nameField.innerHTML = playerName;
                break;
            case 3:
                    playerColor = (sections[i]);
                    updateColor();
                    break;
            case 4:
                playerMoney = parseInt(sections[i]);
                moneyField.innerHTML = formatter.format(playerMoney) || "";
                break;
            case 5:
                currentCall = parseInt(sections[i]);
                playerCall = currentCall;
                break;
            case 6:
            case 7:
                cardvalues = sections[i].split("-");
                addCard(cardvalues);
                break;
            case 8:
                betIncrement = parseInt(sections[i]);
            default:
                break;
        }
    }
}

function updateColor() {
    document.documentElement.style.setProperty('--color', playerColor);
    colorPickerForm.value=playerColor;

    for(card of cardBacks) {
        card.backcolor = playerColor; 
    }
    
}


