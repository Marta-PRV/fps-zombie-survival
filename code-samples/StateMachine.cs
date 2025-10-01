using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState ActiveState { get; private set; }

    public void Initialize(BaseState initialState)
    {
        ChangeState(initialState);
    }

    public void ChangeState(BaseState newState)
    {
        if (ActiveState != null)
        {
            ActiveState.Exit();
        }

        ActiveState = newState;

        if (ActiveState != null)
        {
            ActiveState.Enter();
        }
    }

    private void Update()
    {
        if (ActiveState != null)
        {
            ActiveState.Perform();
        }
    }
}