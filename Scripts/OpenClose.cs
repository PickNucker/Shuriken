using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
    [SerializeField] Transform doorToOpen;
    [SerializeField] float duration = 2f;
    [SerializeField] float angle = 90f;

    int invert = -1;

    float openTime;

    bool active;
    bool open = true;

    void Update()
    {
        if (active)
        {
            if (open)
            {
                openTime += Time.deltaTime;
                doorToOpen.Rotate(new Vector3(0f, -angle / duration * -invert, 0f) * Time.deltaTime);
                if (openTime > duration)
                {
                    openTime = 0f;
                    //active = false;
                    open = false;
                }
            }
            else
            {
               // openTime += Time.deltaTime;
               // doorToOpen.Rotate(new Vector3(0f, -angle / duration * invert, 0f) * Time.deltaTime);
               // if (openTime > duration)
               // {
               //     openTime = 0f;
               //     //active = false;
               // }
            }
        }
    }

    public void Activate()
    {
        if (active) return;
        
         active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Activate();
    }
}
