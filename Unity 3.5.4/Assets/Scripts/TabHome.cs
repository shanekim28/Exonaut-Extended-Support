using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TabHome
{
	public Rect newsGroup;

	public Rect missionGroup;

	private Texture2D mGuestFactionTexture;

	private bool bNewsLoaded;

	private Texture2D mNewsTexture;

	private string mNewsData;

	private FormattedLabel NewsLabel;

	public Vector3[] ModelPositions = new Vector3[4]
	{
		new Vector3(-6.75f, -6.25f, 27.5f),
		new Vector3(-6.5f, -6.25f, 28.5f),
		new Vector3(-6.5f, -6.25f, 28.5f),
		new Vector3(0.5f, 0f, 35f)
	};

	public Vector3[] ModelRotations = new Vector3[4]
	{
		new Vector3(0f, 180f, 0f),
		new Vector3(0f, 180f, 0f),
		new Vector3(0f, 180f, 0f),
		new Vector3(90f, 180f, 0f)
	};

	public Transform ModelTransform;

	public TabHome(GameHome Parent)
	{
		ModelTransform = null;
		mGuestFactionTexture = (Resources.Load("Menus/ex_guest_graphic_" + GameData.MyFactionId) as Texture2D);
		GameObject gameObject = GameObject.Find("homeGUI");
		GameHome gameHome = gameObject.GetComponent("GameHome") as GameHome;
		if (GameData.DoesEventExist("event_news") && GameData.eventObjects["event_news"] != null)
		{
			TextAsset textAsset = GameData.eventObjects["event_news"] as TextAsset;
			mNewsData = textAsset.text;
			Debug.Log("show news: " + mNewsData);
			bNewsLoaded = true;
			NewsLabel = null;
		}
		if (!bNewsLoaded)
		{
			gameHome.StartCoroutine(LoadNews());
		}
	}

	public IEnumerator LoadNews()
	{
		bNewsLoaded = false;
		string url2 = string.Empty;
		url2 = ((!Application.isEditor) ? (Application.dataPath + GameData.NEWS_FILE) : ("file://" + Application.dataPath + GameData.NEWS_FILE));
		WWW newsWWW = new WWW(url2);
		yield return newsWWW;
		if (newsWWW.error == null && !newsWWW.text.Contains("404 Not Found"))
		{
			mNewsData = newsWWW.text;
		}
		else
		{
			mNewsData = string.Empty;
		}
		bNewsLoaded = true;
		NewsLabel = null;
	}

	public void UpdateScreenSpace(Rect screenSpace)
	{
		float num = 400f;
		float num2 = 360f;
		float num3 = 65f;
		float num4 = 125f;
		num3 += (screenSpace.height - num3 - num4 - num) / 2f;
		missionGroup = new Rect(screenSpace.width - num2 - 10f, num3, num2, num * 0.6f);
		newsGroup = new Rect(screenSpace.width - num2 - 10f, missionGroup.y + missionGroup.height + 2f, num2, num * 0.4f);
	}

	public void showTab(Rect tabGroup)
	{
		if (NewsLabel == null)
		{
			if (bNewsLoaded)
			{
				NewsLabel = new FormattedLabel(newsGroup.width - 30f, mNewsData);
			}
			else
			{
				NewsLabel = new FormattedLabel(newsGroup.width - 30f, "Loading News");
			}
		}
		if (GameData.getExosuit(GameData.MySuitID) == null)
		{
			Logger.traceError("My Exosuit was Null = " + GameData.MySuitID);
		}
		if (GameData.getExosuit(GameData.MySuitID).getHighPolyModel() != null)
		{
			ModelTransform = GameData.getExosuit(GameData.MySuitID).getHighPolyModel().transform;
		}
		else
		{
			ModelTransform = null;
		}
		if (ModelTransform != null)
		{
			AnimationState animationState = ModelTransform.GetComponent<Animation>()[ModelTransform.GetComponent<Animation>().clip.name];
			if (animationState != null)
			{
				float num = Time.realtimeSinceStartup / animationState.length;
				animationState.time = (num - (float)Mathf.RoundToInt(num)) * animationState.length;
			}
		}
		GUI.color = Color.white;
		GUI.BeginGroup(newsGroup);
		GUI.color = Color.white;
		if (bNewsLoaded)
		{
			float num2 = 5f;
			if (mNewsTexture != null)
			{
				GUI.Box(new Rect(0f, 0f, newsGroup.width, newsGroup.height), GUIContent.none, GUI.skin.GetStyle("BoxTRCutaway"));
				GUI.DrawTexture(new Rect((newsGroup.width - (float)mNewsTexture.width) / 2f, num2, mNewsTexture.width, mNewsTexture.height), mNewsTexture);
				num2 += (float)mNewsTexture.height;
			}
			else
			{
				GUI.Box(new Rect(0f, 0f, newsGroup.width, newsGroup.height), "NEWS", GUI.skin.GetStyle("BoxTRCutaway"));
			}
			if (mNewsData != null && mNewsData.Length > 0)
			{
				GUI.BeginGroup(new Rect(15f, num2 + 10f, newsGroup.width - 30f, newsGroup.height - num2 - 15f));
				NewsLabel.draw();
				GUI.EndGroup();
			}
		}
		else
		{
			GUI.Box(new Rect(0f, 0f, newsGroup.width, newsGroup.height), "LOADING NEWS...", GUI.skin.GetStyle("BoxTRCutaway"));
		}
		GUI.EndGroup();
		GUI.BeginGroup(missionGroup);
		if (GameData.MyPlayStatus == 1)
		{
			GUI.Box(new Rect(0f, 0f, missionGroup.width, missionGroup.height), GUIContent.none, GUI.skin.GetStyle("BoxTRCutaway"));
			//GUI.DrawTexture(new Rect(5f, 10f, missionGroup.width - 10f, missionGroup.height - 15f), mGuestFactionTexture);
		}
		else
		{
			GUI.color = Color.white;
			GUI.Label(new Rect(0f, 0f, missionGroup.width, 35f), "MISSIONS", "MissionHeader");
			GUI.Label(new Rect(0f, 37f, missionGroup.width, 15f), "IN PROGRESS", "MissionLabel");
			List<SFSObject> latestMissionsInProgress = GameData.LatestMissionsInProgress;
			if (latestMissionsInProgress.Count > 0)
			{
				SFSObject sFSObject = latestMissionsInProgress[latestMissionsInProgress.Count - 1];
				GUI.Label(new Rect(0f, 50f, missionGroup.width, 56f), sFSObject.GetUtfString("Name"), GUI.skin.GetStyle("MedalBGNew"));
				GUI.Box(new Rect(5f, 53f, missionGroup.width, 50f), GUIContent.none, GUI.skin.GetStyle("MedalGeneric"));
				GUI.Label(new Rect(0f, 53f, missionGroup.width, 50f), sFSObject.GetUtfString("Description"), GUI.skin.GetStyle("MedalDesc"));
				GUIUtil.DrawProgressBar(new Rect(0f, 108f, missionGroup.width, 15f), sFSObject.GetInt("Count"), 0f, sFSObject.GetInt("Total"), GUIUtil.BarDirection.Right, "MissionProgressBG", "MissionProgressBar");
				GUI.Label(new Rect(0f, 110f, missionGroup.width, 16f), sFSObject.GetInt("Count") + "/" + sFSObject.GetInt("Total"), "MissionProgressText");
			}
			else
			{
				GUI.Label(new Rect(0f, 50f, missionGroup.width, 56f), ":(", GUI.skin.GetStyle("MedalBGNew"));
				GUI.Box(new Rect(5f, 53f, missionGroup.width, 50f), GUIContent.none, GUI.skin.GetStyle("MedalGeneric"));
				GUI.Label(new Rect(0f, 53f, missionGroup.width, 50f), "You have no missions in progress", GUI.skin.GetStyle("MedalDesc"));
			}
			GUI.Label(new Rect(0f, 147f, missionGroup.width, 15f), "RECENTLY COMPLETED", "MissionLabel");
			List<SFSObject> latestCompletedMissions = GameData.LatestCompletedMissions;
			if (latestCompletedMissions.Count > 0)
			{
				SFSObject sFSObject2 = latestCompletedMissions[latestCompletedMissions.Count - 1];
				GUI.Label(new Rect(0f, 160f, missionGroup.width, 56f), sFSObject2.GetUtfString("Name"), GUI.skin.GetStyle("MedalBGNew"));
				string utfString = sFSObject2.GetUtfString("Image");
				if (utfString != null && utfString.Length > 0)
				{
					Texture2D texture2D = Resources.Load("Menus/Medals/" + utfString) as Texture2D;
					if (texture2D == null)
					{
						GUI.Box(new Rect(5f, 163f, missionGroup.width - 8f, 50f), GUIContent.none, GUI.skin.GetStyle("MedalGeneric"));
					}
					else
					{
						GUI.DrawTexture(new Rect(5f, 163f, 50f, 50f), texture2D);
					}
				}
				else
				{
					int @int = sFSObject2.GetInt("Credits");
					int int2 = sFSObject2.GetInt("XP");
					if (int2 > 0)
					{
						if (@int > 0)
						{
							GUI.Box(new Rect(5f, 163f, missionGroup.width - 8f, 50f), GUIContent.none, GUI.skin.GetStyle("XPCreditsMedal"));
						}
						else
						{
							GUI.Box(new Rect(5f, 163f, missionGroup.width - 8f, 50f), GUIContent.none, GUI.skin.GetStyle("MedalNoReward"));
						}
					}
					else if (@int > 0)
					{
						GUI.Box(new Rect(5f, 163f, missionGroup.width - 8f, 50f), GUIContent.none, GUI.skin.GetStyle("CreditsMedal"));
					}
					else
					{
						GUI.Box(new Rect(5f, 183f, missionGroup.width - 8f, 50f), GUIContent.none, GUI.skin.GetStyle("MedalNoReward"));
					}
				}
				GUI.Label(new Rect(0f, 163f, missionGroup.width, 50f), sFSObject2.GetUtfString("Description"), GUI.skin.GetStyle("MedalDesc"));
			}
			else
			{
				GUI.Label(new Rect(0f, 160f, missionGroup.width, 56f), ":(", GUI.skin.GetStyle("MedalBGNew"));
				GUI.Box(new Rect(5f, 163f, missionGroup.width, 50f), GUIContent.none, GUI.skin.GetStyle("MedalGeneric"));
				GUI.Label(new Rect(0f, 163f, missionGroup.width, 50f), "You have no recently completed missions", GUI.skin.GetStyle("MedalDesc"));
			}
		}
		GUI.EndGroup();
	}
}
