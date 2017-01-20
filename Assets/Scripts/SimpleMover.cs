using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SimpleMover : BaseMovingUnit {
    
	// Use this for initialization
	void Start () {

        this.OnEnableAsObservable()
            .Subscribe(_ =>
            {
                target = GameObject.FindGameObjectsWithTag("Player")
                    .OrderBy(go => (transform.position - go.transform.position).magnitude)
                    .FirstOrDefault()
                    .transform;

            }).AddTo(this);

        gameObject.SetActive(false);
        gameObject.SetActive(true);

        var update = this.UpdateAsObservable()
            .Where(_ => HasTarget)
            .Where(_ => IsOccupied == false)
            .Select(_ => (target.position - transform.position));

        update
            .Where(dist => dist.magnitude > attackRange)
            .Subscribe(dist => transform.position += dist.normalized * Time.deltaTime * speed)
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
	}
}
