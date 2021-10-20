using UnityEditor;

[CustomEditor(typeof(GF_PlayerSelection))]
public class GF_PlayerSelectionEditor : Editor{

    string module = "Player Selection";

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
