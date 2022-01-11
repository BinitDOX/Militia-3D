using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotorUC))]
[RequireComponent(typeof(Animator))]
public class PlayerControllerUCJoy : MonoBehaviour
{
    [SerializeField]
    private float speed = 4f;
    [SerializeField]
    private float speedBoost = 9f;
    [SerializeField]
    private float lookSens = 4f;

    //Fly stuff
    [SerializeField]
    private float thrusterSpeed = 4500f;
    [SerializeField]
    private float thrusterFuelBurn = 0.32f;
    [SerializeField]
    private float thrusterFuelRegen = 0.4f;
    [SerializeField]
    private float jumpSpeed = 32500f;
    private float thrusterFuel = 1f;

    [SerializeField]
    private GameObject joyCanvas;
    [SerializeField]
    private Joystick joystickMove;
    [SerializeField]
    private Joystick joystickAim;
    private bool joyFlag = false;
    public bool joyJump;
    private float joyJumpTimer = -1f;
    private Button joyJumpBtn;

    public float GetThrusterFuelAmt()
    {
        return thrusterFuel;
    }

    //Joint stuff
    [Header("Spring settings:")]
    //[SerializeField]
    //private JointDriveMode jointMode = JointDriveMode.Position; //obselete
    [SerializeField]
    private float jointSpring = 0f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotorUC motor;
    private ConfigurableJoint joint;
    private Animator animator;

    public Animator animator2;

    void Start()
    {
        thrusterFuelBurn = 0.35f;
        thrusterFuelRegen = 0.4f;
        motor = GetComponent<PlayerMotorUC>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);

        //joyCanvas = GameObject.Find("PlayerUI");
        //if (joyCanvas != null)
        //{
        //    joystickMove = joyCanvas.transform.Find("Fixed Joystick").GetComponent<Joystick>();
        //    joystickAim = joyCanvas.transform.Find("Dynamic Joystick").GetComponent<Joystick>();
        //}
    }

    public void joyJumpEnable()
    {
        if(!joyJump)
            joyJump = true;
    }

    void Update()
    {
        if(joyCanvas == null)
        {
            joyCanvas = GameObject.Find("PlayerUI");
        }
        else if(!joyFlag)
        {
            joystickMove = joyCanvas.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            joystickAim = joyCanvas.transform.Find("Dynamic Joystick").GetComponent<Joystick>();
            joyJumpBtn = joyCanvas.transform.Find("Button").GetComponent<Button>();
            joyFlag = true;
        }

        joyJumpBtn.onClick.AddListener(joyJumpEnable);

        //Calc mvmt velocity as 3D vector
        //float xMove = Input.GetAxisRaw("Horizontal"); //Raw means no interpolation by unity..we do ourself later
        //float zMove = Input.GetAxisRaw("Vertical");
        float xMove = joystickMove.Horizontal;
        float zMove = joystickMove.Vertical;

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        //Final mvmt vector
        //Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;
        Vector3 velocity;
        if (joystickMove.Vertical > 0.5f)
        {
            velocity = (moveHorizontal + moveVertical) * speedBoost;
            animator2.SetBool("Run", true);
        }
        else
        {
            velocity = (moveHorizontal + moveVertical) * speed;
            animator2.SetBool("Run", false);
        }

        //Animate mvmt
        animator.SetFloat("ForwardVelocity", zMove);
        animator.SetFloat("SidewardVelocity", xMove);
        animator2.SetFloat("FVelo", zMove);
        animator2.SetFloat("SVelo", xMove);
        if (!motor.isGrounded)
        {
            animator2.SetFloat("FVelo", 0f);
            animator2.SetFloat("SVelo", 0f);
        }

        //Apply mvmt
        motor.Move(velocity);

        //Calc rotation as 3d vector (turning left and right)
        float yRot = joystickAim.Horizontal;

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSens;                    //Note: We only rotation OF SPHERE around y axis so we can move around, but for rotation around z or x axis, WE WILL DO IT ONLY FOR CAMERA bcz if we look down and press forward, we will clip thru ground

        //Apply rotation
        motor.Rotate(rotation);

        //Calc camera rotation as 3d vector (turning up and down)
        float xRot = joystickAim.Vertical;

        //Vector3 camRotation = new Vector3(xRot, 0f, 0f) * lookSens;                    //Note: We only rotation OF SPHERE around y axis so we can move around, but for rotation around z or x axis, WE WILL DO IT ONLY FOR CAMERA bcz if we look down and press forward, we will clip thru ground
        float camRotationX = xRot * lookSens;                    //Note: We only rotation OF SPHERE around y axis so we can move around, but for rotation around z or x axis, WE WILL DO IT ONLY FOR CAMERA bcz if we look down and press forward, we will clip thru ground


        //Apply camerarotation
        motor.RotateCam(camRotationX);

        //Calc Thruster force
        Vector3 thrusterForce = Vector3.zero;
        //Calc Jump force
        Vector3 jumpForce = Vector3.zero;
        if (joyJump && joyJumpTimer >= 0f)
        {
            if (thrusterFuel > 0f)
            {
                if(motor.thrustActivated)
                    thrusterFuel -= thrusterFuelBurn * Time.deltaTime;
                thrusterForce = Vector3.up * thrusterSpeed;
            }
            jumpForce = Vector3.up * jumpSpeed;
            if(motor.isGrounded)
                animator2.SetBool("Jump", true);
            //SetJointSettings(0f);
            joyJumpTimer -= Time.deltaTime;
        }
        else
        {
            thrusterFuel += thrusterFuelRegen * Time.deltaTime;

            animator2.SetBool("Jump", false);
            //SetJointSettings(jointSpring);
            joyJumpTimer = 3f;
            joyJump = false;
        }
        //Apply thruster force
        motor.Thruster(thrusterForce);
        //Apply jump force
        motor.Jump(jumpForce);

        thrusterFuel = Mathf.Clamp(thrusterFuel, 0f, 1f);

        if (joyJumpTimer < -50f)
            joyJumpTimer = -1f;
    }


    private void SetJointSettings(float jointSp)
    {
        joint.yDrive = new JointDrive
        {
            //mode = jointMode, //obselete
            positionSpring = jointSp,
            maximumForce = jointMaxForce
        };
    }
}
