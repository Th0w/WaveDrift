using UnityEngine;

public partial class Enemy_LaserTurret : MonoBehaviour {

	public Transform head;
	public float aimDistance = 16;

	public ShipBehaviour_V2[] players;
	public ShipBehaviour_V2 target;

	public LineRenderer LR;
	public ParticleSystem laserPS;

	public LayerMask raycastMask;

	public bool keepGizmos;

	void OnEnable () {

		players = FindObjectsOfType<ShipBehaviour_V2> () as ShipBehaviour_V2[];
	}
	
	void Update () {

		float dist = aimDistance;
		for (int i = 0; i < players.Length; i++) {

			if (!players [i].death) {
				float playerDist = Vector3.Distance (transform.position, players [i].transform.position);
				if (playerDist < dist) {
					dist = playerDist;
					target = players [i];
				}
			}
		}

		if (head) {
			if (dist != aimDistance) {

				head.rotation = Quaternion.Slerp(head.rotation, Quaternion.FromToRotation (Vector3.forward, target.transform.position - head.position), 2 * Time.deltaTime);

				RaycastHit groundHit;
				if (Physics.Raycast(head.position + head.forward * 1.5f, head.transform.forward, out groundHit, aimDistance + 4, raycastMask)) {
                    if (groundHit.collider.gameObject == target.gameObject)
                    {
                        if ((target.invulnerability || target.airProtection) == false)
                        {
                            target.Death();
                        }
                        else
                        {
                            Debug.Log("TARGET IS IMMUNE");
                        }
                    }
						
					
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
