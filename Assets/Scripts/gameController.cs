using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class gameController : MonoBehaviour
{
    private List<playerController> playerList = new List<playerController>();
    private List<string> turnOrder = new List<string>();
    private int tottalMoney = 0;
    private int ante;
    private static bool gameRunning = false;
    private bool increasingAnte = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAnte(int ante) { this.ante = ante; }

    public void startGame(int startMoney) 
    { 
        if (gameRunning)
        {
            Debug.Log("There is already a game playing." +
                "Please end the current game before starting a new one.");
            return;
        }

        gameRunning = true;
        tottalMoney = startMoney * playerList.Count;

        Debug.Log("A game of Texas Hold'Em Has begun");
        Debug.Log("Tottal Money: " + tottalMoney);
        
        foreach (var playerController in playerList)
        {
            playerController.setMoney(startMoney);
        }
    }

    public void initializePlayer(playerController player)
    {
        playerList.Add(player);
    }

    public void anteUP(string playerName, bool playing)
    {
        if (playing) { Debug.Log(playerName + " Is playing this round"); }
    }

    public void check()
    {

    }

    public void call()
    {

    }
    public void raise(int amount)
    {
        tottalMoney += amount;
    }

    public void fold(string username)
    {

    }
}
