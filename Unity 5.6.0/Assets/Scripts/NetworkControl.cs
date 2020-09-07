public class NetworkControl : Control
{
	private Player me;

	public NetworkControl(Player m)
	{
		me = m;
		controlType = 1;
	}

	public override void Update()
	{
		if (me.myState.desiredState != 32768)
		{
		}
	}
}
