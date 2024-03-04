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
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueText;
    }
}