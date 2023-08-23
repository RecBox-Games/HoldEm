using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class controllerParse : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerUI;
    private List<string> clientList = new List<string>();
    static bool gameStarted = true;

    public void messageParse(string client, string msg)
    {
        // Parse msg by 
        var messages = msg.Split(':');

        if (messages[0] == "RequestState") {

            if (!clientList.Contains(client))
            {
                clientList.Add(client);
                // newPlayer();        
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
            */
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
        player.setPlayerNumber(clientList.Count);

        playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
    }
}
