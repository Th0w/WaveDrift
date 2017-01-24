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

		GameObject.FindObjectOfType<CameraBehaviour> ().GetPlayers ();
    }
}

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }
    

    [SerializeField]
    private PlayerData[] playersData;
    public PlayerData[] PlayersData { get { return playersData; } }

    private ShipBehaviour_V2[] players;
    private ShipBehaviour_V2[] activePlayers;
    
    [SerializeField]
    private SpawnManager spawnManager;

    private Subject<Unit> onGameBegin, onGameEnd;

    public ShipBehaviour_V2[] ActivePlayers { get { return activePlayers; } }
    public bool IsInGame { get; private set; }
    public IObservable<Unit> OnGameBegin { get { return onGameBegin; } }
    public IObservable<Unit> OnGameEnd { get { return onGameEnd; } }

    // Use this for initialization
    private void Awake() {
        onGameBegin = new Subject<Unit>();
        onGameEnd = new Subject<Unit>();
    }
    private IEnumerator Start () {
        players = FindObjectsOfType<ShipBehaviour_V2>();

        yield return new WaitForSeconds(0.25f);

        spawnManager = FindObjectOfType<SpawnManager>();
        spawnManager.Init();

        if (playersData.Length != 4) { Debug.LogError("Missing some players..."); }

        playersData = playersData.OrderBy(player => player.behaviour.playerID).ToArray();
        playersData.ForEach(player => player.SetActive(false));
    }

    internal void EndLobby()
    {
        activePlayers = players
            .Where(player => player.IsFrozen == false)
            .ToArray();
        onGameBegin.OnNext(Unit.Default);

        IsInGame = true;

        playersData.Where(player => player.behaviour.IsFrozen)
            .ForEach(player =>
            {
                player.SetActive(false);
                player.behaviour.gameObject.SetActive(false);
                player.behaviour.invulnerability = true;
            });


        Observable.Timer(TimeSpan.FromSeconds(1.0))
            .Subscribe(_ => spawnManager.BeginSpawn())
            .AddTo(this);
    }

    internal void Unfreeze(int playerID)
    {
        playersData[playerID].SetActive(true);
    }

    internal void Reset()
    {
        onGameEnd.OnNext(Unit.Default);
    }
}
