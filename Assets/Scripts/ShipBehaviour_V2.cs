using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using System;
using Rewired;
using UniRx.Triggers;

public class ShipBehaviour_V2 : MonoBehaviour
{

    // PLAYER
    public enum Players { Player1, Player2, Player3, Player4 }
    [Header("PLAYER")]
    [Space(10)]
    public Players player;
    public int playerID;
    public Vector3 spawnPos;

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
    public bool airProtection;

    //DRIFT
    [Header("DRIFT")]
    [Space(10)]
    public float driftTime;
    public float maxDriftTime;
    public float driftRecoveryFactor = 1;
    public float driftLossRatio = 1;
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
    public bool invulnerability;
    public float spawnTimeProtection;
    [Space(6)]
    public Transform barrier;
    private Renderer barrierRenderer;

    [Header("PowerUps")]
    [Space(10)]
    public Color powerUpWaveEmitColor;
    public bool shield;

    // PRIVATE
    private Rigidbody selfRB;
    public Vector2 textureOffsetFactor;
    private Animator selfAnimator;
    [HideInInspector]
    private UI_DeathOL deathOL;
    [HideInInspector]
    public Coroutine cor;

    private IDisposable deathDisposable, invulDisposable;

    private Player thePlayer;

    public bool IsFrozen { get; set; }

    void Start()
    {

        playerID = (int)player;
        //if (player == Players.Player1)
        //	playerID = 0;
        //else if (player == Players.Player2)
        //	playerID = 1;
        //else if (player == Players.Player3)
        //	playerID = 2;
        //else
        //	playerID = 3;

        thePlayer = ReInput.players.GetPlayer(playerID);

        selfRB = GetComponent<Rigidbody>();
        selfAnimator = GetComponent<Animator>();

        driftTime = maxDriftTime;

        if (barrier)
            barrierRenderer = barrier.GetComponent<Renderer>();

        deathOL = FindObjectOfType<UI_DeathOL>();
        IsFrozen = false;

        this.OnParticleTriggerAsObservable()
            //.Where(_ => IsFrozen)
            .Subscribe(_ => Debug.Log("SHOULD UNFROZE!"))
            .AddTo(this);
    }

    private void OnParticleCollision(GameObject go)
    {
        Debug.LogFormat("ON PARTICLE COLLISION! ON {0} FROM {1}.", name, go);

    }

