using System.Collections;
using UnityEngine;

public class CookieReader : MonoBehaviour
{
	public static Hashtable cookieValues;

	public static bool loaded;

	public static bool isLoggedIn;

	public static bool isLoggedInReturn;

	public static bool isAuthorized;

	public static bool isAuthorizedReturn;

	public static string myName = string.Empty;

	public bool simulateCookies;

	public string simulatedTEGid = "aa4b766-1410328673-1302547580233-1";

	public string simulatedAuthID = "1234";

	public string simulatedDisplayName = "Faked User";

	private void Start()
	{
	}

	private void Awake()
	{
		myName = base.name;
		cookieValues = new Hashtable();
		ReadCookiesFromBrowser();
	}

	private void ReadCookiesFromBrowser()
	{
		Debug.Log("<< ReadingCookiesFromBrowser");
		if (simulateCookies && Application.isEditor)
		{
			cookieValues = new Hashtable();
			cookieValues["TEGid"] = simulatedTEGid;
			cookieValues["authid"] = simulatedAuthID;
			cookieValues["dname"] = simulatedDisplayName;
			loaded = true;
			isLoggedIn = true;
			isLoggedInReturn = true;
		}
		else
		{
			Application.ExternalCall("Exonaut_GetCookies", myName, "GetCookiesCallback");
		}
	}

	public void GetCookiesCallback(string cookie_string)
	{
		Debug.Log("<< getcookies callback " + cookie_string);
		cookieValues = new Hashtable();
		string[] array = cookie_string.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('=');
			if (array3.Length == 2)
			{
				cookieValues[array3[0].Trim()] = array3[1];
			}
		}
		loaded = true;
	}

	public static string GetCookieValue(string sName)
	{
		return (string)cookieValues[sName];
	}

	public static void CheckMSIBLoggedIn()
	{
		isLoggedIn = false;
		isLoggedInReturn = false;
		Debug.Log("<< check msib logged in " + myName);
		Application.ExternalCall("Exonaut_CheckMSIBLoggedIn", myName, "IsLoggedInCallback");
	}

	public void IsLoggedInCallback(string ok)
	{
		if (ok.Equals("true"))
		{
			isLoggedIn = true;
		}
		else
		{
			isLoggedIn = false;
		}
		isLoggedInReturn = true;
	}

	public static void CheckMSIBAuthorized()
	{
		isAuthorized = false;
		isAuthorizedReturn = false;
		Debug.Log("<< check msib is Authorized " + myName);
		Application.ExternalCall("Exonaut_CheckMSIBAuthorized", myName, "IsAuthorizedCallback");
	}

	public void IsAuthorizedCallback(string ok)
	{
		if (ok.Equals("true"))
		{
			isAuthorized = true;
		}
		else
		{
			isAuthorized = false;
		}
		isAuthorizedReturn = true;
	}
}
