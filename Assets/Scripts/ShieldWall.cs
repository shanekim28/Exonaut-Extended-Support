using UnityEngine;

public class ShieldWall : MonoBehaviour
{
	public int TriggersLeft = 3;

	public AudioClip OpenSound;

	public AudioClip CloseSound;

	private bool bCanPlay;

	private void Awake()
	{
	}

	private void Start()
	{
		bCanPlay = true;
	}

	private void Update()
	{
	}

	public void OnDestroy()
	{
		bCanPlay = false;
	}

	public void PlaySound(AudioClip clip)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.position = base.transform.position;
		DestroySelf destroySelf = gameObject.AddComponent<DestroySelf>();
		destroySelf.mTimer = clip.length + 0.1f;
		gameObject.AddComponent<AudioSource>();
		gameObject.GetComponent<AudioSource>().maxDistance = 240f;
		gameObject.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
		gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
	}

	public void OnEnable()
	{
		if (CloseSound != null && bCanPlay)
		{
			PlaySound(CloseSound);
		}
	}

	public void OnDisable()
	{
		if (base.enabled && !base.gameObject.active && OpenSound != null && bCanPlay)
		{
			PlaySound(OpenSound);
		}
	}

	public void KillTrigger()
	{
		TriggersLeft--;
		if (TriggersLeft <= 0)
		{
			base.gameObject.SetActiveRecursively(state: false);
		}
	}

	public void OnApplicationQuit()
	{
		NoSound();
	}

	public void OnLevelWasLoaded(int Level)
	{
		NoSound();
	}

	public void NoSound()
	{
		bCanPlay = false;
	}
}
