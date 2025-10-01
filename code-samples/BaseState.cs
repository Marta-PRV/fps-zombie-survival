using UnityEngine;

public abstract class BaseState
{
    protected Enemy enemy;
    protected StateMachine stateMachine;

    public BaseState(Enemy enemy, StateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}
