using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

namespace Wift {
    public class Consts
    {
        public const string BUTTON_A = "Button A";
        public const string BUTTON_X = "Button X";
        public const string START = "Start";
        public const string SELECT = "Select";
        public const string RIGHT_TRIGGER = "Right Trigger";
        public const string LEFT_STICK_X = "Left Stick X";
        public const string AXIS_Y = "Y Axis";
        public const string MENU_UP = "MenuUp";
        public const string MENU_DOWN = "MenuDown";
        public const string MENU_LEFT = "MenuLeft";
        public const string MENU_RIGHT = "MenuRight";
        public const string MENU_CONFIRM = "MenuConfirm";
        public const string MENU_CANCEL = "MenuCancel";
    }
}

public class PlayerInput : MonoBehaviour {
    private int id = -1;
    private Player src;

    //public IObservable<bool> Jump { get; private set; }
    //public IObservable<bool> Drift { get; private set; }
    //public IObservable<float> Boost { get; private set; }
    //public IObservable<float> Turn { get; private set; }
    public IObservable<bool> SelectPressed { get; private set; }
    public IObservable<bool> StartPressed { get; private set; }

    public bool Init(int id) {
        this.id = id;
        if (this.id == -1) { return false; }

        src = ReInput.players.GetPlayer(this.id);

        //Jump = this.UpdateAsObservable()
        //    .Select(_ => src.GetButtonDown(Wift.Consts.BUTTON_A));

        //Drift = this.UpdateAsObservable()
        //    .Select(_ => src.GetButton(Wift.Consts.BUTTON_X));

        //Turn = this.UpdateAsObservable()
        //    .Select(_ => src.GetAxis(Wift.Consts.LEFT_STICK_X));

        //Boost = this.UpdateAsObservable()
        //    .Select(_ => src.GetAxis(Wift.Consts.RIGHT_TRIGGER));

        StartPressed = ObsEither(2.0, Wift.Consts.START);
        SelectPressed = ObsEither(2.0, Wift.Consts.SELECT);

        return true;
    }

    private IObservable<bool> ObsEither(double duration, string inputName) {
        bool keyDown = false;
        var down = this.UpdateAsObservable()
            .Where(_ => src.GetButtonDown(inputName));
        
        var onTimer = new Subject<Unit>();
        var observable = new Subject<bool>();
        IDisposable timer = null;
        var t = down
            .Subscribe(_ => {
                keyDown = true;
                timer = Observable.Timer(TimeSpan.FromSeconds(duration))
                    .Subscribe(__ => onTimer.OnNext(Unit.Default))
                    .AddTo(this);
            }).AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => keyDown)
            .Where(_ => src.GetButtonUp(Wift.Consts.START))
            .Select(_ => false)
            .Merge(onTimer.Select(_ => true))
            .Subscribe(b => {
                keyDown = false;
                if (timer != null) {
                    timer.Dispose();
                    timer = null;
                }
                observable.OnNext(b);
            })
            .AddTo(this);
        return observable;
    }
}
