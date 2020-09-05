using UnityEngine;

public class FadeLineScript : MonoBehaviour
{
	private float fadeTime = 1f;

	private float fadeRate;

	private float currentAlpha;

	private void Start()
	{
		fadeRate = 1f / fadeTime;
		currentAlpha = 1f;
	}

	private void Update()
	{
		fadeTime -= Time.deltaTime;
		currentAlpha -= fadeRate * Time.deltaTime;
		LineRenderer lineRenderer = GetComponent("LineRenderer") as LineRenderer;
		lineRenderer.SetWidth(0.8f, 0.7f);
		lineRenderer.SetColors(new Color(1f, 0.55f, 0f, 0f), new Color(1f, 0f, 0f, currentAlpha));
		if (currentAlpha <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
