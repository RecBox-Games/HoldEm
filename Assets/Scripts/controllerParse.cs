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


        if(!gameController.gameState)
        {
            if (player.playerNumber == 1)
            {
                stateName = "JoinedHost:";
            }
            else {
                stateName = "JoinedWaitingToStart:";
            }
            
        }

        

        else {

            variables.Add(player.money.ToString());
            variables.Add((gameController.getCurrentCall()-player.bettedRound).ToString());

            //Need to add card variable tie in here
            foreach(var card in player.getHoleCards())
            {
                variables.Add(card.suit.ToString() + "-" + card.rank.ToString());
            }

            if(player.isPlayerTurn){
                stateName = "PlayingPlayerTurn:";
            }

            else {
                stateName = "PlayingWaiting:";
            }
            variables.Add(player.playerColor);
            
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
