using System;

public class PlayerAttributes
{
	public float speedRunMax;

	public float speedRunAdj;

	public float speedJetMax;

	public float fuelAmountMax;

	[NonSerialized]
	public float fuelAmountCurrent;

	[NonSerialized]
	public float speedCurrent;

	public float fuelRegenDelayMax;

	[NonSerialized]
	public float fuelRegenDelayCurrent;

	public float fuelRegenSpeed;

	public float healthMax;

	[NonSerialized]
	public float healthCurrent;

	public float healthRegenDelay = 4f;

	[NonSerialized]
	public float healthRegenDelayCurrent;

	public float healthRegenSpeed;

	[NonSerialized]
	public float captureTimeCurrent;

	public float invincibleTimeMax;

	public float invincibleByFreeTimeMax;

	[NonSerialized]
	public float invincibleTimeCurrent = 2f;

	public float rollDistanceMax;

	public float speedRollMax;

	public float rollRecoveryMax = 1.5f;

	[NonSerialized]
	public float rollRecoveryCurrent;

	public float airdashDistanceMax = 20f;

	[NonSerialized]
	public float airdashDistanceCurrent;

	public float airdashSpeed = 12f;

	public float airdashRecoveryMax;

	[NonSerialized]
	public float airdashRecoveryCurrent;

	public float jumpVelocity;

	public float invisibleTimeMax;

	public float invisibleTimeCurrent;

	public float pickupTimerMax = 5f;

	[NonSerialized]
	public float pickupTimerCurrent;

	[NonSerialized]
	public float pickupTimerAdjust;

	public PlayerAttributes()
	{
		speedRunMax = 5f;
		speedCurrent = 0f;
		invisibleTimeMax = 10f;
		invisibleTimeCurrent = 0f;
	}
}
