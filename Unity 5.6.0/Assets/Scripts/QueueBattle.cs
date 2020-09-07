using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System.Collections;
using UnityEngine;

public class QueueBattle : MonoBehaviour
{
	private ChatModule chat;

	public GameSummary gameSummary;

	private battleQueueUpdate queueState;

	public Texture2D[] mFactionCards = new Texture2D[2];

	public GUISkin SharedSkin;

	public GUISkin QueueSkin;

	private Rect banzaiGroup;

	private Rect atlasGroup;

	private Rect chatGroup;

	private Rect mapGroup;

	private bool isGameStartReceived;

	public bool isWorldLoaded;

	public GameObject FirstUsePrefab;

	private float waitForHeartbeatTimer = 10f;

	private bool bSinglePlayer;

	public GameObject AtlasMusic;

	public GameObject BanzaiMusic;

	public static Rect screenSpace = new Rect(0f, 0f, 900f, 600f);

	private bool bFullScreen;

	public Texture2D[] mMapImages = new Texture2D[9];

	public Texture2D[] mBanzaiImages = new Texture2D[3];

	public Texture2D[] mAtlasImages = new Texture2D[3];

	public Texture2D[] mBanzaiRanks = new Texture2D[6];

	public Texture2D[] mAtlasRanks = new Texture2D[6];

	public Texture2D[] mFactionRankUpBGs = new Texture2D[2];

	public Texture2D[] mFactionRankUpIcons = new Texture2D[10];

	public Texture2D[] mLargeFactionRankIcons = new Texture2D[12];

	public Texture2D mBattleBackground;

	private Rect[] mBanzaiLogoRects = new Rect[2];

	private Rect[] mAtlasLogoRects = new Rect[2];

	public NetworkManager m_networkManager;

	private string lastHover = string.Empty;

	private int[] playerLoadStatus = new int[8];

	private float loadProgress;

	private void Awake()
	{
		GameData.WorldID = -1;
		Debug.Log("Tutorial: " + bSinglePlayer + " " + GameData.MyTutorialStep);
		if (GameData.MyTutorialStep == 1)
		{
			GameData.WorldID = 0;
			GameData.WorldVersion = "tutorial";
			GameData.GameType = 2;
			GameData.MyTutorialStep = 2;
			bSinglePlayer = true;
			Object.Instantiate(FirstUsePrefab);
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
		}
		else
		{
			isGameStartReceived = false;
			GameData.WorldVersion = "normal";
			GameData.GameType = 1;
		}
		SharedSkin = GUIUtil.mInstance.mSharedSkin;
		QueueSkin = GUIUtil.mInstance.mQueueSkin;
		GameObject gameObject = GameObject.Find("NetworkManager");
		m_networkManager = (gameObject.GetComponent("NetworkManager") as NetworkManager);
		GameObject gameObject2 = GameObject.Find("Tracker");
		TrackerScript trackerScript = gameObject2.GetComponent("TrackerScript") as TrackerScript;
		trackerScript.AddMetric(TrackerScript.Metric.IN_LOBBY);
	}

	private void Start()
	{
		QualitySettings.currentLevel = (QualityLevel)(3 + GameData.mGameSettings.mGraphicsLevel);
		StartCoroutine(UpdateScreenSpace());
		DynamicOptions.bDrawCursor = true;
		if (GameObject.Find("GameMusic(Clone)") != null)
		{
			Object.Destroy(GameObject.Find("GameMusic(Clone)"));
		}
		if (GameData.MyFactionId == 1)
		{
			if (GameObject.Find("BanzaiMusic(Clone)") == null)
			{
				BanzaiMusic = (Object.Instantiate(BanzaiMusic) as GameObject);
				BanzaiMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			}
			else
			{
				BanzaiMusic = GameObject.Find("BanzaiMusic(Clone)");
				BanzaiMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			}
		}
		else if (GameData.MyFactionId == 2)
		{
			if (GameObject.Find("AtlasMusic(Clone)") == null)
			{
				AtlasMusic = (Object.Instantiate(AtlasMusic) as GameObject);
				AtlasMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			}
			else
			{
				AtlasMusic = GameObject.Find("AtlasMusic(Clone)");
				AtlasMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			}
		}
		gameSummary.Start(this);
		chat = new ChatModule(this);
		queueState = initMyQueue;
		Camera.main.backgroundColor = Color.black;
		StartCoroutine(HolidayEvent.OnQueueStart());
	}

