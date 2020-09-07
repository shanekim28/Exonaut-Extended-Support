using LitJson;
using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TitleLoading : MonoBehaviour
{
	private enum ScriptDataType
	{
		Unknown,
		String,
		Color,
		Int,
		Float
	}

	private delegate void State();

	private State state;

	private int mCurrentBundle;

	private bool mBundleIsLoading;

	private State[] mLoadBundleList;

	private int mTikUpdate;

	private int mTikDuration;

	private int mPlayStatus = -1;

	private bool mIsConfigLoaded;

	private bool mIsGameDataLoaded;

	private bool mIsCookiesLoaded;

	private bool mIsAuthenticationReceived;

	private string mStatus = string.Empty;

	private Hashtable tempExoPlayerData;

	public GameObject GameMusic;

	public GUISkin mSharedSkin;

	private bool bLoading;

	private TextAsset mVersion;

	private NetworkManager mNetworkManager;

	public GameObject mTracker;

	private State nextState;

	private bool isTestLoggedLocal;

	private bool isTestNewPlayer;

	private string eventBundleName = string.Empty;

	public float StoryTimer;

	public Texture2D StoryBG;

	public Texture2D StoryTextBG;

	public Texture2D StoryTitle;

	public Texture2D StoryText;

	public Texture2D StoryExoSymbol;

	// TODO: Set resolution
	public Rect StoryBGRect;

	public Rect StoryTextBGRect;

	public Rect StoryTitleRect;

	public Rect StoryTextRect;

	public Rect StoryExoSymbolRect;

	private string lastHover = string.Empty;

	private void Start()
	{
		GameMusic = (UnityEngine.Object.Instantiate(GameMusic) as GameObject);
		state = waitForLoadConfig;
		mSharedSkin = GUIUtil.mInstance.mSharedSkin;
		mVersion = (Resources.Load("version/number") as TextAsset);
		GameData.version = mVersion.text;
		if (mVersion == null)
		{
			Debug.Log("************* unable to read version *********");
		}
		mTracker = new GameObject();
		mTracker.name = "Tracker";
		UnityEngine.Object.DontDestroyOnLoad(mTracker);
		TrackerScript trackerScript = mTracker.AddComponent<TrackerScript>();
		trackerScript.AddMetric(TrackerScript.Metric.WANT_TO_PLAY);
		GameObject gameObject = GameObject.Find("NetworkManager");
		mNetworkManager = (gameObject.GetComponent<NetworkManager>() as NetworkManager);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (GameData.MyPlayStatus == 1 || GameData.MyPlayStatus == 2)
		{
			if (Input.GetKey(KeyCode.Space))
			{
				StoryTimer += Time.deltaTime * 1000f;
			}
			StoryTimer += Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		if (state != null)
		{
			state();
		}
	}

	private void setPause(int duration, State callback)
	{
		mTikUpdate = 0;
		mTikDuration = duration;
		nextState = callback;
		state = pauseUpdate;
	}

	private void pauseUpdate()
	{
		mTikUpdate++;
		if (mTikUpdate >= mTikDuration)
		{
			state = nextState;
			nextState = null;
		}
	}

	private void waitForLoadConfig()
	{
		if (mIsConfigLoaded || mNetworkManager.m_serviceSet)
		{
			CheckEventBundle();
			state = null;
		}
	}

	private void waitForGameDataLoaded()
	{
		if (mIsGameDataLoaded)
		{
			if (GameData.WeaponMods.Count == 0)
			{
				GameData.InitWeaponMods();
			}
			if (GameData.WeaponList.Count == 0)
			{
				GameData.InitWeapons();
			}
			if (GameData.MasterSuitList.Count == 0)
			{
				GameData.InitSuitList(string.Empty);
			}
			loadAssetBundles();
			state = waitForAssetLoad;
		}
	}

	private void loadAssetBundles()
	{
		mLoadBundleList = new State[4];
		mLoadBundleList[0] = LoadPickups;
		mLoadBundleList[1] = LoadWeapons;
		mLoadBundleList[2] = LoadAnimations;
		mLoadBundleList[3] = LoadLowPolySuits;
	}

	private void waitForAssetLoad()
	{
		if (!mBundleIsLoading)
		{
			Logger.trace("<< mCurrentBundle " + mCurrentBundle + " out of " + mLoadBundleList.Length);
			if (mCurrentBundle < mLoadBundleList.Length)
			{
				Logger.traceAlways("Loading bundle");
				mLoadBundleList[mCurrentBundle]();
				mCurrentBundle++;
			}
			else
			{
				Logger.traceAlways("Checking login");
				checkLogin();
			}
		}
	}

	private void checkLogin()
	{
		mStatus = "Checking Log In . . .";
		if (Application.isEditor || Debug.isDebugBuild || Application.dataPath.Contains("cn-internal"))
		{
			CookieReader.isLoggedInReturn = true;
			setPause(30, IsLoggedIn);
		}
		else
		{
			setPause(60, readCNCookies);
		}
	}

	private void readCNCookies()
	{
		Debug.Log("readCNCookies " + mIsCookiesLoaded);
		if (!mIsCookiesLoaded && CookieReader.loaded)
		{
			mIsCookiesLoaded = true;
			CookieReader.CheckMSIBLoggedIn();
			state = IsLoggedIn;
			Debug.Log("CookieReader.loaded is true");
		}
	}

	private void IsLoggedIn()
	{
		if (!CookieReader.isLoggedInReturn)
		{
			return;
		}
		if (CookieReader.isLoggedIn)
		{
			mStatus += " Login Found ... wait for authentication";
			GameData.MyAuthid = CookieReader.GetCookieValue("authid");
			GameData.MyTEGid = CookieReader.GetCookieValue("TEGid");
			GameData.MyDisplayName = CookieReader.GetCookieValue("dname");
			mStatus = mStatus + " MyDisplayName " + GameData.MyDisplayName;
			authenticatePlayer();
			state = waitForPlayerAuthentication;
		}
		else if (isTestLoggedLocal)
		{
			if (isTestNewPlayer)
			{
				mPlayStatus = 2;
				mIsAuthenticationReceived = true;
				state = waitForPlayerAuthentication;
			}
			else
			{
				mPlayStatus = 3;
				authenticatePlayer();
				state = waitForPlayerAuthentication;
			}
		}
		else
		{
			mStatus += "  You are not logged in to Cartoon Network?";
			GameData.MyPlayStatus = 1;
			setPause(60, confirmPlayer);
		}
	}

	private void isAuthorized()
	{
	}

	private void waitForPlayerAuthentication()
	{
		if (!mIsAuthenticationReceived)
		{
			return;
		}
		GameData.MyPlayStatus = mPlayStatus;
		switch (mPlayStatus)
		{
		case 1:
			mStatus += "  Cartoon Network Authentication Failed.";
			GameData.MyPlayStatus = 1;
			setPause(60, confirmPlayer);
			break;
		case 2:
			GameData.MyPlayStatus = 2;
			setPause(60, confirmPlayer);
			break;
		case 3:
			if (isTestLoggedLocal)
			{
				loadTestExonautPlayer();
			}
			else
			{
				loadExonautPlayer();
			}
			AchievementManager.SendStat(AchievementManager.ExonautStats.DaysPlayedInARow, 1);
			mStatus = mStatus + "  Welcome Back " + GameData.MyDisplayName + "!  Get Ready to Play!";
			setPause(60, playAsExonaut);
			break;
		}
	}

	private void confirmPlayer()
	{
		state = waitForPlay;
	}

	private void waitForPlay()
	{
	}

	private void loadGuest()
	{
		GameData.MyPlayStatus = 1;
		int[] array = (GameData.MyFactionId != 1) ? GameData.AtlasDefaultSuits : GameData.BanzaiDefaultSuits;
		int[] array2 = array;
		foreach (int id in array2)
		{
			GameData.AddOwnedSuit(id);
		}
		int[] guestSuitList = GameData.GuestSuitList;
		foreach (int id2 in guestSuitList)
		{
			GameData.AddOwnedSuit(id2);
		}
		int num = UnityEngine.Random.Range(0, array.Length - 1);
		GameData.MySuitID = array[num];
		mStatus = " Loading Guest . . .";
		setPause(60, playAsGuest);
	}

	private void playAsGuest()
	{
		if ((GameData.MATCH_MODE != GameData.Build.DEBUG && !PlayerPrefs.HasKey("LastTraining")) || (GameData.MATCH_MODE == GameData.Build.DEBUG && Input.GetKey(KeyCode.Space)))
		{
			setPause(60, playAsNewPlayer);
			return;
		}
		DateTime result;
		DateTime.TryParse(PlayerPrefs.GetString("LastTraining"), out result);
		if ((result - DateTime.Today).TotalMinutes > 0.0)
		{
			mStatus = "Please wait.";
			Application.LoadLevel("GameHome");
			state = waitForLogin;
		}
		else
		{
			setPause(60, playAsNewPlayer);
		}
	}

	private void waitForLogin()
	{
	}

	private void loadNewPlayer()
	{
		mStatus = " Loading New Player . . .";
		if (tempExoPlayerData != null)
		{
			GameData.MyTEGid = (string)tempExoPlayerData["TEGID"];
			GameData.MyLogin = (string)tempExoPlayerData["LoginName"];
		}
		Logger.trace("Load New Player !!!");
		Logger.trace("TEGid " + GameData.MyTEGid);
		Logger.trace("login " + GameData.MyLogin);
		Logger.trace("dname " + GameData.MyDisplayName);
		Logger.trace("suit " + GameData.MySuitID);
		Logger.trace("faction " + GameData.MyFactionId);
	}

	private IEnumerator LoadTutorial()
	{
		mStatus = "Loading Boot Camp.\n0% Loaded";
		GameData.WorldID = 0;
		GameData.WorldVersion = "tutorial";
		GameData.GameType = 2;
		GameData.MyTutorialStep = 2;
		GameData.MyPlayerId = 1;
		SFSObject player = new SFSObject();
		player.PutInt("playStatus", GameData.MyPlayStatus);
		player.PutInt("playerId", GameData.MyPlayerId);
		player.PutUtfString("playerName", GameData.MyDisplayName);
		player.PutInt("suitIdx", GameData.MySuitID);
		player.PutInt("playerFaction", GameData.MyFactionId);
		player.PutInt("textureIdx", GameData.MyTextureID);
		player.PutInt("weaponIdx", GameData.MyWeaponID);
		player.PutInt("powers", GameData.MyPowers);
		player.PutInt("level", GameData.MyLevel);
		player.PutBool("leveledUp", val: false);
		GameData.addPlayer(1, player);
		GameData.LoadWorld();
		while (GameData.getWorldById(GameData.WorldID) == null)
		{
			mStatus = "Loading Boot Camp.\n" + (int)(GameData.WorldLoadProgress * 100f) + "% Loaded";
			yield return new WaitForEndOfFrame();
		}
		GameData.LoadSpawnPoints();
		while (GameData.PickupSpawns == null)
		{
			yield return new WaitForSeconds(1f);
		}
		AsyncOperation async = Application.LoadLevelAsync("TutorialGamePlay");
		yield return async.isDone;
	}

	private void loadTestNewPlayer()
	{
		mStatus = " Loading Test New Player . . .";
		int num = 13125;
		GameData.MyTEGid = "tegid" + num;
		GameData.MyLogin = "testplayer" + num;
		GameData.MyDisplayName = "Test Player" + num;
	}

	private void playAsNewPlayer()
	{
		StartCoroutine(LoadTutorial());
		state = waitForLogin;
	}

	private void OnLevelWasLoaded(int level)
	{
		Logger.trace("############################# LEVEL LOADED: " + level);
	}

	private void loadExonautPlayer()
	{
		GameData.MyExonautId = Convert.ToInt32(tempExoPlayerData["ID"]);
		GameData.MyTEGid = (string)tempExoPlayerData["TEGID"];
		GameData.MyLogin = (string)tempExoPlayerData["LoginName"];
		GameData.MyFactionId = Convert.ToInt32(tempExoPlayerData["Faction"]);
		GameData.MyLevel = Convert.ToInt32(tempExoPlayerData["Level"]);
		GameData.MyBattleXP = 0;
		GameData.MyTotalXP = Convert.ToInt32(tempExoPlayerData["XP"]);
		GameData.MyBattleCredits = 0;
		GameData.MyTotalCredits = Convert.ToInt32(tempExoPlayerData["Credits"]);
		GameData.MyExonautToken = (string)tempExoPlayerData["SessionID"];
		GameData.MySuitID = Convert.ToInt32(tempExoPlayerData["LastSuit"]);
		List<int> list = (List<int>)tempExoPlayerData["ownedSuits"];
		foreach (int item in list)
		{
			GameData.AddOwnedSuit(item);
		}
		List<Hashtable> list2 = (List<Hashtable>)tempExoPlayerData["curMissions"];
		foreach (Hashtable item2 in list2)
		{
			GameData.addCurrentMission(item2);
		}
		List<Hashtable> list3 = (List<Hashtable>)tempExoPlayerData["curMissionProgress"];
		foreach (Hashtable item3 in list3)
		{
			GameData.addMissionInProgress(item3);
		}
		mStatus = " Loading Exonaut Player . . . " + GameData.MyDisplayName;
	}

	private void loadTestExonautPlayer()
	{
		GameData.MyExonautId = 514;
		GameData.MyTEGid = string.Empty;
		GameData.MyAuthid = string.Empty;
		GameData.MyLogin = "stagenobody";
		GameData.MyDisplayName = "Awesome Bob";
		GameData.MyFactionId = 2;
		GameData.MyLevel = 10;
		GameData.MyBattleXP = 0;
		GameData.MyBattleCredits = 0;
		GameData.MyTotalCredits = 2500;
		GameData.MyExonautToken = "sTok1";
		GameData.AddOwnedSuit(22);
		GameData.MySuitID = 22;
		mStatus = " Loading TEST OK Player . . . " + GameData.MyDisplayName;
		mStatus = "Faction is " + GameData.getFactionDisplayName(GameData.MyFactionId);
		mStatus = mStatus + " Current SuitId is " + GameData.MySuitID;
	}

	private void playAsExonaut()
	{
		Application.LoadLevel("GameHome");
	}

	private void OnGUI()
	{
		GUIUtil.GUIEnable(bEnable: true);
		// TODO: Fix GUI skins
		//GUI.Label(new Rect(0f, 5f, 900f, 40f), mStatus, mSharedSkin.GetStyle("invis"));
		//GUI.Label(new Rect(400f, 580f, 100f, 20f), mVersion.text, mSharedSkin.GetStyle("invis"));
		if (bLoading)
		{
			MessageBox.Local("Loading...", mStatus, null, false, MessageBox.MessageType.MB_NoButtons);
		}
		if (GameData.MyPlayStatus == 1)
		{
			DrawStory(bIsGuest: true);
		}
		else if (GameData.MyPlayStatus == 2)
		{
			DrawStory(bIsGuest: false);
		}
	}

	private void loadConfig()
	{
		string empty = string.Empty;
		empty = ((!Application.isEditor) ? (Application.dataPath + "/config.xml") : ("file://" + Application.dataPath + "/config.xml"));
		WWW www = new WWW(empty);
		mStatus += " Loading Server Configuration";
		StartCoroutine(waitForConfigLoad(www));
	}

	private IEnumerator waitForConfigLoad(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
			if (xmlDoc == null)
			{
				Logger.traceError("## Config File Failed ");
				yield break;
			}
			XmlNodeList nodes = xmlDoc.ChildNodes;
			XmlElement config = (XmlElement)nodes.Item(1);
			XmlNodeList data = config.ChildNodes;
			XmlNode bundle = data.Item(0);
			XmlElement bundlepath = (XmlElement)bundle.FirstChild;
			GameData.BUNDLE_PATH = bundlepath.InnerText + "/";
			mStatus = "Bundle Path = " + GameData.BUNDLE_PATH;
			XmlNode smartfox = data.Item(1);
			XmlAttribute mode = (XmlAttribute)smartfox.Attributes.GetNamedItem("mode");
			string matchMode = "debug";
			if (mode != null)
			{
				matchMode = mode.Value;
				GameData.MATCH_MODE = GameData.Build.PRODUCTION;
				if (Application.isEditor || Application.absoluteURL.Contains("http://exonaut/"))
				{
					GameData.MATCH_MODE = GameData.Build.DEBUG;
					mStatus += "\n DEV BUILD";
					yield return new WaitForSeconds(2f);
				}
			}
			XmlElement smartfoxpath = (XmlElement)smartfox.FirstChild;
			GameData.MATCH_SERVER_IP = smartfoxpath.InnerText;
			mStatus = "Server:" + GameData.MATCH_SERVER_IP + " mode:" + matchMode;
			XmlNode services = data.Item(2);
			int i = 0;
			foreach (XmlNode server in services.ChildNodes)
			{
				GameData.SERVICE_CLUSTER.Insert(i, server.InnerText);
				i++;
			}
			GameData.SetServicePath();
			mIsConfigLoaded = true;
		}
		else
		{
			mStatus = "Sorry, game servers are down at the moment. Please check back soon.";
			Logger.traceError("www error:" + www.error);
		}
	}

	private void getGameData()
	{
		string empty = string.Empty;
		empty = ((!Application.isEditor) ? (GameData.SERVICE_PATH + "/gamedata.json") : ("file://" + Application.dataPath + "/gamedata.json"));
		Logger.traceError("Reading Game Data from " + empty);
		WWW www = new WWW(empty);
		mStatus = "Getting Game Data . . .";
		StartCoroutine(waitForGameData(www));
	}

	private IEnumerator waitForGameData(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			mStatus += " Game Data Loaded!";
			JsonData data = JsonMapper.ToObject(www.text);
			GameData.SuitJson = data["suits"];
			GameData.WeaponJson = data["weapons"];
			GameData.WeaponModJson = data["mods"];
			mIsGameDataLoaded = true;
		}
		else
		{
			Logger.traceError("<< error reading gamedata.json");
			mStatus = "Sorry, game servers are down at the moment. Please check back soon.";
		}
	}

	private void authenticatePlayer()
	{
		string url = GameData.SERVICE_PATH + "/ExonautPlayerAuthenticate";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("TEGid", GameData.MyTEGid);
		wWWForm.AddField("authid", GameData.MyAuthid);
		WWW www = new WWW(url, wWWForm);
		mStatus += "  Authenticating Exonaut Player. . .";
		StartCoroutine(waitForAuthenticate(www));
	}

	private IEnumerator waitForAuthenticate(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			Logger.trace("Player Authenticate Received " + www.text);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
			XmlNode node = xmlDoc.FirstChild;
			if (node == null)
			{
				mPlayStatus = 1;
				Logger.traceError("Authentication was NULL");
			}
			else
			{
				XmlAttributeCollection data = node.Attributes;
				XmlAttribute status = (XmlAttribute)data.GetNamedItem("status");
				switch (status.Value)
				{
				case "guest":
					mPlayStatus = 1;
					Logger.trace("Authentication was GUEST");
					break;
				case "exoplayer":
				{
					mPlayStatus = 3;
					tempExoPlayerData = new Hashtable();
					foreach (XmlAttribute x in data)
					{
						tempExoPlayerData.Add(x.Name, x.Value);
						Logger.trace(" key:" + x.Name + "  val:" + x.Value);
					}
					List<int> ownedSuits = new List<int>();
					XmlNode suitNode = xmlDoc.FirstChild.FirstChild;
					IEnumerator suits = suitNode.GetEnumerator();
					if (suits == null)
					{
						Logger.traceError("<< No suits were found");
					}
					while (suits.MoveNext())
					{
						XmlNode suit = (XmlNode)suits.Current;
						ownedSuits.Add(Convert.ToInt32(suit.InnerText));
					}
					tempExoPlayerData.Add("ownedSuits", ownedSuits);
					List<Hashtable> curMissions = new List<Hashtable>();
					XmlNodeList missions = xmlDoc.GetElementsByTagName("mission");
					for (int j = 0; j < missions.Count; j++)
					{
						XmlNode mission = missions.Item(j);
						XmlAttributeCollection mdata = mission.Attributes;
						curMissions.Add(new Hashtable
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
							},
							{
								"image",
								mdata.GetNamedItem("image").Value.Replace(".png", string.Empty)
							}
						});
					}
					tempExoPlayerData.Add("curMissions", curMissions);
					List<Hashtable> curMissionProgress = new List<Hashtable>();
					XmlNodeList missionsProg = xmlDoc.GetElementsByTagName("missionProg");
					for (int i = 0; i < missionsProg.Count; i++)
					{
						XmlNode prog = missionsProg.Item(i);
						XmlAttributeCollection pData = prog.Attributes;
						curMissionProgress.Add(new Hashtable
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
					tempExoPlayerData.Add("curMissionProgress", curMissionProgress);
					mPlayStatus = 3;
					Logger.trace("Authentication was EXONAUT PLAYER");
					break;
				}
				case "newplayer":
					tempExoPlayerData = new Hashtable();
					foreach (XmlAttribute x2 in data)
					{
						tempExoPlayerData.Add(x2.Name, x2.Value);
					}
					mPlayStatus = 2;
					Logger.trace("Authentication was NEW PLAYER");
					break;
				}
			}
			mIsAuthenticationReceived = true;
		}
		else
		{
			mStatus = "There was an error:" + www.error.ToString();
		}
	}

	private void LoadPickups()
	{
		mBundleIsLoading = true;
		string text = GameData.BUNDLE_PATH + "pickups/pickupBundle.unity3d";
		Debug.Log("Loading Pickups from " + text);
		mStatus = "Loading Pickups . . .";
		WWW www = new WWW(text);
		StartCoroutine(WaitForPickupRequest(www));
	}

	private IEnumerator WaitForPickupRequest(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			yield return www;
			AssetBundle assetBundle = www.assetBundle;
			string[] pickupList = GameData.PickupList;
			Logger.trace("<< num pickups: " + pickupList.Length);
			foreach (string str in pickupList)
			{
				AssetBundleRequest abr = assetBundle.LoadAsync(str, typeof(GameObject));
				yield return abr;
				GameData.Pickups.Add(abr.asset as GameObject);
			}
			mStatus += "  ... Pickups Loaded!";
			mBundleIsLoading = false;
		}
		else
		{
			Logger.trace("Pickup List Failed " + www.error);
		}
	}

	private void LoadWeapons()
	{
		Debug.LogError("Loading Weapons");
		mStatus = "Loading Weapons . . .";
		mBundleIsLoading = true;
		string url = GameData.BUNDLE_PATH + "weapons/weaponsBundle.unity3d";
		WWW www = new WWW(url);
		StartCoroutine(WaitForWeaponsRequest(www));
	}

	private IEnumerator WaitForWeaponsRequest(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			yield return www;
			AssetBundle assetBundle = www.assetBundle;
			UnityEngine.Object[] weaponsArray = assetBundle.LoadAll(typeof(GameObject));
			for (int i = 0; i < weaponsArray.Length; i++)
			{
				Logger.trace("<< Weapon Loaded: " + weaponsArray[i].name);
				GameData.Weapons.Add(weaponsArray[i] as GameObject);
			}
			mStatus += " ... Weapons Loaded!";
			mBundleIsLoading = false;
		}
	}

	private void LoadAnimations()
	{
		mStatus = "Loading Animations . . .";
		mBundleIsLoading = true;
		GameData.AnimationNames.Add("idle");
		GameData.AnimationNames.Add("crouch");
		GameData.AnimationNames.Add("roll");
		GameData.AnimationNames.Add("running");
		GameData.AnimationNames.Add("capture_ground");
		GameData.AnimationNames.Add("capture_float");
		GameData.AnimationNames.Add("jump");
		GameData.AnimationNames.Add("shooting");
		GameData.AnimationNames.Add("airdash");
		GameData.AnimationNames.Add("dash_backwards");
		GameData.AnimationNames.Add("dash_backwards_rec");
		GameData.AnimationNames.Add("roll_recover");
		GameData.AnimationNames.Add("jump_at_apex");
		GameData.AnimationNames.Add("jump_land");
		GameData.AnimationNames.Add("jetpackhover");
		GameData.AnimationNames.Add("backwardrun");
		GameData.AnimationNames.Add("airdash_recover");
		GameData.AnimationNames.Add("new_aim");
		string text = GameData.BUNDLE_PATH + "animations/animationClipBundle.unity3d";
		WWW www = new WWW(text);
		Debug.Log("<< animations from : " + text);
		StartCoroutine(WaitForAnimationRequest(www));
	}

	private IEnumerator WaitForAnimationRequest(WWW www)
	{
		yield return www;
		if (www.error != null)
		{
			yield break;
		}
		yield return www;
		Logger.trace("<< Loading Animations . . .");
		AssetBundle assetBundle = www.assetBundle;
		UnityEngine.Object[] anims = assetBundle.LoadAll(typeof(AnimationClip));
		for (int i = 0; i < anims.Length; i++)
		{
			GameData.Animations.Add(anims[i] as AnimationClip);
			if (anims[i].name.Contains("running"))
			{
				int numidx = anims[i].name.IndexOf("_");
				string toCreate = anims[i].name.Substring(numidx);
				AnimationClip ac = UnityEngine.Object.Instantiate(anims[i] as AnimationClip) as AnimationClip;
				ac.name = "move_arm" + toCreate;
				GameData.Animations.Add(ac);
			}
		}
		mStatus += " ... Animations Loaded!";
		mBundleIsLoading = false;
	}

	private void LoadLowPolySuits()
	{
		mStatus = "Loading Suits . . .";
		mBundleIsLoading = true;
		string text = null;
		//text = ((!Application.isEditor) ? (GameData.BUNDLE_PATH + "suits/low_suits.unity3d") : ("file://" + Application.dataPath + "/low_suits.unity3d"));
		text = GameData.BUNDLE_PATH + "suits/low_suits.unity3d";
		WWW www = new WWW(text);
		StartCoroutine(WaitForLowSuitsRequest(www));
	}

	private IEnumerator WaitForLowSuitsRequest(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			yield return www;
			Logger.trace("<< Loading Suits . . .");
			AssetBundle assetBundle = www.assetBundle;
			UnityEngine.Object[] suits = assetBundle.LoadAll(typeof(GameObject));
			for (int i = 0; i < suits.Length; i++)
			{
				foreach (Exosuit Suit in GameData.MasterSuitList.Values)
				{
					if (suits[i].name.Contains(Suit.mSuitFileName))
					{
						string textureName = Suit.mSuitFileName + "_sheet_1";
						AssetBundleRequest abr = assetBundle.LoadAsync(textureName, typeof(Material));
						yield return abr;
						GameData.setLowPolySuitIsLoaded(texture: abr.asset as Material, suitId: Suit.mSuitId, model: suits[i] as GameObject);
					}
				}
			}
			mStatus += " ... Suits Loaded!";
			mBundleIsLoading = false;
		}
	}

	private void CheckEventBundle()
	{
		mStatus = "Check EventBundle . . .";
		mBundleIsLoading = true;
		string text = GameData.BUNDLE_PATH + "events/event.txt";
		Debug.Log("[TitleLoading::CheckEventBundle] - " + text);
		WWW www = new WWW(text);
		StartCoroutine(WaitForCheckEventBundleRequest(www));
	}

	private IEnumerator WaitForCheckEventBundleRequest(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			yield return www;
			string textasset = www.text;
			if (textasset != null)
			{
				Debug.Log("Found event.txt: " + textasset);
				Debug.Log("www: " + www.ToString());
			}
			if (textasset != string.Empty)
			{
				eventBundleName = textasset;
				LoadEventBundle();
			}
			else
			{
				mBundleIsLoading = false;
				getGameData();
				state = waitForGameDataLoaded;
			}
		}
		else
		{
			mBundleIsLoading = false;
			getGameData();
			state = waitForGameDataLoaded;
		}
	}

	private void LoadEventBundle()
	{
		mStatus = "Loading EventBundle . . .";
		mBundleIsLoading = true;
		string text = GameData.BUNDLE_PATH + "events/" + eventBundleName;
		Debug.Log("[TitleLoading::LoadEventBundle] - " + text);
		WWW www = new WWW(text);
		StartCoroutine(WaitForEventBundleRequest(www));
	}

	private IEnumerator WaitForEventBundleRequest(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			yield return www;
			Logger.trace("<< Loading Event Bundle . . .");
			AssetBundle assetBundle = www.assetBundle;
			if (assetBundle == null)
			{
				mBundleIsLoading = false;
				getGameData();
				state = waitForGameDataLoaded;
				yield break;
			}
			AssetBundleRequest abr = assetBundle.LoadAsync("event_manifest", typeof(TextAsset));
			yield return abr;
			if (abr != null)
			{
				GameData.EventManifest = (abr.asset as TextAsset);
			}
			Debug.Log("manifest: " + GameData.EventManifest);
			string[] num_events = GameData.EventManifest.text.Split(new char[1]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array = num_events;
			foreach (string events in array)
			{
				if (events == null || events == string.Empty)
				{
					continue;
				}
				string[] info = events.Split(',');
				string key2 = Convert.ToString(info[0]);
				string val3 = Convert.ToString(info[1]);
				string type2 = Convert.ToString(info[2]);
				key2 = key2.Replace(" ", string.Empty);
				val3 = val3.Replace(" ", string.Empty);
				type2 = type2.Replace(" ", string.Empty);
				string[] objandextension = val3.Split('.');
				val3 = objandextension[0];
				Debug.Log("<< val: " + val3);
				EventTypes LoadType = EventTypes.Unknown;
				try
				{
					LoadType = (EventTypes)(int)Enum.Parse(typeof(EventTypes), type2);
				}
				catch (ArgumentException)
				{
					Logger.traceError(type2 + " is not a valid type");
				}
				switch (LoadType)
				{
				case EventTypes.GameObject:
				{
					GameObject go = assetBundle.Load(val3, typeof(GameObject)) as GameObject;
					if (go == null)
					{
						Debug.Log("<< couldn't find " + val3);
					}
					else
					{
						Debug.Log("adding gameobject [" + key2 + "] = " + go.name);
					}
					GameData.eventObjects.Add(key2, go);
					break;
				}
				case EventTypes.Texture2D:
				{
					Texture2D t2D = assetBundle.Load(val3, typeof(Texture2D)) as Texture2D;
					Debug.Log("adding texture2d [" + key2 + "] = " + t2D.name);
					GameData.eventObjects.Add(key2, t2D);
					break;
				}
				case EventTypes.AudioClip:
				{
					AudioClip Clip = assetBundle.Load(val3, typeof(AudioClip)) as AudioClip;
					if (Clip == null)
					{
						Debug.Log("clip " + key2 + " is null");
						break;
					}
					Debug.Log("adding audioclip [" + key2 + "] = " + Clip.name);
					GameData.eventObjects.Add(key2, Clip);
					break;
				}
				case EventTypes.TextAsset:
				{
					TextAsset Text = assetBundle.Load(val3, typeof(TextAsset)) as TextAsset;
					Debug.Log("adding textasset [" + key2 + "] = " + Text.name);
					GameData.eventObjects.Add(key2, Text);
					break;
				}
				default:
					Logger.traceError("unknown type: [" + key2 + "] = " + val3);
					break;
				}
			}
			if (GameData.DoesEventExist("titlecard_particle"))
			{
				GameObject particle = GameData.eventObjects["titlecard_particle"] as GameObject;
				GameObject particleObject = UnityEngine.Object.Instantiate(particle) as GameObject;
				particleObject.transform.position = new Vector3(0f, 3f, 9f);
			}
			mStatus += " ... Event Bundle Loaded!";
			mBundleIsLoading = false;
			getGameData();
			state = waitForGameDataLoaded;
			OnEventLoaded();
		}
		else
		{
			mBundleIsLoading = false;
			getGameData();
			state = waitForGameDataLoaded;
		}
	}

	public void OnEventLoaded()
	{
		TextAsset textAsset = GameData.eventObjects["Holiday_Script_Data"] as TextAsset;
		if (textAsset != null)
		{
			string[] array = textAsset.text.Split(new char[1]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text == null || text == string.Empty)
				{
					continue;
				}
				string[] array3 = text.Split(new char[1]
				{
					','
				}, 3);
				string text2 = Convert.ToString(array3[0]);
				text2 = text2.Replace(" ", string.Empty);
				if (text2[0] == '/' && text2[1] == '/')
				{
					Debug.Log("Commented out: " + text);
					continue;
				}
				string text3 = Convert.ToString(array3[1]);
				string text4 = Convert.ToString(array3[2]);
				text3 = text3.Replace(" ", string.Empty);
				ScriptDataType scriptDataType = ScriptDataType.Unknown;
				try
				{
					scriptDataType = (ScriptDataType)(int)Enum.Parse(typeof(ScriptDataType), text2);
				}
				catch (ArgumentException)
				{
					Logger.traceError(text2 + " is not a valid type");
				}
				switch (scriptDataType)
				{
				case ScriptDataType.Color:
				{
					Color color = default(Color);
					text4 = text4.Replace("(", string.Empty);
					text4 = text4.Replace(")", string.Empty);
					text4 = text4.Replace(" ", string.Empty);
					string[] array4 = text4.Split(',');
					for (int j = 0; j < text4.Length && j < 4; j++)
					{
						color[j] = float.Parse(array4[j]);
					}
					Debug.Log("adding color [" + text3 + "] = " + color);
					GameData.eventObjects.Add(text3, color);
					break;
				}
				case ScriptDataType.String:
					GameData.eventObjects.Add(text3, text4);
					Debug.Log("adding string [" + text3 + "] = " + text4);
					break;
				case ScriptDataType.Int:
				{
					int num2 = int.Parse(text4);
					GameData.eventObjects.Add(text3, num2);
					Debug.Log("adding int [" + text3 + "] = " + num2);
					break;
				}
				case ScriptDataType.Float:
				{
					float num = float.Parse(text4);
					GameData.eventObjects.Add(text3, num);
					Debug.Log("adding float [" + text3 + "] = " + num);
					break;
				}
				}
			}
		}
		StartCoroutine(HolidayEvent.OnEventLoaded());
	}

	private void DrawStory(bool bIsGuest)
	{
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		GUI.DrawTexture(StoryBGRect, StoryBG);
		GUI.color = new Color(1f, 1f, 1f, Mathf.Max(1f - StoryTimer * 2f, 0f));
		//GUI.Box(StoryBGRect, GUIContent.none, mSharedSkin.GetStyle("blackbox"));
		GUI.color = new Color(1f, 1f, 1f, StoryTimer - 1.5f);
		Rect storyTextBGRect = StoryTextBGRect;
		float width = storyTextBGRect.width;
		Color color = GUI.color;
		storyTextBGRect.width = width * (1f + Mathf.Max(0.25f * (1f - color.a), 0f));
		storyTextBGRect.x = (900f - storyTextBGRect.width) / 2f;
		GUI.DrawTexture(storyTextBGRect, StoryTextBG);
		GUI.color = new Color(1f, 1f, 1f, StoryTimer - 2.5f);
		storyTextBGRect = StoryTitleRect;
		float width2 = storyTextBGRect.width;
		Color color2 = GUI.color;
		storyTextBGRect.width = width2 * (1f + Mathf.Max(0.25f * (1f - color2.a), 0f));
		storyTextBGRect.x = (900f - storyTextBGRect.width) / 2f;
		GUI.DrawTexture(storyTextBGRect, StoryTitle);
		GUI.color = new Color(1f, 1f, 1f, StoryTimer - 3.5f);
		storyTextBGRect = StoryTextRect;
		float width3 = storyTextBGRect.width;
		Color color3 = GUI.color;
		storyTextBGRect.width = width3 * (1f + Mathf.Max(0.25f * (1f - color3.a), 0f));
		storyTextBGRect.x = (900f - storyTextBGRect.width) / 2f;
		GUI.DrawTexture(storyTextBGRect, StoryText);
		GUI.color = new Color(1f, 1f, 1f, StoryTimer - 4.5f);
		storyTextBGRect = StoryExoSymbolRect;
		float width4 = storyTextBGRect.width;
		Color color4 = GUI.color;
		storyTextBGRect.width = width4 * (1f + Mathf.Max(0.25f * (1f - color4.a), 0f));
		storyTextBGRect.x = (900f - storyTextBGRect.width) / 2f;
		GUI.DrawTexture(storyTextBGRect, StoryExoSymbol);
		if (StoryTimer > 6.5f)
		{
			GUI.color = new Color(1f, 1f, 1f, Mathf.Min(1f, StoryTimer - 6.5f));
			if (bIsGuest)
			{
				//switch (GUIUtil.Button(new Rect(290f, 475f, 135f, 60f), "PLAY AS GUEST", mSharedSkin.GetStyle("DarkButton")))
				switch (GUIUtil.Button(new Rect(290f, 475f, 135f, 60f), "PLAY AS GUEST"))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "GUEST";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
				{
					b = "GUEST";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					if (GameData.MATCH_MODE == GameData.Build.DEBUG && Input.GetKey(KeyCode.F1))
					{
						GameData.MyFactionId = 1;
					}
					else if (GameData.MATCH_MODE == GameData.Build.DEBUG && Input.GetKey(KeyCode.F2))
					{
						GameData.MyFactionId = 2;
					}
					else
					{
						GameData.MyFactionId = UnityEngine.Random.Range(1, 3);
					}
					state = loadGuest;
					bLoading = true;
					GameObject gameObject = GameObject.Find("Tracker");
					TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
					trackerScript.AddMetric(TrackerScript.Metric.GUEST_OR_LOGIN);
					break;
				}
				}
				//switch (GUIUtil.Button(new Rect(475f, 475f, 135f, 60f), "LOGIN", mSharedSkin.GetStyle("DarkButton")))
				switch (GUIUtil.Button(new Rect(475f, 475f, 135f, 60f), "LOGIN"))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "LOGIN";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
				{
					b = "LOGIN";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					Application.ExternalCall("LoginModule.showLoginWindow({visible: true}, 'login')");
					mStatus = "Waiting for Cartoon Network Login or Registration . . .";
					state = waitForLogin;
					authenticatePlayer();
					state = waitForPlayerAuthentication;
					mPlayStatus = 0;
					GameObject gameObject2 = GameObject.Find("Tracker");
					TrackerScript trackerScript2 = gameObject2.GetComponent("TrackerScript") as TrackerScript;
					trackerScript2.AddMetric(TrackerScript.Metric.GUEST_OR_LOGIN);
					break;
				}
				}
				if (GameData.MATCH_MODE == GameData.Build.DEBUG)
				{
					switch (GUIUtil.Button(new Rect(290f, 535f, 135f, 60f), "DEV NEW PLAYER", mSharedSkin.GetStyle("DarkButton")))
					{
					case GUIUtil.GUIState.Hover:
					case GUIUtil.GUIState.Active:
						if (Event.current.type == EventType.Repaint)
						{
							b = "DEV NEW PLAYER";
							if (lastHover != b)
							{
								GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
							}
						}
						break;
					case GUIUtil.GUIState.Click:
						b = "DEV NEW PLAYER";
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
						GameData.MyPlayStatus = 2;
						break;
					}
				}
				switch (GUIUtil.Button(new Rect(475f, 475f, 135f, 60f), "LOGIN", mSharedSkin.GetStyle("DarkButton")))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "LOGIN";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
				{
					b = "LOGIN";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					Application.ExternalCall("LoginModule.showLoginWindow({visible: true}, 'login')");
					mStatus = "Waiting for Cartoon Network Login or Registration . . .";
					state = waitForLogin;
					mPlayStatus = 0;
					GameObject gameObject3 = GameObject.Find("Tracker");
					TrackerScript trackerScript3 = gameObject3.GetComponent("TrackerScript") as TrackerScript;
					trackerScript3.AddMetric(TrackerScript.Metric.GUEST_OR_LOGIN);
					break;
				}
				}
			}
			else
			{
				switch (GUIUtil.Button(new Rect(383f, 475f, 135f, 60f), "BOOT CAMP", mSharedSkin.GetStyle("DarkButton")))
				{
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "BOOT CAMP";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				case GUIUtil.GUIState.Click:
					b = "BOOT CAMP";
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
					bLoading = true;
					if (isTestLoggedLocal)
					{
						loadTestNewPlayer();
					}
					else
					{
						loadNewPlayer();
					}
					mStatus = "Loading New Player... " + GameData.MyDisplayName;
					setPause(120, playAsNewPlayer);
					break;
				}
			}
		}
		lastHover = b;
	}
}
