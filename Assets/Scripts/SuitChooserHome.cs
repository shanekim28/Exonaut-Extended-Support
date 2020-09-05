using System.Collections;
using UnityEngine;

public class SuitChooserHome
{
	private static float padWidth = 5f;

	private static float padHeight = 5f;

	private static float masterWidth = 300f;

	private static float masterHeight = 485f;

	private static float chooserHeight = 350f;

	private static float suitDetailHeight = 140f;

	private int factId;

	private string[] factions = new string[2]
	{
		"Banzai",
		"Atlas"
	};

	private int suitId;

	private string[] suitsAtlas = new string[20];

	private string[] suitsBanzai = new string[20];

	private Hashtable atlasLookup = new Hashtable();

	private Hashtable banzaiLookup = new Hashtable();

	private GUISkin exoSkin;

	public SuitChooserHome()
	{
		showAvailableSuits();
		exoSkin = (GUISkin)Resources.Load("Skin/exoGuiSkin");
	}

	private void showAvailableSuits()
	{
		Hashtable masterSuitList = GameData.MasterSuitList;
		IDictionaryEnumerator enumerator = masterSuitList.GetEnumerator();
		int num = 0;
		int num2 = 0;
		while (enumerator.MoveNext())
		{
			Exosuit exosuit = (Exosuit)enumerator.Value;
			if (exosuit.mSuitId == GameData.MySuitID)
			{
				factId = exosuit.mFactionId - 1;
				if (exosuit.mFactionId == 1)
				{
					suitId = num2;
				}
				else
				{
					suitId = num;
				}
			}
			if (exosuit.mFactionId == 1)
			{
				suitsBanzai[num2] = exosuit.mSuitName;
				banzaiLookup.Add(num2, exosuit.mSuitId);
				num2++;
			}
			else
			{
				suitsAtlas[num] = exosuit.mSuitName;
				atlasLookup.Add(num, exosuit.mSuitId);
				num++;
			}
		}
	}

	public int devChooseSuit(Vector2 position)
	{
		Rect position2 = new Rect(position.x, position.y, masterWidth, masterHeight);
		GUI.BeginGroup(position2);
		GUI.Box(new Rect(0f, 0f, position2.width, position2.height), string.Empty);
		GUI.BeginGroup(new Rect(0f, 0f, position2.width, chooserHeight + padHeight));
		GUI.Box(new Rect(padWidth, padHeight, position2.width - padWidth * 2f, chooserHeight), string.Empty);
		factId = GUI.SelectionGrid(new Rect(10f, 10f, 280f, 25f), factId, factions, 2, exoSkin.button);
		string text = string.Empty;
		GameData.MyFactionId = 1 + factId;
		switch (GameData.MyFactionId)
		{
		case 2:
			suitId = GUI.SelectionGrid(new Rect(10f, 45f, 280f, 300f), suitId, suitsAtlas, 2);
			text = suitsAtlas[suitId];
			GameData.MySuitID = (int)atlasLookup[suitId];
			Logger.trace("My Suit Id = " + GameData.MySuitID);
			break;
		case 1:
			suitId = GUI.SelectionGrid(new Rect(10f, 45f, 280f, 300f), suitId, suitsBanzai, 2);
			text = suitsBanzai[suitId];
			GameData.MySuitID = (int)banzaiLookup[suitId];
			Logger.trace("My Suit Id = " + GameData.MySuitID);
			break;
		}
		Logger.trace("CHOOSER MYSUITID=" + GameData.MySuitID);
		GUI.EndGroup();
		Logger.trace("FactID = " + GameData.MyFactionId + "  SuitID = " + GameData.MySuitID);
		GUI.BeginGroup(new Rect(0f, chooserHeight, position2.width, suitDetailHeight));
		GUI.Box(new Rect(padWidth, padHeight, position2.width - padWidth * 2f, suitDetailHeight - padHeight), "Suit Info");
		GUI.TextField(new Rect(40f, 40f, 200f, 25f), text);
		GUI.EndGroup();
		GUI.EndGroup();
		return GameData.MySuitID;
	}
}
