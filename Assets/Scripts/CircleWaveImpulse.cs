using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CircleWaveImpulse : MonoBehaviour {

	public int minSubdivisions;
	public int maxSubdivisions;
	private int subdivisions;
	public float maxRadius;
	public float speed;
	public float speedSmooth;
	public float cooldown;
	public float startDelay;
	private bool recharge;
	public bool killOnEnd;

	Vector3[] directions;
	public float radius;

	LineRenderer lr;
	LayerMask killLayer;
	ShipBehavior[] players;


	// Use this for initialization
	void Start () {
		lr = GetComponent<LineRenderer>();
		players = FindObjectsOfType<ShipBehavior>() as ShipBehavior[];
		lr.numPositions = minSubdivisions;
		directions = new Vector3[minSubdivisions];
		radius = 0f;
		recharge = true;
		lr.enabled = false;
		StartCoroutine(EmitCooldown(startDelay));
	}
	
	// Update is called once per frame
	void Update () {

		if (!recharge)
		{
			float radiusRatio = radius / maxRadius * (maxSubdivisions - minSubdivisions) + minSubdivisions;
			subdivisions = (int)radiusRatio;
			lr.numPositions = subdivisions;
			for (int i = 0; i < subdivisions; i++)
			{
				lr.SetPosition(i, Quaternion.Euler(0f, 360f / (lr.numPositions - 1f) * i, 0f) * Vector3.forward * radius);
			}
			radius = Mathf.Lerp(radius, radius + speed, Time.deltaTime * speedSmooth);

			if (radius > maxRadius)
			{
				if (killOnEnd)
					Destroy(gameObject);
				else
				{
					recharge = true;
					lr.enabled = false;
					StartCoroutine(EmitCooldown(cooldown));
					//Observable.Timer(TimeSpan.FromSeconds(cooldown))
					//	.Subscribe(_ =>
					//	{
					//	}).AddTo(this);
				}
			}
		}
	}
	
	IEnumerator EmitCooldown(float t)
	{
		yield return new WaitForSeconds(t);
		radius = 0f;
		recharge = false;
		yield return null;
		lr.enabled = true;
	}
}
