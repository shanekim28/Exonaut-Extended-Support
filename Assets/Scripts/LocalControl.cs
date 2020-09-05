using UnityEngine;

public class LocalControl : Control
{
	private Player me;

	public Vector3 aimingTarget;

	public float aimingAngle;

	private int lastDirDown;

	private float aimingValueTarget;

	private float aimingValue;

	private float aimingValueCurrent;

	private float aimingTimer;

	private GamePlay gs;

	public CNInputManager inputManager;

	private ContextualHelp help;

	public LocalControl(Player m)
	{
		me = m;
		controlType = 0;
		inputManager = new CNInputManager();
		aimingTimer = 0.5f;
		aimingValueCurrent = 0f;
		gs = GamePlay.GetGamePlayScript();
		inputManager.addCommand(GameData.mGameSettings.mControlValues[2], 2);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[3], 4);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[0], 8);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[1], 1);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[5], 1048576);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[6], 16);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[4], 2097152);
		inputManager.addCommand(GameData.mGameSettings.mControlValues[7], 4194304);
		inputManager.addCommand(KeyCode.Space, 524288);
		inputManager.addCommand(KeyCode.LeftArrow, 2);
		inputManager.addCommand(KeyCode.RightArrow, 4);
		inputManager.addCommand(KeyCode.UpArrow, 8);
		inputManager.addCommand(KeyCode.DownArrow, 1);
	}

	public void setHelp(ContextualHelp ch)
	{
		help = ch;
		if (help == null)
		{
			Logger.trace("************************************* help is null *************************");
		}
	}

	public override void Update()
	{
		if (!Screen.fullScreen)
		{
			Vector3 mousePosition = Input.mousePosition;
			if (!(mousePosition.x < 0f))
			{
				Vector3 mousePosition2 = Input.mousePosition;
				if (!(mousePosition2.y < 0f))
				{
					Vector3 mousePosition3 = Input.mousePosition;
					if (!(mousePosition3.x > (float)Screen.width))
					{
						Vector3 mousePosition4 = Input.mousePosition;
						if (!(mousePosition4.y > (float)Screen.height))
						{
							goto IL_0095;
						}
					}
				}
			}
			me.mInactiveCounter += Time.deltaTime;
			me.myState.desiredState = 0;
			return;
		}
		goto IL_0095;
		IL_0095:
		if (me.captureBubble != null)
		{
			me.myState.desiredState = 0;
			return;
		}
		Vector3 position = me.transform.position;
		float z = position.z;
		Vector3 position2 = Camera.main.transform.position;
		float num = z - position2.z;
		Camera main = Camera.main;
		Vector3 mousePosition5 = Input.mousePosition;
		float x = mousePosition5.x;
		Vector3 mousePosition6 = Input.mousePosition;
		Vector3 vector = main.ScreenToWorldPoint(new Vector3(x, mousePosition6.y, Camera.main.nearClipPlane + num));
		if (me.myState.amDoing(128))
		{
			me.myState.desiredState = 128;
			return;
		}
		aimingTimer -= Time.deltaTime;
		if (aimingTimer <= 0f)
		{
			Transform transform = me.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle");
			aimingTarget = vector;
			aimingAngle = 0f;
			me.aimingWorldTarget = aimingTarget;
			float x2 = aimingTarget.x;
			Vector3 position3 = me.gameObject.transform.position;
			if (x2 > position3.x)
			{
				me.faceTargetDir = 90f;
			}
			else
			{
				me.faceTargetDir = 270f;
			}
			float x3 = aimingTarget.x;
			Vector3 position4 = transform.position;
			float x4 = x3 - position4.x;
			float y = aimingTarget.y;
			Vector3 position5 = transform.position;
			Vector2 v = new Vector3(x4, y - position5.y, 0f);
			Vector2 v2 = new Vector3(0f, 1f, 0f);
			aimingAngle = Vector3.Angle(v, v2);
			aimingAngle = Mathf.Floor(aimingAngle);
			me.myState.currentActionState = 0;
			if (aimingAngle < 90f)
			{
				aimingValue = (90f - (90f - aimingAngle) * 0.8f) / 180f;
			}
			else
			{
				aimingValue = (90f + (aimingAngle - 90f) * 0.8f) / 180f;
			}
			switch (me.myState.currentMoveState)
			{
			case 2:
			case 4:
			case 6:
				aimingValue -= 0.05f;
				break;
			case 256:
				aimingValue -= 0.1f;
				break;
			case 16:
				aimingValue -= 0.1f;
				break;
			case 8:
				aimingValue -= 0.1f;
				break;
			case 32:
				if ((me.myState.currentContextState & 0x20000000) == 536870912)
				{
					aimingValue += 0.2f;
				}
				else
				{
					aimingValue -= 0.2f;
				}
				break;
			}
			aimingValueTarget = aimingValue;
			aimingTimer = 0.15f;
		}
		aimingValueCurrent = me.armAngle;
		if (aimingValueCurrent != aimingValueTarget)
		{
			float num2 = (!(aimingValueTarget > aimingValueCurrent)) ? (-1f) : 1f;
			aimingValueCurrent += 0.04f * num2;
			if (num2 > 0f)
			{
				if (aimingValueCurrent > aimingValueTarget)
				{
					aimingValueCurrent = aimingValueTarget;
				}
			}
			else if (aimingValueCurrent < aimingValueTarget)
			{
				aimingValueCurrent = aimingValueTarget;
			}
			me.armAngle = aimingValueCurrent;
		}
		inputManager.Update();
		int num3 = 0;
		if ((inputManager.btnDoubleTapped & 2) == 2 || (inputManager.btnDoubleTapped & 4) == 4)
		{
			num3 |= 0x20;
		}
		me.moveDir = 0;
		if (inputManager.buttonPressed(2))
		{
			me.moveDir = -1;
		}
		if (inputManager.buttonPressed(4))
		{
			me.moveDir = 1;
		}
		if (inputManager.buttonPressed(6))
		{
			me.moveDir = lastDirDown;
		}
		if (inputManager.buttonJustPressed(2))
		{
			me.moveDir = (lastDirDown = -1);
		}
		if (inputManager.buttonJustPressed(4))
		{
			me.moveDir = (lastDirDown = 1);
		}
		if (inputManager.buttonPressed(1))
		{
			num3 |= 1;
		}
		if (inputManager.buttonJustPressed(8))
		{
			num3 |= 8;
		}
		if (inputManager.buttonJustPressed(4194304))
		{
			num3 |= 0x400000;
		}
		if (me.moveDir != 0)
		{
			num3 |= 6;
		}
		if (inputManager.buttonJustPressed(2097152) && !me.mAmSniping)
		{
			num3 |= 0x200000;
		}
		bool flag = true;
		if (me.weaponIdx == 7 && !me.mAmSniping)
		{
			flag = false;
			if (inputManager.buttonJustReleased(1048576))
			{
				me.mAmSniping = true;
				me.GetComponent<AudioSource>().PlayOneShot(gs.activateSniperSnd);
			}
		}
		if (GamePlay.mPauseScreenActive)
		{
			flag = false;
		}
		if (inputManager.buttonPressed(1048576) && flag)
		{
			num3 |= 0x100000;
		}
		if (inputManager.buttonPressed(16))
		{
			num3 |= 0x10;
		}
		if (inputManager.buttonDoubleTapped(32))
		{
			num3 |= 0x20;
		}
		if (num3 == 0)
		{
			num3 = 0;
		}
		if (inputManager.buttonJustPressed(524288) && me.mAmSniping)
		{
			me.mAmSniping = false;
			CameraScrolling cameraScrolling = Camera.main.GetComponent("CameraScrolling") as CameraScrolling;
			cameraScrolling.SetTarget(me.transform);
			me.GetComponent<AudioSource>().PlayOneShot(gs.deactivateSniperSnd);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			gs.SendTaunt(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			gs.SendTaunt(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			gs.SendTaunt(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			gs.SendTaunt(3);
		}
		if (num3 == 0)
		{
			me.mInactiveCounter += Time.deltaTime;
		}
		else
		{
			me.mInactiveCounter = 0f;
		}
		if (help != null)
		{
			if ((num3 & 0xF) != 0)
			{
				me.gameObject.SendMessage("SetMoveHelp");
			}
			if ((num3 & 0x10) != 0)
			{
				me.gameObject.SendMessage("SetJetpackHelp");
			}
			if ((num3 & 0x100000) != 0)
			{
				me.gameObject.SendMessage("SetShootingHelp");
			}
			if ((num3 & 0x200000) != 0)
			{
				me.gameObject.SendMessage("SetGrenadeHelp");
			}
		}
		me.myState.desiredState = num3;
	}
}
