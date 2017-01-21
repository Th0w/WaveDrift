using UnityEngine;
using System.Collections;

public class Enemy_Bumper : MonoBehaviour {

	public int frequency;

	private Animator selfAnimator;

	public CircleLineRenderer circleLineRenderer;
	public float circleMaxRadius;
	public float circleLife;

	private LineRenderer lr;

	void Start () {

		selfAnimator = GetComponent<Animator> ();
		lr = circleLineRenderer.gameObject.GetComponent<LineRenderer> ();

		StartCoroutine (Bump ());
	}

	void Update () {

		circleLife += Time.deltaTime;
		circleLineRenderer.radius = circleMaxRadius * circleLife / frequency;

		Color col = new Color (1, 1, 1, 1 - Mathf.InverseLerp(frequency - 2, frequency - 1, circleLife));
		if (circleLife > frequency || circleLife < 0.1f)
			col = new Color (1, 1, 1, 0);
		lr.startColor = col;
		lr.endColor = col;

		foreach (ShipBehaviour_V2 sb in ShipDetector.allShipBehaviours) {

			float playerDist = Vector3.Distance (sb.transform.position, transform.position);
			if (!sb.death && !sb.invulnerability && col.a > 0.5f && playerDist > circleLineRenderer.radius - 1 && playerDist < circleLineRenderer.radius + 1)
				StartCoroutine(sb.Death (sb.deathDelay));
		}
	}

	public IEnumerator Bump () {

		while (true) {

			Color col = new Color (1, 1, 1, 0);

			lr.startColor = col;
			lr.endColor = col;

			yield return new WaitForSeconds (frequency);

			selfAnimator.Play ("Anim_Bumper", 0, 0);
		}
	}

	public void Wave () {

		circleLife = 0;
	}
}
