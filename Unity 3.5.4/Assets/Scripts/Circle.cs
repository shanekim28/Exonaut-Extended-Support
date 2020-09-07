using UnityEngine;

public class Circle
{
	private Vector3 Center;

	private float Radius;

	public Circle(Vector3 center, float radius)
	{
		Center = center;
		Radius = radius;
	}

	public Circle(float centerx, float centery, float centerz, float radius)
	{
		Center = new Vector3(centerx, centery, centerz);
		Radius = radius;
	}

	public bool Contains(Vector3 point)
	{
		return (Center - point).sqrMagnitude <= Radius * Radius;
	}
}
