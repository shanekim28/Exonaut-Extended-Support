using UnityEngine;

public class MissionStatus
{
	private static float padWidth = 5f;

	private static float padHeight = 5f;

	private static float masterWidth = 300f;

	private static float masterHeight = 485f;

	public static void showStatus(Vector2 position)
	{
		Rect position2 = new Rect(position.x, position.y, masterWidth, masterHeight);
		GUI.BeginGroup(position2);
		GUI.Box(new Rect(padWidth, padHeight, masterWidth - padWidth, masterHeight - padHeight), "==> Mission Status <==");
		GUI.EndGroup();
	}
}
