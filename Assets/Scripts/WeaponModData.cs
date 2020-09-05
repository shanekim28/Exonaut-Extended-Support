using LitJson;
using System;

public class WeaponModData
{
	public int mId;

	public string mName;

	public string mDescription;

	public int mWeaponIdx;

	public int mModNumProjectiles;

	public int mModAngleRange;

	public int mModClipSize;

	public int mModNumClips;

	public float mModFireRate;

	public float mModReloadRate;

	public int mModProjectileRange;

	public int mModProjectileAccuracy;

	public int mModDamagePerProjectile;

	public int mModProjectileType;

	public int mModUsageTime;

	public WeaponModData(int id, int weaponIdx, int numProjectiles, int angleRange, int clipSize, int numClips, float fireRate, float reloadRate, int projectileRange, int projectileAccuracy, int damagePerProjectile, int projectileType)
	{
		mId = id;
		mWeaponIdx = weaponIdx;
		mModNumProjectiles = numProjectiles;
		mModAngleRange = angleRange;
		mModClipSize = clipSize;
		mModNumClips = numClips;
		mModFireRate = fireRate;
		mModReloadRate = reloadRate;
		mModProjectileRange = projectileRange;
		mModProjectileAccuracy = projectileAccuracy;
		mModDamagePerProjectile = damagePerProjectile;
		mModProjectileType = projectileType;
	}

	public WeaponModData(JsonData weaponData)
	{
		mId = Convert.ToInt32((string)weaponData["ID"]);
		mWeaponIdx = Convert.ToInt32((string)weaponData["WeaponID"]);
		mName = (string)weaponData["Name"];
		mDescription = (string)weaponData["Description"];
		mModNumProjectiles = Convert.ToInt32((string)weaponData["Num_Projectiles"]);
		mModAngleRange = Convert.ToInt32((string)weaponData["Angle_Range"]);
		mModClipSize = Convert.ToInt32((string)weaponData["Clip_Size"]);
		mModNumClips = Convert.ToInt32((string)weaponData["Num_Clips"]);
		mModFireRate = Convert.ToSingle((string)weaponData["Fire_Rate"]);
		mModReloadRate = Convert.ToSingle((string)weaponData["Reload_Rate"]);
		mModProjectileRange = Convert.ToInt32((string)weaponData["Projectile_Range"]);
		mModProjectileAccuracy = Convert.ToInt32((string)weaponData["Projectile_Accuracy"]);
		mModDamagePerProjectile = Convert.ToInt32((string)weaponData["Damage_Per_Projectile"]);
		mModProjectileType = Convert.ToInt32((string)weaponData["Projectile_Type"]);
	}
}
