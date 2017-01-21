using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class Panel_NitroGauge : MonoBehaviour {

	#if UNITY_EDITOR
	void OnDrawGizmos () {

		if (!selfRT)
			GetRT ();

		if (!mainCanvasScaler)
			mainCanvasScaler = mainCanvas.gameObject.GetComponent<CanvasScaler> ();
		
		if (!Application.isPlaying)
			FollowPlayer (mainCanvasScaler.referenceResolution.x * mainCanvas.localScale.x, mainCanvasScaler.referenceResolution.y * mainCanvas.localScale.y);
	}
	#endif
}
