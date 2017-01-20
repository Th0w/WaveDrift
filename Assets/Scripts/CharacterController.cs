using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

	public enum players { P1, P2};
	public players player;

	public float maxSpeed;
	public float speedLerp;
	public float maxRotation;
	public float rotationLerp;
	public float maxDriftRotation;
	public float driftRotationLerp;
	public GameObject driftParticles;

	private string playerPrefix;

	private Rigidbody rgbd;

	public float actualSpeed;
	public float actualRotation;

	// Use this for initialization
	void Start()
	{
		playerPrefix = player.ToString() + "_";
		rgbd = GetComponent<Rigidbody>();
		driftParticles.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		float inputSpeed = Input.GetAxis(playerPrefix + "accelerate");
		float inputTurn = Input.GetAxis(playerPrefix + "turn");
		bool driftInput = Input.GetButton(playerPrefix + "drift");
		bool jumpInput = Input.GetButtonDown(playerPrefix + "jump");

		float currentSpeed = Mathf.Lerp(0, maxSpeed, inputSpeed);
		actualSpeed = Mathf.Lerp(actualSpeed, inputSpeed * maxSpeed, Time.deltaTime * speedLerp);
		rgbd.velocity = transform.forward * actualSpeed;

		Debug.Log(driftInput);
		float maxTotalRotation = driftInput ? maxDriftRotation : maxRotation;
		float currentRotation = Mathf.Lerp(0, maxTotalRotation, inputTurn);
		actualRotation = Mathf.Lerp(actualRotation, inputTurn * maxTotalRotation, Time.deltaTime * (driftInput ? driftRotationLerp : rotationLerp));
		transform.localEulerAngles += new Vector3(transform.localEulerAngles.x, actualRotation, transform.localEulerAngles.z) * Time.deltaTime;

		if (Input.GetButtonDown(playerPrefix + "drift"))
			driftParticles.SetActive(true);
		else if (Input.GetButtonUp(playerPrefix + "drift"))
			driftParticles.SetActive(false);

	}
}
