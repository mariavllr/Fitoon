using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public bool testing;

    Rigidbody rb;
    //PlayerControl playerControl;
    //Animator anim;
    float horizontal;
    float vertical;

    private float moveV;
    private float moveH;
    //private bool isFrozen = false;
    private float speedBoost = 1f;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float rotationSpeed = 1f;
    Vector3 moveDirection;
    //[SerializeField] TextMeshProUGUI velocityText;

    [Header("Ground check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //playerControl = GetComponent<PlayerControl>();
        //anim = GetComponentInChildren<Animator>();
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
        //velocityText.text = $"In game velocity: {Math.Round(rb.velocity.magnitude, 2, MidpointRounding.AwayFromZero)} ({Math.Round(rb.velocity.magnitude * 3.6, 2, MidpointRounding.AwayFromZero)}) km/h";
    }

    private void FixedUpdate()
    {
        //MOVEMENT
        //if (playerControl.MovementAllowed())
        //{
        //    anim.SetBool("isRunning", true);
        //    if (testing) MovePlayerInEditor();
        //    else MovePlayer();
        //}
        MovePlayer();

        //if (rb.velocity.magnitude < 0.3f) anim.SetBool("isRunning", false);

    }

    //private void MovePlayerInEditor()
    //{
    //    //Rotate Player based on Horizontal input
    //    Vector3 rotation = new Vector3(0, horizontal * rotationSpeed, 0);
    //    Vector3 currentRotation = transform.rotation.eulerAngles;
    //    rb.MoveRotation(Quaternion.Euler(currentRotation + rotation));


    //    moveDirection = transform.forward * vertical + transform.right * horizontal;
    //    if (vertical != 0) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * playerControl.speedMultiplier * playerControl.speedBoost, ForceMode.Force);
    //}

    public void SetMovement(float moveV, float moveH)
    {
        //if (countdownTimer.HasFinished())
        //{
        //    this.moveV = moveV;
        //    this.moveH = moveH;
        //}

        //Luego borrar estas 2 lineas
        this.moveV = moveV;
        this.moveH = moveH;
    }

    public void SetBoost(float boost)
    {
        speedBoost = boost;
    }

    private void MovePlayer()
    {
        //Rotation in "FaceTrackingToMovement.cs"

        //rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);

        Vector3 fwd = transform.forward * moveV * moveSpeed * speedBoost;
        Vector3 rotation = new Vector3(0, moveH * rotationSpeed, 0);

        Vector3 currentRotation = transform.rotation.eulerAngles;

        if (rb.velocity.y > 0) rb.velocity = fwd + new Vector3(0, rb.velocity.y, 0);
        else rb.velocity = fwd + new Vector3(0, rb.velocity.y, 0);

        rb.MoveRotation(Quaternion.Euler(currentRotation + rotation));

        //if (!isFrozen)
        //{
        //    if (rb.velocity.y > 0) rb.velocity = fwd + new Vector3(0, rb.velocity.y, 0);
        //    else rb.velocity = fwd + new Vector3(0, rb.velocity.y, 0);

        //    rb.MoveRotation(Quaternion.Euler(currentRotation + rotation));
        //    //charModel.transform.rotation = Quaternion.Euler(currentRotation + rotation);
        //}
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
