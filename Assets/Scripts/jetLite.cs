using UnityEngine;

public class jetLite : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp("space"))
		{
			base.GetComponent<Light>().cullingMask = 1;
			base.GetComponent<Light>().intensity = 0f;
		}
		if (Input.GetKey("space"))
		{
			base.GetComponent<Light>().cullingMask = 1;
			base.GetComponent<Light>().intensity = 2f;
		}
	}
}
