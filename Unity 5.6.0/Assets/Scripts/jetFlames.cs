using UnityEngine;

public class jetFlames : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp("space"))
		{
			base.GetComponent<ParticleEmitter>().emit = false;
		}
		if (Input.GetKey("space"))
		{
			base.GetComponent<ParticleEmitter>().emit = true;
		}
	}
}
