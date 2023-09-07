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
    public class PokerHandResult
    {
        public string HandDescription { get; set; }
        public List<Card> HandCards { get; set; }
        public int HandRank {get; set;}
    }
     public static PokerHandResult FindBestPokerHand(List<Card> cards)
    {
        cards.Sort((c1, c2) => c2.rank.CompareTo(c1.rank));

        if (IsRoyalFlush(cards, out List<Card> royalFlush))
        {
            return new PokerHandResult { HandDescription = "Royal Flush", HandCards = royalFlush, HandRank = 0 };
        }
        if (IsStraightFlush(cards, out List<Card> straightFlush))
        {
            return new PokerHandResult { HandDescription = "Straight Flush", HandCards = straightFlush, HandRank = 1 };
        }
        if (IsFourOfAKind(cards, out List<Card> fourOfAKind))
        {
            return new PokerHandResult { HandDescription = "Four of a Kind", HandCards = fourOfAKind, HandRank = 2 };
        }
        if (IsFullHouse(cards, out List<Card> fullHouse))
        {
            return new PokerHandResult { HandDescription = "Full House", HandCards = fullHouse, HandRank = 3 };
        }
        if (IsFlush(cards, out List<Card> flush))
        {
            return new PokerHandResult { HandDescription = "Flush", HandCards = flush, HandRank = 4 };
        }
        if (IsStraight(cards, out List<Card> straight))
        {
            return new PokerHandResult { HandDescription = "Straight", HandCards = straight, HandRank = 5 };
        }
        if (IsThreeOfAKind(cards, out List<Card> threeOfAKind))
        {
            return new PokerHandResult { HandDescription = "Three of a Kind", HandCards = threeOfAKind, HandRank = 6 };
        }
        if (IsTwoPair(cards, out List<Card> twoPair))
        {
            return new PokerHandResult { HandDescription = "Two Pair", HandCards = twoPair, HandRank = 7};
        }
        if (IsOnePair(cards, out List<Card> onePair))
        {
            return new PokerHandResult { HandDescription = "One Pair", HandCards = onePair, HandRank = 8};
        }
        List<Card> highCard = cards.Take(5).ToList();
        return new PokerHandResult { HandDescription = "High Card", HandCards = highCard, HandRank = 9 };
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


        
        // foreach (List<Card> sublist in testList)
        // {
        //     PokerHandEvaluator.PokerHandResult result = PokerHandEvaluator.FindBestPokerHand(sublist);
        //     string bestHandDescription = result.HandDescription;
        //     List<Card> bestHandCards = result.HandCards;
        //     int bestHandRank = result.HandRank;

        //     Debug.Log("Best Hand: " + bestHandDescription);
        //     Debug.Log("Best Hand Cards: " + string.Join(", ", bestHandCards.Select(card => card.rank + " of " + card.suit)));
        //     Debug.Log("Best Hand Rank: " + bestHandRank);
        // }


        //Kevin Look here for how to call it
        PokerHandEvaluator.PokerHandResult result = PokerHandEvaluator.FindBestPokerHand(cards1);
        string bestHandDescription = result.HandDescription;
        List<Card> bestHandCards = result.HandCards;
        int bestHandRank = result.HandRank;

        Debug.Log("Best Hand: " + bestHandDescription);
        Debug.Log("Best Hand Cards: " + string.Join(", ", bestHandCards.Select(card => card.rank + " of " + card.suit)));
        Debug.Log("Best Hand Rank: " + bestHandRank);


        }
}
