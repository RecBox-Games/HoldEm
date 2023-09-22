// -------- GameNite Controller App --------

// ---- Globals ----
// ---- UI Setup ----
let messages = []; //Array to hold messages to send to the controller
let text_drawables = []; //Array to hold text boxes sent to canvas
let image_drawables = []; //Array to hold images sent to canvas




const chipValues = [100,25,10,5];
const chipFiles = ["/resources/chip3.png", "./resources/chip3_green.png","./resources/chip3_blue.png", "./resources/chip3_red.png"];

const hideables = document.getElementsByClassName('hideables');
const cardElement = document.getElementById('card');
const menuOverlay = document.getElementById('topmenu');
const nameField = document.getElementById('playerName');
const moneyField = document.getElementById('playerMoney');
const statusField = document.getElementById('playerStatus');
const actionButton = document.getElementById('actionButton');
const playButton = document.getElementById('playButton');
const peekButton = document.getElementById('peekButton');
const chips = document.getElementById('chipStack');
const colorPickerForm = document.getElementById('colorPicker');
const soundButton = document.getElementById('soundButton');
const cardBacks = document.getElementsByClassName('cardBack');
const anteMenu = document.getElementById('anteMenu');

const gameTypeRadio = document.querySelectorAll('input[name="game-type"]');
const anteControls = document.getElementById('ante-controls');
const blindControls = document.getElementById('blind-controls');
const gameForm = document.getElementById('gameForm');
const gameFormContainer = document.getElementById('gameFormContainer');
const upArrow = document.getElementById("Up");
const downArrow = document.getElementById("Down");

gameTypeRadio.forEach((radio) => {
    radio.addEventListener('change', () => {
        if (radio.value === 'ante') {
            anteControls.style.display = 'block';
            blindControls.style.display = 'none';
        } else {
            anteControls.style.display = 'none';
            blindControls.style.display = 'block';
        }
    });
});

gameForm.addEventListener("submit",startGame,false);




// const carddiv = document.getElementById('customCards');
const card1 = document.getElementById('card1');
const card2 = document.getElementById('card2');


let needs_draw = false; //Bool to trigger the draw function
let SCREEN_HEIGHT; //Height of screen
let SCREEN_WIDTH; //Width of screen
let SCREEN_ORIENTATION; //Portrait, Landscape, etc.

let isHeld = false;
let timer = null;
let cardFlipped = false;
let foldHold = false;
let foldHoldStartY;
let isHost = false;

let soundSetting = true;


// ---- Game Specific Variables ----

let currentCall; //The amount requested for the user to call
let betIncrement; //Sets the bet increment for the user
let controlpadState; //State of the player that is pulled from the game

// ---- Player Specific Variables ----

let playerName = null; //Name of the player
let playerTurn; //Bool of whether or not it is the player's turn 
let playerCall; //The amount the user actually called. Will be higher than currentCall for a raise
let action; //Will be fold, call, check, or raise. Updates in controller UI
let playerMoney; //Amount of money the player has
let cards = [];
let playerColor;





//Defined tools and utilities

// Number formatter. Formats to USD
function getRandomColor() {
    const r = Math.round(Math.random() * 255);
    const g = Math.round(Math.random() * 255);
    const b = Math.round(Math.random() * 255);
    return `rgb(${r},${g},${b})`;
}

const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',

    maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });


// Name Generator. Will pick a random adjective and a random name

const adjList = [
    'Zany', 'Whimsical', 'Bubbly', 'Playful', 'Lively', 
    'Quirky', 'Cheeky', 'Vibrant', 'Jovial', 'Energetic', 
    'Spirited', 'Carefree', 'Groovy', 'Eclectic', 
    'Spontaneous', 'Lighthearted', 'Hilarious', 'Dynamic', 
    'Frolicsome', 'Witty', 'Radiant', 'Wacky', 'Festive', 
    'Silly', 'Mischievous', 'Animated', 'Exuberant', 'Sprightly',
    'Unpredictable', 'Merry', 'Buoyant', 'Fanciful', 
    'Unconventional', 'Chucklesome', 'Bouncy', 'Joyful', 
    'Effervescent', 'Amusing', 'Breezy', 'Giggly', 
    'Frisky', 'Spunky', 'Jaunty', 'Droll', 'Zesty', 
    'Dynamic', 'Peculiar', 'Euphoric', 'Zippy', 'Hysterical'
    ];
