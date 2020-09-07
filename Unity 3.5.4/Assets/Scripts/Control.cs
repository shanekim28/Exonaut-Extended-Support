public class Control
{
	public const int LOCAL = 0;

	public const int NETWORK = 1;

	protected string moveLeft;

	protected string moveRight;

	protected string crouch;

	protected string jump;

	protected string throwing;

	protected string jet;

	protected string shoot;

	public int controlType;

	public Control()
	{
		moveLeft = "a";
		moveRight = "d";
		crouch = "s";
		jump = "w";
		throwing = "space";
		shoot = "mouse left";
		jet = "mouse right";
	}

	public virtual void Update()
	{
	}

	public void setControls()
	{
	}
}
