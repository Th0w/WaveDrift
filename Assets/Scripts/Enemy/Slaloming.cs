using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slaloming : SimpleMover {
    #region Fields

    #region Serialized

    [SerializeField]
    private Transform childObject;
    [SerializeField]
    private float slalomAmplitude;

    #endregion Serialized

    private BoxCollider bc;
    private float progress;

    #endregion Fields

    #region Methods

    protected override void MoveFunction(Vector3 distance)
    {
        progress += Time.deltaTime;

        bc = bc ?? GetComponent<BoxCollider>();

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(distance), Time.deltaTime * turnSpeed);
        transform.position += transform.forward * Time.deltaTime * speed;
        Vector3 delta = Vector3.right * Mathf.Cos(progress) * slalomAmplitude;
        childObject.transform.localPosition = delta;
        delta.z = bc.center.z;
        bc.center = delta;
    }

    #endregion Methods
}
