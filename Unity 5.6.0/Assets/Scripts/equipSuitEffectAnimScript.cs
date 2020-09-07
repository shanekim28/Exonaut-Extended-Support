using UnityEngine;

public class equipSuitEffectAnimScript : MonoBehaviour
{
	public float rate = 30f;

	private float offset = 1f;

	private void Start()
	{
	}

	private void MyUpdate(float fTime)
	{
		offset += fTime * rate;
		while (offset > 1f)
		{
			offset -= 1f;
		}
		Material material = base.GetComponent<Renderer>().material;
		float x = offset;
		Vector2 mainTextureOffset = base.GetComponent<Renderer>().material.mainTextureOffset;
		material.mainTextureOffset = new Vector2(x, mainTextureOffset.y);
	}

	private void Update()
	{
		MyUpdate(Time.deltaTime);
	}
}
