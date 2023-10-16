using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Vault : MonoBehaviour
{
    private Transform potGUI;
    public playerController lastRaise { get; set; }
    public int potMoney { get; set; } = 0;
    public int tottalMoney { get; set; } = 0;
    public int currentBet { get; set; } = 0;     // Resets every round
    public int revealBet { get; set; } = 0;      // Resets every reveal
    public int minimumBet { get; set; } = 1;


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

    public void ante(List<playerController> players)
    {
        foreach(var player in players)
        {
            potMoney += player.requestFunds(minimumBet);
        }
        lastRaise = players[0];
        // revealBet = minimumBet;
        currentBet = minimumBet;
    }

    public void blind(playerController smallBlind, playerController bigBlind)
    {
        potMoney += smallBlind.requestFunds(minimumBet / 2);
        potMoney += bigBlind.requestFunds(minimumBet);

        revealBet = minimumBet;
        currentBet = minimumBet;
        lastRaise = bigBlind;
    }


    public void raise(playerController player, int amount)
    {
        int money;
        if (player.betted < currentBet)
        {
            // request funds to call the current tottal bet
            int callBet = player.requestFunds(currentBet - player.betted);
            // requst funds to raise the current tottal bet
            int raiseMoney = player.requestFunds(amount);

            money = callBet + raiseMoney;
            currentBet += raiseMoney;
            revealBet += raiseMoney;
        }
        else
        {
            money = player.requestFunds(amount);
            currentBet += money;
            revealBet += money;
        }

        // Increment the amount in the pot and then set the current
        // player as the highest bidder
        potMoney += money;
        lastRaise = player;
    }

    public void call(playerController player)
    {
        Debug.Log(player.betted + " current " + currentBet + " reveal " + revealBet);
        int money;
        if (player.betted < currentBet)
            money = player.requestFunds(currentBet - player.betted);
        else
            money = player.requestFunds(revealBet);

        potMoney += money;
    }
}
