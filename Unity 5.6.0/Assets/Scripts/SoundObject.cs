using UnityEngine;

public class SoundObject : MonoBehaviour
{
	private void Start()
	{
		switch (base.gameObject.tag)
		{
		case "MusicObject":
			base.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			break;
		case "SFXObject":
			base.GetComponent<AudioSource>().volume = GameData.mGameSettings.mSoundVolume;
			break;
		}
	}
}
