using UniRx;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    protected Subject<Unit> onDeath;
    public IObservable<Unit> OnDeath { get { return onDeath; } }
    protected Pool parent;
    public Pool Parent { get { return parent; } }
    public abstract Poolable Init(Pool parent);
    public abstract void Spawn(object args);
    public abstract void Recycle();
}