using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VoxelBusters.NativePlugins;

[System.Serializable]
public class VirtualItem {
    public string Name;
    public string ItemKey;
    public int Price;
    public int AmountToGive;
    public Sprite Icon;
    public Button ItemButton;
}

[System.Serializable]
public class CurrencyItem {
    public string Name;
    public string ItemKey;
    public string Price;
    public Sprite Icon;
    public Button ItemButton;
}

public class GF_StoreManager : MonoBehaviour {

    [Header("Virtual Items")]
    public VirtualItem[] VirtualItems;

    [Header("Currency Items")]
    public CurrencyItem[] CurrencyItems;

    public Text Coins;

	bool _isConnected;

    void Start () {
		InitializeStore();
    }

    void LateUpdate() {
        UpdateCoins();
    }

    void InitializeStore() {
        for (int i = 0; i < VirtualItems.Length; i++) {
            VirtualItems[i].ItemButton.transform.Find("Name").GetComponent<Text>().text = VirtualItems[i].Name;
            VirtualItems[i].ItemButton.transform.Find("Price").GetComponent<Text>().text = VirtualItems[i].Price.ToString();
            VirtualItems[i].ItemButton.transform.Find("Icon").GetComponent<Image>().sprite = VirtualItems[i].Icon;
            int item_id = i;
            VirtualItems[i].ItemButton.onClick.AddListener(() => PurchaseVirtualItem(item_id));
        }

        for (int i = 0; i < CurrencyItems.Length; i++) {
            CurrencyItems[i].ItemButton.transform.Find("Name").GetComponent<Text>().text = CurrencyItems[i].Name;
            CurrencyItems[i].ItemButton.transform.Find("Price").GetComponent<Text>().text = CurrencyItems[i].Price;
            CurrencyItems[i].ItemButton.transform.Find("Icon").GetComponent<Image>().sprite = CurrencyItems[i].Icon;
            int item_id = i;
            CurrencyItems[i].ItemButton.onClick.AddListener(() => PurchaseInAppItem(item_id));
        }
    }

    public void PurchaseVirtualItem(int id) {
        if (CoinsCheck(VirtualItems[id].Price)) {
            if(VirtualItems[id].ItemKey == "Bullets") {
                NPBinding.UI.ShowAlertDialogWithSingleButton("Congratulations", VirtualItems[id].Name+" Added to Your Inventory !", "Ok", null);
            }
            SaveData.Instance.Coins -= VirtualItems[id].Price;
			GF_SaveLoad.SaveProgress();

            //Enter your Inventory Logic in GameManager
            GameManager.Instance.UpdateInventory();
        }
        else {
            NPBinding.UI.ShowAlertDialogWithSingleButton("Alert", " You Don't Have Enough Coins !", "Ok", null);
        }
    }

    public void PurchaseInAppItem(int id) {
        GF_InAppController.Instance.BuyInAppProduct(id+1);
    }

    bool CoinsCheck(int amount) {
        if (SaveData.Instance.Coins >= amount)
            return true;
        else
            return false;
    }

    public void UpdateCoins() {
        Coins.text = SaveData.Instance.Coins.ToString();
    }

	public void CloseStore(){
		Time.timeScale = 1.0f;
		AudioListener.pause = false;
		gameObject.SetActive (false);
	}
}
