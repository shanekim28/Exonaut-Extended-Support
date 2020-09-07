using Sfs2X;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
	public struct ambientSoundInfo
	{
		public AudioSource mSound;

		public float mMaxVolume;

		public ambientSoundInfo(AudioSource sound, float maxVolume)
		{
			mSound = sound;
			mMaxVolume = maxVolume;
		}
	}

	private struct pickupDef
	{
		public string puName;

		public int puType;

		public pickupDef(string puName, int puType, Vector3 pos)
		{
			this.puName = puName;
			this.puType = puType;
		}
	}

	protected delegate void State();

	public const int boost_armor = 0;

	public const int boost_damage = 1;

	public const int boost_invis = 2;

	public const int boost_speed = 3;

	public const int pickup_lobber = 5;

	public const int pickup_sniper = 4;

	public const int pickup_rocket = 6;

	public const int pickup_grenades = 7;

	public const int boost_random = 8;

	public const int boost_team_armor = 10;

	public const int boost_team_damage = 11;

	public const int boost_team_invis = 12;

	public const int boost_team_speed = 13;

	public const int WEAPON_PISTOL = 1;

	public const int WEAPON_RIFLE = 2;

	public const int WEAPON_SHOTGUN = 3;

	public const int WEAPON_SMG = 4;

	public const int WEAPON_SPREAD = 5;

	public const int WEAPON_LOBBER = 6;

	public const int WEAPON_SNIPER = 7;

	public const int WEAPON_ROCKET = 8;

	public const int WEAPON_GRENADE_THROW = 9;

	public string version = "1.0";

	public bool m_showStats;

	public bool m_wasGoingToLoadWrongMap;

	public int m_statPage;

	protected SmartFox sfs;

	public bool isPaused;

	public bool mGameReady;

	public bool mImQuitting;

	public bool mIsBattleStartPaused = true;

	public bool mIsBattleOver;

	public bool mIsBattleSummaryReceived;

	public string mText = string.Empty;

	public int numFramesSinceMessage;

	public float waitForWeaponSelectTimer = 5f;

	public float mBattleMessageTimer;

	public bool amFullScreen;

	public int currentQuality;

	public Rect screenSpace = new Rect(0f, 0f, 900f, 600f);

	private float lastScreenWidth = 900f;

	private float lastScreenHeight = 600f;

	public GameObject myCamera;

	public Material mInvisibleMat;

	public int mNumBanzaiCapturesLeft;

	public int mNumAtlasCapturesLeft;

	public GameHUD mHUD;

	public GameObject bullet;

	public GameObject grenade;

	public GameObject event_lobber_grenade;

	public GameObject event_banzai_grenade;

	public GameObject event_banzai_bubble;

	public GameObject event_banzai_pistol_bubble;

	public GameObject event_banzai_rifle_bubble;

	public GameObject event_banzai_shotgun_bubble;

	public GameObject event_banzai_smg_bubble;

	public GameObject event_banzai_spread_bubble;

	public GameObject event_banzai_lobber_bubble;

	public GameObject event_banzai_sniper_bubble;

	public GameObject event_banzai_rocket_bubble;

	public GameObject event_banzai_grenade_bubble;

	public GameObject event_atlas_grenade;

	public GameObject event_atlas_bubble;

	public GameObject event_atlas_pistol_bubble;

	public GameObject event_atlas_rifle_bubble;

	public GameObject event_atlas_shotgun_bubble;

	public GameObject event_atlas_smg_bubble;

	public GameObject event_atlas_spread_bubble;

	public GameObject event_atlas_lobber_bubble;

	public GameObject event_atlas_sniper_bubble;

	public GameObject event_atlas_rocket_bubble;

	public GameObject event_atlas_grenade_bubble;

	public GameObject mCaptureBubble;

	public GameObject rocket;

	public GameObject hitspark1;

	public GameObject hitspark2;

	public GameObject pickupEffect;

	public GameObject healthEffect;

	public GameObject rootedEffect;

	public GameObject lowHealthEffect;

	public Texture2D teamBoostTexture;

	public GameObject mGrenadeExplosion;

	public GameObject orbitalDamageEffect;

	public GameObject armorEffect;

	public GameObject damageEffect;

	public GameObject speedEffect;

	public List<GameObject> hitsparks;

	public AudioClip jetpackIgnite;

	public AudioClip jetpackLoop;

	public List<AudioClip> weaponSounds;

	public List<AudioClip> hitPlayerSounds;

	public List<AudioClip> hitWallSounds;

	public List<AudioClip> footstepSounds;

	public List<AudioClip> explosionSounds;

	public List<AudioClip> reloadSounds;

	public List<AudioClip> boostSounds;

	public AudioClip levelMusic;

	public AudioClip criticalHitSnd;

	public AudioClip jetpackFuelEmptySnd;

	public AudioClip jetpackFuelLowLoop;

	public AudioClip sniperHitSnd;

	public AudioClip activateSniperSnd;

	public AudioClip deactivateSniperSnd;

	public AudioClip mouseOverBtnSnd;

	public AudioClip mousePressBtnSnd;

	public ArrayList doNotInstantiateBulletList;

	public GameObject myPlayer;

	public Player myGamePlayer;

	public GameObject mHeadshotEmitter;

	protected bool allPlayersSpawned;

	public GameObject[] players = new GameObject[8];

	public GameObject mSnipingTarget;

	public float mGameTimer;

	public float mTotalGameTime;

	public NetworkManager m_networkManager;

	private List<ambientSoundInfo> mWorldSounds = new List<ambientSoundInfo>();

	protected State state;

	public GameObject[] muzzleflash = new GameObject[8];

	public GameObject muzzleflash_lite;

	public static bool showFPS = true;

	public CameraScrolling cameraScrolling;

	public Hashtable mUsedSpawnPts;

	public Texture2D mMapImage;

	public Texture2D mPregameBG;

	public Texture2D mControlsImage;

	public GUIStyle mMapNameStyle;

	public GUIStyle mTypeStyle;

	public GUIStyle mInfoStyle;

	public GUIStyle mStatusStyle;

	public GUIStyle mTipStyle;

	public string mLoadingTip = string.Empty;

	public GameObject mSniperLine;

	public static bool mPauseScreenActive;

	private string[] mWorldSoundList = new string[27]
	{
		"sfx_coldWindLoop",
		"sfx_computerTerminal",
		"sfx_crystalHumLoop",
		"sfx_crystalHumLoop1",
		"sfx_crystalHumLoop2",
		"sfx_vkCastle_clanky_drones2",
		"sfx_vkCastle_distant_echoes",
		"sfx_vkCastle_wind_leaves_loop",
		"sfx_harborAmbience",
		"sfx_machineryLoop",
		"sfx_steamLoop",
		"sfx_steamLoop2",
		"sfx_waterMotor",
		"sfx_ambient_adventure_time1",
		"sfx_ambient_adventure_time2",
		"sfx_ambient_adventure_time3",
		"sfx_ambient_adventure_time4",
		"platformJetSound2",
		"lavaSteamSound1",
		"lavaSteamSound2",
		"piano_music",
		"platformSpaceySound1",
		"platformSpaceySound2",
		"platformSpaceySound3",
		"electricFence1",
		"electricFence2",
		"beamWeaponSound4"
	};

	public bool[] arrowPosition = new bool[8];

	public bool mMusicOn;

	protected float mHoldMessageTimer;

	public AudioSource mLevelMusicSource;

	protected static float messageTimer;

	protected string stateStatus = string.Empty;

	public bool serverReady;

	public static GamePlay GetGamePlayScript()
	{
		GamePlay gamePlay = null;
		GameObject gameObject = GameObject.Find("Game");
		if (gameObject != null)
		{
			gamePlay = (gameObject.GetComponent("GamePlay") as GamePlay);
			if (gamePlay == null)
			{
				gamePlay = (gameObject.GetComponent("TutorialGamePlay") as TutorialGamePlay);
			}
		}
		return gamePlay;
	}

	[Obsolete]
	protected virtual void LoadLevel(string level)
	{
		Application.LoadLevel(level);
	}

	private void Awake()
	{
		sfs = SmartFoxConnection.Connection;
		for (int i = 0; i < 8; i++)
		{
			players[i] = null;
		}
		state = InitGame;
		GameObject gameObject = GameObject.Find("NetworkManager");
		m_networkManager = (gameObject.GetComponent("NetworkManager") as NetworkManager);
		doNotInstantiateBulletList = new ArrayList();
		mImQuitting = false;
		Camera.main.renderingPath = RenderingPath.Forward;
	}

	[Obsolete]
	private void Start()
	{
		myPlayer = null;
		StartCoroutine(UpdateScreenSpace());
		QualitySettings.currentLevel = (QualityLevel)GameData.mGameSettings.mGraphicsLevel;
		mHoldMessageTimer = 2f;
		Application.runInBackground = true;
		m_showStats = false;
		m_wasGoingToLoadWrongMap = false;
		mPauseScreenActive = false;
		mMusicOn = true;
		string str = string.Empty;
		if (GameData.WorldID >= 1)
		{
			GameData.MyTutorialStep = 0;
			if (m_networkManager.currentActiveRoom.GetVariable("mapId") != null)
			{
				if (GameData.WorldID != m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue())
				{
					Logger.traceAlways("[GamePlay::Start] -- THIS IS WHERE THE WRONG WORLD WOULD BE LOADED **");
					m_wasGoingToLoadWrongMap = true;
				}
				GameData.WorldID = m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue();
			}
			if (m_networkManager != null && m_networkManager.user != null && m_networkManager.currentActiveRoom != null && m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue() != m_networkManager.user.GetVariable("lastMapLoadedId").GetIntValue())
			{
				Debug.Log("[GamePlay::Start] was going to play with wrong map" + m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue() + ", last: " + m_networkManager.currentActiveRoom.GetVariable("lastMapLoadedId").GetIntValue());
				QuitGame(7);
				return;
			}
		}
		mNumBanzaiCapturesLeft = 0;
		mNumAtlasCapturesLeft = 0;
		switch (GameData.WorldID)
		{
		case 1:
			str = "CN_Exonaut_SnowLab_M";
			break;
		case 2:
			str = "CN_Exonaut_Botaculous_M";
			break;
		case 3:
			str = "CN_Exonaut_Submarine_M";
			break;
		case 4:
			str = "CN_Exonaut_Treefort_M";
			break;
		case 5:
			str = "CN_Exonaut_Perplexahedron_M";
			break;
		case 6:
			str = "CN_Exonaut_BlingLair_M";
			break;
		case 7:
			str = "CN_Exonaut_Danger_Scape_M";
			break;
		case 8:
			str = "CN_Exonaut_Gumball_Carnival_M";
			break;
		case 9:
			str = "CN_Exonaut_Danger_Scape_M";
			break;
		}
		if (GameData.BattleType == 2)
		{
			Physics.IgnoreLayerCollision(11, 12);
			Physics.IgnoreLayerCollision(11, 11);
			Physics.IgnoreLayerCollision(12, 12);
			Physics.IgnoreLayerCollision(11, 14);
			Physics.IgnoreLayerCollision(12, 14);
		}
		else
		{
			Physics.IgnoreLayerCollision(10, 10);
			Physics.IgnoreLayerCollision(10, 14);
		}
		levelMusic = (Resources.Load("Music/" + str) as AudioClip);
		mLevelMusicSource = (Camera.main.GetComponent("AudioSource") as AudioSource);
		mLevelMusicSource.clip = levelMusic;
		mLevelMusicSource.loop = true;
		mLevelMusicSource.volume = GameData.mGameSettings.mMusicVolume;
		if (mMusicOn)
		{
			mLevelMusicSource.Play();
		}
		mSnipingTarget = new GameObject();
		mSniperLine = (Resources.Load("effects/sniper_line/sniper_line") as GameObject);
		muzzleflash_lite = (Resources.Load("weapons/muzzleFlash_lit_pre") as GameObject);
		lowHealthEffect = (Resources.Load("effects/low_health/lightning_low_health_pre") as GameObject);
		mHeadshotEmitter = (Resources.Load("effects/criticalHeadshotEmitterAssets/criticalHeadshotEmitter_pre") as GameObject);
		string path = "projectiles/grenade_explode_emit";
		if (GameData.DoesEventExist("grenade_explosion"))
		{
			mGrenadeExplosion = (GameData.eventObjects["grenade_explosion"] as GameObject);
		}
		else
		{
			mGrenadeExplosion = (Resources.Load(path) as GameObject);
		}
		mCaptureBubble = (Resources.Load("capture_bubble_new/capture_bubble") as GameObject);
		mUsedSpawnPts = new Hashtable();
		cameraScrolling = (Camera.main.GetComponent("CameraScrolling") as CameraScrolling);
		mLoadingTip = GUIUtil.GetRandomTip();
	}

	public IEnumerator UpdateScreenSpace()
	{
		yield return new WaitForEndOfFrame();
		amFullScreen = Screen.fullScreen;
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		float ar = (float)Screen.width / (float)Screen.height;
		float variance2 = ar / 1.5f;
		if (variance2 > 1f)
		{
			variance2 = 1f / variance2;
			Camera.main.rect = new Rect((1f - variance2) / 2f, 0f, variance2, 1f);
			screenSpace = new Rect((int)((1f - variance2) / 2f * (float)Screen.width), 0f, (int)((float)Screen.width - (1f - variance2) * (float)Screen.width), Screen.height);
		}
		else if (variance2 < 1f)
		{
			screenSpace = new Rect(0f, (int)((1f - variance2) / 2f * (float)Screen.height), Screen.width, (int)((float)Screen.height - (1f - variance2) * (float)Screen.height));
			Camera.main.rect = new Rect(0f, (1f - variance2) / 2f, 1f, variance2);
		}
		else
		{
			screenSpace = new Rect(0f, 0f, Screen.width, Screen.height);
			Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
		}
		MessageBox.ResetWindowPosition();
	}

	private void Update()
	{
		if (amFullScreen != Screen.fullScreen || lastScreenWidth != (float)Screen.width || lastScreenHeight != (float)Screen.height)
		{
			StartCoroutine(UpdateScreenSpace());
		}
		mGameTimer -= Time.deltaTime;
		if (mGameTimer < 0f)
		{
			mGameTimer = 0f;
		}
		if (Debug.isDebugBuild)
		{
			if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.C))
			{
				myPlayer.SendMessage("Captured");
				messageTimer = 5f;
			}
			if (messageTimer > 0f)
			{
				messageTimer -= Time.deltaTime;
				if (messageTimer <= 0f)
				{
					messageTimer = 0f;
					myPlayer.SendMessage("Released");
				}
			}
		}
		if (mPauseScreenActive)
		{
			mLevelMusicSource.volume = GameData.mGameSettings.mMusicVolume;
			for (int i = 0; i < 8; i++)
			{
				if (!(players[i] == null))
				{
					Player player = players[i].GetComponent("Player") as Player;
					if ((bool)player.mPlayerAudioSource)
					{
						player.mPlayerAudioSource.volume = GameData.mGameSettings.mSoundVolume;
					}
					player.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
					if ((bool)player.wpnSnd)
					{
						player.wpnSnd.volume = GameData.mGameSettings.mSoundVolume;
					}
				}
			}
		}
		if (!isPaused && state != null)
		{
			state();
		}
		if (mIsBattleStartPaused)
		{
			isPaused = true;
			if (state != null)
			{
				state();
			}
		}
		if (mIsBattleOver)
		{
			isPaused = true;
			if (state != null)
			{
				state();
			}
		}
	}

	private void FixedUpdate()
	{
		if (m_networkManager != null && m_networkManager.currentActiveRoom != null && m_networkManager.currentActiveRoom.GetVariable("stop") != null && (m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("capturelimit") || m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("timeout")))
		{
			DynamicOptions.bDrawCursor = true;
			GameData.CurPlayState = GameData.PlayState.GAME_IS_OVER;
			if (m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("capturelimit"))
			{
				GameData.setGameEndCondition(2);
			}
			else if (m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("timeout"))
			{
				GameData.setGameEndCondition(3);
			}
		}
		if (m_networkManager != null && m_networkManager.currentActiveRoom != null && m_networkManager.currentActiveRoom.GetVariable("state") != null && m_networkManager.currentActiveRoom.GetVariable("state").Equals("play"))
		{
			Debug.Log("** state : " + m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue());
		}
	}

	public void addBulletNotToAdd(int i)
	{
		doNotInstantiateBulletList.Add(i);
	}

	public bool removeBulletNotToAdd(int i)
	{
		if (doNotInstantiateBulletList.Contains(i))
		{
			doNotInstantiateBulletList.Remove(i);
			return true;
		}
		return false;
	}

	public void removePlayerFromGame(string name, int pId)
	{
		GameData.removePlayer(pId);
		GameObject gameObject = GameObject.Find("remote_" + pId);
		if ((bool)gameObject)
		{
			gameObject.SendMessage("DeleteMe");
		}
		mHUD.AddMessage(name + " Left the Game!");
	}

	private void showPage()
	{
		GUI.Label(new Rect(10f, 10f, 400f, 20f), "page: " + m_statPage);
		if (m_statPage == 0)
		{
			int num = 0;
			GUI.Label(new Rect(10f, 30f, 400f, 20f), "map: " + m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue().ToString());
			GUI.Label(new Rect(10f, 50f, 400f, 20f), "num users: " + m_networkManager.currentActiveRoom.UserCount);
			foreach (User user in m_networkManager.currentActiveRoom.UserList)
			{
				string text = user.GetVariable("nickName").GetStringValue() + " inactive: " + user.GetVariable("inactive").GetBoolValue().ToString() + "PlayerID: " + user.PlayerId;
				GUI.Label(new Rect(10f, 80 + num * 20, 400f, 20f), text);
				num++;
			}
		}
		if (m_statPage < 1 || m_statPage > 8)
		{
			return;
		}
		User userByIndex = m_networkManager.getUserByIndex(m_statPage);
		if (userByIndex != null)
		{
			GUI.Label(new Rect(10f, 60f, 400f, 20f), "nickName: " + userByIndex.GetVariable("nickName").GetStringValue());
			GUI.Label(new Rect(10f, 78f, 400f, 20f), "ClientState: " + userByIndex.GetVariable("clientState").GetStringValue());
			if (userByIndex.GetVariable("myMapId") != null)
			{
				GUI.Label(new Rect(10f, 96f, 400f, 20f), "mapid: " + userByIndex.GetVariable("myMapId").GetStringValue());
			}
			GUI.Label(new Rect(10f, 114f, 400f, 20f), "avatarState: " + userByIndex.GetVariable("avatarState").GetStringValue());
		}
		else
		{
			Debug.Log("Couldn't find User");
		}
	}

	private void OnGUI()
	{
		if (m_showStats)
		{
			GUI.Box(new Rect(10f, 10f, 500f, 500f), "Stats");
			if (m_networkManager != null && m_networkManager.currentActiveRoom != null)
			{
				showPage();
			}
		}
		if (m_wasGoingToLoadWrongMap)
		{
			GUI.Label(new Rect(10f, 20f, 400f, 20f), "WAS GOING TO LOAD WRONG MAP SHOULD BE ON " + GameData.WorldName);
		}
		if (!mGameReady)
		{
			if (mMapImage == null)
			{
				string str = GameData.WorldName.Replace("'", string.Empty).Replace(' ', '_');
				mMapImage = (Resources.Load("Menus/map_image_" + str) as Texture2D);
			}
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width * 100, Screen.height * 100), mPregameBG);
			string text = "Loading...";
			if (serverReady)
			{
				text = "Ready...";
			}
			else if (allPlayersSpawned)
			{
				text = "Syncing...";
			}
			text = stateStatus;
			GUI.BeginGroup(new Rect((Screen.width - 900) / 2, (Screen.height - 600) / 2, 900f, 600f));
			GUI.DrawTexture(new Rect(66.5f, 110f, 767f, 266f), mMapImage);
			GUI.Label(new Rect(0f, 0f, 850f, 25f), text, mStatusStyle);
			GUIUtil.DrawLoadingAnim(new Rect(850f, 0f, 50f, 50f), 0);
			GUI.Label(new Rect(0f, 20f, 900f, 55f), GameData.WorldName.ToUpper(), mMapNameStyle);
			GUI.Label(new Rect(0f, 75f, 900f, 30f), GameData.getBattleTypeDisplayName().ToUpper(), mTypeStyle);
			GUI.Label(new Rect(0f, 440f, 900f, 15f), "Tip: " + mLoadingTip, mTipStyle);
			GUI.DrawTexture(new Rect((900 - mControlsImage.width) / 2, 600 - mControlsImage.height - 10, mControlsImage.width, mControlsImage.height), mControlsImage);
			GUI.EndGroup();
		}
	}

	private void DoMyWindow(int windowID)
	{
	}
	[Obsolete]
	public void DecreaseQuality()
	{
		currentQuality--;
		if (currentQuality < 0)
		{
			currentQuality = 0;
		}
		SetQuality(currentQuality);
	}
	[Obsolete]
	public void IncreaseQuality()
	{
		currentQuality++;
		if (currentQuality > 5)
		{
			currentQuality = 5;
		}
		SetQuality(currentQuality);
	}

	[Obsolete]
	public void SetQuality(int toSet)
	{
		currentQuality = toSet;
		switch (currentQuality)
		{
		case 0:
			QualitySettings.currentLevel = QualityLevel.Fastest;
			break;
		case 1:
			QualitySettings.currentLevel = QualityLevel.Fast;
			break;
		case 2:
			QualitySettings.currentLevel = QualityLevel.Simple;
			break;
		case 3:
			QualitySettings.currentLevel = QualityLevel.Good;
			break;
		case 4:
			QualitySettings.currentLevel = QualityLevel.Beautiful;
			break;
		case 5:
			QualitySettings.currentLevel = QualityLevel.Fantastic;
			break;
		}
	}

	[Obsolete]
	protected void InitGame()
	{
		Logger.trace("<< Init Game Play >>");
		Camera.main.ResetAspect();
		Logger.trace("<< Load Game Objects / bullets / grenade / hitsparks >>");
		bullet = (Resources.Load("projectiles/bullet") as GameObject);
		grenade = (Resources.Load("projectiles/grenade2") as GameObject);
		event_lobber_grenade = null;
		event_atlas_grenade = null;
		event_banzai_grenade = null;
		event_atlas_bubble = null;
		event_atlas_pistol_bubble = null;
		event_atlas_rifle_bubble = null;
		event_atlas_shotgun_bubble = null;
		event_atlas_smg_bubble = null;
		event_atlas_spread_bubble = null;
		event_atlas_lobber_bubble = null;
		event_atlas_sniper_bubble = null;
		event_atlas_rocket_bubble = null;
		event_atlas_grenade_bubble = null;
		event_banzai_bubble = null;
		event_banzai_pistol_bubble = null;
		event_banzai_rifle_bubble = null;
		event_banzai_shotgun_bubble = null;
		event_banzai_smg_bubble = null;
		event_banzai_spread_bubble = null;
		event_banzai_lobber_bubble = null;
		event_banzai_sniper_bubble = null;
		event_banzai_rocket_bubble = null;
		event_banzai_grenade_bubble = null;
		if (GameData.eventObjects.ContainsKey("lobber_grenade"))
		{
			event_lobber_grenade = (GameData.eventObjects["lobber_grenade"] as GameObject);
		}
		if (GameData.eventObjects.ContainsKey("atlas_grenade"))
		{
			event_atlas_grenade = (GameData.eventObjects["atlas_grenade"] as GameObject);
		}
		if (GameData.eventObjects.ContainsKey("banzai_grenade"))
		{
			event_banzai_grenade = (GameData.eventObjects["banzai_grenade"] as GameObject);
		}
		if (GameData.eventObjects.ContainsKey("atlas_bubble"))
		{
			event_atlas_bubble = (GameData.eventObjects["atlas_bubble"] as GameObject);
			event_atlas_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_pistol_bubble"))
		{
			event_atlas_pistol_bubble = (GameData.eventObjects["atlas_pistol_bubble"] as GameObject);
			event_atlas_pistol_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_rifle_bubble"))
		{
			event_atlas_rifle_bubble = (GameData.eventObjects["atlas_rifle_bubble"] as GameObject);
			event_atlas_rifle_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_shotgun_bubble"))
		{
			event_atlas_shotgun_bubble = (GameData.eventObjects["atlas_shotgun_bubble"] as GameObject);
			event_atlas_shotgun_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_smg_bubble"))
		{
			event_atlas_smg_bubble = (GameData.eventObjects["atlas_smg_bubble"] as GameObject);
			event_atlas_smg_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_spread_bubble"))
		{
			event_atlas_spread_bubble = (GameData.eventObjects["atlas_spread_bubble"] as GameObject);
			event_atlas_spread_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_lobber_bubble"))
		{
			event_atlas_lobber_bubble = (GameData.eventObjects["atlas_lobber_bubble"] as GameObject);
			event_atlas_lobber_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_sniper_bubble"))
		{
			event_atlas_sniper_bubble = (GameData.eventObjects["atlas_sniper_bubble"] as GameObject);
			event_atlas_sniper_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_rocket_bubble"))
		{
			event_atlas_rocket_bubble = (GameData.eventObjects["atlas_rocket_bubble"] as GameObject);
			event_atlas_rocket_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("atlas_grenade_bubble"))
		{
			event_atlas_grenade_bubble = (GameData.eventObjects["atlas_grenade_bubble"] as GameObject);
			event_atlas_grenade_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_bubble"))
		{
			event_banzai_bubble = (GameData.eventObjects["banzai_bubble"] as GameObject);
			event_banzai_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_pistol_bubble"))
		{
			event_banzai_pistol_bubble = (GameData.eventObjects["banzai_pistol_bubble"] as GameObject);
			event_banzai_pistol_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_rifle_bubble"))
		{
			event_banzai_rifle_bubble = (GameData.eventObjects["banzai_rifle_bubble"] as GameObject);
			event_banzai_rifle_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_shotgun_bubble"))
		{
			event_banzai_shotgun_bubble = (GameData.eventObjects["banzai_shotgun_bubble"] as GameObject);
			event_banzai_shotgun_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_smg_bubble"))
		{
			event_banzai_smg_bubble = (GameData.eventObjects["banzai_smg_bubble"] as GameObject);
			event_banzai_smg_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_spread_bubble"))
		{
			event_banzai_spread_bubble = (GameData.eventObjects["banzai_spread_bubble"] as GameObject);
			event_banzai_spread_bubble.AddComponent<bubbleRotateScript>();
		}
		if (GameData.eventObjects.ContainsKey("banzai_lobber_bubble"))
		{
			event_banzai_lobber_bubble = (GameData.eventObjects["banzai_lobber_bubble"] as GameObject);
			event_banzai_lobber_bubble.AddComponent<bubbleRotateScript>();		}
		if (GameData.eventObjects.ContainsKey("banzai_sniper_bubble"))
		{
			event_banzai_sniper_bubble = (GameData.eventObjects["banzai_sniper_bubble"] as GameObject);
			event_banzai_sniper_bubble.AddComponent<bubbleRotateScript>();		}
		if (GameData.eventObjects.ContainsKey("banzai_rocket_bubble"))
		{
			event_banzai_rocket_bubble = (GameData.eventObjects["banzai_rocket_bubble"] as GameObject);
			event_banzai_rocket_bubble.AddComponent<bubbleRotateScript>();		}
		if (GameData.eventObjects.ContainsKey("banzai_grenade_bubble"))
		{
			event_banzai_grenade_bubble = (GameData.eventObjects["banzai_grenade_bubble"] as GameObject);
			event_banzai_grenade_bubble.AddComponent<bubbleRotateScript>();		}
		rocket = (Resources.Load("projectiles/rocket") as GameObject);
		hitspark1 = (Resources.Load("projectiles/hitspark1") as GameObject);
		hitspark2 = (Resources.Load("projectiles/hitspark2") as GameObject);
		pickupEffect = (Resources.Load("effects/prefabs/pickup_appear_pre") as GameObject);
		healthEffect = (Resources.Load("effects/prefabs/health_up_effect_pre") as GameObject);
		rootedEffect = (Resources.Load("effects/prefabs/rooted_effect_pre") as GameObject);
		orbitalDamageEffect = (Resources.Load("effects/prefabs/pickup_orbital_arc_pre") as GameObject);
		armorEffect = (Resources.Load("effects/prefabs/armorUpEffect_prefab") as GameObject);
		Logger.trace("armor effect: " + armorEffect);
		damageEffect = (Resources.Load("effects/prefabs/damageUpEffect_prefab") as GameObject);
		Logger.trace("damage effect: " + damageEffect);
		speedEffect = (Resources.Load("effects/prefabs/speed_trail_pre") as GameObject);
		state = SpawnPlayersAndWorlds;
		if (m_networkManager != null && m_networkManager.currentActiveRoom != null)
		{
			if (m_networkManager.currentActiveRoom.GetVariable("stop") != null)
			{
				Debug.Log("<< joining room in stop: " + m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue());
				if (m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("playersleft"))
				{
					QuitGame(3);
				}
				if (m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("atlasleft"))
				{
					QuitGame(4);
				}
				if (m_networkManager.currentActiveRoom.GetVariable("stop").GetStringValue().Equals("banzaileft"))
				{
					QuitGame(5);
				}
			}
			else
			{
				Debug.Log("** Couldn't find variable stop");
			}
			if (m_networkManager.currentActiveRoom.GetVariable("state") != null)
			{
				if (!m_networkManager.currentActiveRoom.GetVariable("state").GetStringValue().Equals("play"))
				{
					Debug.Log("*** Room is out of sync...should quit room");
				}
			}
			else
			{
				Debug.Log("** Couldn't find variable state");
			}
		}
		else
		{
			Debug.Log("*** network manager is null ***");
		}
	}

	[Obsolete]
	protected virtual void SpawnPlayersAndWorlds()
	{
		//SetCameraEffects();
		spawnWorld();
		setWorldScripts();
		spawnPickups(this is TutorialGamePlay);
		state = waitToSpawnPlayer;
	}

	protected virtual void waitToSpawnPlayer()
	{
		spawnPlayers();
		GameObject obj = GameObject.Find("LightToDelete");
		UnityEngine.Object.Destroy(obj);
		if (m_networkManager.isConnected())
		{
			m_networkManager.sendClientState("playing");
		}
		spawnHUD();
		mGameReady = true;
		isPaused = false;
		mIsBattleStartPaused = false;
		state = WaitToChooseWeapon;
		if (GameData.CurPlayState != GameData.PlayState.GAME_IS_PLAYING)
		{
			if (GameData.BattleType == 2)
			{
				mTotalGameTime = (mGameTimer = 900f);
			}
			else
			{
				mTotalGameTime = (mGameTimer = 600f);
			}
		}
		else
		{
			SendOpponentInfo();
			SendTimeUpdate();
		}
		GameData.CurPlayState = GameData.PlayState.GAME_IS_PLAYING;
	}

	private void setWorldScripts()
	{
		stateStatus = "setting world scripts";
		GameObject gameObject = GameObject.Find("world");
		switch (GameData.WorldID)
		{
		case 1:
		{
			Transform transform2 = gameObject.transform.FindChild("SL_Crystal");
			if (transform2 != null)
			{
				transform2.gameObject.AddComponent<RotateMe>();			}
			else
			{
				Logger.traceError("<< couldn't find Crystal");
			}
			break;
		}
		case 3:
		{/* TODO: Replace water
			Transform transform = gameObject.transform.FindChild("FJ_Water/FJ_Water_01");
			if (transform != null)
			{
				transform.gameObject.AddComponent<WaterSimple>();			}
			transform = gameObject.transform.FindChild("FJ_Water/FJ_Water_02");
			if (transform != null)
			{
				transform.gameObject.AddComponent<WaterSimple>();			}
			transform = gameObject.transform.FindChild("FJ_Water/FJ_Water_03");
			if (transform != null)
			{
				transform.gameObject.AddComponent<WaterSimple>();			}
			transform = gameObject.transform.FindChild("FJ_Water/FJ_Water_04");
			if (transform != null)
			{
				transform.gameObject.AddComponent<WaterSimple>();			}
			transform = gameObject.transform.FindChild("FJ_Water/FJ_Water_05");
			if (transform != null)
			{
				transform.gameObject.AddComponent<WaterSimple>();			}
			transform = gameObject.transform.FindChild("FJ_Water/FJ_Water_06");
			if (transform != null)
			{
				transform.gameObject.AddComponent<WaterSimple>();			}*/
			break;
		}
		case 5:
		{
			Transform transform3 = gameObject.transform.FindChild("PP_Black_Walls");
			if (transform3 != null)
			{
				transform3.gameObject.AddComponent<perplexLightBarAnimScript>();			}
			else
			{
				Logger.traceError("<< couldn't find black wall");
			}
			break;
		}
		case 6:
		{
			GameObject gameObject2 = GameObject.Find("blingblingPicSheenAnims");
			if (gameObject2 != null)
			{
				perplexLightBarAnimScript perplexLightBarAnimScript = gameObject2.gameObject.AddComponent<perplexLightBarAnimScript>();				perplexLightBarAnimScript.intervalTime = 8f;
				perplexLightBarAnimScript.offsetAmount = 0.1f;
			}
			else
			{
				Logger.traceError("<< couldn't find bling frame object");
			}
			break;
		}
		default:
			Logger.trace("<< playing : " + GameData.WorldID);
			break;
		case 2:
			break;
		}
		AudioSource[] array = UnityEngine.Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		AudioSource[] array2 = array;
		foreach (AudioSource audioSource in array2)
		{
			if (inSoundList(audioSource.name))
			{
				ambientSoundInfo item = new ambientSoundInfo(audioSource, audioSource.volume);
				mWorldSounds.Add(item);
			}
		}
		setWorldSoundVolume();
	}

	public void setWorldSoundVolume()
	{
		foreach (ambientSoundInfo mWorldSound in mWorldSounds)
		{
			mWorldSound.mSound.volume = mWorldSound.mMaxVolume * GameData.mGameSettings.mSoundVolume;
		}
	}

	private bool inSoundList(string name)
	{
		for (int i = 0; i < mWorldSoundList.Length; i++)
		{
			if (name == mWorldSoundList[i])
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void ShowBanner(int num, Vector3 pos)
	{
		mHUD.ShowBannerMessage(num, pos);
	}

	/*
	[Obsolete]
	public void SetCameraEffects()
	{
		BloomAndFlares bloomAndFlares = Camera.main.GetComponent("BloomAndFlares") as BloomAndFlares;
		ColorCorrectionCurves colorCorrectionCurves = Camera.main.GetComponent("ColorCorrectionCurves") as ColorCorrectionCurves;
		DepthOfField depthOfField = Camera.main.GetComponent("DepthOfField") as DepthOfField;
		//LuminanceEdgeBlur luminanceEdgeBlur = Camera.main.GetComponent("LuminanceEdgeBlur") as LuminanceEdgeBlur;
		if (bloomAndFlares == null || colorCorrectionCurves == null || depthOfField == null /*|| luminanceEdgeBlur == null)
		{
			return;
		}
		bloomAndFlares.enabled = false;
		colorCorrectionCurves.enabled = false;
		depthOfField.enabled = false;
		//luminanceEdgeBlur.enabled = false;
		if (QualitySettings.currentLevel == QualityLevel.Simple)
		{
			if (GameData.WorldID != 5 && GameData.WorldID != 6)
			{
				bloomAndFlares.enabled = true;
			}
			depthOfField.enabled = true;
			//luminanceEdgeBlur.enabled = true;
			if (GameData.WorldID != 6)
			{
				colorCorrectionCurves.enabled = true;
			}
			switch (GameData.WorldID)
			{
			case 1:
				break;
			case 0:
				bloomAndFlares.bloomIntensity = 0f;
				break;
			case 2:
				//depthOfField.focalFalloff = 0.45f;
				break;
			case 3:
				bloomAndFlares.bloomIntensity = 0.4f;
				break;
			case 4:
				bloomAndFlares.bloomIntensity = 0.15f;
				//depthOfField.focalFalloff = 0.2f;
				break;
			case 5:
				//depthOfField.focalFalloff = 0.05f;
				break;
			case 6:
				depthOfField.focalLength = 90f;
				break;
			}
		}
	}
	*/

	public void WaitToChooseWeapon()
	{
		Player player = myPlayer.GetComponent("Player") as Player;
		Logger.trace("<< play the game!");
		if (m_networkManager != null && m_networkManager.user != null && m_networkManager.user.IsItMe)
		{
			SendChangeWeapon(player.previousWeaponNotBlaster);
		}
		GameObject gameObject = GameObject.Find("Tracker");
		TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
		trackerScript.AddMetric(TrackerScript.Metric.NEW_MATCH_START);
		state = PlayingGame;
	}

	public void PlayingGame()
	{
		if (mIsBattleOver)
		{
			Logger.trace("OK - BATTLE IS OVER!");
			state = endBattle;
		}
		else if (Debug.isDebugBuild && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.K))
		{
			sendForceGameEnd();
		}
	}
	[Obsolete]
	private void endBattle()
	{
		GameData.CurPlayState = GameData.PlayState.GAME_IS_WAITING_SUMMARY;
		if (cameraScrolling.distance != 150f && cameraScrolling.distance < 150f)
		{
			cameraScrolling.distance += 100f * Time.deltaTime;
			if (cameraScrolling.distance > 150f)
			{
				cameraScrolling.distance = 150f;
				state = waitForBattleData;
				GameObject gameObject = GameObject.Find("Tracker");
				TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
				trackerScript.AddMetric(TrackerScript.Metric.MATCH_COMPLETED);
				trackerScript.updateMetricStats();
				mBattleMessageTimer = 3f;
			}
		}
	}

	public WeaponDef getWeaponById(int id)
	{
		return GameData.getWeaponById(id);
	}

	[Obsolete]
	private void waitForBattleData()
	{
		mBattleMessageTimer -= Time.deltaTime;
		if (mIsBattleSummaryReceived && mBattleMessageTimer <= 0f)
		{
			GameData.NumBattlesCompleted++;
			GameData.CurPlayState = GameData.PlayState.GAME_SUMMARY_RECEIVED;
			LoadLevel("GameBattleQueue");
		}
	}

	private void spawnPickups(bool isTutorial)
	{
		stateStatus = "spawn pickups";
		Logger.trace("<< num pickups: " + GameData.Pickups.Count);
		int num = 0;
		GameObject gameObject;
		GameObject gameObject2;
		GameObject gameObject3;
		GameObject gameObject4;
		GameObject gameObject5;
		GameObject gameObject6;
		GameObject gameObject7;
		GameObject gameObject8;
		GameObject gameObject9;
		GameObject gameObject10;
		GameObject gameObject11;
		GameObject gameObject12;
		if (GameData.DoesEventExist("random_pickup") && !isTutorial)
		{
			gameObject = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject2 = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject3 = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject4 = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject5 = GameData.Pickups[4];
			gameObject6 = GameData.Pickups[5];
			gameObject7 = GameData.Pickups[6];
			gameObject8 = GameData.Pickups[7];
			gameObject9 = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject10 = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject11 = (GameData.eventObjects["random_pickup"] as GameObject);
			gameObject12 = (GameData.eventObjects["random_pickup"] as GameObject);
		}
		else
		{
			gameObject = GameData.Pickups[0];
			gameObject2 = GameData.Pickups[1];
			gameObject3 = GameData.Pickups[2];
			gameObject4 = GameData.Pickups[3];
			gameObject5 = GameData.Pickups[4];
			gameObject6 = GameData.Pickups[5];
			gameObject7 = GameData.Pickups[6];
			gameObject8 = GameData.Pickups[7];
			gameObject9 = GameData.Pickups[8];
			gameObject10 = GameData.Pickups[9];
			gameObject11 = GameData.Pickups[10];
			gameObject12 = GameData.Pickups[11];
		}
		if (GameData.PickupSpawns.Count == 0)
		{
			TextAsset pickupSpawnPointsById = GameData.getPickupSpawnPointsById(GameData.WorldID - 1, GameData.BattleType - 1, GameData.GameType - 1);
			TextAsset textAsset = UnityEngine.Object.Instantiate(pickupSpawnPointsById) as TextAsset;
			StringReader stringReader = new StringReader(textAsset.text);
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				GameData.PickupSpawns.Add(text);
			}
		}
		foreach (string pickupSpawn in GameData.PickupSpawns)
		{
			if (pickupSpawn != null && !(pickupSpawn == string.Empty))
			{
				string[] array = pickupSpawn.Split(","[0]);
				int num2 = Convert.ToInt32(array[0]);
				float timeToRespawn = Convert.ToSingle(array[1]);
				GameObject original;
				switch (num2)
				{
				case 0:
					original = gameObject;
					break;
				case 10:
					original = gameObject12;
					break;
				case 1:
					original = gameObject2;
					break;
				case 11:
					original = gameObject10;
					break;
				case 2:
					original = gameObject3;
					break;
				case 3:
					original = gameObject4;
					break;
				case 13:
					original = gameObject11;
					break;
				case 4:
					original = gameObject5;
					break;
				case 5:
					original = gameObject6;
					break;
				case 6:
					original = gameObject7;
					break;
				case 7:
					original = gameObject8;
					break;
				case 8:
					original = gameObject9;
					break;
				default:
					original = gameObject;
					break;
				}
				Logger.trace("pickup type: " + num2);
				GameObject gameObject13 = UnityEngine.Object.Instantiate(original, new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), 0f), Quaternion.identity) as GameObject;
				gameObject13.tag = "pickup";
				gameObject13.layer = 2;
				gameObject13.AddComponent<PickUp>();				gameObject13.name = "weapon";
				PickUp pickUp = gameObject13.GetComponent("PickUp") as PickUp;
				pickUp.puType = num2;
				switch (num2)
				{
				case 0:
					pickUp.multiplier = 0.2f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 1:
					pickUp.multiplier = 0.2f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 2:
					pickUp.multiplier = 0.5f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 3:
					pickUp.multiplier = 0.5f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				}
				switch (num2)
				{
				case 10:
					pickUp.multiplier = 0.2f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 11:
					pickUp.multiplier = 0.2f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 12:
					pickUp.multiplier = 0.5f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 13:
					pickUp.multiplier = 0.5f;
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				case 7:
					gameObject13.name = "grenades";
					break;
				case 8:
					pickUp.mContents = UnityEngine.Random.Range(0, 3);
					switch (pickUp.mContents)
					{
					case 0:
					case 1:
						pickUp.multiplier = 0.2f;
						break;
					case 2:
					case 3:
						pickUp.multiplier = 0.5f;
						break;
					}
					pickUp.effectTime = 20f;
					gameObject13.name = "boost";
					break;
				}
				pickUp.puIndex = num;
				num++;
				pickUp.timeToRespawn = timeToRespawn;
			}
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("grenade_pickup");
		GameObject[] array3 = array2;
		foreach (GameObject gameObject14 in array3)
		{
			gameObject14.AddComponent<pickupGrenadeOrbitPlayer>();		}
	}

	[Obsolete]
	public void spawnWorld()
	{
		stateStatus = "Spawn World";
		Logger.traceAlways("Spawn World Num: " + GameData.WorldID);
		bool flag = false;
		if (m_networkManager != null && m_networkManager.currentActiveRoom != null && m_networkManager.currentActiveRoom.GetVariable("mapId") != null && m_networkManager.currentActiveRoom.GetVariable("lastMapLoadedId") != null)
		{
			Logger.traceAlways("Should be spawning world num: " + m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue());
			if (m_networkManager.currentActiveRoom.GetVariable("lastMapLoadedId").GetIntValue() != m_networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue())
			{
				flag = true;
			}
		}
		if (GameData.getWorldById(GameData.WorldID) == null || flag)
		{
			QuitGame(10);
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(GameData.getWorldById(GameData.WorldID), new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
		gameObject.name = "world";
		gameObject.transform.position = new Vector3(0f, 0f, 0f);
		gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		cameraScrolling.levelAttributes = LevelAttributes.GetInstance();
		Rect bounds = cameraScrolling.levelAttributes.bounds;
		Vector3 position = GameObject.Find("bb_left").transform.position;
		bounds.xMin = position.x;
		Rect bounds2 = cameraScrolling.levelAttributes.bounds;
		Vector3 position2 = GameObject.Find("bb_right").transform.position;
		bounds2.xMax = position2.x;
		Rect bounds3 = cameraScrolling.levelAttributes.bounds;
		Vector3 position3 = GameObject.Find("bb_top").transform.position;
		bounds3.yMax = position3.y;
		Rect bounds4 = cameraScrolling.levelAttributes.bounds;
		Vector3 position4 = GameObject.Find("bb_bottom").transform.position;
		bounds4.yMin = position4.y;
		cameraScrolling.levelBounds = cameraScrolling.levelAttributes.bounds;
		cameraScrolling.levelAttributes.SetBoundaries();
		LightmapSettings.lightmaps = GameData.GetLightmapsForWorld(GameData.WorldID);
		// LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		Debug.Log("There are " + LightmapSettings.lightmaps.Length + " lightmaps");
		GameObject gameObject2 = new GameObject();
		gameObject2.transform.position = Camera.main.transform.position;
		gameObject2.name = "LightToDelete";
		Light light = gameObject2.AddComponent<Light>();
		light.enabled = true;
	}

	protected void spawnHUD()
	{
		Logger.trace(":::::::::::::::   Spawn HUD :::::::::::::");
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("HUD/hud") as GameObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
		gameObject.name = "HUD";
		mHUD = (gameObject.GetComponent("GameHUD") as GameHUD);
		Player player = myPlayer.GetComponent("Player") as Player;
		Logger.trace("<< max health, max fuel: " + player.healthMax + " my name: " + myPlayer.name);
		mHUD.createHUD(string.Empty, player.healthMax, player.fuelMax);
	}

	protected virtual bool CanSpawnPlayers()
	{
		return sfs != null && GameData.GameRoom != null;
	}

	public void addJoiner(int factionId, int playerId)
	{
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

	protected virtual void spawnPlayers()
	{
		if (CanSpawnPlayers())
		{
			Logger.trace("::::::::::::::::::::: Spawn Players ::::::::::::::::::::::");
			if (GameData.isTeamBattle())
			{
				GameData.playerSlots1.Sort();
				GameData.playerSlots2.Sort();
			}
			foreach (User user in m_networkManager.currentActiveRoom.UserList)
			{
				if (user.IsItMe)
				{
					spawnMyPlayer(user.PlayerId);
					SendActiveBoostInfo();
				}
				else
				{
					string stringValue = user.GetVariable("clientState").GetStringValue();
					if (stringValue.Equals("playing"))
					{
						Logger.traceAlways("<< SpawnPlayers: " + user.Name);
						spawnRemotePlayer(user);
					}
				}
			}
			stateStatus = "all players spawned";
			allPlayersSpawned = true;
		}
	}

	public bool isOnMyTeam(Player p)
	{
		Player player = myPlayer.GetComponent("Player") as Player;
		if (GameData.BattleType == 2 && player.mFaction == p.mFaction)
		{
			return true;
		}
		return false;
	}

	protected virtual void SetContextualHelp()
	{
		LocalControl localControl = myGamePlayer.playerControl as LocalControl;
		localControl.setHelp(myPlayer.GetComponent("ContextualHelp") as ContextualHelp);
	}

	protected virtual void spawnMyPlayer(int playerId)
	{
		stateStatus = "spawn ME";
		Logger.trace("::::::: Spawn ME ::::::::");
		int num = playerId - 1;
		GameObject gameObject = spawnExosuit(num, GameData.MySuitID, GameData.MyTextureID);
		if (gameObject == null)
		{
			return;
		}
		gameObject.AddComponent<Player>();
		Transform transform = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetLite");
		transform.GetComponent<Light>().cullingMask = 1024;
		Player player = myGamePlayer = (gameObject.GetComponent("Player") as Player);
		player.mAmReady = true;
		gameObject.tag = "Player";
		player.mMyID = playerId;
		player.myIdx = num;
		player.mySuitIdx = GameData.MySuitID;
		player.mySuitTextureIdx = GameData.MyTextureID;
		player.mPlayerAudioSource = (gameObject.AddComponent<AudioSource>());
		player.mPlayerAudioSource.dopplerLevel = 0f;
		player.mPlayerAudioSource.rolloffMode = AudioRolloffMode.Linear;
		player.mPlayerAudioSource.volume = GameData.mGameSettings.mSoundVolume;
		player.mJetPackLowSource = (gameObject.AddComponent<AudioSource>());
		player.mJetPackLowSource.dopplerLevel = 0f;
		player.mJetPackLowSource.rolloffMode = AudioRolloffMode.Linear;
		player.mJetPackLowSource.volume = GameData.mGameSettings.mSoundVolume;
		Exosuit exosuit = GameData.getExosuit(GameData.MySuitID);
		if (this is TutorialGamePlay)
		{
			player.runIdx = 0;
			player.rollIdx = 0;
			player.rollRecoverIdx = 0;
			player.crouchIdx = 0;
			player.airdashIdx = 0;
			player.dashbackwardIdx = 0;
			player.dashrecoverIdx = 0;
			player.dashbackwardrecoverIdx = 0;
			player.runBackwardIdx = 0;
			player.captureFloatIdx = 0;
			player.captureGroundIdx = 0;
			player.jetpackIdx = 0;
			player.jumpbeginIdx = 0;
			player.jumpAtApexIdx = 0;
			player.jumpLandIdx = 0;
			player.aimIdx = 0;
			player.idleIdx = 0;
		}
		else
		{
			player.runIdx = exosuit.mRunIndex;
			player.rollIdx = exosuit.mRollIndex;
			player.rollRecoverIdx = exosuit.Roll_RecoverIdx;
			player.crouchIdx = exosuit.CrouchIdx;
			player.airdashIdx = exosuit.Airdash_ForwardIdx;
			player.dashbackwardIdx = exosuit.Airdash_BackwardIdx;
			player.dashrecoverIdx = exosuit.Airdash_Recover_ForwardIdx;
			player.dashbackwardrecoverIdx = exosuit.Airdash_Recover_BackwardIdx;
			player.runBackwardIdx = exosuit.Run_BackwardIdx;
			player.captureFloatIdx = exosuit.Capture_AirIdx;
			player.captureGroundIdx = exosuit.Capture_GroundIdx;
			player.jetpackIdx = exosuit.JetpackIdx;
			player.jumpbeginIdx = exosuit.Jump_BeginIdx;
			player.jumpAtApexIdx = exosuit.Jump_FallIdx;
			player.jumpLandIdx = exosuit.Jump_LandIdx;
			player.aimIdx = exosuit.ShootIdx;
			player.idleIdx = exosuit.IdleIdx;
		}
		player.mFaction = exosuit.mFactionId;
		player.setupAnims();
		player.setControl(0);
		myPlayer = gameObject;
		gameObject.name = "localPlayer";
		player.handleDamageRing = (gameObject.AddComponent<HandleDamageRing>());
		gameObject.AddComponent<NetworkTransformSender>();
		gameObject.AddComponent<NetworkTransformReceiver>();
		gameObject.AddComponent<ContextualHelp>();
		SetContextualHelp();
		NetworkTransformSender networkTransformSender = gameObject.GetComponent(typeof(NetworkTransformSender)) as NetworkTransformSender;
		networkTransformSender.StartSending();
		int num2 = 10;
		if (GameData.BattleType == 2)
		{
			if (player.mFaction == 1)
			{
				Logger.trace("<< changing layer to banzai ");
				num2 = 11;
			}
			else if (player.mFaction == 2)
			{
				Logger.trace("<< changing layer to atlas");
				num2 = 12;
			}
		}
		player.myLayer = num2;
		SetLayerRecursively(gameObject, num2);
		players[num] = gameObject;
		attachEffects(gameObject, num2);
		player.spawnBubble(-1, 0);
	}

	public void spawnRemotePlayer(User u)
	{
		int playerId = u.PlayerId;
		GameObject exists = GameObject.Find("remote_" + playerId);
		if ((bool)exists)
		{
			return;
		}
		Logger.trace("::::::: Spawning OPPONENT ::::::::");
		stateStatus = "spawning remote player " + playerId;
		if (mHUD != null)
		{
			mHUD.AddMessage(u.GetVariable("nickName").GetStringValue() + " joined game");
		}
		int num = playerId - 1;
		int num2 = u.GetVariable("suitId").GetIntValue();
		string stringValue = u.GetVariable("faction").GetStringValue();
		Exosuit exosuit = GameData.getExosuit(num2);
		if (exosuit == null)
		{
			Logger.traceWarning("No suit found. Using default for:" + stringValue);
			num2 = ((!stringValue.Equals("atlas")) ? 1 : 21);
		}
		int num3 = 1;
		int intValue = u.GetVariable("weaponId").GetIntValue();
		Logger.trace("==> PlayerId: " + playerId);
		Logger.trace("==> SuitId: " + num2);
		Logger.trace("<< SPAWNING in player state " + u.GetVariable("avatarState").ToString());
		GameObject gameObject = spawnExosuit(num, num2, num3);
		if (gameObject == null)
		{
			return;
		}
		gameObject.AddComponent<Player>();
		Transform transform = gameObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetLite");
		transform.GetComponent<Light>().cullingMask = 1024;
		Player player = gameObject.GetComponent("Player") as Player;
		gameObject.tag = "Player";
		player.mMyID = playerId;
		player.myIdx = num;
		player.mySuitIdx = num2;
		player.mySuitTextureIdx = num3;
		player.weaponIdx = intValue;
		player.mAmReady = true;
		gameObject.AddComponent<AudioSource>();
		Exosuit exosuit2 = GameData.getExosuit(num2);
		player.runIdx = exosuit2.mRunIndex;
		player.rollIdx = exosuit2.mRollIndex;
		player.rollRecoverIdx = exosuit2.Roll_RecoverIdx;
		player.crouchIdx = exosuit2.CrouchIdx;
		player.airdashIdx = exosuit2.Airdash_ForwardIdx;
		player.dashbackwardIdx = exosuit2.Airdash_BackwardIdx;
		player.dashrecoverIdx = exosuit2.Airdash_Recover_ForwardIdx;
		player.dashbackwardrecoverIdx = exosuit2.Airdash_Recover_BackwardIdx;
		player.runBackwardIdx = exosuit2.Run_BackwardIdx;
		player.captureFloatIdx = exosuit2.Capture_AirIdx;
		player.captureGroundIdx = exosuit2.Capture_GroundIdx;
		player.jetpackIdx = exosuit2.JetpackIdx;
		player.jumpbeginIdx = exosuit2.Jump_BeginIdx;
		player.jumpAtApexIdx = exosuit2.Jump_FallIdx;
		player.jumpLandIdx = exosuit2.Jump_LandIdx;
		player.aimIdx = exosuit2.ShootIdx;
		player.idleIdx = exosuit2.IdleIdx;
		player.mFaction = exosuit2.mFactionId;
		player.setupAnims();
		player.setControl(1);
		gameObject.name = "remote_" + playerId;
		int num4 = 10;
		if (GameData.BattleType == 2)
		{
			if (player.mFaction == 1)
			{
				Logger.trace("<< changing layer to banzai ");
				num4 = 11;
			}
			else if (player.mFaction == 2)
			{
				Logger.trace("<< changing layer to atlas");
				num4 = 12;
			}
		}
		player.myLayer = num4;
		SetLayerRecursively(gameObject, num4);
		player.handleDamageRing = (gameObject.AddComponent<HandleDamageRing>());
		gameObject.AddComponent<NetworkTransformReceiver>();
		player.SendMessage("StartReceiving");
		players[num] = gameObject;
		attachEffects(gameObject, num4);
		if (player.GetComponent<AudioSource>() != null)
		{
			player.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
		}
		if (u.GetVariable("avatarState").Equals("captured"))
		{
			player.spawnBubble(-1, 0);
		}
	}

	protected void attachEffects(GameObject plyr, int layerToIgnore)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("suits/shadow") as GameObject) as GameObject;
		gameObject.name = "shadow";
		gameObject.transform.parent = plyr.transform;
		gameObject.transform.localPosition = new Vector3(0f, 9f, 0f);
		Projector projector = gameObject.GetComponent("Projector") as Projector;
		projector.ignoreLayers = 1024;
		projector.ignoreLayers |= 2048;
		projector.ignoreLayers |= 4096;
		projector.ignoreLayers |= 16384;
		Transform parent = plyr.transform.Find("Bip01/Bip01 Pelvis");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(speedEffect) as GameObject;
		gameObject2.name = "speedLine1";
		gameObject2.transform.parent = parent;
		gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
		Player player = plyr.GetComponent("Player") as Player;
		player.speedline1 = (gameObject2.GetComponent("TrailRenderer") as TrailRenderer);
		player.speedline1.enabled = false;
		GameObject gameObject3 = UnityEngine.Object.Instantiate(speedEffect) as GameObject;
		gameObject3.name = "speedLine2";
		gameObject3.transform.parent = parent;
		gameObject3.transform.localPosition = new Vector3(-4f, 0f, 0f);
		player.speedline2 = (gameObject3.GetComponent("TrailRenderer") as TrailRenderer);
		player.speedline2.enabled = false;
	}

	public static void SetLayerRecursively(GameObject obj, int newLayer)
	{
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			SetLayerRecursively(item.gameObject, newLayer);
		}
	}

	public virtual GameObject spawnExosuit(int playerIndex, int suitId, int textureId)
	{
		Logger.trace("# SPAWN SUIT " + suitId + " #");
		Exosuit exosuit = GameData.getExosuit(suitId);
		if (exosuit == null)
		{
			Logger.traceError("Horrible error, can't spawn suit.  Returning player in bad state.");
			return null;
		}
		GameObject lowPolyModel = exosuit.getLowPolyModel();
		if (lowPolyModel == null)
		{
			Logger.traceError("<< Suit model " + suitId + " is null");
		}
		else
		{
			Logger.trace("Suit Model:" + lowPolyModel);
		}
		Vector3 vector = new Vector3(0f, 2000f, 0f);
		int num = playerIndex;
		if (GameData.isTeamBattle())
		{
			num = ((exosuit.mFactionId != 1) ? 4 : 0);
			for (int i = 0; i < 4; i++)
			{
				if (num < 4)
				{
					if (GameData.playerSlots1[i] == playerIndex + 1)
					{
						num += i;
						break;
					}
				}
				else if (GameData.playerSlots2[i] == playerIndex + 1)
				{
					num += i;
					break;
				}
			}
		}
		Vector3 vector2 = GameData.PlayerSpawns[num];
		float x = vector2.x;
		Vector3 vector3 = GameData.PlayerSpawns[num];
		float y = vector3.y;
		Vector3 vector4 = GameData.PlayerSpawns[num];
		vector = new Vector3(x, y, vector4.z);
		cameraScrolling.levelBounds = cameraScrolling.levelAttributes.bounds;
		Logger.traceAlways("Spawning " + playerIndex + " at index: " + num + " at SpawnPt: " + vector);
		if (vector.x > cameraScrolling.levelBounds.xMax || vector.x < cameraScrolling.levelBounds.xMin || vector.y > cameraScrolling.levelBounds.yMax || vector.y < cameraScrolling.levelBounds.yMin)
		{
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.xMin);
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.yMin);
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.xMax);
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.yMax);
			vector = Vector3.zero;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(lowPolyModel, vector, Quaternion.identity) as GameObject;
		ArrayList lowPolyTextures = exosuit.getLowPolyTextures();
		Logger.trace("<< Suit Textures: " + lowPolyTextures.Count);
		Material material = lowPolyTextures[0] as Material;
		Transform transform = gameObject.transform.Find("armor");
		SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
		skinnedMeshRenderer.updateWhenOffscreen = true;
		skinnedMeshRenderer.materials = new Material[2];
		skinnedMeshRenderer.materials = new Material[2]
		{
			Resources.Load("effects/healthDamage/linePulseUpwardsMat") as Material,
			material
		};
		ArrayList arrayList = new ArrayList();
		arrayList.Add("crouch_" + exosuit.CrouchIdx);
		arrayList.Add("airdash_" + exosuit.Airdash_ForwardIdx);
		arrayList.Add("dash_backwards_" + exosuit.Airdash_BackwardIdx);
		arrayList.Add("airdash_recover_" + exosuit.Airdash_Recover_ForwardIdx);
		arrayList.Add("dash_backwards_rec_" + exosuit.Airdash_Recover_BackwardIdx);
		arrayList.Add("idle_" + exosuit.IdleIdx);
		arrayList.Add("running_" + exosuit.mRunIndex);
		arrayList.Add("backwardrun_" + exosuit.Run_BackwardIdx);
		arrayList.Add("capture_float_" + exosuit.Capture_AirIdx);
		arrayList.Add("capture_ground_" + exosuit.Capture_GroundIdx);
		arrayList.Add("roll_" + exosuit.mRollIndex);
		arrayList.Add("roll_recover_" + exosuit.Roll_RecoverIdx);
		arrayList.Add("jetpackhover_" + exosuit.JetpackIdx);
		arrayList.Add("jump_" + exosuit.Jump_BeginIdx);
		arrayList.Add("jump_at_apex_" + exosuit.Jump_FallIdx);
		arrayList.Add("jump_land_" + exosuit.Jump_LandIdx);
		arrayList.Add("new_aim_" + exosuit.ShootIdx);
		arrayList.Add("move_arm_" + exosuit.mRunIndex);
		foreach (AnimationClip animation in GameData.Animations)
		{
			if (arrayList.Contains(animation.name))
			{
				gameObject.GetComponent<Animation>().AddClip(UnityEngine.Object.Instantiate(animation) as AnimationClip, animation.name);
			}
		}
		return gameObject;
	}

	public void SpawnHitspark(Vector3 pos, Vector3 norm, int objType)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(hitspark1, pos, Quaternion.identity) as GameObject;
		gameObject.transform.localEulerAngles = new Vector3(norm.x, norm.y, norm.z);
		AudioSource audioSource = gameObject.GetComponent("AudioSource") as AudioSource;
		audioSource.volume = GameData.mGameSettings.mSoundVolume;
		int index = UnityEngine.Random.Range(0, 5);
		if (objType == 1)
		{
			audioSource.PlayOneShot(hitPlayerSounds[index]);
			return;
		}
		index = UnityEngine.Random.Range(0, 4);
		audioSource.PlayOneShot(hitWallSounds[index]);
	}

	public void activatePickup(int puIdx, int puType, int playerId)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("pickup");
		int num = 0;
		PickUp pickUp;
		while (true)
		{
			if (num < array.Length)
			{
				pickUp = (array[num].GetComponent("PickUp") as PickUp);
				if (pickUp == null)
				{
					Logger.trace("****** What?  There is a pickup named " + array[num].name + " without a script *****");
				}
				else if (pickUp.puIndex == puIdx)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		Logger.trace("<< activating pickup index " + puIdx + " of type " + puType + " on player id: " + playerId);
		Player player = players[playerId].GetComponent("Player") as Player;
		pickUp.mIsWaitingForServer = false;
		player.activatePickupOnPlayer(array[num], puType);
	}

	public void setPickupActive(int puIdx, float puTime)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("pickup");
		for (int i = 0; i < array.Length; i++)
		{
			PickUp pickUp = array[i].GetComponent("PickUp") as PickUp;
			if (pickUp.puIndex == puIdx)
			{
				pickUp.deactivatePickup(puTime);
			}
		}
	}

	public void SendMyActionPosition(int state, float posX, float posY)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 3);
			sFSObject.PutFloat("x", posX);
			sFSObject.PutFloat("y", posY);
			sFSObject.PutInt("moveState", state);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
			Logger.trace("Sent My Action Position " + GameData.MyPlayerId);
		}
	}

	public void SendOppReadyInMyGame(int oppWhoSpawned)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("Id", oppWhoSpawned);
			sFSObject.PutInt("msgType", 102);
			if (mHUD != null)
			{
			}
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	private void sendMyLoadingStatus(int status)
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("msgType", 111);
		sFSObject.PutInt("playerId", GameData.MyPlayerId);
		sFSObject.PutInt("status", status);
		sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
	}

	public void SendRoll()
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 26);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendAirdash()
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 27);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendFuelConsumed(int fuel)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 28);
			sFSObject.PutInt("fuel", fuel);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendActiveBoostInfo()
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 25);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendTaunt(int num)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 17);
			sFSObject.PutInt("num", num);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public virtual void SendMyShotPosition(Vector3 pos, float angle, float angleIncrement, float distance)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("msgType", 4);
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutFloat("x", pos.x);
			sFSObject.PutFloat("y", pos.y);
			sFSObject.PutFloat("angle", angle);
			sFSObject.PutFloat("inc", angleIncrement);
			sFSObject.PutFloat("dist", distance);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
			Logger.trace("Sent my shot position: " + GameData.MyPlayerId);
		}
	}

	public void SendSniperLine(Vector3 startPos, Vector3 endPos)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("msgType", 18);
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutFloat("xstart", startPos.x);
			sFSObject.PutFloat("ystart", startPos.y);
			sFSObject.PutFloat("xend", endPos.x);
			sFSObject.PutFloat("yend", endPos.y);
			Logger.trace("<< sending sniper shot " + startPos + " endPos: " + endPos);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendKillBullet(int idx)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("msgType", 14);
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("bnum", idx);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public virtual void SendMyGrenadePosition(Vector3 pos, float angle, int num, int type)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 5);
			sFSObject.PutFloat("x", pos.x);
			sFSObject.PutFloat("y", pos.y);
			sFSObject.PutInt("num", num);
			sFSObject.PutFloat("angle", angle);
			sFSObject.PutInt("type", type);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
			Logger.trace("send my grenade position: " + GameData.MyPlayerId);
		}
	}

	public virtual void SendGrenadeExplode(int num, Vector3 pos, int id)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", id);
			sFSObject.PutInt("msgType", 9);
			sFSObject.PutFloat("x", pos.x);
			sFSObject.PutFloat("y", pos.y);
			sFSObject.PutInt("num", num);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendGrenadeExplodePosition(Vector3 pos)
	{
		Logger.trace("SendGrenadeExplodePosition ");
	}

	public virtual void SendRocketExplode(int num, Vector3 pos)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 16);
			sFSObject.PutFloat("x", pos.x);
			sFSObject.PutFloat("y", pos.y);
			sFSObject.PutInt("num", num);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}

	public void SendOpponentInfo()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("playerId", GameData.MyPlayerId);
		sFSObject.PutInt("msgType", 23);
		sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
	}

	public void SendTimeUpdate()
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("playerId", GameData.MyPlayerId);
		sFSObject.PutInt("msgType", 24);
		sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
	}

	public virtual void SendChangeWeapon(int idx)
	{
		if (sfs != null)
		{
			m_networkManager.setUserVariable("weaponId", idx);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 6);
			sFSObject.PutInt("idx", idx);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
			Logger.trace("SendChangeWeapon: " + idx);
		}
	}

	public virtual void SendActivatePickupEvent(int pickupIdx, int pickupType, int pickupTime, int effectTime)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			Logger.trace("sending pickup activation request");
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 8);
			sFSObject.PutInt("myIdx", GameData.MyPlayerId - 1);
			sFSObject.PutInt("pType", pickupType);
			sFSObject.PutInt("pIdx", pickupIdx);
			sFSObject.PutInt("pTime", pickupTime);
			sFSObject.PutInt("eTime", effectTime);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
			Logger.trace("SendActivatePickupEvent: " + GameData.MyPlayerId + " of pickup type: " + pickupType + "  of pickup Index: " + pickupIdx);
		}
	}

	public void sendForceGameEnd()
	{
		if (sfs != null)
		{
			SFSObject parameters = new SFSObject();
			sfs.Send(new ExtensionRequest("gef", parameters, GameData.GameRoom));
		}
	}

	public void SendTransformToRemotePlayerObject(SFSObject data)
	{
		int @int = data.GetInt("playerId");
		GameObject gameObject = GameObject.Find("remote_" + @int);
		if ((bool)gameObject)
		{
			gameObject.SendMessage("ReceiveTransform", data);
		}
	}

	public void SendEventToRemotePlayerObject(SFSObject data, int fromUser)
	{
		GameObject gameObject = GameObject.Find(fromUser.ToString());
		if (gameObject == null)
		{
			gameObject = GameObject.Find("remote_" + fromUser);
			if (gameObject == null)
			{
				gameObject = GameObject.Find("localPlayer");
				Logger.trace("<< localPlayer receiving message");
			}
			else
			{
				Logger.trace("<< remote receiving message");
			}
		}
		else
		{
			Logger.trace("<< number receiving message");
		}
		if ((bool)gameObject)
		{
			gameObject.SendMessage("ReceiveEvent", data);
		}
		else
		{
			Logger.trace("<< can't find user.. should send to local");
		}
	}

	public void BattleHasEnded()
	{
		CleanupGame();
		mIsBattleOver = true;
	}

	public void BattleSummaryReceived()
	{
		mIsBattleSummaryReceived = true;
	}

	private void CleanupGame()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("grenade");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Grenade grenade = gameObject.GetComponent("Grenade") as Grenade;
			grenade.Explode();
		}
		for (int j = 0; j < 8 && !(players[j] == null); j++)
		{
			NetworkTransformReceiver obj = players[j].GetComponent("NetworkTransformReceiver") as NetworkTransformReceiver;
			UnityEngine.Object.Destroy(obj);
			NetworkTransformSender obj2 = players[j].GetComponent("NetworkTransformSender") as NetworkTransformSender;
			UnityEngine.Object.Destroy(obj2);
		}
	}

	public virtual void DrawSniperLine(float x0, float y0, float x1, float y1)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(mSniperLine) as GameObject;
		LineRenderer lineRenderer = gameObject.GetComponent("LineRenderer") as LineRenderer;
		lineRenderer.SetPosition(0, new Vector3(x0, y0, 0f));
		lineRenderer.SetPosition(1, new Vector3(x1, y1, 0f));
	}

	[Obsolete]
	public virtual void QuitGame(int quitCondition)
	{
		Logger.trace("GamePlay::QuitGame with condition " + quitCondition);
		if (!mImQuitting)
		{
			mImQuitting = true;
			bool flag = false;
			string title = string.Empty;
			string message = string.Empty;
			GameObject gameObject = GameObject.Find("Tracker");
			TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
			switch (quitCondition)
			{
			case 1:
				flag = true;
				title = "Your Game Play Timed Out";
				message = "Sorry, You have been disconnected for inactivity.";
				break;
			case 2:
				flag = true;
				title = "Undefined Error";
				message = "Sorry, There was an undefined error in the game.\nPlease Try Again.";
				break;
			case 3:
				flag = true;
				title = "Your match will end now.";
				message = "All the other players have left the match.\n You're the last player standing!";
				break;
			case 4:
				flag = true;
				title = "Your match will end now.";
				message = "All the members of Banzai Squadron have left the match.\nAtlas Brigade rules!";
				break;
			case 5:
				flag = true;
				title = "Your match will end now.";
				message = "All the members of Atlas Brigade have left the match.\nBanzai Squadron rules!";
				break;
			case 7:
				flag = true;
				title = "Your match will end now.";
				message = "Sorry, this room has been closed.";
				break;
			case 10:
				flag = true;
				title = "Your match will end now.";
				message = "Sorry, you don't have a world to load.";
				break;
			case 11:
				flag = true;
				title = "Your match will end now.";
				message = "Sorry, you shouldn't be playing.";
				break;
			case 12:
				flag = true;
				title = "Your match will end now.";
				message = "Sorry, you seem to have lost your connection to the server.";
				trackerScript.AddMetric(TrackerScript.Metric.MATCH_LEFT_ERROR);
				break;
			default:
				trackerScript.AddMetric(TrackerScript.Metric.MATCH_LEFT_QUIT);
				break;
			}
			m_networkManager.LeaveRoom();
			GameData.CurPlayState = GameData.PlayState.GAME_IS_QUITTING;
			GameData.clearAllSfsPlayers();
			GameData.DestroyCurrentGame();
			if (flag)
			{
				MessageBox.ResetWindowPosition();
				MessageBox.AddMessage(title, message, null, false, MessageBox.MessageType.MB_OK, null);
			}
			LoadLevel("GameHome");
		}
	}

	private void OnApplicationQuit()
	{
		Logger.trace("GamePlay::OnApplicationQuit");
	}
}
