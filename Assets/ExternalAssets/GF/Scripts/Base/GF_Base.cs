using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Scenes {
    SplashScreen,
    MainMenu,
    LevelSelection,
    PlayerSelection,
    GamePlay
}

public enum RewardTypes {
    Coins,
    Other
}

[System.Serializable]
public class Game_Dialogues {
    public GameObject LevelComplete;
    public GameObject FinalComplete;
    public GameObject LevelFailed;
	public GameObject ReasonObject;
	public Image GameOverLogo;
	public Text GameOverText;
    public GameObject GameExit;
    public GameObject PauseMenu;
    public GameObject HelpScreen;
    public GameObject LoadingScreen;
    public GameObject Timer;
    public Text Timer_txt;
    public Text InstructionText;
}
	
[System.Serializable]
public class PlayerSpawn{
	public int PlayerIndex;
	public Transform SpawnPoint;
}

[System.Serializable]
public class ItemSpawner{
	public GameObject Item;
	public Transform SpawnPoint;
}

[System.Serializable]
public class Level_Data {
    public GameObject LevelObject;
	[Header("Player Spawn")]
	public PlayerSpawn PrimaryPlayer;
	public bool EnableSecondarySpawns;
	public PlayerSpawn[] SecondaryPlayers;
	[Header("Items Spawn")]
	public ItemSpawner[] Items;
	[Header("Tasks")]
	public Objectives_Info[] Objectives;
    [Tooltip("Level Time will not be considered if this field is unchecked.")]
    [Header("Level Time")]
    public bool isTimeBased;
    [Range(0, 60)]
    public int Minutes;
    [Range(10, 60)]
    public int Seconds;

	[Header("Level Reward")]
	public bool GiveReward;
	public Reward_Data[] RewardLevels;
}

[System.Serializable]
public class Player_Attributes{
	public GameObject PlayerObject;
	public CanvasGroup PlayerControls;
}


[System.Serializable]
public class Reward_Data {
	[Header("Time Range")]
    [Range(1, 100)]
    public int MinTime;
	[Range(1, 100)]
	public int MaxTime;
    [Header("Reward Type")]
    public Reward_Info[] RewardInfo;
}

[System.Serializable]
public class Reward_Info {
    public RewardTypes RewardType;
    public int RewardAmount;
}

[System.Serializable]
public class Objectives_Info {
	[Tooltip("FinishPoint field is optional. This object will be deactivated upon objective completion.")]
	public GameObject FinishPoint;
	[Multiline]
	public string Instruction;
}

[System.Serializable]
public class SFX_Objects {
	public GameObject[] BGMusicLoops;
    public AudioClip LevelCompleteSFX;
    public AudioClip LevelFailedSFX;
    public GameObject CountDown;
}

[System.Serializable]
public class GameOver{
	public Sprite Icon;
	[Multiline]
	public string Reason;
}