using UnityEngine;

public abstract class BaseMovingUnit : Poolable {

    protected Transform target;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float health;

    [SerializeField]
    protected float attackRange;

    [SerializeField]
    protected float idleTime = 0.5f;

    public bool IsOccupied { get; protected set; }

    public bool HasTarget { get { return target != null; } }
}
