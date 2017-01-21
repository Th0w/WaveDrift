using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerating : SimpleMover {
    private float acceleration = 0.0f;
    [SerializeField]
    private float maxAcceleration = 5.0f;
    [SerializeField]
    private float accelerationRampTime = 1.0f;
    [SerializeField]
    private float rushingDistance = 5.0f;

    public float currentSpeed;

    protected override void MoveFunction(Vector3 distance)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(distance), Time.deltaTime * turnSpeed);
        //acceleration = Mathf.Clamp(acceleration + Time.deltaTime * (distance.magnitude <= rushingDistance ? 1 : -1) / accelerationRampTime, 0, maxAcceleration);
        
        if (distance.magnitude <= rushingDistance)
        {
            Debug.Log("I IZ ACCELERATING!");
            acceleration = Mathf.Clamp(acceleration + maxAcceleration * Time.deltaTime / accelerationRampTime, 0, maxAcceleration);
        } else
        {
            acceleration = Mathf.Clamp(acceleration - maxAcceleration * Time.deltaTime / accelerationRampTime, 0, maxAcceleration);
        }
        transform.position += transform.forward * Time.deltaTime * (speed + acceleration);
        currentSpeed = acceleration + speed;
    }
}
