using UnityEngine;
using System.Collections;

public class GF_AdsManager {

	public static void ShowAdvertisement(int sequenceID, string SceneName){
//		if (ConsoliAds.Instance != null){
//			ConsoliAds.Instance.LogScreen (SceneName);
//			if (!GameManager.Instance.SessionAd){
//				if (!SaveData.Instance.RemoveAds){
//					Debug.Log ("Show Ad Called ----------> Sequence ID : " + sequenceID + " | Scene : " + SceneName);
//					ConsoliAds.Instance.ShowAd (sequenceID);
//				}
//			}
//		} else{
//			Debug.LogWarning("Consoli Ads Instance Not Found !");
//		}
	}

	public static void ShowBanner(){
//		if (ConsoliAds.Instance != null){
//			if (!GameManager.Instance.SessionAd){
//				if (!SaveData.Instance.RemoveAds){
//					Debug.Log ("Show Banner Called ---------->");
//					ConsoliAds.Instance.ShowBanner ();
//				}
//			}
//		}else{
//			Debug.LogWarning("Consoli Ads Instance Not Found !");
//		}
	}

	public static void HideBanner(){
//		if (ConsoliAds.Instance != null){
//			Debug.Log ("Hide Banner Called ---------->");
//			ConsoliAds.Instance.HideBanner ();
//		}else{
//			Debug.LogWarning("Consoli Ads Instance Not Found !");
//		}
	}

	public static void RemoveAdvertisements(){
//		if (ConsoliAds.Instance != null){
//			ConsoliAds.Instance.HideBanner ();
//			ConsoliAds.Instance.hideAds = true;
//		}else{
//			Debug.LogWarning("Consoli Ads Instance Not Found !");
//		}
	}
}
