using System;
using UnityEngine;

internal class SocialPlayer
{
	public enum SocialPlayerState
	{
		Offline,
		In_Menus,
		In_Queue,
		In_Battle,
		In_Team_Battle
	}

	private string playerName;

	private DateTime lastAction;

	private Texture2D avatarTexture;

	private SocialPlayerState playerState;

	private int playerLevel;

	private int suitID;

	public string PlayerName
	{
		get
		{
			return playerName;
		}
		set
		{
			playerName = value;
		}
	}

	public DateTime LastAction
	{
		get
		{
			return lastAction;
		}
		set
		{
			lastAction = value;
		}
	}

	public Texture2D AvatarTexture
	{
		get
		{
			return avatarTexture;
		}
		set
		{
			avatarTexture = value;
		}
	}

	public SocialPlayerState PlayerState
	{
		get
		{
			return playerState;
		}
		set
		{
			playerState = value;
		}
	}

	public int PlayerLevel
	{
		get
		{
			return playerLevel;
		}
		set
		{
			playerLevel = value;
		}
	}

	public int SuitID
	{
		get
		{
			return suitID;
		}
		set
		{
			suitID = value;
		}
	}
}
