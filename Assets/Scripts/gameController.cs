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
    //    [SerializeField] int maxPlayers;
    [SerializeField] int startMoney;    // The default amount of money each player gets
    [SerializeField] int minimumBet;    // Default amount for an ante

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
    private int rounds = 0;         // How many total rounds have been played
    private int remaining;
    private int playerTurn = 0;     // Increments each player turn
    private bool isPregame = false;
    private bool lastFold = false;

    private int playersWent;



    // ---------- Getters ----------
    public List<playerController> getPlayerList() { return playerList; }

    public playerController getCurrentPlayer() { return currentPlayer; }

    public int getPlayerTurn() { return playerTurn; }

    public int getRevealBet() { return vault.revealBet; }


    public bool PreGame() { return isPregame; }


    public int getCurrentCall() { return vault.currentBet; }

    public int getAnte() { return minimumBet; }

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

    private Vector3 calculateNewPlayersPosition(int playerNumber)
    {
        // Constants for table dimensions
        float tableLength = 40f; // The length of the rectangle part of the table
        float tableWidth = 45f;  // The width of the rectangle part of the table
        float semiCircleRadius = 19f; // The radius of the semi-circle parts of the table

        // Positions for players on the longer edges of the rectangle
        if (playerNumber == 1 || playerNumber == 2)
        {
            // Players 1-2 on one long edge
            return new Vector3((-tableLength / 4) * (playerNumber % 2 == 0 ? 1 : -1), 0, -tableWidth / 2);
        }
        else if (playerNumber == 3 || playerNumber == 4)
        {
            // Players 3-4 on the opposite long edge
            return new Vector3((-tableLength / 4) * (playerNumber % 2 == 0 ? 1 : -1), 0, tableWidth / 2);
        }
        else
        {
            // Constants for adjusting the position
            // Calculate angle for players on the semi-circles
            // Spread the players evenly across the semi-circle by dividing the semi-circle
            // into two arcs for three players: (180 degrees or Mathf.PI) / (3 - 1)
            float angleStep = Mathf.PI / 2; // This now represents the step between each player
            float startAngle = playerNumber <= 7 ? Mathf.PI : 0; // Starting from the top for players 5-7, bottom for 8-10

            int playerSemiIndex = playerNumber - 5; // Index of the player on the semicircle
            // float angle = startAngle + (playerSemiIndex * angleStep);

            // Adjust the calculation for the x and z positions
            // float xPosition = Mathf.Cos(angle) * semiCircleRadius;
            // float zPosition = Mathf.Sin(angle) * semiCircleRadius;

            if (playerNumber >= 5 && playerNumber <= 7)
            {
                // Mirror players 8-10
                float angle = Mathf.PI - (playerSemiIndex * angleStep) + startAngle;

                // Calculate the x and z positions based on the angle
                // Note that we also need to adjust the zPosition calculation to ensure they are positioned correctly
                // along the width of the table. The "+ tableWidth / 2" is used to position the players on top of the semi-circle edge.
                float xPosition = Mathf.Sin(angle) * semiCircleRadius;
                float zPosition = Mathf.Cos(angle) * semiCircleRadius;

                // Adjust the xPosition by subtracting from -tableLength / 2 to move to the left side of the table
                // For zPosition, we use the same calculation as for players 8-10 but we don't need to make it negative
                // since we're already dealing with the top semi-circle. We subtract 22f to move them back a bit from the edge.
                return new Vector3(-50 / 2 + xPosition, 0, (zPosition - tableWidth / 2) + 22f);
            }
            // Players 8-10 on the right semi-circle
            else
            {
                float angle = startAngle + (playerSemiIndex * angleStep);
                // Adjust the calculation for the x and z positions
                float xPosition = Mathf.Cos(angle) * semiCircleRadius;
                float zPosition = Mathf.Sin(angle) * semiCircleRadius;
                return new Vector3(50 / 2 + xPosition, 0, -(zPosition - tableWidth / 2) - 22f); // Negative z to move to the other semi-circle
            }
        }
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
        if (playerList.Count > 10)
        {
            stateRequest.maxPlayerError();
            return;
        }
        if (gameState)
        {
            stateRequest.gameInProgressError();
            return;
        }


        Vector3 playerPos = calculateNewPlayersPosition(playerList.Count + 1);


        // Create a new Player object from the Player prefab and name it the new players name
        Object playerObj = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        playerObj.name = playerName;

        // Get the playerController and assign anything new to the player
        playerController player = playerObj.GetComponent<playerController>();
        playerList.Add(player);
        player.username = playerName;
        player.ID = client;
        player.playerColor = colors[playerList.Count - 1];
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
        vault.totalMoney = startMoney * playerList.Count;

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
            await anteUP();
        else
            cardController.dealCards(playerList, playerTurn);

        currentPlayer = playerList[playerTurn];
        currentPlayer.underTheGun = true;
        //currentPlayer.enterFrame();
        if (!blindPlay)
            vault.lastRaise = currentPlayer;
        UpdatePlayerVisuals();
    }

    public void endGame()
    {
        gameState = false;
        stateRequest.gameOver();
    }

    private void blindBets()
    {
        var smallBlind = playerList[playerTurn];
        playerTurn = (playerTurn + 1) % playerList.Count;

        var bigBlind = playerList[playerTurn];
        playerTurn = (playerTurn + 1) % playerList.Count;

        vault.blind(smallBlind, bigBlind);
    }


    /* This function first checks if players are:
     * - Ready to move to the next round (Controller will put up a ready-up button)
     * - Will do an ante-up screen to check if players are ready to play the round
     * - Make sure atleast two players are playing
     * 
     * The function will then call the vualt and ante each player by the minimum bet
     */
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

                if (player.autoSitOut)
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

                if (player.playRound)
                    playersPlaying += 1;
            }

            if (playersPlaying < 2)
                foreach (var player in playerList)
                    controlpads_glue.SendControlpadMessage(player.ID, "alert:Need 2 or more to play");
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

        if (!remainingPlayers.Count.Equals(sidePots.Count))
        {
            foreach (var player in sidePots)
            {
                //Debug.Log("player name: " + player.username);
                player.sidePot = player.betted * remainingPlayers.Count;
                vault.potMoney -= player.sidePot;
                remainingPlayers.Remove(player);
            }
        }

        // Determine the winners and then give them their share of the pot
        List<playerController> winners = new List<playerController>();

        if (remainingPlayers.Count > 1)
            winners = cardController.DetermineWinners(remainingPlayers);
        else
            winners = remainingPlayers;



        int payment = vault.potMoney / winners.Count;
        int remainder = vault.potMoney % winners.Count;
        foreach (playerController winner in winners)
        {
            Debug.Log("winners payout: " + payment);
            Debug.Log("winners name: " + winner.username);
        }
        Debug.Log("winners count " + winners.Count);
        //winners money before payout
        Debug.Log("winners money before payout:" + winners[0].money);
        foreach (var player in winners)
            player.payPlayer(payment);


        Debug.Log("money won: " + payment);
        Debug.Log("player name: " + currentPlayer.username);
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

        foreach (var player in playerList)
        {
            if (player.money == vault.totalMoney)
            {
                Debug.Log(player.username + " is the winner!!!");
                endGame();
                return;
            }
        }

        // Wait for everyone to ready up
        stateRequest.waitingToReadyUp(true);
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
        //currentPlayer.enterFrame();
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
        // Make sure if the player has gone all in to add them to a list of side pots
        if (currentPlayer.tappedOut && !sidePots.Contains(currentPlayer))
            sidePots.Add(currentPlayer);

        // Update to the next player
        playerTurn = (playerTurn + 1) % playerList.Count;
        currentPlayer = playerList[playerTurn];
        UpdatePlayerVisuals();

        Debug.Log("Sidepots count: " + sidePots.Count);
        if (sidePots.Count.Equals(playerList.Count))
            revealCards();

        if (lastFold)
        {
            vault.lastRaise = currentPlayer;
            lastFold = false;
        }

        // If the next player is folded or they cant bet anymore just skip their turn
        if (currentPlayer.folded || currentPlayer.tappedOut)
        {
            do
            {
                playerTurn = (playerTurn + 1) % playerList.Count;
                currentPlayer = playerList[playerTurn];
            } while (currentPlayer.folded || currentPlayer.tappedOut);

            UpdatePlayerVisuals();
            refreshPlayers();
        }


        if (remaining.Equals(1))
        {
            playersWent = 0;
            newRound();
            refreshPlayers();
            return;
        }


        // Check if a round of betting has commenced
        Debug.Log("last raise: " + vault.lastRaise.username);
        Debug.Log("current player username: " + currentPlayer.username);
        Debug.Log("current player object: " + currentPlayer);

        Debug.Log("players went " + playersWent);

        // the below is wrong because newBetRound will prematurely reveal cards if the last user folded
        // Check if we have completed a round of betting
        if (currentPlayer == vault.lastRaise)
        {
            // If the player who started the round folds, do not immediately end the betting round.
            // Check if all other players had a chance to act.
            if (playersWent < howManyActive())
            {
                // Not all players have acted yet, just refresh their options
                refreshPlayers();
            }
            else
            {
                // All players have acted, end the betting round
                playersWent = 0;
                newBetRound();
                refreshPlayers();

            }
            return;
        }

        // If not all players have had their turn, refresh the current player's options

        refreshPlayers();

    }

    private void newBetRound()
    {
        // Reset the turn order till we get to the player who betted first
        revealCards();
        vault.lastRaise = currentPlayer;
        vault.revealBet = 0;
        resetBetRound();
        refreshPlayers();
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
        else
        {
            newRound(); return;
        }
        reveal++;
    }

    // The player wishes to raise the curent bet by amount
    public void raise(int amount)
    {
        playersWent++;
        vault.raise(currentPlayer, amount);
        nextTurn();
    }

    // Player wishes to call the previous bet and stay in
    public void call()
    {
        playersWent++;
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
        foreach (var player in playerList)
        {
            player.bettedRound = 0;
        }
    }

    private void refreshPlayers() { refreshPlayers("Update-Refresh"); }
    public void refreshPlayers(string extra)
    {
        Debug.Log("refresh parameter" + extra);
        foreach (var player in playerList)
        {
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:" + extra);
        }
    }

    public void triggerNextTurn()
    {
        nextTurn();
    }

    private void UpdatePlayerVisuals()
    {
        // Iterate through all players
        for (int i = 0; i < playerList.Count; i++)
        {
            // Get the player game object, assuming each player has a reference to their game object
            GameObject playerGameObject = playerList[i].gameObject;

            // Check if the current index matches the playerTurn
            if (i == playerTurn)
            {
                // It's this player's turn, set their game object color to green
                SetPlayerColor(playerGameObject, Color.green);
            }
            else
            {
                // It's not this player's turn, set their game object color to gray
                SetPlayerColor(playerGameObject, Color.gray);
            }
        }
    }

    private void SetPlayerColor(GameObject playerGameObject, Color color)
    {
        // Assuming the player game object has a Renderer component where the color is to be set
        Renderer renderer = playerGameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Set the color of the material, note this will affect all objects using this material
            renderer.material.color = color;
        }
    }

    // determine how many active hands are still in the round
    private int howManyActive()
    {
        int active = 0;
        foreach (var player in playerList)
        {
            if (!player.folded && !player.tappedOut)
                active++;
        }
        return active;
    }

}
