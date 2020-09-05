using UnityEngine;

public class equipSuitEffectAnimUpwardsScript : MonoBehaviour
{
	public float rate = 30f;

	private float offset = 1f;

	private void Start()
	{
	}

	private void MyUpdate(float fTime)
	{
		offset += fTime * rate;
		while (offset < 0f)
		{
			offset += 1f;
		}
		while (offset > 1f)
		{
			offset -= 1f;
		}
		Material material = base.GetComponent<Renderer>().material;
		Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
		material.mainTextureOffset = new Vector2(mainTextureOffset.x, offset);
	}

	private void Update()
	{
		MyUpdate(Time.deltaTime);
	}
}
