using CHARACTERS;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")] // létrehozható Unity Assets részből (alapértelmezett fájl név, elérési út)
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        // alapértelmezett betűméretek
        public const float DEFAULT_FONTSIZE_DIALOGUE = 18;
        public const float DEFAULT_FONTSIZE_NAME = 22;

        public CharacterConfigSO characterConfigAsset; // karakter konfigurációs beállitásokhoz

        // alapértelmezett értékek beállitása a dialogus tulajdonságokhoz
        public Color defaultTextColor = Color.white;
        public TMP_FontAsset defaultFont;

        public float dialogueFontScale = 1f;
        public float defaultNameFontSize = DEFAULT_FONTSIZE_NAME;
        public float defaultDialogueFontSize = DEFAULT_FONTSIZE_DIALOGUE;
    }
}