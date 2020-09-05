using UnityEngine;

public class HUD_Timer
{
	public Vector2 mPositionCurrent;

	public Vector2 mPositionTarget;

	public bool mIsDebuff;

	public float mTimeTotal;

	public float mTimeCurrent;

	public Texture2D mBackgroundTexture;

	public Texture2D mForegroundTexture;

	public Texture2D mIconTexture;

	public int mType;

	public int mDirection;

	public Color mAlphaWhite;

	public Color mAlphaRed;

	public HUD_Timer(Vector2 position, bool isDebuff, float time, Texture2D backgroundTex, Texture2D foregroundTex, int direction, int type, Texture2D icon, Color alphaWhite)
	{
		mPositionCurrent = (mPositionTarget = position);
		mIsDebuff = isDebuff;
		mTimeTotal = (mTimeCurrent = time);
		mBackgroundTexture = backgroundTex;
		mForegroundTexture = foregroundTex;
		mDirection = direction;
		mType = type;
		mIconTexture = icon;
		mAlphaWhite = alphaWhite;
	}

	public int Update()
	{
		if (GameData.CurPlayState == GameData.PlayState.GAME_IS_QUITTING)
		{
			Logger.traceAlways("[GameHUD::Update] - was going to run");
			return 0;
		}
		if (mPositionCurrent.x < mPositionTarget.x)
		{
			mPositionCurrent.x += 2f;
			if (mPositionCurrent.x > mPositionTarget.x)
			{
				mPositionCurrent.x = mPositionTarget.x;
			}
		}
		if (mPositionCurrent.x > mPositionTarget.x)
		{
			mPositionCurrent.x -= 2f;
			if (mPositionCurrent.x < mPositionTarget.x)
			{
				mPositionCurrent.x = mPositionTarget.x;
			}
		}
		mTimeCurrent -= Time.deltaTime;
		if (mTimeCurrent <= 0f)
		{
			return 1;
		}
		return 0;
	}

	public void Draw()
	{
		float num = mBackgroundTexture.width;
		float height = mBackgroundTexture.height;
		float num2 = mForegroundTexture.width;
		float num3 = mForegroundTexture.height;
		Color color = GUI.color;
		if (mType == 50)
		{
			GUI.color = Color.red;
		}
		GUI.DrawTexture(new Rect(mPositionCurrent.x + num, 0f, 0f - num, height), mBackgroundTexture);
		GUI.color = color;
		float num4 = mTimeCurrent / mTimeTotal;
		if (num4 > 1f)
		{
			num4 = 1f;
		}
		float num5 = num3 * num4;
		float num6 = num3 - num5;
		GUI.BeginGroup(new Rect(mPositionCurrent.x, 0f + num6, num2, num5));
		if (mType == 50)
		{
			GUI.color = new Color(1f, 0f, 0f, 0.5f);
		}
		else
		{
			GUI.color = mAlphaWhite;
		}
		GUI.DrawTexture(new Rect(num2, 0f - num6, 0f - num2, num3), mForegroundTexture);
		GUI.color = Color.white;
		GUI.EndGroup();
		GUI.DrawTexture(new Rect(mPositionCurrent.x, 0f, num, height), mIconTexture);
	}
}
