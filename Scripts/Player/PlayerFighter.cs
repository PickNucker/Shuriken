using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public enum BulletType
{
    Normal,
    Grenade,
    Teleport
}

public class PlayerFighter : MonoBehaviour
{
    public static PlayerFighter instance;

    [HideInInspector]
    public bool deactivateWhileRoll;

    [SerializeField] BulletType bulletType;

    [SerializeField] float aimSpeed = 5f;
    [SerializeField] float enableShootAfterTime = 0.15f;
    [Space]

    [SerializeField] float markedDetectRange = 10f;
    [SerializeField] float followTargetSpeed = 3f;
    [Space]

    [SerializeField] Rig aimRigWeight;
    [SerializeField] Rig idleRigWeight;
    [SerializeField] Transform followTarget;
    [SerializeField] LayerMask whatIsAim;

    [Space]
    [SerializeField] Transform weapon;
    [SerializeField] Transform spawnPos;
    [SerializeField] Transform weaponMuzzelSpawnPos;
    [SerializeField] GameObject muzzelParticle;
    [Space]
    [Header("Normal Bullet")]
    [SerializeField] float normalCoolDown = 2f;
    [SerializeField] GameObject normalBullet;
    [SerializeField] Image normalText;

    [Header("Grenade Bullet")]
    [SerializeField] float grenadeCoolDown = 15f;
    [SerializeField] GameObject grenadeBullet;
    [SerializeField] Image grenadeText;

    [Header("Transport Bullet")]
    [SerializeField] float transportCoolDown = 6f;
    [SerializeField] GameObject transportBullet;
    [SerializeField] Image tpText;
    Animator anim;

    [Space] [SerializeField] AudioTriggerSFX normalShot;

    public bool isAiming;

    float weight;
    float rigWeight;
    float idleWeight;

    bool canShoot;
    bool shootEnbled;
    bool isShooting;

    public int shootCounter;

    public bool enemyMarkedInRange;

    bool canTP;

    float timerNormal = Mathf.NegativeInfinity;
    float timerGrenade = Mathf.NegativeInfinity;
    float timerTeleport = Mathf.NegativeInfinity;

    Vector3 enemyLastPos;
    Vector3 playerLastPos;

    public Enemy enemyMarked;

