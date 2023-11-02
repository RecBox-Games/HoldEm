// ---- UI ----

//UI Includes
//Drawing Screens

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
function drawReadyUp(){
    drawStatus();
    topMenu();
    drawCardBack();
    drawPeek();

    readyButton.style.display= "block";
}

function readyUp()
{
    messages.push('readyUp')
    readyButton.style.display="none";
}

function drawPlayingFolded()
{
    playerStatus = "Sitting Out this Round";
    topMenu();
    drawStatus();
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
    drawCenterMessage("Fold by Swiping Up");
    setTimeout(fadeOutEffect.bind(null,centerMessage),1000);

}

function drawGameFinished() {
    playerStatus = "It's Your Turn!";
    topMenu();
    drawStatus();


    
}


function drawMoneyRequest() {
    topMenu();
    moneyMenu.style.display="block";

}

// ---- Minor Drawing Functions ----

//Draws the card backs

function drawCardBack() {
    cardElement.style.display='flex';
}

function drawCenterMessage(str)
{
    
    centerMessage.innerHTML=str;
    centerMessage.style.display = 'block';
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



function updateActionButton(){
    text = action + ": " + formatter.format(playerCall);
    actionButton.innerHTML=text;

    

}


//Handler for when the peek button is pressed
function peekButtonTouch(){
    cardFlipped = !cardFlipped;
    $("#card").flip(cardFlipped);

}


function fadeOutEffect(element) {

    let fadeTarget = element;
    fadeTarget.style.opacity=1;

    let fadeEffect = setInterval(function () {
        if (!fadeTarget.style.opacity) 
        {
            fadeTarget.style.opacity = 1;
        }
        if (fadeTarget.style.opacity > 0) 
        {
            fadeTarget.style.opacity -= 0.1;
        } 
        else 
        {
            clearInterval(fadeEffect);
            fadeTarget.style.display='none';
        }

    }, 100);
}

function drawStatus() {
    statusField.innerHTML = playerStatus;
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
