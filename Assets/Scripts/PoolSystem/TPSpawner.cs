using UniRx;
using UnityEngine;

public class TPSpawner : Spawner
{
    [SerializeField]
    private Transform[] positions;

    protected override void Start()
    {
        base.Start();
        onSpawnedWave.Subscribe(Observer.Create<Unit>(u =>
        {
            Vector3 oldpos = transform.position;
            Vector3 newpos = positions.Length == 0 ? transform.position :
                positions[Random.Range(0, positions.Length)].position;
            transform.position = newpos;

            Debug.LogWarningFormat("Oldpos: {0}, newpos: {1}.", oldpos, newpos);

            Debug.LogWarning("Is tping!");
        }));
    }
}
