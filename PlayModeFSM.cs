using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeFSM : FiniteStateMachinBase<PlayModeBase>
{
    public void DoFSM_PlayMode_Start()
    {
        StatePlayMode_Start pState = MakeStateInstance<StatePlayMode_Start>();
        pState.SetPlayMode_Start_Info();
        EventStateStart(pState, EStateInsertType.STATE_WAITING);
    }

    public void DoFSM_PlayMode_End()
    {
        StatePlayMode_End pState = MakeStateInstance<StatePlayMode_End>();
        pState.SetPlayMode_End_Info();
        EventStateStart(pState, EStateInsertType.STATE_CHANGE);
    }

    public void DoFSM_PlayMode_Pause()
    {
        StatePlayMode_Pause pState = MakeStateInstance<StatePlayMode_Pause>();
        pState.SetPlayMode_Pause_Info();
        EventStateStart(pState, EStateInsertType.STATE_INTERRUPT);
    }
}
