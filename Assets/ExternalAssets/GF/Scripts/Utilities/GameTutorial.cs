using UnityEngine;
using System.Collections;

public class GameTutorial : MonoBehaviour {
	[Header("Instructions")]
	public GameObject[] Instruction;
	[Header("Main Instruction Panel")]
	public GameObject MsgPanel;

	bool nextMsg;
	int x = 0;
	
	void Start(){
		MsgPanel.SetActive (true);
		Instruction [0].SetActive (true);
		nextMsg = true;
		x++;
	}
	
	public void ShowInstruction(int count){
		if (nextMsg && x == count) {
			x++;
			Time.timeScale = 1;
			MsgPanel.SetActive (false);
			Instruction [count-1].SetActive (false);
			nextMsg = false;
			StartCoroutine (ShowNextMsg(count));
		}
	}
	
	IEnumerator ShowNextMsg(int x){
		yield return  new WaitForSeconds(3.0f);
		if (x != Instruction.Length) {
			MsgPanel.SetActive (true);
			Instruction [x].SetActive (true);
			nextMsg = true;
		} else if(x == Instruction.Length) {
            GameManager.Instance.TaskComplete();
		}
	}
}