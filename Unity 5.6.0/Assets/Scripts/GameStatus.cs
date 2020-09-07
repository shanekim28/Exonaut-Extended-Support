using UnityEngine;

public class GameStatus
{
	public static void showGameSatusMessage(Rect boxRect, string message)
	{
		GUI.Box(boxRect, "Game Starts In");
		Rect position = new Rect(boxRect.x + 20f, boxRect.y + 20f, 150f, 30f);
		GUI.Box(position, message);
	}

	public static void showGameType(Rect boxRect, string data)
	{
		GUI.Box(boxRect, string.Empty);
		GUI.Label(new Rect(20f, boxRect.y + 15f, 100f, 25f), GameData.getBattleTypeDisplayName());
		GUI.Label(new Rect(20f, boxRect.y + 30f, 100f, 25f), GameData.WorldName);
		Rect position = new Rect(boxRect.x + 5f, boxRect.y + 60f, boxRect.width - 10f, boxRect.height - 80f);
		GUI.Box(position, data);
	}

	public static void showControls(Rect boxRect)
	{
		GUI.Box(boxRect, "Game Controls");
	}

	public static void showGameTips(Rect boxRect)
	{
		GUI.Box(boxRect, "Game Tips");
	}

	public static void showButtons(Rect boxRect)
	{
		GUI.Box(boxRect, string.Empty);
	}
}
