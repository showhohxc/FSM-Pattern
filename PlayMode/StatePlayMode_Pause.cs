using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayMode_Pause : StatePlayModeBase
{
    protected override void OnInitStatePlayModeBase()
    {
        Debug.Log("======= StatePlayMode_End OnInitStatePlayModeBase");
    }

    protected override void OnStateEnter(StateBase pPrevState)
    {
        Debug.Log("======= Call_StatePlayMode_Pause OnStateEnter");
    }

    protected override void OnStateLeave(StateBase pNextState)
    {
        Debug.Log("======= Call_StatePlayMode_Pause OnStateLeave");
    }

    public void SetPlayMode_Pause_Info()
    {
        Debug.Log("======= Call_SetPlayMode_Pause_Info");
    }

    protected override void OnStateInterruptResume()
    {
        base.OnStateInterruptResume();

        Debug.Log("======= Call_StatePlayMode_Pause Pause");
    }
}
