using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class unitTest : MonoBehaviour
{
    // Objects to test
    [SerializeField] GameObject controlInterface;
    [SerializeField] GameObject gameInterface;
    [SerializeField] GameObject cardInterface;
    [SerializeField] int startMoney;
    [SerializeField] int ante;
    [SerializeField] int bet;
    [SerializeField] int raise;
    
    
    // Instance Variables
    private controllerParse controllerParse;
    private gameController gameController;
    private cardController cardController;


    // Instance Variables
    [SerializeField] List<string> list = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        controllerParse = controlInterface.GetComponent<controllerParse>();
        gameController = gameInterface.GetComponent<gameController>();
        cardController = cardInterface.GetComponent<cardController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Test the newPlayer function in controllerParse
    public void testNewPlayer(string name)
    {
        gameController.newPlayer(name, "0.0.0.0");
    }

    public void testSeveralPlayers()
    {
        for (int i = 0; i < list.Count; i++)
        {
            gameController.newPlayer(list[i], "0.0.0.0");
        }
    }

    public void testStartGame()
    {
        gameController.startGame(startMoney, ante);
    }

    public void testRaise() { gameController.raise(this.raise); }

    public void testCall() { gameController.call(); }

    public void testFold() { gameController.fold(); }

    public void testTurnOrder() { Debug.Log(gameController.getTurnOrder()); }

    public void testEndTurn()
    {
        gameController.nextTurn();
        Debug.Log("It is currently " + gameController.getCurretPlayer() + "\'s turn");
    }

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

    public void testBlindPlay() { gameController.blindPlay = true; }

    public void testAntePlay() { gameController.antePlay = true; }

    public void testHandEvaluator()
    {
                //Kevin, Start Here. I've defined a list of "Cards" as such.
        //These represent different players 
        //hands. Somewhere in the game, you will need to combine the hole
        //0cards with the community cards for each player

        // Test Case 1: High Card
        var cards1 = new List<Card>
        {
            new Card(Card.Suit.Hearts, Card.Rank.Eight),
            new Card(Card.Suit.Diamonds, Card.Rank.Seven),
            new Card(Card.Suit.Clubs, Card.Rank.Jack),
            new Card(Card.Suit.Spades, Card.Rank.Ten),
            new Card(Card.Suit.Hearts, Card.Rank.Three),
            new Card(Card.Suit.Diamonds, Card.Rank.Two),
            new Card(Card.Suit.Clubs, Card.Rank.Ace)
        };

        // Test Case 2: High Card
        var cards2 = new List<Card>
        {
            new Card(Card.Suit.Spades, Card.Rank.Two),
            new Card(Card.Suit.Hearts, Card.Rank.Five),
            new Card(Card.Suit.Clubs, Card.Rank.Six),
            new Card(Card.Suit.Diamonds, Card.Rank.Nine),
            new Card(Card.Suit.Spades, Card.Rank.Jack),
            new Card(Card.Suit.Hearts, Card.Rank.Ace),
            new Card(Card.Suit.Clubs, Card.Rank.King)
        };

        // Test Case 3: High Card
        var cards3 = new List<Card>
        {
            new Card(Card.Suit.Diamonds, Card.Rank.Ten),
            new Card(Card.Suit.Hearts, Card.Rank.Seven),
            new Card(Card.Suit.Clubs, Card.Rank.Three),
            new Card(Card.Suit.Spades, Card.Rank.Six),
            new Card(Card.Suit.Hearts, Card.Rank.Queen),
            new Card(Card.Suit.Diamonds, Card.Rank.Ace),
            new Card(Card.Suit.Clubs, Card.Rank.King)
        };

        // Test Case 4: High Card
        var cards4 = new List<Card>
        {
            new Card(Card.Suit.Spades, Card.Rank.Four),
            new Card(Card.Suit.Diamonds, Card.Rank.Three),
            new Card(Card.Suit.Clubs, Card.Rank.Five),
            new Card(Card.Suit.Hearts, Card.Rank.Six),
            new Card(Card.Suit.Spades, Card.Rank.Nine),
            new Card(Card.Suit.Diamonds, Card.Rank.Ace),
            new Card(Card.Suit.Hearts, Card.Rank.Ten)
        };







        //Kevin, Here, I made a list of the lists of cards. Ignore This
        //This is only to ease testing.
        List<List<Card>> testList = new List<List<Card>>();
        testList.Add(cards1);
        testList.Add(cards2);
        testList.Add(cards3);
        testList.Add(cards4);



        /*
        

        //Kevin. This is where you'll follow in somewhat a similar pattern. 
        
        //We need to make a list of PlayerHandInfo objects.
        //PlayerHand Info is described above as:

                //      public class PlayerHandInfo
                // {
                //     public PokerHandResult Hand {get; set;}
                //     public int PlayerNumber {get;set;}
                // }


                //PokerHand Result is defined as     
                    // public class PokerHandResult
                    // {
                    //     public string HandDescription { get; set; }
                    //     public List<Card> HandCards { get; set; }
                    //     public int HandRank {get; set;}
                    // }
        //So PokerHand in essence contains..

        // HandCards- a players best 5 cards of the seven available, saved as a 
        // list of Card objects.

        //HandRank - Something I use internally to sort how good a hand is
        // 0 is Royal Flush, 1 is Straight Flush, so on...

        //Hand Description - A string that says Royal Flush, Straight Flush, etc.



        var groupTestList = new List<PlayerHandInfo>();

        //Ignore count, it is effectively the playernumber for my test cases
        int count = 0;

        //For each list of cards in my testlist
        foreach (List<Card> sublist in testList)
        {
            //Create a new PlayerHandInfo object, set hand = the sublist, set playernumber = count

            var addedInfo = new PlayerHandInfo { Hand = PokerHandEvaluator.FindBestPokerHand(sublist) , PlayerNumber = count  };
            groupTestList.Add(addedInfo);
            count = count + 1;
        }

        //This is the big mama of functions. This is where you throw that list of PlayerHandInfos
        // It will output a list of PlayerHandInfo objects that have won that round
        //This takes into account tiebreakers. bbqSauce is what I named that list of winners
        
        List<PlayerHandInfo>  bbqSauce = PokerHandEvaluator.DetermineWinners(groupTestList);

        //Below is just some stuff to print the test cases
        string bestHandDescription = bbqSauce[0].Hand.HandDescription.ToString();

        Debug.Log("Best Hand: " + bestHandDescription);

        foreach (PlayerHandInfo player in bbqSauce)
        {

            Debug.Log("Player " + (player.PlayerNumber + 1).ToString() + " Hand: " + string.Join(", ", player.Hand.HandCards.Select(card => card.rank + " of " + card.suit)));

        }*/
    }

}
