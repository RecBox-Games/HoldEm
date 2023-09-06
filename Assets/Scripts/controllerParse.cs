using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class controllerParse : MonoBehaviour
{

    [SerializeField] gameController gameController;
    public static int gvCall = 20;

    

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

        // This message should come in like "StartGame:Money:Ante"
        // Money = amount host player sets
        if (messages[0] == "StartGame")
        {
            gameController.startGame(int.Parse(messages[1]));
        }
        
        if (messages[0] == "PlayerResponse")
        {
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
                    gameController.raise(amount-gameController.getCurrentCall());
                    break;              
            }
        }

        if (fromPlayer is null)
        {
            controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");

        }

        else {

            if (messages[0] == "RequestState") {
            
                GameState(fromPlayer.getIP(), fromPlayer.getName(),
                fromPlayer.getMoney().ToString(),gvCall.ToString(), 
                fromPlayer.isTurn(), fromPlayer.getPlayerNumber());
            }
            
        }

        

    }


    public void GameState(string ip, string username, 
    string playerMoney, string call, bool isPlayerTurn, 
    int number){

        var stateName = "";
        List<string> variables = new List<string>();
        variables.Add(username);


        if(!gameController.getGameState())
        {
            if (number == 1)
            {
                stateName = "JoinedHost:";
            }
            else {
                stateName = "JoinedWaitingToStart:";
            }
            
        }

        else {

            variables.Add(playerMoney);
            variables.Add(gameController.getCurrentCall().ToString());

            //Need to add card variable tie in here
            variables.Add("Hearts-King");
            variables.Add("Clubs-Queen");

            if((number-1) == gameController.getPlayerTurn()){
                stateName = "PlayingPlayerTurn:";
            }

            else {
                stateName = "PlayingWaiting:";
            }
            
        }
        string variableString = string.Join(":", variables.ToArray());

        controlpads_glue.SendControlpadMessage(ip, "state:" + stateName + variableString);

    }

    public playerController grabPlayer(string client) {
        foreach (var player in gameController.getPlayerList()) 
        {
            string ip = player.getIP();
            //player is recognized
            if(ip == client) {
                return player;
            }
        }
        return null;
    }
}
