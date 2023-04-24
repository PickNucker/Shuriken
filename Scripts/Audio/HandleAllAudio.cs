using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAllAudio : MonoBehaviour
{
    public static HandleAllAudio instance;

    [Header("Player")]
    [SerializeField] AudioTriggerSFX playerMovement;
    [SerializeField] AudioTriggerSFX playerJump;
    [SerializeField] AudioTriggerSFX aimSound;
    [SerializeField] AudioTriggerSFX switchGuns;
    [SerializeField] AudioTriggerSFX shootNormal;
    [SerializeField] AudioTriggerSFX shootGrenade;
    [SerializeField] AudioTriggerSFX grenadeExplode;
     [SerializeField] AudioTriggerSFX shootTeleport;
    [SerializeField] AudioTriggerSFX p_die;
    [SerializeField] AudioTriggerSFX teleported;
    [Space]
    [Header("Enemy")]
    [SerializeField] AudioTriggerSFX run;
    [SerializeField] AudioTriggerSFX e_die;
    [SerializeField] AudioTriggerSFX hit;
    [SerializeField] AudioTriggerSFX celeberate;

    private void Awake() => instance = this;

    public void Play_PlayerMovement(Transform pos) => playerMovement.Play(pos.position);
    public void Play_playerJump(Transform pos) => playerJump.Play(pos.position);
    public void Play_aiming(Transform pos) => aimSound.Play(pos.position);
    public void Play_switchGuns(Transform pos) => switchGuns.Play(pos.position);
    public void Play_shootNormal(Transform pos) => shootNormal.Play(pos.position);
    public void Play_shootGrenade(Transform pos) => shootGrenade.Play(pos.position);
    public void Play_GrenadeExplode(Transform pos) => grenadeExplode.Play(pos.position);
    public void Play_shootTeleport(Transform pos) => shootTeleport.Play(pos.position);
    public void Play_teleported(Transform pos) => teleported.Play(pos.position);
    public void Play_p_die(Transform pos) => p_die.Play(pos.position);
    //___-------------------------------------------------------------------------------___//
    public void Play_enemy_run(Transform pos) => run.Play(pos.position);
    public void Play_enemy_e_die(Transform pos) => e_die.Play(pos.position);
    public void Play_enemy_hit(Transform pos) => hit.Play(pos.position);
    public void Play_enemy_celeberate(Transform pos) => celeberate.Play(pos.position);
}
