using UnityEngine;

public abstract class BaseRandomEvent : Poolable
{
    protected abstract void ObjectInit();

    public override Poolable Init(Pool parent)
    {
        this.parent = parent;

        ObjectInit();

        return this;
    }

    public override void Recycle()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    public override void Spawn(object args)
    {
        if (args is Vector3 == false) {
            parent.Recycle(this);
            return;
        }
        transform.position = (Vector3)args;
        gameObject.SetActive(true);
    }
}
