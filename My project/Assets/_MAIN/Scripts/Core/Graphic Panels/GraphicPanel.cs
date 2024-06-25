using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GraphicPanel
{
    public string panelName; // panel neve
    public GameObject rootPanel; // gyökér rész - tartalmazza a rétegeket
    public List<GraphicLayer> layers { get; private set; } = new List<GraphicLayer>(); // rétegek listája

    public bool isClear => layers == null || layers.Count == 0 || layers.All(layer => layer.currentGraphic == null); // van-e aktiv grafikai réteg
    public GraphicLayer GetLayer(int layerDepth, bool createIfDoesNotExist = false) // visszaadja az adott mélységű réteget
    {
        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].layerDepth == layerDepth)
                return layers[i];
        }

        if (createIfDoesNotExist)
        {
            return CreateLayer(layerDepth); // létrehozza, ha nem létezett
        }

        return null;
    }

    // réteg létrehozása megadott mélységben
    private GraphicLayer CreateLayer(int layerDepth)
    {
        GraphicLayer layer = new GraphicLayer(); // új réteg létrehozása
        GameObject panel = new GameObject(string.Format(GraphicLayer.LAYER_OBJECT_NAME_FORMAT, layerDepth));
        RectTransform rect = panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasGroup>();
        panel.transform.SetParent(rootPanel.transform, false);

        // pozicionálás
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;

        // réteg tulajdonságaimak beállitása
        layer.panel = panel.transform;
        layer.layerDepth = layerDepth;

        int index = layers.FindIndex(l => l.layerDepth > layerDepth); // mélység azonositása, ebből nyer indexet, ebből állapitja meg a sorrendet
        if (index == -1)
            layers.Add(layer);
        else
            layers.Insert(index, layer);

        for (int i = 0; i < layers.Count; i++)
            layers[i].panel.SetSiblingIndex(layers[i].layerDepth);
        
        return layer;
    }

    // rétegek tisztitása paraméterek alapján
    public void Clear(float transitionSpeed = 1, Texture blendTexture = null, bool immediate = false)
    {
        foreach(var layer in layers)
            layer.Clear(transitionSpeed, blendTexture, immediate);
    }
}
