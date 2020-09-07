using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class HolidayEvent
{
	private abstract class HolidayObject
	{
		public abstract bool Update();
	}

	private abstract class HolidayDrawable : HolidayObject
	{
		public abstract void Draw();
	}

	private abstract class HolidaySprite : HolidayDrawable
	{
		protected Texture2D mTexture;

		public HolidaySprite()
		{
			mTexture = (GameData.eventObjects["Holiday_Sprite"] as Texture2D);
		}
	}

	private class SpiderManager : HolidayObject
	{
		private float SpawnTimer = 5f;

		private int SpiderSpawnTime_Min = 15;

		private int SpiderSpawnTime_Max = 20;

		private int LargeSpider_Num = 1;

		private int SmallSpider_Num = 3;

		public SpiderManager()
		{
			SpiderSpawnTime_Min = LoadEventData(SpiderSpawnTime_Min, "Spider_Spawn_Time_Min");
			SpiderSpawnTime_Max = LoadEventData(SpiderSpawnTime_Max, "Spider_Spawn_Time_Max");
			LargeSpider_Num = LoadEventData(LargeSpider_Num, "Spider_Num_Large");
			SmallSpider_Num = LoadEventData(SmallSpider_Num, "Spider_Num_Small");
		}

		public override bool Update()
		{
			SpawnTimer -= Time.deltaTime;
			if (SpawnTimer <= 0f)
			{
				SpawnTimer += UnityEngine.Random.Range(SpiderSpawnTime_Min, SpiderSpawnTime_Max);
				for (int i = 0; i < 1; i++)
				{
					Vector2 vector = Vector2.zero;
					Vector2 a = Vector2.zero;
					switch (UnityEngine.Random.Range(0, 4))
					{
					case 0:
						vector = new Vector2(-60f, UnityEngine.Random.Range(60, Screen.height - 60));
						a = new Vector2(Screen.width + 60, UnityEngine.Random.Range(60, Screen.height - 60));
						break;
					case 1:
						vector = new Vector2(Screen.width + 60, UnityEngine.Random.Range(60, Screen.height - 60));
						a = new Vector2(-60f, UnityEngine.Random.Range(60, Screen.height - 60));
						break;
					case 2:
						vector = new Vector2(UnityEngine.Random.Range(60, Screen.width - 60), -60f);
						a = new Vector2(UnityEngine.Random.Range(60, Screen.width - 60), Screen.height + 60);
						break;
					case 3:
						vector = new Vector2(UnityEngine.Random.Range(60, Screen.width - 60), Screen.height + 60);
						a = new Vector2(UnityEngine.Random.Range(60, Screen.width - 60), -60f);
						break;
					}
					Vector2 a2 = a - vector;
					float magnitude = a2.magnitude;
					a2.Normalize();
					Vector2 a3 = new Vector2(a2.y, 0f - a2.x);
					Vector2 start = vector;
					int num = 40;
					start -= a3 * num * (LargeSpider_Num / 2);
					for (int j = 0; j < LargeSpider_Num; j++)
					{
						Spider spider = new Spider(100, start, Mathf.Atan2(a2.y, a2.x) * 57.29578f + 90f, 500f);
						for (int k = 0; k < 5; k++)
						{
							Vector2 b = a3 * UnityEngine.Random.Range(-30, 30);
							spider.mPath.Add(new Spider.SpiderNode(vector + a2 * (magnitude * ((float)(1 + k) / 5f)) + b, 25f, (UnityEngine.Random.Range(0, 3) != 0) ? 0f : 0.25f));
						}
						start += a3 * num;
						mInstance.m_HolidayAddData[spider] = spider;
					}
					vector -= a3 * num * (SmallSpider_Num / 2);
					for (int l = 0; l < SmallSpider_Num; l++)
					{
						Spider spider2 = new Spider(60, vector - a2 * 160f, Mathf.Atan2(a2.y, a2.x) * 57.29578f + 90f, UnityEngine.Random.Range(380, 440));
						for (int m = 0; m < 5; m++)
						{
							Vector2 b2 = a3 * UnityEngine.Random.Range(-20, 20);
							spider2.mPath.Add(new Spider.SpiderNode(vector + a2 * (magnitude * ((float)(1 + m) / 5f)) + b2, 25f, (UnityEngine.Random.Range(0, 3) != 0) ? 0f : 0.25f));
						}
						mInstance.m_HolidayAddData[spider2] = spider2;
						vector += a3 * num;
					}
				}
			}
			return true;
		}
	}

	private class FallingSprite : HolidaySprite
	{
		private Rect mRect;

		private Vector2 mTrajectory;

		public Vector2 Center {
			get {
				return new Vector2(mRect.x + mRect.width / 2f, mRect.y + mRect.height / 2f);
			}
		}

		public FallingSprite(int Size, Vector2 Start, Vector2 Direction, float Speed)
		{
			mRect = new Rect(Start.x, Start.y, Size, Size);
			mTrajectory = Direction.normalized * Speed;
		}

		public override bool Update()
		{
			Vector2 vector = mTrajectory * Time.deltaTime;
			mRect.x += vector.x;
			mRect.y += vector.y;
			if (mRect.y > (float)Screen.height)
			{
				mRect.y = 0f - mRect.height;
				mRect.x = UnityEngine.Random.Range(-70, Screen.width + 70);
			}
			return true;
		}

		public override void Draw()
		{
			GUI.DrawTexture(mRect, mTexture);
		}
	}

	private class Spider : HolidaySprite
	{
		public class SpiderNode
		{
			private Vector2 mPoint;

			private float mSqrRadius;

			private float mDelay;

			public Vector2 Point {
				get {
					return mPoint;
				}
			}

			public float SqrRadius {
				get {
					return mSqrRadius;
				}
			}

			public float Delay {
				get {
					return mDelay;
				}
			}

			public SpiderNode(Vector2 Point, float Radius, float Delay)
			{
				mPoint = Point;
				mSqrRadius = Radius * Radius;
				mDelay = Delay;
			}
		}

		private Rect mRect;

		private float mAngle;

		private float mSpeed;

		private float mDelay;

		private int mFrames;

		private int mIdleFrame;

		public List<SpiderNode> mPath = new List<SpiderNode>();

		public Vector2 Center {
			get {
				return new Vector2(mRect.x + mRect.width / 2f, mRect.y + mRect.height / 2f);
			}
		}

		public Spider(int Size, Vector2 Start, float Angle, float Speed)
		{
			mRect = new Rect(Start.x, Start.y, Size, Size);
			mAngle = Angle;
			mSpeed = Speed;
			mFrames = LoadEventData(0, "Spider_Animation_Frames");
			mIdleFrame = LoadEventData(0, "Spider_Animation_Idle_Frame");
		}

		public override bool Update()
		{
			mDelay -= Time.deltaTime;
			if (mDelay > 0f)
			{
				return true;
			}
			if (mPath.Count == 0)
			{
				return false;
			}
			SpiderNode spiderNode = mPath[0];
			Vector2 vector = spiderNode.Point - Center;
			float sqrMagnitude = vector.sqrMagnitude;
			float f = mAngle * ((float)Math.PI / 180f);
			vector.Normalize();
			Vector2 lhs = new Vector2(Mathf.Sin(f), 0f - Mathf.Cos(f));
			Vector2 rhs = new Vector2(lhs.y, 0f - lhs.x);
			if (sqrMagnitude <= spiderNode.SqrRadius || mDelay < -3f)
			{
				mPath.RemoveAt(0);
				mDelay = spiderNode.Delay;
			}
			else
			{
				float num = Vector2.Dot(vector, rhs);
				if (Vector2.Dot(lhs, vector) < 0f)
				{
					num = ((num > 0f) ? 1 : (-1));
				}
				mAngle += num * -750f * Time.deltaTime;
				if (mAngle < 0f)
				{
					mAngle += 360f;
				}
				if (mAngle > 360f)
				{
					mAngle -= 360f;
				}
				lhs *= mSpeed * Time.deltaTime;
				mRect.x += lhs.x;
				mRect.y += lhs.y;
			}
			return true;
		}

		public override void Draw()
		{
			GUIUtility.RotateAroundPivot(mAngle, Center);
			if (mDelay <= 0f)
			{
				GUIUtil.DrawAnimatedTextureFrame(mRect, mTexture, mFrames + 1, (int)(Time.realtimeSinceStartup * 30f) % mFrames, MirrorX: false, MirrorY: false);
			}
			else
			{
				GUIUtil.DrawAnimatedTextureFrame(mRect, mTexture, mFrames + 1, mIdleFrame, MirrorX: false, MirrorY: false);
			}
			GUI.matrix = Matrix4x4.identity;
		}
	}

	public delegate void LoadHolidayEvent();

	public delegate void UpdateHolidayEvent();

	public delegate void DrawHolidayEvent();

	public const string Holiday_Script_Data = "Holiday_Script_Data";

	public const string Holiday_Sprite = "Holiday_Sprite";

	public const string Spider_Manager_Create = "Spider_Manager_Create";

	public const string Spider_Spawn_Time_Min = "Spider_Spawn_Time_Min";

	public const string Spider_Spawn_Time_Max = "Spider_Spawn_Time_Max";

	public const string Spider_Num_Large = "Spider_Num_Large";

	public const string Spider_Num_Small = "Spider_Num_Small";

	public const string Spider_Animation_Frames = "Spider_Animation_Frames";

	public const string Spider_Animation_Idle_Frame = "Spider_Animation_Idle_Frame";

	public const string Title_Logo_Base_Color = "Title_Logo_Base_Color";

	public const string Title_Logo_Shift_Color = "Title_Logo_Shift_Color";

	public const string Title_Logo_Shift_Rate = "Title_Logo_Shift_Rate";

	public const string Background_Hex_Base_Color = "Background_Hex_Base_Color";

	public const string Background_Hex_Shift_Color = "Background_Hex_Shift_Color";

	public const string Background_Hex_Shift_Rate = "Background_Hex_Shift_Rate";

	public const string TitleLoading_MusicLoop = "TitleLoading_MusicLoop";

	public const string Queue_MusicLoop = "Queue_MusicLoop";

	public const string GameHome_MusicLoop = "GameHome_MusicLoop";

	public const string Falling_Sprite_X_Min = "Falling_Sprite_X_Min";

	public const string Falling_Sprite_X_Max = "Falling_Sprite_X_Max";

	public const string Falling_Sprite_Size_Min = "Falling_Sprite_Size_Min";

	public const string Falling_Sprite_Size_Max = "Falling_Sprite_Size_Max";

	public const string Falling_Sprite_Speed_Min = "Falling_Sprite_Speed_Min";

	public const string Falling_Sprite_Speed_Max = "Falling_Sprite_Speed_Max";

	public const string Falling_Sprite_Num = "Falling_Sprite_Num";

	public const string Grenade_Explosion = "grenade_explosion";

	public const string Random_Pickup_Icon = "random_pickup";

	public static HolidayEvent mInstance;

	protected Hashtable m_HolidayData = new Hashtable();

	protected Hashtable m_HolidayAddData = new Hashtable();

	private HolidayEvent()
	{
		mInstance = this;
	}

	public static HolidayEvent CreateEvent(out LoadHolidayEvent LoadEvent, out UpdateHolidayEvent UpdateEvent, out DrawHolidayEvent DrawEvent)
	{
		if (GameData.eventObjects["Holiday_Script_Data"] == null)
		{
			LoadEvent = null;
			UpdateEvent = null;
			DrawEvent = null;
			return null;
		}
		HolidayEvent holidayEvent = new HolidayEvent();
		LoadEvent = holidayEvent.Load;
		UpdateEvent = holidayEvent.Update;
		DrawEvent = holidayEvent.Draw;
		return holidayEvent;
	}

	private static int LoadEventData(int def, string key)
	{
		if (GameData.eventObjects[key] != null)
		{
			return (int)GameData.eventObjects[key];
		}
		return def;
	}

	private static float LoadEventData(float def, string key)
	{
		if (GameData.eventObjects[key] != null)
		{
			return (float)GameData.eventObjects[key];
		}
		return def;
	}

	protected void Load()
	{
		SetBGColorShift(bFade: false);
		GameObject gameObject = GameObject.Find("GameMusic(Clone)");
		if (gameObject != null)
		{
			AudioClip audioClip = GameData.eventObjects["GameHome_MusicLoop"] as AudioClip;
			if (audioClip != null)
			{
				float time = gameObject.GetComponent<AudioSource>().time;
				gameObject.GetComponent<AudioSource>().clip = audioClip;
				gameObject.GetComponent<AudioSource>().Play();
				gameObject.GetComponent<AudioSource>().time = time;
			}
		}
		if (GameData.eventObjects["Spider_Manager_Create"] != null)
		{
			SpiderManager spiderManager = new SpiderManager();
			m_HolidayData[spiderManager] = spiderManager;
		}
		float min = LoadEventData(-1f, "Falling_Sprite_X_Min");
		float max = LoadEventData(1f, "Falling_Sprite_X_Max");
		int min2 = LoadEventData(40, "Falling_Sprite_Size_Min");
		int max2 = LoadEventData(60, "Falling_Sprite_Size_Max");
		float min3 = LoadEventData(40, "Falling_Sprite_Speed_Min");
		float max3 = LoadEventData(80, "Falling_Sprite_Speed_Max");
		int num = LoadEventData(0, "Falling_Sprite_Num");
		for (int i = 0; i < num; i++)
		{
			Vector2 normalized = new Vector2(UnityEngine.Random.Range(min, max), 1f).normalized;
			FallingSprite fallingSprite = new FallingSprite(UnityEngine.Random.Range(min2, max2), new Vector2(UnityEngine.Random.Range(-70, Screen.width + 70), UnityEngine.Random.Range(-50, Screen.height)), normalized, UnityEngine.Random.Range(min3, max3));
			m_HolidayData[fallingSprite] = fallingSprite;
		}
	}

	protected virtual void Update()
	{
		ArrayList arrayList = new ArrayList();
		foreach (DictionaryEntry holidayDatum in m_HolidayData)
		{
			HolidayObject holidayObject = holidayDatum.Value as HolidayObject;
			if (holidayObject != null && !holidayObject.Update())
			{
				arrayList.Add(holidayObject);
			}
		}
		foreach (object item in arrayList)
		{
			m_HolidayData.Remove(item);
		}
		foreach (DictionaryEntry holidayAddDatum in m_HolidayAddData)
		{
			m_HolidayData.Add(holidayAddDatum.Key, holidayAddDatum.Value);
		}
		m_HolidayAddData.Clear();
	}

	protected virtual void Draw()
	{
		foreach (DictionaryEntry holidayDatum in m_HolidayData)
		{
			if (holidayDatum.Value as HolidayDrawable != null) {
				(holidayDatum.Value as HolidayDrawable).Draw();
			}
		}
	}

	public static void SetLogoColorShift()
	{
		if (GameData.eventObjects["Title_Logo_Base_Color"] == null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Logo");
		if (gameObject != null)
		{
			MaterialColorShift materialColorShift = gameObject.AddComponent<MaterialColorShift>();
			materialColorShift.mBaseColor = (Color)GameData.eventObjects["Title_Logo_Base_Color"];
			if (GameData.eventObjects["Title_Logo_Shift_Color"] != null)
			{
				materialColorShift.mShiftColor = (Color)GameData.eventObjects["Title_Logo_Shift_Color"];
			}
			if (GameData.eventObjects["Title_Logo_Shift_Rate"] != null)
			{
				materialColorShift.mShiftRate = (float)GameData.eventObjects["Title_Logo_Shift_Rate"];
			}
		}
	}

	public static void SetBGColorShift(bool bFade)
	{
		if (GameData.eventObjects["Background_Hex_Base_Color"] == null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("Background");
		if (gameObject != null)
		{
			MaterialColorShift materialColorShift = gameObject.AddComponent<MaterialColorShift>();
			materialColorShift.mBaseColor = (Color)GameData.eventObjects["Background_Hex_Base_Color"];
			if (GameData.eventObjects["Background_Hex_Shift_Color"] != null)
			{
				materialColorShift.mShiftColor = (Color)GameData.eventObjects["Background_Hex_Shift_Color"];
			}
			if (GameData.eventObjects["Background_Hex_Shift_Rate"] != null)
			{
				materialColorShift.mShiftRate = (float)GameData.eventObjects["Background_Hex_Shift_Rate"];
			}
			materialColorShift.mFading = bFade;
		}
	}

	public static IEnumerator OnEventLoaded()
	{
		GameObject Obj = GameObject.Find("SceneScript");
		if (Obj != null)
		{
			TitleLoading TL = Obj.GetComponent<TitleLoading>();
			if (TL != null)
			{
				while (TL.GameMusic.GetComponent<AudioSource>().volume > 0f)
				{
					TL.GameMusic.GetComponent<AudioSource>().volume -= 0.02f;
					yield return new WaitForSeconds(0.01f);
				}
				TL.GameMusic.GetComponent<AudioSource>().clip = (GameData.eventObjects["TitleLoading_MusicLoop"] as AudioClip);
				TL.GameMusic.GetComponent<AudioSource>().Play();
				TL.GameMusic.GetComponent<AudioSource>().volume = 0f;
				while (TL.GameMusic.GetComponent<AudioSource>().volume < GameData.mGameSettings.mMusicVolume)
				{
					TL.GameMusic.GetComponent<AudioSource>().volume += 0.02f;
					yield return new WaitForSeconds(0.01f);
				}
				TL.GameMusic.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			}
		}
		SetLogoColorShift();
		SetBGColorShift(bFade: true);
	}

	public static IEnumerator OnQueueStart()
	{
		GameObject Sound = GameObject.Find("AtlasMusic(Clone)");
		if (Sound == null)
		{
			Sound = GameObject.Find("BanzaiMusic(Clone)");
		}
		if (Sound != null)
		{
			AudioClip eventQueueClip = GameData.eventObjects["Queue_MusicLoop"] as AudioClip;
			if (eventQueueClip != null)
			{
				Sound.GetComponent<AudioSource>().clip = eventQueueClip;
				Sound.GetComponent<AudioSource>().Play();
				Sound.GetComponent<AudioSource>().volume = GameData.mGameSettings.mMusicVolume;
			}
		}
		SetBGColorShift(bFade: false);
		yield return new WaitForEndOfFrame();
	}
}
