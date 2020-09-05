using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DamageTrigger : MonoBehaviour
{
	[Serializable]
	public class DamageResult
	{
		public GameObject mAffectedObject;

		public ObjectEffects mEffect;

		public AudioClip mSound;

		public Vector3 mOffsetPosition = Vector2.zero;
	}

	public enum ObjectEffects
	{
		DisableObject,
		ShieldWallEffect,
		SpawnObject
	}

	public DamageResult[] mObjectResults;

	public float mHealth = 1f;

	private void Start()
	{
		base.gameObject.tag = "power_wall";
	}

	private void OnEnable()
	{
		mHealth = 1f;
	}

	private void OnCollisionEnter(Collision Other)
	{
		GameObject gameObject = Other.gameObject;
		Grenade grenade = gameObject.GetComponent("Grenade") as Grenade;
		if (grenade != null)
		{
			grenade.StartExplode();
		}
	}

	public void ApplyWallDamage(float amount)
	{
		if (mHealth <= 0f)
		{
			return;
		}
		mHealth -= amount;
		if (mHealth <= 0f)
		{
			DamageResult[] array = mObjectResults;
			foreach (DamageResult res in array)
			{
				DoResult(res);
			}
		}
	}

	public void ApplyWallExplosion(Vector3 ExplosionPosition)
	{
		float num = 15f;
		if (!((ExplosionPosition - base.transform.position).sqrMagnitude > num * num))
		{
			ApplyWallDamage(20f);
		}
	}

	private void DoResult(DamageResult Res)
	{
		if (Res.mSound != null)
		{
			Vector3 position = Camera.main.transform.position + (Res.mAffectedObject.transform.position - Camera.main.transform.position).normalized * 5f;
			AudioSource.PlayClipAtPoint(Res.mSound, position, GameData.mGameSettings.mSoundVolume);
		}
		switch (Res.mEffect)
		{
		case ObjectEffects.DisableObject:
			Res.mAffectedObject.SetActiveRecursively(state: false);
			break;
		case ObjectEffects.ShieldWallEffect:
		{
			ShieldWall shieldWall = null;
			if ((bool)(shieldWall = (Res.mAffectedObject.GetComponent(typeof(ShieldWall)) as ShieldWall)))
			{
				shieldWall.KillTrigger();
			}
			break;
		}
		case ObjectEffects.SpawnObject:
			if (Res.mAffectedObject != null)
			{
				UnityEngine.Object.Instantiate(Res.mAffectedObject, base.transform.position + Res.mOffsetPosition, Quaternion.identity);
			}
			break;
		}
	}
}
