using UnityEngine;
using UniRx;

public class Crystal : MonoBehaviour {
    private GameManager gameManager;
    private void Awake() {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start() {
        gameManager.OnGameEnd
            .Subscribe(_ => transform.parent.gameObject.SetActive(true));
    }
    private void OnParticleCollision(GameObject other)
    {
        FindObjectOfType<GameManager>().EndLobby();
		transform.parent.gameObject.SetActive(false);
    }
}
