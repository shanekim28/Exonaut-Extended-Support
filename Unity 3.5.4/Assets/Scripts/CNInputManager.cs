using UnityEngine;

public class CNInputManager
{
	public const float DOUBLE_TAP_TIME = 0.07f;

	public const int JUST_RELEASED = 0;

	public const int IS_RELEASED = 1;

	public const int JUST_PRESSED = 2;

	public const int IS_PRESSED = 4;

	public const int DOUBLE_TAPPED = 8;

	public int btnJustPressed;

	public int btnIsPressed;

	public int btnDoubleTapped;

	public int btnJustReleased;

	public int commandIdx;

	public int ignoreIdx;

	public Command[] commandList;

	public IgnoreRegion[] ignoreList;

	public CNInputManager()
	{
		commandList = new Command[20];
		ignoreList = new IgnoreRegion[20];
	}

	public void Update()
	{
		btnIsPressed = 0;
		btnJustPressed = 0;
		btnDoubleTapped = 0;
		btnJustReleased = 0;
		for (int i = 0; i < commandIdx; i++)
		{
			if (Input.GetKeyDown(commandList[i].btn))
			{
				commandList[i].btnState = 2;
				btnJustPressed |= commandList[i].action;
				if (commandList[i].timeSinceRelease < 0.07f)
				{
					commandList[i].btnState |= 8;
					btnDoubleTapped |= commandList[i].action;
				}
			}
			else if (Input.GetKey(commandList[i].btn))
			{
				commandList[i].btnState = 4;
				commandList[i].timeSincePress += Time.deltaTime;
				commandList[i].timeSinceRelease = 0f;
				btnIsPressed |= commandList[i].action;
			}
			else if (Input.GetKeyUp(commandList[i].btn))
			{
				commandList[i].btnState = 0;
				commandList[i].timeSincePress = 0f;
				btnJustReleased |= commandList[i].action;
			}
			else
			{
				commandList[i].btnState = 1;
				commandList[i].timeSinceRelease += Time.deltaTime;
			}
		}
	}

	public void addCommand(KeyCode btn, int action)
	{
		commandList[commandIdx] = new Command(btn, action);
		commandIdx++;
	}

	public int addIgnore(Rect toIgnore, bool isActive)
	{
		int i;
		for (i = 0; i < 10 && !ignoreList[i].isActive; i++)
		{
		}
		ignoreList[i] = new IgnoreRegion(toIgnore, onOff: true);
		return i;
	}

	public bool buttonPressed(int wantTo)
	{
		return ((btnIsPressed | btnJustPressed) & wantTo) == wantTo;
	}

	public bool buttonJustPressed(int wantTo)
	{
		return (btnJustPressed & wantTo) == wantTo;
	}

	public bool buttonDoubleTapped(int wantTo)
	{
		return (btnDoubleTapped & wantTo) == wantTo;
	}

	public bool buttonJustReleased(int wantTo)
	{
		return (btnJustReleased & wantTo) == wantTo;
	}
}
