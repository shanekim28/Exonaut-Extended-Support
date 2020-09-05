using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TabShowcase
{
	private GUISkin mShowcaseSkin;

	private GUISkin mSharedSkin;

	public SuitInspector mSuitInspector = new SuitInspector();

	private List<SuitInspector.TempSuitInfo> mSuits = new List<SuitInspector.TempSuitInfo>();

	public int mSelectedIndex;

	public int mHoverIndex = -1;

	public float mHoverTime;

	public float mSelectTop = 70f;

	public float mSelectBottom = 98f;

	public int mNumPerColumn = 4;

	public float mIconSize;

	public float mGapSize = 15f;

	public float anim;

	private GameHome mParent;

	public Rect mInspectGroup = new Rect(5f, 158f, 460f, 392f);

	private Rect mViewToolsGroup;

	private GUIStyle whitebox;

	private bool clickRepeat;

	private float tempheight;

	private KeyCode LastKey = KeyCode.Keypad3;

	private string lastHover = string.Empty;

	public TabShowcase(GameHome Parent)
	{
		mParent = Parent;
		mSuitInspector.Init();
		mSuitInspector.mCameraOffset = (mSuitInspector.mDefaultCameraPosition = Vector3.zero);
		mSuitInspector.ModelTransforms[0] = null;
		mSuitInspector.ModelTransforms[1] = GUIUtil.mInstance.mBackground;
		mSuitInspector.ModelPositions = new Vector3[4];
		mSuitInspector.ModelPositions[0] = new Vector3(0f, -6f, 27.5f);
		mSuitInspector.ModelPositions[1] = new Vector3(0.25f, -6f, 28.5f);
		mSuitInspector.ModelPositions[2] = new Vector3(0.25f, -6f, 28.5f);
		mSuitInspector.ModelPositions[3] = new Vector3(-1.25f, 0f, 35f);
		mSuitInspector.ModelRotations = new Vector3[4];
		mSuitInspector.ModelRotations[0] = new Vector3(0f, 180f, 0f);
		mSuitInspector.ModelRotations[1] = new Vector3(0f, 180f, 0f);
		mSuitInspector.ModelRotations[2] = new Vector3(0f, 180f, 0f);
		mSuitInspector.ModelRotations[3] = new Vector3(90f, 180f, 0f);
		setStyle();
		int num = 0;
		foreach (Exosuit value in GameData.MasterSuitList.Values)
		{
			if (value.mFactionId == GameData.MyFactionId)
			{
				SuitInspector.TempSuitInfo item = new SuitInspector.TempSuitInfo
				{
					mSuitName = value.mSuitName,
					mIndex = value.mSuitId,
					mDescription = value.mDescription,
					mCost = value.mCost,
					mSuitShow = value.mShowName,
					mShieldPower = value.mBaseHealth,
					mShieldRegen = value.mBaseRegenHealth,
					mJetpack = value.mBaseJetFuel,
					mSpeed = value.mBaseSpeed,
					mTech = value.mBaseTech,
					mWeaponModName = value.mWeaponModName,
					mWeaponModDescription = value.mWeaponModDescription,
					mLevelRequirement = value.mLevelRequirement
				};
				mSuits.Add(item);
				num++;
			}
		}
		foreach (Exosuit value2 in GameData.MasterSuitList.Values)
		{
			if (value2.mFactionId == GameData.MyFactionId)
			{
				int priority = 50;
				if (value2.mSuitId == GameData.MySuitID)
				{
					priority = 100;
				}
				else if (GameData.MyOwnedSuitIDs.Contains(value2.mSuitId))
				{
					priority = 75;
				}
				if (GameData.getExosuit(value2.mSuitId).getHighPolyModel() == null)
				{
					AssetLoader.AddSuitToLoad(value2.mSuitId, AssetLoader.SuitAsset.SuitType.high, priority);
				}
			}
		}
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < mSuits.Count - 1; i++)
			{
				if (mSuits[i].mCost > mSuits[i + 1].mCost)
				{
					SwapSuits(i, i + 1);
					flag = true;
				}
				else if (mSuits[i].mCost == mSuits[i + 1].mCost && mSuits[i].mIndex > mSuits[i + 1].mIndex)
				{
					SwapSuits(i, i + 1);
					flag = true;
				}
			}
		}
		if (!GameData.MyOwnedSuitIDs.Contains(GameData.MySuitID))
		{
			GameData.AddOwnedSuit(GameData.MySuitID);
		}
		SetCurrentSelection(GetSuitIndexFromID(GameData.MySuitID));
	}

	private void SwapSuits(int index, int index2)
	{
		SuitInspector.TempSuitInfo value = mSuits[index];
		mSuits[index] = mSuits[index2];
		mSuits[index2] = value;
	}

	private void SetCurrentSelection(int index)
	{
		if ((int)anim != index / mNumPerColumn)
		{
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Suit_Chooser_Change_Column);
		}
		tempheight = 0f;
		mSelectedIndex = index;
		SuitInspector.TempSuitInfo tempSuitInfo = mSuits[mSelectedIndex];
		if (GameData.getExosuit(tempSuitInfo.mIndex).getHighPolyModel() != null)
		{
			mSuitInspector.ModelTransforms[0] = GameData.getExosuit(tempSuitInfo.mIndex).getHighPolyModel().transform;
		}
		else
		{
			mSuitInspector.ModelTransforms[0] = null;
		}
	}

	private void setStyle()
	{
		mSharedSkin = GUIUtil.mInstance.mSharedSkin;
		mShowcaseSkin = GUIUtil.mInstance.mShowcaseSkin;
		whitebox = mSharedSkin.GetStyle("whitebox");
	}

	public void UpdateScreenSpace(Rect screenSpace)
	{
		float num = 387f;
		float num2 = 153f;
		float num3 = 50f;
		mInspectGroup.y = num2 + (GameHome.ScreenSpace.height - num2 - num3 - num) / 2f;
		mViewToolsGroup = new Rect(screenSpace.width / 2f - 121.5f, screenSpace.height - 113f, 243f, 93f);
		mSuitInspector.UpdateScreenSpace(new Rect(0f, 0f, mInspectGroup.width, mInspectGroup.height));
	}

	public void showTab(Rect tabGroup)
	{
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		mIconSize = (tabGroup.height - mSelectTop - mSelectBottom - (float)(mNumPerColumn + 1) * mGapSize) / (float)mNumPerColumn;
		if (mSuitInspector.ModelTransforms[0] == null)
		{
			SuitInspector.TempSuitInfo tempSuitInfo = mSuits[mSelectedIndex];
			if (GameData.getExosuit(tempSuitInfo.mIndex).getHighPolyModel() != null)
			{
				mSuitInspector.ModelTransforms[0] = GameData.getExosuit(tempSuitInfo.mIndex).getHighPolyModel().transform;
			}
			else
			{
				AssetLoader.AddSuitToLoad(tempSuitInfo.mIndex, AssetLoader.SuitAsset.SuitType.high, 1000);
				GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 70, 400f, 40f), "Loading Suit: " + (int)(AssetLoader.GetSuitLoadProgress(tempSuitInfo.mIndex, AssetLoader.SuitAsset.SuitType.high) * 100f) + "%", mShowcaseSkin.GetStyle("SuitLoadStyle"));
				GUIUtil.DrawLoadingAnim(new Rect((Screen.width - 128) / 2, (Screen.height - 128) / 2 + 40, 128f, 128f), 1);
			}
		}
		GUI.color = Color.white;
		if (Input.GetKeyUp(KeyCode.Keypad0))
		{
			tempheight = 0f;
		}
		if (Input.GetKeyUp(KeyCode.Keypad1))
		{
			LastKey = KeyCode.Keypad1;
			tempheight = 0f;
		}
		if (Input.GetKeyUp(KeyCode.Keypad2))
		{
			LastKey = KeyCode.Keypad2;
			tempheight = 0f;
		}
		if (Input.GetKeyUp(KeyCode.Keypad3))
		{
			LastKey = KeyCode.Keypad3;
			tempheight = 0f;
		}
		tempheight += Time.deltaTime * tabGroup.height / 1f;
		tempheight = Mathf.Min(tempheight, mInspectGroup.height);
		switch (LastKey)
		{
		case KeyCode.Keypad1:
			GUI.BeginGroup(new Rect(mInspectGroup.x, mInspectGroup.y, mInspectGroup.width, tempheight));
			ShowPreview();
			GUI.EndGroup();
			break;
		case KeyCode.Keypad2:
			GUI.BeginGroup(new Rect(mInspectGroup.x, mInspectGroup.y + mInspectGroup.height / 2f - tempheight / 2f, mInspectGroup.width, tempheight));
			ShowPreview();
			GUI.EndGroup();
			break;
		case KeyCode.Keypad3:
			GUI.BeginGroup(new Rect(mInspectGroup.x, (int)(mInspectGroup.y + mInspectGroup.height / 2f - tempheight / 2f), mInspectGroup.width, tempheight));
			GUI.BeginGroup(new Rect(0f, (int)(0f - mInspectGroup.height / 2f + tempheight / 2f), mInspectGroup.width, mInspectGroup.height));
			ShowPreview();
			GUI.EndGroup();
			GUI.EndGroup();
			break;
		}
		GUI.BeginGroup(mViewToolsGroup);
		GUI.Box(new Rect(0f, 0f, 0f, 0f), GUIContent.none, mShowcaseSkin.GetStyle("ViewToolsBracket"));
		bool flag = Event.current.type != EventType.Repaint && clickRepeat;
		switch (GUIUtil.RepeatButton(new Rect(49f, mViewToolsGroup.height - 34f, 34f, 34f), GUIContent.none, mShowcaseSkin.GetStyle("RotateLeft" + GameData.MyFactionId)))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "RotateLeft";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "RotateLeft";
			mSuitInspector.ModelRotations[0].y += 75f * Time.deltaTime;
			if (mSuitInspector.ModelRotations[0].y > 360f)
			{
				mSuitInspector.ModelRotations[0].y -= 360f;
			}
			flag = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			break;
		}
		switch (GUIUtil.RepeatButton(new Rect(85f, mViewToolsGroup.height - 34f, 34f, 34f), GUIContent.none, mShowcaseSkin.GetStyle("RotateRight" + GameData.MyFactionId)))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "RotateRight";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "RotateRight";
			mSuitInspector.ModelRotations[0].y -= 75f * Time.deltaTime;
			if (mSuitInspector.ModelRotations[0].y < 0f)
			{
				mSuitInspector.ModelRotations[0].y += 360f;
			}
			flag = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			break;
		}
		switch (GUIUtil.RepeatButton(new Rect(121f, mViewToolsGroup.height - 34f, 34f, 34f), GUIContent.none, mShowcaseSkin.GetStyle("ZoomIn" + GameData.MyFactionId)))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "ZoomIn";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "ZoomIn";
			mSuitInspector.mCameraOffset.z -= Time.deltaTime * 10f;
			mSuitInspector.mCameraOffset.y -= Time.deltaTime * 3.5f;
			mSuitInspector.LimitCameraOffset();
			flag = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			break;
		}
		switch (GUIUtil.RepeatButton(new Rect(157f, mViewToolsGroup.height - 34f, 34f, 34f), GUIContent.none, mShowcaseSkin.GetStyle("ZoomOut" + GameData.MyFactionId)))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "ZoomOut";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "ZoomOut";
			mSuitInspector.mCameraOffset.z += Time.deltaTime * 10f;
			mSuitInspector.mCameraOffset.y += Time.deltaTime * 3.5f;
			mSuitInspector.LimitCameraOffset();
			flag = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			break;
		}
		clickRepeat = flag;
		SuitInspector.TempSuitInfo tempSuitInfo2 = mSuits[mSelectedIndex];
		GUI.color = Color.white;
		if (GameData.MATCH_MODE == GameData.Build.DEBUG || GameData.MyOwnedSuitIDs.Contains(tempSuitInfo2.mIndex) || Application.isEditor)
		{
			if (GameData.MySuitID != tempSuitInfo2.mIndex)
			{
				switch (GUIUtil.Button(new Rect(39f, 5f, 163f, 50f), new GUIContent("Equip Exosuit", GUIUtil.GUISoundClips.TT_Global_Button_Over + "05"), mShowcaseSkin.GetStyle("EquipButton" + GameData.MyFactionId)))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "Equip Exosuit";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "Equip Exosuit";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Equip_Press);
					GameData.MySuitID = tempSuitInfo2.mIndex;
					mParent.avatarTexture = (Resources.Load("HUD/avatar/" + GameHUD.avatar_images[GameData.MySuitID - 1]) as Texture2D);
					break;
				}
			}
			else if (GameData.MySuitID == tempSuitInfo2.mIndex)
			{
				switch (GUIUtil.Button(new Rect(39f, 5f, 163f, 50f), new GUIContent("Current Exosuit", GUIUtil.GUISoundClips.TT_Global_Button_Over + "05"), mShowcaseSkin.GetStyle("EquipButton" + GameData.MyFactionId)))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "Current Exosuit";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "Current Exosuit";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Button_Inactive);
					break;
				}
			}
		}
		else
		{
			bool bEnable = false;
			if (GameData.MyPlayStatus > 1 && tempSuitInfo2.mLevelRequirement <= GameData.MyLevel && GameData.MyTotalCredits >= tempSuitInfo2.mCost)
			{
				bEnable = true;
			}
			GUIUtil.GUIEnable(bEnable);
			switch (GUIUtil.Button(new Rect(39f, 5f, 163f, 50f), new GUIContent("Buy Exosuit", GUIUtil.GUISoundClips.TT_Global_Button_Over + "05"), mShowcaseSkin.GetStyle("EquipButton" + GameData.MyFactionId)))
			{
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "Buy Exosuit";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			case GUIUtil.GUIState.Click:
				b = "Buy Exosuit";
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Buy_Press);
				MessageBox.AddMessageCustom("Confirm Purchase", "Are you sure that you want to buy " + tempSuitInfo2.mSuitName + " for " + tempSuitInfo2.mCost + "c?", null, true, OnClickBuy, "Yes, Buy", "No, Cancel");
				break;
			}
			GUIUtil.GUIEnable(bEnable: true);
		}
		GUI.EndGroup();
		float f = 0.8f;
		GUI.matrix = Matrix4x4.identity;
		int num = Mathf.CeilToInt((float)mSuits.Count / (float)mNumPerColumn);
		float num2 = (float)(mSelectedIndex / mNumPerColumn) - anim;
		if (num2 < 0f)
		{
			num2 += (float)num;
		}
		anim += Time.deltaTime * Mathf.Max(num2, 0.25f) * 1.3f;
		if (anim > (float)num)
		{
			anim -= num;
		}
		int num3 = (int)anim;
		int num4 = num3;
		if (mSelectedIndex / mNumPerColumn == num3)
		{
			anim = num3;
		}
		num4 = num3 - 1;
		int num5 = -1;
		bool flag2 = GUIUtil.TestFlag(GUIUtil.GUIFlags.WindowUp);
		do
		{
			if (num4 < 0)
			{
				num4 = num - 1;
			}
			if (num4 >= num || num4 < 0)
			{
				break;
			}
			float num6 = (float)num4 - anim;
			if (num6 <= -1f)
			{
				num6 = (float)num + num6;
			}
			float num7 = Mathf.Pow(f, num6);
			if (num7 > 1f)
			{
				num7 *= num7;
			}
			GUI.matrix = Matrix4x4.identity * Matrix4x4.Scale(new Vector3(num7, num7, 1f));
			float num8 = (mSelectTop + mGapSize * num6) / num7;
			float num9 = (tabGroup.width - mIconSize - mGapSize) / num7;
			float num10 = Mathf.Clamp(GameHome.ScreenSpace.width / (4.25f * (float)num * GameHome.ScreenSpace.height / GameHome.ScreenSpace.width), mIconSize * 0.4f, mIconSize * 0.75f);
			float num11 = num6;
			while ((num11 -= 1f) >= 0f)
			{
				num9 -= num10 / Mathf.Pow(f, num11);
				num8 += -10f / Mathf.Pow(f, num11);
			}
			float num12 = num6 - (float)Mathf.FloorToInt(num6);
			if (num12 > 0f)
			{
				num9 -= num12 * num10;
				num8 += num12 * -10f;
			}
			for (int i = 0; i < Mathf.Min(mNumPerColumn, mSuits.Count - num4 * mNumPerColumn); i++)
			{
				int num13 = num4 * mNumPerColumn + i;
				SuitInspector.TempSuitInfo tempSuitInfo3 = mSuits[num13];
				bool flag3 = GameData.MyOwnedSuitIDs.Contains(tempSuitInfo3.mIndex);
				string text = "ChooserLockedIcon";
				if (flag3)
				{
					text = "ChooserOwnedIcon";
				}
				else if (tempSuitInfo3.mLevelRequirement <= GameData.MyLevel)
				{
					text = "ChooserBuyableIcon";
				}
				if (num7 > 1f)
				{
					GUI.color = new Color(1f, 1f, 1f, 4.05f - Mathf.Sqrt(num7 * 13f));
					if (GameData.MySuitID == tempSuitInfo3.mIndex)
					{
						text += "Hover";
					}
					Rect position = new Rect(num9 + mIconSize * 0.65f / num7, num8 + (float)i * (mIconSize + mGapSize), mIconSize, mIconSize);
					if ((bool)GameData.getExosuit(tempSuitInfo3.mIndex).mGuiLoadoutImage)
					{
						GUI.DrawTexture(position, GameData.getExosuit(tempSuitInfo3.mIndex).mGuiLoadoutImage);
					}
					GUI.Box(position, GUIContent.none, mShowcaseSkin.GetStyle(text));
					continue;
				}
				Rect position2 = new Rect(num9, num8 + (float)i * (mIconSize + mGapSize), mIconSize, mIconSize);
				if (position2.Contains(Event.current.mousePosition) && !flag2)
				{
					num5 = num13;
					if (Event.current.type == EventType.MouseUp)
					{
						SetCurrentSelection(num13);
					}
				}
				if (mHoverIndex == num13)
				{
					float num14 = (0.05f + Mathf.Lerp(0f, 0.1f, Mathf.Abs(Mathf.Sin(mHoverTime * 2f)) / (float)Math.PI)) * position2.width;
					position2.x -= num14;
					position2.y -= num14;
					position2.width += num14 + num14;
					position2.height += num14 + num14;
					text += "Hover";
				}
				float a = Mathf.Lerp(1f, 0.1f, Mathf.Clamp((num6 / (float)num - 0.2f) * 2f, 0f, 1f));
				if (Event.current.type == EventType.Repaint)
				{
					Texture2D texture2D = Resources.Load("SuitChooser/ex_" + GameData.getExosuit(tempSuitInfo3.mIndex).mSuitFileName) as Texture2D;
					if (texture2D != null)
					{
						if (flag2)
						{
							GUI.DrawTexture(position2, whitebox.normal.background);
							GUI.color = new Color(1f, 1f, 1f, 0.6f);
							mHoverIndex = -1;
						}
						else if (mHoverIndex != num13)
						{
							GUI.color = new Color(1f, 1f, 1f, a);
						}
						GUI.DrawTexture(position2, texture2D);
						GUI.color = Color.white;
					}
					GUI.Box(position2, GUIContent.none, mShowcaseSkin.GetStyle(text));
				}
				if (mHoverIndex != num13)
				{
					GUI.color = new Color(1f, 1f, 1f, a);
				}
				if (!flag3 && tempSuitInfo3.mLevelRequirement > GameData.MyLevel)
				{
					Rect position3 = new Rect(position2.x + position2.width - 42f - 3f, position2.y + position2.height - 42f - 3f, 42f, 42f);
					GUIStyle style = mShowcaseSkin.GetStyle("ChooserLockTag");
					GUI.Box(position3, tempSuitInfo3.mLevelRequirement.ToString(), style);
				}
				else if (!flag3)
				{
					Rect position4 = new Rect(position2.x + 3f, position2.y + position2.height - 30f, position2.width - 6f, 27f);
					GUIStyle style2 = mShowcaseSkin.GetStyle("ChooserBuyTag");
					GUI.Box(position4, GameData.getExosuit(tempSuitInfo3.mIndex).mCost.ToString(), style2);
					GUI.Box(new Rect(position4.x + 5f, position4.y + (position4.height - 23f) / 2f, 23f, 23f), GUIContent.none, mSharedSkin.GetStyle("CreditSymbol"));
				}
				GUI.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.75f, Mathf.Abs(Mathf.Sin(num6 / (float)(num - 1) * (float)Math.PI))));
				GUI.Box(position2, GUIContent.none, whitebox);
				GUI.color = Color.white;
			}
		}
		while (num4-- != num3);
		if (Event.current.type == EventType.Repaint)
		{
			if (flag2)
			{
				mHoverIndex = -1;
			}
			else if (mHoverIndex != num5)
			{
				mHoverIndex = num5;
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Suit_Chooser_Over);
			}
		}
		GUI.matrix = Matrix4x4.identity;
		if (mHoverIndex == -1)
		{
			mHoverTime = 0f;
		}
		else
		{
			GUIUtil.Tooltip = mSuits[mHoverIndex].mSuitName;
			mHoverTime += Time.deltaTime;
		}
		lastHover = b;
	}

	public void EquipSelected(MessageBox.ReturnType Msg)
	{
		if (Msg == MessageBox.ReturnType.MB_YES)
		{
			SuitInspector.TempSuitInfo tempSuitInfo = mSuits[mSelectedIndex];
			GameData.MySuitID = tempSuitInfo.mIndex;
			mParent.avatarTexture = (Resources.Load("HUD/avatar/" + GameHUD.avatar_images[GameData.MySuitID - 1]) as Texture2D);
		}
	}

	private void ShowPreview()
	{
		SuitInspector.TempSuitInfo suitInfo = mSuits[mSelectedIndex];
		mSuitInspector.DrawSuitInfo(suitInfo, bDraw3D: false);
		GUI.color = Color.white;
	}

	private void OnClickBuy(MessageBox.ReturnType Return)
	{
		if (Return == MessageBox.ReturnType.MB_YES)
		{
			GameObject gameObject = GameObject.Find("homeGUI");
			GameHome gameHome = gameObject.GetComponent("GameHome") as GameHome;
			gameHome.executeSuitPurchase(mSuits[mSelectedIndex].mIndex);
		}
	}

	private int GetSuitIndexFromID(int ID)
	{
		for (int i = 0; i < mSuits.Count; i++)
		{
			if (mSuits[i].mIndex == ID)
			{
				return i;
			}
		}
		return 0;
	}
}
