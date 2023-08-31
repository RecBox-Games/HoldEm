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
    private bool folded = false;
    private bool isPlayerTurn = false;
    private bool isHost = false;

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
    public string getIP() { return ID; }

    public string getName() { return username; }

    public int getMoney() { return money; }

    public bool isFolded() {  return folded; }

    public bool isTurn() { return isPlayerTurn;}

    public int getPlayerNumber() { return playerNumber;}

    public int getTurnNumber() {  return turnNumber;}



    // Player Setters
    public void setName(string name) { username = name;}

    public void setPlayerIP(string client) { ID = client; }

    public void setMoney(int money) { this.money = money;}
    public void setPlayerNumber(int num) { playerNumber = num; }

    public void setTurnNumber(int turnNumber) { this.turnNumber = turnNumber; }

    public void setHost() { isHost = true;}

    public void fold() { folded = !folded; }

    public void payPlayer(int amount) { money += amount; }


    public int requestFunds(int amount) 
    {
        if (amount >= money)
        {
            Debug.Log(name + " is ALL IN!!!");
            int finalAmount = money;
            money = 0;

            return finalAmount;
        }

        money -= amount;
        return amount;
    }
}
