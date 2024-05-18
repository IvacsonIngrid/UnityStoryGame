using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicLayerTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Running());
        StartCoroutine(RunningLayers());
    }

    //masodik proba
    IEnumerator RunningLayers()
    {
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer0 = panel.GetLayer(0, true); 
        GraphicLayer layer1 = panel.GetLayer(1, true);

        layer0.SetVideo("Graphics/BG Videos/Nebula");
        layer1.SetTexture("Graphics/BG Images/Spaceshipinterior");

        yield return new WaitForSeconds(3);

        GraphicPanel cinematic = GraphicPanelManager.instance.GetPanel("Cinematic");
        GraphicLayer cinLayer = cinematic.GetLayer(0, true);

        Character Melory = CharacterManager.instance.CreateCharacter("Melory", true);

        yield return DialogueSystem.instance.Say("Melory", "Let's take a look at a picture on cinematic layer.");

        yield return new WaitForSeconds(3);

        cinLayer.SetTexture("Graphics/Gallery/pup");

        yield return DialogueSystem.instance.Say("Narrator", "Hello little dogs!");

        cinLayer.Clear();

        yield return new WaitForSeconds(3);

        panel.Clear();
    }

    //elso proba
    IEnumerator Running()
    {
        //GraphicPanelManager.instance.GetPanel("Background").GetLayer(0, true);
        GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
        GraphicLayer layer = panel.GetLayer(0, true);

        yield return new WaitForSeconds(3);

        Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/hurricane");

        //megjelenik a kep
        layer.SetTexture("Graphics/BG Images/2", blendingTexture: blendTex);
        //layer.SetTexture("Graphics/MyBackground/alaplap1");
        yield return new WaitForSeconds(3);

        //megjelenik a video
        //layer.SetVideo("Graphics/BG Videos/Fantasy Landscape");
        //layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", transitionSpeed: 0.01f, useAudio: true);
        layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", blendingTexture: blendTex);
        yield return new WaitForSeconds(3);

        //eltunik
        layer.currentGraphic.FadeOut();
        yield return new WaitForSeconds(3);
        Debug.Log(layer.currentGraphic);

        //layer.currentGraphic.renderer.material.SetColor("_Color", Color.blue);
    }
}
