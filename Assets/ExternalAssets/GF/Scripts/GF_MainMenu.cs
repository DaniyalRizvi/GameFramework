using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GF_MainMenu : MonoBehaviour{

    [Header("Scene Selection")]
    public Scenes NextScene;

    [Header("UI Panels")]
    public GameObject HelpScreen;
    public GameObject ExitDialogue;

    [Header("Ad Sequence ID")]
    public int SequenceID;

	void Awake(){
		ShowAds(SequenceID);
	}

    void Start(){

        Time.timeScale = 1;
        AudioListener.pause = false;

        if (!GameManager.Instance.Initialized) {
            InitializeGame();
        }

        InitializeUI();
    }

    void InitializeGame() {
        SaveData.Instance = new SaveData();
		GF_SaveLoad.LoadProgress();
        GameManager.Instance.Initialized = true;
    }

    void ShowAds(int id) {
		GF_AdsManager.ShowAdvertisement (id, "Main Menu");
		GF_AdsManager.ShowBanner ();
    }
		
    void Update(){
        if (Application.platform == RuntimePlatform.Android){
            if (Input.GetKey(KeyCode.Escape))
            {
                ExitDialogue.SetActive(true);
            }
        }
    }

    void InitializeUI() {
        HelpScreen.SetActive(false);
        ExitDialogue.SetActive(false);
    }

    public void PlayBtn(){
        SceneManager.LoadScene(NextScene.ToString());
    }

    public void RemoveAds() {
        GF_InAppController.Instance.BuyInAppProduct(0);
    }

    public void RestorePurchases() {
        GF_InAppController.Instance.RestorePurchases();
    }

    public void ShowRateUs(){
        NPBinding.Utility.RateMyApp.AskForReviewNow();
    }

    public void Exit(){
        Application.Quit();
    }

    public void ResetSaveData() {
        SaveData.Instance = null;
		GF_SaveLoad.DeleteProgress();
        SaveData.Instance = new SaveData();
		GF_SaveLoad.LoadProgress();
    }

    public void MoreFunBtn(){
//		Application.OpenURL(ConsoliAds.Instance.MoreFunURL());
    }
}
