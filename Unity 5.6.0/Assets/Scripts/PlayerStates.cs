using UnityEngine;

public class PlayerStates
{
	public const int IDLE = 0;

	public const int CROUCH = 1;

	public const int RUN_LEFT = 2;

	public const int RUN_RIGHT = 4;

	public const int RUN = 6;

	public const int JUMP = 8;

	public const int JETPACK = 16;

	public const int AIRDASH = 32;

	public const int CAPTURED = 64;

	public const int ROLL = 128;

	public const int ROLL_REC = 256;

	public const int FALL = 512;

	public const int RELEASED = 1024;

	public const int FREED = 2048;

	public const int AIRDASH_REC = 4096;

	public const int JUMP_FALL = 8192;

	public const int JUMP_REC = 16384;

	public const int RUN_REV = 32768;

	public const int POWER_1 = 65536;

	public const int POWER_2 = 131072;

	public const int POWER_3 = 262144;

	public const int SNIPE = 524288;

	public const int NO_ACTION = 0;

	public const int SHOOT = 1048576;

	public const int THROW = 2097152;

	public const int RELOAD = 4194304;

	public const int NO_TIME = 0;

	public const int IN_AIR = 268435456;

	public const int DIRECTION = 536870912;

	public const float normalHeight = 10f;

	public const float normalCenter = 6f;

	public const float normalRadius = 1.5f;

	public const float crouchRollHeight = 5f;

	public const float crouchRollCenter = 3.5f;

	public const float crouchRollRadius = 1.5f;

	public int desiredState;

	public int currentState;

	public int previousMoveState;

	public int currentMoveState;

	public int nextMoveState = -1;

	public int previousActionState;

	public int currentActionState;

	public int currentArmState;

	public int currentTimerState;

	public int currentContextState;

	public Player myInfo;

	public Player myAttributes;

	private bool canAirDash = true;

	public int airdashDir;

	public Vector2 airdashStartPt;

	public GamePlay gs;

	public PlayerStates(Player me)
	{
		myInfo = me;
		myAttributes = myInfo;
		gs = GamePlay.GetGamePlayScript();
		currentState = 0;
	}

	private void updateCurrentState()
	{
		currentState = (currentMoveState | currentActionState | currentTimerState | currentContextState);
	}

	public void setMoveState(int toSet)
	{
		currentMoveState = toSet;
		updateCurrentState();
	}

	private void setActionState(int toSet)
	{
		currentActionState = toSet;
		updateCurrentState();
	}

	private void setTimerState(int toSet)
	{
		currentTimerState = toSet;
		updateCurrentState();
	}

	private void setContextState(int toSet)
	{
		currentContextState = toSet;
		updateCurrentState();
	}

	public void clearCurrentState()
	{
		currentState = 0;
	}

	public void clearDesiredState()
	{
		desiredState = 0;
	}

	public void clearCurrentState(int toClear)
	{
		currentState &= ~toClear;
	}

	public void clearCurrentActionState(int toClear)
	{
		currentActionState &= ~toClear;
	}

	public void clearCurrentContextState(int toClear)
	{
		currentContextState &= ~toClear;
	}

	private bool wantToDo(int wantTo)
	{
		return (desiredState & wantTo) == wantTo;
	}

	private void clearDesire(int toClear)
	{
		desiredState &= ~toClear;
	}

	public bool amDoing(int wantTo)
	{
		return (currentState & wantTo) == wantTo;
	}

	public bool updateCapture()
	{
		if (amDoing(64))
		{
			myAttributes.captureTimeCurrent -= Time.deltaTime;
			if (!myInfo.mAmCaptured || wantToDo(2048) || (myInfo.mAmFakeCaptured && (wantToDo(2) || wantToDo(4) || wantToDo(1048576))))
			{
				if (!myInfo.mAmFakeCaptured)
				{
					myAttributes.invincibleTimeCurrent = myAttributes.invincibleTimeMax;
					myAttributes.healthCurrent = myAttributes.healthMax;
					if (myInfo.mIsLocal)
					{
						myInfo.SendChangeWeapon(myInfo.previousWeaponNotBlaster);
					}
				}
				myInfo.mNumGrenades = 3;
				setMoveState(1024);
				return false;
			}
			return false;
		}
		return true;
	}

