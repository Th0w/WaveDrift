using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SimpleMover : BaseMovingUnit {

    public override Poolable Init(Pool parent)
    {
        gameObject.SetActive(false);

        this.parent = parent;

        this.OnEnableAsObservable()
            .Subscribe(_ =>
            {
                var tar = GameObject.FindGameObjectsWithTag("Player")
                    .OrderBy(go => (transform.position - go.transform.position).magnitude)
                    .FirstOrDefault();

                target = tar != null ? tar.transform : null;
                if (target == null)
                {
                    Debug.LogError("No player found!");
                }

            }).AddTo(this);

        var update = this.UpdateAsObservable()
            .Where(_ => HasTarget)
            .Where(_ => IsOccupied == false)
            .Select(_ => (target.position - transform.position));

        update
            .Where(dist => dist.magnitude > attackRange)
            .Subscribe(dist =>
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f)) {
                    Debug.Log(hit.transform.name);
                    return;
                }
                transform.rotation = Quaternion.LookRotation(dist);
                transform.position += transform.forward * Time.deltaTime * speed;
            })
            .AddTo(this);

        update.Where(dist => dist.magnitude <= attackRange)
            .Subscribe(dist =>
            {
                IsOccupied = true;
                Debug.Log("BOUM I ATTACKED!");
                Observable.Timer(TimeSpan.FromSeconds(idleTime))
                    .Subscribe(_ =>
                    {
                        IsOccupied = false;
                        Debug.Log("Done waiting");
                    })
                    .AddTo(this);
            })
            .AddTo(this);

        return this;
    }

    public override void Recycle()
    {
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }

    public override void Spawn(object args)
    {
        if (args is Vector3 == false)
        {
            Debug.LogErrorFormat("Wrong args on {0}'s spawn methods", name);
            return;
        }
        transform.position = (Vector3)args;
        gameObject.SetActive(true);
    }
}
