using UnityEngine;

public class muzzleFlash4FramesAnimScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset;

	private Transform plane1;

	private Transform plane2;

	private void Start()
	{
		currentTimer = 1f / rate;
		plane1 = base.transform.Find("muzzleFlashA");
		plane2 = base.transform.Find("muzzleFlashB");
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (currentTimer <= 0f)
		{
			currentTimer = 1f / rate;
			offset += 0.25f;
			if (offset >= 1f)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			Material material = plane1.gameObject.GetComponent<Renderer>().material;
			float x = offset;
			Vector2 mainTextureOffset = plane1.gameObject.GetComponent<Renderer>().material.mainTextureOffset;
			material.mainTextureOffset = new Vector2(x, mainTextureOffset.y);
			Material material2 = plane2.gameObject.GetComponent<Renderer>().material;
			float x2 = offset;
			Vector2 mainTextureOffset2 = plane2.gameObject.GetComponent<Renderer>().material.mainTextureOffset;
			material2.mainTextureOffset = new Vector2(x2, mainTextureOffset2.y);
		}
	}
}
