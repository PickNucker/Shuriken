using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int checkPointLevel = default;

    void Start()
    {
        if (PlayerPrefs.HasKey("checkPoint"))
        {
            if (checkPointLevel == PlayerPrefs.GetInt("checkPoint"))
            {
                PlayerHeatlh.instance.transform.position = transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerHeatlh.instance.gameObject)
        {
            if (!PlayerPrefs.HasKey("checkPoint") || PlayerPrefs.GetInt("checkPoint") < checkPointLevel)
            {
                PlayerPrefs.SetInt("checkPoint", checkPointLevel);
            }
        }
    }
}
