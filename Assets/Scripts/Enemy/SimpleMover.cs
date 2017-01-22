using System;
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
		var ship = target.GetComponent<ShipBehaviour_V2>();
        if (ship == null) { return; }
		if (!ship.death && !ship.invulnerability && !ship.airProtection)
			ship.Death ();
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
            .Subscribe(_ => UpdateTarget())
            .AddTo(this);

        var update = this.UpdateAsObservable()
            .Where(_ => UpdateTarget())
            .Where(_ => IsOccupied == false)
            .Select(_ => (target.position - transform.position));

        update
            .Where(dist => dist.magnitude > attackRange)
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
        MessagingCenter.Instance.FireMessage("UnitKilled", new object[] { playerID, scorePerKilled });

		Instantiate (deathGroup, transform.position, Quaternion.identity);

		parent.Recycle (this);
    }

	public void OnParticleCollision (GameObject other) {
        string name = other.transform.parent.parent.name;
        name = name.Substring(name.Length - 1);

		TakeDamage (1, int.Parse(name));
	}

    private bool UpdateTarget()
    {
        if (HasTarget)
        {
            var sb = target.GetComponent<ShipBehaviour_V2>();
            if (sb != null)
            {
                return (sb.invulnerability || sb.death) ? GetTarget() : true;
            }
            else
            {
                return GetTarget();
            }            
        }
        else
        {
            return GetTarget();
        }
    }

    private bool GetTarget()
    {
        var tar = ShipDetector.allShipBehaviours
            .Where(sbe => (sbe.invulnerability || sbe.death) == false)
            .OrderBy(sbe => Vector3.Distance(sbe.transform.position, transform.position))
            .FirstOrDefault();

        return (target = ((tar != null) ? tar.transform : null) ?? ShipDetector.DefaultTransform) != null;
    }

    #endregion
}
