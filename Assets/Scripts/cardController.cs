using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class cardController : MonoBehaviour
{
    [SerializeField] List<Material> gameCards = new List<Material>();
    [SerializeField] GameObject flop1, flop2, flop3, turn, river;


    // Instance Variables
    private List<Material> shuffledDeck = new List<Material>();
    private Queue<Material> playDeck = new Queue<Material>();

    // Start is called before the first frame update
    void Start()
    {
        // Clear all cards on table
        resetCards();

        foreach (var card in gameCards) { shuffledDeck.Add(card); }
        shuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shuffleDeck()
    {
        Debug.Log("Shuffling the Deck!!! Shuffle Shuffle Shuffle");
        for (int i = 3; i > 0; i--)
        {
            for (int j = 0; j < shuffledDeck.Count; j++)
            {
                Material temp = shuffledDeck[j];
                int rand = Random.Range(j, shuffledDeck.Count);

                shuffledDeck[j] = shuffledDeck[rand];
                shuffledDeck[rand] = temp;
            }
        }
        foreach (var card in shuffledDeck) { playDeck.Enqueue(card); }
    }

    public void dealCards(List<playerController> playerList)
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var player in playerList)
            {
                player.holeCardAdd(playDeck.Dequeue());
            }
        }
    }

    public void resetCards()
    {
        flop1.GetComponent<Renderer>().enabled = false;
        flop2.GetComponent<Renderer>().enabled = false;
        flop3.GetComponent<Renderer>().enabled = false;
        turn.GetComponent<Renderer>().enabled = false;
        river.GetComponent<Renderer>().enabled = false;

        playDeck.Clear();
        shuffleDeck();
    }

    public void revealFlop()
    {
        Debug.Log("Revealing the Flop!!");
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playDeck.Dequeue();

        flop1.GetComponent<Renderer>().material = playDeck.Dequeue();
        flop2.GetComponent<Renderer>().material = playDeck.Dequeue();
        flop3.GetComponent<Renderer>().material = playDeck.Dequeue();
        flop1.GetComponent<Renderer>().enabled = true;
        flop2.GetComponent<Renderer>().enabled = true;
        flop3.GetComponent<Renderer>().enabled = true;
    }

    public void revealTurn()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playDeck.Dequeue();

        turn.GetComponent<Renderer>().material = playDeck.Dequeue();
        turn.GetComponent<Renderer>().enabled = true;
    }

    public void revealRiver()
    {
        // Burn a Card to prevent cheating (not sure how you can but tradition ya know)
        playDeck.Dequeue();

        river.GetComponent<Renderer>().material = playDeck.Dequeue();
        river.GetComponent<Renderer>().enabled = true;
    }
}
