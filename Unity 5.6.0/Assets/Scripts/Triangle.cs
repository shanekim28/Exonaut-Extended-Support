using System;
using UnityEngine;

[Serializable]
public class Triangle
{
	private Vector3[] point = new Vector3[3];

	public Vector3[] Point {
		get {
			return point;
		}
	}

	public Triangle(Vector3[] _points)
	{
		for (int i = 0; i < point.Length && i < _points.Length; i++)
		{
			point[i] = _points[i];
		}
	}

	public Triangle(Vector3 a, Vector3 b, Vector3 c)
	{
		point[0] = a;
		point[1] = b;
		point[2] = c;
	}

	private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
	{
		Vector3 lhs = Vector3.Cross(b - a, p1 - a);
		Vector3 rhs = Vector3.Cross(b - a, p2 - a);
		if (Vector3.Dot(lhs, rhs) >= 0f)
		{
			return true;
		}
		return false;
	}

	public void Scale(float x, float y, float z)
	{
		for (int i = 0; i < point.Length; i++)
		{
			point[i].x *= x;
			point[i].y *= y;
			point[i].z *= z;
		}
	}

	public bool Contains(Vector3 testPoint)
	{
		if (SameSide(testPoint, point[0], point[1], point[2]) && SameSide(testPoint, point[1], point[0], point[2]) && SameSide(testPoint, point[2], point[0], point[1]))
		{
			return true;
		}
		return false;
	}
}
