// -------- GameNite Controller App --------

// ---- Globals ----

var messages = [];
var text_drawables = [];
var image_drawables = [];
var needs_draw = false;
var logo_image = new Image();

// ---- onFlip ----

function onFlip(width, height) {
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    needs_draw = true;
}

// ---- Messages ----

// handle a single message from the console
function handleMessage(message) {
    console.log('got ' + message);
    text_drawables = [];
    const drbl = {
        type: 'text',
        text: message,
        font: '48px serif',
        x: 30,
        y: 100,
    };
    text_drawables.push(drbl);
    needs_draw = true;
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

// ---- Touch Handlers ----


// Handle a single touch as it starts
function handleTouchStart(id, x, y) {
    let msg = "TouchStart(" + x.toString() + "," + y.toString() + ")";
    messages.push(msg);
    //
    image_drawables.push({
        type: 'image',
        image: logo_image,
        x: x,
        y: y,
        rotation: 0,
    });
    needs_draw = true;
}


// Handle a single touch that has moved
function handleTouchMove(id, x, y) {
    let msg = "TouchMove(" + x.toString() + "," + y.toString() + ")";
    messages.push(msg);
}

// Handle a single touch that has ended
function handleTouchEnd(id, x, y) {
    let msg = "TouchEnd(" + x.toString() + "," + y.toString() + ")";
    messages.push(msg);
}

// Handle a single touch that has ended in an unexpected way
function handleTouchCancel(id, x, y) {
    let msg = "TouchCancel(" + x.toString() + "," + y.toString() + ")";
    messages.push(msg);
}

// ---- Start and Update ----

// Called once upon page load (load your resources here)
function controlpadStart(width, height) {
    SCREEN_WIDTH = width;
    SCREEN_HEIGHT = height;
    logo_image.src = "resources/logo.png";
    logo_image.onload = function () {
        console.log('loaded ' + this.width + ', ' + this.height);
        console.log('natural: ' + this.naturalWidth + ', ' + this.naturalHeight);
    };
}

// Called 30 times per second
function controlpadUpdate() {
    if (image_drawables.length > 0) {
        image_drawables[image_drawables.length-1].rotation += 2 * Math.PI / 60;
        needs_draw = true;
    }
}
