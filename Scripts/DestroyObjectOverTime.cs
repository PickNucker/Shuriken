using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOverTime : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 2f;

    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }
}
