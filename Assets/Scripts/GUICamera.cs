using UnityEngine;

public class GUICamera : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnPreRender()
	{
		GL.Clear(true, false, Color.clear);
	}
}
