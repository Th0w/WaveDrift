using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBehavior : MonoBehaviour
{

	public enum players { P1, P2 };
	public players player;

	public float speed;
	public float maxSpeed;
	public float speedLerp;
	public float leftStickDeadZone;
	public float maxRotation;
	public float rotationLerp;
	public float maxDriftRotation;
	public float driftRotationLerp;
	public float minSpeedTurn;
	public float minSpeedDrift;
	public ParticleSystem driftParticlesLeft;
	public ParticleSystem driftParticlesRight;
	public float minRotationForDrift;

	public Image driftGauge;
	public Color driftMinUsableColor;
	private Color driftGaugeColor;
	public float minUsableDrift;
	public float maxUsableDrift;
	public float driftRegenRate;
	public float driftUseRate;
	bool driftUsable;
	float currentDriftLevel;
	public bool driftAtZero;
	public GameObject projectile;

	private string playerPrefix;

	private Rigidbody rgbd;

	public float actualSpeed;
	public float actualRotation;
	public bool canDrift;
	public bool isDrifting;

	// Use this for initialization
	void Start()
	{
		playerPrefix = player.ToString() + "_";
		rgbd = GetComponent<Rigidbody>();
		currentDriftLevel = maxUsableDrift;
		driftGaugeColor = driftGauge.color;
	}

	// Update is called once per frame
	void Update()
	{
		float inputSpeed = Input.GetAxis(playerPrefix + "accelerate");
		float inputTurn = Input.GetAxis(playerPrefix + "turn") < -leftStickDeadZone || Input.GetAxis(playerPrefix + "turn") > leftStickDeadZone ? Input.GetAxis(playerPrefix + "turn") : 0;
		bool driftInput = Input.GetButton(playerPrefix + "drift");
		bool jumpInput = Input.GetButtonDown(playerPrefix + "jump");
		bool fireInput = Input.GetButtonDown(playerPrefix + "fire");

		bool driftRelease = true;
		if (Input.GetButton(playerPrefix + "drift"))
			driftRelease = false;
		if (Input.GetButtonUp(playerPrefix + "drift"))
			driftRelease = true;
		Debug.Log(driftRelease);
		actualSpeed = inputSpeed * speed;
		rgbd.velocity += transform.forward * actualSpeed;
		rgbd.velocity *= 0.95f;
		if (rgbd.velocity.magnitude > maxSpeed)
		{
			rgbd.velocity = Vector3.Normalize(rgbd.velocity) * maxSpeed;
		}
		/*
			//DriftGaugeSystem
		if (driftRelease && currentDriftLevel < minUsableDrift)
			driftUsable = false;
		if (driftInput && driftUsable && rgbd.velocity.magnitude > minSpeedDrift)
		{
			if (currentDriftLevel > 1)
				currentDriftLevel -= Time.deltaTime * driftUseRate;
			else
				driftUsable = false;
		}
		else
		{
			if (currentDriftLevel < maxUsableDrift)
				currentDriftLevel += Time.deltaTime * driftRegenRate;
			if (currentDriftLevel >= minUsableDrift)
				driftUsable = true;
		}
		if (!driftInput && currentDriftLevel >= minUsableDrift)
			driftUsable = false;

			//GaugeColor
		if (!driftUsable && currentDriftLevel < minUsableDrift)
			driftGauge.color = driftMinUsableColor;
		else
			driftGauge.color = driftGaugeColor;*/
		bool speedAndRotation = rgbd.velocity.magnitude >= minSpeedDrift && (actualRotation <= -minRotationForDrift || actualRotation >= minRotationForDrift);
		canDrift = (currentDriftLevel >= minUsableDrift || !driftAtZero) && speedAndRotation;
		//isDrifting;
		if (driftInput && canDrift)
			isDrifting = true;
		else 
			isDrifting = false;

		if (isDrifting)
		{
			if (currentDriftLevel > 1)
				currentDriftLevel -= Time.deltaTime * driftUseRate;
			SetDriftParticles(128);
		}
		else
		{
			if (currentDriftLevel < maxUsableDrift)
				currentDriftLevel += Time.deltaTime * driftRegenRate;
			SetDriftParticles(0);
		}
		if (currentDriftLevel <= 0)
		{
			driftAtZero = true;
		}
		else if (currentDriftLevel >= minUsableDrift)
			driftAtZero = false;
		if (currentDriftLevel < minUsableDrift)
			driftGauge.color = driftMinUsableColor;
		else
			driftGauge.color = driftGaugeColor;

		driftGauge.fillAmount = currentDriftLevel / maxUsableDrift;

		//DriftFX
		/*bool enoughRotationForDrift = (actualRotation < -minRotationForDrift || actualRotation > minRotationForDrift);
		if (driftUsable && enoughRotationForDrift)
			SetActiveGameObjects(driftParticles, true);
		else
			SetActiveGameObjects(driftParticles, false);

		bool canDrift = driftInput && rgbd.velocity.magnitude > minSpeedDrift && driftUsable && enoughRotationForDrift;*/
		if (rgbd.velocity.magnitude > minSpeedTurn)
		{
			float maxTotalRotation = isDrifting ? maxDriftRotation : maxRotation;
			actualRotation = Mathf.Lerp(actualRotation, inputTurn * maxTotalRotation, Time.deltaTime * (driftInput ? driftRotationLerp : rotationLerp));
			transform.localEulerAngles += new Vector3(transform.localEulerAngles.x, actualRotation, transform.localEulerAngles.z) * Time.deltaTime;
		}

		if (fireInput && rgbd.velocity.magnitude <= minSpeedTurn)
		{
			for (int i = 0; i < 2; i++)
			{
				GameObject emit = Instantiate(projectile, transform.position, Quaternion.identity);
				CircleWaveImpulse cwi = emit.GetComponent<CircleWaveImpulse>();
				cwi.killOnEnd = true;
				cwi.baseTranform = transform;
				cwi.startDelay = i * 0.1f;
			}
		}
	}

	void SetDriftParticles(float intensity)
	{
		driftParticlesLeft.emissionRate = driftParticlesRight.emissionRate = 0;
		if (actualRotation > 0)
			driftParticlesLeft.emissionRate = intensity;
		else
			driftParticlesRight	.emissionRate = intensity;

	}
}
