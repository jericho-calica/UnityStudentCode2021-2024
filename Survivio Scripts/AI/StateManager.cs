using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] State currentState;

    void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        //if currentState is null, ignore
        State nextState = currentState?.RunCurrentState();

        if(nextState != null)
        {
            SwitchToTheNextState(nextState);
        }
    }

    private void SwitchToTheNextState(State nextState)
    {
        currentState = nextState;
    }
}
