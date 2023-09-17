using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayMode_Start : StatePlayModeBase
{ 
    protected override void OnInitStatePlayModeBase()
    {
        Debug.Log("======= StatePlayMode_Start OnInitStatePlayModeBase");
    }

    protected override void OnStateEnter(StateBase pPrevState)
    {
        Debug.Log("======= StatePlayMode_Start OnStateEnter");
    }

    protected override void OnStateLeave(StateBase pNextState)
    {
        Debug.Log("======= StatePlayMode_Start OnStateLeave");
    }

    public void SetPlayMode_Start_Info()
    {
        Debug.Log("======= Call_SetPlayMode_Start_Info");
    }
}
