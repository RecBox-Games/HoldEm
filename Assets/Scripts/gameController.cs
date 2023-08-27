using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class gameController : MonoBehaviour
{

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerUI;
    [SerializeField] int maxPlayers;

    private List<playerController> playerList = new List<playerController>();
    private List<string> turnOrder = new List<string>();
    private int tottalMoney = 0;
    private int ante;
    private static bool gameState = false;
    private bool increasingAnte = false;
    private int currentPlayer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Getters
    public bool getGameState() { return gameState; }

    public string getTurnOrder()
    {
        string order = "";
        foreach (var name in turnOrder) { order += name + " "; }
        return order; 
    }

    public string getCurretPlayer() { return turnOrder[currentPlayer]; }


    // Setters
    public void setAnte(int ante) { this.ante = ante; }


    public void startGame(int startMoney) 
    { 
        if (gameState)
        {
            Debug.Log("There is already a game playing." +
                " Please end the current game before starting a new one.");
            return;
        } else if (playerList.Count < 2) {
            Debug.Log("There are not enough players to play a game of poker. " +
                "Please add more players then continue.");
            return;
        }

        gameState = true;
        initializeTurnOrder();
        tottalMoney = startMoney * playerList.Count;

        Debug.Log("A game of Texas Hold'Em Has begun");
        // Debug.Log("Tottal Money: " + tottalMoney);
        // Debug.Log("Turn order is: " + getTurnOrder());
        Debug.Log("It is currently " + turnOrder[currentPlayer] +"\'s turn.");
        
        foreach (var playerController in playerList)
        {
            playerController.setMoney(startMoney);
        }
    }

    public void newPlayer(string playerName, string client)
    {
        if (playerList.Count > maxPlayers - 1)
        {
            Debug.Log("The maximum amount of players has been reached. No More can be added");
            return;
        }

        // Debug.Log("Player " + playerName + " Added");

        // Create a new Player object from the Player prefab and name it the new players name
        Object playerObj = Instantiate((playerPrefab), new Vector3(0, 0, 10), Quaternion.identity);
        playerObj.name = playerName;

        // Get the playerController and assign anything new to the player
        playerController player = playerObj.GetComponent<playerController>();
        playerList.Add(player);
        player.setName(playerName);
        player.setPlayerIP(client);
        player.setPlayerNumber(playerList.Count);


        // This updates a UI with the new player whos playing
        playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
    }

    // This function will initialize the turnOrder list but filling it with
    // the names from playerList. It then shuffles its self so that there is a 
    // random turn order.
    // SE:
    // - This function assumes that the game has started.
    // - This order cannot/should not be changed in the future.
    // - This also will set the first players turn (currnetPlayer gets initialized here)
    public void initializeTurnOrder()
    {
        foreach (var player in playerList) { turnOrder.Add(player.getName()); }

        for (int i = 0; i < turnOrder.Count; i++)
        {
            string temp = turnOrder[i];
            int rand = Random.Range(i, turnOrder.Count);

            turnOrder[i] = turnOrder[rand];
            turnOrder[rand] = temp;
        }

        currentPlayer = 0;
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

    public void endTurn()
    {
        currentPlayer = (currentPlayer + 1) % turnOrder.Count;
    }
}
