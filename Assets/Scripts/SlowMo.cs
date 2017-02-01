using System.Linq;
using UniRx.Triggers;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UniRx;

public class SlowMo : MonoBehaviour {

	public static Animator selfAnimator;
	public float ts;

	public Fisheye fish;
	public float fishStrength;

    public bool IsActive { get; private set; }

    void Awake () {

		selfAnimator = GetComponent<Animator> ();

        var osmt = selfAnimator.GetBehaviour<ObservableStateMachineTrigger>();

        osmt.OnStateEnterAsObservable()
            .Subscribe(stateInfo => IsActive = true)
            .AddTo(this);

        osmt.OnStateExitAsObservable()
            .Subscribe(stateInfo => IsActive = false)
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => IsActive)
            .Subscribe(_ => {
                Time.timeScale = ts;
                fish.strengthX = fishStrength;
                fish.strengthY = fishStrength;
            })
            .AddTo(this);
    }
}
