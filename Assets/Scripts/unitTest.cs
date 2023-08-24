using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitTest : MonoBehaviour
{
    // Objects to test
    [SerializeField] GameObject controlInterface;
    [SerializeField] GameObject gameInterface;
    private controllerParse controllerParse;
    private gameController gameController;

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
        controllerParse.newPlayer(name, "0.0.0.0");
    }

    public void testStartGame(int startMoney)
    {
        gameController.startGame(startMoney);
    }

}
