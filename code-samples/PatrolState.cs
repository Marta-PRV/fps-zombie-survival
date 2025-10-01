using UnityEngine;

public class PatrolState : BaseState
{
    private int waypointIndex = 0;
    private float waitTimer = 0f;

    public PatrolState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        waypointIndex = 0;
        enemy.SetAnimationState("isWalking", true); // Activa la animación de caminar
        MoveToNextWaypoint();
    }

    public override void Perform()
    {
        if (enemy.Agent.remainingDistance <= 0.2f && !enemy.Agent.pathPending)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer < 3f)
            {
                enemy.SetAnimationState("isWalking", false);
                enemy.SetAnimationState("isIdle", true); // Quieto mientras espera
            }
            else
            {
                MoveToNextWaypoint();
                waitTimer = 0f;
                enemy.SetAnimationState("isIdle", false);
                enemy.SetAnimationState("isWalking", true); // Caminar entre waypoints
            }
        }
        else
        {
            enemy.SetAnimationState("isIdle", false);
            enemy.SetAnimationState("isWalking", true);
        }
    }


    public override void Exit()
    {
        enemy.SetAnimationState("isWalking", false); // Desactiva la animación de caminar
    }

    private void MoveToNextWaypoint()
    {
        if (enemy.Path == null || enemy.Path.waypoints.Count == 0) return;

        waypointIndex = (waypointIndex + 1) % enemy.Path.waypoints.Count;
        enemy.Agent.SetDestination(enemy.Path.waypoints[waypointIndex].position);
    }
    
}