// -------- App --------

//App should contain
//Initialization of all variables
//Javascript Listeners
//Touch Handlers
//Controlpad Start and Update Functions

// ---- Globals ----

const chipValues = [100,25,10,5];
const chipFiles = ["/resources/chip3.png", "./resources/chip3_green.png","./resources/chip3_blue.png", "./resources/chip3_red.png"];

// ---- Canvas Settings ----
let messages = []; //Array to hold messages to send to the controller
let text_drawables = []; //Array to hold text boxes sent to canvas
let image_drawables = []; //Array to hold images sent to canvas
let needs_draw = false; //Bool to trigger the draw function
let SCREEN_HEIGHT; //Height of screen
let SCREEN_WIDTH; //Width of screen
let SCREEN_ORIENTATION; //Portrait, Landscape, etc.

// ---- Controller Mechanics ----
let cardFlipped = false; //Whether or not the card is flipped
let foldHold = false; //Bool to access the validity of the fold drag
let foldHoldStartY; //Starting y index of the fold drag function

// ---- HTML Elements ----

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
const sitOutButton = document.getElementById('sitOutButton');
const cardBacks = document.getElementsByClassName('cardBack');
const anteMenu = document.getElementById('anteMenu');
const gameTypeRadio = document.querySelectorAll('input[name="game-type"]');
const anteInfo = document.getElementById('anteInfo');
const blindInfo = document.getElementById('blindInfo');
const gameForm = document.getElementById('gameForm');
const gameFormContainer = document.getElementById('gameFormContainer');
const upArrow = document.getElementById("Up");
const downArrow = document.getElementById("Down");
const card1 = document.getElementById('card1');
const card2 = document.getElementById('card2');
const moneyMenu = document.getElementById('moneyMenu');
const centerMessage = document.getElementById('centerMessage');
const readyButton = document.getElementById('readyButton');

// // ---- Listeners ----

// Magic function to prevent double tap zoom
$(document).click(function(event) {
});

colorPickerForm.addEventListener("change",changeColor,false);

// ---- Poker Game Specific Variables ----
let currentCall; //The amount requested for the user to call
let betIncrement; //Sets the bet increment for the user
let cards = []; //Array of cards
let action; //Will be fold, call, check, or raise. Updates in controller UI
let playerCall; //The amount the user actually called. Will be higher than currentCall for a raise
let playerMoney; //Amount of money the player has
let playerTurn; //Bool of whether or not it is the player's turn
let playerAskingForMoney; //Name of Player asking for money
let playerAskingForAmount; //Amount of money that player is asking for

// ---- Player Specific Variables ----
let playerName = null; //Name of the player
let controlpadState; //State of the player that is pulled from the game
let playerColor;
let isHost = false;
let soundSetting = true;
let autoSitOut = false;

// ---- Touch Handlers ----

// Handle a single touch as it starts
function handleTouchStart(id, x, y) {

    //Check if players turn before setting a variable to track swipe distance
    if(foldHold)
    {
        foldHoldStartY = y;
    }
    }


// Handle a single touch that has moved
function handleTouchMove(id, x, y) {

    //Check if player is trying to fold
    if(foldHold && (foldHoldStartY-y)> (SCREEN_HEIGHT/8))
    {
        foldHold = false;
        action = "Fold";
        sendResponse();  //messaging
    }

}

// Handle a single touch that has ended
function handleTouchEnd(id, x, y) {
    foldHold = false;
}

// Handle a single touch that has ended in an unexpected way
function handleTouchCancel(id, x, y) {
    foldHold = false;
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
    // Modifies the global variable controlpadState.
    // Calls the drawScreen function.
    // Calls the stateRequest function.

// Assumptions about State:

    // Assumes the existence of global variables SCREEN_WIDTH, SCREEN_HEIGHT, and SCREEN_ORIENTATION.
    // Assumes the existence of drawScreen and stateRequest functions.

function controlpadStart(width, height, ORIENTATION) {

    //Set screen variables from index.js
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    SCREEN_ORIENTATION = ORIENTATION;

    //Initialize controlpadState to "Loading"
    controlpadState = "Loading";

    //Draw the loading screen
    drawScreen(["state:",controlpadState, playerName]);

    //Make a state request to synchronize the player with the game
    stateRequest();

}

// Called 30 times per second
function controlpadUpdate() {

}