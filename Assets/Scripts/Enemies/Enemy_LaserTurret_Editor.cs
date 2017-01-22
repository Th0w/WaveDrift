using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public partial class Enemy_LaserTurret : MonoBehaviour {

	void OnDrawGizmos () {

		if (keepGizmos)
			Draw ();
	}

	void OnDrawGizmosSelected () {

		if (!keepGizmos)
			Draw ();
	}

	void Draw () {

		Handles.color = new Color (1, 0, 1, 0.1f);
		Handles.DrawSolidArc (transform.position + Vector3.up, transform.up, transform.forward, 360, aimDistance);

		Handles.color = Color.black;
		Handles.DrawWireArc (transform.position + Vector3.up, transform.up, transform.forward, 360, aimDistance);
	}
}
#endif
