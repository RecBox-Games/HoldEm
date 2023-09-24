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
    [SerializeField] gameController gameController;

    public List<string> clientsSent = new List<string>();

    public bool someonesAskingForMoneyAgain = false;

    public string playerAskingForMoney;

    public string askAmount;

    public async void messageParse(string client, string msg)
    {
        // Parse msg by
        var messages = msg.Split(':');

        if (messages[0] == "NewPlayer") 
        {
            string playerName = messages[1];
            gameController.newPlayer(playerName, client);

            messageParse(client,"RequestState");
        }

        var fromPlayer = grabPlayer(client);

        if (fromPlayer is null)
        {
            if (clientsSent.Contains(client))
            {
                Debug.Log("Found in Client List");
                return;
            }
            else
            {
                clientsSent.Add(client);
                controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");
                return;

            }
        }


        switch (messages[0])
        {
            //Special Game State Requests
            case "StartGame":

                int startMoney = int.Parse(messages[1]);
                int ante = int.Parse(messages[3]);

                if(messages[2]=="blinds")
                {
                    gameController.blindPlay = true;
                }
                else {
                    gameController.antePlay = true;
                }
                gameController.startGame(startMoney,ante);
                // if(messages[2] == "ante")
                // {
                //     int ante = int.Parse(messages[3]);
                //     gameController.startGame(startMoney,ante);  
                // }
                // else 
                // {
                //     gameController.bigBlind = int.Parse(messages[4]);
                //     gameController.smallBlind = int.Parse(messages[5]);
                //     gameController.blindPlay = true;
                //     gameController.startGame(startMoney);
                // }
                break;

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
                    default:
                        Setting sound = new Setting(messages[1], messages[2]);
                        fromPlayer.CustomSettings.RemoveAll(s => s.name == messages[1]);
                        fromPlayer.CustomSettings.Add(sound);
                        break;
                }
                break;

            case "AvailableColors":
                string playerColors = listColors();
                controlpads_glue.SendControlpadMessage(client,string.Join(":", playerColors));
                break;

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
            
            case "requestMoney":
                askAmount = messages[1];
                Debug.Log("Ask Amount: " + askAmount);
                Debug.Log("Client: " + client);
                await moneyRequest(client,askAmount);
                break;
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
                
            //Normal State Request
            case "RequestState":
                GameState(client);
                UpdateSettings(client);
                UpdateStatus(client);
                break;

        }

    }


    public void GameState(string client){
        var player = grabPlayer(client);


        var stateName = "";
        List<string> variables = new List<string>();
        variables.Add(player.username);
        variables.Add(player.playerColor);


        if (player.playerNumber == 1)
        {
            player.isHost = true;
            controlpads_glue.SendControlpadMessage(player.ID,"host");
        }
        if(!gameController.gameState)
        {
            stateName = "JoinedWaitingToStart:";  
        }
        else {

            variables.Add(player.money.ToString());
            variables.Add((gameController.getCurrentCall()-player.bettedRound).ToString());

            
            //Need to add card variable tie in here
            foreach(var card in player.getHoleCards())
            {
                variables.Add(card.suit.ToString() + "-" + card.rank.ToString());
            }

            if(someonesAskingForMoneyAgain & !player.moneyResponded)
            {
                stateName="MoneyRequest:";

            }
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



            else if (player.folded)
            {
                stateName = "PlayingFolded:";
            }
            else if (gameController.getCurretPlayer().ID == client){
                stateName = "PlayingPlayerTurn:";
            }

            else {
                stateName = "PlayingWaiting:";
            }

            variables.Add(gameController.getAnte().ToString());


            
        }
        string variableString = string.Join(":", variables.ToArray());

        controlpads_glue.SendControlpadMessage(player.ID, "state:" + stateName + variableString);
        
       

    }
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
                Debug.Log("Waiting on " + player.username);
                //Wait 1 seconds, slow poll
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


    public void UpdateSettings(string client){
        var player = grabPlayer(client);
        foreach (var setting in player.CustomSettings)
        {
            controlpads_glue.SendControlpadMessage(player.ID, "setting:" + setting.name + ":" + setting.value);

        }
    }

    public void UpdateStatus(string client) 
    {
        var player = grabPlayer(client);
        if(!String.IsNullOrEmpty(player.status))
        {
            controlpads_glue.SendControlpadMessage(player.ID, "UpdateStatus:" + player.status);
        }
    }

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
}
