using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.UI.Image;

public class Card
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank { 
        Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
}

public class cardController : MonoBehaviour
{
    [SerializeField] List<Material> cardTextures = new List<Material>();
    [SerializeField] List<GameObject> tableCards;
    [SerializeField] gameController gameController;
    [SerializeField] GameObject deck;
    [SerializeField] float velocity;

    // Instance Variables
    // This is the original deck, and it gets shuffled (always has 52 cards)
    private List<Vector3> originPos = new List<Vector3>();
    private List<Card> cardDeck = new List<Card> {
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
    private List<Card> communityCards = new List<Card>();
    // This deck gets shuffled, and handles textures. (always has 52 cards
    private List<Material> textureDeck = new List<Material>();
    private Queue<Material> playTextures= new Queue<Material>();
    private Queue<Card> playDeck = new Queue<Card>();


    // Start is called before the first frame update
    void Start()
    {
        foreach (var card in cardTextures) { textureDeck.Add(card); }
        foreach (var card in tableCards) 
        { 
            originPos.Add(card.transform.position);
            card.transform.position = deck.transform.position;
        
        }
        // Clear all cards on table
        resetCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string getCommunityCards()
    {
        string final = "";
        foreach (var card in communityCards)
            final += card.rank.ToString() + " of " + card.suit.ToString() + ", ";
     
        return final;
    }

    public void shuffleDeck()
    {
        Debug.Log("Shuffling the Deck!!! Shuffle Shuffle Shuffle");
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < textureDeck.Count; j++)
            {
                Material temp = textureDeck[j];
                Card temp2 = cardDeck[j];
                int rand = UnityEngine.Random.Range(j, textureDeck.Count);

                textureDeck[j] = textureDeck[rand];
                cardDeck[j] = cardDeck[rand];
                textureDeck[rand] = temp;
                cardDeck[rand] = temp2;
            }
        }
        
        foreach (var card in textureDeck) { playTextures.Enqueue(card);}   
        foreach (var card in cardDeck) {  playDeck.Enqueue(card); }
        
    }
    
    public void resetCards()
    {
        Vector3 deckBottom = deck.transform.position - new Vector3(0, .60f, 0);
        foreach (var card in tableCards)
        {
            // card.GetComponent<Renderer>().enabled = false;
            StartCoroutine(cardMove(card, deckBottom));
        }

        playTextures.Clear();
        playDeck.Clear();
        communityCards.Clear();
        shuffleDeck();
    }

