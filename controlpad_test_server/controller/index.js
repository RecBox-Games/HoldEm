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

const hitCanvas = document.createElement('canvas');
const hitCtx = hitCanvas.getContext('2d');

var touch_recognized = false

function updateScreenHeightandWidth() {
    SCREEN_HEIGHT = window.innerHeight - offset;
    SCREEN_WIDTH = window.innerWidth - offset;
    canvas.width = SCREEN_WIDTH;
    canvas.height = SCREEN_HEIGHT;
    hitCanvas.width = SCREEN_WIDTH;
    hitCanvas.height = SCREEN_HEIGHT;

    ctx.fillStyle = "#808080";
    ctx.fillRect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
    ctx.fillStyle = "#000000";
}

function screenChange() {
    updateScreenHeightandWidth();
    console.log('resize fill');
    onFlip(SCREEN_WIDTH, SCREEN_HEIGHT, ORIENTATION);

}    
window.addEventListener("resize", (event) => {
    screenChange();
});
screen.orientation.addEventListener("change", (event) => {
    ORIENTATION = screen.orientation;
});
  


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
var ORIENTATION = screen.orientation;

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

    function draw_image(drbl,context) {

        context.setTransform(drbl.scaleX, 0, 0, drbl.scaleY, drbl.x, drbl.y); // sets scale and origin
        context.rotate(drbl.rotation);
        context.drawImage(drbl.image, 0, 0);
        context.setTransform(1,0,0,1,0,0);
    }

    function draw_text(drbl,context) {

        context.font = drbl.font;

        context.fillStyle = drbl.color;

        context.fillText(drbl.text, drbl.x, drbl.y);

    }

    function draw_rect(drbl, context) {
        if (drbl.outline == 0) {
            context.fillStyle = drbl.color;
            context.fillRect(drbl.x, drbl.y, drbl.w, drbl.h);
        } else {
            context.strokeStyle = drbl.color;
            context.lineWidth = drbl.outline;
            context.strokeRect(drbl.x, drbl.y, drbl.w, drbl.h);
        }
    }

    function draw_triangle(drbl, context) {
        context.save();
        var median = (1/3)*drbl.h;
        context.translate(drbl.x+(drbl.b/2), drbl.y + median);
        context.rotate(drbl.rotation*(Math.PI/180));
        context.beginPath();
        context.moveTo(-(drbl.b/2), - median);
        context.lineTo((drbl.b/2),- median);
        context.lineTo(0,drbl.h - median);
        context.closePath();
        context.fillStyle = drbl.color;
        context.fill();
        context.restore();



        if(drbl.outline)
        {
            context.lineWidth = drbl.outline;
            context.strokeStyle = drbl.outlineColor;
            context.stroke()
        }   

    }
    
    // set defaults then call the appropriate draw function depending on the type
    function draw_drawable(drbl) {

        needsCentered = 0;
        //Checking for bad inputs
        if (! drbl) { console.log("none object for drawable");return; }
        if (! drbl.type) { console.log("no type for drawable");return; }

        //Defaults
        if (! drbl.x) { drbl.x = 0; }
        if (! drbl.y) { drbl.y = 0; }
        if (! drbl.centeredX) { drbl.centeredX = false; }
        if (! drbl.centeredY) { drbl.centeredY = false; }
        if (! drbl.context) { drbl.context = ctx; }


        if (drbl.centeredX | drbl.centeredY){
            needsCentered = 1;
        }

        if (drbl.type == 'text') {
            if (! drbl.text) { console.log("no text for text drawable");return; }
            if (! drbl.font) { drbl.font = '24px serif'; }
            if (! drbl.color) { drbl.color = '#000000'; }

            ctx.font = drbl.font;
            drbl.w = ctx.measureText(drbl.text).width;
            drbl.h = parseInt(drbl.font);

            if(needsCentered){
                centerDrawable(drbl);
            }

            draw_text(drbl,drbl.context);

        } else if (drbl.type == 'image') {
            if (! drbl.image) { console.log("no image for image drawable");return; }
            if (! drbl.image.complete) { return; }
            if (drbl.image.naturalWidth === 0) { return; }
            if (! drbl.scaleX) { drbl.scaleX = 1; }            
            if (! drbl.scaleY) { drbl.scaleY = 1; }
            if (! drbl.rotation) { drbl.rotation = 0; }
            drbl.w = drbl.image.naturalWidth*drbl.scaleX;
            drbl.h = drbl.image.naturalHeight*drbl.scaleY;

            if(needsCentered){
                centerDrawable(drbl);
            }

            draw_image(drbl, drbl.context);

        } else if (drbl.type == 'rect') {
            if (! drbl.w) { drbl.w = 10; }
            if (! drbl.h) { drbl.h = 10; }
            if (! drbl.color) { drbl.color = '#000000'; }
            if (! drbl.outline) { drbl.outline = 0; }

            if(needsCentered){
                centerDrawable(drbl);
            }

            draw_rect(drbl,drbl.context)


        } 
        
        else if (drbl.type == 'triangle'){
            if (! drbl.b) { drbl.w = 10; }
            if (! drbl.h) { drbl.h = 10; }
            if (! drbl.color) { drbl.color = '#000000'; }

            if(needsCentered){
                centerDrawable(drbl);
            }

            draw_triangle(drbl, drbl.context);
        }
        else {
            console.log("Drawable type '" + drbl.type.toString() + "' not implemented");
        }

        if (drbl.track)
        {
            if(! drbl.trackType){
                drbl.trackType = 'rectangle'
            }
            drawTrackBox(drbl);
        }
    }
    
    function centerDrawable(drbl){
        switch(drbl.type){
            case "image":
                if(drbl.centeredX){
                    drbl.x = drbl.x - drbl.scaleX*drbl.image.width/2;                  
                }
                if(drbl.centeredY) {
                    drbl.y = drbl.y - drbl.scaleY*drbl.image.height/2;
                }
                break;
            case "text":
                if (drbl.centeredX) {
                    drbl.x -= drbl.w / 2;
                }
                if (drbl.centeredY) {
                    drbl.y += drbl.h / 3;

                }
                break;
            case "rect":
                if (drbl.centeredX) {
                    drbl.x -= drbl.w / 2;
                }
                if (drbl.centeredY) {
                    drbl.y += drbl.h / 2;
                }
                break;
            case "triangle":
                if (drbl.centeredX) {
                    
                    drbl.x -= drbl.b / 2;
                }
                if (drbl.centeredY) {
                    drbl.y -= (drbl.h / 3);

                }
                break;

            case "circle":
                if (drbl.centeredX) {
                    drbl.x -= drbl.r;
                }
                if (drbl.centeredY) {
                    drbl.y += drbl.r;
                }
                break;
            default:
                console.log("Drawable unabled to be centered");
                break;
        }

        drbl.centeredY = "false";
        drbl.centeredX = "false";
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
        if(!drbl.trackType) {
            drbl.trackType = "rectangle";
        }
        let trackDrbl = JSON.parse(JSON.stringify(drbl));
        trackDrbl.track = 0;
        trackDrbl.color = getRandomColor();
        trackDrbl.context = hitCtx;
        if(trackDrbl.type == "image" | trackDrbl.type == "text"){
            trackDrbl.type = "rect";
        }

        trackedDrbls.push(trackDrbl);
        draw_drawable(trackDrbl);
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
            if (hasSameColor(color,drbl.color)){
                switch(drbl.msg)
                {
                    case "Up":
                        UpdateMoney(1);
                        break;
                    case "Down":
                        UpdateMoney(-1);
                        break;
                    case "PlayerResponse":
                        sendResponse();
                        break;
                    default:
                        messages.push(drbl.msg);
                        break;
                }     
            }
        })
    }

    function hasSameColor(color, drbl){
        return color === drbl;
    }


    updateScreenHeightandWidth();
    controlpadStart(SCREEN_WIDTH, SCREEN_HEIGHT);
    setInterval(tick, 33);
}
