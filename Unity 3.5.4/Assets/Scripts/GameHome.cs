using Sfs2X.Entities.Data;
using System.Collections;
using System.Xml;
using UnityEngine;

public class GameHome : MonoBehaviour
{
	private delegate void STATE();

	private const float TransitionLength = 0.5f;

	private static Rect screenSpace = new Rect(0f, 0f, 900f, 600f);

	private static float lastScreenWidth = 900f;

	private static float lastScreenHeight = 600f;

	public TabHome tabhome;

	public TabShowcase tabshowcase;

	private GUISkin mSharedSkin;

	public int battleType;

	private bool bFullScreen;

	private Rect FriendsWindow = new Rect(0f, 0f, 250f, 400f);

	private Vector2 FriendsScroll = default(Vector2);

	private bool bAllowSocial;

	private bool bDrawFriends;

	private bool bExpandFriends;

	private bool bExpandOffline;

	private bool bExpandSentInvites;

	private bool bExpandReceivedInvites;

	private bool bExpandRecentPlaymates;

	public GameObject GuestFirstUse;

	public GameObject RegisteredFirstUse;

	public GameObject GameMusic;

	public Texture2D[] mBanzaiRanks = new Texture2D[6];

	public Texture2D[] mAtlasRanks = new Texture2D[6];

	public Texture2D[] mBanzaiLargeRanks = new Texture2D[6];

	public Texture2D[] mAtlasLargeRanks = new Texture2D[6];

	public Texture2D[] mRankTexts = new Texture2D[6];

	public Texture2D mLevelText;

	public Texture2D mGeneratedBanner;

	public Material mGeneratedBannerMat;

	public Vector2[] mLevelTextSizes = new Vector2[11];

	private bool m_connected;

	private STATE m_state;

	private HolidayEvent.LoadHolidayEvent m_LoadHolidayEvent;

	private HolidayEvent.UpdateHolidayEvent m_UpdateHolidayEvent;

	private HolidayEvent.DrawHolidayEvent m_DrawHolidayEvent;

	private NetworkManager m_networkManager;

