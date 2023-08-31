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
    [SerializeField] GameObject moneyUI;
    [SerializeField] GameObject turnOrderUI;
    [SerializeField] int maxPlayers;
    [SerializeField] int startMoney;
    [SerializeField] int ante;

    // Static Variables
    private static bool gameState = false; // True means a game has started

    // Instance Variables
    private List<playerController> playerList = new List<playerController>();
    private Queue<playerController> turnOrder = new Queue<playerController>();
    private Queue<playerController> roundRobin = new Queue<playerController>();
    private playerController currentPlayer;
    private int rounds = 0;
    private int potMoney = 0;
    private int tottalMoney = 0;
    private int playerTurn = 0;
    private bool increasingAnte = false;
    private bool currentRound = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState)
        {
            turnOrderUI.GetComponent<UnityEngine.UI.Text>().text = getTurnOrder() + "\n";
            moneyUI.GetComponent<UnityEngine.UI.Text>().text = potMoney.ToString();

        }
    }


    // ---------- Getters ----------
    public bool getGameState() { return gameState; }

    public string getTurnOrder()
    {
        string order = "";
        foreach (var player in turnOrder) { order += player.getName() + " "; }
        return order; 
    }

    public string getCurretPlayer() { return currentPlayer.getName(); }

    public int getPlayerTurn() { return playerTurn; }

    public List<playerController> getPlayerList() { return playerList; }

    public void startGame() { startNewGame(this.startMoney, this.ante); }
    
    public void startGame(int startMoney) { startNewGame(startMoney, this.ante); }

    public void startGame(int startMoney, int ante) { startNewGame(startMoney, ante); }


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
        }
        else if (playerList.Count < 2)
        {
            Debug.Log("There are not enough players to play a game of poker. " +
                "Please add more players then continue.");
            return;
        }

        foreach (var player in playerList)
        {
            player.setMoney(startMoney);
            controlpads_glue.SendControlpadMessage(player.getIP(), "refresh");
        }

        // Initialize variables
        gameState = true;
        initializeTurnOrder();
        tottalMoney = startMoney * playerList.Count;
        this.ante = ante;
        rounds++;
        currentRound = true;

        // Debug.Log("A game of Texas Hold'Em Has begun");
        // Debug.Log("Tottal Money: " + tottalMoney);
        // Debug.Log("Turn order is: " + getTurnOrder());
        Debug.Log("It is currently " + getCurretPlayer() + "\'s turn.");
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

        for (int i = 0; i < playerList.Count; i++)
        {
            playerController temp = playerList[i];
            int rand = Random.Range(i, playerList.Count);

            playerList[i] = playerList[rand];
            playerList[rand] = temp;
        }

        foreach (var player in playerList) { turnOrder.Enqueue(player); }
        foreach (var player in playerList) { roundRobin.Enqueue(player); }
        currentPlayer = turnOrder.Peek();
        playerTurn = 0;
    }

    public void newRound()
    {
        currentPlayer = turnOrder.Peek();
        Debug.Log(currentPlayer.getName() + " has won " + potMoney + "$");
        currentPlayer.payPlayer(potMoney);
        potMoney = 0;
        turnOrder.Clear();

        // Move the first player to the back of the roundRobin then reinstantiate the turnOrder
        roundRobin.Enqueue(roundRobin.Dequeue());
        foreach (var player in roundRobin) { turnOrder.Enqueue(player); }
        currentPlayer = turnOrder.Peek();
        playerTurn = 0;

        Debug.Log("A new round has started!");
    }

    public void nextTurn()
    {
        turnOrder.Enqueue(turnOrder.Dequeue());
        if (turnOrder.Count == 1) { newRound(); }

        // Update the current player
        currentPlayer = turnOrder.Peek();
        playerTurn = (playerTurn + 1) % turnOrder.Count;

        foreach (var playerController in playerList)
        {
            controlpads_glue.SendControlpadMessage(playerController.getIP(), "refresh");
        }


        Debug.Log("It is now " + currentPlayer.getName() + "\'s turn.");
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
        int money = currentPlayer.requestFunds(amount);

        potMoney += money;
        Debug.Log(getCurretPlayer() + " has put in " + money + " raising to " + potMoney);
        nextTurn();
    }

    public void fold()
    {
        Debug.Log(getCurretPlayer() + " has folded this round");
        currentPlayer.fold();
        turnOrder.Dequeue();
        nextTurn();
    }
}
