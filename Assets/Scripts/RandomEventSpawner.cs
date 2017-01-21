using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class RandomEventSpawner : MonoBehaviour {
    [SerializeField]
    private List<Transform> levelLimits;
    [SerializeField]
    private float eventDeltaSpawn;

    private Vector2 min, max;

    private List<BaseRandomEvent> eventPrefabs;

    private PoolManager poolManager;

    private void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();

        min = new Vector2(
            Mathf.Min(levelLimits.Select(transform => transform.position.x).ToArray()),
            Mathf.Min(levelLimits.Select(transform => transform.position.y).ToArray()));
        max = new Vector2(
            Mathf.Max(levelLimits.Select(transform => transform.position.x).ToArray()),
            Mathf.Max(levelLimits.Select(transform => transform.position.y).ToArray()));

        eventPrefabs.ForEach(prefab => poolManager.CreatePool(new PoolData(prefab.name, 2, prefab.gameObject)));

        Observable.Interval(TimeSpan.FromSeconds(eventDeltaSpawn))
            .Subscribe(SpawnRandomEvent)
            .AddTo(this);
    }

    private void SpawnRandomEvent(long frameCount)
    {
        Vector3 newPos = new Vector3(UnityEngine.Random.Range(min.x, max.x), 0.0f, UnityEngine.Random.Range(min.y, max.y));

        poolManager[eventPrefabs[UnityEngine.Random.Range(0, eventPrefabs.Count)].gameObject].Spawn(newPos, true);
    }
}
