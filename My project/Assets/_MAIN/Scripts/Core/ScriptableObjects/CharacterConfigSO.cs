using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    [CreateAssetMenu(fileName = "Character Configuration Asset", menuName = "Dialogue System/Character Configuration Asset")] // fajl neve és a menü neve, ami alatt megtalálható a Unity-ban
    public class CharacterConfigSO : ScriptableObject
    {
        public CharacterConfigData[] characters; // karakterek konfigurációs adatait tartalmazza

        public CharacterConfigData GetConfig(string characterName) // a karakterhez tartozó adatokat adja meg - név alapján azonosit
        {
            characterName = characterName.ToLower();

            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfigData data = characters[i];

                if (string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
                {
                    return data.Copy(); // másolat készitése adatokról
                }
            }

            return CharacterConfigData.Default; // ha nincs találat (mellékszereplő), akkor az alapértelmezett adatokat tériti vissza
        }
    }
}