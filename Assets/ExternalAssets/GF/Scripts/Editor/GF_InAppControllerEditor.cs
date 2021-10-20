using UnityEditor;

[CustomEditor(typeof(GF_InAppController))]
public class GF_InAppControllerEditor : Editor{

    string module = "InApp Controller";

    void Awake() {        GF_Editor.GetLogo();    }

    public override void OnInspectorGUI() {        GF_Editor.DefineGUIStyle(module);
    }
}