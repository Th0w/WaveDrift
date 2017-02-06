using UnityEngine;

public class UIMenuLayer : UILayer
{
    [SerializeField]
    private UILayer optionsLayer, leaderboardLayer;

    public override void Init(UIManager manager) {
        base.Init(manager);

        if (Elements.Count != 4) {
            Debug.LogError("NOT ELEMENTS IN BASE MENU LAYER!");
        }

        var gm = FindObjectOfType<GameManager>();
        Elements[0].Init(true, manager.CloseLayer);
        Elements[1].Init(true, () => OpenTarget(optionsLayer));
        Elements[2].Init(true, () => OpenTarget(leaderboardLayer));
        Elements[3].Init(true, gm.Quit);
    }
}