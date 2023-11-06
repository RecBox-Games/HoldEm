using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Vault : MonoBehaviour
{
    private Transform potGUI;
    public playerController lastRaise { get; set; }
    public int potMoney { get; set; } = 0;
    public int totalMoney { get; set; } = 0;
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
        foreach (var player in players)
        {
            potMoney += player.requestFunds(minimumBet);
        }
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
        // If the player is trying to raise more money than they have, go all in.
        amount = Mathf.Max(0, Mathf.Min(amount, player.money - currentBet + player.betted));

        // First, let's make the player call the current bet if they haven't already.
        int toCall = currentBet - player.betted;
        int calledAmount = player.requestFunds(toCall);

        // Now, let's handle the raise amount.
        // If the amount is negative or zero, it means the player can't actually raise and is just calling.
        int raisedAmount = (amount > 0) ? player.requestFunds(amount) : 0;

        // Update the current bet and reveal bet only if the raise is valid.
        if (raisedAmount > 0)
        {
            currentBet += raisedAmount;
            revealBet += raisedAmount;
        }

        // Add the called and raised amounts to the pot.
        potMoney += calledAmount + raisedAmount;

        // Set the current player as the last raiser if an actual raise was made.
        if (raisedAmount > 0)
        {
            lastRaise = player;
        }

        // Log the result of the raise attempt.
        Debug.Log(player.username + " called " + calledAmount + " and raised " + raisedAmount + ". Current pot is " + potMoney);
    }
    public void call(playerController player)
    {
        int money;
        if (player.betted < currentBet)
            money = player.requestFunds(currentBet - player.betted);
        else
            money = player.requestFunds(revealBet);

        potMoney += money;
    }
}
