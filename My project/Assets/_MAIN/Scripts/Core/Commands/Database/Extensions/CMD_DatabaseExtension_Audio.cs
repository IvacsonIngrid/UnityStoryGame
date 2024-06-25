using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Audio : CMD_DatabaseExtension
    {
        // lehetséges paraméterek elődefiniálása
        private static string[] PARAM_SFX = new string[] { "-s", "-sfx"};
        private static string[] PARAM_VOLUME = new string[] { "-v", "-vol", "-volume"};
        private static string[] PARAM_PITCH = new string[] { "-p", "-pitch"};
        private static string[] PARAM_LOOP = new string[] { "-l", "-loop"};

        private static string[] PARAM_CHANNEL = new string[] { "-c", "-channel"};
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate"};
        private static string[] PARAM_START_VOLME = new string[] { "-sv","-startvolume"};
        private static string[] PARAM_SONG = new string[] { "-s", "-song" };
        private static string[] PARAM_AMBIENCE = new string[] { "-a", "-ambience" };

        // absztrakt osztály metódusának implementálása - a forgatókönyvben használható parancsok definiálása, a parancshoz tartozó metódus végrehajtása
        new public static void Extend(CommandDatabase database) // minden a létrehozott adatbázisban tárolva
        {
            database.AddCommand("playsfx", new Action<string[]>(PlaySFX));
            database.AddCommand("stopsfx", new Action<string>(StopSFX));

            database.AddCommand("playvoice", new Action<string[]>(PlayVoice));
            database.AddCommand("stopvoice", new Action<string>(StopSFX));

            database.AddCommand("playsong", new Action<string[]>(PlaySong));
            database.AddCommand("stopsong", new Action<string>(StopSong));

            database.AddCommand("playambience", new Action<string[]>(PlayAmbience));
            database.AddCommand("stopambience", new Action<string>(StopAmbience));
        }

        private static void PlaySFX(string[] data)
        {
            string filePath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            // paraméterek lekérése
            parameters.TryGetValue(PARAM_SFX, out filePath);
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            // betölti az AudioClip elemet
            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_sfx, filePath));

            if (sound == null)
                return;

            // lejátszás végrehajtása
            AudioManager.instance.PlaySoundEffect(sound, volume: volume, pitch: pitch, loop: loop);
        }

        // lejátszás megállitása
        private static void StopSFX(string data)
        {
            AudioManager.instance.StopSoundEffect(data);
        }

        private static void PlayVoice(string[] data)
        {
            string filePath;
            float volume, pitch;
            bool loop;

            var parameters = ConvertDataToParameters(data);

            // paraméterek lekérése
            parameters.TryGetValue(PARAM_SFX, out filePath);
            parameters.TryGetValue(PARAM_VOLUME, out volume, defaultValue: 1f);
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: false);

            //AudioClim elem létrehozása a hanghoz
            AudioClip sound = Resources.Load<AudioClip>(FilePaths.GetPathToResource(FilePaths.resources_voices, filePath));

            if (sound == null)
            {
                Debug.Log($"Was not able to load voice '{filePath}'");
                return;
            }

            // lejátszás
            AudioManager.instance.PlayVoice(sound, volume: volume, pitch: pitch, loop: loop);
        }

        private static void PlaySong(string[] data)
        {
            string filePath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // paraméter lekérése - útvonal elérése
            parameters.TryGetValue(PARAM_SONG, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_music, filePath);

            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 1);

            // lejátszás kérése
            PlayTrack(filePath, channel, parameters);
        }

        private static void PlayAmbience(string[] data)
        {
            string filePath;
            int channel;

            var parameters = ConvertDataToParameters(data);

            // paraméter lekérése - útvonal elérése
            parameters.TryGetValue(PARAM_AMBIENCE, out filePath);
            filePath = FilePaths.GetPathToResource(FilePaths.resources_ambience, filePath);

            parameters.TryGetValue(PARAM_CHANNEL, out channel, defaultValue: 0);

            // lejátszás kérése
            PlayTrack(filePath, channel, parameters);
        }

        private static void PlayTrack(string filePath, int channel, CommandParameters parameters)
        {
            bool loop;
            bool immediate;
            float volumeCap;
            float startVolume;
            float pitch;

            // paraméter lekérése
            parameters.TryGetValue(PARAM_VOLUME, out volumeCap, defaultValue: 1f);
            parameters.TryGetValue(PARAM_START_VOLME, out startVolume, defaultValue: 0f);
            parameters.TryGetValue(PARAM_PITCH, out pitch, defaultValue: 1f);
            parameters.TryGetValue(PARAM_LOOP, out loop, defaultValue: true);

            // AudioClip létrehozása az útvonallal azonositott hanganyaghoz
            AudioClip sound = Resources.Load<AudioClip>(filePath);

            if (sound == null)
            {
                Debug.Log($"Was not able to load voice '{filePath}'");
                return;
            }

            // lejátszás
            AudioManager.instance.PlayTrack(sound, channel, loop, startVolume, volumeCap, pitch, filePath);
        }


        // leállitáshoz használt metódusok, aár csatorna leállitás is
        private static void StopSong(string data)
        {
            if (data == string.Empty)
                StopTrack("1");
            else
                StopTrack(data);
        }

        private static void StopAmbience(string data)
        {
            if (data == string.Empty)
                StopTrack("0");
            else
                StopTrack(data);
        }

        private static void StopTrack(string data)
        {
            if (int.TryParse(data, out int channel))
                AudioManager.instance.StopTrack(channel);
            else
                AudioManager.instance.StopTrack(data);
        }
    }
}

