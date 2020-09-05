using System.Collections;
using UnityEngine;

public class MessageBox
{
	public class MessageItem
	{
		public string mTitle;

		public string mMessage;

		public Texture2D mIcon;

		public MessageType mType = MessageType.MB_OK;

		public string[] mCustomButtons;

		public MessageCallback mCallback;

		public bool bCloseButton = true;
	}

	public enum MessageType
	{
		MB_NoButtons,
		MB_OK,
		MB_CANCEL,
		MB_OKCANCEL,
		MB_RETRYCANCEL,
		MB_YESNO,
		MB_YESNOCANCEL,
		MB_CUSTOM
	}

	public enum ReturnType
	{
		MB_NOTHING = 0,
		MB_YES = 1,
		MB_NO = 2,
		MB_CANCEL = 3,
		MB_OK = 4,
		MB_RETRY = 5,
		MB_CLOSE = 6,
		MB_ONE = 1,
		MB_TWO = 2,
		MB_THREE = 3
	}

	public delegate void MessageCallback(ReturnType Return);

	public static MessageBox mMessageBox;

	private Rect mWindowPosition = new Rect(300f, 300f, 450f, 200f);

	private Rect mIconPosition = new Rect(20f, 20f, 50f, 50f);

	private Rect mFullMessagePosition = new Rect(0f, 0f, 450f, 200f);

	private Rect mPartMessagePosition = new Rect(80f, 20f, 300f, 80f);

	private Rect[] mButtonPositions = new Rect[3]
	{
		new Rect(26f, 150f, 126f, 38f),
		new Rect(162f, 150f, 126f, 38f),
		new Rect(298f, 150f, 126f, 38f)
	};

	private Queue mMessageQueue = new Queue();

	private string mMessage;

	private Texture2D mIcon;

	private bool bCloseButton = true;

	private MessageType mType = MessageType.MB_OK;

	private ReturnType mReturn;

	private string[] mCustomButtons;

	private static string lastHover = string.Empty;

	public Rect WindowPosition {
		get {
			return mWindowPosition;
		}
	}

	public int Queuesize {
		get {
			return mMessageQueue.Count;
		}
	}

	public static ReturnType Local(string Title, string Message, int Icon, bool bCloseButton, MessageType Type)
	{
		return Local(Title, Message, new Texture2D(16, 16), bCloseButton, Type);
	}

	public static ReturnType Local(string Title, string Message, Texture2D Icon, bool bCloseButton, MessageType Type)
	{
		mMessageBox.mMessage = Message;
		mMessageBox.mIcon = Icon;
		mMessageBox.bCloseButton = bCloseButton;
		mMessageBox.mType = Type;
		GUIUtil.OnDrawWindow();
		GUIUtil.GUIEnableOverride(bEnable: true);
		mMessageBox.mWindowPosition = GUI.Window(1000, mMessageBox.mWindowPosition, DoWindow, Title, GUIUtil.mInstance.mSharedSkin.window);
		GUI.BringWindowToFront(1000);
		GUIUtil.GUIEnableOverride(bEnable: false);
		return mMessageBox.mReturn;
	}

	public static ReturnType LocalCustom(string Title, string Message, int Icon, bool bCloseButton, params string[] Buttons)
	{
		return LocalCustom(Title, Message, new Texture2D(16, 16), bCloseButton, Buttons);
	}

	public static ReturnType LocalCustom(string Title, string Message, Texture2D Icon, bool bCloseButton, params string[] Buttons)
	{
		GUIUtil.GUIEnableOverride(bEnable: true);
		mMessageBox.mMessage = Message;
		mMessageBox.mIcon = Icon;
		mMessageBox.bCloseButton = bCloseButton;
		mMessageBox.mCustomButtons = Buttons;
		mMessageBox.mType = MessageType.MB_CUSTOM;
		GUIUtil.OnDrawWindow();
		GUIUtil.GUIEnableOverride(bEnable: true);
		mMessageBox.mWindowPosition = GUI.Window(1000, mMessageBox.mWindowPosition, DoWindow, Title, GUIUtil.mInstance.mSharedSkin.window);
		GUI.BringWindowToFront(1000);
		GUIUtil.GUIEnableOverride(bEnable: false);
		return mMessageBox.mReturn;
	}

