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

public class Spawner : MonoBehaviour
{
    private PoolManager poolManager;

    public List<SpawnData> spawnDatas;

    protected Subject<Unit> onSpawnedWave;

    protected virtual void Start()
    {
        onSpawnedWave = new Subject<Unit>();

        poolManager = FindObjectOfType<PoolManager>();
        float lastTimer = 0.0f;
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
                if (spawnData.timer > lastTimer) { lastTimer = spawnData.timer; }
            }

            Observable.Timer(TimeSpan.FromSeconds(spawnData.timer + 0.25))
                .Subscribe(_ => onSpawnedWave.OnNext(Unit.Default))
                .AddTo(this);
        });

        Observable.Timer(TimeSpan.FromSeconds(lastTimer))
            .Subscribe(_ => MessagingCenter.Instance.FireMessage("SpawnEnd", this))
            .AddTo(this);

    }

    internal void Spawn(List<BaseMovingUnit> list, int distFromSpawn)
    {
        int i, max;
        for (i = 0, max = list.Count; i < max; ++i)
        {
            list[i].Parent.Spawn(transform.position + Quaternion.Euler(0, 360.0f / max * i, 0) * transform.forward * distFromSpawn);
        }
    }
    internal void Spawn(List<Pool> list, int distFromSpawn)
    {
        list.ForEach(pool => Debug.Log(pool.name));
        Debug.Break();
        int i, max;
        for (i = 0, max = list.Count; i < max; ++i)
        {
            list[i].Spawn(transform.position + Quaternion.Euler(0, 360.0f / max * i, 0) * transform.forward * distFromSpawn);
        }
    }
}
