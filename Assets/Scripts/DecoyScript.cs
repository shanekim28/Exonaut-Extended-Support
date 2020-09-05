using UnityEngine;

public class DecoyScript : MonoBehaviour
{
	private float mDecoyTime;

	private float faceDir;

	private float faceTargetDir;

	private float faceTargetSpeed;

	private void Start()
	{
		mDecoyTime = 10f;
		faceDir = (faceTargetDir = 90f);
		faceTargetSpeed = 1800f;
	}

	private void Update()
	{
		mDecoyTime -= Time.deltaTime;
		if (mDecoyTime <= 0f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (faceDir != faceTargetDir)
		{
			if (faceTargetDir == 90f)
			{
				faceDir -= faceTargetSpeed * Time.deltaTime;
				if (faceDir < 90f)
				{
					faceDir = 90f;
				}
			}
			else
			{
				faceDir += faceTargetSpeed * Time.deltaTime;
				if (faceDir > 270f)
				{
					faceDir = 270f;
				}
			}
		}
		base.transform.rotation = Quaternion.Euler(0f, faceDir, 0f);
	}
}
