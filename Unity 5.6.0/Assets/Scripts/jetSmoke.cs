using UnityEngine;

public class jetSmoke : MonoBehaviour
{
	public AudioClip jetPackIgnite;

	public AudioClip jetPackLoop;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp("space"))
		{
			base.GetComponent<AudioSource>().clip = jetPackLoop;
			base.GetComponent<AudioSource>().Stop();
			base.GetComponent<ParticleEmitter>().emit = false;
		}
		if (Input.GetKeyDown("space"))
		{
			base.GetComponent<AudioSource>().clip = jetPackLoop;
			base.GetComponent<AudioSource>().loop = true;
			base.GetComponent<AudioSource>().Play();
			base.GetComponent<AudioSource>().PlayOneShot(jetPackIgnite);
			base.GetComponent<ParticleEmitter>().emit = true;
		}
	}
}
