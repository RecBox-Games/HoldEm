// -------- GameNite Controller App --------

// ---- Globals ----

var messages = [];
var text_drawables = [];
var image_drawables = [];
var needs_draw = false;
var playerName = null;
var controlpadState;
var SCREEN_HEIGHT;
var SCREEN_WIDTH;
var ORIENTATION;
var playerTurn = true;
var currentCall;
var playerCall;
var betIncrement = 5;
var action;
var playerMoney;

// Number formatter.
const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  
    minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });

//Promises
var playerJoinedSuccess = 0;

// Name Generator. Will pick a random adjective and a random name

var adjList = [
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
var nameList = [
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
var cardBack = new Image();
var menuImage = new Image();
var cardHands = new Image();

// ---- onFlip ----

function onFlip(width, height) {
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    needs_draw = true;
    drawScreen(["state:",controlpadState, playerName]);
}

// ---- Messages ----

// handle a single message from the console
function handleMessage(message) {
    console.log('got ' + message);

    sections = message.split(":");

    //If  state function, change the state of the game
    if (sections[0] == "state"){
        setState(sections); 
    }

    if(sections[0] == "refresh")
    {
        stateRequest();

    }

    //TODO. If joining - draw joining screen

    //TODO. If playing - draw playing screen (need playername, if it's the player's turn, money, cards)

    //TODO. If game finished - draw finished screen (need playername, money)
    
}

// Specify the list of messages to be sent to the console
function outgoingMessages() {
    temp = messages;
    messages = [];
    return temp;
}

function getDrawables() {
    if (needs_draw) {
        needs_draw = false;
        return image_drawables.concat(text_drawables);
    }
    return [];
}


// ---- Button Handler ----

//TODO: Function for when button is pressed, send a message
    //Should accomodate check, call, raise, raise incrememnt, raise decrement, menu, hands, long press on cards, 

// ---- Touch Handlers ----

//TODO: Fold Gesture

// Handle a single touch as it starts
function handleTouchStart(id, x, y) {
    // let msg = "TouchStart(" + x.toString() + "," + y.toString() + ")";
    // messages.push(msg);
    }


// Handle a single touch that has moved
function handleTouchMove(id, x, y) {

}

// Handle a single touch that has ended
function handleTouchEnd(id, x, y) {

}

// Handle a single touch that has ended in an unexpected way
function handleTouchCancel(id, x, y) {

}

// ---- Start and Update ----

// Called once upon page load (load your resources here)
function controlpadStart(width, height, orientation) {
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    ORIENTATION = orientation;

    //TODO. Load Resources

    loadImages();
    controlpadState = "Loading";
    drawScreen(["state:",controlpadState, playerName]);

    //TODO. State Request
    stateRequest();
    

}

// ---- State Screens ----

// setState -Sets the active state from the user

//In: -state: The message from the game that 
//            should be carrying an option that describes the current 
//            state of the game

//Out: Will return a message of the recognized state and 
//      execute a drawing function for that specific screen 
//      along with potentially assigning some variables

//Thru: The state will identify what screen to draw

//As: There is a provided state string along with variables if appropriate

//SE: Player name and money will be set using this function. If they are none, 
// the player will have no money or name

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

function drawScreen(sections) {
    wipeScreen();
    updateVariables(sections);
    switch(controlpadState) {
        case "Loading":
            drawLoadingScreen();
            break;
        case "ReadyToJoin":
            drawScreenReadyToJoin();
            break;
        case "JoinedWaitingToStart":
            drawJoinedWaitingToStart();
            break;
        case "JoinedHost":
            drawJoinedHost();
            break;
        case "PlayingWaiting":
            playerStatus = "Waiting for your Turn";
            drawScreenPlayingWaiting();
            break;
        case "PlayingPlayerTurn":
            playerStatus = "It's Your Turn!";
            drawScreenPlayingPlayerTurn();
        case "finished":
            drawScreenFinished();
            break;

    }
}

function wipeScreen()
{
    image_drawables = [];
    text_drawables = [];
    trackedDrbls = [];
    ctx.clearRect(0,0, SCREEN_WIDTH, SCREEN_HEIGHT);
    hitCtx.clearRect(0,0, SCREEN_WIDTH, SCREEN_HEIGHT);

}

function sendResponse(){
    setState(["state","PlayingWaiting",playerName,(playerMoney-playerCall)]);
    playerTurn = false;
    let msg = "PlayerResponse:" + action + ":" + playerCall;
    messages.push(msg);
    
}

function updateVariables(sections){
    for (i = 1; i < sections.length; i++){
        switch(i) {
            case 2:
                playerName = sections[i];
                break;
            case 3:
                playerMoney = parseInt(sections[i]);
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
                break;
            default:
                break;
        }
    }
}

function drawLoadingScreen() {
    text_drawables.push({
        type: 'text',
            text: 'Loading Texas Hold Em',
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
    needs_draw = true;

}

//TODO: Draw Joining Screen
function drawScreenReadyToJoin() {
   
    //Check if a spot is open
    while(playerName==null){
        playerName = prompt("Enter Username", generateName());
    }
    let msg = "NewPlayer:" + playerName;
    messages.push(msg);

}

function drawJoinedWaitingToStart() {

    text_drawables.push({
        type: 'text',
            text: 'Welcome ' + playerName + '!',
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
    text_drawables.push({
        type: 'text',
            text: 'Waiting on Host to Start',
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
    });

    needs_draw = true;

}

function drawJoinedHost() {
    var scale = sizeImage(buttonImage,.5);

    text_drawables.push({
        type: 'text',
            text: 'Welcome ' + playerName + '!',
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
    text_drawables.push({
        type: 'text',
            text: 'Start Game',
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
    });
    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
        scaleY: '.6',
        scaleX: scale,
        track: true,
        msg: "StartGame"
    });

    needs_draw = true;

}


//TODO: Draw Playing Screen

function drawScreenPlayingWaiting() {
     image_drawables.push({
        type: 'image',
        image: cardBack,
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
        scaleY: '2',
        scaleX: '2',
        track: true,
        msg: "FlipCard"
    });
    topMenu();
    drawStatus();
   
    needs_draw = true;
    
}

function drawScreenPlayingPlayerTurn() {

    image_drawables.push({
        type: 'image',
        image: cardBack,
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
        scaleY: '2',
        scaleX: '2',
        track: true,
        msg: "FlipCard"
    });
    topMenu()

    drawActions();
    drawStatus();

    needs_draw = true;
}
    //TODO: Function for Determining which buttons should be active


//Draw top menu
function topMenu() {
    var y = SCREEN_HEIGHT/16;

    autoSize = sizeFont(playerName, 0.5)
    fontSize = autoSize + "px serif";
    text_drawables.push({
        type: 'text',
            text: playerName,
            font: fontSize,
        x: SCREEN_WIDTH/2,
        y: y,
        centeredX: true,
        centeredY: true,
    });
    scale = sizeImage(cardHands,.25)
    image_drawables.push({
        type: 'image',
        image: cardHands,
        x: 50,
        y: y,
        centeredX: true,
        centeredY: true,
        scaleY: scale,
        scaleX: scale,
        track: true,
        msg: "Cards"
    });
    scale = sizeImage(menuImage, .2)
    image_drawables.push({
        type: 'image',
        image: menuImage,
        x: SCREEN_WIDTH - 50,
        y: y,
        centeredX: true,
        centeredY: true,
        scaleY: scale,
        scaleX: scale,
        track: true,
        msg: "Menu"
    });
    text_drawables.push({
        type: 'text',
            text: formatter.format(playerMoney),
            font: fontSize,
        x: SCREEN_WIDTH/2,
        y: (y + autoSize + 10),
        centeredX: true,
        centeredY: true,
        
    });
    
}

function drawActions() {
    var y = 27*SCREEN_HEIGHT/32;
    var scale = sizeImage(buttonImage,.4);
    var triangleOffset = 55;
    var triangleOutline = 5;
    var triangleBase = 80;
    var triangleHeight = 40;

    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: 3*SCREEN_WIDTH/4,
        y: y,
        centeredX: true,
        centeredY: true,
        scaleY: '.6',
        scaleX: scale,
        track: true,
        msg: "PlayerResponse"
    });
    text_drawables.push({
        type: 'text',
            text: action + ": " + formatter.format(playerCall),
            font: '25px serif',
        x: 3*SCREEN_WIDTH/4 - buttonImage.width*scale/2 + 10,
        y: y,
        centeredX: false,
        centeredY: true,
    });
    
    image_drawables.push({
        type: 'triangle',
        x: 3*SCREEN_WIDTH/4,
        y: y + triangleOffset,
        centeredX: true,
        centeredY: true,
        b: triangleBase,
        h: triangleHeight,
        color: '#FF0000',
        rotation: 0,
        outline: triangleOutline,
        msg: "Down",
        track: true


    });
    image_drawables.push({
        type: 'triangle',
        x: 3*SCREEN_WIDTH/4,
        y: y - triangleOffset,
        centeredX: true,
        centeredY: true,
        b: triangleBase,
        h: triangleHeight,
        color: '#FF0000',
        rotation: 180,
        outline: triangleOutline,
        msg: "Up",
        track: true

    });

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
    wipeScreen();
    drawScreenPlayingPlayerTurn();

}

function drawStatus() {

    text_drawables.push({
        type: 'text',
            text: playerStatus,
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: 7* SCREEN_HEIGHT/32,
        centeredX: true,
        centeredY: true,
        
    });

}




//TODO: Finished Screen

function drawScreenFinished() {
    
}

//Extra Screens

//TODO: Show card ranks

//TODO: Show menu


// Called 30 times per second
function controlpadUpdate() {

}


//Utilities

function loadImages() {
    buttonImage.src = "resources/button.png"
    cardBack.src = "resources/card_back_double.png"
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

    return (fontSize + increment)
}

function sizeImage(image, area) {

    var w = image.naturalWidth;
    var scale = (SCREEN_WIDTH*area)/w;
    return scale;
}

