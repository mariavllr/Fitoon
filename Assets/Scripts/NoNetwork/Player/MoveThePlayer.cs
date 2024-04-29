using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThePlayer : MonoBehaviour
{
    public bool testing;

    Rigidbody rb;
    PlayerControl playerControl;
    Animator anim;
    float horizontal;
    float vertical;

    [Header("Movement")]
    public float moveSpeed;
    public float rotationSpeed;
    Vector3 moveDirection;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerControl = GetComponent<PlayerControl>();
        anim = GetComponentInChildren<Animator>();

        if (!testing)
        {
            //Activate face tracking
            GetComponent<FaceTrackingToMovement>().enabled = true;
            GetComponent<Cadencia>().enabled = true;
            GetComponent<WriteDataToCSV>().enabled = true;
        }
    }

    void Update()
    {
        //Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        SpeedControl();

        //Handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }

        else
        {
            rb.drag = 0;
        }
        
    }

    private void FixedUpdate()
    {
        //MOVEMENT
        if (playerControl.MovementAllowed())
        {
            anim.SetBool("isRunning", true);
            if (testing) MovePlayerInEditor();
            else MovePlayer();
        }

        if (rb.velocity.magnitude < 0.3f) anim.SetBool("isRunning", false);

    }

    private void MovePlayerInEditor()
    {
        //Rotate Player based on Horizontal input
        Vector3 rotation = new Vector3(0, horizontal * rotationSpeed, 0);
        Vector3 currentRotation = transform.rotation.eulerAngles;
        rb.MoveRotation(Quaternion.Euler(currentRotation + rotation));


        moveDirection = transform.forward * vertical + transform.right * horizontal;
        if(vertical != 0) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * playerControl.speedMultiplier * playerControl.speedBoost, ForceMode.Force);
    }

    private void MovePlayer()
    {
        //Rotation in "FaceTrackingToMovement.cs"

        rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Limit velocity
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
