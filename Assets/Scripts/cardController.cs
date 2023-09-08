using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class cardController : MonoBehaviour
{
    [SerializeField] List<Material> gameCards = new List<Material>();
    [SerializeField] handEvaluate evaluator;
    [SerializeField] GameObject flop1, flop2, flop3, turn, river;


    // Instance Variables
    private List<Material> shuffledDeck = new List<Material>();
    private List<Card> shuffledCards = new List<Card> {
    new Card(Card.Suit.Spades, Card.Rank.Ace),
    new Card(Card.Suit.Spades, Card.Rank.Two),
    new Card(Card.Suit.Spades, Card.Rank.Three),
    new Card(Card.Suit.Spades, Card.Rank.Four),
    new Card(Card.Suit.Spades, Card.Rank.Five),
    new Card(Card.Suit.Spades, Card.Rank.Six),
    new Card(Card.Suit.Spades, Card.Rank.Seven),
    new Card(Card.Suit.Spades, Card.Rank.Eight),
    new Card(Card.Suit.Spades, Card.Rank.Nine),
    new Card(Card.Suit.Spades, Card.Rank.Ten),
    new Card(Card.Suit.Spades, Card.Rank.Jack),
    new Card(Card.Suit.Spades, Card.Rank.Queen),
    new Card(Card.Suit.Spades, Card.Rank.King),
    // Hearts
    new Card(Card.Suit.Hearts, Card.Rank.Ace),
    new Card(Card.Suit.Hearts, Card.Rank.Two),
    new Card(Card.Suit.Hearts, Card.Rank.Three),
    new Card(Card.Suit.Hearts, Card.Rank.Four),
    new Card(Card.Suit.Hearts, Card.Rank.Five),
    new Card(Card.Suit.Hearts, Card.Rank.Six),
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Hearts, Card.Rank.Eight),
    new Card(Card.Suit.Hearts, Card.Rank.Nine),
    new Card(Card.Suit.Hearts, Card.Rank.Ten),
    new Card(Card.Suit.Hearts, Card.Rank.Jack),
    new Card(Card.Suit.Hearts, Card.Rank.Queen),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    // Diamonds
    new Card(Card.Suit.Diamonds, Card.Rank.Ace),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Diamonds, Card.Rank.Three),
    new Card(Card.Suit.Diamonds, Card.Rank.Four),
    new Card(Card.Suit.Diamonds, Card.Rank.Five),
    new Card(Card.Suit.Diamonds, Card.Rank.Six),
    new Card(Card.Suit.Diamonds, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Eight),
    new Card(Card.Suit.Diamonds, Card.Rank.Nine),
    new Card(Card.Suit.Diamonds, Card.Rank.Ten),
    new Card(Card.Suit.Diamonds, Card.Rank.Jack),
    new Card(Card.Suit.Diamonds, Card.Rank.Queen),
    new Card(Card.Suit.Diamonds, Card.Rank.King),
    // Clubs
    new Card(Card.Suit.Clubs, Card.Rank.Ace),
    new Card(Card.Suit.Clubs, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Three),
    new Card(Card.Suit.Clubs, Card.Rank.Four),
    new Card(Card.Suit.Clubs, Card.Rank.Five),
    new Card(Card.Suit.Clubs, Card.Rank.Six),
    new Card(Card.Suit.Clubs, Card.Rank.Seven),
    new Card(Card.Suit.Clubs, Card.Rank.Eight),
    new Card(Card.Suit.Clubs, Card.Rank.Nine),
    new Card(Card.Suit.Clubs, Card.Rank.Ten),
    new Card(Card.Suit.Clubs, Card.Rank.Jack),
    new Card(Card.Suit.Clubs, Card.Rank.Queen),
    new Card(Card.Suit.Clubs, Card.Rank.King),
    };
    private List<Card> communityCards = new List<Card>();
    private Queue<Material> playDeck = new Queue<Material>();
    private Queue<Card> playCards = new Queue<Card>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var card in gameCards) { shuffledDeck.Add(card); }
        // Clear all cards on table
        resetCards();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shuffleDeck()
    {
        Debug.Log("Shuffling the Deck!!! Shuffle Shuffle Shuffle");
        for (int i = 3; i > 0; i--)
        {
            for (int j = 0; j < shuffledDeck.Count; j++)
            {
                Material temp = shuffledDeck[j];
                Card temp2 = shuffledCards[j];
                int rand = Random.Range(j, shuffledDeck.Count);

                shuffledDeck[j] = shuffledDeck[rand];
                shuffledCards[j] = shuffledCards[rand];
                shuffledDeck[rand] = temp;
                shuffledCards[rand] = temp2;
            }
        }
        foreach (var card in shuffledDeck) { playDeck.Enqueue(card); }
        foreach (var card in shuffledCards) {  playCards.Enqueue(card); }
    }

    public void dealCards(List<playerController> playerList)
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var player in playerList)
            {
                player.holeCardAdd(playDeck.Dequeue(), playCards.Dequeue());
            }
        }
    }

    public void resetCards()
    {
        flop1.GetComponent<Renderer>().enabled = false;
        flop2.GetComponent<Renderer>().enabled = false;
        flop3.GetComponent<Renderer>().enabled = false;
        turn.GetComponent<Renderer>().enabled = false;
        river.GetComponent<Renderer>().enabled = false;

        playDeck.Clear();
        playCards.Clear();
        communityCards.Clear();
        shuffleDeck();
    }

    public void revealFlop()
    {
        Debug.Log("Revealing the Flop!!");
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playDeck.Dequeue();
        playCards.Dequeue();

        flop1.GetComponent<Renderer>().material = playDeck.Dequeue();
        flop2.GetComponent<Renderer>().material = playDeck.Dequeue();
        flop3.GetComponent<Renderer>().material = playDeck.Dequeue();
  
        for (int i = 0; i < 3; i++) { communityCards.Add(playCards.Dequeue()); }
        flop1.GetComponent<Renderer>().enabled = true;
        flop2.GetComponent<Renderer>().enabled = true;
        flop3.GetComponent<Renderer>().enabled = true;
    }

    public void revealTurn()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playDeck.Dequeue();
        playCards.Dequeue();

        turn.GetComponent<Renderer>().material = playDeck.Dequeue();
        communityCards.Add(playCards.Dequeue());
        turn.GetComponent<Renderer>().enabled = true;
    }

    public void revealRiver()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playDeck.Dequeue();
        playCards.Dequeue();

        river.GetComponent<Renderer>().material = playDeck.Dequeue();
        communityCards.Add(playCards.Dequeue());
        river.GetComponent<Renderer>().enabled = true;
    }

    public List<playerController> roundWinner(List<playerController> playerList)
    {
        List<playerController> winner = playerList;

        if (playerList.Count > 1)
        {
            foreach (var player in playerList)
            {
                List<Card> hand = new List<Card>();
                foreach (var card in communityCards) { hand.Add(card); }
                foreach (var card in player.getPlayCards()) { hand.Add(card); }

                player.setHand(PokerHandEvaluator.FindBestPokerHand(hand));
                Debug.Log(player.getName() + " " + player.getHand().HandDescription);
            }
            
            // This checks the highest rank of hands among all players
            int highHandRank = 10;
            foreach (var player in playerList)
            {
                // This is for ties between two players who have the highest hand rank
                int highCardRank = 10;
                if (player.getHand().HandRank < highHandRank)
                {
                    winner.Clear();
                    highHandRank = player.getHand().HandRank;
                    highCardRank = PokerHandEvaluator.FindBestPokerHand(player.getPlayCards()).HandRank;
                    winner.Add(player);
                } 
                else if (player.getHand().HandRank == highHandRank)
                {
                    int compareCard = PokerHandEvaluator.FindBestPokerHand(player.getPlayCards()).HandRank;
                    // Check which player has the highest ranked card
                    // If they have the same ranked card add the player to the winner list
                    if (compareCard < highCardRank)
                    {
                        winner.Clear();
                        winner.Add(player);
                    } else
                    {
                        winner.Add(player);
                    }
                }
            }
        }   
        return winner;
    }
}
