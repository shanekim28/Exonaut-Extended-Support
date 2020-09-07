using UnityEngine;

public class animateTextureScript : MonoBehaviour
{
	public float framerate = 30f;

	public int numFrames = 10;

	public bool loopAnim;

	public bool isAnimating;

	private float currentTimer;

	private float offset;

	private float interval;

	private void Start()
	{
		currentTimer = 1f / framerate;
		offset = 0f;
		interval = 1f / (float)numFrames;
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (!(currentTimer <= 0f))
		{
			return;
		}
		currentTimer = 1f / framerate;
		offset += interval;
		Material material = base.GetComponent<Renderer>().material;
		float x = offset;
		Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
		material.mainTextureOffset = new Vector2(x, mainTextureOffset.y);
		if (offset > interval * (float)numFrames - interval)
		{
			Material material2 = base.GetComponent<Renderer>().material;
			Vector2 mainTextureOffset2 = base.GetComponent<Renderer>().material.mainTextureOffset;
			material2.mainTextureOffset = new Vector2(0f, mainTextureOffset2.y);
			offset = 0f;
			if (!loopAnim)
			{
				isAnimating = false;
			}
		}
	}
}
