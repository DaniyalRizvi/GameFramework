using UnityEditor;

[CustomEditor(typeof(GF_OfferWall))]
public class GF_OfferWallEditor : Editor {

	string module = "Offer Wall";

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
