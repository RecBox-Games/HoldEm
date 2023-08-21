using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class potController : MonoBehaviour
{
    private int tottalMoney = 0;
    private List<string> players = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ante()
    {

    }

    public void check()
    {

    }

    public void call()
    {

    }
    public void raise(int amount)
    {
        tottalMoney += amount;
    }

    public void fold(string username)
    {

    }
}
