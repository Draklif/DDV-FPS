using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    private Transform target;
    public Animator anim;
    public NavMeshAgent agent;
    private Health health;

    [Header("Combat")]
    public float attackRange = 2.5f;
    public float attackCooldown = 1.2f;
    public int damage = 10;

    private bool canAttack = true;
    private bool isDead = false;

    void Start()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
        if (health == null) health = GetComponent<Health>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        health.OnHealthChanged += OnHit;
        health.OnDeath += OnDeath;
    }

    void Update()
    {
        if (isDead) return;
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            TryAttack();
        }

        UpdateAnimation(dist);
    }

    void ChasePlayer()
    {
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    void TryAttack()
    {
        if (!canAttack) return;

        StartCoroutine(AttackRoutine());
    }

    System.Collections.IEnumerator AttackRoutine()
    {
        canAttack = false;

        // detén el movimiento
        agent.isStopped = true;

        // animación de ataque
        anim.SetTrigger("Attack");

        // esperar a mitad del ataque para aplicar daño
        yield return new WaitForSeconds(0.8f);

        // aplicar daño si el jugador sigue cerca
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange + 0.2f)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // cooldown del ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnHit(float normalizedHealth)
    {
        if (isDead) return;

        anim.SetTrigger("Hit");
    }

    void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        // detener movimiento
        agent.isStopped = true;
        agent.enabled = false;

        // animación de muerte
        anim.SetTrigger("Death");

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        // destruir después de la animación
        Destroy(gameObject, info.length);
    }

    void UpdateAnimation(float dist)
    {
        if (isDead) return;

        bool isMoving = dist > attackRange && agent.velocity.magnitude > 0.1f;
        anim.SetBool("IsMoving", isMoving);
    }
}
