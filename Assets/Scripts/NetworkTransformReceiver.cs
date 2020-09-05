using Sfs2X.Entities.Data;
using System;
using System.Collections;
using UnityEngine;

public class NetworkTransformReceiver : MonoBehaviour
{
	private float yAdjust;

	private bool receiveMode;

	private NetworkTransform interpolateTo;

	private NetworkTransform interpolateFrom;

	private Queue queue = new Queue();

	public int queueCount;

	private float gameTime;

	private float lastTime;

	private int timeCount;

	private float lastMoveTime;

	private float localMessageCountTimer;

	private int localMessageCount;

	private int messageDropped;

	private Vector3 previousPosition;

	private Vector3 toMove;

	private float lastQueueTime;

	public float messageTimeDiff;

	public float positionUpdateDiff;

	public float lastPositionUpdateTime;

	public int suitIdx;

	public int weaponIdx;

	public float t;

	private void Start()
	{
		Logger.trace(string.Empty);
		Logger.trace("::::::::: Network Transform Receiver ::::::::::::");
		Logger.trace(string.Empty);
		suitIdx = -1;
		weaponIdx = -1;
	}

	private void StartReceiving()
	{
		Logger.trace(string.Empty);
		Logger.trace("::::::::: START RECEIVING ::::::::::::");
		Logger.trace(string.Empty);
		receiveMode = true;
	}

	private void FixedUpdate()
	{
		if (receiveMode)
		{
			InterpolateTransform();
		}
	}

	public void ReceiveSuitWeapon(SFSObject data)
	{
		Logger.trace(string.Empty);
		Logger.trace("::::::::: ReceiveSuitWeapon ::::::::::::");
		Logger.trace(data.GetDump());
		Logger.trace(string.Empty);
		suitIdx = Convert.ToInt32(data.GetInt("suitIdx"));
		weaponIdx = Convert.ToInt32(data.GetInt("weaponIdx"));
	}

