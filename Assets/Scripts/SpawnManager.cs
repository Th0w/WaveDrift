/*
 *  Responsible for every in game spawning. From the enemy to their spawners and even the bonuses.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Fields
    private const string BASE = "base";
    #region Serialized

    [Header("Wave to spawn")]

    [SerializeField]
    private WaveDataSO waveData;

    [SerializeField]
    private PoolData[] poolsToSpawn;

    [Header("Maximum")]

    [SerializeField]
    private int maxPhaseId;
    [SerializeField]
    private int maxNormalSpawners = 10;
    [SerializeField]
    private int maxTPSpawners = 7;

    [Header("Prefab info")]

    [SerializeField]
    private string[] mobNames;

    [SerializeField]
    private Poolable
        normalSpawnerPrefab,
        tpSpawnerPrefab,
        powerUpPrefab,
        multiplierPrefab;

    [Header("Bonuses info")]
    [SerializeField]
    private float
        powerUpSpawnInterval,
        multSpawnInterval;

    [SerializeField]
    private int spawnedMultValue;
    [SerializeField]
    private Vector3 powerUpOffset;

    [Header("Misc")]
    private float mapDimensions = 170.0f;
    #endregion Serialized

    private int waveId = 0;
    private int phaseId = 0;

    private List<Spawner> spawners;
    private PoolManager poolManager;

    private int normalCount;
    private int tpCount;

    private Subject<Unit> onEndSpawn;
    private IDisposable nextSpawnTimer;
    private IDisposable spawnInterval_powerUp;
    private IDisposable spawnInterval_multiplier;

    // Cached pools
    private Pool
        multiplierPool,
        powerUpPool,
        normalSpawnerPool,
        tpSpawnerPool;

    private GameManager gameManager;
    private MessagingCenter messagingCenter;
    public static Regex regex;

    #endregion Fields

    #region Properties

    public IObservable<Unit> OnEndSpawn { get { return onEndSpawn; } }

    public bool IsSpawning { get; private set; }

    #endregion Properties

    #region Methods

    #region MonoBehaviour

    private void Awake() {
        regex = new Regex(@"^pool_enemy_", RegexOptions.IgnoreCase);

        onEndSpawn = new Subject<Unit>();

        gameManager = FindObjectOfType<GameManager>();
        poolManager = FindObjectOfType<PoolManager>();
        messagingCenter = FindObjectOfType<MessagingCenter>();
    }

    private void Start() {
        // Pool creations
        {
            spawners = new List<Spawner>();

            poolsToSpawn
                .Where(pool => pool.IsValid)
                .ForEach(data => poolManager.CreatePool(data));

            multiplierPool = poolManager.CreatePool("MultiplierBonus", 3, multiplierPrefab);
            powerUpPool = poolManager.CreatePool("PowerUpBonus", 3, powerUpPrefab);

            normalSpawnerPool = poolManager.CreatePool("NormalSpawner", maxNormalSpawners, normalSpawnerPrefab);
            tpSpawnerPool = poolManager.CreatePool("TPSpawner", maxTPSpawners, tpSpawnerPrefab);
        }
        // SpawnerPoolCallBacks
        {
            normalSpawnerPool.OnSpawn
                .Subscribe(spawner => {
                    spawners.Add((Spawner)spawner);
                    normalCount++;
                })
                .AddTo(this);

            tpSpawnerPool.OnSpawn
                .Subscribe(spawner => {
                    spawners.Add((Spawner)spawner);
                    tpCount++;
                })
                .AddTo(this);

            normalSpawnerPool.OnRecycle
                .Subscribe(spawner => {
                    spawners.Remove((Spawner)spawner);
                    normalCount--;
                })
                .AddTo(this);

            tpSpawnerPool.OnRecycle
                .Subscribe(spawner => {
                    spawners.Remove((Spawner)spawner);
                    tpCount--;
                })
                .AddTo(this);
        }
        // GameManager callback settings
        {
            gameManager.OnGameBegin
                .Subscribe(_ => {
                    Observable.Timer(TimeSpan.FromSeconds(1.0))
                        .Subscribe(BeginSpawn)
                        .AddTo(this);
                }).AddTo(this);

            gameManager.OnGameEnd
                .Subscribe(EndSpawn)
                .AddTo(this);
        }
    }

    #endregion MonoBehaviour

    private void SpawnMult(object obj) {
        if (obj is object[] == false) { return; }

        multiplierPool.Spawn(obj);
    }

    private void BeginSpawn(long frameCount) {
        messagingCenter.RegisterMessage("SpawnMult", SpawnMult);

        AddSpawner(special: false);
        AddSpawner(special: false);

        spawnInterval_powerUp = Observable
            .Interval(TimeSpan.FromSeconds(powerUpSpawnInterval))
            .Subscribe(_ => {
                Vector3 pos = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) * Vector3.right
                    * UnityEngine.Random.Range(15.0f, mapDimensions);

                SpawnMult(new object[] { pos, spawnedMultValue });
            })
            .AddTo(this);

        spawnInterval_multiplier = Observable
            .Interval(TimeSpan.FromSeconds(multSpawnInterval))
            .Subscribe(_ => {
                Vector3 pos = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) * Vector3.right
                    * UnityEngine.Random.Range(15.0f, mapDimensions);

                powerUpPool.Spawn(pos + powerUpOffset);
            })
            .AddTo(this);

        IsSpawning = true;

        NextPhase();
    }

    private void EndSpawn(Unit @default) {
        messagingCenter.UnregisterMessage("SpawnMult");
        IsSpawning = false;

        if (spawnInterval_powerUp != null) {
            spawnInterval_powerUp.Dispose();
            spawnInterval_powerUp = null;
        }
        if (spawnInterval_multiplier != null) {
            spawnInterval_multiplier.Dispose();
            spawnInterval_multiplier = null;
        }

        phaseId = 0;
        waveId = 0;
        
        poolManager.RecycleAll();
    }

    private void NextPhase() {
        if (IsSpawning == false) { return; }

        if ((++phaseId) > maxPhaseId) {
            phaseId = 1;
            ++waveId;
            HandleNextWave();
        }
        HandleSpawn();
    }

    private void HandleNextWave() {
        if ((waveId & 1) == 0) {
            AddSpawner(special: false);
        } else {
            AddSpawner(special: true);
        }
    }

    private void AddSpawner(bool special) {
        if (special ? (tpCount >= maxTPSpawners) : (normalCount >= maxNormalSpawners)) { return; }
        Vector3 pos = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f) *
            Vector3.forward * UnityEngine.Random.Range(0.0f, mapDimensions);
        normalSpawnerPool.Spawn(pos);
    }

    private void HandleSpawn() {
        var data = waveData.WaveData[phaseId - 1];
        Spawn(data.distFromSpawn, data.randomToSpawn, data.basicToSpawn);

        nextSpawnTimer = Observable.Timer(TimeSpan.FromSeconds(data.delayTilNextWave))
            .Subscribe(_ => NextPhase())
            .AddTo(this);
    }

    private void Spawn(int distFromSpawn, int randomToSpawn, int basicToSpawn) {
        List<Pool> pools;
        int mobNameLength = mobNames.Length, total;
        spawners.ForEach(spawner => {
            total = randomToSpawn + basicToSpawn;
            pools = null;
            pools = new List<Pool>(total);
            for (int i = 0; i < basicToSpawn; ++i) {
                pools.Add(poolManager[BASE]);
            }
            for (int i = 0; i < randomToSpawn; ++i) {
                Pool pool = poolManager[regex]
                    .Where(p => p.Name.Contains(BASE) == false)
                    .Random();
                pools.Add(pool);
            }
            spawner.DoSpawn(pools, distFromSpawn);
        });
    }

    #endregion Methods
}