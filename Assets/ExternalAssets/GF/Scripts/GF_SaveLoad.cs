using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class GF_SaveLoad {

	public static void SaveProgress(){
		SaveData.Instance.hashOfSaveData = HashGenerator(SaveObjectJSON());
		string saveDataHashed = JsonUtility.ToJson (SaveData.Instance, true);
		File.WriteAllText (GetSavePath (), saveDataHashed);
	}

	public static SaveData SaveObjectCreator(){
		SaveData CheckSave = new SaveData (SaveData.Instance.RemoveAds, SaveData.Instance.Level, SaveData.Instance.Coins);
		return CheckSave;
	}

	public static string SaveObjectJSON(){
		string saveDataString = JsonUtility.ToJson (SaveObjectCreator(), true);
		return saveDataString;
	}

	public static void LoadProgress(){
		if (File.Exists (GetSavePath ())) {
			string fileContent = File.ReadAllText (GetSavePath());
			JsonUtility.FromJsonOverwrite (fileContent, SaveData.Instance);

			#if !UNITY_EDITOR
			//File tampering checks
			if ((HashGenerator (SaveObjectJSON()) != SaveData.Instance.hashOfSaveData)) {
				SaveData.Instance = null;
				SaveData.Instance = new SaveData();
				DeleteProgress ();
				SaveProgress ();
				Debug.LogWarning ("Save file modification detected, Resetting your progress !");
			}
			#endif

			Debug.Log ("Game Load Successful --> "+GetSavePath ());
		} else {
			Debug.Log ("New Game Creation Successful --> "+GetSavePath ());
			SaveProgress ();
		}
	}

	public static string HashGenerator(string saveContent){
		SHA256Managed crypt = new SHA256Managed ();
		string hash = string.Empty;
		byte[] crypto = crypt.ComputeHash (Encoding.UTF8.GetBytes(saveContent), 0, Encoding.UTF8.GetByteCount(saveContent));
		foreach(byte bit in crypto){
			hash += bit.ToString ("x2");
		}
		return hash;
	}

	public static void DeleteProgress(){
		if (File.Exists (GetSavePath ())) {
			File.Delete (GetSavePath());
		}
	}

	private static string GetSavePath(){
		return Path.Combine(Application.persistentDataPath,"SavedGame.json");
	}
}
