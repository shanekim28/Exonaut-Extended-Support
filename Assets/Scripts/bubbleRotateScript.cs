using UnityEngine;

public class bubbleRotateScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset;

	private void Start()
	{
		currentTimer = 1f / rate;
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (currentTimer <= 0f)
		{
			currentTimer = 1f / rate;
			offset += 5f;
			base.transform.rotation = Quaternion.AngleAxis(offset, Vector3.up);
			if (offset > 355f)
			{
				base.transform.rotation = Quaternion.AngleAxis(0f, Vector3.forward);
				offset = 0f;
			}
		}
	}
}
