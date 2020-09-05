using UnityEngine;

public class RotateMe : MonoBehaviour
{
	public float rotateSpeed;

	private void Start()
	{
		rotateSpeed = 0.5f;
	}

	private void FixedUpdate()
	{
		base.transform.Rotate(0f, 0f, rotateSpeed);
	}

	private void Update()
	{
	}
}
