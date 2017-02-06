using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIOptionsLayer : UILayer
{
    [Header("Music UI Elements")]
    [SerializeField]
    private Slider musicSlider;
    private InteractableUIElement currentlySelected;
    private AudioManager audioManager;
    [SerializeField]
    private InteractableUIElement musicElement;
    [SerializeField]
    private Text musicValueText;

    public override void Init(UIManager manager) {
        base.Init(manager);

        var gm = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();

        musicSlider.value = 100;

        musicElement = Elements.Find("Music");

        musicElement.Init(true,
            onSelection: e => currentlySelected = e,
            onDeselection: e => currentlySelected = null);

        Elements.Find("Exit").Init(true, manager.CloseLayer, e => currentlySelected = null);

        var onLeft = manager.OnLeft.Where(_ => IsEnabled && IsFocused);
        onLeft.Where(_ => currentlySelected == musicElement)
            .Subscribe(_ => ChangeVolume(-5.0f))
            .AddTo(this);

        var onRight = manager.OnRight.Where(_ => IsEnabled && IsFocused);
        onRight.Where(_ => currentlySelected == musicElement)
            .Subscribe(_ => ChangeVolume(+5.0f))
            .AddTo(this);
    }

    private void ChangeVolume(float volumeChange) {
        audioManager.ChangeVolume(audioManager.Volume + volumeChange);
        UpdateMusicSlider();
    }

    private void UpdateMusicSlider() {
        musicSlider.value = audioManager.Volume;
        if (musicValueText != null) {
            musicValueText.text = musicSlider.value.ToString() + "%";
        }
    }
}