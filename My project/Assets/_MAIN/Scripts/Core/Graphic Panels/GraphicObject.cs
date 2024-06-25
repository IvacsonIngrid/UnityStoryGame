using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GraphicObject
{
    // konstans nevek, útvonalak
    private const string NAME_FORMAT = "Graphic - [{0}]";
    private const string MATERIAL_PATH = "Materials/layerTransitionMaterial";
    private const string MATERIAL_FIELD_COLOR = "_Color";
    private const string MATERIAL_FIELD_MAINTEX = "_MainTex";
    private const string MATERIAL_FIELD_BLENDTEX = "_BlendTex";
    private const string MATERIAL_FIELD_BLEND = "_Blend";
    private const string MATERIAL_FIELD_ALPHA = "_Alpha";
    public RawImage renderer;

    private GraphicLayer layer; // amin megjelenik

    // amennyiben videó, a hangot is engedélyezni és kezelni kell
    public bool isVideo => video != null;
    public bool useAudio => (audio != null ? !audio.mute : false);
    public VideoPlayer video = null;
    public AudioSource audio = null;

    public string graphicPath = "";
    public string graphicName { get; private set; }

    // átmenetekhez
    private Coroutine co_fadingIn = null;
    private Coroutine co_fadingOut = null;

    // inicializálások elvégzése textura esetén
    public GraphicObject(GraphicLayer layer, string graphicPath, Texture tex, bool immediate)
    {
        this.graphicPath = graphicPath;
        this.layer = layer; // megjelenitési réteg beállitása

        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        renderer = ob.AddComponent<RawImage>(); // hozzárendelés

        graphicName = tex.name;

        InitGraphic(immediate);

        renderer.name = string.Format(NAME_FORMAT, graphicName);

        Material unlitMaterial = new Material(Shader.Find("Unlit/Texture")); // fényhatások miatti beállitás
        //renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
        unlitMaterial.SetTexture(MATERIAL_FIELD_MAINTEX, tex);

        renderer.material = unlitMaterial;
    }

    // inicializálás elvégzése videó esetén
    public GraphicObject(GraphicLayer layer, string graphicPath, VideoClip clip, bool useAudio, bool immediate)
    {
        this.graphicPath = graphicPath;
        this.layer = layer; // megjelenitési réteg beállitása

        GameObject ob = new GameObject();
        ob.transform.SetParent(layer.panel);
        renderer = ob.AddComponent<RawImage>(); // hozzárendelés

        graphicName = clip.name;
        renderer.name = string.Format(NAME_FORMAT, graphicName);

        InitGraphic(immediate);

        Material unlitMaterial = new Material(Shader.Find("Unlit/Texture")); // árnyékhatások miatti beállitás
        RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
        //renderer.material.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
        unlitMaterial.SetTexture(MATERIAL_FIELD_MAINTEX, tex);
        
        renderer.material = unlitMaterial;
            
        video = renderer.AddComponent<VideoPlayer>();
        video.playOnAwake = true;
        video.source = VideoSource.VideoClip;
        video.clip = clip; // videoként felismerni
        video.renderMode = VideoRenderMode.RenderTexture;
        video.targetTexture = tex;
        video.isLooping = true; // ciklikus lejátszás

        video.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audio = video.AddComponent<AudioSource>(); // hanganyag miatt

        audio.volume = immediate ? 1 : 0;
        if (!useAudio)
            audio.mute = true;

        video.SetTargetAudioSource(0, audio);

        video.frame = 0;
        video.Prepare();
        video.Play(); // videó elinditása

        video.enabled = false;
        video.enabled = true;
    }

    // grafikus objektum inicializálása
    private void InitGraphic(bool immediate)
    {
        renderer.transform.localPosition = Vector3.zero;
        renderer.transform.localScale = Vector3.one;

        RectTransform rect = renderer.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        renderer.material = GetTransitionMaterial();

        float startingOpacity = immediate ? 1.0f : 0.0f;
        renderer.material.SetFloat(MATERIAL_FIELD_BLEND, startingOpacity);
        renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, startingOpacity);
    }

    // átmeneti effektus betöltése
    private Material GetTransitionMaterial()
    {
        Material mat = Resources.Load<Material>(MATERIAL_PATH);

        if (mat !=  null)
            return new Material(mat);

        return null;
    }

    GraphicPanelManager panelManager => GraphicPanelManager.instance;

    // fokozatos megjelenités
    public Coroutine FadeIn(float speed = 1f, Texture blend = null)
    {
        if (co_fadingOut != null)
            panelManager.StopCoroutine(co_fadingOut); // leállitja, ha aktiv a fadeOut

        if (co_fadingIn != null) // megnézi, nem-e fut már egy aktiv fadeIn
            return co_fadingIn;

        co_fadingIn = panelManager.StartCoroutine(Fading(1f, speed, blend)); // elinditja az átmenetet a cél átlátszóságot, sebességet, textúrát átadva

        return co_fadingIn;
    }

    // fokozatos eltőntetés
    public Coroutine FadeOut(float speed = 1f, Texture blend = null)
    {
        if (co_fadingIn != null)
            panelManager.StopCoroutine(co_fadingIn); // megjelenités leállitása

        if (co_fadingOut != null) // nincs-e már futó eltüntető effektus?
            return co_fadingOut;

        co_fadingOut = panelManager.StartCoroutine(Fading(0f, speed, blend)); // elinditja az átmenetet

        return co_fadingOut;
    }

    // átmenetek kezelése
    private IEnumerator Fading(float target, float speed, Texture blend) // átlátszóság, sebessége a váltásnak, váltás textúrája
    {
        // paraméter alapján végez beállitásokat
        bool isBlending = blend != null;
        bool fadeingIn = target > 0;

        renderer.material.SetTexture(MATERIAL_FIELD_BLENDTEX, blend);
        renderer.material.SetFloat(MATERIAL_FIELD_ALPHA, isBlending ? 1 : fadeingIn ? 0 : 1);
        renderer.material.SetFloat(MATERIAL_FIELD_BLEND, isBlending ? fadeingIn ? 0 : 1 : 1);

        string opacityParam = isBlending ? MATERIAL_FIELD_BLEND : MATERIAL_FIELD_ALPHA;

        // átlátszóság folyamatos frissitése, mig el nem éri a cél értéket a megadott sebesség mellett dolgozik
        while (renderer.material.GetFloat(opacityParam) != target)
        {
            float opacity = Mathf.MoveTowards(renderer.material.GetFloat(opacityParam), target, speed * Time.deltaTime);
            renderer.material.SetFloat(opacityParam, opacity);

            if (isVideo)
                audio.volume = opacity;

            yield return null;
        }

        co_fadingIn = null;
        co_fadingOut = null;

        if (target == 0) // ha elérte a 0-s célt, törölni a grafkus elemet
            Destroy();
        else
            DestroyBackgroundGraphicsOnLayer(); // elért cél 1, törli a régiek közül, a rétegből
    }

    // grafikus objektum törlése
    public void Destroy()
    {
        if (layer.currentGraphic != null && layer.currentGraphic.renderer.enabled == renderer)
            layer.currentGraphic = null;

        if (layer.oldGraphics.Contains(this))
            layer.oldGraphics.Remove(this); // kiveszi a listából is

        Object.Destroy(renderer.gameObject);
    }

    // régi grafikus objektumok eltávolitása
    private void DestroyBackgroundGraphicsOnLayer()
    {
        layer.DestroyOldGraphics();
    }
}
