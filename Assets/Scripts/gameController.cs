using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class gameController : MonoBehaviour
{
    // Default Varaiables
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerUI;
    [SerializeField] int maxPlayers;
    [SerializeField] int startMoney;
    [SerializeField] int ante;

    // Static Variables
    private static bool gameState = false; // True means a game has started

    // Instance Variables
    private List<playerController> playerList = new List<playerController>();
    private List<playerController> turnOrder = new List<playerController>();
    private int tottalMoney = 0;
    private int potMoney = 0;
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


    // ---------- Getters ----------
    public bool getGameState() { return gameState; }

    public string getTurnOrder()
    {
        string order = "";
        foreach (var name in turnOrder) { order += name + " "; }
        return order; 
    }

    public string getCurretPlayer() { return turnOrder[currentPlayer].getName(); }

    public int getLeviCurrentPlayer() {return currentPlayer;}

    public List<playerController>  getPlayerList() { return playerList; }

    
    public void startGame() { startNewGame(this.startMoney, this.ante); }
    
    public void startGame(int startMoney) { startNewGame(startMoney, this.ante); }

    public void startGame(int startMoney, int ante) { startNewGame(startMoney, ante); }


    /* This function initializes a new game.
     * 
     * IN:
     * startMoney = The amount of money each player starts with
     * 
     * Out:
     * The game doesn't throw any exceptions however it does limit when a new
     * game can start. 
     * - Cant start new game if there is already a game in progress
     * - Cant start a game if there is less than 2 players, (min of 2 people)
     * 
     * AS:
     * - Turns the instance variable gameState to true.
     * - Assumes that all players who are playing are joined.
     */
    private void startNewGame(int startMoney, int ante) 
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

        // Initialize variables
        gameState = true;
        initializeTurnOrder();
        tottalMoney = startMoney * playerList.Count;
        this.ante = ante;

        Debug.Log("A game of Texas Hold'Em Has begun");
        // Debug.Log("Tottal Money: " + tottalMoney);
        // Debug.Log("Turn order is: " + getTurnOrder());
        Debug.Log("It is currently " + getCurretPlayer() +"\'s turn.");
        
        foreach (var playerController in playerList)
        {
            playerController.setMoney(startMoney);
            controlpads_glue.SendControlpadMessage(playerController.getIP(), "refresh");

        }
    }

    /* Creates/Joins a new player to the game
     * 
     * IN: 
     * - playerName = The username the player inputed 
     * - client = the ID/IP of the player to keep them unique
     * 
     * OUT:
     *  Returns if the maximum player limit is reached.
     *  Returns if there is already a game in progress
     * 
     */
    public void newPlayer(string playerName, string client)
    {
        if (playerList.Count > maxPlayers - 1)
        {
            Debug.Log("The maximum amount of players has been reached. No More can be added");
            return;
        }
        if (gameState)
        {
            Debug.Log("There is already a game in progress. No new players can join");
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
        if (playerList.Count == 1) { player.setHost(); }


        // This updates a UI with the new player whos playing
        playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
    }

    // This function will initialize the turnOrder list but filling it with
    // the names from playerList. It then shuffles itself so that there is a 
    // random turn order.
    // SE:
    // - This function assumes that the game has started.
    // - This order cannot/should not be changed in the future.
    // - This also will set the first players turn (currnetPlayer gets initialized here)
    public void initializeTurnOrder()
    {
        foreach (var player in playerList) { turnOrder.Add(player); }

        for (int i = 0; i < turnOrder.Count; i++)
        {
            playerController temp = turnOrder[i];
            int rand = Random.Range(i, turnOrder.Count);

            turnOrder[i] = turnOrder[rand];
            turnOrder[rand] = temp;
        }

        currentPlayer = 0;
    }

    public void startTurn()
    {

    }

    public void endTurn()
    {
        currentPlayer = (currentPlayer + 1) % turnOrder.Count;
        foreach (var playerController in playerList)
        {
            controlpads_glue.SendControlpadMessage(playerController.getIP(), "refresh");

        }

    }

    public void anteUP(string playerName, bool playing)
    {
        if (playing) { Debug.Log(playerName + " Is playing this round"); }
    }

    // Player does not have to call to stay in
    // Player does not wish to raise the bet
    public void check()
    {

    }

    // Player wishes to call the previous bet and stay in
    public void call()
    {

    }

    // The player wishes to raise the curent bet
    public void raise(int amount)
    {
        potMoney += amount;
    }

    public void fold(string username)
    {

    }
}
