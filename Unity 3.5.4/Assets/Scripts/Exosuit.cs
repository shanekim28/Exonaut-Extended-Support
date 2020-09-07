using LitJson;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Exosuit
{
	public int mSuitId = -1;

	public int mFactionId = -1;

	public string mSuitName = string.Empty;

	public string mSuitFileName = string.Empty;

	public string mDescription = string.Empty;

	public string mShowName = string.Empty;

	public int mCost = -1;

	public int mRank = -1;

	public int mLevelRequirement = -1;

	public GameObject mLowPolyModel;

	public GameObject mHighPolyModel;

	public ArrayList mLowPolyTextures = new ArrayList(3);

	public ArrayList mHighPolyTextures = new ArrayList(3);

	public Texture mGuiLoadoutImage;

	public int mRunIndex = -1;

	public int mRollIndex = -1;

	public int mWeaponModIndex = -1;

	public string mWeaponModName;

	public string mWeaponModDescription;

	public int mBaseHealth;

	public int mBaseRegenHealth;

	public int mBaseSpeed;

	public int mBaseJetFuel;

	public int mBaseTech;

	public string[] mAbilityName = new string[3];

	public string[] mAbilityDescription = new string[3];

	public int[] mAbilityDuration = new int[3];

	public int[] mAbilityCooldown = new int[3];

	public int Active;

	public int Power1;

	public int Power2;

	public int Power3;

	public int IdleIdx;

	public int Run_BackwardIdx;

	public int Roll_RecoverIdx;

	public int CrouchIdx;

	public int Airdash_ForwardIdx;

	public int Airdash_BackwardIdx;

	public int Airdash_Recover_ForwardIdx;

	public int Airdash_Recover_BackwardIdx;

	public int Capture_AirIdx;

	public int Capture_GroundIdx;

	public int Release_AirIdx;

	public int Release_GroundIdx;

	public int JetpackIdx;

	public int Jump_BeginIdx;

	public int Jump_FallIdx;

	public int Jump_LandIdx;

	public int ShootIdx;

	public int Left_Arm_RunIdx;

	public int Health;

	public int Regen_Speed;

	public int Regen_Delay;

	public int Run_Spd_Adj;

	public int Roll_Spd;

	public float Roll_Rcvy;

	public int Jetpack_Spd_Adj;

	public int Jetpack_Vrt_Spd;

	public int Airdash_Spd_Max;

	public int Airdash_Dst_Max;

	public float Airdash_Sbd_Rt;

	public int Jump_Vel;

	public int Acc_Grnd;

	public int Acc_Air;

	public int Fuel_Max;

	public int Fuel_Regen;

	public float Fuel_Delay;

	public int Timer_Boosts;

	public int Timer_Sp_Weps;

	public int Timer_Free;

	public int CoolDown_Sp_Weps;

	public Exosuit(JsonData suitData)
	{
		mSuitId = Convert.ToInt32((string)suitData["ID"]);
		mSuitName = (string)suitData["Name"];
		mDescription = (string)suitData["Description"];
		mSuitFileName = (string)suitData["Filename"];
		mShowName = (string)suitData["ShowName"];
		mGuiLoadoutImage = (Resources.Load("SuitChooser/ex_" + mSuitFileName) as Texture2D);
		mFactionId = Convert.ToInt32((string)suitData["Faction"]);
		mRank = Convert.ToInt32((string)suitData["Rank"]);
		mLevelRequirement = Convert.ToInt32((string)suitData["Level"]);
		mCost = Convert.ToInt32((string)suitData["Cost"]);
		Active = Convert.ToInt32((string)suitData["Active"]);
		mBaseHealth = Convert.ToInt32((string)suitData["Health_Rating"]);
		mBaseRegenHealth = Convert.ToInt32((string)suitData["Health_Regen_Rating"]);
		mBaseSpeed = Convert.ToInt32((string)suitData["Speed_Rating"]);
		mBaseJetFuel = Convert.ToInt32((string)suitData["Jetpack_Rating"]);
		mBaseTech = Convert.ToInt32((string)suitData["Tech_Rating"]);
		Power1 = Convert.ToInt32((string)suitData["Power1"]);
		Power2 = Convert.ToInt32((string)suitData["Power2"]);
		Power3 = Convert.ToInt32((string)suitData["Power3"]);
		mWeaponModIndex = Convert.ToInt32((string)suitData["WeaponMod"]);
		WeaponModData weaponModData = (WeaponModData)GameData.WeaponMods[mWeaponModIndex];
		if (weaponModData != null)
		{
			mWeaponModName = weaponModData.mName;
			mWeaponModDescription = weaponModData.mDescription;
		}
		else
		{
			Logger.traceError("no mod for weapon mod index: " + mWeaponModIndex + " the database is messed up.");
		}
		Health = Convert.ToInt32((string)suitData["Health"]);
		Regen_Speed = Convert.ToInt32((string)suitData["Regen_Speed"]);
		Regen_Delay = Convert.ToInt32((string)suitData["Regen_Delay"]);
		Run_Spd_Adj = Convert.ToInt32((string)suitData["Run_Spd_Adj"]);
		Roll_Spd = Convert.ToInt32((string)suitData["Roll_Spd"]);
		Roll_Rcvy = Convert.ToSingle((string)suitData["Roll_Rcvy"]);
		Jetpack_Spd_Adj = Convert.ToInt32((string)suitData["Jetpack_Spd_Adj"]);
		Jetpack_Vrt_Spd = Convert.ToInt32((string)suitData["Jetpack_Vrt_Spd"]);
		Airdash_Spd_Max = Convert.ToInt32((string)suitData["Airdash_Spd_Max"]);
		Airdash_Dst_Max = Convert.ToInt32((string)suitData["Airdash_Dst_Max"]);
		Airdash_Sbd_Rt = Convert.ToSingle((string)suitData["Airdash_Sbd_Rt"]);
		Jump_Vel = Convert.ToInt32((string)suitData["Jump_Vel"]);
		Acc_Grnd = Convert.ToInt32((string)suitData["Acc_Grnd"]);
		Acc_Air = Convert.ToInt32((string)suitData["Acc_Air"]);
		Fuel_Max = Convert.ToInt32((string)suitData["Fuel_Max"]);
		Fuel_Regen = Convert.ToInt32((string)suitData["Fuel_Regen"]);
		Fuel_Delay = Convert.ToSingle((string)suitData["Fuel_Delay"]);
		Timer_Boosts = Convert.ToInt32((string)suitData["Timer_Boosts"]);
		Timer_Sp_Weps = Convert.ToInt32((string)suitData["Timer_Sp_Weps"]);
		Timer_Free = Convert.ToInt32((string)suitData["Timer_Free"]);
		CoolDown_Sp_Weps = Convert.ToInt32((string)suitData["CoolDown_Sp_Weps"]);
		IdleIdx = Convert.ToInt32((string)suitData["IdleIdx"]);
		mRunIndex = Convert.ToInt32((string)suitData["RunIdx"]);
		Run_BackwardIdx = Convert.ToInt32((string)suitData["Run_BackwardIdx"]);
		mRollIndex = Convert.ToInt32((string)suitData["RollIdx"]);
		Roll_RecoverIdx = Convert.ToInt32((string)suitData["Roll_RecoverIdx"]);
		CrouchIdx = Convert.ToInt32((string)suitData["CrouchIdx"]);
		Airdash_ForwardIdx = Convert.ToInt32((string)suitData["Airdash_ForwardIdx"]);
		Airdash_BackwardIdx = Convert.ToInt32((string)suitData["Airdash_BackwardIdx"]);
		Airdash_Recover_ForwardIdx = Convert.ToInt32((string)suitData["Airdash_Recover_ForwardIdx"]);
		Airdash_Recover_BackwardIdx = Convert.ToInt32((string)suitData["Airdash_Recover_BackwardIdx"]);
		Capture_AirIdx = Convert.ToInt32((string)suitData["Capture_AirIdx"]);
		Capture_GroundIdx = Convert.ToInt32((string)suitData["Capture_GroundIdx"]);
		Release_AirIdx = Convert.ToInt32((string)suitData["Release_AirIdx"]);
		Release_GroundIdx = Convert.ToInt32((string)suitData["Release_GroundIdx"]);
		JetpackIdx = Convert.ToInt32((string)suitData["JetpackIdx"]);
		Jump_BeginIdx = Convert.ToInt32((string)suitData["Jump_BeginIdx"]);
		Jump_FallIdx = Convert.ToInt32((string)suitData["Jump_FallIdx"]);
		Jump_LandIdx = Convert.ToInt32((string)suitData["Jump_LandIdx"]);
		ShootIdx = Convert.ToInt32((string)suitData["ShootIdx"]);
		Left_Arm_RunIdx = Convert.ToInt32((string)suitData["Left_Arm_RunIdx"]);
	}

	public void setLowPolyModel(GameObject model, Material texture)
	{
		mLowPolyModel = model;
		mLowPolyTextures.Add(texture);
	}

	public void setHighPolyModel(GameObject model, Material mask, Material armor)
	{
		mHighPolyModel = model;
		mHighPolyTextures.Add(mask);
		mHighPolyTextures.Add(armor);
	}

	public void addLowPolyTexture(Material texture)
	{
		mLowPolyTextures.Add(texture);
	}

	public void addHighPolyTextures(Material mask, Material armor)
	{
		mHighPolyTextures.Add(mask);
		mHighPolyTextures.Add(armor);
	}

	public void swapLowPolyTextureSet(int index)
	{
		mLowPolyModel.transform.FindChild("armor").GetComponent<Renderer>().material = (mLowPolyTextures[index] as Material);
	}

	public void swapHighPolyTextureSet(int index)
	{
		mHighPolyModel.transform.FindChild("mask").GetComponent<Renderer>().material = (mHighPolyTextures[index * 2] as Material);
		mHighPolyModel.transform.FindChild("armor").GetComponent<Renderer>().material = (mHighPolyTextures[index * 2 + 1] as Material);
	}

	public GameObject getLowPolyModel()
	{
		return mLowPolyModel;
	}

	public GameObject getHighPolyModel()
	{
		return mHighPolyModel;
	}

	public bool getIsLowPolyModelLoaded()
	{
		return mLowPolyModel != null;
	}

	public bool getIsHighPolyModelLoaded()
	{
		return mHighPolyModel != null;
	}

	public ArrayList getLowPolyTextures()
	{
		return mLowPolyTextures;
	}

	public ArrayList getHighPolyTextures()
	{
		return mHighPolyTextures;
	}

	public Material getLowPolyTexture()
	{
		return (Material)mLowPolyTextures[0];
	}

	public Material[] getHighPolyTexture()
	{
		return new Material[2]
		{
			(Material)mHighPolyTextures[0],
			(Material)mHighPolyTextures[1]
		};
	}
}
