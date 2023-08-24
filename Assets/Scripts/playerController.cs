using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // Player specific variables
    private string ID;
    private string username;
    private int playerNumber;
    private int turnNumber;

    // Game Specific Variables
    [SerializeField] private int money;
    private bool folded = false;
    private List<string> holeCards = new List<string>(); // this should be a maximum of 2 cards

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


    // Player Setters
    public void setName(string name) { username = name;}

    public void setPlayerIP(string client) { ID = client; }

    public void setMoney(int money) { this.money = money;}
    public void setPlayerNumber(int num) { playerNumber = num; }

    public void setTurnNumber(int turnNumber) { this.turnNumber = turnNumber; }


}
