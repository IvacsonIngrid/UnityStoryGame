using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConversationQueue : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        yield return null;

        List<string> lines = new List<string>()
        {
            "The 1. line from the original conversation.",
            "The 2. line from the original conversation.",
            "The 3. line from the original conversation."
        };

        yield return DialogueSystem.instance.Say(lines);

        DialogueSystem.instance.Hide();
    }

    void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            lines = new List<string>()
            {
                "This is the start of a conversation.",
                "We can keep it going!"
            };
            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.Enqueue(conversation);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            lines = new List<string>()
            {
                "This is an important conversation!",
                "Today is an international day!"
            };
            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.EnqueuePriority(conversation);
        }
    }
}
