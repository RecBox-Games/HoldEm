using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class controllerParse : MonoBehaviour
{
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

    private void newPlayer(string playerName)
    {

    }
}
