using UnityEngine;

public class GameSelector
{
	private GUIStyle mFactionButton;

	public int type;

	private GUISkin mTabSkin;

	private string lastHover = string.Empty;

	public GameSelector(GameHome Parent)
	{
		mTabSkin = GUIUtil.mInstance.mTabSkin;
		if (GameData.MyFactionId == 1)
		{
			mFactionButton = mTabSkin.GetStyle("TopRightBanzai");
		}
		else
		{
			mFactionButton = mTabSkin.GetStyle("TopRightAtlas");
		}
	}

	public void resetType()
	{
		type = 0;
	}

	public int drawButtons(Rect box, GUISkin skin)
	{
		string b = (Event.current.type != EventType.Repaint) ? lastHover : string.Empty;
		GameObject gameObject = GameObject.Find("Tracker");
		TrackerScript trackerScript = gameObject.GetComponent("TrackerScript") as TrackerScript;
		switch (GUIUtil.Button(new Rect(7.5f, box.height / 2f - 29f, 123f, 58f), "PLAY\nBATTLE", mFactionButton))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "PLAY\nBATTLE";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "PLAY\nBATTLE";
			GameData.ConsecutiveGames = 0;
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			type = 1;
			trackerScript.AddMetric(TrackerScript.Metric.REQUEST_BATTLE);
			break;
		}
		GUIUtil.GUIEnable(GameData.MyPlayStatus > 1 || GameData.MATCH_MODE == GameData.Build.DEBUG);
		switch (GUIUtil.Button(new Rect(box.width / 2f + 2.5f - 10f, box.height / 2f - 29f, 143f, 58f), "PLAY\nTEAM BATTLE", mFactionButton))
		{
		case GUIUtil.GUIState.Hover:
		case GUIUtil.GUIState.Active:
			if (Event.current.type == EventType.Repaint)
			{
				b = "PLAY\nTEAM BATTLE";
				if (lastHover != b)
				{
					GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Over);
				}
			}
			break;
		case GUIUtil.GUIState.Click:
			b = "PLAY\nTEAM BATTLE";
			GameData.ConsecutiveGames = 0;
			GUIUtil.PlayGUISound(GUIUtil.GUISoundClips.TT_Global_Button_Press);
			type = 2;
			trackerScript.AddMetric(TrackerScript.Metric.REQUEST_TEAM_BATTLE);
			break;
		}
		GUIUtil.GUIEnable(bEnable: true);
		lastHover = b;
		return type;
	}
}
