using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUIElement : MonoBehaviour
{
    private bool isSelectable;
    private Color unselectableColor;
    private Action onInteraction;
    private Action<InteractableUIElement> onSelection, onDeselection;
    private Vector3 originalScale;

    [SerializeField]
    private float selectedScaleFactor;

    private bool isSelected;
    public bool IsSelected {
        get { return isSelected; }
        set {
            isSelected = value;
            if (isSelected) { onSelection(this); }
            else { onDeselection(this); }
        }
    }
    public Text text { get; private set; }
    public bool IsSelectable {
        get { return isSelectable; }
        protected set {
            isSelectable = value;
            ToggleIsSelectable(isSelectable);
        }
    }

    public int ID { get; private set; }

    public void Init(bool isSelectable, 
                     Action onInteraction = null, 
                     Action<InteractableUIElement> onSelection = null, 
                     Action<InteractableUIElement> onDeselection = null) {
        
        text = GetComponentInChildren<Text>();
        unselectableColor = Color.grey;
        IsSelectable = isSelectable;
        this.onInteraction = onInteraction ?? Empty;
        this.onSelection = OnSelection;
        if (onSelection != null) { this.onSelection += onSelection; }
        this.onDeselection = OnDeselection;
        if (onDeselection != null) { this.onDeselection += onDeselection; }
        originalScale = GetComponent<RectTransform>().localScale;
    }

    public void SetId(int id) {
        ID = id;
    }
    private void OnSelection(InteractableUIElement element) {
        transform.localScale = originalScale * selectedScaleFactor;
    }

    private void OnDeselection(InteractableUIElement element) {
        transform.localScale = originalScale;
    }

    public void Interact() {

        onInteraction();
    }
    protected virtual void ToggleIsSelectable(bool selectable) {
        Color c = selectable ? Color.white : Color.grey;
        c.a = text.color.a;
        text.color = c;
    }

    private static void Empty() { }
}
