using UnityEngine;
using UnityEngine.UI;

public class UI_DeathOL : MonoBehaviour {

	public Camera renderCamera;

	private Animator selfAnimator;
	private Image selfImage;

	void Start () {

		selfAnimator = GetComponent<Animator> ();
		selfImage = GetComponent<Image> ();
	}

	public void RenderDeathOL () {

		renderCamera.Render ();
		selfAnimator.Play ("Anim_UI_DeathOL", 0, 0);
	}
}
