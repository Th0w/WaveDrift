using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Tester : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.F))
            .Subscribe(_ =>
            {
                Debug.Log("Should gain money");
                MessagingCenter.Instance.FireMessage("UnitKilled", new object[] { 1, 50 });
            }).AddTo(this);
	}
}
