using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStateInsertType
{
    NONE = -1,

    STATE_CHANGE,           // 기존 스테이트가 즉각 종료된다.
    STATE_INTERRUPT,        // 기존 스테이트는 인터럽트 스택에 보존되고 요청한 스테이트가 활성화 된다.
    STATE_WAITING,          // 기존 스테이트가 끌날동안 대기한다.
}

/// <summary>
/// Position : FSM - 유한 상태 머신
/// </summary>
/// <typeparam name="TEMPLATE"></typeparam>
public class FiniteStateMachinBase<TEMPLATE> : MonoBehaviour where TEMPLATE : class, new()
{
    protected TEMPLATE m_pOwner = null;
    protected StateBase m_pStateCurrent = null;
    protected IEnumerator m_pCoroutine = null;

    protected MultiSortedDictionary<string, StateBase> m_mapStateInstance = new MultiSortedDictionary<string, StateBase>(); // State Instance, 메모리 관리를 위한 슬롯
    protected Stack<StateBase> m_StackInterrupt = new Stack<StateBase>();
    protected Queue<StateBase> m_QueueWaiting = new Queue<StateBase>();
    protected bool m_bStartCoroutine = false;

    private void LateUpdate()
    {
        if(m_bStartCoroutine)
        {
            m_bStartCoroutine = false;

            if (m_pStateCurrent != null)
                StartCoroutine(m_pStateCurrent.OnStateUpdate()); 
        }
    }

    public void InitFSM(MonoBehaviour pOwner)   // Stat Init Excute
    {
        m_pOwner = pOwner as TEMPLATE;
        OnInitStateController();
    }

    public void DoClearAllState()   // All State Stop and Clear
    { 
        m_bStartCoroutine = false;
        m_pStateCurrent = null;
        m_pCoroutine = null;
        m_StackInterrupt.Clear();
        m_QueueWaiting.Clear();

        StopAllCoroutines();
    }
    
    public void EventStateStart(StateBase pState, EStateInsertType eStateInsertType)
    {
        ProcCombStateInsert(pState, eStateInsertType);
    }

    /// <summary>
    /// 현재 실행중인 m_pStateCurrent가 경과후 종료시킬때 사용
    /// </summary>
    /// <param name="pState"></param>
    public void EventStateEnd(StateBase pState)
    {
        if (m_pStateCurrent == pState)
            EventCurrentStateEnd_Forced();
    }

    public void EventCurrentStateEnd_Forced()
    {
        Proc_StateEnd();
        OnFSMControllerStateEnd();
    }

    public StateBase GetCurrentBase()
    {
        return m_pStateCurrent;
    }

    public bool IsState<TYPE>() where TYPE : StateTemplate<TEMPLATE>
    {
        bool bResult = false;

        if(m_pStateCurrent != null)
        {
            TYPE pState = m_pStateCurrent as TYPE;

            if (pState != null)
                bResult = true;
        }

        return bResult;
    }

    public bool ContainFromWaitState<STATE>() where STATE : StateBase
    {
        bool bContain = false;

        if(m_QueueWaiting.Count != 0)
        {
            StateBase[] arrQueueWaitingState = m_QueueWaiting.ToArray();

            for(int i = 0; i < arrQueueWaitingState.Length; ++i)
            {
                if(arrQueueWaitingState[i] is STATE)
                {
                    bContain = true;
                    break;
                }
            }
        }

        return bContain;
    }
    
    //============================================================================================================================================================================================================
    void ProcCombStateInsert(StateBase pState, EStateInsertType eStateInsertType)
    {
        switch(eStateInsertType)
        {
            case EStateInsertType.STATE_CHANGE:
                Proc_State_Change(pState);
                break;
            case EStateInsertType.STATE_INTERRUPT:
                Proc_State_Interrupt(pState);
                break;
            case EStateInsertType.STATE_WAITING:
                Proc_State_Waiting(pState);
                break;
        }
    }

    /// <summary>
    /// 현재 State 에서 추가된 State로 교체 
    /// 현재 사용중인 State를 모두 정지 후 교체 * 이전 State는 pPrevState = m_pStateCurrent Stack영역에 저장  현재 들어오는 State Proc_SetCurrentState(pState) m_pStateCurrent룰 현재 State로 교체
    /// </summary>
    /// <param name="pState"></param>
    void Proc_State_Change(StateBase pState)
    {
        if(m_pCoroutine != null)
        {
            StopCoroutine(m_pCoroutine);
            m_pCoroutine = null;
        }

        StopAllCoroutines();

        StateBase pPrevState = m_pStateCurrent;
        Proc_SetCurrentState(pState);

        if (pPrevState)
            pPrevState.EventStateLeave(m_pStateCurrent);        // 전 State에서 떠나면서 현재 들어올려는 State 값을 보내준다.

        if (m_pStateCurrent)
            m_pStateCurrent.EventStateEnter(pPrevState);        // 현 State에서 시작하면서 전에 들어왔던 State 값을 보내준다.
    }

