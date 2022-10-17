using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamestrap
{
    public class GraphicsLoader : AssetPostprocessor
    {

        private static Dictionary<string, Texture2D> gamestrapTextures;

        private static void LoadGraphics()
        {
            string[] assets = AssetDatabase.FindAssets("t:Texture gamestrap_");

            if (assets.Length == 0) {
                Debug.LogError("Couldn't find any Gamestrap Graphics, make sure you have all of the graphics imported under the Editor Folders");
                return;
            }

            gamestrapTextures = new Dictionary<string, Texture2D>();
            foreach (string guid in assets) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                string key = texture.name.Replace("gamestrap_", "");
                if (!gamestrapTextures.ContainsKey(key))
                    gamestrapTextures.Add(key, texture);
            }
        }

        public static Texture2D Get(string assetName)
        {
            if (gamestrapTextures == null) {
                LoadGraphics();
            }

            if (gamestrapTextures == null)
            {
                Debug.LogError("Couldn't find any Gamestrap textures");
                return null;
            }
            Texture2D graphic = null;
            if (!gamestrapTextures.TryGetValue(assetName, out graphic))
            {
                LoadGraphics();
                if (gamestrapTextures.TryGetValue(assetName, out graphic))
                {
                    Debug.LogWarning(string.Format("Graphic {0} not found in project", assetName));
                }
            }

            return graphic;
        }

        public static GUISkin GetSkin(string skinName)
        {
            string[] assets = AssetDatabase.FindAssets("t: GUISkin gamestrap_");

            if (assets.Length == 0) {
                Debug.LogError("Couldn't find any Gamestrap Skins, make sure you have all of the graphics imported under the Editor Folders");
                return null;
            }
            foreach (string guid in assets) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GUISkin skin = (GUISkin)AssetDatabase.LoadAssetAtPath(path, typeof(GUISkin));
                string key = skin.name.Replace("gamestrap_", "");
                if (key == skinName)
                    return skin;

            }
            Debug.LogError("Couldn't find the specific Gamestrap Skin, make sure you haven't removed it from the project");
            return null;
        }


        void OnPostprocessTexture(Texture2D texture)
        {
            if (gamestrapTextures == null) {
                LoadGraphics();
            }
            if (assetPath.IndexOf("gamestrap_") == -1)
                return;

            string key = texture.name.Replace("gamestrap_", "");
            if (gamestrapTextures.ContainsKey(key))
                gamestrapTextures.Remove(key);

            gamestrapTextures.Add(key, texture);
        }
    }
}