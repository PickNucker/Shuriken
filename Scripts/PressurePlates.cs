using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlates : MonoBehaviour
{
    [SerializeField] float duration = 2f;
    [SerializeField] float angle = 90f;
    [SerializeField] float plateIdleTime = 1f;

    [SerializeField] int invert = -1;

    float openTime;

    bool active;
    bool open = true;

    float activeTimer;

    void Update()
    {
        if (!active)
        {
            activeTimer += Time.deltaTime;
            if(activeTimer > plateIdleTime)
            {
                active = true;
            }
            return;
        }

        if (open)
        {
            openTime += Time.deltaTime;
            transform.Rotate(new Vector3(-angle / duration * invert, 0f, 0f) * Time.deltaTime);
            if (openTime > duration)
            {
                openTime = 0f;
                open = false;
                active = false;
                activeTimer = 0;
            }
        }
        else
        {
            openTime += Time.deltaTime;
            transform.Rotate(new Vector3(-angle / duration * -invert, 0f, 0f) * Time.deltaTime);
            if (openTime > duration)
            {
                openTime = 0f;
                open = true;
                active = false;
                activeTimer = 0;
            }
        }

    }

    IEnumerator DelayTheSwitch(bool activity)
    {
        yield return new WaitForSeconds(plateIdleTime);
        open = activity;
    }
}
