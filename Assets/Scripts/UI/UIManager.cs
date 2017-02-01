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
    private InteractableUIElement[] menuElements;
    private InteractableUIElement[] optionsElements;
    private InteractableUIElement[] currentlySelectedElements;

    private InteractableUIElement currentlySelectedElement;

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

    #region Methods

    #region MonoBehaviours
    private void Awake() {

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
        gameManager = FindObjectOfType<GameManager>();

        onUp.Subscribe(_ => {
            currentlySelectedIndex--;
            SelectElement(currentlySelectedIndex);
        }).AddTo(this);
        onDown.Subscribe(_ => {
            currentlySelectedIndex++;
            SelectElement(currentlySelectedIndex);
        }).AddTo(this);

        onConfirm.Subscribe(_ => { currentlySelectedElement.Interact(); })
            .AddTo(this);

        onCancel.Subscribe(_ => { gameManager.ToggleMenu(selectedPlayerIndex); })
            .AddTo(this);

        if (menuElements.Length != 4) {
            Debug.LogError("Too much elements in menuElements");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        } else {
            menuElements[0].Init(true, () => gameManager.ToggleMenu(selectedPlayerIndex));
            menuElements[1].Init(true, () => { Debug.LogWarning("Will be options. Soon\u2122"); });
            menuElements[2].Init(true, () => { Debug.LogWarning("Will be Leaderboard. Soon\u2122"); });
            menuElements[3].Init(true, () => { gameManager.Quit(); });
            currentlySelectedElements = menuElements;
            SelectElement(0);
        }
    }

#endregion MonoBehaviours
    
    private void SelectElement(int id) {
        if (currentlySelectedElement != null) {
            currentlySelectedElement.IsSelected = false;
        }
        currentlySelectedIndex = Mathf.Clamp(id, 0, currentlySelectedElements.Length - 1);
        currentlySelectedElement = currentlySelectedElements[currentlySelectedIndex];
        currentlySelectedElement.IsSelected = true;
    }

    public static IObservable<bool> Latch(
        IObservable<Unit> tick,
        IObservable<Unit> latchTrue,
        bool initialValue)
    {
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
    
    internal bool IsMenuOpen { get; private set; }

    internal void ToggleMenu(bool val, int playerID) {
        Debug.LogWarning("Toggling menu.");
        if (val) { selectedPlayerIndex = playerID; }

        IsMenuOpen = val;
        gameManager.PlayersData[playerID].behaviour.SwitchLayout();
        
        infoPanels.SetActive(!val);
        gauges.SetActive(!val);
        menu.SetActive(val);
        
        //if (val) {

        //    onCancelDisposable = onCancel
        //        .Subscribe(_ => ToggleMenu(false, playerID))
        //        .AddTo(this);

        //} else {
        if (val == false) { 
            Clear(onCancelDisposable);
            
            selectedPlayerIndex = -1;
        }
    }

    private void Clear(IDisposable toDispose) {
        if (toDispose == null) { return; }

        toDispose.Dispose();
        toDispose = null;
    }

#endregion Methods
}
