using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GUIUtil : MonoBehaviour
{
	public enum GUIFlags
	{
		WindowUp = 1
	}

	public enum BarDirection
	{
		Left,
		Right,
		Up,
		Down
	}

	public enum GUISoundClips
	{
		TT_Global_Button_Press,
		TT_Global_Button_Over,
		TT_Hangar_Buy_Press,
		TT_Hangar_Equip_Press,
		TT_Hangar_Button_Over,
		TT_Hangar_Button_Press,
		TT_Hangar_Button_Inactive,
		TT_Hangar_Suit_Chooser_Change_Column,
		TT_Hangar_Suit_Chooser_Over,
		TT_Summary_Levelup,
		TT_Summary_XP_Tick,
		NUM_SOUNDS
	}

	[Serializable]
	public class LoadingAnimation
	{
		public Texture2D Texture;

		public int FPS;

		public int Frames;
	}

	public enum ScaleDir
	{
		UpperLeft,
		Upper,
		UpperRight,
		Right,
		LowerRight,
		Lower,
		LowerLeft,
		Left,
		Center
	}

	public enum GUIState
	{
		None = 0,
		Inactive = 1,
		Hover = 2,
		Active = 4,
		Click = 8,
		Toggle_On = 0x10,
		Toggle_Off = 0x20
	}

	private int mFlags;

	public static GUIUtil mInstance;

	public GUISkin mSharedSkin;

	public GUISkin mTabSkin;

	public GUISkin mShowcaseSkin;

	public GUISkin mQueueSkin;

	public GUISkin mStatsSkin;

	public Camera mModelRenderer;

	public Transform mBackground;

	public Transform mSuitEffect;

	public static string Tooltip = string.Empty;

	public static AudioClip[] GUISounds = new AudioClip[11];

	private static string[] GUISoundStrings = new string[11]
	{
		"Sounds/exonaut_globalBtn_press",
		"Sounds/exonaut_globalBtn_over",
		"Sounds/hangar_btn_buy_suit_press",
		"Sounds/hangar_btn_equip_suit_press",
		"Sounds/hangar_btn_over",
		"Sounds/hangar_btn_press",
		"Sounds/hangar_btn_inactive",
		"Sounds/hangar_btn_suit_chooser_change_column",
		"Sounds/hangar_btn_suit_chooser_over",
		"Sounds/sfx_XP_count_levelUP",
		"Sounds/sfx_XP_count_tick"
	};

	public LoadingAnimation[] LoadingAnims;

	private static Matrix4x4 tempMatrix = Matrix4x4.identity;

	public Vector2 sspci = Vector2.zero;

	public static string[] Tips = new string[57]
	{
		"Don't forget to use your jetpack! Just click the right mouse button to fly around the map.",
		"You can roll or airdash to quickly close the distance to opponents and take them by surprise.",
		"You can roll or airdash to quickly escape from opponents when you're in danger.",
		"Exosuit helmets contain vital control systems, so you can often get a critical hit and do extra damage if you hit an enemy's helmet.",
		"An exosuit's Tech factor determines how quickly you can pick up boosts and heavy weapons.",
		"Your exosuit's weapon mod makes that weapon stronger, but it doesn't always mean it's the best weapon for every situation.",
		"Watch out for invisible enemies! If an opponent picks up a stealth boost, you won't be able to see them.",
		"You can't roll through rockets, so be extra careful if the enemy has a Rocket Pod.",
		"The Longshot can hit opponents from across the map. Use it carefully, and they won't even know you're there.",
		"Sometimes you can use your jetpack to fly over bouncing Helix Grenades.",
		"The Marksman specializes in taking down single opponents from long range.",
		"Every exosuit uses the Bulldog energy pistol as a backup sidearm because it never runs out of ammo.",
		"The Wildfire sprays so many shots so quickly it can overwhelm unsuspecting enemies.",
		"When the short range Ballista scores a hit, it can devastate nearby opponents.",
		"Featuring a balance of damage, range and speed, the Tridex fires three shots in a wide angle.",
		"All exosuits can throw Helix Grenades, compact spheres that explode in a burst of Helix energy.",
		"The Rocket Pod can sway the tide of battle with its massive explosions.",
		"The Helix Cannon launches Helix Grenades with great range and accuracy.",
		"The Longshot can zoom in on enemies and deal heavy damage from a safe distance.",
		"Boosts can make you faster, stronger, tougher, and even make you invisible.",
		"You can't pick up a boost while you have another boost active.",
		"After you pick up a heavy weapon, you won't be able to pick up another heavy weapon for a short time.",
		"When you run out of ammo for a heavy weapon, you'll switch to your Bulldog energy pistol.",
		"Some exosuits are fast but can't take much punishment.",
		"Some exosuits are slow but have really tough shields.",
		"The exosuits available to guest players change often.",
		"When your exosuit radiates orange energy, you're about to crash.",
		"Completing missions gives you XP and credits and rewards you with collectible medals.",
		"Each faction has its own set of exclusive exosuits.",
		"When you run out of ammo, you'll still have your Bulldog energy pistol as backup.",
		"Jump before you jetpack to take off faster and conserve fuel.",
		"Each weapon has different strengths and weaknesses. Experiment and see which one is right for you.",
		"The Tridex and Wildfire are good weapons for new recruits.",
		"You'll always earn some XP and credits for every match you play, but you'll earn more when you win.",
		"Opponents leaving a crash bubble are invincible for a moment. Watch out, because you can't hurt them during that time!",
		"Team boosts will power up your entire team. Be sure to grab them before the other team does!",
		"When you come out of a crash bubble, you're invincible for a moment. Use this to your advantage since your enemies can't hurt you during that time!",
		"You can always check your mission progress on the Project Exonaut website.",
		"When you earn a new rank, you'll unlock new exosuits for purchase in the Exosuit Hangar.",
		"Your weapon will reload automatically when it uses up a clip of ammo. Or you can reload manually any time you want by pressing the \"R\" key.",
		"A well-timed grenade can sometimes block a rocket attack.",
		"You can roll through tight spaces to get past obstacles or evade an enemy.",
		"There's no friendly fire in Team Battles, so don't worry about hurting your teammates.",
		"Keep an eye on your jetpack fuel. If you run it all the way empty, it'll take longer to recharge.",
		"You can use your credits to buy new exosuits in the Exosuit Hangar.",
		"In the Exosuit Hangar, you can browse all of your faction's exosuits, buy new exosuits or equip any exosuit you already own.",
		"Some weapons have longer range than others. Watch how far your shots travel, and be careful if the enemy is using a weapon with longer range.",
		"Some weapons do a lot more damage the closer you are to your opponent.",
		"Remember to reload between fights so you don't get stuck reloading in the heat of battle.",
		"You can pick up extra Helix Grenades just by running over them, but you have to stay in place for a moment to pick up boosts or heavy weapons.",
		"The random boost has a question mark on it. You never know what boost you'll get from it, but it'll always be helpful.",
		"When you go near a boost or heavy weapon, a timer will appear to show you how long it takes to pick it up.",
		"Remember to take advantage of the environment. Learn how to get around each map and use obstacles for cover.",
		"Every time you crash, you can choose a different weapon. Don't be afraid to switch things up if you're having trouble using a particular weapon.",
		"Be sure to use teamwork in Team Battles. Work together with your teammates and win the match for your faction!",
		"In a free-for-all Battle, every player is against every other player. So attack anybody you see!",
		"In a Team Battle you can't hurt the players on your team, but you should attack anybody on the opposing team!"
	};

	public static bool GUIEnabled
	{
		get
		{
			return GUI.enabled;
		}
		set
		{
			GUIEnable(value);
		}
	}

	public static bool TestFlag(GUIFlags Flag)
	{
		return (mInstance.mFlags & (int)Flag) != 0;
	}

	public static void PlayGUISound(string EnumName)
	{
		PlayGUISound((GUISoundClips)(int)Enum.Parse(typeof(GUISoundClips), EnumName));
	}

	public static void PlayGUISound(GUISoundClips sound)
	{
		if (sound >= GUISoundClips.TT_Global_Button_Press && (int)sound < GUISounds.Length)
		{
			AudioClip audioClip = GUISounds[(int)sound];
			if (audioClip != null)
			{
				mInstance.GetComponent<AudioSource>().PlayOneShot(audioClip);
			}
			else
			{
				Logger.trace("===== NULL SOUND : " + sound + " =====");
			}
		}
		else
		{
			Logger.trace("PlayGUISound out array range");
		}
	}

	private void Start()
	{
		mInstance = this;
	}

	private void Awake()
	{
		mInstance = this;
		for (int i = 0; i < 11; i++)
		{
			GUISounds[i] = (Resources.Load(GUISoundStrings[i]) as AudioClip);
		}
		MessageBox.mMessageBox = new MessageBox();
		MessageBox.ResetWindowPosition();
		mModelRenderer.gameObject.active = true;
		mModelRenderer.enabled = false;
		mBackground = (UnityEngine.Object.Instantiate(mBackground) as Transform);
		mBackground.transform.position = base.transform.position;
		mSuitEffect = (UnityEngine.Object.Instantiate(mSuitEffect) as Transform);
		mSuitEffect.transform.position = base.transform.position;
		mSuitEffect.gameObject.SetActiveRecursively(state: false);
		UnityEngine.Object.DontDestroyOnLoad(mBackground);
		UnityEngine.Object.DontDestroyOnLoad(mSuitEffect);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void GUIEnable(bool bEnable)
	{
		if (bEnable && (mInstance.mFlags & 1) != 0)
		{
			GUI.enabled = false;
		}
		else
		{
			GUI.enabled = bEnable;
		}
	}

	public static void GUIEnableOverride(bool bEnable)
	{
		GUI.enabled = bEnable;
	}

	public static void DrawProgressBar(Rect Area, float val, float minVal, float maxVal, BarDirection Type, GUIStyle BackgroundStyle, GUIStyle BarStyle)
	{
		if (BarStyle != null)
		{
			Rect position = Area;
			float num = Mathf.Clamp((val - minVal) / (maxVal - minVal), 0f, 1f);
			if (BackgroundStyle != null)
			{
				Color color = GUI.color;
				GUI.color = Color.white;
				GUI.Box(position, string.Empty, BackgroundStyle);
				GUI.color = color;
			}
			switch (Type)
			{
			case BarDirection.Left:
			{
				float num3 = Area.width - Area.width * num;
				Area.x += num3;
				Area.width -= num3;
				GUI.BeginGroup(Area);
				GUI.Box(new Rect(0f - num3, 0f, position.width, position.height), GUIContent.none, BarStyle);
				GUI.EndGroup();
				break;
			}
			case BarDirection.Right:
				Area.width *= num;
				GUI.BeginGroup(Area);
				GUI.Box(new Rect(0f, 0f, position.width, position.height), GUIContent.none, BarStyle);
				GUI.EndGroup();
				break;
			case BarDirection.Up:
			{
				float num2 = Area.height - Area.height * num;
				Area.y += num2;
				Area.height -= num2;
				GUI.BeginGroup(Area);
				GUI.Box(new Rect(0f, 0f - num2, position.width, position.height), GUIContent.none, BarStyle);
				GUI.EndGroup();
				break;
			}
			case BarDirection.Down:
				Area.height *= num;
				GUI.BeginGroup(Area);
				GUI.Box(new Rect(0f, 0f, position.width, position.height), GUIContent.none, BarStyle);
				GUI.EndGroup();
				break;
			}
		}
	}

	public static void RenderModelToTexture(RenderTexture Texture, float FoV, Vector3 Position, Vector3 Rotation, Transform Obj, Color ClearColor)
	{
		Obj.gameObject.SetActiveRecursively(state: true);
		Obj.parent = mInstance.mModelRenderer.gameObject.transform;
		Obj.transform.localPosition = Position;
		Obj.transform.localRotation = Quaternion.Euler(Rotation);
		mInstance.mModelRenderer.backgroundColor = ClearColor;
		mInstance.mModelRenderer.targetTexture = Texture;
		mInstance.mModelRenderer.fieldOfView = FoV;
		mInstance.mModelRenderer.Render();
		Obj.parent = null;
		Obj.gameObject.SetActiveRecursively(state: false);
	}

	public static void RenderModelsToTexture(RenderTexture Texture, float FoV, Vector3[] Position, Vector3[] Rotation, Transform[] Obj, Color ClearColor)
	{
		if (Position.Length != Rotation.Length && Rotation.Length != Obj.Length)
		{
			return;
		}
		for (int i = 0; i < Obj.Length; i++)
		{
			if (Obj[i] != null)
			{
				Obj[i].gameObject.SetActiveRecursively(state: true);
				Obj[i].parent = mInstance.mModelRenderer.gameObject.transform;
				Obj[i].transform.localPosition = Position[i];
				Obj[i].transform.localRotation = Quaternion.Euler(Rotation[i]);
			}
		}
		mInstance.mModelRenderer.backgroundColor = ClearColor;
		mInstance.mModelRenderer.targetTexture = Texture;
		mInstance.mModelRenderer.fieldOfView = FoV;
		mInstance.mModelRenderer.Render();
		for (int j = 0; j < Obj.Length; j++)
		{
			if (Obj[j] != null)
			{
				Obj[j].parent = null;
				Obj[j].gameObject.SetActiveRecursively(state: false);
			}
		}
	}

	public static void RenderModelToGUI(Rect RenderArea, float FoV, Vector3 Position, Vector3 Rotation, Transform Obj, Color ClearColor)
	{
		Obj.gameObject.SetActiveRecursively(state: true);
		Obj.parent = mInstance.mModelRenderer.gameObject.transform;
		Obj.transform.localPosition = Position;
		Obj.transform.localRotation = Quaternion.Euler(Rotation);
		mInstance.mModelRenderer.backgroundColor = ClearColor;
		mInstance.mModelRenderer.fieldOfView = FoV;
		mInstance.mModelRenderer.rect = RenderArea;
		mInstance.mModelRenderer.Render();
		Obj.parent = null;
		Obj.gameObject.SetActiveRecursively(state: false);
	}

	public static void RenderModelsToGUI(Rect RenderArea, float FoV, Vector3[] Position, Vector3[] Rotation, Transform[] Obj, Color ClearColor)
	{
		if (Position.Length != Rotation.Length && Rotation.Length != Obj.Length)
		{
			return;
		}
		for (int i = 0; i < Obj.Length; i++)
		{
			if (Obj[i] != null)
			{
				Obj[i].gameObject.SetActiveRecursively(state: true);
				Obj[i].parent = mInstance.mModelRenderer.gameObject.transform;
				Obj[i].transform.localPosition = Position[i];
				Obj[i].transform.localRotation = Quaternion.Euler(Rotation[i]);
			}
		}
		mInstance.mModelRenderer.backgroundColor = ClearColor;
		mInstance.mModelRenderer.rect = RenderArea;
		mInstance.mModelRenderer.fieldOfView = FoV;
		mInstance.mModelRenderer.Render();
		for (int j = 0; j < Obj.Length; j++)
		{
			if (Obj[j] != null)
			{
				Obj[j].parent = null;
				Obj[j].gameObject.SetActiveRecursively(state: false);
			}
		}
	}

	public void LateUpdate()
	{
		mInstance.mFlags &= -2;
	}

	public void OnGUI()
	{
		mInstance = this;
		MessageBox.DrawMessageQueue();
	}

	public static void OnDrawWindow()
	{
		mInstance.mFlags |= 1;
		if (MessageBox.mMessageBox.Queuesize == 0)
		{
			GUIEnableOverride(bEnable: true);
		}
		else
		{
			GUIEnable(bEnable: true);
		}
	}

	public static void DrawNewIcon(Rect ObjectRect)
	{
		float num = ObjectRect.width / 2f + ObjectRect.x;
		GUI.color = Color.red;
		GUI.Box(new Rect(num - 20f, ObjectRect.y - 7f, 40f, 14f), "NEW!", mInstance.mSharedSkin.box);
		GUI.color = Color.white;
	}

	public static void DrawLoadingAnim(Rect pos, int index)
	{
		LoadingAnimation loadingAnimation = mInstance.LoadingAnims[index];
		DrawAnimatedTexture(pos, loadingAnimation.Texture, loadingAnimation.Frames, loadingAnimation.FPS, MirrorX: false, MirrorY: false);
	}

	public static void DrawAnimatedTextureFrame(Rect pos, Texture2D texture, int frames, int frame, bool MirrorX, bool MirrorY)
	{
		GUI.BeginGroup(pos);
		if (MirrorX)
		{
			pos.x = pos.width * (float)frames - (float)frame * pos.width;
			pos.width = (0f - pos.width) * (float)frames;
		}
		else
		{
			pos.x = (float)(-frame) * pos.width;
			pos.width *= frames;
		}
		if (MirrorY)
		{
			pos.y = pos.height;
			pos.height = 0f - pos.height;
		}
		else
		{
			pos.y = 0f;
		}
		GUI.DrawTexture(pos, texture);
		GUI.EndGroup();
	}

	public static void DrawAnimatedTexture(Rect pos, Texture2D texture, int frames, int FPS, bool MirrorX, bool MirrorY)
	{
		DrawAnimatedTextureFrame(pos, texture, frames, (int)(Time.realtimeSinceStartup * (float)FPS) % frames, MirrorX, MirrorY);
	}

	public static void BeginScaleGroup(Rect rect, Vector2 Angle, Vector3 Scale)
	{
		Vector3 vector = new Vector3(Mathf.Abs(Scale.x), Mathf.Abs(Scale.y), Mathf.Abs(Scale.z));
		Angle.x += (Screen.width - 900) / 2;
		Angle.y += (Screen.height - 600) / 2;
		GUI.BeginGroup(new Rect(rect.x / vector.x, rect.y / vector.y, (!(vector.x >= 1f)) ? (rect.width / vector.x) : (rect.width * vector.x), (!(vector.y >= 1f)) ? (rect.height / vector.y) : (rect.height * vector.y)));
		tempMatrix = GUI.matrix;
		Vector3 zero = Vector3.zero;
		if (Scale.x > 0f)
		{
			zero.x = (1f - Scale.x) * (Angle.x + rect.width / 2f);
		}
		else
		{
			zero.x = rect.x * 2f + rect.width + (-1f - Scale.x) * (0f - Angle.x + rect.width / 2f);
		}
		if (Scale.y > 0f)
		{
			zero.y = (1f - Scale.y) * (Angle.y + rect.height / 2f);
		}
		else
		{
			zero.y = rect.y * 2f + rect.height + (-1f - Scale.y) * (0f - Angle.y + rect.height / 2f);
		}
		GUI.matrix *= Matrix4x4.TRS(zero, Quaternion.identity, Scale);
	}

	public static void EndScaleGroup()
	{
		GUI.matrix = tempMatrix;
		GUI.EndGroup();
	}

	public static GUIState Button(Rect rect, GUIContent content)
	{
		return Button(rect, content, GUI.skin.button);
	}

	public static GUIState Button(Rect rect, string text)
	{
		return Button(rect, new GUIContent(text), GUI.skin.button);
	}

	public static GUIState Button(Rect rect, Texture image)
	{
		return Button(rect, new GUIContent(image), GUI.skin.button);
	}

	public static GUIState Button(Rect rect, GUIContent content, GUIStyle style)
	{
		int num = 0;
		if (!GUI.enabled)
		{
			num |= 1;
		}
		if (GUI.Button(rect, content, style))
		{
			return (GUIState)(num | 8);
		}
		if (rect.Contains(Event.current.mousePosition))
		{
			num = ((!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) ? (num | 2) : (num | 4));
		}
		return (GUIState)num;
	}

	public static GUIState Button(Rect rect, string text, GUIStyle style)
	{
		return Button(rect, new GUIContent(text), style);
	}

	public static GUIState Button(Rect rect, Texture image, GUIStyle style)
	{
		return Button(rect, new GUIContent(image), style);
	}

	public static GUIState RepeatButton(Rect rect, GUIContent content)
	{
		return RepeatButton(rect, content, GUI.skin.button);
	}

	public static GUIState RepeatButton(Rect rect, string text)
	{
		return RepeatButton(rect, new GUIContent(text), GUI.skin.button);
	}

	public static GUIState RepeatButton(Rect rect, Texture image)
	{
		return RepeatButton(rect, new GUIContent(image), GUI.skin.button);
	}

	public static GUIState RepeatButton(Rect rect, GUIContent content, GUIStyle style)
	{
		int num = 0;
		if (!GUI.enabled)
		{
			num |= 1;
		}
		if (GUI.RepeatButton(rect, content, style))
		{
			return (GUIState)(num | 8);
		}
		if (rect.Contains(Event.current.mousePosition))
		{
			num |= 2;
		}
		return (GUIState)num;
	}

	public static GUIState RepeatButton(Rect rect, string text, GUIStyle style)
	{
		return RepeatButton(rect, new GUIContent(text), style);
	}

	public static GUIState RepeatButton(Rect rect, Texture image, GUIStyle style)
	{
		return RepeatButton(rect, new GUIContent(image), style);
	}

	public static GUIState Toggle(Rect rect, bool value, GUIContent content)
	{
		return Toggle(rect, value, content, GUI.skin.button);
	}

	public static GUIState Toggle(Rect rect, bool value, string text)
	{
		return Toggle(rect, value, new GUIContent(text), GUI.skin.button);
	}

	public static GUIState Toggle(Rect rect, bool value, Texture image)
	{
		return Toggle(rect, value, new GUIContent(image), GUI.skin.button);
	}

	public static GUIState Toggle(Rect rect, bool value, GUIContent content, GUIStyle style)
	{
		int num = 0;
		if (!GUI.enabled)
		{
			num |= 1;
		}
		bool flag = GUI.Toggle(rect, value, content, style);
		num = ((!flag) ? (num | 0x20) : (num | 0x10));
		if (flag != value)
		{
			return (GUIState)(num | 8);
		}
		if (rect.Contains(Event.current.mousePosition))
		{
			num = ((!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) ? (num | 2) : (num | 4));
		}
		return (GUIState)num;
	}

	public static GUIState Toggle(Rect rect, bool value, string text, GUIStyle style)
	{
		return Toggle(rect, value, new GUIContent(text), style);
	}

	public static GUIState Toggle(Rect rect, bool value, Texture image, GUIStyle style)
	{
		return Toggle(rect, value, new GUIContent(image), style);
	}

	public static GUIState CircleButton(Circle circ, Rect rect, GUIContent content)
	{
		return CircleButton(circ, rect, content, GUI.skin.button);
	}

	public static GUIState CircleButton(Circle circ, Rect rect, string text)
	{
		return CircleButton(circ, rect, new GUIContent(text), GUI.skin.button);
	}

	public static GUIState CircleButton(Circle circ, Rect rect, Texture image)
	{
		return CircleButton(circ, rect, new GUIContent(image), GUI.skin.button);
	}

	public static GUIState CircleButton(Circle circ, Rect rect, GUIContent content, GUIStyle style)
	{
		int num = 0;
		if (!GUI.enabled)
		{
			num |= 1;
		}
		if (!rect.Contains(Event.current.mousePosition))
		{
			if (Event.current.type == EventType.Repaint)
			{
				style.Draw(rect, content, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
			}
			return (GUIState)num;
		}
		GUI.BeginGroup(rect);
		bool flag = circ.Contains(Event.current.mousePosition);
		GUI.EndGroup();
		if (flag)
		{
			num = (int)Button(rect, content, style);
		}
		else if (Event.current.type == EventType.Repaint)
		{
			style.Draw(rect, content, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
		}
		return (GUIState)num;
	}

	public static GUIState CircleButton(Circle circ, Rect rect, string text, GUIStyle style)
	{
		return CircleButton(circ, rect, new GUIContent(text), style);
	}

	public static GUIState CircleButton(Circle circ, Rect rect, Texture image, GUIStyle style)
	{
		return CircleButton(circ, rect, new GUIContent(image), style);
	}

	public static GUIState CustomButton(Triangle[] Tris, Rect rect, GUIContent content)
	{
		return CustomButton(Tris, rect, content, GUI.skin.button);
	}

	public static GUIState CustomButton(Triangle[] Tris, Rect rect, string text)
	{
		return CustomButton(Tris, rect, new GUIContent(text), GUI.skin.button);
	}

	public static GUIState CustomButton(Triangle[] Tris, Rect rect, Texture image)
	{
		return CustomButton(Tris, rect, new GUIContent(image), GUI.skin.button);
	}

	public static GUIState CustomButton(Triangle[] Tris, Rect rect, GUIContent content, GUIStyle style)
	{
		int num = 0;
		if (!GUI.enabled)
		{
			num |= 1;
		}
		if (!rect.Contains(Event.current.mousePosition))
		{
			if (Event.current.type == EventType.Repaint)
			{
				style.Draw(rect, content, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
			}
			return (GUIState)num;
		}
		GUI.BeginGroup(rect);
		bool flag = false;
		Vector2 mousePosition = Event.current.mousePosition;
		float x = mousePosition.x / rect.width;
		Vector2 mousePosition2 = Event.current.mousePosition;
		Vector2 v = new Vector2(x, mousePosition2.y / rect.height);
		foreach (Triangle triangle in Tris)
		{
			if (triangle.Contains(v))
			{
				flag = true;
				break;
			}
		}
		GUI.EndGroup();
		if (flag)
		{
			num = (int)Button(rect, content, style);
		}
		else if (Event.current.type == EventType.Repaint)
		{
			style.Draw(rect, content, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
		}
		return (GUIState)num;
	}

	public static GUIState CustomButton(Triangle[] Tris, Rect rect, string text, GUIStyle style)
	{
		return CustomButton(Tris, rect, new GUIContent(text), style);
	}

	public static GUIState CustomButton(Triangle[] Tris, Rect rect, Texture image, GUIStyle style)
	{
		return CustomButton(Tris, rect, new GUIContent(image), style);
	}

	public static string GetRandomTip()
	{
		return Tips[UnityEngine.Random.Range(0, Tips.Length)];
	}
}
