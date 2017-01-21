using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class SlowMo : MonoBehaviour {

	public static Animator selfAnimator;
	public float ts;

	public Fisheye fish;
	public float fishStrength;

	void Awake () {

		selfAnimator = GetComponent<Animator> ();
	}

	void Update () {

		Time.timeScale = ts;

		fish.strengthX = fishStrength;
		fish.strengthY = fishStrength;
	}
}
