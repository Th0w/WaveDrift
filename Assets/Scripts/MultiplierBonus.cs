using System;
using System.Text.RegularExpressions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MultiplierBonus : Poolable {

    private int multiplierValue;

    public override Poolable Init(Pool parent)
    {
        Regex regex = new Regex(@"^Ship_P(\w)$");
        this.OnTriggerEnterAsObservable()
            .Subscribe(coll => Debug.Log(coll.name));
        this.OnTriggerEnterAsObservable()
            .Select(collider => regex.Match(collider.name))
            .Where(match => match.Success)
            .Subscribe(match => {
                int pid = int.Parse(match.Groups[1].Value);
                MessagingCenter.Instance.FireMessage("AddPlayerScoreMultiplier", new object[] { pid, multiplierValue });
                Destroy(gameObject);
            }).AddTo(this);
        return this;
    }

    public override void Recycle()
    {
        gameObject.SetActive(false);
    }

    public override void Spawn(object args)
    {
        if (args is object[] == false)
        {
            return;
        }
        var obj = (object[])args;
        transform.position = (Vector3)(obj[0]);
        multiplierValue = (int)(obj[1]);
        gameObject.SetActive(true);
    }
}
