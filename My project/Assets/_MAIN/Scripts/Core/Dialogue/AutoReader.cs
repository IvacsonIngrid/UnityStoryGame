using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    public class AutoReader : MonoBehaviour
    {
        private const int DEFAULT_CHARACTERS_READ_PER_SECOND = 18; // alap�rtelmezett olvas�si sebess�g
        private const float READ_TIME_PADDING = 0.5F;
        private const float MAX_READ_TIME = 99f;
        private const float MIN_READ_TIME = 1f;
        private const string STATUS_TEXT_AUTO = "Auto";
        private const string STATUS_TEXT_SKIPPING = "Skipping";

        private ConversationManager conversationManager; // p�rbesz�d kezel�shez
        private TextArchitect architect => conversationManager.architect;

        public bool skip { get; set; } = false; // �tugorhat�-e a p�rbesz�d
        public float speed { get; set; } = 1f; // automatikus olvas�si sebess�g

        public bool isOn => co_running != null; // aut. olvas�s fut?
        private Coroutine co_running = null;

        [SerializeField] private TextMeshProUGUI statusText;
        [HideInInspector] public bool allowToggle = true; // auto -skip v�lt�st enged�lyez

        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
            statusText.text = string.Empty;
        }

        public void Enable() // aut. olvas�s enged�lyez�se
        {
            if (isOn)
                return;

            co_running = StartCoroutine(AutoRead());
        }

        // automatikus olvas�s letilt�sa
        public void Disable()
        {
            if (!isOn)
                return;

            StopCoroutine(co_running);
            skip = false;
            co_running = null;
            statusText.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            if (!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if (!architect.isBuilding && architect.currentText != string.Empty)
                DialogueSystem.instance.OnSystemPrompt_Next();

            while (conversationManager.isRunning)
            {
                if (!skip)
                {
                    while (!architect.isBuilding && !conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeStarted = Time.time;

                    while (architect.isBuilding || conversationManager.isWaitingOnAutoTimer)
                        yield return null;

                    float timeToRead = Mathf.Clamp(((float)architect.tmpro.textInfo.characterCount / DEFAULT_CHARACTERS_READ_PER_SECOND), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = (timeToRead / speed) + READ_TIME_PADDING;

                    yield return new WaitForSeconds(timeToRead);
                }
                else
                {
                    architect.ForceComplete();
                    yield return new WaitForSeconds(0.05f);
                }

                DialogueSystem.instance.OnSystemPrompt_Next();
            }

            Disable();
        }

        public void Toggle_Auto() // v�ltja az Auto m�dot
        {
            if (!allowToggle)
                return;
            
            bool prevState = skip;
            skip = false;

            if (prevState)
                Enable();
            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
                statusText.text = STATUS_TEXT_AUTO;
        }

        public void Toggle_Skip() // v�ltja a Skip m�dot
        {
            if (!allowToggle)
                return;
            
            bool prevState = skip;
            skip = true;

            if (!prevState)
                Enable();
            else
            {
                if (!isOn)
                    Enable();
                else
                    Disable();
            }

            if (isOn)
                statusText.text = STATUS_TEXT_SKIPPING;
        }
    }
}