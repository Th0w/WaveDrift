using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class PlayerWave : MonoBehaviour {

	public float frequency;
	private CircleLineRenderer circleLineRenderer;
	public float circleMaxRadius;
	public float circleLife;
	public float speed;
	[HideInInspector]
	public Color baseCol;

	private LineRenderer lr;
	List<GameObject> ennemis;
	PoolManager pm;
	public int playerID;

	void Start()
	{
		circleLineRenderer = GetComponent<CircleLineRenderer>();
		lr = circleLineRenderer.gameObject.GetComponent<LineRenderer>();
		pm = FindObjectOfType<PoolManager>();
	}

	void Update()
	{

		circleLife += Time.deltaTime;
		//circleLineRenderer.radius = circleMaxRadius * circleLife / frequency;
		circleLineRenderer.radius += Time.deltaTime * speed;

		Color col = new Color(baseCol.r, baseCol.g, baseCol.b, 1 - Mathf.InverseLerp(frequency - 2, frequency - 1, circleLife));
		if (circleLife > frequency || circleLife < 0.1f)
			col = new Color(baseCol.r, baseCol.g, baseCol.b, 0);
		lr.startColor = col;
		lr.endColor = col;

		Physics.OverlapSphere(transform.position, circleLineRenderer.radius + 1)
			.Where(collider => Vector3.Distance(collider.transform.position, transform.position) > circleLineRenderer.radius - 1)
			.ForEach(coll => coll.SendMessage("Kill", playerID, SendMessageOptions.DontRequireReceiver));

		//foreach (ShipBehaviour_V2 sb in ShipDetector.allShipBehaviours)
		//{

		//	float playerDist = Vector3.Distance(sb.transform.position, transform.position);
		//	if (!sb.death && !sb.invulnerability && !sb.airProtection && col.a > 0.5f && playerDist > circleLineRenderer.radius - 1 && playerDist < circleLineRenderer.radius + 1)
		//		sb.Death();
		//}


		if (circleLineRenderer.radius > circleMaxRadius)
			DestroyObject(gameObject);
	}

}
