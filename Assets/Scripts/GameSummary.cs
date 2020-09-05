using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSummary
{
	[Serializable]
	public class PlayerSummary
	{
		public int ID = -1;

		public string Name = string.Empty;

		public int Faction;

		public int Rank;

		public int Hacks;

		public int Crashes;

		public int Saves;

		public int BattleXP;

		public int SuitID;

		public int GetCrashes()
		{
			return Crashes;
		}

		public int GetHacks()
		{
			return Hacks;
		}

		public void Init(User u)
		{
			if (GameData.BattleSummaryStats.ContainsKey(u.PlayerId))
			{
				SFSObject battleStatObject = GameData.getBattleStatObject(u.PlayerId);
				ID = u.PlayerId;
				Name = u.GetVariable("nickName").GetStringValue();
				int num = Faction = ((!(Factions.atlas.ToString() == u.GetVariable("faction").GetStringValue())) ? 1 : 2);
				Rank = GameData.getPlayerRank(ID);
				Hacks = battleStatObject.GetInt("nCaps");
				Crashes = battleStatObject.GetInt("nFalls");
				BattleXP = battleStatObject.GetInt("battleXP");
				SuitID = u.GetVariable("suitId").GetIntValue();
				Logger.trace("Player " + ID + " :Name=" + Name);
				Logger.trace("Player " + ID + " :Faction=" + Faction);
				Logger.trace("Player " + ID + " :Rank=" + Rank);
				Logger.trace("Player " + ID + " :Hacks=" + Hacks);
				Logger.trace("Player " + ID + " :Crashes=" + Crashes);
				Logger.trace("Player " + ID + " :Saves=" + Saves);
				Logger.trace("Player " + ID + " :BattleXP=" + BattleXP);
				Logger.trace("Player " + ID + " :SuitID=" + SuitID);
			}
		}
	}

	public delegate int Func();

	public bool mDraw;

	public bool mLeveled;

	public GUISkin SharedSkin;

	public GUISkin StatsSkin;

	private QueueBattle Parent;

	public Texture2D mLevelUpAnim;

	public Texture2D[] mFactionCards;

	public Rect MyScreenRect;

	public Rect PlayerGroup;

	public Rect MissionGroup;

	public Rect ScoreGroup;

	public int[] PlayerOrder = new int[8];

	public int[] BanzaiOrder = new int[4];

	public int[] AtlasOrder = new int[4];

	public PlayerSummary[] Players = new PlayerSummary[8];

	public float TimeStart;

	public float TransferDelay = 5f;

	public float TransferPercent;

	public float LevelUpTimer;

	public int TransferStateNum = -1;

	private float[] TransferTimes = new float[6]
	{
		1.5f,
		5.5f,
		1.5f,
		5.5f,
		1.5f,
		5.5f
	};

	private float[] TransferTimer;

	private bool bMax;

	public int MissionNum;

	public int BonusXP;

	public int BonusCredits;

	public int XpGained;

	public int CreditsGained;

	public string BonusXPText = "BONUS XP EARNED";

	public Color CreditColor = new Color(0.8f, 0.8f, 0f);

	public Color XPColor = new Color(0f, 0.52f, 0.62f);

	public Color BonusXPColor = new Color(0f, 0.52f, 0.62f);

	public Texture2D XPBonusTexture;

	private int TransferLevel = 1;

	private bool bRankup;

	private string lastHover = string.Empty;

	private int LastXfer;

	public Vector2 xy1 = new Vector2(95f, 5f);

	public Vector2 xy2 = new Vector2(45f, 15f);

	public Rect ScreenSpace;

	public void Start(QueueBattle parent)
	{
		MyScreenRect = new Rect(0f, 0f, 818f, 555f);
		PlayerGroup = new Rect(2f, 2f, 814f, 186f);
		MissionGroup = new Rect(2f, PlayerGroup.y + PlayerGroup.height, 814f, 95f);
		ScoreGroup = new Rect(2f, MissionGroup.y + MissionGroup.height + 1f, 814f, 230f);
		TransferStateNum = -1;
		TransferLevel = 1;
		TransferTimer = (float[])TransferTimes.Clone();
		TimeStart = Time.realtimeSinceStartup;
		Parent = parent;
		SharedSkin = GUIUtil.mInstance.mSharedSkin;
		StatsSkin = GUIUtil.mInstance.mStatsSkin;
		for (int i = 0; i < 8; i++)
		{
			PlayerOrder[i] = i;
			Players[i] = null;
		}
		for (int j = 0; j < BanzaiOrder.Length; j++)
		{
			BanzaiOrder[j] = -1;
			AtlasOrder[j] = -1;
		}
		mFactionCards = parent.mFactionCards;
		if (GameData.MyLevel == 50 && !GameData.getPlayerHasLeveledUp(GameData.MyPlayerId))
		{
			bMax = true;
			TransferLevel = GameData.MyLevel;
		}
		if (GameData.MyFactionId == 1)
		{
			XPBonusTexture = (GameData.eventObjects["Banzai_BonusXP_Summary_Texture"] as Texture2D);
		}
		else
		{
			XPBonusTexture = (GameData.eventObjects["Atlas_BonusXP_Summary_Texture"] as Texture2D);
		}
		string text = (string)GameData.eventObjects["BonusXP_Text"];
		if (text != null)
		{
			BonusXPText = text;
		}
		if (GameData.eventObjects["BonusXP_Summary_TextColor"] == null)
		{
			BonusXPColor = XPColor;
		}
		else
		{
			BonusXPColor = (Color)GameData.eventObjects["BonusXP_Summary_TextColor"];
		}
		Logger.trace("GameSummary::Start");
	}

	public void TakeSnapshot()
	{
		TimeStart = Time.realtimeSinceStartup;
		Logger.trace("::::: Take Snapshot :::::");
		BonusXP = GameData.getPlayerStat(GameData.MyPlayerId, "bonusXP");
		BonusCredits = GameData.getPlayerStat(GameData.MyPlayerId, "bonusCred");
		XpGained = GameData.MyBattleXP;
		CreditsGained = GameData.MyBattleCredits;
		for (int i = 0; i < 8; i++)
		{
			PlayerOrder[i] = i;
		}
		for (int j = 0; j < BanzaiOrder.Length; j++)
		{
			BanzaiOrder[j] = -1;
			AtlasOrder[j] = -1;
		}
		for (int k = 0; k < 8; k++)
		{
			User userFromRoom = GameData.getUserFromRoom(Parent.m_networkManager, k + 1);
			if (userFromRoom != null)
			{
				Logger.trace("Snapshot Player: " + (k + 1));
				Players[k] = new PlayerSummary();
				Players[k].Init(userFromRoom);
			}
			else
			{
				Players[k] = null;
			}
		}
		Logger.traceError("Starting sort");
		bool flag = true;
		int num = 0;
		while (flag)
		{
			Logger.traceError("Sort loop: " + num++);
			flag = false;
			for (int l = 0; l < PlayerOrder.Length - 1; l++)
			{
				int num2 = CheckSwap(PlayerOrder, l, l + 1, Players[PlayerOrder[l]].GetHacks, Players[PlayerOrder[l + 1]].GetHacks);
				if (1 <= num2)
				{
					flag = true;
				}
				else if (num2 == 0 && CheckSwap(PlayerOrder, l + 1, l, Players[PlayerOrder[l + 1]].GetCrashes, Players[PlayerOrder[l]].GetCrashes) == 1)
				{
					flag = true;
				}
			}
		}
		Logger.traceError("Ended sort loop in " + num + " cycles");
		if (GameData.BattleType == 2)
		{
			int num3 = 0;
			int num4 = 0;
			for (int m = 0; m < 8; m++)
			{
				if (Players[PlayerOrder[m]] != null)
				{
					switch (Players[PlayerOrder[m]].Faction)
					{
					case 1:
						BanzaiOrder[num3++] = PlayerOrder[m];
						break;
					case 2:
						AtlasOrder[num4++] = PlayerOrder[m];
						break;
					}
				}
			}
			bool flag2 = true;
			while (flag2)
			{
				flag2 = false;
				for (int n = 0; n < 3; n++)
				{
					if (BanzaiOrder[n] == -1 && BanzaiOrder[n + 1] != -1)
					{
						int num5 = BanzaiOrder[n];
						BanzaiOrder[n] = BanzaiOrder[n + 1];
						BanzaiOrder[n + 1] = num5;
						flag2 = true;
					}
					else
					{
						if (BanzaiOrder[n] == -1 || BanzaiOrder[n + 1] == -1)
						{
							continue;
						}
						switch (CheckSwap(BanzaiOrder, n, n + 1, Players[BanzaiOrder[n]].GetHacks, Players[BanzaiOrder[n + 1]].GetHacks))
						{
						case 1:
							flag = true;
							break;
						case 0:
							if (CheckSwap(BanzaiOrder, n + 1, n, Players[BanzaiOrder[n + 1]].GetCrashes, Players[BanzaiOrder[n]].GetCrashes) == 1)
							{
								flag = true;
							}
							break;
						}
					}
				}
				for (int num6 = 0; num6 < 3; num6++)
				{
					if (AtlasOrder[num6] == -1 && AtlasOrder[num6 + 1] != -1)
					{
						int num7 = AtlasOrder[num6];
						AtlasOrder[num6] = BanzaiOrder[num6 + 1];
						AtlasOrder[num6 + 1] = num7;
						flag2 = true;
					}
					else
					{
						if (AtlasOrder[num6] == -1 || AtlasOrder[num6 + 1] == -1)
						{
							continue;
						}
						switch (CheckSwap(AtlasOrder, num6, num6 + 1, Players[AtlasOrder[num6]].GetHacks, Players[AtlasOrder[num6 + 1]].GetHacks))
						{
						case 1:
							flag = true;
							break;
						case 0:
							if (CheckSwap(AtlasOrder, num6 + 1, num6, Players[AtlasOrder[num6 + 1]].GetCrashes, Players[AtlasOrder[num6]].GetCrashes) == 1)
							{
								flag = true;
							}
							break;
						}
					}
				}
			}
		}
		MissionNum = GameData.LatestCompletedMissions.Count - 1;
		DoStats();
		bRankup = false;
	}

	public void Draw()
	{
		if (TimeStart > 0f)
		{
			float num = Time.realtimeSinceStartup - TimeStart - TransferDelay;
			if (num > 0f && TransferStateNum < TransferTimer.Length)
			{
				if (TransferStateNum == -1 || TransferTimer[TransferStateNum] == 0f)
				{
					TransferStateNum++;
				}
				if (TransferStateNum < TransferTimer.Length)
				{
					TransferTimer[TransferStateNum] -= Time.deltaTime;
					if (TransferTimer[TransferStateNum] <= 0f)
					{
						TransferTimer[TransferStateNum] = 0f;
					}
					TransferPercent = 1f - TransferTimer[TransferStateNum] / TransferTimes[TransferStateNum];
				}
			}
		}
		GUIUtil.OnDrawWindow();
		Rect myScreenRect = MyScreenRect;
		myScreenRect.x = QueueBattle.screenSpace.x + (QueueBattle.screenSpace.width - MyScreenRect.width) / 2f;
		myScreenRect.y = QueueBattle.screenSpace.y + (QueueBattle.screenSpace.height - MyScreenRect.height) / 2f;
		GUI.color = new Color(1f, 1f, 1f, 0.85f);
		Rect position = new Rect(0f, 0f, QueueBattle.screenSpace.width, QueueBattle.screenSpace.height);
		GUI.Box(position, GUIContent.none, SharedSkin.GetStyle("blackbox"));
		GUI.color = Color.white;
		GUI.Window(123, myScreenRect, DrawScoreWindow, GUIContent.none, StatsSkin.window);
	}

	private void DrawScoreWindow(int WindowID)
	{
		ScreenSpace = new Rect(0f, 0f, Screen.width, Screen.height);
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		GUI.skin = SharedSkin;
		GUI.BeginGroup(PlayerGroup);
		if (Event.current.type == EventType.Repaint)
		{
			StatsSkin.GetStyle("TopFrame").Draw(new Rect(0f, 0f, PlayerGroup.width, 160f), GUIContent.none, -1, GameData.MyFactionId == 2);
			StatsSkin.GetStyle("Name").Draw(new Rect(0f, 160f, PlayerGroup.width, PlayerGroup.height - 160f), new GUIContent(GameData.MyDisplayName.ToUpper()), -1, GameData.MyFactionId == 2);
			GUI.Label(new Rect(0f, 160f, PlayerGroup.width, PlayerGroup.height - 160f), GameData.getRankName(TransferLevel / 10).ToUpper(), StatsSkin.GetStyle("Rank" + GameData.MyFactionId));
		}
		if (GameData.getExosuit(GameData.MySuitID) != null && GameData.getExosuit(GameData.MySuitID).mGuiLoadoutImage != null)
		{
			GUI.DrawTexture(new Rect(0f, 0f, 160f, 160f), GameData.getExosuit(GameData.MySuitID).mGuiLoadoutImage);
		}
		Texture2D texture2D = Parent.mLargeFactionRankIcons[(GameData.MyFactionId - 1) * 6 + TransferLevel / 10];
		GUI.DrawTexture(new Rect((PlayerGroup.width - (float)texture2D.width) / 2f, (PlayerGroup.height - 20f - (float)texture2D.height) / 2f, texture2D.width, texture2D.height), texture2D);
		int num = (TransferStateNum <= 1) ? CreditsGained : 0;
		if (TransferStateNum == 0)
		{
			LastXfer = num;
			GUI.color = Color.Lerp(Color.white, CreditColor, TransferPercent);
			GUI.Label(new Rect(PlayerGroup.width - 135f, PlayerGroup.height - 170f, 135f, 20f), "CREDITS EARNED", StatsSkin.GetStyle("CreditsLabel"));
			GUI.color = Color.white;
		}
		else if (TransferStateNum == 1)
		{
			GUI.color = CreditColor;
			GUI.Label(new Rect(PlayerGroup.width - 135f, PlayerGroup.height - 170f, 135f, 20f), "CREDITS EARNED", StatsSkin.GetStyle("CreditsLabel"));
			float num2 = Mathf.Max((1f - TransferPercent) * 4f - 2f, 1f);
			GUIUtil.BeginScaleGroup(new Rect(PlayerGroup.width - 135f, PlayerGroup.height - 155f, 135f, 40f), xy1, new Vector3(num2, num2, 1f));
			num = Mathf.RoundToInt((float)CreditsGained * Mathf.Clamp(1.3f - TransferPercent / 0.8f, 0f, 1f));
			if (LastXfer - num >= Mathf.Max(CreditsGained / 20, 1))
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Summary_XP_Tick);
				LastXfer = num;
			}
			GUI.Label(new Rect(0f, 0f, 125f, 40f), "+" + num, StatsSkin.GetStyle("CreditsValue"));
			GUIUtil.EndScaleGroup();
			GUI.color = Color.white;
		}
		GUI.Box(new Rect(PlayerGroup.width - 135f, PlayerGroup.height - 115f, 131f, 53f), (GameData.MyTotalCredits - num).ToString(), StatsSkin.GetStyle("TotalCredits"));
		int num3 = XpGained;
		int num4 = XpGained + BonusXP;
		int num5 = (TransferStateNum <= 3) ? (GameData.MyTotalXP - num4) : GameData.MyTotalXP;
		if (bMax)
		{
			num5 = 0;
			num3 = 0;
		}
		if (TransferStateNum == 2)
		{
			LastXfer = num5;
			GUI.color = Color.Lerp(Color.white, XPColor, TransferPercent);
			GUI.Label(new Rect((PlayerGroup.width - 135f) / 2f, PlayerGroup.height - 135f, 135f, 20f), "XP EARNED", StatsSkin.GetStyle("XPLabel"));
			GUI.color = Color.white;
		}
		else if (TransferStateNum == 3)
		{
			num5 = GameData.MyTotalXP - Mathf.RoundToInt(Mathf.Clamp(1.3f - TransferPercent / 0.8f, 0f, 1f) * (float)num3);
			if (num5 - LastXfer >= Mathf.Max(num3 / 20, 1))
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Summary_XP_Tick);
				LastXfer = num5;
			}
			GUI.color = XPColor;
			GUI.Label(new Rect((PlayerGroup.width - 135f) / 2f, PlayerGroup.height - 135f, 135f, 20f), "XP EARNED", StatsSkin.GetStyle("XPLabel"));
			float num6 = Mathf.Max((1f - TransferPercent) * 4f - 2f, 1f);
			GUIUtil.BeginScaleGroup(new Rect((PlayerGroup.width - 135f) / 2f, PlayerGroup.height - 110f, 135f, 60f), xy2, new Vector3(num6, num6, 1f));
			GUI.Label(new Rect(0f, 0f, 135f, 60f), "+" + Mathf.Abs(num5 - GameData.MyTotalXP), StatsSkin.GetStyle("XPValue"));
			GUIUtil.EndScaleGroup();
			GUI.color = Color.white;
		}
		int num7 = (TransferStateNum <= 5) ? BonusXP : 0;
		if (bMax)
		{
			num7 = 0;
		}
		if (TransferStateNum == 4)
		{
			if (BonusXP == 0)
			{
				TransferStateNum += 2;
			}
			else if (XPBonusTexture != null)
			{
				GUI.DrawTexture(new Rect((PlayerGroup.width - (float)XPBonusTexture.width) / 2f, 55f, XPBonusTexture.width, XPBonusTexture.height), XPBonusTexture);
				LastXfer = BonusXP;
				GUI.color = Color.Lerp(Color.white, BonusXPColor, TransferPercent);
				GUI.Label(new Rect((PlayerGroup.width - 400f) / 2f, 10f, 400f, 20f), BonusXPText, StatsSkin.GetStyle("XPLabel"));
				GUI.color = Color.white;
			}
		}
		else if (TransferStateNum == 5)
		{
			num7 = Mathf.RoundToInt(Mathf.Clamp(1.3f - TransferPercent / 0.8f, 0f, 1f) * (float)BonusXP);
			if (LastXfer - num7 >= Mathf.Max(BonusXP / 20, 1))
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Summary_XP_Tick);
				LastXfer = num7;
			}
			if (XPBonusTexture != null)
			{
				GUI.DrawTexture(new Rect((PlayerGroup.width - (float)XPBonusTexture.width) / 2f, 55f, XPBonusTexture.width, XPBonusTexture.height), XPBonusTexture);
				GUI.color = BonusXPColor;
				GUI.Label(new Rect((PlayerGroup.width - 400f) / 2f, 10f, 400f, 20f), BonusXPText, StatsSkin.GetStyle("XPLabel"));
			}
			float num8 = Mathf.Max((1f - TransferPercent) * 4f - 2f, 1f);
			GUIUtil.BeginScaleGroup(new Rect((PlayerGroup.width - 235f) / 2f, 30f, 235f, 60f), xy2, new Vector3(num8, num8, 1f));
			GUI.Label(new Rect(0f, 0f, 235f, 60f), "+" + num7, StatsSkin.GetStyle("XPValue"));
			GUIUtil.EndScaleGroup();
			GUI.color = Color.white;
		}
		int num9 = num5 - num7;
		while (GameData.getExpNeededForLevel(TransferLevel + 1) <= num9 && TransferLevel != 50)
		{
			if (GameData.MyTotalXP - num4 < GameData.getExpNeededForLevel(TransferLevel + 1))
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Summary_Levelup);
				LevelUpTimer = Time.realtimeSinceStartup + 2.4f;
			}
			TransferLevel++;
			if (GameData.getLevelFromExp(GameData.MyTotalXP - num4) / 10 < TransferLevel / 10)
			{
				bRankup = true;
			}
		}
		if (TransferLevel == 50)
		{
			GUI.Box(new Rect(PlayerGroup.width - 246f, PlayerGroup.height - 26f, 246f, 26f), GUIContent.none, "MaxLevel");
		}
		else
		{
			GUI.Box(new Rect(PlayerGroup.width - 246f, PlayerGroup.height - 26f, 246f, 26f), GUIContent.none, StatsSkin.GetStyle("XPBarBackground"));
			GUI.Box(new Rect(PlayerGroup.width - 246f, PlayerGroup.height - 24f, 50f + Mathf.Max(0f, 150f * (((float)num9 - (float)GameData.getExpNeededForLevel(TransferLevel)) / (float)(GameData.getExpNeededForLevel(TransferLevel + 1) - GameData.getExpNeededForLevel(TransferLevel)))), 22f), GUIContent.none, "XPBar");
			GUI.Label(new Rect(PlayerGroup.width - 239f, PlayerGroup.height - 26f, 30f, 28f), TransferLevel.ToString(), StatsSkin.GetStyle("QuantityText"));
			GUI.Label(new Rect(PlayerGroup.width - 35f, PlayerGroup.height - 26f, 30f, 28f), (TransferLevel + 1).ToString(), StatsSkin.GetStyle("QuantityText"));
		}
		if (LevelUpTimer > 0f)
		{
			float num10 = LevelUpTimer - Time.realtimeSinceStartup;
			GUIUtil.DrawAnimatedTextureFrame(new Rect(PlayerGroup.width - 250f, PlayerGroup.height - 108f, 256f, 128f), mLevelUpAnim, 4, (int)(num10 * 20f) % 4, MirrorX: false, MirrorY: false);
			if (num10 <= 2f)
			{
				GUIUtil.DrawAnimatedTextureFrame(new Rect(PlayerGroup.width - 250f, PlayerGroup.height - 108f, 256f, 128f), mLevelUpAnim, 4, 3, MirrorX: false, MirrorY: false);
			}
			if (num10 < 0f)
			{
				LevelUpTimer = 0f;
			}
		}
		GUI.EndGroup();
		GUI.BeginGroup(MissionGroup, (GameData.MyPlayStatus != 1) ? "LATEST COMPLETED MISSIONS" : string.Empty, StatsSkin.GetStyle("MissionLabel"));
		if (GameData.MyPlayStatus == 1)
		{
			GUI.Label(new Rect((MissionGroup.width - 324f) / 2f, 20f, 324f, 56f), "Join Today!", SharedSkin.GetStyle("MedalBG"));
			GUI.Box(new Rect((MissionGroup.width - 324f) / 2f + 5f, 23f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("MedalGeneric"));
			GUI.Label(new Rect((MissionGroup.width - 324f) / 2f, 23f, 324f, 50f), "Register or Login to earn missions for your faction.", SharedSkin.GetStyle("MedalDesc"));
		}
		else
		{
			List<SFSObject> latestCompletedMissions = GameData.LatestCompletedMissions;
			if (latestCompletedMissions.Count > 0)
			{
				int count = latestCompletedMissions.Count;
				int num11 = Mathf.Max(0, count - 5);
				int num12 = count - 1;
				int num13 = Mathf.Min(count, 5);
				if (MissionNum > num11)
				{
					switch (GUIUtil.Button(new Rect(40f, 38f, 14f, 26f), GUIContent.none, SharedSkin.GetStyle("MedalLeftArrow")))
					{
					case GUIUtil.GUIState.Hover:
					case GUIUtil.GUIState.Active:
						if (Event.current.type == EventType.Repaint)
						{
							b = "Left";
							if (lastHover != b)
							{
								GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
							}
						}
						break;
					case GUIUtil.GUIState.Click:
						b = "Left";
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
						MissionNum--;
						break;
					}
				}
				if (MissionNum < num12)
				{
					switch (GUIUtil.Button(new Rect(MissionGroup.width - 54f, 38f, 14f, 26f), GUIContent.none, SharedSkin.GetStyle("MedalRightArrow")))
					{
					case GUIUtil.GUIState.Hover:
					case GUIUtil.GUIState.Active:
						if (Event.current.type == EventType.Repaint)
						{
							b = "Right";
							if (lastHover != b)
							{
								GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
							}
						}
						break;
					case GUIUtil.GUIState.Click:
						b = "Right";
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
						MissionNum++;
						break;
					}
				}
				MissionNum = Mathf.Clamp(MissionNum, num11, num12);
				float num14 = MissionGroup.width / 2f - 4.5f - (float)(24 * (num13 - 1) / 2);
				for (int i = 0; i < num13; i++)
				{
					GUI.Toggle(new Rect(num14, MissionGroup.height - 10f, 9f, 8f), MissionNum != count - num13 + i, GUIContent.none, SharedSkin.GetStyle("MedalCountIndicator"));
					num14 += 24f;
				}
				SFSObject sFSObject = latestCompletedMissions[MissionNum];
				GUI.Label(new Rect((MissionGroup.width - 324f) / 2f, 23f, 324f, 56f), sFSObject.GetUtfString("Name"), SharedSkin.GetStyle("MedalBG"));
				string utfString = sFSObject.GetUtfString("Image");
				if (utfString != null && utfString.Length > 0)
				{
					utfString.Replace(".png", string.Empty);
					Texture2D texture2D2 = Resources.Load("Menus/Medals/" + utfString) as Texture2D;
					if (texture2D2 == null)
					{
						GUI.Box(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("MedalGeneric"));
					}
					else
					{
						GUI.DrawTexture(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 50f, 50f), texture2D2);
					}
				}
				else
				{
					int @int = sFSObject.GetInt("Credits");
					int int2 = sFSObject.GetInt("XP");
					if (int2 > 0)
					{
						if (@int > 0)
						{
							GUI.Box(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("XPCreditsMedal"));
						}
						else
						{
							GUI.Box(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("MedalNoReward"));
						}
					}
					else if (@int > 0)
					{
						GUI.Box(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("CreditsMedal"));
					}
					else
					{
						GUI.Box(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("MedalNoReward"));
					}
				}
				GUI.Label(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), sFSObject.GetUtfString("Description"), SharedSkin.GetStyle("MedalDesc"));
			}
			else
			{
				GUI.Label(new Rect((MissionGroup.width - 324f) / 2f, 23f, 324f, 56f), ":(", SharedSkin.GetStyle("MedalBG"));
				GUI.Box(new Rect((MissionGroup.width - 320f) / 2f + 5f, 26f, 324f, 50f), GUIContent.none, SharedSkin.GetStyle("MedalGeneric"));
				GUI.Label(new Rect((MissionGroup.width - 324f) / 2f, 26f, 324f, 50f), "You have no recently completed missions", SharedSkin.GetStyle("MedalDesc"));
			}
		}
		GUI.EndGroup();
		GUI.BeginGroup(ScoreGroup);
		GUI.Box(new Rect(0f, 0f, ScoreGroup.width, ScoreGroup.height), string.Empty, StatsSkin.GetStyle("ScoreBG"));
		if (Event.current.type == EventType.Repaint)
		{
			if (GameData.BattleType == 1)
			{
				GUI.Label(new Rect(560f, 6f, 60f, 20f), "HACKS", StatsSkin.GetStyle("ColumnTitle"));
				GUI.Label(new Rect(620f, 6f, 60f, 20f), "CRASHES", StatsSkin.GetStyle("ColumnTitle"));
				GUI.Label(new Rect(680f, 6f, 80f, 20f), "XP", StatsSkin.GetStyle("ColumnTitle"));
				DrawPlayerList(PlayerOrder, 52f, 20f);
			}
			else if (GameData.BattleType == 2)
			{
				GUI.Label(new Rect(510f, 6f, 60f, 20f), "HACKS", StatsSkin.GetStyle("ColumnTitle"));
				GUI.Label(new Rect(570f, 6f, 60f, 20f), "CRASHES", StatsSkin.GetStyle("ColumnTitle"));
				GUI.Label(new Rect(630f, 6f, 80f, 20f), "XP", StatsSkin.GetStyle("ColumnTitle"));
				GUI.Label(new Rect(710f, 6f, 100f, 20f), "TEAM SCORE", StatsSkin.GetStyle("ColumnTitle"));
				GUI.Box(new Rect(712f, 20f, 100f, 102f), string.Empty, StatsSkin.GetStyle("BanzaiScoreRank"));
				GUI.Box(new Rect(712f, 20f, 100f, 102f), GameData.BanzaiHacks.ToString(), StatsSkin.GetStyle("TeamBanzaiBG"));
				GUI.Box(new Rect(712f, 20f, 100f, 102f), string.Empty, StatsSkin.GetStyle("TeamBanzaiLogo"));
				GUI.Box(new Rect(712f, 124f, 100f, 102f), string.Empty, StatsSkin.GetStyle("AtlasScoreRank"));
				GUI.Box(new Rect(712f, 124f, 100f, 102f), GameData.AtlasHacks.ToString(), StatsSkin.GetStyle("TeamAtlasBG"));
				GUI.Box(new Rect(712f, 124f, 100f, 102f), string.Empty, StatsSkin.GetStyle("TeamAtlasLogo"));
				float y = DrawPlayerList(BanzaiOrder, 2f, 20f);
				DrawPlayerList(AtlasOrder, 2f, y);
			}
		}
		GUI.EndGroup();
		GUI.Box(new Rect(0f, ScoreGroup.y + ScoreGroup.height + 1f, MyScreenRect.width, 200f), GUIContent.none, StatsSkin.GetStyle("MissionLabel"));
		switch (GUIUtil.Button(new Rect((MyScreenRect.width - 134f) / 2f, MyScreenRect.height - 37f, 134f, 34f), "Continue", StatsSkin.GetStyle("Quit")))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "Continue";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "Continue";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			mDraw = false;
			bRankup = false;
			break;
		}
		if (bRankup)
		{
			float num15 = MyScreenRect.height / 2f;
			float num16 = MyScreenRect.width / 2f;
			Texture2D texture2D3 = Parent.mFactionRankUpBGs[GameData.MyFactionId - 1];
			GUI.DrawTexture(new Rect(num16 - (float)(texture2D3.width / 2), num15 - (float)(texture2D3.height / 2), texture2D3.width, texture2D3.height), texture2D3);
			Texture2D texture2D4 = Parent.mFactionRankUpIcons[(GameData.MyFactionId - 1) * 5 + (GameData.MyRank - 1)];
			GUI.DrawTexture(new Rect(num16 - (float)(texture2D4.width / 2), num15 - 75f - (float)(texture2D4.height / 2), texture2D4.width, texture2D4.height), texture2D4);
			GUI.Label(new Rect(num16 - 200f, num15 - 12f, 400f, 40f), GameData.getRankName(GameData.MyRank).ToUpper(), StatsSkin.GetStyle("RankText" + GameData.MyFactionId));
			switch (GUIUtil.Button(new Rect(num16 - 65f, num15 + (float)(texture2D3.height / 2) - 55f, 130f, 38f), "Close", "ModalButton"))
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
				bRankup = false;
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Hangar_Button_Press);
				break;
			}
		}
		lastHover = b;
	}

	private float DrawPlayerList(int[] list, float x, float y)
	{
		Rect position = default(Rect);
		position.y = y;
		position.height = 24f;
		int num = -1;
		for (int i = 0; i < list.Length; i++)
		{
			position.x = x;
			int num2 = list[i];
			if (num2 != -1 && Players[num2] != null)
			{
				int faction = Players[num2].Faction;
				bool on = faction == 2;
				num++;
				position.width = 24f;
				GUI.DrawTexture(position, Resources.Load("HUD/avatar/" + GameHUD.avatar_images[Players[num2].SuitID - 1]) as Texture2D);
				position.x += position.width;
				GUIStyle style = StatsSkin.GetStyle("ScoreNumberL");
				position.width = 40f;
				style.Draw(position, new GUIContent((i + 1).ToString()), -1, on);
				position.x += position.width;
				position.width = 78f;
				StatsSkin.GetStyle("ScoreNumberD").Draw(position, GUIContent.none, -1, on);
				GUI.DrawTexture(position, mFactionCards[faction - 1]);
				position.x += position.width;
				position.width = 236f;
				StatsSkin.GetStyle("ScoreNameFaction").Draw(position, new GUIContent(Players[num2].Name), -1, on);
				position.x += position.width;
				position.width = 130f;
				GUI.Box(position, GameData.getRankName(Players[num2].Rank).ToUpper(), StatsSkin.GetStyle(((faction != 1) ? "Atlas" : "Banzai") + "ScoreRank"));
				position.x += position.width;
				position.width = 60f;
				style.Draw(position, new GUIContent(Players[num2].Hacks.ToString()), -1, on);
				position.x += position.width;
				position.width = 60f;
				StatsSkin.GetStyle("ScoreNumberD").Draw(position, new GUIContent(Players[num2].Crashes.ToString()), -1, on);
				position.x += position.width;
				position.width = 80f;
				style.Draw(position, new GUIContent(Players[num2].BattleXP.ToString()), -1, on);
				position.x += position.width;
				if (Players[num2].ID == GameData.MyPlayerId)
				{
					GUI.Box(new Rect(x - 1f, position.y - 1f, position.x - x + 1f, 26f), GUIContent.none, StatsSkin.GetStyle("PlayerOutline"));
				}
			}
			position.y += 26f;
		}
		GUI.color = Color.white;
		return position.y;
	}

	private int CheckSwap(int[] arry, int a, int b, Func aVal, Func bVal)
	{
		if (Players[arry[a]] == null)
		{
			if (Players[arry[b]] == null)
			{
				return -3;
			}
			int num = arry[a];
			arry[a] = arry[b];
			arry[b] = num;
			return 2;
		}
		if (Players[arry[b]] == null)
		{
			return -2;
		}
		if (aVal() < bVal())
		{
			int num2 = arry[a];
			arry[a] = arry[b];
			arry[b] = num2;
			return 1;
		}
		if (aVal() == bVal())
		{
			return 0;
		}
		return -1;
	}

	private void DoStats()
	{
		Logger.trace("::::: Send Stats :::::");
		bool flag = false;
		if (GameData.BattleType == 1)
		{
			flag = (Players[PlayerOrder[0]].BattleXP == GameData.MyBattleXP);
			AchievementManager.SendStat(AchievementManager.ExonautStats.GamesPlayed_Battle, 1);
			AchievementManager.SendStat(AchievementManager.ExonautStats.Highest_Ratio_Battle, GameData.MyBattleCaptures / Mathf.Max(GameData.MyBattleFalls, 1));
			if (flag)
			{
				AchievementManager.SendStat(AchievementManager.ExonautStats.GamesWon_Battle, 1);
				AchievementManager.SendStat(AchievementManager.ExonautStats.Highest_Ratio_Battle_Win, GameData.MyBattleCaptures / Mathf.Max(GameData.MyBattleFalls, 1));
			}
		}
		else if (GameData.BattleType == 2)
		{
			AchievementManager.SendStat(AchievementManager.ExonautStats.GamesPlayed_TeamBattle, 1);
			AchievementManager.SendStat(AchievementManager.ExonautStats.Highest_Ratio_TeamBattle, GameData.MyBattleCaptures / Mathf.Max(GameData.MyBattleFalls, 1));
			if (flag)
			{
				AchievementManager.SendStat(AchievementManager.ExonautStats.GamesWon_TeamBattle, 1);
				AchievementManager.SendStat(AchievementManager.ExonautStats.Highest_Ratio_TeamBattle_Win, GameData.MyBattleCaptures / Mathf.Max(GameData.MyBattleFalls, 1));
			}
		}
		AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_Total, GameData.MyBattleCaptures);
		AchievementManager.SendStat(AchievementManager.ExonautStats.Crashes_Total, GameData.MyBattleCaptures);
		AchievementManager.SendStat(AchievementManager.ExonautStats.Level, GameData.MyLevel);
		AchievementManager.SendStat(AchievementManager.ExonautStats.Highest_XP_SingleGame, GameData.MyBattleXP);
		switch (IsDefaultSuit(GameData.MySuitID))
		{
		case 2:
			AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_AtlasTrinity, GameData.MyBattleCaptures);
			break;
		case 1:
			AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_BanzaiTrinity, GameData.MyBattleCaptures);
			break;
		}
		AchievementManager.SendStat(AchievementManager.ExonautStats.GamesPlayedInARow, ++GameData.ConsecutiveGames);
		if (flag)
		{
			switch (GameData.WorldID)
			{
			case 2:
				if (GameData.MySuitID == 6 || GameData.MySuitID == 11)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_Abysus_Rex_Bobo, 1);
				}
				if (GameData.MySuitID == 27 || GameData.MySuitID == 30)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_Abysus_VanKleiss_Skalamander, 1);
				}
				break;
			case 4:
				if (GameData.MySuitID == 26 || GameData.MySuitID == 36)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_Treehouse_Jake_Bubblegum, 1);
				}
				if (GameData.MySuitID == 4 || GameData.MySuitID == 5)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_Treehouse_Finn_Marceline, 1);
				}
				break;
			case 5:
				if (GameData.MySuitID == 24 || GameData.MySuitID == 28 || GameData.MySuitID == 34 || GameData.MySuitID == 37)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_Perplex_UltHS_NRG_FourArms_UltCannonbolt, 1);
				}
				if (GameData.MySuitID == 7 || GameData.MySuitID == 10 || GameData.MySuitID == 14 || GameData.MySuitID == 18)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_Perplex_UltBC_UltEE_UltSF_Heatblast, 1);
				}
				break;
			case 6:
				if (GameData.MySuitID == 13)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_BBB_JohnnyTest, 1);
				}
				if (GameData.MySuitID == 33)
				{
					AchievementManager.SendStat(AchievementManager.ExonautStats.Wins_BBB_BlingBlingBoy, 1);
				}
				break;
			}
		}
		AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_Invisible, GameData.HackWhileInvisible);
		AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_Speed, GameData.HackWhileSpeedBoost);
		AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_DamageBoost, GameData.HackWhileDamageBoost);
		AchievementManager.SendStat(AchievementManager.ExonautStats.Hacks_ArmorBoost, GameData.HackWhileArmorBoost);
	}

	private int IsDefaultSuit(int num)
	{
		int[] atlasDefaultSuits = GameData.AtlasDefaultSuits;
		foreach (int num2 in atlasDefaultSuits)
		{
			if (num2 == num)
			{
				return 2;
			}
		}
		int[] banzaiDefaultSuits = GameData.BanzaiDefaultSuits;
		foreach (int num3 in banzaiDefaultSuits)
		{
			if (num3 == num)
			{
				return 1;
			}
		}
		return -1;
	}
}
