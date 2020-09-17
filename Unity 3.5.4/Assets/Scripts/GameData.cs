using LitJson;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameData {
	public enum Build {
		DEBUG = 1,
		PRODUCTION
	}

	public enum PlayState {
		GAME_LOADING,
		GAME_IN_QUEUE,
		GAME_STARTED,
		GAME_IS_PLAYING,
		GAME_IS_OVER,
		GAME_IS_WAITING_SUMMARY,
		GAME_SUMMARY_RECEIVED,
		GAME_IS_QUITTING
	}

	public const string SERVER_VERSION = "1.1";

	public const int ASSET_VERSION = 2;

	public const int NUM_PLAYERS_MAX = 8;

	public const int NUM_WORLDS = 10;

	public const int NUM_BATTLE_TYPES = 2;

	public const int NUM_GAME_TYPES = 2;

	public const int MAX_LEVEL = 50;

	public const string XTMSG_LOBBY_JOIN_SUCC = "ljs";

	public const string XTMSG_LOBBY_JOIN_FAIL = "ljf";

	public const string XTMSG_GAME_FIND = "gameFind";

	public const string XTMSG_GAME_CREATE = "gameCreate";

	public const string XTMSG_GAME_JOIN = "gameJoin";

	public const string XTMSG_GAME_JOIN_SUCC = "gjs";

	public const string XTMSG_GAME_JOIN_FAIL = "gjf";

	public const string XTMSG_GAME_RESERVE_SUCC = "grs";

	public const string XTMSG_GAME_RESERVE_FAIL = "grf";

	public const string XTMSG_GAME_READY = "gr";

	public const string XTMSG_GAME_START = "gs";

	public const string XTMSG_GAME_END = "ge";

	public const string XTMSG_GAME_END_FORCE = "gef";

	public const string XTMSG_BATTLE_SUMMARY = "sum";

	public const string XTMSG_BATTLE_TYPE = "battleType";

	public const string XTMSG_WORLD_ID = "worldId";

	public const string XTMSG_OPPONENT_JOIN = "oj";

	public const string XTMSG_OPPONENT_LEAVE = "ol";

	public const string XTMSG_OPPONENT_START = "os";

	public const string XTMSG_OPPONENT_LOADED_SUIT = "ols";

	public const string XTMSG_OPPONENT_DROP_START = "ods";

	public const string XTMSG_OPPONENT_CHAT = "oc";

	public const string XTMSG_QUEUE_UPDATE = "qu";

	public const string XTMSG_STATE_UPDATE = "su";

	public const string XTMSG_EVENT = "evt";

	public const string XTMSG_REACTIVATE_BOOST = "reb";

	public const string EVENT_MSG = "msgType";

	public const int EVT_SEND_SUIT_WEAPON = 1;

	public const int EVT_SEND_ACTION_POSITION = 3;

	public const int EVT_SEND_SHOT_POSITION = 4;

	public const int EVT_SEND_GRENADE_POSITION = 5;

	public const int EVT_SEND_CHANGE_WEAPON = 6;

	public const int EVT_SEND_POWER = 7;

	public const int EVT_SEND_PICKUP = 8;

	public const int EVT_SEND_GRENADE_EXPLODE = 9;

	public const int EVT_SEND_DAMAGE = 10;

	public const int EVT_SEND_OVERHEAT_JETPACK = 11;

	public const int EVT_SEND_HEAL = 13;

	public const int EVT_SEND_KILL_BULLET = 14;

	public const int EVT_SEND_ROCKET_EXPLODE = 16;

	public const int EVT_SEND_TAUNT = 17;

	public const int EVT_SEND_SNIPER_SHOT = 18;

	public const int EVT_SEND_POWER_COMPLETE = 19;

	public const int EVT_SEND_CAPTURED = 20;

	public const int EVT_SEND_RELEASED = 21;

	public const int EVT_SEND_PICKUP_COMPLETE = 22;

	public const int EVT_SEND_OPP_INFO = 23;

	public const int EVT_TIME_UPDATE = 24;

	public const int EVT_ACTIVE_BOOST_INFO = 25;

	public const int EVT_SEND_ROLL = 26;

	public const int EVT_SEND_AIRDASH = 27;

	public const int EVT_SEND_FUEL_CONSUMED = 28;

	public const int EVT_SEND_DROP_DAMAGE_CLOUD = 30;

	public const int EVT_SPAWN_PLAYER = 40;

	public const int EVT_SEND_BEEP = 50;

	public const int EVT_SEND_MAKE_VISIBLE = 51;

	public const int EVT_SEND_READY_TO_START = 100;

	public const int EVT_SEND_START_GAME = 101;

	public const int EVT_OPP_HAS_DROP_IN = 102;

	public const int EVT_OPP_HAS_SUIT_LOADED = 103;

	public const int EVT_CHAT = 110;

	public const int EVT_PLAYER_LOAD_PROGRESS = 111;

	public const int EVT_DEV_START = 120;

	public const int EVT_ERROR_MESSAGE = 200;

	public const string GAME_NAME = "gameName";

	public const string GAME_IN_PROGRESS = "gameInProgress";

	public const string GAME_TIME = "gametime";

	public const int FORCE_QUIT = 1;

	public const int CAPTURE_LIMIT_REACHED = 2;

	public const int TIME_LIMIT_REACHED = 3;

	public const int ATLAS_PLAYERS_LEFT = 4;

	public const int BANZAI_PLAYERS_LEFT = 5;

	public const int NO_MORE_PLAYERS = 6;

	public const int LOST_CONNECTION = 7;

	public const int FACTION_BANZAI = 1;

	public const int FACTION_ATLAS = 2;

	public const int BATTLE = 1;

	public const int TEAM_BATTLE = 2;

	public const int BUDDY_BATTLE = 3;

	public const int NEW_LOGIN = 0;

	public const int GUEST = 1;

	public const int NEW_PLAYER = 2;

	public const int EXO_PLAYER = 3;

	public const string PLAYER_KEY = "key";

	public const string PLAYER_PLAY_STATUS = "playStatus";

	public const string PLAYER_ID = "playerId";

	public const string PLAYER_EXONAUT_ID = "exoId";

	public const string PLAYER_NAME = "playerName";

	public const string PLAYER_SUIT = "suitIdx";

	public const string PLAYER_WEAPON = "weaponIdx";

	public const string PLAYER_TEXTURE = "textureIdx";

	public const string PLAYER_FACTION = "playerFaction";

	public const string PLAYER_POWERS = "powers";

	public const string PLAYER_STATS = "stats";

	public const string PLAYER_RANK = "rank";

	public const string PLAYER_LEVEL = "level";

	public const string PLAYER_LEVELED_UP = "leveledUp";

	public const string PLAYER_HEALTH = "currHealth";

	public const string PLAYER_BOOST = "boost";

	public const string PLAYER_CAPTURES = "captures";

	public const int TIME_LIMIT = 1;

	public static string version;

	//public static Build MATCH_MODE = Build.PRODUCTION;
	public static Build MATCH_MODE = Build.DEBUG;

	public static string MATCH_SERVER_IP = "127.0.0.1";

	// TODO: replace with server service path
	//public static string SERVICE_PATH = "http://10.189.49.72:8081/exonaut";
	public static string SERVICE_PATH = "file://" + Application.dataPath + "/Resources/temp/";

	public static string SERVER_PATH = "http://cn-internal/games/exonaut";

	// TODO: replace with server bundle path
	//public static string BUNDLE_PATH = "http://cn-internal/games/exonaut/";
	public static string BUNDLE_PATH = "file://" + Application.dataPath + "/Resources/gameresources/";

	public static string SERVICE_PATH_WITH_PORT = string.Empty;

	public static List<string> SERVICE_CLUSTER = new List<string>();

	public static string NEWS_FILE = "/News.txt";

	public static int mGameEndCondition;

	private static bool mAllowBuddyBattle = false;

	private static int hackWhileInvisible = 0;

	private static int hackWhileSpeedBoost = 0;

	private static int hackWhileDamageBoost = 0;

	private static int hackWhileArmorBoost = 0;

	private static string gameName = string.Empty;

	private static Room gameRoom = null;

	private static PlayState mCurPlayState = PlayState.GAME_LOADING;

	private static bool mIsReservation = false;

	private static SFSArray gameAddress = new SFSArray();

	public static List<int> playerSlots1 = new List<int>();

	public static List<int> playerSlots2 = new List<int>();

	public static int ConsecutiveGames = 0;

	private static int battleType = 1;

	private static int gameType;

	private static int numBattlesCompleted = 0;

	private static SFSArray numCapturesByFaction = new SFSArray();

	private static int banzaiHacks;

	private static int atlasHacks;

	private static TextAsset spawn_points = null;

	private static TextAsset pickup_placement = null;

	private static List<string> pickupSpawns = new List<string>();

	private static List<Vector3> playerSpawns = new List<Vector3>();

	private static List<int> mGuestSuitList = new List<int>
	{
		1,
		2,
		3,
		21,
		22,
		23
	};

	private static int[] mBanzaiDefaultSuits = new int[3]
	{
		1,
		2,
		3
	};

	private static int[] mAtlasDefaultSuits = new int[3]
	{
		21,
		22,
		23
	};

	private static JsonData suitJson = null;

	private static Hashtable masterSuitList = new Hashtable();

	private static JsonData weaponJson = null;

	private static Hashtable mWeapons = new Hashtable();

	private static JsonData weaponModJson = null;

	private static Hashtable mWeaponMods = new Hashtable();

	public static Hashtable WorldLightMaps = new Hashtable();

	private static List<AnimationClip> animations = new List<AnimationClip>();

	private static List<string> animationNames = new List<string>();

	private static string[] pickupList = new string[12]
	{
		"pickupShield",
		"pickupDamage",
		"pickupInvisible",
		"pickupSpeed",
		"pickupSniper",
		"pickupLobber",
		"pickupRocket",
		"pickupGrenade",
		"pickupRandom",
		"pickupTeamDamage",
		"pickupTeamSpeed",
		"pickupTeamArmor"
	};

	private static List<GameObject> pickups = new List<GameObject>();

	private static List<GameObject> weapons = new List<GameObject>();

	private static bool isChooserActive = true;

	private static int worldId = 1;

	private static string worldVersion = "normal";

	private static Hashtable mWorldArray = new Hashtable();

	public static AssetBundle assetBundle = null;

	private static float fWorldLoadProgress = 0f;

	private static TextAsset[] pickupSpawnPoints = new TextAsset[40];

	private static TextAsset[] playerSpawnPoints = new TextAsset[40];

	private static TextAsset event_manifest = null;

	public static Hashtable eventObjects = new Hashtable();

	private static int myTutorialStep = 0;

	private static User mySFSUserObject = null;

	private static string myTegId = string.Empty;

	private static string myAuthId = string.Empty;

	private static string myExonautToken = string.Empty;

	private static int myExonautId = -1;

	private static string myLogin = string.Empty;

	private static string myDisplayName = "Guest Player";

	private static int myPlayerId = -1;

	private static int myPlayStatus = -1;

	private static int myFactionId = 0;

	private static int mySuitID = 12;

	private static int myTextureID = 0;

	private static int myWeaponID = 2;

	private static int myPowers = -1;

	private static List<int> myOwnedSuitIDs = new List<int>();

	private static int myLevel = 1;

	private static int myRank = 0;

	private static int myBattleXP = 0;

	private static int myTotalXP = 0;

	private static int myBattleCredits = 0;

	private static int myTotalCredits = 0;

	private static int myBattleCaptures = 0;

	private static int myBattleSaves = 0;

	private static int myBattleFalls = 0;

	private static List<SFSObject> mLatestCompletedMissions = new List<SFSObject>();

	private static List<SFSObject> mLatestMissionsInProgress = new List<SFSObject>();

	private static SFSArray mBattleWinners = new SFSArray();

	private static List<int> sfsPlayersToRemove = new List<int>(8);

	public static List<int> mOpponentsWhoLoadedMySuit = new List<int>();

	private static Hashtable sfsPlayers = new Hashtable(8);

	private static Hashtable battleSummaryStats = new Hashtable(8);

	private static int gameTeam;

	private static int captureLimit;

	private static float timeLimit;

	private static int[] totalCaptures = new int[2];

	private static int[] suitIdx = new int[8];

	private static int[] weaponIdx = new int[8];

	private static int[] textureIdx = new int[8];

	private static int[] powers = new int[8];

	public static DynamicOptions.Settings mGameSettings = new DynamicOptions.Settings();

	public static bool AllowBuddyBattle {
		get {
			return mAllowBuddyBattle;
		}
	}

	public static int HackWhileInvisible
	{
		get
		{
			return hackWhileInvisible;
		}
		set
		{
			hackWhileInvisible = value;
		}
	}

	public static int HackWhileSpeedBoost
	{
		get
		{
			return hackWhileSpeedBoost;
		}
		set
		{
			hackWhileSpeedBoost = value;
		}
	}

	public static int HackWhileDamageBoost
	{
		get
		{
			return hackWhileDamageBoost;
		}
		set
		{
			hackWhileDamageBoost = value;
		}
	}

	public static int HackWhileArmorBoost
	{
		get
		{
			return hackWhileArmorBoost;
		}
		set
		{
			hackWhileArmorBoost = value;
		}
	}

	public static string GameName
	{
		get
		{
			return gameName;
		}
		set
		{
			gameName = value;
		}
	}

	public static Room GameRoom
	{
		get
		{
			return gameRoom;
		}
		set
		{
			gameRoom = value;
		}
	}

	public static PlayState CurPlayState
	{
		get
		{
			return mCurPlayState;
		}
		set
		{
			mCurPlayState = value;
		}
	}

	public static bool IsReservation
	{
		get
		{
			return mIsReservation;
		}
		set
		{
			mIsReservation = value;
		}
	}

	public static int BattleType
	{
		get
		{
			return battleType;
		}
		set
		{
			battleType = value;
		}
	}

	public static int GameType
	{
		get
		{
			return gameType;
		}
		set
		{
			gameType = value;
		}
	}

	public static int NumBattlesCompleted
	{
		get
		{
			return numBattlesCompleted;
		}
		set
		{
			numBattlesCompleted = value;
		}
	}

	public static int BanzaiHacks
	{
		get
		{
			return banzaiHacks;
		}
		set
		{
			banzaiHacks = value;
		}
	}

	public static int AtlasHacks
	{
		get
		{
			return atlasHacks;
		}
		set
		{
			atlasHacks = value;
		}
	}

	public static TextAsset SpawnPoints
	{
		get
		{
			return spawn_points;
		}
		set
		{
			spawn_points = value;
		}
	}

	public static TextAsset PickupPoints
	{
		get
		{
			return pickup_placement;
		}
		set
		{
			pickup_placement = value;
		}
	}

	public static List<string> PickupSpawns
	{
		get
		{
			return pickupSpawns;
		}
		set
		{
			pickupSpawns = value;
		}
	}

	public static List<Vector3> PlayerSpawns
	{
		get
		{
			return playerSpawns;
		}
		set
		{
			playerSpawns = value;
		}
	}

	public static int[] GuestSuitList {
		get {
			return mGuestSuitList.ToArray();
		}
	}

	public static int[] BanzaiDefaultSuits {
		get { return mBanzaiDefaultSuits; }
	}

	public static int[] AtlasDefaultSuits {
		get { return mAtlasDefaultSuits; }
	}

	public static JsonData SuitJson
	{
		get
		{
			return suitJson;
		}
		set
		{
			suitJson = value;
		}
	}

	public static Hashtable MasterSuitList {
		get { return masterSuitList; }
	}

	public static JsonData WeaponJson
	{
		get
		{
			return weaponJson;
		}
		set
		{
			weaponJson = value;
		}
	}

	public static Hashtable WeaponList {
		get { return mWeapons; }
	}

	public static JsonData WeaponModJson
	{
		get
		{
			return weaponModJson;
		}
		set
		{
			weaponModJson = value;
		}
	}

	public static Hashtable WeaponMods {
		get { return mWeaponMods; }
	}

	public static List<AnimationClip> Animations {
		get {
			return animations;
		}
	}

	public static List<string> AnimationNames {
		get {
			return animationNames;
		}
	}

	public static string[] PickupList {
		get {
			return pickupList;
		}
	}

	public static List<GameObject> Pickups {
		get {
			return pickups;
		}
	}

	public static List<GameObject> Weapons {
		get {
			return weapons;
		}
	}

	public static bool IsChooserActive
	{
		get
		{
			return isChooserActive;
		}
		set
		{
			isChooserActive = value;
		}
	}

	public static int WorldID
	{
		get
		{
			return worldId;
		}
		set
		{
			worldId = value;
		}
	}

	public static string WorldName
	{
		get
		{
			switch (WorldID)
			{
			case 0:
				return "Boot Camp";
			case 1:
				return "Kylmyys";
			case 2:
				return "Abysus Castle";
			case 3:
				return "Stormalong Harbor";
			case 4:
				return "Finn's Treehouse";
			case 5:
				return "Perplexahedron";
			case 6:
				return "Bling Bling Island";
			case 7:
				return "Battlescape Alpha";
			case 8:
				return "Elmore Carnival";
			case 9:
				return "Battlescape Gamma";
			default:
				return "No Name For This World";
			}
		}
	}

	public static string WorldVersion
	{
		get
		{
			return worldVersion;
		}
		set
		{
			worldVersion = value;
		}
	}

	public static Hashtable WorldArray {
		get {
			return mWorldArray;
		}
	}

	public static float WorldLoadProgress {
		get {
			return fWorldLoadProgress;
		}
	}

	public static TextAsset[] PickupSpawnPoints {
		get {
			return pickupSpawnPoints;
		}
	}

	public static TextAsset[] PlayerSpawnPoints {
		get {
			return playerSpawnPoints;
		}
	}

	public static TextAsset EventManifest
	{
		get
		{
			return event_manifest;
		}
		set
		{
			event_manifest = value;
		}
	}

	public static int MyTutorialStep
	{
		get
		{
			return myTutorialStep;
		}
		set
		{
			myTutorialStep = value;
		}
	}

	public static User MySFSUserObject
	{
		get
		{
			return mySFSUserObject;
		}
		set
		{
			mySFSUserObject = value;
		}
	}

	public static string MyTEGid
	{
		get
		{
			return myTegId;
		}
		set
		{
			myTegId = value;
		}
	}

	public static string MyAuthid
	{
		get
		{
			return myAuthId;
		}
		set
		{
			myAuthId = value;
		}
	}

	public static string MyExonautToken
	{
		get
		{
			return myExonautToken;
		}
		set
		{
			myExonautToken = value;
		}
	}

	public static int MyExonautId
	{
		get
		{
			return myExonautId;
		}
		set
		{
			myExonautId = value;
		}
	}

	public static string MyLogin
	{
		get
		{
			return myLogin;
		}
		set
		{
			myLogin = value;
		}
	}

	public static string MyDisplayName
	{
		get
		{
			return myDisplayName;
		}
		set
		{
			myDisplayName = value;
		}
	}

	public static int MyPlayerId
	{
		get
		{
			return myPlayerId;
		}
		set
		{
			myPlayerId = value;
		}
	}

	public static int MyPlayStatus
	{
		get
		{
			return myPlayStatus;
		}
		set
		{
			myPlayStatus = value;
		}
	}

	/// <summary>
	/// Banzai is 1, Atlas is 2
	/// </summary>
	public static int MyFactionId
	{
		get
		{
			return myFactionId;
		}
		set
		{
			myFactionId = value;
			setPlayerFactionID(MyPlayerId, myFactionId);
		}
	}

	public static int MySuitID
	{
		get
		{
			return mySuitID;
		}
		set
		{
			mySuitID = value;
			setPlayerSuitID(MyPlayerId, mySuitID, myTextureID);
		}
	}

	public static int MyTextureID
	{
		get
		{
			return myTextureID;
		}
		set
		{
			myTextureID = value;
			setPlayerSuitID(MyPlayerId, mySuitID, myTextureID);
		}
	}

	public static int MyWeaponID
	{
		get
		{
			return myWeaponID;
		}
		set
		{
			myWeaponID = value;
			setPlayerWeaponID(MyPlayerId, myWeaponID);
		}
	}

	public static int MyPowers
	{
		get
		{
			return myPowers;
		}
		set
		{
			myPowers = value;
			setPlayerPowers(MyPlayerId, myPowers);
		}
	}

	public static List<int> MyOwnedSuitIDs {
		get {
			return myOwnedSuitIDs;
		}
	}

	public static int MyLevel
	{
		get
		{
			return myLevel;
		}
		set
		{
			myLevel = value;
		}
	}

	public static int MyRank
	{
		get
		{
			myRank = myLevel / 10;
			return myRank;
		}
		set
		{
			myRank = value;
		}
	}

	public static int MyBattleXP
	{
		get
		{
			return myBattleXP;
		}
		set
		{
			myBattleXP = value;
		}
	}

	public static int MyTotalXP
	{
		get
		{
			return myTotalXP;
		}
		set
		{
			myTotalXP = value;
		}
	}

	public static int MyBattleCredits
	{
		get
		{
			return myBattleCredits;
		}
		set
		{
			myBattleCredits = value;
		}
	}

	public static int MyTotalCredits
	{
		get
		{
			return myTotalCredits;
		}
		set
		{
			myTotalCredits = value;
		}
	}

	public static int MyBattleCaptures
	{
		get
		{
			return myBattleCaptures;
		}
		set
		{
			myBattleCaptures = value;
		}
	}

	public static int MyBattleSaves
	{
		get
		{
			return myBattleSaves;
		}
		set
		{
			myBattleSaves = value;
		}
	}

	public static int MyBattleFalls
	{
		get
		{
			return myBattleFalls;
		}
		set
		{
			myBattleFalls = value;
		}
	}

	public static List<SFSObject> LatestCompletedMissions {
		get {
			return mLatestCompletedMissions;
		}
	}

	public static List<SFSObject> LatestMissionsInProgress {
		get {
			return mLatestMissionsInProgress;
		}
	}

	public static SFSArray BattleWinners
	{
		get
		{
			return mBattleWinners;
		}
		set
		{
			mBattleWinners = value;
		}
	}

	public static List<int> SFSPlayersToRemove {
		get {
			return sfsPlayersToRemove;
		}
	}

	public static Hashtable SFSPlayers
	{
		get
		{
			return sfsPlayers;
		}
		set
		{
			sfsPlayers = value;
		}
	}

	public static Hashtable BattleSummaryStats
	{
		get
		{
			return battleSummaryStats;
		}
		set
		{
			battleSummaryStats = value;
		}
	}

	public static int GameTeam
	{
		get
		{
			return gameTeam;
		}
		set
		{
			gameTeam = value;
		}
	}

	public static int CaptureLimit
	{
		get
		{
			return captureLimit;
		}
		set
		{
			captureLimit = value;
		}
	}

	public static float TimeLimit
	{
		get
		{
			return timeLimit;
		}
		set
		{
			timeLimit = value;
		}
	}

	public static int[] TotalCaptures
	{
		get
		{
			return totalCaptures;
		}
		set
		{
			totalCaptures = value;
		}
	}

	public static int[] SuitIndex {
		get {
			return suitIdx;
		}
	}

	public static int[] WeaponIndex {
		get {
			return weaponIdx;
		}
	}

	public static int[] TextureIndex {
		get {
			return textureIdx;
		}
	}

	public static int[] Powers {
		get {
			return powers;
		}
	}

	public static string SetServicePath()
	{
		if (SERVICE_CLUSTER.Count > 1)
		{
			int index = UnityEngine.Random.Range(0, SERVICE_CLUSTER.Count - 1);
			SERVICE_PATH = SERVICE_CLUSTER[index];
		}
		else
		{
			SERVICE_PATH = SERVICE_CLUSTER[0];
		}
		return SERVICE_PATH;
	}

	public static string trimDisplayName(string startingString)
	{
		string[] array = startingString.Split(' ');
		string text = array[0];
		if (array.Length > 1)
		{
			text = text + " " + array[1];
		}
		return text;
	}

	public static void setGameEndCondition(int endCondition)
	{
		mGameEndCondition = endCondition;
		GameObject gameObject = GameObject.Find("Game");
		GamePlay gamePlay = gameObject.GetComponent("GamePlay") as GamePlay;
		gamePlay.BattleHasEnded();
	}

	public static string getGameEndConditionString()
	{
		switch (mGameEndCondition)
		{
		case 1:
			return "A Player has Force Quit The Game!";
		case 2:
			return "The Capture Limit Was Reached!";
		case 3:
			return "Game Time Has Expired!";
		case 6:
			return "Sorry. \nThere Are No More Players in the Game!";
		case 7:
			return string.Empty;
		default:
			return "No Game End Condition Exists";
		}
	}

	public static void DestroyCurrentGame()
	{
		gameName = string.Empty;
		gameRoom = null;
		WorldID = 1;
		gameAddress = null;
	}

	public static void setGameAddress(string gameManagerID, string reserveId)
	{
		gameAddress = new SFSArray();
		gameAddress.AddUtfString(gameManagerID);
		gameAddress.AddUtfString(reserveId);
	}

	public static SFSArray getGameAddress()
	{
		return gameAddress;
	}

	public static string getBattleTypeDisplayName()
	{
		switch (BattleType)
		{
		case 1:
			return "Battle";
		case 2:
			return "Team Battle";
		case 3:
			return "Buddy Battle";
		default:
			return "No Name For Battle Type Selected";
		}
	}

	public static bool isTeamBattle()
	{
		if (BattleType == 2)
		{
			return true;
		}
		return false;
	}

	public static int getCapturesByFaction(int factionId)
	{
		return numCapturesByFaction.GetInt(factionId);
	}

	public static void resetAllSpawns()
	{
		pickupSpawns = new List<string>();
		playerSpawns = new List<Vector3>();
	}

	public static string getFactionDisplayName(int factionId)
	{
		string result = "Guest";
		switch (factionId)
		{
		case 1:
			result = "Banzai Squadron";
			break;
		case 2:
			result = "Atlas Brigade";
			break;
		}
		return result;
	}

	public static void AddGuestSuit(int id)
	{
		if (!mGuestSuitList.Contains(id))
		{
			mGuestSuitList.Add(id);
		}
	}

	public static void InitSuitList(string URL)
	{
		Logger.trace("GameData::InitSuitList " + suitJson);
		if (suitJson == null)
		{
			Logger.traceError("Bad JSON for suits");
			return;
		}
		Logger.trace("GameData::InitSuitList " + suitJson.Count);
		for (int i = 0; i < suitJson.Count; i++)
		{
			JsonData jsonData = suitJson[i];
			if (jsonData == null)
			{
				Logger.traceError("jSuit is null: " + i);
			}
			else
			{
				if (Convert.ToInt32((string)jsonData["Active"]) == 0 && MATCH_MODE == Build.PRODUCTION)
				{
					continue;
				}
				int num = Convert.ToInt32((string)jsonData["ID"]);
				if (masterSuitList == null)
				{
					Logger.traceError("Mastersuitlist is null");
					continue;
				}
				string text = (string)jsonData["Guest"];
				if (text != null && text != "0")
				{
					if (!mGuestSuitList.Contains(num))
					{
						AddOwnedSuit(num);
					}
					AddGuestSuit(num);
				}
				masterSuitList.Add(num, new Exosuit(jsonData));
			}
		}
	}

	public static Exosuit getExosuit(int suitId)
	{
		return masterSuitList[suitId] as Exosuit;
	}

	public static void setLowPolySuitIsLoaded(int suitId, GameObject model, Material texture)
	{
		if (getExosuit(suitId) != null)
			getExosuit(suitId).setLowPolyModel(model, texture);
	}

	public static void setHighPolySuitIsLoaded(int suitId, GameObject model, Material mask, Material armor)
	{
		if (getExosuit(suitId) != null)
			getExosuit(suitId).setHighPolyModel(model, mask, armor);
	}

	public static bool getSuitIsLoaded(int suitId)
	{
		bool result = false;
		Exosuit exosuit = getExosuit(suitId);
		if (exosuit != null)
		{
			result = exosuit.getIsLowPolyModelLoaded();
		}
		return result;
	}

	public static void InitWeapons()
	{
		if (weaponJson != null)
		{
			for (int i = 0; i < weaponJson.Count; i++)
			{
				JsonData jsonData = weaponJson[i];
				int num = Convert.ToInt32((string)jsonData["ID"]);
				mWeapons.Add(num, new WeaponDef(jsonData));
			}
		}
	}

	public static WeaponDef getWeaponById(int weaponId)
	{
		return mWeapons[weaponId] as WeaponDef;
	}

	public static void InitWeaponMods()
	{
		if (weaponModJson != null)
		{
			for (int i = 0; i < weaponModJson.Count; i++)
			{
				JsonData jsonData = weaponModJson[i];
				int num = Convert.ToInt32((string)jsonData["ID"]);
				mWeaponMods.Add(num, new WeaponModData(jsonData));
			}
		}
	}

	public static WeaponModData getWeaponModData(int weaponModId)
	{
		return mWeaponMods[weaponModId] as WeaponModData;
	}

	public static void CreateLightMapArray(int worldId, int numMaps, AssetBundle assetBundle)
	{
		WorldLightMaps.Clear();
		worldId = 0;
		LightmapData[] array = new LightmapData[numMaps];
		for (int i = 0; i < numMaps; i++)
		{
			LightmapData lightmapData = new LightmapData();
			lightmapData.lightmapFar= (assetBundle.Load("LightmapFar-" + i.ToString(), typeof(Texture2D)) as Texture2D);
			Logger.trace("loading LightmapFar-" + i.ToString());
			array[i] = lightmapData;
		}
		WorldLightMaps.Add(worldId, array);
	}

	public static LightmapData[] GetLightmapsForWorld(int worldId)
	{
		worldId = 0;
		if (WorldLightMaps.ContainsKey(worldId))
		{
			return WorldLightMaps[worldId] as LightmapData[];
		}
		return null;
	}

	public static void addWorldById(int worldId, GameObject world)
	{
		worldId = 0;
		mWorldArray.Clear();
		mWorldArray.Add(worldId, world);
	}

	public static GameObject getWorldById(int worldId)
	{
		GameObject result = null;
		worldId = 0;
		if (mWorldArray.ContainsKey(worldId))
		{
			result = (mWorldArray[worldId] as GameObject);
		}
		return result;
	}

	public static void LoadWorld()
	{
		Debug.Log("<< load worldNum: " + WorldID + " should load world num: ");
		string text = BUNDLE_PATH + "worlds/world_" + WorldID + ".unity3d";
		Debug.Log("[GameData::LoadWorld]: " + text);
		fWorldLoadProgress = 0f;
		if (assetBundle != null)
		{
			assetBundle.Unload(unloadAllLoadedObjects: true);
		}
		WorldLightMaps.Clear();
		mWorldArray.Clear();
		string[] array = version.Split('.');
		int num = int.Parse(array[array.Length - 1]);
		Debug.Log("Version is: " + num);
		WWW www = WWW.LoadFromCacheOrDownload(text, num);
		GUIUtil.mInstance.StartCoroutine(WaitForWorldRequest(www));
	}

	private static IEnumerator WaitForWorldRequest(WWW www)
	{
		while (!www.isDone)
		{
			fWorldLoadProgress = www.progress;
			Debug.Log("[GameData::WaitForWorldRequest] " + fWorldLoadProgress);
			yield return new WaitForEndOfFrame();
		}
		yield return www;
		if (www.error == null)
		{
			yield return www;
			assetBundle = www.assetBundle;
			string str = "world_" + WorldID + "_pre";
			GameObject go = GameObject.Find("NetworkManager");
			NetworkManager networkManager = go.GetComponent("NetworkManager") as NetworkManager;
			networkManager.setUserVariable("lastMapLoadedId", WorldID);
			Logger.traceAlways("<< Load world prefab: " + str);
			AssetBundleRequest abr = assetBundle.LoadAsync(str, typeof(GameObject));
			yield return abr;
			bool addWorld = true;
			if (networkManager.currentActiveRoom != null && WorldID != networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue())
			{
				WorldID = networkManager.currentActiveRoom.GetVariable("mapId").GetIntValue();
				LoadWorld();
				Logger.traceAlways("<< I was going to play with the wrong room!!!!");
				addWorld = false;
			}
			if (addWorld)
			{
				addWorldById(0, abr.asset as GameObject);
				int numMaps;
				for (numMaps = 0; assetBundle.Contains("LightmapFar-" + numMaps); numMaps++)
				{
				}
				if (numMaps > 0)
				{
					CreateLightMapArray(0, numMaps, assetBundle);
				}
			}
		}
		else
		{
			Logger.traceError("[GameData::WaitForWorldRequest]: " + www.error);
		}
	}

	public static void LoadSpawnPoints()
	{
		resetAllSpawns();
		string text = (!BattleType.Equals(2)) ? "_b" : "_t";
		Logger.trace("<< loading spawnPickups for world : " + WorldID);
		string url = BUNDLE_PATH + "worlds/spawn_pickup/spawn_pickup_" + WorldID + "_" + WorldVersion + text + ".unity3d";
		WWW www = new WWW(url);
		GUIUtil.mInstance.StartCoroutine(WaitForSpawnPointsRequest(www));
	}

	private static IEnumerator WaitForSpawnPointsRequest(WWW www)
	{
		yield return www;
		if (www.error != null)
		{
			yield break;
		}
		yield return www;
		AssetBundle assetBundle = www.assetBundle;
		string battleOrTeam = (!BattleType.Equals(2)) ? "_b" : "_t";
		string assetToFind2 = "spawn_" + WorldID + "_" + WorldVersion + battleOrTeam;
		if (!(assetBundle != null))
		{
			yield break;
		}
		Logger.traceAlways("asset bundle is null");
		AssetBundleRequest abr2;
		if (assetBundle.Contains(assetToFind2))
		{
			abr2 = assetBundle.LoadAsync(assetToFind2, typeof(TextAsset));
			yield return abr2;
			TextAsset spawn_points = abr2.asset as TextAsset;
			Debug.Log(spawn_points.text);
			addPlayerSpawnPointsById(WorldID - 1, BattleType - 1, GameType - 1, spawn_points);
		}
		else
		{
			Logger.trace("<< missing spawnpoint data.. spawn everyone in same place so we know there is a problem");
			for (int i = 0; i < 8; i++)
			{
				PlayerSpawns.Add(new Vector3(0f, 0f, 0f));
			}
		}
		assetToFind2 = "pickup_" + WorldID + "_" + WorldVersion + battleOrTeam;
		abr2 = assetBundle.LoadAsync(assetToFind2, typeof(TextAsset));
		yield return abr2;
		addPickupSpawnPointsById(points: abr2.asset as TextAsset, worldId: WorldID - 1, battle_type: BattleType - 1, game_type: GameType - 1);
		parseAllSpawnPoints();
		assetBundle.Unload(unloadAllLoadedObjects: false);
	}

	public static void parseAllSpawnPoints()
	{
		resetAllSpawns();
		int num = WorldID + 10 * (BattleType - 1);
		Logger.trace("<< TRYING TO PARSE SPAWN POINTS OF ID " + num);
		TextAsset playerSpawnPointsById = getPlayerSpawnPointsById(WorldID - 1, BattleType - 1, GameType - 1);
		TextAsset textAsset = UnityEngine.Object.Instantiate(playerSpawnPointsById) as TextAsset;
		StringReader stringReader = new StringReader(textAsset.text);
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			string[] array = text.Split(","[0]);
			float x = Convert.ToSingle(array[0]);
			float y = Convert.ToSingle(array[1]);
			PlayerSpawns.Add(new Vector3(x, y, 0f));
		}
		playerSpawnPointsById = getPickupSpawnPointsById(WorldID - 1, BattleType - 1, GameType - 1);
		TextAsset textAsset2 = UnityEngine.Object.Instantiate(playerSpawnPointsById) as TextAsset;
		StringReader stringReader2 = new StringReader(textAsset2.text);
		for (string text2 = stringReader2.ReadLine(); text2 != null; text2 = stringReader2.ReadLine())
		{
			PickupSpawns.Add(text2);
		}
	}

	public static void addPickupSpawnPointsById(int worldId, int battle_type, int game_type, TextAsset points)
	{
		int index = worldId + 10 * battle_type + 20 * game_type;
		PickupSpawnPoints.SetValue(points, index);
	}

	public static TextAsset getPickupSpawnPointsById(int worldId, int battle_type, int game_type)
	{
		//Discarded unreachable code: IL_003c
		TextAsset textAsset = null;
		try
		{
			int index = worldId + 10 * battle_type + 20 * game_type;
			return (TextAsset)PickupSpawnPoints.GetValue(index);
		}
		catch (IndexOutOfRangeException arg)
		{
			Logger.trace("cannot return map:" + arg);
			return null;
		}
	}

	public static void addPlayerSpawnPointsById(int worldId, int battle_type, int game_type, TextAsset points)
	{
		int index = worldId + 10 * battle_type + 20 * game_type;
		PlayerSpawnPoints.SetValue(points, index);
	}

	public static TextAsset getPlayerSpawnPointsById(int worldId, int battle_type, int game_type)
	{
		//Discarded unreachable code: IL_003c
		TextAsset textAsset = null;
		try
		{
			int index = worldId + 10 * battle_type + 20 * game_type;
			return (TextAsset)PlayerSpawnPoints.GetValue(index);
		}
		catch (IndexOutOfRangeException arg)
		{
			Logger.trace("cannot return map:" + arg);
			return null;
		}
	}

	public static void ClearSpawnPoints()
	{
		for (int i = 0; i < playerSpawnPoints.Length; i++)
		{
			playerSpawnPoints[i] = null;
		}
	}

	public static bool DoesEventExist(string key)
	{
		if (eventObjects.ContainsKey(key))
		{
			return true;
		}
		return false;
	}

	public static string GetEventObjectName(string key)
	{
		return eventObjects[key] as string;
	}

	public static void AddOwnedSuit(int id)
	{
		if (!myOwnedSuitIDs.Contains(id))
		{
			myOwnedSuitIDs.Add(id);
		}
	}

	private static void updateCompletedMissions(SFSObject missionData)
	{
		string[] keys = missionData.GetKeys();
		for (int i = 0; i < keys.Length; i++)
		{
			SFSObject sFSObject = (SFSObject)missionData.GetSFSObject(keys[i]);
			if (sFSObject.GetUtfString("Image") != null)
			{
				sFSObject.PutUtfString("Image", sFSObject.GetUtfString("Image").Replace(".png", string.Empty));
			}
			mLatestCompletedMissions.Add(sFSObject);
		}
	}

	public static void addCurrentMission(Hashtable mission)
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutUtfString("Name", (string)mission["name"]);
		sFSObject.PutUtfString("Description", (string)mission["description"]);
		sFSObject.PutInt("Credits", Convert.ToInt32(mission["credits"]));
		sFSObject.PutInt("XP", Convert.ToInt32(mission["xp"]));
		if ((string)mission["image"] != null)
		{
			sFSObject.PutUtfString("Image", ((string)mission["image"]).Replace(".png", string.Empty));
		}
		mLatestCompletedMissions.Add(sFSObject);
	}

	private static void updateMissionsInProgress(SFSObject missionData)
	{
		string[] keys = missionData.GetKeys();
		foreach (string key in keys)
		{
			mLatestMissionsInProgress.Add((SFSObject)missionData.GetSFSObject(key));
		}
	}

	public static void addMissionInProgress(Hashtable mission)
	{
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("MissionID", Convert.ToInt32(mission["MissionID"]));
		sFSObject.PutInt("Rank", Convert.ToInt32(mission["Rank"]));
		sFSObject.PutUtfString("Name", (string)mission["Name"]);
		sFSObject.PutUtfString("Description", (string)mission["Description"]);
		sFSObject.PutInt("Credits", Convert.ToInt32(mission["Credits"]));
		sFSObject.PutInt("XP", Convert.ToInt32(mission["XP"]));
		sFSObject.PutInt("Count", Convert.ToInt32(mission["Count"]));
		sFSObject.PutInt("Total", Convert.ToInt32(mission["Total"]));
		sFSObject.PutFloat("Progress", Convert.ToSingle(mission["Progress"]));
		mLatestMissionsInProgress.Add(sFSObject);
	}

	public static void addPlayerLeft(int playerId)
	{
		if (!sfsPlayersToRemove.Contains(playerId))
		{
			sfsPlayersToRemove.Add(playerId);
		}
		else
		{
			Logger.traceError("<< sfsPlayerToRemove already contains " + playerId);
		}
	}

	public static void addOpponentLoadedMySuit(int oppId)
	{
		if (!mOpponentsWhoLoadedMySuit.Contains(oppId))
		{
			mOpponentsWhoLoadedMySuit.Add(oppId);
		}
	}

	public static void removeOpponentWhoLoadedMySuit(int oppId)
	{
		if (mOpponentsWhoLoadedMySuit.Contains(oppId))
		{
			mOpponentsWhoLoadedMySuit.Remove(oppId);
		}
	}

	public static void clearOpponentsWhoLoadedMySuit()
	{
		mOpponentsWhoLoadedMySuit.Clear();
	}

	public static void addPlayer(int playerId, SFSObject player)
	{
		if (sfsPlayers.ContainsKey(playerId))
		{
			sfsPlayers.Remove(playerId);
		}
		sfsPlayers.Add(playerId, player);
	}

	public static void clearAllSfsPlayers()
	{
		sfsPlayers.Clear();
		MySFSUserObject = null;
		MyPlayerId = -1;
		sfsPlayersToRemove.Clear();
	}

	public static void setPlayerPlayStatus(int playerId, int status)
	{
		if (getPlayerObject(playerId) != null)
			getPlayerObject(playerId).PutInt("playStatus", status);
	}

	public static int getPlayerPlayStatus(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetInt("playStatus");
		} else return -1;
	}

	public static void setPlayerSuitAndWeaponID(int playerId, int suitId, int textureId, int weaponId, int pwrs)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject != null)
		{
			Logger.trace("<< setting powers: " + pwrs);
			playerObject.PutInt("suitIdx", suitId);
			playerObject.PutInt("textureIdx", textureId);
			playerObject.PutInt("weaponIdx", weaponId);
			playerObject.PutInt("powers", pwrs);
			suitIdx[playerId - 1] = suitId;
			weaponIdx[playerId - 1] = weaponId;
			powers[playerId - 1] = pwrs;
		}
	}

	public static void setPlayerSuitID(int playerId, int suitId, int textureId)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject != null)
		{
			playerObject.PutInt("suitIdx", suitId);
			playerObject.PutInt("textureIdx", textureId);
			suitIdx[playerId - 1] = suitId;
			textureIdx[playerId - 1] = textureId;
		}
	}

	public static int getPlayerSuitID(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetInt("suitIdx");
		} else return -1;
	}

	public static bool getIsPlayerSuitLoaded(int playerId)
	{
		bool result = false;
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject != null)
		{
			Exosuit exosuit = getExosuit(playerObject.GetInt("suitIdx"));
			if (exosuit != null)
			{
				result = exosuit.getIsLowPolyModelLoaded();
			}
		}
		return result;
	}

	public static void setPlayerWeaponID(int playerId, int weaponId)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject != null)
		{
			playerObject.PutInt("weaponIdx", weaponId);
			weaponIdx[playerId - 1] = weaponId;
		}
	}

	public static int getPlayerWeaponID(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetInt("weaponIdx");
		} else return (-1);
	}

	public static void setPlayerPowers(int playerId, int pwrs)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject != null)
		{
			playerObject.PutInt("powers", pwrs);
			powers[playerId - 1] = pwrs;
		}
	}

	public static int getPlayerPowers(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetInt("powers");
		} else return (-1);
	}

	public static void setPlayerFactionID(int playerId, int factionId)
	{
		if (getPlayerObject(playerId) != null) {
			getPlayerObject(playerId).PutInt("playerFaction", factionId);
		}
	}

	public static void setPlayerLevel(int playerId, int level)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject != null)
		{
			if (playerObject.ContainsKey("level"))
			{
				playerObject.RemoveElement("level");
			}
			playerObject.PutInt("level", level);
		}
	}

	public static int getPlayerLevel(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetInt("level");
		
		} else return (-1);
	}

	public static int getPlayerFactionID(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetInt("playerFaction");

		} else return (-1);
	}

	public static string getPlayerFactionName(int playerId)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject == null)
		{
			return string.Empty;
		}
		int @int = playerObject.GetInt("playerFaction");
		string text = string.Empty;
		switch (@int)
		{
		case 1:
			text = "Banzai";
			break;
		case 2:
			text = "Atlas";
			break;
		}
		int int2 = playerObject.GetInt("playStatus");
		if (int2 == 1)
		{
			text += " Guest";
		}
		return text;
	}

	public static string getRankName(int rank)
	{
		switch (rank)
		{
		case 0:
			return "Novice";
		case 1:
			return "Pilot";
		case 2:
			return "Ace";
		case 3:
			return "Captain";
		case 4:
			return "Commander";
		case 5:
			return "Elite Exonaut";
		default:
			return "Rookie";
		}
	}

	public static int getPlayerRank(int playerId)
	{
		int playerStat = getPlayerStat(playerId, "level");
		return playerStat / 10;
	}

	public static void removePlayer(int playerId)
	{
		if (sfsPlayers.ContainsKey(playerId))
		{
			sfsPlayers.Remove(playerId);
		}
	}

	public static SFSObject getBattleStatObject(int playerId)
	{
		return battleSummaryStats[playerId] as SFSObject;
	}

	public static SFSObject getPlayerObject(int playerId)
	{
		return sfsPlayers[playerId] as SFSObject;
	}

	public static string getPlayerName(int playerId)
	{
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject == null)
		{
			return string.Empty;
		}
		return playerObject.GetUtfString("playerName");
	}

	public static bool getPlayerHasLeveledUp(int playerId)
	{
		if (getPlayerObject(playerId) != null) {
			return getPlayerObject(playerId).GetBool("leveledUp");
		} else return false;
	}

	public static int getPlayerTextureID(int playerId)
	{
		if (getPlayerObject(playerId) != null) {

			return getPlayerObject(playerId).GetInt("textureIdx");
		} else return (-1);
	}

	public static void addPlayerStats(int playerId, SFSObject statData)
	{
		Logger.trace("GameData::addPlayerStats::player id:" + playerId + " data:" + statData.GetDump());
		Logger.trace("Num Falls: " + statData.GetInt("nFalls"));
		if (battleSummaryStats.ContainsKey(playerId))
		{
			battleSummaryStats.Remove(playerId);
		}
		battleSummaryStats.Add(playerId, statData);
		if (!sfsPlayers.ContainsKey(playerId))
		{
			return;
		}
		SFSObject playerObject = getPlayerObject(playerId);
		if (playerObject.ContainsKey("stats"))
		{
			playerObject.RemoveElement("stats");
		}
		playerObject.PutSFSObject("stats", statData);
		playerObject.PutInt("level", statData.GetInt("level"));
		if (playerId == MyPlayerId)
		{
			playerObject.PutBool("leveledUp", val: false);
			int @int = statData.GetInt("level");
			if (@int > MyLevel)
			{
				playerObject.PutInt("level", @int);
				playerObject.PutBool("leveledUp", val: true);
			}
			MyLevel = @int;
			MyBattleXP = statData.GetInt("battleXP");
			MyTotalXP = statData.GetInt("totalXP");
			MyBattleCredits = statData.GetInt("battleCred");
			MyTotalCredits = statData.GetInt("totalCred");
			MyBattleCaptures = statData.GetInt("nCaps");
			MyBattleFalls = statData.GetInt("nFalls");
			MyBattleSaves = statData.GetInt("nSaves");
			SFSObject sFSObject = (SFSObject)statData.GetSFSObject("Missions");
			Logger.trace("Mission Data: " + sFSObject.GetDump());
			if (sFSObject != null)
			{
				updateCompletedMissions(sFSObject);
			}
			Logger.trace("Updated Player Object: " + playerObject.GetDump());
		}
	}

	public static int getExpNeededForLevel(int level)
	{
		if (level <= 40)
		{
			return (level - 1) * (level * 5 + 10);
		}
		if (level < 50)
		{
			return getExpNeededForLevel(40) + (level - 40) * ((level - 40) * 25 + 425);
		}
		if (level == 50)
		{
			return 20000;
		}
		return -1;
	}

	public static int getLevelFromExp(int Xp)
	{
		int num = 0;
		for (int i = 2; i <= 50; i++)
		{
			num = getExpNeededForLevel(i);
			if (num > Xp)
			{
				return i - 1;
			}
		}
		return -1;
	}

	public static User getUserFromRoom(NetworkManager networkManager, int id)
	{
		foreach (User user in networkManager.currentActiveRoom.UserList)
		{
			if (user.PlayerId == id)
			{
				Debug.Log("Player " + user.PlayerId + " exists");
				return user;
			}
		}
		return null;
	}

	public static int getPlayerStat(int playerId, string StatType)
	{
		int result = 0;
		if (sfsPlayers.ContainsKey(playerId))
		{
			SFSObject playerObject = getPlayerObject(playerId);
			if (playerObject == null)
			{
				return 0;
			}
			if (playerObject.ContainsKey("stats"))
			{
				SFSObject sFSObject = (SFSObject)playerObject.GetSFSObject("stats");
				if (sFSObject == null)
				{
					return 0;
				}
				if (sFSObject.ContainsKey(StatType))
				{
					result = sFSObject.GetInt(StatType);
				}
			}
		}
		return result;
	}

	public static void ApplySoundSettings()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("SFXObject");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<AudioSource>().volume = mGameSettings.mSoundVolume;
		}
	}

	public static void ApplyMusicSettings()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("MusicObject");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<AudioSource>().volume = mGameSettings.mMusicVolume;
		}
	}
}
