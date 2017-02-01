using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(PlayerInput))]
public class TesterInputer : MonoBehaviour {
    private PlayerInput input;

    private void Start() {
        input = GetComponent<PlayerInput>();

        if (input.Init(0) == false) {
            return;
        }

        input.SelectPressed
            .Merge(input.StartPressed)
            .Subscribe(b => Debug.LogFormat("Bool value: {0}", b))
            .AddTo(this);
    }
}
