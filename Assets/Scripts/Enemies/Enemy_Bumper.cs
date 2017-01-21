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
		Color col = new Color (1, 1, 1, 1 - Mathf.InverseLerp(frequency - 1, frequency, circleLife));
		lr.startColor = col;
		lr.endColor = col;
	}

	public IEnumerator Bump () {

		while (true) {
			yield return new WaitForSeconds (frequency);
			selfAnimator.Play ("Anim_Bumper", 0, 0);
		}
	}

	public void Wave () {

		circleLife = 0;
	}
}
