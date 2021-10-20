using UnityEngine;
using System.Collections;
using VoxelBusters.NativePlugins;

public class GF_InAppController : MonoBehaviour{

	public static GF_InAppController Instance { get; private set; }

	private BillingProduct[] m_products;
	private bool m_productRequestFinished = false;

	void Awake (){

		if (Instance != null) {
			DestroyImmediate (gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad (gameObject);

		// Intialise
		#if USES_BILLING
		m_products = NPSettings.Billing.Products;
		m_productRequestFinished = false;
		RequestBillingProducts (m_products);
		#endif
	}

	#if USES_BILLING
	void OnEnable (){
		// Register for callbacks
		Billing.DidFinishRequestForBillingProductsEvent += OnDidFinishRequestForBillingProducts;
		Billing.DidFinishProductPurchaseEvent += OnDidFinishProductPurchase;
		Billing.DidFinishRestoringPurchasesEvent += OnDidFinishRestoringPurchases;
	}

	void OnDisable (){
		// Deregister for callbacks
		Billing.DidFinishRequestForBillingProductsEvent -= OnDidFinishRequestForBillingProducts;
		Billing.DidFinishProductPurchaseEvent -= OnDidFinishProductPurchase;
		Billing.DidFinishRestoringPurchasesEvent -= OnDidFinishRestoringPurchases;
	}
	#endif

	private void OnDidFinishRequestForBillingProducts (BillingProduct[] _products, string _error){
		if (_products != null) {
			m_productRequestFinished = true;
		}
	}

	private void OnDidFinishProductPurchase (BillingTransaction _transaction)
	{
		if (_transaction.VerificationState == eBillingTransactionVerificationState.SUCCESS) {
			if (_transaction.TransactionState == eBillingTransactionState.PURCHASED) {
				//Give your In-Apps Here
				switch (_transaction.ProductIdentifier) {
				//Enter your remove ads ID
				case "product_remove_ads":
					SaveData.Instance.RemoveAds = true;
					GF_SaveLoad.SaveProgress ();
					GF_AdsManager.RemoveAdvertisements ();
					NPBinding.UI.ShowAlertDialogWithSingleButton ("Congratulations", "All advertisemetns have been removed !", "Ok", null);
					break;

				case "five_thousand_coins":
					//Give Item Here
					SaveData.Instance.Coins += 5000;
					GF_SaveLoad.SaveProgress ();
					NPBinding.UI.ShowAlertDialogWithSingleButton ("Congratulations", "50000 Coins Added to Your Inventory !", "Ok", null);

					break;
				}
			}  else if (_transaction.TransactionState == eBillingTransactionState.FAILED) {
				NPBinding.UI.ShowAlertDialogWithSingleButton ("Error", "Failed to purchase item ! Please try again later", "Ok", null);
			}
		}
	}

	private void OnDidFinishRestoringPurchases (BillingTransaction[] _transactions, string _error)
	{
		if (_error == null) {
			if (_transactions.Length > 0) {
				foreach (BillingTransaction _eachTransaction in _transactions) {
					if (_eachTransaction.VerificationState == eBillingTransactionVerificationState.SUCCESS) {
						switch (_eachTransaction.ProductIdentifier) {
						case "product_remove_ads":
							SaveData.Instance.RemoveAds = true;
							GF_SaveLoad.SaveProgress ();
							GF_AdsManager.RemoveAdvertisements ();
							NPBinding.UI.ShowAlertDialogWithSingleButton ("Restore Successful", "All advertisemetns have been removed !", "Ok", null);
							break;
						}
					}
				}
			}  else {
				NPBinding.UI.ShowAlertDialogWithSingleButton ("Alert", "No Restoreable Items Found !", "Ok", null);
			}
		}
	}

	private BillingProduct GetCurrentProduct (int id){
		return m_products [id];
	}

	private void BuyProduct (BillingProduct _product){
		#if USES_BILLING
		NPBinding.Billing.BuyProduct (_product);
		#endif
	}

	private void RequestBillingProducts (BillingProduct[] _products){
		#if USES_BILLING
		NPBinding.Billing.RequestForBillingProducts (_products);
		#endif
	}

	#if USES_BILLING
	private bool IsProductPurchased (BillingProduct _product){
		return NPBinding.Billing.IsProductPurchased (_product);
	}
	#endif

	public void BuyInAppProduct (int item_id){
		BuyProduct (GetCurrentProduct (item_id));
	}

	public void RestorePurchases (){
		#if USES_BILLING
		NPBinding.Billing.RestorePurchases ();
		#endif
	}
}