	public static void AddMessage(string Title, string Message, int Icon, bool bCloseButton, MessageType Type, MessageCallback Callback)
	{
		AddMessage(Title, Message, new Texture2D(16, 16), bCloseButton, Type, Callback);
	}

	public static void AddMessage(string Title, string Message, Texture2D Icon, bool bCloseButton, MessageType Type, MessageCallback Callback)
	{
		MessageItem messageItem = new MessageItem();
		messageItem.mTitle = Title;
		messageItem.mMessage = Message;
		messageItem.mIcon = Icon;
		messageItem.bCloseButton = bCloseButton;
		messageItem.mCallback = Callback;
		messageItem.mType = Type;
		mMessageBox.mMessageQueue.Enqueue(messageItem);
	}

	public static void AddMessageCustom(string Title, string Message, int Icon, bool bCloseButton, MessageCallback Callback, params string[] Buttons)
	{
		AddMessageCustom(Title, Message, new Texture2D(16, 16), bCloseButton, Callback, Buttons);
	}

	public static void AddMessageCustom(string Title, string Message, Texture2D Icon, bool bCloseButton, MessageCallback Callback, params string[] Buttons)
	{
		MessageItem messageItem = new MessageItem();
		messageItem.mTitle = Title;
		messageItem.mMessage = Message;
		messageItem.mIcon = Icon;
		messageItem.bCloseButton = bCloseButton;
		messageItem.mCallback = Callback;
		messageItem.mCustomButtons = Buttons;
		messageItem.mType = MessageType.MB_CUSTOM;
		mMessageBox.mMessageQueue.Enqueue(messageItem);
	}

