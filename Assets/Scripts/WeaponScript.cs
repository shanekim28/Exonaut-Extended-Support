using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
	public int numProjectiles;

	public float angleRange;

	public int clipSize;

	[NonSerialized]
	public int numInClip;

	public int numClips;

	public float fireRate;

	public float fireRateAdj;

	public float timeUntilFire;

	public float reloadRate;

	public float reloadRateAdj;

	public float projectileRange;

	public float projectileAccuracy;

	public int totalBullets;

	public float projectileSpeed;

	public int damage_per_projectile;

	private int damageAdj;

	public int projectileType;

	public float usageTime;

	private GameObject go;

	private GamePlay gs;

	private GameObject proj;

	public Player p;

	public bool waitToFire;

	private void Awake()
	{
		gs = GamePlay.GetGamePlayScript();
	}

	private void Start()
	{
		timeUntilFire = usageTime;
		waitToFire = false;
		if (p.mIsLocal && p.weaponIdx == 1)
		{
			doReload(p);
		}
	}

	private void Update()
	{
		if (p == null)
		{
		}
		if (timeUntilFire > 0f)
		{
			timeUntilFire -= Time.deltaTime;
			if (timeUntilFire < 0f)
			{
				timeUntilFire = 0f;
			}
		}
	}

	public bool CanFire()
	{
		bool result = true;
		if (waitToFire || timeUntilFire > 0f || numInClip == 0)
		{
			result = false;
		}
		return result;
	}

	public void WaitToFire(bool toSet)
	{
		waitToFire = toSet;
	}

	public int Fire(Vector3 firePos, float shotAngle, string myName, Player shootPlayer, float distance)
	{
		if (timeUntilFire > 0f || gs.mHUD.AmReloading())
		{
			if (gs.mHUD.AmReloading() && timeUntilFire <= 0f)
			{
				Logger.trace("****************  WOULD HAVE FIRED DURING RELOAD **************");
			}
			return 0;
		}
		if (!shootPlayer.mIsLocal)
		{
			return 0;
		}
		timeUntilFire = fireRate;
		float num = angleRange / 2f;
		float angleIncrement = 0f;
		if (numProjectiles > 1)
		{
			angleIncrement = angleRange / (float)(numProjectiles - 1);
		}
		if (angleRange == 360f)
		{
			angleIncrement = angleRange / (float)numProjectiles;
		}
		Logger.trace("my name: " + myName + " is firing " + numProjectiles + " bullets ");
		float num2 = projectileAccuracy;
		float num3 = 0f - num2 / 2f + UnityEngine.Random.value * num2;
		shotAngle += num3 + num;
		Logger.trace("<< should travel " + distance + " distance ");
		if (shootPlayer.mIsLocal)
		{
			Logger.trace("<< shooting at angle: " + shotAngle);
			gs.SendMyShotPosition(firePos, shotAngle, angleIncrement, distance);
		}
		return spendAmmo(shootPlayer);
	}

	public int spendAmmo(Player me)
	{
		if (!me.mIsLocal)
		{
			return 1;
		}
		numInClip--;
		waitToFire = false;
		timeUntilFire = fireRate;
		totalBullets--;
		if (numInClip == 0)
		{
			Logger.trace("<< need to reload: num shots left - " + totalBullets + "   -- num clips left: " + numClips);
			doReload(me);
		}
		return 1;
	}

	public void doReload(Player me)
	{
		timeUntilFire = reloadRate + reloadRateAdj;
		if (me.mIsLocal)
		{
			gs.mHUD.startReload(timeUntilFire);
		}
		Logger.trace("<< num clips: " + numClips + "    ---  clipSize: " + clipSize);
		me.GetComponent<AudioSource>().PlayOneShot(gs.reloadSounds[0]);
	}

	public int fireBullet(Vector3 firePos, float shotAngle, float angleIncrement, string myName, int bnum, Player me)
	{
		Logger.trace("<< " + myName + " is firing " + numProjectiles + " at angle " + angleIncrement);
		bool isRocket = false;
		if (me.weaponIdx != 8)
		{
			proj = gs.bullet;
		}
		else
		{
			proj = gs.rocket;
			isRocket = true;
		}
		for (int i = 0; i < numProjectiles; i++)
		{
			if (gs.removeBulletNotToAdd(bnum + i))
			{
				Logger.trace("<<< Not intantiating bullet number " + (bnum + i));
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(proj, new Vector3(firePos.x, firePos.y, 0f), Quaternion.identity) as GameObject;
			gameObject.tag = "bulletTag";
			gameObject.name = "bullet_" + (bnum + i);
			Projectile projectile = gameObject.GetComponent("Projectile") as Projectile;
			gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, shotAngle));
			shotAngle -= angleIncrement;
			projectile.travelRange = projectileRange;
			projectile.startPos = new Vector3(firePos.x, firePos.y, firePos.z);
			projectile.damage = damage_per_projectile;
			projectile.idx = bnum + i;
			projectile.isRocket = isRocket;
		}
		Player player = me.GetComponent("Player") as Player;
		player.muzzleflashOn(firePos, shotAngle);
		return 0;
	}
}
