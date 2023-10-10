using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading;
using System.Threading.Tasks;

public class gameController : MonoBehaviour
{

    // Default Varaiables
    [SerializeField] List<Material> colorList = new List<Material>();
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject pregameUI;
    [SerializeField] cardController cardController;
    [SerializeField] stateRequest stateRequest;
    [SerializeField] Vault vault;
    [SerializeField] int maxPlayers;
    [SerializeField] int startMoney;    // The default amount of money each player gets
    [SerializeField] int minimumBet;          // Default amount for an ante

    //Colors
    public static string[] colors = new string[] 
    { "Blue", "Green", "Red", "Purple", "Pink", "Orange", "Yellow", "Brown", "Teal", "Lavender", "Indigo", "Maroon", "Aqua", "Coral", "Gold", "Silver", "Lime", "Olive", "Navy", "Turquoise", "Tan"};

    // Static Variables
    public static bool gameState { get; set; } = false; // True means a game has started
    public static bool blindPlay { get; set; } = false;
    public static bool antePlay { get; set; } = false;

    // Instance Variables
    private List<playerController> playerList = new List<playerController>();
    private List<playerController> sidePots = new List<playerController>();
    private playerController currentPlayer;
    private int reveal = 0;         // which of reveal will be played after betting
    private int rounds = 0;         // How many tottal rounds have been played
    private int remaining;
    private int playerTurn = 0;     // Increments each player turn
    private bool isPregame = false;
    private bool lastFold = false;



    // ---------- Getters ----------
    public List<playerController> getPlayerList() { return playerList; }

    public playerController getCurretPlayer() { return currentPlayer; }

    public int getPlayerTurn() { return playerTurn; }

    public int getRevealBet() {return vault.revealBet;}


    public bool PreGame() {return isPregame;}


    public int getCurrentCall() { return vault.currentBet; }

    public int getAnte() {return minimumBet;}

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
     *  Returns playerError player limit is reached.
     *  Returns gameStateError if there is a game in progress
     * 
     */
    public void newPlayer(string playerName, string client)
    {
        if (playerList.Count > maxPlayers - 1)
        {
            stateRequest.maxPlayerError();
            return;
        }
        if (gameState)
        {
            stateRequest.gameInProgressError();
            return;
        }

        // Create a new Player object from the Player prefab and name it the new players name
        Object playerObj = Instantiate(playerPrefab, new Vector3(30, 6, -23), Quaternion.identity);
        playerObj.name = playerName;

        // Get the playerController and assign anything new to the player
        playerController player = playerObj.GetComponent<playerController>();
        playerList.Add(player);
        player.username = playerName;
        player.ID = client;
        player.playerColor = colors[playerList.Count-1];
        player.playerNumber = playerList.Count;
        if (playerList.Count == 1) { player.isHost = true; }

        stateRequest.sendPlayerSuccess(player);
    }

    private static ManualResetEvent holdForSomethin = new ManualResetEvent(false);


    public void startGame() { startNewGame(this.startMoney, this.minimumBet); }

    public void startGame(int startMoney) { startNewGame(startMoney, this.minimumBet); }

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
    private async void startNewGame(int startMoney, int ante)
    {
        if (gameState)
        {
            stateRequest.gameInProgressError();
            return;
        }
        else if (playerList.Count < 2)
        {
            stateRequest.minPlayerError();
            return;
        }
        
        this.minimumBet = ante;
        pregameUI.gameObject.SetActive(false);

        // Reset Player Variables
        foreach (var player in playerList)
        {
            player.resetPlayer();
            player.money = startMoney;
        }

        // Initialize Vault Variables
        vault.potMoney = 0;
        vault.currentBet = 0;
        vault.minimumBet = minimumBet;
        vault.tottalMoney = startMoney * playerList.Count;

        // Reset & Initialize Game Variables
        reveal = 0;
        rounds = 0;
        playerTurn = 0;
        remaining = playerList.Count;
        
        // Reset Cards
        cardController.resetCards();

        // Update game State
        gameState = true;
        
        // Initialize the first player and deal cards
        if (blindPlay)
            blindBets();

        if (antePlay)
        {
            await anteUP();
        }
        else
            cardController.dealCards(playerList, playerTurn);

        currentPlayer = playerList[playerTurn];
        currentPlayer.underTheGun = true;
        currentPlayer.enterFrame(); 
        if (!blindPlay) 
            vault.lastRaise = currentPlayer;
    }

