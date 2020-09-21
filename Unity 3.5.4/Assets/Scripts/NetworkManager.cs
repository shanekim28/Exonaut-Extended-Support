using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Logging;
using Sfs2X.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	private delegate void STATE();

	private const float PING_FREQUENCY = 1f;

	private bool running;

	private static string CN_INTERNAL = "localhost";

	private string zone = "Exonaut";

	public string serverName = CN_INTERNAL;

	private int serverPort = 9933;

	[NonSerialized]
	public string m_username = string.Empty;

	[NonSerialized]
	public string password = string.Empty;

	private string loginErrorMessage = string.Empty;

	[SerializeField]
	private STATE m_state;

	private Factions m_faction;

	public string m_gameModeStr;

	public Room currentActiveRoom;

	private bool m_isConnected;

	private bool m_isLoggedIn;

	private bool m_isInRoom;

	private bool m_pinger;

	public int m_qTime;

	public User user;

	private ServerMode mServerMode;

	public UrlLocator urlLocator;

	public GameObject urlLocatorPrefab;

	public bool m_serviceSet;

	public SmartFox smartFox;

	private void Awake()
	{
	}

	private void Start()
	{
		m_isConnected = false;
		m_isLoggedIn = false;
		m_isInRoom = false;
		m_serviceSet = false;
		m_qTime = 20;
		smartFox = null;
		if (true)
		{
			mServerMode = ServerMode.director;
		}
		else
		{
			serverName = CN_INTERNAL;
			mServerMode = ServerMode.single;
		}
		if (mServerMode == ServerMode.single)
		{
			InitSmartfox();
		}
		else
		{
			AskDirector();
		}
	}

	public void AskDirector()
	{
		GameObject gameObject = Instantiate(Resources.Load("Locator")) as GameObject;
		urlLocator = (gameObject.GetComponent<UrlLocator>() as UrlLocator);
		gameObject.SendMessage("LoadProps");
		Logger.trace("[NetworkManager::AskDirector]");
		m_state = WaitUntilHaveServer;
	}

	public void WaitUntilHaveServer()
	{
		if (urlLocator.IsReady())
		{
			serverName = urlLocator.GetIP();
			serverPort = urlLocator.GetPort();
			if (Application.isWebPlayer)
			{
				GameData.BUNDLE_PATH = Application.dataPath + "/";
			}
			if (Application.dataPath.Contains("unstable") || Application.dataPath.Contains("cn-internal"))
			{
				GameData.SERVICE_PATH = "http://" + serverName + ":8081/exonaut";
				GameData.SERVICE_PATH_WITH_PORT = GameData.SERVICE_PATH;
			}
			else
			{
				GameData.SERVICE_PATH = "http://" + serverName + "/exonaut";
			}
			m_serviceSet = true;
			Logger.traceAlways("[NetworkManager::WaitUntilHaveServer] server: " + serverName + " port: " + serverPort);
			InitSmartfox();
		}
	}

	public void InitSmartfox()
	{
		if (Application.isWebPlayer || Application.isEditor)
		{
			bool flag = Security.PrefetchSocketPolicy(serverName, serverPort);
			Logger.trace("[NetworkManager::InitSmartfox] securityPolicyPassed: " + flag);
		}
		if (SmartFoxConnection.IsInitialized)
		{
			Debug.Log("<< is initialized");
			smartFox = SmartFoxConnection.Connection;
		}
		else
		{
			bool debug = true;
			Debug.Log("<< initializing smartfox");
			smartFox = new SmartFox(debug);
		}
		GameObject x = GameObject.Find("Game");
		if (x != null)
		{
		}
		smartFox.AddLogListener(LogLevel.INFO, OnDebugMessage);
		m_state = null;
	}

	public void Connect()
	{
		if (smartFox == null)
		{
			Debug.Log("Smartfox is null so i'm reiniting");
			InitSmartfox();
		}
		else if (!smartFox.IsConnected)
		{
			Debug.Log("<< adding event listeners -- server: " + serverName + " port: " + serverPort);
			AddEventListeners();
			smartFox.Connect(serverName, serverPort);
			Debug.Log("<< connecting to " + serverName + " port: " + serverPort);
			running = true;
		}
		else
		{
			running = true;
			Debug.Log("Smartfox is already connected");
		}
	}

	public void FindServer()
	{
	}

	public void WaitForServer()
	{
	}

	public void Login()
	{
		Debug.Log("Connection Established...Sending login request " + GameData.MyTEGid + " with authId " + password);
		m_username = GameData.MyTEGid;
		password = GameData.MyAuthid;
		if (m_username != string.Empty)
		{
			Debug.Log("<< trying to connect as : " + m_username + " with password: " + password);
			SFSObject sFSObject = new SFSObject();
			sFSObject.PutUtfString(m_username, password);
			smartFox.Send(new LoginRequest(m_username, password, zone, sFSObject));
		}
		else
		{
			m_username = string.Empty;
			smartFox.Send(new LoginRequest(m_username, string.Empty, zone));
		}
	}

	public bool isConnected()
	{
		return m_isConnected;
	}

	public bool isLoggedIn()
	{
		return m_isLoggedIn;
	}

	public bool isInRoom()
	{
		return m_isInRoom;
	}

	private void FixedUpdate()
	{
		if (running)
		{
			smartFox.ProcessEvents();
		}
	}

	private void OnApplicationQuit() {
		smartFox.Disconnect();
    }

	/// <summary>
	/// Adds event listeners to handle CONNECTION, CONNECTION_LOST, LOGIN, LOGIN_ERROR, ROOM_JOIN, USER_ENTER_ROOM, USER_EXIT_ROOM, LOGOUT, EXTENSION_RESPONSE, USER_VARIABLES_UPDATE, ROOM_REMOVE, and ROOM_VARIABLES_UPDATE events
	/// </summary>
	private void AddEventListeners()
	{
		if (smartFox == null)
		{
			Logger.traceError("[NetworkManager::AddEventListeners] - unable to add listeners -- smartFox is null ");
			return;
		}
		smartFox.RemoveAllEventListeners();
		smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.LOGIN, OnLogin);
		smartFox.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
		smartFox.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
		smartFox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
		smartFox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
		smartFox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
		smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		smartFox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, UpdateUserVariables);
		smartFox.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomDeleted);
		smartFox.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnUpdateRoomVariables);
	}

	private void RemoveEventListeners()
	{
		smartFox.RemoveAllEventListeners();
	}

	private void OnRoomDeleted(BaseEvent evt)
	{
		Debug.Log("<< Room was closed");
	}

	private void Update()
	{
		if (m_state != null)
		{
			m_state();
		}
	}

	private void UpdateUserVariables(BaseEvent evt)
	{
		ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
		User user = (User)evt.Params["user"];
		GameObject gameObject = (!user.IsItMe) ? GameObject.Find("remote_" + user.PlayerId) : GameObject.Find("localPlayer");
		SFSObject sFSObject = new SFSObject();
		bool flag = false;
		IEnumerator enumerator = arrayList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				switch ((string)enumerator.Current)
				{
				case "clientState":
					checkClientState(user);
					break;
				case "avatarState":
					Debug.Log("<< AVATAR STATE HAS CHANGED: " + user.Name + " to state:  " + user.GetVariable("avatarState").GetStringValue() + " playerID: " + user.PlayerId);
					checkAvatarState(user);
					break;
				case "moveState":
					if (!user.IsItMe)
					{
						sFSObject.PutInt("moveState", user.GetVariable("moveState").GetIntValue());
						flag = true;
					}
					break;
				case "moveDir":
					if (!user.IsItMe)
					{
						sFSObject.PutInt("moveDir", user.GetVariable("moveDir").GetIntValue());
						flag = true;
					}
					break;
				case "x":
					if (!user.IsItMe)
					{
						sFSObject.PutFloat("x", (float)user.GetVariable("x").GetDoubleValue());
						flag = true;
					}
					break;
				case "y":
					if (!user.IsItMe)
					{
						sFSObject.PutFloat("y", (float)user.GetVariable("y").GetDoubleValue());
						flag = true;
					}
					break;
				case "armAngle":
					if (!user.IsItMe)
					{
						sFSObject.PutFloat("armAngle", (float)user.GetVariable("armAngle").GetDoubleValue());
						flag = true;
					}
					break;
				case "faceTargetDir":
					if (!user.IsItMe)
					{
						sFSObject.PutFloat("faceTargetDir", (float)user.GetVariable("faceTargetDir").GetDoubleValue());
						flag = true;
					}
					break;
				case "weaponId":
					if (gameObject != null)
					{
						gameObject.SendMessage("changeWeapon", user.GetVariable("weaponId").GetIntValue());
					}
					break;
				case "health":
					if (gameObject != null)
					{
						gameObject.SendMessage("UpdateHealth", (int)user.GetVariable("health").GetDoubleValue());
					}
					break;
				case "hacks":
					if (gameObject != null)
					{
						gameObject.SendMessage("UpdateHacks", user);
					}
					break;
				case "boost":
					if (user.GetVariable("boost").GetIntValue() < 0)
					{
						Logger.trace("Player boost expired");
						Player player3 = gameObject.GetComponent("Player") as Player;
						player3.deactivePickup(isTeam: false);
					}
					else
					{
						Logger.trace("Player picked up a boost");
					}
					break;
				case "teamBoost":
					if (user.GetVariable("teamBoost").GetIntValue() < 0)
					{
						Debug.Log("Player team boost expired");
						Player player2 = gameObject.GetComponent("Player") as Player;
						player2.deactivePickup(isTeam: true);
					}
					else
					{
						Debug.Log("Player picked up a team boost");
					}
					break;
				case "inactive":
					if (user.IsItMe && user.GetVariable("inactive").GetBoolValue())
					{
						Player player = gameObject.GetComponent("Player") as Player;
						player.gs.QuitGame(1);
					}
					break;
				case "nickName":
					if (user.IsItMe)
					{
						GameData.MyDisplayName = user.GetVariable("nickName").GetStringValue();
						Debug.Log("Setting nickname to value: " + user.GetVariable("nickName").GetStringValue());
					}
					gameObject.SendMessage("SetDisplayName", user.GetVariable("nickName").GetStringValue());
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		if (gameObject != null && flag)
		{
			gameObject.SendMessage("ReceiveTransform", sFSObject);
		}
	}

	private void checkClientState(User u)
	{
	}

	private void checkAvatarState(User u)
	{
		string stringValue = u.GetVariable("avatarState").GetStringValue();
		GameObject gameObject = GameObject.Find("Game");
		GamePlay gamePlay = gameObject.GetComponent("GamePlay") as GamePlay;
		GameObject x = GameObject.Find("remote_" + u.PlayerId);
		if (x == null)
		{
			Debug.Log("Unknown User:" + u.PlayerId + " is in state:" + stringValue);
			if (!u.IsItMe && stringValue == "captured")
			{
				if (u.GetVariable("clientState") != null && !u.GetVariable("clientState").GetStringValue().Equals("playing"))
				{
					Logger.traceAlways("--Player is attempting to play while not playing--their clientState is " + u.GetVariable("clientState").GetStringValue());
				}
				gamePlay.spawnRemotePlayer(u);
			}
		}
		bool flag = false;
		if (currentActiveRoom.GetVariable("stop") != null && (currentActiveRoom.GetVariable("stop").GetStringValue().Equals("capturelimit") || currentActiveRoom.GetVariable("stop").GetStringValue().Equals("timeout")))
		{
			flag = true;
		}
		if (!flag && u.IsItMe && stringValue.Equals("halted"))
		{
			gamePlay.QuitGame(7);
		}
		setAvatarState(u, stringValue);
	}

	public void OnUpdateRoomVariables(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];
		ArrayList arrayList = (ArrayList)evt.Params["changedVars"];
		IEnumerator enumerator = arrayList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				switch ((string)enumerator.Current)
				{
				case "stop":
					if (room.GetVariable("stop").GetStringValue().Equals("capturelimit"))
					{
						Debug.Log("<< Game ended because of " + room.GetVariable("stop").GetStringValue());
						DynamicOptions.bDrawCursor = true;
						GameData.CurPlayState = GameData.PlayState.GAME_IS_OVER;
						GameData.setGameEndCondition(2);
					}
					else if (room.GetVariable("stop").GetStringValue().Equals("timeout"))
					{
						DynamicOptions.bDrawCursor = true;
						GameData.CurPlayState = GameData.PlayState.GAME_IS_OVER;
						GameData.setGameEndCondition(3);
					}
					else if (room.GetVariable("stop").GetStringValue().Equals("banzaileft"))
					{
						GameObject gameObject2 = GameObject.Find("localPlayer");
						if (gameObject2 != null)
						{
							Player player = gameObject2.GetComponent("Player") as Player;
							player.gs.QuitGame(4);
						}
					}
					else if (room.GetVariable("stop").GetStringValue().Equals("atlasleft"))
					{
						GameObject gameObject3 = GameObject.Find("localPlayer");
						if (gameObject3 != null)
						{
							Player player2 = gameObject3.GetComponent("Player") as Player;
							player2.gs.QuitGame(5);
						}
					}
					else if (room.GetVariable("stop").GetStringValue().Equals("playersleft"))
					{
						GameObject gameObject4 = GameObject.Find("localPlayer");
						if (gameObject4 != null)
						{
							Player player3 = gameObject4.GetComponent("Player") as Player;
							player3.gs.QuitGame(3);
						}
					}
					else if (room.GetVariable("stop").GetStringValue().Equals("teamimbalance"))
					{
						DynamicOptions.bDrawCursor = true;
						GameData.CurPlayState = GameData.PlayState.GAME_IS_OVER;
						GameData.setGameEndCondition(6);
					}
					Logger.trace("<< game is halted for reason: " + room.GetVariable("stop").GetStringValue());
					break;
				case "time":
				{
					int intValue = room.GetVariable("time").GetIntValue();
					GameObject gameObject = GameObject.Find("Game");
					if (gameObject != null)
					{
						GamePlay gamePlay = gameObject.GetComponent("GamePlay") as GamePlay;
						if (gamePlay != null)
						{
							gamePlay.mGameTimer = gamePlay.mTotalGameTime - (float)intValue;
						}
					}
					break;
				}
				case "mapId":
					Logger.traceAlways("[NetworkManager::OnUpdateRoomVariables]  - mapId received");
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void OnExtensionResponse(BaseEvent evt)
	{
		try
		{
			string text = (string)evt.Params["cmd"];
			ISFSObject iSFSObject = (SFSObject)evt.Params["params"];
			switch (text)
			{
			case "pingAck":
				break;
			case "spawnPlayerAck":
				break;
			case "findRoom":
				Logger.trace("[OnExtensionResponse] found room");
				break;
			case "findRoomAck":
				Logger.trace("[OnExtensionResponse]  room found " + evt.Params.ToString());
				break;
			case "startGame":
				Logger.trace("[OnExtensionResponse]  Receiving Message startGame");
				break;
			case "stopGame":
				Logger.trace("[OnExtensionResponse] Receiving stop game");
				break;
			case "sendEvents":
			{
				SFSArray sFSArray = (SFSArray)iSFSObject.GetSFSArray("events");
				for (int j = 0; j < sFSArray.Size(); j++)
				{
					SFSObject sFSObject2 = (SFSObject)sFSArray.GetSFSObject(j);
					int int3 = sFSObject2.GetInt("playerId");
					PassEventToPlayerObject(sFSObject2, int3);
				}
				break;
			}
			case "queueTime":
				m_qTime = iSFSObject.GetInt("queueTime");
				break;
			case "sendSummary":
			{
				Debug.Log("\n");
				Debug.Log("\n");
				Debug.Log("\n");
				Debug.Log("### receiving summary");
				Debug.Log("\n");
				Debug.Log("\n");
				Logger.trace("Battle Summary Received");
				Logger.traceError("Summary:" + iSFSObject.GetDump());
				int @int = iSFSObject.GetInt("BanzaiTotal");
				int int2 = iSFSObject.GetInt("AtlasTotal");
				Debug.Log("banzai total: " + @int);
				Debug.Log("atlas total: " + int2);
				GameData.BanzaiHacks = @int;
				GameData.AtlasHacks = int2;
				string[] keys = iSFSObject.GetKeys();
				string[] array = keys;
				foreach (string text2 in array)
				{
					if (!text2.Equals("BanzaiTotal") && !text2.Equals("AtlasTotal"))
					{
						SFSObject sFSObject = (SFSObject)iSFSObject.GetSFSObject(text2);
						Logger.traceError("statData: " + sFSObject.GetDump());
						int playerId = Convert.ToInt32(text2);
						GameData.addPlayerStats(playerId, sFSObject);
					}
				}
				GameObject gameObject = GameObject.Find("Game");
				GamePlay gamePlay = gameObject.GetComponent("GamePlay") as GamePlay;
				if (gamePlay != null)
				{
					gamePlay.BattleSummaryReceived();
				}
				else
				{
					Debug.Log("cant find gameplay");
				}
				break;
			}
			default:
				Logger.trace("[OnExtensionResponse] unhandled event: " + text);
				break;
			}
		}
		catch (Exception ex)
		{
			Logger.traceError("Exception handling response: " + ex.Message + " >>> " + ex.StackTrace);
		}
	}

	public int getMapId()
	{
		if (currentActiveRoom == null)
		{
			return -1;
		}
		if (currentActiveRoom.GetVariable("mapId") == null)
		{
			return -1;
		}
		return currentActiveRoom.GetVariable("mapId").GetIntValue();
	}

	public void sendClientState(string state)
	{
		List<UserVariable> list = new List<UserVariable>();
		list.Add(new SFSUserVariable("clientState", state));
		if (smartFox != null)
		{
			smartFox.Send(new SetUserVariablesRequest(list));
		}
	}

	public void setUserVariable(string key, string val)
	{
		List<UserVariable> list = new List<UserVariable>();
		list.Add(new SFSUserVariable(key, val));
		smartFox.Send(new SetUserVariablesRequest(list));
	}

	public void setUserVariable(string key, int val)
	{
		List<UserVariable> list = new List<UserVariable>();
		list.Add(new SFSUserVariable(key, val));
		smartFox.Send(new SetUserVariablesRequest(list));
	}

	public void setUserVariable(string key, float val)
	{
		List<UserVariable> list = new List<UserVariable>();
		list.Add(new SFSUserVariable(key, val));
		smartFox.Send(new SetUserVariablesRequest(list));
	}

	public void sendStateUpdate(Player p)
	{
		int num = (p.myState.currentMoveState & 0xFFFFF) | (p.myState.currentArmState & 0xF000000) | (p.myState.currentActionState & 0xF00000) | (p.myState.currentContextState & 0x70000000);
		List<UserVariable> list = new List<UserVariable>();
		list.Add(new SFSUserVariable("moveState", num));
		list.Add(new SFSUserVariable("moveDir", p.moveDir));
		Vector3 position = p.transform.position;
		list.Add(new SFSUserVariable("x", (double)position.x));
		Vector3 position2 = p.transform.position;
		list.Add(new SFSUserVariable("y", (double)position2.y));
		list.Add(new SFSUserVariable("armAngle", (double)p.armAngle));
		list.Add(new SFSUserVariable("faceTargetDir", (double)p.faceTargetDir));
		smartFox.Send(new SetUserVariablesRequest(list));
	}

	public void OnConnection(BaseEvent evt)
	{
		bool flag = (bool)evt.Params["success"];
		string text = (string)evt.Params["errorMessage"];
		Debug.Log("On Connection callback got: " + flag + " (error : <" + text + ">)");
		if (flag)
		{
			SmartFoxConnection.Connection = smartFox;
			m_isConnected = true;
		}
	}

	public void OnConnectionLost(BaseEvent evt)
	{
		string str = (string)evt.Params["reason"];
		Logger.traceError("OnConnectionLost: " + str);
		m_isInRoom = false;
		m_isConnected = false;
		m_isLoggedIn = false;
		currentActiveRoom = null;
		RemoveEventListeners();
		GameObject gameObject = GameObject.Find("Game");
		GamePlay gamePlay = gameObject.GetComponent("GamePlay") as GamePlay;
		gamePlay.QuitGame(12);
	}

	public void OnLogin(BaseEvent evt)
	{
		try
		{
			if (evt.Params.ContainsKey("success") && !(bool)evt.Params["success"])
			{
				loginErrorMessage = (string)evt.Params["errorMessage"];
				Debug.Log("Login error: " + loginErrorMessage);
				m_username = string.Empty;
			}
			else
			{
				user = (User)evt.Params["user"];
				m_username = user.Name;
				m_isLoggedIn = true;
				List<UserVariable> list = new List<UserVariable>();
				if (user.GetVariable("nickName") != null)
				{
					Logger.trace(" my nickname is: " + user.GetVariable("nickName").GetStringValue());
					GameData.MyDisplayName = user.GetVariable("nickName").GetStringValue();
					user.GetVariable("nickName").GetStringValue();
				}
				if (GameData.MyFactionId == 1)
				{
					m_faction = Factions.banzai;
				}
				else if (GameData.MyFactionId == 2)
				{
					m_faction = Factions.atlas;
				}
				list.Add(new SFSUserVariable("faction", m_faction.ToString()));
				list.Add(new SFSUserVariable("suitId", GameData.MySuitID));
				list.Add(new SFSUserVariable("weaponId", 1));
				list.Add(new SFSUserVariable("xp", GameData.MyTotalXP));
				smartFox.Send(new SetUserVariablesRequest(list));
				doJoinRoom();
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception handling login request: " + ex.Message + " " + ex.StackTrace);
		}
	}

	public void Logout()
	{
		if (smartFox != null)
		{
			Debug.Log("<< Requesting logout");
			smartFox.Send(new LogoutRequest());
		}
	}

	public void OnLogout(BaseEvent evt)
	{
		Debug.Log("<< you are logged out");
		m_isInRoom = false;
		m_isLoggedIn = false;
		currentActiveRoom = null;
	}

	public void OnLoginError(BaseEvent evt)
	{
		Debug.Log("Login error: " + (string)evt.Params["errorMessage"]);
	}

	public void OnJoinRoom(BaseEvent evt)
	{
		Room room2 = GameData.GameRoom = (currentActiveRoom = (Room)evt.Params["room"]);
		Debug.Log("<< JOINING ROOM " + room2.Name);
		if (room2.IsGame)
		{
			m_isInRoom = true;
			GameData.WorldID = room2.GetVariable("mapId").GetIntValue();
			GameData.GameName = room2.Name;
			GameData.CaptureLimit = room2.GetVariable("hackLimit").GetIntValue();
			m_pinger = true;
			GameData.MyPlayerId = user.PlayerId;
			Logger.trace("<< player id: " + GameData.MyPlayerId + " and I want to play map: " + GameData.WorldID);
			foreach (User user2 in room2.UserList)
			{
				SFSObject sFSObject = new SFSObject();
				if (user2.IsItMe)
				{
					sFSObject.PutInt("playStatus", GameData.MyPlayStatus);
					sFSObject.PutInt("playerId", GameData.MyPlayerId);
					sFSObject.PutUtfString("playerName", GameData.MyDisplayName);
					sFSObject.PutInt("suitIdx", GameData.MySuitID);
					sFSObject.PutInt("playerFaction", GameData.MyFactionId);
					sFSObject.PutInt("textureIdx", GameData.MyTextureID);
					sFSObject.PutInt("weaponIdx", GameData.MyWeaponID);
					sFSObject.PutInt("powers", GameData.MyPowers);
					sFSObject.PutInt("level", GameData.MyLevel);
					sFSObject.PutInt("rank", GameData.MyRank);
					sFSObject.PutBool("leveledUp", val: false);
					GameData.addPlayer(user.PlayerId, sFSObject);
				}
			}
		}
	}

	public void OnUserEnterRoom(BaseEvent evt)
	{
		User user = (User)evt.Params["user"];
		Debug.Log("remote User info: " + user.ToString());
		Debug.Log("NetworkManager::OnUserEnterRoom .. playerId: " + user.PlayerId);
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("playerId", user.PlayerId);
		GameObject gameObject = GameObject.Find("battleQueue");
		if (gameObject != null)
		{
			Debug.Log("<< battleQueue found");
			QueueBattle queueBattle = gameObject.GetComponent("QueueBattle") as QueueBattle;
			if (queueBattle != null)
			{
				int factionId = (!(Factions.atlas.ToString() == user.GetVariable("faction").GetStringValue())) ? 1 : 2;
				queueBattle.addPlayerToSlot(factionId, user.PlayerId);
			}
			else
			{
				Debug.Log("cannot find QueueBattle");
			}
		}
		else
		{
			Debug.Log("Battle Queue game object is null");
			GameObject gameObject2 = GameObject.Find("Game");
			GamePlay gamePlay = gameObject2.GetComponent("GamePlay") as GamePlay;
			int factionId2 = (!(Factions.atlas.ToString() == user.GetVariable("faction").GetStringValue())) ? 1 : 2;
			gamePlay.addJoiner(factionId2, user.PlayerId);
		}
	}

	public void LeaveRoom()
	{
		if (currentActiveRoom != null)
		{
			smartFox.Send(new LeaveRoomRequest(currentActiveRoom));
		}
	}

	private void OnUserExitRoom(BaseEvent evt)
	{
		User user = (User)evt.Params["user"];
		Debug.Log("user " + user.Name + " has left the room");
		GameObject gameObject = GameObject.Find("Game");
		if (gameObject != null)
		{
			GamePlay gamePlay = gameObject.GetComponent("GamePlay") as GamePlay;
			if (gamePlay != null)
			{
				Debug.Log("[NetworkManager::OnUserExitRoom] - removePlayerFromGame");
				gamePlay.removePlayerFromGame(user.GetVariable("nickName").GetStringValue(), user.PlayerId);
			}
		}
		if (user.IsItMe)
		{
			Logout();
		}
	}

	public void OnDebugMessage(BaseEvent evt)
	{
		string str = (string)evt.Params["message"];
		Debug.Log("[SFS DEBUG] " + str);
	}

	/// <summary>
	/// Sends exension request to the server
	/// </summary>
	/// <param name="id">ID of request</param>
	/// <param name="dataObject">Data to send</param>
	public void sendClientRequest(RequestId id, SFSObject dataObject)
	{
		string extCmd = id.ToString();
		smartFox.Send(new ExtensionRequest(extCmd, dataObject, smartFox.LastJoinedRoom));
	}

	private void doJoinRoom()
	{
		SFSObject sFSObject = new SFSObject();
		Debug.Log("Do join Room " + m_gameModeStr);
		sFSObject.PutUtfString("mode", m_gameModeStr);
		sendClientRequest(RequestId.findRoom, sFSObject);
		m_state = null;
	}


	public void sendPing(bool submit)
	{
		if (submit)
		{
			sendClientRequest(RequestId.ping, new SFSObject());
		}
	}

	public void PassEventToPlayerObject(SFSObject data, int fromUser)
	{
		GameObject gameObject = GameObject.Find(fromUser.ToString());
		if (gameObject == null)
		{
			gameObject = GameObject.Find("remote_" + fromUser);
			if (gameObject == null)
			{
				gameObject = GameObject.Find("localPlayer");
			}
		}
		if (gameObject != null)
		{
			gameObject.SendMessage("ReceiveEvent", data);
		}
		else
		{
			Logger.trace("<< can't find user.. should send to local");
		}
	}

	public void setAvatarState(User avatarUser, string state)
	{
		GameObject gameObject = (!avatarUser.IsItMe) ? GameObject.Find("remote_" + avatarUser.PlayerId) : GameObject.Find("localPlayer");
		if (gameObject == null)
		{
			Debug.Log("<<< PLAYEROBJECT IS NULL : " + avatarUser.Name + " not setting to state: " + state);
			return;
		}
		Debug.Log("STATE: " + state + " Player: " + avatarUser.Name);
		switch (state)
		{
		case "invincible":
			gameObject.SendMessage("Released");
			break;
		case "captured":
		{
			int num = (avatarUser.GetVariable("capturedMethod").GetIntValue() << 16) | avatarUser.GetVariable("capturedBy").GetIntValue();
			gameObject.SendMessage("Captured", num);
			break;
		}
		case "normal":
			gameObject.SendMessage("Normal");
			break;
		case "halted":
			gameObject.SendMessage("Halted");
			break;
		default:
			Logger.traceError("[NetworkManager::setAvatarState]  ");
			break;
		}
	}

	public int getFactionByID(int playerID)
	{
		int result = -1;
		if (playerID < 1)
		{
			return result;
		}
		if (currentActiveRoom != null)
		{
			foreach (User user2 in currentActiveRoom.UserList)
			{
				if (user2.PlayerId == playerID)
				{
					return (!(Factions.atlas.ToString() == user2.GetVariable("faction").GetStringValue())) ? 1 : 2;
				}
			}
			return result;
		}
		return result;
	}

	public string getNameByID(int playerID)
	{
		if (currentActiveRoom != null)
		{
			if (currentActiveRoom.UserList == null)
			{
				return string.Empty;
			}
			foreach (User user2 in currentActiveRoom.UserList)
			{
				if (user2 != null && user2.PlayerId == playerID)
				{
					if (user2.GetVariable("nickName") == null)
					{
						return string.Empty;
					}
					return user2.GetVariable("nickName").GetStringValue();
				}
			}
		}
		return string.Empty;
	}

	public User getUserByID(int playerID)
	{
		if (currentActiveRoom != null)
		{
			if (currentActiveRoom.UserList == null)
			{
				return null;
			}
			foreach (User user2 in currentActiveRoom.UserList)
			{
				if (user2 != null && user2.PlayerId == playerID)
				{
					return user2;
				}
			}
		}
		return null;
	}

	public User getUserByIndex(int idx)
	{
		if (currentActiveRoom != null)
		{
			if (currentActiveRoom.UserList == null)
			{
				return null;
			}
			foreach (User user2 in currentActiveRoom.UserList)
			{
				if (user2 != null && user2.PlayerId == idx)
				{
					return user2;
				}
			}
		}
		return null;
	}
}
