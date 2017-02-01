using System.Linq;
using UniRx.Triggers;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UniRx;

public class SlowMo : MonoBehaviour {

    private Subject<Unit> onSlowMoBegin, onSlowMoEnd;
    
	public static Animator selfAnimator;
	public float ts;

	public Fisheye fish;
	public float fishStrength;

    public bool IsActive { get; private set; }

    public IObservable<Unit> OnSlowMoBegin { get { return onSlowMoBegin; } }

    public IObservable<Unit> OnSlowMoEnd { get { return onSlowMoEnd; } }

    void Awake() {
        onSlowMoBegin = new Subject<Unit>();
        onSlowMoEnd = new Subject<Unit>();

        selfAnimator = GetComponent<Animator>();

        var osmt = selfAnimator.GetBehaviour<ObservableStateMachineTrigger>();

        osmt.OnStateEnterAsObservable()
            .Subscribe(stateInfo => {
                IsActive = true;
                onSlowMoBegin.OnNext(Unit.Default);
            })
            .AddTo(this);

        osmt.OnStateExitAsObservable()
            .Subscribe(stateInfo => {
                IsActive = false;
                onSlowMoEnd.OnNext(Unit.Default);
            })
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
