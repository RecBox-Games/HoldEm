using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // Player specific variables
    private string ip;
    private string username;
    private int money;
    private int playerNumber;

    // Game Specific Variables
    private bool folded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public string getIP() { return ip; }

    public string getName() { return username;}

    public int getMoney() { return money;}

    public bool isFolded() {  return folded;}



}