    void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        bulletType = BulletType.Normal;
        aimRigWeight.weight = 0;
    }

    void Update()
    {
        if (PlayerHeatlh.instance.ReturnHealth() <= 0) return;

        timerNormal -= Time.deltaTime;
        timerGrenade -= Time.deltaTime;
        timerTeleport -= Time.deltaTime;


        HandleTextImage();
        if (deactivateWhileRoll) return;
        FollowTargetToMouse();
        HandleShoot();
        HandleAiming();
        HandleTeleport();
    }

    void HandleTextImage()
    {
        if (timerNormal <= 0) normalText.gameObject.SetActive(false);else normalText.gameObject.SetActive(true);
        if (timerGrenade <= 0) grenadeText.gameObject.SetActive(false); else grenadeText.gameObject.SetActive(true);
        if (timerTeleport <= 0) tpText.gameObject.SetActive(false); else tpText.gameObject.SetActive(true);

        normalText.fillAmount = ((timerNormal / normalCoolDown) * 100) / 100;
        grenadeText.fillAmount = ((timerGrenade / grenadeCoolDown) * 100) / 100;
        tpText.fillAmount = ((timerTeleport / transportCoolDown) * 100) / 100;
    }

    void HandleTeleport()
    {
        if (enemyMarked != null) canTP = true; else canTP = false;

        if (canTP)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(TP());
                HandleAllAudio.instance.Play_teleported(transform);
                playerLastPos = transform.position;

                gameObject.transform.position = new Vector3(enemyMarked.GetComponent<Transform>().position.x, enemyMarked.GetComponent<Transform>().position.y, 0);
                enemyMarked.GetComponent<Transform>().position = playerLastPos;
                enemyMarked.ResetTeleport();
                enemyMarked = null;
                CanTP(false);
            }
        }
    }

    IEnumerator TP()
    {
        enemyMarked.GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(.01f);
        GetComponent<CharacterController>().enabled = true;
        yield return new WaitForSeconds(1f);
        //enemyMarked.GetComponent<NavMeshAgent>().enabled = true;
        enemyMarked = null;

    }

    void HandleAiming()
    {
        isAiming = Input.GetMouseButton(1);

        anim.SetLayerWeight(1, weight);

        if (isAiming)
        {
            weight = Mathf.Lerp(anim.GetLayerWeight(1), 1, Time.deltaTime * aimSpeed);
            rigWeight = Mathf.Lerp(rigWeight, 1, Time.deltaTime * 5f);
            idleWeight = Mathf.Lerp(idleWeight, 0, Time.deltaTime * 5f);

            if (!shootEnbled)
            {
                StartCoroutine(EnableShoot(enableShootAfterTime));
                shootEnbled = true;
            }
        }
        else
        {
            weight = Mathf.Lerp(anim.GetLayerWeight(1), 0, Time.deltaTime * aimSpeed);
            rigWeight = Mathf.Lerp(rigWeight, 0, Time.deltaTime * 5f);
            idleWeight = Mathf.Lerp(idleWeight, 1, Time.deltaTime * 5f);

            if (shootEnbled)
            {
                StartCoroutine(EnableShoot(0));
                shootEnbled = false;
            }
        }

        aimRigWeight.weight = rigWeight;
        idleRigWeight.weight = idleWeight;
    }

    void FollowTargetToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, whatIsAim))
        {
            followTarget.position = Vector3.Lerp(followTarget.position, hit.point, Time.deltaTime * followTargetSpeed);
        }
    }

    void HandleShoot()
    {
        if (!canShoot) return;

        if (Input.GetMouseButtonDown(0))
        {
            switch (bulletType)
            {
                case BulletType.Normal:
                    if(timerNormal <= 0)
                        SpawnBulletNormal();
                    break;
                case BulletType.Grenade:
                    if (timerGrenade <= 0)
                        SpawnBulletGrenade();
                    break;
                case BulletType.Teleport:
                    if (timerTeleport <= 0)
                        SpawnBulletTransport();
                    break;
                default:
                    break;
            }
        }
    }

    void SpawnBulletNormal()
    {
        timerNormal = normalCoolDown;

        HandleAllAudio.instance.Play_shootNormal(transform);
        GameObject muzzle = Instantiate(muzzelParticle, weaponMuzzelSpawnPos.position, weaponMuzzelSpawnPos.rotation);
        Destroy(muzzle, 1f);
        GameObject bullet = Instantiate(normalBullet, spawnPos.position, spawnPos.rotation);
        bullet.transform.SetParent(GameObject.Find("Bullet Spawn Holder Parent").transform);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.GetComponent<Bullet>().normal = true;
        rb.AddForce(spawnPos.forward * bullet.GetComponent<Bullet>().GetBulletSpeed(), ForceMode.Impulse);
    }

    void SpawnBulletGrenade()
    {
        timerGrenade = grenadeCoolDown;

        HandleAllAudio.instance.Play_shootGrenade(transform);
        GameObject muzzle = Instantiate(muzzelParticle, weaponMuzzelSpawnPos.position, weaponMuzzelSpawnPos.rotation);
        Destroy(muzzle, 1f);
        GameObject bullet = Instantiate(grenadeBullet, spawnPos.position, spawnPos.rotation);
        bullet.transform.SetParent(GameObject.Find("Bullet Spawn Holder Parent").transform);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.GetComponent<Bullet>().grenade = true;
        rb.AddForce(spawnPos.forward * bullet.GetComponent<Bullet>().GetBulletSpeed(), ForceMode.Impulse);
    }

    void SpawnBulletTransport()
    {
        timerTeleport = transportCoolDown;

        HandleAllAudio.instance.Play_shootTeleport(transform);
        GameObject muzzle = Instantiate(muzzelParticle, weaponMuzzelSpawnPos.position, weaponMuzzelSpawnPos.rotation);
        Destroy(muzzle, 1f);
        GameObject bullet = Instantiate(transportBullet, spawnPos.position, spawnPos.rotation);
        bullet.transform.SetParent(GameObject.Find("Bullet Spawn Holder Parent").transform);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.GetComponent<Bullet>().transport = true;
        rb.AddForce(spawnPos.forward * bullet.GetComponent<Bullet>().GetBulletSpeed(), ForceMode.Impulse);
    }


    IEnumerator EnableShoot(float timerToShoot)
    {
        yield return new WaitForSeconds(timerToShoot);
        canShoot = shootEnbled ? true : false;
    }

    public void ActivityFirst()
    {
        bulletType = BulletType.Normal;
    }

    public void ActivitySecond()
    {
        bulletType = BulletType.Grenade;
    }

    public void ActivityThird()
    {
        bulletType = BulletType.Teleport;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, markedDetectRange);
    }

    public void CanTP(bool activity)
    {
        canTP = activity;
    }
}
