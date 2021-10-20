using UnityEngine;
using System.Collections;

public class GFDemo : MonoBehaviour {

    public void TaskComplete(){
        GameManager.Instance.TaskComplete();
    }

	public void GameLoose(int index=0){
        GameManager.Instance.GameLoose(index);
    }

	public void PauseTimer(){
		GameManager.Instance.PauseTimer ();
	}

	public void ResumeTimer(){
		GameManager.Instance.ResumeTimer ();
	}

	public void SwitchPlayer(int index){
		GameManager.Instance.SwitchPlayer (index,true);
	}

}
