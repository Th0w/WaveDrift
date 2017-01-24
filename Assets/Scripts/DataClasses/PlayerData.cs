using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public ShipBehaviour_V2 behaviour;
    public GameObject infoPanel;
    public GameObject nitroGauge;
    public GameObject cursor3d;
    public GameObject barrier;
    public UI_TextShadow[] shadows;

    public void SetActive(bool val) {
        behaviour.IsFrozen = !val;
        infoPanel.SetActive(val);
        nitroGauge.SetActive(val);
        cursor3d.SetActive(val);
        barrier.SetActive(val);
        shadows.ForEach(shadow => {
            shadow.gameObject.SetActive(false);
            shadow.enabled = true;
            shadow.gameObject.SetActive(true);
        });

        UnityEngine.Object.FindObjectOfType<CameraBehaviour>().GetPlayers();
    }
}