	public void updateState()
	{
		if (!updateCapture())
		{
			return;
		}
		if (myInfo.mAmCaptured || myInfo.mAmFakeCaptured)
		{
			myInfo.mRangeLine.enabled = false;
			myAttributes.captureTimeCurrent = 25f;
			Logger.trace("<< can't shoot if i'm captured");
			clearCurrentActionState(1048576);
			if (myInfo.onGround)
			{
				clearCurrentContextState(268435456);
			}
			else
			{
				setContextState(268435456);
			}
			setMoveState(64);
			return;
		}
		if (wantToDo(4194304) && !gs.mHUD.mReloading)
		{
			WeaponScript weaponScript = myInfo.weapon.GetComponent("WeaponScript") as WeaponScript;
			if (weaponScript.numInClip < weaponScript.clipSize)
			{
				weaponScript.doReload(myInfo);
			}
		}
		if (!amDoing(32) && !wantToDo(32))
		{
			CheckShoot();
		}
		if (amDoing(8) && myInfo.verticalSpeed <= 0f)
		{
			setMoveState(8192);
		}
		else
		{
			if (amDoing(16384) && myInfo.GetComponent<Animation>().IsPlaying(myInfo.anim_jump_land.name))
			{
				return;
			}
			if (amDoing(4096))
			{
				myAttributes.airdashSpeed -= myAttributes.airdashSlowRate;
				if (myAttributes.airdashSpeed >= myAttributes.airdashSpeedDone)
				{
					return;
				}
			}
			if (amDoing(32) && myInfo.mIsLocal)
			{
				myInfo.mRangeLine.enabled = false;
			}
			if (amDoing(32) && !myInfo.GetComponent<Animation>().IsPlaying(myInfo.anim_airdash_f.name) && !myInfo.GetComponent<Animation>().IsPlaying(myInfo.anim_airdash_b.name))
			{
				if (myInfo.controller.collisionFlags == CollisionFlags.None)
				{
					Vector3 position = myInfo.transform.position;
					float num = Mathf.Abs(position.x - airdashStartPt.x);
					if (num > myAttributes.airdashDistanceMax)
					{
						myAttributes.airdashSpeed /= 2f;
						setMoveState(4096);
					}
					return;
				}
				setMoveState(512);
			}
			if (amDoing(256))
			{
				myInfo.velocity = Vector3.zero;
				myAttributes.rollRecoveryCurrent -= Time.deltaTime;
				if (!(myAttributes.rollRecoveryCurrent <= 0f))
				{
					return;
				}
				if (!wantToDo(1))
				{
					Vector3 direction = new Vector3(0f, 1f, 0f);
					Vector3 position2 = myInfo.transform.position;
					position2.y += 5f;
					Ray ray = new Ray(position2, direction);
					RaycastHit hitInfo;
					if (Physics.Raycast(ray, out hitInfo, 1000f) && hitInfo.distance < 6f)
					{
						return;
					}
				}
				clearCurrentState(256);
				if (!wantToDo(128))
				{
					myInfo.controller.height = 10f;
					CharacterController controller = myInfo.controller;
					Vector3 center = myInfo.controller.center;
					float x = center.x;
					Vector3 center2 = myInfo.controller.center;
					controller.center = new Vector3(x, 6f, center2.z);
					myInfo.controller.radius = 1.5f;
				}
			}
			if (amDoing(128))
			{
				if (!myInfo.GetComponent<Animation>().IsPlaying(myInfo.anim_roll.name))
				{
					if (myInfo.onGround)
					{
						myInfo.velocity = Vector3.zero;
						myAttributes.rollRecoveryCurrent = myAttributes.rollRecoveryMax;
						setMoveState(256);
						return;
					}
					myInfo.controller.height = 10f;
					CharacterController controller2 = myInfo.controller;
					Vector3 center3 = myInfo.controller.center;
					float x2 = center3.x;
					Vector3 center4 = myInfo.controller.center;
					controller2.center = new Vector3(x2, 6f, center4.z);
					myInfo.controller.radius = 1.5f;
					setMoveState(512);
				}
				return;
			}
			if (amDoing(1) && !wantToDo(1))
			{
				Vector3 direction2 = new Vector3(0f, 1f, 0f);
				Vector3 position3 = myInfo.transform.position;
				position3.y += 5f;
				Ray ray2 = new Ray(position3, direction2);
				RaycastHit hitInfo2;
				if (Physics.Raycast(ray2, out hitInfo2, 1000f) && hitInfo2.distance < 6f)
				{
					return;
				}
				myInfo.controller.height = 10f;
				CharacterController controller3 = myInfo.controller;
				Vector3 center5 = myInfo.controller.center;
				float x3 = center5.x;
				Vector3 center6 = myInfo.controller.center;
				controller3.center = new Vector3(x3, 6f, center6.z);
				myInfo.controller.radius = 1.5f;
			}
			if (amDoing(2097152))
			{
				clearCurrentState(2097152);
				previousActionState = 0;
			}
			if (myInfo.onGround)
			{
				if (myInfo.moveDir == 0 && (amDoing(8192) || amDoing(512)))
				{
					setMoveState(16384);
					return;
				}
				if (!myInfo.GetComponent<Animation>().IsPlaying(myInfo.anim_idle.name) && (desiredState & 0xFFFFF) == 0)
				{
					setMoveState(0);
					return;
				}
				if (wantToDo(7) && !amDoing(128))
				{
					myInfo.rollDir = myInfo.moveDir;
					if (myInfo.rollDir > 0)
					{
						myInfo.faceDir = (myInfo.faceTargetDir = 90f);
					}
					else
					{
						myInfo.faceDir = (myInfo.faceTargetDir = 270f);
					}
					myInfo.controller.height = 5f;
					CharacterController controller4 = myInfo.controller;
					Vector3 center7 = myInfo.controller.center;
					float x4 = center7.x;
					Vector3 center8 = myInfo.controller.center;
					controller4.center = new Vector3(x4, 3.5f, center8.z);
					myInfo.controller.radius = 1.5f;
					setMoveState(128);
					if (myInfo.mIsLocal)
					{
						gs.SendRoll();
					}
					return;
				}
				if (wantToDo(6) || wantToDo(32768))
				{
					if (movingBackwards())
					{
						setMoveState(32768);
					}
					else
					{
						setMoveState(6);
					}
				}
				if (!amDoing(1) && wantToDo(1))
				{
					setMoveState(1);
					myInfo.controller.height = 5f;
					CharacterController controller5 = myInfo.controller;
					Vector3 center9 = myInfo.controller.center;
					float x5 = center9.x;
					Vector3 center10 = myInfo.controller.center;
					controller5.center = new Vector3(x5, 3.5f, center10.z);
					myInfo.controller.radius = 1.5f;
				}
				if (wantToDo(8))
				{
					setMoveState(8);
					myInfo.verticalSpeed = 60f;
				}
			}
			else if (!amDoing(32) && wantToDo(32) && canAirDash && myInfo.fuelCurrent > 0f)
			{
				Vector3 position4 = myInfo.transform.position;
				float x6 = position4.x;
				Vector3 position5 = myInfo.transform.position;
				airdashStartPt = new Vector2(x6, position5.y);
				airdashDir = myInfo.moveDir;
				setMoveState(32);
				myInfo.airdashSpeed = myInfo.airdashSpeedMax;
				if (myInfo.mIsLocal)
				{
					gs.SendAirdash();
				}
				if (movingBackwards())
				{
					setContextState(536870912);
				}
				else
				{
					clearCurrentContextState(536870912);
				}
				return;
			}
			if (!amDoing(32) && !amDoing(4096) && wantToDo(16) && myInfo.fuelCurrent > 0f)
			{
				if (!amDoing(16))
				{
					myInfo.controller.height = 10f;
					CharacterController controller6 = myInfo.controller;
					Vector3 center11 = myInfo.controller.center;
					float x7 = center11.x;
					Vector3 center12 = myInfo.controller.center;
					controller6.center = new Vector3(x7, 6f, center12.z);
					myInfo.controller.radius = 1.5f;
					if (myInfo.onGround)
					{
						myInfo.jetpackAccel = myInfo.jetpackAccelGround;
					}
					else
					{
						myInfo.jetpackAccel = myInfo.jetpackAccelAir;
						Ray ray3 = new Ray(direction: new Vector3(0f, -5f, 0f), origin: myInfo.transform.position);
						RaycastHit hitInfo3;
						if (Physics.Raycast(ray3, out hitInfo3, 1000f) && hitInfo3.distance < myInfo.jetpackAirHeight)
						{
							myInfo.jetpackAccel = myInfo.jetpackAccelGround;
						}
					}
				}
				setMoveState(16);
			}
			else if (!myInfo.onGround && !amDoing(8) && !amDoing(32))
			{
				setMoveState(512);
			}
			if (!amDoing(32) && !amDoing(2097152) && wantToDo(2097152))
			{
				setActionState(2097152);
			}
			if (previousMoveState == 1 && currentMoveState != 1)
			{
				Logger.trace("<< was crouching");
			}
		}
	}

