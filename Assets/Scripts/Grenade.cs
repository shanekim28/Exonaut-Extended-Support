using System;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	public delegate void FUNC();

	public bool isActive;

	public float shotAngle;

	public float timer;

	public float damage;

	public float splashDamage;

	public float currentScale;

	public float targetScale;

	public float expandTime;

	public int thrower;

	public int throwerID;

	public int grenadeNum;

	public float animRate;

	public float animTime;

	public float offset;

	public bool shouldAnimate = true;

	public FUNC step;

	private GamePlay gs;

	private void Awake()
	{
		isActive = true;
		timer = 2.5f;
		currentScale = 1f;
		targetScale = 20f;
		expandTime = 3f;
	}

	private void Start()
	{
		animRate = 0.2f;
		animTime = animRate;
		offset = 0f;
	}

	public void Throw()
	{
		gs = GamePlay.GetGamePlayScript();
		Logger.trace("<< Player " + thrower + " threw a grenade");
		step = Countdown;
	}

	private void Update()
	{
		if (shouldAnimate)
		{
			animTime -= Time.deltaTime;
			if (animTime <= 0f)
			{
				offset = ((offset != 0f) ? 0f : 0.5f);
				if (timer < 2f)
				{
					animRate = 71f / (678f * (float)Math.PI);
				}
				animTime = animRate;
				base.gameObject.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0f);
			}
		}
		if (step != null)
		{
			step();
		}
	}

	private void Countdown()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			step = StartExplode;
		}
	}

	public void StartExplode()
	{
		gs.SendGrenadeExplode(grenadeNum, base.transform.position, throwerID);
		base.gameObject.GetComponent<Rigidbody>().isKinematic = true;
		step = Explode;
		timer = 2f;
		Logger.trace("start explode");
	}

	private void WaitToExplode()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			Logger.trace("<< exploding hack");
			Explode();
		}
	}

	public void Explode()
	{
		Logger.trace("<< Starting Explosion");
		base.transform.GetComponent<Rigidbody>().isKinematic = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(gs.mGrenadeExplosion, base.transform.position, Quaternion.identity) as GameObject;
		if (!(gameObject.GetComponent<AudioSource>() == null))
		{
			gameObject.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
		}
		AudioSource[] componentsInChildren = gameObject.GetComponentsInChildren<AudioSource>();
		AudioSource[] array = componentsInChildren;
		foreach (AudioSource audioSource in array)
		{
			audioSource.volume = GameData.mGameSettings.mSoundVolume;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void LateUpdate()
	{
		Transform transform = base.transform;
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		transform.position = new Vector3(x, position2.y, 0f);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag != "Player" && other.gameObject.tag != "pickup")
		{
			base.gameObject.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
			base.gameObject.GetComponent<AudioSource>().Play();
		}
		else if (!other.gameObject.name.Contains("Collision"))
		{
			step = StartExplode;
			Logger.trace("i should damage " + other.gameObject.name);
			Logger.trace("<< colliding with: " + other.gameObject.name);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.name.Contains("Collision"))
		{
			Logger.trace("<< triggering collision with: " + other.gameObject.name);
		}
	}
}
