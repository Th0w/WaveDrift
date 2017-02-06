using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AudioManager : MonoBehaviour {
    public float Volume { get; private set; }

    private Transform sourceTransform;
	// Use this for initialization
	void Start () {
        Volume = 100.0f;
        var twm = FindObjectOfType<TerrainWaveManager>();

        sourceTransform = twm.transform;

        twm.OnInitDone
            .Subscribe(_ => ChangeVolume(100.0f))
            .AddTo(this);
	}

    public void ChangeVolume(float volume) {
        Volume = Mathf.Clamp(volume, 0.0f, 100.0f);
        transform.position = sourceTransform.localPosition + Vector3.up * (100.0f - Volume);
    }
}
