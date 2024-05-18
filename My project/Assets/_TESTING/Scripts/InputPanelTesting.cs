using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

public class InputPanelTesting : MonoBehaviour
{
    public InputPanel inputPanel;

    void Start()
    {
        StartCoroutine(Running());   
    }

    IEnumerator Running()
    {
        Character Melory = CharacterManager.instance.CreateCharacter("Melory", revealAfterCreation: true);

        yield return Melory.Say("Hi! What is your name?");
        inputPanel.Show("What is your name?");

        while (inputPanel.isWaitingOnUserInput)
            yield return null;

        string characterName = inputPanel.lastInput;

        yield return Melory.Say($"Nice to meet you, {characterName}!");
    }
}
