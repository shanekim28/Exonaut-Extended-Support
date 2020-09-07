using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
	public class PlayerScore
	{
		public int Faction;

		public int Count;

		public PlayerScore(int _Faction, int _Count)
		{
			Faction = _Faction;
			Count = _Count;
		}
	}

	public delegate void FUNC();

	private const float MSG_TIME = 2f;

	private const float MSG_REMOVE_TIME = 2f;

	public Hashtable PlayerScores = new Hashtable();

	private GUIStyle GUIbigText;

	private GUIStyle GUIsmallText;

	public float leftEdge;

	public float topEdge;

	public float rightEdge;

	public float bottomEdge;

	public float centerX;

	public float centerY;

	private float maxHealth;

	private float currentHealth;

	private float healthAdjustRate;

	private float maxFuel;

	private float currentFuel;

	private float targetFuel;

	private float fuelAdjustRate;

	private int mNumElements;

	private Element healthBar;

	private Element jetpackBar;

	public List<Element> mElements;

	public float mVariance;

	public Vector2 mScreenTopLeft;

	public Vector2 mScreenBotRight;

	public string classPath;

	public WeaponScript myWeapon;

	private Queue mMessageQueue = new Queue();

	public int mMaxMessages = 8;

	public Texture2D activeTimer;

	public Texture2D frame_2_bars;

	public Texture2D banzai_logo;

	public Texture2D atlas_logo;

	public Texture2D jetpack_icon;

	public Texture2D health_icon;

	public Texture2D fill_bar_health;

	public Texture2D fill_bar_jetpack;

	public Texture2D fill_bar_captures_me;

	public Texture2D fill_bar_captures_next;

	public Texture2D frame_boost;

	public Texture2D frame_power;

	public Texture2D fill_boost;

	public Texture2D fill_bar_ammo;

	public Texture2D banzai_icon;

	public Texture2D atlas_icon;

	public Texture2D frame_1_bar;

	public Texture2D fill_bar_captures;

	public Texture2D frame_grenade;

	public Texture2D fill_grenade;

	public Texture2D offscreen_arrow_up;

	public Texture2D offscreen_arrow_down;

	public Texture2D offscreen_arrow_left;

	public Texture2D offscreen_arrow_right;

	public Texture2D mBoostArmorTexture;

	public Texture2D mBoostDamageTexture;

	public Texture2D mBoostInvisTexture;

	public Texture2D mBoostSpeedTexture;

	public Texture2D mInfinityTexture;

	public Texture2D mPickupTimerTexture;

	public Texture2D mChooseWeaponBattleTexture;

	public Texture2D mChooseWeaponTeamTexture;

	public Texture2D mMatchEndTexture;

	public Texture2D mMatchStartTexture;

	public Texture2D mWinnerAtlas;

	public Texture2D mWinnerBanzai;

	public float mBannerMessageTimer;

	public Texture2D mBannerMessage;

	public Rect frame_2_bars_RECT;

	public Rect banzai_logo_RECT;

	public Rect atlas_logo_RECT;

	public Rect jetpack_icon_RECT;

	public Rect health_icon_RECT;

	public Rect fill_bar_health_RECT;

	public Rect fill_bar_jetpack_RECT;

	public Rect frame_boost_RECT;

	public Rect frame_power_RECT;

	public Rect fill_boost_RECT;

	public Rect banzai_icon_RECT;

	public Rect atlas_icon_RECT;

	public Rect frame_1_bar_RECT;

	public Rect frame_grenade_RECT;

	public Rect fill_grenade_RECT;

	public Rect offscreen_arrow_RECT;

	public Texture2D[] mWeaponSelectArrow = new Texture2D[4];

	public Texture2D[] mWeaponSelectCrown = new Texture2D[4];

	public Texture2D[] mWeaponSelectOver = new Texture2D[4];

	public Texture2D mWeaponSelectFrame;

	public Texture[] mWeaponImage = new Texture2D[8];

	public Texture2D[] mPickupTimer = new Texture2D[19];

	public Texture2D mSniperScreen;

	public Texture2D mHeavyWeaponUnavailableTexture;

	public Texture2D mBoostUnavailableTexture;

	public Texture2D[] mReticle = new Texture2D[8];

	public Texture2D[] mReticleReload = new Texture2D[8];

	public int mReticleReloadFrame;

	public float mReticleReloadFramerate;

	public float mReticleReloadFrametime;

	public bool mReloading;

	public GUIStyle mNumAmmoLeftStyle;

	private bool mOverSMG;

	private bool mOverShotgun;

	private bool mOverRifle;

	private bool mOverSpread;

	private int score1;

	private int score2;

	private Texture2D[] mTaunt = new Texture2D[4];

	private string[] powerList = new string[40]
	{
		"HUD_power_icon_damage_cloud",
		"HUD_power_icon_damage_reaction",
		"HUD_power_icon_temp",
		"HUD_power_icon_jetpack_overheat",
		"HUD_power_icon_dash_slow",
		"HUD_power_icon_vampire",
		"HUD_power_icon_decoy",
		"HUD_power_icon_dodge_bullets",
		"HUD_power_icon_escape",
		"HUD_power_icon_fake_capture",
		"HUD_power_icon_health_regen",
		"HUD_power_icon_infinite_ammo",
		"HUD_pickup_inviso_icon",
		"HUD_power_icon_jetpack_overheat",
		"HUD_power_icon_melee_aoe",
		"HUD_power_icon_melee_beam",
		"HUD_power_icon_melee_pulse",
		"HUD_power_icon_melee_punch",
		"HUD_power_icon_melee_sword",
		"HUD_power_icon_mines",
		"HUD_power_icon_poison_cloud",
		"HUD_power_icon_silencer",
		"HUD_power_icon_refill_ammo",
		"HUD_power_icon_jetpack_refill",
		"HUD_power_icon_reflect",
		"HUD_power_icon_grenade_refill",
		"HUD_power_icon_respawn_boosts",
		"HUD_power_icon_blood_bullets",
		"HUD_power_icon_temp",
		"HUD_power_icon_jetpack_overload",
		"HUD_power_icon_laser",
		"HUD_power_icon_shots_pull",
		"HUD_power_icon_shots_push",
		"HUD_power_icon_temp",
		"HUD_power_icon_shot_slow",
		"HUD_power_icon_radar",
		"HUD_power_icon_trap_boosts",
		"HUD_power_icon_turret",
		"HUD_power_icon_wall",
		"HUD_power_icon_weapon_jam"
	};

	public Texture2D avatarTexture;

	public static string[] avatar_images = new string[56]
	{
		"HUD_thumb_banzailight",
		"HUD_thumb_banzaimedium",
		"HUD_thumb_banzaiheavy",
		"HUD_thumb_finn",
		"HUD_thumb_marceline",
		"HUD_thumb_genrex",
		"HUD_thumb_bigchill",
		"HUD_thumb_regularshow",
		"HUD_thumb_manus",
		"HUD_thumb_echoecho",
		"HUD_thumb_bobo",
		"HUD_thumb_flapjack",
		"HUD_thumb_jtest",
		"HUD_thumb_swampfire",
		"HUD_thumb_biowulf",
		"HUD_thumb_corus",
		"HUD_thumb_iceking",
		"HUD_thumb_heatblast",
		"HUD_thumb_ppg",
		"HUD_thumb_samjack",
		"HUD_thumb_atlaslight",
		"HUD_thumb_atlasmedium",
		"HUD_thumb_atlasheavy",
		"HUD_thumb_ulthumong",
		"HUD_thumb_gwen",
		"HUD_thumb_jake",
		"HUD_thumb_vankleiss",
		"HUD_thumb_nrg",
		"HUD_thumb_rath",
		"HUD_thumb_skalamander",
		"HUD_thumb_chowder",
		"HUD_thumb_mojojojo",
		"HUD_thumb_blingboy",
		"HUD_thumb_fourarms",
		"HUD_thumb_six",
		"HUD_thumb_bubblegum",
		"HUD_thumb_cannonbolt",
		"HUD_thumb_grim",
		"HUD_thumb_dexter",
		"HUD_thumb_vilgax",
		"HUD_thumb_darwin",
		"HUD_thumb_gumball",
		"HUD_thumb_lsp",
		"HUD_thumb_beemo",
		"HUD_thumb_ult_kevin",
		"HUD_thumb_alien_x",
		"HUD_thumb_rigby",
		"HUD_thumb_skips",
		"HUD_thumb_tina",
		"HUD_thumb_penny",
		"HUD_thumb_ben10atlas",
		"HUD_thumb_ben10banzai",
		"HUD_thumb_elitemarceline",
		"HUD_thumb_elitemanus",
		"HUD_thumb_eliterath",
		"HUD_thumb_elitemojo"
	};

	private Texture2D[] icons;

	private Color fill_color_white;

	private List<HUD_Timer> effectTimerList = new List<HUD_Timer>();

	private Texture2D[] mBoostMessageIcon = new Texture2D[4];

	private Texture2D[] mBoostMessageText = new Texture2D[4];

	private Texture2D[] mBoostTeamMessageText = new Texture2D[4];

	private Texture2D boostMessageIcon;

	private Texture2D boostMessageText;

	private Vector2 mBoostMessageIconPosition;

	private Vector2 mBoostMessageIconGoalPosition;

	private float boostMessageTimer;

	public FUNC boostMessageStep;

	private Player myPlayer;

	public GamePlay gamePlay;

	private float hud_offset = 12f;

	private Color boostColor;

	public float mBannerMessageX;

	public float mBannerMessageY;

	public bool mBannerMessageFade;

	private void Awake()
	{
		mElements = null;
		fill_color_white = new Color(1f, 1f, 1f, 0.5f);
		healthAdjustRate = 1f;
	}

	private void Start()
	{
		mScreenTopLeft.x = Camera.main.rect.xMin;
		mScreenTopLeft.y = Camera.main.rect.yMin;
		mScreenBotRight.x = Camera.main.rect.xMax;
		mScreenBotRight.y = Camera.main.rect.yMax;
		gamePlay = GamePlay.GetGamePlayScript();
		myPlayer = (gamePlay.myPlayer.GetComponent("Player") as Player);
		if (myPlayer != null)
		{
			myWeapon = (myPlayer.weapon.GetComponent("WeaponScript") as WeaponScript);
		}
		icons = new Texture2D[powerList.Length];
		string str = "HUD/screenelements/";
		for (int i = 0; i < powerList.Length; i++)
		{
			icons[i] = (Resources.Load(str + powerList[i]) as Texture2D);
		}
		for (int j = 0; j < 8; j++)
		{
			mReticle[j] = (Resources.Load("HUD/weaponReticles_" + j.ToString()) as Texture2D);
			mReticleReload[j] = (Resources.Load("HUD/weaponReticleReload_" + j.ToString()) as Texture2D);
		}
		mReticleReloadFrame = 0;
		mReticleReloadFramerate = 71f / (678f * (float)Math.PI);
		mReticleReloadFrametime = 0f;
		mOverSMG = false;
		mOverShotgun = false;
		mOverRifle = false;
		mOverSpread = false;
	}

	public void createHUD(string faction, float mh, float mfuel)
	{
		classPath = "HUD/faction1/";
		GameObject gameObject = GameObject.Find("localPlayer");
		myPlayer = (gameObject.GetComponent("Player") as Player);
		mElements = new List<Element>();
		leftEdge = (float)Screen.width * Camera.main.rect.xMin;
		topEdge = (float)Screen.height * Camera.main.rect.yMin;
		rightEdge = (float)Screen.width * Camera.main.rect.xMax;
		bottomEdge = (float)Screen.height * Camera.main.rect.yMax;
		centerX = (leftEdge + rightEdge) * 0.5f;
		centerY = (topEdge + bottomEdge) * 0.5f;
		Logger.trace("<< leftEdge: " + leftEdge + ", rightEdge: " + rightEdge + " topEdge: " + topEdge + " bottomEdge: " + bottomEdge);
		frame_2_bars = (Resources.Load(classPath + "HUD_frame_2bars") as Texture2D);
		banzai_logo = (Resources.Load(classPath + "HUD_banzai_icon") as Texture2D);
		atlas_logo = (Resources.Load(classPath + "HUD_atlas_icon") as Texture2D);
		jetpack_icon = (Resources.Load(classPath + "HUD_jetpack_icon") as Texture2D);
		health_icon = (Resources.Load(classPath + "HUD_health_icon") as Texture2D);
		fill_bar_health = (Resources.Load(classPath + "HUD_fill_bar") as Texture2D);
		fill_bar_jetpack = (Resources.Load(classPath + "HUD_fill_bar") as Texture2D);
		fill_bar_captures_me = (Resources.Load(classPath + "HUD_fill_bar") as Texture2D);
		fill_bar_captures_next = (Resources.Load(classPath + "HUD_fill_bar") as Texture2D);
		frame_2_bars_RECT = new Rect(0f, 40f, 209f, 43f);
		banzai_logo_RECT = new Rect(16f, 0f, 52f, 52f);
		health_icon_RECT = new Rect(47f, 52f, 10f, 10f);
		jetpack_icon_RECT = new Rect(47f, 70f, 10f, 10f);
		fill_bar_health_RECT = new Rect(0f, 0f, 147f, 17f);
		fill_bar_jetpack_RECT = new Rect(0f, 0f, 147f, 17f);
		frame_boost = (Resources.Load(classPath + "HUD_frame_boost") as Texture2D);
		activeTimer = (Resources.Load(classPath + "HUD_fill_boost") as Texture2D);
		frame_power = (Resources.Load(classPath + "HUD_frame_boost") as Texture2D);
		fill_boost = (Resources.Load(classPath + "HUD_fill_boost") as Texture2D);
		banzai_icon = (Resources.Load(classPath + "HUD_banzai_icon_small") as Texture2D);
		atlas_icon = (Resources.Load(classPath + "HUD_atlas_icon_small") as Texture2D);
		frame_1_bar = (Resources.Load(classPath + "HUD_frame_1bar") as Texture2D);
		fill_bar_ammo = (Resources.Load(classPath + "HUD_fill_bar1") as Texture2D);
		frame_grenade = (Resources.Load(classPath + "HUD_frame_grenade") as Texture2D);
		fill_grenade = (Resources.Load(classPath + "HUD_fill_grenade") as Texture2D);
		classPath = "HUD/screenelements/";
		mInfinityTexture = (Resources.Load(classPath + "HUD_infinity") as Texture2D);
		offscreen_arrow_up = (Resources.Load(classPath + "HUD_radar_arrow_up") as Texture2D);
		offscreen_arrow_down = (Resources.Load(classPath + "HUD_radar_arrow_down") as Texture2D);
		offscreen_arrow_left = (Resources.Load(classPath + "HUD_radar_arrow_left") as Texture2D);
		offscreen_arrow_right = (Resources.Load(classPath + "HUD_radar_arrow_right") as Texture2D);
		mBoostArmorTexture = (Resources.Load(classPath + "HUD_pickup_armor_icon") as Texture2D);
		mBoostDamageTexture = (Resources.Load(classPath + "HUD_pickup_damage_icon") as Texture2D);
		mBoostInvisTexture = (Resources.Load(classPath + "HUD_pickup_inviso_icon") as Texture2D);
		mBoostSpeedTexture = (Resources.Load(classPath + "HUD_pickup_speed_icon") as Texture2D);
		mPickupTimerTexture = (Resources.Load("effects/pickup_timer") as Texture2D);
		for (int i = 0; i < 19; i++)
		{
			mPickupTimer[i] = (Resources.Load("HUD/boostMessages/boost_timer" + i) as Texture2D);
		}
		for (int j = 1; j < 5; j++)
		{
			mWeaponSelectArrow[j - 1] = (Resources.Load("HUD/weaponSelectMenu/HUD_weapon_select_arrow" + j.ToString()) as Texture2D);
			mWeaponSelectCrown[j - 1] = (Resources.Load("HUD/weaponSelectMenu/HUD_weapon_select_crown" + j.ToString()) as Texture2D);
			mWeaponSelectOver[j - 1] = (Resources.Load("HUD/weaponSelectMenu/HUD_weapon_select_over" + j.ToString()) as Texture2D);
		}
		for (int k = 1; k <= 8; k++)
		{
			mWeaponImage[k - 1] = (Resources.Load("HUD/HUD_equipped_weapon_icon" + k.ToString()) as Texture2D);
		}
		mWeaponSelectFrame = (Resources.Load("HUD/weaponSelectMenu/HUD_weapon_select_frame") as Texture2D);
		maxHealth = mh;
		currentHealth = mh;
		maxFuel = mfuel;
		currentFuel = mfuel;
		targetFuel = mfuel;
		mSniperScreen = (Resources.Load("HUD/scope2") as Texture2D);
		for (int l = 1; l <= 4; l++)
		{
			mTaunt[l - 1] = (Resources.Load("HUD/taunts/bubble" + l) as Texture2D);
		}
		mBoostMessageIcon[0] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_damage_icon") as Texture2D);
		mBoostMessageText[0] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_damage_text") as Texture2D);
		mBoostTeamMessageText[0] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_damage_team_text") as Texture2D);
		mBoostMessageIcon[1] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_armor_icon") as Texture2D);
		mBoostMessageText[1] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_armor_text") as Texture2D);
		mBoostTeamMessageText[1] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_armor_team_text") as Texture2D);
		mBoostMessageIcon[2] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_inviso_icon") as Texture2D);
		mBoostMessageText[2] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_inviso_text") as Texture2D);
		mBoostTeamMessageText[2] = null;
		mBoostMessageIcon[3] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_speed_icon") as Texture2D);
		mBoostMessageText[3] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_speed_text") as Texture2D);
		mBoostTeamMessageText[3] = (Resources.Load("HUD/boostMessages/HUD_msg_boost_speed_team_text") as Texture2D);
		mHeavyWeaponUnavailableTexture = (Resources.Load("HUD/boostMessages/heavy_weapon_unavailable") as Texture2D);
		mBoostUnavailableTexture = (Resources.Load("HUD/boostMessages/boost_unavailable") as Texture2D);
		boostMessageStep = null;
		avatarTexture = (Resources.Load("HUD/avatar/" + avatar_images[myPlayer.mySuitIdx - 1]) as Texture2D);
		mChooseWeaponBattleTexture = (Resources.Load("HUD/gameMessages/HUD_msg_choose_weapon_battle") as Texture2D);
		mChooseWeaponTeamTexture = (Resources.Load("HUD/gameMessages/HUD_msg_choose_weapon_team") as Texture2D);
		mMatchEndTexture = (Resources.Load("HUD/gameMessages/HUD_msg_match_end") as Texture2D);
		mMatchStartTexture = (Resources.Load("HUD/gameMessages/HUD_msg_match_start_go") as Texture2D);
		mWinnerAtlas = (Resources.Load("HUD/gameMessages/HUD_msg_winner_atlas") as Texture2D);
		mWinnerBanzai = (Resources.Load("HUD/gameMessages/HUD_msg_winner_banzai") as Texture2D);
		mBannerMessageTimer = 0f;
		mBannerMessage = null;
		showReticle();
	}

	private void FixedUpdate()
	{
		if (GameData.CurPlayState == GameData.PlayState.GAME_IS_QUITTING)
		{
			Logger.traceAlways("[GameHUD::LateUpdate] - was going to run");
			return;
		}
		if (myWeapon == null && myPlayer.weapon != null)
		{
			myWeapon = (myPlayer.weapon.GetComponent("WeaponScript") as WeaponScript);
			if (myWeapon == null)
			{
				Logger.trace("<< I don't have a weapon");
				return;
			}
		}
		if (currentFuel != targetFuel)
		{
			float num = 1f;
			if (currentFuel > targetFuel)
			{
				num = -1f;
			}
			float num2 = currentFuel - targetFuel;
			float num3 = healthAdjustRate * num;
			if (Mathf.Abs(num2) < Mathf.Abs(num3))
			{
				num3 = num2;
			}
			currentFuel += num3;
			if (currentFuel > maxFuel)
			{
				currentFuel = maxFuel;
			}
			if (currentFuel < 0f)
			{
				currentFuel = 0f;
			}
			currentFuel = targetFuel;
		}
		int num4 = 0;
		bool flag = false;
		while (num4 < effectTimerList.Count)
		{
			if (effectTimerList[num4].Update() == 1)
			{
				effectTimerList.RemoveAt(num4);
				flag = true;
			}
			else
			{
				num4++;
			}
		}
		if (flag)
		{
			ResetTargetPosition();
		}
	}

	public void Update()
	{
		if (GameData.CurPlayState == GameData.PlayState.GAME_IS_QUITTING)
		{
			Logger.traceAlways("[GameHUD::Update] - was going to run");
			return;
		}
		if (mReloading)
		{
			handleReload();
		}
		if (mBannerMessageFade && mBannerMessageTimer > 0f)
		{
			mBannerMessageTimer -= Time.deltaTime;
		}
	}

	public void setMaxHealth(float mh)
	{
		maxHealth = mh;
	}

	public void setCurrentHealth(float ch)
	{
		currentHealth = ch;
	}

	public void setTargetHealth(float th)
	{
	}

	public void setMaxFuel(float mfuel)
	{
		maxFuel = mfuel;
	}

	public void setTargetFuel(float tfuel)
	{
		targetFuel = tfuel;
	}

	public void addTimer(int type, float time, int slotIdx)
	{
		Texture2D icon = new Texture2D(mBoostArmorTexture.width, mBoostArmorTexture.height);
		if (slotIdx == -1)
		{
			switch (type)
			{
			case 1:
			case 11:
				icon = mBoostDamageTexture;
				ShowBoostMessage(0, type == 11);
				break;
			case 0:
			case 10:
				icon = mBoostArmorTexture;
				ShowBoostMessage(1, type == 10);
				break;
			case 2:
				icon = mBoostInvisTexture;
				ShowBoostMessage(2, teamBoost: false);
				break;
			case 3:
			case 13:
				icon = mBoostSpeedTexture;
				ShowBoostMessage(3, type == 13);
				break;
			}
		}
		else
		{
			Logger.trace("<< adding weapon cooldown ");
			icon = mHeavyWeaponUnavailableTexture;
		}
		HUD_Timer item = new HUD_Timer(new Vector2(0f, 0f), false, time, frame_power, activeTimer, 0, type, icon, fill_color_white);
		for (int i = 0; i < effectTimerList.Count; i++)
		{
			effectTimerList[i].mPositionTarget.x += 50f;
		}
		effectTimerList.Add(item);
	}

	public void ClearTimers()
	{
		effectTimerList.Clear();
	}

	public void ClearBoostTimers()
	{
		for (int i = 0; i < effectTimerList.Count; i++)
		{
			if (effectTimerList[i].mType != 50)
			{
				effectTimerList.RemoveAt(i);
			}
		}
		ResetTargetPosition();
	}

	private void ResetTargetPosition()
	{
		float num = 0f;
		for (int i = 0; i < effectTimerList.Count; i++)
		{
			effectTimerList[i].mPositionTarget.x = num;
			num += 50f;
		}
	}

	public void AddMessage(string message)
	{
		if (mMessageQueue.Count == mMaxMessages)
		{
			mMessageQueue.Dequeue();
		}
		mMessageQueue.Enqueue(message);
	}

	private void showReticle()
	{
		DynamicOptions.bDrawCursor = false;
	}

	private void hideReticle()
	{
		DynamicOptions.bDrawCursor = true;
	}

	public void startReload(float reloadTime)
	{
		mReloading = true;
		mReticleReloadFrame = 0;
		mReticleReloadFramerate = reloadTime / 8f;
		mReticleReloadFrametime = mReticleReloadFramerate;
		Logger.trace("<< reload time: " + reloadTime);
	}

	private void handleReload()
	{
		mReticleReloadFrametime -= Time.deltaTime;
		if (mReticleReloadFrametime <= 0f)
		{
			mReticleReloadFrame++;
			if (mReticleReloadFrame == 8)
			{
				mReloading = false;
				myWeapon.numInClip = ((myWeapon.totalBullets >= myWeapon.clipSize) ? myWeapon.clipSize : myWeapon.totalBullets);
				myWeapon.numClips = myWeapon.totalBullets / myWeapon.clipSize;
			}
			else
			{
				mReticleReloadFrametime = mReticleReloadFramerate;
			}
		}
	}

	public bool AmReloading()
	{
		return mReloading;
	}

	private void OnGUI()
	{
		if (GameData.CurPlayState == GameData.PlayState.GAME_IS_QUITTING)
		{
			Logger.traceAlways("[OnGUI::Update] - was going to run");
			return;
		}
		if (GameData.MyTutorialStep == 3 && FirstUse.mInstance != null)
		{
			Rect groupPos = FirstUse.mInstance.GetGroupPos();
			groupPos.y += groupPos.height - FirstUse.mInstance.GetFrame().mBoxSize.y;
			groupPos.height = FirstUse.mInstance.GetFrame().mBoxSize.y;
			if (groupPos.Contains(Event.current.mousePosition))
			{
				hideReticle();
			}
			else
			{
				showReticle();
			}
		}
		else if (MessageBox.mMessageBox.Queuesize > 0 && MessageBox.mMessageBox.WindowPosition.Contains(Event.current.mousePosition))
		{
			hideReticle();
		}
		else
		{
			showReticle();
		}
		GUI.depth = 5;
		leftEdge = (float)Screen.width * Camera.main.rect.xMin;
		topEdge = (float)Screen.height * Camera.main.rect.yMin;
		rightEdge = (float)Screen.width * Camera.main.rect.xMax;
		bottomEdge = (float)Screen.height * Camera.main.rect.yMax;
		centerX = (leftEdge + rightEdge) / 2f;
		centerY = (topEdge + bottomEdge) / 2f;
		GameObject gameObject = GameObject.Find("localPlayer");
		Player player = gameObject.GetComponent("Player") as Player;
		maxHealth = player.healthMax;
		maxFuel = player.fuelMax;
		if (currentHealth != player.healthCurrent && player.healthCurrent != maxHealth)
		{
			currentHealth = player.healthCurrent;
		}
		currentFuel = player.fuelCurrent;
		GUI.BeginGroup(new Rect(leftEdge + hud_offset, topEdge + hud_offset, 300f, 50f));
		int count = effectTimerList.Count;
		for (int i = 0; i < count; i++)
		{
			HUD_Timer hUD_Timer = effectTimerList[i];
			hUD_Timer.Draw();
		}
		GUI.EndGroup();
		if (gamePlay.mIsBattleOver)
		{
			if (GameData.isTeamBattle())
			{
				if (score1 == score2)
				{
					mBannerMessage = mMatchEndTexture;
				}
				else if (score1 > score2)
				{
					if (player.mFaction == 1)
					{
						mBannerMessage = mWinnerBanzai;
					}
					else
					{
						mBannerMessage = mWinnerAtlas;
					}
				}
				else if (player.mFaction == 1)
				{
					mBannerMessage = mWinnerAtlas;
				}
				else
				{
					mBannerMessage = mWinnerBanzai;
				}
			}
			else
			{
				mBannerMessage = mMatchEndTexture;
			}
			GUI.DrawTexture(new Rect(centerX - (float)mBannerMessage.width * 0.5f, 50f, mBannerMessage.width, mBannerMessage.height), mBannerMessage);
		}
		if (((bool)player.captureBubble || gamePlay.waitForWeaponSelectTimer > 0f) && gamePlay.mGameReady)
		{
			Vector3 vector = player.transform.position;
			float num = 0f;
			float num2 = 0f;
			if (player != null && player.captureBubble != null)
			{
				Camera main = Camera.main;
				Vector3 position = player.captureBubble.transform.position;
				float x = position.x + num;
				Vector3 position2 = player.captureBubble.transform.position;
				vector = main.WorldToScreenPoint(new Vector3(x, position2.y + num2, 0f));
			}
			GUI.BeginGroup(new Rect(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height, mWeaponSelectFrame.width, mWeaponSelectFrame.height));
			GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectFrame);
			if (Input.GetMouseButtonDown(0))
			{
				if (overSMG(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
				{
					player.previousWeaponNotBlaster = 4;
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickWildfire);
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickWeapon);
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mousePressBtnSnd);
				}
				if (overShotgun(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
				{
					player.previousWeaponNotBlaster = 3;
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickBallista);
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickWeapon);
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mousePressBtnSnd);
				}
				if (overRifle(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
				{
					player.previousWeaponNotBlaster = 2;
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickMarksman);
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickWeapon);
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mousePressBtnSnd);
				}
				if (overSpread(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
				{
					player.previousWeaponNotBlaster = 5;
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickTridex);
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.PickWeapon);
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mousePressBtnSnd);
				}
			}
			if (overSMG(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
			{
				if (!mOverSMG)
				{
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mouseOverBtnSnd);
					mOverSMG = true;
				}
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectOver[0]);
			}
			else
			{
				mOverSMG = false;
			}
			if (overShotgun(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
			{
				if (!mOverShotgun)
				{
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mouseOverBtnSnd);
					mOverShotgun = true;
				}
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectOver[1]);
			}
			else
			{
				mOverShotgun = false;
			}
			if (overRifle(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
			{
				if (!mOverRifle)
				{
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mouseOverBtnSnd);
					mOverRifle = true;
				}
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectOver[2]);
			}
			else
			{
				mOverRifle = false;
			}
			if (overSpread(vector.x - (float)mWeaponSelectFrame.width * 0.5f, (float)Screen.height - vector.y - (float)mWeaponSelectFrame.height))
			{
				if (!mOverSpread)
				{
					player.GetComponent<AudioSource>().PlayOneShot(gamePlay.mouseOverBtnSnd);
					mOverSpread = true;
				}
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectOver[3]);
			}
			else
			{
				mOverSpread = false;
			}
			switch (player.mMySpecialWeapon)
			{
			case 2:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectCrown[2]);
				break;
			case 3:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectCrown[1]);
				break;
			case 5:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectCrown[3]);
				break;
			case 4:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectCrown[0]);
				break;
			}
			switch (player.previousWeaponNotBlaster)
			{
			case 2:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectArrow[2]);
				break;
			case 3:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectArrow[1]);
				break;
			case 5:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectArrow[3]);
				break;
			case 4:
				GUI.DrawTexture(new Rect(0f, 0f, 343f, 178f), mWeaponSelectArrow[0]);
				break;
			}
			GUI.EndGroup();
		}
		DrawBannerMessage();
		float width;
		if (player.mAmSniping)
		{
			GUI.DrawTexture(new Rect(leftEdge, topEdge, rightEdge - leftEdge, bottomEdge - topEdge), mSniperScreen);
		}
		else
		{
			GUI.BeginGroup(new Rect(rightEdge - (float)frame_2_bars.width - (float)atlas_icon.width - 10f - hud_offset, topEdge + hud_offset, 250f, 250f));
			if (GameData.BattleType == 2)
			{
				if (player.mFaction == 1)
				{
					GUI.DrawTexture(new Rect(0f, 0f, 20f, 14f), banzai_icon);
					GUI.DrawTexture(new Rect(3f, 20f, 16f, 16f), atlas_icon);
				}
				else
				{
					GUI.DrawTexture(new Rect(3f, 0f, 16f, 16f), atlas_icon);
					GUI.DrawTexture(new Rect(0f, 20f, 20f, 14f), banzai_icon);
				}
			}
			GUI.DrawTexture(new Rect(234f, 46f, -209f, -43f), frame_2_bars);
			GUI.color = fill_color_white;
			int num3 = 0;
			int num4 = gamePlay.mNumBanzaiCapturesLeft;
			int num5 = gamePlay.mNumAtlasCapturesLeft;
			string text = string.Empty;
			for (int j = 0; j < 8; j++)
			{
				GameObject gameObject2 = gamePlay.players[j];
				if (gameObject2 == null)
				{
					continue;
				}
				Player player2 = gameObject2.GetComponent("Player") as Player;
				if (!GameData.isTeamBattle())
				{
					if (player2.mIsLocal)
					{
						continue;
					}
				}
				else
				{
					PlayerScore playerScore = (PlayerScore)PlayerScores[player2.name];
					if (playerScore == null)
					{
						PlayerScores[player2.name] = new PlayerScore(player2.mFaction, player2.mNumCaptures);
					}
					else
					{
						playerScore.Count = player2.mNumCaptures;
						PlayerScores[player2.name] = playerScore;
					}
				}
				if (player2.mNumCaptures > num3)
				{
					num3 = player2.mNumCaptures;
					text = GameData.trimDisplayName(player2.mScreenName);
				}
			}
			foreach (PlayerScore value in PlayerScores.Values)
			{
				if (value.Faction == 1)
				{
					num4 += value.Count;
				}
				else
				{
					num5 += value.Count;
				}
			}
			float num6 = (float)player.mNumCaptures / (float)GameData.CaptureLimit;
			score1 = player.mNumCaptures;
			if (GameData.isTeamBattle())
			{
				num6 = ((player.mFaction != 1) ? ((float)num5 / (float)GameData.CaptureLimit) : ((float)num4 / (float)GameData.CaptureLimit));
			}
			width = 147f * num6;
			GUI.BeginGroup(new Rect(25f, 4f, width, 17f));
			GUI.DrawTexture(new Rect(0f, 0f, 147f, 17f), fill_bar_captures_me);
			GUI.EndGroup();
			if (!GameData.isTeamBattle())
			{
				GUI.color = Color.white;
				mNumAmmoLeftStyle.fontSize = 14;
				string text2 = GameData.trimDisplayName(player.mScreenName);
				Vector2 vector2 = mNumAmmoLeftStyle.CalcSize(new GUIContent(text2));
				GUI.Label(new Rect(100f - vector2.x * 0.5f, 6f, vector2.x, 12f), text2, mNumAmmoLeftStyle);
				GUI.color = fill_color_white;
			}
			if (GameData.isTeamBattle())
			{
				if (player.mFaction == 1)
				{
					num6 = (float)num5 / (float)GameData.CaptureLimit;
					score1 = num4;
					score2 = num5;
				}
				else
				{
					num6 = (float)num4 / (float)GameData.CaptureLimit;
					score2 = num4;
					score1 = num5;
				}
			}
			else
			{
				num6 = (float)num3 / (float)GameData.CaptureLimit;
				score2 = num3;
			}
			width = 147f * num6;
			GUI.BeginGroup(new Rect(25f, 22f, width, 36f));
			GUI.DrawTexture(new Rect(0f, 0f, 147f, 17f), fill_bar_captures_next);
			GUI.EndGroup();
			GUI.color = Color.white;
			if (!GameData.isTeamBattle())
			{
				mNumAmmoLeftStyle.fontSize = 14;
				Vector2 vector3 = mNumAmmoLeftStyle.CalcSize(new GUIContent(text));
				GUI.Label(new Rect(100f - vector3.x * 0.5f, 25f, vector3.x, 12f), text, mNumAmmoLeftStyle);
			}
			mNumAmmoLeftStyle.fontSize = 24;
			GUI.Label(new Rect(200f, 15f, 100f, 30f), GameData.CaptureLimit.ToString(), mNumAmmoLeftStyle);
			mNumAmmoLeftStyle.fontSize = 10;
			if (GameData.BattleType == 1)
			{
				GUI.Label(new Rect(180f, 7f, 100f, 30f), player.mNumCaptures.ToString(), mNumAmmoLeftStyle);
				GUI.Label(new Rect(180f, 26f, 100f, 30f), num3.ToString(), mNumAmmoLeftStyle);
			}
			else if (player.mFaction == 1)
			{
				GUI.Label(new Rect(180f, 7f, 100f, 30f), num4.ToString(), mNumAmmoLeftStyle);
				GUI.Label(new Rect(180f, 26f, 100f, 30f), num5.ToString(), mNumAmmoLeftStyle);
			}
			else
			{
				GUI.Label(new Rect(180f, 7f, 100f, 30f), num5.ToString(), mNumAmmoLeftStyle);
				GUI.Label(new Rect(180f, 26f, 100f, 30f), num4.ToString(), mNumAmmoLeftStyle);
			}
			int num7 = Mathf.FloorToInt(gamePlay.mGameTimer);
			int num8 = num7 % 60;
			int num9 = num7 / 60;
			string str = string.Format("{0:D2}", num9);
			string str2 = string.Format("{0:D2}", num8);
			GUI.Label(new Rect(208f, 50f, 80f, 30f), str + ":" + str2, mNumAmmoLeftStyle);
			GUI.EndGroup();
		}
		GUI.BeginGroup(new Rect(leftEdge + hud_offset, bottomEdge - (float)frame_2_bars.height - hud_offset - (float)banzai_logo.height + 5f, 210f, 100f));
		float num10 = currentHealth / maxHealth;
		if (currentFuel < 0f)
		{
			currentFuel = 0f;
		}
		float num11 = currentFuel / maxFuel;
		GUI.DrawTexture(frame_2_bars_RECT, frame_2_bars);
		if (player.mFaction == 1)
		{
			GUI.DrawTexture(banzai_logo_RECT, banzai_logo);
		}
		else
		{
			GUI.DrawTexture(banzai_logo_RECT, atlas_logo);
		}
		GUI.DrawTexture(health_icon_RECT, health_icon);
		GUI.DrawTexture(jetpack_icon_RECT, jetpack_icon);
		GUI.color = Color.red;
		GUI.BeginGroup(new Rect(62f, 48f, 147f * num10, 17f));
		GUI.DrawTexture(fill_bar_health_RECT, fill_bar_health);
		GUI.EndGroup();
		GUI.color = Color.blue;
		GUI.BeginGroup(new Rect(62f, 66f, 147f * num11, 17f));
		GUI.DrawTexture(fill_bar_jetpack_RECT, fill_bar_jetpack);
		GUI.EndGroup();
		GUI.color = Color.white;
		if (Application.isEditor)
		{
			GUI.Label(new Rect(92f, 48f, 147f, 18f), currentHealth.ToString());
		}
		if (avatarTexture != null)
		{
			GUI.DrawTexture(new Rect(1f, (float)avatarTexture.height - 2f, avatarTexture.width, avatarTexture.height), avatarTexture);
		}
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(rightEdge - (float)frame_1_bar.width - 28f - hud_offset, bottomEdge - (float)frame_1_bar.height - hud_offset - 28f, 200f, 200f));
		GUI.DrawTexture(new Rect(0f, 10f, 156f, 43f), frame_1_bar);
		GUI.DrawTexture(new Rect(40f, 0f, frame_grenade.width, frame_grenade.height), frame_grenade);
		GUI.DrawTexture(new Rect(58f, 0f, frame_grenade.width, frame_grenade.height), frame_grenade);
		GUI.DrawTexture(new Rect(76f, 0f, frame_grenade.width, frame_grenade.height), frame_grenade);
		int totalBullets = myWeapon.totalBullets;
		int num12 = myWeapon.numInClip;
		if (num12 < 0)
		{
			Logger.trace("<< was going to show: " + num12);
			num12 = 0;
		}
		mNumAmmoLeftStyle.fontSize = 32;
		GUI.Label(new Rect(20f, 27f, 150f, 30f), num12.ToString(), mNumAmmoLeftStyle);
		Vector2 vector4 = mNumAmmoLeftStyle.CalcSize(new GUIContent(totalBullets.ToString()));
		if (player.weaponIdx > 1)
		{
			mNumAmmoLeftStyle.fontSize = 16;
			GUI.Label(new Rect(20f + vector4.x, 37f, 150f, 30f), " / " + totalBullets.ToString(), mNumAmmoLeftStyle);
		}
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(112f, 13f, mWeaponImage[0].width, mWeaponImage[0].height), mWeaponImage[player.weaponIdx - 1]);
		GUI.color = fill_color_white;
		if (player.mNumGrenades > 2)
		{
			GUI.DrawTexture(new Rect(40f, 0f, fill_grenade.width, fill_grenade.height), fill_grenade);
		}
		if (player.mNumGrenades > 1)
		{
			GUI.DrawTexture(new Rect(58f, 0f, fill_grenade.width, fill_grenade.height), fill_grenade);
		}
		if (player.mNumGrenades > 0)
		{
			GUI.DrawTexture(new Rect(76f, 0f, fill_grenade.width, fill_grenade.height), fill_grenade);
		}
		float num13 = (float)myWeapon.numInClip / (float)myWeapon.clipSize;
		if (player.weaponIdx == 1)
		{
			num13 = 1f;
		}
		width = (float)fill_bar_ammo.width * num13;
		float num14 = (float)fill_bar_ammo.width - width;
		GUI.BeginGroup(new Rect(0f + num14, 23f, width, fill_bar_ammo.height));
		GUI.DrawTexture(new Rect(0f - num14, 0f, fill_bar_ammo.width, fill_bar_ammo.height), fill_bar_ammo);
		GUI.EndGroup();
		GUI.EndGroup();
		GUI.BeginGroup(new Rect(rightEdge - 220f, topEdge + 65f, 190f, (float)mMaxMessages * 14f));
		mNumAmmoLeftStyle.fontSize = 12;
		IEnumerator enumerator2 = mMessageQueue.GetEnumerator();
		int num15 = mMessageQueue.Count;
		while (enumerator2.MoveNext())
		{
			string text3 = (string)enumerator2.Current;
			GUI.Label(new Rect(20f, (float)num15 * 14f, 190f, 30f), text3, mNumAmmoLeftStyle);
			num15--;
		}
		GUI.EndGroup();
		if (!Cursor.visible)
		{
			if (mReloading)
			{
				Vector3 mousePosition = Input.mousePosition;
				float left = mousePosition.x - 32f;
				float num16 = Screen.height;
				Vector3 mousePosition2 = Input.mousePosition;
				GUI.DrawTexture(new Rect(left, num16 - mousePosition2.y - 32f, 64f, 64f), mReticleReload[mReticleReloadFrame]);
			}
			else
			{
				Vector3 mousePosition3 = Input.mousePosition;
				float left2 = mousePosition3.x - 32f;
				float num17 = Screen.height;
				Vector3 mousePosition4 = Input.mousePosition;
				GUI.DrawTexture(new Rect(left2, num17 - mousePosition4.y - 32f, 64f, 64f), mReticle[player.weaponIdx - 1]);
			}
		}
		for (int k = 0; k < 8; k++)
		{
			GameObject gameObject3 = gamePlay.players[k];
			if (gameObject3 == null)
			{
				continue;
			}
			Player player3 = gameObject3.GetComponent("Player") as Player;
			Vector3 pos = Camera.main.WorldToScreenPoint(player3.transform.position);
			float num18 = (float)Screen.height - pos.y;
			num18 -= 162f * ((bottomEdge - topEdge) / 600f);
			float num19 = 46f * ((bottomEdge - topEdge) / 600f);
			string text4 = GameData.trimDisplayName(player3.mScreenName);
			vector4 = mNumAmmoLeftStyle.CalcSize(new GUIContent(text4));
			float num20 = mPickupTimer[0].width;
			if (vector4.x > num20)
			{
				num20 = vector4.x;
			}
			float num21 = num20 * 0.5f;
			GUI.BeginGroup(new Rect(pos.x - num21, num18, num20, (float)mPickupTimer[0].height + 30f * ((bottomEdge - topEdge) / 600f)));
			if (player3.mIsLocal)
			{
				if (player3.pickupTimerCurrent > 0f && player3.pickupTimerCurrent <= player3.pickupTimerMax)
				{
					float num22 = 1f - player3.pickupTimerCurrent / player3.pickupTimerMax;
					int num23 = (int)(19f * num22);
					if (num23 < 19)
					{
						GUI.DrawTexture(new Rect(num21 - (float)mPickupTimer[num23].width * 0.5f, num19 - (float)mPickupTimer[num23].height - 2f, mPickupTimer[num23].width, mPickupTimer[num23].height), mPickupTimer[num23]);
					}
				}
				else if (player3.mTryingToPickupWeaponTimer > 2f)
				{
					GUI.DrawTexture(new Rect(num21 - (float)mHeavyWeaponUnavailableTexture.width * 0.5f, num19 - (float)mHeavyWeaponUnavailableTexture.height - 2f, mHeavyWeaponUnavailableTexture.width, mHeavyWeaponUnavailableTexture.height), mHeavyWeaponUnavailableTexture);
				}
			}
			mNumAmmoLeftStyle.fontSize = 12;
			if ((!GameData.isTeamBattle() && !player3.mAmInvisible) || (GameData.isTeamBattle() && player3.mFaction == player.mFaction))
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
				GUI.Label(new Rect(num21 - vector4.x * 0.5f, num19, vector4.x, vector4.y), text4, mNumAmmoLeftStyle);
				GUI.color = color;
			}
			if (player3.tauntTimer > 0f)
			{
				num18 = 20f * ((float)Screen.height / 600f);
				GUI.DrawTexture(new Rect(num21 - (float)mTaunt[player3.tauntNum].width * 0.5f, num18, 64f, 42f), mTaunt[player3.tauntNum]);
			}
			GUI.EndGroup();
			if (!player3.mIsLocal && ((GameData.isTeamBattle() && player3.mFaction == player.mFaction) || player3.mShowArrowTimer > 0f))
			{
				DrawOffscreenArrow(pos, player3.mFaction);
			}
		}
		if (GameData.MyTutorialStep == 3 && (bool)FirstUse.mInstance)
		{
			FirstUse.Frame frame = FirstUse.mInstance.GetFrame();
			if (frame != null && frame.mRectType == FirstUse.Frame.RectType.Game_Point)
			{
				Rect rect = frame.mFocusRect.Calculate();
				DrawOffscreenArrow(Camera.main.WorldToScreenPoint(new Vector3(rect.x, rect.y, 0f)), 0);
			}
		}
		HandleBoostMessage();
	}

	private bool overSMG(float x, float y)
	{
		Vector3 mousePosition = Input.mousePosition;
		float x2 = mousePosition.x;
		float num = Screen.height;
		Vector3 mousePosition2 = Input.mousePosition;
		float num2 = num - mousePosition2.y;
		if (x2 > x + 40f && x2 < x + 76f && num2 > y + 82f && num2 < y + 133f)
		{
			return true;
		}
		if (x2 > x + 77f && x2 < x + 92f && num2 > y + 98f && num2 < y + 123f)
		{
			return true;
		}
		if (x2 > x + 21f && x2 < x + 74f && num2 > y + 126f && num2 < y + 169f)
		{
			return true;
		}
		return false;
	}

	private bool overShotgun(float x, float y)
	{
		Vector3 mousePosition = Input.mousePosition;
		float x2 = mousePosition.x;
		float num = Screen.height;
		Vector3 mousePosition2 = Input.mousePosition;
		float num2 = num - mousePosition2.y;
		if (x2 > x + 76f && x2 < x + 165f && num2 > y + 43f && num2 < y + 72f)
		{
			return true;
		}
		if (x2 > x + 98f && x2 < x + 176f && num2 > y + 30f && num2 < y + 43f)
		{
			return true;
		}
		if (x2 > x + 123f && x2 < x + 165f && num2 > y + 20f && num2 < y + 30f)
		{
			return true;
		}
		if (x2 > x + 86f && x2 < x + 126f && num2 > y + 72f && num2 < y + 92f)
		{
			return true;
		}
		return false;
	}

	private bool overRifle(float x, float y)
	{
		Vector3 mousePosition = Input.mousePosition;
		float x2 = mousePosition.x;
		float num = Screen.height;
		Vector3 mousePosition2 = Input.mousePosition;
		float num2 = num - mousePosition2.y;
		if (x2 > x + 218f && x2 < x + 255f && num2 > y + 72f && num2 < y + 92f)
		{
			return true;
		}
		if (x2 > x + 180f && x2 < x + 256f && num2 > y + 19f && num2 < y + 43f)
		{
			return true;
		}
		if (x2 > x + 180f && x2 < x + 272f && num2 > y + 43f && num2 < y + 72f)
		{
			return true;
		}
		return false;
	}

	private bool overSpread(float x, float y)
	{
		Vector3 mousePosition = Input.mousePosition;
		float x2 = mousePosition.x;
		float num = Screen.height;
		Vector3 mousePosition2 = Input.mousePosition;
		float num2 = num - mousePosition2.y;
		if (x2 > x + 274f && x2 < x + 316f && num2 > y + 42f && num2 < y + 138f)
		{
			return true;
		}
		if (x2 > x + 248f && x2 < x + 327f && num2 > y + 100f && num2 < y + 129f)
		{
			return true;
		}
		if (x2 > x + 274f && x2 < x + 330f && num2 > y + 129f && num2 < y + 168f)
		{
			return true;
		}
		return false;
	}

	private void DrawOffscreenArrow(Vector3 pos, int faction)
	{
		Color color = GUI.color;
		if (GameData.isTeamBattle())
		{
			GUI.color = ((faction != 1) ? new Color(0.59f, 0.6f, 0.32f) : new Color(0.94f, 0.27f, 0.14f));
		}
		else
		{
			GUI.color = Color.white;
		}
		if (pos.y > bottomEdge && pos.x > leftEdge && pos.x < rightEdge)
		{
			GUI.DrawTexture(new Rect(pos.x, topEdge, offscreen_arrow_up.width, offscreen_arrow_down.height), offscreen_arrow_up);
		}
		if (pos.y < 0f && pos.x > leftEdge && pos.x < rightEdge)
		{
			GUI.DrawTexture(new Rect(pos.x, bottomEdge - (float)offscreen_arrow_down.height, offscreen_arrow_down.width, offscreen_arrow_down.height), offscreen_arrow_down);
		}
		pos.y = bottomEdge - pos.y;
		if (pos.x < leftEdge)
		{
			if (pos.y < 0f)
			{
				pos.y = 10f;
			}
			if (pos.y > bottomEdge)
			{
				pos.y = bottomEdge - 10f;
			}
			GUI.DrawTexture(new Rect(leftEdge, pos.y, offscreen_arrow_left.width, offscreen_arrow_left.height), offscreen_arrow_left);
		}
		if (pos.x > rightEdge)
		{
			if (pos.y < 0f)
			{
				pos.y = 10f;
			}
			if (pos.y > bottomEdge)
			{
				pos.y = bottomEdge - 10f;
			}
			GUI.DrawTexture(new Rect(rightEdge - (float)offscreen_arrow_right.width, pos.y, offscreen_arrow_right.width, offscreen_arrow_right.height), offscreen_arrow_right);
		}
		GUI.color = color;
	}

	private void ShowBoostMessage(int num, bool teamBoost)
	{
		boostMessageIcon = mBoostMessageIcon[num];
		if (teamBoost)
		{
			boostMessageText = mBoostTeamMessageText[num];
		}
		else
		{
			boostMessageText = mBoostMessageText[num];
		}
		boostMessageStep = ShowAndHold;
		float x = centerX - (float)(boostMessageIcon.width + boostMessageText.width) * 0.5f;
		mBoostMessageIconPosition = new Vector2(x, 40f);
		mBoostMessageIconGoalPosition = new Vector2(0f, 0f);
		boostMessageTimer = 2f;
		boostColor = new Color(1f, 1f, 1f, 1f);
		Logger.trace("<< show boost message");
	}

	private void HandleBoostMessage()
	{
		if (boostMessageStep != null)
		{
			boostMessageStep();
			float num = centerX - (float)(boostMessageIcon.width + boostMessageText.width) * 0.5f;
			GUI.color = boostColor;
			GUI.DrawTexture(new Rect(mBoostMessageIconPosition.x, mBoostMessageIconPosition.y, boostMessageIcon.width, boostMessageIcon.height), boostMessageIcon);
			GUI.DrawTexture(new Rect(num + (float)boostMessageIcon.width, 40f, boostMessageText.width, boostMessageText.height), boostMessageText);
		}
	}

	private void ShowAndHold()
	{
		boostMessageTimer -= Time.deltaTime;
		if (boostMessageTimer <= 0f)
		{
			boostMessageTimer = 2f;
			boostMessageStep = MoveAndAlpha;
		}
	}

	public void ShowBannerMessage(int messageNum, Vector3 pos)
	{
		mBannerMessageX = pos.x;
		mBannerMessageY = pos.y;
		mBannerMessageFade = false;
		Logger.trace("<< want to show message " + messageNum);
		switch (messageNum)
		{
		case 0:
			mBannerMessage = mChooseWeaponBattleTexture;
			mBannerMessageX -= (float)mBannerMessage.width * 0.5f;
			break;
		case 1:
			mBannerMessage = mChooseWeaponTeamTexture;
			mBannerMessageX -= (float)mBannerMessage.width * 0.5f;
			break;
		case 2:
			mBannerMessage = mMatchEndTexture;
			break;
		case 3:
			mBannerMessage = mMatchStartTexture;
			mBannerMessageX -= (float)mBannerMessage.width * 0.5f;
			mBannerMessageY -= (float)mBannerMessage.height * 0.5f;
			mBannerMessageFade = true;
			break;
		case 4:
			mBannerMessage = mWinnerAtlas;
			break;
		case 5:
			mBannerMessage = mWinnerBanzai;
			break;
		}
		mBannerMessageTimer = 2f;
	}

	private void DrawBannerMessage()
	{
		if (!(mBannerMessageTimer <= 0f))
		{
			float a = 1f;
			if (mBannerMessageTimer < 1f)
			{
				a = mBannerMessageTimer;
			}
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, a);
			GUI.DrawTexture(new Rect(mBannerMessageX, mBannerMessageY, mBannerMessage.width, mBannerMessage.height), mBannerMessage);
			GUI.color = color;
		}
	}

	private void HideBannerMessage()
	{
		mBannerMessageTimer = 0f;
	}

	private void MoveAndAlpha()
	{
		boostMessageTimer -= Time.deltaTime;
		if (boostMessageTimer <= 0f)
		{
			boostMessageStep = null;
		}
		float a = boostMessageTimer / 2f;
		boostColor = new Color(1f, 1f, 1f, a);
		mBoostMessageIconPosition = Vector2.Lerp(mBoostMessageIconPosition, mBoostMessageIconGoalPosition, 2f * Time.deltaTime);
	}
}
