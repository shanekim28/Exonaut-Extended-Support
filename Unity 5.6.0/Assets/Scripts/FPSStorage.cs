using UnityEngine;

public class FPSStorage : MonoBehaviour
{
	private float fps = 15f;

	public float GetCurrentFPS()
	{
		return fps;
	}

	public void FPSChanged(float fps)
	{
		this.fps = fps;
	}
}
