using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private GameObject 
        menu,
        infoPanels,
        gauges;

    [Header("Menu 'buttons'")]
    [SerializeField]
    private List<UILayer> layers;
    private Stack<UILayer> activeLayers;

    private UI_TextShadow[] shadows;

    [Header("Layer behavior")]
    [SerializeField]
    private float layerUnfocusedOpacity = 0.5f;

    private int currentlySelectedIndex = -1;

    private Subject<Unit> 
        onUp,
        onDown,
        onLeft,
        onRight,
        onConfirm,
        onCancel;

    private int selectedPlayerIndex;
    private GameManager gameManager;
    private List<IDisposable> toDispose;
    private IDisposable onCancelDisposable;
    private UILayer menuLayer;

    public IObservable<Unit> OnUp { get { return onUp; } }
    public IObservable<Unit> OnDown { get { return onDown; } }
    public IObservable<Unit> OnLeft { get { return onLeft; } }
    public IObservable<Unit> OnRight { get { return onRight; } }
    public IObservable<Unit> OnConfirm { get { return onConfirm; } }
    public IObservable<Unit> OnCancel { get { return onCancel; } }
    internal bool IsMenuOpen { get { return activeLayers.Count > 0; } }
    public float LayerUnfocusedOpacity { get { return layerUnfocusedOpacity; } }

    #region Methods

    #region MonoBehaviours
    private void Awake() {
        activeLayers = new Stack<UILayer>();
        onUp = new Subject<Unit>();
        onDown = new Subject<Unit>();
        onLeft = new Subject<Unit>();
        onRight = new Subject<Unit>();
        onConfirm = new Subject<Unit>();
        onCancel = new Subject<Unit>();

        ReInput.players.AllPlayers.ForEach(player => {

            this.UpdateAsObservable()
                .Where(_ => player.GetButtonDown(Wift.Consts.MENU_UP))
                .Subscribe(_ => onUp.OnNext(Unit.Default))
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => player.GetButtonDown(Wift.Consts.MENU_DOWN))
                .Subscribe(_ => onDown.OnNext(Unit.Default))
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => player.GetButtonDown(Wift.Consts.MENU_LEFT))
                .Subscribe(_ => onLeft.OnNext(Unit.Default))
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => player.GetButtonDown(Wift.Consts.MENU_RIGHT))
                .Subscribe(_ => onRight.OnNext(Unit.Default))
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => player.GetButtonDown(Wift.Consts.MENU_CONFIRM))
                .Subscribe(_ => onConfirm.OnNext(Unit.Default))
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => player.GetButtonDown(Wift.Consts.MENU_CANCEL))
                .Subscribe(_ => onCancel.OnNext(Unit.Default))
                .AddTo(this);
        });
    }

    private void Start() {
        shadows = Resources.FindObjectsOfTypeAll<UI_TextShadow>();
        gameManager = FindObjectOfType<GameManager>();
        layers.ForEach(layer => layer.Init(this));
        menuLayer = layers.Where(layer => layer is UIMenuLayer).FirstOrDefault();
    }
    public void CloseLayer() {
        int cnt = activeLayers.Count;
        if (cnt == 0) {
            Debug.LogError("Calling CloseMenu while nothing should be opened. Check your calls.");
        } else if (cnt == 1) {
            CloseMenu();
        } else {
            activeLayers.Pop().Disable();
            activeLayers.Peek().Focus();
        }
    }

#endregion MonoBehaviours
    
    internal void OpenMenu(int playerID) {
        if (OpenLayer<UIMenuLayer>() == false) {
#if UNITY_EDITOR
            Debug.LogWarning("Failed to open menu.");
#endif
            return;
        }
        
        gameManager.PlayersData[playerID].behaviour.SwitchLayout(false);
        selectedPlayerIndex = playerID;
        ToggleGameUI(false);
    }

    public bool OpenLayer<T>(T target) where T : UILayer {
        return OpenLayer<T>();
    }

    public bool OpenLayer<T>() where T : UILayer {
        UILayer l = layers.Where(layer => layer is T)
            .Where(layer => layer.IsEnabled == false)
            .FirstOrDefault();
        
        if (l == null) {
#if UNITY_EDITOR
            Debug.LogError("No layer of this type to open");
#endif
            return false;
        }

        activeLayers.Push(l);
        l.Enable();
        return true;
    }

    internal void CloseMenu() {
#if UNITY_EDITOR
        if (activeLayers.Count > 1) {
            Debug.LogError("Too much menu opened. Abnormal behaviour. Fix ASAP.");
            Debug.Break();
        }
#endif
        while(activeLayers.Count != 0) {
            activeLayers.Pop().Disable();
        }

        gameManager.PlayersData[selectedPlayerIndex].behaviour.SwitchLayout(true);
        gameManager.TogglePause(false);
        ToggleGameUI(true);
    }

    private void ToggleGameUI(bool val) {
        infoPanels.SetActive(val);
        gauges.SetActive(val);
        if (val) {
            shadows.ForEach(shadow => {
                shadow.gameObject.SetActive(false);
                shadow.enabled = true;
                shadow.gameObject.SetActive(true);
            });
        }
    }

    private void Clear(IDisposable toDispose) {
        if (toDispose == null) { return; }

        toDispose.Dispose();
        toDispose = null;
    }

#endregion Methods
}

public static class ObservableExtensions
{
    public static IObservable<bool> Latch(
       IObservable<Unit> tick,
       IObservable<Unit> latchTrue,
       bool initialValue) {
        return Observable.Create<bool>(observer => {
            var value = initialValue;

            var latchSub = latchTrue.Subscribe(_ => value = true);

            var tickSub = tick.Subscribe(
                _ => {
                    observer.OnNext(value);
                    value = false;
                },
                observer.OnError,
                observer.OnCompleted);

            return Disposable.Create(() => {
                latchSub.Dispose();
                tickSub.Dispose();
            });
        });
    }
}