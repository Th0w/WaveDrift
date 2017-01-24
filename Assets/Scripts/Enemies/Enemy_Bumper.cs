using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;

public class Enemy_Bumper : MonoBehaviour {

	public int frequency;

	private Animator selfAnimator;

	public CircleLineRenderer circleLineRenderer;
	public float circleMaxRadius;
	public float circleLife;

	private LineRenderer lr;

    private GameManager gameManager;
    private ShipBehaviour_V2[] activePlayers;

    void Start() {

        selfAnimator = GetComponent<Animator>();
        lr = circleLineRenderer.gameObject.GetComponent<LineRenderer>();
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnGameBegin.Subscribe(_ => {
            activePlayers = gameManager.ActivePlayers;
            gameObject.SetActive(true);
            StartCoroutine(Bump());
        });

        gameManager.OnGameEnd.Subscribe(_ => {
            StopAllCoroutines();
            gameObject.SetActive(false);
        });

        gameObject.SetActive(false);
    }

    private void OnDisable() {
    }

    void Update() {

        circleLife += Time.deltaTime;
        circleLineRenderer.radius = circleMaxRadius * circleLife / frequency;

        Color col = new Color(1, 1, 1, 1 - Mathf.InverseLerp(frequency - 2, frequency - 1, circleLife));
        if (circleLife > frequency || circleLife < 0.1f)
            col = new Color(1, 1, 1, 0);
        lr.startColor = col;
        lr.endColor = col;

        if (col.a <= 0.5f) {
            return;
        }

        activePlayers
            .Where(player => (player.death || player.invulnerability || player.airProtection) == false)
            .Where(player => {
                float playerDist = Vector3.Distance(player.transform.position, transform.position);
                return playerDist > circleLineRenderer.radius - 1.0f && playerDist < circleLineRenderer.radius + 1.0f;
            })
            .ForEach(player => player.Death());
    }

	public IEnumerator Bump () {
		while (true) {

			Color col = new Color (1, 1, 1, 0);

			lr.startColor = col;
			lr.endColor = col;

			yield return new WaitForSeconds (frequency);

			selfAnimator.Play ("Anim_Bumper", 0, 0);
		}
	}

	public void Wave () {

		circleLife = 0;
	}
}
