using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
	[Serializable]
	public abstract class ExonautAsset
	{
		public int mPriority;

		public string myFilename;

		public WWW myWWW;

		public abstract IEnumerator Load();
	}

	[Serializable]
	public class SuitAsset : ExonautAsset
	{
		public enum SuitType
		{
			low,
			high
		}

		public int mSuitID = -1;

		public SuitType mModelType;

		public override IEnumerator Load()
		{
			SuitAsset Suit = mInstance.mCurrentLoading as SuitAsset;
			Exosuit suitToLoad = GameData.getExosuit(Suit.mSuitID);
			myWWW = new WWW(GameData.BUNDLE_PATH + Suit.myFilename);
			Logger.trace("LoadSuitAsset " + myWWW.url);
			yield return myWWW;
			if (myWWW.error == null)
			{
				yield return myWWW;
				if (myWWW == null)
				{
					Logger.trace("www is null");
					yield break;
				}
				if (myWWW.assetBundle != null)
				{
					AssetBundle assetBundle = myWWW.assetBundle;
					string fileName = suitToLoad.mSuitFileName;
					AssetBundleRequest abr4 = assetBundle.LoadAssetAsync(fileName + "_" + Suit.mModelType + "_pre", typeof(GameObject));
					yield return abr4;
					GameObject suitModel = abr4.asset as GameObject;
					if (Suit.mModelType == SuitType.low)
					{
						string textureName = fileName + "_sheet_1";
						abr4 = assetBundle.LoadAssetAsync(textureName, typeof(Material));
						yield return abr4;
						GameData.setLowPolySuitIsLoaded(texture: abr4.asset as Material, suitId: Suit.mSuitID, model: suitModel);
					}
					else
					{
						object[] arry = assetBundle.LoadAllAssets();
						object[] array = arry;
						foreach (object obj in array)
						{
							if (obj is Texture2D)
							{
							}
							abr4 = assetBundle.LoadAssetAsync(fileName + "_mask_1", typeof(Material));
							yield return abr4;
							Material MaskMat = abr4.asset as Material;
							abr4 = assetBundle.LoadAssetAsync(fileName + "_armor_1", typeof(Material));
							yield return abr4;
							GameData.setHighPolySuitIsLoaded(armor: abr4.asset as Material, suitId: Suit.mSuitID, model: suitModel, mask: MaskMat);
						}
					}
					assetBundle.Unload(unloadAllLoadedObjects: false);
				}
			}
			mInstance.mCurrentLoading = null;
		}
	}

	public Exosuit[] Suits;

	public ExonautAsset mCurrentLoading;

	public List<ExonautAsset> mAssetQueue = new List<ExonautAsset>();

	public static AssetLoader mInstance;

	private void Awake()
	{
		mInstance = this;
		mCurrentLoading = null;
	}

	public static void AddSuitToLoad(int suitId, SuitAsset.SuitType model_type, int Priority)
	{
		if (mInstance == null)
		{
			Debug.Log("<< mInstance is null ");
		}
		foreach (SuitAsset item in mInstance.mAssetQueue)
		{
			if (item == null)
			{
				Debug.Log("<< Suit is null " + mInstance.mAssetQueue.Count);
			}
			if (item.mSuitID == suitId && item.mModelType == model_type)
			{
				if (item.mPriority != Priority)
				{
					item.mPriority = Priority;
					mInstance.mAssetQueue.Sort(ComparePriority);
				}
				return;
			}
		}
		Exosuit exosuit = GameData.getExosuit(suitId);
		if (exosuit != null)
		{
			string mSuitFileName = exosuit.mSuitFileName;
			string str = mSuitFileName + "_" + model_type + ".unity3d";
			SuitAsset suitAsset2 = new SuitAsset();
			suitAsset2.mSuitID = suitId;
			suitAsset2.mPriority = Priority;
			suitAsset2.myFilename = "suits/" + str;
			suitAsset2.mModelType = model_type;
			mInstance.mAssetQueue.Add(suitAsset2);
			mInstance.mAssetQueue.Sort(ComparePriority);
		}
	}

	public static float GetSuitLoadProgress(int suitID, SuitAsset.SuitType model_type)
	{
		if (mInstance.mCurrentLoading == null)
		{
			return 0f;
		}
		foreach (SuitAsset item in mInstance.mAssetQueue)
		{
			if (item.mSuitID == suitID && item.myFilename == mInstance.mCurrentLoading.myFilename)
			{
				if (mInstance.mCurrentLoading.myWWW != null)
				{
					return mInstance.mCurrentLoading.myWWW.progress;
				}
				return 0f;
			}
		}
		return 0f;
	}

	private static int ComparePriority(ExonautAsset AssetA, ExonautAsset AssetB)
	{
		return AssetB.mPriority.CompareTo(AssetA.mPriority);
	}

	private void FixedUpdate()
	{
		if (mCurrentLoading == null && mAssetQueue.Count > 0)
		{
			LoadAsset(mAssetQueue[0]);
			mAssetQueue.RemoveAt(0);
		}
	}

	private void LoadAsset(ExonautAsset Asset)
	{
		mCurrentLoading = Asset;
		StartCoroutine(Asset.Load());
	}
}
