using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Spawner : Poolable
{
    private PoolManager poolManager;

    protected Subject<Unit> onSpawnedWave;

    public override Poolable Init(Pool parent) {
        gameObject.SetActive(false);

        this.parent = parent;
        onSpawnedWave = new Subject<Unit>();

        poolManager = FindObjectOfType<PoolManager>();

        return this;
    }

    internal void DoSpawn(List<BaseMovingUnit> list, int distFromSpawn) {
        int i, max;
        for (i = 0, max = list.Count; i < max; ++i) {
            list[i].Parent.Spawn(transform.position + Quaternion.Euler(0, 360.0f / max * i, 0) * transform.forward * distFromSpawn);
        }
    }

    internal void DoSpawn(List<Pool> list, int distFromSpawn) {
        int i, max;
        for (i = 0, max = list.Count; i < max; ++i) {
            list[i].Spawn(transform.position + Quaternion.Euler(0, 360.0f / max * i, 0) * transform.forward * distFromSpawn);
        }
    }

    public override void Spawn(object args) {
        transform.position = args is Vector3 ? (Vector3)args : Vector3.zero;
        gameObject.SetActive(true);
    }

    public override void Recycle() {
        gameObject.SetActive(false);
    }
}