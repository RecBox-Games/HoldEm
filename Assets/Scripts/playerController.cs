using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // Player specific variables
    [SerializeField] private int money;
    private string ip;
    private string username;
    private int playerNumber;

    // Game Specific Variables
    [SerializeField] Object playerUI;
    private bool folded = false;

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
    public string getIP() { return ip; }

    public string getName() { return username; }

    public int getMoney() { return money; }

    public bool isFolded() {  return folded; }


    // Player Setters
    public void setName(string name) { username = name;}

    public void setPlayerNumber(int num) { playerNumber = num; }

    public void setPlayerIP(string client)   { ip = client;}
}
