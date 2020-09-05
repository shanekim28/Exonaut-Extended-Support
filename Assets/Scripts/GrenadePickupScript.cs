using UnityEngine;

public class GrenadePickupScript : MonoBehaviour
{
	public float rate = 30f;

	private float offset;

	private Quaternion startRotation;

	private void Start()
	{
		startRotation = base.transform.rotation;
	}

	private void Update()
	{
		offset -= 10f;
		base.transform.rotation = startRotation * Quaternion.AngleAxis(offset, Vector3.forward);
		if (offset < -350f)
		{
			base.transform.rotation = startRotation * Quaternion.AngleAxis(0f, Vector3.forward);
			offset = 0f;
		}
	}
}
