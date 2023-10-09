using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System;

public class controllerParse : MonoBehaviour
{
    [SerializeField] gameController gameController;     //Tie into gameController
    public List<string> clientsSent = new List<string>(); //Temporary list to keep the game from sending multiple requests for newPlayer
    public bool someonesAskingForMoneyAgain = false;     //Bool to determine if we need to go to the asking for money screen

    public string playerAskingForMoney; //String of player who is asking for money for UI purposes

    public string askAmount; //How much they are asking for

    public bool waitingToReadyUp = false; //If everyone hasn't readied up yet
    

    //Function for when the game recieves a message
    public async void messageParse(string client, string msg)
    {
        var messages = msg.Split(':');   // Parse msg by a colon

        //If the player is trying to join the game, register their name and create a new player item for them
        if (messages[0] == "NewPlayer") 
        {
            string playerName = messages[1];
            gameController.newPlayer(playerName, client);

            messageParse(client,"RequestState"); //Lastly, send them a refresh message to request their state again
        }

        //Associated the client with a player object
        var fromPlayer = grabPlayer(client);

        //If that player object is null, they haven't been added yet. This works as somewhat a delay for the server
        if (fromPlayer is null)
        {
            //If client has already been sent a "ReadyToJoin" message
            if (clientsSent.Contains(client))
            {
                Debug.Log("Found in Client List");
                return;
            }
            //Send a "ReadyToJoin" message, which is a validated attempt to join
            else
            {
                clientsSent.Add(client);
                controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");
                return;
            }
        }

        //If the player object was successfully identified, make a switch on their message

        switch (messages[0])
        {
            //Special Game State Requests that come before the normal game mode

            //Host is trying to start the game
            case "StartGame":

                int startMoney = int.Parse(messages[1]);
                int ante = int.Parse(messages[3]);

                if(messages[2]=="blinds")
                {
                    gameController.blindPlay = true;
                }
                else if(messages[2] == "ante") {
                    gameController.antePlay = true;
                }

                gameController.startGame(startMoney,ante);
                break;

            //Player is sending a response from their turn
            case "PlayerResponse":
                string action = messages[1];
                int amount;
                switch (action)
                {
                    case "Fold":
                        gameController.fold();
                        break;
                    case "Call":
                    case "Check":
                        gameController.call();
                        break;
                    case "Raise":
                    case "All In":
                        amount = int.Parse(messages[2]);
                        gameController.raise(amount);
                        break;
                }
                break;
            
            //Player has updated one of their custom settings
            case "Setting":
                switch (messages[1])
                {
                    case "playerColor":
                        string plyclrs = listColors();
                        if (plyclrs.Contains(messages[2]))
                        {
                            fromPlayer.playerColor = messages[2];
                        }
                        else
                        {
                            controlpads_glue.SendControlpadMessage(client,"alert:Please select a different color. That color has already been selected.");
                        }
                        break;
                    case "playerName":
                        fromPlayer.username = messages[2];
                        fromPlayer.name = messages[2];
                        break;
                    case "autoSitOut":
                        if(messages[2] == "true")
                        {
                            fromPlayer.autoSitOut = true;
                        }
                        else 
                        {
                            fromPlayer.autoSitOut = false;
                        }
                        break;

                    default:
                        Setting sound = new Setting(messages[1], messages[2]);
                        fromPlayer.CustomSettings.RemoveAll(s => s.name == messages[1]);
                        fromPlayer.CustomSettings.Add(sound);
                        break;
                }
                break;

            //Player is grabbing a list of colors that they can be
            case "AvailableColors":
                string playerColors = listColors();
                controlpads_glue.SendControlpadMessage(client,string.Join(":", playerColors));
                break;

            //Player has made a decision on ante'ing up
            case "playingRound":
                if (messages[1] == "Sitting")
                {
                    fromPlayer.playRound = false;

                }
                else 
                {
                    fromPlayer.playRound = true;

                }
                fromPlayer.pregameResponded = true;
                break;
            
            //Player has readied up
            case "readyUp":
                fromPlayer.readyForNextRound = true;
                fromPlayer.readyResponded = true;
                fromPlayer.status ="Waiting for all players to respond";
                UpdateStatus(fromPlayer.ID);
                break;
            
            //Player is requesting money from other players
            case "requestMoney":
                askAmount = messages[1];
                // Debug.Log("Ask Amount: " + askAmount);
                // Debug.Log("Client: " + client);
                await moneyRequest(client,askAmount);
                break;

            //Player is sending their response to a player's request for money
            case "moneyResponse":
                if(messages[1] == "1")
                {
                    fromPlayer.moneyResponse = true;
                }
                else
                {
                    fromPlayer.moneyResponse = false;

                }
                fromPlayer.moneyResponded = true;
                break;
            
            //Player is quitting the game
            case "quit":
                var playerList = gameController.getPlayerList();

                if(fromPlayer.isHost)
                {
                    playerList[1].isHost = true;
                    Debug.Log("New host is " + playerList[1].username);

                }
                GameObject playerObject = GameObject.Find(fromPlayer.username);
                Debug.Log(fromPlayer.username + " removed from the game");
                playerList.RemoveAll(p => p.ID == client);
                if(gameController.getCurretPlayer().ID == client)
                {
                    StartCoroutine(RemovePlayer(() =>
                    {
                        Destroy(playerObject);
                    }
                    ));
                }
                else
                {
                        Destroy(playerObject);
                }
                controlpads_glue.SendControlpadMessage(client,"alert:" + "Successful Quit. Close your window at any time");
                break;

            //Normal refresh state request to update where they are in the game. Can be used as a reset/refresh
            case "RequestState":
                //Update the clients custom settings
                UpdateSettings(client);
                //Update the clients GameState
                GameState(client);
                //Update the clients Status Baar
                UpdateStatus(client);
                break;
        }
    }

