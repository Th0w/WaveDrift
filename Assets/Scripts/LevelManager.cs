using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    private int levelId = -1;

    private List<Spawner> spawners;
    private Subject<Unit> onGameEnd;

    private List<Poolable> actives;

    private bool isStillSpawning = true;

    public IObservable<Unit> OnGameEnd { get { return onGameEnd; } }

	void Start () {
        onGameEnd = new Subject<Unit>();
        actives = new List<Poolable>();

        spawners = FindObjectsOfType<Spawner>().ToList();

        var pools = FindObjectsOfType<Pool>();
        pools.ForEach(pool => pool.OnSpawn.Subscribe(poolable => actives.Add(poolable)));
        pools.ForEach(pool => pool.OnRecycle.Subscribe(poolable =>
        {
            actives.Remove(poolable);
            if (isStillSpawning == false && actives.Count == 0)
            {
                Debug.Log("End of level.");
            }
        }));

        MessagingCenter.Instance.RegisterMessage("SpawnEnd", obj =>
        {
            if (obj is Spawner == false) { return; }
            var s = (Spawner)obj;
            if (spawners.Contains(s) == false)
            {
                Debug.LogErrorFormat("Already removed {0}!", s.name);
                return;
            }
            spawners.Remove(s);

            if (spawners.Count == 0)
            {
                onGameEnd.OnNext(Unit.Default);
            }
        });

        onGameEnd.Subscribe(u => Debug.LogError("Done all spawns!"));

        this.OnDestroyAsObservable()
            .Subscribe(_ => MessagingCenter.Instance.UnregisterMessage("SpawnEnd"))
            .AddTo(this);
	}
}
