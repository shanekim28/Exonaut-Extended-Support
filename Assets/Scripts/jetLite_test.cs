using UnityEngine;

public class jetLite_test : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp("space"))
		{
			base.GetComponent<Light>().intensity = 0f;
		}
		if (Input.GetKey("space"))
		{
			base.GetComponent<Light>().intensity = 8f;
		}
	}
}
