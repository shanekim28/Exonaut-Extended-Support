using UnityEngine;

[RequireComponent(typeof(CameraScrolling))]
public class CameraFocus : MonoBehaviour
{
	private CameraScrolling cameraScrolling;

	public Transform[] targets = new Transform[2];

	private void Awake()
	{
	}
}
