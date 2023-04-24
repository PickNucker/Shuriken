using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get2Coord : MonoBehaviour
{
    public Vector3 aZCoordinates;
    public Vector3 bZCoordinates;

    bool coordinatesBestimmt;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


      // if (!coordinatesBestimmt)
      // {
      //     if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out RaycastHit hit, 5f))
      //     {
      //
      //         Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
      //         var rend = hit.transform.GetComponent<Renderer>();
      //     }
      // }
    }
}
