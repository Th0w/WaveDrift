using UnityEngine;
using UnityEngine.UI;

public partial class UI_TextShadow : MonoBehaviour {

	private Text selfText;
	private Text parentText;

	private RectTransform selfRT;
	private RectTransform parentRT;

	void Start () {

		GetTexts ();
	}

	void Update () {

		selfText.text = parentText.text;
	}

	void GetTexts () {

		if (!selfText)
			selfText = GetComponent<Text> ();
		if (!parentText)
			parentText = transform.parent.GetComponent<Text> ();
	}

//	void GetRTs () {
//
//		if (!selfRT)
//			selfRT = GetComponent<RectTransform> ();
//		if (!parentRT)
//			parentRT = GetComponent<RectTransform> ();
//	}
}
