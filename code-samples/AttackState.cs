using UnityEngine;

public class AttackState : BaseState
{
    private float attackCooldown = 1f;
    private float lastAttackTime;
    private Transform player;

    public AttackState(Enemy enemy, StateMachine stateMachine, Transform player) : base(enemy, stateMachine)
    {
        this.player = player;
    }

    public override void Enter()
    {
        enemy.Agent.speed = enemy.chaseSpeed; // Cambia la velocidad a persecución
        enemy.Agent.isStopped = false;
        enemy.SetAnimationState("isWalking", false); // Desactiva caminar
        enemy.SetRunning(true); // Activa la animación de correr
    }

    public override void Perform()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);

        if (distanceToPlayer <= enemy.AttackRange)
        {
            enemy.Agent.isStopped = true;
            enemy.SetRunning(false);
            enemy.SetAnimationState("isWalking", false);
            enemy.SetPunching(true); // Activa la animación de ataque

            if (Time.time - lastAttackTime > attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else if (distanceToPlayer <= enemy.visionRange)
        {
            enemy.Agent.isStopped = false;
            enemy.SetPunching(false); // Deja de atacar
            enemy.SetRunning(true);   // Sigue corriendo
            enemy.Agent.SetDestination(player.position);
        }
        else
        {
            enemy.Agent.isStopped = false;
            enemy.SetPunching(false);
            enemy.SetRunning(false);
            enemy.SetAnimationState("isWalking", true); // Vuelve a patrullar
            stateMachine.ChangeState(new PatrolState(enemy, stateMachine));
        }
    }

    public override void Exit()
    {
        enemy.Agent.isStopped = false; // Asegúrate de que el enemigo pueda moverse
        enemy.SetAnimationState("isPunching", false); // Detener animación de ataque
    }

    private void AttackPlayer()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(5); // Reduce 5 puntos de salud al jugador
            Debug.Log("Jugador atacado por enemigo.");
        }
    }


}