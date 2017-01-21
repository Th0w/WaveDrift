using UnityEngine;
using UnityEngine.UI;

public class Panel_NitroGauge : MonoBehaviour {

	public Transform player;
	public Camera mainCamera;

	public RectTransform mainCanvas;
	public Image fillImage;

	public Vector2 offset;

	private RectTransform selfRT;

	void Start () {

		selfRT = GetComponent<RectTransform> ();

		if (!player.gameObject.activeSelf)
			gameObject.SetActive (false);
	}

	void Update () {

		Vector2 pos = mainCamera.WorldToViewportPoint (player.position);
		pos.x *= Screen.width / mainCanvas.localScale.x;
		pos.y *= Screen.height / mainCanvas.localScale.y;
		selfRT.anchoredPosition = pos + offset;
	}
}
