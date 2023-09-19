using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class controllerParse : MonoBehaviour
{
    [SerializeField] gameController gameController;


    public void messageParse(string client, string msg)
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
            Debug.Log("Sent:ReadyToJoin");
            controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");
            return;
        }


        switch (messages[0])
        {
            //Special Game State Requests
            case "StartGame":
                gameController.startGame(int.Parse(messages[1]));
                break;

            case "PlayerResponse":
                string action = messages[1];
                int amount = int.Parse(messages[2]);
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
                        gameController.raise(amount);
                        break;              
                }
                break;
            case "Setting":
                switch (messages[1])
                {
                    case "playerColor":
                        fromPlayer.playerColor = messages[2];
                        break;
                    default:
                        Setting sound = new Setting(messages[1], messages[2]);
                        fromPlayer.CustomSettings.RemoveAll(s => s.name == messages[1]);
                        fromPlayer.CustomSettings.Add(sound);
                        break;
                }
                break;

            case "AvailableColors":
                listColors(client);
                break;
                
            //Normal State Request
            case "RequestState":
                GameState(client);
                UpdateSettings(client);
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


            if(gameController.PreGame())
            {
                stateName = "PlayingPregame";

            }
            else if (player.isPlayerTurn){
                stateName = "PlayingPlayerTurn:";
            }

            else {
                stateName = "PlayingWaiting:";
            }


            
        }
        string variableString = string.Join(":", variables.ToArray());

        controlpads_glue.SendControlpadMessage(player.ID, "state:" + stateName + variableString);
        
       

    }

    public void UpdateSettings(string client){
        var player = grabPlayer(client);
        foreach (var setting in player.CustomSettings)
        {
            controlpads_glue.SendControlpadMessage(player.ID, "setting:" + setting.name + ":" + setting.value);

        }
    }

    public void listColors(string client){
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

        controlpads_glue.SendControlpadMessage(client,string.Join(":", playerColors));



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
