using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInfoTable : MonoBehaviour
{
    private Transform entryContainer, entryTemplate;
    
    private void Awake()
    {
        entryContainer = transform.Find("Player Entry Container");
        entryTemplate = transform.Find("Player Entry Template");

    }
}
