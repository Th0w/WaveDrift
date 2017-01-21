using UnityEngine;
using UnityEngine.UI;

public partial class Panel_NitroGauge : MonoBehaviour {

	public Transform player;
	public Camera mainCamera;

	public RectTransform mainCanvas;
	private CanvasScaler mainCanvasScaler;
	public Image fillImage;

	public Vector2 offset;

	private RectTransform selfRT;

	void Start () {

		GetRT ();

		if (!player.gameObject.activeSelf)
			gameObject.SetActive (false);
	}

	void Update () {

		FollowPlayer (Screen.width, Screen.height);
	}

	void FollowPlayer (float w, float h) {

		Vector2 pos = mainCamera.WorldToViewportPoint (player.position);
		pos.x *= w / mainCanvas.localScale.x;
		pos.y *= h / mainCanvas.localScale.y;
		selfRT.anchoredPosition = pos + offset;
	}

	public void GetRT () {

		selfRT = GetComponent<RectTransform> ();
	}
}
