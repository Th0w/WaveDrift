using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private GameObject 
        menu,
        infoPanels,
        gauges;

    internal bool IsMenuOpen { get; private set; }

    internal void ToggleMenu(bool val) {
        IsMenuOpen = val;
        infoPanels.SetActive(!val);
        gauges.SetActive(!val);
        menu.SetActive(val);
    }
}
