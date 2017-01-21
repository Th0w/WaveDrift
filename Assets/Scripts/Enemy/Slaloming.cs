using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slaloming : SimpleMover {
    private float progress;

    protected override void MoveFunction(Vector3 distance)
    {
        progress += Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(distance);
        Debug.LogFormat("Current cos: {0}", Mathf.Cos(progress));
        transform.position += transform.forward * Time.deltaTime * speed + transform.right * Mathf.Cos(progress) / Mathf.PI;
    }
}
