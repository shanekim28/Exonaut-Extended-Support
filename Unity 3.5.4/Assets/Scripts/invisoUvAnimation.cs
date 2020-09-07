using UnityEngine;

public class invisoUvAnimation : MonoBehaviour
{
	public GameObject uInviso;

	public float iSpeed;

	private Material mMaterial;

	private float mTime;

	private void Start()
	{
		mMaterial = uInviso.GetComponent<Renderer>().material;
		mTime = 0f;
	}

	private void Update()
	{
		mTime += Time.deltaTime * iSpeed;
		mMaterial.SetFloat("_Offset", Mathf.Repeat(mTime, 1f));
	}
}
