using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public string ID { get; set; }
    public string username { get; set; }
    // This is the number the player joined
    public int playerNumber { get; set; } 
    private int turnNumber;
    public bool isHost { get; set; } = false;

    // Money Variables
    public int money { get; set; } // Ammount of money a player has to play with
    public int betted { get; set; } // Amount of money the player has betted

    public int bettedRound { get; set; } // Amount of money the player has betted


    // Game Specific Variables
    public bool folded { get; set; } = false;
    public bool isPlayerTurn { get; set; } = false;
    public bool tappedOut { get; set; } = false;

    // Card Variables
    // These are the two cards in hand
    public int handRank { get; set; }
    public string handDescription { get; set; }
    private List<Card> holeCards = new List<Card>();
    public List<Card> bestHand { get; set; } = new List<Card>();


    // Getters
    public List<Card> getHoleCards() { return holeCards; }
    public string getHoleCardsDesc()
    {
        string finalString = "";
        foreach (var card in holeCards)
        {
            finalString += card.rank + " of " + card.suit + ", " ;
        }
        return finalString;
    }

    // Setters
    public void drawCard(Card card) { holeCards.Add(card); }


    // Instance Methods
    public void resetHoleCards() { holeCards.Clear(); }

    public void fold() { folded = !folded; }

    public void payPlayer(int amount) { money += amount; }
    public int requestFunds(int amount) 
    {
        if (amount >= money)
        {
            Debug.Log(name + " is ALL IN!!!");
            tappedOut = true;
            int finalAmount = money;
            betted += money;
            bettedRound += money;

            money = 0;
            return finalAmount;
        }

        betted += amount;
        bettedRound += amount;
        money -= amount;
        return amount;
    }
}