	public bool movingBackwards()
	{
		if ((myInfo.faceDir == 90f && (float)myInfo.moveDir < 0f) || (myInfo.faceDir == 270f && (float)myInfo.moveDir > 0f))
		{
			return true;
		}
		return false;
	}

	private void CheckShoot()
	{
		if (amDoing(128) || amDoing(32))
		{
			if (myInfo.mIsLocal)
			{
				myInfo.mRangeLine.enabled = false;
			}
			clearCurrentActionState(1048576);
			return;
		}
		GameObject gameObject = myInfo.transform.gameObject;
		string str = gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/weapon_holder/my_weapon/model/";
		GameObject gameObject2 = GameObject.Find(str + "Point01");
		str = gameObject.name + "/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm";
		GameObject gameObject3 = GameObject.Find(str);
		LocalControl localControl = myInfo.playerControl as LocalControl;
		float x = localControl.aimingTarget.x;
		Vector3 position = gameObject3.transform.position;
		float x2 = x - position.x;
		float y = localControl.aimingTarget.y;
		Vector3 position2 = gameObject3.transform.position;
		Vector2 v = new Vector3(x2, y - position2.y, 0f);
		Vector2 v2 = new Vector3(0f, 10f, 0f);
		myInfo.shotAngle = Vector3.Angle(v, v2);
		Player player = myInfo;
		Vector3 position3 = gameObject2.transform.position;
		float x3 = position3.x;
		Vector3 position4 = gameObject2.transform.position;
		player.shotPoint = new Vector3(x3, position4.y, 0f);
		if (myInfo.mIsLocal)
		{
			if (!myInfo.mAmReady || myInfo.mAmCaptured || !myInfo.mCanShoot)
			{
				myInfo.mRangeLine.enabled = false;
			}
			else
			{
				myInfo.mRangeLine.enabled = true;
				Ray ray = new Ray(myInfo.shotPoint, v);
				int num = 0;
				if (GameData.BattleType == 2)
				{
					num = ((myInfo.mFaction != 1) ? 4096 : 2048);
				}
				WeaponScript weaponScript = myInfo.weapon.GetComponent("WeaponScript") as WeaponScript;
				num |= 0x8000;
				num |= 0x4000;
				num |= 4;
				num = ~num;
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 1000f, num))
				{
					Debug.DrawLine(myInfo.shotPoint, hitInfo.point, Color.red);
					myInfo.mRangeLine.SetColors(Color.red, new Color(1f, 0.55f, 0f, 0.1f));
					Vector3 vector = hitInfo.point - myInfo.shotPoint;
					Vector3 position5 = myInfo.mRangeEnd = hitInfo.point;
					position5.z = 2f;
					Vector3 position6 = myInfo.mRangeStart = myInfo.shotPoint;
					position6.z = 2f;
					if (vector.magnitude > weaponScript.projectileRange)
					{
						vector.Normalize();
						position5.x = myInfo.shotPoint.x + vector.x * weaponScript.projectileRange;
						position5.y = myInfo.shotPoint.y + vector.y * weaponScript.projectileRange;
					}
					myInfo.mRangeLine.useWorldSpace = true;
					myInfo.mRangeLine.SetWidth(0.1f, 0.1f);
					myInfo.mRangeLine.SetPosition(0, position6);
					myInfo.mRangeLine.SetPosition(1, position5);
				}
				num = 0;
				num |= 0x4000;
				num |= 4;
				num |= 0x400;
				num |= 0x800;
				num |= 0x1000;
				num |= 0x2000;
				num = ~num;
				myInfo.mShotDistance = weaponScript.projectileRange;
				if (Physics.Raycast(ray, out hitInfo, 1000f, num) && hitInfo.collider.name.Contains("Collision"))
				{
					Vector3 vector2 = hitInfo.point - myInfo.shotPoint;
					Vector3 point = hitInfo.point;
					point.z = 2f;
					Vector3 shotPoint = myInfo.shotPoint;
					shotPoint.z = 2f;
					if (vector2.magnitude < weaponScript.projectileRange)
					{
						myInfo.mShotDistance = vector2.magnitude;
					}
				}
			}
		}
		if (myInfo.faceDir == 90f)
		{
			myInfo.shotAngle = 360f - myInfo.shotAngle;
		}
		if (wantToDo(1048576))
		{
			setActionState(1048576);
		}
		else
		{
			clearCurrentActionState(1048576);
		}
	}
}
