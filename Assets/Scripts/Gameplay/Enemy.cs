using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action<Enemy> OnEnemyDeath;

    [SerializeField]
    private int health = 100;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private float packSpeed = 16f;

    [SerializeField]
    private int ammoDropAmount = 5;

    [SerializeField]
    private float ammoDropChance = 0.3f;

    [SerializeField]
    private ParticleSystem deathParticle;
    private int currentHealth;
    private Transform player;
    private bool movePack = false;
    private bool isDead = false;
    private PlayerController playerCtrl;

    [SerializeField]
    private GameObject AmmoPack;
    private Transform packParent;
    private Vector3 packLocalPos;

    public enum Lane
    {
        Left,
        Right,
    };

    public Lane lane;

    Animator animator;

    [SerializeField]
    private float attackCooldown = 1f;
    private float attackTimer = 0f;

    private SkinnedMeshRenderer[] renderers;
    private Material[] originalMaterials;

    private AudioManager audioManager;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioManager = FindFirstObjectByType<AudioManager>();
        if (AmmoPack == null)
            AmmoPack = transform.Find("AmmoPack")?.gameObject;
        if (AmmoPack != null)
        {
            packParent = AmmoPack.transform.parent;
            packLocalPos = AmmoPack.transform.localPosition;
        }

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

        CancelInvoke(nameof(DisableEnemy));
        attackTimer = 0f;
        movePack = false;
        isDead = false;
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }

        if (AmmoPack != null)
        {
            AmmoPack.transform.SetParent(packParent);
            AmmoPack.transform.localPosition = packLocalPos;
            AmmoPack.SetActive(UnityEngine.Random.value < ammoDropChance);
        }
        gameObject.SetActive(true);

        if (animator != null)
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, UnityEngine.Random.Range(0f, 1f));
    }

    private void Update()
    {
        if (isDead) return;

        if (movePack && AmmoPack != null)
        {
            AmmoPack.transform.position = Vector3.MoveTowards(
                AmmoPack.transform.position,
                player.position,
                packSpeed * Time.deltaTime
            );
            if (Vector3.Distance(AmmoPack.transform.position, player.position) < 0.5f)
            {
                playerCtrl.AmmoReload(ammoDropAmount);
                AmmoPack.transform.SetParent(packParent);
                AmmoPack.transform.localPosition = packLocalPos;
                AmmoPack.SetActive(false);
                movePack = false;

                Die();
            }
            return;
        }

        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        transform.LookAt(player);

        attackTimer -= Time.deltaTime;

        if (distance > 1.5f)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
        else if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log($"[ENEMY] Attack → player health will drop by {damage}");
        if (playerCtrl)
            playerCtrl.TakeDamage(damage);
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetTrigger("IsAttack");
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (animator != null)
            animator.applyRootMotion = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (deathParticle != null)
            deathParticle.Play();

        HitEffect();
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (AmmoPack != null && AmmoPack.activeSelf)
        {
            AmmoPack.transform.SetParent(null);
            AmmoPack.SetActive(true);
            movePack = true;
            return;
        }

        isDead = true;
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        ScoringSystem.Instance?.AddKillPoints();

        if (animator != null)
            animator.SetTrigger("IsDead");
        if (audioManager != null)
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

