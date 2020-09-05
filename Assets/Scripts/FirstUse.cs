using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstUse : MonoBehaviour
{
	[Serializable]
	public class SmartRect
	{
		public string x;

		public string y;

		public string width;

		public string height;

		public Rect Calculate()
		{
			Rect result = default(Rect);
			result.x = FirstUse.Calculate(x);
			result.y = FirstUse.Calculate(y);
			result.width = FirstUse.Calculate(width);
			result.height = FirstUse.Calculate(height);
			return result;
		}
	}

	[Serializable]
	public class SmartVector2
	{
		public string _x;

		public string _y;

		public Vector2 PreCalc;

		public float x {
			get {
				return PreCalc.x;
			}
			private set {
				PreCalc.x = value;
			}
		}

		public float y {
			get {
				return PreCalc.y;
			}
			private set {
				PreCalc.y = value;
			}
		}

		public SmartVector2(string x, string y)
		{
			_x = x.ToString();
			_y = y.ToString();
		}

		public Vector2 Calculate()
		{
			Vector2 result = default(Vector2);
			result.x = FirstUse.Calculate(_x);
			result.y = FirstUse.Calculate(_y);
			return result;
		}
	}

	[Serializable]
	public class Frame
	{
		public enum BoxAnchor
		{
			Upper_Left,
			Upper_Right,
			Lower_Left,
			Lower_Right,
			Center
		}

		public enum RequiredAction
		{
			Nothing,
			Open_Showcase,
			Open_Options,
			Close_Options,
			Click_Battle,
			Waypoint,
			Shoot,
			Do_Capture,
			PickWeapon,
			ThrowGrenade,
			Roll,
			AirDash,
			Pickup_Shield,
			Pickup_Damage,
			Pickup_Invis,
			Pickup_Speed,
			Pickup_Rocket,
			Do_Shutdown,
			Do_Release,
			PickWildfire,
			PickBallista,
			PickMarksman,
			PickTridex,
			Waypoint_Area,
			Pickup_HelixCannon,
			Pickup_Longshot,
			Obstacle_1,
			Obstacle_2,
			Obstacle_3,
			Obstacle_4,
			Obstacle_5,
			Obstacle_6,
			Obstacle_7,
			Obstacle_8,
			Give_Weapon,
			Deactivate_GameObject,
			Activate_GameObject,
			SetGrenadesActive
		}

		public enum RectType
		{
			None,
			Arrow_Left,
			Arrow_Right,
			Arrow_Top,
			Arrow_Bottom,
			Outline_Box,
			Game_Point
		}

		public enum ContextualHelp
		{
			None,
			ShowMoveHelp,
			ShowJetpackHelp,
			ShowShootingHelp,
			ShowGrenadeHelp
		}

		public enum CharacterImage
		{
			Ben10,
			AgentSix,
			Bubblegum,
			Finn,
			Grim,
			Jake,
			Marceline,
			Rex,
			None,
			Lance
		}

		public CharacterImage mCharImage = CharacterImage.None;

		public BoxAnchor mBoxAnchor = BoxAnchor.Lower_Right;

		[SerializeField]
		public SmartVector2 mBoxSize = new SmartVector2("Screen.width", "146");

		public string mText = string.Empty;

		public SmartRect mFocusRect;

		public RectType mRectType;

		public bool mNextButton;

		public bool mDrawBackdrop;

		[SerializeField]
		public ContextualHelp mContextualHelp;

		public RequiredAction mAction;

		public string mActionSpecific = string.Empty;

		public int ActionCount = 1;

		public Frame()
		{
		}

		public Frame(Frame other)
		{
			mCharImage = other.mCharImage;
			mBoxAnchor = other.mBoxAnchor;
			mBoxSize = other.mBoxSize;
			mText = other.mText;
			mFocusRect = other.mFocusRect;
			mRectType = other.mRectType;
			mNextButton = other.mNextButton;
			mContextualHelp = other.mContextualHelp;
			mAction = other.mAction;
		}
	}

	public bool bInFrontOfOptions;

	private static Rect screenSpace = new Rect(0f, 0f, 900f, 600f);

	private static float lastScreenWidth = 900f;

	private static float lastScreenHeight = 600f;

	private bool bFullScreen;

	public static FirstUse mInstance = null;

	public Frame[] mFrames;

	public int mCurrentFrame;

	public Texture2D[] mArrowTexture = new Texture2D[2];

	public Texture2D[] mBackground = new Texture2D[Enum.GetValues(typeof(Frame.CharacterImage)).Length];

	public Texture2D[] mAvatarName = new Texture2D[Enum.GetValues(typeof(Frame.CharacterImage)).Length];

	public Texture2D mChatBubble;

	public Texture2D mHeaderTexture;

	public Texture2D mBGTexture;

	public GUISkin mSkin;

	public GUIStyle Box;

	public float TextScroll;

	public float TextScrollSpeed = 40f;

	public AudioClip AvatarMessage;

	public AudioClip[] AvatarText;

	private FormattedLabel myLabel;

	private string lastHover = string.Empty;

	public Rect mWindowGroup;

	private void Start()
	{
		TextScroll = 0f;
		StartCoroutine(UpdateScreenSpace());
		mInstance = this;
		Frame[] array = mFrames;
		foreach (Frame frame in array)
		{
			frame.mText = frame.mText.Replace("[factionname]", GameData.getFactionDisplayName(GameData.MyFactionId));
			frame.mText = frame.mText.Replace("[exosuit]", GameData.getExosuit(GameData.MySuitID).mSuitName);
		}
		base.gameObject.AddComponent<AudioSource>();
		StartCoroutine(SendAvatarMessageSound());
		myLabel = null;
	}

	private void Awake()
	{
	}

	public Frame GetFrame()
	{
		if (mCurrentFrame < 0 || mCurrentFrame >= mFrames.Length)
		{
			return null;
		}
		return mFrames[mCurrentFrame];
	}

	public void Kill()
	{
		mCurrentFrame = mFrames.Length;
		GameData.MyTutorialStep = -1;
		PlayerPrefs.SetString("LastTraining", DateTime.Today.AddDays(30.0).ToString());
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnGUI()
	{
		GUI.skin = mSkin;
		if (mCurrentFrame >= mFrames.Length)
		{
			Kill();
			return;
		}
		GUI.BeginGroup(screenSpace);
		Frame frame = GetFrame();
		GUI.depth = 4;
		frame.mBoxSize.PreCalc = frame.mBoxSize.Calculate();
		Rect position = frame.mFocusRect.Calculate();
		if (myLabel == null)
		{
			myLabel = new FormattedLabel(frame.mBoxSize.x - 305f, frame.mText);
		}
		myLabel.mDrawlength = (int)TextScroll;
		switch (frame.mRectType)
		{
		case Frame.RectType.Game_Point:
			if (Camera.main != null)
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(new Vector3(position.x, position.y, 0f));
				Rect rect = new Rect(vector.x - position.width / 2f - screenSpace.x, (float)Screen.height - vector.y - 10f - position.height - screenSpace.y, position.width, position.height);
				Rect position2 = rect;
				position2.width = rect.width * 8f;
				position2.x = (float)(-(int)(Time.realtimeSinceStartup * 8f) % 8) * rect.width;
				position2.y = 0f;
				GUI.BeginGroup(rect);
				GUI.DrawTexture(position2, mArrowTexture[0]);
				GUI.EndGroup();
			}
			break;
		case Frame.RectType.Outline_Box:
			GUI.color = new Color(1f, 1f, 1f, 0.75f + Mathf.Sin(Time.realtimeSinceStartup * 4f) / 4f);
			GUI.Box(position, string.Empty, "GreenOutline");
			GUI.color = Color.white;
			break;
		case Frame.RectType.Arrow_Left:
		{
			Rect pos4 = new Rect(position.x - 138f, position.y + position.height / 2f - 64f, 128f, 128f);
			GUIUtil.DrawAnimatedTexture(pos4, mArrowTexture[1], 8, 8, MirrorX: true, MirrorY: false);
			break;
		}
		case Frame.RectType.Arrow_Right:
		{
			Rect pos3 = new Rect(position.x + position.width + 10f, position.y + position.height / 2f - 64f, 128f, 128f);
			GUIUtil.DrawAnimatedTexture(pos3, mArrowTexture[1], 8, 8, MirrorX: false, MirrorY: false);
			break;
		}
		case Frame.RectType.Arrow_Top:
		{
			Rect pos2 = new Rect(position.x + position.width / 2f - 64f, position.y - 138f, 128f, 128f);
			GUIUtil.DrawAnimatedTexture(pos2, mArrowTexture[0], 8, 8, MirrorX: false, MirrorY: false);
			break;
		}
		case Frame.RectType.Arrow_Bottom:
		{
			Rect pos = new Rect(position.x + position.width / 2f - 64f, position.y + position.height + 10f, 128f, 128f);
			GUIUtil.DrawAnimatedTexture(pos, mArrowTexture[0], 8, 8, MirrorX: false, MirrorY: true);
			break;
		}
		}
		mWindowGroup = GetGroupPos();
		if (frame.mDrawBackdrop)
		{
			Rect position3 = new Rect(0f, 0f, mBGTexture.width, mBGTexture.height);
			while ((float)Screen.width > position3.x)
			{
				GUI.DrawTexture(position3, mBGTexture);
				position3.x += mBGTexture.width;
			}
		}
		GUI.Window(1600, mWindowGroup, DrawDialog, GUIContent.none, GUIStyle.none);
		if (bInFrontOfOptions)
		{
			GUI.BringWindowToFront(1600);
			GUI.FocusWindow(1600);
		}
		else if (DynamicOptions.bDrawing)
		{
			GUI.BringWindowToFront(1500);
			GUI.FocusWindow(1500);
		}
		else if (MessageBox.mMessageBox.Queuesize > 0)
		{
			GUI.BringWindowToFront(1000);
			GUI.FocusWindow(1000);
		}
		GUI.color = Color.white;
		if (frame.mAction == Frame.RequiredAction.Nothing)
		{
			GUI.Button(new Rect(0f, 0f, Screen.width, Screen.height), string.Empty, "invis");
		}
		else
		{
			GUI.Button(new Rect(0f, 0f, position.x, Screen.height), string.Empty, GUIStyle.none);
			GUI.Button(new Rect(position.x + position.width, 0f, (float)Screen.width - (position.x + position.width), Screen.height), string.Empty, GUIStyle.none);
			GUI.Button(new Rect(0f, 0f, Screen.width, position.y), string.Empty, GUIStyle.none);
			GUI.Button(new Rect(0f, position.y + position.height, Screen.width, (float)Screen.height - (position.y + position.height)), string.Empty, GUIStyle.none);
		}
		GUI.EndGroup();
	}

	private void DrawDialog(int id)
	{
		string text = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		Frame frame = GetFrame();
		if (bInFrontOfOptions)
		{
			GUI.FocusWindow(id);
			GUI.BringWindowToFront(id);
		}
		if (frame.mCharImage != Frame.CharacterImage.None)
		{
			Texture2D texture2D = mBackground[(int)frame.mCharImage];
			GUI.BeginGroup(new Rect(0f, mWindowGroup.height - frame.mBoxSize.y, mWindowGroup.width, mWindowGroup.height));
			for (float num = 0f; num < screenSpace.width; num += (float)Box.normal.background.width)
			{
				GUI.Box(new Rect(num, 0f, Box.normal.background.width, frame.mBoxSize.y), string.Empty, Box);
			}
			GUI.BeginGroup(new Rect(300f, 30f, frame.mBoxSize.x - 305f, frame.mBoxSize.y - 40f));
			myLabel.draw();
			GUI.EndGroup();
			GUI.color = Color.white;
			if (frame.mNextButton && (float)frame.mText.Length <= TextScroll)
			{
				GUI.Label(new Rect(0f, frame.mBoxSize.y - 23f, mWindowGroup.width, 24f), "CLICK ANYWHERE TO CONTINUE", "ContinueStyle");
			}
			GUI.EndGroup();
			Texture2D texture2D2 = mAvatarName[(int)frame.mCharImage];
			GUI.DrawTexture(new Rect(mWindowGroup.width - (float)texture2D2.width - 5f, mWindowGroup.height - frame.mBoxSize.y, texture2D2.width, texture2D2.height), texture2D2);
			GUI.DrawTexture(new Rect(0f, mWindowGroup.height - frame.mBoxSize.y, mWindowGroup.width, 24f), mHeaderTexture);
			GUI.DrawTexture(new Rect(0f, mWindowGroup.height - 24f, mWindowGroup.width, 24f), mHeaderTexture);
			GUI.DrawTexture(new Rect(0f, mWindowGroup.height - (float)texture2D.height, texture2D.width, texture2D.height), texture2D);
			GUI.DrawTexture(new Rect(mWindowGroup.width - (float)mChatBubble.width - 5f, mWindowGroup.height - frame.mBoxSize.y - 20f, mChatBubble.width, mChatBubble.height), mChatBubble);
		}
		lastHover = text;
	}

	private IEnumerator SendAvatarMessageSound()
	{
		Frame CurrentFrame = GetFrame();
		if (!(AvatarMessage == null) && CurrentFrame != null && CurrentFrame.mCharImage != Frame.CharacterImage.None)
		{
			yield return new WaitForEndOfFrame();
			if (mCurrentFrame == 0 || mFrames[mCurrentFrame - 1].mCharImage != CurrentFrame.mCharImage)
			{
				GUIUtil.mInstance.GetComponent<AudioSource>().PlayOneShot(AvatarMessage);
			}
		}
	}

	private void Update()
	{
		if (bFullScreen != Screen.fullScreen || lastScreenWidth != (float)Screen.width || lastScreenHeight != (float)Screen.height)
		{
			StartCoroutine(UpdateScreenSpace());
		}
		Frame frame = GetFrame();
		TextScroll += Time.deltaTime * TextScrollSpeed;
		bool flag = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2);
		if (frame == null)
		{
			return;
		}
		if (TextScroll >= (float)frame.mText.Length)
		{
			if (frame.mNextButton && flag)
			{
				myLabel = null;
				mCurrentFrame++;
				TextScroll = 0f;
				StartCoroutine(SendAvatarMessageSound());
				StartCoroutine(ResetScroll());
				flag = false;
			}
		}
		else if (AvatarText.Length > 0 && !base.GetComponent<AudioSource>().isPlaying)
		{
			base.GetComponent<AudioSource>().clip = AvatarText[UnityEngine.Random.Range(0, AvatarText.Length)];
			base.GetComponent<AudioSource>().Play();
		}
		if (flag)
		{
			TextScroll = 10000f;
		}
	}

	public IEnumerator UpdateScreenSpace()
	{
		yield return new WaitForEndOfFrame();
		myLabel = null;
		bFullScreen = Screen.fullScreen;
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		float ar = (float)Screen.width / (float)Screen.height;
		float variance2 = ar / 1.5f;
		if (variance2 > 1f)
		{
			variance2 = 1f / variance2;
			screenSpace = new Rect((int)((1f - variance2) / 2f * (float)Screen.width), 0f, (int)((float)Screen.width - (1f - variance2) * (float)Screen.width), Screen.height);
		}
		else if (variance2 < 1f)
		{
			screenSpace = new Rect(0f, (int)((1f - variance2) / 2f * (float)Screen.height), Screen.width, (int)((float)Screen.height - (1f - variance2) * (float)Screen.height));
		}
		else
		{
			screenSpace = new Rect(0f, 0f, Screen.width, Screen.height);
		}
	}

	public static void DoAction(Frame.RequiredAction Action)
	{
		if (!(mInstance == null) && mInstance.GetFrame().mAction == Action)
		{
			mInstance.GetFrame().ActionCount--;
			if (mInstance.GetFrame().ActionCount <= 0)
			{
				mInstance.myLabel = null;
				mInstance.mCurrentFrame++;
				mInstance.StartCoroutine(mInstance.SendAvatarMessageSound());
				mInstance.StartCoroutine(mInstance.ResetScroll());
			}
		}
	}

	private IEnumerator ResetScroll()
	{
		TextScroll = 0f;
		yield return new WaitForEndOfFrame();
		TextScroll = 0f;
	}

	private static string Calculate(List<string> components)
	{
		if (components.Count == 0)
		{
			return "0";
		}
		int num = 0;
		while (num < components.Count)
		{
			if (components[num] == "(")
			{
				int num2 = components.Count - 1;
				while (num2 >= 0)
				{
					if (components[num2] == ")")
					{
						components[num] = Calculate(components.GetRange(num + 1, num2 - num - 1));
						components.RemoveRange(num + 1, num2 - num);
						num2 -= num2 - num + 1;
					}
					else
					{
						num2--;
					}
				}
			}
			else
			{
				num++;
			}
		}
		int num3 = 1;
		while (num3 < components.Count - 1)
		{
			if (components[num3] == "*")
			{
				components[num3 - 1] = (float.Parse(components[num3 - 1]) * float.Parse(components[num3 + 1])).ToString();
				components.RemoveRange(num3, 2);
			}
			else if (components[num3] == "/")
			{
				components[num3 - 1] = (float.Parse(components[num3 - 1]) / float.Parse(components[num3 + 1])).ToString();
				components.RemoveRange(num3, 2);
			}
			else
			{
				num3++;
			}
		}
		int num4 = 1;
		while (num4 < components.Count - 1)
		{
			if (components[num4] == "+")
			{
				components[num4 - 1] = (float.Parse(components[num4 - 1]) + float.Parse(components[num4 + 1])).ToString();
				components.RemoveRange(num4, 2);
			}
			else if (components[num4] == "-")
			{
				components[num4 - 1] = (float.Parse(components[num4 - 1]) - float.Parse(components[num4 + 1])).ToString();
				components.RemoveRange(num4, 2);
			}
			else
			{
				num4++;
			}
		}
		return components[0];
	}

	public static float Calculate(string math)
	{
		math = math.Replace("Screen.x", screenSpace.x.ToString());
		math = math.Replace("Screen.y", screenSpace.y.ToString());
		math = math.Replace("Screen.width", screenSpace.width.ToString());
		math = math.Replace("Screen.height", screenSpace.height.ToString());
		math = math.Replace("(", " ( ");
		math = math.Replace(")", " ) ");
		List<string> list = new List<string>(math.Split(' '));
		int num = 0;
		while (num < list.Count)
		{
			if (list[num].Length == 0)
			{
				list.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		return float.Parse(Calculate(list));
	}

	public Rect GetGroupPos()
	{
		Frame frame = mFrames[mCurrentFrame];
		if (frame.mCharImage == Frame.CharacterImage.None)
		{
			return default(Rect);
		}
		Rect result = new Rect(screenSpace.x, screenSpace.y, frame.mBoxSize.x, Mathf.Max(frame.mBoxSize.y + 18f, mBackground[(int)frame.mCharImage].height));
		switch (frame.mBoxAnchor)
		{
		case Frame.BoxAnchor.Upper_Right:
			result.x += screenSpace.width - result.width;
			break;
		case Frame.BoxAnchor.Lower_Left:
			result.y += screenSpace.height - result.height;
			break;
		case Frame.BoxAnchor.Lower_Right:
			result.x += screenSpace.width - result.width;
			result.y += screenSpace.height - result.height;
			break;
		case Frame.BoxAnchor.Center:
			result.x += screenSpace.width / 2f - result.width / 2f;
			result.y += screenSpace.height / 2f - result.height / 2f;
			break;
		}
		return result;
	}
}
