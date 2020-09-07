using UnityEngine;

internal class TabDev
{
	private Rect statusGroup;

	private WorldChooserHome worldChooser = new WorldChooserHome();

	private SuitChooserHome suitChooser = new SuitChooserHome();

	public TabDev()
	{
		statusGroup = new Rect(300f, 0f, 600f, 250f);
	}

	public void showTab(Rect tabGroup)
	{
		GUI.BeginGroup(new Rect(50f, 50f, 800f, 800f));
		if (GameData.IsChooserActive)
		{
			suitChooser.devChooseSuit(new Vector2(5f, 0f));
			GUI.BeginGroup(statusGroup);
			GUI.Box(new Rect(5f, 0f, statusGroup.width - 10f, statusGroup.height), string.Empty);
			worldChooser.devChooseWorld(new Vector2(20f, 20f));
			GUI.EndGroup();
		}
		GUI.EndGroup();
	}
}
