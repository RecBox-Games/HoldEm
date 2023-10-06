using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class unitTest : MonoBehaviour
{
    // Objects to test
    [SerializeField] List<string> playerList;
    [SerializeField] controllerParse controllerParse;
    [SerializeField] gameController gameController;
    [SerializeField] cardController cardController;
    [SerializeField] int startMoney;
    [SerializeField] int minimumBet;
    [SerializeField] int raise;
    
    
    // Instance Variables
    private int playerID = 0;


    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Test the newPlayer function in controllerParse
    public void addNewPlayer(string name)
    {
        gameController.newPlayer(name, playerID.ToString());
        playerID++;
    }

    public void addListOfPlayers()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            gameController.newPlayer(playerList[i], playerID.ToString());
            playerID++;
        }
    }

    public void testBlindPlay() 
    { 
        gameController.blindPlay = true;
        Debug.Log("Playing with blinds.");
    }

    public void testAntePlay() 
    { 
        gameController.antePlay = true;
        Debug.Log("Playing with an ante of " + minimumBet);

    }

    public void testStartGame()
    {
        gameController.startGame(startMoney, minimumBet);
    }

    public void testRaise() { gameController.raise(this.raise); }

    public void testCall() { gameController.call(); }

    public void testFold() { gameController.fold(); }

    public void testTurnOrder() { Debug.Log(gameController.getTurnOrder()); }

    public void testMyTottalBet() { 
        Debug.Log("I\'ve Betted: " + gameController.getCurretPlayer().money); }

    public void testShowMyCards()
    {
        string final = "Cards:";
        foreach (var card in gameController.getCurretPlayer().getHoleCardsDesc()) { final += " " + card; }
        Debug.Log(final);
    }

    public void testNewGame() {
        gameController.gameState = false;
        gameController.startGame(); }

    public void testCommunityCards() { 
        Debug.Log(cardController.getCommunityCards()); }

    public void testRevealCards()
    {
        cardController.resetCards();
        StartCoroutine(pauseReveal(3));
    }

    IEnumerator pauseReveal(int seconds) 
    {
        yield return new WaitForSeconds(seconds);
        cardController.revealFlop();
        cardController.revealTurn();
        cardController.revealRiver();
    }
}
