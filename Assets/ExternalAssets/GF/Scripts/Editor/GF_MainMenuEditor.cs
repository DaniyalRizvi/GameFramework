using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GF_MainMenu))]
public class GF_MainMenuEditor : Editor {

    string module = "Main Menu";

    void Awake() {
        GF_Editor.GetLogo();
    }

    public override void OnInspectorGUI() {

        GF_Editor.DefineGUIStyle(module);

        EditorGUILayout.BeginVertical("box");
        DrawDefaultInspector();
        EditorGUILayout.EndHorizontal();
    }
}
