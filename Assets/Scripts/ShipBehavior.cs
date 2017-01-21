using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehavior : MonoBehaviour
{

	public enum players { P1, P2 };
	public players player;

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
	}

	// Update is called once per frame
	void Update()
	{
		float inputSpeed = Input.GetAxis(playerPrefix + "accelerate");
		float inputTurn = Input.GetAxis(playerPrefix + "turn") < -leftStickDeadZone || Input.GetAxis(playerPrefix + "turn") > leftStickDeadZone ? Input.GetAxis(playerPrefix + "turn") : 0;
		bool driftInput = Input.GetButton(playerPrefix + "drift");
		bool jumpInput = Input.GetButtonDown(playerPrefix + "jump");

		actualSpeed = Mathf.Lerp(actualSpeed, inputSpeed * maxSpeed, Time.deltaTime * speedLerp);
		rgbd.velocity = transform.forward * actualSpeed;

		bool canDrift = driftInput && actualSpeed > minSpeedDrift;
		if (actualSpeed > minSpeedTurn)
		{
			float maxTotalRotation = canDrift ? maxDriftRotation : maxRotation;
			actualRotation = Mathf.Lerp(actualRotation, inputTurn * maxTotalRotation, Time.deltaTime * (driftInput ? driftRotationLerp : rotationLerp));
			transform.localEulerAngles += new Vector3(transform.localEulerAngles.x, actualRotation, transform.localEulerAngles.z) * Time.deltaTime;
		}

		if (Input.GetButtonDown(playerPrefix + "drift") && canDrift)
			SetActiveGameObjects(driftParticles, true);
		else if (Input.GetButtonUp(playerPrefix + "drift") && !canDrift)
			SetActiveGameObjects(driftParticles, false);

	}

	void SetActiveGameObjects(GameObject[] go, bool active)
	{
		for (int i = 0; i < go.Length; i++)
			go[i].SetActive(active);
	}
}
