using System;
using UnityEngine;

public class UIMenuLayer : UILayer
{
    [SerializeField]
    private UILayer optionsLayer, leaderboardLayer;

    private GameManager gameManager;

    public override void Init(UIManager manager) {
        base.Init(manager);

        gameManager = FindObjectOfType<GameManager>();
        InitElement("Resume", true, manager.CloseLayer);
        InitElement("GameAction", false, () => { });
        InitElement("Options", true, () => OpenTarget(optionsLayer));
        InitElement("Leaderboard", false, () => OpenTarget(leaderboardLayer));
        InitElement("Exit", true, gameManager.Quit);
    }

    private void InitElement(string name, bool isSelectable,
                             Action onInteraction = null,
                             Action<InteractableUIElement> onSelection = null,
                             Action<InteractableUIElement> onDeselection = null) {

        var elem = Elements.Find(name);
        if (elem == null) {
#if UNITY_EDITOR
            Debug.LogErrorFormat("Missing element {0}.", name);
            UnityEditor.EditorApplication.isPlaying = false;
#else
            return;
#endif
        } else {
            elem.Init(isSelectable, onInteraction, onSelection, onDeselection);
        }
    }

    protected override void OnEnable_() {
        base.OnEnable_();
        if (gameManager.IsInGame) {
            //
        } else {
            //
        }
    }
}