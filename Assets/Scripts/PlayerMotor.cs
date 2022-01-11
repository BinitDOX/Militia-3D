using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    //We make rot around x optional
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float camRotLimit = 85f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    //private Vector3 camRotation = Vector3.zero;
    private float camRotationX = 0f;
    private float currentCamRotX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //Getter
    public void Move(Vector3 velo)
    {
        velocity = velo;
    }

    //Getter
    public void Rotate(Vector3 rot)
    {
        rotation = rot;
    }

    //Getter
    //public void RotateCam(Vector3 camRot)
    //{
    //    camRotation = camRot;
    //}

    //Getter
    public void RotateCam(float camRotX)
    {
        camRotationX = camRotX;
    }

    //Getter
    public void Thruster(Vector3 thrust)
    {
        thrusterForce = thrust;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration); //ForceMode.Acc => Ignore mass
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            //cam.transform.Rotate(-camRotation);
            //New Rot calculation for clamping
            currentCamRotX -= camRotationX;
            currentCamRotX = Mathf.Clamp(currentCamRotX, -camRotLimit, camRotLimit);
            //Apply rot to transform of cam
            cam.transform.localEulerAngles = new Vector3(currentCamRotX, 0f, 0f);
        }
    }

}
