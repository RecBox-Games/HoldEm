using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class stateRequest : MonoBehaviour
{
    [SerializeField] controllerParse controllerParse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void waitingToReadyUp(bool ready)
    {
        controllerParse.waitingToReadyUp = ready;
    }

    public void sendPlayerSuccess(playerController player)
    {
        Debug.Log(player.name + " has been added successfully.");
    }

    public void sendPlayerUpdate(playerController player)
    {
        controlpads_glue.SendControlpadMessage(player.ID, "refresh");
    }


    // =============================== GAME STATE ERRORS ===============================
    private void gameStateError(string message)
    {
        string errorMessage = "GameStateError: " + message;
        Debug.Log(errorMessage);
    }

    public void sendGameStateError(string message)
    {
        gameStateError(message);
    }

    public void gameInProgressError()
    {
        gameStateError("Game in progress.");
    }




    // =============================== PLAYER ERRORS ===============================
    private void playerError(string message) 
    {
        string errorMessage = errorMessage = "PlayerError: " + message;
        Debug.Log(errorMessage);
    }

    public void sendPlayerError(string message)
    {
        playerError(message);
    }

    public void maxPlayerError()
    {
        playerError("Maximum players capacity reached.");
    }

    public void minPlayerError()
    {
        playerError("Not enough players.");
    }
}
