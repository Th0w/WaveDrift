using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CircleLineRenderer : MonoBehaviour {

	public float radius;

	public LineRenderer lr;

	void Update () {

		if (!lr)
			GetLR ();
		else {

			for (int i = 0; i < lr.numPositions; i++)
				lr.SetPosition (i, transform.position + Quaternion.Euler(0, 360f / (lr.numPositions - 1) * i, 0) * Vector3.forward * radius);
		}
	}

	public void GetLR () {

		lr = GetComponent<LineRenderer> ();
	}
}
