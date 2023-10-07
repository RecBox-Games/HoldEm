using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
    public int bettedRound { get; set; } // Amount of money the player has betted each reveal
    public int sidePot { get; set; } = 0;
    public bool underTheGun { get; set; } = false;


    // Game Specific Variables
    public bool playRound { get; set; } // Only used if antePlay is true
    public bool pregameResponded {get; set;} = false; //Used to see if a player has selected if they are playing or not
    public bool readyForNextRound {get; set;} = false; //Whether or not a player is ready for next round
    public bool readyResponded {get; set;} = false; //Whether or not a player has responded to the ready request
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
    private Transform startEntryTransform, entryTransform, namePlate;

    private void Start()
    {
        int templateHight = 20;
        namePlate = transform.Find("Nameplate");

        // Create a GUI entry
        Transform entryContainer = GameObject.Find("playerEntryContainer").transform;
        Transform entryStartContainer = GameObject.Find("playerJoinContainer").transform;
        Transform entryTemplate = entryContainer.transform.Find("playerEntryTemplate");
        Transform entryStartTemplate = entryStartContainer.transform.Find("nameTemplate");

        // Instantiate player entry
        entryTransform = Instantiate(entryTemplate, entryContainer);
        startEntryTransform = Instantiate(entryStartTemplate, entryStartContainer);
        if (playerNumber % 2 == 0)
        {
            entryTransform.Find("Background1").gameObject.SetActive(true);
        }
        else
        {
            entryTransform.Find("Background2").gameObject.SetActive(true);
            startEntryTransform.Find("NameBackground").gameObject.SetActive(true);
        }

        
        RectTransform startRectTransform = startEntryTransform.GetComponent<RectTransform>();
        if (playerNumber <= 11)
            startRectTransform.anchoredPosition = new Vector2(0, -templateHight * ((playerNumber - 1) % 11));
        else
            startRectTransform.anchoredPosition = new Vector2(150, -templateHight * ((playerNumber - 1) % 11));
        startEntryTransform.gameObject.SetActive(true);
        startEntryTransform.Find("nameEntry").gameObject.SetActive(true);

        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHight * (playerNumber + 1));
        entryTransform.gameObject.SetActive(true);
    }

    private void Update()
    {
        namePlate.GetComponent<TextMeshPro>().text = username;
        startEntryTransform.Find("nameEntry").GetComponent<UnityEngine.UI.Text>().text = username;
        entryTransform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = username;
        // startEntryTransform.Find("Name").GetComponent<UnityEngine.UI.Text>().text = username;
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

    public void payPlayer(int amount) { money += amount; }

    public bool IsMoving = false;

    public void setColor(Material color) { gameObject.GetComponent<Renderer>().material = color; }

    public int requestFunds(int amount) 
    {
        // Debug.Log("Requesting " + amount + " from " + username);
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

    public void enterFrame() { StartCoroutine(movePlayer(new Vector3(0, 6, -23)));}

    public void exitFrame() { StartCoroutine(movePlayer(new Vector3(-30, 6, -23)));}

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

    public void roundReset()
    {
        betted = 0;
        bettedRound = 0;
        handRank = 10;
        sidePot = 0;
        getHoleCards().Clear();
        bestHand.Clear();
        handDescription = null;
        if (!tappedOut)
            folded = false;
    }

    public void resetPlayer()
    {
        money = 0;
        betted = 0;
        sidePot = 0;
        bettedRound = 0;
        folded = false;
        tappedOut = false;
        handRank = 10;
        getHoleCards().Clear();
        bestHand.Clear();
        handDescription = null;
    }

    public void resetPosition() { transform.position = new Vector3(30, 6, -23); }
}



