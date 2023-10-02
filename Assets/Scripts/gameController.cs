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
    [SerializeField] GameObject playerUI;
    [SerializeField] GameObject moneyUI;
    [SerializeField] cardController cardController;
    [SerializeField] SceneLoader sceneLoader;

    [SerializeField] controllerParse playerComms;

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

    // GUI Variables
    private Transform entryContainer;
    private Transform entryTemplate;
    private Transform entryTransform;

    // Instance Variables
    private List<playerController> playerList = new List<playerController>();
    private List<playerController> sidePots = new List<playerController>();
    private playerController currentPlayer;
    private playerController highestBidder;
    private Transform potGUI;
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
        potGUI = GameObject.Find("PotMoney").transform;



    }

    // Update is called once per frame
    void Update()
    {
        if (gameState)
        {
            moneyUI.GetComponent<UnityEngine.UI.Text>().text = "Pot: " + potMoney.ToString();
            playerUI.GetComponent<UnityEngine.UI.Text>().text = getPlayerMoneyInfo();
            potGUI.GetComponent<UnityEngine.UI.Text>().text = "$" + potMoney.ToString();
        }
    }


    // ---------- Getters ----------
    public List<playerController> getPlayerList() { return playerList; }

    public playerController getCurretPlayer() { return currentPlayer; }

    public int getPlayerTurn() { return playerTurn; }

        public int getRevealBet() {return revealBet;}


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


        // This updates a UI with the new player whos playing
        // playerUI.GetComponent<UnityEngine.UI.Text>().text += playerName + "\n";
        // Instantiate player entry

    }

    private static ManualResetEvent holdForSomethin = new ManualResetEvent(false);


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
    private async void startNewGame(int startMoney, int ante)
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

        GameObject.Find("StartingGame").gameObject.SetActive(false);

        // Reset Player Variables
        foreach (var player in playerList)
        {
            player.money = startMoney;
            player.betted = 0;
            player.bettedRound = 0;
            player.folded = false;
            player.tappedOut = false;
            player.handRank = 10;
            player.getHoleCards().Clear();
            player.bestHand.Clear();
            player.handDescription = null;
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:2");
        }

        // Reset & Initialize Game Variables
        reveal = 0;
        rounds = 0;
        potMoney = 0;
        currentBet = 0;
        playerTurn = 0;
        tottalMoney = startMoney * playerList.Count;

        // Initialize the first player
        // Reset and Deal Cards
        cardController.resetCards();
        gameState = true;

        if (blindPlay)
            playBlinds();

        if (antePlay)
        {
            await anteUP();
        }
        else
            cardController.dealCards(playerList);

        currentPlayer = playerList[playerTurn];
        currentPlayer.underTheGun = true;
        currentPlayer.enterFrame(); 
        if (!blindPlay) 
            highestBidder = currentPlayer;
        controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh:3");

        // Debug.Log("---------------------- A new game of Texas Hold'Em Has begun ----------------------");
        // Debug.Log("It is now " + currentPlayer.username + "\'s turn. \n " + currentPlayer.getHoleCardsDesc());

    }

    private void playBlinds()
    {
        potMoney += playerList[playerTurn].requestFunds(ante / 2);
        playerTurn = (playerTurn + 1) % playerList.Count;
        potMoney += playerList[playerTurn].requestFunds(ante);
        highestBidder = playerList[playerTurn];
        playerTurn = (playerTurn + 1) % playerList.Count;

        currentBet = ante;
        revealBet = ante;
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
                {
                    await Task.Delay(1000);
                    
                } while (!player.pregameResponded);
                if(player.playRound)
                {
                    playersPlaying += 1;
                }
            }
            if(playersPlaying < 2)
            {
                foreach (var player in playerList)
                {
                    controlpads_glue.SendControlpadMessage(player.ID, "alert:Need more than one person to play a poker game you silly goose."); 
                }

            }
        }
        while (playersPlaying < 2);


        isPregame = false;
        foreach (var player in playerList)
        {
            player.pregameResponded = false;
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:Anteing is finished"); 
        } 
        
         
        var playing = new List<playerController>();
        foreach (var player in playerList)
            if (!player.playRound)
                player.folded = true;
            else
            {
                potMoney += player.requestFunds(ante);
                playing.Add(player);
            }

        cardController.dealCards(playing);
        resetBetRound();
        
        foreach (var player in playerList)
        {
            if (!player.folded)
                break;
            playerTurn = (playerTurn + 1) % playerList.Count;
        }

        currentBet += ante;
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
            potMoney -= player.sidePot;
            remainingPlayers.Remove(player);
        }

        // Determine the winners and then give them their share of the pot
        List<playerController> winners = cardController.DetermineWinners(remainingPlayers);
        int payment = potMoney / winners.Count;
        int remainder = potMoney % winners.Count;

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
        
        
        //Wait for everyone to ready up
        playerComms.waitingToReadyUp = true;

        foreach (var player in playerList)
            {
                player.readyResponded = false;
                player.readyForNextRound = false;
                player.status = "Ready up for the next round!";
            }

        foreach (var player in playerList)
            {
                do
                {
                    await Task.Delay(1000);
                    
                } while (!player.readyForNextRound);
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
            player.resetPlayer();
        }

        // Update Game Variables
        rounds++;
        reveal = 0;
        potMoney = 0;
        revealBet = 0;
        currentBet = 0;
        playerTurn = rounds % playerList.Count;
        sidePots.Clear();

        // Initialize first player of the next round
        // Reset and Deal Cards
        cardController.resetCards();
        if (blindPlay)
            playBlinds();

        if (antePlay)
            await anteUP();
        else
            cardController.dealCards(playerList, playerTurn);
        foreach (var player in playerList)
            controlpads_glue.SendControlpadMessage(player.ID, "refresh:4");

        currentPlayer = playerList[playerTurn];
        currentPlayer.underTheGun = true;
        currentPlayer.enterFrame();
        highestBidder = currentPlayer;

        controlpads_glue.SendControlpadMessage(currentPlayer.ID, "refresh:5");

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
                newBetRound();
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
        highestBidder = currentPlayer;
        revealBet = 0;
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

        // Debug.Log(currentPlayer.username + " sees the last bet of " + lastBet + " and raises it by " + amount + " putting in a tottal of " + money);

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

        // Debug.Log(currentPlayer.username + " needs " + revealBet + " to call.");
        // Debug.Log(currentPlayer.username + " calls the current bet of " + currentBet);
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

        // Debug.Log(currentPlayer.username + " has folded.");
        nextTurn();
    }

    public void resetBetRound()
    {
        foreach(var player in playerList)
        {
            player.bettedRound = 0;
        }
    }

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