    /// <summary>
    /// 주로 Pause / Event 등 현재 상태에서 난입을 위해 사용 
    /// </summary>
    /// <param name="pState"></param>
    void Proc_State_Interrupt(StateBase pState)
    {
        if(m_pCoroutine != null)
        {
            StopCoroutine(m_pCoroutine);
            m_pCoroutine = null;
        }

        StopAllCoroutines();

        StateBase pPrevState = m_pStateCurrent;
        Proc_SetCurrentState(pState);

        if(pPrevState)
        {
            pPrevState.EventStateInterrupted(m_pStateCurrent);
            m_StackInterrupt.Push(pState);
        }

        m_pStateCurrent.EventStateEnter(pPrevState);
    }

    /// <summary>
    /// 기존 State가 존재하지 않으면 현재 State를 Current로 돌린후 현 State에서 시작하면서 null을 보내준다. 기존 State가 존재하면 Queue 삽입후 기존 State가 끝날때 까지 대기한다.
    /// 현재 사용 중인 State가 존재시 EventStateNextStateWait 호출을 통해 대기
    /// </summary>
    /// <param name="pState"></param>
    void Proc_State_Waiting(StateBase pState)
    {
        if(m_pStateCurrent == null)
        {
            StopAllCoroutines();
            Proc_SetCurrentState(pState);
            m_pStateCurrent.EventStateEnter(null);
        }
        else
        {
            m_QueueWaiting.Enqueue(pState);
            m_pStateCurrent.EventStateNextStateWait(pState);
        }
    }

    //============================================================================================================================================================================================================

    void Proc_StateEnd()
    {
        if(m_StackInterrupt.Count > 0)  // Interrupt 우선
        {
            StateBase pStatePop = m_StackInterrupt.Pop();

            if (pStatePop)
            {
                StopAllCoroutines();
                StateBase pPrevState = m_pStateCurrent;
                Proc_SetCurrentState(pStatePop);

                if (pPrevState != null)
                    pPrevState.EventStateLeave(pStatePop);

                m_pStateCurrent.EventStateInterruptResume();
            }
        }
        else if(m_QueueWaiting.Count > 0)
        {
            StateBase pStateDequeue = m_QueueWaiting.Dequeue();

            if(pStateDequeue)
            {
                StopAllCoroutines();
                StateBase pPrevState = m_pStateCurrent;
                Proc_SetCurrentState(pStateDequeue);

                if (pPrevState != null)
                    pPrevState.EventStateLeave(pStateDequeue);

                m_pStateCurrent.EventStateEnter(pPrevState);
            }
        }
        else
        {
            StateBase pPrevState = m_pStateCurrent;
            Proc_SetCurrentState(null);
            StopAllCoroutines();

            if (pPrevState)
                pPrevState.EventStateLeave(null);
        }
    }

    //============================================================================================================================================================================================================


    void Proc_SetCurrentState(StateBase pState)
    {
        m_pStateCurrent = pState;

        if (pState != null)
            m_bStartCoroutine = true;
    }

    //============================================================================================================================================================================================================

    protected TYPE MakeStateInstance<TYPE>() where TYPE : StateTemplate<TEMPLATE>
    {
        TYPE pNewState = FindEmptyState<TYPE>();

        if(pNewState == null)
        {
            pNewState = ScriptableObject.CreateInstance(typeof(TYPE)) as TYPE;
            m_mapStateInstance[typeof(TYPE).ToString()].Add(pNewState);
        }

        if(m_pOwner == null)
        {
            Debug.Log("CFSMControllerBase has not initilize owenr. call InitStateControllor()");
        }

        pNewState.BuildState(m_pOwner as TEMPLATE, this);

        return pNewState;
    }

    private TYPE FindEmptyState<TYPE>() where TYPE : StateTemplate<TEMPLATE>
    {
        TYPE pFindState = null;
        string strStateName = typeof(TYPE).ToString();

        if(m_mapStateInstance.ContainKey(strStateName))
        {
            List<StateBase> pListState = m_mapStateInstance[strStateName];

            for(int i = 0; i < pListState.Count; ++i)
            {
                StateBase pState = pListState[i];

                if(pState.p_bActive != true)
                {
                    pFindState = pState as TYPE;
                    break;
                }
            }
        }

        return pFindState;
    }

    protected virtual void OnInitStateController() { }
    protected virtual void OnFSMControllerStateEnd() { }
}
