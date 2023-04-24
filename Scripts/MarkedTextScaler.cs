using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkedTextScaler : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float change = .3f;

    [SerializeField] Canvas canvas;

    [SerializeField] RectTransform tf;

    float timer;

    void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GetComponentInParent<Enemy>().GetIfRdy())
        {
            canvas.gameObject.SetActive(true);
            timer += Time.deltaTime;

            canvas.transform.LookAt(Camera.main.transform.position);

            if (timer >= change)
            {
                tf.localScale = new Vector3(Mathf.Lerp(tf.localScale.x, 1f, Time.deltaTime * speed), 
                    Mathf.Lerp(tf.localScale.y, 1f, Time.deltaTime * speed), 
                    Mathf.Lerp(tf.localScale.z, 1f, Time.deltaTime * speed));

                timer = 0;
            }else
            {
                tf.localScale = new Vector3(Mathf.Lerp(tf.localScale.x, .6f, Time.deltaTime * speed),
                    Mathf.Lerp(tf.localScale.y, .6f, Time.deltaTime * speed),
                    Mathf.Lerp(tf.localScale.z, .6f, Time.deltaTime * speed));
            }
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
        
    }
}
