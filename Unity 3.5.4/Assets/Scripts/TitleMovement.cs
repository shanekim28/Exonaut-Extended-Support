using UnityEngine;

public class TitleMovement : MonoBehaviour
{
	public Vector2 Multiplier = new Vector2(2f, 2f);

	private Vector3 BaseRotation = Vector3.zero;

	private void Start()
	{
		BaseRotation = base.transform.localRotation.eulerAngles;
	}

	private void Update()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.x = Mathf.Clamp(mousePosition.x / (float)Screen.width, 0f, 1f) * 2f - 1f;
		mousePosition.y = Mathf.Clamp(mousePosition.y / (float)Screen.height, 0f, 1f) * 2f - 1f;
		base.transform.localRotation = Quaternion.Euler(BaseRotation.x + mousePosition.y * (0f - Multiplier.y), BaseRotation.y + mousePosition.x * Multiplier.x, BaseRotation.z);
	}

	private void FixedUpdate()
	{
	}

	private void OnGUI()
	{
	}
}
