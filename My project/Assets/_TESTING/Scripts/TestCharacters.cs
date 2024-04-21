using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;
using System.ComponentModel;

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
            //StartCoroutine(Test());
        }

        //hetedik
        IEnumerator Test()
        {
            Character_Sprite Melory = CreateCharacter("Melory") as Character_Sprite;
            Character_Sprite Karina = CreateCharacter("Karina") as Character_Sprite;

            yield return new WaitForSeconds(1);

            Melory.TransitionSprite(Melory.GetSprite("melory_angry"));
            Melory.Animate("Hop", true);
            yield return new WaitForSeconds(0.06f);
            Melory.Animate("Hop", false);
            yield return Melory.Say("What is this?");

            Karina.TransitionSprite(Karina.GetSprite("karina_shy"));
            Karina.Animate("Shiver", true);
            yield return Karina.Say("I don't know... but I don't like it!{a} It's freezing!");

            Melory.TransitionSprite(Melory.GetSprite("melory_angry"));
            Karina.Animate("Shiver", false);
            yield return Melory.Say("Oh, it's over!");
        }

        //hatodik proba
        /*IEnumerator Test()
        {
            //Melory jon letre elsonek, ezert o lesz Karina mogott
            Character_Sprite Melory = CreateCharacter("Melory") as Character_Sprite;
            Character_Sprite Karina = CreateCharacter("Karina") as Character_Sprite;
            Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;
            Character_Sprite GuardRed = CreateCharacter("Guard Red as Generic") as Character_Sprite;
            GuardRed.SetColor(Color.red);

            Melory.SetPosition(new Vector2(0.3f, 0));
            Karina.SetPosition(new Vector2(0.45f, 0.8f));
            Guard.SetPosition(new Vector2(0.6f, 0));
            Melory.SetPosition(new Vector2(0.65f, 0));


            // akinek a legmagasabb az erteke az lesz elobbre
            GuardRed.SetPriority(80);
            Karina.SetPriority(15);
            Melory.SetPriority(30);
            Guard.SetPriority(100);

            yield return new WaitForSeconds(2);

            CharacterManager.instance.SortCharacters(new string[] { "Melory", "Karina"});

            yield return new WaitForSeconds(2);

            CharacterManager.instance.SortCharacters();

            yield return new WaitForSeconds(2);

            CharacterManager.instance.SortCharacters(new string[] { "Karina", "Guard Red", "Guard", "Melory" });


            yield return null;
        }*/

        //otodik proba
        /*IEnumerator Test()
        {
            Character_Sprite Melory = CreateCharacter("Melory") as Character_Sprite;
            Character_Sprite Karina = CreateCharacter("Karina") as Character_Sprite;
            //Character_Sprite Raelin = CreateCharacter("Melory") as Character_Sprite;

            Melory.SetPosition(Vector2.zero);
            yield return new WaitForSeconds(3);

            yield return Melory.Flip(0.7f);
            yield return Melory.FaceLeft(immediate:false);
            yield return Karina.FaceLeft(immediate: false);

            yield return new WaitForSeconds(1);

            yield return Melory.UnHighlight();
            
            yield return new WaitForSeconds(1);

            yield return Melory.TransitionColor(Color.red);

            yield return new WaitForSeconds(1);

            yield return Melory.Highlight();

            yield return new WaitForSeconds(1);

            yield return Melory.TransitionColor(Color.white);

            //valtogatva a beszedhez kepest
            Melory.SetPosition(Vector2.zero);
            Karina.SetPosition(new Vector2(0.65f, 1));

            Melory.UnHighlight();
            yield return Karina.Say("Hello my friend!");

            Karina.UnHighlight();
            Melory.Highlight();
            Melory.TransitionSprite(Melory.GetSprite("melory_thinking"));
            yield return Melory.Say("Hi! What are you doing here?");

            Melory.UnHighlight();
            Karina.Highlight();
            Karina.TransitionSprite(Karina.GetSprite("karina_shy"));
            yield return Karina.Say("I want to say something. {c}Can we speak?... {a}Now!");
            
            yield return null;
        }*/

        //negyedik proba
        /*IEnumerator Test()
        {
            //Character_Sprite guard = CreateCharacter("Guard as Generic") as Character_Sprite;
            //Character_Sprite raelin = CreateCharacter("Raelin") as Character_Sprite;
            Character_Sprite demian = CreateCharacter("Demian") as Character_Sprite;
            Character_Sprite melory = CreateCharacter("Melory") as Character_Sprite;

            demian.isVisible = false;

            //guard.Show();
            //demian.Show();
            melory.Show();

            yield return new WaitForSeconds(1);

            //melory
            Sprite sprite = melory.GetSprite("melory_angry");
            melory.SetSprite(sprite);
            yield return new WaitForSeconds(1);
            sprite = melory.GetSprite("Default");
            melory.TransitionSprite(sprite);
            yield return new WaitForSeconds(1);
            melory.MoveToPosition(Vector2.zero);
            demian.Show();
            yield return demian.MoveToPosition(new Vector2(1, 1));
            
            yield return new WaitForSeconds(1);
            melory.SetColor(Color.black);
            yield return melory.TransitionColor(Color.red, speed: 0.3f);
            yield return melory.TransitionColor(Color.blue);
            yield return melory.TransitionColor(Color.yellow);
            yield return melory.TransitionColor(Color.white);

            //raelin
            Sprite body = raelin.GetSprite("Raelin_body1");
            Sprite face = raelin.GetSprite("Raelin_happy1");
            raelin.TransitionSprite(body);
            //yield return raelin.TransitionSprite(face, 1);

            //yield return new WaitForSeconds(1);

            //raelin.TransitionSprite(raelin.GetSprite("Raelin_fear1"));

            yield return null;


            // Átlátszóság értékének kiírása a Debug Logba
            Debug.Log($"Guard alpha: {guard.root.GetComponent<CanvasGroup>().alpha}");
            Debug.Log($"Demian alpha: {demian.root.GetComponent<CanvasGroup>().alpha}");
            Debug.Log($"Melory alpha: {melory.root.GetComponent<CanvasGroup>().alpha}");

            Debug.Log($"Guard aktív: {guard.root.gameObject.activeSelf}");
            Debug.Log($"Demian aktív: {demian.root.gameObject.activeSelf}");
            Debug.Log($"Melory aktív: {melory.root.gameObject.activeSelf}");

            Debug.Log($"Visible: {guard.isVisible}");

            yield return new WaitForSeconds(1);

            Sprite s1 = guard.GetSprite("Girl");
            guard.SetSprite(s1);

            Debug.Log($"Visible: {guard.isVisible}");

            yield return null;
        }*/

        //harmadik proba
        /*IEnumerator Test()
        {
            Character generic = CreateCharacter("Guard1 as Generic");
            Character_Sprite raelin = CreateCharacter("Raelin") as Character_Sprite;
            Character demian = CreateCharacter("Demian");
            Character melory = CreateCharacter("Melory");

            generic.SetPosition(Vector2.zero);
            melory.SetPosition(new Vector2(0.2f, 0.05f));
            //raelin.SetPosition(new Vector2(0.3f, 0.05f));
            demian.SetPosition(Vector2.one);
            //melory.SetPosition(new Vector2(2, 1));

            Sprite raelinBodySprite = raelin.GetSprite("Raelin_3");
            Sprite raelinFaceSprite = raelin.GetSprite("Raelin_21");

            raelin.SetSprite(raelinBodySprite, 0);
            raelin.SetSprite(raelinFaceSprite, 1);

            //generic.Show();
            melory.Show();
            demian.Show();
            raelin.Show();

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
        }*/

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