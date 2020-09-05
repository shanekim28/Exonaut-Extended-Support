using System.Collections;
using UnityEngine;

public class TutorialGamePlay : GamePlay
{
	public GameObject GamePlayTutorial;

	public GameObject TutorialTriggers;

	public GameObject TrainingSuit;

	public Material TrainingSuitMaterial;

	public bool bCanUseGrenades;

	private int GrenadeIDcounter;

	private IEnumerator DelayRelease(float time)
	{
		yield return new WaitForSeconds(time);
		myGamePlayer.bDisableControl = false;
	}

	private IEnumerator TutorialUpdate()
	{
		while (!allPlayersSpawned)
		{
			yield return new WaitForEndOfFrame();
		}
		Object.Destroy(GameObject.Find("TT_Glass"));
		Object.Destroy(GameObject.Find("TT_Chamber_Glass"));
		sfs = null;
		SendChangeWeapon(5);
		myGamePlayer.armAngle = 0.4f;
		myGamePlayer.SetSuit(1);
		myGamePlayer.mRegenGrenade = true;
		myGamePlayer.healthCurrent = myGamePlayer.healthMax;
		mHUD.setMaxHealth(myGamePlayer.healthCurrent);
		mHUD.setCurrentHealth(myGamePlayer.healthCurrent);
		mHUD.setTargetHealth(myGamePlayer.healthCurrent);
		while (FirstUse.mInstance == null)
		{
			yield return new WaitForEndOfFrame();
		}
		FirstUse.Frame CurrentFrame = FirstUse.mInstance.GetFrame();
		while (CurrentFrame != null)
		{
			CurrentFrame = FirstUse.mInstance.GetFrame();
			if (CurrentFrame.mCharImage != FirstUse.Frame.CharacterImage.None)
			{
				myGamePlayer.bDisableControl = true;
			}
			else if (myGamePlayer.bDisableControl)
			{
				StartCoroutine(DelayRelease(0.25f));
			}
			if (bCanUseGrenades)
			{
				if (myGamePlayer.mNumGrenades == 0)
				{
					myGamePlayer.mNumGrenades = myGamePlayer.mMaxGrenades;
				}
			}
			else
			{
				myGamePlayer.mNumGrenades = 0;
			}
			myPlayer.gameObject.SendMessage("ResetHelp", false);
			if (CurrentFrame.mContextualHelp != 0)
			{
				myPlayer.gameObject.SendMessage(CurrentFrame.mContextualHelp.ToString());
			}
			switch (CurrentFrame.mAction)
			{
			case FirstUse.Frame.RequiredAction.Waypoint:
			{
				Rect FocusRect = CurrentFrame.mFocusRect.Calculate();
				Vector3 Waypoint = new Vector3(FocusRect.x, FocusRect.y, 0f);
				float x = Waypoint.x;
				Vector3 position = myPlayer.transform.position;
				if (Mathf.Abs(x - position.x) < 5f)
				{
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.Waypoint);
				}
				break;
			}
			case FirstUse.Frame.RequiredAction.Waypoint_Area:
			{
				Rect FocusRect2 = CurrentFrame.mFocusRect.Calculate();
				Vector3 Waypoint2 = new Vector3(FocusRect2.x, FocusRect2.y, 0f);
				if ((Waypoint2 - myPlayer.transform.position).sqrMagnitude < 2500f)
				{
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.Waypoint_Area);
				}
				break;
			}
			case FirstUse.Frame.RequiredAction.Do_Capture:
				if (CurrentFrame.ActionCount > 0)
				{
					CurrentFrame.ActionCount--;
					int toPass = 0;
					myPlayer.SendMessage("Captured", toPass);
					GamePlay.messageTimer = 500000f;
					if (!CurrentFrame.mNextButton)
					{
						FirstUse.DoAction(FirstUse.Frame.RequiredAction.Do_Capture);
					}
				}
				break;
			case FirstUse.Frame.RequiredAction.Do_Shutdown:
				GameData.MyTutorialStep = 4;
				TutorialTriggers.SendMessageUpwards("NoSound");
				QuitGame(0);
				break;
			case FirstUse.Frame.RequiredAction.Do_Release:
				GamePlay.messageTimer = 0f;
				myPlayer.SendMessage("Released");
				if (!CurrentFrame.mNextButton)
				{
					FirstUse.DoAction(FirstUse.Frame.RequiredAction.Do_Release);
				}
				myGamePlayer.StopInvincible();
				break;
			case FirstUse.Frame.RequiredAction.Pickup_Rocket:
				myGamePlayer.mSpecialWeaponCooldownTimer = 0f;
				mHUD.ClearTimers();
				if (myGamePlayer.weaponIdx == 6 || myGamePlayer.weaponIdx == 7)
				{
					SendChangeWeapon(1);
				}
				break;
			case FirstUse.Frame.RequiredAction.Pickup_Longshot:
				myGamePlayer.mSpecialWeaponCooldownTimer = 0f;
				mHUD.ClearTimers();
				if (myGamePlayer.weaponIdx == 6 || myGamePlayer.weaponIdx == 8)
				{
					SendChangeWeapon(1);
				}
				break;
			case FirstUse.Frame.RequiredAction.Pickup_HelixCannon:
				myGamePlayer.mSpecialWeaponCooldownTimer = 0f;
				mHUD.ClearTimers();
				if (myGamePlayer.weaponIdx == 8 || myGamePlayer.weaponIdx == 7)
				{
					SendChangeWeapon(1);
				}
				break;
			case FirstUse.Frame.RequiredAction.Obstacle_1:
			case FirstUse.Frame.RequiredAction.Obstacle_2:
			case FirstUse.Frame.RequiredAction.Obstacle_3:
			case FirstUse.Frame.RequiredAction.Obstacle_4:
			case FirstUse.Frame.RequiredAction.Obstacle_5:
			case FirstUse.Frame.RequiredAction.Obstacle_6:
			case FirstUse.Frame.RequiredAction.Obstacle_7:
			case FirstUse.Frame.RequiredAction.Obstacle_8:
			{
				int ID = (int)(CurrentFrame.mAction - 26 + 1);
				Transform Obstacle = TutorialTriggers.transform.FindChild("Obstacle" + ID);
				if (Obstacle != null)
				{
					bool bCompleted = true;
					foreach (Transform child in Obstacle)
					{
						if (child != Obstacle && child.gameObject.active)
						{
							bCompleted = false;
							break;
						}
					}
					if (bCompleted)
					{
						FirstUse.DoAction(CurrentFrame.mAction);
					}
				}
				break;
			}
			case FirstUse.Frame.RequiredAction.Give_Weapon:
				SendChangeWeapon(int.Parse(CurrentFrame.mActionSpecific));
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.Give_Weapon);
				break;
			case FirstUse.Frame.RequiredAction.Deactivate_GameObject:
			{
				Transform Target2 = TutorialTriggers.transform.Find(CurrentFrame.mActionSpecific);
				if (Target2 != null)
				{
					Target2.gameObject.SetActiveRecursively(state: false);
				}
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.Deactivate_GameObject);
				break;
			}
			case FirstUse.Frame.RequiredAction.Activate_GameObject:
			{
				Transform Target = TutorialTriggers.transform.Find(CurrentFrame.mActionSpecific);
				if (Target != null)
				{
					Target.gameObject.SetActiveRecursively(state: true);
				}
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.Activate_GameObject);
				break;
			}
			case FirstUse.Frame.RequiredAction.SetGrenadesActive:
				bCanUseGrenades = bool.Parse(CurrentFrame.mActionSpecific);
				FirstUse.DoAction(FirstUse.Frame.RequiredAction.SetGrenadesActive);
				break;
			}
			yield return 0;
		}
	}

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("DynamicOptions");
		if (gameObject != null)
		{
			DynamicOptions dynamicOptions = gameObject.GetComponent("DynamicOptions") as DynamicOptions;
			dynamicOptions.bCanFullscreen = true;
		}
		for (int i = 0; i < 8; i++)
		{
			players[i] = null;
		}
		state = base.InitGame;
		doNotInstantiateBulletList = new ArrayList();
		TutorialTriggers = (Object.Instantiate(TutorialTriggers) as GameObject);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("shieldwall"), LayerMask.NameToLayer("Default"));
		GameObject gameObject2 = GameObject.Find("NetworkManager");
		m_networkManager = (gameObject2.GetComponent("NetworkManager") as NetworkManager);
		sfs = null;
		Camera.main.renderingPath = RenderingPath.Forward;
	}

	protected override void SpawnPlayersAndWorlds()
	{
		if (GameData.getWorldById(GameData.WorldID) == null)
		{
			return;
		}
		TextAsset playerSpawnPointsById = GameData.getPlayerSpawnPointsById(GameData.WorldID - 1, GameData.BattleType - 1, GameData.GameType - 1);
		TextAsset pickupSpawnPointsById = GameData.getPickupSpawnPointsById(GameData.WorldID - 1, GameData.BattleType - 1, GameData.GameType - 1);
		if (playerSpawnPointsById == null || pickupSpawnPointsById == null)
		{
			GameData.LoadSpawnPoints();
			return;
		}
		GameData.parseAllSpawnPoints();
		FactionSelection.DownloadTextureBundles();
		int[] atlasDefaultSuits = GameData.AtlasDefaultSuits;
		foreach (int suitId in atlasDefaultSuits)
		{
			AssetLoader.AddSuitToLoad(suitId, AssetLoader.SuitAsset.SuitType.high, 50);
		}
		int[] banzaiDefaultSuits = GameData.BanzaiDefaultSuits;
		foreach (int suitId2 in banzaiDefaultSuits)
		{
			AssetLoader.AddSuitToLoad(suitId2, AssetLoader.SuitAsset.SuitType.high, 50);
		}
		base.SpawnPlayersAndWorlds();
	}

	private void FixedUpdate()
	{
		Projectile[] array = (Projectile[])Object.FindObjectsOfType(typeof(Projectile));
		LayerMask mask = 0;
		mask = ((int)mask | 0x4000);
		mask = ((int)mask | 0x8000);
		mask = ((int)mask | 4);
		mask = ((int)mask | 0x400);
		mask = ((int)mask | 0x800);
		mask = ((int)mask | 0x1000);
		mask = ((int)mask | 0x2000);
		mask = ~(int)mask;
		int num = 8;
		Projectile[] array2 = array;
		foreach (Projectile projectile in array2)
		{
			if (projectile.isRocket)
			{
				Debug.Log(projectile.name);
				Vector3 origin = projectile.transform.position + -projectile.transform.up * num / 2f;
				RaycastHit hitInfo;
				if (Physics.Raycast(new Ray(origin, projectile.transform.up), out hitInfo, num, mask))
				{
					SendRocketExplode(projectile.idx, hitInfo.point);
				}
			}
		}
	}

	public override void SendRocketExplode(int num, Vector3 pos)
	{
		Projectile[] array = Object.FindObjectsOfType(typeof(Projectile)) as Projectile[];
		Debug.Log("SendRocketExplode " + array.Length);
		Projectile[] array2 = array;
		int num2 = 0;
		Projectile projectile;
		while (true)
		{
			if (num2 < array2.Length)
			{
				projectile = array2[num2];
				Debug.Log(projectile.idx + " " + projectile.isRocket);
				if (projectile.idx == num && projectile.isRocket)
				{
					break;
				}
				num2++;
				continue;
			}
			return;
		}
		Debug.Log("SendRocketExplode Found Rocket");
		DamageTrigger[] componentsInChildren = TutorialTriggers.GetComponentsInChildren<DamageTrigger>();
		DamageTrigger[] array3 = componentsInChildren;
		foreach (DamageTrigger damageTrigger in array3)
		{
			damageTrigger.ApplyWallExplosion(pos);
		}
		projectile.Explode();
	}

	public override void QuitGame(int quitCondition)
	{
		if (sfs != null)
		{
			Logger.traceError("TutorialGamePlay::QuitGame");
		}
		FirstUse.mInstance.Kill();
		switch (quitCondition)
		{
		case 1:
			break;
		case 2:
			break;
		case 3:
			GameData.MyTutorialStep = -1;
			GameData.removePlayer(1);
			GameData.ClearSpawnPoints();
			GameData.clearAllSfsPlayers();
			m_networkManager.LeaveRoom();
			GameData.CurPlayState = GameData.PlayState.GAME_IS_QUITTING;
			GL.Clear(true, true, new Color(0f, 0f, 0f, 255f));
			LoadLevel((GameData.MyPlayStatus != 2 || GameData.MyFactionId != 0) ? "GameHome" : "FactionSelection");
			break;
		default:
			GameData.removePlayer(1);
			GameData.ClearSpawnPoints();
			GameData.clearAllSfsPlayers();
			m_networkManager.LeaveRoom();
			GL.Clear(true, true, new Color(0f, 0f, 0f, 255f));
			GameData.CurPlayState = GameData.PlayState.GAME_IS_QUITTING;
			LoadLevel((GameData.MyPlayStatus != 2 || GameData.MyFactionId != 0) ? "GameHome" : "FactionSelection");
			break;
		}
	}

	protected override void waitToSpawnPlayer()
	{
		MessageBox.AddMessageCustom("Go Fullscreen?", "We suggest training in fullscreen. Would you like to go fullscreen now?", null, false, FullScreenCallback, "No", "Yes");
		base.waitToSpawnPlayer();
		mHUD.avatarTexture = (Resources.Load("HUD/avatar/HUD_thumb_training") as Texture2D);
	}

	public void FullScreenCallback(MessageBox.ReturnType Return)
	{
		if (Return == MessageBox.ReturnType.MB_NO)
		{
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen: true);
		}
		if (GameData.MyTutorialStep == 2)
		{
			GameData.MyTutorialStep = 3;
			Object.Instantiate(GamePlayTutorial);
			StartCoroutine("TutorialUpdate");
		}
	}

	protected override void spawnPlayers()
	{
		if (CanSpawnPlayers())
		{
			Logger.trace("::::::::::::::::::::: Tutorial Spawn Players ::::::::::::::::::::::");
			spawnMyPlayer(1);
			myGamePlayer.bDisableControl = true;
			stateStatus = "all players spawned";
			allPlayersSpawned = true;
		}
	}

	public override GameObject spawnExosuit(int playerIndex, int suitId, int textureId)
	{
		Logger.trace("# SPAWN SUIT " + suitId + " #");
		GameObject trainingSuit = TrainingSuit;
		if (trainingSuit == null)
		{
			Logger.traceError("<< Suit model is null");
		}
		else
		{
			Logger.trace("Suit Model:" + trainingSuit);
		}
		Vector3 vector = new Vector3(0f, 2000f, 0f);
		Vector3 vector2 = GameData.PlayerSpawns[playerIndex];
		float x = vector2.x;
		Vector3 vector3 = GameData.PlayerSpawns[playerIndex];
		float y = vector3.y;
		Vector3 vector4 = GameData.PlayerSpawns[playerIndex];
		vector = new Vector3(x, y, vector4.z);
		cameraScrolling.levelBounds = cameraScrolling.levelAttributes.bounds;
		if (vector.x > cameraScrolling.levelBounds.xMax || vector.x < cameraScrolling.levelBounds.xMin || vector.y > cameraScrolling.levelBounds.yMax || vector.y < cameraScrolling.levelBounds.yMin)
		{
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.xMin);
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.yMin);
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.xMax);
			Logger.trace("camerabounds: " + cameraScrolling.levelBounds.yMax);
			vector = Vector3.zero;
		}
		Logger.trace("SpawnPt: " + vector);
		GameObject gameObject = Object.Instantiate(trainingSuit, vector, Quaternion.identity) as GameObject;
		Transform transform = gameObject.transform.Find("armor");
		SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer;
		skinnedMeshRenderer.updateWhenOffscreen = true;
		skinnedMeshRenderer.materials = new Material[2];
		skinnedMeshRenderer.materials = new Material[2]
		{
			Resources.Load("effects/healthDamage/linePulseUpwardsMat") as Material,
			Object.Instantiate(TrainingSuitMaterial) as Material
		};
		return gameObject;
	}

	protected override void ShowBanner(int num, Vector3 pos)
	{
	}

	public override void SendActivatePickupEvent(int pickupIdx, int pickupType, int pickupTime, int effectTime)
	{
		switch (pickupType)
		{
		case 0:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_Shield);
			break;
		case 1:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_Damage);
			break;
		case 2:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_Invis);
			break;
		case 3:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_Speed);
			break;
		case 6:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_Rocket);
			break;
		case 5:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_HelixCannon);
			break;
		case 4:
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Pickup_Longshot);
			break;
		}
		activatePickup(pickupIdx, pickupType, GameData.MyPlayerId - 1);
	}

	public override void DrawSniperLine(float x0, float y0, float x1, float y1)
	{
		WeaponScript weaponScript = null;
		if (myGamePlayer.weapon != null && (bool)(weaponScript = myGamePlayer.weapon.GetComponent<WeaponScript>()) && weaponScript.totalBullets == 1)
		{
			weaponScript.totalBullets = weaponScript.clipSize + 1;
		}
		base.DrawSniperLine(x0, y0, x1, y1);
		LayerMask mask = 0;
		mask = ((int)mask | 0x4000);
		mask = ((int)mask | 0x8000);
		mask = ((int)mask | 4);
		mask = ((int)mask | 0x400);
		mask = ((int)mask | 0x800);
		mask = ((int)mask | 0x1000);
		mask = ((int)mask | 0x2000);
		mask = ~(int)mask;
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(myGamePlayer.mRangeStart, (myGamePlayer.mRangeEnd - myGamePlayer.mRangeStart).normalized), out hitInfo, 9999f, mask))
		{
			DamageTrigger damageTrigger = hitInfo.collider.gameObject.GetComponent(typeof(DamageTrigger)) as DamageTrigger;
			if (damageTrigger != null)
			{
				damageTrigger.ApplyWallDamage(100f);
			}
		}
	}

	protected override void SetContextualHelp()
	{
	}

	public override void SendMyShotPosition(Vector3 pos, float angle, float angleIncrement, float distance)
	{
		Vector2 point = Input.mousePosition;
		point.y = (float)Screen.height - point.y;
		WeaponScript weaponScript = null;
		if (myGamePlayer.weapon != null && (bool)(weaponScript = myGamePlayer.weapon.GetComponent<WeaponScript>()) && weaponScript.totalBullets == 1)
		{
			weaponScript.totalBullets = weaponScript.clipSize + 1;
		}
		Rect groupPos = FirstUse.mInstance.GetGroupPos();
		groupPos.y += groupPos.height - FirstUse.mInstance.GetFrame().mBoxSize.y;
		groupPos.height = FirstUse.mInstance.GetFrame().mBoxSize.y;
		if (!groupPos.Contains(point))
		{
			FirstUse.DoAction(FirstUse.Frame.RequiredAction.Shoot);
			myGamePlayer.remoteShoot(pos, angle, angleIncrement, 0);
		}
	}

	public override void SendMyGrenadePosition(Vector3 pos, float angle, int num, int type)
	{
		WeaponScript weaponScript = null;
		if (myGamePlayer.weapon != null && (bool)(weaponScript = myGamePlayer.weapon.GetComponent<WeaponScript>()) && weaponScript.totalBullets == 1)
		{
			weaponScript.totalBullets = weaponScript.clipSize + 1;
		}
		Player player = myPlayer.GetComponent("Player") as Player;
		player.ThrowGrenade(pos, angle, type, GrenadeIDcounter++);
		FirstUse.DoAction(FirstUse.Frame.RequiredAction.ThrowGrenade);
	}

	public override void SendGrenadeExplode(int num, Vector3 pos, int id)
	{
		Debug.Log("SendGrenadeExplode");
		GameObject[] array = GameObject.FindGameObjectsWithTag("grenade");
		GameObject[] array2 = array;
		int num2 = 0;
		Grenade grenade;
		while (true)
		{
			if (num2 < array2.Length)
			{
				GameObject gameObject = array2[num2];
				grenade = (gameObject.GetComponent("Grenade") as Grenade);
				if (grenade.thrower == myGamePlayer.myIdx && grenade.grenadeNum == num)
				{
					break;
				}
				num2++;
				continue;
			}
			return;
		}
		DamageTrigger[] componentsInChildren = TutorialTriggers.GetComponentsInChildren<DamageTrigger>();
		DamageTrigger[] array3 = componentsInChildren;
		foreach (DamageTrigger damageTrigger in array3)
		{
			damageTrigger.ApplyWallExplosion(grenade.transform.position);
		}
		grenade.Explode();
	}

	public override void SendChangeWeapon(int idx)
	{
		myGamePlayer.changeWeapon(idx);
		StartCoroutine(DelayWeaponFire(0.75f));
	}

	private IEnumerator DelayWeaponFire(float time)
	{
		float TimeToEnd = Time.time + time;
		while (TimeToEnd > Time.time)
		{
			WeaponScript wpn = myGamePlayer.weapon.GetComponent<WeaponScript>();
			wpn.timeUntilFire = TimeToEnd - Time.time;
			yield return new WaitForEndOfFrame();
		}
	}

	protected override bool CanSpawnPlayers()
	{
		return true;
	}

	protected override void LoadLevel(string level)
	{
		TutorialTriggers.SendMessageUpwards("NoSound");
		Object.DestroyImmediate(TutorialTriggers);
		var dc_583_2_637348406167448521 = Camera.main;
		StopCoroutine("TutorialUpdate");
		base.LoadLevel(level);
	}
}
