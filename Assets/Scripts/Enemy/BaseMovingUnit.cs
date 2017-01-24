using UniRx;
using UnityEngine;

public abstract class BaseMovingUnit : Poolable {

    #region Fields

    #region Serialized
    [Header("Base enemy info")]
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

    [Header("Scoring")]
    [SerializeField]
    protected int scorePerHitPoint;
    [SerializeField]
    protected int scorePerKilled;

    #endregion Serialized

    #endregion Fields

    #region Properties

    public bool IsOccupied { get; protected set; }

    public bool HasTarget { get { return target != null; } }

    public IObservable<Unit> OnTakeDamage { get { return onTakeDamage; } }


    public void TakeDamage(int v, int playerID)
    {
        health -= v;

        messagingCenter.FireMessage("UnitTookDamage", new object[] { playerID, scorePerHitPoint });

        if (health <= 0)
        {
            Death(playerID);
        }
    }

	public void Kill(int playerId)
	{
		Death(playerId + 1);
	}

    protected abstract void Death(int playerID);

    #endregion Properties
}
