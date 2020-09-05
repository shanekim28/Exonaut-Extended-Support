using UnityEngine;

public class HUDFPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private float fps;

	private void Start()
	{
		if (!base.GetComponent<GUIText>())
		{
			Logger.traceError("UtilityFramesPerSecond needs a GUIText component!");
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			fps = accum / (float)frames;
			string text = string.Format("{0:D2}", fps) + "FPS";
			base.GetComponent<GUIText>().text = text;
			if (fps < 30f)
			{
				base.GetComponent<GUIText>().material.color = Color.yellow;
			}
			else if (fps < 10f)
			{
				base.GetComponent<GUIText>().material.color = Color.red;
			}
			else
			{
				base.GetComponent<GUIText>().material.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}

	private void OnGUI()
	{
	}
}
