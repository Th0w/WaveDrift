using UnityEngine;

public class AnimatedMaterialOffset : MonoBehaviour {

	private Renderer rend;
	public Vector2 offset;

	void Start () {

		rend = GetComponent<Renderer> ();
	}

	void Update () {

		rend.material.SetTextureOffset ("_MainTex", offset * Time.timeSinceLevelLoad);
	}
}
