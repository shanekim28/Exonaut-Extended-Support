using UnityEngine;

public class keyCommandsAnimScript : MonoBehaviour
{
	public float rate = 30f;

	private float currentTimer;

	private float offset;

	public int[] AnimRows;

	public float RowHeight = 0.125f;

	private int CurrentRow;

	private void Start()
	{
		currentTimer = 1f / rate;
	}

	private void Update()
	{
		currentTimer -= Time.deltaTime;
		if (!(currentTimer <= 0f))
		{
			return;
		}
		currentTimer = 1f / rate;
		offset += 0.125f;
		if (offset > 0.875f)
		{
			offset = 0f;
			CurrentRow++;
			if (CurrentRow >= AnimRows.Length)
			{
				CurrentRow = 0;
			}
		}
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, (float)AnimRows[CurrentRow] * RowHeight);
	}
}
