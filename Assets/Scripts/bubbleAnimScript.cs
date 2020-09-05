using UnityEngine;

public class bubbleAnimScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset = 0.1f;

	private int currentState;

	public AudioClip bubbleOffSound;

	private void Start()
	{
		currentTimer = 1f / rate;
		currentState = 0;
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (!(currentTimer <= 0f))
		{
			return;
		}
		currentTimer = 1f / rate;
		if (currentState == 0)
		{
			offset += 0.1f;
			Material material = base.GetComponent<Renderer>().material;
			float x = offset;
			Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
			material.mainTextureOffset = new Vector2(x, mainTextureOffset.y);
			if (offset == 0.5f)
			{
				currentState = 1;
			}
		}
		else if (currentState != 1 && currentState == 2)
		{
			offset += 0.1f;
			if (offset > 1f)
			{
				offset = 0f;
			}
			Material material2 = base.GetComponent<Renderer>().material;
			float x2 = offset;
			Vector2 mainTextureOffset2 = base.GetComponent<Renderer>().material.mainTextureOffset;
			material2.mainTextureOffset = new Vector2(x2, mainTextureOffset2.y);
		}
	}

	public void setCurrentState(int toSet)
	{
		currentState = toSet;
	}
}
