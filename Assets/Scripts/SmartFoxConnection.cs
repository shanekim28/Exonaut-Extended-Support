using Sfs2X;
using UnityEngine;

public class SmartFoxConnection : MonoBehaviour
{
	private static SmartFoxConnection mInstance;

	private static SmartFox smartFox;

	public static SmartFox Connection
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = (new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection);
			}
			return smartFox;
		}
		set
		{
			if (mInstance == null)
			{
				mInstance = (new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection);
			}
			smartFox = value;
		}
	}

	public static bool IsInitialized {
		get {
			return smartFox != null;
		}
	}

	private void OnApplicationQuit()
	{
		if (smartFox.IsConnected)
		{
			smartFox.Disconnect();
		}
	}
}