const nameList = [
    'Dog', 'Cat', 'Elephant', 'Lion', 'Tiger', 'Giraffe', 
    'Zebra', 'Bear', 'Monkey', 'Kangaroo', 'Dolphin', 
    'Penguin', 'Owl', 'Koala', 'Cheetah', 'Rhino', 'Hippo',
    'Fox', 'Wolf', 'Horse', 'Rabbit', 'Squirrel', 'Gorilla',
    'Orangutan', 'Octopus', 'Crocodile', 'Alligator', 
    'Camel', 'Peacock', 'Parrot', 'Eagle', 'Hummingbird',
    'Sloth', 'Panda', 'Otter', 'Seal', 'Lemur', 'Raccoon',
    'Jellyfish', 'Seahorse', 'Butterfly', 'Meerkat', 
    'Chimpanzee', 'Platypus', 'Hedgehog', 'Llama', 
    'Armadillo', 'Ostrich', 'Tortoise', 'Gazelle'
]

//Assets
var buttonImage = new Image();
var menuImage = new Image();
var cardHands = new Image();

// ---- onFlip ----
//  This function is called when the screen is flipped, typically due to a change
//  in device orientation. It updates the global screen width and height variables, sets a flag
//  to indicate that a redraw is needed, and then triggers a screen redraw with updated state
//  information.
//  
//  Inputs:
//  - width (number): The new width of the screen.
//  - height (number): The new height of the screen.
//  
//  Outputs:
//  - None
//  
//  Through Processing:
//  1. Update the global SCREEN_WIDTH and SCREEN_HEIGHT variables with the provided values.
//  2. Set the needs_draw flag to true, indicating that a screen redraw is required.
//  3. Call the drawScreen function with an array containing the current controlpadState and playerName.
//  
//  Assumptions:
//  - The function assumes the existence of global variables SCREEN_WIDTH, SCREEN_HEIGHT, needs_draw,
//  controlpadState, and playerName.
//  - The drawScreen function is assumed to be defined and implemented elsewhere.
//  
//  Side Effects:
//  - Modifies the global SCREEN_WIDTH and SCREEN_HEIGHT variables.
//  - Sets the needs_draw flag to true.
//  - Calls the drawScreen function.

function onFlip(width, height) {
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    drawScreen(["state:",controlpadState, playerName]);
}

// ---- Messages ----
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
        
        case "state":
            setState(sections); 
            break;
        case "refresh":
            stateRequest();
            break;
        case "setting":
            updateSettings(sections);
            break;
        case "colors":
            updateAvailableColors(sections);
            break;
        case "host":
            isHost=true;
            break;
        case "alert":
            alert(sections[1]);
            break;
        
    }



}

function updateAvailableColors(sections){
    colorPickerForm.replaceChildren();
    var placeholderOption = document.createElement("OPTION");
        placeholderOption.innerHTML="New Color";
        placeholderOption.disabled = true;
        placeholderOption.selected = true;
        colorPickerForm.appendChild(placeholderOption);

    for( i=1; i < sections.length; i++)
    {
        let color=sections[i];
        var colorOption = document.createElement("OPTION");
        colorOption.setAttribute("value", color);
        colorOption.innerHTML=color[0].toUpperCase() + color.substring(1);
        colorPickerForm.appendChild(colorOption);
    }
}


// Specify the list of messages to be sent to the console
function outgoingMessages() {
    temp = messages;
    messages = [];
    return temp;
}

// Returns a list of concatenated text and images drawables for the game to draw
function getDrawables() {
    if (needs_draw) {
        needs_draw = false;
        return image_drawables.concat(text_drawables);
    }
    return [];
}


function foldTimer()    {
    if(playerTurn){
        foldHold = true;


    }

}

function fold() {
//     action = "Fold";
//     playerCall = 0;
//     sendResponse();
} 

// ---- Touch Handlers ----

//TODO: Fold Gesture

// Handle a single touch as it starts
function handleTouchStart(id, x, y) {
    if(foldHold)
    {
        foldHoldStartY = y;
    }
    }


// Handle a single touch that has moved
function handleTouchMove(id, x, y) {
    if(foldHold && (foldHoldStartY-y)> (SCREEN_HEIGHT/8))
    {
        foldHold = false;
        action = "Fold";
        playerCall = 0;
        sendResponse();
    }

}

// Handle a single touch that has ended
function handleTouchEnd(id, x, y) {
    foldHold = false;
    clearTimeout( timer );
    if(cardFlipped){
        $("#card").flip(false);
        cardFlipped=false;

    }

}

// Handle a single touch that has ended in an unexpected way
function handleTouchCancel(id, x, y) {
    clearTimeout( timer );
    foldHold = false;

    if(cardFlipped){
        $("#card").flip(false);
        cardFlipped=false;

    }

}

