using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position : FSM의 State를 정의하기 위한 Class ScriptableObject 상속
/// </summary>
public class StateBase : ScriptableObject
{
    protected bool m_bActive; public bool p_bActive { get { return m_bActive; } }

    public void EventStateEnter(StateBase pPreState)
    {
        m_bActive = true;
        OnStateEnter(pPreState);
    }

    public void EventStateLeave(StateBase pNextState)
    {
        m_bActive = false;
        OnStateLeave(pNextState);
    }

    public void EventStateNextStateWait(StateBase pStateWait)
    {
        OnStateNextStateWait(pStateWait);
    }

    public void EventStateInterrupted(StateBase pStateInterrupt)
    {
        OnStateInterrupted(pStateInterrupt);
    }

    public void EventStateInterruptResume()
    {
        OnStateInterruptResume();
    }

    //--------------------------------------------------------------------------
    #region StateEventHandle
    protected virtual void OnStateEnter(StateBase pPrevState) { }
    protected virtual void OnStateLeave(StateBase pNextState) { }
    protected virtual void OnStateNextStateWait(StateBase pStateWait) { }
    protected virtual void OnStateInterrupted(StateBase pStateInterrupt) { }
    protected virtual void OnStateInterruptResume() { }
    public virtual IEnumerator OnStateUpdate() { yield break; }
    #endregion
    //---------------------------------------------------------------------------

}
