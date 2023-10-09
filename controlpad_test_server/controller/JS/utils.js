// ---- Utils ----

//Utils include
//Utility functions



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

//Generate a random name from the above lists
function generateName() {
    return adjList[Math.floor( Math.random() * adjList.length )] + " " + nameList[Math.floor( Math.random() * nameList.length )];
 };



 //Function to update the available colors in the select List in the player options menu
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

//Makes sure it's the players turn before being able to
function foldTimer()    {
    if(playerTurn){
        foldHold = true;
    }
}


//Provides an autosized font
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

//Converts string to bool
function convertToBool(value)
{
    if(value == "false")
            {
                value = !!!value;
                
            }
    else{
        value = !!value;
    }
    return value;

}

// Number formatter. Formats GameNite Controller to USD
const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',

    maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });