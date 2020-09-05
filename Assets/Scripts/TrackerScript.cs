using System.Collections;
using System.Xml;
using UnityEngine;

public class TrackerScript : MonoBehaviour
{
	public enum TrackEvent
	{
		ERROR_DISCONNECTED_FROM_SERVER,
		ERROR_KICKED_FOR_INACTIVITY,
		ERROR_OTHER,
		TUTORIAL_STARTED,
		TUTORIAL_COMPLETED,
		POPUP_JETPACK,
		POPUP_SHOOT,
		GAME_LAUNCHED_BATTLE,
		GAME_LAUNCHED_TEAM_BATTLE,
		SUMMARY_SCREEN_BATTLE,
		SUMMARY_SCREEN_TEAM_BATTLE,
		GAME_COMPLETED_TIME,
		GAME_COMPLETED_CAPTURES,
		LOBBY_TIME_LIMIT_REACHED
	}

	public enum Metric
	{
		WANT_TO_PLAY,
		GUEST_OR_LOGIN,
		IN_HANGAR,
		REQUEST_BATTLE,
		REQUEST_TEAM_BATTLE,
		IN_LOBBY,
		MATCH_READY,
		NEW_MATCH_START,
		DROP_INTO_MATCH,
		MATCH_LEFT_QUIT,
		MATCH_LEFT_ERROR,
		MATCH_COMPLETED
	}

	private string m_SessionID;

	private string m_ID;

	private int m_WantToPlay;

	private int m_GuestOrLogin;

	private int m_InHangar;

	private int m_RequestBattle;

	private int m_RequestTeamBattle;

	private int m_InLobby;

	private int m_MatchReady;

	private int m_NewMatchStart;

	private int m_DropIntoMatch;

	private int m_MatchLeftQuit;

	private int m_MatchLeftError;

	private int m_MatchCompleted;

	private void Start()
	{
		Debug.Log(" ********* Starting TrackerScript ********");
	}

	private void Update()
	{
	}

	public void AddMetric(Metric metric)
	{
		switch (metric)
		{
		case Metric.WANT_TO_PLAY:
			m_WantToPlay++;
			break;
		case Metric.GUEST_OR_LOGIN:
			m_GuestOrLogin++;
			break;
		case Metric.IN_HANGAR:
			m_InHangar++;
			break;
		case Metric.REQUEST_BATTLE:
			m_RequestBattle++;
			break;
		case Metric.REQUEST_TEAM_BATTLE:
			m_RequestTeamBattle++;
			break;
		case Metric.IN_LOBBY:
			m_InLobby++;
			break;
		case Metric.MATCH_READY:
			m_MatchReady++;
			break;
		case Metric.NEW_MATCH_START:
			m_NewMatchStart++;
			break;
		case Metric.DROP_INTO_MATCH:
			m_DropIntoMatch++;
			break;
		case Metric.MATCH_LEFT_QUIT:
			m_MatchLeftQuit++;
			break;
		case Metric.MATCH_LEFT_ERROR:
			m_MatchLeftError++;
			break;
		case Metric.MATCH_COMPLETED:
			m_MatchCompleted++;
			break;
		}
	}

	public void updateMetricStats()
	{
		postMetric();
	}

	private void postMetric()
	{
		string text = GameData.SERVICE_PATH + "/ExonautMetric";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("SessionID", GameData.MyExonautToken);
		wWWForm.AddField("ID", GameData.MyExonautId);
		wWWForm.AddField("WantToPlay", m_WantToPlay);
		wWWForm.AddField("GuestOrLogin", m_GuestOrLogin);
		wWWForm.AddField("InHangar", m_InHangar);
		wWWForm.AddField("RequestBattle", m_RequestBattle);
		wWWForm.AddField("RequestTeamBattle", m_RequestTeamBattle);
		wWWForm.AddField("InLobby", m_InLobby);
		wWWForm.AddField("MatchReady", m_MatchReady);
		wWWForm.AddField("NewMatchStart", m_NewMatchStart);
		wWWForm.AddField("DropIntoMatch", m_DropIntoMatch);
		wWWForm.AddField("MatchLeftQuit", m_MatchLeftQuit);
		wWWForm.AddField("MatchLeftError", m_MatchLeftError);
		wWWForm.AddField("MatchCompleted", m_MatchCompleted);
		wWWForm.AddField("sessionTime", Time.realtimeSinceStartup.ToString());
		WWW www = new WWW(text, wWWForm);
		Debug.Log("Tracking Metric @ " + text);
		StartCoroutine(waitForMetricPosted(www));
	}

	private IEnumerator waitForMetricPosted(WWW www)
	{
		yield return www;
		if (www.error == null)
		{
			XmlDocument xmlDoc = new XmlDocument();
			Debug.Log("www.text: " + www.text);
			if (www.text != null)
			{
				yield break;
			}
			xmlDoc.LoadXml(www.text);
			if (xmlDoc == null)
			{
				Logger.trace("## Metrics File Failed ");
				yield break;
			}
			XmlNodeList sessionIDs = xmlDoc.GetElementsByTagName("sessionID");
			XmlNodeList status = xmlDoc.GetElementsByTagName("status");
			if (status != null && status.Item(0) != null)
			{
				Logger.trace("<< status: " + status.Item(0).InnerText);
				if (status.Item(0).InnerText.Contains("new"))
				{
					GameData.MyExonautToken = sessionIDs.Item(0).InnerText;
					Logger.trace("<< setting new token: " + GameData.MyExonautToken);
				}
			}
		}
		else
		{
			Logger.traceError("www error:" + www.error);
		}
	}
}
