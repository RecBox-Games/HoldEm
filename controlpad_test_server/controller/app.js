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

    if(sections[0] == "playerJoined")
    {
        playerJoinedSuccess = 1;

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
    switch(controlpadState) {
        case "Loading":
            drawLoadingScreen();
            break;
        case "ReadyToJoin":
            drawScreenReadyToJoin();
            break;
        case "JoinedWaitingToStart":
            playerName = sections[2];
            drawJoinedWaitingToStart();
            break;
        case "PlayingWaiting":
            playerName = sections[2];
            drawScreenPlayingWaiting();
            break;
        case "PlayingPlayerTurn":
            playerName = sections[2];
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
    // image_drawables.push({
    //     type: 'image',
    //     image: cardBack,
    //     x: SCREEN_WIDTH/2,
    //     y: SCREEN_HEIGHT/2,
    //     centeredX: true,
    //     centeredY: true,
    //     scaleY: '2',
    //     scaleX: '2',
    //     track: true,
    //     msg: "FlipCard"
    // });
    topMenu()
    drawActions();
    drawStatus();
    drawMoney();

    needs_draw = true;
}
    //TODO: Function for Determining which buttons should be active


//Draw top menu
function topMenu() {
    var y = SCREEN_HEIGHT/16;

    fontSize = sizeFont(playerName, 0.5) + "px serif";
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
    
}

function drawActions() {
    var y = 27*SCREEN_HEIGHT/32;
    var scale = sizeImage(buttonImage,.4);
    var action = "Call";

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
        msg: action 
    });
    text_drawables.push({
        type: 'text',
            text: action + ":",
            font: '25px serif',
        x: 3*SCREEN_WIDTH/4 - buttonImage.width*scale/2 + 10,
        y: y,
        centeredX: false,
        centeredY: true,
    });
    text_drawables.push({
        type: 'text',
            text: '$15',
            font: '25px serif',
        x: 3*SCREEN_WIDTH/4 - buttonImage.width*scale/2 + 10 + ctx.measureText(action).width + 20,

        y: y,
        centeredX: false,
        centeredY: true,
    });
    image_drawables.push({
        type: 'triangle',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/2,
        centeredX: true,
        centeredY: true,
        b: 100,
        h: 100,
        color: '#FF0000',
        rotation: 360

    });


    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: 1*SCREEN_WIDTH/4,
        y: y,
        centeredX: true,
        centeredY: true,
        scaleY: '.6',
        scaleX: '0.3',
        track: true,
        msg: "Check",
    });
        text_drawables.push({
        type: 'text',
            text: 'Check',
            font: '25px serif',
            x: 1*SCREEN_WIDTH/4,
            y: y,
        centeredX: true,
        centeredY: true,
    });
}

function drawStatus() {

    if(playerTurn){
        playerStatus = "It's Your Turn!";
    }
    else {
        playerStatus = "Waiting for Player Turn";
    }
    text_drawables.push({
        type: 'text',
            text: playerStatus,
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: 3* SCREEN_HEIGHT/16,
        centeredX: true,
        centeredY: true,
        
    });

}

function drawMoney() {
    fontSize = sizeFont("Remaining Money", 0.4) + "px serif";
    text_drawables.push({
        type: 'text',
            text: "Remaining Money:",
            font: fontSize,
        x: 10,
        y: 15* SCREEN_HEIGHT/16,
        centeredX: false,
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

