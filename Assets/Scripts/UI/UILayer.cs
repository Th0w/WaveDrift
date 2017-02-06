using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class UILayer : MonoBehaviour
{
    struct GraphTuple {
        public Graphic graph;
        public float defaultAlpha;
    }

    protected List<IDisposable> disposables;

    [SerializeField]
    private List<InteractableUIElement> elements;

    protected List<InteractableUIElement> Elements { get { return elements; } }
    protected InteractableUIElement element = null;
    private int selectedIndex = -1;
    private UIManager manager;
    private bool isFocused;
    private List<GraphTuple> graphics;

    public bool IsFocused {
        get { return isFocused; }
        private set {
            isFocused = value;
            if (isFocused) { OnFocus(); } 
            else { OnUnfocus(); }
        }
    }
    public bool IsEnabled { get; private set; }
    
    public virtual void Init(UIManager manager) {
        this.manager = manager;

        graphics = new List<GraphTuple>();

        Elements.ForEach(elem => {
            Image img = elem.GetComponent<Image>();
            Text txt = elem.GetComponent<Text>();
            if (img != null) { graphics.Add(new GraphTuple { graph = img, defaultAlpha=img.color.a }); }
            if (txt != null) { graphics.Add(new GraphTuple { graph = txt, defaultAlpha = txt.color.a }); }
        });

        disposables = new List<IDisposable>();
        disposables.Add(manager.OnUp
            .Where(_ => IsEnabled && IsFocused)
            .Subscribe(_ => SelectElement(--selectedIndex)));
        disposables.Add(manager.OnDown
            .Where(_ => IsEnabled && IsFocused)
            .Subscribe(_ => SelectElement(++selectedIndex)));
        disposables.Add(manager.OnConfirm
            .Where(_ => IsEnabled && IsFocused)
            .Where(_ => element != null)
            .Subscribe(_ => element.Interact()));
        disposables.Add(manager.OnCancel
            .Where(_ => IsEnabled && IsFocused)
            .Subscribe(_ => OnCancel()));
    }

    private void OnDestroy() {
        disposables.ForEach(disposable => disposable.Dispose());
        disposables.Clear();
        disposables = null;
    }
    
    protected void OpenTarget<T>(T target) where T : UILayer {
        if (target == null) { return; }
        OpenTarget<T>();
    }

    protected void OpenTarget<T>() where T : UILayer {
        Debug.LogFormat("Target type: {0}", typeof(T).ToString());
        IsFocused = false;
        manager.OpenLayer<T>();
    }

    private void OnCancel() {
        manager.CloseLayer();
    }

    public void Enable() {
        gameObject.SetActive(true);
        IsFocused = true;
        OnEnable_();
    }

    public void Disable() {
        gameObject.SetActive(false);
        IsFocused = false;
        OnDisable_();
    }

    protected virtual void OnEnable_() {
        IsEnabled = true;
        SelectElement(0);
    }

    protected virtual void OnDisable_() {
        IsEnabled = false;
        SelectElement(-2);
    }

    public void Focus() {
        IsFocused = true;
    }

    private void OnFocus() {
        graphics.ForEach(graphTup => {
            if (graphTup.graph.color.a != graphTup.defaultAlpha) {
                Color col = graphTup.graph.color;
                col.a = graphTup.defaultAlpha;
                graphTup.graph.color = col;
            }
        });
    }

    private void OnUnfocus() {
        graphics.ForEach(graphTup => {
            Color col = graphTup.graph.color;
            col.a *= manager.LayerUnfocusedOpacity;
            graphTup.graph.color = col;
        });
    }

    // TODO: To refactor.
    private void SelectElement(int id) {
        if (id == -2) {
            selectedIndex = id;
            if (element != null) {
                element.IsSelected = false;
                element = null;
            }
            return;
        }

        if (element != null) {
            element.IsSelected = false;
        }
        selectedIndex = Mathf.Clamp(id, 0, elements.Count - 1);

        element = elements[selectedIndex];
        element.IsSelected = true;
    }
}
