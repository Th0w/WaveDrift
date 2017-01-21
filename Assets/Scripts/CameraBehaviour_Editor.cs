using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public partial class CameraBehaviour : MonoBehaviour {

	void OnDrawGizmos () {

		if (!Application.isPlaying) {
			
			GetPlayers ();
			CalculateBarycenter ();

			transform.position = barycenter;

			CalculateZoom ();
		}

		DrawPlayersFrame ();

		Handles.color = Color.yellow;
		DrawHandleframe (maxFrame);
		Gizmos.color = Color.yellow;
		DrawGizmoframe (minFrame);

		Handles.color = Color.red;
		DrawHandleframe (playersMaxFrame);
		Gizmos.color = Color.red;
		DrawGizmoframe (playersMinFrame);
	}

	void DrawGizmoframe (Vector2 frame) {

		Gizmos.DrawLine (transform.position - Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2, transform.position - Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2);
		Gizmos.DrawLine (transform.position - Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2, transform.position + Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2);
		Gizmos.DrawLine (transform.position + Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2, transform.position + Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2);
		Gizmos.DrawLine (transform.position + Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2, transform.position - Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2);
	}

	void DrawHandleframe (Vector2 frame) {

		Handles.DrawDottedLine (transform.position - Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2, transform.position - Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2, 10);
		Handles.DrawDottedLine (transform.position - Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2, transform.position + Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2, 10);
		Handles.DrawDottedLine (transform.position + Vector3.right * frame.x / 2 + Vector3.forward * frame.y / 2, transform.position + Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2, 10);
		Handles.DrawDottedLine (transform.position + Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2, transform.position - Vector3.right * frame.x / 2 - Vector3.forward * frame.y / 2, 10);
	}

	void DrawPlayersFrame () {

		Handles.color = Color.red;

		if (players.Length > 2) {
			for (int i = 0; i < players.Length; i++) {

				if (i < players.Length - 1)
					Handles.DrawDottedLine (players [i].transform.position, players [i + 1].transform.position, 10);
				else
					Handles.DrawDottedLine (players [i].transform.position, players [0].transform.position, 10);
			}
		} else if (players.Length == 2)
			Handles.DrawDottedLine (players [0].transform.position, players [1].transform.position, 10);

		Gizmos.color = Color.red;

		Gizmos.DrawSphere (barycenter, 1);
	}
}
#endif