    private void blindBets()
    {
        var smallBlind = playerList[playerTurn];
        playerTurn = (playerTurn + 1) % playerList.Count;
        
        var bigBlind = playerList[playerTurn];
        playerTurn = (playerTurn + 1) % playerList.Count;

        vault.blind(smallBlind, bigBlind);
    }

    private async Task anteUP()
    {
        int playersPlaying;

        //Setting isPregame to true will force the controller into pregame status
        isPregame = true;
        //Loop to make sure at least two players end up being in the game
        do
        {
            playersPlaying = 0;
            foreach (var player in playerList)
            {

                if(player.autoSitOut)
                {
                    player.pregameResponded = true;
                    player.playRound = false;
                }
                else
                {
                   player.pregameResponded = false;

                }
            }
            refreshPlayers("Time to Ante Up!");
        
            foreach (var player in playerList)
            {
                do
                    await Task.Delay(1000);
                while (!player.pregameResponded);

                if(player.playRound)
                    playersPlaying += 1;
            }

            if (playersPlaying < 2)
                foreach (var player in playerList)
                    controlpads_glue.SendControlpadMessage(player.ID, "alert:Need more than one person to play a poker game you silly goose."); 
        }
        while (playersPlaying < 2);

        isPregame = false;
        foreach (var player in playerList)
        {
            player.pregameResponded = false;
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:Anteing is finished"); 
        }

        // Find out which players are playing this round
        var playing = new List<playerController>();
        foreach (var player in playerList)
            if (!player.playRound)
                player.folded = true;
            else
                playing.Add(player);

        remaining = playing.Count;
        vault.ante(playing);
        cardController.dealCards(playing, playerTurn);
        resetBetRound();
    }


    private async void newRound()
    {

        // Parse the players for only the players who havent folded
        List<playerController> remainingPlayers = new List<playerController>();
        foreach (var player in playerList) 
            if (!player.folded)
                remainingPlayers.Add(player);

        // Calculate Side Pots
        foreach (var player in sidePots)
        {
            player.sidePot = player.betted * remainingPlayers.Count;
            vault.potMoney -= player.sidePot;
            remainingPlayers.Remove(player);
        }


        // Determine the winners and then give them their share of the pot
        List<playerController> winners = new List<playerController>();
        if (remainingPlayers.Count > 2)
            winners = cardController.DetermineWinners(remainingPlayers);
        else
            winners = remainingPlayers;

        int payment = vault.potMoney / winners.Count;
        int remainder = vault.potMoney % winners.Count;

        foreach (var player in winners)
            player.payPlayer(payment);

        // Give the remainder to the player who is a winner but also 
        // the closest to the first players turn
        playerTurn = rounds % playerList.Count;
        currentPlayer = playerList[playerTurn];
        foreach (var player in playerList)
        {
            if (!currentPlayer.folded)
            {
                currentPlayer.payPlayer(remainder);
                break;
            }
            playerTurn = (playerTurn + 1) % playerList.Count;
            currentPlayer = playerList[playerTurn];
        }

        // Wait for everyone to ready up
        stateRequest.waitingToReadyUp(true);

        foreach (var player in playerList)
            {
                player.readyResponded = false;
                player.readyForNextRound = false;
                player.status = "Ready up for the next round!";
            }



        // Now Determine the winner of each sidepot
        sidePots.Reverse();
        foreach (var player in sidePots)
        {
            winners.Add(player);
            List<playerController> sideWinners = cardController.DetermineWinners(winners);

            payment = player.sidePot / sideWinners.Count;
            remainder = player.sidePot % sideWinners.Count;

            foreach (var winner in sideWinners)
                winner.payPlayer(payment);

            foreach (var play in playerList)
            {
                if (!currentPlayer.folded)
                {
                    currentPlayer.payPlayer(remainder);
                    break;
                }
                playerTurn = (playerTurn + 1) % playerList.Count;
                currentPlayer = playerList[playerTurn];
            }
        }

        refreshPlayers("Ready up for next round");

        await WaitForReadyForNextRound();


        // Reset Player Objects to the right position and make sure they arnt folded
        foreach (var player in playerList) 
        {
            player.roundReset();
        }
        
        // Update Game Variables
        rounds++;
        reveal = 0;
        remaining = playerList.Count;
        vault.potMoney = 0;
        vault.revealBet = 0;
        vault.currentBet = 0;
        playerTurn = rounds % playerList.Count;
        sidePots.Clear();

        // Initialize first player of the next round
        // Reset and Deal Cards
        cardController.resetCards();
        if (blindPlay)
            blindBets();

        if (antePlay)
            await anteUP();
        else
            cardController.dealCards(playerList, playerTurn);

        Debug.Log("Pot Money: " + vault.potMoney);

        currentPlayer = playerList[playerTurn];
        currentPlayer.underTheGun = true;
        currentPlayer.enterFrame();
        vault.lastRaise = currentPlayer;

        refreshPlayers();

        // Debug.Log("---------------------- A new round has started! ----------------------");
        // Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " + currentPlayer.getHoleCardsDesc());
    }

