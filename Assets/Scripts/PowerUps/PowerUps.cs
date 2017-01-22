using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PowerUps : MonoBehaviour {

	public enum powerUp { unlimitedDrift,  waveEmit, shield}
	public powerUp type;

	public bool random;
	public float unlimitedDriftTime;
	public float waveEmitTime;
	public float delayBetweenWaves;
	public GameObject wavePrefab;
	public float shieldTime;
	Coroutine cor;


	bool taken;
	ShipBehaviour_V2 player;

	// Use this for initialization
	void Start () {
		if (random)
		{
			float random = Random.Range(0.0f, System.Enum.GetValues(typeof(powerUp)).Length);
			type = (powerUp)random;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && !taken)
		{
			HidePowerUp();
			taken = true;
			player = other.transform.parent.gameObject.GetComponent<ShipBehaviour_V2>();
			switch (type)
			{
				case powerUp.unlimitedDrift:
					UnlimitedDrift();
					break;
				case powerUp.waveEmit:
					cor = StartCoroutine(WaveEmit());
					break;
				case powerUp.shield:
					Shield();
					break;
			}
		}
	}

	private void HidePowerUp()
	{
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<SphereCollider>().enabled = false;
	}

	private void UnlimitedDrift()
	{
		player.driftLossRatio = -1f;
		Observable.Timer(System.TimeSpan.FromSeconds(unlimitedDriftTime))
			.Subscribe(_ =>
			{
				player.driftLossRatio = 1f;
			}).AddTo(this);
	}


	IEnumerator WaveEmit()
	{
		float t = (waveEmitTime / delayBetweenWaves);
		for (int i=0; i<t; i++)
		{
			yield return new WaitForSeconds(delayBetweenWaves);
			if (player.death)
				yield break;
			GameObject go = Instantiate(wavePrefab, player.transform.position, Quaternion.identity);
			go.GetComponent<PlayerWave>().baseCol = player.powerUpWaveEmitColor;
			go.GetComponent<PlayerWave>().playerID = player.playerID;
		}
	}

	IEnumerator Shield()
	{
		player.shield = true;
		yield return new WaitForSeconds(shieldTime);
		player.shield = false;
	}
}
