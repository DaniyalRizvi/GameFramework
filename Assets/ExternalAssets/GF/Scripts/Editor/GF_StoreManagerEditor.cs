using UnityEditor;

[CustomEditor(typeof(GF_StoreManager))]
public class GF_StoreManagerEditor : Editor {

    string module = "Store Manager";

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
