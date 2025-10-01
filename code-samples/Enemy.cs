using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected  StateMachine stateMachine;
    protected  NavMeshAgent agent;
    protected int health = 3; // Salud inicial del enemigo

    [SerializeField] protected Path path;
    [SerializeField] public Transform player;
    [SerializeField] public float chaseSpeed = 2.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] public float visionRange = 15f;
    [SerializeField] private Vector3 respawnPosition;
    [SerializeField] private GameObject coinPrefab; // Prefab de la moneda
    [SerializeField] private int maxCoins = 3;     // Máximo de monedas a soltar

    public NavMeshAgent Agent => agent;
    public Transform Player => player;
    public float AttackRange => attackRange;
    public Path Path => path;

    private void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();

        if (path == null)
        {
            Debug.LogError("Path no está asignado en el inspector");
            return;
        }

        respawnPosition = transform.position; // Guarda la posición inicial
        stateMachine.Initialize(new PatrolState(this, stateMachine));
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionRange && !(stateMachine.ActiveState is AttackState))
        {
            stateMachine.ChangeState(new AttackState(this, stateMachine, player));
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        // Incrementa el contador global de zombies eliminados
        GameStats.ZombiesKilled++; 

        // Soltar monedas al morir
        int coinsToDrop = Random.Range(0, maxCoins + 1); // Genera un número aleatorio de monedas
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 0.5f; // Posición aleatoria cerca del enemigo
            spawnPosition.y = 0.5f; // Asegurar que esté en el mismo nivel
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity); // Crear moneda
        }

        // Desactiva el enemigo
        gameObject.SetActive(false); 

        // Llamar al respawn después de 10 segundos
        if (EnemyRespawner.Instance != null)
        {
            EnemyRespawner.Instance.RespawnEnemy(gameObject, respawnPosition, 5f);
        }
    }

    public void ResetHealth()
    {
        health = 3; // Restaura la salud inicial
    }

    public void SetRunning(bool isRunning)
    {
        SetAnimationState("isRunning", isRunning); // Activa o desactiva la animación de correr
    }

    public void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            SetAnimationState("isWalking", false); // Desactiva caminar
            SetAnimationState("isIdle", true);     // Activa Idle
        }
    }

    public void ResumeMovement()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            SetAnimationState("isWalking", true); // Activa la animación de caminar
            SetAnimationState("isIdle", false);  // Desactiva Idle
        }
    }

    public void SetAnimationState(string parameter, bool state)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool(parameter, state);
        }
    }

    public void SetPunching(bool isPunching)
    {
        SetAnimationState("isPunching", isPunching);
    }

}
