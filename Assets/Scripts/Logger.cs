using UnityEngine;

internal static class Logger
{
	public static void trace(string message)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(message);
		}
	}

	public static void traceError(string message)
	{
		Debug.LogError(message);
	}

	public static void traceWarning(string message)
	{
		if (Debug.isDebugBuild)
		{
			Debug.LogWarning(message);
		}
	}

	public static void traceAlways(string message)
	{
		Debug.Log(message);
	}
}
