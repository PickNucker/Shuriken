using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HandleWeaponType : MonoBehaviour
{
    [SerializeField] float MaxSwitchTime = 3f;
    [SerializeField] Image choice_01;
    [SerializeField] Image choice_02;
    [SerializeField] Image choice_03;

    [SerializeField] Image coolDownImage;

    PlayerFighter fighter;

    float timer = Mathf.Infinity;

    bool active_01 = true;
    bool active_02;
    bool active_03;

    void Start()
    {
        fighter = FindObjectOfType<PlayerFighter>();
        choice_01.enabled = true;
        choice_02.enabled = false;
        choice_03.enabled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        coolDownImage.fillAmount = ((timer / MaxSwitchTime) * 100) / 100;

        if (timer < MaxSwitchTime) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (active_01) return;
            HandleAllAudio.instance.Play_switchGuns(transform);

            choice_01.enabled = true;
            choice_02.enabled = false;
            choice_03.enabled = false;
            active_01 = true;
            active_02 = false;
            active_03 = false;

            fighter.ActivityFirst();

            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (active_02) return;
            HandleAllAudio.instance.Play_switchGuns(transform);
            choice_01.enabled = false;
            choice_02.enabled = true;
            choice_03.enabled = false;
            active_01 = false;
            active_02 = true;
            active_03 = false;

            fighter.ActivitySecond();

            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (active_03) return;
            HandleAllAudio.instance.Play_switchGuns(transform);
            choice_01.enabled = false;
            choice_02.enabled = false;
            choice_03.enabled = true;

            active_01 = false;
            active_02 = false;
            active_03 = true;

            fighter.ActivityThird();

            timer = 0;
        }
    }
}
