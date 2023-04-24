using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public static DroneMovement instance;

    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float rotationAcceleration = 0.1f;
    [SerializeField] float jumpAcceleration = 5f;
    [SerializeField] float gravityAcceleration = 10f;
    [SerializeField] float gravity = 30f;
    [Space]
    [SerializeField] Transform parent;


    PlayerFighter fighter;

    CharacterController cc;

    Vector3 velocity;
    Vector3 parentPos;
    Quaternion parentRot;
    Vector3 dir;
    Vector3 moveDir;

    float moveBlendDir;
    float angleWhileAiming;

    bool exit;
    void Awake()
    {
        instance = this;

        fighter = GetComponent<PlayerFighter>();
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        parentPos = transform.position;
        parentRot = transform.rotation;
    }

    void Update()
    {
        CheckForInput();
        HandleMovement();
    }

    void HandleMovement()
    {
        

        if (velocity.y < 0.1f && cc.isGrounded) velocity.y = -1f;

        if (dir.magnitude >= 0.1f)
        {
            float angle;

            if (dir.x < 0) angle = -90;
            else angle = 90;

            if (angleWhileAiming == -90 && dir.x < 0 || angleWhileAiming == 90 && dir.x > 0) moveBlendDir = 1;
            if (angleWhileAiming == -90 && dir.x > 0 || angleWhileAiming == 90 && dir.x < 0) moveBlendDir = -1;


            transform.rotation = Quaternion.Euler(0, angle, 0);
            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            cc.Move(moveDir * movementSpeed * Time.deltaTime);
        }


        velocity.y = Mathf.Max(velocity.y - Time.deltaTime * gravityAcceleration, cc.isGrounded ? -1f : -gravity);

        if (Input.GetKey(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(-2f * jumpAcceleration * Time.deltaTime * -gravity);
        }

        cc.Move(velocity * Time.deltaTime);
    }

    void CheckForInput()
    {
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0).normalized;

        exit = Input.GetKeyDown(KeyCode.Q);

    }

    public void ParentDrone() => this.transform.SetParent(parent);
    public void UnParentDrone() => this.transform.SetParent(null);
    public void SetParentPosRot()
    {
        StartCoroutine(ResetPos());
    }

    IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(1);
        transform.position = new Vector3(.35f, .29f, 0);
        transform.rotation = Quaternion.Euler(-74.38f, 90, 0);
    }
}
