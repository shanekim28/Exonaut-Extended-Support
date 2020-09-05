using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class FactionSelection : MonoBehaviour
{
	public bool bTexturesLoaded;

	private static WWW mTextureBundles;

	public GUISkin SharedHudSkin;

	public string[] mFactionName = new string[2]
	{
		"Banzai Squad",
		"Atlas Brigade"
	};

	public Rect screenSpace = new Rect(0f, 0f, 900f, 600f);

	public SuitInspector mSuitInspector;

	public Texture mBackground;

	public Texture2D ChooseYourFaction;

	public Texture2D ChooseYourExosuit;

	private float fIgnoreClick;

	public int mFactionSelected = -1;

	public int mFactionHovered = -1;

	public float mCurrentScroll;

	public int mSuitSelected = -1;

	public float mFadeLeft;

	public int[] mBanzaiSuits;

	public int[] mAtlasSuits;

	public GameObject GameMusic;

	public GameObject BanzaiMusic;

	public GameObject AtlasMusic;

	private bool bLoading;

	public bool[] DrawBanzaiOutlines = new bool[3];

	public bool[] DrawAtlasOutlines = new bool[3];

	public Rect[] BanzaiSuitRects = new Rect[3];

	public Rect[] AtlasSuitRects = new Rect[3];

	public Rect[] BanzaiSuitOutlineRects = new Rect[3];

	public Rect[] AtlasSuitOutlineRects = new Rect[3];

	public Texture2D[] BanzaiSuitTextures = new Texture2D[3];

	public Texture2D[] AtlasSuitTextures = new Texture2D[3];

	public Texture2D[] BanzaiSuitOutlineTextures = new Texture2D[3];

	public Texture2D[] AtlasSuitOutlineTextures = new Texture2D[3];

	public Texture2D[] BanzaiSuitIcons = new Texture2D[3];

	public Texture2D[] AtlasSuitIcons = new Texture2D[3];

	public Texture2D[] BanzaiSuitText = new Texture2D[3];

	public Texture2D[] AtlasSuitText = new Texture2D[3];

	public Rect[] BanzaiSuitIconRect = new Rect[3];

	public Rect[] AtlasSuitIconRect = new Rect[3];

	public Texture2D[] FactionTextBG = new Texture2D[2];

	public Color OutlineColor = Color.black;

	public float BlackFade;

	public Rect[] FactionButtons = new Rect[2];

	public Rect[] BanzaiButtons = new Rect[3];

	public Rect[] AtlasButtons = new Rect[3];

	public float[] mNameTimer = new float[3];

	private float fMiddle = 0.3f;

	private float fObjectOffsetX = 1f;

	private bool bFullScreen;

	public float AnimDelay;

	private float ScrollFactor;

	private string lastHover = string.Empty;

	public int StartAnim;

	private bool bViewFaction;

	public Rect ScreenSpace {
		get {
			return screenSpace;
		}
		private set {
			screenSpace = value;
		}
	}

	public static void DownloadTextureBundles()
	{
		if (mTextureBundles == null)
		{
			mTextureBundles = new WWW(GameData.BUNDLE_PATH + "textures/FactionSelectionTextures.unity3d");
		}
	}

	private void Start()
	{
		DownloadTextureBundles();
		GameData.mGameSettings.mMusicVolume = 0f;
		mCurrentScroll = fMiddle;
		if (GameObject.Find("BanzaiMusic(Clone)") == null)
		{
			BanzaiMusic = (UnityEngine.Object.Instantiate(BanzaiMusic) as GameObject);
		}
		else
		{
			BanzaiMusic = GameObject.Find("BanzaiMusic(Clone)");
		}
		if (GameObject.Find("AtlasMusic(Clone)") == null)
		{
			AtlasMusic = (UnityEngine.Object.Instantiate(AtlasMusic) as GameObject);
		}
		else
		{
			AtlasMusic = GameObject.Find("AtlasMusic(Clone)");
		}
		if (GameObject.Find("GameMusic(Clone)") == null)
		{
			GameMusic = (UnityEngine.Object.Instantiate(GameMusic) as GameObject);
			GameMusic.GetComponent<AudioSource>().volume = 1f * GameData.mGameSettings.mMusicVolume;
		}
		else
		{
			GameMusic = GameObject.Find("GameMusic(Clone)");
			GameMusic.GetComponent<AudioSource>().volume = 1f * GameData.mGameSettings.mMusicVolume;
		}
		if (GameData.MasterSuitList.Count == 0)
		{
			GameData.InitSuitList(string.Empty);
		}
		mSuitInspector = new SuitInspector();
		mSuitInspector.Init();
		mSuitInspector.mSharedHudSkin = GUIUtil.mInstance.mSharedSkin;
		mSuitInspector.mShowcaseSkin = GUIUtil.mInstance.mShowcaseSkin;
		mSuitInspector.mCameraOffset = (mSuitInspector.mDefaultCameraPosition = Vector3.zero);
		mSuitInspector.ModelTransforms[0] = null;
		mSuitInspector.ModelTransforms[1] = GUIUtil.mInstance.mBackground;
		mSuitInspector.ModelPositions[0] = new Vector3(4f, -5f, 23.5f);
		mSuitInspector.ModelPositions[1] = new Vector3(4.15f, -5f, 24.5f);
		StartCoroutine(UpdateScreenSpace());
		if (BanzaiMusic.GetComponent("SoundObject") != null)
		{
			(BanzaiMusic.GetComponent("SoundObject") as SoundObject).enabled = false;
		}
		if (AtlasMusic.GetComponent("SoundObject") != null)
		{
			(AtlasMusic.GetComponent("SoundObject") as SoundObject).enabled = false;
		}
	}

	public IEnumerator UpdateScreenSpace()
	{
		yield return new WaitForEndOfFrame();
		screenSpace = new Rect(0f, 0f, Screen.width, Screen.height);
		mSuitInspector.UpdateScreenSpace(new Rect(150f, 60f, screenSpace.width - 300f, screenSpace.height - 120f));
		bFullScreen = Screen.fullScreen;
		fObjectOffsetX = screenSpace.height / 600f;
		ScrollFactor = (screenSpace.height * 4.5f - screenSpace.width) / 8f;
	}

	public void LoadTextures()
	{
		AssetBundle assetBundle = mTextureBundles.assetBundle;
		mBanzaiSuits = (int[])GameData.BanzaiDefaultSuits.Clone();
		mAtlasSuits = (int[])GameData.AtlasDefaultSuits.Clone();
		mAtlasSuits[0] = GameData.AtlasDefaultSuits[2];
		mAtlasSuits[2] = GameData.AtlasDefaultSuits[0];
		int[] atlasDefaultSuits = GameData.AtlasDefaultSuits;
		foreach (int suitId in atlasDefaultSuits)
		{
			AssetLoader.AddSuitToLoad(suitId, AssetLoader.SuitAsset.SuitType.high, 100);
		}
		int[] banzaiDefaultSuits = GameData.BanzaiDefaultSuits;
		foreach (int suitId2 in banzaiDefaultSuits)
		{
			AssetLoader.AddSuitToLoad(suitId2, AssetLoader.SuitAsset.SuitType.high, 100);
		}
		BanzaiSuitTextures = new Texture2D[mBanzaiSuits.Length];
		BanzaiSuitOutlineTextures = new Texture2D[mBanzaiSuits.Length];
		BanzaiSuitText = new Texture2D[mBanzaiSuits.Length];
		BanzaiSuitIcons = new Texture2D[mBanzaiSuits.Length];
		for (int k = 0; k < mBanzaiSuits.Length; k++)
		{
			string text = GameData.getExosuit(mBanzaiSuits[k]).mSuitFileName.ToLower();
			BanzaiSuitTextures[k] = (assetBundle.LoadAsset(text) as Texture2D);
			BanzaiSuitOutlineTextures[k] = (assetBundle.LoadAsset(text + "_over") as Texture2D);
			BanzaiSuitText[k] = (assetBundle.LoadAsset(text + "_text") as Texture2D);
			BanzaiSuitIcons[k] = (assetBundle.LoadAsset(text + "_icon") as Texture2D);
		}
		AtlasSuitTextures = new Texture2D[mAtlasSuits.Length];
		AtlasSuitOutlineTextures = new Texture2D[mAtlasSuits.Length];
		AtlasSuitText = new Texture2D[mAtlasSuits.Length];
		AtlasSuitIcons = new Texture2D[mAtlasSuits.Length];
		for (int l = 0; l < mAtlasSuits.Length; l++)
		{
			string text2 = GameData.getExosuit(mAtlasSuits[l]).mSuitFileName.ToLower();
			AtlasSuitTextures[l] = (assetBundle.LoadAsset(text2) as Texture2D);
			AtlasSuitOutlineTextures[l] = (assetBundle.LoadAsset(text2 + "_over") as Texture2D);
			AtlasSuitText[l] = (assetBundle.LoadAsset(text2 + "_text") as Texture2D);
			AtlasSuitIcons[l] = (assetBundle.LoadAsset(text2 + "_icon") as Texture2D);
		}
		mBackground = (assetBundle.LoadAsset("Background") as Texture2D);
		FactionTextBG = new Texture2D[2];
		FactionTextBG[0] = (assetBundle.LoadAsset("BanzaiSlidingBar") as Texture2D);
		FactionTextBG[1] = (assetBundle.LoadAsset("AtlasSlidingBar") as Texture2D);
		ChooseYourExosuit = (assetBundle.LoadAsset("ChooseYourExosuit") as Texture2D);
		ChooseYourFaction = (assetBundle.LoadAsset("ChooseYourFaction") as Texture2D);
		for (int m = 0; m < AtlasSuitIconRect.Length; m++)
		{
			AtlasSuitIconRect[m].height = AtlasSuitIcons[m].height;
			AtlasSuitIconRect[m].width = AtlasSuitIcons[m].width;
		}
		for (int n = 0; n < BanzaiSuitIconRect.Length; n++)
		{
			BanzaiSuitIconRect[n].height = BanzaiSuitIcons[n].height;
			BanzaiSuitIconRect[n].width = BanzaiSuitIcons[n].width;
		}
		for (int num = 0; num < BanzaiSuitOutlineRects.Length; num++)
		{
			BanzaiSuitOutlineRects[num] = new Rect(BanzaiSuitRects[num].x - 3f, BanzaiSuitRects[num].y - 3f, BanzaiSuitOutlineTextures[num].width, BanzaiSuitOutlineTextures[num].height);
			AtlasSuitOutlineRects[num] = new Rect(AtlasSuitRects[num].x - 3f, AtlasSuitRects[num].y - 3f, AtlasSuitOutlineTextures[num].width, AtlasSuitOutlineTextures[num].height);
		}
		bTexturesLoaded = true;
	}

	private void Update()
	{
		if (bFullScreen != Screen.fullScreen || screenSpace.width != (float)Screen.width || screenSpace.height != (float)Screen.height)
		{
			StartCoroutine(UpdateScreenSpace());
		}
		if (!mTextureBundles.isDone || !(mTextureBundles.assetBundle != null))
		{
			return;
		}
		if (!bTexturesLoaded)
		{
			LoadTextures();
			return;
		}
		if (StartAnim != -1)
		{
			if (AnimDelay > 0f)
			{
				AnimDelay -= Time.deltaTime;
				if (AnimDelay <= 0f)
				{
					StartAnim++;
				}
				return;
			}
			switch (StartAnim)
			{
			case 0:
				BlackFade = 0f;
				mCurrentScroll = 4f;
				AnimDelay = 0.5f;
				break;
			case 1:
				mCurrentScroll += -0.45f * Time.deltaTime;
				BlackFade = Mathf.Min(4f - mCurrentScroll, mCurrentScroll - 1.75f) * 2f;
				if (mCurrentScroll <= 1.75f)
				{
					AnimDelay = 0.5f;
					mCurrentScroll = -4f;
					AnimDelay = 0.5f;
				}
				break;
			case 2:
				BlackFade = Mathf.Min(4f + mCurrentScroll, 0f - mCurrentScroll - 1.45f) * 2f;
				mCurrentScroll += 0.51f * Time.deltaTime;
				if (mCurrentScroll >= -1.45f)
				{
					BlackFade = 0f;
					mCurrentScroll = fMiddle;
					AnimDelay = 0.5f;
				}
				break;
			case 3:
				BlackFade += Time.deltaTime / 2f;
				if (BlackFade >= 1f)
				{
					StartAnim = -1;
				}
				break;
			}
			return;
		}
		if (mFactionSelected != -1 && mFadeLeft == 0f)
		{
			bool[] array = (mFactionSelected != 0) ? DrawAtlasOutlines : DrawBanzaiOutlines;
			for (int i = 0; i < mNameTimer.Length; i++)
			{
				if (array[i])
				{
					mNameTimer[i] = Mathf.Max(mNameTimer[i] - Time.deltaTime * 4f, 0f);
				}
				else
				{
					mNameTimer[i] = Mathf.Min(mNameTimer[i] + Time.deltaTime * 4f, 1f);
				}
			}
		}
		if (fIgnoreClick > 0f)
		{
			fIgnoreClick -= Time.deltaTime;
		}
		if (mFadeLeft > 0f)
		{
			mFadeLeft = Mathf.Max(mFadeLeft - Time.deltaTime, 0f);
			if (mFactionSelected == -1)
			{
				GameMusic.GetComponent<AudioSource>().volume = (1f - mFadeLeft) * GameData.mGameSettings.mMusicVolume;
				AtlasMusic.GetComponent<AudioSource>().volume = mFadeLeft * GameData.mGameSettings.mMusicVolume;
				BanzaiMusic.GetComponent<AudioSource>().volume = mFadeLeft * GameData.mGameSettings.mMusicVolume;
			}
			else if (mFactionSelected == 0)
			{
				BanzaiMusic.GetComponent<AudioSource>().volume = (1f - mFadeLeft) * GameData.mGameSettings.mMusicVolume;
				GameMusic.GetComponent<AudioSource>().volume = mFadeLeft * GameData.mGameSettings.mMusicVolume;
			}
			else
			{
				AtlasMusic.GetComponent<AudioSource>().volume = (1f - mFadeLeft) * GameData.mGameSettings.mMusicVolume;
				GameMusic.GetComponent<AudioSource>().volume = mFadeLeft * GameData.mGameSettings.mMusicVolume;
			}
		}
	}

	private void OnGUI()
	{
		GUIUtil.GUIEnable(bEnable: true);
		GUI.skin = SharedHudSkin;
		if ((mTextureBundles.isDone && mTextureBundles.assetBundle == null) || mTextureBundles.error != null)
		{
			GUI.Box(screenSpace, GUIContent.none, "blackbox");
			GUI.Label(new Rect(0f, screenSpace.height / 2f, screenSpace.width, 150f), "Loading Assets Error.", "MissionHeader");
			Logger.traceError(mTextureBundles.error);
			return;
		}
		GUI.Box(screenSpace, GUIContent.none, "blackbox");
		GUI.Label(new Rect(0f, screenSpace.height / 2f, screenSpace.width, 30f), "Loading Assets: " + mTextureBundles.progress * 100f + "%", GUIUtil.mInstance.mShowcaseSkin.GetStyle("SuitLoadStyle"));
		if (mTextureBundles.isDone && !bTexturesLoaded)
		{
			return;
		}
		GUI.BeginGroup(screenSpace);
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		if (bLoading)
		{
			MessageBox.Local("Loading...", "Please wait.", null, false, MessageBox.MessageType.MB_NoButtons);
		}
		Vector2 mousePosition = Event.current.mousePosition;
		float num = 0f;
		if (StartAnim == -1)
		{
			if (mFactionSelected == -1)
			{
				num = fMiddle;
			}
			else if (mFactionSelected == 0)
			{
				if (bViewFaction)
				{
					Vector2 mousePosition2 = Event.current.mousePosition;
					num = Mathf.Clamp((0f - mousePosition2.x) / (float)Screen.width * 3.25f + 4.55f, 1.75f, 4f);
				}
				else
				{
					num = 1.75f;
				}
			}
			else if (mFactionSelected == 1)
			{
				if (bViewFaction)
				{
					Vector2 mousePosition3 = Event.current.mousePosition;
					num = Mathf.Clamp((0f - mousePosition3.x) / (float)Screen.width * 3.55f - 1.25f, -4f, -1.45f);
				}
				else
				{
					num = -1.45f;
				}
			}
			mCurrentScroll += (num - mCurrentScroll) * Time.deltaTime;
		}
		num = mCurrentScroll;
		float num2 = num * ScrollFactor;
		float num3 = screenSpace.width / 2f + num2;
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(((0f - screenSpace.height) * 4.5f + screenSpace.width) / 2f + num2, 0f, screenSpace.height * 4.5f, screenSpace.height), mBackground);
		if (StartAnim != -1)
		{
			if (StartAnim == 3)
			{
				GUI.DrawTexture(new Rect(0f, 0f, screenSpace.width, 23f), ChooseYourFaction);
			}
			GUI.color = new Color(1f, 1f, 1f, 1f - BlackFade);
			GUI.Box(screenSpace, GUIContent.none, "blackbox");
			GUI.color = Color.white;
			GUI.EndGroup();
			return;
		}
		GUI.color = OutlineColor;
		for (int i = 0; i < DrawBanzaiOutlines.Length; i++)
		{
			if (DrawBanzaiOutlines[i])
			{
				GUI.DrawTexture(new Rect(num3 + fObjectOffsetX * BanzaiSuitOutlineRects[i].x, fObjectOffsetX * BanzaiSuitOutlineRects[i].y, fObjectOffsetX * (float)BanzaiSuitOutlineTextures[i].width, fObjectOffsetX * (float)BanzaiSuitOutlineTextures[i].height), BanzaiSuitOutlineTextures[i]);
			}
		}
		GUI.color = Color.white;
		for (int j = 0; j < DrawBanzaiOutlines.Length; j++)
		{
			if (!DrawBanzaiOutlines[j])
			{
				continue;
			}
			GUI.DrawTexture(new Rect(num3 + fObjectOffsetX * BanzaiSuitRects[j].x, fObjectOffsetX * BanzaiSuitRects[j].y, fObjectOffsetX * (float)BanzaiSuitTextures[j].width, fObjectOffsetX * (float)BanzaiSuitTextures[j].height), BanzaiSuitTextures[j]);
			if (Event.current.type != EventType.MouseUp || !(fIgnoreClick <= 0f))
			{
				continue;
			}
			if (mFactionSelected == -1)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Suit_Chooser_Change_Column);
				mFadeLeft = 1f;
				mFactionSelected = 0;
				mSuitSelected = -1;
				for (int k = 0; k < mNameTimer.Length; k++)
				{
					mNameTimer[k] = 1f;
				}
				break;
			}
			if (mFactionSelected == 0 && mSuitSelected == -1)
			{
				mSuitSelected = mBanzaiSuits[j];
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Equip_Press);
			}
		}
		GUI.color = OutlineColor;
		for (int l = 0; l < DrawAtlasOutlines.Length; l++)
		{
			if (DrawAtlasOutlines[l])
			{
				GUI.DrawTexture(new Rect(num3 + fObjectOffsetX * AtlasSuitOutlineRects[l].x, fObjectOffsetX * AtlasSuitOutlineRects[l].y, fObjectOffsetX * (float)AtlasSuitOutlineTextures[l].width, fObjectOffsetX * (float)AtlasSuitOutlineTextures[l].height), AtlasSuitOutlineTextures[l]);
			}
		}
		GUI.color = Color.white;
		for (int m = 0; m < DrawAtlasOutlines.Length; m++)
		{
			if (!DrawAtlasOutlines[m])
			{
				continue;
			}
			GUI.DrawTexture(new Rect(num3 + fObjectOffsetX * AtlasSuitRects[m].x, fObjectOffsetX * AtlasSuitRects[m].y, fObjectOffsetX * (float)AtlasSuitTextures[m].width, fObjectOffsetX * (float)AtlasSuitTextures[m].height), AtlasSuitTextures[m]);
			if (Event.current.type != EventType.MouseUp || !(fIgnoreClick <= 0f))
			{
				continue;
			}
			if (mFactionSelected == -1)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Suit_Chooser_Change_Column);
				mFadeLeft = 1f;
				mFactionSelected = 1;
				mSuitSelected = -1;
				for (int n = 0; n < mNameTimer.Length; n++)
				{
					mNameTimer[n] = 1f;
				}
				break;
			}
			if (mFactionSelected == 1 && mSuitSelected == -1)
			{
				mSuitSelected = mAtlasSuits[m];
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Equip_Press);
			}
		}
		if (mFactionSelected == -1)
		{
			GUI.DrawTexture(new Rect(0f, 0f, screenSpace.width, 23f), ChooseYourFaction);
			int num4 = -1;
			for (int num5 = 0; num5 < FactionButtons.Length; num5++)
			{
				Rect rect = FactionButtons[num5];
				rect.x = num3 + fObjectOffsetX * rect.x;
				rect.y *= fObjectOffsetX;
				rect.height *= fObjectOffsetX;
				rect.width *= fObjectOffsetX;
				if (rect.Contains(mousePosition))
				{
					num4 = num5;
				}
			}
			if (num4 != -1 && mFactionHovered != num4)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Suit_Chooser_Over);
			}
			mFactionHovered = num4;
			for (int num6 = 0; num6 < DrawBanzaiOutlines.Length; num6++)
			{
				DrawBanzaiOutlines[num6] = (mFactionHovered == 0);
			}
			for (int num7 = 0; num7 < DrawAtlasOutlines.Length; num7++)
			{
				DrawAtlasOutlines[num7] = (mFactionHovered == 1);
			}
		}
		else
		{
			if (!bViewFaction)
			{
				GUI.DrawTexture(new Rect(0f, 0f, screenSpace.width, 23f), ChooseYourExosuit);
			}
			Rect[] array = null;
			bool[] array2 = null;
			if (mFactionSelected == 0)
			{
				if (!bViewFaction)
				{
					for (int num8 = 0; num8 < BanzaiSuitIcons.Length; num8++)
					{
						Rect position = BanzaiSuitIconRect[num8];
						position.x *= fObjectOffsetX;
						position.y *= fObjectOffsetX;
						position.width *= fObjectOffsetX;
						position.height *= fObjectOffsetX;
						position.x += num3 + (float)(int)(position.width * mFadeLeft);
						position.width = (int)(position.width * (1f - mFadeLeft));
						GUI.BeginGroup(position);
						Rect position2 = new Rect((0f - BanzaiSuitIconRect[num8].width) * fObjectOffsetX * mFadeLeft, 0f, (float)BanzaiSuitIcons[num8].width * fObjectOffsetX, (float)BanzaiSuitIcons[num8].height * fObjectOffsetX);
						GUI.DrawTexture(position2, BanzaiSuitIcons[num8]);
						GUI.EndGroup();
						Rect position3 = new Rect(position.x, position.y, (int)((float)FactionTextBG[0].width * fObjectOffsetX * (1f - mNameTimer[num8])), (float)FactionTextBG[0].height * fObjectOffsetX);
						position3.x -= position3.width;
						GUI.BeginGroup(position3);
						GUI.DrawTexture(new Rect((int)((float)(-FactionTextBG[0].width) * fObjectOffsetX * mNameTimer[num8]), 0f, (float)FactionTextBG[0].width * fObjectOffsetX, (float)FactionTextBG[0].height * fObjectOffsetX), FactionTextBG[0]);
						GUI.DrawTexture(new Rect((float)(int)((float)(-FactionTextBG[0].width) * fObjectOffsetX * mNameTimer[num8]) + ((float)FactionTextBG[0].width * fObjectOffsetX - (float)BanzaiSuitText[num8].width * fObjectOffsetX) / 2f, 10f, (float)BanzaiSuitText[num8].width * fObjectOffsetX, (float)BanzaiSuitText[num8].height * fObjectOffsetX), BanzaiSuitText[num8]);
						GUI.EndGroup();
					}
					switch (GUIUtil.Button(new Rect(10f, 10f, 80f, 38f), "BACK", "ModalButton"))
					{
					case GUIUtil.GUIState.Hover:
					case GUIUtil.GUIState.Active:
						if (Event.current.type == EventType.Repaint)
						{
							b = "BACK";
							if (lastHover != b)
							{
								GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
							}
						}
						break;
					case GUIUtil.GUIState.Click:
						b = "BACK";
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
						mFactionSelected = -1;
						mSuitSelected = -1;
						mFadeLeft = 1f;
						GUI.EndGroup();
						return;
					}
				}
				array = BanzaiButtons;
				array2 = DrawBanzaiOutlines;
				switch (GUIUtil.Button(new Rect(10f, screenSpace.height - 48f, 120f, 38f), (!bViewFaction) ? "VIEW FACTION" : "BACK TO SUITS", "ModalButton"))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "BACK TO SUITS";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "BACK TO SUITS";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					bViewFaction = !bViewFaction;
					mSuitSelected = -1;
					GUI.EndGroup();
					return;
				}
			}
			else if (mFactionSelected == 1)
			{
				if (!bViewFaction)
				{
					for (int num9 = 0; num9 < AtlasSuitIcons.Length; num9++)
					{
						Rect position4 = AtlasSuitIconRect[num9];
						position4.x *= fObjectOffsetX;
						position4.y *= fObjectOffsetX;
						position4.width *= fObjectOffsetX;
						position4.height *= fObjectOffsetX;
						position4.x += num3;
						position4.width *= 1f - mFadeLeft;
						GUI.BeginGroup(position4);
						GUI.DrawTexture(new Rect(0f, 0f, (float)AtlasSuitIcons[num9].width * fObjectOffsetX, (float)AtlasSuitIcons[num9].height * fObjectOffsetX), AtlasSuitIcons[num9]);
						GUI.EndGroup();
						Rect position5 = new Rect(position4.x + position4.width, position4.y, (float)FactionTextBG[1].width * fObjectOffsetX * (1f - mNameTimer[num9]), (float)FactionTextBG[1].height * fObjectOffsetX);
						GUI.BeginGroup(position5);
						GUI.DrawTexture(new Rect(0f, 0f, (float)FactionTextBG[1].width * fObjectOffsetX, (float)FactionTextBG[1].height * fObjectOffsetX), FactionTextBG[1]);
						GUI.DrawTexture(new Rect(((float)FactionTextBG[1].width * fObjectOffsetX - (float)AtlasSuitText[num9].width * fObjectOffsetX) / 2f, 10f, (float)AtlasSuitText[num9].width * fObjectOffsetX, (float)AtlasSuitText[num9].height * fObjectOffsetX), AtlasSuitText[num9]);
						GUI.EndGroup();
					}
					switch (GUIUtil.Button(new Rect(screenSpace.width - 90f, 10f, 80f, 38f), "BACK", "ModalButton"))
					{
					case GUIUtil.GUIState.Hover:
					case GUIUtil.GUIState.Active:
						if (Event.current.type == EventType.Repaint)
						{
							b = "BACK";
							if (lastHover != b)
							{
								GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
							}
						}
						break;
					case GUIUtil.GUIState.Click:
						b = "BACK";
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
						mFactionSelected = -1;
						mSuitSelected = -1;
						mFadeLeft = 1f;
						GUI.EndGroup();
						return;
					}
				}
				array = AtlasButtons;
				array2 = DrawAtlasOutlines;
				switch (GUIUtil.Button(new Rect(screenSpace.width - 130f, screenSpace.height - 48f, 120f, 38f), (!bViewFaction) ? "VIEW FACTION" : "BACK TO SUITS", "ModalButton"))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "BACK TO SUITS";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "BACK TO SUITS";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					bViewFaction = !bViewFaction;
					mSuitSelected = -1;
					GUI.EndGroup();
					return;
				}
			}
			if (mSuitSelected == -1 && !bViewFaction)
			{
				for (int num10 = 0; num10 < array.Length; num10++)
				{
					Rect rect2 = array[num10];
					rect2.x *= fObjectOffsetX;
					rect2.y *= fObjectOffsetX;
					rect2.width *= fObjectOffsetX;
					rect2.height *= fObjectOffsetX;
					rect2.x += num2;
					if (rect2.Contains(mousePosition))
					{
						if (!array2[num10])
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Suit_Chooser_Over);
						}
						array2[num10] = true;
					}
					else
					{
						array2[num10] = false;
					}
				}
			}
			SuitInspector.TempSuitInfo tempSuitInfo = new SuitInspector.TempSuitInfo();
			GUIUtil.GUIEnable(bEnable: true);
			if (mSuitSelected != -1)
			{
				tempSuitInfo.mSuitName = GameData.getExosuit(mSuitSelected).mSuitName;
				Exosuit exosuit = GameData.getExosuit(mSuitSelected);
				tempSuitInfo.mSuitShow = exosuit.mShowName;
				tempSuitInfo.mSuitName = exosuit.mSuitName;
				tempSuitInfo.mIndex = mSuitSelected;
				tempSuitInfo.mDescription = exosuit.mDescription;
				tempSuitInfo.mShieldPower = exosuit.mBaseHealth;
				tempSuitInfo.mShieldRegen = exosuit.mBaseRegenHealth;
				tempSuitInfo.mJetpack = exosuit.mBaseJetFuel;
				tempSuitInfo.mSpeed = exosuit.mBaseSpeed;
				tempSuitInfo.mTech = exosuit.mBaseTech;
				Rect position6 = new Rect(150f, 60f, screenSpace.width - 300f, screenSpace.height - 120f);
				GUI.BeginGroup(position6);
				mSuitInspector.DrawSuitInfo(tempSuitInfo, bDraw3D: true);
				float num11 = 230f + (position6.width - 230f) / 2f;
				if (GameData.getExosuit(mSuitSelected).getHighPolyModel() == null)
				{
					GUI.Label(new Rect(num11 - 200f, position6.height / 2f - 90f, 400f, 40f), "Loading Suit: " + (int)(AssetLoader.GetSuitLoadProgress(tempSuitInfo.mIndex, AssetLoader.SuitAsset.SuitType.high) * 100f) + "%", GUIUtil.mInstance.mShowcaseSkin.GetStyle("SuitLoadStyle"));
					GUIUtil.DrawLoadingAnim(new Rect(num11 - 64f, (Screen.height - 128) / 2, 128f, 128f), 1);
				}
				GUI.color = Color.white;
				switch (GUIUtil.Button(new Rect(num11 - 70f, position6.height - 45f, 140f, 34f), "CHOOSE EXOSUIT", "ModalButton"))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "CHOOSE EXOSUIT";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "CHOOSE EXOSUIT";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					MessageBox.ResetWindowPosition();
					MessageBox.AddMessageCustom("Join " + GameData.getFactionDisplayName(mFactionSelected + 1) + "?", "Once you pledge your allegiance to a faction, you can not switch sides. Are you sure you want to select the " + tempSuitInfo.mSuitName + " and join " + GameData.getFactionDisplayName(mFactionSelected + 1) + "?", null, true, OnJoinConfirm, "Yes, Join", "No, Cancel");
					break;
				}
				switch (GUIUtil.Button(new Rect(position6.width - 64f, 0f, 64f, 42f), GUIContent.none, "Close"))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "Close";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "Close";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Button_Inactive);
					mSuitSelected = -1;
					break;
				}
				GUI.EndGroup();
			}
		}
		GUI.EndGroup();
		if (Event.current.type == EventType.MouseUp)
		{
			fIgnoreClick = 0.05f;
		}
		lastHover = b;
	}

	private void OnJoinConfirm(MessageBox.ReturnType Return)
	{
		if (Return == MessageBox.ReturnType.MB_YES)
		{
			Logger.trace("mFactionSelected:" + mFactionSelected);
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Buy_Press);
			GameData.MyFactionId = mFactionSelected + 1;
			GameData.MySuitID = mSuitSelected;
			GameData.AddOwnedSuit(mSuitSelected);
			Logger.trace("suit " + GameData.MySuitID);
			Logger.trace("faction " + GameData.MyFactionId);
			string factionDisplayName = GameData.getFactionDisplayName(GameData.MyFactionId);
			MessageBox.AddMessageCustom("Congratulations!", "You have been accepted to join " + factionDisplayName + "!", null, true, OnJoinComplete, "Sweet!");
		}
	}

	private void OnJoinComplete(MessageBox.ReturnType Return)
	{
		bLoading = true;
		Logger.trace("Installing Exonaut Player . . .");
		installExonautPlayer();
	}

	private void installExonautPlayer()
	{
		string text = GameData.SERVICE_PATH + "/ExonautPlayerInstall";
		WWWForm wWWForm = new WWWForm();
		Logger.trace("url " + text);
		Logger.trace("TEGid " + GameData.MyTEGid);
		Logger.trace("login " + GameData.MyLogin);
		Logger.trace("dname " + GameData.MyDisplayName);
		Logger.trace("suit " + GameData.MySuitID);
		Logger.trace("faction " + GameData.MyFactionId);
		wWWForm.AddField("TEGid", GameData.MyTEGid);
		wWWForm.AddField("login", GameData.MyLogin);
		wWWForm.AddField("dname", GameData.MyDisplayName);
		wWWForm.AddField("suit", GameData.MySuitID);
		wWWForm.AddField("faction", GameData.MyFactionId);
		WWW www = new WWW(text, wWWForm);
		Logger.trace("INSTALL Exonaut Player. . .");
		StartCoroutine(waitForPlayerInstall(www));
	}

	private IEnumerator waitForPlayerInstall(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			Logger.trace("Player INSTALL Received " + www.text);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
			XmlNode node = xmlDoc.FirstChild;
			if (node == null)
			{
				Logger.trace("INSTALL was NULL");
			}
			else
			{
				XmlAttributeCollection data = node.Attributes;
				XmlAttribute status = (XmlAttribute)data.GetNamedItem("status");
				if (status.Value.Equals("new"))
				{
					XmlAttribute exoId = (XmlAttribute)data.GetNamedItem("id");
					GameData.MyExonautId = Convert.ToInt32(exoId.Value);
					XmlNode suitNode = xmlDoc.FirstChild.FirstChild;
					if (suitNode != null)
					{
						XmlNodeList suitList = suitNode.ChildNodes;
						Logger.trace("Suit List " + suitList);
						IEnumerator suits = suitList.GetEnumerator();
						while (suits.MoveNext())
						{
							XmlNode suit = (XmlNode)suits.Current;
							int suitId = Convert.ToInt32(suit.InnerText);
							Logger.trace("suit " + suitId);
							GameData.AddOwnedSuit(suitId);
						}
					}
					XmlNodeList missions = xmlDoc.GetElementsByTagName("mission");
					for (int j = 0; j < missions.Count; j++)
					{
						XmlNode mission = missions.Item(j);
						XmlAttributeCollection mdata = mission.Attributes;
						Hashtable missData = new Hashtable
						{
							{
								"name",
								mdata.GetNamedItem("name").Value
							},
							{
								"description",
								mdata.GetNamedItem("description").Value
							},
							{
								"credits",
								mdata.GetNamedItem("credits").Value
							},
							{
								"xp",
								mdata.GetNamedItem("xp").Value
							}
						};
						if (mdata.GetNamedItem("Image") != null && mdata.GetNamedItem("Image").Value != null)
						{
							missData.Add("image", mdata.GetNamedItem("Image").Value.Replace(".png", string.Empty));
						}
						GameData.addCurrentMission(missData);
					}
					XmlNodeList missionsProg = xmlDoc.GetElementsByTagName("missionProg");
					for (int i = 0; i < missionsProg.Count; i++)
					{
						XmlNode prog = missionsProg.Item(i);
						XmlAttributeCollection pData = prog.Attributes;
						GameData.addMissionInProgress(new Hashtable
						{
							{
								"MissionID",
								pData.GetNamedItem("MissionID").Value
							},
							{
								"Rank",
								pData.GetNamedItem("Rank").Value
							},
							{
								"Name",
								pData.GetNamedItem("Name").Value
							},
							{
								"Description",
								pData.GetNamedItem("Description").Value
							},
							{
								"Credits",
								pData.GetNamedItem("Credits").Value
							},
							{
								"XP",
								pData.GetNamedItem("XP").Value
							},
							{
								"Count",
								pData.GetNamedItem("Count").Value
							},
							{
								"Total",
								pData.GetNamedItem("Total").Value
							},
							{
								"Progress",
								pData.GetNamedItem("Progress").Value
							}
						});
					}
				}
			}
			UnityEngine.Object.Destroy(GameMusic);
			Application.LoadLevel("GameHome");
		}
		else
		{
			Logger.trace("There was an error:" + www.error.ToString());
			if (Application.isEditor)
			{
				UnityEngine.Object.Destroy(GameMusic);
				Application.LoadLevel("GameHome");
			}
		}
	}
}
