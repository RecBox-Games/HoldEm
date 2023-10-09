// ----- Settings -----

//Settings includes
//Toggling options in the user menu
//Handling those setting changes



function openPlayerMenu()
{
    document.getElementById('playerMenu').style.display='block';
    messages.push("AvailableColors");
}

function changeColor(event)
{
    event.preventDefault();
    playerColor = event.target.value;
    
    updateColor();
    sendSetting("playerColor",event.target.value);
}

function toggleSound() {
    soundSetting = !soundSetting;
    updateSound();
    sendSetting('soundOn',soundSetting);
}

function toggleAutoSitOut(){
    autoSitOut = !autoSitOut
    updateAutoSitOut();

    sendSetting('autoSitOut',autoSitOut);

}

function changeName() {
    const newName = prompt("What would you like your new name to be?");
    if(newName)
    {
        sendSetting('playerName', newName);
        stateRequest();
    }
}

function updateAutoSitOut()
{
    if(autoSitOut == false)
    {
        sitOutButton.innerHTML = "Auto Sit Out: Off";
    }
    else{
        sitOutButton.innerHTML = "Auto Sit Out: On";
    }
}

function updateSound() {
    if(soundSetting)
    {
        soundButton.innerHTML="Sound On";

    }
    else {
        soundButton.innerHTML="Sound Off";
    }

}

function sendSetting(setting, variable){
    let msg = "Setting:" + setting + ":" + variable;
    messages.push(msg);
}

function updateSettings(sections) {
    switch (sections[1]) {
        case "soundOn":
            soundSetting = convertToBool(sections[2]);
            updateSound();
            break;
        case "autoSitOut":
            autoSitOut = convertToBool(sections[2]);
            updateAutoSitOut();
            break;

    }
}