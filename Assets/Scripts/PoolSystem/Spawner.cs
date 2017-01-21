using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public GameObject prefab;
    public float timer;
    public int quantity;
    public float distFromSpawn;
}

public class Spawner : MonoBehaviour {
    private PoolManager poolManager;

    public List<SpawnData> spawnDatas;

    protected Subject<Unit> onSpawnedWave;

    protected virtual void Start()
    {
        onSpawnedWave = new Subject<Unit>();

        poolManager = FindObjectOfType<PoolManager>();

        spawnDatas.ForEach(spawnData =>
        {
            Pool p;
            int i, max;
            p = poolManager[spawnData.prefab];
            max = spawnData.quantity;
            for (i = 0; i < max; ++i)
            {
                int i2 = i;
                Observable.Timer(TimeSpan.FromSeconds(spawnData.timer))
                    .Subscribe(_ => p.Spawn(transform.position + Quaternion.Euler(0, 360.0f / max * i2, 0) * transform.forward * spawnData.distFromSpawn))
                    .AddTo(this);
            }

            Observable.Timer(TimeSpan.FromSeconds(spawnData.timer + 0.25))
                .Subscribe(_ => onSpawnedWave.OnNext(Unit.Default))
                .AddTo(this);
        });
    }
}
