using System;
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public event Action<Enemy> OnEnemyDeath;
    [SerializeField] private int health = 100;
    [SerializeField] private float speed = 1f;
    [SerializeField] private int damage = 20;
    [SerializeField]private float packSpeed=16f;
    [SerializeField]private ParticleSystem deathParticle;
    private int currentHealth;
    private Transform player;
    private bool movePack=false;
    private PlayerController playerCtrl;
    private GameObject AmmoPack;
    private Transform packParent;
    private Vector3 packLocalPos;

    public enum Lane { Left, Right};
    public Lane lane;

    Animator animator;

    private bool hasAttacked = false;

    private SkinnedMeshRenderer[] renderers;
    private Material[] originalMaterials;

    private AudioManager audioManager;

    

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        AmmoPack=transform.Find("AmmoPack").gameObject;
        packParent = AmmoPack.transform.parent;
        packLocalPos = AmmoPack.transform.localPosition;

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        originalMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterial;
        }

        }
    public void Initialize(Transform target)
    {
        player = target;
        playerCtrl = player.GetComponent<PlayerController>();
        currentHealth = health;

        hasAttacked = false;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }


        if (AmmoPack != null)
        {
            AmmoPack.SetActive(false);
            AmmoPack.SetActive(UnityEngine.Random.value<0.3f);
        }
        gameObject.SetActive(true);
    }
    private void Update()
    {
        
        if (movePack && AmmoPack != null)
        {
            AmmoPack.transform.position = Vector3.MoveTowards(AmmoPack.transform.position,player.position,packSpeed * Time.deltaTime);
            if (Vector3.Distance(AmmoPack.transform.position, player.position) < 0.5f)
            {
                playerCtrl.AmmoReload(5);
                AmmoPack.transform.SetParent(packParent);
                AmmoPack.transform.localPosition = packLocalPos;
                AmmoPack.SetActive(false);
                movePack = false;

                Die();
                
            }
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        transform.LookAt(player);
        

        if (distance > 1.5f)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            
            if (!hasAttacked)
            {
                hasAttacked = true;
                Attack();
            }
        }
    }
    private void Attack()
    {
        if (playerCtrl) playerCtrl.TakeDamage(damage);
        animator.SetTrigger("IsAttack");
        
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        deathParticle.Play();

        HitEffect();
        if (currentHealth <= 0) Die();
    }
    private void Die()
    {
        if (AmmoPack != null && AmmoPack.activeSelf)
        {
            AmmoPack.transform.SetParent(null);
            AmmoPack.SetActive(true);
            movePack=true;
            return;
             
        }
        ScoringSystem.Instance?.AddKillPoints();
        
        animator.SetTrigger("IsDead");
        if(audioManager != null)
        {
            audioManager.PlayZombieDead();
        }
        Invoke(nameof(DisableEnemy), 2f);
    }
    void DisableEnemy()
    {
        OnEnemyDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
    public void HitEffect()
    {
        StartCoroutine(HitFlash());
    }

    IEnumerator HitFlash()
    {
        
        foreach (var r in renderers)
        {
            r.material.SetColor("_BaseColor", Color.red);
        }

        yield return new WaitForSeconds(0.5f);

        
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }
}