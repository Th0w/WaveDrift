using UnityEngine;

public class SelfDestroy : MonoBehaviour {

	public float delay = 2;

	void Start () {

		Destroy (gameObject, delay);
	}
}
