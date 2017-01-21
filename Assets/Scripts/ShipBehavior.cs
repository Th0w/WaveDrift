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
	public GameObject[] driftParticles;

	public Image driftGauge;
	public Color driftMinUsableColor;
	private Color driftGaugeColor;
	public float minUsableDrift;
	public float maxUsableDrift;
	public float driftRegenRate;
	public float driftUseRate;
	public bool driftUsable;
	public float currentDriftLevel;

	private string playerPrefix;

	private Rigidbody rgbd;

	public float actualSpeed;
	public float actualRotation;

	// Use this for initialization
	void Start()
	{
		playerPrefix = player.ToString() + "_";
		rgbd = GetComponent<Rigidbody>();
		SetActiveGameObjects(driftParticles, false);
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
		bool driftRelease = Input.GetButtonUp(playerPrefix + "drift");
		Debug.Log(driftRelease);

		actualSpeed = inputSpeed * speed;
		rgbd.velocity += transform.forward * actualSpeed;
		rgbd.velocity *= 0.95f;
		if (rgbd.velocity.magnitude > maxSpeed)
		{
			rgbd.velocity = Vector3.Normalize(rgbd.velocity) * maxSpeed;
		}

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
			driftGauge.color = driftGaugeColor;
		driftGauge.fillAmount = currentDriftLevel / maxUsableDrift;
		
			//DriftFX
		if (driftUsable)
			SetActiveGameObjects(driftParticles, true);
		else
			SetActiveGameObjects(driftParticles, false);

		bool canDrift = driftInput && rgbd.velocity.magnitude > minSpeedDrift && driftUsable;
		if (rgbd.velocity.magnitude > minSpeedTurn)
		{
			float maxTotalRotation = canDrift ? maxDriftRotation : maxRotation;
			actualRotation = Mathf.Lerp(actualRotation, inputTurn * maxTotalRotation, Time.deltaTime * (driftInput ? driftRotationLerp : rotationLerp));
			transform.localEulerAngles += new Vector3(transform.localEulerAngles.x, actualRotation, transform.localEulerAngles.z) * Time.deltaTime;
		}
	}

	void SetActiveGameObjects(GameObject[] go, bool active)
	{
		for (int i = 0; i < go.Length; i++)
			go[i].SetActive(active);
	}
}
