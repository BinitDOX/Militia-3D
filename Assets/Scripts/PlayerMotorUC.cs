using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotorUC : MonoBehaviour
{
    //We make rot around x optional
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float camRotLimit = 85f;

    public bool isGrounded = true;
    private float thrustActivationTime = 0.5f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    //private Vector3 camRotation = Vector3.zero;
    private float camRotationX = 0f;
    private float currentCamRotX = 0f;
    private Vector3 thrusterForce = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;

    private Rigidbody rb;

    public GameObject pivot;
    public Collider collider;
    public bool thrustActivated;

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

    //Getter
    public void Jump(Vector3 jump)
    {
        jumpForce = jump;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        if (isGrounded && jumpForce != Vector3.zero)
        {
            rb.AddForce(jumpForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        thrustActivationTime -= Time.fixedDeltaTime;
        if (!isGrounded && thrusterForce != Vector3.zero && thrustActivationTime <= 0.2f)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration); //ForceMode.Acc => Ignore mass
            thrustActivated = true;
        }
        else
        {
            thrustActivated = false;
        }
        if (thrustActivationTime < -60f)
            thrustActivationTime = -1f;
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
            pivot.transform.localEulerAngles = new Vector3(currentCamRotX, 0f, 0f);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Floor")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "Floor")
        {
            isGrounded = false;
            thrustActivationTime = 0.5f;
        }
    }

}
