using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneActivity : MonoBehaviour
{
    [SerializeField] float distance = 2f;
    [SerializeField] float followSpeed = 5f;
    [SerializeField] Vector3 pos;
    [Space]
    [SerializeField] GameObject DroneCam;
    [SerializeField] KeyCode activateMode;
    [SerializeField] KeyCode DeactivateMode;
    [SerializeField] float coolDown = 30f;

    [SerializeField] GameObject drone;
    [SerializeField] GameObject player;

    [HideInInspector]
    public bool modeEnabled = true;
    bool activity;

    float timer = Mathf.Infinity;

    bool followObject = true;

    Vector3 offset;

    int number;

    private void Start()
    {
        drone.GetComponent<CharacterController>().enabled = false;
    }

    private void Update()
    {
        if (followObject)
        {
            HandleFollowTarget();
        }

        timer += Time.deltaTime;

        if (!modeEnabled || timer < coolDown) return;

        if (!activity)
        {
            if (Input.GetKeyDown(activateMode)) 
            {
                activity = true;
                EnterDroneMode();
            }
        }
       
        if (activity)
        {
            if (Input.GetKeyDown(DeactivateMode))
            {
                ExitDroneMode();
                activity = false;
                timer = 0;
            }
        }

    }

    private void HandleFollowTarget()
    {
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            number = -90;
        }else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            number = 90;
        }

        drone.transform.rotation = Quaternion.Euler(0, number, 0);

        Vector3 offset = Camera.main.transform.position + Camera.main.transform.forward * distance - transform.position;
        transform.position += offset * Time.deltaTime * followSpeed;

        if (offset.magnitude < 1f)
        {
            drone.GetComponent<Rigidbody>().velocity = offset * 5f;
        }
        else
        {
            drone.GetComponent<Rigidbody>().velocity = offset.normalized * 5f;
        }
    }

    public void EnterDroneMode()
    {
        PlayerMovement.instance.enabled = false;
        DroneMovement.instance.enabled = true;
        DroneCam.SetActive(true);
        followObject = false;
        drone.GetComponent<CharacterController>().enabled = true;

    }

    public void ExitDroneMode()
    {
        followObject = true;
        DroneCam.SetActive(false);
        PlayerMovement.instance.enabled = true;
        DroneMovement.instance.enabled = false;
        DroneCam.SetActive(false);
        drone.GetComponent<CharacterController>().enabled = false;
    }
}
