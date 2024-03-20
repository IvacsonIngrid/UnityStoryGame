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
            Character generic = CreateCharacter("Guard1 as Generic");
            Character raelin = CreateCharacter("Raelin");
            Character demian = CreateCharacter("Demian");
            Character melory = CreateCharacter("Melory");

            generic.SetPosition(Vector2.zero);
            melory.SetPosition(new Vector2(0.2f, 0.05f));
            demian.SetPosition(Vector2.one);
            //melory.SetPosition(new Vector2(2, 1));

            //generic.Show();
            melory.Show();
            demian.Show();

            //elmegy balrol jobbra s vissza, smooth = lassitja a megerkezes pillanataban, nem olyan hirtelen tortenik
            yield return generic.Show();
            yield return generic.MoveToPosition(Vector2.one, smooth : true);
            yield return generic.MoveToPosition(Vector2.zero, smooth : true);


            generic.SetDialogueFont(tempFont);
            generic.SetNameFont(tempFont);
            melory.SetDialogueColor(Color.cyan);
            demian.SetNameColor(Color.red);

            yield return generic.Say("It is important");
            yield return melory.Say("What happend?");
            yield return demian.Say("I don't know...");

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