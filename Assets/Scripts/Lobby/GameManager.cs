using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UniRx;

[Serializable]
public class PlayerData
{
    public ShipBehaviour_V2 behaviour;
    public GameObject infoPanel;
    public GameObject nitroGauge;
    public GameObject cursor3d;
    public GameObject barrier;
    public UI_TextShadow[] shadows;

    public void SetActive(bool val)
    {
        behaviour.IsFrozen = !val;
        infoPanel.SetActive(val);
        nitroGauge.SetActive(val);
        cursor3d.SetActive(val);
        barrier.SetActive(val);
        shadows.ForEach(shadow =>
        {
            shadow.gameObject.SetActive(false);
            shadow.enabled = true;
            shadow.gameObject.SetActive(true);
        });
    }
}

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    private Enemy_LaserTurret[] laserz;
    private Enemy_Bumper[] bumperz;

    [SerializeField]
    private PlayerData[] playerz;

    [SerializeField]
    private SpawnManager spawnManager;

    public bool CanSpawnBonus { get; private set; }

	// Use this for initialization
	private IEnumerator Start () {

        yield return new WaitForSeconds(0.25f);

        laserz = FindObjectsOfType<Enemy_LaserTurret>();
        bumperz = FindObjectsOfType<Enemy_Bumper>();

        laserz.ForEach(laser => laser.gameObject.SetActive(false));
        bumperz.ForEach(bumper => bumper.gameObject.SetActive(false));

        spawnManager = FindObjectOfType<SpawnManager>();
        spawnManager.Init();

        if (playerz.Length != 4) { Debug.LogError("Missing some players..."); }

        playerz = playerz.OrderBy(player => player.behaviour.playerID).ToArray();
        playerz.ForEach(player => player.SetActive(false));
    }

    private void BeginSpawn()
    {
        CanSpawnBonus = true;
        spawnManager.BeginSpawn();
    }

    internal void EndLobby()
    {
        playerz.Where(player => player.behaviour.IsFrozen)
            .ForEach(player =>
            {
                player.SetActive(false);
                player.behaviour.gameObject.SetActive(false);
                player.behaviour.invulnerability = true;
            });

        laserz.ForEach(laser => laser.gameObject.SetActive(true));
        bumperz.ForEach(bumper => bumper.gameObject.SetActive(true));


        Observable.Timer(TimeSpan.FromSeconds(1.0))
            .Subscribe(_ => spawnManager.BeginSpawn())
            .AddTo(this);
    }

    internal void Unfreeze(int playerID)
    {
        playerz[playerID].SetActive(true);
    }
}
