using UnityEngine;
using System.Collections.Generic;
using System;
using SillyGames.SGBase;
using UnityEngine.SceneManagement;

namespace SillyGames.SGBase
{

    /// <summary>
    /// monobehaviour class for game
    /// it just holds the Game class
    /// you may derrive this class to put your monobehaviour specific code
    /// all game play specific code should go in Game class
    /// </summary>
    /// <typeparam name="T">Any class derriving Game, this is to make sure it servs some basic functionality</typeparam>
    public class GameBehaviour<T> : MonoBehaviour where T: Game
    {
        [SerializeField]
        private T m_game = null;

        public T Game
        {
            get
            {
                return m_game;
            }
        }

        /// <summary>
        /// do not declare this function in derrived class
        /// keeping it protected so that you will get a warning when you declare one in your class
        /// </summary>
        protected void Awake()
        {
            m_game.Init(this.gameObject);
        }

        protected virtual void Update()
        {
            ///at this point we can have our custom delta time, so that we can control it independently
            m_game.GameUpdate(Time.deltaTime);
        }
    }
    

    /// <summary>
    /// base class for the game, its itself is an FSM  for the state type <typeparamref name="GameStateBase"/>
    /// derrive this class to put your game specific code
    /// this calss does not hold any list of states, but just manages transitions
    /// this class does provide a state transitioning by type, to enable this define 'ALLOW_TRANSITION_BY_TYPE'.
    /// </summary>
    /// <seealso cref="GameStateBase"/>
    /// <seealso cref="FSMEventbased<T>"/>
    [Serializable]
    public class Game : FSMEventbased<GameStateBase>
    {
        #region singleton impl and a contructor

        private static Game s_instance = null;
        public static Game Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                s_instance = value;
            }
        }

        #endregion 

        [SerializeField]
        private GameStateBase m_initialState = null;
        public GameStateBase InitialState
        {
            get { return m_initialState; }
            set { m_initialState = value; }
        }
        
        public GameObject GameObject { get; set; }
        internal void Init(GameObject a_parentObject)
        {
            GameObject = a_parentObject;    
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Was not expecting 2 or more instances of this type: " + this);
                return;
            }
            RegisterTransitionsByType();
        }

        

        void RegisterTransitionsByType()
        {
            var statesInScene = GameObject.FindObjectsOfType<GameStateBase>();
            foreach (var item in statesInScene)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Registering State: " + item);
                }
                if (!m_dicTransitions.ContainsKey(item.GetType()))
                {
                    m_dicTransitions.Add(item.GetType(), item);
                }
                else
                {
                    Debug.LogWarning("The state with the type is already registered, hence ignoring the state: " + item);
                }
            }
        }
        private Dictionary<Type, GameStateBase> m_dicTransitions = new Dictionary<Type, GameStateBase>();
        
        /// <summary>
        /// transitions to a class based on the type provided as a generic parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void TransitionTo<T>() where T: GameStateBase
        {
            GameStateBase state = null;
            if(m_dicTransitions.TryGetValue(typeof(T), out state))
            {
                TransitionTo(state);
            }
            else
            {
                Debug.LogWarning("No state is registered with type: " + typeof(T));
            }
        }

        public T GetRegisteredState<T>() where T:GameStateBase
        {
            GameStateBase state = null;
            m_dicTransitions.TryGetValue(typeof(T), out state);
            return state as T;
        }

        
    }
}