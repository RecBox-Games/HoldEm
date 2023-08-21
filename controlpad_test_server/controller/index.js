// determine if this is a touch device like a phone or tablet or DevTools emulation
const isTouchDevice = 'ontouchstart' in window || navigator.maxTouchPoints > 0 || navigator.msMaxTouchPoints > 0;

// parse the url for sub ID
const url_arg_str = window.location.search;
const url_params = new URLSearchParams(url_arg_str);
const subid = url_params.get('subid');
const box_ip = window.location.href.split('/')[2].split(':')[0];
console.log("Sub ID: " + subid);

// canvas
const canvas = document.querySelector("canvas");
const ctx = canvas.getContext("2d");
const offset = 4;

canvas.width = window.innerWidth-offset;
canvas.height = window.innerHeight-offset;


const hitCanvas = document.createElement('canvas');
const hitCtx = hitCanvas.getContext('2d');

hitCanvas.width = window.innerWidth-offset;
hitCanvas.height = window.innerHeight-offset;

ctx.fillStyle = "#808080";
ctx.fillRect(0, 0, canvas.width, canvas.height);
ctx.fillStyle = "#000000";
/*ctx.font = "48px serif";
ctx.fillText("Touch", 100, 100);
*/

var touch_recognized = false

function screenChange() {
    canvas.width = window.innerWidth-offset;
    canvas.height = window.innerHeight-offset;

    hitCanvas.width = window.innerWidth-offset;
    hitCanvas.height = window.innerHeight-offset;
    SCREEN_HEIGHT = canvas.height;
    SCREEN_WIDTH = canvas.width;


    ctx.fillStyle = "#808080";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    console.log('resize fill');
    onFlip(window.innerWidth, window.innerHeight);

}    

window.onresize = screenChange;
window.onOrientationChange = screenChange;

// helpers
function isMobile() {
    //TODO: this function could probably be made to be more robust. tablets
    //      need be handled as well
    const regex = /Mobi|Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i;
    return refex.test(navigator.userAgent);
}

// globals
let drag_start_x = 0;
let drag_start_y = 0;
var trackedDrbls = [];

// websocket and main
ws = new WebSocket("ws://" + box_ip + ":50079");

window.onload = () => {
    if (ws.readState == WebSocket.CLOSED) {
        ws = new WebSocket("ws://" + box_ip + ":50079");
    }
}
ws.onclose = (event) => {
    console.log("closed websocket");
    ws = new WebSocket("ws://" + box_ip + ":50079");    
}

