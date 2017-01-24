using UniRx;
using UnityEngine;

public class TPSpawner : Spawner
{
    public override Poolable Init(Pool parent) {
        base.Init(parent);
        onSpawnedWave.Subscribe(u => {
            Vector3 newpos = Random.insideUnitSphere * 350.0f;
            newpos.y = 0.0f;
            transform.position = newpos;
        });
        return this;
    }
}