	public IEnumerator UpdateScreenSpace()
	{
		yield return new WaitForEndOfFrame();
		screenSpace = new Rect(0f, 0f, Screen.width, Screen.height);
		MessageBox.ResetWindowPosition();
		mapGroup = new Rect(0f, 0f, screenSpace.width, (int)(screenSpace.width * (3f / 32f)));
		float ChatHeight = (float)((int)(screenSpace.height - mapGroup.height - 350f) * 600) / screenSpace.height;
		chatGroup = new Rect(0f, screenSpace.height - ChatHeight, screenSpace.width, ChatHeight);
		banzaiGroup = new Rect(Mathf.RoundToInt((screenSpace.height - mapGroup.height - chatGroup.height - 12f) / 2f), 0f, mapGroup.height + 10f, screenSpace.width);
		atlasGroup = new Rect(0f, banzaiGroup.y + banzaiGroup.height + 1f, screenSpace.width, banzaiGroup.height);
		mBanzaiLogoRects[0] = new Rect(0f, 0f, (float)mBanzaiImages[0].width * banzaiGroup.height / (float)mBanzaiImages[0].height, banzaiGroup.height);
		mBanzaiLogoRects[1] = new Rect(banzaiGroup.width - (float)mBanzaiImages[1].width * banzaiGroup.height / (float)mBanzaiImages[1].height, 0f, (float)mBanzaiImages[1].width * banzaiGroup.height / (float)mBanzaiImages[1].height, banzaiGroup.height);
		mAtlasLogoRects[0] = new Rect(0f, 0f, (float)mAtlasImages[0].width * atlasGroup.height / (float)mAtlasImages[0].height, atlasGroup.height);
		mAtlasLogoRects[1] = new Rect(atlasGroup.width - (float)mAtlasImages[1].width * atlasGroup.height / (float)mAtlasImages[1].height, 0f, (float)mAtlasImages[1].width * atlasGroup.height / (float)mAtlasImages[1].height, atlasGroup.height);
	}

	private void Update()
	{
		if (m_networkManager.currentActiveRoom == null && GameData.MyTutorialStep <= 0)
		{
			HandlePlayerQuit();
			return;
		}
		sendHeartbeat();
		chat.OnUpdate();
		if (Input.GetKeyDown(KeyCode.LeftControl) || (Input.GetKeyDown(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.F)))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
		if (bFullScreen != Screen.fullScreen || screenSpace.width != (float)Screen.width || screenSpace.height != (float)Screen.height)
		{
			StartCoroutine(UpdateScreenSpace());
		}
	}

	private void FixedUpdate()
	{
		queueState();
	}

	private void initMyQueue()
	{
		Logger.trace("### INIT MY QUEUE ###");
		if (m_networkManager == null)
		{
			Debug.Log("[QueueBattle::initMyQueue] network Manager is null");
		}
		if (m_networkManager.currentActiveRoom != null)
		{
			foreach (User user in m_networkManager.currentActiveRoom.UserList)
			{
				if (user != null)
				{
					int factionId = (!(Factions.atlas.ToString() == user.GetVariable("faction").GetStringValue())) ? 1 : 2;
					addPlayerToSlot(factionId, user.PlayerId);
				}
			}
			if (!bSinglePlayer && m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("play"))
			{
				m_networkManager.m_qTime = 0;
			}
			queueState = LoadMyWorld;
		}
	}

