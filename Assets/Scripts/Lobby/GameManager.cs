using System;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour {
    #region Fields

    #region Serialized

    [SerializeField]
    private PlayerData[] playersData;
    [Header("GameMode data")]
    [SerializeField]
    private int scoreGoal;
    
    #endregion Serialized

    private ShipBehaviour_V2[] players;
    private ShipBehaviour_V2[] activePlayers;
    private Subject<Unit> onGameBegin, onGameEnd;
    private ScoreManager scoreManager;
    private SpawnManager spawnManager;

    #endregion Fields

    #region Properties

    public PlayerData[] PlayersData { get { return playersData; } }
    public ShipBehaviour_V2[] ActivePlayers { get { return activePlayers; } }
    public bool IsInGame { get; private set; }
    public IObservable<Unit> OnGameBegin { get { return onGameBegin; } }
    public IObservable<Unit> OnGameEnd { get { return onGameEnd; } }
    
    #endregion Properties
    
    private void Awake() {
        onGameBegin = new Subject<Unit>();
        onGameEnd = new Subject<Unit>();
    }

    private IEnumerator Start () {
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreGoal > 0) {
            scoreManager.SetScoreGoal(scoreGoal);
            scoreManager.OnGoalScoreReached
                .Subscribe(t => {
                    Debug.LogWarningFormat("FYI: Winning player ID: #{0}, score: {1}.", t.Item1, t.Item2);
                    Observable.Timer(TimeSpan.FromSeconds(0.5))
                        .Subscribe(_ => Reset())
                        .AddTo(this);
                    scoreManager.CanScore = false;
                    KillPlayers();
                })
                .AddTo(this);
        }
        players = FindObjectsOfType<ShipBehaviour_V2>();

        yield return new WaitForSeconds(0.25f);

        spawnManager = FindObjectOfType<SpawnManager>();

        if (playersData.Length != 4) { Debug.LogError("Missing some players..."); }

        playersData = playersData.OrderBy(player => player.behaviour.playerID).ToArray();
        playersData.ForEach(player => player.SetActive(false));
    }

    internal void EndLobby()
    {
        activePlayers = players
            .Where(player => player.IsFrozen == false)
            .ToArray();

        playersData.Where(player => player.behaviour.IsFrozen)
            .ForEach(player => {
                player.SetActive(false);
                player.behaviour.gameObject.SetActive(false);
                player.behaviour.invulnerability = true;
            });

        onGameBegin.OnNext(Unit.Default);

        scoreManager.CanScore = true;
        IsInGame = true;
    }

    internal void Unfreeze(int playerID)
    {
        playersData[playerID].SetActive(true);
    }

    internal void Reset()
    {
        KillPlayers();
        onGameEnd.OnNext(Unit.Default);
        scoreManager.CanScore = true;
    }

    private void KillPlayers() {
        playersData.ForEach(player => {
            if (player.behaviour.gameObject.activeSelf == true) {
                player.behaviour.Death();
            } else {
                player.behaviour.gameObject.SetActive(true);
            }
            player.SetActive(false);
        });
    }
}
