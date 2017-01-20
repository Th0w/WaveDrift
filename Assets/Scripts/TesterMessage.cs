using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class TesterMessage : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MessagingCenter mc = MessagingCenter.Instance;

        mc.RegisterMessage("Awakened", o => Debug.Log("Awakened!"));
        mc.RegisterMessage("Interval", o => Debug.LogFormat("Interval! Saying {0}.", o));

        Observable.Timer(TimeSpan.FromSeconds(1.0))
            .Subscribe(_ =>
            {
                mc.FireMessage("Awakened", null);
                Observable.Interval(TimeSpan.FromSeconds(1.0))
                    .Subscribe(__ => mc.FireMessage("Interval", "int"))
                    .AddTo(this);
            })
            .AddTo(this);

        Observable.Timer(TimeSpan.FromSeconds(5.0))
            .Subscribe(o => Debug.Log("Waited 5sec"))
            .AddTo(this);
	}
}
