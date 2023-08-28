using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitTest : MonoBehaviour
{
    // Objects to test
    [SerializeField] GameObject controlInterface;
    [SerializeField] GameObject gameInterface;
    [SerializeField] int startMoney;
    [SerializeField] int ante;
    
    
    // Instance Variables
    private controllerParse controllerParse;
    private gameController gameController;


    // Instance Variables
    [SerializeField] List<string> list = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        controllerParse = controlInterface.GetComponent<controllerParse>();
        gameController = gameInterface.GetComponent<gameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Test the newPlayer function in controllerParse
    public void testNewPlayer(string name)
    {
        gameController.newPlayer(name, "0.0.0.0");
    }

    public void testSeveralPlayers()
    {
        for (int i = 0; i < list.Count; i++)
        {
            gameController.newPlayer(list[i], "0.0.0.0");
        }
    }

    public void testStartGame()
    {
        gameController.startGame(startMoney, ante);
    }

    public void testRaise(int raise) { gameController.raise(raise); }

    public void testTurnOrder() { Debug.Log(gameController.getTurnOrder()); }

    public void testEndTurn()
    {
        gameController.endTurn();
        Debug.Log("It is currently " + gameController.getCurretPlayer() + "\'s turn");
    }

}
