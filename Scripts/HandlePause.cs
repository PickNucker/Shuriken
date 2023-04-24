using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandlePause : MonoBehaviour
{
    [SerializeField] UnityEvent s;
    [SerializeField] GameObject pause;

    void Start()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
    }

    bool paused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        s.Invoke();
        Time.timeScale = 0;
        paused = true;
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1;
        pause.SetActive(false);
    }
}
