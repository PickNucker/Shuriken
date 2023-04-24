using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float rotationAcceleration = 0.1f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float gravityAcceleration = 10f;
    [SerializeField] float gravity = 30f;
    [SerializeField] float groundRadius = .5f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask whatIsGround;

    PlayerFighter fighter;
    
    CharacterController cc;
    Animator anim;
    Rigidbody rigid;

    Vector3 velocity;

    Vector3 dir;
    Vector3 moveDir;
    Vector3 input;

    float maxMovementSpeed;
    float turnSmoothVel;
    float angleWhileAiming;
    float inputDir;
    float moveBlendDir;

    bool speedChanged;
    bool isRunning;
    bool isGrounded;
    bool rolled;

    public bool deActivateQuick;

    HandleAllAudio audio;

    void Awake()
    {
        instance = this;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        fighter = GetComponent<PlayerFighter>();
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        maxMovementSpeed = movementSpeed;

        audio = HandleAllAudio.instance;

        deActivateQuick = true;
    }

    void Update()
    {
        if (PlayerHeatlh.instance.ReturnHealth() <= 0) return;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, whatIsGround);

        if (!deActivateQuick)
        {
            CheckForInput();
            HandleMovement();
            HandleMoveSpeed();
        }
    }


    private void LateUpdate()
    {
        UpdateAnimation();
    }

    public void ActivateControls()
    {
        deActivateQuick = false;
    }

    void HandleMovement()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, whatIsGround);

        //if (velocity.y < 0.1f && isGrounded) velocity.y = -1f;

        if (dir.magnitude >= 0.1f)
        {
            if(isGrounded)
                isRunning = true;
            else isRunning = false;

            float angle;

            if(inputDir > .5f) angleWhileAiming = -90;
            else angleWhileAiming = 90;

            if (dir.x < 0) angle = -90;
            else angle = 90;

            if (angleWhileAiming == -90 && dir.x < 0 || angleWhileAiming == 90 && dir.x > 0) moveBlendDir = 1;
            if (angleWhileAiming == -90 && dir.x > 0 || angleWhileAiming == 90 && dir.x < 0) moveBlendDir = -1;


            transform.rotation = Quaternion.Euler(0, fighter.isAiming ? angleWhileAiming : angle, 0);
            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            cc.Move(moveDir * movementSpeed * Time.deltaTime);
        }
        else isRunning = false;

        velocity.y = Mathf.Max(velocity.y - Time.deltaTime * gravityAcceleration, isGrounded ? -1f : -gravity);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !rolled)
        {

            HandleAllAudio.instance.Play_playerJump(transform);
            //velocity.y = Mathf.Sqrt(-2f * jumpHeight * -gravity);
            anim.SetTrigger("roll");
            rolled = true;

        }

        cc.Move(velocity * Time.deltaTime);
    }

    public void DeactivateRootMotion()
    {
        anim.applyRootMotion = false;
        PlayerFighter.instance.deactivateWhileRoll = false;
        PlayerHeatlh.instance.hittable = true;
        cc.enabled = true;
        rolled = false;
    }
    public void ActivateRootMotion()
    {
        anim.applyRootMotion = true;
        PlayerHeatlh.instance.hittable = false;
        PlayerFighter.instance.deactivateWhileRoll = true;
        cc.enabled = false;
    }

    void HandleMoveSpeed()
    {
        if (fighter.isAiming)
        {
            if (!speedChanged)
            {
                movementSpeed /= 1.5f;
                speedChanged = true;
            }
        }
        else
        {
            if (speedChanged)
            {
                movementSpeed = maxMovementSpeed;
                speedChanged = false;
            }
        }
    }

    void CheckForInput()
    {
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0).normalized;

        input = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (input.x > .5f) inputDir = -1f; else inputDir = 1f;
    }

    void UpdateAnimation()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetFloat("moveBlend", velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    public float GetDir()
    {
        return inputDir;
    }

    public void PlayPlayerRun()
    {
        audio.Play_PlayerMovement(this.transform);
    }
}
