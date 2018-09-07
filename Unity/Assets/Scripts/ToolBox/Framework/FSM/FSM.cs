using System.Collections;


namespace SillyGames.SGBase
{
    
    /// <summary>
    /// generic class for the FSM, which takes state type as one of its generic argument
    /// keeps track of states and provide event when transition happen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSM<T>
    {
        //private T m_currentState = default(T);

        public T CurrentState
        {
            get;
            private set;
        }

        public T PreviousState
        {
            get;
            private set;
        }

        public void TransitionTo(T a_state)
        {
            //better to get rid of a debug at such a low level
            //if (Debug.isDebugBuild)
            //{
            //    Debug.Log("FSM:'"+ this + "' State change: [" + CurrentState + "] >>---------> [" + a_state + "]" );
            //}
            
            OnExit(CurrentState);
            PreviousState = CurrentState;
            CurrentState = a_state;
            OnEnter(CurrentState);
        }

        public void GameUpdate(float a_fDeltaTime)
        {
            OnUpdate(CurrentState, a_fDeltaTime);
        }

        protected virtual void OnEnter(T a_state) { }
        protected virtual void OnExit(T a_state) { }
        protected virtual void OnUpdate(T a_state, float a_fDeltaTime) { }
    }

    public interface IState
    {
        void OnEnter();
        void OnExit();

        void OnUpdate(float a_fDeltaTime);
    }

    /// <summary>
    ///Base class which manages OnEnte/Exit/Update functions on states
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSMEventbased<T> : FSM<T> where T : IState
    {
        protected override sealed void OnEnter(T a_state)
        {
            if (a_state != null)
            {
                a_state.OnEnter();
            }
        }

        protected override sealed void OnExit(T a_state)
        {
            if (a_state != null)
            {
                a_state.OnExit();
            }
        }

        protected override sealed void OnUpdate(T a_state, float a_fDeltaTime)
        {
            if (a_state != null)
            {
                a_state.OnUpdate(a_fDeltaTime);
            }
        }
    }
}