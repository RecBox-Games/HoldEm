// -------- GameNite Controller App --------

// ---- Globals ----

// ---- UI Setup ----
let messages = []; //Array to hold messages to send to the controller
let text_drawables = []; //Array to hold text boxes sent to canvas
let image_drawables = []; //Array to hold images sent to canvas
const hideables = document.getElementsByClassName('hideables');
const cardElement = document.getElementById('card');
const menuOverlay = document.getElementById('topmenu');
const nameField = document.getElementById('playerName');
const moneyField = document.getElementById('playerMoney');
const statusField = document.getElementById('playerStatus');
const actionButton = document.getElementById('actionButton');
const playButton = document.getElementById('playButton');
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


// ---- Game Specific Variables ----

let currentCall; //The amount requested for the user to call
let betIncrement = 5; //Sets the bet increment for the user
let controlpadState; //State of the player that is pulled from the game

// ---- Player Specific Variables ----

let playerName = null; //Name of the player
let playerTurn; //Bool of whether or not it is the player's turn 
let playerCall; //The amount the user actually called. Will be higher than currentCall for a raise
let action; //Will be fold, call, check, or raise. Updates in controller UI
let playerMoney; //Amount of money the player has
let cards = [];


//Defined tools and utilities

// Number formatter. Formats to USD

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

    //If state function recieved, change the state of the game
    if (sections[0] == "state"){
        setState(sections); 
    }

    //If a refresh message is recieved, the game is trying 
    // to update the controller's UI. Perform a state request.

    if(sections[0] == "refresh")
    {
        stateRequest();
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
    console.log("fold");
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
        case "JoinedWaiting":
            drawJoinedWaiting();
            break;
        case "PlayingWaiting":
            playerStatus = "Waiting for your Turn";
            drawPlayingWaiting();
            break;
        case "PlayingPlayerTurn":
            playerTurn=true;
            playerStatus = "It's Your Turn!";
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
    waitingScreen("Waiting on Host");

}

function drawJoinedHost() {
    waitingScreen("Start Game");

    scale = sizeImage(buttonImage,.5);

    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
        scaleY: buttonScale(2),
        scaleX: scale,
        track: true,
        msg: "StartGame:1000"
    });
}

function drawJoinedWaiting() {
  waitingScreen("Waiting for next hand...");

}

function drawPlayingWaiting() {
    wipeScreen();
    drawCardBack();
    topMenu();
    drawPeek()
    drawStatus();    
}

function drawPlayingPlayerTurn() {

    drawCardBack();
    topMenu();
    drawActions();
    drawStatus();
}

function drawGameFinished() {
    waitingScreen("Game Finished");
    
}

// ---- Minor Drawing Functions ----

//Draws the card backs

function drawCardBack() {
    cardElement.style.display='flex';
}

// Draws the waiting screen when a user needs to wait before an action

function waitingScreen(waitingText) {
    text = 'Welcome ' + playerName + '!';
    autoSize = sizeFont(text, 0.75)

    text_drawables.push({
        type: 'text',
        text: text,
        font: autoSize,
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
    text = waitingText;
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
    y = 27*SCREEN_HEIGHT/32;
   
    x=SCREEN_WIDTH/4;
    text = "Hold to Peek"; 
    autoSize = sizeFont(text,0.25);
    scale = sizeImage(buttonImage,.3);
    scaleForButtonY = buttonScale(3);



    text_drawables.push({
        type: 'text',
        text: text,
        font: autoSize,
        x: x,
        y: y,
        centeredX: true,
        centeredY: true,
    });
    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: x,
        y: y,
        centeredX: true,
        centeredY: true,
        scaleY: scaleForButtonY,
        scaleX: scale,
        track: true,
        msg: "Peek"
    });
}

function updateActionButton(){
    text = action + ": " + formatter.format(playerCall);
    actionButton.innerHTML=text;

    

}

function flipCard(){
    $("#card").flip(true);
    cardFlipped = true;

}




function UpdateMoney(amount) {
    let attemptedValue = playerCall + amount*betIncrement;
    if (attemptedValue === 0) {
        action = "Check";
        playerCall = currentCall;

    }
    
    else if (attemptedValue === currentCall)
    {
        action = "Call";
        playerCall = currentCall;
        
    }
    else if (attemptedValue > currentCall) {
        action = "Raise";
        playerCall = playerCall + amount*betIncrement;
    }
    updateActionButton();

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
    ctx.fillStyle = "#808080";
    ctx.fillRect(borderWidth, borderWidth, canvas.width, canvas.height);
}


function sendResponse(){
    setState(["state","PlayingWaiting",playerName,(playerMoney-playerCall)]);
    playButton.style.display="none";
    playerTurn = false;
    let msg = "PlayerResponse:" + action + ":" + playerCall;
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
                playerMoney = parseInt(sections[i]);
                moneyField.innerHTML = formatter.format(playerMoney);
                break;
            case 4:
                currentCall = parseInt(sections[i]);
                playerCall = currentCall;
                if (parseInt(currentCall) > 0){
                    action = "Call";
                }
                else {
                    action = "Check";
                }
                break;
            case 5:
            case 6:
                cardvalues = sections[i].split("-");
                addCard(cardvalues);
                break;
            default:
                break;
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

