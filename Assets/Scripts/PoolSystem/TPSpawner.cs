using UniRx;
using UnityEngine;

public class TPSpawner : Spawner
{
    public override Poolable Init(Pool parent) {
        base.Init(parent);
        onSpawnedWave.Subscribe(u => {
            Vector3 pos = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Vector3.right
                * Random.Range(45.0f, 150.0f);
            transform.position = pos;
        });
        return this;
    }
}
