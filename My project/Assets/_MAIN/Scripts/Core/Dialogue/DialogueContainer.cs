using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        //public TextMeshProUGUI nameText;
        public NameContainer nameContainer; // beszélő nevét kezeli
        public TextMeshProUGUI dialogueText; // dialogus szövegét jeleniti meg

        private CanvasGroupController cgController; // segit a rott - CanvasGroup láthatóságát kezelni

        // dialogus formázási beállitásai: szin, méret, stilus
        public void SetDialogueColor(Color color) => dialogueText.color = color;
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

        private bool initialized = false;
        public void Initialize()
        {
            if (initialized)
                return;

            cgController = new CanvasGroupController(DialogueSystem.instance, root.GetComponent<CanvasGroup>());
        }

        public bool isVisible => cgController.isVisible; // láthatóságot vizsgál

        // animációs funkciót használva a dialogus konténeke megjelenik - eltűnik
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed =1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }
}