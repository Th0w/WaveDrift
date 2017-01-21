using UnityEngine;
using UnityEngine.UI;

public class Canvas3D_Cursor : MonoBehaviour {

	public Transform player;
	public Vector3 offset;
	public Transform shipBody;

	private RectTransform selfRT;
	private Image selfImage;

	void Start () {

		selfRT = GetComponent<RectTransform> ();
		selfImage = transform.GetChild(0).GetComponent<Image> ();
	}
	
	void Update () {

		transform.position = player.position + offset;

		float heightLerp = Mathf.InverseLerp (0, 30, shipBody.localPosition.y);
		selfImage.color = new Color(1, 1, 1, heightLerp);
		selfRT.localScale = Vector3.one * ((1 - heightLerp) + 0.5f) / 1.5f;

	}
}