	public void ReceiveEvent(SFSObject data)
	{
		Logger.trace(string.Empty);
		Logger.trace("::::::::: ReceiveEvent ::::::::::::");
		Logger.trace(data.GetDump());
		Logger.trace(string.Empty);
		GamePlay gamePlayScript = GamePlay.GetGamePlayScript();
		Player player = GetComponent("Player") as Player;
		int num = Convert.ToInt32(data.GetInt("playerId"));
		int num2 = Convert.ToInt32(data.GetInt("msgType"));
		switch (num2)
		{
		case 21:
		case 120:
		case 200:
			break;
		case 1:
			ReceiveSuitWeapon(data);
			break;
		case 101:
			gamePlayScript.serverReady = true;
			Logger.trace("<<  <<  should start game >> >>");
			break;
		case 40:
			Logger.trace("<<  <<  ignoring spawn message for player:" + num.ToString() + " >> >>");
			break;
		case 23:
		{
			if (!player.mIsLocal)
			{
				break;
			}
			int num15 = Convert.ToInt32(data.GetInt("oppId"));
			int num16 = Convert.ToInt32(data.GetInt("currHealth"));
			int num17 = Convert.ToInt32(data.GetInt("boost"));
			int mNumCaptures = Convert.ToInt32(data.GetInt("captures"));
			int idx = Convert.ToInt32(data.GetInt("weaponIdx"));
			Player player3 = gamePlayScript.players[num15 - 1].GetComponent("Player") as Player;
			player3.healthCurrent = num16;
			player3.mNumCaptures = mNumCaptures;
			player3.changeWeapon(idx);
			if (num17 > 0)
			{
				int num18 = num17 >> 16;
				if ((num18 & 0x8000) > 0)
				{
					num18 &= 0x7FFF;
					player3.activateBoostsInProgress(num18);
				}
				num18 = (num17 & 0xFFFF);
				if ((num18 & 0x8000) > 0)
				{
					num18 &= 0x7FFF;
					player3.activateBoostsInProgress(num18);
				}
			}
			break;
		}
		case 25:
			if (player.mIsLocal)
			{
				int puIdx2 = Convert.ToInt32(data.GetInt("boostIdx"));
				float puTime = Convert.ToSingle(data.GetFloat("boostTime"));
				gamePlayScript.setPickupActive(puIdx2, puTime);
			}
			break;
		case 4:
		{
			Logger.trace("<< myPlayer name: " + player.name);
			float shotAngle = Convert.ToSingle(data.GetFloat("angle"));
			float angleIncrement = Convert.ToSingle(data.GetFloat("inc"));
			int bnum = Convert.ToInt32(data.GetInt("bnum"));
			Vector3 startPt = new Vector3(Convert.ToSingle(data.GetFloat("x")), Convert.ToSingle(data.GetFloat("y")) + yAdjust, 0f);
			Logger.trace("<< " + base.gameObject.name + " weapon: " + player.weaponIdx);
			player.remoteShoot(startPt, shotAngle, angleIncrement, bnum);
			break;
		}
		case 5:
		{
			float ang = Convert.ToSingle(data.GetFloat("angle"));
			int isGun = Convert.ToInt32(data.GetInt("type"));
			int num19 = Convert.ToInt32(data.GetInt("num"));
			if (num != GameData.MyPlayerId)
			{
				Logger.trace("<< someone else telling me to throw");
			}
			Vector3 pos = new Vector3(Convert.ToSingle(data.GetFloat("x")), Convert.ToSingle(data.GetFloat("y")) + yAdjust, 0f);
			player.ThrowGrenade(pos, ang, isGun, num19);
			break;
		}
		case 9:
		{
			Logger.trace("received explode message");
			int num14 = Convert.ToInt32(data.GetInt("num"));
			Logger.trace("<< trying to find grenade num " + num14);
			GameObject gameObject6 = GameObject.Find("grenade_" + num14.ToString());
			if (gameObject6 != null)
			{
				Grenade grenade = gameObject6.GetComponent("Grenade") as Grenade;
				grenade.Explode();
			}
			break;
		}
		case 24:
			if (player.mIsLocal)
			{
				gamePlayScript.mGameTimer = Convert.ToSingle(data.GetFloat("time"));
				gamePlayScript.serverReady = true;
			}
			break;
		case 6:
		{
			int num10 = Convert.ToInt32(data.GetInt("idx"));
			Logger.trace("<< need to change weapon " + num10);
			if (player != null)
			{
				player.changeWeapon(num10);
			}
			break;
		}
		case 18:
		{
			float x2 = Convert.ToSingle(data.GetFloat("xstart"));
			float y2 = Convert.ToSingle(data.GetFloat("ystart"));
			float x3 = Convert.ToSingle(data.GetFloat("xend"));
			float y3 = Convert.ToSingle(data.GetFloat("yend"));
			gamePlayScript.DrawSniperLine(x2, y2, x3, y3);
			break;
		}
		case 8:
		{
			int num6 = Convert.ToInt32(data.GetInt("myIdx"));
			int num7 = Convert.ToInt32(data.GetInt("pType"));
			int puIdx = Convert.ToInt32(data.GetInt("pIdx"));
			Logger.trace("<< receiving pickup " + num6 + "  of type " + num7);
			gamePlayScript.activatePickup(puIdx, num7, num6);
			break;
		}
		case 10:
		{
			float damage = Convert.ToSingle(data.GetFloat("damage"));
			int num11 = Convert.ToInt32(data.GetInt("playerId"));
			int attackerId = Convert.ToInt32(data.GetInt("uAttackerID"));
			int num12 = Convert.ToInt32(data.GetInt("bnum"));
			int num13 = Convert.ToInt32(data.GetInt("hs"));
			float health = Convert.ToSingle(data.GetFloat("health"));
			if (num13 == 1)
			{
				Logger.trace("<< shot was a HEADSHOT!");
			}
			string text2 = "bullet_" + num12;
			Debug.Log("<< damage done by bullet " + text2);
			if (num12 >= 0)
			{
				GameObject gameObject4 = GameObject.Find(text2);
				Debug.Log("<< should destroy bullet " + num12);
				if (gameObject4 != null)
				{
					Debug.Log("<< should be destroying bullet " + text2);
					UnityEngine.Object.Destroy(gameObject4);
				}
				else
				{
					Debug.Log("<< Couldn't find Bullet " + text2);
					gamePlayScript.addBulletNotToAdd(num12);
				}
			}
			GameObject gameObject5 = gamePlayScript.players[num11];
			if (!(gameObject5 != null))
			{
				break;
			}
			Player player2 = gameObject5.GetComponent("Player") as Player;
			if (player2 != null)
			{
				player2.ApplyDamage(damage, health, num13 == 1, attackerId);
				if (gamePlayScript != null && !(gamePlayScript.mHUD != null))
				{
				}
			}
			break;
		}
		case 16:
		{
			Logger.trace("received rocket explode message");
			int num9 = Convert.ToInt32(data.GetInt("num"));
			float x = Convert.ToSingle(data.GetFloat("x"));
			float y = Convert.ToSingle(data.GetFloat("y"));
			string text = "bullet_" + num9;
			Logger.trace("<< trying to find  " + text);
			GameObject gameObject2 = GameObject.Find(text);
			if (gameObject2 == null)
			{
				Debug.Log("couldn't find bullet");
				GameObject gameObject3 = UnityEngine.Object.Instantiate(gamePlayScript.rocket, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				Projectile projectile = gameObject3.GetComponent("Projectile") as Projectile;
				projectile.transform.position = new Vector3(x, y, 0f);
				projectile.Explode();
			}
			else
			{
				Projectile projectile2 = gameObject2.GetComponent("Projectile") as Projectile;
				projectile2.transform.position = new Vector3(x, y, 0f);
				projectile2.Explode();
			}
			break;
		}
		case 17:
		{
			int num8 = Convert.ToInt32(data.GetInt("num"));
			player.ShowTaunt(num8);
			break;
		}
		case 20:
		{
			int num3 = Convert.ToInt32(data.GetInt("uAttackerID"));
			int num4 = Convert.ToInt32(data.GetInt("playerId"));
			int num5 = Convert.ToInt32(data.GetInt("bnum"));
			if (num5 >= 0)
			{
				GameObject gameObject = GameObject.Find("bullet_" + num5);
				if (gameObject != null)
				{
					Logger.trace("<< killing bullet " + num5 + " because it collided with player");
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			gamePlayScript.mHUD.AddMessage(GameData.trimDisplayName(gamePlayScript.m_networkManager.getNameByID(num3 + 1)) + " hacked " + GameData.trimDisplayName(gamePlayScript.m_networkManager.getNameByID(num4 + 1)));
			break;
		}
		case 50:
			if (!(base.gameObject.name != "localPlayer"))
			{
				Logger.trace("<< play beep because can't perform action");
			}
			break;
		case 22:
		{
			int pickupType = Convert.ToInt32(data.GetInt("uPickup"));
			player.deactivatePickupOnPlayer(pickupType);
			break;
		}
		default:
			Logger.traceError("<< UNHANDLED MESSAGE : " + num2);
			break;
		}
	}

	public void ReceiveTransform(SFSObject data)
	{
		Player player = base.gameObject.GetComponent("Player") as Player;
		if (receiveMode && player.mAmReady)
		{
			Vector3 pos = new Vector3(Convert.ToSingle(data.GetFloat("x")), Convert.ToSingle(data.GetFloat("y")) + yAdjust, 0f);
			int currentState = Convert.ToInt32(data.GetInt("moveState"));
			float armAngle = Convert.ToSingle(data.GetFloat("armAngle"));
			int moveDir = Convert.ToInt32(data.GetInt("moveDir"));
			float faceTargetDir = Convert.ToSingle(data.GetFloat("faceTargetDir"));
			NetworkTransform networkTransform = new NetworkTransform(base.gameObject);
			networkTransform.InitFromValues(pos, currentState, armAngle, moveDir, faceTargetDir);
			queue.Enqueue(networkTransform);
		}
	}

	private void InterpolateTransform()
	{
		int count = queue.Count;
		if (interpolateTo == null)
		{
			interpolateTo = new NetworkTransform(base.gameObject);
		}
		if (interpolateFrom == null)
		{
			interpolateFrom = new NetworkTransform(base.gameObject);
		}
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					interpolateFrom.position = interpolateTo.position;
				}
				else
				{
					interpolateFrom.position = base.transform.position;
				}
				interpolateTo = (queue.Dequeue() as NetworkTransform);
			}
		}
		Player player = GetComponent("Player") as Player;
		if (!(player == null))
		{
			t = 0.1f;
			t = 0.3f;
			if ((interpolateTo.currentState & 0xFFFFF & 8) == 8 || (interpolateTo.currentState & 0xFFFFF & 0x4000) == 16384 || (interpolateTo.currentState & 0xFFFFF & 0x2000) == 8192)
			{
				base.transform.position = interpolateTo.position;
			}
			else
			{
				base.transform.position = Vector3.Lerp(base.transform.position, interpolateTo.position, t);
			}
			player.moveDir = interpolateTo.moveDir;
			player.faceTargetDir = interpolateTo.faceTargetDir;
			player.myState.currentMoveState = (interpolateTo.currentState & 0xFFFFF);
			player.myState.currentArmState = (interpolateTo.currentState & 0xF000000);
			player.myState.currentActionState = 0;
			player.myState.currentContextState = (interpolateTo.currentState & 0x70000000);
			player.armAngle = interpolateTo.armAngle;
		}
	}

	private void OnGUI()
	{
	}
}
