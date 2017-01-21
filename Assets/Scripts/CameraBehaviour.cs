using UnityEngine;

public partial class CameraBehaviour : MonoBehaviour {

	public Camera mainCamera;

	public ShipBehaviour_V2[] players;
	public Vector3 barycenter;

	public Vector2 maxFrame;
	public Vector2 minFrame;

	public Vector2 playersMaxFrame;
	public Vector2 playersMinFrame;

	public float posLerpRate;
	public float zoomLerpRate;

	[Range(0f, 1f)]
	public float zoom = 0.5f;

	void Start () {

		GetPlayers ();
	}

	void Update () {

		CalculateBarycenter ();

		transform.position = Vector3.Lerp(transform.position, barycenter, posLerpRate * Time.deltaTime);

		CalculateZoom ();
	}

	public void GetPlayers () {

		players = FindObjectsOfType<ShipBehaviour_V2> () as ShipBehaviour_V2[];
	}

	public void CalculateBarycenter () {

		barycenter = Vector3.zero;

		for (int i = 0; i < players.Length; i++)
			barycenter += players [i].transform.position;

		barycenter /= players.Length;
	}

	public void CalculateZoom () {

		Vector2 playersDistance = Vector2.zero;

		foreach (ShipBehaviour_V2 sb in players) {

			float distX = Mathf.Abs(sb.transform.position.x - transform.position.x);
			if (distX > playersDistance.x)
				playersDistance.x = distX;

			float distZ = Mathf.Abs(sb.transform.position.z - transform.position.z);
			if (distZ > playersDistance.y)
				playersDistance.y = distZ;
		}

		float xDist = Mathf.InverseLerp (playersMaxFrame.x / 2, playersMinFrame.x / 2, playersDistance.x);
		float zDist = Mathf.InverseLerp (playersMaxFrame.y / 2, playersMinFrame.y / 2, playersDistance.y);
		if (xDist > zDist)
			zoom = xDist;
		else
			zoom = zDist;

		float zoomZPosMax = maxFrame.y * 0.5f / Mathf.Tan (mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float zoomZPosMin = minFrame.y * 0.5f / Mathf.Tan (mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float targetZoom = Mathf.Lerp (zoomZPosMax, zoomZPosMin, zoom);

		mainCamera.transform.localPosition = new Vector3 (0, Mathf.Lerp(mainCamera.transform.localPosition.y, targetZoom, zoomLerpRate * Time.deltaTime), 0);
	}
}
