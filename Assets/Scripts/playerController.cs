using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Setting
{
    public string name;

    public string value;
    public Setting(string name, string value)
    {
        this.name = name;
        this.value = value;
    }
}

public class playerController : MonoBehaviour
{
    [SerializeField] float velocity;
    public string ID { get; set; }
    public string username { get; set; }
    public int playerNumber { get; set; } // This is the number the player joined
    public bool isHost { get; set; } = false;
    public string playerColor {get; set;}
    

    // Money Variables
    public int money { get; set; } // Ammount of money a player has to play with
    public int betted { get; set; } // Amount of money the player has betted
    public int bettedRound { get; set; } // Amount of money the player has betted
    public bool underTheGun { get; set; } = false;


    // Game Specific Variables
    public bool playRound { get; set; } // Only used if antePlay is true
    
    public bool pregameResponded {get; set;} = false; //Used to see if a player has selected if they are playing or not
    
    public bool moneyResponded {get; set;} = false; //Used to see if a player has responded yet to the grab for money

    public bool moneyResponse {get; set;} //Used to determine if another player accepteed the grab for money

    public bool folded { get; set; } = false;
    public bool tappedOut { get; set; } = false;

    public bool autoSitOut {get; set; } = false;

    // Card Variables
    // These are the two cards in hand
    public int handRank { get; set; }
    public string handDescription { get; set; }
    private List<Card> holeCards = new List<Card>();
    public List<Card> bestHand { get; set; } = new List<Card>();

    public string status {get; set;}

    //Bonus Settings
    public List<Setting> CustomSettings {get; set;} = new List<Setting>();

    // GUI Variables
    private Transform entryTransform, namePlate;

    private void Start()
    {
        namePlate = transform.Find("Nameplate");
        Transform entryContainer = GameObject.Find("playerEntryContainer").transform;
        Transform entryTemplate = entryContainer.transform.Find("playerEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        int templateHight = 20;
        entryTransform = Instantiate(entryTemplate, entryContainer);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHight * (playerNumber + 1));
        entryTransform.gameObject.SetActive(true);
        
    }

    private void Update()
    {
        namePlate.GetComponent<TextMeshPro>().text = username;
        entryTransform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = username;
        entryTransform.Find("Money").GetComponent<UnityEngine.UI.Text>().text = money.ToString();
        if (folded)
            entryTransform.Find("InOut").GetComponent<UnityEngine.UI.Text>().text = "Folded";
        else
            entryTransform.Find("InOut").GetComponent<UnityEngine.UI.Text>().text = "-";

        //gameObject.transform.Find("Money").GetComponent<TextMeshPro>().text = "Bet: $" + betted + "\nMoney: $" + money;
    }

    // Getters
    public List<Card> getHoleCards() { return holeCards; }
    public string getHoleCardsDesc()
    {
        string finalString = "";
        foreach (var card in holeCards)
        {
            finalString += card.rank + " of " + card.suit + ", " ;
        }
        return finalString;
    }


    // Setters
    public void drawCard(Card card) { holeCards.Add(card); }


    // Instance Methods
    public void resetHoleCards() { holeCards.Clear(); }

    public void fold() { folded = true; }

    public void payPlayer(int amount) { money += amount; }

    public bool IsMoving = false;

    public void setColor(Material color) { gameObject.GetComponent<Renderer>().material = color; }

    public int requestFunds(int amount) 
    {
        Debug.Log("Requesting " + amount + " from " + username);
        if (amount >= money)
        {
            Debug.Log(name + " is ALL IN!!!");
            tappedOut = true;
            int finalAmount = money;
            betted += money;
            bettedRound += money;

            money = 0;
            return finalAmount;
        }

        betted += amount;
        bettedRound += amount;
        money -= amount;
        return amount;
    }

    public void enterFrame() 
    {
        StartCoroutine(movePlayer(new Vector3(0, 6, -23)));
    }

    public void exitFrame() 
    { 
        StartCoroutine(movePlayer(new Vector3(-30, 6, -23)));
    }


    private IEnumerator movePlayer(Vector3 vector, System.Action onComplete = null)
    {
        IsMoving = true; // Set a flag to indicate that the player is currently moving.
        while (transform.position != vector)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                vector,
                velocity * Time.deltaTime);    
            yield return null;
        }
        
        onComplete?.Invoke(); // Invoke the onComplete callback

        IsMoving = false; // Clear the flag to indicate that the movement is complete.



    }

    public void resetPlayer()
    {
        resetPosition();
        folded = false;
        betted = 0;
        bettedRound = 0;
        resetHoleCards();
    }

    public void resetPosition() { transform.position = new Vector3(30, 6, -23); }
}



