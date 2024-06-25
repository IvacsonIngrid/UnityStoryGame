using DIALOGUE; // párbeszédhez köthető funkciók elérése érdekében
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    [System.Serializable]
    public class CharacterConfigData
    {
        // egy karakter megjelenéséhez szükséges alap tulajdonságok (szüvegdobozhoz szükséges beállitások is - név, név szine...)
        public string name;
        public string alias; // becenév
        public Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;

        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float nameFontSize;
        public float dialogueFontSize;

        // betűméret aránya
        public float nameFontScale;
        public float dialogueFontScale;

        // másolat készitése az adatokról
        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;
            result.characterType = characterType;
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.dialogueFontSize = dialogueFontSize;
            result.nameFontSize = nameFontSize;

            result.nameFontScale = nameFontScale;
            result.dialogueFontScale = dialogueFontScale;

            return result;
        }

        // amennyiben nincs beállitva egy karakternek semmi, úgy az alapértelmezett tulajdonságokkal ruházzák fel
        private static Color defaultColor => DialogueSystem.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;
        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.alias = "";
                result.characterType = Character.CharacterType.Text;

                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;
                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.dialogueFontSize = DialogueSystem.instance.config.defaultDialogueFontSize;
                result.nameFontSize = DialogueSystem.instance.config.defaultNameFontSize;

                return result;
            }
        }
    }
}