// wait for websocket to connect
ws.onopen = (event) => {
    console.log("openned websocket")

    byte_array = new Uint8Array(1);
    byte_array[0] = subid;
    ws.send(byte_array.buffer);

    ws.addEventListener('message', (event) => {
	msg = event.data;
        handleMessage(msg);
    });

    let lastTimeMove = new Date().getTime();
    
    if (isTouchDevice) {
	window.addEventListener("touchstart", (event) => {

            for (touch of event.changedTouches) {
                checkTrackedDrbls(touch.pageX, touch.pageY);
		        handleTouchStart(touch.identifier, touch.pageX, touch.pageY);
            }
	});
	
	window.addEventListener("touchmove", (event) => {
	    let thisTime = new Date().getTime();
	    if (thisTime - lastTimeMove < 50)
		return;
	    lastTimeMove=thisTime;
            for (touch of event.changedTouches) {
		handleTouchMove(touch.identifier, touch.pageX, touch.pageY);
            }
	});
	
	window.addEventListener("touchend", (event) => {
            for (touch of event.changedTouches) {
		handleTouchEnd(touch.identifier, touch.pageX, touch.pageY);
            }
	});
	
	window.addEventListener("touchcancel", (event) => {
            for (touch of event.changedTouches) {
		handleTouchCancel(touch.identifier, touch.pageX, touch.pageY);
            }
	});
    } else {
	let isDragging = false;
	window.addEventListener("mousedown", (event) => {
	    handleTouchStart(1, event.pageX, event.pageY);
	    isDragging = true;
	});
	
	window.addEventListener("mousemove", (event) => {
	    let thisTime = new Date().getTime();
	    if (thisTime - lastTimeMove < 50)
		return;
	    lastTimeMove=thisTime;
	    if (isDragging) {
		handleTouchMove(1, event.pageX, event.pageY);
	    }
	});
	
	window.addEventListener("mouseup", (event) => {
	    handleTouchEnd(1, event.pageX, event.pageY);
	    isDragging = false;
	});
    }    

    function draw_image(drbl) {

        var x = drbl.x;
        var y = drbl.y;

	if (drbl.centeredX) {
	    x = drbl.x - drbl.scaleX*drbl.image.width/2;
	}
	if (drbl.centeredY) {
	    y = drbl.y - drbl.scaleY*drbl.image.height/2;
	}
        ctx.setTransform(drbl.scaleX, 0, 0, drbl.scaleY, x, y); // sets scale and origin
        ctx.rotate(drbl.rotation);
        ctx.drawImage(drbl.image, 0, 0);
        ctx.setTransform(1,0,0,1,0,0);
    }

    function draw_text(drbl) {

        var x = drbl.x;
        var y = drbl.y;

        ctx.font = drbl.font;

        if (drbl.centeredX) {
            x -= drbl.w / 2;
        }
        if (drbl.centeredY) {
            y += drbl.h / 3;
        }
        ctx.fillStyle = drbl.color;

        ctx.fillText(drbl.text, x, y);

    }

    function draw_rect(drbl) {
        if (drbl.outline == 0) {
            ctx.fillStyle = drbl.color;
            ctx.fillRect(drbl.x, drbl.y, drbl.w, drbl.h);
        } else {
            ctx.strokeStyle = drbl.color;
            ctx.lineWidth = drbl.outline;
            ctx.strokeRect(drbl.x, drbl.y, drbl.w, drbl.h);
        }
    }
    
    // set defaults then call the appropriate draw function depending on the type
    function draw_drawable(drbl) {
        if (! drbl) { console.log("none object for drawable");return; }
        if (! drbl.type) { console.log("no type for drawable");return; }
        if (drbl.type == 'text') {
            if (! drbl.text) { console.log("no text for text drawable");return; }
            if (! drbl.x) { drbl.x = 0; }
            if (! drbl.y) { drbl.y = 0; }
            if (! drbl.centeredX) { drbl.centeredX = false; }
            if (! drbl.centeredY) { drbl.centeredY = false; }
            if (! drbl.font) { drbl.font = '24px serif'; }
            if (! drbl.color) { drbl.color = '#000000'; }

            ctx.font = drbl.font;
            drbl.w = ctx.measureText(drbl.text).width;
            drbl.h = parseInt(drbl.font);
 
            draw_text(drbl);
        } else if (drbl.type == 'image') {
            if (! drbl.image) { console.log("no image for image drawable");return; }
            if (! drbl.image.complete) { return; }
            if (drbl.image.naturalWidth === 0) { return; }
            if (! drbl.x) { drbl.x = 0; }
            if (! drbl.y) { drbl.y = 0; }
            if (! drbl.scaleX) { drbl.scaleX = 1; }            
            if (! drbl.scaleY) { drbl.scaleY = 1; }
            if (! drbl.centeredX) { drbl.centeredX = false; }
            if (! drbl.centeredY) { drbl.centeredY = false; }
            if (! drbl.rotation) { drbl.rotation = 0; }
            drbl.w = drbl.image.naturalWidth*drbl.scaleX;
            drbl.h = drbl.image.naturalHeight*drbl.scaleY;
            draw_image(drbl);
        } else if (drbl.type == 'rect') {
            if (! drbl.x) { drbl.x = 0; }
            if (! drbl.y) { drbl.y = 0; }
            if (! drbl.w) { drbl.x = 10; }
            if (! drbl.h) { drbl.y = 10; }
            if (! drbl.color) { drbl.color = '#000000'; }
            if (! drbl.outline) { drbl.outline = 0; }            
            draw_rect(drbl);
        } else {
            console.log("Drawable type '" + drbl.type.toString() + "' not implemented");
        }
        if (drbl.track)
        {
            drawTrackBox(drbl);
        }
    }
    
    function tick() {
        controlpadUpdate();
        let msgs = outgoingMessages();
        for (msg of msgs) {
            console.log("sending <" + msg + ">");
            ws.send(msg);
        }
        let drbls = getDrawables();
            if (drbls.length > 0) {
                ctx.fillStyle = "#808080";
                ctx.fillRect(0, 0, canvas.width, canvas.height);
                trackedDrbls = [];
                for (drbl of drbls) {
                    draw_drawable(drbl);
                }

                if (trackedDrbls > 0) {

                    trackedDrbls.forEach(drbl => {
                        drawTrackBox(drbl);
                    })
                }
                
            

            }
    }


    function drawTrackBox(drbl) {
        if(!drbl.msg){
            console.log("Trackbox Made without message");
            return;
        }

        var x = drbl.x;
        var y = drbl.y;

        switch (drbl.type) {
            case "image":
                if (drbl.centeredX) {
                    x = drbl.x - drbl.scaleX*drbl.image.width/2;
                }
                if (drbl.centeredY) {
                    y = drbl.y - drbl.scaleY*drbl.image.height/2;
                }
                
                break;

            case "text":
                if (drbl.centeredX) {
                    x -= drbl.w / 2;
                }
                if (drbl.centeredY) {
                    y += drbl.h / 3;
                }
                break;
            default:
                break;
        }

        drbl.trackColor = getRandomColor();
        trackedDrbls.push(drbl);

        hitCtx.fillStyle = drbl.trackColor;
  



        hitCtx.fillRect(x, y, drbl.w, drbl.h);
    }

    function getRandomColor() {
        const r = Math.round(Math.random() * 255);
        const g = Math.round(Math.random() * 255);
        const b = Math.round(Math.random() * 255);
        return `rgb(${r},${g},${b})`;
    }

    function checkTrackedDrbls(x,y) {
  

        const pixel = hitCtx.getImageData(x,y,1,1).data;

        const color = `rgb(${pixel[0]},${pixel[1]},${pixel[2]})`;

        trackedDrbls.forEach(drbl => {
            if (hasSameColor(color,drbl.trackColor)){
                messages.push(drbl.msg);
            }
        })
    }

    function hasSameColor(color, drbl){
        return color === drbl;
    }
    controlpadStart(canvas.width, canvas.height);
    setInterval(tick, 33);
}
