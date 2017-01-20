using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public partial class UI_TextShadow : MonoBehaviour {

	void OnDrawGizmos () {

		if (!selfText || !parentText)
			GetTexts ();

//		if (!selfRT || !parentRT)
//			GetRTs ();

		selfText.text = parentText.text;
	}
}
