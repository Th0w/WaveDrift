using UniRx;
using UnityEngine;

public class TPSpawner : Spawner
{
    protected override void Start()
    {
        base.Start();
        onSpawnedWave.Subscribe(Observer.Create<Unit>(u =>
        {
            Vector3 newpos = Random.insideUnitSphere * 350.0f;
            newpos.y = 0.0f;
            transform.position = newpos;
        }));
    }
}