    //Normal GameState Request for the player refreshing their screen
    public void GameState(string client){
        var player = grabPlayer(client); //Grab the correct player object from client
        var stateName = ""; //Initialize statename
        List<string> variables = new List<string>(); //Initialize a list of variables to be sent to the client
        variables.Add(player.username); //Add the user's name from saved player object
        variables.Add(player.playerColor); //Add the player's color from saved player object

        //If player is host, send a seperate message to designate that
        if (player.isHost)
        {
            controlpads_glue.SendControlpadMessage(player.ID,"host");
        }

        //If the game has not started yet
        if(!gameController.gameState)
        {
            stateName = "JoinedWaitingToStart:";  
        }

        //If the game has started
        else 
        {
            variables.Add(player.money.ToString()); //Add the player's money from saved player object
            variables.Add((gameController.getRevealBet() - player.bettedRound).ToString()); //Add the current bet from saved player object

            //Add player cards
            foreach(var card in player.getHoleCards())
            {
                variables.Add(card.suit.ToString() + "-" + card.rank.ToString());
            }
            variables.Add(gameController.getAnte().ToString()); //Add the minimum bet from saved game object

            //If the game is waiting to ready up and the player hasn't responded yet
            if(waitingToReadyUp & !player.readyResponded)
            {
                stateName = "ReadyUp:";
            }

            //Else if the ask for money screen is triggered and the player hasn't responded yet
            else if(someonesAskingForMoneyAgain & !player.moneyResponded)
            {
                stateName="MoneyRequest:";
            }

            //Else if the pregaming state is triggered(ante'ing)
            else if(gameController.PreGame())
            {
                if (!player.pregameResponded)
                {
                    stateName = "PlayingPregame:";
                }
                else {
                    stateName = "JoinedWaiting:";
                }

            }

            //Else if player has folded
            else if (player.folded)
            {
                stateName = "PlayingFolded:";
            }

            //Else if it is the player's turn
            else if (gameController.getCurretPlayer().ID == client){
                stateName = "PlayingPlayerTurn:";
            }

            //Otherwise, they are playing but it's not their turn
            else {
                stateName = "PlayingWaiting:";
            } 
        }
        string variableString = string.Join(":", variables.ToArray()); //Bundle up the variables and send with a colon
        controlpads_glue.SendControlpadMessage(player.ID, "state:" + stateName + variableString); //Send the player a message with their gamestate and the key variables
    }
    
