using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] cardController cardController;
    [SerializeField] int maxPlayers;
    [SerializeField] int startMoney;
    [SerializeField] int ante;

    // Static Variables
    public static bool gameState { get; set; } = false; // True means a game has started

    // Instance Variables
    private List<playerController> playerList = new List<playerController>();
    private Queue<playerController> turnOrder = new Queue<playerController>();
    private Queue<playerController> roundRobin = new Queue<playerController>();
    private playerController currentPlayer;
    private playerController highestBidder;
    private int rounds = 0;
    private int turn = 0;
    private int potMoney = 0;
    private int tottalMoney = 0;
    private int playerTurn = 0;
    private int currentBet = 0;
    private bool increasingAnte = false;


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
            playerUI.GetComponent<UnityEngine.UI.Text>().text = getPlayerMoneyInfo();

        }
    }


    // ---------- Getters ----------
    public List<playerController> getPlayerList() { return playerList; }

    public playerController getCurretPlayer() { return currentPlayer; }

    public int getPlayerTurn() { return playerTurn; }

    public int getCurrentCall() { return currentBet; }

    public string getTurnOrder()
    {
        string order = "";
        foreach (var player in turnOrder) { order += player.username + " "; }
        return order; 
    }

    public string getPlayerMoneyInfo()
    {
        string finalInfo = "";
        foreach (var player in playerList)
        {
            finalInfo += player.username + "    " + player.betted + "     " +
                player.money + "\n";
        }

        return finalInfo;
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
        player.username = playerName;
        player.ID = client;
        player.playerNumber = playerList.Count;
        if (playerList.Count == 1) { player.isHost = true; }


        // This updates a UI with the new player whos playing
        playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
    }

    public void startGame() { startNewGame(this.startMoney, this.ante); }

    public void startGame(int startMoney) { startNewGame(startMoney, this.ante); }

    public void startGame(int startMoney, int ante) { startNewGame(startMoney, ante); }

    /* This function initializes a new game.
      * 
      * IN:
      * startMoney = The amount of money each player starts with
      * ante = how much each player has to atne to play a round
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
            player.money = startMoney;
            player.betted = 0;
            player.bettedRound =0;
            player.isPlayerTurn = false;
            player.folded = false;
            player.tappedOut = false;
            player.handRank = 10;
            player.getHoleCards().Clear();
            player.bestHand.Clear();
            player.handDescription = null;
            controlpads_glue.SendControlpadMessage(player.ID, "refresh");
        }

        // Initialize variables
        cardController.resetCards();
        initializeTurnOrder();
        tottalMoney = startMoney * playerList.Count;
        cardController.dealCards(turnOrder.ToList());
        rounds = 1;
        potMoney = 0;
        currentBet = 0;
        playerTurn = 0;
        turn = 0;
        this.ante = ante;

        Debug.Log("A new game of Texas Hold'Em Has begun");
        Debug.Log("Turn order is: " + getTurnOrder());
        Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " + 
            currentPlayer.getHoleCardsDesc());

        gameState = true;
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
        turnOrder.Clear();
        roundRobin.Clear();

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
        currentPlayer.isPlayerTurn = true;
        highestBidder = currentPlayer;
        playerTurn = 0;
    }

    public void newRound()
    {
        if (turn == 0  && turnOrder.Count > 1) { cardController.revealFlop(); turn++; resetBetRound(); currentBet = 0; return; }
        else if (turn == 1 && turnOrder.Count > 1) { cardController.revealTurn(); turn++; resetBetRound(); currentBet = 0; return; }
        else if (turn == 2 && turnOrder.Count > 1) { cardController.revealRiver(); turn++; resetBetRound(); currentBet = 0; return; }

        List<playerController> winner = cardController.DetermineWinners(turnOrder.ToList());

        foreach (var player in winner) 
        {
            int payment = potMoney / winner.Count;
            Debug.Log(player.username + " has won " + payment + "$");
            player.payPlayer(payment);
        }

        rounds++;
        turnOrder.Clear();
        cardController.resetCards();

        // Move the first player to the back of the roundRobin then reinstantiate the turnOrder
        roundRobin.Enqueue(roundRobin.Dequeue());
        foreach (var player in roundRobin.ToList()) {
            if (player.folded) { player.fold(); }
            player.betted = 0;
            player.bettedRound =0;
            player.resetHoleCards();
            turnOrder.Enqueue(player);
        }

        // Reset variables
        currentPlayer = turnOrder.Peek();
        highestBidder = currentPlayer;
        potMoney = 0;
        currentBet = 0;
        playerTurn = 0;
        turn = 0;
        cardController.dealCards(turnOrder.ToList());

        Debug.Log("A new round has started!");
    }

    public void nextTurn()
    {
        currentPlayer.isPlayerTurn = false;
        // Update the to the next player, unless the current player folded then just remove them.
        if (currentPlayer.folded)
            turnOrder.Dequeue();
        else
            turnOrder.Enqueue(turnOrder.Dequeue());

        currentPlayer = turnOrder.Peek();
        currentPlayer.isPlayerTurn = true;
        playerTurn = (playerTurn + 1) % turnOrder.Count;
        controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh");

        string cards = "Hole Cards:";
        // foreach (var card in currentPlayer.getHoleCards()) { cards = cards + " " + card + ","; }
        Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " + 
            currentPlayer.getHoleCardsDesc());

        // This is for limited play, we're only allowing players to bet once per draw.
        if (currentPlayer == highestBidder) { newRound(); return; }

        if (currentPlayer.tappedOut) { nextTurn(); }
    }

    public void anteUP(string playerName, bool playing)
    {
        if (playing) { Debug.Log(playerName + " Is playing this round"); }
    }

    // The player wishes to raise the curent bet by amount
    public void raise(int amount)
    {
        int money;
        int lastBet = currentBet;

        if (currentPlayer.betted < currentBet)
        {
            // request funds to call the current bet
            int callBet = currentPlayer.requestFunds(currentBet - currentPlayer.betted);
            // requst funds to raise the current bet
            int raiseMoney = currentPlayer.requestFunds(amount);

            money = callBet + raiseMoney;
            currentBet += raiseMoney;
        } else
        {
            money = currentPlayer.requestFunds(amount);
            currentBet += money;
        }

        // Increment the amount in the pot and then set the current player as the highest bidder
        potMoney += money;
        highestBidder = currentPlayer;

        Debug.Log(currentPlayer.username + " sees the last bet of " + 
            lastBet + " and raises it by " + amount + " putting in a tottal of " + money);

        nextTurn();
    }

    // Player wishes to call the previous bet and stay in
    public void call()
    {
        // No limits
        // if (currentPlayer == highestBidder) { newRound(); return; }

        int money;
        if (currentPlayer.betted < currentBet)
            money = currentPlayer.requestFunds(currentBet - currentPlayer.betted);
        else
            money = currentPlayer.requestFunds(currentBet);
        
        Debug.Log(currentPlayer.username + " calls the current bet of " + currentBet);
        potMoney += money;
        nextTurn();
    }

    public void fold()
    {
        // No limits
        // if (currentPlayer == highestBidder) { highestBidder = turnOrder.Peek();}
        Debug.Log(currentPlayer.username + " has folded this round");
        currentPlayer.fold();
        
        // Check if everyone has folded
        if (turnOrder.Count == 1) { newRound(); return; }

        nextTurn();
    }

    public void resetBetRound()
    {
        foreach(var player in playerList)
        {
            player.bettedRound = 0;
        }
    }

}
