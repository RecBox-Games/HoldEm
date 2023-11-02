using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EditorWindow is a base class for inheriting creating custom windows 
public class FindButtonsWithOnClick: EditorWindow
{
    [MenuItem("Tools/Find Buttons that have OnClick")]

        public static void FindButtons()
    {
        // get all buttons
        Button[] allButtons = GameObject.FindObjectsOfType<Button>();

        var buttonsWithOnClick =
            allButtons.Where(button => button.onClick.GetPersistentEventCount() > 0);

        foreach(Button button in buttonsWithOnClick) {
            Debug.Log(button.name, button.gameObject);
        }

        if (!buttonsWithOnClick.Any()) {
            Debug.Log("No Onclick Buttons");
        }
    }
    
}
