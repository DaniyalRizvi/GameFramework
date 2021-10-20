using UnityEditor;

[CustomEditor(typeof(GF_SplashScreen))]
public class GF_SplashScreenEditor : Editor {

	string module = "Splash Screen";

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
