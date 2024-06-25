using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_GraphicPanel : CMD_DatabaseExtension
    {
        private static string[] PARAM_PANEL = new string[] { "-p", "-panel" };
        private static string[] PARAM_LAYER = new string[] { "-l", "-layer" };
        private static string[] PARAM_MEDIA = new string[] { "-m", "-media" };
        private static string[] PARAM_GRAPHIC = new string[] { "-g", "-graphic" };
        private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_BLENDTEX = new string[] { "-b", "-blend" };
        private static string[] PARAM_USEVIDEOAUDIO = new string[] { "-aud", "-audio" };
        
        private const string HOME_DIRECTORY_SYMBOL = "~/";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setlayermedia", new Func<string[], IEnumerator>(SetLayerMedia));
            database.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
        }

        private static IEnumerator ClearLayerMedia(string[] data)
        {
            //param�terek
            string panelName = "";
            int layer = 0;
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";

            Texture blendTex = null;

            // parancshoz tartoz� param�terek bet�lt�se
            var parameters = ConvertDataToParameters(data);

            // panel lek�r�se n�v alapj�n
            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);

            if (panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is net a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            // r�teg lek�r�se
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: -1);

            // azonnali kell-e legyen az �tmenet
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            // ha nem, akkor milyen a sebess�g
            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);

            // �tmeneti text�ra lek�r�se
            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            if (layer == -1)
                panel.Clear(transitionSpeed, blendTex, immediate); // panel tisztit�sa
            else
            {
                GraphicLayer graphicLayer = panel.GetLayer(layer);

                if (graphicLayer == null)
                {
                    Debug.LogError($"Could not clear layer [{layer}] on panel '{panel.panelName}'");
                    yield break;
                }

                graphicLayer.Clear(transitionSpeed, blendTex, immediate); // r�teg tisztit�sa
            }
        }

        // r�teg be�llit�sa
        private static IEnumerator SetLayerMedia(string[] data)
        {
            //param�terek
            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0;
            bool immediate = false;
            string blendTexName = "";
            bool useAudio = false;

            string pathToGraphic = "";
            UnityEngine.Object graphic = null;
            Texture blendTex = null;

            //parancshoz tartoz� param�terek el�r�se
            var parameters = ConvertDataToParameters(data);

            // panel lek�r�se n�v alapj�n
            parameters.TryGetValue(PARAM_PANEL, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);

            if (panel == null)
            {
                Debug.LogError($"Unable to grab panel '{panelName}' because it is net a valid panel. Please check the panel name and adjust the command.");
                yield break;
            }

            // r�teg lek�rdez�se
            parameters.TryGetValue(PARAM_LAYER, out layer, defaultValue: 0);

            // m�dia elem lek�rdez�se
            parameters.TryGetValue(PARAM_MEDIA, out mediaName);

            // azonnali kell-e legyen a v�lt�s
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            // ha nem, akkor a sebess�g lek�r�se
            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out transitionSpeed, defaultValue: 1);

            // az �tmenet text�r�ja
            parameters.TryGetValue(PARAM_BLENDTEX, out blendTexName);

            // haszn�l-e videot
            parameters.TryGetValue(PARAM_USEVIDEOAUDIO, out useAudio, defaultValue: false);

            // grafikus elem el�r�si �tvonala, bt�lt�se
            pathToGraphic = GetPathToGraphic(FilePaths.resources_backgroundImages, mediaName);
            graphic = Resources.Load<Texture>(pathToGraphic);

            if (graphic == null)
            {
                pathToGraphic = GetPathToGraphic(FilePaths.resources_backgroundVideos, mediaName);
                graphic = Resources.Load<VideoClip>(pathToGraphic);
            }

            if (graphic == null)
            {
                Debug.LogError($"Could not find media file called '{mediaName}' in Resources directory. Make sure thet the file exists and specify the full path");
                yield break;
            }

            if (!immediate && blendTexName != string.Empty)
                blendTex = Resources.Load<Texture>(FilePaths.resources_blendTextures + blendTexName);

            // r�teg lek�rdez�se, vagy l�trehoz�sa
            GraphicLayer graphicLayer = panel.GetLayer(layer, createIfDoesNotExist: true);

            if (graphic is Texture)
            {
                yield return graphicLayer.SetTexture(graphic as Texture, transitionSpeed, blendTex, pathToGraphic, immediate);  // text�ra be�llit�s
            }
            else
            {
                yield return graphicLayer.SetVideo(graphic as VideoClip, transitionSpeed, useAudio, blendTex, pathToGraphic, immediate); // vide� be�llit�s
            }
        }

        private static string GetPathToGraphic(string defaultPath, string graphicName)
        {
            if (graphicName.StartsWith(HOME_DIRECTORY_SYMBOL))
                return graphicName.Substring(HOME_DIRECTORY_SYMBOL.Length);

            return defaultPath + graphicName;
        }
    }
}
