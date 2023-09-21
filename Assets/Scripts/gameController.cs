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
    [SerializeField] List<Material> colorList = new List<Material>();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerUI;
    [SerializeField] GameObject moneyUI;
    [SerializeField] GameObject turnOrderUI;
    [SerializeField] cardController cardController;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] int maxPlayers;
    [SerializeField] int startMoney;
    [SerializeField] int ante;

    //Colors
    public static string[] colors = new string[] 
    { "Blue", "Green", "Red", "Purple", "Pink", "Orange", "Yellow", "Brown", "Teal", "Lavender", "Indigo", "Maroon", "Aqua", "Coral", "Gold", "Silver", "Lime", "Olive", "Navy", "Turquoise", "Tan"};

    // Static Variables
    public static bool gameState { get; set; } = false; // True means a game has started
    public static bool blindPlay { get; set; } = false;
    public static bool antePlay { get; set; } = false;


    // Instance Variables
    private List<playerController> playerList = new List<playerController>();
    private playerController currentPlayer;
    private playerController highestBidder;
    private int reveal = 0;         // which of reveal will be played after betting
    private int rounds = 0;         // How many tottal rounds have been played
    private int potMoney = 0; 
    private int tottalMoney = 0;
    private int currentBet = 0;     // Resets every round
    private int revealBet = 0;      // Resets every reveal
    private int playerTurn = 0;     // Increments each player turn
    private bool isPregame = false;


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
            moneyUI.GetComponent<UnityEngine.UI.Text>().text = "Pot: " + potMoney.ToString();
            playerUI.GetComponent<UnityEngine.UI.Text>().text = getPlayerMoneyInfo();

        }
    }


    // ---------- Getters ----------
    public List<playerController> getPlayerList() { return playerList; }

    public playerController getCurretPlayer() { return currentPlayer; }

    public int getPlayerTurn() { return playerTurn; }

    public bool PreGame() {return isPregame;}

    public int getCurrentCall() { return currentBet; }

    public int getAnte() {return ante;}

    public string getTurnOrder()
    {
        string order = "";
        foreach (var player in playerList) { order += player.username + " "; }
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
        Object playerObj = Instantiate((playerPrefab), new Vector3(30, 6, -23), Quaternion.identity);
        playerObj.name = playerName;

        // Get the playerController and assign anything new to the player
        playerController player = playerObj.GetComponent<playerController>();
        playerList.Add(player);
        player.username = playerName;
        player.ID = client;
        player.playerColor = colors[playerList.Count-1];
        // Debug.Log(player.playerColor);
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
        this.ante = ante;

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

        // Reset Player Variables
        foreach (var player in playerList)
        {
            player.money = startMoney;
            player.betted = 0;
            player.bettedRound = 0;
            player.isPlayerTurn = false;
            player.folded = false;
            player.tappedOut = false;
            player.handRank = 10;
            player.getHoleCards().Clear();
            player.bestHand.Clear();
            player.handDescription = null;
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:2");
        }

        // Reset and Deal Cards
        cardController.resetCards();
        cardController.dealCards(playerList);

        // Reset & Initialize Game Variables
        reveal = 0;
        rounds = 0;
        potMoney = 0;
        currentBet = 0;
        playerTurn = 0;
        tottalMoney = startMoney * playerList.Count;

        // Initialize the first player
        if (blindPlay)
            playBlinds();

        currentPlayer = playerList[playerTurn];
        currentPlayer.isPlayerTurn = true;
        currentPlayer.underTheGun = true;
        currentPlayer.enterFrame();
        highestBidder = currentPlayer;
        controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh:3");

        Debug.Log("---------------------- A new game of Texas Hold'Em Has begun ----------------------");
        Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " + 
            currentPlayer.getHoleCardsDesc());

        gameState = true;
    }

    private void playBlinds()
    {
        potMoney += playerList[playerTurn].requestFunds(ante / 2);
        playerTurn++;
        potMoney += playerList[playerTurn].requestFunds(ante);
        playerTurn++;
    }


    private void newRound()
    {
        // Reset Player Objects to the right position and make sure they arnt folded
        foreach (var player in playerList) 
        {
            player.transform.position = new Vector3(-30, 6, -23);
            player.folded = false;
        }

        // Reset and Deal Cards
        cardController.resetCards();
        cardController.dealCards(playerList);

        // Update Game Variables
        rounds++;
        reveal = 0;
        potMoney = 0;
        revealBet = 0;
        currentBet = 0;
        playerTurn = rounds % playerList.Count;

        // Initialize first player of the next round
        if (blindPlay)
            playBlinds();

        currentPlayer = playerList[playerTurn];
        currentPlayer.underTheGun = true;
        currentPlayer.enterFrame();
        highestBidder = currentPlayer;
        controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh:4");

        Debug.Log("---------------------- A new round has started! ----------------------");
        Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " +
            currentPlayer.getHoleCardsDesc());
    }

    public void nextTurn()
    {
        Debug.Log("Highest Bidder: " + highestBidder.username);

        // Make sure the previous playrs turn ends
        currentPlayer.isPlayerTurn = false;
        currentPlayer.exitFrame();

        // Update to the next player
        playerTurn = (playerTurn + 1) % playerList.Count;
        currentPlayer = playerList[playerTurn];

        // If the next player is folded or they cant bet anymore just skip their turn
        if (currentPlayer.folded || currentPlayer.tappedOut) 
        {
            nextTurn();
            return;
        }

        // Check if everyone has folded except the current player.
        int i = 0;
        foreach (var player in playerList) 
        { 
            if (player.folded)
            { 
                i++; 
            } 
        }
        if (i == playerList.Count - 1)
        {
            newRound();
            return;
        }

        // Check if a round of betting has commenced
        if (currentPlayer == highestBidder)
        {
            Debug.Log("Current player = highest bidder");
            newBetRound();
            return;
        }
        currentPlayer.isPlayerTurn = true;
        currentPlayer.enterFrame();
        controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh:1");
    }

    private void newBetRound()
    {
        revealCards();
        Debug.Log("I made it");
        // Reset the turn order till we get to the player who betted first
        playerTurn = (rounds % playerList.Count) - 1;
        highestBidder = playerList[playerTurn + 1];
        Debug.Log("I made it");
        nextTurn();
    }

    private void revealCards()
    {
        Debug.Log("---------------------- Revealing Card(s) ----------------------");
        if (reveal == 0)
            cardController.revealFlop();
        else if (reveal == 1)
            cardController.revealTurn();
        else if (reveal == 2)
            cardController.revealRiver();
        else {
            newRound(); return; }
        reveal++;
    }

    // The player wishes to raise the curent bet by amount
    public void raise(int amount)
    {
        int money;
        int lastBet = currentBet;

        if (currentPlayer.betted < currentBet)
        {
            // request funds to call the current tottal bet
            int callBet = currentPlayer.requestFunds(currentBet - currentPlayer.betted);
            // requst funds to raise the current tottal bet
            int raiseMoney = currentPlayer.requestFunds(amount);

            money = callBet + raiseMoney;
            currentBet += raiseMoney;
            revealBet += raiseMoney;
        } else
        {
            money = currentPlayer.requestFunds(amount);
            currentBet += money;
            revealBet += money;
        }

        // Increment the amount in the pot and then set the current
        // player as the highest bidder
        potMoney += money;
        highestBidder = currentPlayer;

        Debug.Log(currentPlayer.username + " sees the last bet of " + 
            lastBet + " and raises it by " + amount + " putting in a tottal of " + money);

        nextTurn();
    }

    // Player wishes to call the previous bet and stay in
    public void call()
    {
        int money;
        if (currentPlayer.betted < currentBet)
            money = currentPlayer.requestFunds(currentBet - currentPlayer.betted);
        else
            money = currentPlayer.requestFunds(revealBet);   
        
        Debug.Log(currentPlayer.username + " calls the current bet of " + currentBet);
        potMoney += money;
        nextTurn();
    }

    public void fold()
    {
        currentPlayer.fold();

        if (currentPlayer == highestBidder)
            for (int i = playerList.Count - 1; i > 0; i--) 
                if (!playerList[i].folded)
                {
                    highestBidder = playerList[i];
                    break;
                }

        Debug.Log(currentPlayer.username + " has folded.");
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
