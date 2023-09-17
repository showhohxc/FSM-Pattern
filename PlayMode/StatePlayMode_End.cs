using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayMode_End : StatePlayModeBase
{
    protected override void OnInitStatePlayModeBase()
    {
        Debug.Log("======= StatePlayMode_End OnInitStatePlayModeBase");
    }

    protected override void OnStateEnter(StateBase pPrevState)
    {
        Debug.Log("======= Call_StatePlayMode_End OnStateEnter");
    }

    protected override void OnStateLeave(StateBase pNextState)
    {
        Debug.Log("======= Call_StatePlayMode_End OnStateLeave");
    }

    protected override void OnStateInterrupted(StateBase pStateInterrupt)
    {
        Debug.Log("======= Call_StatePlayMode_End OnStateInterrupted");

        DoStateSelfEnd();
    }

    protected override void OnStateInterruptResume()
    {
        Debug.Log("======= Call_StatePlayMode_End OnStateInterruptResume");
    }

    public void SetPlayMode_End_Info()
    {
        Debug.Log("======= Call_SetPlayMode_End_Info");
    }
}
