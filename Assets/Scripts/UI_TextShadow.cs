using UnityEngine;
using UnityEngine.UI;

public partial class UI_TextShadow : MonoBehaviour {
    private Canvas canvas;
    private void OnEnable()
    {
        Debug.LogWarningFormat("{0} (re-)enabled!", GetType());
        (selfRT = selfRT ?? GetComponent<RectTransform>()).anchoredPosition = selfRT.anchoredPosition + Vector2.one * float.Epsilon;
        (canvas = canvas ?? GetComponent<Canvas>()).sortingOrder = -1;
    }

    private Text selfText;
    private Text parentText;

    private RectTransform selfRT;
    private RectTransform parentRT;

    //void Start () {

    //	GetTexts ();

    //	GetComponent<Canvas> ().sortingOrder = -1;
    //}

    //void Update () {

    //	selfText.text = parentText.text;
    //}

    void GetTexts()
    {

        if (!selfText)
            selfText = GetComponent<Text>();
        if (!parentText)
            parentText = transform.parent.GetComponent<Text>();
    }

    //	void GetRTs () {
    //
    //		if (!selfRT)
    //			selfRT = GetComponent<RectTransform> ();
    //		if (!parentRT)
    //			parentRT = GetComponent<RectTransform> ();
    //	}
}
