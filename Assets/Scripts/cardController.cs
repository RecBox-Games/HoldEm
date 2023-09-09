using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class cardController : MonoBehaviour
{
    [SerializeField] List<Material> cardTextures = new List<Material>();
    [SerializeField] GameObject flop1, flop2, flop3, turn, river;
    [SerializeField] handEvaluate evaluator;


    // Instance Variables
    // This is the original deck, and it gets shuffled (always has 52 cards)
    private List<Card> cardDeck;
    private List<Card> communityCards = new List<Card>();
    // This deck gets shuffled, and handles textures. (always has 52 cards
    private List<Material> textureDeck = new List<Material>();
    private Queue<Material> playTextures= new Queue<Material>();
    private Queue<Card> playDeck = new Queue<Card>();


    // Start is called before the first frame update
    void Start()
    {
        resetDeck();
        foreach (var card in cardTextures) { textureDeck.Add(card); }
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
            for (int j = 0; j < textureDeck.Count; j++)
            {
                Material temp = textureDeck[j];
                Card temp2 = cardDeck[j];
                int rand = Random.Range(j, textureDeck.Count);

                textureDeck[j] = textureDeck[rand];
                cardDeck[j] = cardDeck[rand];
                textureDeck[rand] = temp;
                cardDeck[rand] = temp2;
            }
        }
        foreach (var card in textureDeck) { playTextures.Enqueue(card); }
        foreach (var card in cardDeck) {  playDeck.Enqueue(card); }
    }

    public void dealCards(List<playerController> playerList)
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var player in playerList)
            {
                player.drawCard(playDeck.Dequeue());
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

        playTextures.Clear();
        playDeck.Clear();
        communityCards.Clear();
        shuffleDeck();
    }

    public void revealFlop()
    {
        Debug.Log("Revealing the Flop!!");
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playTextures.Dequeue();
        playDeck.Dequeue();

        flop1.GetComponent<Renderer>().material = playTextures.Dequeue();
        flop2.GetComponent<Renderer>().material = playTextures.Dequeue();
        flop3.GetComponent<Renderer>().material = playTextures.Dequeue();
  
        for (int i = 0; i < 3; i++) { communityCards.Add(playDeck.Dequeue()); }
        flop1.GetComponent<Renderer>().enabled = true;
        flop2.GetComponent<Renderer>().enabled = true;
        flop3.GetComponent<Renderer>().enabled = true;
    }

    public void revealTurn()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playTextures.Dequeue();
        playDeck.Dequeue();

        turn.GetComponent<Renderer>().material = playTextures.Dequeue();
        communityCards.Add(playDeck.Dequeue());
        turn.GetComponent<Renderer>().enabled = true;
    }

    public void revealRiver()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playTextures.Dequeue();
        playDeck.Dequeue();

        river.GetComponent<Renderer>().material = playTextures.Dequeue();
        communityCards.Add(playDeck.Dequeue());
        river.GetComponent<Renderer>().enabled = true;
    }

    private void resetDeck()
    {
        cardDeck = new List<Card> {
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
    new Card(Card.Suit.Clubs, Card.Rank.King) };
    }
}
