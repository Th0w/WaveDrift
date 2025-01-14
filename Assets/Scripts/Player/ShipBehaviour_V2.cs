﻿using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject driftMegaMegaGauge;
    public Color powerUpWaveEmitColor;
    public bool shield;
    [HideInInspector]
    public Material baseMat;
    public Material shieldMat;

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
    private GameManager gameManager;
    private MessagingCenter messagingCenter;

    private PlayerInput input;
    private ControllerMap kbCtrlDefault;
    private ControllerMap kbCtrlMenu;
    private ControllerMap jsCtrlDefault;
    private ControllerMap jsCtrlMenu;

    public bool IsFrozen { get; set; }
    public bool HasLayoutSwitched { get; private set; }

    private void Awake() {
        messagingCenter = FindObjectOfType<MessagingCenter>();
        gameManager = FindObjectOfType<GameManager>();
        selfRB = GetComponent<Rigidbody>();
        selfAnimator = GetComponent<Animator>();
        deathOL = FindObjectOfType<UI_DeathOL>();
    }

    void Start() {
        playerID = (int)player;

        thePlayer = ReInput.players.GetPlayer(playerID);


        driftTime = maxDriftTime;

        if (barrier)
            barrierRenderer = barrier.GetComponent<Renderer>();

        baseMat = ship.GetComponent<MeshRenderer>().material;

        IsFrozen = false;

        input = GetComponent<PlayerInput>();
        if (input != null) {
            input.Init(playerID);

            input.SelectPressed
                .Where(b => b)
                .Subscribe(b => gameManager.Quit())
                .AddTo(this);

            input.StartPressed
                .Where(b => b)
                .Subscribe(b => gameManager.Reset())
                .AddTo(this);

            // Prepared for ui opening and interaction.
            input.StartPressed
                .Where(b => b == false)
                .Subscribe(b => gameManager.OpenMenu(playerID))
                .AddTo(this);

            // Prepared for ui opening and interaction.
            //input.SelectPressed
            //    .Where(b => b == false)
            //    .Subscribe()
            //    .AddTo(this);
        }
        var audioManager = FindObjectOfType<AudioManager>();
        thePlayer.AddInputEventDelegate(
            iaed => audioManager.ChangeVolume(audioManager.Volume == 100.0f ? 0.0f : 100.0f),
            UpdateLoopType.Update,
            InputActionEventType.ButtonJustPressed,
            "MuteMusic");

        var kbMaps = thePlayer.controllers.maps.GetAllMaps(ControllerType.Keyboard);
        if (kbMaps.Count() == 2) {
            kbMaps.ForEach(map => {
                if (map.enabled) {
                    kbCtrlDefault = map;
                } else {
                    kbCtrlMenu = map;
                }
            });
        }
        var jsMaps = thePlayer.controllers.maps.GetAllMaps(ControllerType.Joystick);
        if (jsMaps.Count() == 2) {
            jsMaps.ForEach(map => {
                if (map.enabled) {
                    jsCtrlDefault = map;
                } else {
                    jsCtrlMenu = map;
                }
            });
        }
    }

    public void SwitchLayout(bool gameLayout) {
        if (kbCtrlDefault != null) {

            thePlayer.controllers.maps.SetMapsEnabled(
                !gameLayout,
                ControllerType.Keyboard,
                kbCtrlMenu.categoryId,
                kbCtrlMenu.layoutId);

            thePlayer.controllers.maps.SetMapsEnabled(
                gameLayout,
                ControllerType.Keyboard,
                kbCtrlDefault.categoryId,
                kbCtrlDefault.layoutId);
        }
        if (jsCtrlDefault != null) {

            thePlayer.controllers.maps.SetMapsEnabled(
                !gameLayout,
                ControllerType.Joystick,
                jsCtrlMenu.categoryId,
                jsCtrlMenu.layoutId);

            thePlayer.controllers.maps.SetMapsEnabled(
                gameLayout,
                ControllerType.Joystick,
                jsCtrlDefault.categoryId,
                jsCtrlDefault.layoutId);
        }
    }

    void Update() {
        if (gameManager.IsPaused == true) { return; }

        if (IsFrozen == true) {
            if (thePlayer.GetButtonDown("Button A")) {
                gameManager.Unfreeze(playerID);
            }
            return;
        }
        // DEV CHEATS
        if (Input.GetKeyDown(KeyCode.T))
            Death();

        // Out of bounds!
        if (transform.position.magnitude > 177 && !death) {
            shield = false;
            Death();
        }
        // DEATH LOCK!!
        if (death)
            return;

        // Inputs
        rightTriggerInput = thePlayer.GetAxis("Right Trigger");
        leftStickHorizontalInput = thePlayer.GetAxis("Left Stick X");

        // Drift input
        float targetRotStrength = rotStrength;
        if (thePlayer.GetButton("Button X")) {

            targetRotStrength += additionnalDriftRotStrength;
            drift = true;
        } else {
            drift = false;
        }

        // Speed & rot lerps
        actualSpeed = Mathf.Lerp(actualSpeed, speed * rightTriggerInput, speedLerprate * Time.deltaTime);
        actualRotStrength = Mathf.Lerp(actualRotStrength, leftStickHorizontalInput * targetRotStrength, rotLerpRate * Time.deltaTime);

        // Motion
        if (!jump) {
            selfRB.velocity += transform.forward * actualSpeed;
            selfRB.velocity *= 0.9f;
            transform.localEulerAngles += new Vector3(0, actualRotStrength, 0);
        } else {
            transform.localEulerAngles += new Vector3(0, actualRotStrength * 2, 0);
        }

        // Jump
        if (thePlayer.GetButtonDown("Button A") && !jump) {

            selfAnimator.Play("Anim_Ship_Jump", 0, 0);
            jump = true;
            airProtection = true;
        }

        // Fire PS
        foreach (ParticleSystem ps in firePS) {
            ps.emissionRate = actualSpeed / speed * 128f;
        }

        // Drifts!
        if (drift && Mathf.Abs(actualRotStrength) > minRotToDrift && driftTime > 0 && !cooldown && !jump) {
            driftTime = Mathf.Clamp(driftTime - Time.deltaTime * driftLossRatio, 0, maxDriftTime);

            if (driftTime == 0) {
                cooldown = true;
            }

            if (actualRotStrength > 0) { // Rot left

                driftPS[0].emissionRate = 256;
                driftPS[1].emissionRate = 0;

            } else { // Rot right

                driftPS[0].emissionRate = 0;
                driftPS[1].emissionRate = 256;
            }
        } else {

            driftTime = Mathf.Clamp(driftTime + Time.deltaTime * driftRecoveryFactor, 0, maxDriftTime);

            if (cooldown && driftTime == maxDriftTime) {
                cooldown = false;
            }

            foreach (ParticleSystem ps in driftPS) {
                ps.emissionRate = 0;
            }
        }

        // Drift gauge
        driftGauge.fillAmount = driftTime / maxDriftTime;

        if (cooldown) {
            driftGauge.color = new Color((Mathf.Sin(Time.timeSinceLevelLoad * 12) + 1) / 2, 0, 0, 1);
        } else {
            driftGauge.color = Color.white;
        }

        // Barrier
        barrier.transform.position = new Vector3(0, transform.position.y, 0);
        barrier.transform.rotation = Quaternion.FromToRotation(Vector3.forward, transform.position);
        barrierRenderer.material.SetTextureOffset("_MainTex", new Vector2(barrier.transform.localEulerAngles.y * textureOffsetFactor.x, transform.position.y * textureOffsetFactor.y));
        barrierRenderer.material.SetFloat("_GlobalAlpha", Mathf.InverseLerp(145f, 175f, transform.position.magnitude));
    }

    public void Death() {
        if (!shield) {
            if (cor != null) {
                StopCoroutine(cor);
            }
            cor = StartCoroutine(CoDeath());
        }
    }

    public IEnumerator CoDeath() {
        string id = player.ToString();
        id = id.Substring(id.Length - 1);
        Vector3 deathPos = transform.position;

        SlowMo.selfAnimator.Play("Anim_SlowMo", 0, 0);

        ship.gameObject.SetActive(false);
        ship.GetComponent<MeshRenderer>().material = baseMat;
        deathGroup.SetActive(true);

        foreach (ParticleSystem ps in firePS) {
            ps.emissionRate = 0;
        }
        foreach (ParticleSystem ps in driftPS) {
            ps.emissionRate = 0;
        }

        selfRB.velocity = Vector3.zero;

        driftGauge.fillAmount = 0;

        actualSpeed = 0;
        actualRotStrength = 0;

        death = true;
        jump = false;
        invulnerability = true;
        shield = false;

        deathOL.RenderDeathOL();

        // Only for lobby.
        if (gameManager.IsInGame == false) {
            gameManager.PlayersData[playerID].SetActive(false);
            invulnerability = false;

            ship.gameObject.SetActive(true);
            deathGroup.SetActive(false);

            driftTime = maxDriftTime;

            death = false;

            transform.position = spawnPos;
            transform.rotation = Quaternion.identity;
            yield break;
        }

        messagingCenter.FireMessage("PlayerDeath", new object[] { int.Parse(id), deathPos });

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

    public void Respawn() {

        ship.gameObject.SetActive(true);
        deathGroup.SetActive(false);

        driftTime = maxDriftTime;

        transform.position = spawnPos;
        transform.rotation = Quaternion.identity;

        death = false;
    }

    public void Land() {
        jump = false;
    }

    public void RemoveAirProtection() {
        airProtection = false;
        invulnerability = false;
    }
}