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
        case "PlayingWaiting":
            playerName = sections[2];

            drawScreenPlayingWaiting();
            break;
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
    stateRequest();


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
    text_drawables.push({
        type: 'text',
            text: playerName,
            font: '36px serif',
        x: SCREEN_WIDTH/2,
        y: SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
     image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: SCREEN_WIDTH/2,
        y: 7*SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
        scaleY: '.6',
        scaleX: '0.3',
        track: true,
        msg: "Call"
    });
        text_drawables.push({
        type: 'text',
            text: 'Call',
            font: '25px serif',
        x: SCREEN_WIDTH/2,
        y: 7*SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: 3*SCREEN_WIDTH/4,
        y: 7*SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
        scaleY: '.6',
        scaleX: '0.3',
        track: true,
        msg: "Raise"
    });
        text_drawables.push({
        type: 'text',
            text: 'Raise',
            font: '25px serif',
            x: 3*SCREEN_WIDTH/4,
            y: 7*SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });
    image_drawables.push({
        type: 'image',
        image: buttonImage,
        x: 1*SCREEN_WIDTH/4,
        y: 7*SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
        scaleY: '.6',
        scaleX: '0.3',
        track: true,
        msg: "Check"
    });
        text_drawables.push({
        type: 'text',
            text: 'Check',
            font: '25px serif',
            x: 1*SCREEN_WIDTH/4,
            y: 7*SCREEN_HEIGHT/8,
        centeredX: true,
        centeredY: true,
    });

    

    
   
    needs_draw = true;
    
}
    //TODO: Function for Determining which buttons should be active




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
}

function generateName() {
    return adjList[Math.floor( Math.random() * adjList.length )] + " " + nameList[Math.floor( Math.random() * nameList.length )];
 };

function stateRequest() {
    let msg = "RequestState";
    messages.push(msg);
}
