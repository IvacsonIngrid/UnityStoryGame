using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CHARACTERS;

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        // párbeszéd konfigurációs beállitásai
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config => _config;

        // párbeszédek megjelenítéséhez szükséges elemek és funkciók
        public DialogueContainer dialogueContainer = new DialogueContainer();

        // párbeszédek kezeléséért és irányításáért felel
        public ConversationManager conversationManager {  get; private set; }

        // automatikus olvasó mód kezelése és a szövegépítés
        private AutoReader autoReader;
        private TextArchitect architect;

        [SerializeField] private CanvasGroup mainCanvas; // párbeszédablak része, láthatóságot kezel

        public static DialogueSystem instance { get; private set; } // globális hozzáférés érdekében

        // párbeszéd haladásával történhető változások
        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;
        public event DialogueSystemEvent onClear;

        public bool isRunningConversation => conversationManager.isRunning; // fut-e párbeszéd

        public DialogueContinuePrompt prompt; // folytatásért felugró ablak

        private CanvasGroupController cgController; // ablak láthatósága

        // egyetlen példány létrehozása, inicializálás
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }

        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
                return;

            architect = new TextArchitect(dialogueContainer.dialogueText); // szövegépitő
            conversationManager = new ConversationManager(architect); // konverzió kezelő
            cgController = new CanvasGroupController(this, mainCanvas);

            dialogueContainer.Initialize();

            if (TryGetComponent(out autoReader))
                autoReader.Initialize(conversationManager); // automatikus olvasómód inicializálása
        }

        // amikor a felhasználó továbblép a párbeszédben
        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();

            if (autoReader != null && autoReader.isOn)
                autoReader.Disable(); // kikapcsolja az automatikus olvasó módot, ha aktív
        }

        // rendszer halad a párbeszédben
        public void OnSystemPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        // tisztitja a párbeszéd ablakot
        public void OnSystemPrompt_Clear()
        {
            onClear?.Invoke();
        }

        public void OnStartViewingHistory()
        {
            prompt.Hide();
            autoReader.allowToggle = false;
            conversationManager.allowUserPrompt = false;

            if (autoReader.isOn)
                autoReader.Disable();
        }

        public void OnStopViewingHistory()
        {
            prompt.Show();
            autoReader.allowToggle = true;
            conversationManager.allowUserPrompt = true;
        }

        // beállítja a dialógus és a karakternevek stílusát és formázását a karakterkonfiguráció alapján
        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            ApplySpeakerDataToDialogueContainer(config);
        }

        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            //dialogueContainer.SetDialogueFontSize(config.dialogueFontSize * this.config.dialogueFontScale);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            //dialogueContainer.nameContainer.SetNameFontSize(config.nameFontSize);
        }

        // karakter nevének megjelenitése
        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
            {
                HideSpeakerName();
                dialogueContainer.nameContainer.nameText.text = "";
            }
        }

        // karakter nevének elrejtése
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();
        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() {$"{speaker} \"{dialogue}\""};
            return Say(conversation);
        }

        // párbeszéd elinditása
        public Coroutine Say (List<string> lines)
        {
            Conversation conversation = new Conversation(lines);
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public bool isVisible => cgController.isVisible;

        // dialogus ablak megjelenitése, elrejtése
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }
}
