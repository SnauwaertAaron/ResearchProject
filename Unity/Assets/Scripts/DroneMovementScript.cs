using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovementScript : MonoBehaviour
{
    Rigidbody drone;
    private float tiltAmountSideways;

    void Awake(){
        drone = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        MovementUpDown();
        MovementForwardBackwards();
        MovementLeftRight();
        Rotation();
        ClampingSpeedValues();

        // Make drone hover
        drone.AddRelativeForce(Vector3.up * upForce);
        drone.rotation = Quaternion.Euler(
            new Vector3(tiltAmount, currentYRotation, tiltAmountSideways)
            );
    }

    public float upForce;
    void MovementUpDown(){
        if (Input.GetKey(KeyCode.UpArrow)){
            upForce = 9000;
        }
        else if (Input.GetKey(KeyCode.DownArrow)){
            upForce = -4000;
        }
        else if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)){
            upForce = 98.1f;
        }
    }

    private float movementSpeed = 5000.0f;
    private float tiltAmount = 0;
    private float tiltVelocity;
    void MovementForwardBackwards(){
        if(Input.GetKey(KeyCode.Z)){
            drone.AddRelativeForce(Vector3.forward * movementSpeed);
        }

        if (Input.GetKey(KeyCode.S)){
            drone.AddRelativeForce(Vector3.back * movementSpeed);
        }
    }

    void MovementLeftRight(){
        if (Input.GetKey(KeyCode.Q)){
            drone.AddRelativeForce(Vector3.left * movementSpeed);
        }
        if (Input.GetKey(KeyCode.D)){
           drone.AddRelativeForce(Vector3.right * movementSpeed);
        }
    }

    private float wantedYRotation;
    private float currentYRotation;
    private float rotationAmountByKey = 1.875f;
    private float rotationYVelocity;
    void Rotation(){
        if (Input.GetKey(KeyCode.LeftArrow)){
            wantedYRotation -= rotationAmountByKey;
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            wantedYRotation += rotationAmountByKey;
        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }

    private Vector3 velocityToSmoothDampToZero;
    void ClampingSpeedValues() {
        if (Input.GetKey(KeyCode.Z)) {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 1000.0f, Time.deltaTime * 5f));
        }
        else if (Input.GetKey(KeyCode.S)) {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 1000.0f, Time.deltaTime * 5f));
        }
        if (Input.GetKey(KeyCode.Q)) {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 1000.0f, Time.deltaTime * 5f));
        }
        else if (Input.GetKey(KeyCode.D)) {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 1000.0f, Time.deltaTime * 5f));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 100.0f, Time.deltaTime * 5f));
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 100.0f, Time.deltaTime * 5f));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
    }
}
