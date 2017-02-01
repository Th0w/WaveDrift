using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUIElement : UIElement {
    private Action onInteraction;
    private Action onSelection, onDeselection;
    private Vector3 originalScale;

    private bool isSelected;
    public bool IsSelected {
        get { return isSelected; }
        protected set {
            isSelected = value;
            if (isSelected) { onSelection(); }
            else { onDeselection(); }
        }
    }

    public void Init(bool isSelectable, Action onInteraction = null, Action onSelection = null, Action onDeselection = null) {
        IsSelectable = isSelectable;
        this.onInteraction = onInteraction ?? Empty;
        this.onSelection = onSelection ?? Empty;
        this.onDeselection = onDeselection ?? Empty;
        originalScale = GetComponent<RectTransform>().localScale;
    }

    public void Interact() {

        onInteraction();
    }

    private static void Empty() { }
}
