using UnityEngine;

public class NetworkTransformSender : MonoBehaviour
{
	private float sendingPeriod = 0.125f;

	private float timeLastSending;

	private bool sendMode;

	private NetworkTransform lastState;

	private void Start()
	{
		lastState = new NetworkTransform(base.gameObject);
	}

	public void StartSending()
	{
		sendMode = true;
	}

	private void FixedUpdate()
	{
		if (sendMode)
		{
			SendTransform();
		}
	}

	public void SendTransform()
	{
		if (lastState != null)
		{
			if (lastState.UpdateIfDifferent() && timeLastSending >= sendingPeriod)
			{
				lastState.DoSend();
				timeLastSending = 0f;
			}
			else
			{
				timeLastSending += Time.deltaTime;
			}
		}
	}

	public void ForceSendTransform()
	{
		lastState.DoSend();
	}

	public void SendEvt()
	{
		lastState.DoSendEvt();
	}
}
