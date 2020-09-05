using System;
using UnityEngine;

[Serializable]
public class SuitInspector
{
	public class TempSuitInfo
	{
		public int mIndex;

		public string mSuitShow = "Show";

		public string mSuitName;

		public string mDescription;

		public int mCost;

		public int mLevelRequirement;

		public int mShieldPower;

		public int mShieldRegen;

		public int mJetpack;

		public int mSpeed;

		public int mTech;

		public string mWeaponModName;

		public string mWeaponModDescription;
	}

	public GUISkin mSharedHudSkin;

	public GUISkin mShowcaseSkin;

	public Vector3[] ModelPositions = new Vector3[2]
	{
		new Vector3(4.2f, -5f, 25f),
		new Vector3(0f, 0f, 60f)
	};

	public Vector3[] ModelRotations = new Vector3[2]
	{
		new Vector3(0f, 180f, 0f),
		new Vector3(0f, 180f, 0f)
	};

	public Transform[] ModelTransforms = new Transform[2];

	public Vector3 mDefaultCameraPosition = new Vector3(-0f, 0f, 0f);

	public Vector3 mCameraOffset = Vector3.zero;

	private Vector3 mMaxCameraOffset = new Vector3(0f, 0f, 0f);

	private Vector3 mMinCameraOffset = new Vector3(0f, -3.5f, -10f);

	public Rect mRenderRect;

	public Texture2D[] WeaponTextures = new Texture2D[5];

	private Rect mWindowRect;

	private Rect mRightPanel;

	private Rect mBlock1;

	private Rect mBlock2;

	private Rect mBlock4;

	public SuitInspector()
	{
		mCameraOffset = mDefaultCameraPosition;
		mSharedHudSkin = GUIUtil.mInstance.mSharedSkin;
		mShowcaseSkin = GUIUtil.mInstance.mShowcaseSkin;
		ModelTransforms[1] = GUIUtil.mInstance.mBackground;
	}

	public void Init()
	{
		WeaponTextures[0] = (Resources.Load("Menus/hangar_weapon_icon_bulldog") as Texture2D);
		WeaponTextures[1] = (Resources.Load("Menus/hangar_weapon_icon_marksman") as Texture2D);
		WeaponTextures[2] = (Resources.Load("Menus/hangar_weapon_icon_ballista") as Texture2D);
		WeaponTextures[3] = (Resources.Load("Menus/hangar_weapon_icon_wildfire") as Texture2D);
		WeaponTextures[4] = (Resources.Load("Menus/hangar_weapon_icon_tridex") as Texture2D);
	}

	public void LimitCameraOffset()
	{
		mCameraOffset.x = Mathf.Clamp(mCameraOffset.x, mMinCameraOffset.x + mDefaultCameraPosition.x, mMaxCameraOffset.x + mDefaultCameraPosition.x);
		mCameraOffset.y = Mathf.Clamp(mCameraOffset.y, mMinCameraOffset.y + mDefaultCameraPosition.y, mMaxCameraOffset.y + mDefaultCameraPosition.y);
		mCameraOffset.z = Mathf.Clamp(mCameraOffset.z, mMinCameraOffset.z + mDefaultCameraPosition.z, mMaxCameraOffset.z + mDefaultCameraPosition.z);
	}

	public void UpdateScreenSpace(Rect screenSpace)
	{
		mWindowRect = new Rect(0f, 0f, screenSpace.width, screenSpace.height);
		mRenderRect.x = (screenSpace.x + 3f) / (float)Screen.width;
		mRenderRect.width = (screenSpace.width - 6f) / (float)Screen.width;
		mRenderRect.height = screenSpace.height - 6f;
		mRenderRect.y = ((float)Screen.height - mRenderRect.height - (screenSpace.y + 3f)) / (float)Screen.height;
		mRenderRect.height /= Screen.height;
		mRightPanel = new Rect(5f, 5f, 230f, screenSpace.height - 10f);
		mBlock1 = new Rect(0f, 0f, mRightPanel.width - 10f, (mRightPanel.height - 8f) * 0.26f);
		mBlock2 = new Rect(3f, mBlock1.y + mBlock1.height + 3f, mRightPanel.width - 16f, (mRightPanel.height - 8f) * 0.35f);
		mBlock4 = new Rect(3f, mBlock2.y + mBlock2.height + 3f, mRightPanel.width - 16f, (mRightPanel.height - 8f) * 0.39f);
	}

