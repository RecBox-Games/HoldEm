using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class controllerParse : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    static string playerName = "Greg";
    static bool gameStarted = true;

    public static void messageParse(string client, string msg)
    {

        var messages = msg.Split(':');

        if (messages[0] == "RequestState") {

            if (!PlayerList.playersInGame.Contains(client))
            {
                PlayerList.playersInGame.Add(client);
                Debug.Log("Player" + playerName + "Added");
                controlpads_glue.SendControlpadMessage(client, "state:ReadyToJoin");          
            }

            else {

                if(!controllerParse.gameStarted) {
                    controlpads_glue.SendControlpadMessage(client, "state:JoinedWaitingToStart"+ ":" + controllerParse.playerName);
                }

                else {
                    controlpads_glue.SendControlpadMessage(client,
                    "state:PlayingWaiting"+ ":" + controllerParse.playerName);

                }
            

            }




        }
    }

    public void newPlayer(string playerName)
    {
        // Create a new Player object from the Player prefab and name it the new players name
        Object playerObj = Instantiate((playerPrefab), new Vector3(0, 0, 10), Quaternion.identity);
        playerObj.name = playerName;

        // Get the playerController and assign anything new to the player
        playerController player = playerObj.GetComponent<playerController>();
        player.setName(playerName);
    }
}