    private async Task WaitForReadyForNextRound()
    {
        foreach (var player in playerList)
    {
        // Wait until the player signals they are ready for the next round
        await Task.Run(() => { while (!player.readyForNextRound) { } });
    }

    }

    private void nextTurn()
    {
        // Make sure the previous players turn ends
        StartCoroutine(ExitFrameCoroutine(() =>
        {
            // This code will run when ExitFrame is done
            // Make sure if the player has gone all in to add them to a list of side pots
            if (currentPlayer.tappedOut && !sidePots.Contains(currentPlayer)) 
                sidePots.Add(currentPlayer);

            // Update to the next player
            playerTurn = (playerTurn + 1) % playerList.Count;
            currentPlayer = playerList[playerTurn];

            if (lastFold)
            {
                
                vault.lastRaise = currentPlayer;

                lastFold = false;
            }

            // If the next player is folded or they cant bet anymore just skip their turn
            if (currentPlayer.folded || currentPlayer.tappedOut) 
            {
                if (vault.lastRaise ==  currentPlayer)
                    lastFold = true;
                nextTurn();
                return;
            }

            
            if (remaining.Equals(1))
            {
                newRound();
                return;
            }

            
            // Check if a round of betting has commenced
            if (currentPlayer == vault.lastRaise)
            {
                newBetRound();
                return;
            }

            StartCoroutine(EnterFrameCoroutine(() =>
            {
                // Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " + currentPlayer.getHoleCardsDesc());
                controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh:1");
            }));
        }));
    }

    private IEnumerator ExitFrameCoroutine(System.Action onComplete)
    {
        currentPlayer.exitFrame();

        // Call the onComplete callback when exitFrame is done
        yield return new WaitUntil(() => !currentPlayer.IsMoving); // Modify this condition as needed
    
        onComplete?.Invoke();
    }

    private IEnumerator EnterFrameCoroutine(System.Action onComplete)
    {
        currentPlayer.enterFrame();
    
        // Call the onComplete callback when exitFrame is done
        yield return new WaitUntil(() => !currentPlayer.IsMoving); // Modify this condition as needed
    
        onComplete?.Invoke();
    }
    

    private void newBetRound()
    {
        revealCards();
        foreach (var player in playerList) { player.transform.position = new Vector3(30, 6, -23); }

        // Reset the turn order till we get to the player who betted first
        playerTurn = rounds % playerList.Count;
        currentPlayer = playerList[playerTurn];
        vault.lastRaise = currentPlayer;
        vault.revealBet = 0;
        resetBetRound();

    }

    private void revealCards()
    {
        // Debug.Log("---------------------- Revealing Card(s) ----------------------");
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
        vault.raise(currentPlayer, amount);
        nextTurn();
    }

    // Player wishes to call the previous bet and stay in
    public void call()
    {
        vault.call(currentPlayer);
        nextTurn();
    }

    public void fold()
    {
        currentPlayer.folded = true;
        remaining--;
        lastFold = true;

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

    private void refreshPlayers() { refreshPlayers("Update-Refresh"); }
    public void refreshPlayers(string extra)
    {
        foreach(var player in playerList)
        {
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:" + extra);
        }
    }

    public void triggerNextTurn()
    {
        nextTurn();
    }

}
