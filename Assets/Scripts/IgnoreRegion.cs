using UnityEngine;

public class IgnoreRegion
{
	public Rect ignore;

	public bool isActive;

	public IgnoreRegion(Rect region, bool onOff)
	{
		ignore = region;
		isActive = onOff;
	}
}
