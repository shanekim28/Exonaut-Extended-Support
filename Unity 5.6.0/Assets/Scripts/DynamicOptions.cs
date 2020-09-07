using UnityEngine;

public class DynamicOptions : MonoBehaviour
{
	public class Settings
	{
		public enum GraphicsLevel
		{
			Low,
			Medium,
			High
		}

		public float mSoundVolume = 0.5f;

		public float mMusicVolume = 0.35f;

		public KeyCode[] mControlValues = new KeyCode[8]
		{
			KeyCode.W,
			KeyCode.S,
			KeyCode.A,
			KeyCode.D,
			KeyCode.Space,
			KeyCode.Mouse0,
			KeyCode.Mouse1,
			KeyCode.R
		};

		public GraphicsLevel mGraphicsLevel = GraphicsLevel.Medium;
	}

	public enum ControlNames
	{
		JUMP,
		CROUCH,
		LEFT,
		RIGHT,
		GRENADE,
		FIRE,
		JETPACK,
		RELOAD,
		NUM_CONTROLS
	}

	private string lastHover = string.Empty;

	private bool bAppFocus = true;

	public static bool bDrawing;

	private GUISkin mSharedSkin;

	public Texture2D mCursorTexture;

	public static bool bDrawCursor = true;

	public float width;

	public float height;

	public bool bCanFullscreen;

	public Rect ControlsRect;

	public Rect SoundRect;

	public Rect MusicRect;

	public Rect GraphicsRect;

	public Rect FullScreenRect;

	public Texture2D[] MouseButtons;

	private string[] mControlNames = new string[8]
	{
		"UP/JUMP",
		"CROUCH",
		"LEFT",
		"RIGHT",
		"GRENADE",
		"FIRE",
		"JETPACK",
		"RELOAD"
	};

	public KeyCode[] mDefaultControls = new KeyCode[8]
	{
		KeyCode.W,
		KeyCode.S,
		KeyCode.A,
		KeyCode.D,
		KeyCode.Space,
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.R
	};

	private bool didrepeat;

	private int mSetInputIndex = -1;

	public float TimeHeld;

	private bool bDown;

	private bool clickRepeat;

	private string curhover = string.Empty;

	private void Start()
	{
		Application.ExternalEval("function onfoc(){GetUnity().SendMessage('WEBCONNECTOR','BrowserFocus','...');}");
		Application.ExternalEval("function onblr(){GetUnity().SendMessage('WEBCONNECTOR','BrowserBlur','...');}");
		Application.ExternalEval("window.onfocus=onfoc;");
		Application.ExternalEval("window.onblur=onblr;");
		LoadOptions();
		mSharedSkin = GUIUtil.mInstance.mSharedSkin;
		Object.DontDestroyOnLoad(base.gameObject);
		Cursor.visible = false;
		ControlsRect = new Rect(20f, 50f, 280f, 360f);
		SoundRect = new Rect(ControlsRect.x + ControlsRect.width + 30f, 50f, 330f, 95f);
		MusicRect = new Rect(ControlsRect.x + ControlsRect.width + 30f, SoundRect.y + SoundRect.height + 3f, SoundRect.width, 95f);
		GraphicsRect = new Rect(ControlsRect.x + ControlsRect.width + 30f, MusicRect.y + MusicRect.height + 28f, SoundRect.width, 82f);
		FullScreenRect = new Rect(ControlsRect.x + ControlsRect.width + 30f, GraphicsRect.y + GraphicsRect.height + 3f, SoundRect.width, 53f);
		width = SoundRect.x + SoundRect.width + 20f;
		height = ControlsRect.y + ControlsRect.height + 58f;
	}

	private void OnApplicationFocus(bool bFocus)
	{
		bAppFocus = bFocus;
	}

	private void BrowserBlur()
	{
		bAppFocus = false;
	}

