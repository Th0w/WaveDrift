using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipBehaviour_V2 : MonoBehaviour {

	// PLAYER
	public enum Players {Player1, Player2}
	[Header("PLAYER")]
	[Space(10)]
	public Players player;
	public string playerPrefix;

	// INPUTS
	[Header("INPUTS")]
	[Space(10)]
	[Range(0f, 1f)]
	public float leftStickHorizontalInput;
	[Range(0f, 1f)]
	public float rightTriggerInput;

	// SHIP
	[Header("SHIP")]
	[Space(10)]
	public Transform ship;
	[Space(6)]
	public float speed;
	public float speedLerprate;
	public float actualSpeed;
	[Space(6)]
	public float rotStrength;
	public float additionnalDriftRotStrength;
	public float rotLerpRate;
	public float actualRotStrength;
	[Space(6)]
	public float minRotToDrift;
	public bool drift;
	public bool cooldown;
	[Space(6)]
	public bool jump;
	public bool invulnerability;

	//DRIFT
	[Header("DRIFT")]
	[Space(10)]
	public float driftTime;
	public float maxDriftTime;
	public float driftRecoveryFactor = 1;
	[Space(6)]
	public Image driftGauge;

	// PARTICLE SYSTEMS
	[Header("PARTICLE SYSTEMS")]
	[Space(10)]
	public ParticleSystem[] firePS;
	public ParticleSystem[] driftPS;
	[Space(6)]
	public GameObject deathGroup;

	// MISC
	[Header("PARTICLE SYSTEMS")]
	[Space(10)]
	public bool death;
	public float deathDelay;
	[Space(6)]
	public Transform barrier;
	private Renderer barrierRenderer;

	// PRIVATE
	private Rigidbody selfRB;
	public Vector2 textureOffsetFactor;
	private Animator selfAnimator;

	void Start () {

		if (player == Players.Player1)
			playerPrefix = "P1_";
		else
			playerPrefix = "P2_";

		selfRB = GetComponent<Rigidbody> ();
		selfAnimator = GetComponent<Animator> ();

		driftTime = maxDriftTime;

		if (barrier)
			barrierRenderer = barrier.GetComponent<Renderer> ();
	}

	void Update () {

		// DEV CHEATS
		if (Input.GetKeyDown (KeyCode.T))
			StartCoroutine(Death (deathDelay));

		// Out of bounds!
		if (transform.position.magnitude > 176 && !death)
			StartCoroutine(Death (deathDelay));

		// DEATH LOCK!!
		if (death)
			return;
		
		// Inputs
		rightTriggerInput = Input.GetAxis (playerPrefix + "RightTrigger");
		leftStickHorizontalInput = Input.GetAxis (playerPrefix + "LeftStick_Horizontal");

		// Drift input
		float targetRotStrength = rotStrength;
		if (Input.GetButton (playerPrefix + "Button_X")) {
			
			targetRotStrength += additionnalDriftRotStrength;
			drift = true;
		} else
			drift = false;

		// Speed & rot lerps
		actualSpeed = Mathf.Lerp (actualSpeed, speed * rightTriggerInput, speedLerprate * Time.deltaTime);
		actualRotStrength = Mathf.Lerp (actualRotStrength, leftStickHorizontalInput * targetRotStrength, rotLerpRate * Time.deltaTime);

		// Motion
		if (!jump) {
			selfRB.velocity += transform.forward * actualSpeed;
			selfRB.velocity *= 0.9f;
			transform.localEulerAngles += new Vector3 (0, actualRotStrength, 0);
		} else
			transform.localEulerAngles += new Vector3 (0, actualRotStrength * 2, 0);

		// Jump
		if (Input.GetButtonDown (playerPrefix + "Button_A") && !jump) {
			
			selfAnimator.Play ("Anim_Ship_Jump", 0, 0);
			jump = true;
			invulnerability = true;
		}

		// Fire PS
		foreach (ParticleSystem ps in firePS)
			ps.emissionRate = actualSpeed / speed * 128f;

		// Drifts!
		if (drift && Mathf.Abs(actualRotStrength) > minRotToDrift && driftTime > 0 && !cooldown && !jump) {

			driftTime = Mathf.Clamp(driftTime - Time.deltaTime, 0, maxDriftTime);

			if (driftTime == 0)
				cooldown = true;

			if (actualRotStrength > 0) { // Rot left
				
				driftPS [0].emissionRate = 256;
				driftPS [1].emissionRate = 0;

			} else { // Rot right

				driftPS [0].emissionRate = 0;
				driftPS [1].emissionRate = 256;
			}
		} else {

			driftTime = Mathf.Clamp(driftTime + Time.deltaTime * driftRecoveryFactor, 0, maxDriftTime);

			if (cooldown && driftTime == maxDriftTime)
				cooldown = false;

			foreach (ParticleSystem ps in driftPS)
				ps.emissionRate = 0;
		}

		// Drift gauge
		driftGauge.fillAmount = driftTime / maxDriftTime;
		if (cooldown)
			driftGauge.color = new Color ((Mathf.Sin (Time.timeSinceLevelLoad * 12) + 1) / 2, 0, 0, 1);
		else
			driftGauge.color = Color.white;

		// Barrier
		barrier.transform.position = new Vector3(0, transform.position.y, 0);
		barrier.transform.rotation = Quaternion.FromToRotation (Vector3.forward, transform.position);
		barrierRenderer.material.SetTextureOffset ("_MainTex", new Vector2(barrier.transform.localEulerAngles.y * textureOffsetFactor.x, transform.position.y * textureOffsetFactor.y));
		barrierRenderer.material.SetFloat("_GlobalAlpha", Mathf.InverseLerp(145f, 175f, transform.position.magnitude));
	}

	public IEnumerator Death (float deathDelay) {


		ship.gameObject.SetActive (false);
		deathGroup.SetActive (true);

		foreach (ParticleSystem ps in firePS)
			ps.emissionRate = 0;
		foreach (ParticleSystem ps in driftPS)
			ps.emissionRate = 0;

		selfRB.velocity = Vector3.zero;

		driftGauge.fillAmount = 0;

		actualSpeed = 0;
		actualRotStrength = 0;

		death = true;
		jump = false;

		yield return new WaitForSeconds (deathDelay);

		ship.gameObject.SetActive (true);
		deathGroup.SetActive (false);

		driftTime = maxDriftTime;

		transform.position = Vector3.zero;

		death = false;
	}

	public void Land () {

		jump = false;
	}

	public void Vulnerable () {

		invulnerability = false;
	}
}
