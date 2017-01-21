using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUB : MonoBehaviour {

    private Enemy_Bumper[] bumpers;
    private Enemy_LaserTurret[] lasers;

	// Use this for initialization
	void Start () {
        bumpers = FindObjectsOfType<Enemy_Bumper>();
        lasers = FindObjectsOfType<Enemy_LaserTurret>();
	}
}
