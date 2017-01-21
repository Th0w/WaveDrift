using UnityEngine;

public class ShipDetector : MonoBehaviour {
    private static Transform defaultTransform;

	public static ShipBehaviour_V2[] allShipBehaviours;

    public static Transform DefaultTransform
    {
        get
        {
            if (defaultTransform == null)
            {
                var eb = FindObjectOfType<Enemy_Bumper>();
                defaultTransform = eb != null ? eb.transform : null;
            }
            return defaultTransform;
        }
    }

	void Awake () {

		allShipBehaviours = FindObjectsOfType<ShipBehaviour_V2> ();
	}
}
