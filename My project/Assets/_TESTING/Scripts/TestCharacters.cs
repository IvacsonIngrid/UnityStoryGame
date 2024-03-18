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

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            //Character Melory = CharacterManager.instance.CreateCharacter("Melory");
            //Character Demian = CharacterManager.instance.CreateCharacter("Demian");
            //Character Demian2 = CharacterManager.instance.CreateCharacter("Demian");
            //Character Adam = CharacterManager.instance.CreateCharacter("Livia");
            StartCoroutine(Test());
        }

        //masodik proba
        IEnumerator Test()
        {
            Character guard1 = CreateCharacter("Guard1 as Generic");
            Character guard2 = CreateCharacter("Guard2 as Generic");
            Character guard3 = CreateCharacter("Guard3 as Generic");

            guard1.Show();
            guard2.Show();
            guard3.Show();

            guard1.SetDialogueFont(tempFont);
            guard1.SetNameFont(tempFont);
            guard2.SetDialogueColor(Color.cyan);
            guard3.SetNameColor(Color.red);

            yield return guard1.Say("It is important");
            yield return guard2.Say("What happend?");
            yield return guard3.Say("I don't know...");

            yield return null;
        }

        //masodik proba
        /*IEnumerator Test()
        {
            yield return new WaitForSeconds(1f); 
            Character Melory = CharacterManager.instance.CreateCharacter("Melory");

            yield return new WaitForSeconds(1f);
            yield return Melory.Hide();
            yield return new WaitForSeconds(0.5f);
            yield return Melory.Show();
            yield return Melory.Say("Hello!");
        }*/

        //elso proba
        /*IEnumerator Test()
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
        }*/

        // Update is called once per frame
        void Update()
        {

        }
    }
}