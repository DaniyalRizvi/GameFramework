using UnityEditor;

[CustomEditor(typeof(GF_LevelSelection))]
public class GF_LevelSelectionEditor : Editor {

    string module = "Level Selection";

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