// ---- Start and Update ----

// Function: controlpadStart

// Inputs:
    // width (number): The width of the screen.
    // height (number): The height of the screen.
    // ORIENTATION (string): The orientation of the screen.

// Returns:
    // None

// Input-Output Relation:
    // The function takes three inputs: width, height, and ORIENTATION.
    //  It sets the global variables SCREEN_WIDTH, SCREEN_HEIGHT, and
    //  SCREEN_ORIENTATION based on these inputs. It then proceeds to
    //  load image resources, initialize the controlpadState to 
    // "Loading", and draw the loading screen using the drawScreen 
    // function. Finally, it makes a state request to synchronize 
    // the player with the game. The function doesn't directly 
    // return any value.

// Side Effects:
    // Modifies the global variables SCREEN_WIDTH, SCREEN_HEIGHT, and SCREEN_ORIENTATION.
    // Calls the loadImages function.
    // Modifies the global variable controlpadState.
    // Calls the drawScreen function.
    // Calls the stateRequest function.

// Assumptions about State:

    // Assumes the existence of global variables SCREEN_WIDTH, SCREEN_HEIGHT, and SCREEN_ORIENTATION.
    // Assumes the existence of the loadImages, drawScreen, and stateRequest functions.

function controlpadStart(width, height, ORIENTATION) {

    //Set screen variables from index.js
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    SCREEN_ORIENTATION = ORIENTATION;

    //Load Resources
    loadImages();

    //Initialize controlpadState to "Loading"
    controlpadState = "Loading";

    //Draw the loading screen
    drawScreen(["state:",controlpadState, playerName]);

    //Make a state request to synchronize the player with the game
    stateRequest();

}

// ---- State Screens ----


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
            drawPregame();
            break;
        //Waiting till next hand
        case "JoinedWaiting":
            drawJoinedWaiting();
            break;
        case "PlayingWaiting":
            drawPlayingWaiting();
            break;
        case "PlayingPlayerTurn":
            drawPlayingPlayerTurn();
            break;
        case "GameFinished":
            drawGameFinished();
            break;
        default:
            break;
    }
    needs_draw=true;
}

// ---- Major Drawing Functions ----

function drawLoadingScreen() {
    text = "Loading Texas Hold Em"
    autoSize = sizeFont(text, 0.75)

    text_drawables.push({
        type: 'text',
        text: text,
        font: autoSize,
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
    });
}

function drawReadyToJoin() {
   
    //Check if a spot is open
    while(playerName==null){
        playerName = prompt("Enter Username", generateName());
    }

    //Send a message to the game with the new player information
    let msg = "NewPlayer:" + playerName;
    messages.push(msg);
}

function drawJoinedWaitingToStart() {
    if(isHost)
    {
        playerStatus = "You're the Host. Start Game When Ready!";
        gameFormContainer.style.display = 'block';
    }
    else
    {
        playerStatus = "Waiting on Host To Start";

    }
    topMenu();
    drawStatus();

}

function drawJoinedHost() {
    playerStatus = "You are the host. Start game when ready!";
    topMenu();
    drawStatus();
}

function drawJoinedWaiting() {
    playerStatus = "Waiting for the next round to begin";
    topMenu();
    drawStatus();

}

function drawPregame() {
    playerStatus= "Sit out or Play this round?"
    drawStatus();
    topMenu();
    anteMenu.style.display = "block";


}

function drawPlayingWaiting() {
    playerStatus = "Waiting for your Turn";
    drawCardBack();
    topMenu();
    drawPeek()
    drawStatus();    
}

function drawPlayingPlayerTurn() {
    UpdateMoney(0);
    playerTurn=true;
    playerStatus = "It's Your Turn!";
    drawCardBack();
    topMenu();
    drawActions();
    drawStatus();
    chipStack();

}

function drawGameFinished() {
    playerStatus = "It's Your Turn!";
    topMenu();
    drawStatus();


    
}

// ---- Minor Drawing Functions ----

//Draws the card backs

function drawCardBack() {
    cardElement.style.display='flex';
}



//Draw top menu
function topMenu() {
    menuOverlay.style.display='flex';

}

function drawActions() {
    playButton.style.display='flex';
    updateActionButton();
   drawPeek();

}
function drawPeek(){
   peekButton.style.display="flex";
  
}

function peekButtonTouch(){
    timer = setTimeout( flipCard, 1000 );
}

