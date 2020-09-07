using UnityEngine;

public class pickupOrbitIconFaceCamera : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	public float offset;

	private float offsetInc;

	private Quaternion iconRotation;

	private void Start()
	{
		iconRotation = base.transform.rotation;
		currentTimer = 1f / rate;
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (currentTimer <= 0f)
		{
			currentTimer = 1f / rate;
			offsetInc -= offset;
			base.transform.rotation = iconRotation * Quaternion.AngleAxis(offsetInc, Vector3.forward);
			if (offsetInc < -360f)
			{
				base.transform.rotation = iconRotation * Quaternion.AngleAxis(0f, Vector3.forward);
				offsetInc = 0f;
			}
		}
	}
}
