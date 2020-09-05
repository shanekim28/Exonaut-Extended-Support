using System;
using UnityEngine;

public class AnimateShield : MonoBehaviour
{
	private void Start()
	{
	}
	private void Update()
	{
		base.GetComponent<Renderer>().material.SetFloat("_Offset", (float)(Mathf.RoundToInt(Time.realtimeSinceStartup * 1000f) % 1000) / 1000f);
	}
}