    public void dealCards(List<playerController> playerList)
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var player in playerList)
            {
                playTextures.Dequeue();
                player.drawCard(playDeck.Dequeue());
            }
        }
    }

    public void revealFlop()
    {
        Debug.Log("Revealing the Flop!!");
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playTextures.Dequeue();
        playDeck.Dequeue();
  
        for (int i = 0; i < 3; i++) 
        {
            tableCards[i].GetComponent<Renderer>().material = playTextures.Dequeue();
            tableCards[i].GetComponent<Renderer>().enabled = true;
            StartCoroutine(cardMove(tableCards[i], originPos[i]));
            communityCards.Add(playDeck.Dequeue()); 
        }

        foreach (var player in gameController.getPlayerList()) { FindBestPokerHand(player); }
    }

    public void revealTurn()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playTextures.Dequeue();
        playDeck.Dequeue();

        tableCards[3].GetComponent<Renderer>().material = playTextures.Dequeue();
        tableCards[3].GetComponent<Renderer>().enabled = true;
        StartCoroutine(cardMove(tableCards[3], originPos[3]));
        communityCards.Add(playDeck.Dequeue());
        foreach (var player in gameController.getPlayerList()) { FindBestPokerHand(player); }

    }

    public void revealRiver()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playTextures.Dequeue();
        playDeck.Dequeue();

        tableCards[4].GetComponent<Renderer>().material = playTextures.Dequeue();
        tableCards[4].GetComponent<Renderer>().enabled = true;
        StartCoroutine(cardMove(tableCards[4], originPos[4]));
        communityCards.Add(playDeck.Dequeue());
        foreach (var player in gameController.getPlayerList()) { FindBestPokerHand(player); }
    }

    IEnumerator cardMove(GameObject card, Vector3 origin)
    {
        while (card.transform.position != origin)
        {
            card.transform.position = Vector3.MoveTowards(
                card.transform.position,
                origin,
                velocity * Time.deltaTime);
            yield return null;
        }
        card.transform.position = origin;  
    }
    /*
    IEnumerator reDeckCards(GameObject card)
    {
        
        while (card.transform.position != deckBottom)
        {
            card.transform.position = Vector3.MoveTowards(
                card.transform.position,
                deckBottom,
                velocity * Time.deltaTime);
            yield return null;
        }
        card.transform.position = deck.transform.position + new Vector3(0, .65f, 0);
    } */

    public static List<playerController> DetermineWinners(List<playerController> playerList)
    {
        if (playerList == null || playerList.Count == 0)
        {
            throw new ArgumentException("Invalid player list input");
        }

        playerList.Sort((p1, p2) => p1.handRank.CompareTo(p2.handRank));
        int winningRank = playerList[0].handRank;
        var winnerList = new List<playerController>();

        foreach (var playerHand in playerList)
        {
            Debug.Log(playerHand.username.ToString() + " " + playerHand.handDescription.ToString());
            foreach(var card in playerHand.bestHand )
            {
                Debug.Log(card.rank.ToString() + " of " + card.suit.ToString());

            }
        
            if (playerHand.handRank == winningRank)
            {
                winnerList.Add(playerHand);
            }
            else
            {
                break;
            }
            if (playerHand.handRank == winningRank)
            {
                winnerList.Add(playerHand);
            }
            else
            {
                break;
            }
        }




        //Check for multiple winners -Tiebreak
        if (winnerList.Count > 1)
        {
            //Cycle through each card, one by one
            for (int i = 0; i < 5; i++)
            {
                //If the winner list is down to only one, break loop
                if (winnerList.Count == 1)
                {
                    break;
                }
                //Sort the winner list by the rank of the i-th card
                winnerList.Sort((p1, p2) => p2.bestHand[i].rank.CompareTo(p1.bestHand[i].rank));

                //Create a variable of the rank of that card
                var highestCard = winnerList[0].bestHand[i].rank;

                //Remove all players in the winner list that don't have at least as high as 
                // that rank of card

                winnerList.RemoveAll(p => p.bestHand[i].rank < highestCard);

            }
        }

        return winnerList;
    }

    public void FindBestPokerHand(playerController player)
    {
        List<Card> cards = new List<Card>();
        foreach (var card in communityCards) { cards.Add(card); }
        foreach (var card in player.getHoleCards()) { cards.Add(card); }

        cards.Sort((c1, c2) => c2.rank.CompareTo(c1.rank));

        if (IsRoyalFlush(cards, out List<Card> royalFlush))
        {
            player.handDescription = "Royal Flush";
            player.bestHand = royalFlush;
            player.handRank = 0;
            return;
        }
        if (IsStraightFlush(cards, out List<Card> straightFlush))
        {
            player.handDescription = "Straight Flush";
            player.bestHand = straightFlush;
            player.handRank = 1;
            return;
        }
        if (IsFourOfAKind(cards, out List<Card> fourOfAKind))
        {
            player.handDescription = "Four of a Kind";
            player.bestHand = fourOfAKind;
            player.handRank = 2;
            return;
        }
        if (IsFullHouse(cards, out List<Card> fullHouse))
        {
            player.handDescription = "Full House";
            player.bestHand = fullHouse;
            player.handRank = 3;
            return;
        }
        if (IsFlush(cards, out List<Card> flush))
        {
            player.handDescription = "Flush";
            player.bestHand = flush;
            player.handRank = 4;
            return;
        }
        if (IsStraight(cards, out List<Card> straight))
        {
            player.handDescription = "Straight";
            player.bestHand = straight;
            player.handRank = 5;
            return;
        }
        if (IsThreeOfAKind(cards, out List<Card> threeOfAKind))
        {
            player.handDescription = "Three of a Kind";
            player.bestHand = threeOfAKind;
            player.handRank = 6;
            return;
        }
        if (IsTwoPair(cards, out List<Card> twoPair))
        {
            player.handDescription = "Two Pair";
            player.bestHand = twoPair;
            player.handRank = 7;
            return;
        }
        if (IsOnePair(cards, out List<Card> onePair))
        {
            player.handDescription = "One Pair";
            player.bestHand = onePair;
            player.handRank = 8;
            return;
        }
        List<Card> highCard = cards.Take(5).ToList();
        player.handDescription = "High Card";
        player.bestHand = highCard;
        player.handRank = 9;
    }


    // Implement methods to check for poker hands (Four of a Kind, Full House, etc.) 
    public static bool IsRoyalFlush(List<Card> cards, out List<Card> royalFlush)
    {
        royalFlush = null;
        if (IsStraightFlush(cards, out List<Card> candidate))
        {
            if (candidate[0].rank.ToString() == "Ace")
            {
                royalFlush = candidate;
                return true;
            }
            return false;
        }


        return false;
    }
    public static bool IsStraightFlush(List<Card> cards, out List<Card> straightFlush)
    {
        straightFlush = null;

        foreach (var suit in Enum.GetValues(typeof(Card.Suit)))
        {
            var suitCards = cards.Where(c => c.suit == (Card.Suit)suit).ToList();
            if (suitCards.Count >= 5)
            {
                if (IsStraight(suitCards, out List<Card> bbqSauce))
                {
                    straightFlush = bbqSauce;
                    return true;
                }
            }
        }
        return false;
    }
    public static bool IsFourOfAKind(List<Card> cards, out List<Card> fourOfAKind)
    {
        fourOfAKind = null;
        for (int i = 0; i <= cards.Count - 4; i++)
        {
            if (cards[i].rank == cards[i + 1].rank &&
                cards[i + 1].rank == cards[i + 2].rank &&
                cards[i + 2].rank == cards[i + 3].rank)
            {
                fourOfAKind = cards.GetRange(i, 4);
                fourOfAKind.Add(cards[0]); // Add the highest card as the fifth card
                return true;
            }
        }
        return false;
    }
    public static bool IsFullHouse(List<Card> cards, out List<Card> fullHouse)
    {
        fullHouse = null;
        if (IsThreeOfAKind(cards, out List<Card> threeOfAKind))
        {
            List<Card> leftovers = new List<Card>(cards);

            for (int i = 0; i < 3; i++)
            {
                leftovers.Remove(threeOfAKind[i]);
            }

            if (IsOnePair(leftovers, out List<Card> onePair))
            {

                fullHouse = threeOfAKind.Take(3).Concat(onePair.Take(2)).ToList();
                return true;
            }

        }

        return false;
    }
    public static bool IsFlush(List<Card> cards, out List<Card> flush)
    {
        flush = null;
        foreach (var suit in Enum.GetValues(typeof(Card.Suit)))
        {
            var suitCards = cards.Where(c => c.suit == (Card.Suit)suit).ToList();
            if (suitCards.Count >= 5)
            {
                flush = suitCards.OrderByDescending(c => c.rank).Take(5).ToList();
                return true;
            }
        }
        return false;
    }

    public static bool IsStraight(List<Card> cards, out List<Card> straight)
    {
        straight = null;
        for (int i = 0; i <= cards.Count - 5; i++)
        {
            bool isConsecutive = true;
            for (int j = i; j < i + 4; j++)
            {
                if (cards[j + 1].rank != cards[j].rank - 1)
                {
                    isConsecutive = false;
                    break;
                }
            }
            if (isConsecutive)
            {
                straight = cards.GetRange(i, 5);
                return true;
            }
        }
        return false;
    }

    public static bool IsThreeOfAKind(List<Card> cards, out List<Card> threeOfAKind)
    {
        threeOfAKind = null;
        for (int i = 0; i <= cards.Count - 3; i++)
        {
            if (cards[i].rank == cards[i + 1].rank && cards[i + 1].rank == cards[i + 2].rank)
            {
                threeOfAKind = cards.GetRange(i, 3);
                threeOfAKind.AddRange(cards.Where(c => c.rank != cards[i].rank).Take(2)); // Add the highest two non-matching cards
                return true;
            }
        }
        return false;
    }

    public static bool IsTwoPair(List<Card> cards, out List<Card> twoPair)
    {
        twoPair = null;
        var pairs = new List<Card>();
        for (int i = 0; i <= cards.Count - 2; i++)
        {
            if (cards[i].rank == cards[i + 1].rank)
            {
                pairs.Add(cards[i]);
                pairs.Add(cards[i + 1]);
                i++; // Skip the next card since it's part of the pair
            }
        }

        if (pairs.Count >= 4)
        {
            var tempTwoPair = new List<Card>(pairs.OrderByDescending(c => c.rank).Take(4));
            tempTwoPair.Add(cards.First(c => c.rank != tempTwoPair[0].rank && c.rank != tempTwoPair[2].rank)); // Add the highest non-matching card
            twoPair = tempTwoPair; // Assign the result to the 'twoPair' out parameter
            return true;
        }
        return false;
    }

    public static bool IsOnePair(List<Card> cards, out List<Card> onePair)
    {
        onePair = null;
        for (int i = 0; i <= cards.Count - 2; i++)
        {
            if (cards[i].rank == cards[i + 1].rank)
            {
                onePair = cards.GetRange(i, 2);
                onePair.AddRange(cards.Where(c => c.rank != cards[i].rank).Take(3)); // Add the highest three non-matching cards
                return true;
            }
        }
        return false;
    }
}
