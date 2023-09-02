using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // Player specific variables
    private string ID;
    private string username;
    private int playerNumber; // This is the order number that the player joined
    private int turnNumber;


    // Game Specific Variables
    private List<string> holeCards = new List<string>(); // this should be a maximum of 2 cards
    private int money; // Ammount of money a player has to play with
    private int playMoney; // Amount of money the player has played this round
    private bool folded = false;
    private bool isPlayerTurn = false;
    private bool isHost = false;
    private bool tappedOut = false;
    private bool gunPoint = false; // The first player to act, after the two blinds

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(getName() + " Is number: " + playerNumber);


    }


    // Player Getters
    // No Longer Playing this round
    public bool isFolded() { return folded; }

    // Still playing but has no money
    public bool isTappedOut() { return tappedOut; }

    public bool isTurn() { return isPlayerTurn; }

    public string getIP() { return ID; }

    public string getName() { return username; }

    public int getMoney() { return money; }

    public int getPlayerNumber() { return playerNumber;}

    public int getTurnNumber() {  return turnNumber;}

    public int getPlayMoney() { return playMoney; }


    // Player Setters
    public void setName(string name) { username = name;}

    public void setPlayerIP(string client) { ID = client; }

    public void setMoney(int money) { this.money = money;}

    public void setPlayerNumber(int num) { playerNumber = num; }

    public void setTurnNumber(int turnNumber) { this.turnNumber = turnNumber; }

    public void setHost() { isHost = true;}

    public void resetPlayMoney() { playMoney = 0; }

    public void fold() { folded = !folded; }

    public void payPlayer(int amount) { money += amount; }


    public int requestFunds(int amount) 
    {
        if (amount >= money)
        {
            Debug.Log(name + " is ALL IN!!!");
            tappedOut = true;
            int finalAmount = money;
            playMoney += money;

            money = 0;
            return finalAmount;
        }

        playMoney += amount;
        money -= amount;
        return amount;
    }
}
