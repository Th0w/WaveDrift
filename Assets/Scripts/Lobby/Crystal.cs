using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

public class Crystal : MonoBehaviour {
    private void OnParticleCollision(GameObject other)
    {
        GameManager.Instance.EndLobby();
//        Destroy(gameObject, 0.25f);
		transform.parent.gameObject.SetActive(false);
    }
}
