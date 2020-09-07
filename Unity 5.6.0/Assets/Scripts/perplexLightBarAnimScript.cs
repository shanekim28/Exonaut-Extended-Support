using UnityEngine;

public class perplexLightBarAnimScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset;

	public float interval = 25f;

	public float intervalTime = 25f;

	public float offsetAmount = 0.05f;

	private void Start()
	{
		currentTimer = 1f / rate;
		interval = 25f;
	}

	private void Update()
	{
		interval -= Time.deltaTime;
		if (!(interval <= 0f))
		{
			return;
		}
		currentTimer -= Time.deltaTime;
		if (currentTimer <= 0f)
		{
			currentTimer = 1f / rate;
			offset += offsetAmount;
			Material material = base.GetComponent<Renderer>().material;
			float x = offset;
			Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
			material.mainTextureOffset = new Vector2(x, mainTextureOffset.y);
			if (offset > 1f)
			{
				Material material2 = base.GetComponent<Renderer>().material;
				Vector2 mainTextureOffset2 = base.GetComponent<Renderer>().material.mainTextureOffset;
				material2.mainTextureOffset = new Vector2(0f, mainTextureOffset2.y);
				offset = 0f;
				interval = intervalTime;
			}
		}
	}
}
