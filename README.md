# FSM-Pattern Difinition
 Struct design for initial game design
 
![Class Diagram](https://github.com/showhohxc/FSM-Pattern/assets/98040028/0b3e50e8-6ffc-4b0c-b68e-1b61459b6e10)

## 전체적인 게임 흐름에 있어서 어떠한 기능 구현에 있어서 즉각적인 기능 구현은 프로그램 제작자에 한해서 알아볼수 있겠지만, 제 3자의 제작자는 난해하고 해당 기능을 찾는데 많은 시간이 걸릴수도 있다. 
## 그러한 생각으로 프로젝트 첫 제작시 이러한 FSM 구조로 기틀을 다지면 다중의 제작자가 협업 프로그램의 제작에 있어서 규칙이 정해져 있으므로 코드가 난잡해지지 않고 안정성 있는 코드를 제작하도록 용이하기위해 제작

### State Type 
FSM 주 State Type을 3가지의 상태로 정의 [Change / Interrupt / Waiting] 추후 State Type 추가 가능 ex) Back State / Immediately State

Change State

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

- 클래스에서 Change 타입의 State를 호출시 전에 실행중인 State는 즉각 EventStateLeave를 통해 중단하고 Change 타입의 State가 EventStateEnter를 호출

Interrupt State

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

- 클래스에서 Interrupt를 요청시 현재 실행중인 State는 EventStateInterrupted 함수를 호출 Interrupt를 요청 후 State는 Stack에 State를 보관후 EventStateEnter를 실행
ex) 보통 게임을 Pause 하거나 이벤트 돌입으로 인한 BGM 및 카메라등이 사용한다.

Waiting State

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

- Class에서 Waiting을 요청시 최근 사용한 State가 null 일시 즉각 EventStateEnter 호출하고 아니면 Queue에 State를 Enqueue 후 EventStateNextStateWait 호출

