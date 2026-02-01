using System;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public event Action<Enemy> OnEnemyDeath;
    [SerializeField] private int health = 100;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage = 20;
    private int currentHealth;
    private Transform player;
    private PlayerController playerCtrl;
    public void Initialize(Transform target)
    {
        player = target;
        playerCtrl = player.GetComponent<PlayerController>();
        currentHealth = health;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, player.position) < 1.5f) { Attack(); }
    }
    private void Attack()
    {
        if (playerCtrl) playerCtrl.TakeDamage(damage);
        Die();
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }
    private void Die()
    {
        OnEnemyDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}