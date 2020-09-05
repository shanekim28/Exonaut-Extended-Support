using UnityEngine;

public class tutorialTargetRotate : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset;

	private Quaternion startRotation;

	private void Start()
	{
		startRotation = base.transform.rotation;
		currentTimer = 1f / rate;
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (currentTimer <= 0f)
		{
			currentTimer = 1f / rate;
			offset -= 10f;
			base.transform.rotation = startRotation * Quaternion.AngleAxis(offset, Vector3.forward);
			if (offset < -360f)
			{
				base.transform.rotation = startRotation * Quaternion.AngleAxis(0f, Vector3.forward);
				offset = 0f;
			}
		}
	}
}
