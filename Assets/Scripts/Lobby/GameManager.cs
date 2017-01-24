using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour {   

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

        IsInGame = true;
    }

    internal void Unfreeze(int playerID)
    {
        playersData[playerID].SetActive(true);
    }

    internal void Reset()
    {
        playersData.ForEach(player => {
            if (player.behaviour.gameObject.activeSelf == true) {
                player.behaviour.Death();
            } else {
                player.behaviour.gameObject.SetActive(true);
            }
            player.SetActive(false);
        });
        onGameEnd.OnNext(Unit.Default);
    }
}
