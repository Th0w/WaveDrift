using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slaloming : SimpleMover {
    private float progress;
    [SerializeField]
    private Transform childObject;

    private BoxCollider bc;

    [SerializeField]
    private float slalomAmplitude;

    protected override void MoveFunction(Vector3 distance)
    {
        progress += Time.deltaTime;

        bc = bc ?? GetComponent<BoxCollider>();

        transform.rotation = Quaternion.LookRotation(distance);
        Debug.LogFormat("Current cos: {0}", Mathf.Cos(progress));
        transform.position += transform.forward * Time.deltaTime * speed;
        Vector3 delta = Vector3.right * Mathf.Cos(progress) * slalomAmplitude;
        childObject.transform.localPosition = delta;
        delta.z = bc.center.z;
        bc.center = delta;
    }
}
