using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardController : MonoBehaviour
{
    [SerializeField] List<Material> gameCards = new List<Material>();
    [SerializeField] GameObject flop1, flop2, flop3, turn, river;


    // Instance Variables
    private List<Material> deck = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        deck = gameCards;
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
            for (int j = 0; j < deck.Count; j++)
            {
                Material temp = deck[j];
                int rand = Random.Range(j, deck.Count);

                deck[j] = deck[rand];
                deck[rand] = temp;
            }
        }
    }
}
