using UnityEngine;

public class Projectile : MonoBehaviour
{
	public bool isActive;

	public Vector3 startPos;

	public Vector3 endPos;

	public float damage;

	public float splashDamage;

	public float travelRange;

	public int shootingPower;

	public bool isLaser;

	public int idx;

	private GamePlay gs;

	public bool isRocket;

	private void Awake()
	{
		isActive = true;
	}

	private void Start()
	{
		gs = GamePlay.GetGamePlayScript();
	}

	private void LateUpdate()
	{
	}

	private void Update()
	{
		Vector3 translation = Vector3.up * 150f * Time.deltaTime;
		Vector3 position = base.transform.position;
		base.transform.Translate(translation);
		Vector3 position2 = base.transform.position;
		Vector3 direction = position2 - position;
		Ray ray = new Ray(position, direction);
		Vector3 vector = position2 - startPos;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, direction.magnitude * 1f, 9))
		{
			Vector3 vector2 = hitInfo.transform.position - startPos;
			if (hitInfo.collider.gameObject != base.gameObject)
			{
				Logger.trace("<< colliding with: " + hitInfo.collider.name + " at distance traveled: " + vector2.magnitude + " should have traveled: " + travelRange);
				if (hitInfo.collider.gameObject.tag == "pickup")
				{
					Logger.trace("<< don't collide with pickup");
					return;
				}
				int objType = 0;
				if (hitInfo.collider.gameObject.tag == "power_wall")
				{
					hitInfo.collider.gameObject.SendMessage("ApplyWallDamage", damage);
					if (isRocket)
					{
						Logger.trace("<< should explode rocket");
						gs.SendRocketExplode(idx, hitInfo.point);
						return;
					}
				}
				else if (hitInfo.collider.gameObject.tag == "Player")
				{
					GameObject gameObject = GameObject.Find(hitInfo.collider.gameObject.name);
					Player player = gameObject.GetComponent("Player") as Player;
					Logger.trace("bullet is colliding with Player");
					if (player.mAmInvincible)
					{
						Logger.trace("player is invincible");
						return;
					}
					objType = 1;
				}
				else
				{
					if (isRocket)
					{
						Logger.trace("<< should explode rocket");
						gs.SendRocketExplode(idx, hitInfo.point);
						return;
					}
					gs.SpawnHitspark(hitInfo.point, hitInfo.normal, objType);
				}
				Object.Destroy(base.gameObject);
			}
			else
			{
				Debug.DrawLine(position, hitInfo.point, Color.yellow);
				Logger.trace("<< colliding with: " + hitInfo.collider.name + " at distance traveled: " + vector2.magnitude + " should have traveled: " + travelRange);
			}
		}
		if (vector.magnitude > travelRange)
		{
			Logger.trace("<< Destroying because distance traveled " + travelRange);
			Object.Destroy(base.gameObject);
		}
	}

	public void Explode()
	{
		Logger.trace("<< Exploding Rocket");
		GameObject gameObject = Object.Instantiate(Resources.Load("projectiles/grenade_explode_emit") as GameObject, base.transform.position, Quaternion.identity) as GameObject;
		gameObject.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
		gameObject.GetComponent<ParticleEmitter>().emit = true;
		Transform transform = gameObject.transform.Find("particles");
		transform.GetComponent<ParticleEmitter>().emit = true;
		GameObject gameObject2 = GameObject.Find(base.gameObject.name + "/projectile/center/jetSmoke");
		if (gameObject2 == null)
		{
			Logger.trace("<< couldn't find smoke");
		}
		else
		{
			gameObject2.transform.parent = null;
			gameObject2.GetComponent<ParticleEmitter>().emit = false;
		}
		Object.Destroy(base.gameObject);
	}
}