	private Triangle[] NavButtonTris = new Triangle[4]
	{
		new Triangle(new Vector3(0f, 0.5f, 0f), new Vector3(0.24f, 0f, 0f), new Vector3(0.73f, 0f, 0f)),
		new Triangle(new Vector3(0f, 0.5f, 0f), new Vector3(0.73f, 0f, 0f), new Vector3(1f, 0.5f, 0f)),
		new Triangle(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 0.5f, 0f), new Vector3(0.73f, 1f, 0f)),
		new Triangle(new Vector3(0f, 0.5f, 0f), new Vector3(0.73f, 1f, 0f), new Vector3(0.24f, 1f, 0f))
	};

	private Triangle[] PlayButtonTris = new Triangle[2]
	{
		new Triangle(new Vector3(0f, 0.04f, 0f), new Vector3(0.84f, 0.04f, 0f), new Vector3(0.15f, 0.96f, 0f)),
		new Triangle(new Vector3(0.15f, 0.96f, 0f), new Vector3(0.84f, 0.04f, 0f), new Vector3(1f, 0.96f, 0f))
	};

	private bool m_isLogin;

	private Rect BlackBox1;

	private Rect BlackBox2;

	public Transform[] ModelTransforms = new Transform[4];

	public Transform mCurrentSuit;

	public GameObject mInstancedSuit;

	public float[] FlagPositions = new float[2]
	{
		0.34f,
		0.265f
	};

	public float FlagPos;

	public Texture2D XPCreditsImg;

	public Texture2D NavButtonsBG;

	public Texture2D PlayerImgBG;

	public Texture2D PlayButtonBG;

	public Texture2D avatarTexture;

	public Texture2D[] NavStateImg;

	public Texture2D Border;

	public Texture2D[] BattleButtonAnimFrames;

	public Texture2D BattleText;

	public Texture2D TeamBattleText;

	public Vector3 TestVector;

	public float alpha;

	private int NavState;

	private int TargetState = -1;

	private int LastState = -1;

	public int animmod = 34;

	private float NavIndicator = 60f;

	private float TransitionTimer;

	private float SuitEffectTimer;

	private string lastHover = string.Empty;

	private string curhover = string.Empty;

	private string lastHover2 = string.Empty;

	private string curhover2 = string.Empty;

	public static Rect ScreenSpace {
		get {
			return screenSpace;
		}
	}

	private void Awake()
	{
		if (Application.isEditor)
		{
			GameData.MATCH_MODE = GameData.Build.DEBUG;
		}
		if (GameData.MyPlayStatus > 1)
		{
			getMissionInProgressUpdate();
		}
		GL.ClearWithSkybox( true, Camera.main);
		GameObject gameObject = GameObject.Find("Tracker");
		TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
		trackerScript.AddMetric(TrackerScript.Metric.IN_HANGAR);
		trackerScript.updateMetricStats();
		GameObject gameObject2 = GameObject.Find("NetworkManager");
		m_networkManager = (gameObject2.GetComponent("NetworkManager") as NetworkManager);
		m_state = Home;
		Camera.main.renderingPath = RenderingPath.UsePlayerSettings;
		if (GameData.eventObjects.ContainsKey("platform"))
		{
			GameObject gameObject3 = Object.Instantiate(GameData.eventObjects["platform"] as GameObject) as GameObject;
			GUIUtil.mInstance.mBackground = gameObject3.transform;
		}
	}

	private void Start()
	{
		QualitySettings.currentLevel = (QualityLevel)(3 + GameData.mGameSettings.mGraphicsLevel);
		DynamicOptions.bDrawCursor = true;
		GameData.IsChooserActive = true;
		GameObject gameObject = GameObject.Find("DynamicOptions");
		if (gameObject != null)
		{
			DynamicOptions dynamicOptions = gameObject.GetComponent("DynamicOptions") as DynamicOptions;
			dynamicOptions.bCanFullscreen = true;
		}
		if (GameObject.Find("GameMusic(Clone)") == null)
		{
			if (GameData.DoesEventExist("GameHome_MusicLoop"))
			{
				GameMusic.GetComponent<AudioSource>().clip = (GameData.eventObjects["GameHome_MusicLoop"] as AudioClip);
			}
			GameMusic = (Object.Instantiate(GameMusic) as GameObject);
			GameMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
		}
		else
		{
			GameMusic = GameObject.Find("GameMusic(Clone)");
			GameMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
		}
		if (GameData.MasterSuitList.Count == 0)
		{
			GameData.InitSuitList(string.Empty);
		}
		ModelTransforms = new Transform[4];
		ModelTransforms[0] = null;
		ModelTransforms[1] = GUIUtil.mInstance.mBackground;
		ModelTransforms[2] = GUIUtil.mInstance.mSuitEffect;
		ModelTransforms[3] = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
		tabhome = new TabHome(this);
		tabshowcase = new TabShowcase(this);
		mSharedSkin = GUIUtil.mInstance.mSharedSkin;
		StartCoroutine(UpdateScreenSpace());
		CheckTutorial(bOverride: false);
		HolidayEvent.CreateEvent(out m_LoadHolidayEvent, out m_UpdateHolidayEvent, out m_DrawHolidayEvent);
		if (m_LoadHolidayEvent != null)
		{
			m_LoadHolidayEvent();
		}
	}

	public void onSFSMessage(string msg)
	{
		Logger.trace(this + " RECEIVED MESSAGE " + msg);
	}

	private void Update()
	{
		UpdateModelPositions();
		m_state();
		if (m_UpdateHolidayEvent != null)
		{
			m_UpdateHolidayEvent();
		}
		if (GameData.MATCH_MODE == GameData.Build.PRODUCTION && !Application.dataPath.Contains("cn-internal"))
		{
			return;
		}
		if (!m_isLogin)
		{
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))
			{
				Debug.Log("Bring up Login");
				m_isLogin = true;
			}
		}
		else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))
		{
			Debug.Log("Remove Login");
			m_isLogin = false;
		}
	}

	private void Home()
	{
		if (SuitEffectTimer > 0f)
		{
			SuitEffectTimer -= Time.deltaTime;
			if (SuitEffectTimer <= 0f)
			{
				SuitEffectTimer = 0f;
				ModelTransforms[2].gameObject.SetActiveRecursively(state: false);
				ModelTransforms[2] = null;
			}
			else
			{
				ModelTransforms[2] = GUIUtil.mInstance.mSuitEffect;
				ModelTransforms[2].gameObject.SetActiveRecursively(state: true);
				ModelTransforms[1].gameObject.SetActiveRecursively(state: true);
			}
		}
		if (TransitionTimer > 0f)
		{
			TransitionTimer -= Time.deltaTime;
			float num = 0.25f;
			NavIndicator = Mathf.Lerp(0f, 60f, Mathf.Abs(TransitionTimer - num) / num);
			FlagPos = Mathf.Lerp(FlagPositions[LastState] * screenSpace.width, FlagPositions[TargetState] * screenSpace.width, 1f - 2f * TransitionTimer);
			if (TransitionTimer < 0.25f)
			{
				NavState = TargetState;
			}
			if (TransitionTimer <= 0f)
			{
				TransitionTimer = 0f;
			}
		}
		if (bFullScreen != Screen.fullScreen || lastScreenWidth != (float)Screen.width || lastScreenHeight != (float)Screen.height)
		{
			StartCoroutine(UpdateScreenSpace());
		}
	}

	private void Connect()
	{
		Logger.traceAlways("<< Connect... ");
		m_connected = true;
		if (!m_networkManager.isConnected())
		{
			Debug.Log("<< Not connected so reconnect ");
			m_networkManager.Connect();
		}
		m_state = Login;
	}

	private void Login()
	{
		if (m_networkManager.isConnected())
		{
			m_networkManager.Login();
			m_state = GotoQueue;
		}
	}

	private void GotoQueue()
	{
		if (!m_networkManager.isLoggedIn())
		{
			Debug.Log("<< not logged in");
		}
		if (m_networkManager.isLoggedIn() && m_networkManager.isInRoom())
		{
			Application.LoadLevel("GameBattleQueue");
			ModelTransforms[1].gameObject.SetActiveRecursively(state: false);
		}
		Debug.Log("<< want to goto Queue");
	}

	private void BuildFactoinFlag()
	{
		mGeneratedBanner = new Texture2D(180, (int)screenSpace.height, TextureFormat.ARGB32, mipmap: false);
		mGeneratedBanner.filterMode = FilterMode.Trilinear;
		mGeneratedBanner.wrapMode = TextureWrapMode.Clamp;
		Color[] pixels = mGeneratedBanner.GetPixels();
		Color color = (GameData.MyFactionId != 1) ? new Color(0.2f, 0.2f, 0.1f) : new Color(0.26f, 0.0600000024f, 0.0600000024f);
		for (int i = 0; i < pixels.Length; i++)
		{
			int num = i / mGeneratedBanner.width;
			pixels[i] = color;
			pixels[i].a = Mathf.Lerp(0.3f, 0.9f, (float)num / (float)mGeneratedBanner.height);
		}
		mGeneratedBanner.SetPixels(pixels);
		mGeneratedBanner.Apply();
		Texture2D texture2D = (GameData.MyFactionId != 1) ? mAtlasLargeRanks[GameData.MyRank] : mBanzaiLargeRanks[GameData.MyRank];
		Texture2D texture2D2 = mRankTexts[GameData.MyRank];
		CustomBlit(texture2D, mGeneratedBanner, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2((mGeneratedBanner.width - texture2D.width) / 2, 90f), Color.black);
		CustomBlit(texture2D2, mGeneratedBanner, new Rect(0f, 0f, texture2D2.width, texture2D2.height), new Vector2((mGeneratedBanner.width - texture2D2.width) / 2, 70f), Color.black);
		int[] array = new int[3]
		{
			10,
			(GameData.MyLevel / 10 != 0) ? (GameData.MyLevel / 10) : (-1),
			GameData.MyLevel % 10
		};
		float num2 = 0f;
		int[] array2 = array;
		foreach (int num3 in array2)
		{
			if (num3 != -1)
			{
				num2 += mLevelTextSizes[num3].y;
			}
		}
		float num4 = ((float)mGeneratedBanner.width - num2) / 2f;
		int[] array3 = array;
		foreach (int num5 in array3)
		{
			if (num5 != -1)
			{
				CustomBlit(mLevelText, mGeneratedBanner, new Rect(mLevelTextSizes[num5].x, 0f, mLevelTextSizes[num5].y, mLevelText.height), new Vector2(num4, 50f), Color.black);
				num4 += mLevelTextSizes[num5].y;
			}
		}
		Vector3 b = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 40f));
		Vector3 a = Camera.main.ScreenToWorldPoint(new Vector3(0f, screenSpace.height, 40f));
		Vector3 a2 = Camera.main.ScreenToWorldPoint(new Vector3(0f, 180f, 40f));
		float num6 = (a - b).magnitude / 10f;
		float x = (a2 - b).magnitude / 10f;
		ModelTransforms[3].localScale = new Vector3(x, num6, num6);
		ModelTransforms[3].GetComponent<Renderer>().material = mGeneratedBannerMat;
		ModelTransforms[3].GetComponent<Renderer>().material.mainTexture = mGeneratedBanner;
		Quaternion rotation = Camera.main.transform.rotation;
		Camera.main.transform.rotation = Quaternion.identity;
		tabhome.ModelPositions[3] = Camera.main.ScreenToWorldPoint(new Vector3(screenSpace.x + FlagPositions[0] * screenSpace.width + 90f, screenSpace.height / 2f + screenSpace.y, 40f));
		tabshowcase.mSuitInspector.ModelPositions[3] = Camera.main.ScreenToWorldPoint(new Vector3(screenSpace.x + FlagPositions[1] * screenSpace.width + 90f, screenSpace.height / 2f + screenSpace.y, 40f));
		Camera.main.transform.rotation = rotation;
	}

	private void CustomBlit(Texture2D SrcImg, Texture2D DestImg, Rect SrcPos, Vector2 DestPos, Color SrcColorMod)
	{
		Color[] pixels = SrcImg.GetPixels((int)SrcPos.x, (int)SrcPos.y, (int)SrcPos.width, (int)SrcPos.height);
		int y = (int)((float)DestImg.height - DestPos.y - SrcPos.height);
		Color[] pixels2 = DestImg.GetPixels((int)DestPos.x, y, (int)SrcPos.width, (int)SrcPos.height);
		for (int i = 0; i < pixels2.Length; i++)
		{
			Color color = pixels[i];
			pixels2[i].r = Mathf.Lerp(pixels2[i].r, color.r * SrcColorMod.r, color.a);
			pixels2[i].g = Mathf.Lerp(pixels2[i].g, color.g * SrcColorMod.g, color.a);
			pixels2[i].b = Mathf.Lerp(pixels2[i].b, color.b * SrcColorMod.b, color.a);
			pixels2[i].a = Mathf.Lerp(pixels2[i].a, color.a * SrcColorMod.a, color.a);
		}
		DestImg.SetPixels((int)DestPos.x, y, (int)SrcPos.width, (int)SrcPos.height, pixels2);
		DestImg.Apply();
	}

	public IEnumerator UpdateScreenSpace()
	{
		yield return new WaitForEndOfFrame();
		bFullScreen = Screen.fullScreen;
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		float ar = (float)Screen.width / (float)Screen.height;
		float variance = ar / 1.5f;
		if (variance > 1f)
		{
			variance = 1f / variance;
			screenSpace = new Rect((int)((1f - variance) / 2f * (float)Screen.width), 0f, (int)((float)Screen.width - (1f - variance) * (float)Screen.width), Screen.height);
			BlackBox1 = new Rect(0f, 0f, screenSpace.x, Screen.height);
			BlackBox2 = new Rect(screenSpace.x + screenSpace.width, 0f, screenSpace.x, Screen.height);
		}
		else if (variance < 1f)
		{
			screenSpace = new Rect(0f, (int)((1f - variance) / 2f * (float)Screen.height), Screen.width, (int)((float)Screen.height - (1f - variance) * (float)Screen.height));
			BlackBox1 = new Rect(0f, 0f, Screen.width, screenSpace.y);
			BlackBox2 = new Rect(0f, screenSpace.y + screenSpace.height, Screen.width, screenSpace.y + screenSpace.height);
		}
		else
		{
			screenSpace = new Rect(0f, 0f, Screen.width, Screen.height);
			BlackBox1 = default(Rect);
			BlackBox2 = default(Rect);
		}
		Debug.Log(variance + " " + screenSpace);
		FlagPos = FlagPositions[NavState] * screenSpace.width;
		MessageBox.ResetWindowPosition();
		tabhome.UpdateScreenSpace(screenSpace);
		tabshowcase.UpdateScreenSpace(screenSpace);
		BuildFactoinFlag();
	}

	private void FixedUpdate()
	{
	}

	public void SetSuitTransform(Transform trans)
	{
		if (mCurrentSuit != trans)
		{
			if (ModelTransforms[0] != null)
			{
				Object.DestroyImmediate(ModelTransforms[0].gameObject);
				ModelTransforms[0] = null;
			}
			PlaySuitEffect(0.5f);
			mCurrentSuit = trans;
			if (mCurrentSuit != null)
			{
				ModelTransforms[0] = (Object.Instantiate(mCurrentSuit, Vector3.zero, Quaternion.identity) as Transform);
			}
		}
	}

	public void PlaySuitEffect(float Time)
	{
		SuitEffectTimer = Time;
	}

	private void DoTransition(int Nav)
	{
		if (Nav != NavState && Nav != TargetState)
		{
			LastState = NavState;
			TransitionTimer = 0.5f;
			TargetState = Nav;
		}
	}

	private void UpdateModelPositions()
	{
		Vector3[] array = (Vector3[])tabshowcase.mSuitInspector.ModelPositions.Clone();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] += tabshowcase.mSuitInspector.mCameraOffset;
		}
		Vector3[] array2 = new Vector3[ModelTransforms.Length];
		Vector3[] array3 = new Vector3[ModelTransforms.Length];
		for (int j = 0; j < tabhome.ModelPositions.Length; j++)
		{
			float t = Mathf.Abs((float)((TargetState == 1) ? 1 : 0) - 2f * TransitionTimer);
			array2[j] = Vector3.Lerp(tabhome.ModelPositions[j], array[j], t);
			array3[j] = Vector3.Lerp(tabhome.ModelRotations[j], tabshowcase.mSuitInspector.ModelRotations[j], t);
		}
		if (array2.Length != array3.Length && array3.Length != ModelTransforms.Length)
		{
			return;
		}
		for (int k = 0; k < ModelTransforms.Length; k++)
		{
			if (ModelTransforms[k] != null)
			{
				ModelTransforms[k].parent = Camera.main.transform;
				ModelTransforms[k].transform.localPosition = array2[k];
				ModelTransforms[k].transform.localRotation = Quaternion.Euler(array3[k]);
				ModelTransforms[k].parent = null;
			}
		}
	}

	private void OnGUI()
	{
		curhover = ((Event.current.type != EventType.Repaint) ? lastHover : string.Empty);
		GUIUtil.GUIEnabled = true;
		GUI.BeginGroup(new Rect(screenSpace.x, screenSpace.y, 100000f, 200000f));
		GUI.skin = mSharedSkin;
		GUI.depth = 5;
		string str = (GameData.MyPlayStatus <= 1) ? "Guest " : string.Empty;
		str = str + " " + GameData.getFactionDisplayName(GameData.MyFactionId);
		GUIUtil.mInstance.mModelRenderer.transform.rotation = Quaternion.identity;
		switch (NavState)
		{
		case 0:
			SetSuitTransform(tabhome.ModelTransform);
			tabhome.showTab(screenSpace);
			break;
		case 1:
			SetSuitTransform(tabshowcase.mSuitInspector.ModelTransforms[0]);
			tabshowcase.showTab(screenSpace);
			break;
		}
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(0f, 0f, screenSpace.width, 5f), Border);
		GUI.DrawTexture(new Rect(0f, screenSpace.height - 24f, screenSpace.width, 24f), Border);
		GUI.DrawTexture(new Rect(0f, 0f, 5f, screenSpace.height), Border);
		GUI.DrawTexture(new Rect(screenSpace.width - 5f, 0f, 5f, screenSpace.height), Border);
		GUIUtil.GUIEnableOverride(bEnable: true);
		GUI.Box(new Rect(FlagPos - 43.5f, 5f, 257f, 20f), GameData.getFactionDisplayName(GameData.MyFactionId).ToUpper(), "FactionFlagTop");
		GUIUtil.GUIEnable(bEnable: true);
		GUI.BeginGroup(new Rect(screenSpace.width - (float)XPCreditsImg.width, screenSpace.height - (float)XPCreditsImg.height, XPCreditsImg.width, XPCreditsImg.height));
		GUI.DrawTexture(new Rect(0f, 0f, XPCreditsImg.width, XPCreditsImg.height), XPCreditsImg);
		GUI.Label(new Rect(0f, 0f, XPCreditsImg.width, XPCreditsImg.height), GameData.MyTotalCredits.ToString(), "Credits");
		if (GameData.MyLevel == 50)
		{
			GUI.Box(new Rect(10f, 72f, 243f, 26f), GUIContent.none, "MaxLevel");
		}
		else
		{
			GUI.Box(new Rect(12f, 74f, 50f + Mathf.Max(0f, 150f * (((float)GameData.MyTotalXP - (float)GameData.getExpNeededForLevel(GameData.MyLevel)) / (float)(GameData.getExpNeededForLevel(GameData.MyLevel + 1) - GameData.getExpNeededForLevel(GameData.MyLevel)))), 22f), GUIContent.none, "XPBar");
			GUI.Label(new Rect(0f, 0f, XPCreditsImg.width, XPCreditsImg.height), GameData.MyLevel.ToString(), "MyLevel");
			GUI.Label(new Rect(0f, 0f, XPCreditsImg.width, XPCreditsImg.height), (GameData.MyLevel + 1).ToString(), "NextLevel");
		}
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(0f, screenSpace.height - (float)PlayerImgBG.height, PlayerImgBG.width, PlayerImgBG.height));
		GUI.DrawTexture(new Rect(0f, 0f, PlayerImgBG.width, PlayerImgBG.height), PlayerImgBG);
		if (avatarTexture != null)
		{
			GUI.DrawTexture(new Rect(4f, PlayerImgBG.height - 47, 43f, 43f), avatarTexture);
		}
		else
		{
			avatarTexture = (Resources.Load("HUD/avatar/" + GameHUD.avatar_images[GameData.MySuitID - 1]) as Texture2D);
		}
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(0f, 0f, NavButtonsBG.width + 100, NavButtonsBG.height));
		GUI.DrawTexture(new Rect(NavIndicator, 37.5f, 87f, 79f), NavStateImg[(GameData.MyFactionId - 1) * 2 + NavState]);
		GUI.DrawTexture(new Rect(0f, 0f, NavButtonsBG.width, NavButtonsBG.height), NavButtonsBG);
		switch (GUIUtil.CustomButton(NavButtonTris, new Rect(6f, 5f, 81f, 72f), GUIContent.none, "Home" + GameData.MyFactionId))
		{
		case GUIUtil.GUIState.Click:
			curhover = "Home";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			DoTransition(0);
			break;
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Home";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		}
		switch (GUIUtil.CustomButton(NavButtonTris, new Rect(6f, 80f, 81f, 72f), GUIContent.none, "Hangar" + GameData.MyFactionId))
		{
		case GUIUtil.GUIState.Click:
			curhover = "Hangar";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			DoTransition(1);
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Open_Showcase);
			break;
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Hangar";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		}
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(screenSpace.width - (float)PlayButtonBG.width, 0f, PlayButtonBG.width, PlayButtonBG.height));
		GUI.DrawTexture(new Rect(0f, 0f, PlayButtonBG.width, PlayButtonBG.height), PlayButtonBG);
		int num = (int)(Time.realtimeSinceStartup * 24f) % animmod;
		switch (GUIUtil.CustomButton(PlayButtonTris, new Rect(8f, 4f, 195f, 51f), GUIContent.none, "Battle" + GameData.MyFactionId))
		{
		case GUIUtil.GUIState.Inactive:
		case (GUIUtil.GUIState)3:
		case (GUIUtil.GUIState)5:
			GUI.DrawTexture(new Rect(8f, 4f, 195f, 51f), BattleText);
			break;
		case GUIUtil.GUIState.None:
			if (num < 20)
			{
				GUI.color = ((GameData.MyFactionId != 1) ? new Color(0.8f, 0.8f, 0.8f) : Color.yellow);
				GUI.DrawTexture(new Rect(6f, 4f, 174f, 50f), BattleButtonAnimFrames[num]);
				GUI.color = Color.white;
			}
			GUI.DrawTexture(new Rect(8f, 4f, 195f, 51f), BattleText);
			break;
		case GUIUtil.GUIState.Active:
			if (num < 20)
			{
				GUI.color = ((GameData.MyFactionId != 1) ? new Color(0.48f, 0.49f, 0.22f) : new Color(0.73f, 0.19f, 0.09f));
				GUI.DrawTexture(new Rect(6f, 4f, 174f, 50f), BattleButtonAnimFrames[num]);
				GUI.color = Color.white;
			}
			GUI.DrawTexture(new Rect(8f, 4f, 195f, 51f), BattleText);
			break;
		case GUIUtil.GUIState.Hover:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Battle";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
				if (num < 20)
				{
					GUI.color = ((GameData.MyFactionId != 1) ? new Color(0.48f, 0.49f, 0.22f) : new Color(0.73f, 0.19f, 0.09f));
					GUI.DrawTexture(new Rect(6f, 4f, 174f, 50f), BattleButtonAnimFrames[num]);
					GUI.color = Color.white;
				}
				GUI.color = Color.black;
				GUI.DrawTexture(new Rect(8f, 4f, 195f, 51f), BattleText);
				GUI.color = Color.white;
			}
			break;
		case GUIUtil.GUIState.Click:
			curhover = "Battle";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			battleType = 1;
			m_networkManager.m_gameModeStr = GameMode.freeforall.ToString();
			break;
		}
		GUIUtil.GUIEnable(GameData.MyPlayStatus != 1 || GameData.MATCH_MODE == GameData.Build.DEBUG);
		switch (GUIUtil.CustomButton(PlayButtonTris, new Rect(176f, 4f, 195f, 51f), GUIContent.none, "TeamBattle" + GameData.MyFactionId))
		{
		case GUIUtil.GUIState.Inactive:
		case (GUIUtil.GUIState)3:
		case (GUIUtil.GUIState)5:
			GUI.DrawTexture(new Rect(176f, 4f, 195f, 51f), TeamBattleText);
			break;
		case GUIUtil.GUIState.None:
			GUI.color = ((GameData.MyFactionId != 1) ? new Color(0.8f, 0.8f, 0.8f) : Color.yellow);
			if (num < 20)
			{
				GUI.DrawTexture(new Rect(175f, 4f, 174f, 50f), BattleButtonAnimFrames[num]);
			}
			if (num >= 7 && num < 25)
			{
				GUI.DrawTexture(new Rect(158f, 4f, 174f, 50f), BattleButtonAnimFrames[num - 5]);
			}
			if (num >= 14 && num < 30)
			{
				GUI.DrawTexture(new Rect(141f, 4f, 174f, 50f), BattleButtonAnimFrames[num - 10]);
			}
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(176f, 4f, 195f, 51f), TeamBattleText);
			break;
		case GUIUtil.GUIState.Active:
			GUI.color = ((GameData.MyFactionId != 1) ? new Color(0.48f, 0.49f, 0.22f) : new Color(0.73f, 0.19f, 0.09f));
			if (num < 20)
			{
				GUI.DrawTexture(new Rect(175f, 4f, 174f, 50f), BattleButtonAnimFrames[num]);
			}
			if (num >= 7 && num < 25)
			{
				GUI.DrawTexture(new Rect(158f, 4f, 174f, 50f), BattleButtonAnimFrames[num - 5]);
			}
			if (num >= 14 && num < 30)
			{
				GUI.DrawTexture(new Rect(141f, 4f, 174f, 50f), BattleButtonAnimFrames[num - 10]);
			}
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(176f, 4f, 195f, 51f), TeamBattleText);
			break;
		case GUIUtil.GUIState.Hover:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "TeamBattle";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
				GUI.color = ((GameData.MyFactionId != 1) ? new Color(0.48f, 0.49f, 0.22f) : new Color(0.73f, 0.19f, 0.09f));
				if (num < 20)
				{
					GUI.DrawTexture(new Rect(175f, 4f, 174f, 50f), BattleButtonAnimFrames[num]);
				}
				if (num >= 7 && num < 25)
				{
					GUI.DrawTexture(new Rect(158f, 4f, 174f, 50f), BattleButtonAnimFrames[num - 5]);
				}
				if (num >= 14 && num < 30)
				{
					GUI.DrawTexture(new Rect(141f, 4f, 174f, 50f), BattleButtonAnimFrames[num - 10]);
				}
				GUI.color = Color.black;
				GUI.DrawTexture(new Rect(176f, 4f, 195f, 51f), TeamBattleText);
				GUI.color = Color.white;
			}
			break;
		case GUIUtil.GUIState.Click:
			curhover = "TeamBattle";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			battleType = 2;
			m_networkManager.m_gameModeStr = GameMode.team.ToString();
			break;
		}
		GUIUtil.GUIEnable(bEnable: true);
		GUI.EndGroup();
		GUI.Box(new Rect(50f, screenSpace.height - 20f, screenSpace.width - 55f, 16f), GameData.MyDisplayName.ToUpper(), "NameBanner" + GameData.MyFactionId);
		GUI.Box(new Rect(screenSpace.width - 124f, screenSpace.height - 20f, 4f, 16f), GUIContent.none, "Separator");
		GUI.Box(new Rect(screenSpace.width - 243f, screenSpace.height - 20f, 4f, 16f), GUIContent.none, "Separator");
		switch (GUIUtil.Toggle(new Rect(screenSpace.width - 120f, screenSpace.height - 20f, 115f, 16f), GameData.MyFactionId == 2, GUIContent.none, "FullScreen"))
		{
		case GUIUtil.GUIState.Click:
		case (GUIUtil.GUIState)24:
		case (GUIUtil.GUIState)40:
			curhover = "FullScreen";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			if (Screen.fullScreen)
			{
				Screen.SetResolution(900, 600, fullscreen: false);
			}
			else
			{
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
			}
			break;
		case GUIUtil.GUIState.Hover:
		case (GUIUtil.GUIState)18:
		case (GUIUtil.GUIState)20:
		case (GUIUtil.GUIState)34:
		case (GUIUtil.GUIState)36:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "FullScreen";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		}
		switch (GUIUtil.Toggle(new Rect(screenSpace.width - 239f, screenSpace.height - 20f, 115f, 16f), GameData.MyFactionId == 2, GUIContent.none, "Options"))
		{
		case GUIUtil.GUIState.Click:
		case (GUIUtil.GUIState)24:
		case (GUIUtil.GUIState)40:
			curhover = "Options";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			DynamicOptions.bDrawing = true;
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Open_Options);
			break;
		case GUIUtil.GUIState.Hover:
		case (GUIUtil.GUIState)18:
		case (GUIUtil.GUIState)20:
		case (GUIUtil.GUIState)34:
		case (GUIUtil.GUIState)36:
			if (Event.current.type == EventType.Repaint)
			{
				curhover = "Options";
				if (lastHover != curhover)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		}
		if (bAllowSocial)
		{
			GUI.Box(new Rect(screenSpace.width - 362f, screenSpace.height - 20f, 4f, 16f), GUIContent.none, "Separator");
			switch (GUIUtil.Toggle(new Rect(screenSpace.width - 358f, screenSpace.height - 20f, 115f, 16f), GameData.MyFactionId == 2, GUIContent.none, "Friends"))
			{
			case GUIUtil.GUIState.Click:
			case (GUIUtil.GUIState)24:
			case (GUIUtil.GUIState)40:
				curhover = "Friends";
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				bDrawFriends = !bDrawFriends;
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.Open_Options);
				break;
			case GUIUtil.GUIState.Hover:
			case (GUIUtil.GUIState)18:
			case (GUIUtil.GUIState)20:
			case (GUIUtil.GUIState)34:
			case (GUIUtil.GUIState)36:
				if (Event.current.type == EventType.Repaint)
				{
					curhover = "Friends";
					if (lastHover != curhover)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
		}
		if (battleType > 0 && !m_connected)
		{
			GameObject gameObject = GameObject.Find("Tracker");
			TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
			GameData.ConsecutiveGames = 0;
			trackerScript.AddMetric((battleType != 1) ? TrackerScript.Metric.REQUEST_TEAM_BATTLE : TrackerScript.Metric.REQUEST_BATTLE);
			trackerScript.updateMetricStats();
			GameData.BattleType = battleType;
			GameData.IsChooserActive = false;
			Debug.Log("requesting battletype " + battleType);
			Connect();
		}
		GUI.EndGroup();
		if (m_DrawHolidayEvent != null)
		{
			m_DrawHolidayEvent();
		}
		if (m_isLogin)
		{
			GUI.Label(new Rect(10f, 90f, 90f, 100f), "TegId: ");
			GameData.MyTEGid = GUI.TextField(new Rect(100f, 90f, 150f, 20f), GameData.MyTEGid);
			GUI.Label(new Rect(270f, 90f, 90f, 100f), "AuthId: ");
			GameData.MyAuthid = GUI.TextField(new Rect(370f, 90f, 200f, 20f), GameData.MyAuthid);
			GameData.MyPlayStatus = 3;
		}
		GUI.DrawTexture(BlackBox1, GUI.skin.GetStyle("blackbox").normal.background);
		GUI.DrawTexture(BlackBox2, GUI.skin.GetStyle("blackbox").normal.background);
		if (bAllowSocial && bDrawFriends && !DynamicOptions.bDrawing)
		{
			FriendsWindow.x = screenSpace.width - 358f;
			FriendsWindow.y = screenSpace.height - FriendsWindow.height - 27f;
			GUI.Window(1700, FriendsWindow, DrawFriendsList, GUIContent.none);
		}
		lastHover = curhover;
	}

	private void DrawFriendsList(int windowID)
	{
		curhover2 = ((Event.current.type != EventType.Repaint) ? lastHover2 : string.Empty);
		Rect position = new Rect(0f, 0f, FriendsWindow.width, FriendsWindow.height);
		GUI.Box(position, GUIContent.none, "blackbox");
		Rect position2 = new Rect(5f, 42f, position.width - 10f, position.height - 42f - 25f);
		GUI.color = Color.gray;
		GUI.Label(new Rect(5f, 5f, position.width - 80f, 40f), "Friends");
		GUI.Box(position2, GUIContent.none, "whitebox");
		GUI.color = Color.white;
		switch (GUIUtil.Button(new Rect(FriendsWindow.width - 64f, 0f, 64f, 42f), GUIContent.none, "Close"))
		{
		case GUIUtil.GUIState.Click:
			curhover2 = "Close";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Button_Inactive);
			bDrawFriends = false;
			break;
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				curhover2 = "Close";
				if (lastHover2 != curhover2)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Space(position2.x);
		GUILayout.BeginVertical();
		GUILayout.Space(position2.y - 5f);
		FriendsScroll = GUILayout.BeginScrollView(FriendsScroll, GUILayout.Width(position2.width), GUILayout.Height(position2.height));
		if (bExpandFriends = GUILayout.Toggle(bExpandFriends, "Friends"))
		{
			for (int i = 0; i < 6; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.Box(GUIContent.none, "whitebox", GUILayout.Width(30f), GUILayout.Height(30f));
				GUILayout.Space(5f);
				GUILayout.BeginVertical();
				GUILayout.Space(-2f);
				GUILayout.Label("Friend:" + i);
				GUILayout.Space(-12f);
				GUILayout.Label("In a Battle: 4 minutes");
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
		if (bExpandOffline = GUILayout.Toggle(bExpandOffline, "Offline Friends"))
		{
			for (int j = 0; j < 6; j++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.Box(GUIContent.none, "whitebox", GUILayout.Width(30f), GUILayout.Height(30f));
				GUILayout.Space(5f);
				GUILayout.BeginVertical();
				GUILayout.Space(-2f);
				GUILayout.Label("Offline Friend:" + j);
				GUILayout.Space(-12f);
				GUILayout.Label("Last Online: 6 hours ago");
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
		if (bExpandSentInvites = GUILayout.Toggle(bExpandSentInvites, "Invites Sent"))
		{
			for (int k = 0; k < 6; k++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.Box(GUIContent.none, "whitebox", GUILayout.Width(30f), GUILayout.Height(30f));
				GUILayout.Space(5f);
				GUILayout.BeginVertical();
				GUILayout.Space(-2f);
				GUILayout.Label("Potential Friend:" + k);
				GUILayout.Space(-12f);
				GUILayout.Label(string.Empty);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
		if (bExpandReceivedInvites = GUILayout.Toggle(bExpandReceivedInvites, "Invites Received"))
		{
			for (int l = 0; l < 6; l++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.Box(GUIContent.none, "whitebox", GUILayout.Width(30f), GUILayout.Height(30f));
				GUILayout.Space(5f);
				GUILayout.BeginVertical();
				GUILayout.Space(-2f);
				GUILayout.Label("Potential Friend:" + l);
				GUILayout.Space(-12f);
				GUILayout.Label(string.Empty);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
		if (bExpandRecentPlaymates = GUILayout.Toggle(bExpandRecentPlaymates, "Recent Battlemates"))
		{
			for (int m = 0; m < 6; m++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.Box(GUIContent.none, "whitebox", GUILayout.Width(30f), GUILayout.Height(30f));
				GUILayout.Space(5f);
				GUILayout.BeginVertical();
				GUILayout.Space(-2f);
				GUILayout.Label("Recent Player:" + m);
				GUILayout.Space(-12f);
				GUILayout.Label(string.Empty);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Label("+ Add Friend");
		lastHover2 = curhover2;
	}

	public void executeSuitPurchase(int suitToBuy)
	{
		string text = GameData.SERVICE_PATH + "/ExonautPlayerBuySuit";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("TEGid", GameData.MyTEGid);
		wWWForm.AddField("exId", GameData.MyExonautId);
		wWWForm.AddField("buySuitId", suitToBuy);
		wWWForm.AddField("toCharge", 1);
		WWW www = new WWW(text, wWWForm);
		Logger.trace("Buying Suit @ " + text);
		StartCoroutine(waitForSuitPurchase(www, suitToBuy));
	}

	private IEnumerator waitForSuitPurchase(WWW www, int suitToBuy)
	{
		yield return www;
		if (www.error == null)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
			XmlNode node = xmlDoc.FirstChild;
			if (node == null)
			{
				Debug.Log("Purchase Authentication was NULL");
				yield break;
			}
			XmlAttributeCollection data = node.Attributes;
			XmlAttribute status = (XmlAttribute)data.GetNamedItem("status");
			string msg = string.Empty;
			if (data.GetNamedItem("message") != null)
			{
				msg = data.GetNamedItem("message").Value;
			}
			switch (status.Value)
			{
			case "success":
				GameData.AddOwnedSuit(suitToBuy);
				GameData.MyTotalCredits -= GameData.getExosuit(suitToBuy).mCost;
				MessageBox.AddMessage("Suit Purchase Success!", "Your Suit was successfully Purchased.", null, true, MessageBox.MessageType.MB_OK, null);
				MessageBox.AddMessage("Equip New Suit?", "Would you like to equip your new suit?", null, false, MessageBox.MessageType.MB_YESNO, tabshowcase.EquipSelected);
				break;
			case "fail":
				MessageBox.AddMessage("Operation Error | Game DB Says You Cannot Purchase", msg, null, true, MessageBox.MessageType.MB_OK, null);
				break;
			}
		}
		else
		{
			MessageBox.AddMessage("ERROR PURCHASING SUIT!", "You Cannot Purchase this suit at this time.", null, true, MessageBox.MessageType.MB_OK, null);
			Debug.Log("@Error Purchasing Suit:" + www.error.ToString());
		}
	}

	public void getMissionInProgressUpdate()
	{
		string url = GameData.SERVICE_PATH + "/ExonautPlayerGetMissionProgress";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("exId", GameData.MyExonautId);
		WWW www = new WWW(url, wWWForm);
		Debug.Log("getMissionInProgressUpdate");
		StartCoroutine(waitForProgUpdate(www));
	}

	private IEnumerator waitForProgUpdate(WWW www)
	{
		while (!www.isDone)
		{
			Debug.Log(www.progress);
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("Mission Progress update\n" + www.text);
		if (www.error == null)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
			XmlNode node = xmlDoc.FirstChild;
			if (node == null)
			{
				Debug.Log("Mission Progress was NULL");
				yield break;
			}
			Debug.Log("Mission Progress update\n" + www.text);
			XmlAttributeCollection data = node.Attributes;
			XmlAttribute status = (XmlAttribute)data.GetNamedItem("status");
			switch (status.Value)
			{
			case "success":
			{
				GameData.LatestMissionsInProgress.Clear();
				XmlNodeList missionsProg = xmlDoc.GetElementsByTagName("mission");
				for (int i = 0; i < missionsProg.Count; i++)
				{
					XmlNode prog = missionsProg.Item(i);
					XmlNodeList pData = prog.ChildNodes;
					Hashtable progData = new Hashtable();
					for (int s = 0; s < pData.Count; s++)
					{
						XmlNode Node = pData.Item(s);
						progData.Add(value: Node.Attributes.GetNamedItem("value").Value, key: Node.Name);
					}
					GameData.addMissionInProgress(progData);
				}
				break;
			}
			case "fail":
				Logger.trace("No Mission Progress update");
				break;
			}
		}
		else
		{
			Logger.trace("No Mission Progress update");
		}
	}

	public void CheckTutorial(bool bOverride)
	{
		Logger.traceError("CheckTutorial(" + bOverride + ")");
		Logger.traceError("GameData.MyPlayStatus = " + GameData.MyPlayStatus);
		if (bOverride)
		{
			GameObject gameObject = GameObject.Find("NetworkManager");
			m_networkManager = gameObject.GetComponent<NetworkManager>();
			m_networkManager.Logout();
			GameData.WorldID = 0;
			GameData.WorldVersion = "tutorial";
			GameData.GameType = 2;
			GameData.MyTutorialStep = 2;
			GameData.MyPlayerId = 1;
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playStatus", GameData.MyPlayStatus);
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutUtfString("playerName", GameData.MyDisplayName);
			sFSObject.PutInt("suitIdx", GameData.MySuitID);
			sFSObject.PutInt("playerFaction", GameData.MyFactionId);
			sFSObject.PutInt("textureIdx", GameData.MyTextureID);
			sFSObject.PutInt("weaponIdx", GameData.MyWeaponID);
			sFSObject.PutInt("powers", GameData.MyPowers);
			sFSObject.PutInt("level", GameData.MyLevel);
			sFSObject.PutBool("leveledUp", val: false);
			GameData.addPlayer(1, sFSObject);
			GameData.LoadWorld();
			GameData.LoadSpawnPoints();
			Application.LoadLevel("TutorialGamePlay");
		}
		else if (GameData.MyTutorialStep == 4)
		{
			if (GameData.MyPlayStatus == 1)
			{
				Object.Instantiate(GuestFirstUse);
				GameData.MyTutorialStep = 1;
			}
			else if (GameData.MyPlayStatus == 2 || GameData.MyPlayStatus == 3)
			{
				Object.Instantiate(RegisteredFirstUse);
				GameData.MyTutorialStep = 1;
			}
		}
	}

	private void OnApplicationQuit()
	{
		Debug.Log("Application Quit - should destroy");
	}
}
