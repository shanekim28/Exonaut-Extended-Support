using UnityEngine;

public class PlayForScript : MonoBehaviour
{
	public float timeToPlay;

	private void Start()
	{
	}

	private void Update()
	{
		timeToPlay -= Time.deltaTime;
		if (timeToPlay <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
