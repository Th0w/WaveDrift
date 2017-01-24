using UniRx;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    protected Subject<Unit> onDeath;
    protected Pool parent;
    protected MessagingCenter messagingCenter { get; private set; }
    public IObservable<Unit> OnDeath { get { return onDeath; } }
    public Pool Parent { get { return parent; } }

    public abstract Poolable Init(Pool parent);
    public abstract void Spawn(object args);
    public abstract void Recycle();
    private void Awake() {
        messagingCenter = FindObjectOfType<MessagingCenter>();
    }
}