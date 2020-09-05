using UnityEngine;

public class PickUp : MonoBehaviour
{
	public int puType;

	public int mContents;

	public bool isActive;

	public float timeToRespawn;

	public float multiplier;

	public float effectTime;

	public bool isBoobyTrapped;

	public GameObject effect;

	public ParticleEmitter emitter;

	public float emitTimer;

	public int puIndex;

	public float mHeightOffGround;

	public float mHeightOffGroundLeft;

	public float mHeightOffGroundRight;

	public bool mIsWaitingForServer;

	private float timeToReactivate;

	private void Start()
	{
		timeToReactivate = 0f;
		isActive = true;
		isBoobyTrapped = false;
		GamePlay gamePlayScript = GamePlay.GetGamePlayScript();
		effect = (Object.Instantiate(gamePlayScript.pickupEffect, base.gameObject.transform.position, Quaternion.identity) as GameObject);
		emitter = (effect.GetComponent("ParticleEmitter") as ParticleEmitter);
		emitter.emit = false;
		mIsWaitingForServer = false;
		if (puType < 4)
		{
		}
		emitTimer = 0f;
		Vector3 direction = new Vector3(0f, -5f, 0f);
		Ray ray = new Ray(base.transform.position, direction);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo, 1000f);
		mHeightOffGround = hitInfo.distance;
		if (mHeightOffGround < 10f)
		{
			Vector3 position = base.transform.position;
			float x = position.x - 5f;
			Vector3 position2 = base.transform.position;
			Vector3 origin = new Vector3(x, position2.y, 0f);
			Ray ray2 = new Ray(origin, direction);
			Physics.Raycast(ray2, out hitInfo, 1000f);
			mHeightOffGroundLeft = hitInfo.distance;
			Vector3 position3 = base.transform.position;
			float x2 = position3.x + 5f;
			Vector3 position4 = base.transform.position;
			Vector3 origin2 = new Vector3(x2, position4.y, 0f);
			Ray ray3 = new Ray(origin2, direction);
			Physics.Raycast(ray3, out hitInfo, 1000f);
			mHeightOffGroundRight = hitInfo.distance;
			float num = mHeightOffGround;
			if (mHeightOffGround < mHeightOffGroundLeft)
			{
				num = ((!(mHeightOffGroundLeft < mHeightOffGroundRight)) ? mHeightOffGroundLeft : mHeightOffGroundRight);
			}
			else if (mHeightOffGround < mHeightOffGroundRight)
			{
				num = mHeightOffGroundRight;
			}
			if (num > 12f)
			{
				num = 12f;
			}
			Vector3 localScale = base.transform.localScale;
			float num2 = 1f / localScale.x;
			CapsuleCollider capsuleCollider = base.gameObject.GetComponent("CapsuleCollider") as CapsuleCollider;
			capsuleCollider.height = num * 2f * num2;
		}
	}

	private void Update()
	{
		if (emitTimer > 0f)
		{
			emitTimer -= Time.deltaTime;
			if (emitTimer <= 0f)
			{
				emitTimer = 0f;
				emitter.emit = false;
			}
		}
		if (isActive)
		{
			base.transform.Rotate(0f, 45f * Time.deltaTime, 0f);
		}
		else
		{
			if (!(timeToReactivate > 0f))
			{
				return;
			}
			if (timeToReactivate <= 3f && !emitter.emit)
			{
				emitTimer = timeToReactivate;
				emitter.emit = true;
			}
			timeToReactivate -= Time.deltaTime;
			if (!(timeToReactivate <= 0f))
			{
				return;
			}
			Logger.trace("<< reactivating");
			Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
			if (puType == 8)
			{
				mContents = Random.Range(0, 3);
				if (mContents < 2)
				{
					multiplier = 0.2f;
				}
				else
				{
					multiplier = 0.5f;
				}
			}
			isActive = true;
			if (mIsWaitingForServer)
			{
				Logger.trace("******* shouldn't be waiting for server anymore******");
				mIsWaitingForServer = false;
			}
		}
	}

	public void setContents(int type)
	{
		mContents = type;
	}

	public int getContents()
	{
		return mContents;
	}

	public bool playEffect()
	{
		if (!isActive)
		{
			return false;
		}
		emitter.emit = true;
		emitTimer = 2f;
		Logger.trace("Play Effect");
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = false;
		}
		isActive = false;
		mIsWaitingForServer = false;
		timeToReactivate = timeToRespawn;
		return true;
	}

	public void deactivatePickup(float time)
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = false;
		}
		isActive = false;
		timeToReactivate = time;
	}
}
