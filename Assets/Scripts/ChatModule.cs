using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System.Collections;
using UnityEngine;

public class ChatModule
{
	internal class ChatMessage
	{
		public enum ChatType
		{
			IGNORE,
			SYSTEM,
			CHAT,
			JOIN,
			LEAVE,
			FACTION_CHAT
		}

		private ChatType type;

		private string message;

		public ChatMessage()
		{
			type = ChatType.IGNORE;
			message = string.Empty;
		}

		public ChatMessage(ChatType type, string message)
		{
			this.type = type;
			this.message = message;
		}

		public ChatType GetChatType()
		{
			return type;
		}

		public string GetMessage()
		{
			return message;
		}
	}

	private SmartFox sfs;

	private ArrayList messages = new ArrayList();

	private string newMessage = string.Empty;

	private Vector2 chatScrollPosition;

	private Rect chatWindow;

	public bool bChatOn = true;

	public bool bAllChat = true;

	private GUISkin QueueSkin;

	private string lastHover = string.Empty;

	public ChatModule(QueueBattle Parent)
	{
		sfs = Parent.m_networkManager.smartFox;
		sfs = null;
		QueueSkin = GUIUtil.mInstance.mQueueSkin;
		if (GameData.BattleType == 2)
		{
			bAllChat = false;
		}
	}

	public void OnUpdate()
	{
	}

	public void AddSystemMessage(string message)
	{
		messages.Add(new ChatMessage(ChatMessage.ChatType.SYSTEM, message));
		chatScrollPosition.y = 100000f;
	}

	public void AddChatMessage(string message)
	{
		messages.Add(new ChatMessage(ChatMessage.ChatType.CHAT, message));
		chatScrollPosition.y = 100000f;
	}

	public void AddFactionChatMessage(string message, int factionId)
	{
		messages.Add(new ChatMessage(ChatMessage.ChatType.FACTION_CHAT, message));
		chatScrollPosition.y = 100000f;
	}

	public void AddPlayerJoinMessage(string message)
	{
		messages.Add(new ChatMessage(ChatMessage.ChatType.JOIN, message));
		chatScrollPosition.y = 100000f;
	}

	public void AddPlayerLeftMessage(string message)
	{
		messages.Add(new ChatMessage(ChatMessage.ChatType.LEAVE, message));
		chatScrollPosition.y = 100000f;
	}

