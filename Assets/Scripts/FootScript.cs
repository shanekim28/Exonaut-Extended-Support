using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class FootScript : MonoBehaviour
{
	public float baseFootAudioVolume = 1f;

	public float soundEffectPitchRandomness = 0.05f;

	public int soundIdx;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.name != "SL_Collision"))
		{
			GamePlay gamePlayScript = GamePlay.GetGamePlayScript();
			Logger.trace("<< should play sound " + other.name);
			AudioClip audioClip = gamePlayScript.footstepSounds[soundIdx];
			if (audioClip != null && !base.GetComponent<AudioSource>().isPlaying)
			{
				base.GetComponent<AudioSource>().clip = audioClip;
				base.GetComponent<AudioSource>().volume = baseFootAudioVolume * GameData.mGameSettings.mSoundVolume;
				base.GetComponent<AudioSource>().Play();
			}
		}
	}

	private void Reset()
	{
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Collider>().isTrigger = true;
	}
}
