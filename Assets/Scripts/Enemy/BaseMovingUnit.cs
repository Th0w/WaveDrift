using UniRx;
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
    protected Subject<Unit> onTakeDamage;

    [SerializeField]
    protected int scorePerHitPoint, scorePerKilled;

    #endregion Serialized

    #endregion Fields

    #region Properties

    public bool IsOccupied { get; protected set; }

    public bool HasTarget { get { return target != null; } }

    public IObservable<Unit> OnTakeDamage { get { return onTakeDamage; } }

    public void TakeDamage(int v, int playerID)
    {
        health -= v;

        MessagingCenter.Instance.FireMessage("UnitTookDamage", new object[] { playerID, scorePerHitPoint });

        if (health <= 0)
        {
            Death(playerID);
        }
    }

    protected abstract void Death(int playerID);

    #endregion Properties
}
