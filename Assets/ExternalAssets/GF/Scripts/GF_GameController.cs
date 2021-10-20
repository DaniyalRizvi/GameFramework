using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GF_GameController : MonoBehaviour {

	[Header ("Scene Selection")]
	public Scenes PreviousScene;
	public Scenes NextScene;

	[Header ("Main Player", order = 1)]
	public Player_Attributes[] Players;

	[Header ("Game Dialogues")]
	public Game_Dialogues Game_Elements;

	[Header ("SFX Objects")]
	public SFX_Objects SFX_Elements;

	[Header ("Level Information")]
	public int PlayableLevels = 6;
	public Level_Data[] Levels;
	[Header ("Gameover States")]
	public bool ReasonBased;
	[Tooltip ("Gameover information is optional. This will not appear if un-checked.")]
	public GameOver[] States;

	[Header ("Level End Delay")]
	public float GameWinDelay;
	public float GameLooseDelay;

	[Header ("Ad Sequence ID")]
	public int SequenceID;

	//Local Variables
	GameObject PlayerMain;
	GameObject AudioSource_Parent;
	GameObject FX_AudioSource;
	//Timer
	int minutes;
	int seconds;
	string time;
	private int currentLevel;
	private int currentPlayer;
	private int FinishCount = 0;
	private bool isTimerEnabled;
	private int Rewardamount = 0;
	[HideInInspector]
	public bool TimerPaused = false;

	#region debug

	[Header ("Debug Values")]
	[Range (1, 8)]
	public int StartLevel = 1;
	[Range (1, 2)]
	public int StartPlayer = 1;
	public int ObjectivesLeft = 0;
	public float LevelTime = 0.0f;

	#endregion

	void InitializeAudio (GameObject obj, string name){
		AudioSource_Parent = GameObject.Find ("SFXController");
		obj = new GameObject (name);
		obj.transform.position = AudioSource_Parent.transform.position;
		obj.transform.rotation = AudioSource_Parent.transform.rotation;
		obj.transform.parent = AudioSource_Parent.transform;
		obj.AddComponent<AudioSource> ();
		obj.GetComponent<AudioSource> ().priority = 128;
	}

	void Start () {

		//GameManager Variables Reset
		GameManager.Instance.GameStatus = null;
		Time.timeScale = 1;
		AudioListener.pause = false;

		#if UNITY_EDITOR
		if (GameManager.Instance.SessionStatus == 1) {
			currentLevel = GameManager.Instance.CurrentLevel;
			currentPlayer = GameManager.Instance.CurrentPlayer;
        } else {
			currentLevel = StartLevel;
			currentPlayer = StartPlayer;
			GameManager.Instance.CurrentLevel = currentLevel;
			GameManager.Instance.CurrentPlayer = currentPlayer;
        }
		#else
		currentLevel = GameManager.Instance.CurrentLevel;
		currentPlayer = GameManager.Instance.CurrentPlayer;
		#endif
		StartLevel = GameManager.Instance.CurrentLevel;
		StartPlayer = GameManager.Instance.CurrentPlayer;

		if (Levels [currentLevel - 1].isTimeBased) {
			isTimerEnabled = true;
			Game_Elements.Timer.SetActive (true);
        } else {
			isTimerEnabled = false;
			Game_Elements.Timer.SetActive (false);
        }

		if (!GameManager.Instance.Initialized) {
			InitializeGame ();
		}

		InitializeLevel ();

		//Initialize Audio Sources
		InitializeAudio (FX_AudioSource, "FX_AudioSource");
		FX_AudioSource = GameObject.Find ("FX_AudioSource");

		//SpawnPlayer();
		SpawnPlayers ();
		ActivateLevel ();
		if (Levels [currentLevel - 1].Objectives.Length > 0) {
			ActivateFinishPoint ();
        }

		//In-Game Timer
		if (isTimerEnabled)
			InvokeRepeating ("GameTimer", 0, 1);

		GF_AdsManager.HideBanner ();
    }

	void InitializeGame () {
		SaveData.Instance = new SaveData ();
		GF_SaveLoad.LoadProgress ();
		GameManager.Instance.Initialized = true;
	}

	void InitializeLevel () {

		Time.timeScale = 1;

		Game_Elements.LevelComplete.SetActive (false);
		Game_Elements.FinalComplete.SetActive (false);
		Game_Elements.LevelFailed.SetActive (false);
		Game_Elements.GameExit.SetActive (false);
		Game_Elements.LoadingScreen.SetActive (false);
		Game_Elements.PauseMenu.SetActive (false);
		Game_Elements.HelpScreen.SetActive (false);

		//Reset Players
		if (Players.Length > 0) {
			for (int i = 0; i < Players.Length; i++) {
				Players [i].PlayerObject.SetActive (false);
				Players [i].PlayerControls.alpha = 0;
				Players [i].PlayerControls.interactable = false;
				Players [i].PlayerControls.blocksRaycasts = false;
            }
        } else if (Players.Length == 0) {
			Debug.LogError ("No Players have been assigned in the inspector !");
        }

		//Reset Finish Points
		if (Levels [currentLevel - 1].Objectives.Length > 0) {
			for (int i = 0; i < Levels [currentLevel - 1].Objectives.Length; i++) {
				if (Levels [currentLevel - 1].Objectives [i].FinishPoint != null)
					Levels [currentLevel - 1].Objectives [i].FinishPoint.SetActive (false);
				if (Levels [currentLevel - 1].Objectives [i].Instruction == "")
					Debug.LogWarning ("Please write insctruction for Level->" + GameManager.Instance.CurrentLevel + " Objective->" + (i + 1) + " in the inspector !");
            }
        } else if (Levels [currentLevel - 1].Objectives.Length == 0) {
			Debug.LogError ("No Objectives have been defined in the inspector !");
        }

		//SpawnItems
		if (Levels [currentLevel - 1].Items.Length > 0){
			for (int i = 0; i < Levels [currentLevel - 1].Items.Length; i++){
				SetPlayerPosition (Levels [currentLevel - 1].Items [i].Item, Levels [currentLevel - 1].Items [i].SpawnPoint);
			}
		}

		if (Levels [currentLevel - 1].GiveReward) {
			if (Levels [currentLevel - 1].RewardLevels.Length == 0)
				Debug.LogError ("No Rewards have been defined in the inspector !");
		}

    }

	#region PlayerMain (Spawn / Controls)

	void SpawnPlayers (){
		if (Players.Length > 0 && Players.Length > currentPlayer - 1) {
			
			//Primary player Spawn
			if (Levels [currentLevel - 1].PrimaryPlayer.PlayerIndex >= 0 && Levels [currentLevel - 1].PrimaryPlayer.PlayerIndex < Players.Length){
				if (GameManager.Instance.SessionStatus == 1) {
					SetPlayerPosition (Players [currentPlayer - 1].PlayerObject, Levels [currentLevel - 1].PrimaryPlayer.SpawnPoint);
					SwitchPlayer ((currentPlayer - 1), true);
				} else{
					SetPlayerPosition (Players [Levels [currentLevel - 1].PrimaryPlayer.PlayerIndex].PlayerObject, Levels [currentLevel - 1].PrimaryPlayer.SpawnPoint);
					SwitchPlayer (Levels [currentLevel - 1].PrimaryPlayer.PlayerIndex, true);
				}

			} else{
				Debug.LogError ("Player at Index->" + (Levels [currentLevel - 1].PrimaryPlayer.PlayerIndex) + " has not been assigned in the inspector !");
			}

			//Secondary players Spawn
			if (Levels [currentLevel - 1].EnableSecondarySpawns){
				if (Levels [currentLevel - 1].SecondaryPlayers.Length > 0){
					for (int i = 0; i < Levels [currentLevel - 1].SecondaryPlayers.Length; i++){
						if (Levels [currentLevel - 1].SecondaryPlayers [i].PlayerIndex != Levels [currentLevel - 1].PrimaryPlayer.PlayerIndex){
							if (Levels [currentLevel - 1].SecondaryPlayers [i].PlayerIndex >= 0 && Levels [currentLevel - 1].SecondaryPlayers [i].PlayerIndex < Players.Length){
								SetPlayerPosition (Players [Levels [currentLevel - 1].SecondaryPlayers [i].PlayerIndex].PlayerObject, Levels [currentLevel - 1].SecondaryPlayers [i].SpawnPoint);
							} else{
								Debug.LogError ("Player at Index->" + (Levels [currentLevel - 1].SecondaryPlayers [i].PlayerIndex) + " has not been assigned in the inspector !");
							}
						} else{
							Debug.LogError ("Secondary player at Index->" + Levels [currentLevel - 1].SecondaryPlayers [i].PlayerIndex + " already specified as primary !");
						}
					}
				} else{
					Debug.LogWarning ("You have not specified any secondary players for Level->" + currentLevel);
				}
			}
		} else if (Players.Length <= currentPlayer - 1 && Players.Length != 0) {
			Debug.LogError ("Player->" + (currentPlayer) + " has not been assigned in the inspector !");
		}
	}

	void SetPlayerPosition (GameObject Player, Transform Position){
		Player.transform.position = Position.position;
		Player.transform.rotation = Position.rotation;
	}

	public void SwitchPlayer (int PlayerIndex, bool isActive){
		if (PlayerIndex > Players.Length - 1){
			Debug.LogError ("Player at index-> " + PlayerIndex + " does not exist !");
		} else{
			if (PlayerMain != null){
				DeactivatePlayer (currentPlayer, isActive);
			}
			currentPlayer = PlayerIndex;
			SwitchControls ();
			ActivatePlayer (currentPlayer);
		}

	}

	void ActivatePlayer (int PlayerIndex){
		PlayerMain = Players [PlayerIndex].PlayerObject;
		/*
        * IMPORTANT NOTE: ONLY USE (PlayerMain) FOR REFERENCE OF SPECIFIC PLAYER TO GET ALL TYPES OF COMPONENT
        * TURN ON EVERYTHING WHICH CAN CONTROL THIS PLAYER
        * FOR EXAMPLE:
        * RIGIDBODY, CHARACTER CONTROLLER
        * RCC CONTROLLER
        * ANY FPS CONTROLLER OR TP CONTROLLER
        * ASSIGN TARGET TO MINIMAP IF ANY
        */
		PlayerMain.SetActive (true);
	}

	void DeactivatePlayer (int PlayerIndex, bool isActive){
		/*
        * IMPORTANT NOTE: ONLY USE (PlayerMain) FOR REFERENCE OF SPECIFIC PLAYER TO GET ALL TYPES OF COMPONENT
        * TURN ON EVERYTHING WHICH CAN CONTROL THIS PLAYER
        * FOR EXAMPLE:
        * RIGIDBODY, CHARACTER CONTROLLER
        * RCC CONTROLLER
        * ANY FPS CONTROLLER OR TP CONTROLLER
        */
		if (isActive){
			PlayerMain.SetActive (false);
		}
	}

	void SwitchControls (){
		for (int i = 0; i < Players.Length; i++){
			if (i == currentPlayer){
				Players [i].PlayerObject.SetActive (true);
				Players [i].PlayerControls.alpha = 1;
				Players [i].PlayerControls.interactable = true;
				Players [i].PlayerControls.blocksRaycasts = true;
			} else{
				Players [i].PlayerControls.alpha = 0;
				Players [i].PlayerControls.interactable = false;
				Players [i].PlayerControls.blocksRaycasts = false;
			}
		}
	}

	#endregion

	void ActivateLevel () {
		for (int i = 0; i < Levels.Length; i++){
			if (i == currentLevel - 1){
				Levels [i].LevelObject.SetActive (true);
			} else{
				Destroy (Levels [i].PrimaryPlayer.SpawnPoint.gameObject);
				Destroy (Levels [i].LevelObject);
			}
		}

		GameManager.Instance.Objectives = Levels [currentLevel - 1].Objectives.Length;
		//For Debug
		ObjectivesLeft = GameManager.Instance.Objectives;

		LevelTime = (Levels [currentLevel - 1].Minutes * 60) + Levels [currentLevel - 1].Seconds;
    }

	void ActivateFinishPoint () {
		if (FinishCount == 0) {
			if (Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint.SetActive (true);
			ShowInstruction ();
		} else if (FinishCount == Levels [currentLevel - 1].Objectives.Length){
			if (Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint.SetActive (false);
		} else {
			if (Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount - 1].FinishPoint.SetActive (false);
			if (Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint != null)
				Levels [currentLevel - 1].Objectives [FinishCount].FinishPoint.SetActive (true);
			ShowInstruction ();
        }
    }

	public void ShowInstruction () {
		Game_Elements.InstructionText.text = Levels [currentLevel - 1].Objectives [FinishCount].Instruction;
		FinishCount++;
    }

	void GameTimer () {
		if (!TimerPaused){
			if (LevelTime >= 0.0f && GameManager.Instance.GameStatus != "Loose" && GameManager.Instance.Objectives > 0)
				LevelTime -= 1;
			minutes = ((int)LevelTime / 60);
			seconds = ((int)LevelTime % 60);
			time = minutes.ToString ("00") + ":" + seconds.ToString ("00");
			Game_Elements.Timer_txt.text = time;

			if (LevelTime <= 15.0f && LevelTime > 0.0f) {
				Game_Elements.Timer_txt.color = Color.red;
				SFX_Elements.CountDown.SetActive (true);
				if (GameManager.Instance.Objectives <= 0) {
					SFX_Elements.CountDown.SetActive (false);
            }
        } else if (LevelTime == 0.0f && GameManager.Instance.GameStatus != "Loose" && GameManager.Instance.Objectives > 0) {
				SFX_Elements.CountDown.SetActive (false);
				GameManager.Instance.GameLoose ();
        }
		}
    }

	//Dialogues Logic
	public void OnLevelCheck (int reasonIndex) {
		//For Debug
		ObjectivesLeft = GameManager.Instance.Objectives;

		if (GameManager.Instance.Objectives > 0 && GameManager.Instance.GameStatus != "Loose") {
			if (Levels [currentLevel - 1].Objectives.Length != 0)
				ActivateFinishPoint ();
			else
				Debug.LogWarning ("No Objectives have been defined in the inspector !");
        } else if (GameManager.Instance.Objectives == 0) {
			if (Levels [currentLevel - 1].Objectives.Length != 0)
				ActivateFinishPoint ();
			else
				Debug.LogWarning ("No Objectives have been defined in the inspector !");

			//Calculate Reward
			if (Levels [currentLevel - 1].GiveReward){
				GiveRewards ();
			}
			DisableAudio ();
			FX_AudioSource.GetComponent<AudioSource> ().PlayOneShot (SFX_Elements.LevelCompleteSFX);
			StartCoroutine (OnLevelStatus ());
        } else if (GameManager.Instance.GameStatus == "Loose") {
			DisableAudio ();
			if (ReasonBased)
				SetGameOverReason (reasonIndex);
			FX_AudioSource.GetComponent<AudioSource> ().PlayOneShot (SFX_Elements.LevelFailedSFX);
			StartCoroutine (OnLevelStatus ());
        }
    }

	void DisableAudio (){
		for (int i = 0; i < SFX_Elements.BGMusicLoops.Length; i++){
			SFX_Elements.BGMusicLoops [i].SetActive (false);
		}
	}

	void SetGameOverReason (int reasonIndex){
		if (States.Length > 0 && reasonIndex < States.Length){
			Game_Elements.ReasonObject.SetActive (true);
			Game_Elements.GameOverLogo.sprite = States [reasonIndex].Icon;
			Game_Elements.GameOverText.text = States [reasonIndex].Reason;
		} else{
			Debug.LogError ("Game over reason for index " + reasonIndex + " does not exist !");
		}
	}

	void GiveRewards () {
		if (Levels [currentLevel - 1].RewardLevels.Length > 0) {
			for (int i = 0; i < Levels [currentLevel - 1].RewardLevels.Length; i++) {
				//Give reward here
				CalculateRewardAmount (i);
            }
        } else {
			Debug.LogError ("No rewards have been defined in the inspector !");
        }
    }


	IEnumerator OnLevelStatus () {
		CancelInvoke ();
		GameManager.Instance.PauseTimer ();
		SFX_Elements.CountDown.SetActive (false);
		if (GameManager.Instance.GameStatus == "Loose") {
			yield return new WaitForSeconds (GameLooseDelay); 
			Game_Elements.LevelFailed.SetActive (true);
        } else {
			UpdateLevel ();
			yield return new WaitForSeconds (GameWinDelay); 
			if (currentLevel == PlayableLevels) {
				Game_Elements.FinalComplete.SetActive (true);
            } else {
				Game_Elements.LevelComplete.SetActive (true);
            }
        }
		yield return new WaitForSeconds (2.0f);
		Time.timeScale = 0;
		ShowAds (SequenceID);
    }

	int reward;
	float TimePerct;
	void CalculateRewardAmount (int index){
		TimePerct = (LevelTime / ((Levels [currentLevel - 1].Minutes * 60) + Levels [currentLevel - 1].Seconds)) * 100;
		if ((int)TimePerct >= Levels [currentLevel - 1].RewardLevels [index].MinTime && (int)TimePerct <= Levels [currentLevel - 1].RewardLevels [index].MaxTime) {
			for (int i = 0; i < Levels [currentLevel - 1].RewardLevels [index].RewardInfo.Length; i++) {
				reward = Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardAmount;
			
				//Give Your Rewards Here
				switch (Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardType){
				case RewardTypes.Coins:
					Debug.Log ("Reward # " + i + "-> " + reward + " " + Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardType);
					break;
				case RewardTypes.Other:
					Debug.Log ("Reward # " + i + "-> " + reward + " " + Levels [currentLevel - 1].RewardLevels [index].RewardInfo [i].RewardType);
					break;
				}
			}
		}
	}

	void UpdateLevel () {
		if (currentLevel == SaveData.Instance.Level) {
			SaveData.Instance.Level++;
			GF_SaveLoad.SaveProgress ();
        }
    }

	void ShowAds (int id) {
		GF_AdsManager.ShowAdvertisement (id, "Game Play - Level : " + currentLevel);
    }

	public void PauseGame () {
		ShowAds (SequenceID);
		Time.timeScale = 0.0f;
		AudioListener.pause = true;
    }

	public void ResumeGame () {
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
    }

	public void RetryLevel () {
		Game_Elements.LoadingScreen.SetActive (true);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
    }

	public void NextLevel () {
		if (currentLevel != PlayableLevels) {
			#if UNITY_EDITOR
			GameManager.Instance.SessionStatus = 1;
			#endif
			GameManager.Instance.CurrentLevel += 1;
			Game_Elements.LoadingScreen.SetActive (true);
			SceneManager.LoadScene (NextScene.ToString ());
        }
    }

	public void MainMenu () {
		Game_Elements.LoadingScreen.SetActive (true);
		SceneManager.LoadScene (PreviousScene.ToString ());
    }
}