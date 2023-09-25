using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInfoTable : MonoBehaviour
{
    private Transform entryContainer, entryTemplate;
    
    private void Awake()
    {
        entryContainer = transform.Find("Player Entry Container");
        entryTemplate = entryContainer.Find("Player Entry Template");
        
        entryTemplate.gameObject.SetActive(false);

    }
}
