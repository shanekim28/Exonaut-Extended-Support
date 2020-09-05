using UnityEngine;

public class ContextualHelp : MonoBehaviour
{
	private bool mShowMoveHelp;

	private float mTimeForMoveHelp;

	private bool mShowShootingHelp;

	private float mTimeForShootingHelp;

	private bool mShowJetpackHelp;

	private float mTimeForJetpackHelp;

	public bool mShowInvincibleHelp;

	public float mTimeForInvincibleHelp;

	private bool mShowGrenadeHelp;

	private float mTimeForGrenadeHelp;

	private Texture2D mMoveMessage;

	private Texture2D mShootMessage;

	private Texture2D mJetpackMessage;

	private Texture2D mGrenadeMessage;

	private Player mMe;

	private bool bCanDestroy = true;

	private void Awake()
	{
	}

	private void Start()
	{
		mMe = (base.gameObject.GetComponent("Player") as Player);
		ResetHelp(CanDestroy: true);
	}

	private void Update()
	{
		if (!mMe.mAmReady)
		{
			return;
		}
		if (mShowMoveHelp && mTimeForMoveHelp > 0f)
		{
			mTimeForMoveHelp -= Time.deltaTime;
			if (mTimeForMoveHelp <= 0f)
			{
				mMoveMessage = (Resources.Load("HUD/help/help_bubbles_move") as Texture2D);
			}
		}
		if (mShowShootingHelp && mTimeForShootingHelp > 0f)
		{
			mTimeForShootingHelp -= Time.deltaTime;
			if (mTimeForShootingHelp <= 0f)
			{
				mShootMessage = (Resources.Load("HUD/help/help_bubbles_shoot") as Texture2D);
			}
		}
		if (mShowJetpackHelp && mTimeForJetpackHelp > 0f)
		{
			mTimeForJetpackHelp -= Time.deltaTime;
			if (mTimeForJetpackHelp <= 0f)
			{
				mJetpackMessage = (Resources.Load("HUD/help/help_bubbles_jetpack") as Texture2D);
			}
		}
		if (mShowGrenadeHelp && mTimeForGrenadeHelp > 0f)
		{
			mTimeForGrenadeHelp -= Time.deltaTime;
			if (mTimeForGrenadeHelp <= 0f)
			{
				if (GameData.eventObjects.ContainsKey("help_grenade"))
				{
					mGrenadeMessage = (GameData.eventObjects["help_grenade"] as Texture2D);
				}
				else
				{
					mGrenadeMessage = (Resources.Load("HUD/help/help_bubbles_grenade") as Texture2D);
				}
			}
		}
		if (mTimeForInvincibleHelp > 0f)
		{
			mTimeForInvincibleHelp -= Time.deltaTime;
		}
		else if (bCanDestroy && !mShowMoveHelp && !mShowShootingHelp && !mShowJetpackHelp && !mShowGrenadeHelp)
		{
			Object.Destroy(this);
		}
	}

	public void ResetHelp(bool CanDestroy)
	{
		bCanDestroy = CanDestroy;
		mShowMoveHelp = true;
		mShowShootingHelp = true;
		mShowJetpackHelp = true;
		mShowGrenadeHelp = true;
		mShowInvincibleHelp = false;
		mTimeForMoveHelp = 15f;
		mTimeForShootingHelp = 60f;
		mTimeForJetpackHelp = 30f;
		mTimeForGrenadeHelp = 120f;
		mMoveMessage = null;
		mShootMessage = null;
		mJetpackMessage = null;
		mGrenadeMessage = null;
	}

	public void SetMoveHelp()
	{
		mShowMoveHelp = false;
	}

	public void ShowMoveHelp()
	{
		mShowMoveHelp = true;
		mMoveMessage = (Resources.Load("HUD/help/help_bubbles_move") as Texture2D);
	}

	public bool GetMoveHelp()
	{
		return mShowMoveHelp;
	}

	public void SetShootingHelp()
	{
		mShowShootingHelp = false;
	}

	public void ShowShootingHelp()
	{
		mShowShootingHelp = true;
		mShootMessage = (Resources.Load("HUD/help/help_bubbles_shoot") as Texture2D);
	}

	public bool GetShootingHelp()
	{
		return mShowShootingHelp;
	}

	public void SetJetpackHelp()
	{
		mShowJetpackHelp = false;
	}

	public void ShowJetpackHelp()
	{
		mShowJetpackHelp = true;
		mJetpackMessage = (Resources.Load("HUD/help/help_bubbles_jetpack") as Texture2D);
	}

	public bool GetJetpackHelp()
	{
		return mShowJetpackHelp;
	}

	public void SetGrenadeHelp()
	{
		mShowGrenadeHelp = false;
	}

	public void ShowGrenadeHelp()
	{
		mShowGrenadeHelp = true;
		mGrenadeMessage = (Resources.Load("HUD/help/help_bubbles_grenade") as Texture2D);
	}

	public bool GetGrenadeHelp()
	{
		return mShowGrenadeHelp;
	}

	public void ShowInvincibleHelp(bool toShow)
	{
		if (mTimeForInvincibleHelp <= 0f)
		{
			toShow = false;
		}
		mShowInvincibleHelp = toShow;
	}

	private void OnGUI()
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(mMe.transform.position);
		if (!mMe.mAmCaptured)
		{
			vector.y = (float)Screen.height - vector.y;
			vector.y -= 160f * ((float)Screen.height / 600f);
			if (mShootMessage != null && mShowShootingHelp)
			{
				GUI.DrawTexture(new Rect(vector.x - (float)mShootMessage.width * 0.5f, vector.y, mShootMessage.width, mShootMessage.height), mShootMessage);
			}
			if (mJetpackMessage != null && mShowJetpackHelp)
			{
				GUI.DrawTexture(new Rect(vector.x - (float)mJetpackMessage.width * 0.5f, vector.y, mJetpackMessage.width, mJetpackMessage.height), mJetpackMessage);
			}
			if (mMoveMessage != null && mShowMoveHelp)
			{
				GUI.DrawTexture(new Rect(vector.x - (float)mMoveMessage.width * 0.5f, vector.y, mMoveMessage.width, mMoveMessage.height), mMoveMessage);
			}
			if (mGrenadeMessage != null && mShowGrenadeHelp)
			{
				GUI.DrawTexture(new Rect(vector.x - (float)mGrenadeMessage.width * 0.5f, vector.y, mGrenadeMessage.width, mGrenadeMessage.height), mGrenadeMessage);
			}
		}
	}
}
