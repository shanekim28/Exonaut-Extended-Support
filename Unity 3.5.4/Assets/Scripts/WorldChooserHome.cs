using UnityEngine;

public class WorldChooserHome
{
	private static float chooserWidth = 300f;

	private static float chooserHeight = 150f;

	private int worldId = 1;

	private string[] levels = new string[6]
	{
		"Snow Lab",
		"Castle",
		"Storm Harbor",
		"Tree House",
		"Perplex",
		"Bling Bling"
	};

	public int devChooseWorld(Vector2 position)
	{
		Rect position2 = new Rect(position.x, position.y, chooserWidth, chooserHeight);
		GUI.BeginGroup(position2);
		worldId = GUI.SelectionGrid(new Rect(10f, 30f, 280f, 80f), worldId, levels, 2);
		GameData.WorldID = worldId + 1;
		Logger.trace("<< chosing world " + GameData.WorldID);
		GUI.EndGroup();
		return worldId;
	}
}
