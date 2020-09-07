using UnityEngine;

public class benSuitCylinderLinesScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset = 1f;

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
			offset -= 0.1f;
			Material material = base.GetComponent<Renderer>().material;
			Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
			material.mainTextureOffset = new Vector2(mainTextureOffset.x, offset);
			if (offset < 0f)
			{
				Material material2 = base.GetComponent<Renderer>().material;
				Vector2 mainTextureOffset2 = base.GetComponent<Renderer>().material.mainTextureOffset;
				material2.mainTextureOffset = new Vector2(mainTextureOffset2.x, 1f);
				offset = 0.9f;
			}
		}
	}
}