	private static void DoWindow(int ID)
	{
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		GUISkin skin = GUI.skin;
		GUI.skin = GUIUtil.mInstance.mSharedSkin;
		GUI.DragWindow(new Rect(0f, 0f, mMessageBox.mWindowPosition.width - 30f, 20f));
		mMessageBox.mReturn = ReturnType.MB_NOTHING;
		if (mMessageBox.mIcon == null)
		{
			GUI.Label(mMessageBox.mFullMessagePosition, mMessageBox.mMessage, GUI.skin.GetStyle("ModalText"));
		}
		else
		{
			GUI.DrawTexture(mMessageBox.mIconPosition, mMessageBox.mIcon);
			GUI.Label(mMessageBox.mPartMessagePosition, mMessageBox.mMessage, GUI.skin.GetStyle("ModalText"));
		}
		switch (mMessageBox.mType)
		{
		case MessageType.MB_OK:
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], "OK", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_OK";
				mMessageBox.mReturn = ReturnType.MB_OK;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_OK";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			break;
		case MessageType.MB_CANCEL:
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], "Cancel", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_CANCEL";
				mMessageBox.mReturn = ReturnType.MB_CANCEL;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_CANCEL";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			break;
		case MessageType.MB_OKCANCEL:
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], "OK", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_OK";
				mMessageBox.mReturn = ReturnType.MB_OK;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_OK";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[2], "Cancel", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_CANCEL";
				mMessageBox.mReturn = ReturnType.MB_CANCEL;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_CANCEL";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			break;
		case MessageType.MB_RETRYCANCEL:
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], "Retry", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_RETRY";
				mMessageBox.mReturn = ReturnType.MB_RETRY;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_RETRY";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[2], "Cancel", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_CANCEL";
				mMessageBox.mReturn = ReturnType.MB_CANCEL;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_CANCEL";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			break;
		case MessageType.MB_YESNO:
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], "Yes", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_YES";
				mMessageBox.mReturn = ReturnType.MB_YES;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_YES";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[2], "No", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_NO";
				mMessageBox.mReturn = ReturnType.MB_NO;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_NO";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			break;
		case MessageType.MB_YESNOCANCEL:
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[0], "Yes", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_YES";
				mMessageBox.mReturn = ReturnType.MB_YES;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_YES";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], "No", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_NO";
				mMessageBox.mReturn = ReturnType.MB_NO;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_NO";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			switch (GUIUtil.Button(mMessageBox.mButtonPositions[2], "Cancel", GUI.skin.GetStyle("ModalButton")))
			{
			case GUIUtil.GUIState.Click:
				b = "MB_CANCEL";
				mMessageBox.mReturn = ReturnType.MB_CANCEL;
				break;
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "MB_CANCEL";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			}
			break;
		case MessageType.MB_CUSTOM:
			if (mMessageBox.mCustomButtons == null)
			{
				break;
			}
			switch (mMessageBox.mCustomButtons.Length)
			{
			case 1:
				switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], mMessageBox.mCustomButtons[0], GUI.skin.GetStyle("ModalButton")))
				{
				case GUIUtil.GUIState.Click:
					b = "MB_ONE";
					mMessageBox.mReturn = ReturnType.MB_YES;
					break;
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "MB_ONE";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				}
				break;
			case 2:
				switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], mMessageBox.mCustomButtons[0], GUI.skin.GetStyle("ModalButton")))
				{
				case GUIUtil.GUIState.Click:
					b = "MB_ONE";
					mMessageBox.mReturn = ReturnType.MB_YES;
					break;
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "MB_ONE";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				}
				switch (GUIUtil.Button(mMessageBox.mButtonPositions[2], mMessageBox.mCustomButtons[1], GUI.skin.GetStyle("ModalButton")))
				{
				case GUIUtil.GUIState.Click:
					b = "MB_TWO";
					mMessageBox.mReturn = ReturnType.MB_NO;
					break;
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "MB_TWO";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				}
				break;
			case 3:
				switch (GUIUtil.Button(mMessageBox.mButtonPositions[0], mMessageBox.mCustomButtons[0], GUI.skin.GetStyle("ModalButton")))
				{
				case GUIUtil.GUIState.Click:
					b = "MB_ONE";
					mMessageBox.mReturn = ReturnType.MB_YES;
					break;
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "MB_ONE";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				}
				switch (GUIUtil.Button(mMessageBox.mButtonPositions[1], mMessageBox.mCustomButtons[1], GUI.skin.GetStyle("ModalButton")))
				{
				case GUIUtil.GUIState.Click:
					b = "MB_TWO";
					mMessageBox.mReturn = ReturnType.MB_NO;
					break;
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "MB_TWO";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				}
				switch (GUIUtil.Button(mMessageBox.mButtonPositions[2], mMessageBox.mCustomButtons[2], GUI.skin.GetStyle("ModalButton")))
				{
				case GUIUtil.GUIState.Click:
					b = "MB_THREE";
					mMessageBox.mReturn = ReturnType.MB_CANCEL;
					break;
				case GUIUtil.GUIState.Hover:
				case GUIUtil.GUIState.Active:
					if (Event.current.type == EventType.Repaint)
					{
						b = "MB_THREE";
						if (lastHover != b)
						{
							GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
						}
					}
					break;
				}
				break;
			}
			break;
		}
		if (mMessageBox.mReturn != 0)
		{
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
		}
		lastHover = b;
		GUI.skin = skin;
	}

	public static void DrawMessageQueue()
	{
		if (mMessageBox.mMessageQueue.Count <= 0)
		{
			return;
		}
		MessageItem messageItem = mMessageBox.mMessageQueue.Peek() as MessageItem;
		ReturnType returnType = ReturnType.MB_NOTHING;
		returnType = ((messageItem.mType != MessageType.MB_CUSTOM) ? Local(messageItem.mTitle, messageItem.mMessage, messageItem.mIcon, messageItem.bCloseButton, messageItem.mType) : LocalCustom(messageItem.mTitle, messageItem.mMessage, messageItem.mIcon, messageItem.bCloseButton, messageItem.mCustomButtons));
		if (returnType != 0)
		{
			if (messageItem.mCallback != null)
			{
				messageItem.mCallback(returnType);
			}
			mMessageBox.mMessageQueue.Dequeue();
			ResetWindowPosition();
		}
	}

	public static void ResetWindowPosition()
	{
		mMessageBox.mWindowPosition.x = (float)(Screen.width / 2) - mMessageBox.mWindowPosition.width / 2f;
		mMessageBox.mWindowPosition.y = (float)(Screen.height / 2) - mMessageBox.mWindowPosition.height / 2f;
	}
}
