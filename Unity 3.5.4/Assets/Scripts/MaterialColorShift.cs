using UnityEngine;

internal class MaterialColorShift : MonoBehaviour
{
	public Color mBaseColor = Color.white;

	public Color mShiftColor = new Color(0f, 0f, 0f, 0f);

	public float mShiftRate = 1f;

	public bool mFading = true;

	private Color Difference {
		get {
			return mShiftColor * Mathf.Sin(Time.realtimeSinceStartup * mShiftRate * 2f);
		}
	}

	private void Start()
	{
	}

	private void Awake()
	{
		base.gameObject.GetComponent<Renderer>().material = new Material(base.gameObject.GetComponent<Renderer>().material);
	}

	private void Update()
	{
		if (mFading)
		{
			Color color = base.gameObject.GetComponent<Renderer>().material.color;
			color.r = Mathf.MoveTowards(color.r, mBaseColor.r, 0.05f);
			color.g = Mathf.MoveTowards(color.g, mBaseColor.g, 0.05f);
			color.b = Mathf.MoveTowards(color.b, mBaseColor.b, 0.05f);
			color.a = Mathf.MoveTowards(color.a, mBaseColor.a, 0.05f);
			base.gameObject.GetComponent<Renderer>().material.color = color;
			if (color == mBaseColor)
			{
				mFading = false;
			}
		}
		else
		{
			base.gameObject.GetComponent<Renderer>().material.color = mBaseColor - Difference;
		}
	}
}
