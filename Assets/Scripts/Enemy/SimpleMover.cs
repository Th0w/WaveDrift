﻿using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SimpleMover : BaseMovingUnit {
    #region Fields
    #region Serialized
    [Header("Turn smoother")]
    [SerializeField]
    protected float turnSpeed = 2.0f  ;
	public GameObject deathGroup;
    #endregion Serialized
    #endregion Fields
    #region Methods
    protected virtual void Attack()
    {
//        Debug.Log("BOUM I ATTACKED!");
		ShipBehaviour_V2 ship = target.GetComponent<ShipBehaviour_V2>();
		if (!ship.death && !ship.invulnerability && ship.currentCoroutine == null)
			ship.currentCoroutine = StartCoroutine(ship.Death (ship.deathDelay));
    }

    protected virtual void MoveFunction(Vector3 distance)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(distance), Time.deltaTime * turnSpeed);
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public override Poolable Init(Pool parent)
    {
        gameObject.SetActive(false);

        this.parent = parent;

        this.OnEnableAsObservable()
            .Subscribe(_ =>
            {
                var tar = FindObjectsOfType<ShipBehaviour_V2>()
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
            //.Where(dist => Physics.Raycast(transform.position, transform.forward, 1.0f) == false)
            .Subscribe(MoveFunction)
            .AddTo(this);

        update.Where(dist => dist.magnitude <= attackRange)
            .Subscribe(dist =>
            {
                IsOccupied = true;
                Attack();
                Observable.Timer(TimeSpan.FromSeconds(idleTime))
                    .Subscribe(_ =>
                    {
                        IsOccupied = false;
//                        Debug.Log("Done waiting");
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

    protected override void Death(int playerID)
    {
        // TODO Visual death


        MessagingCenter.Instance.FireMessage("UnitKilled", new object[] { playerID, scorePerKilled });

		Instantiate (deathGroup, transform.position, Quaternion.identity);

		parent.Recycle (this);
    }

	public void OnParticleCollision (GameObject other) {
		TakeDamage (1, 1);
	}

    #endregion
}
