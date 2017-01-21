using UnityEngine;

public class UI_PlayerScore : MonoBehaviour {

	private Animator selfAnimator;

	void Awake () {

		selfAnimator = GetComponent<Animator> ();
	}

	public void PlayAnim () {

		if (!selfAnimator.GetCurrentAnimatorStateInfo(0).IsName("Anim_UI_Score"))
		selfAnimator.Play ("Anim_UI_Score", 0, 0);
	}
}