	private void LoadMyWorld()
	{
		if (!bSinglePlayer)
		{
			Debug.Log("<< check for loading world " + m_networkManager.getMapId());
			GameData.WorldID = m_networkManager.getMapId();
			if (GameData.WorldID == -1)
			{
				return;
			}
		}
		Debug.Log("[QueueBattle::LoadMyWorld] want to load map: " + m_networkManager.getMapId());
		GameData.LoadWorld();
		isWorldLoaded = false;
		queueState = waitForWorldLoad;
	}

	private void waitForWorldLoad()
	{
		if (GameData.getWorldById(GameData.WorldID) != null)
		{
			Logger.traceAlways("[QueueBattle::waitForWorldLoad] - Loaded");
			chat.AddSystemMessage(GameData.WorldName + " Loaded!");
			TextAsset playerSpawnPointsById = GameData.getPlayerSpawnPointsById(GameData.WorldID - 1, GameData.BattleType - 1, GameData.GameType - 1);
			if (playerSpawnPointsById == null)
			{
				GameData.LoadSpawnPoints();
			}
			else
			{
				GameData.parseAllSpawnPoints();
			}
			queueState = waitForSpawnPointsLoad;
		}
	}

	private void waitForSpawnPointsLoad()
	{
		if (GameData.PickupSpawns != null)
		{
			Debug.Log("[QueueBattle::waitForSpawnPointsLoad]");
			chat.AddSystemMessage(" Spawn Points Loaded!");
			isWorldLoaded = true;
			queueState = amIReady;
		}
	}

	private void amIReady()
	{
		queueState = waitForGameStart;
		if (bSinglePlayer)
		{
			chat.AddSystemMessage("Single Player Called Ready!");
			gameStartReceived();
			queueState = waitForGameStart;
		}
		else
		{
			chat.AddSystemMessage("Waiting for isGameStartReceived " + m_networkManager.m_qTime);
		}
	}

	private void waitForGameStart()
	{
		if (!m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("wait_for_min_players") && (isGameStartReceived || m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("play")))
		{
			m_networkManager.sendClientState("ready");
			if (m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("play"))
			{
				chat.AddSystemMessage("Starting Game In Progress . . .");
			}
			else
			{
				chat.AddSystemMessage("Starting Game . . .");
			}
			GameObject gameObject = GameObject.Find("Tracker");
			TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
			trackerScript.AddMetric(TrackerScript.Metric.MATCH_READY);
			if (m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("play"))
			{
				trackerScript.AddMetric(TrackerScript.Metric.DROP_INTO_MATCH);
			}
			Object.DestroyObject(AtlasMusic);
			Object.DestroyObject(BanzaiMusic);
			StartCoroutine(LoadGameAndWait());
			queueState = pause;
		}
	}

	private IEnumerator LoadGameAndWait()
	{
		AsyncOperation async = Application.LoadLevelAsync("GamePlay");
		yield return async.isDone;
		chat.AddSystemMessage("Get Ready To Play");
	}

	private void pause()
	{
	}

