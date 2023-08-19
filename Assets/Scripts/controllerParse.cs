using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class controllerParse : MonoBehaviour
{
    public static void messageParse(string client, string msg)
    {
        Debug.Log(msg);

        var messages = msg.Split(':');
    }

    private void newPlayer(string playerName)
    {

    }
}
