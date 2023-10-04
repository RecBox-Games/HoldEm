using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vault : MonoBehaviour
{
    private Transform potGUI;
    public playerController highestBidder { get; set; }
    public int potMoney { get; set; }
    public int tottalMoney { get; set; }
    public int currentBet { get; set; } = 0;     // Resets every round
    public int revealBet { get; set; } = 0;      // Resets every reveal


    // Start is called before the first frame update
    void Start()
    {
        potGUI = GameObject.Find("PotMoney").transform;
    }

    // Update is called once per frame
    void Update()
    {
        potGUI.GetComponent<UnityEngine.UI.Text>().text = "$" + potMoney.ToString();
    }
}
