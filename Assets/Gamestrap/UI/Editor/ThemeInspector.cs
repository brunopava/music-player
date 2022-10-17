using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Gamestrap
{
    [CustomEditor(typeof(GamestrapTheme))]
    [CanEditMultipleObjects]
    public class ThemeInspector : Editor
    {
        private static string websiteURL = "http://www.gamestrap.info/#!/ui";
        private static string contactURL = "http://www.gamestrap.info/#!/contact";
        private static string threadURL = "https://forum.unity3d.com/threads/ui-gamestrap-icons-shapes-effects-and-tools.291090/";
        private static string storeURL = "https://www.assetstore.unity3d.com/en/#!/content/28599";
        private static string rateURL = "https://www.assetstore.unity3d.com/#!/account/downloads/search=gamestrap";

        private GUIContent website = new GUIContent("Documentation", websiteURL);
        private GUIContent contact = new GUIContent("Contact Us", contactURL);
        private GUIContent thread = new GUIContent("Feedback", threadURL);
        private GUIContent rate = new GUIContent("Rate Us", rateURL);
        private GUIContent store = new GUIContent("Write a Review", storeURL);

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Gamestrap UI Window", GUILayout.Height(50))) {
                ThemeWindow.ShowWindow().Theme = (GamestrapTheme) target;
            }
            GUILayout.Space(5f);
            EditorGUILayout.HelpBox("The following buttons will open a tab in your browser", MessageType.None);
            if (GUILayout.Button(website)) {
                Application.OpenURL(websiteURL);
            }
            if (GUILayout.Button(thread)) {
                Application.OpenURL(threadURL);
            }
            if (GUILayout.Button(contact)) {
                Application.OpenURL(contactURL);
            }
            EditorGUILayout.HelpBox("If you are having issues, contact us first.", MessageType.Info);
            if (GUILayout.Button(rate)) {
                Application.OpenURL(rateURL);
            }
            if (GUILayout.Button(store)) {
                Application.OpenURL(storeURL);
            }
        }


    }
}