	private void BrowserFocus()
	{
		bAppFocus = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
		{
			if (mSetInputIndex == -1)
			{
				bDrawing = !bDrawing;
				if (bDrawing)
				{
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.Open_Options);
				}
				else
				{
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.Close_Options);
				}
			}
			mSetInputIndex = -1;
		}
		if (bDrawing && FirstUse.mInstance != null && GameData.MyTutorialStep == 1 && FirstUse.mInstance.GetFrame().mAction != FirstUse.Frame.RequiredAction.Open_Options && FirstUse.mInstance.GetFrame().mAction != FirstUse.Frame.RequiredAction.Close_Options)
		{
			Debug.Log(FirstUse.mInstance.mCurrentFrame + " " + FirstUse.mInstance.GetFrame().mAction);
			bDrawing = false;
		}
		GUI.depth = 0;
		if (bDrawing)
		{
			GamePlay.mPauseScreenActive = true;
			Cursor.visible = true;
			GUIUtil.OnDrawWindow();
			GUI.Window(1500, new Rect(((float)Screen.width - width) / 2f, ((float)Screen.height - height) / 2f, width, height), DoWindow, GUIContent.none, mSharedSkin.window);
		}
		else
		{
			GamePlay.mPauseScreenActive = false;
		}
		Vector2 point = Input.mousePosition;
		point.y = (float)Screen.height - point.y;
		if (point.x > (float)Screen.width || point.x < 0f || point.y > (float)Screen.height || point.y < 0f)
		{
			Cursor.visible = true;
			bDrawCursor = false;
		}
		else
		{
			Cursor.visible = false;
		}
		if (bDrawing && new Rect(((float)Screen.width - width) / 2f, ((float)Screen.height - height) / 2f, width, height).Contains(point))
		{
			bDrawCursor = true;
		}
		if (GUIUtil.Tooltip != string.Empty)
		{
			Vector2 vector = mSharedSkin.GetStyle("ToolTip").CalcSize(new GUIContent(GUIUtil.Tooltip));
			vector.x = Mathf.Min(vector.x, 200f);
			vector.y = mSharedSkin.GetStyle("ToolTip").CalcHeight(new GUIContent(GUIUtil.Tooltip), vector.x);
			Rect position = new Rect(new Rect(point.x + 25f, point.y, vector.x, vector.y));
			if (point.x + position.width + 30f > (float)Screen.width)
			{
				position.x -= vector.x + 50f;
			}
			if (point.y + position.height + 5f > (float)Screen.height)
			{
				position.y = (float)Screen.height - (vector.y + 5f);
			}
			GUI.Box(position, GUIUtil.Tooltip, mSharedSkin.GetStyle("ToolTip"));
			GUIUtil.Tooltip = string.Empty;
		}
		if (bAppFocus)
		{
			if (bDrawCursor)
			{
				GUI.DrawTexture(new Rect(point.x, point.y, mCursorTexture.width, mCursorTexture.height), mCursorTexture);
			}
		}
		else
		{
			Cursor.visible = true;
		}
	}

	private void LateUpdate()
	{
		bDrawCursor = true;
	}

	private float DrawSlider(Rect GroupRect, float Value)
	{
		GUIUtil.DrawProgressBar(new Rect(60f, 20f, GroupRect.width - 120f, 40f), Value, 0f, 1f, GUIUtil.BarDirection.Right, "VolumeBarBG", "VolumeBar");
		if (GUI.RepeatButton(new Rect(50f, 20f, GroupRect.width - 100f, 72f), GUIContent.none, GUIStyle.none))
		{
			didrepeat = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			Vector2 mousePosition = Event.current.mousePosition;
			Value = (mousePosition.x - 60f) / (GroupRect.width - 120f);
		}
		Value = Mathf.Clamp(Value, 0f, 1f);
		float left = (GroupRect.width - 120f) * Value + 60f - 20f;
		switch (GUIUtil.Button(new Rect(left, 60f, 40f, 32f), Mathf.RoundToInt(Value * 100f).ToString(), "VolumeSlider"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Slider";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		}
		switch (GUIUtil.RepeatButton(new Rect(15f, 30f, 30f, 24f), GUIContent.none, "VolumeDown"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "VolumeDown";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			didrepeat = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			curhover = "VolumeDown";
			bDown = true;
			TimeHeld += Time.deltaTime;
			Value -= TimeHeld / 500f;
			break;
		}
		switch (GUIUtil.RepeatButton(new Rect(GroupRect.width - 45f, 30f, 30f, 24f), GUIContent.none, "VolumeUp"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "VolumeUp";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			didrepeat = true;
			if (!clickRepeat)
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			}
			curhover = "VolumeUp";
			bDown = true;
			TimeHeld += Time.deltaTime;
			Value += TimeHeld / 500f;
			break;
		}
		return Value;
	}

	private void DoWindow(int id)
	{
		curhover = ((Event.current.type != EventType.Repaint) ? lastHover : string.Empty);
		GUI.skin = mSharedSkin;
		if (mSetInputIndex != -1)
		{
			GUIUtil.GUIEnable(bEnable: false);
		}
		GUI.Label(new Rect(width / 2f - 125f, 5f, 250f, 30f), "OPTIONS MENU", "OptionsLabel");
		bool gUIEnabled = GUIUtil.GUIEnabled;
		if (GameData.CurPlayState == GameData.PlayState.GAME_STARTED || GameData.CurPlayState == GameData.PlayState.GAME_IS_PLAYING)
		{
			GUIUtil.GUIEnableOverride(bEnable: false);
		}
		GUI.BeginGroup(ControlsRect, "CONTROLS", "OptionsBase");
		for (int i = 0; i < mControlNames.Length; i++)
		{
			Rect position = new Rect(5f, 5 + i * 40, ControlsRect.width - 10f, 35f);
			if (mSetInputIndex == i)
			{
				GUI.color = Color.white;
				if (Input.GetKeyUp(KeyCode.Escape))
				{
					break;
				}
				for (int j = 0; j < 410; j++)
				{
					if (!Input.GetKeyUp((KeyCode)j))
					{
						continue;
					}
					GameData.mGameSettings.mControlValues[mSetInputIndex] = (KeyCode)j;
					for (int k = 0; k < GameData.mGameSettings.mControlValues.Length; k++)
					{
						if (k != mSetInputIndex && GameData.mGameSettings.mControlValues[k] == GameData.mGameSettings.mControlValues[mSetInputIndex])
						{
							GameData.mGameSettings.mControlValues[k] = KeyCode.None;
						}
					}
					mSetInputIndex = -1;
					SaveOptions();
					break;
				}
				GUI.color = Color.white;
				bool gUIEnabled2 = GUIUtil.GUIEnabled;
				GUIUtil.GUIEnableOverride(bEnable: true);
				Rect position2 = new Rect(position.x - 5f, position.y - 45f, position.width + 10f, position.height + 50f);
				GUI.Box(position2, "SET CONTROL", "MapKeyFrame");
				GUI.Label(position2, "PRESS ANY BUTTON TO SET AS INPUT", "MapKeyInstructions");
				GUIUtil.GUIEnableOverride(gUIEnabled2);
			}
			else if (GameData.mGameSettings.mControlValues[i] == KeyCode.None)
			{
				GUI.color = Color.red;
			}
			else
			{
				GUI.color = Color.white;
			}
			GUI.BeginGroup(position);
			if (!GUI.enabled)
			{
				GUI.color = Color.gray;
				GUI.contentColor = Color.black;
			}
			switch (GUIUtil.Button(new Rect(0f, 0f, position.width, position.height), mControlNames[i], "ControlObject"))
			{
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					curhover = "Control " + i;
					if (lastHover != curhover)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			case GUIUtil.GUIState.Click:
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				Input.ResetInputAxes();
				mSetInputIndex = i;
				break;
			}
			switch (GameData.mGameSettings.mControlValues[i])
			{
			case KeyCode.Mouse0:
				GUI.DrawTexture(new Rect(190 - MouseButtons[0].width / 2, 35 - MouseButtons[0].height, MouseButtons[0].width, MouseButtons[0].height), MouseButtons[0]);
				break;
			case KeyCode.Mouse1:
				GUI.DrawTexture(new Rect(190 - MouseButtons[0].width / 2, 35 - MouseButtons[1].height, MouseButtons[1].width, MouseButtons[1].height), MouseButtons[1]);
				break;
			default:
			{
				GUIStyle style = mSharedSkin.GetStyle("KeyControl");
				float minWidth = 0f;
				float maxWidth = 0f;
				string text = GameData.mGameSettings.mControlValues[i].ToString().ToUpper();
				style.CalcMinMaxWidth(new GUIContent(text), out minWidth, out maxWidth);
				minWidth = Mathf.Max(30f, minWidth);
				GUI.Box(new Rect(190f - minWidth / 2f, 5f, minWidth, 25f), text, style);
				break;
			}
			}
			GUI.EndGroup();
		}
		GUI.color = Color.white;
		GUI.contentColor = Color.white;
		switch (GUIUtil.Button(new Rect(5f, ControlsRect.height - 30f, ControlsRect.width - 10f, 25f), "RESTORE DEFAULT CONTROLS", "KeyControl"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "RESTORE DEFAULT CONTROLS";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			GameData.mGameSettings.mControlValues = (KeyCode[])mDefaultControls.Clone();
			SaveOptions();
			break;
		}
		GUI.EndGroup();
		GUIUtil.GUIEnableOverride(gUIEnabled);
		didrepeat = (Event.current.type != EventType.Repaint && clickRepeat);
		GUI.BeginGroup(SoundRect, "SOUND", "OptionsBase");
		GUI.Label(new Rect(0f, 5f, SoundRect.width, 15f), "SOUND EFFECTS VOLUME", "OptionsInfo");
		float num = DrawSlider(SoundRect, GameData.mGameSettings.mSoundVolume);
		if (num != GameData.mGameSettings.mSoundVolume)
		{
			num = Mathf.Clamp(num, 0f, 1f);
			GameData.mGameSettings.mSoundVolume = num;
			GameData.ApplySoundSettings();
			SaveOptions();
		}
		GUI.EndGroup();
		GUI.BeginGroup(MusicRect, GUIContent.none, "OptionsBase");
		GUI.Label(new Rect(0f, 5f, MusicRect.width, 15f), "MUSIC VOLUME", "OptionsInfo");
		float num2 = DrawSlider(MusicRect, GameData.mGameSettings.mMusicVolume);
		if (num2 != GameData.mGameSettings.mMusicVolume)
		{
			num2 = Mathf.Clamp(num2, 0f, 1f);
			GameData.mGameSettings.mMusicVolume = num2;
			GameData.ApplyMusicSettings();
			SaveOptions();
		}
		GUI.EndGroup();
		clickRepeat = didrepeat;
		if (Event.current.type == EventType.Repaint && !bDown)
		{
			TimeHeld = 0f;
		}
		bDown = false;
		GUI.BeginGroup(GraphicsRect, "GRAPHICS", "OptionsBase");
		GUI.BeginGroup(new Rect(0.5f * (GraphicsRect.width - 282f), 5f, 282f, 78f), GUIContent.none, "GraphicsSliderBG");
		Settings.GraphicsLevel graphicsLevel = GameData.mGameSettings.mGraphicsLevel;
		switch (GUIUtil.Button(new Rect(0f, 0f, 41f, 46f), GUIContent.none, GUIStyle.none))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Low Graphics";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			graphicsLevel = Settings.GraphicsLevel.Low;
			break;
		}
		switch (GUIUtil.Button(new Rect(120.5f, 0f, 41f, 46f), GUIContent.none, GUIStyle.none))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Medium Graphics";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			graphicsLevel = Settings.GraphicsLevel.Medium;
			break;
		}
		switch (GUIUtil.Button(new Rect(241f, 0f, 41f, 46f), GUIContent.none, GUIStyle.none))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "High Graphics";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			graphicsLevel = Settings.GraphicsLevel.High;
			break;
		}
		if (graphicsLevel != GameData.mGameSettings.mGraphicsLevel)
		{
			GameData.mGameSettings.mGraphicsLevel = graphicsLevel;
			SaveOptions();
			if (GameData.CurPlayState == GameData.PlayState.GAME_STARTED || GameData.CurPlayState == GameData.PlayState.GAME_IS_PLAYING)
			{
				QualitySettings.currentLevel = (QualityLevel)GameData.mGameSettings.mGraphicsLevel;
			}
			else
			{
				QualitySettings.currentLevel = (QualityLevel)(3 + GameData.mGameSettings.mGraphicsLevel);
			}
		}
		GUI.Toggle(new Rect(0f, 0f, 41f, 46f), GameData.mGameSettings.mGraphicsLevel == Settings.GraphicsLevel.Low, "LOW", "GraphicsSliderButton");
		GUI.Toggle(new Rect(120.5f, 0f, 41f, 46f), GameData.mGameSettings.mGraphicsLevel == Settings.GraphicsLevel.Medium, "MEDIUM", "GraphicsSliderButton");
		GUI.Toggle(new Rect(241f, 0f, 41f, 46f), GameData.mGameSettings.mGraphicsLevel == Settings.GraphicsLevel.High, "HIGH", "GraphicsSliderButton");
		GUI.EndGroup();
		GUI.EndGroup();
		GUI.BeginGroup(FullScreenRect, GUIContent.none, "OptionsBase");
		bool gUIEnabled3 = GUIUtil.GUIEnabled;
		switch (GUIUtil.Toggle(new Rect(10f, 10f, (FullScreenRect.width - 30f) / 2f, 35f), !Screen.fullScreen, "PLAY IN BROWSER", "PlayBrowser"))
		{
		case (GUIUtil.GUIState)18:
		case (GUIUtil.GUIState)20:
		case (GUIUtil.GUIState)34:
		case (GUIUtil.GUIState)36:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "PLAY IN BROWSER";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case (GUIUtil.GUIState)24:
			Screen.SetResolution(900, 600, fullscreen: false);
			break;
		}
		GUIUtil.Toggle(new Rect((FullScreenRect.width - 30f) / 2f - 25f, 17f, 28f, 21f), !Screen.fullScreen, GUIContent.none, "ScreenDongleBrowser");
		GUIUtil.GUIEnableOverride(mSetInputIndex == -1 && bCanFullscreen);
		if (!GUI.enabled)
		{
			GUI.color = Color.gray;
		}
		switch (GUIUtil.Toggle(new Rect((FullScreenRect.width - 30f) / 2f + 20f, 10f, (FullScreenRect.width - 30f) / 2f, 35f), Screen.fullScreen, "PLAY FULLSCREEN", "PlayFullScreen"))
		{
		case (GUIUtil.GUIState)18:
		case (GUIUtil.GUIState)20:
		case (GUIUtil.GUIState)34:
		case (GUIUtil.GUIState)36:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "FULLSCREEN";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case (GUIUtil.GUIState)24:
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
			break;
		}
		GUIUtil.Toggle(new Rect((FullScreenRect.width - 30f) / 2f + 25f, 17f, 28f, 21f), Screen.fullScreen, GUIContent.none, "ScreenDongleFullScreen");
		GUIUtil.GUIEnableOverride(gUIEnabled3);
		GUI.color = Color.white;
		GUI.EndGroup();
		gUIEnabled3 = GUIUtil.GUIEnabled;
		GUIUtil.GUIEnableOverride(mSetInputIndex == -1 && Application.loadedLevelName == "GameHome");
		switch (GUIUtil.Button(new Rect(20f, height - 48f, 120f, 38f), "BOOT CAMP", "ModalButton"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "BOOT CAMP";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
		{
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			bDrawing = false;
			GameData.MyTutorialStep = 0;
			GameObject gameObject = GameObject.Find("homeGUI");
			if (gameObject != null)
			{
				GameHome gameHome = gameObject.GetComponent("GameHome") as GameHome;
				if (gameHome != null)
				{
					gameHome.CheckTutorial(bOverride: true);
				}
			}
			break;
		}
		}
		GUIUtil.GUIEnableOverride(gUIEnabled3);
		switch (GUIUtil.Button(new Rect(width - 140f, height - 48f, 120f, 38f), "CLOSE", "ModalButton"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "CLOSE";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Close_Options);
			bDrawing = false;
			mSetInputIndex = -1;
			break;
		}
		gUIEnabled3 = GUIUtil.GUIEnabled;
		GUIUtil.GUIEnableOverride(mSetInputIndex == -1 && (GameData.CurPlayState == GameData.PlayState.GAME_STARTED || GameData.CurPlayState == GameData.PlayState.GAME_IS_PLAYING));
		switch (GUIUtil.Button(new Rect(width / 2f - 60f, height - 48f, 120f, 38f), (GameData.MyTutorialStep <= 0) ? "QUIT MATCH" : "SKIP TRAINING", "ModalButton"))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "QUIT MATCH";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
		{
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			bDrawing = false;
			GameObject x = GameObject.Find("Game");
			if (x != null)
			{
				GamePlay gamePlayScript = GamePlay.GetGamePlayScript();
				if (gamePlayScript != null)
				{
					gamePlayScript.QuitGame(6);
				}
			}
			break;
		}
		}
		GUIUtil.GUIEnableOverride(gUIEnabled3);
		lastHover = curhover;
	}

	public static void SaveOptions()
	{
		for (int i = 0; i < GameData.mGameSettings.mControlValues.Length; i++)
		{
			PlayerPrefs.SetInt(((ControlNames)i).ToString(), (int)GameData.mGameSettings.mControlValues[i]);
		}
		PlayerPrefs.SetInt("Graphics", (int)GameData.mGameSettings.mGraphicsLevel);
		PlayerPrefs.SetFloat("MusicVolume", GameData.mGameSettings.mMusicVolume);
		PlayerPrefs.SetFloat("SoundVolume", GameData.mGameSettings.mSoundVolume);
		GameObject x = GameObject.Find("Game");
		if (x != null)
		{
			GamePlay gamePlayScript = GamePlay.GetGamePlayScript();
			gamePlayScript.setWorldSoundVolume();
		}
	}

	public static void LoadOptions()
	{
		for (int i = 0; i < GameData.mGameSettings.mControlValues.Length; i++)
		{
			if (PlayerPrefs.HasKey(((ControlNames)i).ToString()))
			{
				GameData.mGameSettings.mControlValues[i] = (KeyCode)PlayerPrefs.GetInt(((ControlNames)i).ToString());
			}
		}
		if (PlayerPrefs.HasKey("Graphics"))
		{
			GameData.mGameSettings.mGraphicsLevel = (Settings.GraphicsLevel)PlayerPrefs.GetInt("Graphics");
		}
		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			GameData.mGameSettings.mMusicVolume = PlayerPrefs.GetFloat("MusicVolume");
		}
		if (PlayerPrefs.HasKey("SoundVolume"))
		{
			GameData.mGameSettings.mSoundVolume = PlayerPrefs.GetFloat("SoundVolume");
		}
		GameData.ApplySoundSettings();
		GameData.ApplyMusicSettings();
		SaveOptions();
	}
}
