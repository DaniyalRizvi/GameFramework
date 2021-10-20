using UnityEditor;

[CustomEditor(typeof(GF_GameController))]
public class GF_GameControllerEditor : Editor {

    string module = "Game Controller";

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
