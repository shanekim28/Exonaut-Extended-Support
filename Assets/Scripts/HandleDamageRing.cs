using UnityEngine;

public class HandleDamageRing : MonoBehaviour
{
	public float framerate = 30f;

	public int numFrames = 10;

	public bool loopAnim;

	public bool isAnimating;

	private float currentTimer;

	private float offset;

	private float interval;

	private float mTimeToPlay;

	private string mCallback;

	private SkinnedMeshRenderer armorSMR;

	private void Awake()
	{
		offset = 0f;
		isAnimating = false;
		Transform transform = base.transform.Find("armor");
		armorSMR = (transform.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer);
		isAnimating = false;
	}

	public void StartAnimating(bool isLooping, float fr, int framesNum, float timeToPlay, string callback)
	{
		if (armorSMR == null)
		{
			Transform transform = base.transform.Find("armor");
			armorSMR = (transform.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer);
		}
		loopAnim = isLooping;
		framerate = fr;
		currentTimer = 1f / framerate;
		offset = 0f;
		numFrames = framesNum;
		interval = 1f / (float)numFrames;
		isAnimating = true;
		mCallback = callback;
		mTimeToPlay = timeToPlay;
	}

	public void StopAnimating()
	{
		if (isAnimating)
		{
			offset = 0f;
			isAnimating = false;
			Material material = armorSMR.GetComponent<Renderer>().material;
			Vector2 mainTextureOffset = armorSMR.GetComponent<Renderer>().material.mainTextureOffset;
			material.mainTextureOffset = new Vector2(0f, mainTextureOffset.y);
			armorSMR.GetComponent<Renderer>().materials[0].mainTextureScale = new Vector2(0f, 0f);
			Color color = new Color(0f, 0f, 0f, 0f);
			armorSMR.GetComponent<Renderer>().materials[0].SetColor("_TintColor", color);
		}
	}

	public void SetupRing(Color color, float thickness)
	{
		if (armorSMR == null)
		{
			Transform transform = base.transform.Find("armor");
			armorSMR = (transform.GetComponent("SkinnedMeshRenderer") as SkinnedMeshRenderer);
		}
		armorSMR.GetComponent<Renderer>().materials[0].mainTextureScale = new Vector2(0f, thickness);
		armorSMR.GetComponent<Renderer>().materials[0].SetColor("_TintColor", color);
	}

	private void Update()
	{
		if (!isAnimating)
		{
			return;
		}
		if (mCallback.Equals("grenades") && loopAnim && mTimeToPlay > 0f)
		{
			mTimeToPlay -= Time.deltaTime;
			if (mTimeToPlay <= 0f)
			{
				mTimeToPlay = 0f;
				StopAnimating();
			}
		}
		currentTimer -= Time.deltaTime;
		if (!(currentTimer <= 0f))
		{
			return;
		}
		currentTimer = 1f / framerate;
		offset += interval;
		Material material = armorSMR.GetComponent<Renderer>().material;
		float x = offset;
		Vector2 mainTextureOffset = armorSMR.GetComponent<Renderer>().material.mainTextureOffset;
		material.mainTextureOffset = new Vector2(x, mainTextureOffset.y);
		if (offset > interval * (float)numFrames - interval)
		{
			Material material2 = armorSMR.GetComponent<Renderer>().material;
			Vector2 mainTextureOffset2 = armorSMR.GetComponent<Renderer>().material.mainTextureOffset;
			material2.mainTextureOffset = new Vector2(0f, mainTextureOffset2.y);
			offset = 0f;
			if (!loopAnim)
			{
				StopAnimating();
			}
		}
	}
}
