using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCam : MonoBehaviour
{
    public static ShakeCam instance;

    [SerializeField] float schnelleZumNull = 5f;

    [SerializeField] CinemachineVirtualCamera cam;

    float timer;

    private void Awake()
    {
        instance = this;
    }

    public void Shake(float strength, float timer)
    {
        CinemachineBasicMultiChannelPerlin camPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        camPerlin.m_AmplitudeGain = strength;
        this.timer = timer;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            var camPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            camPerlin.m_AmplitudeGain = Mathf.Lerp(camPerlin.m_AmplitudeGain, 0, Time.deltaTime * schnelleZumNull);
        }
    }

    public void HandleExplosionRadius(float timer, float near, float middle, float wide, Transform explosionSpot)
    {
        Transform player = PlayerFighter.instance.transform;
        if      (Vector3.Distance(explosionSpot.position, player.position) < 5f) 
                 Shake(near, timer);
        else if (Vector3.Distance(explosionSpot.position, player.position) >= 5f &&
                 Vector3.Distance(explosionSpot.position, player.position) < 15f) 
                 Shake(middle, timer);
        else if (Vector3.Distance(explosionSpot.position, player.position) >= 15f &&
                 Vector3.Distance(explosionSpot.position, player.position) < 30f) 
                 Shake(wide, timer);
        else return;
    }
}