    //Async task to initialize the money request screen. Waits for each player to respond before proceeding
    private async Task moneyRequest(string client, string amount)
    {
        someonesAskingForMoneyAgain = true;
        var RequestingPlayer = grabPlayer(client);
        var playerList = gameController.getPlayerList();

        foreach (var player in playerList)
        {
            if (player.ID == client)
            {
                playerAskingForMoney = player.username;
                player.moneyResponded = true;
                player.moneyResponse = true;        
            }
            else
            {   
                player.moneyResponded = false;
                player.status = ("Approve or reject " + playerAskingForMoney+ "'s request for $" + askAmount);
            }
        }
        gameController.refreshPlayers(playerAskingForMoney + " is asking for money again");
        foreach (var player in playerList)
        {
            do
            {
                // Debug.Log("Waiting on " + player.username);
                // Wait 1 seconds, slow poll
                await Task.Delay(1000);
                
            } while (!player.moneyResponded);
        }

        someonesAskingForMoneyAgain = false;
        bool everyoneAgrees = true;

        foreach (var player in playerList)
        {
            player.status="";
            if(!player.moneyResponse)
            {
                everyoneAgrees = false;
                
            }
        }

        string moneyMessage;
        if(everyoneAgrees)
        {
            moneyMessage = "Request for money was approved";
            RequestingPlayer.money += Int32.Parse(amount);
            controlpads_glue.SendControlpadMessage(client,"refresh: You got your Money!");


        }
        else
        {
            moneyMessage = "Request for money didn't get approved";
        }
        someonesAskingForMoneyAgain = false;
        foreach (var player in playerList)
        {
            controlpads_glue.SendControlpadMessage(player.ID,"alert:" + moneyMessage);
        }
    }

    //Updates the players custom settings by sending a : delimited list
    public void UpdateSettings(string client){
        var player = grabPlayer(client);
        foreach (var setting in player.CustomSettings)
        {
            controlpads_glue.SendControlpadMessage(player.ID, "setting:" + setting.name + ":" + setting.value);

        }
    }

    //Updates the player status banner by the player's current status
    public void UpdateStatus(string client) 
    {
        var player = grabPlayer(client);
        if(!String.IsNullOrEmpty(player.status))
        {
            controlpads_glue.SendControlpadMessage(player.ID, "UpdateStatus:" + player.status);
        }
    }

    //Returns a string of all available colors for the user to select from
    public string listColors(){
        List<string> playerColors = new List<string>();
        playerColors.Add("colors");
        foreach (var color in gameController.colors)

        
        {
            bool colorFound = false;
            foreach (var player in gameController.getPlayerList())
            {
                if (player.playerColor == color){
                    colorFound = true;
                    break;
                }
            }
            if(!colorFound)
            {
                playerColors.Add(color);
            }
            
        }

        return string.Join(":", playerColors);



    }

    //Returns a player object that matches the provided client
    public playerController grabPlayer(string client) {
        foreach (var player in gameController.getPlayerList()) 
        {
            string ip = player.ID;
            //player is recognized
            if(ip == client) {
                return player;
            }
        }
        return null;
    }

    //Async task for a player being removed from the game
    private IEnumerator RemovePlayer(System.Action onComplete)
    {
    gameController.getCurretPlayer().IsMoving = true;
    gameController.triggerNextTurn();

    // Call the onComplete callback when exitFrame is done
    yield return new WaitUntil(() => !gameController.getCurretPlayer().IsMoving); // Modify this condition as needed
    
    onComplete?.Invoke();
    }



}


