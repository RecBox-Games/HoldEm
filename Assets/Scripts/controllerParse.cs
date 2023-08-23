using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class controllerParse : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerUI;
    private List<playerController> playersInGame = new List<playerController>();
    public static bool gameStarted = false;
    

    public void messageParse(string client, string msg)
    {
        
        // Parse msg by

        var messages = msg.Split(':');

        if (messages[0] == "NewPlayer") {
            string playerName = messages[1];
            newPlayer(playerName, client);
            checkIfGameStarted(client, playerName);

        }

        var fromPlayer = grabPlayer(client);

        if(fromPlayer is null)
        {
            controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");

        }

        else{

            if (messages[0] == "RequestState") {
            
                checkIfGameStarted(fromPlayer.getIP(), fromPlayer.getName());
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

        playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
    }

    public void checkIfGameStarted(string ip, string username){


        if(!gameStarted)
        {
            controlpads_glue.SendControlpadMessage(ip, "state:JoinedWaitingToStart:" + username);
            
        }

        else {
            controlpads_glue.SendControlpadMessage(ip,
            "state:PlayingWaiting" + ":" + username);
        }  
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
