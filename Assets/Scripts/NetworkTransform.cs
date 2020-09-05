using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

public class NetworkTransform
{
	public Vector3 position;

	private GameObject obj;

	public int currentState;

	public float armAngle;

	public int moveDir;

	public float faceTargetDir;

	public int messageNum;

	public float gameTime;

	public float averageTime;

	public ExtensionRequest er;

	private SmartFox client;

	private SFSObject data;

	private NetworkManager m_networkManager;

	public NetworkTransform(GameObject obj)
	{
		this.obj = obj;
		InitFromCurrent();
		messageNum = 0;
		GameObject gameObject = GameObject.Find("NetworkManager");
		m_networkManager = (gameObject.GetComponent("NetworkManager") as NetworkManager);
	}

	public bool UpdateIfDifferent()
	{
		InitFromCurrent();
		return true;
	}

	public bool UpdateIfStateChange()
	{
		Player player = obj.GetComponent("Player") as Player;
		if (player == null)
		{
			return false;
		}
		if (player.myState == null)
		{
			return false;
		}
		if (player.armAngle == armAngle && (player.myState.currentMoveState | player.myState.currentArmState) == currentState)
		{
			return false;
		}
		return true;
	}

	public void DoSend()
	{
		if (GameData.GameRoom == null)
		{
			return;
		}
		if (client == null)
		{
			client = SmartFoxConnection.Connection;
			if (client == null)
			{
				return;
			}
		}
		Player p = obj.GetComponent("Player") as Player;
		m_networkManager.sendStateUpdate(p);
	}

	public void DoSendEvt()
	{
		SmartFox connection = SmartFoxConnection.Connection;
		SFSObject sFSObject = new SFSObject();
		sFSObject.PutInt("playerId", connection.MySelf.Id);
		connection.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
	}

	public void InitFromValues(Vector3 pos, int currentState, float armAngle, int moveDir, float faceTargetDir)
	{
		position = pos;
		this.currentState = currentState;
		this.armAngle = armAngle;
		this.moveDir = moveDir;
		this.faceTargetDir = faceTargetDir;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		Transform transform = obj as Transform;
		NetworkTransform networkTransform = obj as NetworkTransform;
		if (transform != null)
		{
			return transform.position == position;
		}
		if (networkTransform != null)
		{
			return networkTransform.position == position;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	private void InitFromCurrent()
	{
		position = obj.transform.position;
		Player player = obj.GetComponent("Player") as Player;
		if (player != null && player.myState != null)
		{
			armAngle = player.armAngle;
			currentState = ((player.myState.currentMoveState & 0xFFFFF) | (player.myState.currentArmState & 0xF000000) | (player.myState.currentActionState & 0xF00000) | (player.myState.currentContextState & 0x70000000));
			moveDir = player.moveDir;
			faceTargetDir = player.faceTargetDir;
		}
	}

	private void InitFromGiven(Transform trans)
	{
		position = trans.position;
	}
}
