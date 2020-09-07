using UnityEngine;

public class DestroySelf : MonoBehaviour
{
	public float mTimer = 10f;

	private void Start()
	{
	}

	private void Update()
	{
		mTimer -= Time.deltaTime;
		if (mTimer <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnApplicationQuit()
	{
		Object.DestroyImmediate(base.gameObject);
	}
}
