using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleTPSymbol : MonoBehaviour
{
    public static HandleTPSymbol instance;

    [SerializeField] Image image;

    public bool rdyTobeGreen;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        image.color = Color.red;
    }

    void Update()
    {
        // Enemy[] enemy = FindObjectsOfType<Enemy>();
        // if(enemy != null)
        // {
        //     foreach (Enemy mob in enemy)
        //     {
        //         Debug.Log("was geht");
        //         if (mob.GetIfRdy())
        //         {
        //             Debug.Log("rdy");
        //             image.color = Color.green;
        //             Invoke(nameof(ResetColor), 5f);
        //         }
        //     }
        // }

        if(rdyTobeGreen) image.color = Color.green;
        else image.color = Color.red;
    }

    void ResetColor()
    {
        image.color = Color.red;
    }
}
