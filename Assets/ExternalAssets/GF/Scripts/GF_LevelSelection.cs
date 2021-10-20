using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class GF_LevelSelection : MonoBehaviour {

    [Header("Scene Selection")]
    public Scenes PreviousScene;
    public Scenes NextScene;

	[Header("Settings")]
	public bool Locked;
	public int PlayableLevels = 6;

    [Header("UI Panels")]
    public GameObject LoadingScreen;
	public GameObject LevelsPanel;
    public Slider FillBar;

    [Header("Audio Settings")]
	public AudioSource ButtonClick;

    [Header("Ad Sequence ID")]
    public int SequenceID;
	public bool LoadingSequence;
	public int LoadingSequenceID;

	private List<Button> LevelButtons = new List<Button>();
    AsyncOperation async = null;

    void Start () {
        FillBar.value = 0;
		Time.timeScale = 1;
		AudioListener.pause = false;
        LoadingScreen.SetActive(false);
		if (!GameManager.Instance.Initialized) {
			InitializeGame();
		}
		CacheButtons ();
		LevelsInit();
		ShowAds(SequenceID, "Level Selection");
    }

	void InitializeGame() {
		SaveData.Instance = new SaveData();
		GF_SaveLoad.LoadProgress();
		GameManager.Instance.Initialized = true;
	}

	void ShowAds(int id, string SceneName) {
		GF_AdsManager.ShowAdvertisement (id, SceneName);
    }

    void Update(){
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(PreviousScene.ToString());
            }
        }

        if (async != null)
        {
            FillBar.value = async.progress;
            if (async.progress >= 0.9f)
            {
                FillBar.value = 1.0f;
            }
        }

    }

	void CacheButtons(){
		Button[] levelButtons = LevelsPanel.transform.GetComponentsInChildren <Button> ();
		for (int i = 0; i < levelButtons.Length; i++){
			LevelButtons.Add (levelButtons[i]);		
		}
		LevelButtons = LevelButtons.OrderBy (x => Int32.Parse (x.gameObject.name)).ToList ();
		for (int i = 0; i < LevelButtons.Count; i++){
			int LevelIndex = i + 1;
			LevelButtons[i].onClick.AddListener(() => PlayLevel(LevelIndex));
			LevelButtons[i].onClick.AddListener(() => ButtonClick.Play ());
		}
	}

	void LevelsInit(){
		if (!Locked){
			for (int i = 0; i < LevelButtons.Count; i++){
				if (i < PlayableLevels)
					LevelButtons [i].interactable = true;
				else
					LevelButtons [i].interactable = false;
			}
		} 
		else{
			for (int i = 0; i < LevelButtons.Count; i++){
				LevelButtons[i].interactable = false;
			}

			for (int i = 0; i < LevelButtons.Count; i++){
				if (i < SaveData.Instance.Level && i < PlayableLevels){
					LevelButtons[i].interactable = true;
				}
			}
		}
	}

    public void PlayLevel(int level){
        GameManager.Instance.CurrentLevel = level;
        GameManager.Instance.SessionStatus = 1;
		LoadingScreen.SetActive(true);
        StartCoroutine(LevelStart());
    }

    IEnumerator LevelStart(){
		if (LoadingSequence){
			ShowAds(LoadingSequenceID, "Loading Screen");
			yield return new WaitForSeconds (3);
		}
        async = SceneManager.LoadSceneAsync(NextScene.ToString());
        yield return async;
    }

    public void BackBtn(){
        SceneManager.LoadScene(PreviousScene.ToString());
    }
}
