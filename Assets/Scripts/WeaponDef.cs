using LitJson;
using System;

public class WeaponDef
{
	public int id;

	public string name;

	public string description;

	public int numProjectiles;

	public float angleRange;

	public int clipSize;

	public int numClips;

	public float fireRate;

	public float reloadRate;

	public float projectileRange;

	public float projectileAccuracy;

	public int damage_per_projectile;

	public int projectileType;

	public float usageTime;

	public int radius1;

	public int radius1Damage;

	public int radius2;

	public int radius2Damage;

	public float expireTime;

	public int velocity;

	public WeaponDef(string name, int numProjectiles, float angleRange, int clipSize, int numClips, float fireRate, float reloadRate, float projectileRange, float projectileAccuracy, int damage_per_projectile, int projectileType, float usageTime)
	{
		this.name = name;
		this.numProjectiles = numProjectiles;
		this.angleRange = angleRange;
		this.clipSize = clipSize;
		this.numClips = numClips;
		this.fireRate = fireRate;
		this.reloadRate = reloadRate;
		this.projectileRange = projectileRange;
		this.projectileAccuracy = projectileAccuracy;
		this.damage_per_projectile = damage_per_projectile;
		this.projectileType = projectileType;
		this.usageTime = usageTime;
	}

	public WeaponDef(JsonData weaponData)
	{
		id = Convert.ToInt32((string)weaponData["ID"]);
		name = (string)weaponData["Name"];
		description = (string)weaponData["Description"];
		projectileType = Convert.ToInt32((string)weaponData["Projectile_Type"]);
		numProjectiles = Convert.ToInt32((string)weaponData["Projectiles"]);
		angleRange = Convert.ToSingle((string)weaponData["Angle"]);
		clipSize = Convert.ToInt32((string)weaponData["Clip_Size"]);
		numClips = Convert.ToInt32((string)weaponData["Num_Clips"]);
		fireRate = Convert.ToSingle((string)weaponData["Fire_Rate"]);
		reloadRate = Convert.ToSingle((string)weaponData["Reload_Rate"]);
		projectileRange = Convert.ToSingle((string)weaponData["Range"]);
		projectileAccuracy = Convert.ToSingle((string)weaponData["Accuracy"]);
		damage_per_projectile = Convert.ToInt32((string)weaponData["Damage"]);
		radius1 = Convert.ToInt32((string)weaponData["Radius1"]);
		radius1Damage = Convert.ToInt32((string)weaponData["Radius1_Damage"]);
		radius2 = Convert.ToInt32((string)weaponData["Radius2"]);
		radius2Damage = Convert.ToInt32((string)weaponData["Radius2_Damage"]);
		expireTime = Convert.ToSingle((string)weaponData["Expire_Time"]);
		velocity = Convert.ToInt32((string)weaponData["Velocity"]);
	}
}
