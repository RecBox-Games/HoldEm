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
    public static bool gameStarted = true;

    public static int gvCall = 0;

    

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

        if(fromPlayer is null)
        {
            controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");

        }

        else{

            if (messages[0] == "RequestState") {
            
                GameState(fromPlayer.getIP(), fromPlayer.getName(),fromPlayer.getMoney().ToString(),gvCall.ToString(), fromPlayer.isTurn());
            }
            
        }

        

    }


    public void newPlayer(string playerName, string client)
    {
        Debug.Log("Player" + playerName + "Added");

        // Create a new Player object from the Player prefab and name it the new players name
        Object playerObj = Instantiate((playerPrefab), new Vector3(0, 0, 10), Quaternion.identity);
        playerObj.name = playerName;

        // Get the playerController and assign anything new to the player
        playerController player = playerObj.GetComponent<playerController>();
        playersInGame.Add(player);
        player.setName(playerName);
        player.setPlayerIP(client);
        player.setPlayerNumber(playersInGame.Count);
        // gameController.initializePlayer(player);


        // This updates a UI with the new player whos playing
        playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
    }


    public void GameState(string ip, string username, string playerMoney, string call, bool isPlayerTurn){

        var stateName = "";
        List<string> variables = new List<string>();
        variables.Add(username);


        if(!gameController.getGameState())
        {
            stateName = "Joined Waiting to Start:";
            
        }

        else {

            variables.Add(playerMoney);
            variables.Add(call);
            

            if(isPlayerTurn){
                stateName = "PlayingPlayerTurn:";
            }

            else {
                stateName = "PlayingWaiting:";
            }
            
        }
        string variableString = string.Join(":", variables.ToArray());
        Debug.Log(variableString);

        controlpads_glue.SendControlpadMessage(ip, "state:" + stateName + variableString);

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
