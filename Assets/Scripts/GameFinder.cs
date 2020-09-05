using UnityEngine;

public abstract class GameFinder
{
	public abstract void startSearch(string matchNodeIP, int battleType);

	public abstract void stopSearch();

	public abstract bool getSearchStarted();

	public abstract void joinGame(string matchNodeIP, string gMgrId, string reserveId);

	public abstract void findReservation(string matchNodeIP, int battleType);

	public abstract int drawStatus(Rect box);

	public abstract string Update();

	public abstract void destroyConnection();
}
