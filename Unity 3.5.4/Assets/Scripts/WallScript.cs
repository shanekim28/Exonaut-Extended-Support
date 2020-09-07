using UnityEngine;

public class WallScript : MonoBehaviour
{
	public int mState;

	public float mStartScale;

	public float mMaxScale;

	public float mEndScale;

	private float mScaleRate;

	private float mCurrentScale;

	public float mTimeToMax;

	public float mTimeToEnd;

	public float mTimeToShrink;

	public float mHealth;

	private void Awake()
	{
		mHealth = 100f;
		mStartScale = 1f;
		mMaxScale = 20f;
		mEndScale = 15f;
		mState = 0;
		mCurrentScale = mStartScale;
		mTimeToMax = 2f;
		mTimeToEnd = 0.1f;
		mScaleRate = (mMaxScale - mStartScale) / mTimeToMax;
	}

	private void Start()
	{
		base.gameObject.transform.localScale = new Vector3(mCurrentScale, mCurrentScale, mCurrentScale);
	}

	private void Update()
	{
		switch (mState)
		{
		case 0:
			mCurrentScale += mScaleRate;
			if (mCurrentScale > mMaxScale)
			{
				mCurrentScale = mMaxScale;
				mState++;
				mScaleRate = (mEndScale - mMaxScale) / mTimeToEnd;
			}
			base.gameObject.transform.localScale = new Vector3(mCurrentScale, mCurrentScale, mCurrentScale);
			break;
		case 1:
			mCurrentScale += mScaleRate;
			if (mCurrentScale < mEndScale)
			{
				mCurrentScale = mEndScale;
				mState++;
			}
			base.gameObject.transform.localScale = new Vector3(mCurrentScale, mCurrentScale, mCurrentScale);
			break;
		case 2:
			if (mHealth <= 0f)
			{
				mScaleRate = (0f - mEndScale) / mTimeToShrink;
				mState++;
			}
			break;
		case 3:
			mCurrentScale += mScaleRate;
			if (mCurrentScale < 0f)
			{
				Object.Destroy(base.gameObject);
			}
			break;
		}
	}

	public void ApplyWallDamage(float amount)
	{
		mHealth -= amount;
		Logger.trace("<< wall health: " + mHealth);
	}
}
