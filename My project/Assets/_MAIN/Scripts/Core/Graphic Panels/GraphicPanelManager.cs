using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelManager : MonoBehaviour
{
    public static GraphicPanelManager instance { get; private set; } // ezen keresztül lehet hozzáférni az osztályhoz más osztályok számára

    public const float DEFAULT_TRANSITION_SPEED = 1f; // alapértelmezett sebesség grafikus elemek átváltása között

    [field:SerializeField] public GraphicPanel[] allPanels { get; private set; } // minden panel egy tömbben, a UnityEditorban szerkeszthető

    private void Awake() // inicializálás
    {
        instance = this; 
    }

    public GraphicPanel GetPanel(string name) // panel azonositása
    {
        name = name.ToLower();

        foreach (var panel in allPanels) // végig megy a panel tömbön, név alapján keres
        {
            if (panel.panelName.ToLower() == name)
                return panel;
        }

        return null;
    }
}
