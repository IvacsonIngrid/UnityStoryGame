using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class ChoicePanelTesting : MonoBehaviour
    {
        ChoicePanel panel;

        void Start()
        {
            StartCoroutine(WaitForPanel());
        }

        IEnumerator WaitForPanel()
        {
            // Várj egy keretet
            yield return null;

            panel = ChoicePanel.instance;

            if (panel == null)
            {
                Debug.LogError("ChoicePanel instance is null.");
                yield break;
            }

            string[] choices = new string[]
            {
                "I am in my room",
                "Oh... I don't know, where I am.",
                "What?",
                "You don't need to know, but you are angry, therefore I tell you everything."
            };

            panel.Show("Where are you now???", choices);

            while (panel.isWaitingOnUserChoice)
                yield return null;

            var decision = panel.lastDecision;

            Debug.Log($"The choice is: '{decision.choices[decision.answerIndex]}'({decision.answerIndex})");
        }
    }
}
