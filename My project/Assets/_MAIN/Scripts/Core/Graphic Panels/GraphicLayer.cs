using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GraphicLayer
{
    public const string LAYER_OBJECT_NAME_FORMAT = "Layer: {0}"; // grafikus réteg neve
    public int layerDepth = 0; // mélysége a rétegnek
    public Transform panel;

    public GraphicObject currentGraphic = null; // aktuálisan megjelenitett grafikus objektum
    public List<GraphicObject> oldGraphics = new List<GraphicObject>(); // a már eddig megjelenitettek listája

    // textúra betültése, megjelenitése
    public Coroutine SetTexture(string filePath, float transitionSpeed = 1f, Texture blendingTexture = null, bool immediate = false)
    {
        Texture tex = Resources.Load<Texture>(filePath);

        if (tex == null)
        {
            Debug.LogError($"Could not load graphic texture from path '{filePath}.' Please ensure it exists within Resources!");
            return null;
        }

        return SetTexture(tex, transitionSpeed, blendingTexture, filePath);
    }
    public Coroutine SetTexture(Texture tex, float transitionSpeed = 1f, Texture blendingTexture = null, string filePath = "", bool immediate = false)
    {
        return CreateGraphic(tex, transitionSpeed, filePath, blendingTexture: blendingTexture, immediate: immediate);
    }

    // lehetővé teszivideók betöltését, megjelenitését
    public Coroutine SetVideo (string filePath, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null, bool immediate = false)
    {
        VideoClip clip = Resources.Load<VideoClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load graphic video from path '{filePath}.' Please ensure it exists within Resources!");
            return null;
        }

        return SetVideo(clip, transitionSpeed, useAudio, blendingTexture, filePath);
    }

    public Coroutine SetVideo(VideoClip clip, float transitionSpeed = 1f, bool useAudio = true, Texture blendingTexture = null, string filePath = "", bool immediate = false)
    {
        return CreateGraphic(clip, transitionSpeed, filePath, useAudio, blendingTexture, immediate: immediate);
    }

    // új grafikus elemet hoz létre
    private Coroutine CreateGraphic<T>(T graphicData, float transitionSpeed, string filePath, bool useAudioForVideo = true, Texture blendingTexture = null, bool immediate = false)
    {
        GraphicObject newGraphic = null;

        // új textúrát
        if (graphicData is Texture)
        {
 
            newGraphic = new GraphicObject(this, filePath, graphicData as Texture, immediate);
        }
            
        else if (graphicData is VideoClip) // új videót
            newGraphic = new GraphicObject(this, filePath, graphicData as VideoClip, useAudioForVideo, immediate);

        // ha még nem része, hozzáadni a megjelenitettek listájához
        if (currentGraphic != null && !oldGraphics.Contains(currentGraphic))
            oldGraphics.Add(currentGraphic);

        currentGraphic = newGraphic; // aktiv grafikus elem beállitása

        // ha nem azonnali, akkor a sebességnek és effektusnak megfelelően kezeli a grafikus elemek közötti váltást
        if (!immediate)
            return currentGraphic.FadeIn(transitionSpeed, blendingTexture);

        // türli a grafikus elemet
        DestroyOldGraphics();
        return null;
    }

    public void DestroyOldGraphics()
    {
        foreach (var g in oldGraphics) // bejárja a régieket tartalmazó listát
        { 
            Object.Destroy(g.renderer.gameObject); // törli őket
        }

        oldGraphics.Clear();
    }

    // az aktuális és a régiek is törlődnek
    public void Clear(float transitionSpeed = 1, Texture blendTexture = null, bool immediate = false)
    {
        if (currentGraphic != null) // aktuális törlése
        {
            if (!immediate)
                currentGraphic.FadeOut(transitionSpeed, blendTexture); // akár átmeneti effektussal is törlődik
            else
                currentGraphic.Destroy(); // azonnali törlés
        }

        foreach(var g in oldGraphics) // régiek törlése
        {
            if (!immediate)
                g.FadeOut(transitionSpeed, blendTexture); // átmeneti effektussal töröl
            else
                g.Destroy(); // azonnali törlés
        }
    }
}
