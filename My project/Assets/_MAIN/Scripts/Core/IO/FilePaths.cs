﻿using UnityEngine;

public class FilePaths
{
    // gyökér mappa megadása
    public static readonly string root = $"{Application.dataPath}/gameData/";
    //public static readonly string root = $"{Application.dataPath}/_MAIN/Resources";

    public const string HOME_DIRECTORY_SYMBOL = "~/";

    //Resources paths - background
    public static readonly string resources_graphics = "Graphics/";
    public static readonly string resources_backgroundImages = $"{resources_graphics}BG Images/";
    public static readonly string resources_backgroundVideos = $"{resources_graphics}BG Videos/";
    public static readonly string resources_blendTextures = $"{resources_graphics}Transition Effects/";

    //Resources paths - audio
    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_voices = $"{resources_audio}Voices/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambience = $"{resources_audio}Ambience/";

    //Resource path - chapters
    public static readonly string resource_dialogueFile = $"Dialogue Files/";
    
    //Resource path - text
    public static readonly string resource_font = "Font/";

    // elérési útvonal meghatározása annak függvényében, hogy a gyökérkönyvtártól érkezik vagy csak erőforrás neve kell
    public static string GetPathToResource(string defaultPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);

        return defaultPath + resourceName;
    }
}
