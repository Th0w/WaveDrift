using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Text.RegularExpressions;
using System;

[RequireComponent(typeof(Collider))]
public class ScoreBonus : Poolable
{
    private int scoreValue;

    public override Poolable Init(Pool parent)
    {
        Regex regex = new Regex(@"^Ship_P(\w)$");
        this.OnTriggerEnterAsObservable()
            .Select(collider => regex.Match(collider.name))
            .Where(match => match.Success)
            .Subscribe(match => {
                int pid = int.Parse(match.Groups[1].Value);
                MessagingCenter.Instance.FireMessage("PlayerGainScore", new object[] { pid, scoreValue });
                Destroy(gameObject);
            }).AddTo(this);
        this.parent = parent;
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
        scoreValue = (int)(obj[1]);
        gameObject.SetActive(true);
    }
}
