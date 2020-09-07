using Sfs2X.Entities;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
	private const float SNIPE_EDGE = 80f;

	private const float SNIPE_SPEED = 100f;

	private const float INACTIVE_TIME_LIMIT = 300f;

	[NonSerialized]
	public int moveDir = 1;

	[NonSerialized]
	public int rollDir = 1;

	[NonSerialized]
	public float faceDir = 90f;

	[NonSerialized]
	public float faceTargetDir = 90f;

	[NonSerialized]
	public float shotAngle;

	public float grenadeAngle;

	[NonSerialized]
	public Vector3 shotPoint;

	public Vector3 grenadePoint;

	[NonSerialized]
	public Vector3 targetPos;

	private float currentInvisibleAlpha = 0.45f;

	[NonSerialized]
	public float targetInvisibleAlpha = 0.45f;

	[NonSerialized]
	public Vector3 aimingWorldTarget;

	public PlayerStates myState;

	[NonSerialized]
	public bool onGround;

	[NonSerialized]
	public bool canControl = true;

	[NonSerialized]
	public PlayerJump jump = new PlayerJump();

	[NonSerialized]
	public Control playerControl;

	public bool bDisableControl;

	[NonSerialized]
	public CharacterController controller;

	public GameObject pickingUp;

	[NonSerialized]
	public int previousState;

	public float armAngle;

	public int currentPowerup = -1;

	[NonSerialized]
	public GameObject weapon;

	public int mSuitId;

	[NonSerialized]
	public GameObject namelabel;

	public int mMyID;

	public int[] pickups = new int[3]
	{
		-1,
		-1,
		-1
	};

	[NonSerialized]
	public float blendTime = 0.0833333358f;

	[NonSerialized]
	public float faceTargetSpeed = 1800f;

	private float pickupTime;

	public float pickupEffectTimer;

	public float pickupTeamEffectTimer;

	private GameObject invincibleEffect;

	public int mPickupActive = -1;

	public int mPickupTeamActive = -1;

	public float speedRunBase = 32f;

	public float speedRunAdj;

	public float speedJetpackBase = 54f;

	public float speedJetpackAdj;

	[NonSerialized]
	public float runSpeed = 40f;

	public float rollSpeed = 64f;

	[NonSerialized]
	public float jetpackSpeed = 40f;

	public float jetpackVerticalSpeed = 28f;

	public float jetpackAccelGround = 20f;

	public float jetpackAccelAir = 12f;

	public float jetpackAccel = 20f;

	public float jetpackAccelSpeed = 5f;

	public float jetpackAirHeight = 10f;

	public float airdashSpeed = 120f;

	public float airdashSpeedMax = 100f;

	public float airdashDistanceMax = 30f;

	public float airdashSlowRate = 0.5f;

	public float airdashSpeedDone = 40f;

	public float airdashJetpackCost = 25f;

	public float airdashJetpackPenalty = 0.1f;

	public float grenadeVelocity = 200f;

	public float armorSuitAdj;

	public float armorPowerAdj;

	public float armorPickupAdj;

	public float armorPickupTeamAdj;

	public float damageSuitAdj;

	public float damagePowerAdj;

	public float damagePickupAdj;

	public float damagePickupTeamAdj;

	public float speedSuitAdj;

	public float speedPowerAdj;

	public float speedPickupAdj;

	public float speedPickupTeamAdj;

	public float mShotChargeTimer;

	public Material[] myArmor;

	public Material mySuitMat;

	public Material myWeaponTexture;

	public Material invisibleMat;

	public bool mAmReady;

	public float mTimeToFree;

	public float mTimeToPickupBoost;

	public float mTimeToPickupWeapon;

	public float mSpecialWeaponCooldown;

	public float mSpecialWeaponCooldownTimer;

	public bool mTryingToPickupWeapon;

	public float mTryingToPickupWeaponTimer;

	public string mScreenName;

	public GameObject mLowHealthEffect;

	public int mFaction;

	public bool mIsLocal;

	public float tauntTimer;

	public int tauntNum;

	public int myLayer;

	public float mInactiveCounter;

	private Texture2D mInvincibleMessage;

	private float mTimeForInvincibleHelp;

	private float arrowTimer;

	[NonSerialized]
	public float airdashDistanceCurrent;

	public float airdashRecoveryMax;

	[NonSerialized]
	public float airdashRecoveryCurrent;

	public float gravity = 100f;

	public float maxFallSpeed = 180f;

	public float speedSmoothing = 5f;

	[NonSerialized]
	public Vector3 direction = Vector3.zero;

	public float verticalSpeed;

	public float verticalSpeedMax = 75f;

	[NonSerialized]
	public float speed;

	private float lastSpeed;

	[NonSerialized]
	public bool isMoving;

	[NonSerialized]
	public CollisionFlags collisionFlags;

	[NonSerialized]
	public Vector3 velocity = default(Vector3);

	public float fuelMax;

	public float fuelCurrent;

	[NonSerialized]
	public float speedCurrent;

	public float fuelRegenDelayMax = 3f;

	public float fuelUseRate = 20f;

	public float fuelRegenDelayCurrent;

	public float fuelRegenSpeed;

	public float mFuelConsumed;

	[NonSerialized]
	public float healthMax;

	[NonSerialized]
	public float healthCurrent;

	public float healthRegenDelayMax = 4f;

	[NonSerialized]
	public float healthRegenDelayCurrent;

	public float healthRegenSpeed = 20f;

	public float captTimeMax;

	[NonSerialized]
	public float captureTimeCurrent;

	public float invincibleTimeMax;

	public float invincibleByFreeTimeMax;

	[NonSerialized]
	public float invincibleTimeCurrent = 5f;

	public float rollDistanceMax;

	public float speedRollMax;

	public float rollRecoveryMax = 0.5f;

	[NonSerialized]
	public float rollRecoveryCurrent;

	public float jumpVelocity;

	public float invisibleTimeMax;

	public float invisibleTimeCurrent;

	public float pickupTimerMax = 5f;

	[NonSerialized]
	public float pickupTimerCurrent;

	[NonSerialized]
	public float pickupTimerAdjust;

	public Vector3 damagePoint;

	public int myIdx;

	public int mySuitIdx;

	public int mySuitTextureIdx;

	public int runIdx;

	public int rollIdx;

	public int rollRecoverIdx;

	public int idleIdx;

	public int crouchIdx;

	public int airdashIdx;

	public int dashbackwardIdx;

	public int dashrecoverIdx;

	public int dashbackwardrecoverIdx;

	public int runBackwardIdx;

	public int captureFloatIdx;

	public int captureGroundIdx;

	public int jetpackIdx;

	public int jumpbeginIdx;

	public int jumpAtApexIdx;

	public int jumpLandIdx;

	public int aimIdx;

	private Vector3 collisionNormal;

	public float mShotDistance;

	private AnimationEvent ae;

	public Vector3 grenadePos;

	public AudioSource mPlayerAudioSource;

	public AudioSource wpnSnd;

	public AudioSource mJetPackLowSource;

	public float grenadeThrowDelay;

	public float grenadeThrowTimer;

	public float previousArmAngle;

	public float armTimer;

	public float mShowArrowTimer;

	public bool mCanShoot;

	public AnimationState anim_crouch;

	public AnimationState anim_airdash_f;

	public AnimationState anim_airdash_b;

	public AnimationState anim_airdash_rec_f;

	public AnimationState anim_airdash_rec_b;

	public AnimationState anim_idle;

	public AnimationState anim_run_f;

	public AnimationState anim_run_b;

	public AnimationState anim_capture_air;

	public AnimationState anim_capture_ground;

	public AnimationState anim_roll;

	public AnimationState anim_roll_rec;

	public AnimationState anim_jetpack;

	public AnimationState anim_jump;

	public AnimationState anim_jump_fall;

	public AnimationState anim_jump_land;

	public AnimationState anim_shoot;

	public AnimationState anim_left_arm_run;

	public GameObject arrow;

	public GamePlay gs;

	public HandleDamageRing handleDamageRing;

	private ContextualHelp mContextualHelp;

	public int weaponIdx = -1;

	public int mNumCaptures;

	public float[] cooldownTime = new float[3];

	public float[] currentCooldownTime = new float[3];

	public GameObject[] emitters = new GameObject[3];

	public int mShootingPower;

	public int mNumGrenades = 3;

	public int mMaxGrenades = 3;

	public int mNumGrenadesThrown;

	public bool mRadarOn;

	public bool mAmSilenced;

	public bool mAmTurreted;

	public bool mAmVampireDashing;

	public bool mAmDashDamage;

	public bool mShotsOverheat;

	public bool mLaserShot;

	public bool mInfiniteAmmo;

	public bool mAmInvincible;

	public bool mAmInvisible;

	public bool mRegenGrenade;

	public float mTimeToRegenGrenade;

	public float mCantMoveTimer;

	public float mSlowTimer;

	public int mActivePickup;

	public int mActivePickupIdx;

	public bool mAmFakeCaptured;

	public bool mAmCaptured;

	public bool mAmSniping;

	public Vector3 actualMovement;

	private float invisibleOffset;

	public GameObject me;

	private bool mHalted;

	public int mMySpecialWeapon;

	[NonSerialized]
	public int previousWeaponNotBlaster;

	public GameObject captureBubble;

	public LineRenderer mRangeLine;

	public Vector3 mRangeStart;

	public Vector3 mRangeEnd;

	public TrailRenderer speedline1;

	public TrailRenderer speedline2;

	private Transform grimCloudPosition;

	private GameObject grimCloudEffect;

	private Transform invinciblePosition;

	public void setupAnims()
	{
		anim_crouch = base.GetComponent<Animation>()["crouch_" + crouchIdx];
		anim_airdash_f = base.GetComponent<Animation>()["airdash_" + airdashIdx];
		anim_airdash_b = base.GetComponent<Animation>()["dash_backwards_" + dashbackwardIdx];
		anim_airdash_rec_f = base.GetComponent<Animation>()["airdash_recover_" + dashrecoverIdx];
		anim_airdash_rec_b = base.GetComponent<Animation>()["dash_backwards_rec_" + dashbackwardrecoverIdx];
		anim_idle = base.GetComponent<Animation>()["idle_" + idleIdx];
		anim_run_f = base.GetComponent<Animation>()["running_" + runIdx];
		anim_run_b = base.GetComponent<Animation>()["backwardrun_" + runBackwardIdx];
		anim_capture_air = base.GetComponent<Animation>()["capture_float_" + captureFloatIdx];
		anim_capture_ground = base.GetComponent<Animation>()["capture_ground_" + captureGroundIdx];
		anim_roll = base.GetComponent<Animation>()["roll_" + rollIdx];
		anim_roll_rec = base.GetComponent<Animation>()["roll_recover_" + rollRecoverIdx];
		anim_jetpack = base.GetComponent<Animation>()["jetpackhover_" + jetpackIdx];
		anim_jump = base.GetComponent<Animation>()["jump_" + jumpbeginIdx];
		anim_jump_fall = base.GetComponent<Animation>()["jump_at_apex_" + jumpAtApexIdx];
		anim_jump_land = base.GetComponent<Animation>()["jump_land_" + jumpLandIdx];
		anim_shoot = base.GetComponent<Animation>()["new_aim_" + aimIdx];
		anim_left_arm_run = base.GetComponent<Animation>()["move_arm_" + runIdx];
		anim_crouch.wrapMode = WrapMode.Once;
		anim_crouch.speed = 1f;
		anim_crouch.layer = 1;
		anim_airdash_f.wrapMode = WrapMode.Once;
		anim_airdash_f.layer = 1;
		anim_airdash_b.wrapMode = WrapMode.Once;
		anim_airdash_b.layer = 1;
		anim_airdash_rec_f.wrapMode = WrapMode.Once;
		anim_airdash_rec_f.layer = 1;
		anim_airdash_rec_f.speed = 1f;
		anim_airdash_rec_b.wrapMode = WrapMode.Once;
		anim_airdash_rec_b.layer = 1;
		anim_airdash_rec_b.speed = 1f;
		anim_idle.wrapMode = WrapMode.Loop;
		anim_idle.layer = 1;
		anim_idle.weight = 0.1f;
		anim_idle.blendMode = AnimationBlendMode.Blend;
		base.GetComponent<Animation>().CrossFade(anim_idle.name);
		anim_run_f.wrapMode = WrapMode.Loop;
		anim_run_f.layer = 1;
		anim_run_b.wrapMode = WrapMode.Loop;
		anim_run_b.layer = 1;
		anim_capture_air.wrapMode = WrapMode.Once;
		anim_capture_air.layer = 1;
		anim_capture_ground.wrapMode = WrapMode.Once;
		anim_capture_ground.layer = 1;
		anim_roll.wrapMode = WrapMode.Once;
		anim_roll.speed = 1.2f;
		anim_roll.layer = 1;
		anim_roll_rec.wrapMode = WrapMode.Once;
		anim_roll_rec.layer = 1;
		anim_jetpack.wrapMode = WrapMode.Once;
		anim_jetpack.layer = 1;
		anim_jump.wrapMode = WrapMode.Once;
		anim_jump.layer = 1;
		anim_jump_fall.wrapMode = WrapMode.Once;
		anim_jump_fall.layer = 1;
		anim_jump_land.wrapMode = WrapMode.Once;
		anim_jump_land.layer = 1;
		Transform mix = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle");
		anim_left_arm_run.wrapMode = WrapMode.Loop;
		anim_left_arm_run.layer = 3;
		anim_left_arm_run.blendMode = AnimationBlendMode.Blend;
		anim_left_arm_run.AddMixingTransform(mix);
		anim_left_arm_run.weight = 1f;
		grimCloudPosition = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot");
		grimCloudEffect = null;
		if (mySuitIdx == 38)
		{
			Vector3 position = grimCloudPosition.position;
			position.z = 2f;
			grimCloudEffect = (UnityEngine.Object.Instantiate(Resources.Load("effects/grimSuitEmitterPrefab"), position, Quaternion.identity) as GameObject);
			grimCloudEffect.GetComponent<ParticleEmitter>().emit = true;
		}
		Logger.trace("<<  ***** run clip length: " + anim_run_f.clip.length);
		ae = new AnimationEvent();
		ae.time = anim_run_f.clip.length * 0.2f;
		ae.functionName = "PlayRightFootSoundEvent";
		anim_run_f.clip.AddEvent(ae);
		ae = new AnimationEvent();
		ae.time = anim_run_f.clip.length * 0.5f;
		ae.functionName = "PlayLeftFootSoundEvent";
		anim_run_f.clip.AddEvent(ae);
		ae = new AnimationEvent();
		ae.time = anim_run_b.clip.length * 0f;
		ae.functionName = "PlayRightFootSoundEvent";
		anim_run_b.clip.AddEvent(ae);
		ae = new AnimationEvent();
		ae.time = anim_run_b.clip.length * 0.3f;
		ae.functionName = "PlayLeftFootSoundEvent";
		anim_run_b.clip.AddEvent(ae);
		Transform mix2 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
		anim_shoot.wrapMode = WrapMode.Once;
		anim_shoot.layer = 2;
		anim_shoot.blendMode = AnimationBlendMode.Blend;
		anim_shoot.enabled = false;
		anim_shoot.weight = 1f;
		anim_shoot.AddMixingTransform(mix2);
		anim_shoot.normalizedSpeed = 0f;
	}

	public void SetSuit(int suitID)
	{
		Exosuit exosuit = GameData.getExosuit(mySuitIdx);
		WeaponModData weaponModData = GameData.getWeaponModData(exosuit.mWeaponModIndex);
		mMySpecialWeapon = weaponModData.mWeaponIdx;
		verticalSpeed = 0f;
		speed = 0f;
		pickingUp = null;
		mPickupActive = -1;
		mPickupTeamActive = -1;
		healthMax = exosuit.Health;
		if (healthCurrent == 0f)
		{
			healthCurrent = healthMax;
		}
		healthRegenSpeed = exosuit.Regen_Speed;
		healthRegenDelayCurrent = (healthRegenDelayMax = exosuit.Regen_Delay);
		fuelCurrent = (fuelMax = exosuit.Fuel_Max);
		fuelRegenDelayCurrent = (fuelRegenDelayMax = exosuit.Fuel_Delay);
		fuelRegenSpeed = exosuit.Fuel_Regen;
		jetpackVerticalSpeed = exosuit.Jetpack_Vrt_Spd;
		speedJetpackBase += exosuit.Jetpack_Spd_Adj;
		speedRunBase += exosuit.Run_Spd_Adj;
		rollSpeed = exosuit.Roll_Spd;
		rollRecoveryMax = exosuit.Roll_Rcvy;
		airdashSpeedMax = exosuit.Airdash_Spd_Max;
		airdashDistanceMax = exosuit.Airdash_Dst_Max;
		airdashSlowRate = exosuit.Airdash_Sbd_Rt;
		jetpackAccelAir = exosuit.Acc_Air;
		jetpackAccelGround = exosuit.Acc_Grnd;
		mTimeToFree = exosuit.Timer_Free;
		mTimeToPickupBoost = exosuit.Timer_Boosts;
		mTimeToPickupWeapon = exosuit.Timer_Sp_Weps;
		mSpecialWeaponCooldown = exosuit.CoolDown_Sp_Weps;
	}

	private void Start()
	{
		direction = base.transform.TransformDirection(Vector3.forward);
		controller = (GetComponent("CharacterController") as CharacterController);
		controller.height = 10f;
		CharacterController characterController = controller;
		Vector3 center = controller.center;
		float x = center.x;
		Vector3 center2 = controller.center;
		characterController.center = new Vector3(x, 6f, center2.z);
		Logger.trace(":::::::::::: Player Start ::::::::::::::::");
		Logger.trace("<< mSuitId: " + mySuitIdx);
		mIsLocal = ((base.gameObject.name == "localPlayer") ? true : false);
		SetSuit(GameData.MySuitID);
		gs.m_networkManager.setUserVariable("clientVersion", GameData.version);
		if (GameData.getWorldById(0) != null)
		{
			gs.m_networkManager.setUserVariable("myMapId", GameData.WorldID);
		}
		if (mIsLocal)
		{
			GameObject gameObject = new GameObject();
			mRangeLine = (gameObject.AddComponent<LineRenderer>());
			mRangeLine.material = new Material(Shader.Find("Particles/Additive"));
			mRangeLine.enabled = false;
		}
		myState = new PlayerStates(this);
		previousState = 0;
		if (weaponIdx == -1)
		{
			weaponIdx = (previousWeaponNotBlaster = ((mMySpecialWeapon != 1) ? mMySpecialWeapon : 5));
		}
		else
		{
			previousWeaponNotBlaster = 5;
		}
		attachWeapon(weaponIdx);
		captureTimeCurrent = 0f;
		grenadeThrowDelay = 0.75f;
		grenadeThrowTimer = 0f;
		mNumGrenadesThrown = 0;
		armTimer = 3f;
		damageSuitAdj = 1f;
		mRadarOn = false;
		mAmSilenced = false;
		mAmTurreted = false;
		mAmVampireDashing = false;
		mAmDashDamage = false;
		mShotsOverheat = false;
		mLaserShot = false;
		mAmFakeCaptured = false;
		mInfiniteAmmo = false;
		mAmInvincible = false;
		mRegenGrenade = false;
		mTimeToRegenGrenade = 0.5f;
		mAmCaptured = false;
		mAmInvisible = false;
		mAmSniping = false;
		mShootingPower = -1;
		mShotChargeTimer = 0f;
		mFuelConsumed = 0f;
		mTryingToPickupWeapon = false;
		mTryingToPickupWeaponTimer = 0f;
		myArmor = new Material[2];
		GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/armor");
		SkinnedMeshRenderer skinnedMeshRenderer = gameObject2.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
		Logger.trace("GameObject.name: " + base.gameObject.name);
		myArmor = skinnedMeshRenderer.materials;
		mySuitMat = new Material(skinnedMeshRenderer.materials[1]);
		speedSuitAdj = 1f;
		speedPickupAdj = 0f;
		speedPickupTeamAdj = 0f;
		mCantMoveTimer = 0f;
		mActivePickup = -1;
		mActivePickupIdx = -1;
		string[] array = GameData.getPlayerName(mMyID).Split(' ');
		mScreenName = array[0];
		if (array.Length > 1)
		{
			mScreenName = mScreenName + " " + array[1];
		}
		if (gs.mHUD != null)
		{
			gs.mHUD.setMaxFuel(fuelMax);
			gs.mHUD.setMaxHealth(healthMax);
		}
		invincibleTimeMax = 5f;
		tauntTimer = 0f;
		tauntNum = 0;
		mLowHealthEffect = (UnityEngine.Object.Instantiate(gs.lowHealthEffect) as GameObject);
		mLowHealthEffect.name = "low_health_effect";
		mLowHealthEffect.GetComponent<AudioSource>().mute = true;
		Transform transform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
		mLowHealthEffect.transform.parent = transform;
		mLowHealthEffect.transform.position = transform.position;
		wpnSnd = (base.gameObject.AddComponent<AudioSource>());
		wpnSnd.dopplerLevel = 0f;
		wpnSnd.rolloffMode = AudioRolloffMode.Linear;
		wpnSnd.volume = GameData.mGameSettings.mSoundVolume;
		mInactiveCounter = 0f;
		mShowArrowTimer = 0f;
		mHalted = false;
		mTimeForInvincibleHelp = 60f;
		if (mIsLocal)
		{
			mContextualHelp = (base.gameObject.GetComponent("ContextualHelp") as ContextualHelp);
		}
	}

	public void PlayRightFootSoundEvent(AnimationEvent animEvent)
	{
		base.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
		base.GetComponent<AudioSource>().PlayOneShot(gs.footstepSounds[0]);
	}

	public void PlayLeftFootSoundEvent(AnimationEvent animEvent)
	{
		base.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
		base.GetComponent<AudioSource>().PlayOneShot(gs.footstepSounds[1]);
	}

	public void ShowTaunt(int num)
	{
		tauntNum = num;
		tauntTimer = 3f;
	}

	public void ThrowGrenade(Vector3 pos, float ang, int isGun, int num)
	{
		GameObject grenade = gs.grenade;
		GameObject gameObject = null;
		if (!(gs is TutorialGamePlay))
		{
			if (isGun == 1 && GameData.eventObjects.ContainsKey("lobber_grenade"))
			{
				Grenade grenade2 = grenade.GetComponent("Grenade") as Grenade;
				grenade2.shouldAnimate = false;
				MeshFilter meshFilter = grenade.GetComponent("MeshFilter") as MeshFilter;
				MeshFilter meshFilter2 = gs.event_lobber_grenade.GetComponent("MeshFilter") as MeshFilter;
				meshFilter.mesh = meshFilter2.mesh;
				grenade.GetComponent<Renderer>().material = gs.event_lobber_grenade.GetComponent<Renderer>().material;
				if (GameData.eventObjects.ContainsKey("lobber_grenade_emitter"))
				{
					gameObject = (UnityEngine.Object.Instantiate(GameData.eventObjects["lobber_grenade_emitter"] as GameObject) as GameObject);
				}
			}
			else
			{
				if (mFaction == 2 && GameData.eventObjects.ContainsKey("atlas_grenade"))
				{
					Grenade grenade3 = grenade.GetComponent("Grenade") as Grenade;
					grenade3.shouldAnimate = false;
					GameObject gameObject2 = GameData.eventObjects["atlas_grenade"] as GameObject;
					MeshFilter meshFilter3 = grenade.GetComponent("MeshFilter") as MeshFilter;
					MeshFilter meshFilter4 = gameObject2.GetComponent("MeshFilter") as MeshFilter;
					meshFilter3.mesh = meshFilter4.mesh;
					grenade.GetComponent<Renderer>().material = gameObject2.GetComponent<Renderer>().material;
					if (GameData.eventObjects.ContainsKey("atlas_grenade_emitter"))
					{
						gameObject = (UnityEngine.Object.Instantiate(GameData.eventObjects["atlas_grenade_emitter"] as GameObject) as GameObject);
					}
				}
				if (mFaction == 1 && GameData.eventObjects.ContainsKey("banzai_grenade"))
				{
					Grenade grenade4 = grenade.GetComponent("Grenade") as Grenade;
					grenade4.shouldAnimate = false;
					GameObject gameObject3 = GameData.eventObjects["banzai_grenade"] as GameObject;
					MeshFilter meshFilter5 = grenade.GetComponent("MeshFilter") as MeshFilter;
					MeshFilter meshFilter6 = gameObject3.GetComponent("MeshFilter") as MeshFilter;
					meshFilter5.mesh = meshFilter6.mesh;
					grenade.GetComponent<Renderer>().material = gameObject3.GetComponent<Renderer>().material;
					if (GameData.eventObjects.ContainsKey("banzai_grenade_emitter"))
					{
						gameObject = (UnityEngine.Object.Instantiate(GameData.eventObjects["banzai_grenade_emitter"] as GameObject) as GameObject);
					}
				}
			}
		}
		GameObject gameObject4 = UnityEngine.Object.Instantiate(grenade, pos, Quaternion.identity) as GameObject;
		gameObject4.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
		if (gameObject != null)
		{
			Color color = gameObject.GetComponent<Renderer>().material.color;
			Debug.Log("<< emitColor: " + color);
			Material material = gameObject4.GetComponent<TrailRenderer>().material;
			material.SetColor("_TintColor", color);
			gameObject.AddComponent<halloweenGrenadeEmitterAnim>();
			gameObject.transform.parent = gameObject4.transform;
			gameObject.transform.localPosition = Vector3.zero;
		}
		else
		{
			Color color2 = new Color(1f, 0.576f, 0f, 0.39f);
			Material material2 = gameObject4.GetComponent<TrailRenderer>().material;
			material2.SetColor("_TintColor", color2);
		}
		Grenade grenade5 = gameObject4.GetComponent("Grenade") as Grenade;
		grenade5.thrower = myIdx;
		grenade5.throwerID = mMyID;
		grenade5.grenadeNum = num;
		mNumGrenadesThrown++;
		grenade5.Throw();
		if (gameObject4 == null)
		{
			Logger.trace("<< unable to create grenade");
		}
		Physics.IgnoreCollision(gameObject4.GetComponent<Collider>(), GetComponent<Collider>());
		for (int i = 0; i < 8; i++)
		{
			GameObject gameObject5 = gs.players[i];
			if (!(gameObject5 == null))
			{
				Player player = gameObject5.GetComponent("Player") as Player;
				if (!player.mAmReady || player.mAmCaptured || player.mAmInvincible || (GameData.isTeamBattle() && player.mFaction == mFaction))
				{
					Logger.trace("<< ignoring collision with player");
					Physics.IgnoreCollision(gameObject4.GetComponent<Collider>(), gameObject5.GetComponent<Collider>());
				}
			}
		}
		gameObject4.name = "grenade_" + num.ToString();
		gameObject4.tag = "grenade";
		float num2 = (faceTargetDir != 90f) ? (-1f) : 1f;
		float num3 = grenadeVelocity;
		if (isGun == 1)
		{
			num2 = -1f;
			num3 *= 1.5f;
			grenade5.timer = 4f;
			WeaponScript weaponScript = weapon.GetComponent("WeaponScript") as WeaponScript;
			wpnSnd.PlayOneShot(gs.weaponSounds[5]);
			if (mIsLocal)
			{
				weaponScript.spendAmmo(this);
			}
			if (weaponScript.totalBullets <= 0 && mIsLocal)
			{
				SendChangeWeapon(1);
			}
			mShowArrowTimer = 2f;
		}
		else
		{
			grenadeThrowTimer = grenadeThrowDelay;
			mNumGrenades--;
			grenade5.timer = 2.5f;
			wpnSnd.PlayOneShot(gs.weaponSounds[9]);
		}
		float num4 = num3 * Mathf.Sin(ang * ((float)Math.PI / 180f));
		float y = num3 * Mathf.Cos(ang * ((float)Math.PI / 180f));
		gameObject4.GetComponent<Rigidbody>().AddForce(num4 * num2, y, 0f, ForceMode.Impulse);
	}

	public void SetThrowGrenadePos(Vector3 pos)
	{
		grenadePos = pos;
	}

	public void setControl(int localOrNetwork)
	{
		if (localOrNetwork == 0)
		{
			playerControl = new LocalControl(this);
			CameraFocus cameraFocus = Camera.main.GetComponent("CameraFocus") as CameraFocus;
			cameraFocus.targets[0] = base.transform;
			CameraScrolling cameraScrolling = Camera.main.GetComponent("CameraScrolling") as CameraScrolling;
			cameraScrolling.distance = 75f;
			cameraScrolling.SetTarget(cameraFocus.targets[0], snap: true);
		}
		else
		{
			playerControl = null;
			targetInvisibleAlpha = 0f;
		}
	}

	private void Awake()
	{
		direction = base.transform.TransformDirection(Vector3.forward);
		controller = (GetComponent("CharacterController") as CharacterController);
		controller.height = 10f;
		CharacterController characterController = controller;
		Vector3 center = controller.center;
		float x = center.x;
		Vector3 center2 = controller.center;
		characterController.center = new Vector3(x, 6f, center2.z);
		mInvincibleMessage = (Resources.Load("HUD/help/help_bubbles_invincible") as Texture2D);
		gs = GamePlay.GetGamePlayScript();
	}

	private void OnDrawGizmos()
	{
	}

	public void attachWeapon(int idx)
	{
		weaponIdx = idx;
		string text = gs.getWeaponById(weaponIdx).id.ToString();
		GameObject gameObject = null;
		for (int i = 0; i < GameData.Weapons.Count; i++)
		{
			Logger.trace("looking for " + text + " found " + GameData.Weapons[i].name);
			if (GameData.Weapons[i].name == text)
			{
				gameObject = GameData.Weapons[i];
				break;
			}
		}
		if (gameObject != null)
		{
			Logger.trace("<< found weapon: " + gameObject.name);
		}
		else
		{
			Logger.trace("weapons size: " + GameData.Weapons.Count);
		}
		weapon = (UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject);
		weapon.name = "my_weapon";
		GamePlay.SetLayerRecursively(weapon, myLayer);
		WeaponScript weaponScript = weapon.AddComponent<WeaponScript>();
		weaponScript.numProjectiles = gs.getWeaponById(weaponIdx).numProjectiles;
		weaponScript.angleRange = gs.getWeaponById(weaponIdx).angleRange;
		weaponScript.clipSize = gs.getWeaponById(weaponIdx).clipSize;
		weaponScript.numClips = gs.getWeaponById(weaponIdx).numClips;
		weaponScript.fireRate = gs.getWeaponById(weaponIdx).fireRate;
		weaponScript.fireRateAdj = 0f;
		weaponScript.reloadRate = gs.getWeaponById(weaponIdx).reloadRate;
		weaponScript.reloadRateAdj = 0f;
		weaponScript.projectileRange = gs.getWeaponById(weaponIdx).projectileRange;
		weaponScript.projectileAccuracy = gs.getWeaponById(weaponIdx).projectileAccuracy;
		weaponScript.projectileSpeed = 0f;
		weaponScript.damage_per_projectile = gs.getWeaponById(weaponIdx).damage_per_projectile;
		weaponScript.projectileType = gs.getWeaponById(weaponIdx).projectileType;
		weaponScript.usageTime = gs.getWeaponById(weaponIdx).usageTime;
		Exosuit exosuit = GameData.getExosuit(mySuitIdx);
		WeaponModData weaponModData = GameData.getWeaponModData(exosuit.mWeaponModIndex);
		if (weaponModData != null && mMySpecialWeapon == idx)
		{
			Logger.trace("<< change weapon properties because i have preferred weapon ");
			weaponScript.numProjectiles += weaponModData.mModNumProjectiles;
			weaponScript.angleRange += weaponModData.mModAngleRange;
			weaponScript.clipSize += weaponModData.mModClipSize;
			weaponScript.numClips += weaponModData.mModNumClips;
			weaponScript.fireRate += weaponModData.mModFireRate;
			weaponScript.fireRateAdj = weaponScript.fireRateAdj;
			weaponScript.reloadRate += weaponModData.mModReloadRate;
			weaponScript.reloadRateAdj = 0f;
			weaponScript.projectileRange += weaponModData.mModProjectileRange;
			weaponScript.projectileAccuracy += weaponModData.mModProjectileAccuracy;
			weaponScript.projectileSpeed = 0f;
			weaponScript.damage_per_projectile += weaponModData.mModDamagePerProjectile;
			weaponScript.projectileType += weaponModData.mModProjectileType;
		}
		weaponScript.numClips--;
		weaponScript.numInClip = weaponScript.clipSize;
		weaponScript.totalBullets = weaponScript.numInClip + weaponScript.clipSize * weaponScript.numClips;
		Logger.trace("total bullets: " + weaponScript.totalBullets + " , " + weaponScript.numInClip + ", " + weaponScript.clipSize + ", " + weaponScript.numClips);
		weaponScript.p = this;
		string name = base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder";
		GameObject gameObject2 = GameObject.Find(name);
		weapon.transform.position = gameObject2.transform.position;
		weapon.transform.parent = gameObject2.transform;
		weapon.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		if (mAmInvisible)
		{
			SetWeaponInvisible();
		}
		name = base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model/Point01";
		GameObject gameObject3 = GameObject.Find(name);
		gameObject3.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
	}

	public void muzzleflashOn(Vector3 startPt, float shotAngle)
	{
		int index = weaponIdx - 1;
		Logger.trace("weaponIdx: " + weaponIdx);
		GameObject gameObject = UnityEngine.Object.Instantiate(gs.muzzleflash_lite, startPt, Quaternion.identity) as GameObject;
		string name = base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model/Point01";
		GameObject gameObject2 = GameObject.Find(name);
		gameObject.name = "muzzleflash";
		gameObject.transform.parent = gameObject2.transform;
		gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
		wpnSnd.PlayOneShot(gs.weaponSounds[index]);
	}

	private void addPickup(int pickupType)
	{
		int num = 0;
		while (true)
		{
			if (num < 3)
			{
				if (pickups[num] == -1)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		pickups[num] = pickupType;
		Logger.trace("<< adding pickup");
	}

	private void UpdateSmoothedMovementDirection()
	{
		if (myState == null || !mAmReady)
		{
			return;
		}
		float num;
		if (myState.amDoing(32) || myState.amDoing(4096))
		{
			num = Mathf.Min(Mathf.Abs(myState.airdashDir), 1f);
			num = airdashSpeed;
			moveDir = myState.airdashDir;
			speed = Mathf.Lerp(speed, num, 1f);
			direction = new Vector3(myState.airdashDir, 0f, 0f);
			return;
		}
		float num2 = 0f;
		if (myState.amDoing(6) || myState.amDoing(32768) || myState.amDoing(16) || myState.amDoing(8) || myState.amDoing(512))
		{
			num2 = moveDir;
		}
		if (myState.amDoing(128))
		{
			num2 = rollDir;
		}
		if (!canControl)
		{
			num2 = 0f;
		}
		isMoving = (num2 != 0f);
		if (isMoving)
		{
			direction = new Vector3(num2, 0f, 0f);
		}
		if (controller.isGrounded)
		{
			onGround = true;
		}
		else
		{
			Vector3 vector = new Vector3(0f, -5f, 0f);
			Vector3 position = base.transform.position;
			position.y += 2f;
			Ray ray = new Ray(position, vector);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f))
			{
				if (hitInfo.distance < 5.5f)
				{
					onGround = true;
				}
				else
				{
					onGround = false;
				}
			}
			else
			{
				onGround = false;
			}
		}
		runSpeed = speedRunBase;
		runSpeed += speedRunBase * speedPickupAdj;
		runSpeed += speedRunBase * speedPickupTeamAdj;
		jetpackSpeed = speedJetpackBase + speedJetpackAdj;
		num = Mathf.Min(Mathf.Abs(num2), 1f);
		if (onGround)
		{
			if (myState.amDoing(128))
			{
				num *= rollSpeed;
				speed = Mathf.Lerp(speed, num, 1f);
			}
			else
			{
				num *= runSpeed;
				speed = Mathf.Lerp(speed, num, 1f);
			}
			if (num2 == 0f)
			{
				speed = 0f;
			}
			if (speed != 0f && lastSpeed != speed)
			{
				Logger.trace("<< speed: " + speed);
				lastSpeed = speed;
			}
		}
		else
		{
			speed = Mathf.Lerp(myState.amDoing(128) ? (num * rollSpeed) : ((!myState.amDoing(16)) ? (num * runSpeed) : (num * jetpackSpeed)), speed, 1f);
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		collisionNormal = hit.normal;
	}

	private void ApplyGravity()
	{
		if (myState == null)
		{
			Logger.trace("<< myState is null");
			return;
		}
		if (myState.amDoing(32) || myState.amDoing(4096) || myState.amDoing(64) || mCantMoveTimer > 0f)
		{
			verticalSpeed = 0f;
			return;
		}
		if (controller.isGrounded && !myState.amDoing(8))
		{
			verticalSpeed = (0f - gravity) * Time.deltaTime;
		}
		else
		{
			verticalSpeed -= gravity * Time.deltaTime;
		}
		if (myState.amDoing(16) && collisionNormal.y >= 0f)
		{
			jetpackAccel -= jetpackAccelSpeed * Time.deltaTime;
			if (jetpackAccel < jetpackAccelAir)
			{
				jetpackAccel = jetpackAccelAir;
			}
			gs.mHUD.setMaxFuel(fuelMax);
			float num = fuelUseRate * Time.deltaTime;
			mFuelConsumed += num;
			fuelCurrent -= num;
			if (fuelCurrent <= 0f)
			{
				mJetPackLowSource.loop = false;
				mJetPackLowSource.Stop();
				mJetPackLowSource.volume = GameData.mGameSettings.mSoundVolume;
				mJetPackLowSource.PlayOneShot(gs.jetpackFuelEmptySnd);
			}
			else if (fuelCurrent < 30f)
			{
				mJetPackLowSource.volume = GameData.mGameSettings.mSoundVolume;
				mJetPackLowSource.clip = gs.jetpackFuelLowLoop;
				if (!mJetPackLowSource.isPlaying)
				{
					mJetPackLowSource.loop = true;
					mJetPackLowSource.Play();
				}
			}
			else if (mJetPackLowSource.isPlaying)
			{
				mJetPackLowSource.Stop();
			}
			gs.mHUD.setTargetFuel(fuelCurrent);
			verticalSpeed += gravity * Time.deltaTime * jetpackVerticalSpeed / jetpackAccel;
		}
		if (fuelCurrent >= 30f && mJetPackLowSource.isPlaying)
		{
			mJetPackLowSource.Stop();
		}
		if (verticalSpeed < 0f)
		{
			verticalSpeed = Mathf.Max(verticalSpeed, 0f - maxFallSpeed);
		}
		else if (verticalSpeed > verticalSpeedMax)
		{
			verticalSpeed = verticalSpeedMax;
		}
	}

	private void MovePlayer()
	{
		if (myState.amDoing(256) || myState.amDoing(64) || mCantMoveTimer > 0f)
		{
			velocity = Vector3.zero;
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 motion = direction * speed + new Vector3(0f, verticalSpeed, 0f);
		motion *= Time.deltaTime;
		collisionNormal = new Vector3(0f, 0f, 0f);
		collisionFlags = controller.Move(motion);
		actualMovement = base.transform.position - position;
		velocity = (base.transform.position - position) / Time.deltaTime;
		if (onGround)
		{
			velocity *= 0.5f;
			if (jump.jumping)
			{
				jump.jumping = false;
			}
		}
	}

	public void StartJetPack()
	{
		Transform transform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetLeft");
		Transform transform2 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetCenter");
		Transform transform3 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetRight");
		Transform transform4 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetSmoke");
		Transform transform5 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetLite");
		if (!mAmInvisible)
		{
			if (transform != null)
			{
				transform.GetComponent<ParticleEmitter>().emit = true;
			}
			if (transform3 != null)
			{
				transform3.GetComponent<ParticleEmitter>().emit = true;
			}
			if (transform2 != null)
			{
				transform2.GetComponent<ParticleEmitter>().emit = true;
			}
			if (transform5 != null)
			{
				transform5.GetComponent<Light>().enabled = true;
			}
		}
		if ((!mAmInvisible || mIsLocal) && transform4 != null)
		{
			transform4.GetComponent<ParticleEmitter>().emit = true;
		}
		base.GetComponent<AudioSource>().PlayOneShot(gs.jetpackIgnite);
		base.GetComponent<AudioSource>().clip = gs.jetpackLoop;
		base.GetComponent<AudioSource>().loop = true;
		base.GetComponent<AudioSource>().Play();
	}

	public void StopJetPack()
	{
		if (base.GetComponent<AudioSource>().isPlaying)
		{
			fuelRegenDelayCurrent = fuelRegenDelayMax;
			base.GetComponent<AudioSource>().Stop();
		}
		Transform transform = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetLeft");
		Transform transform2 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetCenter");
		Transform transform3 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetRight");
		Transform transform4 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetSmoke");
		Transform transform5 = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/jetpack/jetLite");
		if (transform != null)
		{
			transform.GetComponent<ParticleEmitter>().emit = false;
		}
		if (transform3 != null)
		{
			transform3.GetComponent<ParticleEmitter>().emit = false;
		}
		if (transform2 != null)
		{
			transform2.GetComponent<ParticleEmitter>().emit = false;
		}
		if (transform4 != null)
		{
			transform4.GetComponent<ParticleEmitter>().emit = false;
		}
		if (transform5 != null)
		{
			transform5.GetComponent<Light>().enabled = false;
		}
	}

	public void shoot(Vector3 startPt, float shotAngle, int power)
	{
		if (mIsLocal && weaponIdx == 7 && !mAmSniping)
		{
			mAmSniping = true;
			return;
		}
		WeaponScript weaponScript = weapon.GetComponent("WeaponScript") as WeaponScript;
		if (weaponScript.totalBullets <= 0)
		{
			return;
		}
		if (mIsLocal && mAmSniping)
		{
			Vector3 vector = mRangeEnd - mRangeStart;
			float magnitude = vector.magnitude;
			magnitude += 2f;
			vector.Normalize();
			Vector3 endPos = default(Vector3);
			endPos.x = mRangeStart.x + vector.x * magnitude;
			endPos.y = mRangeStart.y + vector.y * magnitude;
			if (!weaponScript.CanFire())
			{
				return;
			}
			if (gs is TutorialGamePlay)
			{
				gs.DrawSniperLine(mRangeStart.x, mRangeStart.y, mRangeEnd.x, mRangeEnd.y);
			}
			else
			{
				gs.SendSniperLine(mRangeStart, endPos);
			}
			Logger.trace("<< shooting sniper");
			muzzleflashOn(startPt, shotAngle);
			if (mIsLocal)
			{
				weaponScript.spendAmmo(this);
				if (weaponScript.totalBullets <= 0)
				{
					SendChangeWeapon(1);
				}
			}
		}
		else
		{
			int num = weaponScript.Fire(startPt, shotAngle, base.gameObject.name, this, mShotDistance);
			if (gs is TutorialGamePlay && weaponScript.totalBullets <= 0)
			{
				SendChangeWeapon(1);
			}
			if (num != 0)
			{
				Logger.trace("gameobject name: " + base.gameObject.name + "     transform name: " + base.transform.name);
			}
			else
			{
				myState.clearCurrentState(1048576);
			}
			myState.clearCurrentState(1048576);
		}
	}

	public void remoteShoot(Vector3 startPt, float shotAngle, float angleIncrement, int bnum)
	{
		WeaponScript weaponScript = weapon.GetComponent("WeaponScript") as WeaponScript;
		if (!mIsLocal && mAmInvisible)
		{
			currentInvisibleAlpha = 0.6f;
		}
		weaponScript.fireBullet(startPt, shotAngle, angleIncrement, base.gameObject.name, bnum, this);
		if (weaponScript.totalBullets <= 0 && mIsLocal)
		{
			SendChangeWeapon(1);
		}
		mShowArrowTimer = 2f;
		myState.clearCurrentState(1048576);
	}

	public void killBullet(int idx)
	{
	}

	public void changeWeapon(int idx)
	{
		if (weapon != null)
		{
			weapon.transform.parent = null;
			UnityEngine.Object.Destroy(weapon);
		}
		if (mAmSniping && idx != 7)
		{
			mAmSniping = false;
			CameraScrolling cameraScrolling = Camera.main.GetComponent("CameraScrolling") as CameraScrolling;
			cameraScrolling.SetTarget(base.gameObject.transform);
		}
		Logger.trace("<< " + mScreenName + " changing weapon to " + idx);
		attachWeapon(idx);
	}

	private void FixedUpdate()
	{
		if (GameData.CurPlayState != GameData.PlayState.GAME_IS_QUITTING && !mHalted)
		{
			if (gs.m_networkManager != null)
			{
				mScreenName = gs.m_networkManager.getNameByID(mMyID);
			}
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			Vector3 position3 = new Vector3(x, position2.y, 0f);
			base.transform.position = position3;
		}
	}

	private void Update()
	{
		if (mTimeForInvincibleHelp > 0f)
		{
			mTimeForInvincibleHelp -= Time.deltaTime;
			if (mTimeForInvincibleHelp < 0f)
			{
				mTimeForInvincibleHelp = 0f;
			}
		}
		if (GameData.CurPlayState == GameData.PlayState.GAME_IS_QUITTING)
		{
			Logger.traceAlways("[Player::Update] - was going to run");
			return;
		}
		User userByID = gs.m_networkManager.getUserByID(myIdx + 1);
		if (userByID != null && userByID.GetVariable("hacks") != null && userByID.GetVariable("hacks").GetIntValue() > mNumCaptures)
		{
			mNumCaptures = userByID.GetVariable("hacks").GetIntValue();
		}
		if (mHalted || gs.mIsBattleOver)
		{
			return;
		}
		if (mShowArrowTimer > 0f)
		{
			mShowArrowTimer -= Time.deltaTime;
			if (mShowArrowTimer < 0f)
			{
				mShowArrowTimer = 0f;
			}
		}
		if (mTryingToPickupWeapon)
		{
			mTryingToPickupWeaponTimer += Time.deltaTime;
		}
		if (pickupTime > 2f)
		{
			pickupTime -= Time.deltaTime;
			if (!(pickupTime <= 0f))
			{
			}
		}
		if (grenadeThrowTimer > 0f)
		{
			grenadeThrowTimer -= Time.deltaTime;
			if (grenadeThrowTimer < 0f)
			{
				grenadeThrowTimer = 0f;
			}
		}
		if (mAmInvincible)
		{
			if (userByID != null && userByID.GetVariable("avatarState").GetStringValue() == "normal")
			{
				Debug.Log("<< should be normal");
			}
		}
		else if (userByID != null && userByID.GetVariable("hacks") != null)
		{
			mNumCaptures = userByID.GetVariable("hacks").GetIntValue();
		}
		if (mSpecialWeaponCooldownTimer > 0f)
		{
			mSpecialWeaponCooldownTimer -= Time.deltaTime;
			if (mSpecialWeaponCooldownTimer <= 0f)
			{
				mSpecialWeaponCooldownTimer = 0f;
			}
		}
		if (grimCloudEffect != null)
		{
			bool emit = onGround ? true : false;
			if (mAmCaptured || mAmInvisible)
			{
				emit = false;
			}
			if (!userByID.IsItMe)
			{
				Vector3 vector = new Vector3(0f, -5f, 0f);
				Vector3 position = base.transform.position;
				position.y += 2f;
				Ray ray = new Ray(position, vector);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 1000f))
				{
					if (hitInfo.distance < 5.5f)
					{
						onGround = true;
					}
					else
					{
						onGround = false;
					}
				}
				else
				{
					onGround = false;
				}
			}
			grimCloudEffect.GetComponent<ParticleEmitter>().emit = emit;
			Vector3 position2 = grimCloudPosition.position;
			position2.z = -3f;
			grimCloudEffect.transform.position = position2;
		}
		if (tauntTimer > 0f)
		{
			tauntTimer -= Time.deltaTime;
			if ((double)tauntTimer <= 0.0)
			{
				tauntTimer = 0f;
			}
		}
		if (mIsLocal && mAmSniping)
		{
			float num = (float)Screen.width * Camera.main.rect.xMin;
			float num2 = (float)Screen.height * Camera.main.rect.yMin;
			float num3 = (float)Screen.width * Camera.main.rect.xMax;
			float num4 = (float)Screen.height * Camera.main.rect.yMax;
			float num5 = (num + num3) * 0.5f;
			float num6 = (num2 + num4) * 0.5f;
			num = num5 - 80f;
			num3 = num5 + 80f;
			num2 = num6 - 80f;
			num4 = num6 + 80f;
		}
		if (pickupTeamEffectTimer > 0f)
		{
			pickupTeamEffectTimer -= Time.deltaTime;
			if (pickupTeamEffectTimer <= 0f)
			{
				pickupTeamEffectTimer = 0f;
			}
		}
		if (mCantMoveTimer > 0f)
		{
			mCantMoveTimer -= Time.deltaTime;
			if (mCantMoveTimer <= 0f)
			{
				mCantMoveTimer = 0f;
			}
		}
		if (mSlowTimer > 0f)
		{
			mSlowTimer -= Time.deltaTime;
			if (mSlowTimer <= 0f)
			{
				mSlowTimer = 0f;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (currentCooldownTime[i] > 0f)
			{
				currentCooldownTime[i] -= Time.deltaTime;
				if (currentCooldownTime[i] <= 0f)
				{
					currentCooldownTime[i] = 0f;
				}
			}
		}
		if (fuelRegenDelayCurrent > 0f)
		{
			fuelRegenDelayCurrent -= Time.deltaTime;
		}
		if (!myState.amDoing(16) && fuelRegenDelayCurrent <= 0f && fuelCurrent < fuelMax)
		{
			fuelCurrent += fuelRegenSpeed * Time.deltaTime;
			if (mFuelConsumed > 0f)
			{
				gs.SendFuelConsumed((int)mFuelConsumed);
				mFuelConsumed = 0f;
			}
			if (fuelCurrent > fuelMax)
			{
				fuelCurrent = fuelMax;
			}
			if (mIsLocal)
			{
				gs.mHUD.setTargetFuel(fuelCurrent);
			}
		}
		if (healthRegenDelayCurrent > 0f)
		{
			healthRegenDelayCurrent -= Time.deltaTime;
		}
		if (healthRegenDelayCurrent <= 0f && healthCurrent < healthMax)
		{
			healthCurrent += healthRegenSpeed * Time.deltaTime;
			if (healthCurrent >= healthMax)
			{
				healthCurrent = healthMax;
				if (mIsLocal)
				{
					gs.mHUD.setCurrentHealth(healthCurrent);
				}
			}
			if (mIsLocal)
			{
				gs.mHUD.setTargetHealth(healthCurrent);
			}
		}
		if (healthCurrent < 30f && healthCurrent > 0f && !myState.amDoing(64) && !mAmCaptured && !mAmInvincible)
		{
			if (mLowHealthEffect != null)
			{
				mLowHealthEffect.GetComponent<ParticleEmitter>().emit = true;
				mLowHealthEffect.GetComponent<AudioSource>().mute = false;
				mLowHealthEffect.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
			}
		}
		else if (mLowHealthEffect != null)
		{
			mLowHealthEffect.GetComponent<ParticleEmitter>().emit = false;
			mLowHealthEffect.GetComponent<AudioSource>().mute = true;
		}
		HandleInvisible();
		if (mIsLocal)
		{
			handlePickup();
		}
		if (!gs.isPaused && playerControl != null && playerControl.controlType != 1)
		{
			float num7 = armAngle;
			if (playerControl != null)
			{
				playerControl.Update();
			}
			if (bDisableControl)
			{
				armAngle = num7;
				myState.updateCapture();
				myState.setMoveState(0);
				myState.clearCurrentState();
				myState.clearDesiredState();
				mCanShoot = false;
			}
			if (myState != null)
			{
				gs.numFramesSinceMessage++;
				myState.updateState();
			}
			if (mIsLocal)
			{
				UpdateSmoothedMovementDirection();
				ApplyGravity();
				MovePlayer();
			}
		}
	}

	private void CheckCloseToCapturedPlayer()
	{
		for (int i = 0; i < 8; i++)
		{
			if (i != myIdx && !(gs.players[i] == null))
			{
				Player player = gs.players[i].GetComponent("Player") as Player;
				if ((player.mAmCaptured || player.mAmInvincible) && (base.transform.position - gs.players[i].transform.position).magnitude < 30f && mContextualHelp != null)
				{
					mContextualHelp.ShowInvincibleHelp(toShow: true);
				}
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Logger.trace("<< colliding with " + collision.gameObject.tag);
		if (collision.gameObject.tag == "ground")
		{
			Logger.trace("<< colliding with ground ");
			onGround = true;
		}
	}

	private void LateUpdate()
	{
		if (GameData.CurPlayState == GameData.PlayState.GAME_IS_QUITTING)
		{
			Logger.traceAlways("[Player::LateUpdate] - was going to run");
		}
		else
		{
			if (gs.mIsBattleOver)
			{
				return;
			}
			if (captureBubble == null)
			{
				if (faceDir != faceTargetDir)
				{
					if (faceTargetDir == 90f)
					{
						faceDir -= faceTargetSpeed * Time.deltaTime;
						if (faceDir < 90f)
						{
							faceDir = 90f;
						}
					}
					else
					{
						faceDir += faceTargetSpeed * Time.deltaTime;
						if (faceDir > 270f)
						{
							faceDir = 270f;
						}
					}
				}
			}
			else
			{
				float num = 0f;
				float num2 = 3f;
				Transform transform = captureBubble.transform;
				Vector3 position = base.transform.position;
				float x = position.x + num;
				Vector3 position2 = base.transform.position;
				transform.position = new Vector3(x, position2.y + num2, 0f);
			}
			base.transform.rotation = Quaternion.Euler(0f, faceDir, 0f);
			if (mAmReady)
			{
				anim_shoot.enabled = true;
			}
			if ((myState.currentMoveState & 0x80) == 128 || myState.amDoing(64))
			{
				anim_shoot.weight = 0f;
			}
			else
			{
				anim_shoot.weight = 1f;
			}
			if (!myState.amDoing(64))
			{
				anim_shoot.normalizedTime = armAngle;
			}
			if (myState == null)
			{
				return;
			}
			checkInWall();
			if ((myState.currentActionState & 0x100000) == 1048576)
			{
				if (weaponIdx == 6)
				{
					PlayThrow(1);
				}
				else if (!myState.amDoing(64) && gs.waitForWeaponSelectTimer <= 0f && mCanShoot)
				{
					shoot(shotPoint, shotAngle, mShootingPower);
				}
			}
			if (myState.amDoing(2097152))
			{
				PlayThrow(0);
			}
			if (!myState.amDoing(32) && (myState.currentMoveState & 0x10) != 16)
			{
				StopJetPack();
			}
			if (myState.previousMoveState == myState.currentMoveState)
			{
				return;
			}
			anim_left_arm_run.enabled = false;
			if (captureBubble != null)
			{
				return;
			}
			switch (myState.currentMoveState)
			{
			case 0:
				base.GetComponent<Animation>().CrossFade(anim_idle.name, blendTime);
				break;
			case 1:
				base.GetComponent<Animation>().CrossFade(anim_crouch.name, 0f);
				break;
			case 2:
			case 4:
			case 6:
				base.GetComponent<Animation>().CrossFade(anim_run_f.name, blendTime);
				anim_left_arm_run.enabled = true;
				break;
			case 32768:
				base.GetComponent<Animation>().CrossFade(anim_run_b.name, blendTime);
				break;
			case 8:
				base.GetComponent<Animation>().CrossFade(anim_jump.name, blendTime);
				break;
			case 16:
				base.GetComponent<Animation>().CrossFade(anim_jetpack.name, blendTime);
				StartJetPack();
				break;
			case 32:
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.AirDash);
				if ((myState.currentContextState & 0x20000000) == 536870912)
				{
					base.GetComponent<Animation>().CrossFade(anim_airdash_b.name, blendTime);
				}
				else
				{
					base.GetComponent<Animation>().CrossFade(anim_airdash_f.name, blendTime);
				}
				fuelCurrent -= airdashJetpackCost;
				if (mIsLocal)
				{
					gs.mHUD.setTargetFuel(fuelCurrent);
				}
				StartJetPack();
				break;
			case 128:
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.Roll);
				base.GetComponent<Animation>().CrossFade(anim_roll.name, blendTime);
				anim_shoot.enabled = false;
				break;
			case 256:
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.Roll);
				base.GetComponent<Animation>().CrossFade(anim_roll_rec.name, blendTime);
				anim_shoot.enabled = true;
				break;
			case 1024:
				anim_shoot.enabled = true;
				if ((myState.currentContextState & 0x10000000) == 268435456)
				{
					base.GetComponent<Animation>().CrossFade(anim_jump_fall.name, blendTime);
				}
				else
				{
					base.GetComponent<Animation>().CrossFade(anim_idle.name, blendTime);
				}
				if ((bool)captureBubble)
				{
					UnityEngine.Object.Destroy(captureBubble);
					captureBubble = null;
				}
				break;
			case 4096:
				if ((myState.currentContextState & 0x20000000) == 536870912)
				{
					base.GetComponent<Animation>().CrossFade(anim_airdash_rec_b.name, 0f);
				}
				else
				{
					base.GetComponent<Animation>().CrossFade(anim_airdash_rec_f.name, 0f);
				}
				StopJetPack();
				break;
			case 512:
			case 8192:
				base.GetComponent<Animation>().CrossFade(anim_jump_fall.name, blendTime);
				break;
			case 16384:
				base.GetComponent<Animation>().CrossFade(anim_jump_land.name, 0f);
				base.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
				base.GetComponent<AudioSource>().PlayOneShot(gs.footstepSounds[0]);
				break;
			}
			myState.previousMoveState = myState.currentMoveState;
			myState.previousActionState = myState.currentActionState;
		}
	}

	public void checkInWall()
	{
		string name = base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm";
		GameObject gameObject = GameObject.Find(name);
		Vector3 position = gameObject.transform.position;
		Vector3 vector = shotPoint - position;
		Ray ray = new Ray(position, vector);
		mCanShoot = true;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, vector.magnitude * 1f, 9) && hitInfo.collider.gameObject != base.gameObject && !hitInfo.collider.gameObject.CompareTag("pickup") && !hitInfo.collider.gameObject.CompareTag("Player"))
		{
			mCanShoot = false;
		}
	}

	public void setBubbleAnim()
	{
		if (base.GetComponent<Animation>() != null)
		{
			anim_shoot.weight = 0f;
			anim_shoot.enabled = false;
			bool flag = false;
			Vector3 vector = new Vector3(0f, -10f, 0f);
			Vector3 position = base.transform.position;
			position.y += 3f;
			Ray ray = new Ray(position, vector);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 1000f) && hitInfo.distance < 6f)
			{
				flag = true;
			}
			if (flag)
			{
				base.GetComponent<Animation>().CrossFade(anim_capture_ground.name, 0f);
			}
			else
			{
				base.GetComponent<Animation>().CrossFade(anim_capture_air.name, 0f);
			}
			anim_shoot.enabled = false;
		}
		else
		{
			Logger.trace("<< animation is NULL");
		}
	}

	public void spawnBubble(int weaponType, int attackerId)
	{
		string name = "capture_bubble" + myIdx;
		GameObject gameObject = GameObject.Find(name);
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		Logger.trace("[Player::spawnBubble] - spawning guy in bubble " + base.gameObject.name);
		setBubbleAnim();
		float num = 0f;
		float num2 = 3f;
		anim_shoot.enabled = false;
		int factionByID = gs.m_networkManager.getFactionByID(attackerId);
		if (factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_bubble"))
		{
			GameObject event_atlas_bubble = gs.event_atlas_bubble;
			Vector3 position = base.transform.position;
			float x = position.x + num;
			Vector3 position2 = base.transform.position;
			float y = position2.y + num2;
			Vector3 position3 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_bubble, new Vector3(x, y, position3.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 1 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_pistol_bubble"))
		{
			GameObject event_atlas_pistol_bubble = gs.event_atlas_pistol_bubble;
			Vector3 position4 = base.transform.position;
			float x2 = position4.x + num;
			Vector3 position5 = base.transform.position;
			float y2 = position5.y + num2;
			Vector3 position6 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_pistol_bubble, new Vector3(x2, y2, position6.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 2 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_rifle_bubble"))
		{
			GameObject event_atlas_rifle_bubble = gs.event_atlas_rifle_bubble;
			Vector3 position7 = base.transform.position;
			float x3 = position7.x + num;
			Vector3 position8 = base.transform.position;
			float y3 = position8.y + num2;
			Vector3 position9 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_rifle_bubble, new Vector3(x3, y3, position9.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 3 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_shotgun_bubble"))
		{
			GameObject event_atlas_shotgun_bubble = gs.event_atlas_shotgun_bubble;
			Vector3 position10 = base.transform.position;
			float x4 = position10.x + num;
			Vector3 position11 = base.transform.position;
			float y4 = position11.y + num2;
			Vector3 position12 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_shotgun_bubble, new Vector3(x4, y4, position12.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 4 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_smg_bubble"))
		{
			GameObject event_atlas_smg_bubble = gs.event_atlas_smg_bubble;
			Vector3 position13 = base.transform.position;
			float x5 = position13.x + num;
			Vector3 position14 = base.transform.position;
			float y5 = position14.y + num2;
			Vector3 position15 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_smg_bubble, new Vector3(x5, y5, position15.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 5 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_spread_bubble"))
		{
			GameObject event_atlas_spread_bubble = gs.event_atlas_spread_bubble;
			Vector3 position16 = base.transform.position;
			float x6 = position16.x + num;
			Vector3 position17 = base.transform.position;
			float y6 = position17.y + num2;
			Vector3 position18 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_spread_bubble, new Vector3(x6, y6, position18.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 6 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_lobber_bubble"))
		{
			GameObject event_atlas_lobber_bubble = gs.event_atlas_lobber_bubble;
			Vector3 position19 = base.transform.position;
			float x7 = position19.x + num;
			Vector3 position20 = base.transform.position;
			float y7 = position20.y + num2;
			Vector3 position21 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_lobber_bubble, new Vector3(x7, y7, position21.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 7 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_sniper_bubble"))
		{
			GameObject event_atlas_sniper_bubble = gs.event_atlas_sniper_bubble;
			Vector3 position22 = base.transform.position;
			float x8 = position22.x + num;
			Vector3 position23 = base.transform.position;
			float y8 = position23.y + num2;
			Vector3 position24 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_sniper_bubble, new Vector3(x8, y8, position24.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 8 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_rocket_bubble"))
		{
			GameObject event_atlas_rocket_bubble = gs.event_atlas_rocket_bubble;
			Vector3 position25 = base.transform.position;
			float x9 = position25.x + num;
			Vector3 position26 = base.transform.position;
			float y9 = position26.y + num2;
			Vector3 position27 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_rocket_bubble, new Vector3(x9, y9, position27.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 9 && factionByID == 2 && GameData.eventObjects.ContainsKey("atlas_grenade_bubble"))
		{
			GameObject event_atlas_grenade_bubble = gs.event_atlas_grenade_bubble;
			Vector3 position28 = base.transform.position;
			float x10 = position28.x + num;
			Vector3 position29 = base.transform.position;
			float y10 = position29.y + num2;
			Vector3 position30 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_grenade_bubble, new Vector3(x10, y10, position30.z), Quaternion.identity) as GameObject);
		}
		else if (factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_bubble"))
		{
			GameObject event_banzai_bubble = gs.event_banzai_bubble;
			Vector3 position31 = base.transform.position;
			float x11 = position31.x + num;
			Vector3 position32 = base.transform.position;
			float y11 = position32.y + num2;
			Vector3 position33 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_bubble, new Vector3(x11, y11, position33.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 1 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_pistol_bubble"))
		{
			GameObject event_banzai_pistol_bubble = gs.event_banzai_pistol_bubble;
			Vector3 position34 = base.transform.position;
			float x12 = position34.x + num;
			Vector3 position35 = base.transform.position;
			float y12 = position35.y + num2;
			Vector3 position36 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_pistol_bubble, new Vector3(x12, y12, position36.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 2 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_rifle_bubble"))
		{
			GameObject event_banzai_rifle_bubble = gs.event_banzai_rifle_bubble;
			Vector3 position37 = base.transform.position;
			float x13 = position37.x + num;
			Vector3 position38 = base.transform.position;
			float y13 = position38.y + num2;
			Vector3 position39 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_rifle_bubble, new Vector3(x13, y13, position39.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 3 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_shotgun_bubble"))
		{
			GameObject event_banzai_shotgun_bubble = gs.event_banzai_shotgun_bubble;
			Vector3 position40 = base.transform.position;
			float x14 = position40.x + num;
			Vector3 position41 = base.transform.position;
			float y14 = position41.y + num2;
			Vector3 position42 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_shotgun_bubble, new Vector3(x14, y14, position42.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 4 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_smg_bubble"))
		{
			GameObject event_banzai_smg_bubble = gs.event_banzai_smg_bubble;
			Vector3 position43 = base.transform.position;
			float x15 = position43.x + num;
			Vector3 position44 = base.transform.position;
			float y15 = position44.y + num2;
			Vector3 position45 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_smg_bubble, new Vector3(x15, y15, position45.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 5 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_spread_bubble"))
		{
			GameObject event_banzai_spread_bubble = gs.event_banzai_spread_bubble;
			Vector3 position46 = base.transform.position;
			float x16 = position46.x + num;
			Vector3 position47 = base.transform.position;
			float y16 = position47.y + num2;
			Vector3 position48 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_spread_bubble, new Vector3(x16, y16, position48.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 6 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_lobber_bubble"))
		{
			GameObject event_banzai_lobber_bubble = gs.event_banzai_lobber_bubble;
			Vector3 position49 = base.transform.position;
			float x17 = position49.x + num;
			Vector3 position50 = base.transform.position;
			float y17 = position50.y + num2;
			Vector3 position51 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_lobber_bubble, new Vector3(x17, y17, position51.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 7 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_sniper_bubble"))
		{
			GameObject event_banzai_sniper_bubble = gs.event_banzai_sniper_bubble;
			Vector3 position52 = base.transform.position;
			float x18 = position52.x + num;
			Vector3 position53 = base.transform.position;
			float y18 = position53.y + num2;
			Vector3 position54 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_sniper_bubble, new Vector3(x18, y18, position54.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 8 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_rocket_bubble"))
		{
			GameObject event_banzai_rocket_bubble = gs.event_banzai_rocket_bubble;
			Vector3 position55 = base.transform.position;
			float x19 = position55.x + num;
			Vector3 position56 = base.transform.position;
			float y19 = position56.y + num2;
			Vector3 position57 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_banzai_rocket_bubble, new Vector3(x19, y19, position57.z), Quaternion.identity) as GameObject);
		}
		else if (weaponType == 9 && factionByID == 1 && GameData.eventObjects.ContainsKey("banzai_grenade_bubble"))
		{
			GameObject event_atlas_grenade_bubble2 = gs.event_atlas_grenade_bubble;
			Vector3 position58 = base.transform.position;
			float x20 = position58.x + num;
			Vector3 position59 = base.transform.position;
			float y20 = position59.y + num2;
			Vector3 position60 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(event_atlas_grenade_bubble2, new Vector3(x20, y20, position60.z), Quaternion.identity) as GameObject);
		}
		else
		{
			GameObject mCaptureBubble = gs.mCaptureBubble;
			Vector3 position61 = base.transform.position;
			float x21 = position61.x + num;
			Vector3 position62 = base.transform.position;
			float y21 = position62.y + num2;
			Vector3 position63 = base.transform.position;
			captureBubble = (UnityEngine.Object.Instantiate(mCaptureBubble, new Vector3(x21, y21, position63.z), Quaternion.identity) as GameObject);
		}
		if (captureBubble.GetComponent<AudioSource>() != null)
		{
			captureBubble.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
			if (!gs.mGameReady)
			{
				captureBubble.GetComponent<AudioSource>().mute = true;
			}
		}
		captureBubble.name = name;
	}

	public void clearBubble()
	{
		GameObject gameObject = GameObject.Find("capture_bubble" + myIdx);
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		anim_shoot.enabled = true;
	}

	public void prepareForBubbleRelease()
	{
		GameObject gameObject = GameObject.Find(captureBubble.name + "Hedra01");
		bubbleAnimScript bubbleAnimScript = gameObject.GetComponent("bubbleAnimScript") as bubbleAnimScript;
		bubbleAnimScript.setCurrentState(2);
	}

	public Vector3 GetVelocity()
	{
		return velocity;
	}

	public void PlayThrow(int type)
	{
		WeaponScript weaponScript = weapon.GetComponent("WeaponScript") as WeaponScript;
		if (type == 0)
		{
			if (grenadeThrowTimer > 0f || mNumGrenades == 0)
			{
				return;
			}
		}
		else if (!weaponScript.CanFire())
		{
			Logger.trace("<< can't fire grenade");
			return;
		}
		if (type == 0)
		{
			myState.previousActionState |= 2097152;
		}
		string name = base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm";
		GameObject gameObject = GameObject.Find(name);
		LocalControl localControl = playerControl as LocalControl;
		name = ((type != 0) ? (base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model/Point01") : (base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand"));
		GameObject gameObject2 = GameObject.Find(name);
		float x = localControl.aimingTarget.x;
		Vector3 position = gameObject.transform.position;
		float x2 = x - position.x;
		float y = localControl.aimingTarget.y;
		Vector3 position2 = gameObject.transform.position;
		Vector2 v = new Vector3(x2, y - position2.y, 0f);
		Vector3 vector = gameObject2.transform.position - gameObject.transform.position;
		Ray ray = new Ray(gameObject.transform.position, vector);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, vector.magnitude * 1f, 9) || !(hitInfo.collider.gameObject != base.gameObject) || !hitInfo.collider.name.Contains("Collision"))
		{
			Vector2 v2 = new Vector3(0f, 10f, 0f);
			grenadeAngle = Vector3.Angle(v, v2);
			Vector3 position3 = gameObject2.transform.position;
			float x3 = position3.x;
			Vector3 position4 = gameObject2.transform.position;
			grenadePoint = new Vector3(x3, position4.y, 0f);
			if (type == 1)
			{
				float num = (faceTargetDir != 90f) ? 1f : (-1f);
				grenadeAngle *= num;
				Logger.trace("<< waiting to fire");
				weaponScript.WaitToFire(toSet: true);
			}
			else
			{
				grenadeThrowTimer = 2f;
			}
			Logger.trace("<< " + base.gameObject.name + " throwing grenade at angle: " + grenadeAngle);
			gs.SendMyGrenadePosition(grenadePoint, grenadeAngle, mNumGrenadesThrown, type);
		}
	}

	public void setMyCamera(GameObject mycamera)
	{
	}

	public void ApplyDamage(float damage, float health, bool isHeadshot, int attackerId)
	{
		if (isHeadshot)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gs.mHeadshotEmitter) as GameObject;
			GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck");
			gameObject.transform.parent = gameObject2.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			if (attackerId < 0 || attackerId != gs.myGamePlayer.myIdx)
			{
				base.GetComponent<AudioSource>().PlayOneShot(gs.criticalHitSnd);
			}
			else
			{
				gs.myPlayer.GetComponent<AudioSource>().PlayOneShot(gs.criticalHitSnd);
			}
		}
		healthCurrent = health;
		if (healthCurrent < 0f)
		{
			healthCurrent = 0f;
		}
		else
		{
			handleDamageRing.SetupRing(new Color(1f, 0f, 0f, 86f / 255f), 0f);
			handleDamageRing.StartAnimating(false, 20f, 10, 0f, string.Empty);
		}
		if (mIsLocal)
		{
			gs.mHUD.setMaxHealth(healthMax);
			gs.mHUD.setCurrentHealth(healthCurrent);
			gs.mHUD.setTargetHealth(healthCurrent);
		}
		healthRegenDelayCurrent = healthRegenDelayMax;
	}

	public void SlowPlayer()
	{
		Logger.trace("<< slowing player " + base.gameObject.name);
		mSlowTimer = 10f;
	}

	public void Captured(int toPass)
	{
		int num = toPass >> 16;
		int attackerId = toPass & 0xFFFF;
		ApplyDamage(healthCurrent, 0f, false, -1);
		mAmCaptured = true;
		Logger.trace("<< player " + base.gameObject.name + " is captured");
		handleDamageRing.StopAnimating();
		if ((bool)invincibleEffect)
		{
			UnityEngine.Object.Destroy(invincibleEffect);
		}
		if (mIsLocal)
		{
			gs.mHUD.ClearBoostTimers();
		}
		if (mAmSniping)
		{
			CameraScrolling cameraScrolling = Camera.main.GetComponent("CameraScrolling") as CameraScrolling;
			cameraScrolling.SetTarget(base.gameObject.transform);
			mAmSniping = false;
		}
		if (mLowHealthEffect != null)
		{
			mLowHealthEffect.GetComponent<ParticleEmitter>().emit = false;
		}
		damagePickupAdj = 0f;
		armorPickupAdj = 0f;
		speedPickupAdj = 0f;
		damagePickupTeamAdj = 0f;
		armorPickupTeamAdj = 0f;
		speedPickupTeamAdj = 0f;
		GameObject gameObject = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/orbit");
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		GamePlay.SetLayerRecursively(base.gameObject, 14);
		if (mAmInvisible)
		{
			SetVisible();
		}
		Debug.Log("Captured with weapon type " + num);
		spawnBubble(num, attackerId);
	}

	public void Normal()
	{
		StopInvincible();
	}

	public void Halted()
	{
		mHalted = true;
	}

	public void UpdateHacks(User u)
	{
		if (u.GetVariable("hacks").GetIntValue() > mNumCaptures)
		{
			mNumCaptures = u.GetVariable("hacks").GetIntValue();
		}
		if (mAmInvisible)
		{
			GameData.HackWhileInvisible++;
		}
		else if (damagePickupAdj > 0f || damagePickupTeamAdj > 0f)
		{
			GameData.HackWhileDamageBoost++;
		}
		else if (armorPickupAdj > 0f || armorPickupTeamAdj > 0f)
		{
			GameData.HackWhileArmorBoost++;
		}
		else if (speedPickupAdj > 0f || speedPickupTeamAdj > 0f)
		{
			GameData.HackWhileSpeedBoost++;
		}
	}

	public void UpdateHealth(int healthAmt)
	{
	}

	public void Released()
	{
		Logger.trace(base.gameObject.name + " is being released");
		mAmReady = true;
		GamePlay.SetLayerRecursively(base.gameObject, myLayer);
		handleDamageRing.SetupRing(new Color(0f, 0.5f, 1f), 5f);
		handleDamageRing.StartAnimating(true, 20f, 10, 3f, "StopInvincible");
		mAmInvincible = true;
		mAmCaptured = false;
		invinciblePosition = base.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot");
		Vector3 position = invinciblePosition.position;
		invincibleEffect = (UnityEngine.Object.Instantiate(Resources.Load("Effects/InvincibleEffect/InvincibleFinal"), position, Quaternion.identity) as GameObject);
		invincibleEffect.transform.parent = controller.transform;
		healthCurrent = healthMax;
		if (mIsLocal)
		{
			gs.mHUD.setCurrentHealth(healthCurrent);
			gs.mHUD.setTargetHealth(healthCurrent);
		}
		if (gs is TutorialGamePlay)
		{
			mAmReady = true;
		}
		if (GameData.isTeamBattle())
		{
			int num = 0;
			if (gs.mUsedSpawnPts.ContainsValue(myIdx))
			{
				foreach (int value in gs.mUsedSpawnPts.Values)
				{
					if (value == myIdx)
					{
						gs.mUsedSpawnPts.Remove(num);
						break;
					}
					num++;
				}
			}
		}
		clearBubble();
	}

	public void StopInvincible()
	{
		mAmInvincible = false;
		handleDamageRing.StopAnimating();
		if ((bool)invincibleEffect)
		{
			UnityEngine.Object.Destroy(invincibleEffect);
		}
	}

	public void SetWeaponInvisible()
	{
		Logger.trace("<< setting " + base.gameObject.name + "'s weapon to invisible");
		GameObject gameObject = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model");
		myWeaponTexture = gameObject.GetComponent<Renderer>().material;
		gameObject.GetComponent<Renderer>().material = gs.mInvisibleMat;
		Color color = gameObject.GetComponent<Renderer>().material.color;
		float r = color.r;
		Color color2 = gameObject.GetComponent<Renderer>().material.color;
		float g = color2.g;
		Color color3 = gameObject.GetComponent<Renderer>().material.color;
		Color color4 = new Color(r, g, color3.b, targetInvisibleAlpha);
		gameObject.GetComponent<Renderer>().material.SetColor("_Color", color4);
	}

	public void SetInvisible()
	{
		invisibleTimeCurrent = invisibleTimeMax;
		Logger.trace("<< setting " + base.gameObject.name + " to invisible");
		GameObject gameObject = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model");
		myWeaponTexture = gameObject.GetComponent<Renderer>().material;
		gameObject.GetComponent<Renderer>().material = gs.mInvisibleMat;
		GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/armor");
		SkinnedMeshRenderer skinnedMeshRenderer = gameObject2.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
		if (gs == null)
		{
			Logger.traceError("<< gs is null");
		}
		if (myArmor == null)
		{
			Logger.traceError("<< myArmor is null ");
		}
		if (gs.mInvisibleMat == null)
		{
			Logger.traceError("gs Invisible mat is null");
		}
		myArmor[1] = gs.mInvisibleMat;
		skinnedMeshRenderer.materials = myArmor;
		if (mIsLocal || (GameData.isTeamBattle() && mFaction == gs.myGamePlayer.mFaction))
		{
			targetInvisibleAlpha = 0.45f;
			Logger.trace("<<< he's on my team and is team battle");
		}
		else
		{
			if (!GameData.isTeamBattle())
			{
				Logger.trace("<<< not team battle");
			}
			if (mFaction != gs.myGamePlayer.mFaction)
			{
				Logger.trace("<< different factions");
			}
			targetInvisibleAlpha = 0f;
		}
		currentInvisibleAlpha = targetInvisibleAlpha;
		GameObject gameObject3 = GameObject.Find(base.gameObject.name + "/shadow");
		Projector projector = gameObject3.GetComponent("Projector") as Projector;
		projector.enabled = false;
		Color color = skinnedMeshRenderer.materials[1].color;
		float r = color.r;
		Color color2 = skinnedMeshRenderer.materials[1].color;
		float g = color2.g;
		Color color3 = skinnedMeshRenderer.materials[1].color;
		Color color4 = new Color(r, g, color3.b, currentInvisibleAlpha);
		skinnedMeshRenderer.materials[1].SetColor("_Color", color4);
		Color color5 = gameObject.GetComponent<Renderer>().material.color;
		float r2 = color5.r;
		Color color6 = gameObject.GetComponent<Renderer>().material.color;
		float g2 = color6.g;
		Color color7 = gameObject.GetComponent<Renderer>().material.color;
		color4 = new Color(r2, g2, color7.b, currentInvisibleAlpha);
		gameObject.GetComponent<Renderer>().material.SetColor("_Color", color4);
	}

	public void HandleInvisible()
	{
		if (mIsLocal || (GameData.isTeamBattle() && mFaction == gs.myGamePlayer.mFaction))
		{
			invisibleOffset += 0.01f;
			if (invisibleOffset > 1f)
			{
				invisibleOffset = 0f;
			}
			GameObject gameObject = GameObject.Find(base.gameObject.name + "/armor");
			SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
			skinnedMeshRenderer.materials[1].SetFloat("_Offset", invisibleOffset);
		}
		else if (mAmInvisible && !(currentInvisibleAlpha <= 0f))
		{
			currentInvisibleAlpha -= Time.deltaTime;
			if (currentInvisibleAlpha < 0f)
			{
				currentInvisibleAlpha = 0f;
			}
			GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model");
			GameObject gameObject3 = GameObject.Find(base.gameObject.name + "/armor");
			SkinnedMeshRenderer skinnedMeshRenderer2 = gameObject3.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
			if (gs == null)
			{
				Logger.traceError("<< gs is null");
			}
			if (myArmor == null)
			{
				Logger.traceError("<< myArmor is null ");
			}
			if (gs.mInvisibleMat == null)
			{
				Logger.traceError("gs Invisible mat is null");
			}
			Color color = skinnedMeshRenderer2.materials[1].color;
			float r = color.r;
			Color color2 = skinnedMeshRenderer2.materials[1].color;
			float g = color2.g;
			Color color3 = skinnedMeshRenderer2.materials[1].color;
			Color color4 = new Color(r, g, color3.b, currentInvisibleAlpha);
			skinnedMeshRenderer2.materials[1].SetColor("_Color", color4);
			Color color5 = gameObject2.GetComponent<Renderer>().material.color;
			float r2 = color5.r;
			Color color6 = gameObject2.GetComponent<Renderer>().material.color;
			float g2 = color6.g;
			Color color7 = gameObject2.GetComponent<Renderer>().material.color;
			color4 = new Color(r2, g2, color7.b, currentInvisibleAlpha);
			gameObject2.GetComponent<Renderer>().material.SetColor("_Color", color4);
		}
	}

	public void SetVisible()
	{
		GameObject gameObject = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model");
		gameObject.GetComponent<Renderer>().material = myWeaponTexture;
		GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/shadow");
		Projector projector = gameObject2.GetComponent("Projector") as Projector;
		projector.enabled = true;
		GameObject gameObject3 = GameObject.Find(base.gameObject.name + "/armor");
		SkinnedMeshRenderer skinnedMeshRenderer = gameObject3.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
		Exosuit exosuit = GameData.getExosuit(mySuitIdx);
		myArmor[1] = exosuit.getLowPolyTexture();
		skinnedMeshRenderer.materials = myArmor;
		Color color = skinnedMeshRenderer.materials[1].color;
		float r = color.r;
		Color color2 = skinnedMeshRenderer.materials[1].color;
		float g = color2.g;
		Color color3 = skinnedMeshRenderer.materials[1].color;
		Color color4 = new Color(r, g, color3.b, 1f);
		skinnedMeshRenderer.materials[1].SetColor("_Color", color4);
		Color color5 = gameObject.GetComponent<Renderer>().material.color;
		float r2 = color5.r;
		Color color6 = gameObject.GetComponent<Renderer>().material.color;
		float g2 = color6.g;
		Color color7 = gameObject.GetComponent<Renderer>().material.color;
		color4 = new Color(r2, g2, color7.b, 1f);
		gameObject.GetComponent<Renderer>().material.SetColor("_Color", color4);
		if (mIsLocal || (GameData.isTeamBattle() && mFaction == gs.myGamePlayer.mFaction))
		{
			targetInvisibleAlpha = 0.45f;
		}
		else
		{
			targetInvisibleAlpha = 0f;
		}
		mAmInvisible = false;
	}

	public void handlePickup()
	{
		if (mAmCaptured)
		{
			pickingUp = null;
			pickupTimerCurrent = 20f;
			mTryingToPickupWeapon = false;
			mTryingToPickupWeaponTimer = 0f;
			return;
		}
		if (gs.m_networkManager != null && gs.m_networkManager.user != null && pickingUp != null && gs.m_networkManager.user.GetVariable("pickingup") != null && gs.m_networkManager.user.IsItMe && !gs.m_networkManager.user.GetVariable("pickingup").GetBoolValue())
		{
			pickingUp = null;
			mTryingToPickupWeapon = false;
			mTryingToPickupWeaponTimer = 0f;
			return;
		}
		if (pickingUp == null)
		{
			if (pickupTimerCurrent > 0f)
			{
				if (pickupTimerCurrent < pickupTimerMax)
				{
					pickupTimerCurrent += Time.deltaTime;
				}
				if (pickupTimerCurrent >= pickupTimerMax)
				{
					pickupTimerCurrent = 20f;
				}
			}
			return;
		}
		PickUp pickUp = pickingUp.GetComponent("PickUp") as PickUp;
		if (!pickUp.mIsWaitingForServer && pickUp.isActive)
		{
			pickupTimerCurrent -= Time.deltaTime;
			if (!(pickupTimerCurrent > 0f))
			{
				int puType = pickUp.puType;
				pickUp.mIsWaitingForServer = true;
				gs.SendActivatePickupEvent(pickUp.puIndex, puType, (int)pickUp.timeToRespawn, (int)pickUp.effectTime);
			}
		}
	}

	public void activatePickupOnPlayer(GameObject pickup, int pickupType)
	{
		pickupTimerCurrent = 20f;
		PickUp pickUp = pickup.GetComponent("PickUp") as PickUp;
		pickingUp = null;
		pickUp.playEffect();
		Logger.trace("<< " + base.gameObject.name + " picked up " + pickup.name + "type : " + pickUp.puType);
		bool flag = false;
		switch (pickupType)
		{
		case 1:
			damagePickupAdj = pickUp.multiplier;
			Logger.trace("<< pickup damageadj " + pickUp.multiplier + " effectTime " + pickUp.effectTime);
			flag = true;
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[0]);
			mPickupActive = 1;
			break;
		case 0:
			armorPickupAdj = pickUp.multiplier;
			Logger.trace("<< pickup armoradj " + pickUp.multiplier + " effectTime " + pickUp.effectTime);
			flag = true;
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[1]);
			mPickupActive = 0;
			break;
		case 2:
			SetInvisible();
			Logger.trace("<< setInvisible ");
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[2]);
			mPickupActive = 2;
			mAmInvisible = true;
			break;
		case 3:
			speedPickupAdj = pickUp.multiplier;
			Logger.trace("<< pickup speedadj " + pickUp.multiplier + " effectTime " + pickUp.effectTime);
			speedline1.enabled = true;
			speedline2.enabled = true;
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[3]);
			mPickupActive = 3;
			break;
		case 5:
			if (mIsLocal)
			{
				SendChangeWeapon(6);
				base.GetComponent<AudioSource>().PlayOneShot(gs.mousePressBtnSnd);
			}
			break;
		case 4:
			if (mIsLocal)
			{
				SendChangeWeapon(7);
				base.GetComponent<AudioSource>().PlayOneShot(gs.mousePressBtnSnd);
			}
			break;
		case 6:
			if (mIsLocal)
			{
				SendChangeWeapon(8);
				base.GetComponent<AudioSource>().PlayOneShot(gs.mousePressBtnSnd);
			}
			break;
		case 7:
			if (Debug.isDebugBuild)
			{
				gs.mHUD.AddMessage(mScreenName + " picked up a grenade boost");
			}
			handleDamageRing.SetupRing(new Color(1f, 1f, 1f, 1f), 0.2f);
			handleDamageRing.StartAnimating(true, 20f, 10, 1f, "grenades");
			pickUp.playEffect();
			mNumGrenades = 3;
			break;
		case 11:
			damagePickupTeamAdj = pickUp.multiplier;
			Logger.trace("<< pickup damageadj " + pickUp.multiplier + " effectTime " + pickUp.effectTime);
			flag = true;
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[0]);
			mPickupTeamActive = 11;
			break;
		case 10:
			armorPickupTeamAdj = pickUp.multiplier;
			Logger.trace("<< pickup armoradj " + pickUp.multiplier + " effectTime " + pickUp.effectTime);
			flag = true;
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[1]);
			mPickupTeamActive = 11;
			break;
		case 13:
			speedPickupTeamAdj = pickUp.multiplier;
			Logger.trace("<< pickup speedadj " + pickUp.multiplier + " effectTime " + pickUp.effectTime);
			speedline1.enabled = true;
			speedline2.enabled = true;
			base.GetComponent<AudioSource>().PlayOneShot(gs.boostSounds[3]);
			mPickupTeamActive = 13;
			break;
		default:
			Logger.trace("don't know type");
			break;
		case 12:
			break;
		}
		if (pickupType == 7)
		{
			return;
		}
		mActivePickup = pickupType;
		if (mIsLocal && pickupType != 7)
		{
			if (pickupType < 4 || pickupType == 10 || pickupType == 11 || pickupType == 13)
			{
				gs.mHUD.addTimer(pickupType, pickUp.effectTime, -1);
			}
			else
			{
				mSpecialWeaponCooldownTimer = mSpecialWeaponCooldown;
				gs.mHUD.addTimer(50, mSpecialWeaponCooldown, -2);
			}
		}
		if (!mAmInvisible && flag)
		{
			GameObject gameObject = (pickupType != 0 && pickupType != 10) ? (UnityEngine.Object.Instantiate(gs.damageEffect, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject) : (UnityEngine.Object.Instantiate(gs.armorEffect, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject);
			gameObject.name = "orbit";
			GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis");
			gameObject.transform.parent = gameObject2.transform;
			gameObject.transform.localPosition = new Vector3(-4f, 0f, 0f);
		}
	}

	public void activateBoostsInProgress(int boost)
	{
		bool flag = false;
		switch (boost)
		{
		case 1:
			flag = true;
			mPickupActive = 1;
			break;
		case 0:
			flag = true;
			mPickupActive = 0;
			break;
		case 2:
			SetInvisible();
			mPickupActive = 2;
			mAmInvisible = true;
			break;
		case 3:
			speedline1.enabled = true;
			speedline2.enabled = true;
			mPickupActive = 3;
			break;
		case 11:
			flag = true;
			mPickupTeamActive = 11;
			break;
		case 10:
			flag = true;
			mPickupTeamActive = 10;
			break;
		case 13:
			speedline1.enabled = true;
			speedline2.enabled = true;
			mPickupTeamActive = 13;
			break;
		default:
			Logger.trace("don't know type");
			break;
		case 12:
			break;
		}
		if (!flag)
		{
			return;
		}
		GameObject gameObject;
		if (boost == 0 || boost == 10)
		{
			if (gs == null)
			{
				Logger.trace("<< gs == null");
				return;
			}
			if (gs.armorEffect == null)
			{
				return;
			}
			gameObject = (UnityEngine.Object.Instantiate(gs.armorEffect, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject);
		}
		else
		{
			if (gs.damageEffect == null)
			{
				return;
			}
			gameObject = (UnityEngine.Object.Instantiate(gs.damageEffect, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject);
		}
		gameObject.name = "orbit";
		GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis");
		gameObject.transform.parent = gameObject2.transform;
		gameObject.transform.localPosition = new Vector3(-4f, 0f, 0f);
	}

	public void deactivePickup(bool isTeam)
	{
		if (isTeam)
		{
			deactivatePickupOnPlayer(mPickupTeamActive);
		}
		else
		{
			deactivatePickupOnPlayer(mPickupActive);
		}
	}

	public void deactivatePickupOnPlayer(int pickupType)
	{
		Logger.trace("<< deactivate pickup: " + pickupType);
		switch (pickupType)
		{
		case 1:
			damagePickupAdj = 0f;
			mPickupActive = -1;
			break;
		case 0:
			armorPickupAdj = 0f;
			mPickupActive = -1;
			break;
		case 2:
			mPickupActive = -1;
			SetVisible();
			break;
		case 3:
			speedPickupAdj = 0f;
			mPickupActive = -1;
			speedline1.enabled = false;
			speedline2.enabled = false;
			break;
		case 11:
			damagePickupTeamAdj = 0f;
			mPickupTeamActive = -1;
			break;
		case 10:
			armorPickupTeamAdj = 0f;
			mPickupTeamActive = -1;
			break;
		case 12:
			mPickupTeamActive = -1;
			SetVisible();
			break;
		case 13:
			speedPickupTeamAdj = 0f;
			mPickupTeamActive = -1;
			speedline1.enabled = false;
			speedline2.enabled = false;
			break;
		default:
			Logger.trace("don't know type");
			break;
		}
		GameObject obj = GameObject.Find(base.gameObject.name + "/Bip01/Bip01 Pelvis/orbit");
		UnityEngine.Object.Destroy(obj);
	}

	public void dontActivatePickup()
	{
		pickingUp = null;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (mIsLocal)
		{
			handleBoostCollision(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (mIsLocal)
		{
			pickingUp = null;
			mTryingToPickupWeapon = false;
			mTryingToPickupWeaponTimer = 0f;
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (mIsLocal)
		{
			handleBoostCollision(other);
		}
	}

	private void handleBoostCollision(Collider other)
	{
		if (mAmCaptured)
		{
			return;
		}
		PickUp pickUp = other.GetComponent("PickUp") as PickUp;
		if (pickUp == null)
		{
			return;
		}
		if (!pickUp.isActive)
		{
			pickingUp = null;
			return;
		}
		if (weaponIdx >= 6 && pickUp.puType > 3 && pickUp.puType < 7)
		{
			mTryingToPickupWeapon = true;
			Logger.trace("<< can't pickup another special weapon while holding a special");
			return;
		}
		pickingUp = other.gameObject;
		if (pickingUp.name == "grenades")
		{
			pickingUp = null;
			if (mNumGrenades < 3)
			{
				pickUp.mIsWaitingForServer = true;
				gs.SendActivatePickupEvent(pickUp.puIndex, pickUp.puType, (int)pickUp.timeToRespawn, (int)pickUp.effectTime);
			}
		}
		else if (pickingUp.name == "boost")
		{
			if (mPickupActive >= 0 || mPickupTeamActive >= 0)
			{
				pickingUp = null;
				return;
			}
			pickupTimerMax = mTimeToPickupBoost;
			if (mActivePickupIdx != pickUp.puIndex)
			{
				pickupTimerCurrent = pickupTimerMax;
			}
			if (pickupTimerCurrent > pickupTimerMax)
			{
				pickupTimerCurrent = pickupTimerMax;
			}
			mActivePickupIdx = pickUp.puIndex;
		}
		else
		{
			if (!(pickingUp.name == "weapon"))
			{
				return;
			}
			if (mSpecialWeaponCooldownTimer > 0f)
			{
				pickingUp = null;
				mTryingToPickupWeapon = true;
				return;
			}
			pickupTimerMax = mTimeToPickupWeapon;
			if (mActivePickupIdx != pickUp.puIndex)
			{
				pickupTimerCurrent = pickupTimerMax;
			}
			if (pickupTimerCurrent > pickupTimerMax)
			{
				pickupTimerCurrent = pickupTimerMax;
			}
			mActivePickupIdx = pickUp.puIndex;
		}
	}

	public void SetDisplayName(string displayname)
	{
		mScreenName = displayname;
	}

	private void OnGUI()
	{
		arrowTimer -= Time.deltaTime;
		if ((!(arrowTimer > 0f) || !mIsLocal) && !mIsLocal && mAmCaptured && mTimeForInvincibleHelp > 0f)
		{
			bool flag = false;
			if (gs.m_networkManager.getFactionByID(mMyID) == gs.m_networkManager.getFactionByID(gs.m_networkManager.user.PlayerId))
			{
				flag = true;
			}
			if (!GameData.isTeamBattle() || (GameData.isTeamBattle() && !flag))
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
				vector.y = (float)Screen.height - vector.y;
				vector.y -= 160f * ((float)Screen.height / 600f);
				GUI.DrawTexture(new Rect(vector.x - (float)mInvincibleMessage.width * 0.5f, vector.y, mInvincibleMessage.width, mInvincibleMessage.height), mInvincibleMessage);
			}
		}
	}

	public void SendChangeWeapon(int idx)
	{
		gs.SendChangeWeapon(idx);
	}

	public void DeleteMe()
	{
		if (mFaction == 1)
		{
			gs.mNumBanzaiCapturesLeft += mNumCaptures;
		}
		else
		{
			gs.mNumAtlasCapturesLeft += mNumCaptures;
		}
		if (captureBubble != null)
		{
			UnityEngine.Object.Destroy(captureBubble);
		}
		UnityEngine.Object.Destroy(namelabel);
		UnityEngine.Object.Destroy(base.gameObject);
		if (grimCloudEffect != null)
		{
			UnityEngine.Object.Destroy(grimCloudEffect);
		}
	}
}
