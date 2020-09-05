using UnityEngine;

public class pickupAnimScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset;

	private void Start()
	{
		currentTimer = 1f / rate;
		Transform transform = base.transform;
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		transform.position = new Vector3(x, position2.y, 0f);
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (rate != 0f && currentTimer <= 0f)
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
