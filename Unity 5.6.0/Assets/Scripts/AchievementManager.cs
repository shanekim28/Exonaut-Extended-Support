using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
	public enum ExonautStats
	{
		Sneak_Atlas = 100,
		Sneak_Banzai = 101,
		Hacks_Total = 400,
		Crashes_Total = 401,
		GamesPlayed_Battle = 402,
		GamesPlayed_TeamBattle = 403,
		GamesWon_Battle = 404,
		GamesWon_TeamBattle = 405,
		Hacks_AtlasTrinity = 406,
		Hacks_BanzaiTrinity = 407,
		Level = 200,
		DaysPlayedInARow = 201,
		GamesPlayedInARow = 202,
		Highest_XP_SingleGame = 203,
		Highest_Ratio_Battle_Win = 204,
		Highest_Ratio_TeamBattle_Win = 205,
		Highest_Ratio_Battle = 206,
		Highest_Ratio_TeamBattle = 207,
		Wins_Treehouse_Jake_Bubblegum = 408,
		Wins_Treehouse_Finn_Marceline = 409,
		Wins_Abysus_Rex_Bobo = 410,
		Wins_Abysus_VanKleiss_Skalamander = 411,
		Wins_BBB_JohnnyTest = 412,
		Wins_Perplex_UltHS_NRG_FourArms_UltCannonbolt = 413,
		Wins_Perplex_UltBC_UltEE_UltSF_Heatblast = 414,
		Wins_BBB_BlingBlingBoy = 415,
		Hacks_Invisible = 416,
		Hacks_Speed = 417,
		Hacks_DamageBoost = 418,
		Hacks_ArmorBoost = 419
	}

	public class Achievement
	{
		public Texture2D mImage;

		public string mText = string.Empty;

		public Achievement(Texture2D Image, string Text)
		{
			mImage = Image;
			mText = Text;
		}
	}

	public enum PopupDirection
	{
		Left,
		Right,
		Top,
		Bottom
	}

	private const float mFadedInTime = 1f;

	private const float mFadingOutTime = 6f;

	private const float mAchievementUpTime = 7f;

	private Queue<Achievement> mAchievementQueue = new Queue<Achievement>();

	private float mCurrentAchievementTime;

	public GUIStyle mTitleStyle;

	public GUIStyle mDescriptionStyle;

	public Texture2D mBackground;

	private Rect mGroupRect;

	private Rect mBackgroundRect;

	private Rect mTitleRect;

	private Rect mImageRect;

	private Rect mDescriptionRect;

	public static AchievementManager mInstance;

	private float mPositionScale;

	public PopupDirection mDirection = PopupDirection.Bottom;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		mBackgroundRect = new Rect(0f, 0f, mBackground.width, mBackground.height);
		mImageRect = new Rect(0f, 0f, mBackgroundRect.height, mBackgroundRect.height);
		mTitleRect = new Rect(mImageRect.x + mImageRect.width, 0f, mBackgroundRect.width - (mImageRect.x + mImageRect.width), 15f);
		mDescriptionRect = new Rect(mImageRect.x + mImageRect.width, mTitleRect.y + mTitleRect.height, mBackgroundRect.width - (mImageRect.x + mImageRect.width), mBackgroundRect.height - (mTitleRect.y + mTitleRect.height));
		mGroupRect = new Rect(-1000f, 0f, mBackgroundRect.width, mBackgroundRect.height);
		mCurrentAchievementTime = 7f;
		Application.ExternalCall("AchievementUnityComm.doUnityLoaded");
		mInstance = this;
	}

	private void FixedUpdate()
	{
		if (mAchievementQueue.Count > 0)
		{
			mCurrentAchievementTime -= Time.fixedDeltaTime;
			if (mCurrentAchievementTime <= 0f)
			{
				mAchievementQueue.Dequeue();
				mCurrentAchievementTime = 7f;
			}
			if (mCurrentAchievementTime < 1f)
			{
				mPositionScale = mCurrentAchievementTime / 1f;
			}
			else if (mCurrentAchievementTime > 6f)
			{
				mPositionScale = (7f - mCurrentAchievementTime) / 1f;
			}
			else
			{
				mPositionScale = 1f;
			}
			switch (mDirection)
			{
			case PopupDirection.Left:
				mGroupRect.x = Mathf.Lerp(-10f - mBackgroundRect.width, 20f, mPositionScale);
				mGroupRect.y = (float)(Screen.height / 2) - mBackgroundRect.height / 2f;
				break;
			case PopupDirection.Right:
				mGroupRect.x = Mathf.Lerp(Screen.width + 10, (float)Screen.width - mBackgroundRect.width - 20f, mPositionScale);
				mGroupRect.y = (float)(Screen.height / 2) - mBackgroundRect.height / 2f;
				break;
			case PopupDirection.Top:
				mGroupRect.x = (float)(Screen.width / 2) - mBackgroundRect.width / 2f;
				mGroupRect.y = Mathf.Lerp(-10f - mBackgroundRect.height, 20f, mPositionScale);
				break;
			case PopupDirection.Bottom:
				mGroupRect.x = (float)(Screen.width / 2) - mBackgroundRect.width / 2f;
				mGroupRect.y = Mathf.Lerp(Screen.height + 10, (float)Screen.height - mBackgroundRect.height - 20f, mPositionScale);
				break;
			}
		}
	}

	private void OnGUI()
	{
		if (mAchievementQueue.Count > 0)
		{
			GUI.Window(23, mGroupRect, DrawMedal, GUIContent.none);
			GUI.BringWindowToFront(23);
		}
	}

	private void DrawMedal(int id)
	{
		GUI.DrawTexture(mBackgroundRect, mBackground);
		Texture2D mImage = mAchievementQueue.Peek().mImage;
		if (mImage != null)
		{
			GUI.DrawTexture(new Rect((mImageRect.width - (float)mImage.width) / 2f, (mImageRect.width - (float)mImage.width) / 2f, mImage.width, mImage.height), mImage);
		}
		GUI.Label(mTitleRect, "You got a badge!", mTitleStyle);
		GUI.Label(mDescriptionRect, mAchievementQueue.Peek().mText, mDescriptionStyle);
	}

	public IEnumerator AchievementAwarded(string text)
	{
		if (text != null)
		{
			string[] AchievInfo = text.Split(',');
			if (AchievInfo.Length >= 2)
			{
				WWW www = new WWW(AchievInfo[0]);
				yield return www;
				Achievement NewAchiev = new Achievement(www.texture, AchievInfo[1].Replace("%20", " "));
				mAchievementQueue.Enqueue(NewAchiev);
			}
		}
	}

	public void AchievementAwardedResource(string text)
	{
		if (text != null)
		{
			string[] array = text.Split(',');
			if (array.Length >= 2)
			{
				Achievement item = new Achievement(Resources.Load(array[0]) as Texture2D, array[1].Replace("%20", " "));
				mAchievementQueue.Enqueue(item);
			}
		}
	}

	public void AchievementAwardedLocal(Texture2D texture, string text)
	{
		Achievement item = new Achievement(texture, text);
		mAchievementQueue.Enqueue(item);
	}

	public static void SendStat(ExonautStats StatID, int Value)
	{
		Logger.trace("Stat: " + StatID + " - " + Value);
		Application.ExternalCall("AchievementUnityComm.doSendStat", (int)StatID + "," + Value);
	}
}
