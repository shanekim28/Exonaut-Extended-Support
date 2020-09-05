using UnityEngine;

public class Command
{
	public KeyCode btn;

	public int action;

	public int btnState;

	public float timeSincePress;

	public float timeSinceRelease;

	public Command(KeyCode b, int a)
	{
		btn = b;
		action = a;
		btnState = 0;
		timeSincePress = 0f;
		timeSinceRelease = 0f;
	}
}
