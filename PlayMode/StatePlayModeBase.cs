using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position : FSM - State의 집합체 StatePlayModeBase 속한 State들을 통제 및 관리한다
/// ex) StatePlayMode_Start / StatePlayMode_End / StatePlayMode_Pause
/// </summary>
abstract public class StatePlayModeBase : StateTemplate<PlayModeBase>
{
    protected abstract void OnInitStatePlayModeBase();

    protected override void OnStateEnter(StateBase pPrevState)
    {
        Debug.Log("======= Call_StatePlayModeBase OnStateEnter");
    }

    protected override void OnStateLeave(StateBase pNextState)
    {
        Debug.Log("======= StatePlayModeBase OnStateLeave");
    }

    protected override void OnStateInterrupted(StateBase pStateInterrupt)
    {
        Debug.Log("======= Call_StatePlayModeBase Interrupted");
    }

    protected override void OnStateInterruptResume()
    {
        Debug.Log("======= Call_StatePlayModeBase InterruptedResume");
    }
}
