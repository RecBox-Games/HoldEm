using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Card
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
    }

    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
}

public class PokerHandEvaluator : MonoBehaviour
{
    public static List<Card> FindBestPokerHand(List<Card> cards)
    {
        cards.Sort((c1, c2) => c2.rank.CompareTo(c1.rank));

        

        // Check for the best poker hand starting with the highest ranked cards
        if (IsRoyalFlush(cards, out List<Card> royalFlush))
        {
            Debug.Log("royal flush");
            return royalFlush;
        }
        if (IsStraightFlush(cards, out List<Card> straightFlush))
        {
            Debug.Log("straight flush");
            return straightFlush;
        }
        if (IsFourOfAKind(cards, out List<Card> fourOfAKind))
        {
                        Debug.Log("four of a kind");

            return fourOfAKind;
        }

        if (IsFullHouse(cards, out List<Card> fullHouse))
        {
                        Debug.Log("full house");

            return fullHouse;
        }

        if (IsFlush(cards, out List<Card> flush))
        {
            Debug.Log("flush");
            return flush;
        }

        if (IsStraight(cards, out List<Card> straight))
        {
                        Debug.Log("straight");

            return straight;
        }

        if (IsThreeOfAKind(cards, out List<Card> threeOfAKind))
        {
                        Debug.Log("three of a kind");

            return threeOfAKind;
        }

        if (IsTwoPair(cards, out List<Card> twoPair))
        {
                        Debug.Log("two pair");

            return twoPair;
        }

        if (IsOnePair(cards, out List<Card> onePair))
        {
                        Debug.Log("one pair");

            return onePair;
        }

        // If no specific hand is found, return the highest ranked five cards
        Debug.Log("High Card");
        return cards.Take(5).ToList();
    }

    // Implement methods to check for poker hands (Four of a Kind, Full House, etc.) 
    public static bool IsRoyalFlush(List<Card> cards, out List<Card> royalFlush)
    {
        royalFlush = null;
        if(IsStraightFlush(cards, out List<Card> candidate))
        {
            if(candidate[0].rank.ToString() == "Ace")
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
                if(IsStraight(suitCards, out List<Card> bbqSauce))
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
        if(IsThreeOfAKind(cards, out List<Card> threeOfAKind))
        {
            List<Card> leftovers = new List<Card>(cards);

            for(int i = 0; i < 3; i++)
            {
                leftovers.Remove(threeOfAKind[i]);
            }

            if(IsOnePair(leftovers, out List<Card> onePair))
            {
                fullHouse = threeOfAKind.Concat(onePair).ToList();
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
                if (cards[j+1].rank != cards[j].rank - 1)
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
                pairs.Add(cards[i+1]);
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

public class handEvaluate : MonoBehaviour
{
    void Start()
    {

        // Example usage:
        // Test Case 1: High Card
var cards1 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Three),
    new Card(Card.Suit.Clubs, Card.Rank.Ten),
    new Card(Card.Suit.Spades, Card.Rank.Ace),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};


// Test Case 2: One Pair
var cards2 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Seven),
    new Card(Card.Suit.Clubs, Card.Rank.Ten),
    new Card(Card.Suit.Spades, Card.Rank.Ace),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

// Test Case 3: Two Pair
var cards3 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Seven),
    new Card(Card.Suit.Clubs, Card.Rank.Ten),
    new Card(Card.Suit.Spades, Card.Rank.Ten),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

// Test Case 4: Three of a Kind
var cards4 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Seven),
    new Card(Card.Suit.Clubs, Card.Rank.Seven),
    new Card(Card.Suit.Spades, Card.Rank.Ace),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

// Test Case 5: Straight
var cards5 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Three),
    new Card(Card.Suit.Diamonds, Card.Rank.Four),
    new Card(Card.Suit.Clubs, Card.Rank.Five),
    new Card(Card.Suit.Spades, Card.Rank.Six),
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Ace)
};

// Test Case 6: Flush
var cards6 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Hearts, Card.Rank.Ten),
    new Card(Card.Suit.Hearts, Card.Rank.Two),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Hearts, Card.Rank.Eight),
    new Card(Card.Suit.Diamonds, Card.Rank.Six),
    new Card(Card.Suit.Clubs, Card.Rank.Five)
};

// Test Case 7: Full House
var cards7 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Seven),
    new Card(Card.Suit.Clubs, Card.Rank.Seven),
    new Card(Card.Suit.Spades, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

// Test Case 8: Four of a Kind
var cards8 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Diamonds, Card.Rank.Seven),
    new Card(Card.Suit.Clubs, Card.Rank.Seven),
    new Card(Card.Suit.Spades, Card.Rank.Seven),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

// Test Case 9: Straight Flush
var cards9 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Seven),
    new Card(Card.Suit.Hearts, Card.Rank.Eight),
    new Card(Card.Suit.Hearts, Card.Rank.Nine),
    new Card(Card.Suit.Hearts, Card.Rank.Ten),
    new Card(Card.Suit.Hearts, Card.Rank.Jack),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

// Test Case 10: Royal Flush
var cards10 = new List<Card>
{
    new Card(Card.Suit.Hearts, Card.Rank.Ten),
    new Card(Card.Suit.Hearts, Card.Rank.Jack),
    new Card(Card.Suit.Hearts, Card.Rank.Queen),
    new Card(Card.Suit.Hearts, Card.Rank.King),
    new Card(Card.Suit.Hearts, Card.Rank.Ace),
    new Card(Card.Suit.Diamonds, Card.Rank.Two),
    new Card(Card.Suit.Clubs, Card.Rank.Queen)
};

List<List<Card>> testList = new List<List<Card>>();
testList.Add(cards1);
testList.Add(cards2);
testList.Add(cards3);
testList.Add(cards4);
testList.Add(cards5);
testList.Add(cards6);
testList.Add(cards7);
testList.Add(cards8);
testList.Add(cards9);
testList.Add(cards10);



foreach(List<Card> sublist in testList){
    List<Card> bestHand = PokerHandEvaluator.FindBestPokerHand(sublist);

}                       

}
}
