using UnityEngine;

public class CameraScrolling : MonoBehaviour
{
	private Transform target;

	public float distance = 25f;

	public float springiness = 100f;

	public LevelAttributes levelAttributes;

	public Rect levelBounds = default(Rect);

	private GamePlay gs;

	private RigidbodyInterpolation savedInterpolationSetting;

	private void Awake()
	{
	}

	private void Start()
	{
		gs = GamePlay.GetGamePlayScript();
	}

	public void SetTarget(Transform newTarget, bool snap)
	{
		if ((bool)target)
		{
			Rigidbody rigidbody = target.GetComponent("Rigidbody") as Rigidbody;
			if ((bool)rigidbody)
			{
				rigidbody.interpolation = savedInterpolationSetting;
			}
		}
		target = newTarget;
		if ((bool)target)
		{
			Rigidbody rigidbody2 = target.GetComponent("Rigidbody") as Rigidbody;
			if ((bool)rigidbody2)
			{
				savedInterpolationSetting = rigidbody2.interpolation;
				rigidbody2.interpolation = RigidbodyInterpolation.Interpolate;
			}
		}
		if (snap)
		{
			base.transform.position = GetGoalPosition();
		}
	}

	public void SetTarget(Transform newTarget)
	{
		SetTarget(newTarget, snap: false);
	}

	public Transform GetTarget()
	{
		return target;
	}

	private void LateUpdate()
	{
		Vector3 goalPosition = GetGoalPosition();
		if (!(gs.myPlayer == null))
		{
			Player player = gs.myPlayer.GetComponent("Player") as Player;
			if (player.mAmSniping)
			{
				springiness = 4f;
			}
			else
			{
				springiness = 7.5f;
			}
			if (gs.myPlayer != null && player != null && player.myState.amDoing(64) && player.mAmReady)
			{
				springiness = 1f;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, goalPosition, Time.deltaTime * springiness);
		}
	}

	public Vector3 GetGoalPosition()
	{
		if (!target)
		{
			return base.transform.position;
		}
		float num = 1f;
		float d = 0f;
		Vector2 vector = new Vector2(0f, 20f);
		CameraTargetAttributes exists = target.GetComponent("CameraTargetAttributes") as CameraTargetAttributes;
		if ((bool)exists)
		{
			num = CameraTargetAttributes.distanceModifier;
			d = CameraTargetAttributes.velocityLookAhead;
			vector = CameraTargetAttributes.maxLookAhead;
		}
		Vector3 vector2 = target.position + new Vector3(0f, 0f, (0f - distance) * num);
		Vector3 a = Vector3.zero;
		Rigidbody rigidbody = target.GetComponent("Rigidbody") as Rigidbody;
		if ((bool)rigidbody)
		{
			a = rigidbody.velocity;
		}
		Vector3 vector3 = a * d;
		vector3.x = Mathf.Clamp(vector3.x, 0f - vector.x, vector.x);
		vector3.y = Mathf.Clamp(vector3.y, 0f - vector.y, vector.y);
		vector3.z = 0f;
		vector2 += vector3;
		Vector3 zero = Vector3.zero;
		Vector3 position = base.transform.position;
		if (gs.myPlayer != null)
		{
			Player player = gs.myPlayer.GetComponent("Player") as Player;
			if (player != null && !player.myState.amDoing(64) && player.mAmReady && !GamePlay.mPauseScreenActive && !player.bDisableControl)
			{
				float min = 0f;
				float min2 = 0f;
				float max = Screen.width;
				float max2 = Screen.height;
				if (gs != null && gs.mHUD != null)
				{
					min = gs.mHUD.leftEdge;
					max = gs.mHUD.rightEdge;
					min2 = gs.mHUD.topEdge;
					max2 = gs.mHUD.bottomEdge;
				}
				Vector3 mousePosition = Input.mousePosition;
				float x = Mathf.Clamp(mousePosition.x, min, max);
				Vector3 mousePosition2 = Input.mousePosition;
				float y = Mathf.Clamp(mousePosition2.y, min2, max2);
				Vector3 position2 = target.transform.position;
				float z = position2.z;
				Vector3 position3 = Camera.main.transform.position;
				float num2 = z - position3.z;
				Vector3 a2 = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.nearClipPlane + num2));
				Vector3 vector4 = a2 - target.transform.position;
				float num3 = vector4.magnitude * 0.5f;
				if (player.mAmSniping)
				{
					num3 *= 2f;
					num3 = Mathf.Clamp(num3, num3, 500f);
				}
				else
				{
					num3 = Mathf.Clamp(num3, num3, 30f);
				}
				vector4.Normalize();
				vector4.x *= num3;
				vector4.y *= num3;
				vector4.z = 0f;
				vector2 += vector4;
			}
		}
		base.transform.position = vector2;
		Vector3 vector5 = GetComponent<Camera>().WorldToViewportPoint(target.position);
		Vector3 vector6 = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1f, 1f, vector5.z));
		zero.x = Mathf.Min(levelBounds.xMax - vector6.x, 0f);
		zero.y = Mathf.Min(levelBounds.yMax - vector6.y, 0f);
		vector2 += zero;
		base.transform.position = vector2;
		Vector3 vector7 = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0f, 0f, vector5.z));
		zero.x = Mathf.Max(levelBounds.xMin - vector7.x, 0f);
		zero.y = Mathf.Max(levelBounds.yMin - vector7.y, 0f);
		vector2 += zero;
		base.transform.position = position;
		return vector2;
	}
}
