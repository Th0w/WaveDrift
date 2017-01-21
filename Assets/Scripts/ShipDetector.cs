using UnityEngine;

public class ShipDetector : MonoBehaviour {

	public static ShipBehaviour_V2[] allShipBehaviours;

	void Awake () {

		allShipBehaviours = FindObjectsOfType<ShipBehaviour_V2> ();
	}
}
