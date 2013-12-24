using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace com.soomla.unity
{
	/// <summary>
	/// This class holds the store's meta data including:
	/// - Virtual Currencies definitions
	/// - Virtual Currency Packs definitions
	/// - Virtual Goods definitions
	/// - Virtual Categories definitions
	/// - Virtual Non-Consumable items definitions
	/// </summary>
	public static class StoreInfo
	{
		private const string TAG = "SOOMLA StoreInfo";

#if UNITY_EDITOR
		//****
		// Create a local copy of VirtualGoods for display in the Unity editor window 
		private static List<VirtualCurrency>localCurrencies;
		private static List<VirtualCurrencyPack>localCurrencyPacks;
		private static List<VirtualGood>localVirtualGoods;
		private static List<VirtualCategory>localCategories;
		private static List<NonConsumableItem>localNonConsumableItems;

#endif

		
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetItemByItemId(string itemId, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetPurchasableItemWithProductId(string productId, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetCategoryForVirtualGood(string goodItemId, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetFirstUpgradeForVirtualGood(string goodItemId, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetLastUpgradeForVirtualGood(string goodItemId, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetUpgradesForVirtualGood(string goodItemId, out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetVirtualCurrencies(out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetVirtualGoods(out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetVirtualCurrencyPacks(out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetNonConsumableItems(out IntPtr json);
		[DllImport ("__Internal")]
		private static extern int storeInfo_GetVirtualCategories(out IntPtr json);
		[DllImport ("__Internal")]
		private static extern void storeAssets_Init(int version, string storeAssetsJSON);
#endif
	


		public static void Initialize(IStoreAssets storeAssets) {
			// Initialize

#if UNITY_EDITOR
			// Initialise lists of local data for viewing in the Unity editor
			//**** 
			localCurrencies = 			new List<VirtualCurrency>(storeAssets.GetCurrencies());
			localCurrencyPacks = 		new List<VirtualCurrencyPack>(storeAssets.GetCurrencyPacks());
			localVirtualGoods = 		new List<VirtualGood>(storeAssets.GetGoods());
			localCategories = 			new List<VirtualCategory>(storeAssets.GetCategories());
			localNonConsumableItems = 	new List<NonConsumableItem>(storeAssets.GetNonConsumableItems());

//			Debug.Log("localCurrencies: ");
//			foreach(VirtualCurrency vc in localCurrencies){
//				Debug.Log(vc.Name);
//			}
//			
//			Debug.Log("localCurrencyPacks: ");
//			foreach(VirtualCurrencyPack vc in localCurrencyPacks){
//				Debug.Log(vc.Name);
//			}
//			
//			Debug.Log("localVirtualGoods: ");
//			foreach(VirtualGood vc in localVirtualGoods){
//				Debug.Log(vc.Name);
//			}
//			
//			Debug.Log("localCategories: ");
//			foreach(VirtualCategory vc in localCategories){
//				Debug.Log(vc.Name);
//			}
//			
//			Debug.Log("localNonConsumableItems: ");
//			foreach(NonConsumableItem vc in localNonConsumableItems){
//				Debug.Log(vc.Name);
//			}

#endif


//			StoreUtils.LogDebug(TAG, "Adding currency");
			JSONObject currencies = new JSONObject(JSONObject.Type.ARRAY);
			foreach(VirtualCurrency vi in storeAssets.GetCurrencies()) {
				currencies.Add(vi.toJSONObject());
				//Debug.Log("vi.Name: " + vi.Name);
			}

//			StoreUtils.LogDebug(TAG, "Adding packs");
			JSONObject packs = new JSONObject(JSONObject.Type.ARRAY);
			foreach(VirtualCurrencyPack vi in storeAssets.GetCurrencyPacks()) {
				packs.Add(vi.toJSONObject());
				//Debug.Log("vi.Name: " + vi.Name);
			}

//			StoreUtils.LogDebug(TAG, "Adding goods");

		    JSONObject suGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject ltGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject eqGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject upGoods = new JSONObject(JSONObject.Type.ARRAY);
		    JSONObject paGoods = new JSONObject(JSONObject.Type.ARRAY);
		    foreach(VirtualGood g in storeAssets.GetGoods()){
		        if (g is SingleUseVG) {
		            suGoods.Add(g.toJSONObject());
		        } else if (g is EquippableVG) {
		            eqGoods.Add(g.toJSONObject());
			    } else if (g is UpgradeVG) {
   		            upGoods.Add(g.toJSONObject());
   		        } else if (g is LifetimeVG) {
		            ltGoods.Add(g.toJSONObject());
		        } else if (g is SingleUsePackVG) {
		            paGoods.Add(g.toJSONObject());
		        }
		    }
			JSONObject goods = new JSONObject(JSONObject.Type.OBJECT);
			goods.AddField(JSONConsts.STORE_GOODS_SU, suGoods);
			goods.AddField(JSONConsts.STORE_GOODS_LT, ltGoods);
			goods.AddField(JSONConsts.STORE_GOODS_EQ, eqGoods);
			goods.AddField(JSONConsts.STORE_GOODS_UP, upGoods);
			goods.AddField(JSONConsts.STORE_GOODS_PA, paGoods);
			
//			StoreUtils.LogDebug(TAG, "Adding categories");
			JSONObject categories = new JSONObject(JSONObject.Type.ARRAY);
			foreach(VirtualCategory vi in storeAssets.GetCategories()) {
				categories.Add(vi.toJSONObject());
			}
			
//			StoreUtils.LogDebug(TAG, "Adding nonConsumables");
			JSONObject nonConsumables = new JSONObject(JSONObject.Type.ARRAY);
			foreach(NonConsumableItem vi in storeAssets.GetNonConsumableItems()) {
				nonConsumables.Add(vi.toJSONObject());
			}
			
//			StoreUtils.LogDebug(TAG, "Preparing StoreAssets  JSONObject");
			JSONObject storeAssetsObj = new JSONObject(JSONObject.Type.OBJECT);
			storeAssetsObj.AddField(JSONConsts.STORE_CATEGORIES, categories);
			storeAssetsObj.AddField(JSONConsts.STORE_CURRENCIES, currencies);
			storeAssetsObj.AddField(JSONConsts.STORE_CURRENCYPACKS, packs);
			storeAssetsObj.AddField(JSONConsts.STORE_GOODS, goods);
			storeAssetsObj.AddField(JSONConsts.STORE_NONCONSUMABLES, nonConsumables);
			
			//string storeAssetsJSON = storeAssetsObj.print();
			//Debug.Log("storeAssetsJSON: " + storeAssetsJSON);
			
#if UNITY_ANDROID && !UNITY_EDITOR
			StoreUtils.LogDebug(TAG, "pushing data to StoreAssets on java side");
			using(AndroidJavaClass jniStoreAssets = new AndroidJavaClass("com.soomla.unity.StoreAssets")) {
				jniStoreAssets.CallStatic("prepare", storeAssets.GetVersion(), storeAssetsJSON);
			}
			StoreUtils.LogDebug(TAG, "done! (pushing data to StoreAssets on java side)");
#elif UNITY_IOS && !UNITY_EDITOR
			StoreUtils.LogDebug(TAG, "pushing data to StoreAssets on ios side");
			storeAssets_Init(storeAssets.GetVersion(), storeAssetsJSON);
			StoreUtils.LogDebug(TAG, "done! (pushing data to StoreAssets on ios side)");
#endif
		}
		
		public static VirtualItem GetItemByItemId(string itemId) {
			StoreUtils.LogDebug(TAG, "Trying to fetch an item with itemId: " + itemId);
#if UNITY_ANDROID && !UNITY_EDITOR
			VirtualItem vi = null;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualItem = AndroidJNIHandler.CallStatic<AndroidJavaObject>(
				new AndroidJavaClass("com.soomla.store.data.StoreInfo"),"getVirtualItem", itemId)) {
				vi = VirtualItem.factoryItemFromJNI(jniVirtualItem);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return vi;
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetItemByItemId(itemId, out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string json = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + json);
			
			JSONObject obj = new JSONObject(json);
			return VirtualItem.factoryItemFromJSONObject(obj);
#elif UNITY_EDITOR
			// Get the Virtual Good with itemId from localVirtualGoods[] and return it
			//****
			foreach(VirtualGood vc in localVirtualGoods){

				if(vc.ItemId == itemId){
					Debug.Log(vc.Name);
					return vc;
				}

			}
			Debug.Log("Item not found!!!!");
			return null;

#else
			return null;
#endif
		}
		
		public static PurchasableVirtualItem GetPurchasableItemWithProductId(string productId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			VirtualItem vi = null;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualItem = AndroidJNIHandler.CallStatic<AndroidJavaObject>(
				new AndroidJavaClass("com.soomla.store.data.StoreInfo"),"getPurchasableItem", productId)) {
				vi = VirtualItem.factoryItemFromJNI(jniVirtualItem);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return (PurchasableVirtualItem)vi;
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetPurchasableItemWithProductId(productId, out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string nonConsJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			JSONObject obj = new JSONObject(nonConsJson);
			return (PurchasableVirtualItem)VirtualItem.factoryItemFromJSONObject(obj);
#elif UNITY_EDITOR
			// Get the PurchasableVirtualItem from localVirtualGoods[]
			//****
			return null;

#else
			return null;
#endif
		}
		
		public static VirtualCategory GetCategoryForVirtualGood(string goodItemId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			VirtualCategory vc = null;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualVategory = AndroidJNIHandler.CallStatic<AndroidJavaObject>(
				new AndroidJavaClass("com.soomla.store.data.StoreInfo"),"getCategory", goodItemId)) {
				vc = new VirtualCategory(jniVirtualVategory);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return vc;
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetCategoryForVirtualGood(goodItemId, out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string json = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			JSONObject obj = new JSONObject(json);
			return new VirtualCategory(obj);
#elif UNITY_EDITOR
			// Get the VirtualCategory from localCategories[] 
			//****

			foreach(VirtualCategory obj in localCategories) {
				foreach(String itemID in obj.GoodItemIds){
					if(itemID == goodItemId){
						return obj;
					}
				}
			}
			Debug.Log("Category not found!!!!");
			return null;

#else
			return null;
#endif
		}
		
		public static UpgradeVG GetFirstUpgradeForVirtualGood(string goodItemId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			UpgradeVG vgu = null;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniUpgradeVG = AndroidJNIHandler.CallStatic<AndroidJavaObject>(
				new AndroidJavaClass("com.soomla.store.data.StoreInfo"),"getGoodFirstUpgrade", goodItemId)) {
				vgu = new UpgradeVG(jniUpgradeVG);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return vgu;
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetFirstUpgradeForVirtualGood(goodItemId, out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string json = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			JSONObject obj = new JSONObject(json);
			return new UpgradeVG(obj);
#elif UNITY_EDITOR
			// Get the UpgradeVG from localVirtualGoods[] and return it
			//****
			// I am not sure what the rules for this are.
			return null;

#else
			return null;
#endif
		}
		
		public static UpgradeVG GetLastUpgradeForVirtualGood(string goodItemId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			UpgradeVG vgu = null;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniUpgradeVG = AndroidJNIHandler.CallStatic<AndroidJavaObject>(
				new AndroidJavaClass("com.soomla.store.data.StoreInfo"),"getGoodLastUpgrade", goodItemId)) {
				vgu = new UpgradeVG(jniUpgradeVG);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			return vgu;
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetLastUpgradeForVirtualGood(goodItemId, out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string json = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			JSONObject obj = new JSONObject(json);
			return new UpgradeVG(obj);
#elif UNITY_EDITOR
			// Get the UpgradeVG from localVirtualGoods[] and return it
			// I am not entirely sure what this does.
			// We may need to create the functionality to store upgrade infomation in Unity for testing
			//****
			return null;

#else
			return null;
#endif
		}
			
		public static List<UpgradeVG> GetUpgradesForVirtualGood(string goodItemId) {
			StoreUtils.LogDebug(TAG, "Trying to fetch upgrades for " + goodItemId);
			List<UpgradeVG> vgus = new List<UpgradeVG>();
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniUpgradeVGs = new AndroidJavaClass("com.soomla.store.data.StoreInfo").CallStatic<AndroidJavaObject>("getGoodUpgrades")) {
				for(int i=0; i<jniUpgradeVGs.Call<int>("size"); i++) {
					using(AndroidJavaObject jnivgu = jniUpgradeVGs.Call<AndroidJavaObject>("get", i)) {
						vgus.Add(new UpgradeVG(jnivgu));
					}
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetUpgradesForVirtualGood(goodItemId, out p);

			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string upgradesJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + upgradesJson);
			
			JSONObject upgradesArr = new JSONObject(upgradesJson);
			foreach(JSONObject obj in upgradesArr.list) {
				vgus.Add(new UpgradeVG(obj));
			}
#elif UNITY_EDITOR
			// Get the list of UpgradeVG's from localVirtualGoods[] and return it
			//****
			Debug.Log("localVirtualGoods.count: " + localVirtualGoods.Count);
			foreach(VirtualGood obj in localVirtualGoods) {
				if(obj.GetType() == typeof(UpgradeVG)){

					UpgradeVG upgradeVG = (UpgradeVG)obj;
					if(upgradeVG.GoodItemId == goodItemId){
						vgus.Add((UpgradeVG)obj);
					}
				}
			}

#endif
			return vgus;
		}
		
		public static List<VirtualCurrency> GetVirtualCurrencies() {
			StoreUtils.LogDebug(TAG, "Trying to fetch currencies");
			List<VirtualCurrency> vcs = new List<VirtualCurrency>();
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualCurrencies = new AndroidJavaClass("com.soomla.store.data.StoreInfo").CallStatic<AndroidJavaObject>("getCurrencies")) {
				for(int i=0; i<jniVirtualCurrencies.Call<int>("size"); i++) {
					using(AndroidJavaObject jnivc = jniVirtualCurrencies.Call<AndroidJavaObject>("get", i)) {
						vcs.Add(new VirtualCurrency(jnivc));
					}
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetVirtualCurrencies(out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string currenciesJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + currenciesJson);
			
			JSONObject currenciesArr = new JSONObject(currenciesJson);
			foreach(JSONObject obj in currenciesArr.list) {
				vcs.Add(new VirtualCurrency(obj));
			}
#elif UNITY_EDITOR
			// Get the list of VirtualCurrency's from localCurrencies[] 
			//****
			Debug.Log("localCurrencies.count: " + localCurrencies.Count);
			foreach(VirtualCurrency obj in localCurrencies) {
				vcs.Add(obj);
			}

#endif
			return vcs;
		}
		
		public static List<VirtualGood> GetVirtualGoods() {
			Debug.Log("GetVirtualGoods ");
			StoreUtils.LogDebug(TAG, "Trying to fetch goods");
			List<VirtualGood> virtualGoods = new List<VirtualGood>();
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualGoods = new AndroidJavaClass("com.soomla.store.data.StoreInfo").CallStatic<AndroidJavaObject>("getGoods")) {
				for(int i=0; i<jniVirtualGoods.Call<int>("size"); i++) {
					AndroidJNI.PushLocalFrame(100);
					using(AndroidJavaObject jniGood = jniVirtualGoods.Call<AndroidJavaObject>("get", i)) {
						virtualGoods.Add((VirtualGood)VirtualItem.factoryItemFromJNI(jniGood));
					}
					AndroidJNI.PopLocalFrame(IntPtr.Zero);
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetVirtualGoods(out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string goodsJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + goodsJson);
			
			JSONObject goodsArr = new JSONObject(goodsJson);
			foreach(JSONObject obj in goodsArr.list) {
				virtualGoods.Add((VirtualGood)VirtualItem.factoryItemFromJSONObject(obj));
			}
#elif UNITY_EDITOR
			// Get the list of VirtualGoods from localVirtualGoods[] and return it
			//****
			Debug.Log("localVirtualGoods.count: " + localVirtualGoods.Count);
			foreach(VirtualGood obj in localVirtualGoods) {
				virtualGoods.Add(obj);
			}
#endif
			return virtualGoods;
		}
		
		public static List<VirtualCurrencyPack> GetVirtualCurrencyPacks() {
			StoreUtils.LogDebug(TAG, "Trying to fetch packs");
			List<VirtualCurrencyPack> vcps = new List<VirtualCurrencyPack>();
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualCurrencyPacks = new AndroidJavaClass("com.soomla.store.data.StoreInfo").CallStatic<AndroidJavaObject>("getCurrencyPacks")) {
				for(int i=0; i<jniVirtualCurrencyPacks.Call<int>("size"); i++) {
					using(AndroidJavaObject jnivcp = jniVirtualCurrencyPacks.Call<AndroidJavaObject>("get", i)) {
						vcps.Add(new VirtualCurrencyPack(jnivcp));
					}
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetVirtualCurrencyPacks(out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string packsJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + packsJson);
			
			JSONObject packsArr = new JSONObject(packsJson);
			foreach(JSONObject obj in packsArr.list) {
				vcps.Add(new VirtualCurrencyPack(obj));
			}
#elif UNITY_EDITOR
			// Get the list of VirtualCurrencyPack's from localCurrencyPacks[] 
			//****
			Debug.Log("localCurrencyPacks.count: " + localCurrencyPacks.Count);
			foreach(VirtualCurrencyPack obj in localCurrencyPacks) {
				vcps.Add(obj);
			}

#endif
			return vcps;
		}
		
		public static List<NonConsumableItem> GetNonConsumableItems() {
			StoreUtils.LogDebug(TAG, "Trying to fetch noncons");
			List<NonConsumableItem> nonConsumableItems = new List<NonConsumableItem>();
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniNonConsumableItems = new AndroidJavaClass("com.soomla.store.data.StoreInfo").CallStatic<AndroidJavaObject>("getNonConsumableItems")) {
				for(int i=0; i<jniNonConsumableItems.Call<int>("size"); i++) {
					using(AndroidJavaObject jniNon = jniNonConsumableItems.Call<AndroidJavaObject>("get", i)) {
						nonConsumableItems.Add(new NonConsumableItem(jniNon));
					}
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetNonConsumableItems(out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string nonConsumableJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + nonConsumableJson);
			
			JSONObject nonConsArr = new JSONObject(nonConsumableJson);
			foreach(JSONObject obj in nonConsArr.list) {
				nonConsumableItems.Add(new NonConsumableItem(obj));
			}
#elif UNITY_EDITOR
			// Get the list of NonConsumableItem's from localNonConsumableItems[] 
			//**** 
			Debug.Log("localNonConsumableItems.count: " + localNonConsumableItems.Count);
			foreach(NonConsumableItem obj in localNonConsumableItems) {
				nonConsumableItems.Add(obj);
			}

#endif
			return nonConsumableItems;
		}
		
		public static List<VirtualCategory> GetVirtualCategories() {
			StoreUtils.LogDebug(TAG, "Trying to fetch categories");
			List<VirtualCategory> virtualCategories = new List<VirtualCategory>();
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaObject jniVirtualCategories = new AndroidJavaClass("com.soomla.store.data.StoreInfo").CallStatic<AndroidJavaObject>("getCategories")) {
				for(int i=0; i<jniVirtualCategories.Call<int>("size"); i++) {
					using(AndroidJavaObject jniCat = jniVirtualCategories.Call<AndroidJavaObject>("get", i)) {
						virtualCategories.Add(new VirtualCategory(jniCat));
					}
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			int err = storeInfo_GetVirtualCategories(out p);
				
			IOS_ErrorCodes.CheckAndThrowException(err);
			
			string categoriesJson = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			
			StoreUtils.LogDebug(TAG, "Got json: " + categoriesJson);
			
			JSONObject categoriesArr = new JSONObject(categoriesJson);
			foreach(JSONObject obj in categoriesArr.list) {
				virtualCategories.Add(new VirtualCategory(obj));
			}
#elif UNITY_EDITOR
			// Get the list of VirtualCategory's from localCategories[] 
			//**** 
			foreach(VirtualCategory obj in localCategories) {
				virtualCategories.Add(obj);
			}

#endif
			return virtualCategories;
		}
	}
}

