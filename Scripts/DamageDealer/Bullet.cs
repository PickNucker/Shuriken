using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float turnSpeed = 150f;

    [Header("Normal Shuriken")]
    [SerializeField] float normalDmg = 5f;
    [SerializeField] float normalCoolDown = .5f;

    [Header("Grenade Shuriken")]
    [SerializeField] float grenadeDmg = 20f;
    [SerializeField] float explosionRange = 5f;
    [SerializeField] float grenadeCoolDown = 15f;
    [SerializeField] float explosionForce = 500f;
    [SerializeField] float explosionsTimer = 3f;
    [SerializeField] Text grenadeTimerText = default;
    [SerializeField] GameObject explosionEffect;
    [Space]
    [SerializeField] float explodeRadiusNear = 13f;
    [SerializeField] float explodeRadiusMiddle = 7f;
    [SerializeField] float explodeRadiusWide = 3f;
    [SerializeField] float explodeDuration = .2f;
    [Space]
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;

    [Header("Transport Shuriken")]
    [SerializeField] float transportDmg = 10f;
    [SerializeField] float transportCoolDown = 2f;

    public bool normal;
    public bool grenade;
    public bool transport;

    Rigidbody rb;

    bool stopRot;

    float normalTimer;
    float grenadeTimer;
    float transportTimer;

    bool startTimerNo;
    bool startTimerGr;
    bool startTimerTr;

    bool marked;
    Transform enemyMarked;
    MeshRenderer renderer;

    void Start()
    {
        grenadeTimer = explosionsTimer;

        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (normal)
        {
            transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
        }

        if (grenade)
        {

            canvas.transform.LookAt(Camera.main.transform.position);

            if (startTimerGr) grenadeTimer = Mathf.Max(grenadeTimer - Time.deltaTime, 0);

            text.text = grenadeTimer.ToString("0");

            transform.Rotate(0, Time.deltaTime * turnSpeed, 0);

            var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRange);
            if (grenadeTimer <= 0)
            {
                GameObject go = Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
                ShakeCam.instance.HandleExplosionRadius(explodeDuration, explodeRadiusNear, explodeRadiusMiddle, explodeRadiusWide, this.transform);
                HandleAllAudio.instance.Play_GrenadeExplode(transform);

                foreach (var obj in surroundingObjects)
                {
                    if (!obj.CompareTag("Enemy")) continue;

                    var rb = obj.GetComponent<Rigidbody>();
                    obj.GetComponent<IDamagable>().Damage(grenadeDmg);
                    //rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
                }
                Destroy(this.gameObject);
            }
        }

        if (transport)
        {
            if(!stopRot)
                transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (normal)
        {
            if(collision.transform.TryGetComponent<IDamagable>(out IDamagable dmg) && !(collision.transform.tag == "Player"))
            {
                dmg.Damage(normalDmg);
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        if (grenade)
        {
            startTimerGr = true;
        }

        if (transport)
        {
            rb.isKinematic = true;
            stopRot = true;
            transform.SetParent(collision.transform);

            collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy);
            if (enemy) renderer.enabled = false;
            if (enemy) enemy.MarkTeleport();
            GetComponent<MeshRenderer>().enabled = false;
            PlayerFighter.instance.enemyMarked = enemy;

            StartCoroutine(DestroyObject());
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        PlayerFighter.instance.enemyMarked = null;
        Destroy(this.gameObject);
    }

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }

    private void OnDrawGizmos()
    {
        if (grenade)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, explosionRange);
        }
    }

}
