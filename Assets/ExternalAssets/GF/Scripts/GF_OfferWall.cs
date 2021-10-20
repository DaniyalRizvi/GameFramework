using UnityEngine;
using System.Collections;

public class GF_OfferWall : MonoBehaviour {

	[Header("Offer Wall Settings")]
	[Space]
	public bool EnabeOfferWall = false;

	[Header("UI Panels")]
	public GameObject OfferWall;

	[Header("Ad Sequence ID")]
	public int SequenceID;

	void OnEnable(){
		//ConsoliAds.onRewardedVideoAdCompletedEvent += RewardedVideoCompleted;
	}

	void OnDisable() {
		//ConsoliAds.onRewardedVideoAdCompletedEvent -= RewardedVideoCompleted;
	}
		
	void Start () {
		if (OfferWall){
			OfferWall.SetActive (false);
			if(!GameManager.Instance.SessionAd)
				StartCoroutine (CheckRewardedVideo ());
		} else{
			OfferWall.SetActive (false);
		}

	}

	public void ShowRewardedVideo(){
		GF_AdsManager.ShowAdvertisement (SequenceID, "Rewarded Video");
	}

	void RewardedVideoCompleted() {
		GameManager.Instance.SessionAd = true;
		GF_AdsManager.HideBanner ();
		OfferWall.SetActive (false);
	}

	IEnumerator CheckRewardedVideo(){
		yield return new WaitForSeconds (2.0f);
//		if (ConsoliAds.Instance.IsAdAvailable (3)){
//			OfferWall.SetActive (true);
//		} else{
//			Debug.Log ("Rewarded Video Not Available !");
//		}
	}
}
