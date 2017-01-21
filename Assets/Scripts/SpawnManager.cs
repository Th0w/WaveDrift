using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class WaveData
{
    public int distFromSpawn;
    public int randomToSpawn;
    public int basicToSpawn;
    public float delayTilNextWave;
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private WaveData[] waveData;
    [Header("Maximums")]
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
    private Spawner normalSpawnerPrefab;

    [SerializeField]
    private Spawner tpSpawnerPrefab;

    [SerializeField]
    private Transform spawnedHolder;

    private int waveId = 0;
    private int phaseId = 0;
    
    private List<Spawner> spawners;
    private PoolManager poolManager;

    private int normalCount;
    private int tpCount;

    public Transform SpawnedHolder { get { return spawnedHolder; } }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        Init();
    }

    private void Init()
    {
        spawners = new List<Spawner>(2);
        poolManager = FindObjectOfType<PoolManager>();
        
        AddNormalSpawner();
        AddNormalSpawner();

        NextPhase();
    }

    private void NextPhase()
    {
        if ((++phaseId) > maxPhaseId)
        {
            phaseId = 1;
            ++waveId;
            HandleNextWave();
        }
        HandleSpawn();
    }

    private void HandleNextWave()
    {
        if((waveId & 1) == 0)
        {
            AddNormalSpawner();
        }
        else
        {
            AddTPSpawner();
        }
    }

    private void AddTPSpawner()
    {
        if (tpCount >= maxTPSpawners) { return; }

        var spn = Instantiate(tpSpawnerPrefab);
        spawners.Add(spn);
        Vector3 pos = UnityEngine.Random.insideUnitSphere * 175.0f;
        pos.y = 0.0f;
        spn.transform.position = pos;
        ++tpCount;
    }

    private void AddNormalSpawner()
    {
        if(normalCount >= maxNormalSpawners) { return; }

        var spn = Instantiate(normalSpawnerPrefab);
        spawners.Add(spn);
        Vector3 pos = UnityEngine.Random.insideUnitSphere * 175.0f;
        pos.y = 0.0f;
        spn.transform.position = pos;
        ++normalCount;
    }

    private void HandleSpawn()
    {
        var data = waveData[phaseId - 1];
        Spawn(data.distFromSpawn, data.randomToSpawn, data.basicToSpawn);
        Observable.Timer(TimeSpan.FromSeconds(data.delayTilNextWave))
            .Subscribe(_ => NextPhase())
            .AddTo(this);
    }

    private void Spawn(int distFromSpawn, int randomToSpawn, int basicToSpawn)
    {
        List<Pool> plz;
        int mobNameLength = mobNames.Length, total;
        spawners.ForEach(spawner =>
        {
            total = randomToSpawn + basicToSpawn;
            plz = null;
            plz = new List<Pool>(total);
            for (int i = 0; i < basicToSpawn; ++i) {
                plz.Add(poolManager["base"]);
            }
            for(int i=0; i < randomToSpawn; ++i)
            {
                string name = mobNames[UnityEngine.Random.Range(0, mobNameLength)];
                Pool p = poolManager[name];
                plz.Add(poolManager[name]);
            }
            spawner.Spawn(plz, distFromSpawn);
        });
    }
}