	public void DrawSuitInfo(TempSuitInfo SuitInfo, bool bDraw3D)
	{
		Exosuit exosuit = GameData.getExosuit(SuitInfo.mIndex);
		if (GameData.getExosuit(SuitInfo.mIndex).getHighPolyModel() != null)
		{
			ModelTransforms[0] = GameData.getExosuit(SuitInfo.mIndex).getHighPolyModel().transform;
		}
		else
		{
			ModelTransforms[0] = null;
		}
		GUISkin skin = GUI.skin;
		GUI.skin = mSharedHudSkin;
		if (ModelTransforms[0] != null)
		{
			AnimationState animationState = ModelTransforms[0].GetComponent<Animation>()[ModelTransforms[0].GetComponent<Animation>().clip.name];
			if (animationState != null)
			{
				float num = Time.realtimeSinceStartup / animationState.length;
				animationState.time = (num - (float)Mathf.RoundToInt(num)) * animationState.length;
			}
		}
		GUI.BeginGroup(mWindowRect);
		if (bDraw3D && Event.current.type == EventType.Repaint)
		{
			GUI.Box(mWindowRect, string.Empty, mSharedHudSkin.box);
			CameraClearFlags clearFlags = GUIUtil.mInstance.mModelRenderer.clearFlags;
			GUIUtil.mInstance.mModelRenderer.clearFlags = CameraClearFlags.Color;
			Vector3[] array = (Vector3[])ModelPositions.Clone();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] += mCameraOffset;
			}
			GUIUtil.RenderModelsToGUI(mRenderRect, 35f, array, ModelRotations, ModelTransforms, Color.black);
			GUIUtil.mInstance.mModelRenderer.clearFlags = clearFlags;
		}
		GUI.BeginGroup(mRightPanel);
		GUI.color = Color.white;
		if (Event.current.type == EventType.Repaint)
		{
			mSharedHudSkin.GetStyle("FactionFlag").Draw(new Rect(0f, 27f, mBlock1.width, mRightPanel.height - 27f), GUIContent.none, false, false, exosuit.mFactionId == 2, hasKeyboardFocus: false);
			mSharedHudSkin.GetStyle("InfoPanelOutline").Draw(new Rect(0f, 27f, mBlock1.width, mRightPanel.height - 27f), GUIContent.none, false, false, exosuit.mFactionId == 2, hasKeyboardFocus: false);
		}
		GUI.color = Color.white;
		GUI.BeginGroup(mBlock1);
		if (Event.current.type == EventType.Repaint)
		{
			mSharedHudSkin.GetStyle("InfoPanelHeader").Draw(mBlock1, SuitInfo.mSuitName, false, false, exosuit.mFactionId == 2, hasKeyboardFocus: false);
		}
		GUI.Box(new Rect(3f, 30f, mBlock1.width - 6f, mBlock1.height - 30f), SuitInfo.mSuitShow, "InfoPanelOverlay");
		GUI.Label(new Rect(5f, 45f, mBlock1.width - 10f, mBlock1.height - 48f), SuitInfo.mDescription, mShowcaseSkin.label);
		GUI.EndGroup();
		GUI.BeginGroup(mBlock2, "STATS", "InfoPanelOverlay");
		GUI.color = Color.white;
		GUI.Box(new Rect(0f, 15f, 50f, 26f), GUIContent.none, mShowcaseSkin.GetStyle("ShieldPower"));
		GUI.Box(new Rect(0f, 38f, 50f, 26f), GUIContent.none, mShowcaseSkin.GetStyle("ShieldRegen"));
		GUI.Box(new Rect(0f, 60f, 50f, 26f), GUIContent.none, mShowcaseSkin.GetStyle("Speed"));
		GUI.Box(new Rect(0f, 83f, 50f, 26f), GUIContent.none, mShowcaseSkin.GetStyle("Jetpack"));
		GUI.Box(new Rect(0f, 106f, 50f, 26f), GUIContent.none, mShowcaseSkin.GetStyle("Tech"));
		GUI.Label(new Rect(0f, 15f, 140f, 26f), new GUIContent("SHIELD", "The amount of damage you can take before you crash."), mShowcaseSkin.GetStyle("StatText"));
		GUI.Label(new Rect(0f, 38f, 140f, 26f), new GUIContent("REGEN", "How quickly your shield regenerates after it's damaged."), mShowcaseSkin.GetStyle("StatText"));
		GUI.Label(new Rect(0f, 60f, 140f, 26f), new GUIContent("SPEED", "How fast you can run and jetpack."), mShowcaseSkin.GetStyle("StatText"));
		GUI.Label(new Rect(0f, 83f, 140f, 26f), new GUIContent("JETPACK", "The amount of jetpack fuel you have."), mShowcaseSkin.GetStyle("StatText"));
		GUI.Label(new Rect(0f, 106f, 140f, 26f), new GUIContent("TECH", "How quickly you can pick up boosts and heavy weapons."), mShowcaseSkin.GetStyle("StatText"));
		DrawStatBoxes(new Rect(125f, 19f, 100f, 16f), SuitInfo.mShieldPower, new Color(1f, 0f, 0f), mShowcaseSkin.GetStyle("StatBackground"), mShowcaseSkin.GetStyle("StatBox"));
		DrawStatBoxes(new Rect(125f, 42f, 100f, 16f), SuitInfo.mShieldRegen, new Color(1f, 0.5f, 0f), mShowcaseSkin.GetStyle("StatBackground"), mShowcaseSkin.GetStyle("StatBox"));
		DrawStatBoxes(new Rect(125f, 64f, 100f, 16f), SuitInfo.mSpeed, new Color(1f, 1f, 0f), mShowcaseSkin.GetStyle("StatBackground"), mShowcaseSkin.GetStyle("StatBox"));
		DrawStatBoxes(new Rect(125f, 87f, 100f, 16f), SuitInfo.mJetpack, new Color(0f, 0.5f, 0.5f), mShowcaseSkin.GetStyle("StatBackground"), mShowcaseSkin.GetStyle("StatBox"));
		DrawStatBoxes(new Rect(125f, 110f, 100f, 16f), SuitInfo.mTech, new Color(0f, 0f, 1f), mShowcaseSkin.GetStyle("StatBackground"), mShowcaseSkin.GetStyle("StatBox"));
		GUI.EndGroup();
		GUI.color = Color.white;
		GUI.BeginGroup(mBlock4, "WEAPON MOD", "InfoPanelOverlay");
		Rect position = new Rect(3f, 17f, mBlock4.width - 6f, 47f);
		int mWeaponIdx = GameData.getWeaponModData(exosuit.mWeaponModIndex).mWeaponIdx;
		switch (mWeaponIdx)
		{
		case 1:
			GUI.Box(position, "Bulldog", mShowcaseSkin.GetStyle("WeaponModName"));
			break;
		case 2:
			GUI.Box(position, "Marksman", mShowcaseSkin.GetStyle("WeaponModName"));
			break;
		case 3:
			GUI.Box(position, "Ballista", mShowcaseSkin.GetStyle("WeaponModName"));
			break;
		case 4:
			GUI.Box(position, "Wildfire", mShowcaseSkin.GetStyle("WeaponModName"));
			break;
		case 5:
			GUI.Box(position, "Tridex", mShowcaseSkin.GetStyle("WeaponModName"));
			break;
		}
		GUI.DrawTexture(new Rect(position.x + 1f, position.y + 1f, 64f, 48f), WeaponTextures[mWeaponIdx - 1]);
		if (Event.current.type == EventType.Repaint)
		{
			mShowcaseSkin.GetStyle("WeaponModInfo").Draw(position, exosuit.mWeaponModName, false, false, exosuit.mFactionId == 2, hasKeyboardFocus: false);
		}
		GUI.Label(new Rect(2f, 63f, mBlock4.width - 4f, mBlock4.height - 70f), exosuit.mWeaponModDescription, mShowcaseSkin.label);
		GUI.EndGroup();
		GUI.EndGroup();
		GUI.skin = skin;
		GUI.EndGroup();
		if (GUI.tooltip != string.Empty)
		{
			GUIUtil.Tooltip = GUI.tooltip;
		}
	}

	private void DrawStatBoxes(Rect Position, int Value, Color color, GUIStyle Background, GUIStyle box)
	{
		GUI.BeginGroup(Position);
		for (int i = 0; i < 5; i++)
		{
			GUI.color = Color.white;
			GUI.Box(new Rect(i * 15, 0f, 12f, 16f), string.Empty, Background);
			if (Value >= i + 1)
			{
				GUI.color = color;
				GUI.Box(new Rect(i * 15 + 1, 1f, 10f, 14f), string.Empty, box);
			}
		}
		GUI.color = Color.white;
		GUI.EndGroup();
	}
}
