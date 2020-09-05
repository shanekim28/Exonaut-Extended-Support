using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class UrlLocator : MonoBehaviour
{
	[NonSerialized]
	public string url = "http://www.cartoonnetwork.com/servicesdirector/find?pid=";

	[NonSerialized]
	public string pid = "cn.exonaut.event";

	private Hashtable props;

	[NonSerialized]
	public string ip = string.Empty;

	[NonSerialized]
	public int port;

	public bool success;

	public bool complete;

	public bool IsComplete()
	{
		return complete;
	}

	public bool IsSuccess()
	{
		return success;
	}

	public bool IsReady()
	{
		return IsComplete() && IsSuccess();
	}

	public string GetIP()
	{
		return ip;
	}

	public int GetPort()
	{
		return port;
	}

	private void Awake()
	{
	}

	private IEnumerator LoadProps()
	{
		Debug.Log("Entering load props");
		props = new Hashtable();
		WWW propsFile = null;
		if (Application.isEditor)
		{
			propsFile = new WWW("file://" + Application.dataPath + "/client.props");
		}
		if (Application.isWebPlayer)
		{
			propsFile = new WWW(Application.dataPath + "/client.props");
		}
		yield return propsFile;
		if (propsFile.error != null)
		{
			Debug.Log("Failed to find properties file!");
		}
		else
		{
			Debug.Log("Loading properties");
			string contents = propsFile.text;
			if (contents != null)
			{
				StringReader reader = new StringReader(contents);
				for (string s = reader.ReadLine(); s != null; s = reader.ReadLine())
				{
					string[] pairs = s.Split(' ');
					props.Add(pairs[0], pairs[1]);
				}
				url = (string)props["url"];
				pid = (string)props["pid"];
			}
		}
		SendMessage("StartUrlLocator");
	}

	private IEnumerator StartUrlLocator()
	{
		int attempt = 0;
		Debug.Log("[UrlLocator::StartUrlLocator] - connect to director");
		WWW www;
		do
		{
			Debug.Log("Connecting to: " + url + pid);
			www = new WWW(url + pid);
			yield return www;
			if (www.error == null)
			{
				break;
			}
			yield return new WaitForSeconds(0.05f);
			attempt++;
			Logger.trace("[UrlLocator::StartUrlLocator] - attempt to connect to director number " + attempt);
		}
		while (www.error != null && 5 > attempt);
		if (www.error == null)
		{
			Hashtable table = (Hashtable)MiniJSON.JsonDecode(www.text);
			int flags = 0;
			Logger.trace("[UrlLocator::StartUrlLocator] - table has " + table.Count + " entries ");
			foreach (DictionaryEntry de in table)
			{
				Debug.Log("key:" + de.Key + ", value: " + de.Value);
				if ("ip" == (string)de.Key)
				{
					ip = (string)de.Value;
					flags |= 1;
				}
				else if ("port" == (string)de.Key)
				{
					double d = (double)de.Value;
					port = (int)d;
					flags |= 2;
				}
			}
			success = (3 == flags);
		}
		else
		{
			Logger.trace("[UrlLocator::Start] There was an error " + www.error.ToString());
		}
		complete = true;
	}

	private void Update()
	{
	}
}
