using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position : 여러개의 FSM을 관리 보관하기 위한 Class
/// ex) StatePlayModeBase / StateSoundModeBase / StateCameraModeBase / StateEventModeBase
/// </summary>
/// <typeparam name="TEMPLATE"></typeparam>
public class StateTemplate<TEMPLATE> : StateBase where TEMPLATE : class, new()
{
    protected TEMPLATE m_pOwner = null;
    protected FiniteStateMachinBase<TEMPLATE> m_pControllOwner = null;

    public void BuildState(TEMPLATE pOwner, FiniteStateMachinBase<TEMPLATE> pControllOwner)
    {
        m_pOwner = pOwner;
        m_pControllOwner = pControllOwner;

        OnStateInit();
    }

    public void DoStateSelfEnd()
    {
        m_pControllOwner.EventStateEnd(this);
    }

    protected virtual void OnStateInit() { }
}