    void Update()
    {
        if (IsFrozen == true)
        {
            if (thePlayer.GetButtonDown("Button A"))
            {
                GameManager.Instance.Unfreeze(playerID);
                Debug.Log("UNFREEZING!");
            }
            return;
        }
        // DEV CHEATS
        if (Input.GetKeyDown(KeyCode.T))
            Death();

        // Out of bounds!
        if (transform.position.magnitude > 177 && !death)
            Death();

        // DEATH LOCK!!
        if (death)
            return;

        // Inputs
        //rightTriggerInput = Input.GetAxis (playerPrefix + "RightTrigger");
        //leftStickHorizontalInput = Input.GetAxis (playerPrefix + "LeftStick_Horizontal");
        rightTriggerInput = thePlayer.GetAxis("Right Trigger");
        leftStickHorizontalInput = thePlayer.GetAxis("Left Stick X");

        // Drift input
        float targetRotStrength = rotStrength;
        //if (Input.GetButton (playerPrefix + "Button_X")) 
        if (thePlayer.GetButton("Button X"))
        {

            targetRotStrength += additionnalDriftRotStrength;
            drift = true;
        }
        else
            drift = false;

        // Speed & rot lerps
        actualSpeed = Mathf.Lerp(actualSpeed, speed * rightTriggerInput, speedLerprate * Time.deltaTime);
        actualRotStrength = Mathf.Lerp(actualRotStrength, leftStickHorizontalInput * targetRotStrength, rotLerpRate * Time.deltaTime);

        // Motion
        if (!jump)
        {
            selfRB.velocity += transform.forward * actualSpeed;
            selfRB.velocity *= 0.9f;
            transform.localEulerAngles += new Vector3(0, actualRotStrength, 0);
        }
        else
            transform.localEulerAngles += new Vector3(0, actualRotStrength * 2, 0);

        // Jump
        //if (Input.GetButtonDown (playerPrefix + "Button_A") && !jump) 
        if (thePlayer.GetButtonDown("Button A") && !jump)
        {

            selfAnimator.Play("Anim_Ship_Jump", 0, 0);
            jump = true;
            airProtection = true;
        }

        // Fire PS
        foreach (ParticleSystem ps in firePS)
            ps.emissionRate = actualSpeed / speed * 128f;

        // Drifts!
        if (drift && Mathf.Abs(actualRotStrength) > minRotToDrift && driftTime > 0 && !cooldown && !jump)
        {
            driftTime = Mathf.Clamp(driftTime - Time.deltaTime * driftLossRatio, 0, maxDriftTime);

            if (driftTime == 0)
                cooldown = true;

            if (actualRotStrength > 0)
            { // Rot left

                driftPS[0].emissionRate = 256;
                driftPS[1].emissionRate = 0;

            }
            else
            { // Rot right

                driftPS[0].emissionRate = 0;
                driftPS[1].emissionRate = 256;
            }
        }
        else
        {

            driftTime = Mathf.Clamp(driftTime + Time.deltaTime * driftRecoveryFactor, 0, maxDriftTime);

            if (cooldown && driftTime == maxDriftTime)
                cooldown = false;

            foreach (ParticleSystem ps in driftPS)
                ps.emissionRate = 0;
        }

        // Drift gauge
        driftGauge.fillAmount = driftTime / maxDriftTime;
        if (cooldown)
            driftGauge.color = new Color((Mathf.Sin(Time.timeSinceLevelLoad * 12) + 1) / 2, 0, 0, 1);
        else
            driftGauge.color = Color.white;

        // Barrier
        barrier.transform.position = new Vector3(0, transform.position.y, 0);
        barrier.transform.rotation = Quaternion.FromToRotation(Vector3.forward, transform.position);
        barrierRenderer.material.SetTextureOffset("_MainTex", new Vector2(barrier.transform.localEulerAngles.y * textureOffsetFactor.x, transform.position.y * textureOffsetFactor.y));
        barrierRenderer.material.SetFloat("_GlobalAlpha", Mathf.InverseLerp(145f, 175f, transform.position.magnitude));
    }

    /*public void KillPlayer() {
		
		SlowMo.selfAnimator.Play ("Anim_SlowMo", 0, 0);

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
		invulnerability = true;

		deathOL.RenderDeathOL ();

		deathDisposable = Observable.Timer (TimeSpan.FromSeconds (deathDelay))
			.Subscribe (_ => {
				ship.gameObject.SetActive (true);
				deathGroup.SetActive (false);

				driftTime = maxDriftTime;

				transform.position = spawnPos;
				transform.rotation = Quaternion.identity;

				death = false;

				invulDisposable = Observable.Timer(TimeSpan.FromSeconds(spawnTimeProtection))
					.Subscribe(
						__ => {if (jump) {invulnerability = false;}},
						() => {
							invulDisposable.Dispose();
							invulDisposable = null;
						})
					.AddTo(this);
				
		},
				() => {
					deathDisposable.Dispose();
					deathDisposable = null;
				}).AddTo (this);
	}*/

    public void Death()
    {
        if (!shield)
        {
            if (cor != null)
                StopCoroutine(cor);
            cor = StartCoroutine(CoDeath());
        }
    }

    public IEnumerator CoDeath()
    {
        string id = player.ToString();
        id = id.Substring(id.Length - 1);
        Vector3 deathPos = transform.position;

        SlowMo.selfAnimator.Play("Anim_SlowMo", 0, 0);

        ship.gameObject.SetActive(false);
        deathGroup.SetActive(true);

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
        invulnerability = true;
        shield = false;

        deathOL.RenderDeathOL();

        MessagingCenter.Instance.FireMessage("PlayerDeath",
            new object[] { int.Parse(id), deathPos });

        yield return new WaitForSeconds(deathDelay);

        ship.gameObject.SetActive(true);
        deathGroup.SetActive(false);

        driftTime = maxDriftTime;

        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;

        death = false;

        yield return new WaitForSeconds(spawnTimeProtection);

        invulnerability = false;
    }

    public void Respawn()
    {

        ship.gameObject.SetActive(true);
        deathGroup.SetActive(false);

        driftTime = maxDriftTime;

        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;

        death = false;
    }

    public void Land()
    {

        jump = false;
    }

    public void RemoveAirProtection()
    {

        airProtection = false;
        invulnerability = false;
    }
}