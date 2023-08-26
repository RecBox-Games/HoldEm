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
    private List<playerController> playersInGame = new List<playerController>();
    public static bool playerTurn = true;
    

    public void messageParse(string client, string msg)
    {
        
        // Parse msg by ':'
        var messages = msg.Split(':');

        if (messages[0] == "NewPlayer") 
        {
            string playerName = messages[1];
            gameController.newPlayer(playerName, client);
            checkIfGameStarted(client, playerName);
        }

        var fromPlayer = grabPlayer(client);

        if(fromPlayer is null)
        {
            controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");
        } else if (messages[0] == "RequestState") 
        {
            checkIfGameStarted(fromPlayer.getIP(), fromPlayer.getName());
        }
    }

    public void checkIfGameStarted(string ip, string username){
        var stateName = "";

        if(!gameController.getGameState())
        {
            stateName = "Joined Waiting to Start:";
            
        }

        else {

            if(playerTurn){
                stateName = "PlayingPlayerTurn:";
            }

            else {
                stateName = "PlayingWaiting:";
            }
            
        }

        controlpads_glue.SendControlpadMessage(ip, "state:" + stateName + username);

    }

    public playerController grabPlayer(string client) {
        foreach (var player in playersInGame) 
        {
            var ip = player.getIP();
            //player is recognized
            if(ip == client) {
                return player;
            }
        }
        return null;
    }
}
