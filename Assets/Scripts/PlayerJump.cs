using System;

public class PlayerJump
{
	public bool enabled = true;

	public float height = 1f;

	public float extraHeight = 4.1f;

	[NonSerialized]
	public float repeatTime = 0.05f;

	[NonSerialized]
	public float timeout = 0.15f;

	[NonSerialized]
	public bool jumping;

	[NonSerialized]
	public bool reachedApex;

	[NonSerialized]
	public float lastButtonTime = -10f;

	[NonSerialized]
	public float lastTime = -1f;

	[NonSerialized]
	public float lastStartHeight;
}
