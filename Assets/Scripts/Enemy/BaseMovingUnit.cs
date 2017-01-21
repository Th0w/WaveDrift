using UnityEngine;

public abstract class BaseMovingUnit : Poolable {

    #region Fields

    #region Serialized

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float health;

    [SerializeField]
    protected float attackRange;

    [SerializeField]
    protected float idleTime = 0.5f;

    protected Transform target;

    #endregion Serialized

    #endregion Fields

    #region Properties

    public bool IsOccupied { get; protected set; }

    public bool HasTarget { get { return target != null; } }

    #endregion Properties
}
