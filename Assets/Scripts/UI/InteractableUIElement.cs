using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUIElement : UIElement {
    private Action onInteraction;
    private Action onSelection, onDeselection;
    private Vector3 originalScale;

    [SerializeField]
    private float selectedScaleFactor;

    private bool isSelected;
    public bool IsSelected {
        get { return isSelected; }
        set {
            isSelected = value;
            if (isSelected) { onSelection(); }
            else { onDeselection(); }
        }
    }

    public void Init(bool isSelectable, Action onInteraction = null, Action onSelection = null, Action onDeselection = null) {
        IsSelectable = isSelectable;
        this.onInteraction = onInteraction ?? Empty;
        this.onSelection = OnSelection;
        if (onSelection != null) { this.onSelection += onSelection; }
        this.onDeselection = OnDeselection;
        if (onDeselection != null) { this.onDeselection += onDeselection; }
        originalScale = GetComponent<RectTransform>().localScale;
    }

    private void OnSelection() {
        transform.localScale = originalScale * selectedScaleFactor;
    }

    private void OnDeselection() {
        transform.localScale = originalScale;
    }

    public void Interact() {

        onInteraction();
    }

    private static void Empty() { }
}