function updateActionButton(){
    text = action + ": " + formatter.format(playerCall);
    actionButton.innerHTML=text;

    

}

function flipCard(){
    $("#card").flip(true);
    cardFlipped = true;

}

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




function UpdateMoney(amount) {
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

function drawStatus() {
    statusField.innerHTML = playerStatus;
}

// Called 30 times per second
function controlpadUpdate() {

}


//Utilities

//Returns a scale y for the button that is dependent on the last drawn text size

function buttonScale(padding) {
    return (padding*parseInt(ctx.font))/buttonImage.naturalHeight
}

function loadImages() {
    buttonImage.src = "resources/button.png"
    menuImage.src = "resources/menu_bars.png"
    cardHands.src = "resources/cardhand.png"
}

function generateName() {
    return adjList[Math.floor( Math.random() * adjList.length )] + " " + nameList[Math.floor( Math.random() * nameList.length )];
 };

function stateRequest() {
    let msg = "RequestState";
    messages.push(msg);
}

function sizeFont(text, area) {
    fontSize = 30;
    var width;
    var increment = 2;
    do{
    ctx.font = fontSize + "px serif";
    width = ctx.measureText(text).width;
    fontSize -= increment;

    }
    while(width>SCREEN_WIDTH*area); 

    return (fontSize + increment + "px serif")
}

function sizeImage(image, area) {

    var w = image.naturalWidth;
    var scale = (SCREEN_WIDTH*area)/w;
    return scale;
}

function wipeScreen()
{
    image_drawables = [];
    text_drawables = [];
    trackedDrbls = [];
    for (let element of hideables){
        element.style.display = 'none';
    }
    ctx.fillStyle = pattern;
    ctx.fillRect(borderWidth, borderWidth, canvas.width, canvas.height);
}


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

        switch (action) {
            case "Fold":
                playFoldSound();
                setState(["state","JoinedWaiting",playerName,playerColor,playerMoney]);
                break;
            default:
                playerCall = playerCall - currentCall;
                playCommitSound();
                setState(["state","PlayingWaiting",playerName,playerColor,(playerMoney-playerCall-currentCall)]);

        
        }
        playerTurn = false;
        let msg = "PlayerResponse:" + action + ":" + playerCall;
        messages.push(msg);
    }    
}

function playChipSound() {
    if(soundSetting)
    {
        var audio = new Audio('./resources/chipAdd.mp3');
        audio.play();

    }

}
function playFoldSound() {
    if(soundSetting)
    {
        var audio = new Audio('./resources/fold.mp3');
        audio.play();
    }
}
function playCommitSound() {
    if(soundSetting)
    {
        var audio = new Audio('./resources/pushChips.mp3');
        audio.play();
    }
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
colorPickerForm.addEventListener("change",changeColor,false);

function openPlayerMenu()
{
    document.getElementById('playerMenu').style.display='block';
    messages.push("AvailableColors");


}

function startGame(event)
{
    event.preventDefault();
    event.stopImmediatePropagation();


        
        const startingMoney = document.getElementById('starting-money').value;
        const gameType = document.querySelector('input[name="game-type"]:checked').value;
        let anteAmount = null;
        let bigBlind = null;
        let smallBlind = null;
        msg=null;
        
        if (gameType === 'ante') {
            anteAmount = document.getElementById('ante-amount').value;
            msg = ["StartGame",startingMoney,gameType,anteAmount];
            
        } else {
            bigBlind = document.getElementById('big-blind').value;
            smallBlind = document.getElementById('small-blind').value;
            msg = ["StartGame",startingMoney,gameType,bigBlind,smallBlind];

        }

        
        // Output the values (you can modify this to do something useful with the values)
        messages.push(msg.join(":"));
    

}

function changeColor(event)
{
    event.preventDefault();
    playerColor = event.target.value;
    
    updateColor();
    sendSetting("playerColor",event.target.value);
}

function toggleSound() {
    soundSetting = !soundSetting;
    updateSound();
    sendSetting('soundOn',soundSetting);
}

function updateSound() {
    if(soundSetting)
    {
        soundButton.innerHTML="Sound On";

    }
    else {
        soundButton.innerHTML="Sound Off";
    }

}

function sendSetting(setting, variable){
    let msg = "Setting:" + setting + ":" + variable;
    messages.push(msg);
}

function updateSettings(sections) {
    switch (sections[1]) {
        case "soundOn":
            let value = sections[2];
            if(value == "false")
            {
                value = !!!value;
                
            }
            else{
                value = !!value;
            }
            soundSetting = value;
            updateSound();
            break;
    }
}

