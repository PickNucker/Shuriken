using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerHeatlh : MonoBehaviour, IDamagable
{
    public static PlayerHeatlh instance;

    [SerializeField] CinemachineVirtualCamera playerCam;
    [SerializeField] float zoomSpeed = 10;
    [SerializeField] float zoomAbstand = 1f;
    public int currentHealth;
    [SerializeField] int maxHealth;
    [Space]
    [SerializeField] Image[] images;
    [Space]
    [SerializeField] Sprite fullHearth;
    [SerializeField] Sprite emptyHearth;

    [SerializeField] GameObject deathScreen;

    Animator anim;

    bool gotDmg;
    int counter;

    public bool hittable;
    bool instantiateDeathSequenz;

    bool died;
     
    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        hittable = true;
        playerCam.m_Lens.FieldOfView = 60f;

        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            if (!instantiateDeathSequenz)
            {
                HandleAllAudio.instance.Play_p_die(transform);
                this.gameObject.layer = LayerMask.NameToLayer("Default");
                DeathSequenz(45);
                instantiateDeathSequenz = true;
                anim.SetTrigger("dead");
                Invoke(nameof(HandleDeath), 1f);
            }
        }

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        for (int i = 0; i < images.Length; i++)
        {
            if (i < currentHealth) images[i].sprite = fullHearth;
            else images[i].sprite = emptyHearth;

            if (i < maxHealth) images[i].gameObject.SetActive(true);
            else images[i].gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TakeDamage(float dmg)
    {
        if (!gotDmg && hittable)
        {
            currentHealth = (int)Mathf.Max(currentHealth - dmg, 0);
            Invoke(nameof(ResetDmgTakable), .5f);
            gotDmg = true;
        }
    }



    public void Damage(float dmg)
    {
        TakeDamage(dmg);
    }

    void ResetDmgTakable()
    {
        gotDmg = false;
    }

    void HandleDeath()
    {
        deathScreen.SetActive(true);
    }

    void DeathSequenz(float zahl)
    {
        if(counter < 2)
        {
            
        } 
    }

    void CounterThem()
    {
        //counter++;
        //DeathSequenz(30);
    }

    public int ReturnHealth()
    {
        return currentHealth;
    }
}