	public void drawChatWindow(Rect chatRect)
	{
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		GUI.Box(new Rect(0f, chatRect.height - 60f, chatRect.width - 180f, 60f), string.Empty, QueueSkin.GetStyle("BottomFrame_Left"));
		GUI.Box(new Rect(chatRect.width - 179f, chatRect.height - 60f, 179f, 60f), string.Empty, QueueSkin.GetStyle("BottomFrame_Right"));
		GUI.Box(new Rect(chatRect.width - 180f, chatRect.height - 58f, 1f, 58f), string.Empty, QueueSkin.GetStyle("BottomFrame_Divide"));
		GUI.Box(new Rect(0f, 0f, chatRect.width, 5f), string.Empty, QueueSkin.GetStyle("ChatFrame_Top"));
		GUI.Box(new Rect(0f, 5f, chatRect.width, chatRect.height - 65f), string.Empty, QueueSkin.GetStyle("ChatFrame_Background"));
		GUISkin skin = GUI.skin;
		GUI.skin = QueueSkin;
		GUILayout.BeginArea(new Rect(10f, 5f, chatRect.width, chatRect.height));
		chatScrollPosition = GUILayout.BeginScrollView(chatScrollPosition, GUILayout.Width(chatRect.width - 10f), GUILayout.Height(chatRect.height - 65f));
		GUI.skin = skin;
		if (messages.Count > 0)
		{
			foreach (ChatMessage message in messages)
			{
				GUILayout.BeginHorizontal();
				switch (message.GetChatType())
				{
				case ChatMessage.ChatType.SYSTEM:
					GUILayout.Label(message.GetMessage(), QueueSkin.GetStyle("SystemText"));
					break;
				case ChatMessage.ChatType.CHAT:
					if (bChatOn)
					{
						int num2 = message.GetMessage().IndexOf(":") + 1;
						GUILayout.Label(message.GetMessage().Substring(0, num2), QueueSkin.GetStyle("ChatPlayerName"));
						GUILayout.Label(message.GetMessage().Substring(num2, message.GetMessage().Length - num2), QueueSkin.GetStyle("ChatText"));
					}
					break;
				case ChatMessage.ChatType.FACTION_CHAT:
					if (bChatOn)
					{
						int num = message.GetMessage().IndexOf(":") + 1;
						if (GameData.MyFactionId == 1)
						{
							GUILayout.Label(message.GetMessage().Substring(0, num), QueueSkin.GetStyle("ChatBanzaiName"));
							GUILayout.Label(message.GetMessage().Substring(num, message.GetMessage().Length - num), QueueSkin.GetStyle("BanzaiText"));
						}
						else
						{
							GUILayout.Label(message.GetMessage().Substring(0, num), QueueSkin.GetStyle("ChatAtlasName"));
							GUILayout.Label(message.GetMessage().Substring(num, message.GetMessage().Length - num), QueueSkin.GetStyle("AtlasText"));
						}
					}
					break;
				case ChatMessage.ChatType.JOIN:
				case ChatMessage.ChatType.LEAVE:
					GUILayout.Label(message.GetMessage(), QueueSkin.GetStyle("SystemText"));
					break;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(1f);
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		bool flag = true;
		GUIUtil.GUIEnable(bEnable: false);
		switch (GUIUtil.Button(new Rect(chatRect.width - 260f, chatRect.height - 51f, 64f, 42f), string.Empty, QueueSkin.GetStyle("Chat_Button")))
		{
		case GUIUtil.GUIState.Hover:
			if (Event.current.type == EventType.Repaint)
			{
				b = "Chat_Button";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
		{
			b = "Chat_Button";
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			int num3 = 0;
			while (num3 < messages.Count)
			{
				if (messages[num3] is ChatMessage)
				{
					ChatMessage chatMessage2 = messages[num3] as ChatMessage;
					if (chatMessage2.GetChatType() == ChatMessage.ChatType.CHAT || chatMessage2.GetChatType() == ChatMessage.ChatType.FACTION_CHAT)
					{
						messages.RemoveAt(num3);
						continue;
					}
				}
				num3++;
			}
			break;
		}
		}
		GUI.Toggle(new Rect(chatRect.width - 260f, chatRect.height - 51f, 64f, 42f), bChatOn, string.Empty, QueueSkin.GetStyle("ChatPref"));
		GUIUtil.GUIEnable(bChatOn);
		if (GameData.BattleType == 2)
		{
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				if (newMessage.Length > 0)
				{
					flag = false;
					sendChatMessage(newMessage, !bAllChat);
					newMessage = string.Empty;
				}
			}
			switch (GUIUtil.Button(new Rect(chatRect.width - 440f, chatRect.height - 51f, 80f, 42f), "SUBMIT", QueueSkin.GetStyle("Chat_Button")))
			{
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "SUBMIT";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			case GUIUtil.GUIState.Click:
				b = "SUBMIT";
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				if (newMessage.Length > 0)
				{
					flag = false;
					sendChatMessage(newMessage, !bAllChat);
					newMessage = string.Empty;
				}
				break;
			}
			switch (GUIUtil.Button(new Rect(chatRect.width - 350f, chatRect.height - 51f, 80f, 42f), (!bAllChat) ? "FACTION\nCHAT" : "ALL\nCHAT", QueueSkin.GetStyle("Chat_Button")))
			{
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "TOGGLECHAT";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			case GUIUtil.GUIState.Click:
				b = "TOGGLECHAT";
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				bAllChat = !bAllChat;
				break;
			}
			if (flag)
			{
				GUI.SetNextControlName("TextBox");
				newMessage = GUI.TextField(new Rect(10f, chatRect.height - 50f, chatRect.width - 460f, 40f), newMessage, 60, QueueSkin.GetStyle("ChatText_Box"));
				GUI.FocusControl("TextBox");
			}
			newMessage = newMessage.Replace("\n", string.Empty);
		}
		else
		{
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
			{
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				if (newMessage.Length > 0)
				{
					flag = false;
					sendChatMessage(newMessage, forFactionOnly: false);
					newMessage = string.Empty;
				}
			}
			switch (GUIUtil.Button(new Rect(chatRect.width - 350f, chatRect.height - 51f, 80f, 42f), "SUBMIT", QueueSkin.GetStyle("Chat_Button")))
			{
			case GUIUtil.GUIState.Hover:
			case GUIUtil.GUIState.Active:
				if (Event.current.type == EventType.Repaint)
				{
					b = "SUBMIT";
					if (lastHover != b)
					{
						GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
					}
				}
				break;
			case GUIUtil.GUIState.Click:
				b = "SUBMIT";
				GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
				if (newMessage.Length > 0)
				{
					flag = false;
					sendChatMessage(newMessage, forFactionOnly: false);
					newMessage = string.Empty;
				}
				break;
			}
			if (flag)
			{
				GUI.SetNextControlName("TextBox");
				newMessage = GUI.TextField(new Rect(10f, chatRect.height - 50f, chatRect.width - 370f, 40f), newMessage, 60, QueueSkin.GetStyle("ChatText_Box"));
				GUI.FocusControl("TextBox");
			}
			newMessage = newMessage.Replace("\n", string.Empty);
		}
		GUIUtil.GUIEnable(bEnable: true);
		lastHover = b;
	}

	public void sendChatMessage(string message, bool forFactionOnly)
	{
		if (sfs != null)
		{
			SFSObject sFSObject = new SFSObject();
			Logger.trace("send chat message: " + GameData.MyDisplayName);
			sFSObject.PutInt("playerId", GameData.MyPlayerId);
			sFSObject.PutInt("msgType", 110);
			sFSObject.PutUtfString("playerName", GameData.MyDisplayName);
			sFSObject.PutUtfString("msg", message);
			sFSObject.PutBool("factOnly", forFactionOnly);
			sfs.Send(new ExtensionRequest("evt", sFSObject, GameData.GameRoom));
		}
	}
}
