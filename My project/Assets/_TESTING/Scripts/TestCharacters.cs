using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        // Start is called before the first frame update
        void Start()
        {
            //Character Melory = CharacterManager.instance.CreateCharacter("Melory");
            //Character Demian = CharacterManager.instance.CreateCharacter("Demian");
            //Character Demian2 = CharacterManager.instance.CreateCharacter("Demian");
            //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character Melory = CharacterManager.instance.CreateCharacter("Melory");
            Character fifi = CharacterManager.instance.CreateCharacter("f");
            Character karina = CharacterManager.instance.CreateCharacter("Karina");
            Character demian = CharacterManager.instance.CreateCharacter("Demian");
            Character Adam = CharacterManager.instance.CreateCharacter("Adam");

            List<string> lines = new List<string>()
            {
                "Hello there!",
                "My name is Melory",
                "And you, who are you?",
                "Oh, {wa 1} that's very nice."
            };
            yield return Melory.Say(lines);

            Melory.SetNameColor(Color.red);
            Melory.SetDialogueColor(Color.green);
            Melory.SetNameFont(tempFont);
            Melory.SetDialogueFont(tempFont);
            yield return Melory.Say(lines);

            Melory.ResetConfigurationData();
            yield return Melory.Say(lines);

            lines = new List<string>()
            {
                "I am Demian",
                "More lines{c}Here!"
            };
            yield return demian.Say(lines);

            lines = new List<string>()
            {
                "I am Karina",
                "More lines{c}Here!"
            };
            yield return karina.Say(lines);

            lines = new List<string>()
            {
                "I am a narrator",
                "More lines{c}Here!"
            };
            yield return fifi.Say(lines);

            yield return Adam.Say("This is just one line. {a}It is a simple line.");

            Debug.Log("Finished");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}