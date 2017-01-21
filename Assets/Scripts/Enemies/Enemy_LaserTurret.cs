using UnityEngine;

public partial class Enemy_LaserTurret : MonoBehaviour {

	public Transform head;
	public float aimDistance = 16;

	public ShipBehavior[] players;
	public Transform target;

	public LineRenderer LR;
	public ParticleSystem laserPS;

	public bool keepGizmos;

	void Start () {

		players = FindObjectsOfType<ShipBehavior> () as ShipBehavior[];
	}
	
	void Update () {

		float dist = aimDistance;
		for (int i = 0; i < players.Length; i++) {

			float playerDist = Vector3.Distance (transform.position, players [i].transform.position);
			if (playerDist < dist) {
				dist = playerDist;
				target = players[i].transform;
			}
		}

		if (head) {
			if (dist != aimDistance) {

				head.rotation = Quaternion.Slerp(head.rotation, Quaternion.FromToRotation (Vector3.forward, target.position - head.position), 2 * Time.deltaTime);

				RaycastHit groundHit;
				if (Physics.Raycast(head.position + head.forward * 1.5f, head.transform.forward, out groundHit, aimDistance + 4)) {

					LR.SetPosition(0, head.position + head.transform.forward * 1.5f);
					LR.SetPosition(1, groundHit.point);

					laserPS.emissionRate = 128;
					laserPS.transform.position = groundHit.point;
				}

			} else {
				
				target = null;

				head.rotation = Quaternion.Slerp(head.rotation, Quaternion.Euler(90, 0, 0), 2 * Time.deltaTime);

				LR.SetPosition(0, Vector3.down * 8);
				LR.SetPosition(1, Vector3.down * 8);

				laserPS.emissionRate = 0;
			}
		}
	}
}
