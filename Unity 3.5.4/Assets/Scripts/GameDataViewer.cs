using UnityEngine;

public class GameDataViewer : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
	}
}