	private void OnGUI()
	{
		if (GameData.WorldID == -1)
		{
			return;
		}
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		GUI.depth = 5;
		GUI.BeginGroup(screenSpace);
		GUI.DrawTexture(mapGroup, mMapImages[GameData.WorldID]);
		GUI.BeginGroup(mapGroup);
		string text = "BATTLE";
		string empty = string.Empty;
		if (m_networkManager.currentActiveRoom != null)
		{
			int intValue = m_networkManager.currentActiveRoom.GetVariable("hackLimit").GetIntValue();
			empty = "MATCH RULES: " + intValue + " HACKS OR 10 MINUTES";
			if (GameData.BattleType == 2)
			{
				text = "TEAM BATTLE";
				empty = "MATCH RULES: " + intValue + " HACKS OR 15 MINUTES";
			}
			if (GameData.BattleType == 3)
			{
				text = "BUDDY BATTLE";
				empty = "MATCH RULES: BUDDY BATTLE RULESET NOT DEFINED.";
			}
		}
		else
		{
			text = "TRAINING";
			empty = "COMPLETE THE TRAINING MISSION";
		}
		Vector2 vector = QueueSkin.GetStyle("MapNameText").CalcSize(new GUIContent(GameData.WorldName.ToUpper()));
		GUI.Box(new Rect(5f, 5f, vector.x, 30f), GameData.WorldName.ToUpper(), QueueSkin.GetStyle("MapNameText"));
		Vector2 vector2 = QueueSkin.GetStyle("BattleText").CalcSize(new GUIContent(text));
		GUI.Box(new Rect((mapGroup.width - vector2.x) / 2f, 15f, vector2.x, vector2.y), text, QueueSkin.GetStyle("BattleText"));
		Vector2 vector3 = QueueSkin.GetStyle("MatchRulesText").CalcSize(new GUIContent(empty));
		GUI.Box(new Rect((mapGroup.width - vector3.x) / 2f, 55f, vector3.x, vector3.y), empty, QueueSkin.GetStyle("MatchRulesText"));
		string text2 = "WAITING...";
		string str = string.Format("{0:D2}", m_networkManager.m_qTime);
		if (GameData.getWorldById(GameData.WorldID) == null)
		{
			text2 = "LOADING WORLD";
			Debug.Log("[QueueBattle::OnGUI] World ID is null " + GameData.WorldID + " isworldLoaded: " + isWorldLoaded);
		}
		else if (m_networkManager.currentActiveRoom != null)
		{
			if (!m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("play") || 1 == 0)
			{
				text2 = ((!m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("wait_for_min_players")) ? ("LAUNCHING GAME 0:" + str) : "WAITING FOR MORE PLAYERS");
			}
			else
			{
				Debug.Log("<<<< showing waiting because game is in: " + m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue());
			}
		}
		GUI.Box(new Rect(mapGroup.width - 200f - 5f - 80f, 5f, 24f, 300f), text2, QueueSkin.GetStyle("WaitingText"));
		GUI.EndGroup();
		GUI.Box(new Rect(0f, mapGroup.height, screenSpace.width, banzaiGroup.y - mapGroup.height), string.Empty, QueueSkin.GetStyle("BlackBox"));
		if (GameData.BattleType != 2)
		{
			GUI.DrawTexture(new Rect(banzaiGroup.x, banzaiGroup.y, banzaiGroup.width + atlasGroup.width + 2f, banzaiGroup.height + atlasGroup.height + 2f), mBattleBackground);
		}
		float num = 160f;
		float num2 = 3f;
		float num3 = banzaiGroup.width / 2f - (num * 2f + num2 * 1.5f);
		GameData.playerSlots1.Clear();
		GameData.playerSlots2.Clear();
		if (m_networkManager.currentActiveRoom != null)
		{
			foreach (User user in m_networkManager.currentActiveRoom.UserList)
			{
				int factionId = (!(Factions.atlas.ToString() == user.GetVariable("faction").GetStringValue())) ? 1 : 2;
				addPlayerToSlot(factionId, user.PlayerId);
			}
		}
		if (GameData.BattleType != 2)
		{
			for (int i = 0; i < 8; i++)
			{
				int num4 = i % 2;
				int num5 = 0;
				if (GameData.playerSlots1.Contains(i + 1))
				{
					num5 = i + 1;
				}
				int num6 = i / 2;
				if (num4 == 0)
				{
					GUI.BeginGroup(banzaiGroup);
					Rect position = new Rect(num3 + (num + num2) * (float)num6, (banzaiGroup.height - num) / 2f, num, num);
					showLoadoutCard(num5, position, m_networkManager.getFactionByID(num5));
					GUI.EndGroup();
				}
				else
				{
					GUI.BeginGroup(atlasGroup);
					Rect position2 = new Rect(num3 + (num + num2) * (float)num6, (banzaiGroup.height - num) / 2f, num, num);
					showLoadoutCard(num5, position2, m_networkManager.getFactionByID(num5));
					GUI.EndGroup();
				}
			}
		}
		else
		{
			GUI.BeginGroup(banzaiGroup);
			if (GameData.BattleType == 2)
			{
				GUI.DrawTexture(new Rect(0f, 0f, banzaiGroup.width, banzaiGroup.height), mBanzaiImages[2]);
				GUI.DrawTexture(mBanzaiLogoRects[0], mBanzaiImages[0]);
				GUI.DrawTexture(mBanzaiLogoRects[1], mBanzaiImages[1]);
			}
			int count = GameData.playerSlots1.Count;
			for (int j = 0; j < 4; j++)
			{
				int playerId = 0;
				if (j < count)
				{
					playerId = GameData.playerSlots1[j];
				}
				Rect position3 = new Rect(num3 + (num + num2) * (float)j, (banzaiGroup.height - num) / 2f, num, num);
				showLoadoutCard(playerId, position3, (GameData.BattleType == 2) ? 1 : 0);
			}
			if (GameData.BattleType == 2)
			{
				GUI.Box(new Rect(0f, 0f, banzaiGroup.width, banzaiGroup.height), string.Empty, QueueSkin.GetStyle("BanzaiOutline"));
			}
			GUI.EndGroup();
			GUI.BeginGroup(atlasGroup);
			if (GameData.BattleType == 2)
			{
				GUI.DrawTexture(new Rect(0f, 0f, banzaiGroup.width, banzaiGroup.height), mAtlasImages[2]);
				GUI.DrawTexture(mAtlasLogoRects[0], mAtlasImages[0]);
				GUI.DrawTexture(mAtlasLogoRects[1], mAtlasImages[1]);
			}
			int count2 = GameData.playerSlots2.Count;
			for (int k = 0; k < 4; k++)
			{
				int playerId2 = 0;
				if (k < count2)
				{
					playerId2 = GameData.playerSlots2[k];
				}
				Rect position4 = new Rect(num3 + (num + num2) * (float)k, (banzaiGroup.height - num) / 2f, num, num);
				showLoadoutCard(playerId2, position4, (GameData.BattleType == 2) ? 2 : 0);
			}
			if (GameData.BattleType == 2)
			{
				GUI.Box(new Rect(0f, 0f, banzaiGroup.width, banzaiGroup.height), string.Empty, QueueSkin.GetStyle("AtlasOutline"));
			}
			GUI.EndGroup();
		}
		GUI.BeginGroup(chatGroup);
		chat.drawChatWindow(chatGroup);
		switch (GUIUtil.Button(new Rect(chatGroup.width - 160f, chatGroup.height - 47f, 67f, 34f), "STATS", QueueSkin.GetStyle("Quit_Button")))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "STATS";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "STATS";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			gameSummary.mDraw = true;
			break;
		}
		switch (GUIUtil.Button(new Rect(chatGroup.width - 85f, chatGroup.height - 47f, 67f, 34f), "QUIT", QueueSkin.GetStyle("Quit_Button")))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "QUIT";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "QUIT";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			HandlePlayerQuit();
			break;
		}
		GUIUtil.GUIEnable(bEnable: true);
		GUI.EndGroup();
		if (GameData.CurPlayState == GameData.PlayState.GAME_SUMMARY_RECEIVED)
		{
			gameSummary.TakeSnapshot();
			gameSummary.mDraw = true;
			GameData.CurPlayState = GameData.PlayState.GAME_IN_QUEUE;
		}
		if (gameSummary.mDraw)
		{
			gameSummary.Draw();
		}
		lastHover = b;
		GUI.EndGroup();
	}

	public void addPlayerToSlot(int factionId, int playerId)
	{
		if (isPlayerInSlot(playerId))
		{
			Debug.Log("## player is already in slot " + playerId);
			return;
		}
		if (GameData.BattleType == 1 || GameData.BattleType == 3)
		{
			GameData.playerSlots1.Add(playerId);
			return;
		}
		switch (factionId)
		{
		case 1:
			GameData.playerSlots1.Add(playerId);
			break;
		case 2:
			GameData.playerSlots2.Add(playerId);
			break;
		}
	}

	private bool isPlayerInSlot(int playerId)
	{
		if (GameData.playerSlots1.Contains(playerId))
		{
			return true;
		}
		if (GameData.playerSlots2.Contains(playerId))
		{
			return true;
		}
		return false;
	}

	private void removePlayerFromSlot(int playerId)
	{
		Logger.trace("## removing player from slot");
		if (GameData.playerSlots1.Contains(playerId))
		{
			GameData.playerSlots1.Remove(playerId);
		}
		else if (GameData.playerSlots2.Contains(playerId))
		{
			GameData.playerSlots2.Remove(playerId);
		}
	}

	public void addChatMessage(string chatMessage)
	{
		chat.AddChatMessage(chatMessage);
	}

	public void addFactionChatMessage(string chatMessage, int senderFaction)
	{
		chat.AddFactionChatMessage(chatMessage, senderFaction);
	}

	public void gameStartReceived()
	{
		isGameStartReceived = true;
		Debug.Log("<< gameStartReceived");
	}

	public void UpdatePlayerLoadStatus(int playerId, int status)
	{
		Logger.trace("<< playerId: " + playerId);
		playerLoadStatus[playerId - 1] = status;
	}

	private void setAllPlayersLoadStatusFull()
	{
		for (int i = 0; i < 8; i++)
		{
			playerLoadStatus[i] = 100;
		}
	}

	public void sendHeartbeat()
	{
		waitForHeartbeatTimer -= Time.deltaTime;
		if (waitForHeartbeatTimer <= 0f)
		{
			m_networkManager.sendPing(submit: true);
			waitForHeartbeatTimer = 10f;
		}
	}

	private void HandlePlayerQuit()
	{
		m_networkManager.LeaveRoom();
		GameData.clearAllSfsPlayers();
		GameData.DestroyCurrentGame();
		GameData.CurPlayState = GameData.PlayState.GAME_IS_QUITTING;
		Application.LoadLevel("GameHome");
	}

	private void OnApplicationQuit()
	{
		Logger.traceError("QueueBattle::OnApplicationQuit");
	}

	private void LoadSuit(int suitId, string model_type)
	{
		Exosuit exosuit = GameData.getExosuit(suitId);
		string mSuitFileName = exosuit.mSuitFileName;
		string text = mSuitFileName + "_" + model_type + ".unity3d";
		string url = GameData.BUNDLE_PATH + "suits/" + text;
		Logger.trace("<< load suit: " + text);
		WWW www = new WWW(url);
		StartCoroutine(WaitForSuitRequest(www, exosuit, model_type));
	}

	private IEnumerator WaitForSuitRequest(WWW www, Exosuit suit, string model_type)
	{
		yield return www;
		float LastProgress = 0f;
		loadProgress -= LastProgress;
		LastProgress = (float)www.size * www.progress;
		loadProgress += LastProgress;
		Logger.trace("my load progress = " + loadProgress);
		Logger.trace("WWW load progress = " + www.progress);
		if (www.error == null)
		{
			yield return www;
			AssetBundle assetBundle = www.assetBundle;
			string fileName = suit.mSuitFileName;
			Logger.trace("Load Model : " + fileName + "_" + model_type + "_pre");
			AssetBundleRequest abr2 = assetBundle.LoadAssetAsync(fileName + "_" + model_type + "_pre", typeof(GameObject));
			Logger.trace("loading... " + abr2.progress);
			yield return abr2;
			GameObject suitModel = abr2.asset as GameObject;
			if (suitModel != null)
			{
				Logger.trace("Model Loaded! " + suitModel.name);
			}
			else
			{
				Logger.trace("Could not load suit");
			}
			string textureName = fileName + "_sheet_1";
			Logger.trace("Load Texture : " + textureName);
			abr2 = assetBundle.LoadAssetAsync(textureName, typeof(Material));
			Logger.trace("loading..." + abr2.progress);
			yield return abr2;
			Logger.trace("Texture Loaded!");
			GameData.setLowPolySuitIsLoaded(texture: abr2.asset as Material, suitId: suit.mSuitId, model: suitModel);
		}
		else
		{
			Logger.trace("<< there was an error " + www.error);
		}
	}

	public void showLoadoutCard(int playerId, Rect Position, int Faction)
	{
		Room currentActiveRoom = m_networkManager.currentActiveRoom;
		User user = null;
		if (currentActiveRoom != null)
		{
			foreach (User user2 in currentActiveRoom.UserList)
			{
				if (playerId == user2.PlayerId)
				{
					user = user2;
					break;
				}
			}
		}
		if (user == null)
		{
			switch (Faction)
			{
			case 1:
				GUI.Box(Position, "WAITING ON PLAYER...", QueueSkin.GetStyle("EmptyBanzaiPlayerCard"));
				break;
			case 2:
				GUI.Box(Position, "WAITING ON PLAYER...", QueueSkin.GetStyle("EmptyAtlasPlayerCard"));
				break;
			default:
				GUI.Box(Position, "WAITING ON PLAYER...", QueueSkin.GetStyle("EmptyPlayerCard"));
				break;
			}
			return;
		}
		if (user.IsItMe)
		{
			GUI.Box(new Rect(Position.x - 2f, Position.y - 2f, Position.width + 4f, Position.height + 4f), GUIContent.none, SharedSkin.GetStyle("whitebox"));
		}
		string text = null;
		if (user.GetVariable("nickName") != null)
		{
			text = user.GetVariable("nickName").GetStringValue();
			if (text == null)
			{
				text = "INVALID";
			}
		}
		string[] array = text.Split(' ');
		text = array[0];
		if (array.Length > 1)
		{
			text = text + " " + array[1];
		}
		Exosuit exosuit = GameData.getExosuit(user.GetVariable("suitId").GetIntValue());
		if (exosuit != null && exosuit.mGuiLoadoutImage != null)
		{
			if (user.IsItMe)
			{
				GUI.color = new Color(0.25f, 0.25f, 0.25f);
			}
			GUI.color = Color.white;
			GUI.DrawTexture(Position, exosuit.mGuiLoadoutImage);
			GUI.color = Color.white;
		}
		switch (Faction)
		{
		case 1:
			GUI.Box(Position, text.ToUpper(), QueueSkin.GetStyle("BanzaiPlayerCard"));
			break;
		case 2:
			GUI.Box(Position, text.ToUpper(), QueueSkin.GetStyle("AtlasPlayerCard"));
			break;
		default:
			GUI.Box(Position, text.ToUpper(), QueueSkin.GetStyle("BattlePlayerCard"));
			break;
		}
		if (user.IsItMe)
		{
			GUI.Box(Position, GUIContent.none, QueueSkin.GetStyle("YouTag"));
		}
		int intValue = user.GetVariable("level").GetIntValue();
		string text2 = intValue.ToString();
		if (text2.Length == 1)
		{
			text2 = "0" + text2;
		}
		GUI.Label(Position, text2, QueueSkin.GetStyle("Level_Text"));
		Position.x += Position.width - 70f;
		Position.y += Position.height - 47f;
		Position.width = 40f;
		Position.height = 28f;
		if (Faction == 1)
		{
			GUI.DrawTexture(Position, mBanzaiRanks[intValue / 10]);
		}
		else
		{
			GUI.DrawTexture(Position, mAtlasRanks[intValue / 10]);
		}
	}
}
