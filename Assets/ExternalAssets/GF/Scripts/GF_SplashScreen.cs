using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GF_SplashScreen : MonoBehaviour {

	[Header("Scene Selection")]
	public Scenes NextScene;

	[Header("Scene Settings")]
	public float WaitTime;

	void Start () {

		Time.timeScale = 1;
		AudioListener.pause = false;

		if (!GameManager.Instance.Initialized) {
			InitializeGame();
		}

		StartCoroutine (StartGame ());
	}

	void InitializeGame() {
		SaveData.Instance = new SaveData();
		GF_SaveLoad.LoadProgress();
		GameManager.Instance.Initialized = true;
	}

	IEnumerator StartGame(){
		yield return new WaitForSeconds (WaitTime);
		SceneManager.LoadScene(NextScene.ToString());
	}
}
