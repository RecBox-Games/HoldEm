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
                    gameController.raise(amount);
                    break;              
            }
        }

        if (fromPlayer is null)
        {
            Debug.Log("Sent:ReadyToJoin");
            controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");

        }

        else {

            if (messages[0] == "RequestState") {

                Debug.Log("betted" + ":" + fromPlayer.betted.ToString());
            

            
                GameState(
                fromPlayer.ID, 
                fromPlayer.username,
                fromPlayer.money.ToString(),
                gameController.getCurrentCall().ToString(), 
                fromPlayer.isPlayerTurn,
                fromPlayer.playerNumber,
                fromPlayer.getHoleCards()
                );
            }
            
        }

        

    }


    public void GameState(string ip, string username, 
    string playerMoney, string call, bool playerTurn, 
    int playerNumber, List<Card> cards){

        var stateName = "";
        List<string> variables = new List<string>();
        variables.Add(username);


        if(!gameController.getGameState())
        {
            if (playerNumber == 1)
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
            foreach(var card in cards)
            {
                variables.Add(card.suit.ToString() + "-" + card.rank.ToString());
            }

            Debug.Log(playerTurn);
            if(playerTurn){
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
            string ip = player.ID;
            //player is recognized
            if(ip == client) {
                return player;
            }
        }
        return null;
    }
}
