using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class RandomEventSpawner : MonoBehaviour {
    [SerializeField]
    private float eventDeltaSpawn;

    private List<BaseRandomEvent> eventPrefabs;

    private PoolManager poolManager;

    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();

        

        eventPrefabs.ForEach(prefab => poolManager.CreatePool(new PoolData(prefab.name, 2, prefab.gameObject)));

        Observable.Interval(TimeSpan.FromSeconds(eventDeltaSpawn))
            .Subscribe(SpawnRandomEvent)
            .AddTo(this);
    }

    private void SpawnRandomEvent(long frameCount)
    {
        var dim = ArenaDimensions.Instance;
        Vector3 newPos = new Vector3(UnityEngine.Random.Range(dim.min.x, dim.max.x), 0.0f, UnityEngine.Random.Range(dim.min.y, dim.max.y));

        poolManager[eventPrefabs[UnityEngine.Random.Range(0, eventPrefabs.Count)].gameObject].Spawn(newPos, true);
    }
}
