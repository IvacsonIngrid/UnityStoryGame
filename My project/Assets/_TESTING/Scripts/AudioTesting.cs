using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Running());
    }

    Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

    IEnumerator Running()
    {
        //1- 2- 3- 4 proba eseten kell
        //negydik proba
        yield return new WaitForSeconds(1);

        Character_Sprite Melory = CreateCharacter("Melory") as Character_Sprite;
        Melory.Show();

        Character Me = CreateCharacter("Me");
        GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/villagenight");
        AudioManager.instance.PlayTrack("Audio/Ambience/RainyMood", 0);
        AudioManager.instance.PlayTrack("Audio/Music/Calm", 1, pitch: 0.7f);

        yield return DialogueSystem.instance.Say("Melory", "We can have multiple channels!");

        AudioManager.instance.StopTrack(1);

        /*yield return new WaitForSeconds(0.5f);
        yield return DialogueSystem.instance.Say("Narrator", "Can we see your room?");

        GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/5");
        AudioManager.instance.PlayTrack("Audio/Music/Calm2", volumeCap: 0.5f);
        //AudioManager.instance.PlayTrack("Audio/Music/Calm2");
        //AudioManager.instance.PlayTrack("Audio/Music/Calm2", startingVolume: 0.7f);
        AudioManager.instance.PlayVoice("Audio/Voices/wakeup");

        Melory.SetSprite(Melory.GetSprite("melory_angry"));
        Melory.MoveToPosition(new Vector2(0.7f, 0), speed: 0.5f);
        yield return DialogueSystem.instance.Say("Melory", "Yes, of course!");
        yield return DialogueSystem.instance.Say("Melory", "Let me show you the engine room.");

        GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/EngineRoom");
        AudioManager.instance.PlayTrack("Audio/Music/Upbeat", volumeCap: 0.8f);

        yield return null;*/


        //harmadik proba
        /*Character Me = CreateCharacter("Me");

        AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic", loop: true);

        yield return DialogueSystem.instance.Say("Me", "Please turn off the radio.");

        AudioManager.instance.StopSoundEffect("RadioStatic");
        AudioManager.instance.PlayVoice("Audio/Voices/wakeup");

        yield return DialogueSystem.instance.Say("Melory", "Okay!");*/

        //masodik proba
        /*AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic", loop: true);
        yield return DialogueSystem.instance.Say("Melory", "I'm going to turn off the radio!");

        AudioManager.instance.StopSoundEffect("RadioStatic");
        yield return DialogueSystem.instance.Say("Melory", "It's off now!");*/

        //elso proba
        /*AudioManager.instance.PlaySoundEffect("Audio/SFX/thunder_strong_01");
        yield return new WaitForSeconds(1f);
        Melory.Animate("Hop", true);
        yield return new WaitForSeconds(0.6f);
        Melory.Animate("Hop", false);
        yield return DialogueSystem.instance.Say("Melory", "Whoaaaaa!");
        Melory.TransitionSprite(Melory.GetSprite("melory_angry"));*/
    }
}
